using UnityEngine;
namespace OneMoreTime {
public sealed class EndingCinematic {
 readonly EndingAssets assets;
 readonly Texture2D revealShade;
 public EndingSequence Sequence{get;}=new EndingSequence();
 public EndingCinematic(){
  assets=Resources.Load<EndingAssets>("Ending/EndingAssets");
  // One filtered surface avoids seams between adjacent translucent GUI panels.
  revealShade=new Texture2D(256,1,TextureFormat.RGBA32,false){filterMode=FilterMode.Bilinear,wrapMode=TextureWrapMode.Clamp};
  var pixels=new Color[256];
  for(int i=0;i<pixels.Length;i++)pixels[i]=new Color(.02f,.08f,.12f,.35f*(1-i/255f));
  revealShade.SetPixels(pixels);revealShade.Apply(false,true);
 }
 public void Dispose(){if(revealShade!=null)Object.Destroy(revealShade);}
 public void Draw(){
  if(assets==null||assets.background==null)return;
  GUI.DrawTexture(new Rect(0,0,960,540),assets.background,ScaleMode.StretchToFill);
  float t=Sequence.Elapsed;
  float doorX=assets.doorway.x*960,feet=assets.doorway.y*540;
  // Ambient lights rise only after the goat has entered the tower.
  for(int i=0;i<12;i++){
   float p=Mathf.Repeat(t*.17f+i*.0833f,1);float x=doorX+Mathf.Sin(i*2.4f+t)*38;
   float alpha=Mathf.Sin(p*Mathf.PI)*Mathf.Clamp01(t-8)*.5f;
   GameUI.Panel(new Rect(x,feet-p*140,3,3),new Color(.9f,1,.65f,alpha));
  }
  if(Sequence.Enter<1&&assets.goat!=null){
   float enter=Sequence.Enter;float x=Mathf.Lerp(80,doorX,Sequence.Walk);
   float size=Mathf.Lerp(78,48,enter);float y=feet-enter*9;
   bool moving=t<6||t>7;
   // Keep stride phase tied to travel, including the eased approach to the door.
   int frame=moving?(int)(((x-80)/64f+enter*.75f)*AnimationLibrary.RunCycleFrames):(int)((t-6)*6);
   Sprite sprite=assets.goat.Get(moving?1:0,frame);
   if(sprite!=null){var r=sprite.textureRect;var uv=new Rect(r.x/sprite.texture.width,r.y/sprite.texture.height,r.width/sprite.texture.width,r.height/sprite.texture.height);
    var old=GUI.color;GUI.color=new Color(1,1,1,1-enter);GUI.DrawTextureWithTexCoords(PlayerVisual.SpriteGUIRect(sprite,new Vector2(x,y),size/256f),sprite.texture,uv,true);GUI.color=old;}
  }
  if(Sequence.Reveal>0){
   var old=GUI.color;GUI.color=new Color(1,1,1,Sequence.Reveal);
   GUI.DrawTexture(new Rect(0,0,480,540),revealShade,ScaleMode.StretchToFill,true);
   GameUI.Text(new Rect(60,170,410,80),"恭喜通關！",48,GameUI.Cream,TextAnchor.MiddleLeft);GUI.color=old;
  }
  if(!Sequence.Complete&&GameUI.Button(new Rect(706,28,214,36),"略過動畫",false,"Esc"))Sequence.Skip();
 }
}
}
