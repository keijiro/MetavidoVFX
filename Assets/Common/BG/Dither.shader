Shader "Hidden/Bibcam/HDRP/Background/Dither"
{
    HLSLINCLUDE

#include "Packages/com.unity.render-pipelines.high-definition/Runtime/RenderPipeline/RenderPass/CustomPass/CustomPassCommon.hlsl"
#include "Packages/jp.keijiro.bibcam/Decoder/Shaders/Utils.hlsl"

sampler2D _ColorTexture;
sampler2D _DepthTexture;

float4 _RayParams;
float4x4 _InverseView;
float2 _DepthRange;

float4 _Tint;

float Dither(uint2 cs)
{
    const float4x4 bayer = float4x4( 0,  8,  2, 10,
                                    12,  4, 14,  6,
                                     3, 11,  1,  9,
                                    15,  7, 13,  5);
    return (bayer[cs.y & 3][cs.x & 3] + 0.5) / 16 - 0.5;
}

float4 FullScreenPass(Varyings varyings) : SV_Target
{
    // Calculate the UV coordinates from varyings
    float2 uv = (varyings.positionCS.xy + 0.5) * _ScreenSize.zw;

    // Color/depth samples
    float4 color = tex2D(_ColorTexture, uv);
    float depth = tex2D(_DepthTexture, uv).x;

    // Image effects
    float l = 1 - Luminance(FastLinearToSRGB(color.rgb));
    l *= saturate(depth - _DepthRange.y + 1.5);
    l += Dither(varyings.positionCS.xy / 2);
    float3 rgb = (l > 0.5) * _Tint.rgb;

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
