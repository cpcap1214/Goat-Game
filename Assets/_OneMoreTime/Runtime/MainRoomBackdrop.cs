using UnityEngine;
namespace OneMoreTime {
// Repeats the tutorial scenery while following both axes of tall main rooms.
[DefaultExecutionOrder(220)]
public sealed class MainRoomBackdrop:MonoBehaviour {
 public Camera view;public Transform mountains,forest;
 void LateUpdate(){var p=view.transform.position;mountains.position=new Vector3(p.x*.8f,p.y,0);forest.position=new Vector3(p.x*.45f,p.y*.9f,0);}
}
}
