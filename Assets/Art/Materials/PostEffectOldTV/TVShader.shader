Shader "Custom/TVShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"
            #define PI 3.141592
            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            float3 tvshader(float2 UV){
                UV.x = (UV.x - 0.5) / 3 * 4 * 380 / 640 + 0.5;

                float sc = UV.y * 300 * PI;

                float wh = (sin(sc) + 1) / 2;

                return tex2D(_MainTex, UV).rgb * wh;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = float4(tvshader(i.uv),1);
                return col;
            }
            ENDCG
        }
    }
}