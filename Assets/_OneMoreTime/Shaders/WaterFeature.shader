Shader "OneMoreTime/Water Feature" {
 Properties {
  _MainTex("Shared water atlas",2D)="white" {}
  _FramesPerSecond("Frames per second",Float)=4
  _PreviewTime("Preview time (-1 uses game clock)",Float)=-1
  _Size("World size",Vector)=(6,12,0,0)
  _Shape("Waterfall / river / pool / diagonal / mist",Float)=0
  _Thickness("Diagonal stream thickness",Float)=1
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
   float4 _Size; float _Shape, _Thickness;
   struct input { float4 vertex:POSITION; float2 uv:TEXCOORD0; fixed4 color:COLOR; };
   struct output { float4 vertex:SV_POSITION; float2 uv:TEXCOORD0; fixed4 color:COLOR; };
   output vert(input v) { output o; o.vertex=UnityObjectToClipPos(v.vertex);o.uv=v.uv;o.color=v.color;return o; }
   float RepeatX(float x) { return fmod(floor(x),2)<1?frac(x):1-frac(x); }
   fixed4 Body(float2 p) {
    // Keep a common animation clock while staggering each ribbon in space.
    // Continuous offsets share values at tile edges, avoiding aligned diamond rows.
    float offset=sin(p.x*1.17)*.47+sin(p.x*3.41)*.13;
    float length=.52+.065*sin(p.x*.79);
    float2 uv=float2(RepeatX(p.x),frac(p.y*length+offset));
    fixed4 c=WaterSample(uv,2);
    // Both ends meet the same color: repeating a tall fall never exposes a horizontal cut.
    fixed4 join=(WaterSample(float2(uv.x,.03125),2)+WaterSample(float2(uv.x,.96875),2))*.5;
    return lerp(c,join,smoothstep(.34,.47,abs(uv.y-.5)));
   }
   fixed4 Surface(float2 p,float height) {
    float surfaceY=p.y-height+.75;
    fixed4 top=WaterSample(float2(RepeatX(p.x),saturate(surfaceY/.75)),0);
    fixed4 fill=WaterSample(float2(RepeatX(p.x+floor(p.y)*.31),.07+frac(p.y)*.13),0);
    float3 deep=float3(.204,.318,.420);
    #ifndef UNITY_COLORSPACE_GAMMA
    deep=GammaToLinearSpace(deep);
    #endif
    // Deep water only borrows subtle ripples; repeating the bright surface
    // detail underwater makes a pool look like a stack of separate shelves.
    fill.rgb=lerp(deep*fill.a,fill.rgb,.35);
    return surfaceY>=0?top:fill;
   }
   fixed4 frag(output i):SV_Target {
    float2 size=max(_Size.xy,.25);
    float2 p=(floor(saturate(i.uv)*size*16)+.5)/16;
    fixed4 c;
    if(_Shape<.5) {
     c=Body(p);
     float lip=min(1,size.y*.25),foam=min(.75,size.y*.25);
     if(p.y>size.y-lip) c=WaterSample(float2(RepeatX(p.x),(p.y-size.y+lip)/lip),1);
     if(p.y<foam) {
      fixed4 splash=WaterSample(float2(RepeatX(p.x),p.y/foam),3);
      c=splash+c*(1-splash.a);
     }
     // Outer edges have a one-pixel irregular silhouette; interior tiles stay joined.
     float edge=.0625*(1+step(.65,frac(p.y*.73)));
     float mask=step(edge,p.x)*step(edge,size.x-p.x);
     c*=mask;
    } else if(_Shape<2.5) {
     c=Surface(p,size.y);
    } else if(_Shape<3.5) {
     float slope=(size.y-_Thickness)/max(.25,size.x);
     float band=p.y-((size.y-_Thickness)-p.x*slope);
     float width=max(.25,_Thickness);
     c=WaterSample(float2(RepeatX(p.x),saturate(band/width)),0);
     c*=step(0,band)*step(band,width);
    } else {
     // Isolate bright foam as scattered droplets. Never repeat the opaque
     // submerged part of a splash across a wide rectangular mist overlay.
     float2 uv=float2(RepeatX(p.x),saturate(i.uv.y*.6+.36+.08*sin(p.x*2.3)));
     c=WaterSample(uv,3);
     float3 unpremultiplied=c.rgb/max(.001,c.a);
     #ifndef UNITY_COLORSPACE_GAMMA
     unpremultiplied=LinearToGammaSpace(unpremultiplied);
     #endif
     float pale=step(.82,max(unpremultiplied.r,max(unpremultiplied.g,unpremultiplied.b)));
     float2 cell=floor(p*16);
     float noise=frac(sin(dot(cell,float2(12.9898,78.233)))*43758.5453);
     float scatter=step(.48+i.uv.y*.3,noise);
     float fade=smoothstep(0,.12,i.uv.y)*(1-i.uv.y)*(1-i.uv.y);
     fade*=smoothstep(0,.15,i.uv.x)*smoothstep(0,.15,1-i.uv.x);
     c*=pale*scatter*fade*.8;
    }
    c.rgb*=i.color.rgb*i.color.a;c.a*=i.color.a;return c;
   }
   ENDCG
  }
 }
}
