Shader "Hidden/Bibcam/HDRP/Background/Strips"
{
    HLSLINCLUDE

#include "Packages/com.unity.render-pipelines.high-definition/Runtime/RenderPipeline/RenderPass/CustomPass/CustomPassCommon.hlsl"
#include "Packages/jp.keijiro.bibcam/Decoder/Shaders/Utils.hlsl"

sampler2D _ColorTexture;
sampler2D _DepthTexture;

float4 _RayParams;
float4x4 _InverseView;
float2 _DepthRange;

float4 FullScreenPass(Varyings varyings) : SV_Target
{
    // Calculate the UV coordinates from varyings
    float2 uv = (varyings.positionCS.xy + 0.5) * _ScreenSize.zw;

    // Color/depth samples
    float4 color = tex2D(_ColorTexture, uv);
    float depth = tex2D(_DepthTexture, uv).x;

    // World space position
    float3 wpos = DistanceToWorldPosition
      (uv, depth, _RayParams, _InverseView);

    uint seed1 = (uint)((wpos.x + 100) * 10) * 2;
    float freq = lerp(0.2, 2, Hash(seed1 + 0));
    float speed = lerp(2, 8, Hash(seed1 + 1));

    uint seed2 = (wpos.z + 100) * freq + _Time.y * speed;
    float hue = Hash(seed2);

    float l = floor(Luminance(color.rgb) * 4) / 4;
    hue = frac(hue + l * 0.3);

    float3 rgb = HsvToRgb(float3(hue, 1, 1));

    float param = smoothstep(-0.1, 0, depth - _DepthRange.y);

    // Output
    return float4(lerp(rgb, 1 - Luminance(color.rgb), param), 1);
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
