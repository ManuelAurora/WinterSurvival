Shader "Custom/ScaleTransparentFigure"
{
    Properties
    {
        _MainTex ("Base (RGB)", 2D) = "white" {}
        _Scale ("Scale", Range(0.0, 2.0)) = 1.0
    }
    
    SubShader
    {
        Tags { "RenderType" = "Opaque" }
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata_t
            {
                float4 vertex : POSITION;
                float2 texcoord : TEXCOORD0;
            };

            struct v2f
            {
                float4 pos : POSITION;
                float2 uv : TEXCOORD0;
            };

            sampler2D _MainTex;
            float _Scale;

            v2f vert (appdata_t v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.texcoord;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float2 uv = i.uv;
                // Adjust UVs for scaling around the center
                uv = (uv - 0.5) / _Scale + 0.5;
                fixed4 col = tex2D(_MainTex, uv);

                // Only scale the transparent figure, keep the black background intact
                if (col.a < 0.1)
                {
                    col = tex2D(_MainTex, i.uv);
                }

                return col;
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}
