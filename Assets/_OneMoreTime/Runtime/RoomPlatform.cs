using UnityEngine;
namespace OneMoreTime {
// Plate-controlled one-way surface, advanced only by the room's fixed simulation clock.
public sealed class RoomPlatform:MonoBehaviour {
 [Tooltip("Restore this object's recorded state during a world rewind.")] public bool affectedByRewind=true;
 public PressurePlate plate;public BoxCollider2D solid;public SpriteRenderer[] artwork;public TextMesh counter;
 public float releaseDelay,speed=2;public Vector2 travel;
 public float Remaining{get;private set;}public bool Active{get;private set;}public bool WasActive{get;private set;}
 public Vector2 Delta{get;private set;}public float PreviousTop{get;private set;}
 Vector2 home;float distance;int direction=1;bool initialized;
 public struct RewindState {public Vector2 position,home,delta;public float distance,remaining,previousTop;public int direction;public bool active,wasActive;}
 public RewindState CaptureRewind(){Init();return new RewindState{position=transform.position,home=home,delta=Delta,distance=distance,remaining=Remaining,previousTop=PreviousTop,direction=direction,active=Active,wasActive=WasActive};}
 public void RestoreRewind(RewindState s){initialized=true;transform.position=s.position;home=s.home;Delta=s.delta;distance=s.distance;Remaining=s.remaining;PreviousTop=s.previousTop;direction=s.direction;Active=s.active;WasActive=s.wasActive;Present();}
 void Init(){if(initialized)return;initialized=true;home=transform.position;}
 public void ResetState(){Init();distance=0;direction=1;transform.position=home;Remaining=0;Delta=Vector2.zero;Active=WasActive=false;}
 public void Tick(float dt){Init();var old=(Vector2)transform.position;PreviousTop=old.y+solid.offset.y+solid.size.y/2;WasActive=Active;
  if(plate==null||plate.Pressed)Remaining=releaseDelay;else Remaining=Mathf.Max(0,Remaining-dt);
  if(Remaining<.0001f)Remaining=0;
  Active=plate==null||plate.Pressed||Remaining>0;
  float length=travel.magnitude;
  if(Active&&length>.001f){distance=Mathf.MoveTowards(distance,direction>0?length:0,speed*dt);if(distance>=length)direction=-1;else if(distance<=0)direction=1;transform.position=home+travel.normalized*distance;}
  Delta=(Vector2)transform.position-old;Present();
 }
 void Present(){solid.enabled=Active;
  foreach(var art in artwork)art.color=Active?Color.white:new Color(.55f,.7f,.7f,.22f);
  if(counter!=null){counter.gameObject.SetActive(Remaining>0);counter.text=Remaining>0?Remaining.ToString("0.0"):string.Empty;counter.color=Active?new Color(1,.88f,.5f):new Color(.65f,.7f,.7f,.5f);}
 }
}
}
