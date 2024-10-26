
Shader "Custom/Hidden/EdgeDetection"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _EdgeColor ("Edge Color", Color) = (1,1,1,1)
        _BackgroundColor ("Background Color", Color) = (0,0,0,1)
        _EdgeThreshold ("Edge Threshold", Range(0,1)) = 0.1
        _Circle1Center ("Circle 1 Center", Vector) = (0.5, 0.5, 0, 0) // Center of circle 1
        _Circle2Center ("Circle 2 Center", Vector) = (0.5, 0.5, 0, 0) // Center of circle 2
        _CircleRadius ("Circle Radius", Float) = 0.25 // Radius of both circles
        [Toggle(MORE_CALCULATION)]_MORE_CALCULATION("Better Calculation", Float) = 0
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
            #pragma shader_feature MORE_CALCULATION

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 pos : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _EdgeColor;
            float4 _BackgroundColor;
            float _EdgeThreshold;
            float2 _Circle1Center;
            float2 _Circle2Center;
            float _CircleRadius;

            v2f vert (appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            float4 frag (v2f i) : SV_Target
            {
                float2 texelSize = 1.0 / _ScreenParams.xy; // size of a single texel
                float4 col = tex2D(_MainTex, i.uv);

                if (dot(col, float3(0.299, 0.587, 0.114)) < 0.05) {
                    return float4(0.0, 0.0, 0.0, 1.0);
                }

                float dist1 = distance(i.uv, _Circle1Center);
                float dist2 = distance(i.uv, _Circle2Center);

                // Check if pixel is inside either circle
                bool insideCircle1 = dist1 < _CircleRadius;
                bool insideCircle2 = dist2 < _CircleRadius;

                if (insideCircle1 || insideCircle2)
                {
                    #ifdef _MORE_CALCULATION
                        float3 sampleTL = tex2D(_MainTex, i.uv + texelSize * float2(-1, 1)).rgb; // Top left
                        float3 sampleT = tex2D(_MainTex, i.uv + texelSize * float2(0, 1)).rgb;   // Top
                        float3 sampleTR = tex2D(_MainTex, i.uv + texelSize * float2(1, 1)).rgb;  // Top right
                        float3 sampleL = tex2D(_MainTex, i.uv + texelSize * float2(-1, 0)).rgb;  // Left
                        float3 sampleR = tex2D(_MainTex, i.uv + texelSize * float2(1, 0)).rgb;   // Right
                        float3 sampleBL = tex2D(_MainTex, i.uv + texelSize * float2(-1, -1)).rgb;// Bottom left
                        float3 sampleB = tex2D(_MainTex, i.uv + texelSize * float2(0, -1)).rgb;  // Bottom
                        float3 sampleBR = tex2D(_MainTex, i.uv + texelSize * float2(1, -1)).rgb; // Bottom right

                        float3 horizontalEdge = (sampleTL + 2.0 * sampleL + sampleBL) - (sampleTR + 2.0 * sampleR + sampleBR);
                        float3 verticalEdge = (sampleTL + 2.0 * sampleT + sampleTR) - (sampleBL + 2.0 * sampleB + sampleBR);
                        float edgeStrength = length(horizontalEdge + verticalEdge);
                        if (edgeStrength > _EdgeThreshold)
                        return _EdgeColor; // Edge color
                    #else
                        // Sample only 4 points instead of 9 for edge detection
                        float3 texSampleTL = tex2D(_MainTex, i.uv + texelSize * float2(-1, 1)).rgb;
                        float3 texSampleTR = tex2D(_MainTex, i.uv + texelSize * float2(1, 1)).rgb;
                        float3 texSampleBL = tex2D(_MainTex, i.uv + texelSize * float2(-1, -1)).rgb;
                        float3 texSampleBR = tex2D(_MainTex, i.uv + texelSize * float2(1, -1)).rgb;

                        half3 sobelX = texSampleTL - texSampleTR;
                        half3 sobelY = texSampleBL - texSampleBR;

                        half edge = length(sobelX + sobelY);
                        if (edge > _EdgeThreshold)
                            return _EdgeColor; // Edge color
                    #endif


                }

                    return _BackgroundColor; // Background color
            }
            ENDCG
        }
    }
}

    