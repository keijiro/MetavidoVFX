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

    public string ProjectionVectorProperty
      { get => (string)_projectionVectorProperty;
        set => _projectionVectorProperty = value; }

    public string InverseViewMatrixProperty
      { get => (string)_inverseViewMatrixProperty;
        set => _inverseViewMatrixProperty = value; }

    [VFXPropertyBinding("UnityEngine.Texture2D"), SerializeField]
    ExposedProperty _colorMapProperty = "ColorMap";

    [VFXPropertyBinding("UnityEngine.Texture2D"), SerializeField]
    ExposedProperty _depthMapProperty = "DepthMap";

    [VFXPropertyBinding("UnityEngine.Vector4"), SerializeField]
    ExposedProperty _projectionVectorProperty = "ProjectionVector";

    [VFXPropertyBinding("UnityEngine.Matrix4x4"), SerializeField]
    ExposedProperty _inverseViewMatrixProperty = "InverseViewMatrix";

    public override bool IsValid(VisualEffect component)
      => _decoder != null && _demux != null &&
         component.HasTexture(_colorMapProperty) &&
         component.HasTexture(_depthMapProperty) &&
         component.HasVector4(_projectionVectorProperty) &&
         component.HasMatrix4x4(_inverseViewMatrixProperty);

    public override void UpdateBinding(VisualEffect component)
    {
        // Do nothing if metadata is not ready.
        var meta = _decoder.Metadata;
        if (!meta.IsValid) return;

        // Projection parameters
        var pm = meta.ProjectionMatrix;
        var pv = new Vector4(pm[0, 2], pm[1, 2], pm[0, 0], pm[1, 1]);

        // Inverse view matrix
        var v2w = Matrix4x4.TRS(meta.CameraPosition,
                                meta.CameraRotation,
                                new Vector3(1, 1, -1));

        // Property update
        component.SetTexture(_colorMapProperty, _demux.ColorTexture);
        component.SetTexture(_depthMapProperty, _demux.DepthTexture);
        component.SetVector4(_projectionVectorProperty, pv);
        component.SetMatrix4x4(_inverseViewMatrixProperty, v2w);
    }

    public override string ToString()
      => $"Bibcam : {_colorMapProperty}, {_depthMapProperty}";
}

} // namespace Bibcam
