using UnityEngine;
using UnityEngine.Video;
using UnityEngine.Rendering.Universal;

public sealed class ArgsParser : MonoBehaviour
{
    void Start()
    {
        var args = System.Environment.GetCommandLineArgs();
        for (var i = 1; i < args.Length; i++)
        {
            if (args[i] == "--lite")
                Camera.main.GetComponent<UniversalAdditionalCameraData>().SetRenderer(1);
            if (i < args.Length - 1 && args[i] == "--sourceURL")
                GetComponent<VideoPlayer>().url = args[++i];
        }
    }
}
