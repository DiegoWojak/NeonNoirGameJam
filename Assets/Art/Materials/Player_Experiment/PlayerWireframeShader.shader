Shader "Custom/PlayerStencilWireframe"
{
     Properties
    {
        _WireThickness ("Wire Thickness", RANGE(0, 800)) = 100
        _ColorIndex("ColorIndex", Integer) = 0

    }

    CGINCLUDE
    #include "UnityCG.cginc"
  
    struct v2fShadow {
        V2F_SHADOW_CASTER;
        UNITY_VERTEX_OUTPUT_STEREO
    };

    v2fShadow vertShadow( appdata_base v )
    {
        v2fShadow o;
        UNITY_SETUP_INSTANCE_ID(v);
        UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
        TRANSFER_SHADOW_CASTER_NORMALOFFSET(o)
        return o;
    }

    float4 fragShadow( v2fShadow i ) : SV_Target
    {
        SHADOW_CASTER_FRAGMENT(i)
    }
    ENDCG

    SubShader
    {

        Pass
        {
            Tags { "RenderType"="Opaque" "LightMode" = "ForwardBase"}

            Stencil
            {
                Ref 10 // Reference value for the player
                Comp always // Always pass the stencil test
                Pass replace // Replace stencil value with Ref
            }

            CGPROGRAM
            #pragma vertex vert
            #pragma geometry geom
            #pragma fragment frag           

            #include "UnityCG.cginc"
            #include "UnityStandardBRDF.cginc" // for shader lighting info and some utils
            #include "UnityStandardUtils.cginc" // for energy conservation

            float _WireThickness;

            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2g
            {
                float4 projectionSpaceVertex : SV_POSITION;
                float4 worldSpacePosition : TEXCOORD1;
                UNITY_VERTEX_OUTPUT_STEREO_EYE_INDEX
                UNITY_VERTEX_INPUT_INSTANCE_ID 
            };

            struct g2f
            {
                float4 projectionSpaceVertex : SV_POSITION;
                float4 worldSpacePosition : TEXCOORD0;
                float4 dist : TEXCOORD1;
                UNITY_VERTEX_OUTPUT_STEREO
            };

            UNITY_INSTANCING_BUFFER_START(Props)
            UNITY_DEFINE_INSTANCED_PROP(int, _ColorIndex)
            UNITY_INSTANCING_BUFFER_END(Props)
            float3 _normal;
            v2g vert (appdata v)
            {
                v2g o;
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_INITIALIZE_OUTPUT_STEREO_EYE_INDEX(o);
                o.projectionSpaceVertex = UnityObjectToClipPos(v.vertex);
                o.worldSpacePosition = mul(unity_ObjectToWorld, v.vertex);
                _normal = v.normal;
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

                // To find the distance to the opposite edge, we take the
                // formula for finding the area of a triangle Area = Base/2 * Height,
                // and solve for the Height = (Area * 2)/Base.
                // We can get the area of a triangle by taking its cross product
                // divided by 2.  However we can avoid dividing our area/base by 2
                // since our cross product will already be double our area.
                float area = abs(edge1.x * edge2.y - edge1.y * edge2.x);
                float wireThickness = 800 - _WireThickness;

                g2f o;
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
                
                 // Initialize world normal (you can calculate or use a default normal if none is provided)
                //float3 normal0 = normalize(cross(i[1].worldSpacePosition.xyz - i[0].worldSpacePosition.xyz, i[2].worldSpacePosition.xyz - i[0].worldSpacePosition.xyz));

                o.worldSpacePosition = i[0].worldSpacePosition;
                o.projectionSpaceVertex = i[0].projectionSpaceVertex;
                o.dist.xyz = float3( (area / length(edge0)), 0.0, 0.0) * o.projectionSpaceVertex.w * wireThickness;
                o.dist.w = 1.0 / o.projectionSpaceVertex.w;
                //o.normal = normal0; 
                triangleStream.Append(o);

                o.worldSpacePosition = i[1].worldSpacePosition;
                o.projectionSpaceVertex = i[1].projectionSpaceVertex;
                o.dist.xyz = float3(0.0, (area / length(edge1)), 0.0) * o.projectionSpaceVertex.w * wireThickness;
                o.dist.w = 1.0 / o.projectionSpaceVertex.w;
                //o.normal = normal0; 
                triangleStream.Append(o);

                o.worldSpacePosition = i[2].worldSpacePosition;
                o.projectionSpaceVertex = i[2].projectionSpaceVertex;
                o.dist.xyz = float3(0.0, 0.0, (area / length(edge2))) * o.projectionSpaceVertex.w * wireThickness;
                o.dist.w = 1.0 / o.projectionSpaceVertex.w;
                //o.normal = normal0; 
                triangleStream.Append(o);
            }

            fixed4 frag (g2f i) : SV_Target
            {

                float minDistanceToEdge = min(i.dist[0], min(i.dist[1], i.dist[2])) * i.dist[3];

                // Early out if we know we are not on a line segment.
                if(minDistanceToEdge > 0.9)
                {
                    return fixed4(0,0,0,0);
                }                

                const fixed4 colors[11] = {
                        fixed4(1.0, 1.0, 1.0, 1.0),  // White
                        fixed4(1.0, 0.0, 0.0, 1.0),  // Red
                        fixed4(0.0, 1.0, 0.0, 1.0),  // Green
                        fixed4(0.0, 0.0, 1.0, 1.0),  // Blue
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

                fixed4 finalColor = lerp(float4(0,0,0,1), wireColor, 1);
                
                finalColor.a = 1.;

                return finalColor;
            }
            ENDCG
        }

        Pass
        {
            Name "ShadowCaster"
            Tags { "LightMode" = "ShadowCaster" }
            CGPROGRAM
            #pragma vertex vertShadow
            #pragma fragment fragShadow
            #pragma target 2.0
            #pragma multi_compile_shadowcaster
            ENDCG
        }
    }
}