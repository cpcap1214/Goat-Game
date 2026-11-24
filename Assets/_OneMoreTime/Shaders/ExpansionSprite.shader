Shader "OneMoreTime/Expansion Sprite" {
 Properties { _MainTex("Sprite",2D)="white" {} _PixelGrid("Native pixel grid",Vector)=(64,64,0,0) }
 SubShader { Tags {"Queue"="Transparent" "RenderType"="Transparent" "CanUseSpriteAtlas"="False"} Cull Off ZWrite Off Blend One OneMinusSrcAlpha
 Pass { CGPROGRAM
 #pragma vertex vert
 #pragma fragment frag
 #include "UnityCG.cginc"
 sampler2D _MainTex;float4 _PixelGrid;
 struct input {float4 vertex:POSITION;float2 uv:TEXCOORD0;fixed4 color:COLOR;};
 struct output {float4 vertex:SV_POSITION;float2 uv:TEXCOORD0;fixed4 color:COLOR;};
 output vert(input v){output o;o.vertex=UnityObjectToClipPos(v.vertex);o.uv=v.uv;o.color=v.color;return o;}
 fixed4 frag(output i):SV_Target {float2 uv=(floor(i.uv*_PixelGrid.xy)+.5)/_PixelGrid.xy;fixed4 c=tex2D(_MainTex,uv);c.a=step(.625,c.a);c*=i.color;c.rgb*=c.a;return c;}
 ENDCG }
 }
}
