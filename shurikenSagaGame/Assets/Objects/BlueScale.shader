Shader "Unlit/BlueScale"
{
    Properties
    {
        _MainTex ("Sprite Texture", 2D) = "white" {}
        _ColorScale ("Tint Color", Color) = (.4, .4, 1, 1) // Default to white
        _Brightness ("Brightness", Range(0, 1)) = 1 // Brightness control
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
            float4 _ColorScale; // Tint color
            float _Brightness; // Brightness multiplier

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

                // Apply grayscale conversion
                float gray = dot(col.rgb, float3(0.299, 0.587, 0.114));

                // Apply tint and brightness
                col.rgb = gray * _ColorScale.rgb * _Brightness;

                return fixed4(col.rgb, col.a); // Preserve alpha
            }
            ENDCG
        }
    }
}
