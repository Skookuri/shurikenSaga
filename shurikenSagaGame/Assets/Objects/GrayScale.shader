Shader "Unlit/GrayScaleWithColorShift"
{
    Properties
    {
        _MainTex ("Sprite Texture", 2D) = "white" {}
        _Brightness ("Brightness", Range(0, 1)) = 1.0
        _ColorShift ("Color Shift", Color) = (1, 1, 1, 1) // Default to white (no shift)
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        Blend SrcAlpha OneMinusSrcAlpha
        Cull Off
        Lighting Off
        ZWrite Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata_t
            {
                float4 vertex : POSITION;
                float2 texcoord : TEXCOORD0;
            };

            struct v2f
            {
                float2 texcoord : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float _Brightness;
            float4 _ColorShift;

            v2f vert (appdata_t v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.texcoord = v.texcoord;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.texcoord);

                // Simplified grayscale logic
                float gray = dot(col.rgb, float3(0.299, 0.587, 0.114));
                col.rgb = gray * _Brightness;

                // Apply color shift (tint) correctly
                col.rgb = col.rgb * _ColorShift.rgb;

                return fixed4(col.rgb, col.a); // Preserve alpha
            }
            ENDCG
        }
    }
}