using UnityEngine;
namespace OneMoreTime {
[CreateAssetMenu(menuName="One More Time/Player Tuning")]
public sealed class PlayerTuning : ScriptableObject {
 public float speed=6, acceleration=60, deceleration=80, jumpSpeed=14, gravity=32, terminalSpeed=18;
 public float coyoteTime=.10f, bufferTime=.12f, width=.70f, height=1, skin=.002f, backHeight=.90f, backWidth=1.10f;
}
}
