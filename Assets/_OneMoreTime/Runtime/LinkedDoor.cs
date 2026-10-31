using UnityEngine;
namespace OneMoreTime {
public sealed class LinkedDoor : MonoBehaviour {
 public PressurePlate plate;public BoxCollider2D solid;public SpriteRenderer graphic;public bool Open{get;private set;}
 public void Evaluate(PlayerMotor2D player){var rect=new Rect((Vector2)transform.position+solid.offset-solid.size/2,solid.size);bool occupied=rect.Overlaps(new Rect(player.Position-new Vector2(player.tuning.width/2,0),new Vector2(player.tuning.width,player.tuning.height)));Open=plate.Pressed||(Open&&occupied);solid.enabled=!Open;if(graphic!=null)graphic.color=Open?new Color(.4f,1,.7f,.2f):new Color(.30f,.25f,.19f);}
 public void ResetState(){Open=false;solid.enabled=true;}
}
}
