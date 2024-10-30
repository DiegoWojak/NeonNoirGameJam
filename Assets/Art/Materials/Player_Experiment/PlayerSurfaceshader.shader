Shader "Custom/PlayerSurfaceshader"
{
     Properties
    {
        _WireThickness ("Wire Thickness", RANGE(0, 800)) = 100
        _ColorIndex("ColorIndex", Integer) = 0
        _Glossiness ("Smoothness", Range(0,1))=0.5
        _Metallic ("Metallic", Range(0,1))=0.0
        _Color ("Color", Color)=(1,1,1,1)

        _GIAlbedoColor ("Color Albedo (GI)", Color)=(1,1,1,1)
    }

    SubShader
    {
        Pass{
            Tags { "LightMode" = "ShadowCaster" }
        }
        
        Pass
        {
            Tags { "RenderType"="Opaque" "Lightmode" = "ForwardBase" }
            
            CGPROGRAM
            #pragma vertex vert
            #pragma geometry geom
            #pragma fragment frag           

            #include "UnityCG.cginc"
            #include "Lighting.cginc"

            float _WireThickness;
            float3 _normal;
            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 uv : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2g
            {
                float4 projectionSpaceVertex : SV_POSITION;
                float4 worldSpacePosition : TEXCOORD1;
                float3 worldPos : TEXCOORD2;
                float3 worldNormal : TEXCOORD3;
                UNITY_VERTEX_OUTPUT_STEREO_EYE_INDEX
                UNITY_VERTEX_INPUT_INSTANCE_ID 
            };

            struct g2f
            {
                float4 projectionSpaceVertex : SV_POSITION;
                float4 worldSpacePosition : TEXCOORD0;
                float4 dist : TEXCOORD1;
                float3 worldPos : TEXCOORD2;
                float3 worldNormal : TEXCOORD3;
                UNITY_VERTEX_OUTPUT_STEREO
            };

            UNITY_INSTANCING_BUFFER_START(Props)
            UNITY_DEFINE_INSTANCED_PROP(int, _ColorIndex)
            UNITY_INSTANCING_BUFFER_END(Props)
            
            fixed4 _GIAlbedoColor;

            v2g vert (appdata v)
            {
                v2g o;
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_INITIALIZE_OUTPUT_STEREO_EYE_INDEX(o);
                o.projectionSpaceVertex = UnityObjectToClipPos(v.vertex);
                o.worldSpacePosition = mul(unity_ObjectToWorld, v.vertex);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                o.worldNormal = normalize(mul((float3x3)unity_ObjectToWorld, v.normal));
                return o;
            }

            [maxvertexcount(3)]
            void geom(triangle v2g i[3], inout TriangleStream<g2f> triangleStream)
            {
                UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i[0]);

                float2 p0 = i[0].projectionSpaceVertex.xy / i[0].projectionSpaceVertex.w;
                float2 p1 = i[1].projectionSpaceVertex.xy / i[1].projectionSpaceVertex.w;
                float2 p2 = i[2].projectionSpaceVertex.xy / i[2].projectionSpaceVertex.w;

                float2 edge0 = p2 - p1;
                float2 edge1 = p2 - p0;
                float2 edge2 = p1 - p0;

               
                float area = abs(edge1.x * edge2.y - edge1.y * edge2.x);
                float wireThickness = 800 - _WireThickness;

                g2f o;
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
                   

                o.worldSpacePosition = i[0].worldSpacePosition;
                o.projectionSpaceVertex = i[0].projectionSpaceVertex;
                o.dist.xyz = float3( (area / length(edge0)), 0.0, 0.0) * o.projectionSpaceVertex.w * wireThickness;
                o.dist.w = 1.0 / o.projectionSpaceVertex.w;
                o.worldPos = i[0].worldPos;
                o.worldNormal = i[0].worldNormal;
                triangleStream.Append(o);

                o.worldSpacePosition = i[1].worldSpacePosition;
                o.projectionSpaceVertex = i[1].projectionSpaceVertex;
                o.dist.xyz = float3(0.0, (area / length(edge1)), 0.0) * o.projectionSpaceVertex.w * wireThickness;
                o.dist.w = 1 / o.projectionSpaceVertex.w;
                o.worldPos = i[1].worldPos;
                o.worldNormal = i[1].worldNormal;
                triangleStream.Append(o);

                o.worldSpacePosition = i[2].worldSpacePosition;
                o.projectionSpaceVertex = i[2].projectionSpaceVertex;
                o.worldPos = i[2].worldPos;
                o.worldNormal = i[2].worldNormal;
                o.dist.xyz = float3(0.0, 0.0, (area / length(edge2))) * o.projectionSpaceVertex.w * wireThickness;
                o.dist.w = 1.0 / o.projectionSpaceVertex.w;
                //o.normal = normal0; 
                triangleStream.Append(o);
            }

            fixed4 frag (g2f i) : SV_Target
            {
                
                fixed4 c = fixed4(0,0,0,1);
                
                float minDistanceToEdge = min(i.dist[0], min(i.dist[1], i.dist[2])) * i.dist[3];

                // Early out if we know we are not on a line segment.
                if(minDistanceToEdge > 0.9)
                {
                    return c;
                }                

                float3 normal = normalize(i.worldNormal);
                float3 viewDir = normalize(_WorldSpaceCameraPos.xyz - i.worldPos);

                float3 lightDir = normalize(_WorldSpaceLightPos0.xyz - i.worldPos);

                float3 combinedDir = normalize(lightDir + viewDir);

                float NdotL = max(dot(normal, combinedDir), 0.0);

                const fixed4 colors[11] = {
                        fixed4(1.0, 1.0, 1.0, 1.0),  // White
                        fixed4(1.0, 0.0, 0.0, 1.0),  // Red
                        fixed4(0.0, 1.0, 0.0, 1.0),  // Green
                        fixed4(0.0, 0.0, 0.0, 0.1),  // Blue
                        fixed4(1.0, 1.0, 0.0, 1.0),  // Yellow
                        fixed4(0.0, 1.0, 1.0, 1.0),  // Cyan/Aqua
                        fixed4(1.0, 0.0, 1.0, 1.0),  // Magenta
                        fixed4(0.5, 0.0, 0.0, 1.0),  // Maroon
                        fixed4(0.0, 0.5, 0.5, 1.0),  // Teal
                        fixed4(1.0, 0.65, 0.0, 1.0), // Orange
                        fixed4(1.0, 1.0, 1.0, 1.0)   // White
                    };

                int index = clamp(UNITY_ACCESS_INSTANCED_PROP(Props, _ColorIndex),0,10);
                fixed4 wireColor = colors[index];
                wireColor.rgb *= NdotL;
                fixed4 finalColor = lerp(float4(0,0,0,1), wireColor, 1);
                finalColor.a = 1;
                
                return finalColor;
            }
            ENDCG

            
        }      
    }
}