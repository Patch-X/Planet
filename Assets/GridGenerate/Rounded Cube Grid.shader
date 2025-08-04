Shader "URP/Custom/RoundedCubeGrid"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0
        [KeywordEnum(X, Y, Z)] _Faces ("Faces", Float) = 0
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        Pass
        {
            Name "ForwardLit"
            Tags { "LightMode" = "UniversalForward" }

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_fog
            #pragma multi_compile _ _FACES_X _FACES_Y _FACES_Z
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Common.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 normalOS   : NORMAL;
                float4 color      : COLOR;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float2 uv : TEXCOORD0;
                float3 normalWS : TEXCOORD1;
                float fogCoord : TEXCOORD2;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            half _Glossiness;
            half _Metallic;
            float4 _Color;

            Varyings vert (Attributes IN)
            {
                Varyings OUT;

                float3 posWS = TransformObjectToWorld(IN.positionOS.xyz);
                OUT.positionHCS = TransformWorldToHClip(posWS);
                OUT.normalWS = TransformObjectToWorldNormal(IN.normalOS);

                #if defined(_FACES_X)
                    OUT.uv = IN.color.yz * 255;
                #elif defined(_FACES_Y)
                    OUT.uv = IN.color.xz * 255;
                #elif defined(_FACES_Z)
                    OUT.uv = IN.color.xy * 255;
                #else
                    OUT.uv = IN.color.xy * 255;
                #endif

                OUT.fogCoord = ComputeFogFactor(OUT.positionHCS.z);
                return OUT;
            }

			half4 frag (Varyings IN) : SV_Target
			{
				half4 col = tex2D(_MainTex, IN.uv) * _Color;

				SurfaceData surfaceData;
				ZERO_INITIALIZE(SurfaceData, surfaceData);
				surfaceData.albedo = col.rgb;
				surfaceData.specular = half3(0, 0, 0);
				surfaceData.metallic = _Metallic;
				surfaceData.smoothness = _Glossiness;
				surfaceData.occlusion = 1.0;
				surfaceData.emission = half3(0, 0, 0);
				surfaceData.alpha = col.a;
				surfaceData.clearCoatMask = 0.0;
				surfaceData.clearCoatSmoothness = 0.0;
				surfaceData.normalTS = half3(0, 0, 1); // 如果报错，可注释此行

				InputData inputData;
				ZERO_INITIALIZE(InputData, inputData);
				inputData.positionWS = IN.positionHCS.xyz;
				inputData.normalWS = normalize(IN.normalWS);
				inputData.viewDirectionWS = normalize(_WorldSpaceCameraPos - inputData.positionWS);
				inputData.shadowCoord = float4(0, 0, 0, 0);
				inputData.fogCoord = IN.fogCoord;
				inputData.vertexLighting = float3(0, 0, 0);
				inputData.bakedGI = float3(0, 0, 0);
				inputData.normalizedScreenSpaceUV = float2(0, 0);
				inputData.shadowMask = float4(1, 1, 1, 1);

				return UniversalFragmentPBR(inputData, surfaceData);
			}

            ENDHLSL
        }
    }

    FallBack "Hidden/InternalErrorShader"
}
