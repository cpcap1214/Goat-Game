Shader "OneMoreTime/Looping Forest" {
 Properties {
  _MainTex("Background",2D)="white" {} _Overlap("Edge blend",Range(.01,.3))=.12 _PixelGrid("Native pixel grid",Vector)=(384,384,0,0)
  _PastTex("Past background (optional)",2D)="white" {} _PastPixelGrid("Past pixel grid",Vector)=(384,384,0,0)
  _PastAmount("Past blend",Range(0,1))=0 _HasPastBackground("Use past artwork",Float)=0
  _PastTint("Temporary past tint",Color)=(1.08,1.02,.82,1) _PastSaturation("Temporary past saturation",Range(0,1))=.48
 }
 SubShader { Tags {"Queue"="Background" "RenderType"="Opaque"} Cull Off ZWrite Off
 Pass { CGPROGRAM
 #pragma vertex vert
 #pragma fragment frag
 #include "UnityCG.cginc"
 sampler2D _MainTex,_PastTex;float4 _Scroll,_RepeatSpan,_PixelGrid,_PastPixelGrid,_PastTint;float _Overlap,_PastAmount,_HasPastBackground,_PastSaturation;
 struct input {float4 vertex:POSITION;float2 uv:TEXCOORD0;};
 struct output {float4 vertex:SV_POSITION;float2 uv:TEXCOORD0;};
 output vert(input v){output o;o.vertex=UnityObjectToClipPos(v.vertex);o.uv=v.uv;return o;}
 fixed4 LoopSample(sampler2D artwork,float2 phase,float2 grid){
  // Repeat with an overlapping strip at each edge. The wrap samples the same
  // pixels on both sides, avoiding a hard cut even when the artwork edges differ.
  float2 distance=(floor(phase*(1-_Overlap)*grid)+.5)/grid;
  float2 uv=distance;
  float2 blend=1-smoothstep(0,_Overlap,distance);
  float2 other=distance+1-_Overlap;
  fixed4 a=tex2D(artwork,uv);
  fixed4 b=tex2D(artwork,float2(other.x,uv.y));
  fixed4 c=tex2D(artwork,float2(uv.x,other.y));
  fixed4 d=tex2D(artwork,other);
  fixed4 color=lerp(lerp(a,b,blend.x),lerp(c,d,blend.x),blend.y);
  color.rgb=floor(color.rgb*31+.5)/31;return color;
 }
 fixed4 frag(output i):SV_Target {
  float2 phase=frac(i.uv*_RepeatSpan.xy+_Scroll.xy);
  fixed4 present=LoopSample(_MainTex,phase,_PixelGrid.xy);
  if(_PastAmount<=0)return present;
  fixed4 past;
  if(_HasPastBackground>.5)past=LoopSample(_PastTex,phase,_PastPixelGrid.xy);
  else {
   // Temporary treatment until the past scenery is chosen. Do not recolor
   // replacement artwork or change the present image, camera, or loop phase.
   float luminance=dot(present.rgb,float3(.299,.587,.114));
   past=fixed4(saturate(lerp(luminance.xxx,present.rgb,_PastSaturation)*_PastTint.rgb),present.a);
   past.rgb=floor(past.rgb*31+.5)/31;
  }
  return lerp(present,past,saturate(_PastAmount));
 }
 ENDCG }
 }
}
