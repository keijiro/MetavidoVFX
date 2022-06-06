Shader "Hidden/Bibcam/HDRP/Background/Poster"
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

float4 FullScreenPass(Varyings varyings) : SV_Target
{
    // Calculate the UV coordinates from varyings
    float2 uv0 = (varyings.positionCS.xy + 0.5) * _ScreenSize.zw;

    float2 uv1 = uv0 + _ScreenSize.zw;
    float2 uv2 = float2(uv1.x, uv0.y);
    float2 uv3 = float2(uv0.x, uv1.y);

    // Color/depth samples
    float4 color = tex2D(_ColorTexture, uv0);
    float depth = tex2D(_DepthTexture, uv0).x;

    float3 c0 = color.rgb;
    float3 c1 = tex2D(_ColorTexture, uv1).rgb;
    float3 c2 = tex2D(_ColorTexture, uv2).rgb;
    float3 c3 = tex2D(_ColorTexture, uv3).rgb;

    // Roberts cross operator
    float3 g1 = c1 - c0.rgb;
    float3 g2 = c3 - c2;
    float g = saturate((sqrt(dot(g1, g1) + dot(g2, g2)) - 0.02) * 2);

    // Image effects
    float l = g;//Luminance(FastLinearToSRGB(color.rgb));
    l *= saturate(depth - _DepthRange.y + 1.5);
    float3 rgb = FastSRGBToLinear(l) * _Tint.rgb;

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
