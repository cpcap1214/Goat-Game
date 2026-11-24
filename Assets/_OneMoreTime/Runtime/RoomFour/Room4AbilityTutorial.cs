using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
namespace OneMoreTime {
[DefaultExecutionOrder(100)]
public sealed class Room4AbilityTutorial:MonoBehaviour {
 public const string SceneName="Main_Room05";
 public enum Phase {Arriving,Approach,Breaking,Falling,AwaitRecord,TutorialRewind,Playing,Drowning,Complete,Exiting}
 public MainRoom room;public RewindPlaytest replay;public BreakableBridge bridge;
 public Transform pickup;public Collider2D abilityBlocker;public WaterHazard[] waters;
 public Vector2 tutorialReturnGround=new Vector2(14.5f,4);
 public Vector2 pickupFeet=new Vector2(19.5f,4);
 public float pickupRadius=.6f,waterSurface=0,holdDistance=.08f,holdTolerance=.015f;
 public float zoomTarget=3,zoomInDuration=.65f,zoomOutDuration=.7f,exitX=64;
 public string nextRoomScene="Main_Room06";
 public float slowMotionStartDistance=3.6f,slowMotionDuration=.5f,minimumSpeed=.12f,resumeDuration=.45f;
 public CrateSpawner crateSource;
 public float GameplaySpeed{get;private set;}=1;
 public bool AutoTick=true;
 public Phase CurrentPhase{get;private set;}
 public bool TutorialVisible=>CurrentPhase==Phase.AwaitRecord;
 public string TutorialPrompt=>ControlPrompt.Format(replayHint?"Press [Q] to replay":"Hold [Q] to record and rewind");
 public float TutorialPromptAlpha=>.88f*Mathf.SmoothStep(0,1,promptBlend);
 public bool Unlocked=>MainGameProgress.BasicReplayUnlocked;
 public float CameraSize{get;private set;}
 public float Fade{get;private set;}=1;
 readonly KeyboardPlayerInput input=new KeyboardPlayerInput();
 float slowTime,simulationBudget;bool slowing;InputFrame pending;
 float time,zoomBlend;bool held,releaseRequired;WorldOneSequence drowning;
 float promptBlend;bool replayHint,replayHintDismissed;
 void Awake(){AbilityArt.ReplacePickup(pickup,AbilityArt.ReplayTape);room.AutoTick=false;room.enabled=false;replay.AutoTick=false;replay.externalCamera=true;replay.externalHud=true;CurrentPhase=Phase.Arriving;CameraSize=NormalSize;
  replay.pauseTitle="Room 5";
  if(room.lift==null){room.player.ResetAt(room.entrance);if(room.entranceBarrier!=null)room.entranceBarrier.enabled=false;}
  replay.PauseResetRequested+=ResetFromPause;
  if(crateSource!=null)crateSource.spawningEnabled=Unlocked;
  if(Unlocked){pickup.gameObject.SetActive(false);abilityBlocker.enabled=false;bridge.SetPostAcquisition();}
 }
 void OnDestroy(){if(replay!=null)replay.PauseResetRequested-=ResetFromPause;}
 void ResetFromPause(){
  room.PrepareInstantDeath(CurrentPhase==Phase.Drowning);
  if(drowning!=null){Destroy(drowning.gameObject);drowning=null;}
  input.Clear();held=releaseRequired=slowing=false;pending=default;simulationBudget=slowTime=zoomBlend=0;
  promptBlend=0;replayHint=replayHintDismissed=false;
  GameplaySpeed=1;CameraSize=NormalSize;Fade=0;time=resumeDuration;
  CurrentPhase=Unlocked?Phase.Playing:Phase.Approach;
  if(room.entranceBarrier!=null)room.entranceBarrier.enabled=true;
  if(Unlocked){bridge.SetPostAcquisition();pickup.gameObject.SetActive(false);abilityBlocker.enabled=false;}
  replay.Retry();
  if(crateSource!=null)crateSource.spawningEnabled=Unlocked;
 }
 float NormalSize=>10f/Mathf.Max(.1f,room.view.aspect);
 void Update(){if(!AutoTick)return;if(replay.Controls.IsOpen){replay.Controls.Tick();return;}var k=Keyboard.current;if(k==null)return;
  if(KeyBindings.WasPressed(GameAction.Pause)){replay.SetPaused(!replay.Paused);input.Clear();held=false;pending=default;}
  if(replay.Paused||room.IsReviving)return;
  input.Poll();held=KeyBindings.IsHeld(GameAction.RecordReplay);
 }
 void FixedUpdate(){if(AutoTick)Step(input.Consume(),held,Time.fixedDeltaTime);}
 public void Step(InputFrame command,bool recordHeld,float dt){
  if(replay.Paused||CurrentPhase==Phase.Complete)return;
  if(room.TickRevival(dt)){input.Clear();held=false;pending=default;simulationBudget=0;return;}
  TickPrompt(dt);
  time+=dt;
  if(CurrentPhase==Phase.Exiting){room.player.Step(new InputFrame{move=.55f},dt);Fade=Mathf.Clamp01((time-.25f)/.8f);if(Fade>=1){CurrentPhase=Phase.Complete;if(!string.IsNullOrEmpty(nextRoomScene)&&Application.CanStreamedLevelBeLoaded(nextRoomScene))SceneManager.LoadScene(nextRoomScene);}Pose();return;}
  if(CurrentPhase==Phase.Playing||CurrentPhase==Phase.Drowning||CurrentPhase==Phase.TutorialRewind){
   zoomBlend=Mathf.Max(0,zoomBlend-dt/Mathf.Max(.01f,zoomOutDuration));
   GameplaySpeed=Mathf.Lerp(minimumSpeed,1,Mathf.SmoothStep(0,1,time/Mathf.Max(.01f,resumeDuration)));
  }
  bridge.Tick(dt*GameplaySpeed);
  if(CurrentPhase==Phase.Arriving){
   bool done;
   if(room.lift!=null){done=room.lift.Tick(dt);Fade=1-room.lift.Progress;}
   else{
    float distance=room.start.x-room.player.Position.x;
    room.player.Step(new InputFrame{move=Mathf.Abs(distance)>.12f?Mathf.Sign(distance)*.55f:0},dt);
    Fade=Mathf.Clamp01(1-time/.65f);done=Mathf.Abs(room.start.x-room.player.Position.x)<.2f&&time>=.65f;
   }
   if(done){Fade=0;CurrentPhase=Unlocked?Phase.Playing:Phase.Approach;if(room.entranceBarrier!=null)room.entranceBarrier.enabled=true;replay.SetRetryAnchor();time=Unlocked?resumeDuration:0;}
   Pose();return;
  }
  if(CurrentPhase==Phase.Drowning){drowning.Tick(dt);if(drowning.Finished){Destroy(drowning.gameObject);drowning=null;room.visual.renderer2D.enabled=true;replay.Retry();CurrentPhase=Phase.Playing;}return;}
  if(CurrentPhase==Phase.Approach){
   replay.Step(command,false);
   // Pickup spans the approach route, so jumping cannot skip the acquisition.
   if(Mathf.Abs(room.player.Position.x-pickupFeet.x)<=pickupRadius){MainGameProgress.GrantReplay();pickup.gameObject.SetActive(false);abilityBlocker.enabled=false;CurrentPhase=Phase.Breaking;time=0;input.ClearActions();}
  }else if(CurrentPhase==Phase.Breaking){
   replay.Step(default,false);
   if(time>=bridge.breakDelay){bridge.Break();CurrentPhase=Phase.Falling;time=0;}
  }else if(CurrentPhase==Phase.Falling){
   float distance=room.player.Position.y-waterSurface;
   if(distance<=slowMotionStartDistance)slowing=true;
   if(slowing){slowTime+=dt;GameplaySpeed=Mathf.Lerp(1,minimumSpeed,Mathf.SmoothStep(0,1,slowTime/Mathf.Max(.01f,slowMotionDuration)));}
   // Integrate the existing motor with a smaller timestep. Never move the goat upward to freeze it.
   float fallStep=dt*GameplaySpeed;
   float remaining=Mathf.Max(0,distance-holdDistance);
   fallStep=Mathf.Min(fallStep,remaining/Mathf.Max(.01f,Mathf.Abs(room.player.Velocity.y)+room.player.tuning.gravity*fallStep));
   if(fallStep>0)room.player.Step(default,fallStep);
   zoomBlend=Mathf.Clamp01(zoomBlend+dt/Mathf.Max(.01f,zoomInDuration));
   if(room.player.Position.y<=waterSurface+holdDistance+holdTolerance){var state=room.player.CaptureRewind();state.velocity=Vector2.zero;GameplaySpeed=0;room.player.RestoreRewind(state);releaseRequired=recordHeld;CurrentPhase=Phase.AwaitRecord;time=0;}
  }else if(CurrentPhase==Phase.AwaitRecord){
   zoomBlend=Mathf.Clamp01(zoomBlend+dt/Mathf.Max(.01f,zoomInDuration));
   if(!recordHeld)releaseRequired=false;
   replay.ChargeWhileSuspended(!releaseRequired&&recordHeld);
   if(replay.State==RewindPlaytest.Mode.Recording){replay.BeginImaginaryRewind(tutorialReturnGround,pickupFeet);CurrentPhase=Phase.TutorialRewind;time=0;simulationBudget=0;pending=default;input.ClearActions();}
  }else if(CurrentPhase==Phase.TutorialRewind){
   replay.Step(default,false);
   if(replay.State==RewindPlaytest.Mode.Ready){replay.SetRetryAnchor();CurrentPhase=Phase.Playing;time=resumeDuration;GameplaySpeed=1;replayHint=true;replayHintDismissed=false;promptBlend=0;if(crateSource!=null)crateSource.spawningEnabled=true;input.ClearActions();}
  }else{
   if(room.shutter!=null){
    if(room.shutter.Tick(dt)){input.ClearActions();Pose();return;}
    if(command.freeze&&replay.TryUseShutter(room.shutter)){input.ClearActions();Pose();return;}
   }
   // Keep recording, platforms and crates on the same fixed simulation clock during recovery.
   pending.move=command.move;pending.held=command.held;pending.jump|=command.jump;pending.drop|=command.drop;pending.retry|=command.retry;
   simulationBudget+=dt*GameplaySpeed;
   while(simulationBudget+0.00001f>=RewindPlaytest.StepSeconds){
    replay.Step(pending,Unlocked&&recordHeld,false,false,room.shutter!=null?room.shutter.echo:null);
    pending.jump=pending.drop=pending.retry=false;simulationBudget-=RewindPlaytest.StepSeconds;
    if(room.IsReviving){pending=default;simulationBudget=0;input.Clear();return;}
   }
   if(room.CheckForCheckpoint())SetCheckpointRetryAnchor();
   if(replay.State==RewindPlaytest.Mode.Recording&&room.shutter!=null)room.shutter.echo.Clear();
   if(replay.State!=RewindPlaytest.Mode.Rewinding){
    foreach(var water in waters)if(water.Contains(room.player.Position)){
     room.RecordDeath();
     if(replay.State==RewindPlaytest.Mode.Recording){replay.FinishRecording();return;}
     drowning=Instantiate(water.drowningPrefab,room.player.Position,Quaternion.identity);drowning.autoPlay=false;drowning.Play();room.visual.renderer2D.enabled=false;CurrentPhase=Phase.Drowning;return;
    }
   }
   if(room.player.Position.x>=exitX&&room.player.Grounded){CurrentPhase=string.IsNullOrEmpty(nextRoomScene)?Phase.Complete:Phase.Exiting;time=0;input.ClearActions();}
  }
  Pose();
 }
 void TickPrompt(float dt){
  if(replayHint&&CurrentPhase==Phase.Playing&&(replay.State==RewindPlaytest.Mode.Playing||replay.State==RewindPlaytest.Mode.Recording))replayHintDismissed=true;
  bool show=TutorialVisible||(replayHint&&!replayHintDismissed&&CurrentPhase==Phase.Playing&&replay.AwaitingReplay);
  promptBlend=Mathf.MoveTowards(promptBlend,show?1:0,dt/(show?.7f:.3f));
 }
 void SetCheckpointRetryAnchor(){var live=room.player.CaptureRewind();room.player.ResetAt(room.checkpoint);replay.SetRetryAnchor();room.player.RestoreRewind(live);}
 void Pose(){room.visual.Pose(room.player.Position,room.player.Facing,room.player.VisualState,room.player.VisualFrame,replay.State==RewindPlaytest.Mode.Rewinding?RewindPlaytest.ReplayColor:Color.white);}
 void LateUpdate(){
  var camera=room.view;camera.rect=GameUI.GameplayViewport(Screen.width,Screen.height);
  CameraSize=Mathf.Lerp(NormalSize,zoomTarget,Mathf.SmoothStep(0,1,zoomBlend));camera.orthographicSize=CameraSize;
  float half=CameraSize*camera.aspect;
  Vector2 normal=new Vector2(Mathf.Clamp(room.player.Position.x+3,room.roomBounds.xMin+half,room.roomBounds.xMax-half),Mathf.Clamp(room.player.Position.y+1,room.roomBounds.yMin+CameraSize,room.roomBounds.yMax-CameraSize));
  Vector2 close=room.player.Position+Vector2.up*.7f;
  var center=Vector2.Lerp(normal,close,Mathf.SmoothStep(0,1,zoomBlend));camera.transform.position=new Vector3(center.x,center.y,-10);
 }
 void OnGUI(){
  if(!replay.Paused&&Unlocked&&(CurrentPhase==Phase.Playing))replay.DrawHUD();
  room.shutter?.DrawOverlay();
  GameUI.Begin(room.font);
  if(!replay.Paused&&TutorialPromptAlpha>0)GameUI.Text(new Rect(70,455,820,45),TutorialPrompt,23,new Color(1,1,1,TutorialPromptAlpha),TextAnchor.MiddleCenter);
  if(CurrentPhase==Phase.Complete){GameUI.Frame(new Rect(240,180,480,180));GameUI.Text(new Rect(265,200,430,50),"Room 5 complete — more to come",25,GameUI.Cream,TextAnchor.MiddleCenter);if(GameUI.Button(new Rect(300,280,360,45),"Main menu"))SceneFlow.Load(0);}
  if(Fade>0){GUI.matrix=Matrix4x4.identity;GameUI.Panel(new Rect(0,0,Screen.width,Screen.height),new Color(0,0,0,Fade));}
  if(replay.Paused)replay.DrawHUD();
 }
}
}
