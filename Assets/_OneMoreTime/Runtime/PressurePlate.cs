using UnityEngine;
namespace OneMoreTime {
public sealed class PressurePlate : MonoBehaviour {
 public float width=1.4f;public SpriteRenderer graphic;public bool Pressed{get;private set;}public int Sources{get;private set;}
 public bool Accepts(Vector2 feet,bool grounded){return grounded&&Mathf.Abs(feet.y-transform.position.y)<=.06f&&Mathf.Abs(feet.x-transform.position.x)<(width+.7f)/2;}
 public void Evaluate(PlayerMotor2D player,EchoSurface echo){Sources=(Accepts(player.Position,player.Grounded)?1:0)+(echo.Active&&Accepts(echo.Pose.position,echo.Pose.grounded)?1:0);Pressed=Sources>0;if(graphic!=null)graphic.color=Pressed?new Color(.51f,.95f,.71f):new Color(.94f,.72f,.37f);}
 public void ResetState(){Sources=0;Pressed=false;}
}
}
