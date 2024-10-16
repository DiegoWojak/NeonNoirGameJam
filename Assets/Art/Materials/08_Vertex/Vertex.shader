Shader "Custom/Vertex"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Speed("Speed", Range(1,5)) = 1
        _Frequency("Frequency", Range(1,5)) = 1
        _Amplitud("Amplitud", Range(1,5)) = 1
        [Toggle] _FLAGMODE("Flag mode?", Float) = 0
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
            float _Speed;
            float _Frequency;
            float _Amplitud;
            float _FLAGMODE;

            float4 flag(float4 vertexPosition, float2 uv) 
            {
                vertexPosition.y += sin((uv.x- (_Time.y * _Speed)) * _Frequency) * (uv.x * _Amplitud);
                float4 vertex = UnityObjectToClipPos(vertexPosition);
                return vertex;
            }

            float4 wave(float4 vertexPosition, float2 uv)
            {
                vertexPosition.y += sin((uv.x- (_Time.y * _Speed)) * _Frequency);
                float4 vertex = UnityObjectToClipPos(vertexPosition);
                return vertex;
            }

            v2f vert (appdata v)
            {
                v2f o;
                if(_FLAGMODE == 0)
                    o.vertex = flag(v.vertex, v.uv);
                else
                    o.vertex = wave(v.vertex, v.uv);
                
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
                fixed4 col = tex2D(_MainTex, i.uv);
                return col;
            }
            ENDCG
        }
    }
}