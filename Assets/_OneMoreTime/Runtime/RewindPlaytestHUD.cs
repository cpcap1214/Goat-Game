using UnityEngine;
namespace OneMoreTime {
// Code-drawn film perforations, grain, charge ring and read-only video timeline.
public sealed class RewindPlaytestHUD {
 Texture2D grain,disc;float filmTime,filmEntry;bool wasRecording;
 public void Dispose(){if(grain!=null)Object.Destroy(grain);if(disc!=null)Object.Destroy(disc);}
 void Textures(){
  if(grain!=null)return;
  grain=new Texture2D(64,64,TextureFormat.RGBA32,false){wrapMode=TextureWrapMode.Repeat,filterMode=FilterMode.Point};uint seed=1709;
  for(int y=0;y<64;y++)for(int x=0;x<64;x++){seed=1664525*seed+1013904223;float v=(seed>>24)/255f;grain.SetPixel(x,y,new Color(v,v,v,.065f));}grain.Apply();
  disc=new Texture2D(48,48,TextureFormat.RGBA32,false){filterMode=FilterMode.Bilinear};
  for(int y=0;y<48;y++)for(int x=0;x<48;x++)disc.SetPixel(x,y,new Color(1,1,1,Mathf.Clamp01(23.5f-Vector2.Distance(new Vector2(x,y),new Vector2(23.5f,23.5f)))));disc.Apply();
 }
 public void Draw(RewindPlaytest room){
  Textures();if(Event.current.type==EventType.Repaint&&!room.Paused)filmTime+=Time.unscaledDeltaTime;
  GameUI.Begin(room.font);
  bool recording=room.State==RewindPlaytest.Mode.Recording;
  if(recording&&!wasRecording)filmEntry=0;
  if(recording&&Event.current.type==EventType.Repaint&&!room.Paused)filmEntry=Mathf.Min(1,filmEntry+Time.unscaledDeltaTime/.16f);
  wasRecording=recording;
  if(recording)Film();
  if(room.Memory!=null)Timeline(room);
  if(room.Charging){
   var screen=room.view.WorldToScreenPoint(room.player.Position+Vector2.up*(room.player.tuning.height+.65f));
   if(screen.z>0&&room.view.pixelRect.Contains(new Vector2(screen.x,screen.y))){var p=GUI.matrix.inverse.MultiplyPoint3x4(new Vector3(screen.x,Screen.height-screen.y,0));Ring(new Vector2(p.x,p.y),room.HoldProgress);}
  }
  if(room.Paused&&room.Controls.IsOpen){room.Controls.Draw();return;}
  if(room.Paused){GameUI.Shade();GameUI.Frame(new Rect(300,65,360,425));GameUI.Text(new Rect(330,87,300,40),room.pauseTitle,26);if(GameUI.Button(new Rect(330,150,300,45),"Resume",true))room.SetPaused(false);if(GameUI.Button(new Rect(330,207,300,45),"Options / Key Bindings"))room.Controls.Open();if(GameUI.Button(new Rect(330,264,300,45),"Reset"))room.ResetFromPause();if(GameUI.Button(new Rect(330,321,300,45),"Back to main menu"))SceneFlow.Load(0);
   if(room.Paused){CameraEquipment.Draw(new Rect(314,390,136,88),room);if(MainGameProgress.IsMainJourney&&room.GetComponent<MainRoom>()!=null)GameUI.PauseDeathCounter(new Vector2(642,476),MainGameProgress.DeathCount);}
  }
 }
 void Film(){
  // Warm photographic tint, fine grain and soft edge shading over the game only.
  GameUI.Panel(new Rect(0,0,960,540),new Color(.58f,.36f,.13f,.15f*filmEntry));
  var color=GUI.color;GUI.color=new Color(1,1,1,filmEntry);GUI.DrawTextureWithTexCoords(new Rect(0,0,960,540),grain,new Rect(Mathf.Floor(filmTime*12)*.173f,filmTime*.07f,15,8));GUI.color=color;
  for(int i=0;i<12;i++){float a=.018f*(1-i/12f);GameUI.Panel(new Rect(i*5,42,5,456),new Color(.12f,.08f,.04f,a));GameUI.Panel(new Rect(955-i*5,42,5,456),new Color(.12f,.08f,.04f,a));}
  Strip(0);Strip(498);
 }
 void Strip(float y){
  // A soft leading edge sweeps left-to-right in 0.16 seconds; holes run 6x faster.
  float front=Mathf.SmoothStep(0,1,filmEntry)*1080;
  if(filmEntry>=1)GameUI.Panel(new Rect(0,y,960,42),new Color(.035f,.032f,.026f,.98f));
  else for(int x=0;x<960;x+=8){float a=Mathf.Clamp01((front-x)/120);GameUI.Panel(new Rect(x,y,8,42),new Color(.035f,.032f,.026f,.98f*a));}
  float offset=filmTime*180%40;
  for(float x=-40+offset;x<960;x+=40){float a=Mathf.Clamp01((front-x)/120);GameUI.Panel(new Rect(x,y+12,20,18),new Color(.72f,.68f,.54f,.82f*a));GameUI.Panel(new Rect(x+3,y+15,14,12),new Color(.9f,.86f,.71f,.7f*a));}
 }
 void Ring(Vector2 center,float progress){
  var color=GUI.color;GUI.color=new Color(.02f,.06f,.07f,.85f);GUI.DrawTexture(new Rect(center.x-20,center.y-20,40,40),disc);GUI.color=color;
  for(int i=0;i<64;i++){
   float angle=i*Mathf.PI*2/64-Mathf.PI/2;var point=center+new Vector2(Mathf.Cos(angle),Mathf.Sin(angle))*17;
   GUI.color=i/64f<progress?GameUI.Gold:new Color(.5f,.65f,.63f,.3f);GUI.DrawTexture(new Rect(point.x-2,point.y-2,4,4),disc);
  }
  GUI.color=color;
  string recordKey=KeyBindings.Label(GameAction.RecordReplay);
  int size=Mathf.Clamp(Mathf.FloorToInt(30f/(Mathf.Max(1,recordKey.Length)*.6f)),6,13);
  GameUI.Text(new Rect(center.x-16,center.y-10,32,20),recordKey,size,GameUI.Cream,TextAnchor.MiddleCenter);
 }
 void Timeline(RewindPlaytest room){
  bool reverse=room.State==RewindPlaytest.Mode.Rewinding;float duration=room.Memory.Duration;float time=reverse?room.RewindSeconds:room.PlaybackSeconds;float progress=reverse?Mathf.Clamp01(time/duration):room.PlaybackProgress;
  GameUI.Panel(new Rect(178,523,604,10),new Color(.35f,.24f,.15f,.65f));
  var warm=RewindPlaytest.ReplayColor;warm.a=.95f;GameUI.Panel(new Rect(178,523,604*progress,10),warm);
  float head=178+604*progress;
  var color=GUI.color;
  GUI.color=new Color(.25f,.13f,.06f,.9f);GUI.DrawTexture(new Rect(head-9,519,18,18),disc);
  GUI.color=new Color(1f,.88f,.66f);GUI.DrawTexture(new Rect(head-7,521,14,14),disc);
  GUI.color=color;
 }
}
}
