using UnityEngine;
namespace OneMoreTime {
[RequireComponent(typeof(Rigidbody2D),typeof(BoxCollider2D))]
public sealed class PlayerMotor2D : MonoBehaviour {
 public PlayerTuning tuning; public LayerMask worldMask;
 public Vector2 Position {get;private set;} public Vector2 Velocity {get;private set;}
 public bool Grounded {get;private set;} public int Facing {get;private set;}=1;
 public Collider2D GroundCollider {get;private set;}
 public bool OnEcho {get;private set;}
 public int VisualState{get;private set;}public int VisualFrame{get;private set;}float poseTime,landingTime;
 Collider2D droppedPlatform,carriedCollider;
 Rigidbody2D body; readonly RaycastHit2D[] hits=new RaycastHit2D[16]; float coyote,buffer;
 public struct RewindState {
  public Vector2 position,velocity;public bool grounded,onEcho;public int facing,visualState,visualFrame;
  public Collider2D ground,dropped;public float coyote,buffer,poseTime,landingTime;
 }
 public RewindState CaptureRewind()=>new RewindState{position=Position,velocity=Velocity,grounded=Grounded,onEcho=OnEcho,facing=Facing,visualState=VisualState,visualFrame=VisualFrame,ground=GroundCollider,dropped=droppedPlatform,coyote=coyote,buffer=buffer,poseTime=poseTime,landingTime=landingTime};
 public void RestoreRewind(RewindState s){
  if(body==null)Init();Position=s.position;Velocity=s.velocity;Grounded=s.grounded;OnEcho=s.onEcho;Facing=s.facing;VisualState=s.visualState;VisualFrame=s.visualFrame;
  GroundCollider=s.ground;droppedPlatform=s.dropped;carriedCollider=null;coyote=s.coyote;buffer=s.buffer;poseTime=s.poseTime;landingTime=s.landingTime;body.position=Position;transform.position=Position;
 }
 public void Init(){body=GetComponent<Rigidbody2D>();body.bodyType=RigidbodyType2D.Kinematic;body.gravityScale=0;body.freezeRotation=true;var c=GetComponent<BoxCollider2D>();c.size=new Vector2(tuning.width,tuning.height);c.offset=new Vector2(0,tuning.height/2);Position=body.position;}
 public void ResetAt(Vector2 p){if(body==null)Init();Position=p;Velocity=Vector2.zero;Grounded=OnEcho=false;GroundCollider=null;droppedPlatform=null;coyote=buffer=poseTime=landingTime=0;VisualState=VisualFrame=0;body.position=p;transform.position=p;}
 // A single motor owns every displacement, including platform carry. Echo has no solid side collider.
 public void Step(InputFrame input,float dt, bool echoActive=false,Vector2 echoOld=default,Vector2 echoNew=default,RoomPlatform[] platforms=null){
  if(body==null)Init();
  if(droppedPlatform!=null&&(Position.y+tuning.height<droppedPlatform.bounds.min.y||Mathf.Abs(Position.x-droppedPlatform.bounds.center.x)>(droppedPlatform.bounds.size.x+tuning.width)/2))droppedPlatform=null;
  if(input.drop&&Grounded&&GroundCollider!=null&&GroundCollider.TryGetComponent<OneWayPlatform>(out _)){
   droppedPlatform=GroundCollider;Grounded=OnEcho=false;GroundCollider=null;coyote=buffer=0;input.jump=false;
  }
  if(GroundCollider!=null&&GroundCollider.TryGetComponent<RoomPlatform>(out var moving)){
   if(!moving.Active){Grounded=false;GroundCollider=null;}
   else if(Grounded&&!input.jump&&!input.drop){carriedCollider=GroundCollider;MoveAxis(moving.Delta.x,true);MoveAxis(moving.Delta.y,false);carriedCollider=null;}
  }
  bool wasGrounded=Grounded;float previousY=Velocity.y;
  coyote=Grounded?tuning.coyoteTime:Mathf.Max(0,coyote-dt);
  buffer=input.jump?tuning.bufferTime:Mathf.Max(0,buffer-dt);
  var v=Velocity;v.x=Mathf.MoveTowards(v.x,input.move*tuning.speed,(input.move==0?tuning.deceleration:tuning.acceleration)*dt);
  if(input.move!=0)Facing=input.move>0?1:-1;
  bool takeoff=buffer>0&&coyote>0;
  if(OnEcho){
   if(!echoActive||Mathf.Abs(Position.x-echoOld.x)>(tuning.backWidth+tuning.width)/2)OnEcho=false;
   else if(!takeoff){var wanted=echoNew-echoOld;var start=Position;MoveAxis(wanted.x,true);MoveAxis(wanted.y,false);if((Position-start-wanted).sqrMagnitude>.0001f)OnEcho=false;}
  }
  if(takeoff){v.y=tuning.jumpSpeed;buffer=coyote=0;Grounded=OnEcho=false;}
  else if(!input.held&&v.y>0)v.y*=.5f;
  v.y=Mathf.Max(-tuning.terminalSpeed,v.y-tuning.gravity*dt);
  Vector2 prior=Position;
  if(MoveAxis(v.x*dt,true))v.x=0;
  Grounded=false;GroundCollider=null;
  if(MoveAxis(v.y*dt,false)){if(v.y<0)Grounded=true;v.y=0;}
  if(platforms!=null&&!takeoff){foreach(var platform in platforms){
   if(!platform.Active||platform.solid==droppedPlatform||platform.travel.sqrMagnitude<.001f)continue;
   float top=platform.solid.bounds.max.y;bool overlap=Mathf.Abs(Position.x-platform.solid.bounds.center.x)<(tuning.width+platform.solid.size.x)/2-.005f;
   bool crossing=prior.y>=platform.PreviousTop-.02f&&Position.y<=top+.002f&&(Position.y-prior.y)-platform.Delta.y<=.001f;
   if(overlap&&crossing){carriedCollider=platform.solid;bool blocked=MoveAxis(top-Position.y,false);carriedCollider=null;if(!blocked){Grounded=true;GroundCollider=platform.solid;v.y=0;}}
  }}
  if(echoActive&&!takeoff){
   float oldTop=echoOld.y+tuning.backHeight,newTop=echoNew.y+tuning.backHeight;
   bool overlap=Mathf.Abs(Position.x-echoNew.x)<(tuning.width+tuning.backWidth)/2-.005f;
   bool crossing=prior.y>=oldTop-.02f&&Position.y<=newTop+.002f&&(Position.y-prior.y)-(newTop-oldTop)<=.001f;
   if(overlap&&(OnEcho||crossing)&&Position.y<=newTop+.02f){
    float rise=newTop-Position.y;
    if(!MoveAxis(rise,false)){Grounded=OnEcho=true;v.y=0;GroundCollider=null;}else OnEcho=false;
   }else OnEcho=false;
  }else OnEcho=false;
  Velocity=v;body.position=Position;transform.position=Position;
  if(!wasGrounded&&Grounded&&previousY<-.5f)landingTime=.24f;else landingTime=Mathf.Max(0,landingTime-dt);
  int next=!Grounded?(v.y>0?2:3):Mathf.Abs(v.x)>.1f?1:landingTime>0?4:0;
  if(next!=VisualState){VisualState=next;poseTime=0;}else poseTime+=dt*(next==1?Mathf.Clamp01(Mathf.Abs(v.x)/tuning.speed):1);
  VisualFrame=(int)(poseTime*(next==4?24:next==1?AnimationLibrary.RunFramesPerSecond:next>=2?8:6));
 }
 bool MoveAxis(float distance,bool horizontal){
  if(Mathf.Abs(distance)<.0000001f)return false;
  var dir=horizontal?new Vector2(Mathf.Sign(distance),0):new Vector2(0,Mathf.Sign(distance));
  var filter=new ContactFilter2D();filter.SetLayerMask(worldMask);filter.useTriggers=false;
  // Inset perpendicular dimension prevents adjacent floor tiles catching the side cast.
  Vector2 size=new Vector2(tuning.width-.004f,tuning.height-.004f);
  int n=Physics2D.BoxCast(Position+Vector2.up*tuning.height/2,size,0,dir,filter,hits,Mathf.Abs(distance)+tuning.skin);
  float allowed=Mathf.Abs(distance);Collider2D obstacle=null;
  for(int i=0;i<n;i++){if(hits[i].collider==null||hits[i].collider==droppedPlatform||hits[i].collider==carriedCollider||Vector2.Dot(hits[i].normal,dir)>-.5f)continue;
   // One-way grass only catches feet descending from above its surface.
   // Box casts do not apply PlatformEffector2D rules, so the motor owns this filter.
   if(hits[i].collider.TryGetComponent<OneWayPlatform>(out _)&&(horizontal||distance>0||Position.y<hits[i].collider.bounds.max.y-tuning.skin))continue;
   // Box2D can report a horizontal normal at an adjacent coplanar floor corner.
   if(horizontal&&hits[i].collider.bounds.max.y<=Position.y+tuning.skin)continue;float d=Mathf.Max(0,hits[i].distance-tuning.skin);if(d<=allowed){allowed=d;obstacle=hits[i].collider;}}
  Position+=dir*allowed;if(obstacle!=null&&!horizontal&&distance<0)GroundCollider=obstacle;return obstacle!=null;
 }
}
}
