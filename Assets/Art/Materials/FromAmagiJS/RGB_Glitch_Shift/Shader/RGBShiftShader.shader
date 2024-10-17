Shader "Custom/Amagi/RGBShiftShader"
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

            float nn(float y, float t){
                float n = (
                    sin(y * .07 + t * 8. + sin(y * .5 + t * 10.)) + 
                    sin(y * .7 + t * 2. + sin(y * .3 + t * 8. )) * .7 +
                    sin(y * 1.1 + t * 2.8) * .4
                );

                n += sin(y * 124. + t * 100.7) * sin(y * 877. - t * 38.8) * .3;
                return n;
            }

            float step2(float t, float2 uv){
                return step(t, uv.x) * step(t, uv.y);
            }

            float inside(float2 uv){
                return step2(0., uv) * step2(0., 1. - uv);
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
                float t = fmod(_Time.y, 30.);
                float amp = 10. / _ScreenParams.x;

                if(abs(nn(i.uv.y, t)) > 1. ){
                    uvr.x += nn(i.uv.y, t) * amp;
                    uvg.x += nn(i.uv.y, t + 10.) * amp;
                    uvb.x += nn(i.uv.y, t + 20.) * amp;
                }

                float4 cr = tex2D(_MainTex, uvr) * inside(uvr);
                float4 cg = tex2D(_MainTex, uvg) * inside(uvg);
                float4 cb = tex2D(_MainTex, uvb) * inside(uvb);

                fixed4 col = fixed4(cr.r,cg.g, cb.b , smoothstep(.0, 1., cr.a + cg.a + cb.a));
                return col;
            }
            ENDCG
        }
    }
}
 
