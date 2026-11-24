using System;
using UnityEngine;

namespace OneMoreTime {
[Flags]
public enum CameraModules { None=0, Shutter=1, Replay=2, AdvancedReplay=4, Flashlight=8, PastLens=16 }

// A read-only view of this room's usable equipment. Debug scenes deliberately
// do not inherit campaign flags, and inspecting equipment never grants powers.
public static class CameraEquipment {
 public static CameraModules For(Component owner){
  if(owner==null)return CameraModules.None;
  CameraModules result=CameraModules.None;
  var legacy=owner.GetComponent<RunManager>();
  if(legacy!=null){if(legacy.FreezeUnlocked)result|=CameraModules.Shutter;if(legacy.TraceUnlocked)result|=CameraModules.Replay;return result;}
  var foothills=owner.GetComponent<NostalgiaChapter>();
  if(foothills!=null){if(foothills.Unlocked)result|=CameraModules.PastLens;if(foothills.memories!=null&&foothills.memories.Unlocked)result|=CameraModules.Shutter|CameraModules.Replay;return result;}
  var main=owner.GetComponent<MainRoom>();
  if(main!=null&&main.shutter!=null&&main.shutter.Unlocked)result|=CameraModules.Shutter;
  var replay=owner.GetComponent<RewindPlaytest>();
  var tutorial=owner.GetComponent<Room4AbilityTutorial>();
  var chapter=owner.GetComponent<ChapterOneRoom>();
  if(replay!=null&&(tutorial==null||tutorial.Unlocked)&&(chapter==null||chapter.ReplayAvailable)){
   result|=CameraModules.Replay;
   if(replay.ReplayUpgradeUnlocked&&(main==null||MainGameProgress.IsDebugMode))result|=CameraModules.AdvancedReplay;
  }
  if(chapter!=null&&chapter.FilterAvailable)result|=CameraModules.PastLens;
  var flashlight=owner.GetComponent<ChapterTwoFlashlightPlaytest>();
  var light=flashlight!=null?flashlight.flashlight:owner.GetComponent<FlashlightAbility>();
  if(light!=null&&light.unlocked&&(main==null||MainGameProgress.IsDebugMode))result|=CameraModules.Flashlight;
  return result;
 }

 static Texture2D camera,tape,arcaneTape,flash,lens;
 static void Load(){
  if(camera==null)camera=Resources.Load<Texture2D>("AbilityArt/ShutterCamera");
  if(tape==null)tape=Resources.Load<Texture2D>("AbilityArt/ReplayTape");
  if(arcaneTape==null)arcaneTape=Resources.Load<Texture2D>("AbilityArt/ArcaneReplayTape");
  if(flash==null)flash=Resources.Load<Texture2D>("AbilityArt/FlashModule");
  if(lens==null)lens=Resources.Load<Texture2D>("AbilityArt/PastLens");
 }
 public static void Draw(Rect area,Component owner)=>Draw(area,For(owner));
 public static void Draw(Rect area,CameraModules modules){
  Load();
  // All parts share a 136 x 88 assembly canvas with physical mounting points.
  // Crop transparent source padding so upgrades meet the body instead of floating.
  // Every piece represents an acquired ability, including the shutter body.
  var old=GUI.color;GUI.color=Color.white;
  bool shutter=(modules&CameraModules.Shutter)!=0;
  bool recording=(modules&(CameraModules.Replay|CameraModules.AdvancedReplay))!=0;
  bool advanced=(modules&CameraModules.AdvancedReplay)!=0;
  // Both cassette variants occupy the same side socket; the connector slips
  // behind the camera edge, leaving the two reels visible.
  if(shutter&&recording)Socket(area,new Rect(42,53,9,7));
  if(shutter&&(modules&CameraModules.Flashlight)!=0)Socket(area,new Rect(83,22,7,8));
  if(recording)Part(area,advanced?arcaneTape:tape,new Rect(0,44,48,31),advanced?new Rect(176f/1312,437f/1199,1015f/1312,654f/1199):new Rect(240f/1536,171f/1024,1141f/1536,702f/1024));
  if(shutter)Part(area,camera,new Rect(44,22,84,65),new Rect(120f/1254,305f/1254,1015f/1254,785f/1254));
  if((modules&CameraModules.Flashlight)!=0)Part(area,flash,new Rect(73,0,27,27),new Rect(194f/1254,222f/1254,863f/1254,870f/1254));
  if((modules&CameraModules.PastLens)!=0)Part(area,lens,new Rect(67,36,41,42),new Rect(180f/1254,183f/1254,894f/1254,912f/1254));
  GUI.color=old;
 }
 static void Socket(Rect area,Rect local){
  GUI.color=new Color(.48f,.50f,.52f);
  GUI.DrawTexture(new Rect(Mathf.Round(area.x+local.x*area.width/136),Mathf.Round(area.y+local.y*area.height/88),Mathf.Round(local.width*area.width/136),Mathf.Round(local.height*area.height/88)),Texture2D.whiteTexture);
  GUI.color=Color.white;
 }
 // source is normalized, measured from the artwork's top-left corner.
 static void Part(Rect area,Texture2D texture,Rect local,Rect source){
  if(texture==null)return;
  var rect=new Rect(Mathf.Round(area.x+local.x*area.width/136),Mathf.Round(area.y+local.y*area.height/88),Mathf.Round(local.width*area.width/136),Mathf.Round(local.height*area.height/88));
  GUI.DrawTextureWithTexCoords(rect,texture,new Rect(source.x,1-source.yMax,source.width,source.height),true);
 }
}
}
