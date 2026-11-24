using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
namespace OneMoreTime {
// Isolated opt-in debug room. Every dynamic participant is sampled on this clock.
public sealed class RewindPlaytest:MonoBehaviour {
 public const string SceneName="Debug_Rewind";public const float HoldSeconds=.25f,MaxSeconds=10,StepSeconds=.02f;
 public static readonly Color ReplayColor=new Color(1f,.69f,.36f,.72f);
 public enum Mode{Idle,Recording,Rewinding,Ready,Playing}
 public PlayerMotor2D player;public PlayerVisual visual;public EchoSurface echo;public EchoPath path;public Camera view;public Font font;
 public RoomPlatform[] platforms;public RewindCrate[] crates;public PressurePlate plate;public LinkedDoor door;
 public bool AutoTick=true;public bool externalCamera,externalHud;public string hudFooter="Push crates or stand on the plate while recording. Hold Q for 0.25 seconds to record again.",pauseTitle="Rewind Playtest";public RewindParticipant[] participants=new RewindParticipant[0];public Vector2 spawn=new Vector2(3,0);public Rect bounds=new Rect(0,-4,40,15);
 public float deathY=-5;
 public event System.Action Retried;
 public event System.Action RetryRestored;
 public event System.Action WorldRewindFinished;
 public event System.Action Died;
 public Mode State{get;private set;}public MemorySnapshot Memory{get;private set;}public bool Paused{get;private set;}
 public readonly KeyBindingMenu Controls=new KeyBindingMenu();
 // Ready also describes a replay parked at its last frame. Only the interval
 // before its first playback (or after retry) holds the restored world still.
 public bool AwaitingReplay=>Memory!=null&&State==Mode.Ready&&!echo.Active;
 public float HoldProgress=>Mathf.Clamp01(holdTime/HoldSeconds);public float RecordSeconds=>Mathf.Max(0,(frames.Count-1)*StepSeconds);
 public float RewindSeconds=>rewindTime;public float PlaybackSeconds=>playbackTime;
 public bool PlaybackBackwards{get;private set;}
 public bool ReplayUpgradeUnlocked{get;private set;}
 public void UnlockReplayUpgrade(){SetReplayUpgradeAvailable(true);}
 public void SetReplayUpgradeAvailable(bool available){ReplayUpgradeUnlocked=available;if(!available){PlaybackBackwards=false;PlaybackHeld=false;directionPressed=shiftTracking=false;}}
 public bool PlaybackHeld{get;private set;}
 public float PlaybackProgress=>Memory==null?0:Mathf.Clamp01(playbackTime/Mathf.Max(.001f,Memory.Duration));
 public bool Charging=>qWasHeld&&!holdTriggered&&State!=Mode.Recording&&State!=Mode.Rewinding;
 readonly RewindObjectPaths objectPaths=new RewindObjectPaths();
 readonly RewindPlaytestHUD hud=new RewindPlaytestHUD();
 readonly KeyboardPlayerInput input=new KeyboardPlayerInput();readonly List<WorldFrame> frames=new List<WorldFrame>(501);
 bool recordedCrates,playbackRestarted;
 bool qWasHeld,holdTriggered,qHeld,directionPressed,shiftTracking;float shiftTime,holdTime,rewindTime,playbackTime;WorldFrame initial;
 sealed class WorldFrame {
  public PlayerMotor2D.RewindState player;public RecordedFrame pose;public RoomPlatform.RewindState[] platforms;public Vector3[] crates;public int sources;public bool door;public object[] participants;
 }
 void Awake(){ReplayUpgradeUnlocked=SceneFlow.ConsumeReplayUpgrade();player.Init();player.ResetAt(spawn);echo.Clear();echo.support.enabled=false;StyleTrace(path.GetComponent<LineRenderer>());foreach(var p in platforms)p.Tick(0);Physics2D.SyncTransforms();player.Step(default,StepSeconds);plate?.Evaluate(player,echo);door?.Evaluate(player);initial=Capture(0);Pose();}
 public static void StyleTrace(LineRenderer line){line.startWidth=line.endWidth=.022f;line.startColor=line.endColor=new Color(1f,.69f,.36f,.24f);line.sortingOrder=2;}
 void OnDestroy(){objectPaths.Dispose();hud.Dispose();}
 void Update(){if(!AutoTick)return;if(Controls.IsOpen){Controls.Tick();return;}var k=Keyboard.current;if(k==null)return;if(KeyBindings.WasPressed(GameAction.Pause))SetPaused(!Paused);if(Paused)return;input.Poll();qHeld=KeyBindings.IsHeld(GameAction.RecordReplay);
  if(KeyBindings.WasPressed(GameAction.ReplayDirection)){shiftTracking=true;shiftTime=0;}
  if(shiftTracking){shiftTime+=Time.unscaledDeltaTime;if(KeyBindings.WasReleased(GameAction.ReplayDirection)){directionPressed|=shiftTime<.25f;shiftTracking=false;}}
 }
 void FixedUpdate(){if(AutoTick){Step(input.Consume(),qHeld,directionPressed,KeyBindings.IsHeld(GameAction.ReplayDirection));directionPressed=false;}}
 public void SetPaused(bool paused){Paused=paused;if(!paused&&Controls.IsOpen)Controls.Close();holdTime=0;holdTriggered=true;qWasHeld=qHeld=false;directionPressed=false;shiftTracking=false;PlaybackHeld=false;input.Clear();KeyBindings.SuppressUntilRelease();}
 public void Step(InputFrame command,bool rewindHeld,bool toggleDirection=false,bool holdPlayback=false,EchoSurface fallbackSupport=null){
  if(Paused)return;
  HandleKey(rewindHeld);
  PlaybackHeld=ReplayUpgradeUnlocked&&holdPlayback&&Memory!=null&&(State==Mode.Ready||State==Mode.Playing);
  if(ReplayUpgradeUnlocked&&toggleDirection&&Memory!=null&&(State==Mode.Ready||State==Mode.Playing)){
   PlaybackBackwards=!PlaybackBackwards;
   if(echo.Active)State=Mode.Playing;
  }
  if(State==Mode.Rewinding){
   rewindTime=Mathf.Max(0,rewindTime-StepSeconds*5);int index=Mathf.Clamp(Mathf.CeilToInt(rewindTime/StepSeconds),0,frames.Count-1);Restore(frames[index]);
   if(rewindTime<=0){Restore(frames[0]);State=Mode.Ready;holdTime=0;holdTriggered=true;input.ClearActions();WorldRewindFinished?.Invoke();}Pose();return;
  }
  if(command.retry){Retry();return;}
  Vector2 oldEcho=echo.Active?echo.Pose.position:Vector2.zero;
  if(State==Mode.Playing&&!PlaybackHeld){playbackTime=Mathf.Clamp(playbackTime+(PlaybackBackwards?-StepSeconds:StepSeconds),0,Memory.Duration);ShowEcho(Memory.Sample(playbackTime));if(PlaybackBackwards?playbackTime<=0:playbackTime>=Memory.Duration)State=Mode.Ready;}
  EvaluatePlate();
  foreach(var p in platforms){if(AwaitingReplay&&p.affectedByRewind)p.HoldFrame();else p.Tick(StepSeconds);}
  bool replayCrates=recordedCrates&&Memory!=null&&echo.Active&&(State==Mode.Playing||State==Mode.Ready);
  WorldFrame replayFrame=replayCrates?frames[Mathf.Clamp(Mathf.RoundToInt(playbackTime/StepSeconds),0,frames.Count-1)]:null;
  for(int i=0;i<crates.Length;i++){if(AwaitingReplay&&crates[i].affectedByRewind)crates[i].ReplayTo(crates[i].transform.position,false);else if(replayCrates&&crates[i].affectedByRewind)crates[i].ReplayTo(replayFrame.crates[i],!playbackRestarted);else crates[i].Tick(player,command.move,StepSeconds);}
  for(int i=0;i<participants.Length;i++){if(AwaitingReplay&&participants[i].affectedByRewind)participants[i].HoldFrame();else if(replayCrates&&participants[i].affectedByRewind&&participants[i] is CrateSpawner source)source.RestorePlayback(replayFrame.participants[i],!playbackRestarted);else participants[i].Tick(StepSeconds);}
  playbackRestarted=false;EvaluateDoor();Physics2D.SyncTransforms();
  bool useFallback=!echo.Active&&fallbackSupport!=null&&fallbackSupport.CanSupport(player);
  player.Step(command,StepSeconds,echo.Active||useFallback,useFallback?fallbackSupport.Pose.position:oldEcho,useFallback?fallbackSupport.Pose.position:echo.Active?echo.Pose.position:Vector2.zero,platforms);
  EvaluatePlate();EvaluateDoor();echo.TickFade(StepSeconds);
  bool fell=player.Position.y<deathY;
  if(fell)Died?.Invoke();
  if(State==Mode.Recording){frames.Add(Capture(frames.Count*StepSeconds));objectPaths.Sample();if(frames.Count>=501||fell)FinishRecording();}
  else if(fell){Retry();return;}
  Pose();
 }
 void EvaluatePlate(){if(plate!=null&&!(AwaitingReplay&&plate.affectedByRewind))plate.Evaluate(player,echo);}
 void EvaluateDoor(){if(door!=null&&!(AwaitingReplay&&door.affectedByRewind))door.Evaluate(player);}
 void HandleKey(bool held){
  if(State==Mode.Rewinding){qWasHeld=held;holdTriggered=true;holdTime=0;return;}
  if(held){
   if(!qWasHeld){holdTime=0;holdTriggered=false;}
   holdTime+=StepSeconds;
   if(!holdTriggered&&holdTime>=HoldSeconds-.0001f&&State!=Mode.Recording){BeginRecording();holdTriggered=true;}
  }else if(qWasHeld){
   if(!holdTriggered){if(State==Mode.Recording)FinishRecording();else if(Memory!=null&&(State==Mode.Ready||State==Mode.Playing)){if(State==Mode.Playing)PlaybackBackwards=false;Play();}}
   holdTime=0;holdTriggered=false;
  }
  qWasHeld=held;
 }
 void BeginRecording(){recordedCrates=true;echo.Clear();Memory=null;path.Show(null);frames.Clear();playbackTime=0;PlaybackBackwards=false;State=Mode.Recording;frames.Add(Capture(0));objectPaths.Begin(this);}
 // Switching camera modes shares one memory slot. Only a successful shutter
 // capture may replace a saved replay; the live player and world stay in place.
 public bool TryUseShutter(ShutterAbility shutter){
  if(Paused||State==Mode.Recording||State==Mode.Rewinding||shutter==null||!shutter.Press())return false;
  echo.Clear();Memory=null;path.Show(null);frames.Clear();objectPaths.Clear();
  State=Mode.Idle;recordedCrates=playbackRestarted=false;playbackTime=rewindTime=holdTime=0;
  PlaybackBackwards=PlaybackHeld=false;qWasHeld=qHeld=false;holdTriggered=true;directionPressed=shiftTracking=false;input.ClearActions();
  return true;
 }
 public void FinishRecording(){
  if(State!=Mode.Recording||frames.Count<2)return;
  var poses=new RecordedFrame[frames.Count];for(int i=0;i<poses.Length;i++)poses[i]=frames[i].pose;
  Memory=new MemorySnapshot(poses,0);path.Show(Memory);objectPaths.Show(transform);rewindTime=Memory.Duration;State=Mode.Rewinding;echo.Clear();holdTime=0;holdTriggered=true;
 }
 // Scripted acquisition memory; normal recordings still capture actual simulation.
 public void BeginImaginaryRewind(Vector2 ground, Vector2 pickup, float duration=4){
  var live=player.CaptureRewind();BeginRecording();frames.Clear();
  int count=Mathf.Clamp(Mathf.RoundToInt(duration/StepSeconds),2,500);
  for(int i=0;i<=count;i++){
   float progress=i/(float)count;bool walking=progress<=.6f;
   var state=live;
   // Walk along the intact deck to the relic, then accelerate into the fall.
   // Reversing these samples rises to the relic before walking back to the bank.
   float fall=Mathf.Clamp01((progress-.6f)/.4f);
   state.position=walking?Vector2.Lerp(ground,pickup,progress/.6f):Vector2.Lerp(pickup,live.position,fall*fall);
   if(i==0)state.position=ground;else if(i==count)state.position=live.position;
   state.visualState=walking?1:3;state.visualFrame=Mathf.FloorToInt(i*StepSeconds*12);state.facing=1;
   state.velocity=Vector2.zero;state.grounded=walking;state.ground=null;state.dropped=null;state.onEcho=false;
   player.RestoreRewind(state);frames.Add(Capture(i*StepSeconds));
  }
  player.RestoreRewind(live);FinishRecording();recordedCrates=false;
 }
 void ShowEcho(RecordedFrame pose){echo.Show(pose,true);echo.visual.renderer2D.color=ReplayColor;echo.support.enabled=false;}
 void Play(){playbackRestarted=true;State=Mode.Playing;playbackTime=PlaybackBackwards?Memory.Duration:0;ShowEcho(Memory.Sample(playbackTime));}
 WorldFrame Capture(float time){var f=new WorldFrame{player=player.CaptureRewind(),pose=new RecordedFrame(time,player),platforms=new RoomPlatform.RewindState[platforms.Length],crates=new Vector3[crates.Length],sources=plate!=null?plate.Sources:0,door=door!=null&&door.Open,participants=new object[participants.Length]};for(int i=0;i<platforms.Length;i++)f.platforms[i]=platforms[i].CaptureRewind();for(int i=0;i<crates.Length;i++)f.crates[i]=crates[i].transform.position;for(int i=0;i<participants.Length;i++)f.participants[i]=participants[i].Capture();return f;}
 void Restore(WorldFrame f,bool restart=false){for(int i=0;i<platforms.Length;i++)if(restart||platforms[i].affectedByRewind)platforms[i].RestoreRewind(f.platforms[i]);for(int i=0;i<crates.Length;i++)if(restart||crates[i].affectedByRewind)crates[i].transform.position=f.crates[i];if(plate!=null&&(restart||plate.affectedByRewind))plate.RestoreRewind(f.sources);if(door!=null&&(restart||door.affectedByRewind))door.RestoreRewind(f.door);for(int i=0;i<participants.Length;i++)if(restart||participants[i].affectedByRewind)participants[i].Restore(f.participants[i]);player.RestoreRewind(f.player);Physics2D.SyncTransforms();}
 void Pose(){objectPaths.Tint(State==Mode.Rewinding);visual.Pose(player.Position,player.Facing,player.VisualState,player.VisualFrame,State==Mode.Rewinding?ReplayColor:Color.white);}
 void LateUpdate(){if(externalCamera)return;view.rect=GameUI.GameplayViewport(Screen.width,Screen.height);view.orthographicSize=10/view.aspect;view.transform.position=new Vector3(Mathf.Clamp(player.Position.x+3,bounds.xMin+10,bounds.xMax-10),Mathf.Min(4,bounds.yMax-view.orthographicSize),-10);}
 public void DrawHUD(){hud.Draw(this);}
 void OnGUI(){if(!externalHud)hud.Draw(this);}
 public void SetRetryAnchor(){initial=Capture(0);}
 public event System.Action PauseResetRequested;
 public void ResetFromPause(){
  if(!Paused)return;
  SetPaused(false);
  if(PauseResetRequested!=null)PauseResetRequested();else Retry();
 }
 public void Retry(){if(State==Mode.Recording&&frames.Count>=2){FinishRecording();return;}Restore(initial,true);echo.Clear();State=Memory==null?Mode.Idle:Mode.Ready;holdTime=0;qWasHeld=qHeld=false;holdTriggered=true;directionPressed=shiftTracking=false;PlaybackHeld=false;input.ClearActions();RetryRestored?.Invoke();Pose();Retried?.Invoke();}
 public void ChargeWhileSuspended(bool held){if(!Paused)HandleKey(held);}
}
}
