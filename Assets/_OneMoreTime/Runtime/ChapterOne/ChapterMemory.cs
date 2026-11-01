using UnityEngine;
namespace OneMoreTime {
// Uses the same snapshots, fixed clock and sole echo-carry motor as the legacy levels.
public sealed class ChapterMemory:MonoBehaviour {
 public NostalgiaChapter game;public EchoSurface echo;public BoxCollider2D freezeGate,traceGate;
 public SpriteRenderer freezePad,padA,padB,console;
 public MemorySnapshot Memory{get;private set;}public Recorder Recorder{get;}=new Recorder();
 public bool Recording{get;private set;}public bool DeathReview{get;private set;}public bool MemoryPast{get;private set;}
 public PlaybackState Playback{get;private set;}=PlaybackState.Ready;public float PlaybackTime{get;private set;}
 public bool FreezeSolved{get;private set;}public bool TraceSolved{get;private set;}public int Signals{get;private set;}
 float recordClock;public bool Unlocked=>game.player.Position.x>=64||Memory!=null;
 public string Action=>Recording?"Q 保存並回旗幟":Memory==null?"E 定格 / Q 開始錄影":Memory.Kind==MemoryKind.Freeze?"E 清除定格":Playback==PlaybackState.Ready?"Q 播放錄影":"Q 清除 / R 保留重試";
 public string NextStep {
  get {
   float x=game.player.Position.x;bool relay=x>=315;
   if(x>=128&&x<192)return game.Devices[0]?"水車已啟動。切回現在，沿升起的橋前進。":"靠近水車，按 F 切回過去，再按 X 啟動機關。";
   if(x>=256&&x<315)return game.Devices[1]?"鐘樓已啟動。切換年代，沿右側橋面下山。":"登上鐘樓，切到過去，在鐘繩旁按 X。";
   if(x>=343&&game.player.Position.y>=6.2f)return "已登上高崖！前往右側相機標記，切回現在按 X 拍照。";
   if(relay&&Memory!=null&&Memory.Kind==MemoryKind.Trace&&Memory.Sample(0).position.x<315)return Playback==PlaybackState.Ready?"這是上一區的錄影。按 Q 播放，再按一次 Q 清除，才能重錄。":"這是上一區的錄影。按 Q 清除，回旗幟準備新路線。";
   if(game.player.Position.x<128){if(Memory==null)return game.Past?"走上金色踏板並站穩，再按 E。保存後會回到旗幟。":"按 F 回到過去，找到金色踏板。";if(Memory.Kind==MemoryKind.Freeze)return FreezeSolved?"定格正在壓住踏板。向右穿過石門，到相機旁按 X 拍照。":"這份定格沒有壓住過去的踏板。按 E 清除，再試一次。";return "這裡需要定格：先按 Q 清除錄影，再按 E 留下自己。";}
   if(Memory!=null&&Memory.Kind==MemoryKind.Freeze)return "仍保存著上一份定格。先按 E 清除，才能用 Q 錄影。";
   if(Recording)return relay?"錄影中：先等候上車，跳到中島、等候、長跳。落地按 Q 保存。":"錄影中：依序踩 A、B。到 B 後按 Q 保存並回旗幟。";
   if(Memory==null)return relay?"走到左岸標記，按 Q 開始錄影。H 可看完整示範步驟。":"走到 A 標記，按 Q 開始錄影。Q 不會自動錄影。";
   if(Playback==PlaybackState.Ready)return relay?"錄影已保存。回到左岸，按 Q 播放，立刻跳上殘影。":"錄影已保存。本體先走上白色控制台，再按 Q 播放。";
   if(Playback==PlaybackState.Playing)return relay?"放開方向鍵，讓殘影載你。它跳高時，再起跳向右登上高崖。":TraceSolved?"石門已開！向右走，到相機旁按 X 拍照。":"正在回放：本體留在白色控制台，等 A、B 依序亮起。";
   return TraceSolved&&!relay?"時序門已解鎖，可以向右通過。下一區按 Q 清除這份舊錄影。":"回放結束。R 保留錄影重試；Q 清除，重新錄製。";
  }
 }
 void Awake(){echo.Clear();}
 public Vector2 Advance(){var old=echo.Pose.position;echo.TickFade(.02f);if(Memory!=null&&Memory.Kind==MemoryKind.Trace&&Playback==PlaybackState.Playing){PlaybackTime+=.02f;if(PlaybackTime>=Memory.Duration){Playback=PlaybackState.Finished;echo.Finish();}else echo.Show(Memory.Sample(PlaybackTime),true);}return old;}
 public void AfterMove(InputFrame f){
  if(Recording){recordClock+=.02f;Recorder.Add(new RecordedFrame(recordClock,game.player));}
  if(Unlocked&&f.freeze){
   if(Memory!=null&&Memory.Kind!=MemoryKind.Freeze){game.Say("先按 Q 清除錄影，再使用定格");return;}
   if(Memory==null){Memory=new MemorySnapshot(new RecordedFrame(0,game.player),1);MemoryPast=game.Past;}else Memory=null;
   game.Respawn();return;
  }
  if(Unlocked&&f.trace){
   if(Memory==null){if(Recording)Save();else{Recording=true;MemoryPast=game.Past;recordClock=0;Recorder.Clear();Recorder.Add(new RecordedFrame(0,game.player));game.Say(game.player.Position.x>=315?"錄影開始：先等候上車，再跳到中島、等候、長跳":"錄影開始：走上 A，再走到 B；等待也會被記下");}}
   else if(Memory.Kind==MemoryKind.Freeze)game.Say("先按 E 清除定格，再開始錄影");
   else if(Playback==PlaybackState.Ready){Playback=PlaybackState.Playing;PlaybackTime=0;Signals=0;echo.Show(Memory.Sample(0),true);}
   else{Memory=null;game.Respawn();}
  }
  Evaluate();
 }
 public void Save(){var saved=Recorder.Save(1);if(saved==null){game.Say("錄影至少需要四分之一秒，繼續走動或等待");return;}Memory=saved;game.Respawn();}
 public void Review(InputFrame f){if(f.retry)game.Respawn();else if(f.trace)Save();}
 public bool OnDeath(){if(!Recording)return false;DeathReview=true;return true;}
 public void ResetRound(){Recording=false;DeathReview=false;Recorder.Clear();recordClock=0;Playback=PlaybackState.Ready;PlaybackTime=0;Signals=0;echo.Clear();if(Memory!=null)echo.Show(Memory.Sample(0),Memory.Kind==MemoryKind.Freeze);Evaluate();}
 static bool At(Vector2 p,Vector2 target)=>Mathf.Abs(p.x-target.x)<1.15f&&Mathf.Abs(p.y-target.y)<.12f;
 public void Evaluate(){
  bool echoHeld=echo.Active&&echo.Pose.grounded&&MemoryPast&&At(echo.Pose.position,new Vector2(80,0));bool held=echoHeld;
  // The live player may open the gate, but walking away releases it: a saved self is needed.
  held|=game.Past&&game.player.Grounded&&At(game.player.Position,new Vector2(80,0));
  if(echoHeld)FreezeSolved=true;
  bool occupying=new Bounds(freezeGate.transform.position,freezeGate.transform.lossyScale).Intersects(new Bounds(game.player.Position+Vector2.up*.5f,new Vector3(.7f,1,1)));
  freezeGate.enabled=!held&&!occupying;freezeGate.GetComponent<SpriteRenderer>().enabled=freezeGate.enabled;freezePad.color=held?GameUI.Mint:GameUI.Gold;
  bool live=game.player.Grounded&&At(game.player.Position,new Vector2(230,0));
  bool a=echo.Active&&echo.Pose.grounded&&At(echo.Pose.position,new Vector2(205,0)),b=echo.Active&&echo.Pose.grounded&&At(echo.Pose.position,new Vector2(216,0));
  if(!TraceSolved){if(!live)Signals=0;else if(a)Signals=1;else if(b&&Signals==1){Signals=2;TraceSolved=true;game.Say("兩段訊號對上了！時序石門已解鎖");}}
  traceGate.enabled=!TraceSolved;traceGate.GetComponent<SpriteRenderer>().enabled=!TraceSolved;padA.color=a?GameUI.Mint:GameUI.Gold;padB.color=b?GameUI.Mint:GameUI.Gold;console.color=live?GameUI.Mint:GameUI.Cream;Physics2D.SyncTransforms();
 }
}
}
