Shader "Custom/GhostTrailShader"
{
    Properties
    {
        _MainTex ("Main Texture", 2D) = "white" {}
        _GhostTex ("Ghost Texture", 2D) = "black" {}
        _FadeAmount ("Fade Amount", Range(0,1)) = 0.8
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            Stencil
            {
                Ref 10 // Apply ghost effect only to objects with Stencil Ref 1 (the player)
                Comp equal // Apply only where the stencil buffer equals Ref
            }

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            sampler2D _MainTex;
            sampler2D _GhostTex;
            float _FadeAmount;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };
            struct v2f
            {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            v2f vert(appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                fixed4 mainColor = tex2D(_MainTex, i.uv); // Scene texture
                fixed4 ghostColor = tex2D(_GhostTex, i.uv); // Ghost trail texture

                // Combine scene texture with ghost effect (faded over time)
                return lerp(mainColor, ghostColor, _FadeAmount);
            }
            ENDCG
        }
    }
}