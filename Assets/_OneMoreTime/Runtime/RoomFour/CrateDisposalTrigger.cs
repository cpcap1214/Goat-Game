using UnityEngine;
namespace OneMoreTime {
[RequireComponent(typeof(BoxCollider2D))]
public sealed class CrateDisposalTrigger:MonoBehaviour {
 public CrateSpawner owner;public float boundaryX;public bool disposeToRight;
 [Tooltip("Pull a floating object below the water before returning it to its pool.")]
 public bool sinkBeforeDisposal;
 [Min(.1f)]public float sinkDepth=2.5f,sinkSpeed=3.5f;
 void OnTriggerEnter2D(Collider2D other){var crate=other.GetComponent<FloatingObject2D>();if(crate==null||owner==null)return;if(sinkBeforeDisposal)Check(crate);else owner.Dispose(crate);}
 // Explicit boundary evaluation also works when simulation is stepped by tests/rewind.
 public void Check(FloatingObject2D crate){
  if(crate==null||owner==null||owner.AwaitingReplay||!crate.gameObject.activeSelf||crate.Sinking)return;
  if(!(disposeToRight?crate.transform.position.x>boundaryX:crate.transform.position.x<boundaryX))return;
  if(!sinkBeforeDisposal){owner.Dispose(crate);return;}
  foreach(var owned in owner.pool)if(owned==crate){
   float surface=crate.water!=null?crate.water.surfaceY:crate.transform.position.y;
   crate.BeginSink(boundaryX,surface-Mathf.Max(.1f,sinkDepth),sinkSpeed);return;
  }
 }
}
}
