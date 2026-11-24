using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

namespace OneMoreTime {
// Room-local onboarding and transitions. Movement stays on the existing fixed-step motor.
public sealed class MainRoom : MonoBehaviour {
 public const string SceneName="Main_Room01";
 public const string RoomTwoScene="Main_Room02";
 public const string RoomThreeScene="Main_Room03",LiftLandingScene="Main_LiftLanding";
 public enum RoomPhase { Entering, Playing, Exiting, Complete }
 public PlayerMotor2D player;
 public PlayerVisual visual;
 public Camera view;
 public Transform sky;
 Vector2 previous;
 public Font font;
 public WaterHazard[] waterHazards=new WaterHazard[0];
 WorldOneSequence drowning;
 public bool IsDrowning=>drowning!=null;
 public StatueRevival Revival{get;private set;}
 public bool IsReviving=>Revival!=null&&Revival.Playing;
 RewindPlaytest replay;
 public ShutterAbility shutter;
 public RoomMechanisms mechanisms;
 public RoomLift lift;
 public bool movementTutorial=true;
 public bool showTips=true;
 public SpriteRenderer[] statueEyes;
 public RoomCheckpoint[] checkpoints=new RoomCheckpoint[0];
 public RoomCheckpoint ActiveCheckpoint{get;private set;}
 public Collider2D entranceBarrier;
 public Vector2 entrance=new Vector2(-1,0),start=new Vector2(3.2f,0),checkpoint=new Vector2(30.8f,2.8f);
 public Rect roomBounds=new Rect(0,-7,76.4f,27);
 public float jumpPromptX=18.8f,exitX=75.6f,exitFloor=12.4f,deathY=-6.2f;
 public string nextRoomScene;
 public bool AutoTick=true;
 public RoomPhase Phase {get;private set;}
 public bool CheckpointSaved {get;private set;}
 public bool Paused {get;private set;}
 public readonly KeyBindingMenu Controls=new KeyBindingMenu();
 public int Deaths {get;private set;}
 public float Fade {get;private set;}=1;
 public string Prompt {get;private set;}="";
 readonly KeyboardPlayerInput input=new KeyboardPlayerInput();
 float phaseTime,checkpointNotice;
 bool moved,jumped,jumpPromptReached;
 void Awake(){player.Init();player.ResetAt(entrance);if(entranceBarrier!=null)entranceBarrier.enabled=false;previous=player.Position;Phase=RoomPhase.Entering;input.Clear();SetEyes();FrameCamera();Pose();replay=GetComponent<RewindPlaytest>();if(replay!=null){replay.Retried+=BeginRevival;replay.Died+=RecordDeath;}}
 void OnDestroy(){if(replay!=null){replay.Retried-=BeginRevival;replay.Died-=RecordDeath;}Revival?.Cancel();}
 void Update(){
  if(!AutoTick)return;
  if(Controls.IsOpen){Controls.Tick();return;}
  var k=Keyboard.current;
  if(k!=null&&KeyBindings.WasPressed(GameAction.Pause)&&Phase!=RoomPhase.Complete)SetPaused(!Paused);
  if(!Paused&&Phase==RoomPhase.Playing&&!IsReviving)input.Poll(shutter!=null&&shutter.Snapping);
 }
 public void SetPaused(bool paused){Paused=paused;if(!paused&&Controls.IsOpen)Controls.Close();input.Clear();KeyBindings.SuppressUntilRelease();}
 void FixedUpdate(){if(AutoTick)Step(input.Consume(),Time.fixedDeltaTime);}
 public void Step(InputFrame frame,float dt){
  if(Paused||Phase==RoomPhase.Complete)return;
  previous=player.Position;
  if(TickRevival(dt))return;
  if(drowning!=null){drowning.Tick(dt);if(drowning.Finished){Destroy(drowning.gameObject);drowning=null;visual.renderer2D.enabled=true;Respawn();}return;}
  if(shutter!=null&&shutter.Tick(dt)){input.ClearActions();Pose();return;}
  phaseTime+=dt;checkpointNotice=Mathf.Max(0,checkpointNotice-dt);
  if(Phase==RoomPhase.Entering&&lift!=null&&lift.arrival){
   bool arrived=lift.Tick(dt);Fade=Mathf.Clamp01(1-lift.Progress*2);if(arrived){Phase=RoomPhase.Playing;Fade=0;input.Clear();}
  }else if(Phase==RoomPhase.Entering){
   Fade=Mathf.Clamp01(1-phaseTime/.65f);
   player.Step(new InputFrame{move=player.Position.x<start.x? .55f:0},dt);
   if(player.Position.x>=start.x){Phase=RoomPhase.Playing;phaseTime=0;Fade=0;if(entranceBarrier!=null)entranceBarrier.enabled=true;input.Clear();}
  }else if(Phase==RoomPhase.Exiting&&lift!=null&&lift.Riding){
   bool done=lift.Tick(dt);Fade=Mathf.Clamp01((lift.Progress-.45f)/.5f);
   if(done){Phase=RoomPhase.Complete;if(!string.IsNullOrEmpty(nextRoomScene))SceneManager.LoadScene(nextRoomScene);}
  }else if(Phase==RoomPhase.Exiting){
   player.Step(new InputFrame{move=.6f},dt);
   Fade=Mathf.Clamp01((phaseTime-.45f)/.9f);
   if(Fade>=1){Phase=RoomPhase.Complete;if(!string.IsNullOrEmpty(nextRoomScene))SceneManager.LoadScene(nextRoomScene);}
  }else{
   if(frame.retry){Respawn();return;}
   if(Mathf.Abs(frame.move)>.1f)moved=true;
   jumpPromptReached|=player.Position.x>=jumpPromptX;
   if(jumpPromptReached&&frame.jump)jumped=true;
   if(shutter!=null){shutter.CheckPickup();if(frame.freeze&&shutter.Press()){input.ClearActions();Pose();return;}}
   mechanisms?.Tick(dt);lift?.Evaluate(dt);
   player.Step(frame,dt,shutter!=null&&shutter.SupportReady,shutter!=null?shutter.Position:Vector2.zero,shutter!=null?shutter.Position:Vector2.zero,mechanisms!=null?mechanisms.platforms:null);
   shutter?.CheckPickup();mechanisms?.Evaluate();
   if(lift!=null&&lift.ReadyToBoard){lift.Board();Phase=RoomPhase.Exiting;phaseTime=0;input.Clear();}
   CheckForCheckpoint();
   foreach(var water in waterHazards)if(water!=null&&water.drowningPrefab!=null&&water.Contains(player.Position)){RecordDeath();drowning=Instantiate(water.drowningPrefab,player.Position,Quaternion.identity);drowning.autoPlay=false;drowning.target.flipX=player.Facing<0;drowning.Play();visual.renderer2D.enabled=false;input.Clear();return;}
   if(player.Position.y<deathY){RecordDeath();Respawn();return;}
   if(lift==null&&player.Position.x>=exitX&&player.Position.y>=exitFloor-.1f&&player.Grounded){Phase=RoomPhase.Exiting;phaseTime=0;input.Clear();}
  }
  Prompt=!showTips||Phase!=RoomPhase.Playing?"":checkpointNotice>0?"Checkpoint saved":movementTutorial&&jumpPromptReached&&!jumped?"Press [Space] to jump":movementTutorial&&!moved?"Press [A] [D] or [←] [→] to move":player.Grounded&&player.GroundCollider!=null&&player.GroundCollider.TryGetComponent<OneWayPlatform>(out _)?"Press [↓] + [Space] to drop through":mechanisms!=null?mechanisms.Hint:shutter!=null?shutter.Hint:"";
  Pose();
 }
 // Shared by rooms with a custom tutorial clock; checkpoint semantics stay identical.
 public bool CheckForCheckpoint(){var old=checkpoint;bool saved=CheckpointSaved;
   if(checkpoints.Length>0){foreach(var mark in checkpoints)if(!mark.Saved&&Vector2.Distance(player.Position,mark.transform.position)<1.25f){checkpoint=mark.transform.position;ActiveCheckpoint=mark;mark.Save();CheckpointSaved=true;checkpointNotice=3;}}
   else if(statueEyes!=null&&statueEyes.Length>0&&!CheckpointSaved&&Vector2.Distance(player.Position,checkpoint)<1.45f){CheckpointSaved=true;checkpointNotice=3;SetEyes();}
  return CheckpointSaved&&(!saved||checkpoint!=old);
 }
 void Respawn(){player.ResetAt(CheckpointSaved?checkpoint:start);previous=player.Position;mechanisms?.ResetState();input.Clear();Pose();BeginRevival();}
 // Pause-menu death also cancels presentation that would otherwise keep moving or hiding the goat.
 public void RecordDeath(){Deaths++;MainGameProgress.RecordDeath();}
 public void PrepareInstantDeath(bool deathAlreadyCounted=false){
  Deaths++;if(!deathAlreadyCounted&&drowning==null)MainGameProgress.RecordDeath();Revival?.Cancel();
  if(drowning!=null){Destroy(drowning.gameObject);drowning=null;}
  visual.renderer2D.enabled=true;shutter?.CancelSnap();lift?.ResetToDock();
  input.Clear();KeyBindings.SuppressUntilRelease();
 }
 public void ResetFromPause(){
  if(!Paused)return;
  Paused=false;PrepareInstantDeath();Phase=RoomPhase.Playing;phaseTime=0;Fade=0;
  if(entranceBarrier!=null)entranceBarrier.enabled=true;
  Respawn();
 }
 // Only explicit retries/deaths call this. Checkpoint anchor capture and
 // recorded rewind deliberately do not play a resurrection animation.
 public void BeginRevival(){
  if(!CheckpointSaved||Vector2.Distance(player.Position,checkpoint)>.3f)return;
  SpriteRenderer statue=null;StoneCheckpointVisual stone=null;RoomCheckpoint chosen=null;
  foreach(var mark in checkpoints)if(mark!=null&&mark.Saved&&Vector2.Distance(mark.transform.position,checkpoint)<.1f){
   chosen=mark;stone=mark.stoneVisual;
   statue=stone!=null?stone.target:mark.GetComponentInChildren<SpriteRenderer>();break;
  }
  // Rooms 1–2 retain their original gray player-sprite statue.
  if(statue==null&&statueEyes!=null)foreach(var eye in statueEyes)if(eye!=null&&eye.transform.parent!=null){
   foreach(var candidate in eye.transform.parent.GetComponentsInChildren<SpriteRenderer>())if(candidate!=eye){statue=candidate;break;}
   if(statue!=null)break;
  }
  if(statue==null)return;
  if(stone==null){
   stone=statue.GetComponent<StoneCheckpointVisual>();
   if(stone==null){stone=statue.gameObject.AddComponent<StoneCheckpointVisual>();stone.target=statue;stone.frames=new[]{statue.sprite};stone.hasPedestal=false;stone.auxiliaryRenderers=chosen!=null?chosen.eyes:statueEyes;}
   if(chosen!=null)chosen.stoneVisual=stone;
  }
  player.Face(StoneCheckpointVisual.FacingOf(statue));
  if(Revival==null){var effect=new GameObject("Statue revival");effect.transform.SetParent(transform,false);Revival=effect.AddComponent<StatueRevival>();}
  previous=player.Position;input.Clear();KeyBindings.SuppressUntilRelease();
  Revival.Begin(statue,stone,visual,player.Position,player.Facing);
 }
 public bool TickRevival(float dt){
  if(!IsReviving)return false;
  Revival.Tick(dt);input.ClearActions();
  if(!IsReviving){input.Clear();KeyBindings.SuppressUntilRelease();Pose();}
  return true;
 }
 void SetEyes(){foreach(var eye in statueEyes)if(eye!=null)eye.color=CheckpointSaved?new Color(.25f,1,.35f):new Color(.12f,.15f,.15f);}
 void Pose(){visual.Pose(player.Position,player.Facing,player.VisualState,player.VisualFrame,Color.white);}
 void LateUpdate(){
  if(shutter!=null&&shutter.Snapping)return;
  var position=AutoTick&&!Paused?Vector2.Lerp(previous,player.Position,Mathf.Clamp01((Time.time-Time.fixedTime)/Time.fixedDeltaTime)):player.Position;
  visual.transform.position=position;
  if(lift==null||(!lift.Riding&&!(Phase==RoomPhase.Entering&&lift.arrival)))FrameCamera(position);
 }
 void FrameCamera(){FrameCamera(player.Position);}
 void FrameCamera(Vector2 position){
  view.rect=GameUI.GameplayViewport(Screen.width,Screen.height);view.orthographicSize=10f/view.aspect;
  const float halfWidth=10f;
  float x=Mathf.Clamp(position.x+3,roomBounds.xMin+halfWidth,roomBounds.xMax-halfWidth);
  float y=Mathf.Min(Mathf.Max(view.orthographicSize*(3.2f/6.1f),position.y+1f),roomBounds.yMax-view.orthographicSize);
  // Hold the camera at each room edge during the scripted walk-in and walk-out.
  view.transform.position=new Vector3(x,y,-10);
  if(sky!=null)sky.position=new Vector3(x+(roomBounds.center.x-x)*.025f,y+1,3);
 }
 void OnGUI(){
  GUI.matrix=Matrix4x4.identity;
  // Same gameplay viewport as the tutorial, with the surrounding area masked.
  Rect r=view.pixelRect;
  GameUI.Panel(new Rect(0,0,Screen.width,Screen.height-r.yMax),Color.black);
  GameUI.Panel(new Rect(0,Screen.height-r.y,Screen.width,r.y),Color.black);
  GameUI.Panel(new Rect(0,0,r.x,Screen.height),Color.black);
  GameUI.Panel(new Rect(r.xMax,0,Screen.width-r.xMax,Screen.height),Color.black);
  GameUI.Begin(font);
  if(showTips&&Phase==RoomPhase.Playing&&!Paused&&!string.IsNullOrEmpty(Prompt)){
   GameUI.Frame(new Rect(230,440,500,54));GameUI.ControlText(new Rect(245,448,470,38),Prompt,21,GameUI.Cream,TextAnchor.MiddleCenter);
  }
  GUI.matrix=Matrix4x4.identity;if(Fade>0)GameUI.Panel(new Rect(0,0,Screen.width,Screen.height),new Color(0,0,0,Fade));
  shutter?.DrawOverlay();
  GameUI.Begin(font);
  if(Paused&&Controls.IsOpen){Controls.Draw();return;}
  if(Paused){GameUI.Shade();GameUI.Frame(new Rect(300,65,360,425));GameUI.Text(new Rect(330,87,300,40),"Paused",30);
   if(GameUI.ControlButton(new Rect(330,150,300,45),"Continue",true,"Esc"))SetPaused(false);
   if(GameUI.Button(new Rect(330,207,300,45),"Options / Key Bindings"))Controls.Open();
   if(GameUI.Button(new Rect(330,264,300,45),"Reset"))ResetFromPause();
   if(GameUI.Button(new Rect(330,321,300,45),"Main menu"))SceneFlow.Load(0);
   if(Paused){CameraEquipment.Draw(new Rect(314,390,136,88),this);if(MainGameProgress.IsMainJourney)GameUI.PauseDeathCounter(new Vector2(642,476),MainGameProgress.DeathCount);}
  }
  if(Phase==RoomPhase.Complete){GameUI.Text(new Rect(230,200,500,55),"To be continued",32,GameUI.Cream,TextAnchor.MiddleCenter);
   if(GameUI.Button(new Rect(330,300,300,42),"Main menu",true))SceneFlow.Load(0);
  }
 }
}
}
