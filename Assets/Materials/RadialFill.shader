Shader "Custom/RadialFill"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)
        _FillAmount ("Fill Amount", Range(0, 1)) = 1.0
        _StartAngle ("Start Angle Offset (Degrees)", Range(0, 360)) = 90
        [Toggle] _Clockwise ("Clockwise", Float) = 1
    }

    SubShader
    {
        Tags
        {
            "Queue"="Transparent"
            "IgnoreProjector"="True"
            "RenderType"="Transparent"
            "PreviewType"="Plane"
            "CanUseSpriteAtlas"="True"
        }

        Cull Off
        Lighting Off
        ZWrite Off
        Blend One OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata_t
            {
                float4 vertex   : POSITION;
                float4 color    : COLOR;
                float2 texcoord : TEXCOORD0;
            };

            struct v2f
            {
                float4 vertex   : SV_POSITION;
                fixed4 color    : COLOR;
                float2 texcoord : TEXCOORD0;
            };

            sampler2D _MainTex;
            fixed4 _Color;
            float _FillAmount;
            float _StartAngle;
            float _Clockwise;

            v2f vert(appdata_t IN)
            {
                v2f OUT;
                OUT.vertex = UnityObjectToClipPos(IN.vertex);
                OUT.texcoord = IN.texcoord;
                OUT.color = IN.color * _Color;
                return OUT;
            }

            fixed4 frag(v2f IN) : SV_Target
            {
                // Center UVs from [0, 1] to [-0.5, 0.5]
                float2 uv = IN.texcoord - float2(0.5, 0.5);

                // Calculate angle in radians (-PI to PI)
                float angle = atan2(uv.y, uv.x);

                // Convert to degrees [0, 360]
                float deg = degrees(angle);
                if (deg < 0) deg += 360.0;

                // Apply Start Angle offset
                deg = fmod(deg - _StartAngle + 360.0, 360.0);

                // Flip if Clockwise fill is enabled
                if (_Clockwise > 0.5)
                {
                    deg = 360.0 - deg;
                    if (deg >= 360.0) deg -= 360.0;
                }

                // Calculate normalized progress (0 to 1)
                float progress = deg / 360.0;

                // Discard pixels beyond current fill amount
                if (progress > _FillAmount)
                {
                    discard;
                }

                fixed4 c = tex2D(_MainTex, IN.texcoord) * IN.color;
                c.rgb *= c.a;
                return c;
            }
            ENDCG
        }
    }
}
