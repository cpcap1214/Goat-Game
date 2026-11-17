using UnityEngine;
namespace OneMoreTime {
// Presentation only: era collision changes remain immediate and safe for midair switches.
public sealed class PastLensEffect {
 public const float Duration=.36f;
 public float Remaining{get;private set;}
 public float PastAmount{get;private set;}
 bool past;Texture2D glass,patina;
 public void Switch(bool toPast){past=toPast;Remaining=Duration;}
 public void Reset(){past=false;Remaining=0;PastAmount=0;}
 public void Tick(float dt){Remaining=Mathf.Max(0,Remaining-dt);PastAmount=Mathf.MoveTowards(PastAmount,past?1:0,dt/Duration);}
 public void Dispose(){if(glass!=null)Object.Destroy(glass);if(patina!=null)Object.Destroy(patina);}
 void Textures(){
  if(glass!=null)return;
  glass=new Texture2D(256,256,TextureFormat.RGBA32,false){filterMode=FilterMode.Bilinear,wrapMode=TextureWrapMode.Clamp};
  patina=new Texture2D(256,144,TextureFormat.RGBA32,false){filterMode=FilterMode.Bilinear,wrapMode=TextureWrapMode.Repeat};
  for(int y=0;y<256;y++)for(int x=0;x<256;x++){
   float r=Vector2.Distance(new Vector2(x,y),new Vector2(127.5f,127.5f))/126;
   float rim=Mathf.Clamp01(1-Mathf.Abs(r-.96f)/.025f);float edge=1-Mathf.SmoothStep(0,1,Mathf.InverseLerp(.98f,1,r));
   glass.SetPixel(x,y,new Color(.8f,.7f,.43f,edge*(.065f+rim*.65f)));
  }glass.Apply();
  uint seed=731;
  for(int y=0;y<144;y++)for(int x=0;x<256;x++){
   seed=1664525*seed+1013904223;float noise=(seed>>24)/255f;
   float edge=Mathf.Clamp01((new Vector2((x-127.5f)/127.5f,(y-71.5f)/71.5f).magnitude-.5f)/.9f);
   patina.SetPixel(x,y,new Color(.25f,.17f,.10f,.012f+noise*.022f+edge*edge*.11f));
  }patina.Apply();
 }
 public void Draw(){
  if(PastAmount<=0&&Remaining<=0)return;Textures();var old=GUI.color;
  // Warm paper tint plus restrained grain/edge falloff, drawn beneath the HUD.
  GameUI.Panel(new Rect(0,0,960,503),new Color(.78f,.59f,.32f,.085f*PastAmount));
  GUI.color=new Color(1,1,1,PastAmount);GUI.DrawTexture(new Rect(0,0,960,503),patina);
  if(Remaining>0){
   float t=1-Remaining/Duration;float eased=Mathf.SmoothStep(0,1,t);float size=Mathf.Lerp(past?180:1250,past?1250:180,eased);
   GUI.BeginGroup(new Rect(0,0,960,503));GUI.color=new Color(1,1,1,Mathf.Sin(t*Mathf.PI));
   GUI.DrawTexture(new Rect(480-size*.5f,251.5f-size*.5f,size,size),glass);GUI.EndGroup();
  }
  GUI.color=old;
 }
}
}
