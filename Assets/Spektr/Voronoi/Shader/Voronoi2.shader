Shader "Hidden/Spektr/Voronoi"
{
    SubShader
    {
        Tags { "RenderType"="Opaque" }

        CGPROGRAM

        #pragma surface surf Standard vertex:vert addshadow nolightmap
        #pragma target 3.0

        struct Input { float dummy; };

        float nrand01(float seed, float salt)
        {
            float2 uv = float2(seed, salt);
            return frac(sin(dot(uv, float2(12.9898, 78.233))) * 43758.5453);
        }

        void vert(inout appdata_full v)
        {
            float2 uv = v.texcoord.xy;
            v.vertex.xy += float2(nrand01(uv.y, 0), nrand01(uv.y, 1));
        }

        void surf(Input IN, inout SurfaceOutputStandard o)
        {
        }

        ENDCG
    } 
    FallBack "Diffuse"
}
