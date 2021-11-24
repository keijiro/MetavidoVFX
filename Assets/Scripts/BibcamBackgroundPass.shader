Shader "Hidden/Bibcam/BibcamBackgroundPass"
{
    HLSLINCLUDE

#include "Packages/com.unity.render-pipelines.high-definition/Runtime/RenderPipeline/RenderPass/CustomPass/CustomPassCommon.hlsl"
#include "Packages/jp.keijiro.bibcam/Decoder/Shaders/Utils.hlsl"

sampler2D _ColorTexture;
sampler2D _DepthTexture;

float4 _RayParams;
float4x4 _InverseView;
float2 _DepthRange;

float4 _FillColor;

float4 FullScreenPass(Varyings varyings) : SV_Target
{
    // Calculate the UV coordinates from varyings
    float2 uv = (varyings.positionCS.xy + float2(0.5, 0.5)) * _ScreenSize.zw;

    // Color/depth samples
    float4 color = tex2D(_ColorTexture, uv);
    float depth = tex2D(_DepthTexture, uv).x;

    // Dither pattern
    const float4x4 bayer =
      float4x4(0, 8, 2, 10, 12, 4, 14, 6, 3, 11, 1, 9, 15, 7, 13, 5) / 16;
    uint2 cs = varyings.positionCS / 2;
    float dither = bayer[cs.y & 3][cs.x & 3] - 0.5;

    // Image effects
    float l = 1 - Luminance(FastLinearToSRGB(color.rgb));
    l *= saturate(depth - _DepthRange.y + 1);
    l += dither;
    float3 rgb = (l > 0.5) * _FillColor.rgb;

    // Output
    return float4(rgb, 1);
}

    ENDHLSL

    SubShader
    {
        Pass
        {
            Cull Off ZWrite Off ZTest LEqual
            HLSLPROGRAM
            #pragma vertex Vert
            #pragma fragment FullScreenPass
            ENDHLSL
        }
    }
    Fallback Off
}
