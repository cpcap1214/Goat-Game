using UnityEngine;
namespace OneMoreTime {
// Presentation only: era collision changes remain immediate and safe for midair switches.
public sealed class PastLensEffect {
 public const float Duration=.36f;
 public float Remaining{get;private set;}
 public float PastAmount{get;private set;}
 bool past;Texture2D glass,patina;
 public void Switch(bool toPast){
  // Reversing a switch midway follows the same path back, without teleporting
  // the lens to a different corner or delaying the actual terrain change.
  float progress=past?1-Remaining/Duration:Remaining/Duration;
  past=toPast;Remaining=(past?1-progress:progress)*Duration;
 }
 public void Reset(){past=false;Remaining=0;PastAmount=0;}
 public void Tick(float dt){Remaining=Mathf.Max(0,Remaining-dt);PastAmount=Mathf.MoveTowards(PastAmount,past?1:0,dt/Duration);}
 public void Dispose(){if(glass!=null)Object.Destroy(glass);if(patina!=null)Object.Destroy(patina);}
 void Textures(){
  if(glass!=null)return;
  glass=new Texture2D(256,256,TextureFormat.RGBA32,false){filterMode=FilterMode.Bilinear,wrapMode=TextureWrapMode.Clamp};
  patina=new Texture2D(256,144,TextureFormat.RGBA32,false){filterMode=FilterMode.Bilinear,wrapMode=TextureWrapMode.Repeat};
  for(int y=0;y<256;y++)for(int x=0;x<256;x++){
   var p=(new Vector2(x,y)-new Vector2(127.5f,127.5f))/126;
   float r=p.magnitude,angle=Mathf.Atan2(p.y,p.x);
   float edge=1-Mathf.SmoothStep(0,1,Mathf.InverseLerp(.985f,1,r));
   float rim=Mathf.SmoothStep(0,1,Mathf.InverseLerp(.9f,.925f,r));
   Color color=Color.Lerp(new Color(.8f,.7f,.43f,.055f),new Color(.31f,.26f,.16f,.86f),rim);
   // An index mark, rim grooves and off-center glass reflection make rotation
   // readable; rotating a perfectly uniform circle would look stationary.
   if(r>.925f&&r<.978f&&Mathf.Cos(angle*24)>.92f)color=new Color(.12f,.11f,.085f,.9f);
   if(r>.91f&&r<.982f&&Mathf.Abs(Mathf.DeltaAngle(angle*Mathf.Rad2Deg,40))<4)color=new Color(.98f,.88f,.62f,.95f);
   if(r<.88f){float glint=Mathf.Clamp01(1-Mathf.Abs(p.x+p.y*.45f+.25f)/.075f)*.13f;color=Color.Lerp(color,new Color(.96f,.91f,.75f,.22f),glint/.13f);}
   color.a*=edge;glass.SetPixel(x,y,color);
  }glass.Apply();
  uint seed=731;
  for(int y=0;y<144;y++)for(int x=0;x<256;x++){
   seed=1664525*seed+1013904223;float noise=(seed>>24)/255f;
   float edge=Mathf.Clamp01((new Vector2((x-127.5f)/127.5f,(y-71.5f)/71.5f).magnitude-.5f)/.9f);
   patina.SetPixel(x,y,new Color(.25f,.17f,.10f,.012f+noise*.022f+edge*edge*.11f));
  }patina.Apply();
 }
 public void Draw(){
  if(PastAmount<=0&&Remaining<=0)return;Textures();var old=GUI.color;var oldMatrix=GUI.matrix;
  // Warm paper tint plus restrained grain/edge falloff, drawn beneath the HUD.
  GameUI.Panel(new Rect(0,0,960,540),new Color(.78f,.59f,.32f,.085f*PastAmount));
  GUI.color=new Color(1,1,1,PastAmount);GUI.DrawTexture(new Rect(0,0,960,540),patina);
  if(Remaining>0){
   float progress=past?1-Remaining/Duration:Remaining/Duration;
   float swing=Mathf.SmoothStep(0,1,Mathf.Clamp01(progress/.62f));
   float zoom=Mathf.SmoothStep(0,1,Mathf.InverseLerp(.62f,1,progress));
   float angle=90*(1-swing),radians=angle*Mathf.Deg2Rad;
   // Orbit from off the upper-left around a hinge above the view. Once seated,
   // enlarge only the lens until its rim passes beyond every gameplay corner.
   var pivot=new Vector2(365,-315);var arm=new Vector2(480,270)-pivot;
   var center=pivot+new Vector2(arm.x*Mathf.Cos(radians)-arm.y*Mathf.Sin(radians),arm.x*Mathf.Sin(radians)+arm.y*Mathf.Cos(radians));
   float size=Mathf.Lerp(420,1250,zoom);
   GUI.BeginGroup(new Rect(0,0,960,540));
   GUI.color=new Color(1,1,1,1-Mathf.SmoothStep(0,1,Mathf.InverseLerp(.84f,1,progress)));
   GUIUtility.RotateAroundPivot(angle,center);
   GUI.DrawTexture(new Rect(center.x-size*.5f,center.y-size*.5f,size,size),glass);
   GUI.matrix=oldMatrix;GUI.EndGroup();
  }
  GUI.matrix=oldMatrix;GUI.color=old;
 }
}
}
