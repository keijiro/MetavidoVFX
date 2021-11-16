using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;
using Bibcam.Decoder;

namespace Bibcam {

[System.Serializable]
sealed class BibcamBackgroundPass : CustomPass
{
    #region Editable attributes

    [SerializeField] BibcamTextureDemuxer _demux = null;
    [SerializeField, ColorUsage(false)] Color _fillTint = Color.white;
    [SerializeField] Color _gridColor = Color.white;
    [SerializeField] Color _stencilColor = Color.red;
    [SerializeField] float _depthOffset = 0;
    [SerializeField] Shader _shader = null;

    #endregion

    #region Private objects

    Material _material;

    #endregion

    #region CustomPass overrides

    protected override void Execute(CustomPassContext context)
    {
        if (_demux == null || _shader == null) return;

        // Run it only when the textures are ready.
        if (_demux.ColorTexture == null) return;

        // Shader lazy setup
        if (_material == null)
            _material = CoreUtils.CreateEngineMaterial(_shader);

        // Projection parameters
        var camera = context.hdCamera.camera;
        var pm = camera.projectionMatrix;
        var pv = new Vector4(pm[0, 2], pm[1, 2], pm[0, 0], pm[1, 1]);

        // Inverse view matrix
        var v2w = Matrix4x4.TRS(camera.transform.position,
                                camera.transform.rotation,
                                new Vector3(1, 1, -1));

        // Material property update
        _material.SetFloat(ShaderID.DepthOffset, _depthOffset);
        _material.SetColor(ShaderID.TintColor, _fillTint);
        _material.SetColor(ShaderID.GridColor, _gridColor);
        _material.SetColor(ShaderID.StencilColor, _stencilColor);
        _material.SetVector(ShaderID.ProjectionVector, pv);
        _material.SetMatrix(ShaderID.InverseViewMatrix, v2w);
        _material.SetTexture(ShaderID.ColorTexture, _demux.ColorTexture);
        _material.SetTexture(ShaderID.DepthTexture, _demux.DepthTexture);

        // Fullscreen quad drawcall
        CoreUtils.DrawFullScreen(context.cmd, _material);
    }

    protected override void Cleanup()
      => CoreUtils.Destroy(_material);

    #endregion
}

} // namespace Bibcam
