Shader "OneMoreTime/Chapter Two Obscurity"
{
 Properties
 {
  [PerRendererData] _MainTex ("Sprite texture", 2D) = "white" {}
  _Tint ("Darkness or mist tint", Color) = (.065,.078,.105,1)
  _Opacity ("Opacity", Range(0,1)) = .93
  _Size ("Local coverage", Vector) = (20,12,0,0)
  _EdgeFeather ("Boundary feather", Float) = .5
  _Kind ("0 Darkness / 1 Mist", Float) = 0
  _BeamOrigin ("Flashlight world origin", Vector) = (0,0,0,0)
  _BeamFacing ("Horizontal facing", Float) = 1
  _BeamRange ("Beam range", Float) = 6
  _NearRadius ("Near light radius", Float) = 1.1
  _HalfAngle ("Cone half angle in degrees", Float) = 32
  _Strength ("Flashlight strength", Range(0,1)) = 0
  _Clock ("Room simulation clock", Float) = 0
 }
 SubShader
 {
  Tags { "Queue"="Transparent" "RenderType"="Transparent" "IgnoreProjector"="True" "PreviewType"="Plane" "CanUseSpriteAtlas"="False" "DisableBatching"="True" }
  Cull Off Lighting Off ZWrite Off
  Blend One OneMinusSrcAlpha
  Pass
  {
   CGPROGRAM
   #pragma vertex vert
   #pragma fragment frag
   #pragma target 2.0
   #include "UnityCG.cginc"
   struct input { float4 vertex:POSITION; };
   struct output { float4 vertex:SV_POSITION; float2 world:TEXCOORD0; float2 local:TEXCOORD1; };
   float4 _Tint, _Size, _BeamOrigin;
   float _Opacity, _EdgeFeather, _Kind, _BeamFacing, _BeamRange, _NearRadius, _HalfAngle, _Strength, _Clock;

   output vert(input v) {
    output o; o.vertex = UnityObjectToClipPos(v.vertex);
    o.world = mul(unity_ObjectToWorld, v.vertex).xy; o.local = v.vertex.xy;
    return o;
   }

   float hash(float2 p) { return frac(sin(dot(p, float2(127.1,311.7))) * 43758.5453); }
   float noise(float2 p) {
    float2 cell = floor(p), f = frac(p); f = f*f*(3-2*f);
    return lerp(lerp(hash(cell), hash(cell+float2(1,0)), f.x), lerp(hash(cell+float2(0,1)), hash(cell+1), f.x), f.y);
   }
   // Same alpha-density calculation as ChapterTwoObscurity.MistDensity.
   float mistDensity(float2 world) {
    float warp = sin(world.x*.47+_Clock*.12)*1.25
     + sin(world.x*.19-world.y*.32-_Clock*.09)*.7;
    float ribbon = .5+.5*sin(world.y*1.7+warp-_Clock*.2);
    float curl = .5+.5*sin(world.y*.8-world.x*.17
     + sin(world.x*.25+world.y*.21+_Clock*.08)*1.3+_Clock*.12);
    return .22+.53*ribbon*ribbon+.25*curl*curl*curl;
   }
   // Mirrors FlashlightAbility.ExposureAt. Pixel centers match Chapter 1's 16 PPU art.
   float exposure(float2 world) {
    float2 delta = world-_BeamOrigin.xy;
    float distance = length(delta);
    float nearby = 1-smoothstep(_NearRadius*.65, _NearRadius, distance);
    float forward = delta.x*_BeamFacing/max(distance,.00001);
    float cone = smoothstep(cos(radians(_HalfAngle)), cos(radians(_HalfAngle*.75)), forward);
    float reach = 1-smoothstep(_BeamRange*.78, _BeamRange, distance);
    return max(nearby, cone*reach)*_Strength;
   }
   fixed4 frag(output i):SV_Target {
    float2 world = (floor(i.world*16)+.5)/16;
    float clear = exposure(world);
    float2 boundary = _Size.xy*.5-abs(i.local);
    float edgeDistance = min(boundary.x,boundary.y);
    float edge = _EdgeFeather > .00001 ? smoothstep(0,_EdgeFeather,edgeDistance) : step(0,edgeDistance);
    float density = mistDensity(world);
    float alpha = saturate(_Opacity)*_Tint.a*edge*(1-clear)*lerp(1,density,saturate(_Kind));

    // Preserve the quiet darkness grain. Mist uses smooth, translucent ribbons
    // instead of a uniform gray sheet or small-scale visual noise.
    float drift = _Clock*.07;
    float broad = noise(world*float2(.22,.36)+float2(drift,-drift*.3));
    float folds = noise(world*float2(.6,.8)+float2(-drift*.45,drift*.4)+broad*.7);
    float grain = floor((broad*.7+folds*.3)*7)/7;
    float variation = lerp(.94+grain*.06, .88+density*.14, saturate(_Kind));
    float3 color = _Tint.rgb*variation;
    float3 warm = float3(.31,.25,.16);
    #ifndef UNITY_COLORSPACE_GAMMA
    warm = GammaToLinearSpace(warm);
    #endif
    // A restrained warm rim reads as lamplight; the center is genuinely transparent.
    color = lerp(color,warm,clear*(1-clear)*.55);
    return fixed4(color*alpha,alpha);
   }
   ENDCG
  }
 }
}
