using UnityEngine;
namespace OneMoreTime {
// A presentation-only stage: never moves the live player or writes memory.
public sealed class AbilityDemo {
 public float Time {get;private set;}
 public MemoryKind Kind {get;}
 public float Duration=>Kind==MemoryKind.Freeze?16:22;
 public bool Finished=>Time>=Duration;
 public AbilityDemo(MemoryKind kind){Kind=kind;}
 public void Tick(float dt){Time=Mathf.Min(Duration,Time+Mathf.Max(0,dt));}
 public void Restart(){Time=0;}
 float Progress(float start,float end)=>Mathf.Clamp01((Time-start)/(end-start));
 void Goat(AnimationLibrary library,float x,float feet,bool ghost=false,bool moving=false){
  Sprite sprite=library.Get(moving?1:0,moving?(int)(Mathf.Max(0,x-190)/44f*AnimationLibrary.RunCycleFrames):(ghost&&Kind==MemoryKind.Freeze?0:(int)(Time*6)));if(sprite==null)return;
  Rect uv=sprite.textureRect;uv=new Rect(uv.x/sprite.texture.width,uv.y/sprite.texture.height,uv.width/sprite.texture.width,uv.height/sprite.texture.height);
  Color old=GUI.color;GUI.color=ghost?new Color(.57f,1,.79f,.7f):Color.white;
  GUI.DrawTextureWithTexCoords(PlayerVisual.SpriteGUIRect(sprite,new Vector2(x,feet),.28f),sprite.texture,uv,true);GUI.color=old;
  if(ghost)GameUI.Panel(new Rect(x-27,feet-49,54,3),GameUI.Mint);
 }
 void Ground(float x,float y,float width){GameUI.Panel(new Rect(x,y,width,400-y),new Color(.25f,.22f,.19f));GameUI.Panel(new Rect(x,y,width,7),GameUI.Mint);}
 public int Draw(AnimationLibrary library){
  GameUI.Shade();GameUI.Frame(new Rect(70,35,820,465));
  GameUI.Text(new Rect(100,55,660,34),Kind==MemoryKind.Freeze?"定格懷錶 · 操作示範":"回溯沙漏 · 操作示範",25);

  GameUI.Panel(new Rect(100,185,760,222),new Color(.07f,.15f,.18f));
  string caption,key;
  if(Kind==MemoryKind.Freeze){
   Ground(115,370,425);Ground(540,248,305);
   float x=190,y=370;
   if(Time<3){x=Mathf.Lerp(190,410,Progress(0,3));caption="走到高台旁，跳到想留下定格的位置";key="D + Space";}
   else if(Time<5){x=410;y=Mathf.Lerp(370,302,Progress(3,4));caption="在半空按 E，留下一隻定格的自己";key="E";}
   else if(Time<8){caption="保存後，你會回到起點；定格留在原地";key="E 已保存";}
   else if(Time<11){float p=Progress(8,11);x=Mathf.Lerp(190,410,p);y=Mathf.Lerp(370,253,p)-Mathf.Sin(p*Mathf.PI)*55;caption="重新靠近，從上方跳到定格背上";key="D + Space";}
   else{float p=Progress(12,15);x=Mathf.Lerp(410,640,p);y=Mathf.Lerp(253,248,p)-Mathf.Sin(p*Mathf.PI)*15;caption="站穩後再跳一次，就能登上高台！";key="D + Space";}
   if(Time>=4)Goat(library,410,302,true);
   Goat(library,x,y,false,Time<3||(Time>=8&&Time<15));
  }else{
   Ground(115,370,730);Ground(640,245,205);
   float x=190,y=370;float echoX=190;
   if(Time<2){caption="按 Q 開始錄製，先等一下方便自己上車";key="Q 開始錄製";}
   else if(Time<6){x=Mathf.Lerp(190,550,Progress(2,6));caption="走一段路；移動、跳躍和等待都會記住";key="D";}
   else if(Time<8){x=550;caption="停一下也會錄進去，再按 Q 保存";key="Q 保存";}
   else if(Time<10){caption="保存後回到起點，軌跡還沒開始播放";key="已保存";}
   else if(Time<12){float p=Progress(10,12);y=370-49*p-Mathf.Sin(p*Mathf.PI)*60;caption="按 Q 播放，再跳上正在等待的殘影";key="Q → Space";}
   else if(Time<18){echoX=Mathf.Lerp(190,550,Progress(12,16));x=echoX;y=321;caption="站在殘影背上，讓過去的自己載你前進";key="搭乘中";}
   else{echoX=550;float p=Progress(18,20);x=Mathf.Lerp(550,730,p);y=Mathf.Lerp(321,245,p)-Mathf.Sin(p*Mathf.PI)*30;caption="到高台旁再跳！回放結束，殘影就會消失";key="D + Space";}
   if(Time>=8){for(int i=0;i<19;i++)GameUI.Panel(new Rect(190+i*20,378,7,3),GameUI.Muted);}
   if(Time>=10&&Time<21)Goat(library,echoX,370,true,Time>=12&&Time<16);
   Goat(library,x,y,false,Time>=2&&Time<6);
   GameUI.Text(new Rect(120,190,420,25),Time<8?$"錄製示意  {Time:F1} / 10 秒":Time<10?"已保存 · 等待你按 Q":Time<21?"播放一次":"播放結束",15,GameUI.Gold);
  }
  GameUI.Text(new Rect(120,112,710,32),caption,19,GameUI.Cream,TextAnchor.MiddleCenter);
  GameUI.Key(new Rect(355,150,250,28),key);
  GameUI.Panel(new Rect(100,414,760,4),GameUI.Border);GameUI.Panel(new Rect(100,414,760*Time/Duration,4),GameUI.Gold);
  if(GameUI.Button(new Rect(100,442,200,36),"重播示範"))return 1;
  if(GameUI.Button(new Rect(625,442,235,36),Finished?"換我試試":"略過示範",true,Finished?"Enter":"Esc"))return 2;
  return 0;
 }
}
}
