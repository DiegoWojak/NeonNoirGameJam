Shader "Custom/Thinking/14.1.5"
{
    Properties
    {
        [HideInInspector]_MainTex ("Texture", 2D) = "white" {}
        _TypeSelection("SelectionFigure", int) = 0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" "Queue" = "Transparent" }
        LOD 100

        GrabPass{"_GrabTexture"}
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 grabUv : TEXCOORD1;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex, _GrabTexture;
            float4 _MainTex_ST;


            int _TypeSelection;
            #define POINT_COUNT 8
            static const float POINT_EXTRA = 7.0;

            static float2 points[8];
            static const float speed = -0.5;
            static const float len = 0.25;
            static const float scale = 0.012;
            static float intensity = 1.3;
            static float radius = 0.015;


            float sdBezier(float2 pos, float2 A, float2 B, float2 C){    
                float2 a = B - A;
                float2 b = A - 2.0*B + C;
                float2 c = a * 2.0;
                float2 d = A - pos;

                float kk = 1.0 / dot(b,b);
                float kx = kk * dot(a,b);
                float ky = kk * (2.0*dot(a,a)+dot(d,b)) / 3.0;
                float kz = kk * dot(d,a);      

                float res = 0.0;

                float p = ky - kx*kx;
                float p3 = p*p*p;
                float q = kx*(2.0*kx*kx - 3.0*ky) + kz;
                float h = q*q + 4.0*p3;

                if(h >= 0.0){ 
                    h = sqrt(h);
                    float2 x = (float2(h, -h) - q) / 2.0;
                    float2 uv = sign(x)*pow(abs(x), (1.0/3.0));
                    float t = uv.x + uv.y - kx;
                    t = clamp( t, 0.0, 1.0 );

                    // 1 root
                    float2 qos = d + (c + b*t)*t;
                    res = length(qos);
                }else{
                    float z = sqrt(-p);
                    float v = acos( q/(p*z*2.0) ) / 3.0;
                    float m = cos(v);
                    float n = sin(v)*1.732050808;
                    float3 t = float3(m + m, -n - m, n - m) * z - kx;
                    t = clamp( t, 0.0, 1.0 );

                    // 3 roots
                    float2 qos = d + (c + b*t.x)*t.x;
                    float dis = dot(qos,qos);
                    
                    res = dis;

                    qos = d + (c + b*t.y)*t.y;
                    dis = dot(qos,qos);
                    res = min(res,dis);

                    qos = d + (c + b*t.z)*t.z;
                    dis = dot(qos,qos);
                    res = min(res,dis);

                    res = sqrt( res );
                }
                
                return res;
            }


            float2 getoFormPosition(float t){
                float x,y;
                float count = 4294967295.0;
                float e = pow( (1.0/count + 1.0),count);
                float alpha = 16.0;
                switch(_TypeSelection){
                    case 0: 
                        x = 15.0 *sin(t);
                        y = 15.0 *cos(t);
                    break;
                    case 1: 
                         x = 16.0 * sin(t) * sin(t) * sin(t);
                    y = 13.0 * cos(t) - 5.0 * cos(2.0 * t) - 2.0 * cos(3.0 * t) - cos (4.0 * t) ; 
                    y = -y;
                    break;
                    case 2: 
                    
                    x = alpha * cos(t) - (alpha * sin(t) * sin(t))*0.5; 
                    y = alpha * cos(t)*sin(t);
                    break;
                    case 3: 
                        e = 2.71828;
                        x = sin(t) * (pow(e,cos(t))- 2.0 * cos(4.0*t) + pow(sin(1.0 * t/12.0),5.0));
                        y = cos(t) * (pow(e,cos(t))- 2.0 * cos(4.0*t) + pow(sin(1.0 * t/12.0),5.0));
                        return float2(6.0*x,-4.0*y);
                    break;
                    default: 
                        x = 15.0*sin(t);
                        y = -10.0*pow(sin(2.0*t),4.0);
                    break;
                }
            
                return float2(-x,y);
            }

            float getGlow(float dist, float radius, float intensity){
                return pow(radius/dist, intensity);
            }

            float getSegment(float t, float2 pos, float offset){
                for(int i = 0; i < 8; i++){
                    points[i] = getoFormPosition(offset + float(i)*len + frac(speed * t) * 6.28);
                }
                
                float2 c = (points[0] + points[1]) / 2.0;
                float2 c_prev;
                float dist = 10000.0;
                
                for(int b = 0; b < 8-1; b++){
                    //https://tinyurl.com/y2htbwkm
                    c_prev = c;
                    c = (points[b] + points[b+1]) / 2.0;
                    dist = min(dist, sdBezier(pos, scale * c_prev, scale * points[b], scale * c));
                }
                return max(0.0, dist);
            }
            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.grabUv = UNITY_PROJ_COORD(ComputeGrabScreenPos(o.vertex)); 
                return o;
            }

            float N21(float2 p){
                p = frac(p*float2(123.34,345.45));
                p += dot(p, p+34.345);
                return frac(p.x*p.y);
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float2 uv = i.uv;
                float widthHeightRatio = .01;
                float2 centre = float2(0.5, 0.5);
                float2 pos = centre - uv;
                //pos.y /= widthHeightRatio;
                pos.y += 0.03;
                
                float t = _Time.y;
                
                //Get first segment
                float dist = getSegment(t, pos, 0.0);
                float glow = getGlow(dist, radius, intensity);
                
                float3 col = (0.0);
                float2 projUv = i.grabUv.xy / i.grabUv.w;
                float a= N21(i.uv)*6.2831;
                float blur = 0;
                const float numSamples = 32;
                for(float i=0; i<numSamples; i++){
                    float2 offs = float2(sin(a), cos(a))*blur;
                    offs *= frac(sin((i+1)*546.)*5424.);
                    col += tex2D(_GrabTexture, projUv+offs);
                    a++;
                }

                col /= numSamples;
                //White core
                col += 10.0*(smoothstep(0.006, 0.003, dist));
                //Pink glow
                col += glow * float3(1.0,0.05,0.3);
                
                //Get second segment
                dist = getSegment(t, pos, 3.4);
                glow = getGlow(dist, radius, intensity);
                
                //White core
                col += 10.0*(smoothstep(0.006, 0.003, dist));
                //Blue glow
                col += glow * float3(0.1,0.4,1.0);
                    
                //Tone mapping
                col = 1.0 - exp(-col);
                
                //Gamma
                //col = pow(col, (0.4545));

                //Output to screen
                return float4(col,1.0);

            }


            
            ENDCG
        }
    }
}
