// "Invisible" Occlusion Shader
// Thanks so much to Mark Johns / Doomlaser - https://twitter.com/Doomlaser for answering my shader questions
// and guiding me on the best practices when working with shaders and AR.
// All this shader does is prevents pixels from being renders at that objects location. Again, there are lots of shaders like this one oneline
// this is the combination that worked best in AR

Shader "DepthMask"
{
    Properties
    {
    }
    SubShader
    {
        Tags
        {
            "RenderType" = "Opaque"
            "Queue" = "Geometry-1"
        }
        Pass
        {
            ColorMask 0 // Color will not be rendered.

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct v2f
            {
                float4 pos : SV_POSITION;
            };

            v2f vert(appdata_base v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                return o;
            }

            half4 frag(v2f i) : COLOR
            {
                return float4(1,1,1,1);
            }
            ENDCG
        }
    }
}