using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

namespace OneMoreTime {
// Main-room orchestration only. RewindPlaytest remains the sole simulation clock
// for the motor, recorded platforms and floating objects.
[DefaultExecutionOrder(100)]
public sealed class ChapterOneRoom : RewindParticipant {
 public enum Phase { Entering, Playing, Exiting, Drowning, Complete }
 public MainRoom room;
 public RoomPlatformReveal platformReveal;
 public MovingRoomCheckpoint movingCheckpoint;
 public bool enterFromBelow;
 public RewindPlaytest replay;
 public int roomNumber=6;
 public string roomTitle="Broken bridges",nextRoomScene="Main_Room07";
 public Rect exitArea=new Rect(50,0,4,4);
 public bool chapterFinale,filterEnabled=true,replayUpgradeOnEntry,AutoTick=true;
 public RoomTrapdoor trapdoor;
 public bool exitUpwards;
 public Transform replayUpgradePickup;
 public float upgradePickupRadius=1.2f;
 public ChapterEraGroup[] eraGroups=new ChapterEraGroup[0];
 public WaterHazard[] waters=new WaterHazard[0];
 public PressurePlate[] plates=new PressurePlate[0];
 public LinkedDoor[] doors=new LinkedDoor[0];
 public Phase CurrentPhase {get;private set;}
 public bool Past {get;private set;}
 public bool DebugAbilities {get;private set;}
 public bool FilterAvailable=>DebugAbilities&&filterEnabled;
 // Riverworks now precedes the acquisition. A debug launch of Room 4 still
 // teaches Shutter only, regardless of any previous session's replay flag.
 public bool ReplayAvailable=>roomNumber>4&&MainGameProgress.BasicReplayUnlocked;
 public float Fade {get;private set;}=1;
 public PastLensEffect Lens {get;}=new PastLensEffect();
 public string Notice {get;private set;}="";
 public override bool HasTransformHistory=>false;
 readonly KeyboardPlayerInput input=new KeyboardPlayerInput();
 float phaseTime,noticeTime,shiftTime;
 bool recordHeld,switchQueued,directionQueued,shiftTracking;
 WorldOneSequence drowning;

 sealed class State {
  public bool past;
  public int[] plateSources;
  public bool[] openDoors;
 }

 void Awake(){
  DebugAbilities=MainGameProgress.IsDebugMode;
  if(replayUpgradePickup!=null)replayUpgradePickup.gameObject.SetActive(DebugAbilities);
  if(DebugAbilities)AbilityArt.ReplacePickup(replayUpgradePickup,AbilityArt.ArcaneReplayTape);
  if(room==null||replay==null)return;
  room.AutoTick=false;room.enabled=false;
  replay.AutoTick=false;replay.externalCamera=true;replay.externalHud=true;
  replay.deathY=room.deathY;
  replay.PauseResetRequested+=ResetFromPause;
  replay.Died+=ResetPlatformReveal;
  CurrentPhase=Phase.Entering;
 }
 void Start(){
  movingCheckpoint?.Initialize();
  if(roomNumber>4)MainGameProgress.GrantReplay();
  replay.SetReplayUpgradeAvailable(false);
  if(ReplayAvailable&&DebugAbilities&&replayUpgradeOnEntry)MainGameProgress.GrantReplayUpgrade();
  if(ReplayAvailable&&DebugAbilities&&MainGameProgress.ReplayUpgradeUnlocked){replay.UnlockReplayUpgrade();if(replayUpgradePickup!=null)replayUpgradePickup.gameObject.SetActive(false);}
  if(!DebugAbilities)ConfigureChapterStreams();
  replay.hudFooter=!ReplayAvailable?"E: shutter   Down + Jump: drop":FilterAvailable?"F: past / present   E: shutter   Hold Q: record   Tap Q: finish / replay   Down + Jump: drop":"E: shutter   Hold Q: record   Tap Q: finish / replay   Down + Jump: drop";
  ApplyEra();
  if(room.lift!=null&&room.lift.arrival)room.lift.Tick(0);
  else room.player.ResetAt(room.entrance);
  if(room.entranceBarrier!=null)room.entranceBarrier.enabled=false;
  FrameCamera();Pose();
 }
 void OnDestroy(){if(replay!=null){replay.PauseResetRequested-=ResetFromPause;replay.Died-=ResetPlatformReveal;}Lens.Dispose();}
 void ResetPlatformReveal(){platformReveal?.ResetReveal();}
 void MarkMovingCheckpointDeath(){movingCheckpoint?.MarkDeath();ResetPlatformReveal();}
 void ResetFromPause(){
  MarkMovingCheckpointDeath();room.PrepareInstantDeath(CurrentPhase==Phase.Drowning);
  if(drowning!=null){Destroy(drowning.gameObject);drowning=null;}
  CurrentPhase=Phase.Playing;phaseTime=0;Fade=0;
  if(room.entranceBarrier!=null)room.entranceBarrier.enabled=true;
  input.Clear();recordHeld=false;switchQueued=directionQueued=shiftTracking=false;
  replay.Retry();
 }
 void Update(){
  if(!AutoTick)return;
  if(replay.Controls.IsOpen){replay.Controls.Tick();return;}
  var k=Keyboard.current;if(k==null)return;
  if(KeyBindings.WasPressed(GameAction.Pause)&&CurrentPhase!=Phase.Complete){replay.SetPaused(!replay.Paused);input.Clear();recordHeld=false;switchQueued=directionQueued=false;shiftTracking=false;}
  if(replay.Paused||CurrentPhase!=Phase.Playing||room.IsReviving||trapdoor!=null&&trapdoor.Revealing||platformReveal!=null&&platformReveal.Revealing)return;
  input.Poll(room.shutter!=null&&room.shutter.Snapping);
  recordHeld=ReplayAvailable&&KeyBindings.IsHeld(GameAction.RecordReplay);switchQueued|=KeyBindings.WasPressed(GameAction.Filter);
  if(ReplayAvailable&&KeyBindings.WasPressed(GameAction.ReplayDirection)){shiftTracking=true;shiftTime=0;}
  if(ReplayAvailable&&shiftTracking){shiftTime+=Time.unscaledDeltaTime;if(KeyBindings.WasReleased(GameAction.ReplayDirection)){directionQueued|=shiftTime<.25f;shiftTracking=false;}}
 }
 void FixedUpdate(){
  if(!AutoTick)return;
  Step(input.Consume(),recordHeld,switchQueued,directionQueued,ReplayAvailable&&KeyBindings.IsHeld(GameAction.ReplayDirection));
  switchQueued=directionQueued=false;
 }

 // The public fixed step is also used by deterministic room traversal checks.
 public void Step(InputFrame command,bool rewindHeld,bool toggleEra=false,bool toggleDirection=false,bool holdPlayback=false){
  if(replay.Paused||CurrentPhase==Phase.Complete)return;
  const float dt=RewindPlaytest.StepSeconds;
  if(CurrentPhase==Phase.Playing&&trapdoor!=null){
   // The live trigger is outside rewind. Once revealing, all other clocks and
   // mechanism evaluation wait until the camera has returned to the goat.
   if(!trapdoor.Revealing)EvaluateMechanisms();
   if(trapdoor.Tick(dt)){
    input.Clear();recordHeld=false;switchQueued=directionQueued=shiftTracking=false;
    Pose();return;
   }
  }
  if(CurrentPhase==Phase.Playing&&platformReveal!=null&&replay.State!=RewindPlaytest.Mode.Rewinding){
   if(!platformReveal.Revealing)EvaluateMechanisms();
   if(platformReveal.Tick(dt)){input.Clear();recordHeld=false;switchQueued=directionQueued=shiftTracking=false;Pose();return;}
  }
  phaseTime+=dt;noticeTime=Mathf.Max(0,noticeTime-dt);if(noticeTime==0)Notice="";
  Lens.Tick(dt);
  if(room.TickRevival(dt)){input.Clear();recordHeld=false;switchQueued=directionQueued=shiftTracking=false;return;}
  if(CurrentPhase==Phase.Entering){
   bool arrived;
   if(room.lift!=null&&room.lift.arrival){arrived=room.lift.Tick(dt);Fade=1-room.lift.Progress;}
   else if(enterFromBelow){
    // Rise outside the bank first, then arc over its edge onto the landing.
    // This presentation path never runs the motor through the solid bank wall.
    const float duration=1.15f,gravity=22;
    float t=Mathf.Min(phaseTime,duration),launch=(room.start.y-room.entrance.y+.5f*gravity*duration*duration)/duration;
    float sideways=Mathf.SmoothStep(0,1,Mathf.InverseLerp(.4f,.95f,t));
    var state=room.player.CaptureRewind();
    state.position=new Vector2(Mathf.Lerp(room.entrance.x,room.start.x,sideways),room.entrance.y+launch*t-.5f*gravity*t*t);
    state.velocity=new Vector2(0,launch-gravity*t);state.grounded=state.onEcho=false;state.ground=null;state.visualState=2;state.visualFrame=(int)(phaseTime*8);
    room.player.RestoreRewind(state);
    Fade=Mathf.Clamp01(1-phaseTime/.35f);arrived=phaseTime>=duration;
    if(arrived)room.player.ResetAt(room.start);
   }else {
    float distance=room.start.x-room.player.Position.x;
    room.player.Step(new InputFrame{move=Mathf.Abs(distance)>.12f?Mathf.Sign(distance)*.55f:0},dt);
    Fade=Mathf.Clamp01(1-phaseTime/.65f);arrived=Mathf.Abs(distance)<.2f&&phaseTime>=.65f;
   }
   if(arrived){CurrentPhase=Phase.Playing;phaseTime=0;Fade=0;if(room.entranceBarrier!=null)room.entranceBarrier.enabled=true;EvaluateMechanisms();replay.SetRetryAnchor();input.ClearActions();}
   Pose();return;
  }
  if(CurrentPhase==Phase.Exiting){
   if(room.lift!=null&&!room.lift.arrival&&room.lift.Riding){bool done=room.lift.Tick(dt);Fade=Mathf.Clamp01((room.lift.Progress-.4f)/.6f);if(done)CompleteTransition();}
   else if(exitUpwards){
    var state=room.player.CaptureRewind();state.position+=Vector2.up*(3*dt);state.velocity=Vector2.up*3;
    state.grounded=state.onEcho=false;state.ground=null;state.visualState=2;state.visualFrame=(int)(phaseTime*8);
    room.player.RestoreRewind(state);Fade=Mathf.Clamp01((phaseTime-.25f)/.8f);if(Fade>=1)CompleteTransition();
   }
   else {room.player.Step(new InputFrame{move=.55f},dt);Fade=Mathf.Clamp01((phaseTime-.25f)/.8f);if(Fade>=1)CompleteTransition();}
   Pose();return;
  }
  if(CurrentPhase==Phase.Drowning){
   if(drowning!=null){drowning.Tick(dt);if(!drowning.Finished)return;Destroy(drowning.gameObject);drowning=null;}
   room.visual.renderer2D.enabled=true;replay.Retry();CurrentPhase=Phase.Playing;phaseTime=0;Pose();return;
  }
  if(room.shutter!=null){
   if(room.shutter.Tick(dt)){input.ClearActions();Pose();return;}
   if(command.freeze&&replay.TryUseShutter(room.shutter)){input.ClearActions();Pose();return;}
  }
  if(toggleEra&&replay.State!=RewindPlaytest.Mode.Rewinding)TrySwitchEra();
  EvaluateMechanisms();
  var previousMode=replay.State;
  replay.Step(command,ReplayAvailable&&rewindHeld,ReplayAvailable&&DebugAbilities&&toggleDirection,ReplayAvailable&&DebugAbilities&&holdPlayback,room.shutter!=null?room.shutter.echo:null);
  movingCheckpoint?.SyncCheckpoint();
  if(room.IsReviving){input.Clear();return;}
  if(previousMode!=RewindPlaytest.Mode.Recording&&replay.State==RewindPlaytest.Mode.Recording&&room.shutter!=null)room.shutter.echo.Clear();
  if(replay.State==RewindPlaytest.Mode.Rewinding){Pose();return;}
  EvaluateMechanisms();
  if(room.CheckForCheckpoint()){
   movingCheckpoint?.SyncCheckpoint();
   // Capture the safe checkpoint world without moving the live goat or changing its momentum.
   var live=room.player.CaptureRewind();room.player.ResetAt(room.checkpoint);replay.SetRetryAnchor();room.player.RestoreRewind(live);Say("Checkpoint saved");
  }
  if(ReplayAvailable&&DebugAbilities&&replayUpgradePickup!=null&&replayUpgradePickup.gameObject.activeSelf&&Vector2.Distance(room.player.Position,replayUpgradePickup.position)<=upgradePickupRadius){
   MainGameProgress.GrantReplayUpgrade();replay.UnlockReplayUpgrade();replayUpgradePickup.gameObject.SetActive(false);Say("Reverse replay unlocked — tap Left Shift to reverse; hold to pause");
  }
  foreach(var water in waters)if(water!=null&&water.Contains(room.player.Position)){
   MarkMovingCheckpointDeath();room.RecordDeath();
   if(replay.State==RewindPlaytest.Mode.Recording){replay.FinishRecording();Pose();return;}
   if(water.drowningPrefab!=null){drowning=Instantiate(water.drowningPrefab,room.player.Position,Quaternion.identity);drowning.autoPlay=false;drowning.target.flipX=room.player.Facing<0;drowning.Play();room.visual.renderer2D.enabled=false;CurrentPhase=Phase.Drowning;input.ClearActions();}
   else replay.Retry();
   return;
  }
  room.lift?.Evaluate(dt);
  bool liftExit=room.lift!=null&&!room.lift.arrival;
  bool passageExit=exitArea.Contains(room.player.Position)&&(exitUpwards?(trapdoor==null||trapdoor.Opened):room.player.Grounded);
  if(liftExit?room.lift.ReadyToBoard:passageExit){
   if(liftExit)room.lift.Board();CurrentPhase=Phase.Exiting;phaseTime=0;input.ClearActions();
  }
  Pose();
 }
 public bool TrySwitchEra(){
  if(!FilterAvailable)return false;
  var body=new Rect(room.player.Position+new Vector2(-room.player.tuning.width*.5f+.01f,.03f),new Vector2(room.player.tuning.width-.02f,room.player.tuning.height-.05f));
  foreach(var group in eraGroups)if(group!=null&&group.activeInPast!=Past&&group.WouldEmbed(body)){Say("Move clear of the bridge before switching the filter");return false;}
  Past=!Past;ApplyEra();Lens.Switch(Past);return true;
 }
 // The campaign travels downstream with basic replay. The upstream/reverse
 // puzzle remains available when this room is explicitly opened in Debug Mode.
 void ConfigureChapterStreams(){
  foreach(var participant in replay.participants)if(participant is CrateSpawner source&&source.water!=null&&source.spawnPoint!=null){
   // A whirlpool source has an authored entry/drain inside its water channel.
   if(source.activationPlate!=null||source.disposal!=null&&source.disposal.sinkBeforeDisposal)continue;
   var current=source.water;current.velocity=Mathf.Max(.1f,Mathf.Abs(current.velocity));
   source.initialHorizontalVelocity=current.velocity;
   source.spawnPoint.position=new Vector3(current.left+1,current.surfaceY+.28f,source.spawnPoint.position.z);
   if(source.disposal!=null){source.disposal.disposeToRight=true;source.disposal.boundaryX=current.right+2;source.disposal.transform.position=new Vector3(current.right+3,current.surfaceY,0);}
  }
 }
 public void ApplyEra(){foreach(var group in eraGroups)if(group!=null)group.Apply(FilterAvailable?Past:group.activeInPast);Physics2D.SyncTransforms();}
 void EvaluateMechanisms(){
  foreach(var plate in plates){
   if(plate==null||affectedByRewind&&replay.AwaitingReplay&&plate.affectedByRewind)continue;
   int sources=plate.Accepts(room.player.Position,room.player.Grounded)?1:0;
   if(replay.echo.Active&&plate.Accepts(replay.echo.Pose.position,replay.echo.Pose.grounded))sources++;
   var frozen=room.shutter!=null?room.shutter.echo:null;
   if(frozen!=null&&frozen.Active&&plate.Accepts(frozen.Pose.position,frozen.Pose.grounded))sources++;
   plate.RestoreRewind(sources);
  }
  foreach(var door in doors)if(door!=null&&!(affectedByRewind&&replay.AwaitingReplay&&door.affectedByRewind))door.Evaluate(room.player);
 }
 public override object Capture(){
  var s=new State{past=Past,plateSources=new int[plates.Length],openDoors=new bool[doors.Length]};
  for(int i=0;i<plates.Length;i++)s.plateSources[i]=plates[i]!=null?plates[i].Sources:0;
  for(int i=0;i<doors.Length;i++)s.openDoors[i]=doors[i]!=null&&doors[i].Open;
  return s;
 }
 public override void Restore(object value){
  var s=(State)value;bool changed=Past!=s.past;Past=FilterAvailable&&s.past;ApplyEra();if(changed)Lens.Switch(Past);
  for(int i=0;i<plates.Length;i++)if(plates[i]!=null&&plates[i].affectedByRewind)plates[i].RestoreRewind(s.plateSources[i]);
  for(int i=0;i<doors.Length;i++)if(doors[i]!=null&&doors[i].affectedByRewind)doors[i].RestoreRewind(s.openDoors[i]);
 }
 // Registered as a participant solely for state capture; never recursively advance the clock.
 public override void Tick(float dt){}
 void Say(string text){Notice=text;noticeTime=4;}
 void CompleteTransition(){
  CurrentPhase=Phase.Complete;Fade=1;
  if(!chapterFinale&&!string.IsNullOrEmpty(nextRoomScene)&&Application.CanStreamedLevelBeLoaded(nextRoomScene))SceneManager.LoadScene(nextRoomScene);
 }
 void Pose(){room.visual.Pose(room.player.Position,room.player.Facing,room.player.VisualState,room.player.VisualFrame,replay.State==RewindPlaytest.Mode.Rewinding?RewindPlaytest.ReplayColor:Color.white);}
 void LateUpdate(){if(room!=null&&room.view!=null)FrameCamera();}
 void FrameCamera(){
  var camera=room.view;camera.rect=GameUI.GameplayViewport(Screen.width,Screen.height);camera.orthographicSize=10f/Mathf.Max(.1f,camera.aspect);
  var bounds=room.roomBounds;float h=camera.orthographicSize;
  float x=bounds.width<=20?bounds.center.x:Mathf.Clamp(room.player.Position.x+3,bounds.xMin+10,bounds.xMax-10);
  float y=bounds.height<=h*2?bounds.center.y:Mathf.Clamp(room.player.Position.y+1,bounds.yMin+h,bounds.yMax-h);
  camera.transform.position=new Vector3(x,y,-10);
  if(trapdoor!=null){trapdoor.ApplyCamera(camera,camera.transform.position,camera.orthographicSize);x=camera.transform.position.x;y=camera.transform.position.y;}
  platformReveal?.ApplyCamera(camera,camera.transform.position,bounds);
  // Reveals and upward exits must respect the same visible room boundary.
  // The goat can jump above it, but the camera never exposes the wall ends.
  h=camera.orthographicSize;var position=camera.transform.position;
  position.y=bounds.height<=h*2?bounds.center.y:Mathf.Clamp(position.y,bounds.yMin+h,bounds.yMax-h);
  camera.transform.position=position;x=position.x;y=position.y;
  if(room.sky!=null)room.sky.position=new Vector3(x+(bounds.center.x-x)*.025f,y+1,3);
 }
 void OnGUI(){
  if(room==null||replay==null)return;
  GUI.matrix=Matrix4x4.identity;Rect r=room.view.pixelRect;
  GameUI.Panel(new Rect(0,0,Screen.width,Screen.height-r.yMax),Color.black);GameUI.Panel(new Rect(0,Screen.height-r.y,Screen.width,r.y),Color.black);
  GameUI.Panel(new Rect(0,0,r.x,Screen.height),Color.black);GameUI.Panel(new Rect(r.xMax,0,Screen.width-r.xMax,Screen.height),Color.black);
  GameUI.Begin(room.font);Lens.Draw();
  if(CurrentPhase==Phase.Playing&&!replay.Paused&&ReplayAvailable)replay.DrawHUD();
  room.shutter?.DrawOverlay();GameUI.Begin(room.font);
  if(!string.IsNullOrEmpty(Notice))GameUI.ControlText(new Rect(60,440,840,35),Notice,20,GameUI.Cream,TextAnchor.MiddleCenter);
  else if(CurrentPhase==Phase.Entering||CurrentPhase==Phase.Playing&&phaseTime<3)GameUI.Text(new Rect(50,400,860,40),"Room "+roomNumber+" · "+roomTitle,24,GameUI.Cream,TextAnchor.MiddleCenter);
  if(Fade>0){GUI.matrix=Matrix4x4.identity;GameUI.Panel(new Rect(0,0,Screen.width,Screen.height),new Color(0,0,0,Fade));}
  // Pause remains usable even when the entry/exit fade is fully black.
  if(replay.Paused){replay.DrawHUD();return;}
  if(CurrentPhase==Phase.Complete){
   GameUI.Begin(room.font);GameUI.Text(new Rect(150,180,660,50),chapterFinale?"Chapter 1 complete":"Room "+roomNumber+" complete",32,GameUI.Cream,TextAnchor.MiddleCenter);
   GameUI.Text(new Rect(150,235,660,50),chapterFinale?"Beyond the waterfall, Chapter 2 awaits.":"The next room is still being developed.",22,GameUI.Muted,TextAnchor.MiddleCenter);
   if(GameUI.Button(new Rect(330,320,300,45),"Main menu",true))SceneFlow.Load(0);
  }
 }
}
}
