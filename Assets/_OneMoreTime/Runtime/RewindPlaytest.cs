using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
namespace OneMoreTime {
// Isolated opt-in debug room. Every dynamic participant is sampled on this clock.
public sealed class RewindPlaytest:MonoBehaviour {
 public const string SceneName="Debug_Rewind";public const float HoldSeconds=.5f,MaxSeconds=10,StepSeconds=.02f;
 public static readonly Color ReplayColor=new Color(1f,.69f,.36f,.72f);
 public enum Mode{Idle,Recording,Rewinding,Ready,Playing}
 public PlayerMotor2D player;public PlayerVisual visual;public EchoSurface echo;public EchoPath path;public Camera view;public Font font;
 public RoomPlatform[] platforms;public RewindCrate[] crates;public PressurePlate plate;public LinkedDoor door;
 public bool AutoTick=true;public Vector2 spawn=new Vector2(3,0);public Rect bounds=new Rect(0,-4,40,15);
 public Mode State{get;private set;}public MemorySnapshot Memory{get;private set;}public bool Paused{get;private set;}
 public float HoldProgress=>Mathf.Clamp01(holdTime/HoldSeconds);public float RecordSeconds=>Mathf.Max(0,(frames.Count-1)*StepSeconds);
 public float RewindSeconds=>rewindTime;public float PlaybackSeconds=>playbackTime;
 public bool PlaybackBackwards{get;private set;}
 public bool ReplayUpgradeUnlocked{get;private set;}
 public void UnlockReplayUpgrade(){ReplayUpgradeUnlocked=true;}
 public bool PlaybackHeld{get;private set;}
 public float PlaybackProgress=>Memory==null?0:Mathf.Clamp01(playbackTime/Mathf.Max(.001f,Memory.Duration));
 public bool Charging=>qWasHeld&&!holdTriggered&&State!=Mode.Recording&&State!=Mode.Rewinding;
 readonly RewindPlaytestHUD hud=new RewindPlaytestHUD();
 readonly KeyboardPlayerInput input=new KeyboardPlayerInput();readonly List<WorldFrame> frames=new List<WorldFrame>(501);
 bool qWasHeld,holdTriggered,qHeld,directionPressed,shiftTracking;float shiftTime,holdTime,rewindTime,playbackTime;WorldFrame initial;
 sealed class WorldFrame {
  public PlayerMotor2D.RewindState player;public RecordedFrame pose;public RoomPlatform.RewindState[] platforms;public Vector3[] crates;public int sources;public bool door;
 }
 void Awake(){ReplayUpgradeUnlocked=SceneFlow.ConsumeReplayUpgrade();player.Init();player.ResetAt(spawn);echo.Clear();echo.support.enabled=false;StyleTrace(path.GetComponent<LineRenderer>());foreach(var p in platforms)p.Tick(0);Physics2D.SyncTransforms();player.Step(default,StepSeconds);plate.Evaluate(player,echo);door.Evaluate(player);initial=Capture(0);Pose();}
 public static void StyleTrace(LineRenderer line){line.startWidth=line.endWidth=.022f;line.startColor=line.endColor=new Color(1f,.69f,.36f,.24f);line.sortingOrder=2;}
 void OnDestroy(){hud.Dispose();}
 void Update(){if(!AutoTick)return;var k=Keyboard.current;if(k==null)return;if(k.escapeKey.wasPressedThisFrame)SetPaused(!Paused);if(Paused)return;input.Poll();qHeld=k.qKey.isPressed;
  if(k.leftShiftKey.wasPressedThisFrame){shiftTracking=true;shiftTime=0;}
  if(shiftTracking){shiftTime+=Time.unscaledDeltaTime;if(k.leftShiftKey.wasReleasedThisFrame){directionPressed|=shiftTime<.25f;shiftTracking=false;}}
 }
 void FixedUpdate(){if(AutoTick){Step(input.Consume(),qHeld,directionPressed,Keyboard.current!=null&&Keyboard.current.leftShiftKey.isPressed);directionPressed=false;}}
 public void SetPaused(bool paused){Paused=paused;holdTime=0;holdTriggered=true;directionPressed=false;shiftTracking=false;PlaybackHeld=false;input.ClearActions();}
 public void Step(InputFrame command,bool rewindHeld,bool toggleDirection=false,bool holdPlayback=false){
  if(Paused)return;
  HandleKey(rewindHeld);
  PlaybackHeld=ReplayUpgradeUnlocked&&holdPlayback&&Memory!=null&&(State==Mode.Ready||State==Mode.Playing);
  if(ReplayUpgradeUnlocked&&toggleDirection&&Memory!=null&&(State==Mode.Ready||State==Mode.Playing)){
   PlaybackBackwards=!PlaybackBackwards;
   if(echo.Active)State=Mode.Playing;
  }
  if(State==Mode.Rewinding){
   rewindTime=Mathf.Max(0,rewindTime-StepSeconds*5);int index=Mathf.Clamp(Mathf.CeilToInt(rewindTime/StepSeconds),0,frames.Count-1);Restore(frames[index]);
   if(rewindTime<=0){Restore(frames[0]);State=Mode.Ready;holdTime=0;holdTriggered=true;input.ClearActions();}Pose();return;
  }
  if(command.retry){if(State==Mode.Recording){FinishRecording();return;}Restore(initial,true);echo.Clear();State=Memory==null?Mode.Idle:Mode.Ready;Pose();return;}
  Vector2 oldEcho=echo.Active?echo.Pose.position:Vector2.zero;
  if(State==Mode.Playing&&!PlaybackHeld){playbackTime=Mathf.Clamp(playbackTime+(PlaybackBackwards?-StepSeconds:StepSeconds),0,Memory.Duration);ShowEcho(Memory.Sample(playbackTime));if(PlaybackBackwards?playbackTime<=0:playbackTime>=Memory.Duration)State=Mode.Ready;}
  plate.Evaluate(player,echo);foreach(var p in platforms)p.Tick(StepSeconds);foreach(var c in crates)c.Tick(player,command.move,StepSeconds);door.Evaluate(player);Physics2D.SyncTransforms();
  player.Step(command,StepSeconds,echo.Active,oldEcho,echo.Active?echo.Pose.position:Vector2.zero,platforms);
  plate.Evaluate(player,echo);door.Evaluate(player);echo.TickFade(StepSeconds);
  if(State==Mode.Recording){frames.Add(Capture(frames.Count*StepSeconds));if(frames.Count>=501||player.Position.y< -5)FinishRecording();}
  else if(player.Position.y< -5){Restore(initial,true);echo.Clear();State=Memory==null?Mode.Idle:Mode.Ready;}
  Pose();
 }
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
 void BeginRecording(){echo.Clear();Memory=null;path.Show(null);frames.Clear();playbackTime=0;PlaybackBackwards=false;State=Mode.Recording;frames.Add(Capture(0));}
 public void FinishRecording(){
  if(State!=Mode.Recording||frames.Count<2)return;
  var poses=new RecordedFrame[frames.Count];for(int i=0;i<poses.Length;i++)poses[i]=frames[i].pose;
  Memory=new MemorySnapshot(poses,0);path.Show(Memory);rewindTime=Memory.Duration;State=Mode.Rewinding;echo.Clear();holdTime=0;holdTriggered=true;
 }
 void ShowEcho(RecordedFrame pose){echo.Show(pose,true);echo.visual.renderer2D.color=ReplayColor;echo.support.enabled=false;}
 void Play(){State=Mode.Playing;playbackTime=PlaybackBackwards?Memory.Duration:0;ShowEcho(Memory.Sample(playbackTime));}
 WorldFrame Capture(float time){var f=new WorldFrame{player=player.CaptureRewind(),pose=new RecordedFrame(time,player),platforms=new RoomPlatform.RewindState[platforms.Length],crates=new Vector3[crates.Length],sources=plate.Sources,door=door.Open};for(int i=0;i<platforms.Length;i++)f.platforms[i]=platforms[i].CaptureRewind();for(int i=0;i<crates.Length;i++)f.crates[i]=crates[i].transform.position;return f;}
 void Restore(WorldFrame f,bool restart=false){for(int i=0;i<platforms.Length;i++)if(restart||platforms[i].affectedByRewind)platforms[i].RestoreRewind(f.platforms[i]);for(int i=0;i<crates.Length;i++)if(restart||crates[i].affectedByRewind)crates[i].transform.position=f.crates[i];if(restart||plate.affectedByRewind)plate.RestoreRewind(f.sources);if(restart||door.affectedByRewind)door.RestoreRewind(f.door);player.RestoreRewind(f.player);Physics2D.SyncTransforms();}
 void Pose(){visual.Pose(player.Position,player.Facing,player.VisualState,player.VisualFrame,State==Mode.Rewinding?ReplayColor:Color.white);}
 void LateUpdate(){view.rect=GameUI.GameplayViewport(Screen.width,Screen.height);view.orthographicSize=10/view.aspect;view.transform.position=new Vector3(Mathf.Clamp(player.Position.x+3,bounds.xMin+10,bounds.xMax-10),Mathf.Min(4,bounds.yMax-view.orthographicSize),-10);}
 void OnGUI(){hud.Draw(this);}
}
}
