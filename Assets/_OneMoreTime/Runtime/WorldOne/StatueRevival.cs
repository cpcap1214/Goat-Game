using System.Collections.Generic;
using UnityEngine;

namespace OneMoreTime {
// A visual shell only. The room owns respawn position, invulnerability and its fixed clock.
public sealed class StatueRevival:MonoBehaviour {
 public const float Duration=.64f;
 public bool Playing{get;private set;}
 public float Progress{get;private set;}

 sealed class Fragment {
  public Transform transform;
  public MeshRenderer renderer;
  public Mesh mesh;
  public Vector3 origin,velocity;
  public float spin;
  public Color[] colors=new Color[3];
 }
 struct RendererState {
  public Sprite sprite;
  public Color color;
  public bool enabled,flipX,flipY;
  public Vector3 position,scale;
  public Quaternion rotation;
  public RendererState(SpriteRenderer renderer){
   sprite=renderer.sprite;color=renderer.color;enabled=renderer.enabled;
   flipX=renderer.flipX;flipY=renderer.flipY;
   position=renderer.transform.localPosition;scale=renderer.transform.localScale;
   rotation=renderer.transform.localRotation;
  }
  public void Restore(SpriteRenderer renderer){
   if(renderer==null)return;
   renderer.sprite=sprite;renderer.color=color;renderer.enabled=enabled;
   renderer.flipX=flipX;renderer.flipY=flipY;
   renderer.transform.localPosition=position;renderer.transform.localScale=scale;
   renderer.transform.localRotation=rotation;
  }
 }

 readonly List<Fragment> fragments=new List<Fragment>();
 static readonly Dictionary<Sprite,Rect> visibleRects=new Dictionary<Sprite,Rect>();
 SpriteRenderer statue,goat,shell,energy;
 StoneCheckpointVisual stone;
 PlayerVisual player;
 RendererState statueState,playerState;
 GameObject shellRoot,energyRoot;
 Vector3 goatOffset;
 bool stoneWasEnabled,broken;
 float elapsed;

 public void Begin(SpriteRenderer statue,StoneCheckpointVisual stone,PlayerVisual player,Vector2 feet,int facing){
  Cancel();
  if(statue==null||statue.sprite==null||player==null||player.renderer2D==null||player.library==null)return;
  this.statue=statue;this.stone=stone;this.player=player;goat=player.renderer2D;
  statueState=new RendererState(statue);playerState=new RendererState(goat);
  if(stone!=null){stoneWasEnabled=stone.enabled;stone.enabled=false;}

  // Keep the original goat's pivot and size; the larger checkpoint never scales the live actor.
  player.Pose(feet,facing,0,0,Color.white);
  goatOffset=goat.transform.localPosition;
  goat.enabled=false;
  shellRoot=new GameObject("Revival stone shell");
  shellRoot.transform.SetParent(statue.transform,false);
  shell=shellRoot.AddComponent<SpriteRenderer>();
  CopyRenderer(statue,shell);
  if(stone!=null){shell.sprite=stone.IntactSprite;shell.color=stone.IntactColor;}
  shell.sortingLayerID=goat.sortingLayerID;
  shell.sortingOrder=goat.sortingOrder+1;
  statue.enabled=false;

  energyRoot=new GameObject("Revival green energy");
  energyRoot.transform.SetParent(goat.transform,false);
  energyRoot.transform.localScale=Vector3.one*1.06f;
  energy=energyRoot.AddComponent<SpriteRenderer>();
  CopyRenderer(goat,energy);
  energy.sortingOrder=goat.sortingOrder-1;
  energy.color=Color.clear;

  // Prepare the small stone chips once, before animation. The burst's green outline must not shatter.
  var fragmentSprite=stone!=null&&stone.frames!=null&&stone.frames.Length>2&&stone.frames[2]!=null?stone.frames[2]:stone!=null?stone.IntactSprite:statue.sprite;
  MakeFragments(fragmentSprite);

  elapsed=0;Progress=0;broken=false;Playing=true;
  Show();
 }

 public void Tick(float dt){
  if(!Playing)return;
  if(statue==null||goat==null){Cancel();return;}
  elapsed=Mathf.Min(Duration,elapsed+Mathf.Max(0,dt));
  if(elapsed>=Duration-.00001f)elapsed=Duration;
  Progress=elapsed/Duration;
  Show();
  if(elapsed>=Duration)Finish();
 }

 void Show(){
  float charge=Mathf.Clamp01(elapsed/.16f);
  if(!broken){
   // Existing art provides the cracks and green eyes, rather than drawing a new goat silhouette.
   if(stone!=null&&stone.frames!=null&&stone.frames.Length>=4)
    shell.sprite=stone.frames[Mathf.Clamp(1+Mathf.FloorToInt(charge*3),1,3)];
   Color shellColor=stone!=null?stone.IntactColor:statueState.color;
   shell.color=Color.Lerp(shellColor,new Color(.65f,1,.72f,shellColor.a),Mathf.Sin(charge*Mathf.PI)*.34f);
   if(elapsed>=.16f){
    shell.enabled=false;broken=true;
    // The goat has left this statue. Keep its pedestal instead of rebuilding the body behind it.
    if(stone!=null){stone.LeaveBase();statue.enabled=statueState.enabled;}
   }
  }

  float emerge=Mathf.Clamp01((elapsed-.16f)/.26f);
  goat.enabled=elapsed>=.16f;
  goat.color=new Color(1,1,1,Mathf.SmoothStep(0,1,Mathf.Clamp01(emerge*3)));
  // A tiny vertical settle belongs to the sprite child, never the motor or checkpoint location.
  goat.transform.localPosition=goatOffset+Vector3.up*(.045f*Mathf.Sin(emerge*Mathf.PI));
  if(player.library!=null&&emerge>0)
   goat.sprite=player.library.Get(emerge<.8f?4:0,Mathf.FloorToInt(emerge*4));
  energy.sprite=goat.sprite;
  float glow=elapsed<.16f?Mathf.Sin(charge*Mathf.PI)*.16f:Mathf.Sin(emerge*Mathf.PI)*.45f;
  energy.color=new Color(.32f,1,.56f,glow);

  float shardTime=Mathf.Max(0,elapsed-.16f);
  float alpha=1-Mathf.SmoothStep(0,1,Mathf.InverseLerp(.035f,.28f,shardTime));
  foreach(var fragment in fragments){
   fragment.renderer.enabled=broken&&alpha>0;
   fragment.transform.localPosition=fragment.origin+fragment.velocity*shardTime+Vector3.down*(3.5f*shardTime*shardTime);
   fragment.transform.localRotation=Quaternion.Euler(0,0,fragment.spin*shardTime);
   Color tint=stone!=null?stone.IntactColor:statueState.color;
   tint.a=alpha*statueState.color.a;
   for(int i=0;i<3;i++)fragment.colors[i]=tint;
   fragment.mesh.colors=fragment.colors;
  }
 }

 void MakeFragments(Sprite sprite){
  if(sprite==null)return;
  var bounds=VisibleRect(sprite);
  // Leave the pedestal out of the moving shell; it should never become a giant flying slab.
  if(stone!=null&&stone.hasPedestal)bounds.yMin+=bounds.height*.14f;
  var positions=sprite.vertices;var uv=sprite.uv;var indices=sprite.triangles;
  if(indices.Length<3)return;
  // Reconstruct the sprite's affine UV mapping, including packed texture offsets.
  int ia=indices[0],ib=indices[1],ic=indices[2];
  Vector2 a=positions[ia],b=positions[ib]-a,c=positions[ic]-a;
  float determinant=b.x*c.y-b.y*c.x;
  if(Mathf.Abs(determinant)<.000001f)return;
  Vector2 TextureUV(Vector2 point){
   Vector2 p=point-a;
   float x=(p.x*c.y-p.y*c.x)/determinant,y=(b.x*p.y-b.y*p.x)/determinant;
   return uv[ia]+(uv[ib]-uv[ia])*x+(uv[ic]-uv[ia])*y;
  }
  // Alternating triangular fractures retain the real stone texture and transparent outline.
  for(int y=0;y<4;y++)for(int x=0;x<4;x++){
   float x0=Mathf.Lerp(bounds.xMin,bounds.xMax,x/4f),x1=Mathf.Lerp(bounds.xMin,bounds.xMax,(x+1)/4f);
   float y0=Mathf.Lerp(bounds.yMin,bounds.yMax,y/4f),y1=Mathf.Lerp(bounds.yMin,bounds.yMax,(y+1)/4f);
   var p0=new Vector2(x0,y0);var p1=new Vector2(x1,y0);var p2=new Vector2(x1,y1);var p3=new Vector2(x0,y1);
   if((x+y)%2==0){AddFragment(p0,p1,p2,TextureUV,sprite);AddFragment(p0,p2,p3,TextureUV,sprite);}
   else {AddFragment(p0,p1,p3,TextureUV,sprite);AddFragment(p1,p2,p3,TextureUV,sprite);}
  }
 }

 internal static Rect VisibleRect(Sprite sprite){
  if(visibleRects.TryGetValue(sprite,out var result))return result;
  var bounds=sprite.bounds;
  result=new Rect(bounds.min.x,bounds.min.y,bounds.size.x,bounds.size.y);
  // The project sprites are readable and unpacked. Other import modes safely use their mesh bounds.
  if(sprite.texture!=null&&sprite.texture.isReadable&&!sprite.packed){
   var rect=sprite.rect;
   int width=Mathf.RoundToInt(rect.width),height=Mathf.RoundToInt(rect.height);
   var pixels=sprite.texture.GetPixels(Mathf.RoundToInt(rect.x),Mathf.RoundToInt(rect.y),width,height);
   int minX=width,minY=height,maxX=-1,maxY=-1;
   for(int y=0;y<height;y++)for(int x=0;x<width;x++)if(pixels[y*width+x].a>.5f){
    minX=Mathf.Min(minX,x);minY=Mathf.Min(minY,y);maxX=Mathf.Max(maxX,x);maxY=Mathf.Max(maxY,y);
   }
   if(maxX>=minX&&maxY>=minY)result=new Rect((minX-sprite.pivot.x)/sprite.pixelsPerUnit,
    (minY-sprite.pivot.y)/sprite.pixelsPerUnit,(maxX-minX+1)/sprite.pixelsPerUnit,(maxY-minY+1)/sprite.pixelsPerUnit);
  }
  visibleRects[sprite]=result;return result;
 }

 void AddFragment(Vector2 a,Vector2 b,Vector2 c,System.Func<Vector2,Vector2> textureUV,Sprite sprite){
  Vector2 centre=(a+b+c)/3;
  Vector2 flip=new Vector2(shell.flipX?-1:1,shell.flipY?-1:1);
  Vector2 displayCentre=Vector2.Scale(centre,flip);
  var piece=new GameObject("Stone fragment");piece.transform.SetParent(shellRoot.transform,false);
  var mesh=new Mesh{name="Revival stone triangle"};
  mesh.vertices=new[]{(Vector3)Vector2.Scale(a-centre,flip),(Vector3)Vector2.Scale(b-centre,flip),(Vector3)Vector2.Scale(c-centre,flip)};
  mesh.uv=new[]{textureUV(a),textureUV(b),textureUV(c)};
  mesh.triangles=new[]{0,1,2};mesh.colors=new[]{Color.white,Color.white,Color.white};mesh.RecalculateBounds();
  piece.AddComponent<MeshFilter>().sharedMesh=mesh;
  var renderer=piece.AddComponent<MeshRenderer>();
  renderer.sharedMaterial=shell.sharedMaterial;
  renderer.sortingLayerID=goat.sortingLayerID;renderer.sortingOrder=goat.sortingOrder-2;
  renderer.enabled=false;
  renderer.shadowCastingMode=UnityEngine.Rendering.ShadowCastingMode.Off;
  renderer.receiveShadows=false;
  var properties=new MaterialPropertyBlock();
  properties.SetTexture("_MainTex",sprite.texture);
  properties.SetColor("_RendererColor",Color.white);properties.SetVector("_Flip",Vector4.one);
  renderer.SetPropertyBlock(properties);
  Vector2 radial=displayCentre-(Vector2)Vector2.Scale(sprite.bounds.center,flip);
  float side=radial.x<0?-1:1;
  fragments.Add(new Fragment{transform=piece.transform,renderer=renderer,mesh=mesh,origin=displayCentre,
   velocity=new Vector3(side*(1.8f+Mathf.Abs(radial.x)*2),1+Mathf.Max(0,radial.y)*1.6f,0),
   spin=side*(105+fragments.Count%4*32)});
 }

 static void CopyRenderer(SpriteRenderer source,SpriteRenderer target){
  target.sprite=source.sprite;target.sharedMaterial=source.sharedMaterial;
  target.color=source.color;target.flipX=source.flipX;target.flipY=source.flipY;
  target.sortingLayerID=source.sortingLayerID;target.sortingOrder=source.sortingOrder;
  target.maskInteraction=source.maskInteraction;
 }

 void Finish(){
  RestoreStatue();
  if(goat!=null){
   goat.transform.localPosition=goatOffset;
   goat.color=playerState.color;goat.enabled=playerState.enabled;
   if(player!=null&&player.library!=null)goat.sprite=player.library.Get(0,0);
  }
  Release();Playing=false;Progress=1;
 }

 public void Cancel(){
  if(Playing){RestoreStatue();playerState.Restore(goat);}
  Release();Playing=false;Progress=0;elapsed=0;
 }

 void RestoreStatue(){
  statueState.Restore(statue);
  if(stone!=null){stone.LeaveBase();stone.enabled=stoneWasEnabled;stone.Tick(0);}
  else if(statue!=null)statue.enabled=false;
 }

 void Release(){
  if(shellRoot!=null){shellRoot.SetActive(false);DestroyOwned(shellRoot);}
  if(energyRoot!=null){energyRoot.SetActive(false);DestroyOwned(energyRoot);}
  foreach(var fragment in fragments)DestroyOwned(fragment.mesh);
  fragments.Clear();shellRoot=null;energyRoot=null;shell=null;energy=null;
  statue=null;stone=null;goat=null;player=null;
 }

 static void DestroyOwned(Object value){if(value==null)return;if(Application.isPlaying)Destroy(value);else DestroyImmediate(value);}
 void OnDisable(){Cancel();}
 void OnDestroy(){Cancel();}
}
}
