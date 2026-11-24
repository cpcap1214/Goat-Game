#ifndef OMT_WATER_COMMON
#define OMT_WATER_COMMON
sampler2D _MainTex;
float _FramesPerSecond;
float _PreviewTime;
// All water uses the same four-frame clock, palette and 16-pixel tile grid.
fixed4 WaterPalette(fixed4 c) {
 float3 source=c.rgb;
 // Unity samples an sRGB texture in linear space. Compare against palette
 // swatches in their original sRGB space, then convert the chosen swatch once.
 #ifndef UNITY_COLORSPACE_GAMMA
 source=LinearToGammaSpace(source);
 #endif
 float3 palette[6]={float3(.204,.318,.420),float3(.278,.498,.612),float3(.396,.655,.718),float3(.580,.784,.776),float3(.890,.902,.753),float3(.188,.294,.349)};
 float distance=100;float3 chosen=palette[0];
 for(int k=0;k<6;k++){float d=dot(source-palette[k],source-palette[k]);if(d<distance){distance=d;chosen=palette[k];}}
 #ifndef UNITY_COLORSPACE_GAMMA
 chosen=GammaToLinearSpace(chosen);
 #endif
 c.rgb=chosen*c.a;return c;
}
fixed4 WaterFrame(float2 local, float row, float frame) {
 local=(floor(saturate(local)*15.999)+.5)/16;
 fixed4 c=tex2D(_MainTex,float2((frame+local.x)/4,(3-row+local.y)/4));
 // Reject stray semi-transparent matte pixels around the generated silhouette.
 c.a=step(.5,c.a);
 return WaterPalette(c);
}
fixed4 WaterSample(float2 local,float row) {
 float phase=(_PreviewTime>=0?_PreviewTime:_Time.y)*max(.01,_FramesPerSecond);
 float frame=fmod(floor(phase),4),next=fmod(frame+1,4);
 return lerp(WaterFrame(local,row,frame),WaterFrame(local,row,next),smoothstep(0,1,frac(phase)));
}
#endif
