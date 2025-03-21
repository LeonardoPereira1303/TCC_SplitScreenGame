Shader "Mask/SplitScreen" {
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _SplitLine ("Split Line", Range(0,1)) = 0.5
        _BlendWidth ("Blend Width", Range(0, 0.1)) = 0.02
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" }
        Pass
        {
            ZWrite Off
            Blend SrcAlpha OneMinusSrcAlpha

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata_t {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float _SplitLine;
            float _BlendWidth;

            v2f vert (appdata_t v) {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target {
                float blendFactor = smoothstep(_SplitLine - _BlendWidth, _SplitLine + _BlendWidth, i.uv.x);
                return fixed4(1, 1, 1, blendFactor);
            }
            ENDCG
        }
    }
}
