Shader "UI/CircleCutoutSharp"
{
    Properties
    {
        _Color ("Color", Color) = (0,0,0,1)
        _MainTex ("Texture", 2D) = "white" {}
        _Radius ("Radius", Range(0,1)) = 0.3
        _Center ("Center", Vector) = (0.5, 0.5, 0, 0)
    }

    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" "IgnoreProjector"="True" "Canvas"="True" }
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha
        Cull Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata_t {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            sampler2D _MainTex;
            fixed4 _Color;
            float _Radius;
            float2 _Center;

            v2f vert (appdata_t v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float dist = distance(i.uv, _Center);
                float alpha = dist > _Radius ? 1.0 : 0.0; // 🔥 ВНЕ круга — чёрное, ВНУТРИ — прозрачное
                return fixed4(_Color.rgb, alpha);
            }
            ENDCG
        }
    }
}
