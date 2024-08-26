using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.Video;
using Metavido.Decoder;

public sealed class Monitor : MonoBehaviour
{
    [field:SerializeField] public MetadataDecoder Decoder { get; set; }
    [field:SerializeField] public VideoPlayer Source { get; set; }

    VisualElement RootUI => GetComponent<UIDocument>().rootVisualElement;

    RenderTexture _frame;

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

    void Start()
    {
        _frame = RenderTexture.GetTemporary(1920, 1080);
        RootUI.Q<Label>("url-label").text = "Source: " + Source.url;
        RootUI.Q("video-view").style.backgroundImage = Background.FromRenderTexture(_frame);
    }

    void OnDestroy()
      => RenderTexture.ReleaseTemporary(_frame);

    void Update()
    {
       RootUI.Q<Label>("metadata-label").text = GetMetadataString();
       if (Source.texture != null) Graphics.Blit(Source.texture, _frame);
    }
}
