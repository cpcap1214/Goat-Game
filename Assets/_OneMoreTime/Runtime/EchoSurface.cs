using UnityEngine;
namespace OneMoreTime {
public sealed class EchoSurface : MonoBehaviour {
 public PlayerVisual visual;public SpriteRenderer support;
 float fade;public bool Active{get;private set;}public RecordedFrame Pose{get;private set;}
 public bool SupportPendingClearance{get;private set;}
 PlayerMotor2D capturedPlayer;Rect capturedBody;
 public void Show(RecordedFrame pose,bool active){fade=0;Pose=pose;Active=active;SupportPendingClearance=false;capturedPlayer=null;gameObject.SetActive(true);visual.Pose(pose.position,pose.facing,pose.state,pose.frame,new Color(.4f,.95f,1,active?.72f:.25f));support.enabled=active;}
 // Shutter remains an active plate source while its collision waits for the
 // live goat to leave the captured body completely. Replay does not use this gate.
 public void WaitForPlayerToExit(PlayerMotor2D player){
  capturedPlayer=player;SupportPendingClearance=player!=null;
  if(player==null)return;
  float width=Mathf.Max(player.tuning.width,player.tuning.backWidth),height=Mathf.Max(player.tuning.height,player.tuning.backHeight);
  capturedBody=new Rect(Pose.position.x-width*.5f,Pose.position.y,width,height);
 }
 public bool CanSupport(PlayerMotor2D player){
  if(!Active)return false;
  if(!SupportPendingClearance)return true;
  var owner=capturedPlayer!=null?capturedPlayer:player;if(owner==null)return false;
  var body=new Rect(owner.Position.x-owner.tuning.width*.5f,owner.Position.y,owner.tuning.width,owner.tuning.height);
  float gap=Mathf.Max(.001f,owner.tuning.skin);
  bool clear=body.xMax<capturedBody.xMin-gap||body.xMin>capturedBody.xMax+gap||body.yMax<capturedBody.yMin-gap||body.yMin>capturedBody.yMax+gap;
  if(clear){SupportPendingClearance=false;capturedPlayer=null;}
  return !SupportPendingClearance;
 }
 public void Finish(){Active=false;SupportPendingClearance=false;capturedPlayer=null;support.enabled=false;fade=.22f;}
 public void TickFade(float dt){if(fade<=0)return;fade=Mathf.Max(0,fade-dt);var c=visual.renderer2D.color;c.a=.72f*fade/.22f;visual.renderer2D.color=c;if(fade==0)gameObject.SetActive(false);}
 public void Clear(){fade=0;Active=false;SupportPendingClearance=false;capturedPlayer=null;gameObject.SetActive(false);}
}
}
