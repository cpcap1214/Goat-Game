using UnityEngine;using UnityEngine.InputSystem;
namespace OneMoreTime {
[DefaultExecutionOrder(100)]
public sealed class RunManager : MonoBehaviour {
 public EchoPath path;public Font uiFont;public GameAudio audioBus;public LevelConfig config;public PlayerMotor2D player;public PlayerVisual visual;public EchoSurface echo;public Camera view;
 public MemorySnapshot Memory{get;private set;}public RunPhase Phase=>Memory==null?RunPhase.Preparation:RunPhase.Execution;
 public FlowState State{get;private set;}=FlowState.Playing;public int Generation{get;private set;}
 public IPlayerInput InputSource{get;set;}public bool AutoTick=true;public float Clock{get;private set;}
 public Recorder Recorder{get;}=new Recorder();public PlaybackState Playback{get;private set;}=PlaybackState.Ready;public float PlaybackTime{get;private set;} int ticks;float flash;
 Vector2 previousPlayerPosition,previousEchoPosition;
 PressurePlate[] plates;LinkedDoor[] doors;CrumblingPlatform[] crumbles;
 public bool IsRecording{get;private set;} float recordingStart;
 public bool FreezeUnlocked{get;private set;}public bool TraceUnlocked{get;private set;}
 public AbilityPickup Relic{get;private set;}public int AbilityPage{get;private set;}
 public AbilityDemo Demonstration{get;private set;}
 public float TraceTimelineSeconds=>IsRecording?Recorder.Duration:Memory!=null&&Memory.Kind==MemoryKind.Trace?(Playback==PlaybackState.Ready?Memory.Duration:Mathf.Min(PlaybackTime,Memory.Duration)):0;
 public float TraceTimelineLimit=>IsRecording||Memory==null||Memory.Kind!=MemoryKind.Trace?10:Memory.Duration;
 void SetupAbilities(){FreezeUnlocked=config.level!=1;TraceUnlocked=config.level>=4;if(config.level!=1&&config.level!=3)return;var item=new GameObject(config.level==1?"Freeze watch pickup":"Trace hourglass pickup");item.transform.SetParent(transform);Relic=item.AddComponent<AbilityPickup>();Relic.Init(config.level==1?MemoryKind.Freeze:MemoryKind.Trace,config.spawn+new Vector2(1.7f,.85f));}
 bool TryCollectAbility(){if(Relic==null||!Relic.Touches(player))return false;Relic.Collect();if(Relic.Kind==MemoryKind.Freeze)FreezeUnlocked=true;else TraceUnlocked=true;AbilityPage=0;State=FlowState.AbilityDialogue;InputSource.Clear();audioBus?.Cue(1);Render();return true;}
 public void AdvanceAbilityDialogue(){if(State!=FlowState.AbilityDialogue)return;AbilityPage++;if(AbilityPage>=AbilityTutorial.Pages(Relic.Kind).Length){Demonstration=new AbilityDemo(Relic.Kind);State=FlowState.AbilityDemonstration;}InputSource?.Clear();}
 public void FinishAbilityDemo(){if(State!=FlowState.AbilityDemonstration)return;Demonstration=null;State=FlowState.Playing;InputSource?.Clear();}
 KeyboardPlayerInput keyboard;string notice="";
 void Awake(){crumbles=GetComponentsInChildren<CrumblingPlatform>();plates=GetComponentsInChildren<PressurePlate>();doors=GetComponentsInChildren<LinkedDoor>();keyboard=new KeyboardPlayerInput();InputSource=keyboard;player.Init();SetupAbilities();ResetRun();}
 void Update(){if(!AutoTick)return;var k=Keyboard.current;if(State==FlowState.AbilityDemonstration){AdvancePresentation(Time.unscaledDeltaTime);if(k!=null&&(k.escapeKey.wasPressedThisFrame||(Demonstration.Finished&&(k.enterKey.wasPressedThisFrame||k.spaceKey.wasPressedThisFrame))))FinishAbilityDemo();return;}if(State==FlowState.AbilityDialogue){if(k!=null&&(k.enterKey.wasPressedThisFrame||k.spaceKey.wasPressedThisFrame))AdvanceAbilityDialogue();return;}if(k!=null&&State==FlowState.Completed&&k.enterKey.wasPressedThisFrame){SceneFlow.Load(config.level+1);return;}if(k!=null&&(k.escapeKey.wasPressedThisFrame||k.pKey.wasPressedThisFrame))SetPaused(State!=FlowState.Paused);if(State==FlowState.Playing||State==FlowState.DeathReview)keyboard.Poll();}
 public void AdvancePresentation(float delta){if(State==FlowState.AbilityDemonstration)Demonstration.Tick(delta);}
 void FixedUpdate(){if(AutoTick)Tick(InputSource.Consume());}
 public void Tick(InputFrame input){
  if(State==FlowState.AbilityDemonstration)return;
  if(State==FlowState.DeathReview){if(input.retry){ResetRun();return;}if(input.trace)SaveTrace();return;}
  if(State!=FlowState.Playing)return;
  previousPlayerPosition=player.Position;previousEchoPosition=echo.Pose.position;
  flash=Mathf.Max(0,flash-.02f);echo.TickFade(.02f);
  Vector2 old=echo.Pose.position;
  if(Memory!=null&&Memory.Kind==MemoryKind.Trace&&Playback==PlaybackState.Playing){PlaybackTime+=.02f;if(PlaybackTime>=Memory.Duration){Playback=PlaybackState.Finished;echo.Finish();path?.Visible(false);}else echo.Show(Memory.Sample(PlaybackTime),true);}
  player.Step(input,.02f,echo.Active,old,echo.Pose.position);Clock=++ticks*.02f;
  if(input.jump&&player.Velocity.y>0)audioBus?.Cue(0);
  if(IsDead()){audioBus?.Cue(2);if(Memory==null&&IsRecording){State=FlowState.DeathReview;InputSource.Clear();}else ResetRun();return;}
  if(input.retry){ResetRun();return;}
  Relic?.Animate(Clock);if(TryCollectAbility())return;
  UpdateMechanisms();foreach(var c in crumbles){bool before=c.Collapsed;c.Tick(player,.02f);if(!before&&c.Collapsed)audioBus?.Cue(5);}
  if(Memory==null&&IsRecording)Recorder.Add(new RecordedFrame(Clock-recordingStart,player));
  if(input.freeze&&FreezeUnlocked){if(Memory==null){Memory=new MemorySnapshot(new RecordedFrame(Clock,player),config.level);audioBus?.Cue(1);ResetRun();return;}if(Memory.Kind==MemoryKind.Freeze){Rerecord();return;}notice="按 Q 清除軌跡，回起點重新準備。";}
  else if(input.trace&&TraceUnlocked&&config.level>=3){if(Memory==null){if(IsRecording)SaveTrace();else{IsRecording=true;recordingStart=Clock;Recorder.Clear();Recorder.Add(new RecordedFrame(0,player));notice="";}Render();return;}if(Memory.Kind==MemoryKind.Trace&&Playback==PlaybackState.Ready){Playback=PlaybackState.Playing;PlaybackTime=0;echo.Show(Memory.Sample(0),true);}else if(Memory.Kind==MemoryKind.Trace){Rerecord();return;}else notice="按 E 清除定格，回起點重新準備。";}
  if(config.goal.Overlaps(new Rect(player.Position-new Vector2(.35f,0),new Vector2(.7f,1)))){State=FlowState.Completed;InputSource.Clear();audioBus?.Cue(6);}
  Render();
 }
 void SaveTrace(){var snapshot=Recorder.Save(config.level);if(snapshot==null){notice="軌跡太短，請先走動或等一下。";return;}Memory=snapshot;audioBus?.Cue(1);ResetRun();}
 public void ResetRun(){Demonstration=null;Generation++;flash=.14f;audioBus?.Cue(2);InputSource?.Clear();echo.Clear();foreach(var p in plates)p.ResetState();foreach(var d in doors)d.ResetState();foreach(var c in crumbles)c.ResetState();player.ResetAt(config.spawn);Clock=0;ticks=0;Recorder.Clear();IsRecording=false;recordingStart=0;Playback=PlaybackState.Ready;PlaybackTime=0;State=FlowState.Playing;notice="";if(Memory!=null)echo.Show(Memory.Sample(0),Memory.Kind==MemoryKind.Freeze);path?.Show(Memory);UpdateMechanisms();Physics2D.SyncTransforms();ResetVisualHistory();Render();}
 bool IsDead(){if(player.Position.y<config.killY)return true;var body=new Rect(player.Position-new Vector2(.35f,0),new Vector2(.7f,1));foreach(var h in config.hazards)if(h.Overlaps(body))return true;return false;}
 void UpdateMechanisms(){foreach(var p in plates)p.Evaluate(player,echo);foreach(var d in doors){bool before=d.Open;d.Evaluate(player);if(before!=d.Open)audioBus?.Cue(4);}}
 public void Rerecord(){Memory=null;ResetRun();}
 public void SetPaused(bool value){if(State==FlowState.Completed||State==FlowState.DeathReview||State==FlowState.AbilityDialogue||State==FlowState.AbilityDemonstration)return;State=value?FlowState.Paused:FlowState.Playing;InputSource?.Clear();}
 void OnApplicationFocus(bool focus){if(!focus&&State==FlowState.Playing)SetPaused(true);}
 void Render(){var pose=new RecordedFrame(Clock,player);visual.Pose(pose.position,pose.facing,pose.state,pose.frame,Color.white);}
 void ResetVisualHistory(){previousPlayerPosition=player.Position;previousEchoPosition=echo.Pose.position;}
 // Interpolate presentation only. Physics, recording and platform carry keep their fixed-step positions.
 public void PresentFrame(float alpha){
  Vector2 shown=Vector2.Lerp(previousPlayerPosition,player.Position,alpha);
  visual.transform.position=shown;
  if(echo.gameObject.activeSelf)echo.visual.transform.position=Vector2.Lerp(previousEchoPosition,echo.Pose.position,alpha);
  if(view!=null)view.transform.position=new Vector3(Mathf.Clamp(shown.x,config.cameraMin,config.cameraMax),config.cameraY,-10);
 }
 void LateUpdate(){
  if(view!=null)view.rect=GameUI.GameplayViewport(Screen.width,Screen.height);
  if(!AutoTick||State!=FlowState.Playing){ResetVisualHistory();PresentFrame(1);return;}
  PresentFrame(Mathf.Clamp01((Time.time-Time.fixedTime)/Time.fixedDeltaTime));
 }
 public string FreezeAction=>!FreezeUnlocked?"先撿懷錶":Memory!=null&&Memory.Kind==MemoryKind.Freeze?"重新準備":"定格";
 public string TraceAction=>!TraceUnlocked?"先撿沙漏":Memory==null?(IsRecording?"保存軌跡":"開始錄製"):Memory.Kind==MemoryKind.Freeze?"先按 E 清除":Playback==PlaybackState.Ready?"播放軌跡":"重新準備";
 public string Tutorial=>notice!=""?notice:Memory==null?config.hint:Memory.Kind==MemoryKind.Freeze?"從上方踩住定格，站穩再跳。E 清除；R 保留記憶重試。":Playback==PlaybackState.Ready?(config.level==4?"走上層，在門前準備好再 Q。A 控制第一、第三扇門。":"先到左岸上車處，再 Q 播放；跟隨跳躍，在右岸接力登高。"):Playback==PlaybackState.Playing?(config.level==4?"跨越斷橋，在永久草地等門；第三扇門需要過去的你回到 A。":"站穩再出發。在殘影最後一跳的高處起跳，離開它登上高崖。 "):"回放結束。R 保留記憶重試；Q 清除並回起點重新準備。";
 void OnGUI(){
  GameUI.Begin(uiFont);if(flash>0)GameUI.Panel(new Rect(0,0,960,540),new Color(.5f,1,.75f,flash));
  GameUI.Panel(new Rect(0,503,960,37),new Color(.04f,.11f,.10f,1));GameUI.KeyLabel(25,507,"A D","移動");GameUI.KeyLabel(160,507,"Space","跳躍");GameUI.KeyLabel(308,507,"E",FreezeAction);if(config.level>=3)GameUI.KeyLabel(448,507,"Q",TraceAction);GameUI.KeyLabel(615,507,"R","重試");GameUI.KeyLabel(754,507,"Esc","暫停");
  DrawTraceTimeline();
  if(State==FlowState.AbilityDialogue)DrawAbilityDialogue();
  if(State==FlowState.AbilityDemonstration){int action=Demonstration.Draw(visual.library);if(action==1)Demonstration.Restart();else if(action==2)FinishAbilityDemo();}
  if(State==FlowState.Paused)DrawPause();
  if(State==FlowState.DeathReview){GameUI.Shade();GameUI.Frame(new Rect(222,110,516,322));GameUI.Text(new Rect(258,140,444,23),"每一次嘗試，都留下些什麼。",13,GameUI.Gold);GameUI.Text(new Rect(258,179,444,80),"再一次，\n帶上剛才的自己？",30);GameUI.Text(new Rect(258,273,444,33),"保存死前的軌跡，或重新出發。",16,GameUI.Muted);GUI.enabled=Recorder.Duration>=.25f;if(GameUI.Button(new Rect(258,324,212,48),"保存軌跡",true,"Q"))SaveTrace();GUI.enabled=true;if(GameUI.Button(new Rect(490,324,212,48),"重新準備",false,"R"))ResetRun();}
  if(State==FlowState.Completed){GameUI.Shade();GameUI.Frame(new Rect(223,118,514,310));GameUI.Text(new Rect(258,193,444,65),"通關！",36);if(GameUI.Button(new Rect(258,338,444,51),config.level==4?"走向結尾":"下一段旅程",true,"Enter"))SceneFlow.Load(config.level+1);}
 }
 void DrawTraceTimeline(){
  if(!TraceUnlocked||config.level<3)return;
  bool saved=Memory!=null&&Memory.Kind==MemoryKind.Trace;float seconds=TraceTimelineSeconds,limit=TraceTimelineLimit;
  string status=IsRecording?"錄製中":saved?(Playback==PlaybackState.Ready?"已保存":Playback==PlaybackState.Playing?"播放中":"播放結束"):"Q 開始錄製";
  GameUI.Frame(new Rect(304,18,352,75));GameUI.Text(new Rect(320,27,180,22),status,16,IsRecording?GameUI.Gold:GameUI.Mint);GameUI.Text(new Rect(498,27,142,22),$"{seconds:F1} / {limit:F1} 秒",14,GameUI.Cream,TextAnchor.MiddleRight);
  GameUI.Panel(new Rect(320,55,320,6),GameUI.Border);GameUI.Panel(new Rect(320,55,320*Mathf.Clamp01(seconds/Mathf.Max(.02f,limit)),6),IsRecording?GameUI.Gold:GameUI.Mint);
  for(int i=1;i<10;i++)GameUI.Panel(new Rect(320+i*32,54,1,8),GameUI.Ink);
  string hint=IsRecording?(Recorder.Duration>=9.99f?"已滿 10 秒 · 持續保留最近 10 秒":"Q 保存並回到起點"):saved?(Playback==PlaybackState.Ready?"Q 播放軌跡":Playback==PlaybackState.Playing?"Q 清除 · R 保留並重試":"R 再試一次 · Q 清除"):Memory!=null?"先按 E 清除定格":"";
  GameUI.Text(new Rect(320,67,320,19),hint,11,GameUI.Muted);
 }
 void DrawAbilityDialogue(){
  if(GameUI.Dialogue(AbilityTutorial.Pages(Relic.Kind)[AbilityPage],AbilityPage==AbilityTutorial.Pages(Relic.Kind).Length-1?"看操作示範":"下一句",20))AdvanceAbilityDialogue();
 }
 void DrawPause(){
  GameUI.Shade();GameUI.Frame(new Rect(252,42,456,456));GameUI.Text(new Rect(286,100,388,45),"暫停",28);
  if(GameUI.Button(new Rect(286,157,388,42),"繼續旅程",true,"Esc"))SetPaused(false);if(GameUI.Button(new Rect(286,212,388,42),"保留記憶重試",false,"R"))ResetRun();if(GameUI.Button(new Rect(286,268,388,40),"回到主選單"))SceneFlow.Load(0);
  GameUI.Divider(286,330,388);GameUI.Text(new Rect(286,351,88,25),"音樂",15,GameUI.Muted);GameUI.Text(new Rect(286,401,88,25),"音效",15,GameUI.Muted);
  GameAudio.MusicVolume=VolumeSlider(new Rect(385,352,285,25),GameAudio.MusicVolume);GameAudio.EffectsVolume=VolumeSlider(new Rect(385,402,285,25),GameAudio.EffectsVolume);
 }
 float VolumeSlider(Rect r,float value){GameUI.Panel(new Rect(r.x,r.y+10,r.width,4),GameUI.Border);GameUI.Panel(new Rect(r.x,r.y+10,r.width*value,4),GameUI.Gold);var invisible=new GUIStyle(GUIStyle.none);value=GUI.HorizontalSlider(r,value,0,1,invisible,invisible);GameUI.Panel(new Rect(r.x+(r.width-10)*value,r.y+4,10,16),GameUI.Cream);return value;}
}
}
