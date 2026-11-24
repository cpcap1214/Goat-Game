using UnityEngine;
namespace OneMoreTime {
// Optional fixed-clock world participant; snapshots also include pooled-object lifetimes.
public abstract class RewindParticipant:MonoBehaviour {
 public bool affectedByRewind=true;
 // State-only participants (era/logic) do not own visual object trajectories.
 public virtual bool HasTransformHistory=>true;
 public abstract object Capture();
 public abstract void Restore(object state);
 public abstract void Tick(float dt);
 // Stop simulation without consuming time or evaluating new interactions.
 // Carriers override this to discard displacement left over from a restored frame.
 public virtual void HoldFrame(){}
}
public interface IPlayerCarrier { Vector2 CarryDelta {get;} }
}
