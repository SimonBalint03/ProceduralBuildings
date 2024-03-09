Shader "Custom/URP_WindowShader"
{
    Properties
    {
        _BaseMap("Base (RGB)", 2D) = "white" { }
        _WindowMask("Window Mask (A)", 2D) = "white" { }
    }

    SubShader
    {
        Tags { "RenderPipeline" = "UniversalRenderPipeline" }

        Pass
        {
            Blend SrcAlpha OneMinusSrcAlpha

            CGPROGRAM
            #pragma vertex vert
            #pragma exclude_renderers gles xbox360 ps3

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
            };

            struct v2f
            {
                float4 pos : POSITION;
                float4 color : COLOR;
            };

            sampler2D _BaseMap;
            sampler2D _WindowMask;

            v2f vert(appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.color = tex2D(_WindowMask, v.vertex);
                return o;
            }
            ENDCG
        }
    }

    Fallback "Diffuse"
}
