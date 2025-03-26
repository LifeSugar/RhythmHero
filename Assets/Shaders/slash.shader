Shader "Custom/vfx/slash"
{
    Properties
    {
        _MainTex("Main Tex", 2D) = "white" {}
        _Emission_Power("Emission Power", Float) = 1.0
    }
    SubShader
    {
        Tags { "RenderPipeline"="UniversalPipeline"
            "Queue"="Transparent" "RenderType"="Transparent"
            "IgnoreProjector"="True" }
        LOD 100

        Pass
        {
            Name "UniversalForward"
            Tags { "LightMode"="UniversalForward" }
            Blend SrcAlpha One
            Cull Off
            ZWrite Off
            ZTest LEqual

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_fog

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderVariablesFunctions.hlsl"

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);

            CBUFFER_START(UnityPerMaterial)
                float4 _MainTex_ST;
                float _Emission_Power;
            CBUFFER_END

            struct Attributes
            {
                float4 positionOS : POSITION;
                float4 color      : COLOR;
                float2 uv         : TEXCOORD0;
                float4 uv1        : TEXCOORD1;
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float4 color      : COLOR;
                float2 uv         : TEXCOORD0;
                float4 uv1        : TEXCOORD1;
            };

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                OUT.positionCS = TransformObjectToHClip(IN.positionOS.xyz);
                OUT.color = IN.color;
                OUT.uv = TRANSFORM_TEX(IN.uv, _MainTex);
                OUT.uv1 = IN.uv1;
                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target
            {
                float2 uv = IN.uv;
                float4 baseSample = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv);
                float distortion = baseSample.b * IN.uv1.z;
                float4 texSample = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv + distortion);

                float clampVal = saturate(((-IN.uv1.x * IN.uv1.y) + ((texSample.g + (1.0 - IN.uv1.x)) * (1.0 - (-IN.uv1.x * IN.uv1.y)) / ((IN.uv1.x * 0.1) + 1.0))));
                float fade = 1.0;

                float alpha = texSample.r * IN.color.a * clampVal * _Emission_Power * fade;
                float3 rgb = texSample.r * IN.color.rgb * clampVal * _Emission_Power * fade;

                // Fog calculation using URP ShaderVariablesFunctions
                float fogFactor = ComputeFogFactor(IN.positionCS.z);
                half fogIntensity = ComputeFogIntensity(fogFactor);
                float3 fogColor = unity_FogColor;
                float3 finalColor = lerp(fogColor, rgb, fogIntensity);

                return half4(finalColor, alpha);
            }
            ENDHLSL
        }
    }
    FallBack Off
}
