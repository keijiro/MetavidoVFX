using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;
using Bibcam.Common;
using Bibcam.Decoder;

namespace Bibcam {

[System.Serializable]
sealed class BibcamBackgroundPass : CustomPass
{
    #region Scene object references

    [SerializeField] BibcamMetadataDecoder _decoder = null;
    [SerializeField] BibcamTextureDemuxer _demux = null;

    #endregion

    #region Editable attributes

    [SerializeField] Color _fillColor = Color.white;
    [SerializeField] Shader _shader = null;

    #endregion

    #region Private objects

    Material _material;

    #endregion

    #region CustomPass overrides

    protected override void Execute(CustomPassContext context)
    {
        if (_decoder == null || _demux == null || _shader == null) return;

        // Run it only when the textures are ready.
        if (_demux.ColorTexture == null) return;

        // Shader lazy setup
        if (_material == null)
            _material = CoreUtils.CreateEngineMaterial(_shader);

        // Camera parameters
        var meta = _decoder.Metadata;
        var ray = BibcamRenderUtils.RayParams(meta);
        var iview = BibcamRenderUtils.InverseView(meta);

        // Material property update
        _material.SetVector(ShaderID.RayParams, ray);
        _material.SetMatrix(ShaderID.InverseView, iview);
        _material.SetVector(ShaderID.DepthRange, meta.DepthRange);
        _material.SetTexture(ShaderID.ColorTexture, _demux.ColorTexture);
        _material.SetTexture(ShaderID.DepthTexture, _demux.DepthTexture);
        _material.SetColor("_FillColor", _fillColor);

        // Fullscreen quad drawcall
        CoreUtils.DrawFullScreen(context.cmd, _material);
    }

    protected override void Cleanup()
      => CoreUtils.Destroy(_material);

    #endregion
}

} // namespace Bibcam
