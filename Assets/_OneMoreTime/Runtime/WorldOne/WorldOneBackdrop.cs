using UnityEngine;
namespace OneMoreTime {
// Camera-sized quad with world-position-driven UVs: no finite climb limit or tile spawning.
[DefaultExecutionOrder(250)]
public sealed class WorldOneBackdrop:MonoBehaviour {
 public Camera view;public SpriteRenderer scenery;
 [Header("Past filter background")]
 [Tooltip("Uses the enclosing main room when left empty. Background blending follows its lens, including pause and rewind.")]
 public ChapterOneRoom eraSource;
 [Tooltip("Optional final past artwork. Leave empty for a warm, faded version of the present background. Use the same proportions as the present image; both share its loop and pixel scale.")]
 public Texture2D pastBackground;
 [ColorUsage(false)]public Color pastTint=new Color(1.08f,1.02f,.82f,1);
 [Range(0,1)]public float pastSaturation=.48f;
 // 384 cells vertically, aspect-correct horizontal grid at 16 pixels/world unit.
 [Min(1)]public float repeatWidth=63.36f,repeatHeight=21.12f;
 public Vector2 parallax=new Vector2(.2f,.35f);
 [Range(.01f,.3f)]public float edgeBlend=.12f;
 Material runtimeMaterial;Material originalMaterial;
 public Vector2 ScrollPhase{get;private set;}
 void Awake(){if(eraSource==null)eraSource=GetComponentInParent<ChapterOneRoom>();}
 void OnDisable(){if(runtimeMaterial!=null){if(scenery!=null)scenery.sharedMaterial=originalMaterial;Destroy(runtimeMaterial);runtimeMaterial=null;}}
 void LateUpdate(){
  if(view==null)view=Camera.main;
  if(view==null||scenery==null||scenery.sprite==null)return;
  if(runtimeMaterial==null){var shader=Shader.Find("OneMoreTime/Looping Forest");if(shader==null)return;originalMaterial=scenery.sharedMaterial;runtimeMaterial=new Material(shader);scenery.sharedMaterial=runtimeMaterial;}
  float h=view.orthographicSize*2,w=h*view.aspect;var size=scenery.sprite.bounds.size;
  scenery.transform.localScale=new Vector3(w/size.x,h/size.y,1);
  scenery.transform.position=new Vector3(view.transform.position.x,view.transform.position.y,0);
  float rw=Mathf.Max(1,repeatWidth),rh=Mathf.Max(1,repeatHeight);
  // Offset by the lower-left camera corner, so changing aspect/zoom keeps the center anchored.
  ScrollPhase=new Vector2(Mathf.Repeat((view.transform.position.x*parallax.x-w*.5f)/rw,1),Mathf.Repeat((view.transform.position.y*parallax.y-h*.5f)/rh,1));
  runtimeMaterial.mainTexture=scenery.sprite.texture;
  runtimeMaterial.SetVector("_PixelGrid",new Vector4(384f*scenery.sprite.texture.width/scenery.sprite.texture.height,384,0,0));
  runtimeMaterial.SetVector("_Scroll",new Vector4(ScrollPhase.x,ScrollPhase.y,0,0));
  runtimeMaterial.SetVector("_RepeatSpan",new Vector4(w/rw,h/rh,0,0));
  runtimeMaterial.SetFloat("_Overlap",Mathf.Clamp(edgeBlend,.01f,.3f));
  // Reuse the lens clock rather than starting another transition. A rejected
  // era switch leaves this unchanged; retry/rewind follows the restored lens.
  runtimeMaterial.SetFloat("_PastAmount",eraSource!=null?eraSource.Lens.PastAmount:0);
  runtimeMaterial.SetTexture("_PastTex",pastBackground!=null?pastBackground:scenery.sprite.texture);
  runtimeMaterial.SetFloat("_HasPastBackground",pastBackground!=null?1:0);
  runtimeMaterial.SetVector("_PastPixelGrid",new Vector4(pastBackground!=null?384f*pastBackground.width/pastBackground.height:384f*scenery.sprite.texture.width/scenery.sprite.texture.height,384,0,0));
  runtimeMaterial.SetColor("_PastTint",pastTint);
  runtimeMaterial.SetFloat("_PastSaturation",pastSaturation);
 }
}
}
