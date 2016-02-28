Shader "Hidden/Spektr/Voronoi/Contour"
{
    Properties
    {
        _ColorTexture("", 2D) = ""{}
        _NormalTexture("", 2D) = ""{}
    }

    CGINCLUDE

    #include "UnityCG.cginc"

    sampler2D _ColorTexture;
    float2 _ColorTexture_TexelSize;

    sampler2D _NormalTexture;
    float2 _NormalTexture_TexelSize;

    float _LowThreshold;
    float _HighThreshold;

    half4 frag(v2f_img i) : SV_Target
    {
        float4 disp = float4(_NormalTexture_TexelSize.xy, -_NormalTexture_TexelSize.x, 0);

        // four sample points for the roberts cross operator
        float2 uv0 = i.uv;           // TL
        float2 uv1 = i.uv + disp.xy; // BR
        float2 uv2 = i.uv + disp.xw; // TR
        float2 uv3 = i.uv + disp.wy; // BL

        // sample normal vector values from the g-buffer
        float3 n0 = tex2D(_NormalTexture, uv0);
        float3 n1 = tex2D(_NormalTexture, uv1);
        float3 n2 = tex2D(_NormalTexture, uv2);
        float3 n3 = tex2D(_NormalTexture, uv3);

        // roberts cross operator
        float3 ng1 = n1 - n0;
        float3 ng2 = n3 - n2;
        float ng = sqrt(dot(ng1, ng1) + dot(ng2, ng2));

        // thresholding
        float edge = saturate((ng - _LowThreshold) / (_HighThreshold - _LowThreshold));

        half3 src = tex2D(_ColorTexture, i.uv).rgb;
        half lm = Luminance(src);
        src = LinearToGammaSpace(src);
        src = lerp(1, src, lm) * edge + (lm > 0.5);
        src = GammaToLinearSpace(src);
        return half4(src, 1);
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
