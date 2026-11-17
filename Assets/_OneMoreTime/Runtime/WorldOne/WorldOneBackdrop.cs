using UnityEngine;
namespace OneMoreTime {
// Camera-sized quad with world-position-driven UVs: no finite climb limit or tile spawning.
[DefaultExecutionOrder(250)]
public sealed class WorldOneBackdrop:MonoBehaviour {
 public Camera view;public SpriteRenderer scenery;
 [Min(1)]public float repeatWidth=30,repeatHeight=30;
 public Vector2 parallax=new Vector2(.2f,.35f);
 [Range(.01f,.3f)]public float edgeBlend=.12f;
 Material runtimeMaterial;Material originalMaterial;
 public Vector2 ScrollPhase{get;private set;}
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
  runtimeMaterial.SetVector("_Scroll",new Vector4(ScrollPhase.x,ScrollPhase.y,0,0));
  runtimeMaterial.SetVector("_RepeatSpan",new Vector4(w/rw,h/rh,0,0));
  runtimeMaterial.SetFloat("_Overlap",Mathf.Clamp(edgeBlend,.01f,.3f));
 }
}
}
