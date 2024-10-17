Shader "Custom/Amagi/RGBGlitchShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
    }
    SubShader
    {
        Tags { "RenderType"="Cutout" }
        //Blend SrcAlpha DstAlpha //Blend Factors : One - Zero - SrcColor - SrcAlpha - DstColor - DstAlpha - OneMinusSrcColor - OneMinusSrcAlpha - OneMinusDstColor - OneMinusDstAlpha
        //BlendOp Add // (Default)Add -Sub -Max -Min -RevSub

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

            float random(float2 st){
                return frac(sin(dot(st, float2(948. , 824. ))) * 30284.);
            }

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float2 uvr = i.uv, uvg = i.uv, uvb = i.uv;
                float tt = fmod(_Time.y, 17.); //<---??? wtf amagi
                if(frac(tt * 0.73) > .8 || frac(tt * 0.91) > .8){
                    float t = floor(tt * 11.);

                    float n = random(float2(t, floor(i.uv.y * 17.7)));
                    if(n > .7){
                        uvr.x += random(float2(t, 1.)) * .1 - 0.05;
                        uvg.x += random(float2(t, 2.)) * .1 - 0.05;
                        uvb.x += random(float2(t, 3.)) * .1 - 0.05;
                    }
                    float ny = random(float2(t * 17. + floor(i.uv * 19.7)));
                    if(ny > .7){
                        uvr.x += random(float2(t, 4.)) * .1 - 0.05;
                        uvg.x += random(float2(t, 5.)) * .1 - 0.05;
                        uvb.x += random(float2(t, 6.)) * .1 - 0.05;
                    }
                }

                float4 cr = tex2D(_MainTex, uvr);
                float4 cg = tex2D(_MainTex, uvg);
                float4 cb = tex2D(_MainTex, uvb);
                
                fixed4 col = fixed4(cr.r, cg.g, cb.b, step(.1, cr.a + cg.a + cb.a));
                //fixed4 col = tex2D(_MainTex, i.uv);
                return col;
            }
            ENDCG
        }
    }
}
 
