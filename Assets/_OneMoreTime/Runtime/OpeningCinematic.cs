using UnityEngine;
namespace OneMoreTime {
// Presentation time only: never advances the gameplay simulation or records a memory.
public sealed class OpeningSequence {
 public const float Duration=27;
 public float Elapsed{get;private set;}
 public bool Complete=>Elapsed>=Duration;
 public int Shot=>Elapsed<7?0:Elapsed<13?1:Elapsed<20?2:3;
 public void Tick(float delta){Elapsed=Mathf.Min(Duration,Elapsed+Mathf.Max(0,delta));}
 static readonly float[] starts={0,3.5f,7,10,13,16.5f,20,23.5f};
 static readonly string[] speakers={"主持人","主持人","主持人","觀眾們","小山羊","小山羊","小山羊","小山羊"};
 static readonly string[] lines={
  "各位攝影高手，歡迎來到山羊世界攝影大賽！",
  "今年的挑戰：拍下世界上最美的風景！",
  "冠軍將獲得……大家期待的《GTA7》！",
  "咩——！GTA7！我要把它帶回家！",
  "最美的風景，一定藏在那座傳說中的山頂。",
  "就算是最難攀登的山，我也要拍給大家看！",
  "咦？這座山有魔力……我的相機在發光！",
  "照片竟然變成真的了？帶著這份力量，出發吧！"};
 public int Cue {get{for(int i=starts.Length-1;i>=0;i--)if(Elapsed>=starts[i])return i;return 0;}}
 public float CueElapsed=>Elapsed-starts[Cue];
 public string Speaker=>speakers[Cue];
 public string Line=>lines[Cue];
 public void Next(){Elapsed=Cue+1<starts.Length?starts[Cue+1]:Duration;}
 public void Skip(){Elapsed=Duration;}
}
public sealed class OpeningCinematic {
 public readonly OpeningSequence Sequence=new OpeningSequence();
 readonly Texture2D prize,landscape;
 readonly AnimationLibrary goat;
 static readonly Color Night=new Color(.055f,.055f,.15f),Purple=new Color(.35f,.19f,.56f),Cyan=new Color(.35f,.96f,1);
 public OpeningCinematic(){
  prize=Resources.Load<Texture2D>("Opening/GTA7Prize");
  var a=Resources.Load<EndingAssets>("Ending/EndingAssets");goat=a==null?null:a.goat;
  landscape=new Texture2D(240,135,TextureFormat.RGB24,false){filterMode=FilterMode.Point,wrapMode=TextureWrapMode.Clamp};
  for(int y=0;y<135;y++)for(int x=0;x<240;x++){
   Color c=Color.Lerp(new Color(.84f,.58f,.59f),new Color(.12f,.17f,.37f),y/135f);
   float far=55+25*Mathf.Sin(x*.025f)+12*Mathf.Sin(x*.067f);
   float peak=110-Mathf.Abs(x-163)*.79f;
   if(y<far)c=new Color(.31f,.32f,.49f);
   if(y<peak)c=x<163?new Color(.24f,.25f,.39f):new Color(.16f,.19f,.32f);
   if(y<peak&&y>94+Mathf.Sin(x*.3f)*3)c=new Color(.85f,.86f,.89f);
   if(y<24+9*Mathf.Sin(x*.034f))c=new Color(.10f,.19f,.24f);
   landscape.SetPixel(x,y,c);
  }
  landscape.Apply(false,true);
 }
 public void Dispose(){if(landscape!=null)Object.Destroy(landscape);}
 void Goat(float x,float y,float size,bool walk,float phase=0){
  if(goat==null)return;
  var sprite=goat.Get(walk?1:0,(int)(Sequence.Elapsed*(walk?12:6)+phase));if(sprite==null)return;
  var r=sprite.textureRect;GUI.DrawTextureWithTexCoords(PlayerVisual.SpriteGUIRect(sprite,new Vector2(x,y),size/256f),sprite.texture,new Rect(r.x/sprite.texture.width,r.y/sprite.texture.height,r.width/sprite.texture.width,r.height/sprite.texture.height),true);
 }
 void Camera(float x,float y,float scale,Color glow){
  GameUI.Panel(new Rect(x-23*scale,y-15*scale,46*scale,30*scale),Night);
  GameUI.Panel(new Rect(x-17*scale,y-20*scale,17*scale,7*scale),GameUI.Gold);
  GameUI.Panel(new Rect(x-10*scale,y-10*scale,20*scale,20*scale),glow);
  GameUI.Panel(new Rect(x-6*scale,y-6*scale,12*scale,12*scale),Night);
  GameUI.Panel(new Rect(x+14*scale,y-10*scale,5*scale,5*scale),GameUI.Cream);
 }
 void Prize(float cx,float cy,float height,float spin){
  // A rotating display board: its edge narrows at quarter turns, with a designed reverse face.
  float face=Mathf.Cos(spin),width=height*2/3f*Mathf.Abs(face);
  GameUI.Panel(new Rect(cx-width/2-5,cy-height/2-5,width+10,height+10),GameUI.Gold);
  GameUI.Panel(new Rect(cx-width/2,cy-height/2,width,height),Purple);
  if(face>0&&prize!=null)GUI.DrawTexture(new Rect(cx-width/2,cy-height/2,width,height),prize,ScaleMode.StretchToFill);
  if(face<0&&width>55)GameUI.Text(new Rect(cx-width/2,cy-28,width,56),"GTA7",Mathf.Min(40,(int)(width/3)),GameUI.Gold,TextAnchor.MiddleCenter);
 }
 void Tree(float x,float y,float scale){
  GameUI.Panel(new Rect(x-7*scale,y-70*scale,14*scale,70*scale),new Color(.35f,.24f,.19f));
  GameUI.Panel(new Rect(x-43*scale,y-125*scale,86*scale,65*scale),new Color(.13f,.36f,.29f));
  GameUI.Panel(new Rect(x-29*scale,y-146*scale,58*scale,75*scale),new Color(.22f,.49f,.32f));
  GameUI.Panel(new Rect(x-35*scale,y-119*scale,49*scale,20*scale),new Color(.36f,.61f,.37f));
 }
 void Outdoors(float t){
  GameUI.Panel(new Rect(0,0,960,540),new Color(.47f,.77f,.85f));
  GameUI.Panel(new Rect(784,44,52,52),new Color(1,.90f,.60f));
  for(int i=0;i<7;i++){float x=Mathf.Repeat(i*167-t*5,1100)-140;GameUI.Panel(new Rect(x,48+i%3*30,94,13),new Color(.95f,.96f,.88f));GameUI.Panel(new Rect(x+20,39+i%3*30,48,12),new Color(.95f,.96f,.88f));}
  // Distant wooded hills, then a grassy festival clearing.
  for(int x=0;x<960;x+=8){float y=204+Mathf.Sin(x*.008f)*24;GameUI.Panel(new Rect(x,y,8,220-y),new Color(.37f,.60f,.48f));}
  GameUI.Panel(new Rect(0,220,960,320),new Color(.39f,.59f,.34f));
  Tree(50,270,1.3f);Tree(900,262,1.5f);Tree(135,246,.8f);Tree(818,245,.8f);
  GameUI.Panel(new Rect(200,269,560,38),new Color(.43f,.29f,.21f));
  GameUI.Panel(new Rect(190,264,580,10),new Color(.72f,.52f,.33f));
  for(int i=0;i<14;i++)GameUI.Panel(new Rect(202+i*40,277,2,29),new Color(.28f,.19f,.17f));
  GameUI.Panel(new Rect(182,90,7,180),new Color(.44f,.28f,.19f));GameUI.Panel(new Rect(771,90,7,180),new Color(.44f,.28f,.19f));
  GameUI.Panel(new Rect(182,90,596,57),new Color(.16f,.34f,.32f));
  GameUI.Panel(new Rect(188,96,584,45),new Color(.98f,.91f,.72f));
  GameUI.Text(new Rect(210,100,540,37),"山羊世界攝影大賽",27,GameUI.Ink,TextAnchor.MiddleCenter);
  for(int i=0;i<18;i++){float x=14+i*54,y=43+Mathf.Sin(i*.19f)*16;var color=i%3==0?GameUI.Gold:i%3==1?new Color(.88f,.47f,.43f):new Color(.30f,.65f,.65f);for(int j=0;j<6;j++)GameUI.Panel(new Rect(x+j*2,y+j*3,24-j*4,3),color);}
  for(int i=0;i<26;i++){float x=17+i*37,y=322+(i%3)*21;GameUI.Panel(new Rect(x,y,3,8),new Color(.27f,.43f,.23f));GameUI.Panel(new Rect(x-2,y,7,3),i%2==0?GameUI.Cream:new Color(.93f,.67f,.55f));}
 }
 void Host(float x,float feet,float size){
  Goat(x,feet,size,false);
  // Gold bow tie and a handheld microphone distinguish the host from the audience.
  GameUI.Diamond(x+size*.12f,feet-size*.36f,GameUI.Gold);
  GameUI.Panel(new Rect(x+size*.34f,feet-size*.43f,4,23),Night);
  GameUI.Panel(new Rect(x+size*.34f-3,feet-size*.43f-6,10,11),GameUI.Cream);
 }
 void Dialogue(){
  bool host=Sequence.Cue<3;
  GameUI.Frame(new Rect(30,378,900,140),new Color(.055f,.13f,.16f,.98f));
  GameUI.Panel(new Rect(44,392,109,111),new Color(.17f,.28f,.29f));
  if(host)Host(91,496,126);else Goat(91,496,126,false);
  GameUI.Panel(new Rect(166,364,136,32),host?GameUI.Gold:GameUI.Mint);
  GameUI.Text(new Rect(174,366,120,27),Sequence.Speaker,19,GameUI.Ink,TextAnchor.MiddleCenter);
  int count=Mathf.Min(Sequence.Line.Length,Mathf.FloorToInt(Sequence.CueElapsed*24)+1);
  GameUI.Text(new Rect(177,406,710,64),Sequence.Line.Substring(0,count),24,GameUI.Cream,TextAnchor.UpperLeft);
  GameUI.Text(new Rect(178,482,360,20),"自動播放  /  Enter 或空白鍵下一句",12,GameUI.Muted);
  if(GameUI.Button(new Rect(754,475,150,30),Sequence.Cue==7?"出發吧":"下一句"))Sequence.Next();
  GameUI.Panel(new Rect(30,528,900*Sequence.Elapsed/OpeningSequence.Duration,2),GameUI.Gold);
 }
 public void Draw(){
  float t=Sequence.Elapsed;int shot=Sequence.Shot;
  GameUI.Panel(new Rect(0,0,960,540),Night);
  if(shot<2){
   Outdoors(t);
   Host(300,264,133);
   Prize(540,213,110,shot==0?Mathf.Sin(t*.5f)*.2f:(t-7)*1.2f);
   GameUI.Text(new Rect(603,174,130,64),"冠軍獎品\nGTA7",23,GameUI.Cream,TextAnchor.MiddleCenter);
   for(int i=0;i<9;i++){float x=92+i*95+Mathf.Sin(t+i)*3;float y=356+Mathf.Sin(t*3+i)*3;Goat(x,y,98,false,i*2);Camera(x+15,y-30,.38f,GameUI.Cream);}
   if(shot==1){
    // Bring the prize forward without covering the host or the dialogue area.
    GameUI.Panel(new Rect(423,153,153,210),new Color(.08f,.18f,.20f,.85f));
    Prize(500,253,186,(t-7)*1.2f);
    for(int i=0;i<8;i++)GameUI.Diamond(405+i*27,151+Mathf.Sin(t*3+i)*6,GameUI.Gold);
   }
  }else{
   GUI.DrawTexture(new Rect(0,0,960,540),landscape);
   for(int i=0;i<7;i++){float x=Mathf.Repeat(i*167-t*5,1100)-140;GameUI.Panel(new Rect(x,80+i%3*35,100,8),new Color(.86f,.8f,.85f,.2f));}
   if(shot==2){
    float u=(t-13)/7;float x=Mathf.Lerp(145,545,u);float y=347;
    GameUI.Panel(new Rect(0,347,960,34),new Color(.15f,.23f,.25f));
    Goat(x,y,154,true);Camera(x+27,y-47,.65f,GameUI.Cream);
    GameUI.Text(new Rect(520,38,370,55),"傳說中最難攀登的山",23,GameUI.Cream,TextAnchor.MiddleCenter);
   }else{
    GameUI.Panel(new Rect(0,0,960,540),new Color(.07f,.03f,.18f,.38f));
    Goat(350,351,192,false);float pulse=.6f+.4f*Mathf.Sin(t*4);
    Camera(450,252,1.8f,Color.Lerp(Cyan,GameUI.Cream,pulse));
    for(int i=0;i<20;i++){float a=t*1.1f+i*Mathf.PI/10;float rad=50+Mathf.Repeat(i*17-t*25,125);GameUI.Diamond(450+Mathf.Cos(a)*rad,252+Mathf.Sin(a)*rad*.65f,new Color(.5f,1,1,.7f));}
    // A translucent photographic echo foreshadows the existing freeze / trace mechanics.
    var old=GUI.color;GUI.color=new Color(.45f,1,1,Mathf.Clamp01((t-22)/2)*.4f);Goat(650,349,160,false);GUI.color=old;
   }
  }
  Dialogue();
  float local=t-(shot==0?0:shot==1?7:shot==2?13:20);
  float fade=Mathf.Max(1-local/.45f,Mathf.Clamp01((t-26.3f)/.7f));
  if(fade>0)GameUI.Panel(new Rect(0,0,960,540),new Color(.025f,.03f,.08f,fade));
 }
}
}
