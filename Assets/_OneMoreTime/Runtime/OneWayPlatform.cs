using UnityEngine;
namespace OneMoreTime {
// Marker consumed by PlayerMotor2D's box casts. No collision on ascent or from the sides.
[RequireComponent(typeof(BoxCollider2D))]
public sealed class OneWayPlatform : MonoBehaviour {}
}
