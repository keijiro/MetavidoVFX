Shader "Hidden/Bibcam/BibcamBackgroundPass"
{
    HLSLINCLUDE

#include "Packages/com.unity.render-pipelines.high-definition/Runtime/RenderPipeline/RenderPass/CustomPass/CustomPassCommon.hlsl"
#include "Packages/jp.keijiro.bibcam/Decoder/Shaders/Utils.hlsl"

sampler2D _ColorTexture;
sampler2D _DepthTexture;
float4 _RayParams;
float4x4 _InverseView;
float _DepthOffset;
float3 _TintColor;
float4 _GridColor;
float4 _StencilColor;

// 3-axis Gridline
float Gridline(float3 p)
{
    float3 b = 1 - saturate(0.5 * min(frac(1 - p), frac(p)) / fwidth(p));
    return max(max(b.x, b.y), b.z);
}

void FullScreenPass(Varyings varyings,
                    out float4 outColor : SV_Target,
                    out float outDepth : SV_Depth)
{
    // Calculate the UV coordinates from varyings
    float2 uv = (varyings.positionCS.xy + float2(0.5, 0.5)) * _ScreenSize.zw;

    // Color/depth samples
    float4 c = tex2D(_ColorTexture, uv);
    float d = tex2D(_DepthTexture, uv).x;

    // Inverse projection
    float3 p = DistanceToWorldPosition(uv, d, _RayParams, _InverseView);

    // Coloring
    c.rgb *= _TintColor;
    c.rgb = lerp(c.rgb, _GridColor.rgb, Gridline(p * 10) * _GridColor.a);
    c.rgb = lerp(c.rgb, _StencilColor.rgb, c.a * _StencilColor.a);

    // Output
    outColor = c;
    outDepth = DistanceToDepth(d) + _DepthOffset;
}

    ENDHLSL

    SubShader
    {
        Pass
        {
            Cull Off ZWrite On ZTest LEqual
            HLSLPROGRAM
            #pragma vertex Vert
            #pragma fragment FullScreenPass
            ENDHLSL
        }
    }
    Fallback Off
}
