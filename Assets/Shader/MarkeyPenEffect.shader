Shader "Brush/MarkPenEffect"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _BrushPos ("BrushPos", Vector) = (0,0,0,0)
        _BrushColor ("BrushColor", Color) = (1,1,1,1)
        _BrushSize ("BrushSize", float) = 0.01
    }

    Subshader
    {
        Tags{"RenderType" = "Opaque"}
        pass
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
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            // float4 _MainTex_ST;
            float4 _BrushPos;
            float4 _BrushColor;
            float _BrushSize;

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                // float2 brushPos = _BrushTex.xy;
                // float2 uv = i.uv;
                // float2 brushUV = float2(brushPos.x, brushPos.y);
                // float2 diff = uv - brushUV;
                // float dist = length(diff);
                float4 color = tex2D(_MainTex, i.uv);
                if (length(i.uv-_BrushPos.xy) < _BrushSize)
                {
                    color = _BrushColor;
                }
                return color;
            }
            ENDCG
        }
    }
}