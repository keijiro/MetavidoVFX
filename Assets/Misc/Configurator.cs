using UnityEngine;
using UnityEngine.Video;
using UnityEngine.Rendering.Universal;

public sealed class Configurator : MonoBehaviour
{
    [SerializeField] string _defaultSourceUrl = null;
    [SerializeField] string _testSourceFilePath = null;
    [SerializeField] VideoPlayer _videoPlayer = null;
    [SerializeField] GameObject[] _optionalVfxList = null;

    void ApplyLiteSettings()
    {
        Camera.main.GetComponent<UniversalAdditionalCameraData>().SetRenderer(1);
        foreach (var go in _optionalVfxList) go.SetActive(false);
    }

    string ResolveUrl(string url)
      => url.StartsWith("sa://") ?
           Application.streamingAssetsPath + url.Substring(4) : url;

    void Start()
    {
        if (Application.isMobilePlatform) ApplyLiteSettings();

        var args = System.Environment.GetCommandLineArgs();
        for (var i = 1; i < args.Length; i++)
        {
            if (args[i] == "--lite")
                ApplyLiteSettings();
            if (i < args.Length - 1 && args[i] == "--sourceURL")
                _videoPlayer.url = args[++i];
        }

#if UNITY_EDITOR
        if (System.IO.File.Exists(_testSourceFilePath))
            _videoPlayer.url = "file://" + _testSourceFilePath;
#endif

        if (string.IsNullOrEmpty(_videoPlayer.url))
            _videoPlayer.url = ResolveUrl(_defaultSourceUrl);
    }
}
