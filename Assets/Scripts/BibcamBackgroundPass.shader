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
float _DepthOffset;

float4 _DepthColor;
float4 _StencilColor;

void FullScreenPass(Varyings varyings,
                    out float4 outColor : SV_Target,
                    out float outDepth : SV_Depth)
{
    // Calculate the UV coordinates from varyings
    float2 uv = (varyings.positionCS.xy + float2(0.5, 0.5)) * _ScreenSize.zw;

    // Color/depth samples
    float4 color = tex2D(_ColorTexture, uv);
    float depth = tex2D(_DepthTexture, uv).x;

    // World space position
    float3 wpos = DistanceToWorldPosition
      (uv, depth, _RayParams, _InverseView);

    // Depth range mask
    float d_near = 1 - smoothstep(0.0, 0.1, depth - _DepthRange.x);
    float d_far = smoothstep(-0.1, 0, depth - _DepthRange.y);
    float d_safe = 1 - max(d_near, d_far);

    // Zebra pattern
    float zebra = frac(dot(uv, 20)) < 0.25;

    // 3-axis grid lines
    float3 wpc = wpos * 5;
    wpc = min(frac(1 - wpc), frac(wpc)) / fwidth(wpc);
    wpc = 1 - saturate(wpc * 0.5);
    float grid = max(max(wpc.x, wpc.y), wpc.z);

    // Depth overlay
    float d_ovr = d_safe * grid;
    d_ovr = max(d_ovr, d_near * zebra);
    d_ovr = max(d_ovr, d_far * zebra);

    // Stencil edge lines
    float s_edge = color.a * 2 - 1;
    s_edge = saturate(1 - 0.2 * abs(s_edge / fwidth(s_edge)));

    // Blending
    float3 rgb = color.rgb;
    /*
    rgb = lerp(rgb, _DepthColor.rgb, _DepthColor.a * d_ovr);
    rgb = lerp(rgb, _StencilColor.rgb, _StencilColor.a * s_edge);
    */

    // Output
    //if (d_far < 0.99) discard;
    rgb = FastLinearToSRGB(rgb);
    rgb = dot(rgb, 1.0 / 3);
    rgb = round(rgb * 3) / 3;
    rgb *= float3(0.5, 0.7, 0.9);
    rgb = FastSRGBToLinear(rgb);

    rgb *= smoothstep(_DepthRange.y - 1, _DepthRange.y, depth);

    outColor = float4(rgb, 1);
    outDepth = DistanceToDepth(depth + _DepthOffset + 10);
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
