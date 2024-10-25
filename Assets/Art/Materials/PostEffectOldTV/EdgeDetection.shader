
Shader "Custom/Hidden/EdgeDetection"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _EdgeColor ("Edge Color", Color) = (1,1,1,1)
        _BackgroundColor ("Background Color", Color) = (0,0,0,1)
        _EdgeThreshold ("Edge Threshold", Range(0,1)) = 0.1
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
                float4 pos : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _EdgeColor;
            float4 _BackgroundColor;
            float _EdgeThreshold;

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

                // Sample neighboring pixels for edge detection (Sobel)
                float3 sampleTL = tex2D(_MainTex, i.uv + texelSize * float2(-1, 1)).rgb; // Top left
                float3 sampleT = tex2D(_MainTex, i.uv + texelSize * float2(0, 1)).rgb;   // Top
                float3 sampleTR = tex2D(_MainTex, i.uv + texelSize * float2(1, 1)).rgb;  // Top right
                float3 sampleL = tex2D(_MainTex, i.uv + texelSize * float2(-1, 0)).rgb;  // Left
                float3 sampleR = tex2D(_MainTex, i.uv + texelSize * float2(1, 0)).rgb;   // Right
                float3 sampleBL = tex2D(_MainTex, i.uv + texelSize * float2(-1, -1)).rgb;// Bottom left
                float3 sampleB = tex2D(_MainTex, i.uv + texelSize * float2(0, -1)).rgb;  // Bottom
                float3 sampleBR = tex2D(_MainTex, i.uv + texelSize * float2(1, -1)).rgb; // Bottom right

                // Sobel edge detection filter
                float3 horizontalEdge = (sampleTL + 2.0 * sampleL + sampleBL) - (sampleTR + 2.0 * sampleR + sampleBR);
                float3 verticalEdge = (sampleTL + 2.0 * sampleT + sampleTR) - (sampleBL + 2.0 * sampleB + sampleBR);
                float edgeStrength = length(horizontalEdge + verticalEdge);

                // Compare edge strength to the threshold to determine edge
                if (edgeStrength > _EdgeThreshold)
                    return _EdgeColor; // Edge color
                else
                    return _BackgroundColor; // Background color
            }
            ENDCG
        }
    }
}

    