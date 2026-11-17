Shader "OneMoreTime/Looping Forest" {
 Properties { _MainTex("Background",2D)="white" {} _Overlap("Edge blend",Range(.01,.3))=.12 }
 SubShader { Tags {"Queue"="Background" "RenderType"="Opaque"} Cull Off ZWrite Off
 Pass { CGPROGRAM
 #pragma vertex vert
 #pragma fragment frag
 #include "UnityCG.cginc"
 sampler2D _MainTex;float4 _Scroll,_RepeatSpan;float _Overlap;
 struct input {float4 vertex:POSITION;float2 uv:TEXCOORD0;};
 struct output {float4 vertex:SV_POSITION;float2 uv:TEXCOORD0;};
 output vert(input v){output o;o.vertex=UnityObjectToClipPos(v.vertex);o.uv=v.uv;return o;}
 fixed4 frag(output i):SV_Target {
  // Repeat with an overlapping strip at each edge. The wrap samples the same
  // pixels on both sides, avoiding a hard cut even when the artwork edges differ.
  float2 phase=frac(i.uv*_RepeatSpan.xy+_Scroll.xy);
  float2 distance=phase*(1-_Overlap);
  float2 uv=distance;
  float2 blend=1-smoothstep(0,_Overlap,distance);
  float2 other=distance+1-_Overlap;
  fixed4 a=tex2D(_MainTex,uv);
  fixed4 b=tex2D(_MainTex,float2(other.x,uv.y));
  fixed4 c=tex2D(_MainTex,float2(uv.x,other.y));
  fixed4 d=tex2D(_MainTex,other);
  return lerp(lerp(a,b,blend.x),lerp(c,d,blend.x),blend.y);
 }
 ENDCG }
 }
}
