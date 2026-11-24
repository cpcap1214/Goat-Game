Shader "OneMoreTime/Water Accent" {
 Properties {
  _MainTex("Accent atlas (4 x 4)",2D)="white" {}
  _FramesPerSecond("Frames per second",Float)=4
  _PreviewTime("Preview time (-1 uses game clock)",Float)=-1
  _PixelGrid("Native pixel grid",Vector)=(128,128,0,0)
 }
 SubShader {
  Tags { "Queue"="Transparent" "RenderType"="Transparent" "CanUseSpriteAtlas"="False" }
  Cull Off ZWrite Off Blend One OneMinusSrcAlpha
  Pass {
   CGPROGRAM
   #pragma vertex vert
   #pragma fragment frag
   #include "UnityCG.cginc"
   #include "WaterCommon.cginc"
   float4 _PixelGrid;
   struct input {float4 vertex:POSITION;float2 uv:TEXCOORD0;fixed4 color:COLOR;};
   struct output {float4 vertex:SV_POSITION;float2 uv:TEXCOORD0;fixed4 color:COLOR;};
   output vert(input v) {output o;o.vertex=UnityObjectToClipPos(v.vertex);o.uv=v.uv;o.color=v.color;return o;}
   fixed4 Frame(float2 uv,float frame) {
    float2 grid=max(4,_PixelGrid.xy);
    uv=(floor(uv*grid)+.5)/grid;
    uv.x=(frame+frac(uv.x*4))*.25;
    fixed4 c=tex2D(_MainTex,uv);c.a=c.a<.05?0:c.a;return WaterPalette(c);
   }
   fixed4 frag(output i):SV_Target {
    float phase=(_PreviewTime>=0?_PreviewTime:_Time.y)*max(.01,_FramesPerSecond);
    float frame=fmod(floor(phase),4),next=fmod(frame+1,4);
    fixed4 c=lerp(Frame(i.uv,frame),Frame(i.uv,next),smoothstep(0,1,frac(phase)));
    c.rgb*=i.color.rgb*i.color.a;c.a*=i.color.a;return c;
   }
   ENDCG
  }
 }
}
