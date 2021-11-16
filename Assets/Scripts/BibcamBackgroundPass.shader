Shader "Hidden/Bibcam/BibcamBackgroundPass"
{
    HLSLINCLUDE

#include "Packages/com.unity.render-pipelines.high-definition/Runtime/RenderPipeline/RenderPass/CustomPass/CustomPassCommon.hlsl"

sampler2D _ColorTexture;
sampler2D _DepthTexture;
float4 _ProjectionVector;
float4x4 _InverseViewMatrix;
float _DepthOffset;
float3 _TintColor;
float4 _GridColor;
float4 _StencilColor;

// Linear distance to Z depth
float DistanceToDepth(float d)
{
    return d < _ProjectionParams.y ? 0 :
      (0.5 / _ZBufferParams.z * (1 / d - _ZBufferParams.w));
}

// Inversion projection into the world space
float3 DistanceToWorldPosition(float2 uv, float d)
{
    float3 p = float3((uv - 0.5) * 2, -1);
    p.xy += _ProjectionVector.xy;
    p.xy /= _ProjectionVector.zw;
    return mul(_InverseViewMatrix, float4(p * d, 1)).xyz;
}

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
    float2 uv =
      (varyings.positionCS.xy + float2(0.5, 0.5)) * _ScreenSize.zw;

    // Color/depth samples
    float4 c = tex2D(_ColorTexture, uv);
    float d = tex2D(_DepthTexture, uv).x;

    // Inverse projection
    float3 p = DistanceToWorldPosition(uv, d);

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
