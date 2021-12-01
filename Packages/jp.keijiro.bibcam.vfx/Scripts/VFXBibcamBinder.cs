using UnityEngine;
using UnityEngine.VFX;
using UnityEngine.VFX.Utility;
using Bibcam.Decoder;

namespace Bibcam {

[AddComponentMenu("VFX/Property Binders/Bibcam Binder")]
[VFXBinder("Bibcam")]
class VFXBibcamBinder : VFXBinderBase
{
    [SerializeField] BibcamMetadataDecoder _decoder = null;
    [SerializeField] BibcamTextureDemuxer _demux = null;

    public string ColorMapProperty
      { get => (string)_colorMapProperty;
        set => _colorMapProperty = value; }

    public string DepthMapProperty
      { get => (string)_depthMapProperty;
        set => _depthMapProperty = value; }

    public string RayParamsProperty
      { get => (string)_rayParamsProperty;
        set => _rayParamsProperty = value; }

    public string InverseViewProperty
      { get => (string)_inverseViewProperty;
        set => _inverseViewProperty = value; }

    public string DepthRangeProperty
      { get => (string)_depthRangeProperty;
        set => _depthRangeProperty = value; }

    [VFXPropertyBinding("UnityEngine.Texture2D"), SerializeField]
    ExposedProperty _colorMapProperty = "ColorMap";

    [VFXPropertyBinding("UnityEngine.Texture2D"), SerializeField]
    ExposedProperty _depthMapProperty = "DepthMap";

    [VFXPropertyBinding("UnityEngine.Vector4"), SerializeField]
    ExposedProperty _rayParamsProperty = "RayParams";

    [VFXPropertyBinding("UnityEngine.Matrix4x4"), SerializeField]
    ExposedProperty _inverseViewProperty = "InverseView";

    [VFXPropertyBinding("UnityEngine.Vector2"), SerializeField]
    ExposedProperty _depthRangeProperty = "DepthRange";

    public override bool IsValid(VisualEffect component)
      => _decoder != null && _demux != null &&
         component.HasTexture(_colorMapProperty) &&
         component.HasTexture(_depthMapProperty) &&
         component.HasVector4(_rayParamsProperty) &&
         component.HasMatrix4x4(_inverseViewProperty) &&
         component.HasVector2(_depthRangeProperty);

    public override void UpdateBinding(VisualEffect component)
    {
        // Do nothing if metadata is not ready.
        var meta = _decoder.Metadata;
        if (!meta.IsValid) return;

        // Camera parameters
        var ray = BibcamRenderUtils.RayParams(meta);
        var iview = BibcamRenderUtils.InverseView(meta);

        // Property update
        component.SetTexture(_colorMapProperty, _demux.ColorTexture);
        component.SetTexture(_depthMapProperty, _demux.DepthTexture);
        component.SetVector4(_rayParamsProperty, ray);
        component.SetMatrix4x4(_inverseViewProperty, iview);
        component.SetVector2(_depthRangeProperty, meta.DepthRange);
    }

    public override string ToString()
      => $"Bibcam : {_colorMapProperty}, {_depthMapProperty}";
}

} // namespace Bibcam
