Shader"Custom/GradientFromTop"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
    }
    SubShader
    {
        Tags {"Queue"="Transparent" "RenderType"="Transparent"}
Blend SrcAlpha OneMinusSrcAlpha
        
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
                UNITY_FOG_COORDS(1)
    float4 vertex : SV_POSITION;
};

fixed4 _Color;

v2f vert(appdata v)
{
    v2f o;
    o.vertex = UnityObjectToClipPos(v.vertex);
    o.uv = v.uv;
    UNITY_TRANSFER_FOG(o, o.vertex);
    return o;
}

fixed4 frag(v2f i) : SV_Target
{
    fixed4 col = _Color;
    col.a = i.uv.y; // Прозрачность от верха к низу
    return col;
}
            ENDCG
        }
    }
}
