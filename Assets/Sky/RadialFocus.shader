Shader "Custom/RadialFocus"
{
    Properties
    {
        _MainTex ("Base (RGB)", 2D) = "white" {}
        _Center ("Focus Center", Vector) = (0.5, 0.5, 0, 0)
        _Radius ("Focus Radius", Range(0, 1)) = 0.5
        _Softness ("Edge Softness", Range(0.001, 1)) = 0.1
        _DarkColor ("Dark Color", Color) = (0, 0, 0, 1)
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" }
        Pass
        {
            ZTest Always Cull Off ZWrite Off

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float4 _MainTex_TexelSize;

            float4 _Center;
            float _Radius;
            float _Softness;
            float4 _DarkColor;

            struct appdata {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            v2f vert (appdata v) {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float2 uv = i.uv;
                float2 center = _Center.xy;

                float2 offset = uv - center;
                float aspect = _MainTex_TexelSize.y / _MainTex_TexelSize.x;
                offset.x *= aspect;

                float dist = length(offset);
                float mask = smoothstep(_Radius, _Radius + _Softness, dist);

                fixed4 color = tex2D(_MainTex, uv);
                color.rgb = lerp(color.rgb, _DarkColor.rgb, mask);
                return color;
            }
            ENDCG
        }
    }
}
