using UnityEngine;
using UnityEngine.Video;
using Directory = System.IO.Directory;

namespace Bibcam {

public sealed class VideoSelector : MonoBehaviour
{
    void ScanDirectory(string dirPath)
    {
        if (!Directory.Exists(dirPath)) return;
        var files = Directory.GetFiles(dirPath, "*.mp4");
        if (files.Length > 0) GetComponent<VideoPlayer>().url = files[0];
    }

    void Start()
    {
        ScanDirectory(Application.streamingAssetsPath);
        ScanDirectory(".");
    }
}

} // namespace Bibcam
