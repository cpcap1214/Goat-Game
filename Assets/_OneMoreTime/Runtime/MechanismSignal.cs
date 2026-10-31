using UnityEngine;
namespace OneMoreTime {
public sealed class MechanismSignal:MonoBehaviour {
 public PressurePlate plate;public LineRenderer wire;public LinkedDoor door;public SpriteRenderer[] bars;
 void LateUpdate(){if(wire!=null)wire.startColor=wire.endColor=plate.Pressed?new Color(.48f,.91f,.7f,.85f):new Color(.88f,.7f,.4f,.32f);if(bars!=null)foreach(var b in bars)b.color=door.Open?new Color(.48f,.91f,.7f,.12f):new Color(.96f,.78f,.47f);}
}
}
