Shader "Custom/NormalBlend(WithColor)"
{
   Properties
   {
       [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
   }

   SubShader
   {
       Tags
       {
           "Queue"="Transparent"
           "IgnoreProjector"="True"
           "RenderType"="Transparent"
           "PreviewType"="Plane"
           "CanUseSpriteAtlas"="True"
       }

       Cull Off
       Lighting Off
       ZWrite Off
       Blend One OneMinusSrcAlpha

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
               fixed4 color : COLOR;
           };

           struct v2f
           {
               float4 pos : SV_POSITION;
               float2 uv : TEXCOORD0;
               fixed4 color : COLOR;
           };

           sampler2D _MainTex;
           float4 _MainTex_ST;

           v2f vert(appdata v)
           {
               v2f o;
               o.pos = UnityObjectToClipPos(v.vertex);
               o.uv = TRANSFORM_TEX(v.uv, _MainTex);
               o.color = v.color;
               return o;
           }

           fixed4 frag(v2f i) : SV_Target
           {
               fixed4 tex = tex2D(_MainTex, i.uv);

               // Normal blend: spriteRenderer.color.a controls blend strength
               tex.rgb = lerp(tex.rgb, i.color.rgb, i.color.a);

               // Keep original texture alpha
               clip(tex.a - 0.001);

               return tex;
           }
           ENDCG
       }
   }
}