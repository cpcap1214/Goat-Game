Shader "OneMoreTime/Looping Water" {
 Properties { _MainTex("Water atlas (4 x 4)",2D)="white" {} _FramesPerSecond("Frames per second",Float)=4 _PreviewTime("Preview time (-1 uses game clock)",Float)=-1 }
 SubShader { Tags {"Queue"="Transparent" "RenderType"="Transparent" "CanUseSpriteAtlas"="False"} Cull Off ZWrite Off Blend One OneMinusSrcAlpha
 Pass { CGPROGRAM
 #pragma vertex vert
 #pragma fragment frag
 #include "UnityCG.cginc"
 #include "WaterCommon.cginc"
 struct input {float4 vertex:POSITION;float2 uv:TEXCOORD0;fixed4 color:COLOR;};
 struct output {float4 vertex:SV_POSITION;float2 uv:TEXCOORD0;fixed4 color:COLOR;};
 output vert(input v){output o;o.vertex=UnityObjectToClipPos(v.vertex);o.uv=v.uv;o.color=v.color;return o;}
 fixed4 frag(output i):SV_Target {
  fixed4 c=WaterSample(frac(i.uv*4),3-floor(i.uv.y*4));
  c.rgb*=i.color.rgb*i.color.a;c.a*=i.color.a;return c;
 }
 ENDCG }
 }
}
