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
float _DepthOffset;

float4 _Tint;

void Fragment(Varyings varyings,
              out float4 outColor : SV_Target,
              out float outDepth : SV_Depth)
{
    // Calculate the UV coordinates from varyings
    float2 uv0 = (varyings.positionCS.xy + 0.5) * _ScreenSize.zw;

    // Displaced UVs for edge detection
    float2 uv1 = uv0 + _ScreenSize.zw;
    float2 uv2 = float2(uv1.x, uv0.y);
    float2 uv3 = float2(uv0.x, uv1.y);

    // Color/depth samples
    float3 c0 = tex2D(_ColorTexture, uv0).rgb;
    float3 c1 = tex2D(_ColorTexture, uv1).rgb;
    float3 c2 = tex2D(_ColorTexture, uv2).rgb;
    float3 c3 = tex2D(_ColorTexture, uv3).rgb;
    float  d0 = tex2D(_DepthTexture, uv0).x;

    // Edge detection (Roberts cross operator)
    float3 g1 = c1 - c0;
    float3 g2 = c3 - c2;
    float g = sqrt(dot(g1, g1) + dot(g2, g2));

    // Contrast enhancement
    g = saturate((g - 0.02) * 2);

    // Depth fading
    float fade = saturate(d0 - _DepthRange.y + 1.5);

    // Output
    outColor = float4(_Tint.rgb * FastSRGBToLinear(g * fade), 1);
    outDepth = DistanceToDepth(lerp(_DepthRange.y, d0, fade) + _DepthOffset);
}

    ENDHLSL

    SubShader
    {
        Pass
        {
            Cull Off ZWrite On ZTest LEqual
            HLSLPROGRAM
            #pragma vertex Vert
            #pragma fragment Fragment
            ENDHLSL
        }
    }
    Fallback Off
}
