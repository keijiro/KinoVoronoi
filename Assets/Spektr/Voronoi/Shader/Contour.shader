Shader "Hidden/Spektr/Voronoi/Contour"
{
    Properties
    {
        _MainTex ("-", 2D) = "" {}
    }

    CGINCLUDE

    #include "UnityCG.cginc"

    sampler2D _MainTex;
    float2 _MainTex_TexelSize;

    float _LowThreshold;
    float _HighThreshold;

    half4 frag(v2f_img i) : SV_Target
    {
        float4 disp = float4(_MainTex_TexelSize.xy, -_MainTex_TexelSize.x, 0);

        // four sample points for the roberts cross operator
        float2 uv0 = i.uv;           // TL
        float2 uv1 = i.uv + disp.xy; // BR
        float2 uv2 = i.uv + disp.xw; // TR
        float2 uv3 = i.uv + disp.wy; // BL

        // sample normal vector values from the g-buffer
        float3 n0 = tex2D(_MainTex, uv0);
        float3 n1 = tex2D(_MainTex, uv1);
        float3 n2 = tex2D(_MainTex, uv2);
        float3 n3 = tex2D(_MainTex, uv3);

        // roberts cross operator
        float3 ng1 = n1 - n0;
        float3 ng2 = n3 - n2;
        float ng = sqrt(dot(ng1, ng1) + dot(ng2, ng2));

        // thresholding
        float edge = saturate((ng - _LowThreshold) / (_HighThreshold - _LowThreshold));

        return edge;
    }

    ENDCG
    SubShader
    {
        Pass
        {
            ZTest Always Cull Off ZWrite Off
            CGPROGRAM
            #pragma vertex vert_img
            #pragma fragment frag
            #pragma target 3.0
            ENDCG
        }
    }
}
