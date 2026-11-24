using UnityEngine;

namespace OneMoreTime {
/// <summary>Rectangular Sunny Land grass/soil art. Only the separate top collider is playable.</summary>
[ExecuteAlways, DisallowMultipleComponent]
public sealed class GrassyColumnVisual : MonoBehaviour {
 [Min(.25f)] public float width=4;
 [Min(1f)] public float depth=6;
 [Tooltip("Local height of the walking surface. Existing room colliders remain unchanged.")]
 public float topOffset;
 public bool background;
 [Tooltip("Enable on reusable platform prefabs; scene migrations preserve existing collision exactly.")]
 public bool resizeTopCollider;
 public Color soilColor=new Color(.8f,.78f,.72f,1);
 public Sprite grassLeft,grassMiddle,grassRight;
 public Sprite soilLeft,soilMiddle,soilRight;
 public Sprite bottomLeft,bottomMiddle,bottomRight;
 public Material spriteMaterial;
 [SerializeField] Transform artwork;
 bool dirty=true;
 void OnEnable(){dirty=true;}
 void OnValidate(){width=Mathf.Max(.25f,width);depth=Mathf.Max(1,depth);dirty=true;}
 void LateUpdate(){if(dirty)Refresh();}

 [ContextMenu("Refresh grass column")]
 public void Refresh(){
  dirty=false;
  if(grassMiddle==null||soilMiddle==null)return;
  if(artwork==null){var existing=transform.Find("Rectangular grass column art");artwork=existing!=null?existing:new GameObject("Rectangular grass column art").transform;artwork.SetParent(transform,false);}
  artwork.localPosition=Vector3.zero;artwork.localRotation=Quaternion.identity;artwork.localScale=Vector3.one;
  float edge=Mathf.Min(1,width*.5f),middle=Mathf.Max(0,width-edge*2);
  float cap=Mathf.Min(1,depth),foot=Mathf.Min(1,Mathf.Max(0,depth-cap)),body=Mathf.Max(0,depth-cap-foot);
  float left=-width/2+edge/2,right=width/2-edge/2;
  Color soil=soilColor*(background?new Color(.5f,.5f,.5f,1):Color.white);
  Color grass=background?new Color(.5f,.5f,.5f,1):Color.white;
  int soilOrder=background?-20:-5,grassOrder=background?-19:5;
  Draw("Soil left rim",soilLeft,left,topOffset-cap-body/2,edge,body,soil,soilOrder);
  Draw("Soil interior",soilMiddle,0,topOffset-cap-body/2,middle,body,soil,soilOrder);
  Draw("Soil right rim",soilRight,right,topOffset-cap-body/2,edge,body,soil,soilOrder);
  Draw("Soil bottom left",bottomLeft,left,topOffset-depth+foot/2,edge,foot,soil,soilOrder);
  Draw("Soil bottom",bottomMiddle,0,topOffset-depth+foot/2,middle,foot,soil,soilOrder);
  Draw("Soil bottom right",bottomRight,right,topOffset-depth+foot/2,edge,foot,soil,soilOrder);
  Draw("Grass left edge",grassLeft,left,topOffset-cap/2,edge,cap,grass,grassOrder);
  Draw("Continuous grass top",grassMiddle,0,topOffset-cap/2,middle,cap,grass,grassOrder);
  Draw("Grass right edge",grassRight,right,topOffset-cap/2,edge,cap,grass,grassOrder);
  if(resizeTopCollider&&!background){var box=GetComponent<BoxCollider2D>();if(box!=null){box.size=new Vector2(width,.18f);box.offset=new Vector2(0,topOffset-.09f);}}
 }
 void Draw(string part,Sprite sprite,float x,float y,float w,float h,Color color,int order){
  var child=artwork.Find(part);if(child==null){child=new GameObject(part).transform;child.SetParent(artwork,false);}
  child.localPosition=new Vector3(x,y,0);child.localScale=Vector3.one;
  var renderer=child.GetComponent<SpriteRenderer>();if(renderer==null)renderer=child.gameObject.AddComponent<SpriteRenderer>();
  renderer.sprite=sprite;renderer.drawMode=SpriteDrawMode.Tiled;renderer.tileMode=SpriteTileMode.Continuous;
  renderer.size=new Vector2(Mathf.Max(.001f,w),Mathf.Max(.001f,h));renderer.color=color;renderer.sortingOrder=order;
  if(spriteMaterial!=null)renderer.sharedMaterial=spriteMaterial;
  renderer.enabled=w>.001f&&h>.001f&&sprite!=null;
 }
}
}
