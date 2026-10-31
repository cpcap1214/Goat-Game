Shader "OneMoreTime/LogoMask" {
 Properties { _MainTex ("Logo", 2D) = "white" {} }
 SubShader {
  Tags { "Queue"="Overlay" "RenderType"="Transparent" }
  Cull Off ZWrite Off ZTest Always Blend SrcAlpha OneMinusSrcAlpha
  Pass {
   CGPROGRAM
   #pragma vertex vert
   #pragma fragment frag
   #include "UnityCG.cginc"
   struct appdata { float4 vertex:POSITION; float2 uv:TEXCOORD0; };
   struct v2f { float4 position:SV_POSITION; float2 uv:TEXCOORD0; };
   sampler2D _MainTex;
   v2f vert(appdata v){v2f o;o.position=UnityObjectToClipPos(v.vertex);o.uv=v.uv;return o;}
   fixed4 frag(v2f i):SV_Target {fixed3 rgb=tex2D(_MainTex,i.uv).rgb;float mask=saturate((max(rgb.r,max(rgb.g,rgb.b))-.08)/.84);return fixed4(1,1,1,mask);}
   ENDCG
  }
 }
}
