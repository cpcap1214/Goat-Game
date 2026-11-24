Shader "OneMoreTime/Floating Submersion" {
 Properties {
  [PerRendererData] _MainTex("Sprite",2D)="white" {}
  _Color("Tint",Color)=(1,1,1,1)
  [MaterialToggle] PixelSnap("Pixel snap",Float)=0
  [HideInInspector] _RendererColor("Renderer color",Color)=(1,1,1,1)
  [HideInInspector] _Flip("Flip",Vector)=(1,1,1,1)
  [PerRendererData] _AlphaTex("External alpha",2D)="white" {}
  [PerRendererData] _EnableExternalAlpha("Enable external alpha",Float)=0
  _WaterlineY("Water surface in world units",Float)=0
  _UsePixelGrid("Preserve native pixel grid",Float)=0
  _PixelGrid("Native pixel grid",Vector)=(64,32,0,0)
 }
 SubShader {
  Tags {"Queue"="Transparent" "RenderType"="Transparent" "IgnoreProjector"="True" "CanUseSpriteAtlas"="True" "PreviewType"="Plane"}
  Cull Off Lighting Off ZWrite Off Blend One OneMinusSrcAlpha
  Pass {
   CGPROGRAM
   #pragma vertex vert
   #pragma fragment frag
   #pragma target 2.0
   #pragma multi_compile_instancing
   #pragma multi_compile_local _ PIXELSNAP_ON
   #pragma multi_compile _ ETC1_EXTERNAL_ALPHA
   #include "UnitySprites.cginc"
   float _WaterlineY,_UsePixelGrid;float4 _PixelGrid;
   struct output {
    float4 vertex:SV_POSITION;
    fixed4 color:COLOR;
    float2 uv:TEXCOORD0;
    float worldY:TEXCOORD1;
    UNITY_VERTEX_OUTPUT_STEREO
   };
   output vert(appdata_t v){
    output o;
    UNITY_SETUP_INSTANCE_ID(v);
    UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
    v.vertex=UnityFlipSprite(v.vertex,_Flip);
    o.vertex=UnityObjectToClipPos(v.vertex);
    o.worldY=mul(unity_ObjectToWorld,v.vertex).y;
    o.uv=v.texcoord;o.color=v.color*_Color*_RendererColor;
    #ifdef PIXELSNAP_ON
    o.vertex=UnityPixelSnap(o.vertex);
    #endif
    return o;
   }
   fixed4 frag(output i):SV_Target {
    // The physics object keeps falling to its pooled disposal depth. Only the
    // pixels below this fixed world-space waterline disappear as it submerges.
    clip(i.worldY-_WaterlineY);
    float2 uv=i.uv;
    if(_UsePixelGrid>.5)uv=(floor(uv*_PixelGrid.xy)+.5)/_PixelGrid.xy;
    fixed4 c=SampleSpriteTexture(uv)*i.color;c.rgb*=c.a;return c;
   }
   ENDCG
  }
 }
}
