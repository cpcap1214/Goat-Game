using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;
using Object=UnityEngine.Object;
namespace OneMoreTime.EditorTools {
public static class BackgroundVariantBuilder {
 const string Root="Assets/_OneMoreTime";
 static readonly Color Tint=new Color(.5f,.5f,.5f,1);
 [MenuItem("Tools/One More Time/Build Background Variants")]
 public static void Build(){
  if(EditorApplication.isPlaying)throw new System.InvalidOperationException("Exit Play mode first");
  Directory.CreateDirectory(Root+"/Generated/Background");Directory.CreateDirectory(Root+"/Prefabs/Background");AssetDatabase.Refresh();
  foreach(var guid in AssetDatabase.FindAssets("t:TileBase",new[]{Root+"/Generated"})){
   string path=AssetDatabase.GUIDToAssetPath(guid);if(path.Contains("/Background/"))continue;
   var source=AssetDatabase.LoadAssetAtPath<TileBase>(path);if(source==null)continue;
   string dest=Root+"/Generated/Background/"+source.name+"Background.asset";
   var tile=AssetDatabase.LoadAssetAtPath<BackgroundTile>(dest);if(tile==null){tile=ScriptableObject.CreateInstance<BackgroundTile>();AssetDatabase.CreateAsset(tile,dest);}tile.source=source;tile.tint=Tint;EditorUtility.SetDirty(tile);
  }
  // Original terrain is stored as Sprite assets, not TileBase assets.
  foreach(var guid in AssetDatabase.FindAssets("t:Sprite",new[]{Root+"/Generated"})){
   string path=AssetDatabase.GUIDToAssetPath(guid);if(path.Contains("/Background/"))continue;
   var sprite=AssetDatabase.LoadAssetAtPath<Sprite>(path);if(sprite==null)continue;
   string n=Path.GetFileNameWithoutExtension(path);
   if(!(n.StartsWith("Earth")||n.StartsWith("Grass")||n.StartsWith("Dirt")||n.StartsWith("RockSoil")||n.StartsWith("TerrainLandscapeSprite")||n.StartsWith("WoodenPlatform")||n.StartsWith("Bridge")))continue;
   string dest=Root+"/Generated/Background/"+n+"SpriteBackground.asset";
   var tile=AssetDatabase.LoadAssetAtPath<Tile>(dest);if(tile==null){tile=ScriptableObject.CreateInstance<Tile>();AssetDatabase.CreateAsset(tile,dest);}tile.sprite=sprite;tile.color=Tint;tile.colliderType=Tile.ColliderType.None;EditorUtility.SetDirty(tile);
  }
  foreach(var guid in AssetDatabase.FindAssets("t:Prefab",new[]{Root+"/Prefabs"})){
   string path=AssetDatabase.GUIDToAssetPath(guid);if(path.Contains("/Background/"))continue;
   var source=AssetDatabase.LoadAssetAtPath<GameObject>(path);
   if(source.GetComponentInChildren<OneWayPlatform>(true)!=null||source.GetComponentInChildren<RoomPlatform>(true)!=null||source.GetComponentInChildren<Tilemap>(true)!=null||source.name.Contains("Platform")||source.name.Contains("Stream")||source.name.Contains("Bridge")){
    if(source.GetComponentsInChildren<Renderer>(true).Length>0)SaveVisual(source,Path.GetFileNameWithoutExtension(path));
   }
  }
  // Main-room grassy platforms are scene-built; preserve their cap/pillar artwork too.
  foreach(string name in new[]{"Main_Room01","Main_Room02","Main_Room03","Main_Room05"}){
   string path=Root+"/Scenes/"+name+".unity";var scene=SceneManager.GetSceneByPath(path);bool loaded=scene.IsValid()&&scene.isLoaded;
   if(!loaded)scene=EditorSceneManager.OpenScene(path,OpenSceneMode.Additive);
   int i=0;foreach(var platform in scene.GetRootGameObjects().SelectMany(g=>g.GetComponentsInChildren<OneWayPlatform>(true))){SaveVisual(platform.gameObject,name+"_"+(i++)+"_"+platform.name);}
   if(!loaded)EditorSceneManager.CloseScene(scene,true);
  }
  AssetDatabase.SaveAssets();
 }
 static void SaveVisual(GameObject source,string name){
  var clone=Object.Instantiate(source);clone.name=name+" Background";clone.SetActive(false);
  // Backgrounds keep rendered geometry/materials only; no moving-platform,
  // hazard, checkpoint or collision logic can affect the foreground player.
  // Remove dependents first (the bridge requires OneWayPlatform).
  foreach(var behaviour in clone.GetComponentsInChildren<MonoBehaviour>(true).OrderBy(b=>b is OneWayPlatform?1:0))Object.DestroyImmediate(behaviour);
  foreach(var c in clone.GetComponentsInChildren<Collider2D>(true))Object.DestroyImmediate(c);
  foreach(var c in clone.GetComponentsInChildren<Rigidbody2D>(true))Object.DestroyImmediate(c);
  foreach(var renderer in clone.GetComponentsInChildren<Renderer>(true)){renderer.sortingLayerID=0;renderer.sortingOrder=-20;if(renderer is SpriteRenderer sr)sr.color*=Tint;else if(renderer is TilemapRenderer)renderer.GetComponent<Tilemap>().color*=Tint;}
  clone.transform.position=Vector3.zero;clone.SetActive(true);
  PrefabUtility.SaveAsPrefabAsset(clone,Root+"/Prefabs/Background/"+name.Replace('/','_')+"Background.prefab");Object.DestroyImmediate(clone);
 }
}
}
