Shader "Custom/PortalSurface"
{
    Properties
    {
        _MainTex ("Render Texture", 2D) = "white" {}    
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" "queue"="Geometry" }

        Stencil
        {
            Ref 1
            Comp Always
            Pass Replace
        }

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;

            struct appdata { float4 vertex : POSITION; };
            struct v2f
            {
                float4 pos : SV_POSITION;
                float4 screenPos : TEXCOORD0;
            };

            v2f vert(appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.screenPos = ComputeScreenPos(o.pos);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float2 uv = i.screenPos.xy / i.screenPos.w;
                return tex2D(_MainTex, uv);
            }
            ENDCG
        }
    }
}