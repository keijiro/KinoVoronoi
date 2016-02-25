Shader "Hidden/Spektr/Voronoi_"
{
    CGINCLUDE

    #include "UnityCG.cginc"

    struct appdata
    {
        float4 vertex : POSITION;
        float2 uv : TEXCOORD0;
    };

    struct v2f
    {
        float4 vertex : SV_POSITION;
        float2 uv : TEXCOORD0;
    };

    float nrand01(float seed, float salt)
    {
        float2 uv = float2(seed, salt);
        return frac(sin(dot(uv, float2(12.9898, 78.233))) * 43758.5453);
    }

    v2f vert(appdata v)
    {
        float4 offs = float4(
            nrand01(v.uv.y, 0),
            nrand01(v.uv.y, 1), 0, 0);

        v2f o;
        o.vertex = mul(UNITY_MATRIX_MVP, v.vertex + offs);
        o.uv = v.uv;
        return o;
    }

    half4 frag (v2f i) : SV_Target
    {
        return i.uv.x * 0.1;
    }

    ENDCG
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            ENDCG
        }
    }
}
