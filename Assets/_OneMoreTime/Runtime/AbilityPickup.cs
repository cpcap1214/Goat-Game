using UnityEngine;
namespace OneMoreTime {
// Pickup tests use the live player's body, never the echo. Animation uses the same fixed clock.
public sealed class AbilityPickup:MonoBehaviour {
 public MemoryKind Kind{get;private set;}public Vector2 Anchor{get;private set;}public bool Collected{get;private set;}
 public Texture2D Icon{get;private set;}Sprite sprite;SpriteRenderer graphic;
 public void Init(MemoryKind kind,Vector2 anchor){Kind=kind;Anchor=anchor;Icon=AbilityArt.Texture(kind==MemoryKind.Freeze?AbilityArt.ShutterCamera:AbilityArt.ReplayTape);
  if(Icon==null)Icon=Resources.Load<Texture2D>(kind==MemoryKind.Freeze?"Relics/FreezeWatch":"Relics/TraceHourglass");
  if(Icon==null)throw new System.InvalidOperationException("Missing relic artwork: "+kind);
  sprite=Sprite.Create(Icon,new Rect(0,0,Icon.width,Icon.height),new Vector2(.5f,.5f),Mathf.Max(Icon.width,Icon.height)/AbilityArt.PickupWorldSize,0,SpriteMeshType.FullRect);graphic=gameObject.AddComponent<SpriteRenderer>();graphic.sprite=sprite;graphic.sortingOrder=12;Animate(0);
 }
 public void Animate(float clock){if(!Collected)transform.position=Anchor+Vector2.up*(Mathf.Sin(clock*2.4f)*.14f);}
 public bool Touches(PlayerMotor2D player){return !Collected&&new Rect(Anchor-new Vector2(.42f,.5f),new Vector2(.84f,1)).Overlaps(new Rect(player.Position-new Vector2(.35f,0),new Vector2(.7f,1)));}
 public void Collect(){Collected=true;graphic.enabled=false;}
 void OnDestroy(){if(sprite!=null)Destroy(sprite);}
}
}
