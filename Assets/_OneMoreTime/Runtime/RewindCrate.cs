using UnityEngine;
namespace OneMoreTime {
// Kinematic floor crate: the debug room's fixed clock owns movement and rewind.
public sealed class RewindCrate:MonoBehaviour,IPlayerCarrier {
 [Tooltip("Restore this object's recorded state during a world rewind.")] public bool affectedByRewind=true;
 public Vector2 CarryDelta{get;private set;}
 public void ReplayTo(Vector3 position,bool carry){CarryDelta=carry?(Vector2)(position-transform.position):Vector2.zero;transform.position=position;}
 public BoxCollider2D solid;public float minX=6,maxX=13;
 public void Tick(PlayerMotor2D player,float move,float dt){
  CarryDelta=Vector2.zero;
  if(Mathf.Abs(move)<.01f||!player.Grounded||Mathf.Abs(player.Position.y-solid.bounds.min.y)>.08f)return;
  float dx=transform.position.x-player.Position.x;
  if(Mathf.Sign(dx)!=Mathf.Sign(move)||Mathf.Abs(dx)>(solid.size.x+player.tuning.width)/2+.14f)return;
  var p=transform.position;p.x=Mathf.Clamp(p.x+move*player.tuning.speed*dt,minX,maxX);transform.position=p;
 }
}
}
