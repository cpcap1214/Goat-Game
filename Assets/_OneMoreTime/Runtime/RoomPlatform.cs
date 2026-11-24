using UnityEngine;
namespace OneMoreTime {
// Plate-controlled one-way surface, advanced only by the room's fixed simulation clock.
public sealed class RoomPlatform:MonoBehaviour {
 public enum Motion { PingPong, ClosedRoute, DispatchedRoute, OneWayTravel }
 [Tooltip("Restore this object's recorded state during a world rewind.")] public bool affectedByRewind=true;
 public PressurePlate plate;public BoxCollider2D solid;public SpriteRenderer[] artwork;public TextMesh counter;
 public float releaseDelay,speed=2;public Vector2 travel;
 public Motion motion;
 [Tooltip("Offsets from the initial deck position. Include zero as the first waypoint. Closed routes also connect the last waypoint back to the first.")] public Vector2[] route=new Vector2[0];
 [Tooltip("Full movement cycle in seconds, including the return trip. Zero keeps speed-based movement.")] public float cycleSeconds;
 [Tooltip("OneWayTravel only: seconds for the complete one-way journey. A press starts the journey; release never returns it.")] public float journeySeconds=10;
 [Min(0),Tooltip("DispatchedRoute only: seconds to stay solid after arriving. Zero waits until redispatched.")] public float endLifetime;
 [Tooltip("When assigned, every plate must be pressed at the same time.")] public PressurePlate[] requiredPlates=new PressurePlate[0];
 [Tooltip("Once all required plates are pressed together, keep the platform unlocked until a full reset, including through rewinds.")] public bool latchWhenAllPressed;
 [Tooltip("Stay solid; move to the travel endpoint while the plate is held and return home when released, instead of shuttling back and forth.")] public bool returnWhenReleased;
 public float Remaining{get;private set;}public bool Active{get;private set;}public bool WasActive{get;private set;}
 public float EndRemaining{get;private set;}
 public Vector2 Delta{get;private set;}public float PreviousTop{get;private set;}
 public bool ActivationLatched{get;private set;}public int PressedPlateCount{get;private set;}public bool InTransit{get;private set;}
 public bool JourneyStarted{get;private set;}
 public float JourneyProgress=>travel.sqrMagnitude<.000001f?(JourneyStarted?1:0):Mathf.Clamp01(distance/travel.magnitude);
 Vector2 home;float distance;int direction=1;bool initialized;
 public struct RewindState {public Vector2 position,home,delta;public float distance,remaining,endRemaining,previousTop;public int direction,pressedPlateCount;public bool active,wasActive,inTransit,journeyStarted;}
 public RewindState CaptureRewind(){Init();return new RewindState{position=transform.position,home=home,delta=Delta,distance=distance,remaining=Remaining,endRemaining=EndRemaining,previousTop=PreviousTop,direction=direction,active=Active,wasActive=WasActive,inTransit=InTransit,journeyStarted=JourneyStarted,pressedPlateCount=PressedPlateCount};}
 public void RestoreRewind(RewindState s){initialized=true;transform.position=s.position;home=s.home;Delta=s.delta;distance=s.distance;Remaining=s.remaining;EndRemaining=s.endRemaining;PreviousTop=s.previousTop;direction=s.direction;Active=s.active||ActivationLatched;WasActive=s.wasActive;InTransit=s.inTransit;JourneyStarted=s.journeyStarted;PressedPlateCount=ActivationLatched?RequiredCount:s.pressedPlateCount;Present();}
 int RequiredCount=>requiredPlates==null?0:requiredPlates.Length;
 float Top=>transform.position.y+(solid==null?0:solid.offset.y+solid.size.y/2);
 void Init(){if(initialized)return;initialized=true;home=transform.position;if(returnWhenReleased||motion==Motion.OneWayTravel){Active=WasActive=true;Present();}else if(motion==Motion.DispatchedRoute)Present();}
 // Do not Tick(0): that would still change activation and shuttle direction.
 public void HoldFrame(){Init();Delta=Vector2.zero;PreviousTop=Top;WasActive=Active;}
 public void ResetState(){Init();distance=0;direction=1;transform.position=home;Remaining=EndRemaining=0;Delta=Vector2.zero;ActivationLatched=false;PressedPlateCount=0;InTransit=JourneyStarted=false;Active=WasActive=returnWhenReleased||motion==Motion.OneWayTravel;PreviousTop=Top;Present();}
 // Pool entries keep their GameObjects alive so the fixed clock and rewind snapshots retain them.
 public bool Launch(){Init();if(motion!=Motion.DispatchedRoute||InTransit)return false;distance=EndRemaining=0;transform.position=home+RoutePosition(0,false);InTransit=Active=true;WasActive=false;Delta=Vector2.zero;PreviousTop=Top;Present();return true;}
 bool GatePressed(){
  if(RequiredCount==0)return plate==null||plate.Pressed;
  PressedPlateCount=0;foreach(var required in requiredPlates)if(required!=null&&required.Pressed)PressedPlateCount++;
  if(latchWhenAllPressed&&PressedPlateCount==RequiredCount)ActivationLatched=true;
  if(ActivationLatched)PressedPlateCount=RequiredCount;
  return ActivationLatched||PressedPlateCount==RequiredCount;
 }
 public void Tick(float dt){Init();var old=(Vector2)transform.position;PreviousTop=Top;WasActive=Active;
  if(motion==Motion.DispatchedRoute){
   // Start the endpoint timer only on arrival, using the same rewindable clock
   // as movement. Waiting for replay and pause/reveal therefore freeze it too.
   if(InTransit){
    float routeLength=RouteLength(false);distance=Mathf.Min(routeLength,distance+Mathf.Max(0,speed)*Mathf.Max(0,dt));transform.position=home+RoutePosition(distance,false);
    if(distance>=routeLength){InTransit=false;EndRemaining=Mathf.Max(0,endLifetime);}
   }else if(Active&&endLifetime>0){
    EndRemaining=Mathf.Max(0,EndRemaining-Mathf.Max(0,dt));
    if(EndRemaining<.0001f){EndRemaining=0;Active=false;}
   }
   Delta=(Vector2)transform.position-old;Present();return;
  }
  bool pressed=GatePressed();
  if(motion==Motion.OneWayTravel){
   // The trigger is recorded state, unlike Room 7's permanent puzzle unlock.
   // Rewind can restore an earlier point in the trip; death explicitly resets it.
   if(pressed)JourneyStarted=true;
   Active=true;Remaining=0;
   float journeyLength=travel.magnitude;
   if(JourneyStarted){distance=Mathf.MoveTowards(distance,journeyLength,journeyLength/Mathf.Max(.001f,journeySeconds)*Mathf.Max(0,dt));if(journeyLength-distance<.0001f)distance=journeyLength;transform.position=home+travel.normalized*distance;}
   InTransit=JourneyStarted&&distance<journeyLength;
   Delta=(Vector2)transform.position-old;Present();return;
  }
  if(returnWhenReleased){
   Remaining=0;Active=true;
   distance=Mathf.MoveTowards(distance,pressed?travel.magnitude:0,speed*dt);
   transform.position=home+travel.normalized*distance;
   Delta=(Vector2)transform.position-old;Present();return;
  }
  if(pressed)Remaining=releaseDelay;else Remaining=Mathf.Max(0,Remaining-dt);
  if(Remaining<.0001f)Remaining=0;
  Active=pressed||Remaining>0;
  if(motion==Motion.ClosedRoute){
   float routeLength=RouteLength(true);
   if(Active&&routeLength>.001f){distance=Mathf.Repeat(distance+(cycleSeconds>0?routeLength/cycleSeconds:speed)*dt,routeLength);transform.position=home+RoutePosition(distance,true);}
   Delta=(Vector2)transform.position-old;Present();return;
  }
  float length=travel.magnitude;
  if(Active&&length>.001f){
   if(cycleSeconds>0){distance=Mathf.Repeat(distance+2*length*dt/cycleSeconds,2*length);transform.position=home+travel.normalized*Mathf.PingPong(distance,length);}
   else{distance=Mathf.MoveTowards(distance,direction>0?length:0,speed*dt);if(distance>=length)direction=-1;else if(distance<=0)direction=1;transform.position=home+travel.normalized*distance;}
  }
  Delta=(Vector2)transform.position-old;Present();
 }
 float RouteLength(bool closed){float length=0;if(route==null||route.Length<2)return length;int segments=closed?route.Length:route.Length-1;for(int i=0;i<segments;i++)length+=Vector2.Distance(route[i],route[(i+1)%route.Length]);return length;}
 Vector2 RoutePosition(float along,bool closed){
  if(route==null||route.Length==0)return Vector2.zero;
  int segments=closed?route.Length:route.Length-1;
  for(int i=0;i<segments;i++){Vector2 a=route[i],b=route[(i+1)%route.Length];float length=Vector2.Distance(a,b);if(along<=length&&length>.0001f)return Vector2.Lerp(a,b,along/length);along-=length;}
  return closed?route[0]:route[route.Length-1];
 }
 void Present(){if(solid!=null)solid.enabled=Active;
  if(artwork!=null)foreach(var art in artwork)if(art!=null){art.enabled=motion!=Motion.DispatchedRoute||Active;art.color=Active?Color.white:new Color(.55f,.7f,.7f,.22f);}
  if(counter!=null){
   if(RequiredCount>0){counter.gameObject.SetActive(true);counter.text=PressedPlateCount+"/"+RequiredCount;counter.color=PressedPlateCount==RequiredCount?new Color(.4f,1,.5f):Color.white;}
   else{float countdown=motion==Motion.DispatchedRoute?EndRemaining:Remaining;counter.gameObject.SetActive(countdown>0);counter.text=countdown>0?countdown.ToString("0.0"):string.Empty;counter.color=Active?new Color(1,.88f,.5f):new Color(.65f,.7f,.7f,.5f);}
  }
 }
}
}
