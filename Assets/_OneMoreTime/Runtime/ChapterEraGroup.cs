using UnityEngine;

namespace OneMoreTime {
// Inactive bridge sections stay visible as darker background remnants.
public sealed class ChapterEraGroup : MonoBehaviour {
 public bool activeInPast;
 public Collider2D[] solids=new Collider2D[0];
 public SpriteRenderer[] artwork=new SpriteRenderer[0];
 [HideInInspector]public Color[] activeColors=new Color[0];
 [HideInInspector]public int[] activeOrders=new int[0];
 // Persist the authored appearance before an Editor builder applies the entry era.
 public void CaptureAppearance(){
  activeColors=new Color[artwork.Length];activeOrders=new int[artwork.Length];
  for(int i=0;i<artwork.Length;i++)if(artwork[i]!=null){activeColors[i]=artwork[i].color;activeOrders[i]=artwork[i].sortingOrder;}
 }
 void Initialize(){
  if(activeColors==null||activeOrders==null||activeColors.Length!=artwork.Length||activeOrders.Length!=artwork.Length)CaptureAppearance();
 }
 public void Apply(bool past){
  Initialize();bool active=past==activeInPast;
  foreach(var solid in solids)if(solid!=null)solid.enabled=active;
  for(int i=0;i<artwork.Length;i++)if(artwork[i]!=null){var c=activeColors[i];if(!active)c=new Color(c.r*.42f,c.g*.42f,c.b*.42f,c.a*.5f);artwork[i].color=c;artwork[i].sortingOrder=active?activeOrders[i]:Mathf.Min(-12,activeOrders[i]-15);}
 }
 public bool WouldEmbed(Rect body){
  foreach(var solid in solids){
   if(solid==null||solid.isTrigger)continue;
   // Disabled colliders have empty Unity bounds, so transform the authored shape.
   if(solid is BoxCollider2D box){
    var half=box.size*.5f;var offset=box.offset;
    Vector2 a=box.transform.TransformPoint(offset+new Vector2(-half.x,-half.y));
    Vector2 b=box.transform.TransformPoint(offset+new Vector2(half.x,-half.y));
    Vector2 c=box.transform.TransformPoint(offset+new Vector2(half.x,half.y));
    Vector2 d=box.transform.TransformPoint(offset+new Vector2(-half.x,half.y));
    var min=Vector2.Min(Vector2.Min(a,b),Vector2.Min(c,d));var max=Vector2.Max(Vector2.Max(a,b),Vector2.Max(c,d));
    if(body.Overlaps(Rect.MinMaxRect(min.x,min.y,max.x,max.y)))return true;
   }else if(solid is PolygonCollider2D polygon){
    Vector2 min=new Vector2(float.MaxValue,float.MaxValue),max=new Vector2(float.MinValue,float.MinValue);
    foreach(var p in polygon.points){Vector2 point=polygon.transform.TransformPoint(p+polygon.offset);min=Vector2.Min(min,point);max=Vector2.Max(max,point);}
    if(body.Overlaps(Rect.MinMaxRect(min.x,min.y,max.x,max.y)))return true;
   }
  }
  return false;
 }
}
}
