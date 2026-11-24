using UnityEngine;

namespace OneMoreTime {
// Shared by pickups and UI. These visuals never decide whether an ability is unlocked.
public static class AbilityArt {
 public const string ShutterCamera="ShutterCamera",ReplayTape="ReplayTape",ArcaneReplayTape="ArcaneReplayTape",FlashModule="FlashModule",PastLens="PastLens";
 // The standing goat's sprite canvas is approximately 1.5 world units.
 public const float PickupWorldSize=1.5f*.9f;
 const string PickupChild="Camera ability sprite";
 public static Sprite Sprite(string name)=>Resources.Load<Sprite>("AbilityArt/"+name);
 public static Texture2D Texture(string name)=>Resources.Load<Texture2D>("AbilityArt/"+name);

 // Keep the authored root, pickup radius, colliders and animation unchanged. A separate
 // visual child fits 90% of the goat's size without distorting the pixel art
 // or resizing shared UI textures.
 public static SpriteRenderer ReplacePickup(Transform target,string name){
  if(target==null)return null;
  var sprite=Sprite(name);if(sprite==null)return null;
  var existing=target.Find(PickupChild);
  var previous=target.GetComponentsInChildren<SpriteRenderer>(true);
  var bounds=new Bounds(target.position,Vector3.one);
  bool hasBounds=false;int order=10,layer=0;
  foreach(var renderer in previous){
   // Retain the authored center/layer, independent of the replacement's size.
   if(renderer.transform==existing||renderer.sprite==null||(!renderer.enabled&&existing==null))continue;
   if(!hasBounds){bounds=renderer.bounds;order=renderer.sortingOrder;layer=renderer.sortingLayerID;hasBounds=true;}
   else{bounds.Encapsulate(renderer.bounds);if(renderer.sortingOrder>order){order=renderer.sortingOrder;layer=renderer.sortingLayerID;}}
  }
  var child=existing!=null?existing:new GameObject(PickupChild).transform;
  child.SetParent(target,false);child.position=bounds.center;
  float size=PickupWorldSize;
  float scale=size/Mathf.Max(.001f,Mathf.Max(sprite.bounds.size.x,sprite.bounds.size.y));
  var parentScale=target.lossyScale;
  child.localScale=new Vector3(scale/Mathf.Max(.001f,Mathf.Abs(parentScale.x)),scale/Mathf.Max(.001f,Mathf.Abs(parentScale.y)),1);
  var graphic=child.GetComponent<SpriteRenderer>();if(graphic==null)graphic=child.gameObject.AddComponent<SpriteRenderer>();
  graphic.sprite=sprite;graphic.color=Color.white;graphic.sortingOrder=order;graphic.sortingLayerID=layer;
  foreach(var renderer in previous)if(renderer!=graphic)renderer.enabled=false;
  return graphic;
 }
}
}
