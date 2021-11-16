using UnityEngine;

namespace Bibcam {

static class ShaderID
{
    public static readonly int ColorTexture = Shader.PropertyToID("_ColorTexture");
    public static readonly int DepthOffset = Shader.PropertyToID("_DepthOffset");
    public static readonly int DepthTexture = Shader.PropertyToID("_DepthTexture");
    public static readonly int GridColor = Shader.PropertyToID("_GridColor");
    public static readonly int InverseView = Shader.PropertyToID("_InverseView");
    public static readonly int RayParams = Shader.PropertyToID("_RayParams");
    public static readonly int StencilColor = Shader.PropertyToID("_StencilColor");
    public static readonly int TintColor = Shader.PropertyToID("_TintColor");
}

} // namespace Bibcam
