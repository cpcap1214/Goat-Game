Shader "OneMoreTime/Water Volume" {
 Properties { _MainTex("Shared water atlas",2D)="white" {} _FramesPerSecond("Frames per second",Float)=4 _PreviewTime("Preview time (-1 uses game clock)",Float)=-1 }
 SubShader { Tags {"Queue"="Transparent" "RenderType"="Transparent"} Cull Off ZWrite Off Blend One OneMinusSrcAlpha
 Pass { CGPROGRAM
 #pragma vertex vert
 #pragma fragment frag
 #include "UnityCG.cginc"
 #include "WaterCommon.cginc"
 struct input {float4 vertex:POSITION;fixed4 color:COLOR;};
 struct output {float4 vertex:SV_POSITION;float2 world:TEXCOORD0;fixed4 color:COLOR;};
 output vert(input v){output o;o.vertex=UnityObjectToClipPos(v.vertex);o.world=mul(unity_ObjectToWorld,v.vertex).xy;o.color=v.color;return o;}
 fixed4 frag(output i):SV_Target {
  float2 cell=(floor(i.world*16)+.5)/16;
  float2 tile=1-abs(frac(cell)*2-1);
  // Sample only the quiet submerged river band; mirror joins on both axes.
  fixed4 c=WaterSample(float2(tile.x,.08+tile.y*.3),0);
  float3 deep=float3(.204,.318,.420);
  #ifndef UNITY_COLORSPACE_GAMMA
  deep=GammaToLinearSpace(deep);
  #endif
  c.rgb=lerp(c.rgb,deep,.65);c.a=1;
  c.rgb*=i.color.rgb*i.color.a;c.a*=i.color.a;return c;
 }
 ENDCG }
 }
}
