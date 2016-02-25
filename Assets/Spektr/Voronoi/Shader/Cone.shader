Shader "Hidden/Spektr/Voronoi/Cone"
{
    CGINCLUDE

    #include "UnityCG.cginc"
    #include "SimplexNoise2D.cginc"

    struct appdata
    {
        float4 vertex : POSITION;
        float3 normal : NORMAL;
        float2 uv : TEXCOORD0;
    };

    struct v2f
    {
        float4 vertex : SV_POSITION;
        float3 normal : NORMAL;
    };

    float nrand01(float seed, float salt)
    {
        float2 uv = float2(seed, salt);
        return frac(sin(dot(uv, float2(12.9898, 78.233))) * 43758.5453);
    }

    v2f vert(appdata v)
    {
        float4 offs = float4(
            nrand01(v.uv.y, 0)* 2 + 0.04 * snoise(float2(v.uv.y, 0 + _Time.y * 0.8)),
            nrand01(v.uv.y, 1)* 2 + 0.04 * snoise(float2(v.uv.y, 1 + _Time.y * 0.8)), 0, 0);

        v2f o;
        //o.vertex = mul(UNITY_MATRIX_MVP, v.vertex + offs);
        o.vertex = v.vertex + offs - float4(1, 1, 0, 0);
        o.normal = v.normal;
        return o;
    }

    half4 frag (v2f i) : SV_Target
    {
        return float4((i.normal + 1) * 0.5, 1);
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
