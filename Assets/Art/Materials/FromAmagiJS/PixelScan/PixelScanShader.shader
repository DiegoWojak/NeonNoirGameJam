Shader "Custom/PixelScanShader"
{
    Properties
    {
        [HideInInspector]_MainTex ("Texture", 2D) = "white" {}
        _TransitionDuration("Transition Duration", Range(1, 10)) = 2
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        //Blend SrcAlpha DstAlpha //Blend Factors : One - Zero - SrcColor - SrcAlpha - DstColor - DstAlpha - OneMinusSrcColor - OneMinusSrcAlpha - OneMinusDstColor - OneMinusDstAlpha
        //BlendOp Add // (Default)Add -Sub -Max -Min -RevSub
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"
            
            #define MODE 0 // 0 <-- Left - Right , 1 Top - To - Bottom , 2: Radial
            #define LAYERS 5.0
            #define SPEED 1.0
            #define DELAY 0.0
            #define WIDTH 0.05

            #define W WIDTH
            #define MAX_LAYERS 32.0 // Define a maximum number of layers


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
            float _TransitionDuration;
            //
            float4 readTex(float2 uv) {
                if (uv.x < 0.0 || uv.x > 1.0 || uv.y < 0.0 || uv.y > 1.0) {
                    return float4(0.0,0.0,0.0,0);
                }
                return tex2D(_MainTex, uv);
            }

            //
            float hash(float2 p) {
                return frac(sin(dot(p, float2(4859.0, 3985.0))) * 3984.0);
            }

            //
            float3 hsv2rgb(float3 c) {
                float4 K = float4(1.0, 2.0 / 3.0, 1.0 / 3.0, 3.0);
                float3 p = abs(frac(c.xxx + K.xyz) * 6.0 - K.www);
                return c.z * lerp(K.xxx, clamp(p - K.xxx, 0.0, 1.0), c.y);
            }

            //
            float sdBox(float2 p, float r) {
                float2 q = abs(p) - r;
                return min(length(q), max(q.y, q.x));
            }
            //

            float dir = 1.0;

            float toRangeT(float2 p, float scale){
                float d;

                if(MODE == 0){
                    d = p.x / (scale * 2.0) + 0.5;
                }else if( MODE == 1){
                    d = 1.0 - (p.y / (scale * 2.0) + 0.5);
                }else if( MODE == 2){
                    d = length(p) / scale;
                }
                d = dir > 0.0 ? d : (1.0 - d);
                return d;
            }

            float4 cell(float2 p, float2 pi, float scale, float t, float edge) {
                float2 pc = pi + 0.5;
                float2 uvc = pc / scale;
                uvc.y /= _ScreenParams.y / _ScreenParams.x;
                uvc = uvc * 0.5 + 0.5;
                if (uvc.x < 0.0 || uvc.x > 1.0 || uvc.y < 0.0 || uvc.y > 1.0) {
                    return float4(.0,.0,.0,.0);
                }
                float alpha = smoothstep(0.0, 0.1, tex2D(_MainTex, uvc).a);
                
                float4 color = float4(hsv2rgb(float3((pc.x * 13.0 / pc.y * 17.0) * 0.3, 1.0, 1.0)), 1.0);
                
                float x = toRangeT(pi, scale);
                float n = hash(pi);
                float anim = smoothstep(W * 2.0, 0.0, abs(x + n * W - t));
                color *= anim;    
                
                color *= lerp(
                    1.0, 
                    clamp(0.3 / abs(sdBox(p - pc, 0.5)), 0.0, 10.0),
                    edge * pow(anim, 10.0)
                ); 
                
                return color * alpha;
            }

            float4 cellsColor(float2 p, float scale, float t) {
                float2 pi = floor(p);
                
                float2 d = float2(0.0, 1.0);
                float4 cc = float4(.0,.0,.0,.0);
                cc += cell(p, pi, scale, t, 0.2) * 4.0;
                cc += cell(p, pi + d.xy, scale, t, 0.9);
                cc += cell(p, pi - d.xy, scale, t, 0.9);
                cc += cell(p, pi + d.yx, scale, t, 0.9);
                cc += cell(p, pi - d.yx, scale, t, 0.9);
                
                return cc / 8.0;
            }

            float4 draw(float2 uv, float2 p, float t, float scale) {
                float4 c = readTex(uv);
                float2 pi = floor(p * scale);
                float n = hash(pi);
                t = t * (1.0 + W * 4.0) - W * 2.0;
                
                float x = toRangeT(pi, scale);
                float a1 = smoothstep(t, t - W, x + n * W);    
                c *= a1;
                c += cellsColor(p * scale, scale, t) * 1.5;
                
                return c;
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
                float2 uv = i.uv; //<-- 
                float2 p = uv  * 2. - 1.; //<-- 
                p.y *= _ScreenParams.y / _ScreenParams.x; //<-- 

                //float transitionDuration = 2.0; // Adjust this value to change transition duration
                float t = (_Time.y / _TransitionDuration) % 2.0;
                if (t > 1.0) {
                    t = 2.0 - t;
                    dir = -1.0;
                } else {
                    dir = 1.0;
                }
                t = clamp((t - DELAY) * SPEED, 0.0, 1.0);
                //t = (frac(t * 0.99999) - 0.5) * dir + 0.5;
                
                float4 finalColor = float4(0.,0.,0.,0.);
                float layerCount = 0.0;
                for (float i = 0.0; i < MAX_LAYERS; i++) {
                    if (i >= LAYERS) break;
                    float s = cos(i) * 7.3 + 10.0; 
                    finalColor += draw(uv, p, t, abs(s));
                    layerCount += 1.0;
                }
                fixed4 fragColor = finalColor / layerCount;  
                
                fragColor *= smoothstep(0.0, 0.01, t);

                return fragColor;
            }
            ENDCG
        }
    }
}