using UnityEngine;using UnityEngine.InputSystem;using UnityEngine.SceneManagement;
namespace OneMoreTime {
public sealed class NostalgiaChapter:MonoBehaviour {
 public ChapterMemory memories;public ChapterGuidance Guidance{get;}=new ChapterGuidance();
 public PlayerMotor2D player;public PlayerVisual visual;public Camera view;public Font font;public GameAudio audioBus;
 public EraTerrain[] terrain;public SpriteRenderer presentSky,pastSky;public Transform pickup;public SpriteRenderer[] deviceArt,photoArt;public Transform[] checkpointArt;
 public Transform waterwheel,bell;public BoxCollider2D[] photoGates;public Texture2D filterIcon;public bool AutoTick=true;public bool Past{get;private set;}public bool Unlocked{get;private set;}public bool Complete{get;private set;}public bool Paused{get;private set;}
 public bool[] Devices{get;private set;}=new bool[2];public bool[] Photos{get;private set;}=new bool[3];
 public int Checkpoint{get;private set;}public int Deaths{get;private set;}public int Switches{get;private set;}public float Elapsed{get;private set;}
 public string Notice{get;private set;}="";public int Tutorial{get;private set;}=0;
 KeyboardPlayerInput input=new KeyboardPlayerInput();bool switchQueued,interactQueued;float noticeTime,flash;Vector2 previous;Vector3 presentSkyScale,pastSkyScale;
 public string Objective=>!Unlocked?"走近發光鏡片，拾取懷舊濾鏡":!Photos[0]?(memories.FreezeSolved?"石門已開，前往右側取景點拍照":"石門與踏板相隔太遠，誰能留在過去？"):!Devices[0]?"回到過去，啟動水車":!Photos[1]?(memories.TraceSolved?"時序門已解鎖，前往右側取景點":"兩個訊號，一個控制台，需要兩個你"):!Devices[1]?"登上鐘樓，在過去拉動鐘繩":!Photos[2]?"錄下跳躍，搭乘回放，再從殘影背上起跳":"第一大關完成";
 void Awake(){presentSkyScale=presentSky.transform.localScale;pastSkyScale=pastSky.transform.localScale;player.Init();player.ResetAt(ChapterLayout.Checkpoints[0]);previous=player.Position;ApplyEra();UpdatePhotoGates();}
 void Update(){
  if(!AutoTick)return;var k=Keyboard.current;if(k==null)return;
  if(Complete){if(k.enterKey.wasPressedThisFrame)SceneFlow.Load(0);return;}
  if(Tutorial>=0){if(k.enterKey.wasPressedThisFrame||k.spaceKey.wasPressedThisFrame)AdvanceTutorial();return;}
  if(Guidance.IsOpen){if(k.enterKey.wasPressedThisFrame)AdvanceGuidance();else if(k.escapeKey.wasPressedThisFrame){Guidance.Close();ClearPendingInput();}return;}
  if(!Paused&&k.hKey.wasPressedThisFrame){Guidance.OpenAt(player.Position.x);ClearPendingInput();return;}
  if(k.escapeKey.wasPressedThisFrame){Paused=!Paused;input.Clear();switchQueued=false;}
  if(Paused)return;input.Poll();switchQueued|=k.fKey.wasPressedThisFrame;interactQueued|=k.xKey.wasPressedThisFrame;
 }
 void ClearPendingInput(){input.Clear();switchQueued=interactQueued=false;}
 public void AdvanceGuidance(){Guidance.Advance();ClearPendingInput();}
 public void AdvanceTutorial(){if(Tutorial==0)Tutorial=-1;else if(Tutorial==1)Tutorial=2;else Tutorial=-1;input.Clear();switchQueued=false;}
 void FixedUpdate(){if(!AutoTick)return;bool flip=switchQueued;switchQueued=false;Tick(input.Consume(),flip);}
 public void Tick(InputFrame f,bool flip=false){
  if(Paused||Complete||Tutorial>=0||Guidance.IsOpen)return;if(memories.DeathReview){memories.Review(f);return;}Elapsed+=.02f;noticeTime-=.02f;flash=Mathf.Max(0,flash-.02f);if(noticeTime<=0)Notice="";
  previous=player.Position;
  if(f.retry){Respawn();return;}
  if(flip)TrySwitch();var oldEcho=memories.Advance();player.Step(f,.02f,memories.echo.Active,oldEcho,memories.echo.Pose.position);
  if(player.Position.y<-5){Deaths++;if(!memories.OnDeath())Respawn();else input.Clear();return;}
  if(!Unlocked&&Vector2.Distance(player.Position,new Vector2(7,.5f))<1.6f){Unlocked=true;pickup.gameObject.SetActive(false);Tutorial=1;input.Clear();audioBus?.Cue(1);}
  for(int i=Checkpoint+1;i<ChapterLayout.Checkpoints.Length;i++)if(Vector2.Distance(player.Position,ChapterLayout.Checkpoints[i])<1.4f&&player.Grounded){Checkpoint=i;Say("已記錄重生點");audioBus?.Cue(1);}
  memories.AfterMove(f);if(interactQueued){interactQueued=false;Interact();}
  for(int i=0;i<photoGates.Length;i++)if(!Photos[i]&&Mathf.Abs(player.Position.x-photoGates[i].transform.position.x)<3)Say("先在相機標記按 X，拍下這裡的風景");
  if(player.Grounded){Guidance.Check(player.Position.x);if(Guidance.IsOpen)ClearPendingInput();}
  Render();
 }
 public bool TrySwitch(){
  if(!Unlocked){Say("先拾取懷舊濾鏡");return false;}
  var body=new Rect(player.Position+new Vector2(-.34f,.025f),new Vector2(.68f,.95f));
  foreach(var t in terrain)if(ChapterLayout.Active(t.block,!Past,Devices)&&t.block.rect.Overlaps(body)){Say("另一個年代這裡有障礙，移開後再切換");return false;}
  Past=!Past;Switches++;ApplyEra();flash=.22f;audioBus?.Cue(3);return true;
 }
 void UpdatePhotoGates(){for(int i=0;i<photoGates.Length;i++){photoGates[i].enabled=!Photos[i];photoGates[i].gameObject.GetComponent<SpriteRenderer>().enabled=!Photos[i];}Physics2D.SyncTransforms();}
 public void ApplyEra(){foreach(var t in terrain)t.Apply(Past,Devices);if(presentSky!=null)presentSky.enabled=!Past;if(pastSky!=null)pastSky.enabled=Past;Physics2D.SyncTransforms();}
 public void Interact(){
  for(int i=0;i<2;i++)if(Vector2.Distance(player.Position,ChapterLayout.Devices[i])<2){if(!Past){Say("機關已經損壞……試著回到過去");return;}Devices[i]=true;ApplyEra();audioBus?.Cue(4);Say(i==0?"水車重新轉動！現在的斷橋升起了":"古鐘敲響！現在的石橋恢復通路");return;}
  for(int i=0;i<3;i++)if(Vector2.Distance(player.Position,ChapterLayout.Photos[i])<2){if(Past){Say("切回現在，拍下歲月留下的風景");return;}if(i==2&&(!Photos[0]||!Photos[1]||!Devices[0]||!Devices[1]||!memories.FreezeSolved||!memories.TraceSolved)){Say("還有旅途風景尚未拍下，請查看左上目標");return;}Photos[i]=true;UpdatePhotoGates();flash=.3f;audioBus?.Cue(6);Say("風景已收入相簿");if(i==2){Complete=true;input.Clear();}return;}
  Say("靠近發光的相機標記或機關，再按 X");
 }
 public void Respawn(){interactQueued=false;Past=false;ApplyEra();player.ResetAt(ChapterLayout.Checkpoints[Checkpoint]);previous=player.Position;input.Clear();switchQueued=false;memories.ResetRound();Say("回到最近重生點；記憶、機關與照片保留");}
 public void Say(string s){Notice=s;noticeTime=4;}
 void Render(){visual.Pose(player.Position,player.Facing,player.VisualState,player.VisualFrame,Color.white);for(int i=0;i<deviceArt.Length;i++)deviceArt[i].color=Devices[i]?GameUI.Mint:GameUI.Gold;for(int i=0;i<photoArt.Length;i++)photoArt[i].color=Photos[i]?GameUI.Mint:Color.white;}
 void LateUpdate(){
  if(waterwheel!=null)waterwheel.localRotation=Quaternion.Euler(0,0,Devices[0]?Elapsed*-35:0);
  if(bell!=null)bell.localRotation=Quaternion.Euler(0,0,Devices[1]?Mathf.Sin(Elapsed*4)*12:0);
  Render();var p=Vector2.Lerp(previous,player.Position,Mathf.Clamp01((Time.time-Time.fixedTime)/.02f));if(!AutoTick||Paused||Tutorial>=0||Guidance.IsOpen)p=player.Position;visual.transform.position=p;
  view.rect=GameUI.GameplayViewport(Screen.width,Screen.height);
  float follow=Mathf.Clamp(p.x+3,8,380),x=follow,y=Mathf.Max(3.2f,p.y+2.2f);
  float courtyard=Mathf.SmoothStep(0,1,Mathf.InverseLerp(70,78,p.x))*(1-Mathf.SmoothStep(0,1,Mathf.InverseLerp(100,124,p.x)));
  float signals=Mathf.SmoothStep(0,1,Mathf.InverseLerp(194,204,p.x))*(1-Mathf.SmoothStep(0,1,Mathf.InverseLerp(230,252,p.x)));
  view.orthographicSize=6.1f+3.2f*courtyard+6.2f*signals;
  x=Mathf.Lerp(Mathf.Lerp(follow,92,courtyard),220,signals);
  // Keep the player inside the central safe area, even during puzzle-room framing.
  float safeHalfWidth=view.orthographicSize*view.aspect*.45f;
  x=Mathf.Clamp(x,p.x-safeHalfWidth,p.x+safeHalfWidth);
  view.transform.position=new Vector3(x,y,-10);
  foreach(var sky in new[]{presentSky,pastSky})if(sky!=null){sky.transform.position=new Vector3(x+(190-x)*.025f,y+1,3);sky.transform.localScale=(sky==presentSky?presentSkyScale:pastSkyScale)*(view.orthographicSize/6.1f);}
  if(pickup!=null&&pickup.gameObject.activeSelf)pickup.localPosition=new Vector3(7,1.2f+Mathf.Sin(Time.time*2)*.15f,0);
 }
 void OnApplicationFocus(bool focus){if(!focus&&!Complete&&Tutorial<0&&!Guidance.IsOpen){Paused=true;input.Clear();}}
 void LabelWorld(Vector2 world,string text){var p=view.WorldToViewportPoint(world);if(p.x<0||p.x>1||p.y<0||p.y>1)return;var r=new Rect(p.x*960-95,(1-p.y)*503-14,190,28);GameUI.Panel(r,GameUI.Ink);GameUI.Text(r,text,14,GameUI.Gold,TextAnchor.MiddleCenter);}
 void OnGUI(){
  GUI.matrix=Matrix4x4.identity;float scale=Mathf.Min(Screen.width/960f,Screen.height/540f),ox=(Screen.width-960*scale)*.5f,oy=(Screen.height-540*scale)*.5f;
  GameUI.Panel(new Rect(0,0,ox,Screen.height),Color.black);GameUI.Panel(new Rect(Screen.width-ox,0,ox,Screen.height),Color.black);GameUI.Panel(new Rect(0,0,Screen.width,oy),Color.black);GameUI.Panel(new Rect(0,Screen.height-oy,Screen.width,oy),Color.black);
  GameUI.Begin(font);GameUI.Panel(new Rect(0,503,960,37),GameUI.Ink);GameUI.KeyLabel(10,507,"A D","移動");GameUI.KeyLabel(140,507,"Space","跳");GameUI.KeyLabel(270,507,"F","年代");GameUI.KeyLabel(380,507,"E",memories.Memory!=null&&memories.Memory.Kind==MemoryKind.Freeze?"清除":"定格");GameUI.KeyLabel(490,507,"Q",memories.Recording?"保存":memories.Memory!=null&&memories.Memory.Kind==MemoryKind.Trace?(memories.Playback==PlaybackState.Ready?"播放":"清除"):"錄影");GameUI.KeyLabel(600,507,"X","互動");GameUI.KeyLabel(720,507,"R","重試");GameUI.KeyLabel(830,507,"Esc","暫停");
  GameUI.Frame(new Rect(20,16,390,73));GameUI.Text(new Rect(34,24,360,23),"第一大關 · 回聲山麓 / "+ChapterLayout.Zones[ChapterLayout.Zone(player.Position.x)],16,GameUI.Gold);GameUI.Text(new Rect(34,51,360,25),Objective,16);
  GameUI.Frame(new Rect(754,16,186,67));GameUI.Text(new Rect(766,22,162,28),Past?"過去 · 懷舊濾鏡":"現在 · 原色",20,Past?GameUI.Gold:GameUI.Mint,TextAnchor.MiddleCenter);GameUI.Text(new Rect(768,54,158,20),$"相簿 {(Photos[0]?1:0)+(Photos[1]?1:0)+(Photos[2]?1:0)} / 3",14,GameUI.Cream,TextAnchor.MiddleCenter);
  LabelWorld(new Vector2(331.4f,2.4f),"錄影：等候 → 跳過斷口");LabelWorld(new Vector2(341.7f,3.9f),"等候 → 原地長跳 → 保存");
  LabelWorld(new Vector2(80,2.4f),"過去踩踏板 → E 定格");LabelWorld(new Vector2(205,1.6f),"A · 錄影起點");LabelWorld(new Vector2(216,1.6f),"B · 第二訊號");LabelWorld(new Vector2(230,2.4f),"站在此處 → Q 播放");
  if(memories.Unlocked){GameUI.Frame(new Rect(20,96,390,62));GameUI.Text(new Rect(34,102,360,23),memories.Action,16,GameUI.Mint);GameUI.Text(new Rect(34,127,360,22),memories.Recording?$"錄影 {memories.Recorder.Duration:F1} / 10 秒":memories.Memory==null?"一次只能保留一份定格或錄影":memories.Memory.Kind==MemoryKind.Freeze?"已保存定格 · R 保留記憶重試":$"{(memories.Playback==PlaybackState.Ready?"已保存":memories.Playback==PlaybackState.Playing?"播放中":"播放結束")} · {memories.PlaybackTime:F1} / {memories.Memory.Duration:F1} 秒",14);}
  if(player.Position.x>=192&&player.Position.x<256){GameUI.Frame(new Rect(414,16,330,67));GameUI.Text(new Rect(426,27,306,44),memories.TraceSolved?"A → B 訊號完成 · 石門已開":$"回放訊號：{(memories.Signals>=1?"A ✓":"A —")} → B\n本體須站在白色控制台",16,GameUI.Mint);}
  if(memories.DeathReview){GameUI.Shade();GameUI.Frame(new Rect(220,160,520,200));GameUI.Text(new Rect(250,185,460,50),"保存死前的錄影？",28);if(GameUI.Button(new Rect(250,270,210,45),"保存",true,"Q"))memories.Save();if(GameUI.Button(new Rect(480,270,210,45),"重新準備",false,"R"))Respawn();}
  if(Notice!=""){GameUI.Frame(new Rect(190,438,580,44));GameUI.Text(new Rect(205,447,550,27),Notice,17,GameUI.Cream,TextAnchor.MiddleCenter);}
  if(flash>0)GameUI.Panel(new Rect(0,0,960,503),new Color(1,.91f,.67f,flash));
  if(Tutorial>=0){GameUI.Shade();GameUI.Frame(new Rect(190,130,580,295));GameUI.Text(new Rect(220,152,520,50),Tutorial==0?"第一大關 · 回聲山麓":"獲得：懷舊濾鏡",30,GameUI.Gold);if(Tutorial>0&&filterIcon!=null)GUI.DrawTexture(new Rect(663,145,74,74),filterIcon,ScaleMode.ScaleToFit);string copy=Tutorial==0?"最美的風景，藏在歲月的另一面。\n帶著相機穿越山麓，收集三處風景。\n\nA / D 移動，空白鍵跳躍。先找前方發光的鏡片。":Tutorial==1?"按 F，在現在與過去之間切換。\n古橋可能還沒倒塌，樹木也可能尚未長大。\n淡色橋影提示另一個年代的平台。":"跳起後也能按 F，接上另一個年代的平台。\n如果另一個年代會把你卡住，濾鏡會拒絕切換。\n\nE 留下定格，Q 錄影／保存／播放。\nX 操作機關／拍照；R 保留記憶回旗幟。";GameUI.Text(new Rect(220,215,520,140),copy,20);if(GameUI.Button(new Rect(480,365,250,40),Tutorial==1?"繼續":"出發",true,"Enter"))AdvanceTutorial();}
  if(Paused&&!Complete){GameUI.Shade();GameUI.Frame(new Rect(300,155,360,225));GameUI.Text(new Rect(330,180,300,50),"暫停",32);if(GameUI.Button(new Rect(330,250,300,42),"繼續探索",true)){Paused=false;input.Clear();}if(GameUI.Button(new Rect(330,310,300,42),"回到主選單"))SceneFlow.Load(0);}
  if(!Paused&&!Complete&&Tutorial<0){
   if(GameUI.Button(new Rect(754,91,186,34),"操作教學",false,"H")){Guidance.OpenAt(player.Position.x);ClearPendingInput();}
   if(!Guidance.IsOpen&&!memories.DeathReview&&memories.Unlocked){GameUI.Frame(new Rect(20,166,390,82));GameUI.Text(new Rect(34,176,360,62),memories.NextStep,17,GameUI.Cream);}
  }
  if(Guidance.IsOpen){GameUI.Shade();GameUI.Frame(new Rect(120,65,720,420));GameUI.Text(new Rect(150,85,660,42),Guidance.Title,29,GameUI.Gold);GameUI.Text(new Rect(150,135,660,32),Guidance.Goal,18);var lines=Guidance.Steps;for(int i=0;i<lines.Length;i++){GameUI.Key(new Rect(150,186+i*40,28,27),(i+1).ToString());GameUI.Text(new Rect(190,186+i*40,620,32),lines[i],18);}
   GameUI.Text(new Rect(150,355,660,52),Guidance.Reminder,14,GameUI.Muted);
   if(GameUI.Button(new Rect(500,425,300,40),Guidance.HasNext?"下一頁：如何搭乘":"開始嘗試",true,"Enter"))AdvanceGuidance();
   GameUI.Text(new Rect(150,434,330,26),"教學中遊戲暫停 · Esc 關閉",14,GameUI.Muted);
  }
  if(Complete){GameUI.Shade();GameUI.Frame(new Rect(210,110,540,330));GameUI.Text(new Rect(245,140,470,55),"第一大關完成！",34,GameUI.Gold);GameUI.Text(new Rect(245,212,470,110),$"三張風景，兩個年代，一段新的旅程。\n探索時間 {Mathf.FloorToInt(Elapsed/60)} 分 {Mathf.FloorToInt(Elapsed%60)} 秒\n年代切換 {Switches} 次 · 重試 {Deaths} 次",22);if(GameUI.Button(new Rect(245,365,470,42),"帶著照片回到主選單",true,"Enter"))SceneFlow.Load(0);}
 }
}
}
