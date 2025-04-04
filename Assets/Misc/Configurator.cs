using UnityEngine;
using UnityEngine.Video;
using UnityEngine.Rendering.Universal;

public sealed class Configurator : MonoBehaviour
{
    [SerializeField] VideoPlayer _videoPlayer = null;
    [SerializeField] GameObject[] _optionalVfxList = null;
    [SerializeField] string _testSource = "Test.mp4";

    void ApplyLiteSettings()
    {
        Camera.main.GetComponent<UniversalAdditionalCameraData>().SetRenderer(1);
        foreach (var go in _optionalVfxList) go.SetActive(false);
    }

    void Start()
    {
        var args = System.Environment.GetCommandLineArgs();
        for (var i = 1; i < args.Length; i++)
        {
            if (args[i] == "--lite")
                ApplyLiteSettings();
            if (i < args.Length - 1 && args[i] == "--sourceURL")
                _videoPlayer.url = args[++i];
        }

#if UNITY_EDITOR
        if (System.IO.File.Exists(_testSource))
            _videoPlayer.url = "file://" + _testSource;
#endif
    }
}
