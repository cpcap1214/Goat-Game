using UnityEngine;
namespace OneMoreTime {
// Scripted transport: MainRoom's fixed clock owns cabin and passenger movement.
public sealed class RoomLift:MonoBehaviour {
 public MainRoom room;public PressurePlate plate;public Transform cabin,gate;public BoxCollider2D gateCollider;
 public Vector2 dock;public bool arrival;public float travel=7,speed=3;
 public bool Open{get;private set;}public bool Riding{get;private set;}
 public bool ReadyToBoard=>!arrival&&!Riding&&Open&&room.player.Grounded&&Mathf.Abs(room.player.Position.y-dock.y)<.1f&&Mathf.Abs(room.player.Position.x-dock.x)<.45f;
 float distance;Vector3 gateLocal;
 void Awake(){gateLocal=gate.localPosition;if(arrival){Riding=true;distance=-travel;SetPosition();}}
 public void Evaluate(float dt){if(Riding)return;var gateRect=new Rect((Vector2)gateCollider.transform.position+gateCollider.offset-gateCollider.size/2,gateCollider.size);bool occupied=gateRect.Overlaps(new Rect(room.player.Position-new Vector2(room.player.tuning.width/2,0),new Vector2(room.player.tuning.width,room.player.tuning.height)));Open=arrival||(plate!=null&&plate.Pressed)||(Open&&occupied);gateCollider.enabled=!Open;gate.localPosition=Vector3.MoveTowards(gate.localPosition,gateLocal+Vector3.up*(Open?2.5f:0),dt*7);}
 public void Board(){Riding=true;Open=false;distance=0;gateCollider.enabled=false;gate.localPosition=gateLocal;room.player.ResetAt(dock);}
 void SetPosition(){cabin.position=new Vector3(dock.x,dock.y+distance,0);room.player.ResetAt(dock+Vector2.up*distance);}
 public bool Tick(float dt){distance=Mathf.MoveTowards(distance,arrival?0:travel,speed*dt);SetPosition();if(arrival&&distance>=0){Riding=false;Open=true;gateCollider.enabled=false;gate.localPosition=gateLocal+Vector3.up*2.5f;return true;}return !arrival&&distance>=travel;}
 public float Progress=>arrival?1+distance/travel:distance/travel;
}
}
