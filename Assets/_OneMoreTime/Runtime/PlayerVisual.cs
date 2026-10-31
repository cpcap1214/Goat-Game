using UnityEngine;
namespace OneMoreTime {
public sealed class PlayerVisual : MonoBehaviour {
 // Shared sprite alignment for GUI cinematics and tutorials, matching SpriteRenderer pivots.
 public static Rect SpriteGUIRect(Sprite sprite,Vector2 feet,float scale){
  var r=sprite.rect;var pivot=sprite.pivot;
  return new Rect(feet.x-pivot.x*scale,feet.y-(r.height-pivot.y)*scale,r.width*scale,r.height*scale);
 }
 public AnimationLibrary library;public SpriteRenderer renderer2D;
 public void Pose(Vector2 feet,int facing,int state,int frame,Color color){transform.position=feet;renderer2D.transform.localScale=Vector3.one*library.visualScale;renderer2D.transform.localPosition=library.visualOffset;renderer2D.sprite=library.Get(state,frame);renderer2D.flipX=facing<0;renderer2D.color=color;}
}
}
