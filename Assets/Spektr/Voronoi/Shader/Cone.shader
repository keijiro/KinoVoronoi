Shader "Hidden/Spektr/Voronoi/Cone"
{
    CGINCLUDE

    #include "UnityCG.cginc"
    #include "SimplexNoise2D.cginc"

    sampler2D _Source;
    float _Aspect;
    float _LowThreshold;
    float _HighThreshold;
    float _RandomSeed;

    struct appdata
    {
        float4 vertex : POSITION;
        float2 uv : TEXCOORD0;
    };

    struct v2f
    {
        float4 vertex : SV_POSITION;
        float3 normal : NORMAL;
        half4 color : COLOR;
    };

    struct FragOutput
    {
        half4 color : COLOR0;
        half4 normal : COLOR1;
    };

    // PRNG function (0-1 range)
    float Random01(float seed1, float seed2)
    {
        float2 uv = float2(seed1, seed2 + _RandomSeed);
        return frac(sin(dot(uv, float2(12.9898, 78.233))) * 43758.5453);
    }

    // Sample point generator
    float2 SamplePoint(float id)
    {
        float rx = Random01(id, 0);
        float ry = Random01(id, 1);
        float nx = snoise(float2(id, _Time.x)) * 0.2;
        float ny = snoise(float2(_Time.x, id)) * 0.2;
        return float2(rx + nx, ry + ny);
    }

    v2f vert(appdata v)
    {
        // vertex id
        float id = v.uv.y;

        // sample point
        float2 spos = SamplePoint(id);

        // cone vertex position (without transfomation)
        float4 vpos = v.vertex * float4(2, _Aspect * 2, 1, 1);
        vpos.xy += spos * 2 - 1;

        // normal vector (remap position to 0-1 range)
        float3 vnrm = v.vertex.xyz * float3(0.5, 0.5, 1) + float3(0.5, 0.5, 0);

        // sample source color and reject the vertex if it's under the threshold
        half4 col = tex2D(_Source, spos);
        float thr = lerp(_LowThreshold, _HighThreshold, Random01(id, 2));
        vpos.xy += (Luminance(col.rgb) < thr) * 10000;

        // shader output
        v2f o;
        o.vertex = vpos;
        o.normal = vnrm;
        o.color = col;
        return o;
    }

    FragOutput frag(v2f i) : SV_Target
    {
        FragOutput o;
        o.color = i.color;
        o.normal = float4(i.normal, 1);
        return o;
    }

    ENDCG
    SubShader
    {
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 3.0
            ENDCG
        }
    }
}
