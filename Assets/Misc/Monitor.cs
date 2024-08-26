using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.Video;
using Metavido.Decoder;

public sealed class Monitor : MonoBehaviour
{
    [field:SerializeField] public MetadataDecoder Decoder { get; set; }
    [field:SerializeField] public VideoPlayer Source { get; set; }

    VisualElement RootUI => GetComponent<UIDocument>().rootVisualElement;

    string GetMetadataString()
    {
        var data = Decoder.Metadata;
        if (!data.IsValid) return "Loading...";
        return $"Position: {data.CameraPosition}\n" +
               $"Rotation: {data.CameraRotation.eulerAngles}\n" +
               $"Center:   {data.CenterShift}\n" +
               $"FoV:      {data.FieldOfView * Mathf.Rad2Deg:F2}\n" +
               $"Range:    {data.DepthRange}";
    }

    async Awaitable<Background> GetSourceImage()
    {
        while (true)
        {
            var rt = Source.texture as RenderTexture;
            if (rt != null) return Background.FromRenderTexture(rt);
            await Awaitable.NextFrameAsync();
        }
    }

    async void Start()
    {
        RootUI.Q<Label>("url-label").text = "Source: " + Source.url;
        RootUI.Q("video-view").style.backgroundImage = await GetSourceImage();
    }

    void Update()
      => RootUI.Q<Label>("metadata-label").text = GetMetadataString();
}
