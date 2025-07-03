Shader "Almgp/two_side_shader" {
    Properties {
        _diffuse ("diffuse", 2D) = "white" {}
        _spec ("spec", Range(0, 1)) = 0
        _gloss ("gloss", Range(0, 1)) = 0
    }
    SubShader {
        Tags { "RenderType"="Opaque" }
        Cull Off

        CGPROGRAM
        #pragma surface surf Standard fullforwardshadows
        #pragma target 3.0

        sampler2D _diffuse;
        half _spec;
        half _gloss;

        struct Input {
            float2 uv_diffuse;
        };

        void surf (Input IN, inout SurfaceOutputStandard o) {
            half4 c = tex2D(_diffuse, IN.uv_diffuse);
            o.Albedo = c.rgb;
            o.Metallic = _spec;
            o.Smoothness = _gloss;
            o.Alpha = c.a;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
