using UnityEngine;
namespace OneMoreTime {
// The collider/checkpoint location belongs to the room; this component only animates art.
public sealed class StoneCheckpointVisual:MonoBehaviour {
 public SpriteRenderer target;public Sprite[] frames;
 // The older player-sprite checkpoints have no painted plinth; they use a simple stone base.
 public bool hasPedestal=true;
 public SpriteRenderer[] auxiliaryRenderers=new SpriteRenderer[0];
 public bool Active{get;private set;}
 public bool BaseOnly{get;private set;}
 public Sprite IntactSprite{get{RememberSource();return intactSprite;}}
 public Color IntactColor{get{RememberSource();return intactColor;}}
 Sprite intactSprite,baseSprite;Texture2D baseTexture;Color intactColor;
 float clock;
 void Awake(){if(target!=null)target.sortingOrder=5;}
 public void Activate(){if(Active)return;Active=true;clock=0;}
 public void ResetDisplay(){Active=false;BaseOnly=false;clock=0;if(target!=null){RememberSource();target.color=intactColor;}foreach(var renderer in auxiliaryRenderers)if(renderer!=null)renderer.enabled=true;Show();}
 public void LeaveBase(){
  RememberSource();BaseOnly=true;
  if(baseSprite==null&&intactSprite!=null)baseSprite=MakeBase(intactSprite);
  foreach(var renderer in auxiliaryRenderers)if(renderer!=null)renderer.enabled=false;
  Show();
 }
 // Both statue sprite sets face right before renderer or hierarchy mirroring.
 public static int FacingOf(SpriteRenderer renderer)=>renderer==null?1:renderer.transform.TransformVector(Vector3.right*(renderer.flipX?-1:1)).x<0?-1:1;
 void Update(){Tick(Time.deltaTime);}
 public void Tick(float dt){if(Active)clock+=Mathf.Max(0,dt);Show();}
 void RememberSource(){if(intactSprite!=null||target==null)return;intactSprite=frames!=null&&frames.Length>0&&frames[0]!=null?frames[0]:target.sprite;intactColor=target.color;}
 void Show(){
  if(target==null)return;
  if(BaseOnly){if(baseSprite!=null)target.sprite=baseSprite;target.color=hasPedestal?intactColor:new Color(.45f,.49f,.47f,intactColor.a);return;}
  if(frames==null||frames.Length<6){if(intactSprite!=null)target.sprite=intactSprite;return;}
  int index=!Active?0:clock<.8f?Mathf.Min(3,1+Mathf.FloorToInt(clock*4)):4+Mathf.FloorToInt((clock-.8f)*3)%2;target.sprite=frames[index];
 }
 Sprite MakeBase(Sprite source){
  var bounds=StatueRevival.VisibleRect(source);Sprite result;
  if(hasPedestal){
   // Crop the actual texture rectangle. Unity can regenerate Sprite mesh geometry after a
   // renderer update, so an overridden mesh over the full statue is not a durable crop.
   float ppu=source.pixelsPerUnit;var rect=source.rect;var pivot=source.pivot;
   var min=new Vector2(Mathf.Clamp(bounds.xMin*ppu+pivot.x,0,rect.width),Mathf.Clamp(bounds.yMin*ppu+pivot.y,0,rect.height));
   var size=new Vector2(Mathf.Min(bounds.width*ppu,rect.width-min.x),Mathf.Min(bounds.height*ppu*.14f,rect.height-min.y));
   result=Sprite.Create(source.texture,new Rect(rect.position+min,size),(pivot-min)/size,ppu,0,SpriteMeshType.FullRect);
  }else{
   // A tiny opaque primitive gives legacy statues a plinth without relying on fractional
   // texture rectangles or altering the checkpoint/renderer transform.
   float width=bounds.width*.8f;
   baseTexture=new Texture2D(25,4,TextureFormat.RGBA32,false){name="Legacy checkpoint plinth",filterMode=FilterMode.Point,wrapMode=TextureWrapMode.Clamp};
   var pixels=new Color[100];for(int i=0;i<pixels.Length;i++)pixels[i]=Color.white;
   baseTexture.SetPixels(pixels);baseTexture.Apply(false,true);
   result=Sprite.Create(baseTexture,new Rect(0,0,25,4),new Vector2(.5f,0),25/width,0,SpriteMeshType.FullRect);
  }
  result.name=source.name+" — broken checkpoint base";
  return result;
 }
 void OnDestroy(){DestroyOwned(baseSprite);DestroyOwned(baseTexture);}
 static void DestroyOwned(Object value){if(value==null)return;if(Application.isPlaying)Destroy(value);else DestroyImmediate(value);}
}
}
