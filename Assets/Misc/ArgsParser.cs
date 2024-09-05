using UnityEngine;
using UnityEngine.Video;
using UnityEngine.Rendering.Universal;

public sealed class ArgsParser : MonoBehaviour
{
    [SerializeField] GameObject[] _optionalVfxList = null;

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
                GetComponent<VideoPlayer>().url = args[++i];
        }
    }
}
