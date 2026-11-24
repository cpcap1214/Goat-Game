using UnityEngine;

namespace OneMoreTime {
// An isolated ability sandbox, not a new Chapter 2 story room. Future room
// directors can reuse FlashlightAbility/ChapterTwoObscurity on their own clock.
public sealed class ChapterTwoFlashlightPlaytest : MonoBehaviour {
 public PlayerMotor2D player;
 public PlayerVisual visual;
 public FlashlightAbility flashlight;
 public Camera view;
 public Font font;
 public Rect bounds=new Rect(0,-3,48,16);
 public Vector2 spawn=new Vector2(3,0);
 public float deathY=-6;
 public bool AutoTick=true;
 public bool Paused {get;private set;}
 public readonly KeyBindingMenu Controls=new KeyBindingMenu();
 readonly KeyboardPlayerInput input=new KeyboardPlayerInput();
 bool activateQueued;

 void Start(){Retry();FrameCamera();}
 void Update(){
  if(!AutoTick)return;
  if(Controls.IsOpen){Controls.Tick();return;}
  if(KeyBindings.WasPressed(GameAction.Pause)){SetPaused(!Paused);return;}
  if(Paused)return;
  input.Poll();activateQueued|=KeyBindings.WasPressed(GameAction.Flashlight);
 }
 void FixedUpdate(){
  if(!AutoTick)return;
  Step(input.Consume(),activateQueued);activateQueued=false;
 }
 public void Step(InputFrame command,bool activate=false){
  if(Paused||player==null||flashlight==null)return;
  if(command.retry){Retry();return;}
  const float dt=RewindPlaytest.StepSeconds;
  player.Step(command,dt);
  if(player.Position.y<deathY){Retry();return;}
  if(activate)flashlight.TryActivate();
  flashlight.Tick(dt);Pose();
 }
 public void Retry(){
  if(player!=null)player.ResetAt(spawn);
  flashlight?.ResetAbility();input.Clear();activateQueued=false;
  KeyBindings.SuppressUntilRelease();Pose();
 }
 public void SetPaused(bool paused){
  Paused=paused;input.Clear();activateQueued=false;KeyBindings.SuppressUntilRelease();
 }
 public void ResetFromPause(){if(!Paused)return;SetPaused(false);Retry();}
 void OnApplicationFocus(bool focus){if(!focus&&Application.isPlaying)SetPaused(true);}
 void Pose(){if(player!=null&&visual!=null)visual.Pose(player.Position,player.Facing,player.VisualState,player.VisualFrame,Color.white);}
 void LateUpdate(){FrameCamera();}
 public void FrameCamera(){
  if(view==null||player==null)return;
  view.rect=GameUI.GameplayViewport(Screen.width,Screen.height);
  view.orthographicSize=10/Mathf.Max(.1f,view.aspect);
  float h=view.orthographicSize;
  float x=bounds.width<=20?bounds.center.x:Mathf.Clamp(player.Position.x+3,bounds.xMin+10,bounds.xMax-10);
  float y=bounds.height<=h*2?bounds.center.y:Mathf.Clamp(player.Position.y+1,bounds.yMin+h,bounds.yMax-h);
  view.transform.position=new Vector3(x,y,-10);
 }
 void OnGUI(){
  if(flashlight==null)return;
  GameUI.Begin(font);
  if(Paused&&Controls.IsOpen){Controls.Draw();return;}
  GameUI.Text(new Rect(28,18,904,32),"CHAPTER 2 · THE LIGHT BEYOND THE FALLS",21,GameUI.Cream);
  string key=KeyBindings.Label(GameAction.Flashlight);
  string state=!flashlight.unlocked?"Locked":flashlight.Active?"LIGHT  "+flashlight.Remaining.ToString("0.0")+"s":flashlight.Ready?"["+key+"]  Flashlight":"Recharging  "+flashlight.CooldownRemaining.ToString("0.0")+"s";
  GameUI.Text(new Rect(28,55,460,26),state,17,GameUI.Gold);
  GameUI.Panel(new Rect(28,87,156,5),new Color(.12f,.15f,.19f,.9f));
  GameUI.Panel(new Rect(28,87,156*flashlight.Charge,5),flashlight.Active?GameUI.Gold:new Color(.63f,.69f,.66f));
  if(Paused){
   GameUI.Shade();GameUI.Frame(new Rect(285,125,390,370));GameUI.Text(new Rect(315,147,330,40),"Flashlight playtest",26);
   if(GameUI.Button(new Rect(315,200,330,40),"Resume",true))SetPaused(false);
   if(GameUI.Button(new Rect(315,250,330,40),"Reset"))ResetFromPause();
   if(GameUI.Button(new Rect(315,300,330,40),"Key Bindings"))Controls.Open();
   if(GameUI.Button(new Rect(315,350,330,40),"Back to main menu"))SceneFlow.Load(0);
   if(Paused)CameraEquipment.Draw(new Rect(299,397,136,88),this);
  }
 }
}
}
