using UnityEngine;
namespace OneMoreTime {
// Deck origin is the walking surface. Ropes and posts are decoration only.
[ExecuteAlways, RequireComponent(typeof(BoxCollider2D), typeof(OneWayPlatform))]
public sealed class WoodenSuspensionBridge : MonoBehaviour {
 [Min(1)] public int middleTiles=2;
 public SpriteRenderer left, middle, right;
 public void Refresh(){
  if(left==null||middle==null||right==null||middle.sprite==null)return;
  middleTiles=Mathf.Max(1,middleTiles);
  float tile=middle.sprite.bounds.size.x, span=tile*middleTiles;
  middle.drawMode=SpriteDrawMode.Tiled;
  middle.size=new Vector2(span,middle.sprite.bounds.size.y);
  middle.transform.localPosition=Vector3.zero;
  left.transform.localPosition=new Vector3(-(span+left.sprite.bounds.size.x)*.5f,0,0);
  right.transform.localPosition=new Vector3((span+right.sprite.bounds.size.x)*.5f,0,0);
  var deck=GetComponent<BoxCollider2D>();
  deck.size=new Vector2(span+left.sprite.bounds.size.x+right.sprite.bounds.size.x,.18f);
  deck.offset=new Vector2(0,-.09f);
 }
 void OnEnable(){Refresh();}
 void OnValidate(){Refresh();}
}
}
