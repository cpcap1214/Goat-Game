using UnityEngine;
namespace OneMoreTime {
// Add only to deep water; waterfall art and shallow decorative water are not lethal.
[RequireComponent(typeof(BoxCollider2D))]
public sealed class WaterHazard:MonoBehaviour {
 public bool causesDrowning=true;public WorldOneSequence drowningPrefab;
 public bool Contains(Vector2 feet){var c=GetComponent<BoxCollider2D>();return causesDrowning&&c.enabled&&c.bounds.Contains(new Vector3(feet.x,feet.y+.3f,transform.position.z));}
 void Reset(){GetComponent<BoxCollider2D>().isTrigger=true;}
}
}
