Shader "Custom/URP/DistanceFog"
{
    Properties
    {
        _CenterPos ("centerpos", Vector) = (0, 0, 0, 0)
        _MaxDistance("Fog Max Distance", Float) = 5.0
        _FadeRange("Fog Fade Range", Float) = 2.0
        _FogColor("Fog Color", Color) = (0,0,0,1)
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" "Queue"="Overlay" }
        Pass
        {
            Name "DistanceFogPass"
            ZTest Always
            ZWrite Off
            Cull Back

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DeclareDepthTexture.hlsl"
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Common.hlsl"
            
            float4 _CenterPos;
            float _MaxDistance;
            float _FadeRange;
            float4 _FogColor;
            float4x4 _InvViewProj;

            Texture2D _CameraColorTexture;
            SamplerState sampler_CameraColorTexture;


            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            Varyings vert (Attributes IN)
            {
                Varyings OUT;
                OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);
                OUT.uv = IN.uv;
                return OUT;
            }

            float4 frag (Varyings IN) : SV_Target
            {

                float2 ScreenUV = IN.positionHCS.xy / _ScreenParams.xy;

                #if UNITY_REVERSED_Z
                    real depth = SampleSceneDepth(IN.uv);
                #else
                    real depth = lerp(UNITY_NEAR_CLIP_VALUE, 1, SampleSceneDepth(ScreenUV);
                #endif

                float3 worldPos = ComputeWorldSpacePosition(ScreenUV, depth, UNITY_MATRIX_I_VP);
      
                // 读取屏幕颜色
                float4 col = SAMPLE_TEXTURE2D(_CameraColorTexture, sampler_CameraColorTexture, ScreenUV);

                float dist = distance(worldPos.xyz, _CenterPos.xyz);

                // 计算插值：当 dist <= _MaxDistance 时 完全显示原色；当 dist >= _MaxDistance+_FadeRange 时 完全雾色
                float alpha = saturate((dist - _MaxDistance) / _FadeRange);
                
                // 雾化过渡
                float4 finalColor = lerp(col, _FogColor, alpha);
                // return float4(depth, depth, depth, 1);

                return finalColor;
            }
            ENDHLSL
        }
    }
}
