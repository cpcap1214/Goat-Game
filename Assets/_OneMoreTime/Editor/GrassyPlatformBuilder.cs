using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;
using Object=UnityEngine.Object;

namespace OneMoreTime.EditorTools {
/// <summary>Only owns grass-column artwork; never rebuilds room gameplay objects.</summary>
public static class GrassyPlatformBuilder {
 const string Root=DemoBuilder.Root;
 public const string Generated=Root+"/Generated/GrassyColumns";
 public const string PrefabPath=Root+"/Prefabs/WorldOne/GrassyColumnPlatform.prefab";
 public const string BackgroundPath=Root+"/Prefabs/Background/GrassyColumnPlatformBackground.prefab";
 static readonly string[] Names={"GrassLeft","GrassMiddle","GrassRight","SoilLeft","SoilMiddle","SoilRight","SoilBottomLeft","SoilBottomMiddle","SoilBottomRight"};
 static readonly Color Dark=new Color(.5f,.5f,.5f,1);
 static Sprite Load(string name)=>AssetDatabase.LoadAssetAtPath<Sprite>(Generated+"/"+name+".asset");

 [MenuItem("Tools/One More Time/Build Rectangular Grassy Platforms")]
 public static void Build(){
  if(EditorApplication.isPlaying)throw new InvalidOperationException("Exit Play mode before updating platform art");
  EnsureAssets();BuildPrefab(false);BuildPrefab(true);ApplyToMainRooms();AssetDatabase.SaveAssets();
 }
 public static void EnsureAssets(){
  Directory.CreateDirectory(Generated);Directory.CreateDirectory(Root+"/Prefabs/WorldOne");Directory.CreateDirectory(Root+"/Prefabs/Background");AssetDatabase.Refresh();
  var atlas=new Texture2D(2,2);atlas.LoadImage(File.ReadAllBytes("Assets/SunnyLand Artwork/Environment/Tileset/tileset-sliced.png"));
  for(int i=0;i<Names.Length;i++){
   string path=Generated+"/"+Names[i];var texture=AssetDatabase.LoadAssetAtPath<Texture2D>(path+"Texture.asset");
   if(texture==null){texture=new Texture2D(16,16,TextureFormat.RGBA32,false);texture.name=Names[i]+"Texture";texture.SetPixels(atlas.GetPixels(16+(i%3)*32,336-(i/3)*32,16,16));texture.filterMode=FilterMode.Point;texture.wrapMode=TextureWrapMode.Clamp;texture.Apply();AssetDatabase.CreateAsset(texture,path+"Texture.asset");}
   var sprite=Load(Names[i]);if(sprite==null){sprite=Sprite.Create(texture,new Rect(0,0,16,16),new Vector2(.5f,.5f),16,0,SpriteMeshType.FullRect);sprite.name=Names[i];AssetDatabase.CreateAsset(sprite,path+".asset");}
   foreach(bool background in new[]{false,true}){string tilePath=path+(background?"BackgroundTile":"Tile")+".asset";var tile=AssetDatabase.LoadAssetAtPath<Tile>(tilePath);if(tile==null){tile=ScriptableObject.CreateInstance<Tile>();AssetDatabase.CreateAsset(tile,tilePath);}tile.sprite=sprite;tile.color=background?Dark:Color.white;tile.colliderType=Tile.ColliderType.None;EditorUtility.SetDirty(tile);}
  }
  Object.DestroyImmediate(atlas);
  string materialPath=Generated+"/GrassyColumn.mat";if(AssetDatabase.LoadAssetAtPath<Material>(materialPath)==null)AssetDatabase.CreateAsset(new Material(Shader.Find("Sprites/Default")),materialPath);
 }
 static void Configure(GrassyColumnVisual visual){
  visual.grassLeft=Load("GrassLeft");visual.grassMiddle=Load("GrassMiddle");visual.grassRight=Load("GrassRight");
  visual.soilLeft=Load("SoilLeft");visual.soilMiddle=Load("SoilMiddle");visual.soilRight=Load("SoilRight");
  visual.bottomLeft=Load("SoilBottomLeft");visual.bottomMiddle=Load("SoilBottomMiddle");visual.bottomRight=Load("SoilBottomRight");
  visual.spriteMaterial=AssetDatabase.LoadAssetAtPath<Material>(Generated+"/GrassyColumn.mat");
 }
 public static GrassyColumnVisual Attach(Transform platform,Rect surface,float bottom){
  if(Load("GrassMiddle")==null)EnsureAssets();
  var visual=platform.GetComponent<GrassyColumnVisual>();if(visual==null)visual=platform.gameObject.AddComponent<GrassyColumnVisual>();
  Configure(visual);visual.width=surface.width/Mathf.Abs(platform.lossyScale.x);visual.depth=Mathf.Max(1,(surface.yMax-bottom)/Mathf.Abs(platform.lossyScale.y));visual.topOffset=platform.InverseTransformPoint(new Vector3(surface.center.x,surface.yMax,0)).y;
  visual.resizeTopCollider=false;visual.Refresh();EditorUtility.SetDirty(visual);return visual;
 }
 static void BuildPrefab(bool background){
  var go=new GameObject(background?"Grassy Column Background":"Grassy Column Platform");var visual=go.AddComponent<GrassyColumnVisual>();Configure(visual);visual.width=5;visual.depth=6;visual.topOffset=0;visual.background=background;visual.resizeTopCollider=!background;
  if(!background){go.layer=DemoBuilder.GetLayer();go.AddComponent<BoxCollider2D>();go.AddComponent<OneWayPlatform>();}
  visual.Refresh();PrefabUtility.SaveAsPrefabAsset(go,background?BackgroundPath:PrefabPath);Object.DestroyImmediate(go);
 }
 [MenuItem("Tools/One More Time/Apply Grass Art to Existing Main Rooms")]
 public static void ApplyToMainRooms(){
  if(EditorApplication.isPlaying)throw new InvalidOperationException("Exit Play mode before updating platform art");
  EnsureAssets();var previous=SceneManager.GetActiveScene();var report=new List<string>();
  foreach(string name in new[]{"Main_Room01","Main_Room02","Main_Room03","Main_Room05"}){
   string path=Root+"/Scenes/"+name+".unity";var scene=SceneManager.GetSceneByPath(path);bool loaded=scene.IsValid()&&scene.isLoaded;
   if(loaded&&scene.isDirty)throw new InvalidOperationException("Save existing edits in "+name+" before the grass art migration");
   if(!loaded)scene=EditorSceneManager.OpenScene(path,OpenSceneMode.Additive);
   try{
    Physics2D.SyncTransforms();var colliders=scene.GetRootGameObjects().SelectMany(g=>g.GetComponentsInChildren<Collider2D>(true)).ToDictionary(c=>c.GetEntityId(),c=>EditorJsonUtility.ToJson(c));
    var all=scene.GetRootGameObjects().SelectMany(g=>g.GetComponentsInChildren<Transform>(true)).ToArray();int count=0;
    var candidates=all.Select(t=>t.GetComponent<OneWayPlatform>()).Where(p=>p!=null&&(p.GetComponent<GrassyColumnVisual>()!=null||p.transform.Find("Grass blades and roots")!=null||p.name.ToLowerInvariant().Contains("grass"))).ToArray();
    foreach(var platform in candidates){
     var box=platform.GetComponent<BoxCollider2D>();var bounds=box.bounds;var surface=new Rect(bounds.min.x,bounds.min.y,bounds.size.x,bounds.size.y);
     var oldPillars=all.Where(t=>t!=null&&t.name.StartsWith("Dirt pillar")&&MatchingPillar(t,surface)).ToArray();
     float bottom=oldPillars.Length>0?oldPillars.Min(t=>t.GetComponentsInChildren<SpriteRenderer>(true).Min(s=>s.bounds.min.y)):surface.yMax-(platform.GetComponent<GrassyColumnVisual>()?.depth??6);
     foreach(var pillar in oldPillars)Object.DestroyImmediate(pillar.gameObject);
     foreach(string oldName in new[]{"Grass blades and roots","Grass platform top"}){var old=platform.transform.Find(oldName);if(old!=null)Object.DestroyImmediate(old.gameObject);}
     Attach(platform.transform,surface,bottom);count++;
    }
    var after=scene.GetRootGameObjects().SelectMany(g=>g.GetComponentsInChildren<Collider2D>(true)).ToDictionary(c=>c.GetEntityId(),c=>EditorJsonUtility.ToJson(c));
    if(colliders.Count!=after.Count||colliders.Any(pair=>!after.TryGetValue(pair.Key,out var json)||pair.Value!=json))throw new InvalidOperationException(name+": art migration changed collision data");
    if(count>0){EditorSceneManager.MarkSceneDirty(scene);EditorSceneManager.SaveScene(scene);}report.Add(name+": "+count+" grass columns refreshed; PASS "+colliders.Count+" original colliders byte-equivalent and gameplay objects retained.");
   }finally{if(!loaded)EditorSceneManager.CloseScene(scene,true);}
  }
  if(previous.IsValid()&&previous.isLoaded)SceneManager.SetActiveScene(previous);
  Directory.CreateDirectory("Artifacts/Logs");File.WriteAllLines("Artifacts/Logs/grassy-column-migration.txt",report);
 }
 static bool MatchingPillar(Transform pillar,Rect surface){
  var sprites=pillar.GetComponentsInChildren<SpriteRenderer>(true);if(sprites.Length==0)return false;var bounds=sprites[0].bounds;foreach(var s in sprites.Skip(1))bounds.Encapsulate(s.bounds);
  return Mathf.Abs(bounds.center.x-surface.center.x)<.02f&&Mathf.Abs(bounds.size.x-surface.width)<.02f&&Mathf.Abs(bounds.max.y-surface.yMax)<.05f;
 }
 public static string Validate(){
  var prefab=AssetDatabase.LoadAssetAtPath<GameObject>(PrefabPath);var instance=(GameObject)PrefabUtility.InstantiatePrefab(prefab);try{
   var visual=instance.GetComponent<GrassyColumnVisual>();visual.width=7;visual.depth=9;visual.Refresh();Physics2D.SyncTransforms();var box=instance.GetComponent<BoxCollider2D>();
   if(Mathf.Abs(box.size.x-7)>.001f||Mathf.Abs(box.bounds.max.y-instance.transform.position.y)>.001f||instance.GetComponentsInChildren<Collider2D>().Length!=1||instance.GetComponent<OneWayPlatform>()==null)throw new InvalidOperationException("Grassy platform collision invariant failed");
   var body=instance.transform.Find("Rectangular grass column art/Soil interior").GetComponent<SpriteRenderer>();if(body.sprite.pixelsPerUnit!=16||body.drawMode!=SpriteDrawMode.Tiled||Mathf.Abs(body.size.y-7)>.001f)throw new InvalidOperationException("Grassy platform tiling invariant failed");
   var bg=AssetDatabase.LoadAssetAtPath<GameObject>(BackgroundPath);if(bg.GetComponentsInChildren<Collider2D>(true).Length!=0||!bg.GetComponent<GrassyColumnVisual>().background)throw new InvalidOperationException("Background platform must remain visual only");
   return "PASS: native 16-pixel rectangular terrain, configurable width/depth, top-only one-way collider, no pillar/background colliders.";
  }finally{Object.DestroyImmediate(instance);}
 }
}
}
