using System;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object=UnityEngine.Object;

namespace OneMoreTime.EditorTools {
// Bridge walls use continuous lumber, never repeated complete frame props with
// their own stone feet. The source is the existing mountain timber atlas.
public static class BridgeScaffoldBuilder {
 const string Generated=ForestExpansionBuilder.Generated;
 const string Suffix=" — scaffold wall art";
 const string TopMarker="Scaffold bounds — top left";
 const string BottomMarker="Scaffold bounds — bottom right";
 static readonly Color TimberColor=new Color(.48f,.47f,.43f,1);

 static Sprite BeamSprite(string name,Rect rect,Vector4 border) {
  string path=Generated+"/"+name+".asset";
  var sprite=AssetDatabase.LoadAssetAtPath<Sprite>(path);
  if(sprite!=null)return sprite;
  var source=AssetDatabase.LoadAssetAtPath<Sprite>(Generated+"/TimberFrame.asset");
  if(source==null)throw new InvalidOperationException("Missing existing TimberFrame sprite.");
  sprite=Sprite.Create(source.texture,rect,new Vector2(.5f,.5f),source.pixelsPerUnit,0,SpriteMeshType.FullRect,border);
  sprite.name=name;AssetDatabase.CreateAsset(sprite,path);return sprite;
 }

 static Transform Child(Transform parent,string name,Vector2 position) {
  var t=new GameObject(name).transform;t.SetParent(parent,false);t.position=position;return t;
 }

 static void Beam(Transform parent,string name,Sprite sprite,Vector2 position,Vector2 size,int order) {
  var t=Child(parent,name,position);var sr=t.gameObject.AddComponent<SpriteRenderer>();
  sr.sprite=sprite;sr.sharedMaterial=AssetDatabase.LoadAssetAtPath<Material>(Generated+"/TimberFrame.mat");
  sr.drawMode=SpriteDrawMode.Tiled;sr.tileMode=SpriteTileMode.Continuous;sr.size=size;
  sr.sortingOrder=order;sr.color=TimberColor;
 }

 public static Transform Build(Transform parent,string name,float left,float right,float top,float bottom) {
  string managedName=name.EndsWith(Suffix)?name:name+Suffix;
  var old=parent.Find(managedName);
  if(old!=null) {
   if(old.GetComponentsInChildren<Collider2D>(true).Length!=0)throw new InvalidOperationException("Refusing to replace colliding scaffold "+managedName);
   Object.DestroyImmediate(old.gameObject);
  }
  // Crop only straight lengths of the already-authored wood texture. The beam
  // ends are preserved while the middle repeats, with no frame or rock base.
  var post=BeamSprite("BridgeScaffoldUpright",new Rect(62,400,35,70),new Vector4(0,5,0,5));
  var cross=BeamSprite("BridgeScaffoldCrossbar",new Rect(102,533,112,28),new Vector4(5,0,5,0));
  var root=Child(parent,managedName,Vector2.zero);
  Child(root,TopMarker,new Vector2(left,top));Child(root,BottomMarker,new Vector2(right,bottom));
  float high=top-.2f,height=high-bottom;
  // Three-unit bays keep the scaffolding open and its horizontals aligned
  // across adjoining bridge fragments. All support artwork is behind the goat.
  int bays=Mathf.Max(1,Mathf.CeilToInt((right-left)/3f));
  for(int i=0;i<=bays;i++) {
   float x=Mathf.Lerp(left+.16f,right-.16f,i/(float)bays);
   Beam(root,"Continuous upright "+(i+1),post,new Vector2(x,(high+bottom)/2),new Vector2(.42f,height),-18);
  }
  int rows=0;
  for(float y=high-.18f;y>bottom+.25f;y-=2.8f)
   Beam(root,"Horizontal crosspiece "+(++rows),cross,new Vector2((left+right)/2,y),new Vector2(right-left,.36f),-17);
  return root;
 }

 public static string Patch(Scene scene) {
  if(EditorApplication.isPlaying)throw new InvalidOperationException("Exit Play mode before updating bridge artwork.");
  if(!scene.IsValid()||!scene.isLoaded)throw new InvalidOperationException("Load Room 5 or 6 first.");
  if(scene.name!="Main_Room05"&&scene.name!="Main_Room06")throw new InvalidOperationException("This patch only manages Room 5 and Room 6 scaffold artwork.");
  Physics2D.SyncTransforms();
  int count=0;
  var tutorial=scene.GetRootGameObjects().SelectMany(g=>g.GetComponentsInChildren<Room4AbilityTutorial>(true)).SingleOrDefault();
  var roots=scene.GetRootGameObjects().SelectMany(g=>g.GetComponentsInChildren<Transform>(true)).Where(t=>t.name.EndsWith(Suffix)&&(tutorial==null||t.name!="Pickup bridge supports"+Suffix)).ToArray();
  foreach(var root in roots) {
   var top=root.Find(TopMarker);var bottom=root.Find(BottomMarker);
   float left,right,high,low;
   if(top!=null&&bottom!=null){left=top.position.x;high=top.position.y;right=bottom.position.x;low=bottom.position.y;}
   else {
    var art=root.GetComponentsInChildren<SpriteRenderer>(true);if(art.Length==0)continue;
    var bounds=art[0].bounds;foreach(var sr in art.Skip(1))bounds.Encapsulate(sr.bounds);
    left=bounds.min.x;right=bounds.max.x;high=bounds.max.y+.15f;low=bounds.min.y;
   }
   Build(root.parent,root.name,left,right,high,low);count++;
  }
  if(tutorial!=null&&tutorial.bridge!=null&&tutorial.bridge.deck!=null) {
   var bounds=tutorial.bridge.deck.bounds;
   Build(tutorial.transform.Find("Environment"),"Pickup bridge supports",bounds.min.x,bounds.max.x,bounds.max.y,tutorial.room.roomBounds.yMin);
   count++;
  }
  EditorSceneManager.MarkSceneDirty(scene);return "Room "+(tutorial!=null?5:6)+": "+count+" bridge support groups replaced with continuous posts and horizontal crosspieces; no gameplay components changed.";
 }

 [MenuItem("Tools/One More Time/Update Room 5 and 6 Bridge Supports")]
 public static void ApplyFromMenu(){Debug.Log(Apply());}
 public static string Apply() {
  if(EditorApplication.isPlaying)throw new InvalidOperationException("Exit Play mode before updating bridge artwork.");
  for(int i=0;i<SceneManager.sceneCount;i++)if(SceneManager.GetSceneAt(i).isDirty)throw new InvalidOperationException("Save scene edits before updating bridge artwork.");
  var previous=SceneManager.GetActiveScene();var results=new string[2];
  for(int room=5;room<=6;room++) {
   string path="Assets/_OneMoreTime/Scenes/Main_Room0"+room+".unity";
   var scene=SceneManager.GetSceneByPath(path);bool loaded=scene.IsValid()&&scene.isLoaded;
   if(!loaded)scene=EditorSceneManager.OpenScene(path,OpenSceneMode.Additive);
   try {results[room-5]=Patch(scene);EditorSceneManager.SaveScene(scene);}
   finally {if(!loaded)EditorSceneManager.CloseScene(scene,true);}
  }
  AssetDatabase.SaveAssets();if(previous.IsValid()&&previous.isLoaded)SceneManager.SetActiveScene(previous);
  return string.Join("\n",results);
 }
}
}
