using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

namespace OneMoreTime.EditorTools {
/// <summary>Art-only overlay. Owns one child per room; never rebuilds terrain or mechanisms.</summary>
public static partial class MainRoomDecorationBuilder {
 public const string ManagedName = "Chapter 1 expansion scenery";
 const string SceneFolder = "Assets/_OneMoreTime/Scenes/";
 const string Evidence = "Artifacts/Logs/";

 [MenuItem("Tools/One More Time/Decorate Main Rooms With Forest Expansion")]
 public static void Build() { foreach(int room in new[]{1,2,3,5})Apply(room); }

 public static string Apply(int number) {
  if(number<1||number>5||number==4)throw new ArgumentOutOfRangeException(nameof(number));
  if(EditorApplication.isPlaying)throw new InvalidOperationException("Exit Play mode before decorating rooms.");
  var previous=SceneManager.GetActiveScene();
  string path=SceneFolder+"Main_Room0"+number+".unity";
  var scene=SceneManager.GetSceneByPath(path);bool loaded=scene.IsValid()&&scene.isLoaded;
  if(loaded&&scene.isDirty)throw new InvalidOperationException("Save your edits to "+path+" before decorating it.");
  if(!loaded)scene=EditorSceneManager.OpenScene(path,OpenSceneMode.Additive);
  SceneManager.SetActiveScene(scene);
  try {
   var room=scene.GetRootGameObjects().SelectMany(g=>g.GetComponentsInChildren<MainRoom>(true)).Single();
   var before=Snapshot(scene);
   var old=room.transform.Find(ManagedName);if(old!=null)Object.DestroyImmediate(old.gameObject);
   var root=new GameObject(ManagedName).transform;root.SetParent(room.transform,false);
   switch(number){case 1:DecorateRoom1(root);break;case 2:DecorateRoom2(root);break;case 3:DecorateRoom3(root);break;case 5:DecorateRoom4(root);break;}
   string cleanup=ChapterDecorationCleanup.ApplyToRoom(room,number);
   ValidateDecoration(root);
   var after=Snapshot(scene);
   if(before.Count!=after.Count||before.Any(p=>!after.TryGetValue(p.Key,out var value)||p.Value!=value))
    throw new InvalidOperationException("Decoration changed an existing object in Room "+number+". Scene has not been saved.");
   EditorSceneManager.MarkSceneDirty(scene);EditorSceneManager.SaveScene(scene);
   string report="PASS Room "+number+": "+root.childCount+" retained water groups; "+before.Count+" existing component/transform records unchanged; zero added colliders, platforms, hazards or rewind participants. "+cleanup;
   Directory.CreateDirectory(Evidence);File.AppendAllText(Evidence+"main-room-decoration-validation.txt",report+"\n");
   return report;
  } finally {
   if(!loaded)EditorSceneManager.CloseScene(scene,true);
   if(previous.IsValid()&&previous.isLoaded)SceneManager.SetActiveScene(previous);
  }
 }

 // Includes disabled components, pooled objects, original art, physics, ability configuration,
 // transforms and active flags. Only the original scenery and its grounded replacement
 // are excluded; the cleanup helper independently checks everything outside its props.
 static Dictionary<string,string> Snapshot(Scene scene) {
  var result=new Dictionary<string,string>();
  foreach(var root in scene.GetRootGameObjects())foreach(var t in root.GetComponentsInChildren<Transform>(true)) {
   bool owned=false;for(var p=t;p!=null;p=p.parent)if(p.name==ManagedName||p.name==ChapterDecorationCleanup.ManagedName){owned=true;break;}
   if(owned)continue;
   string id=t.GetEntityId().ToString();
   result[id+"/transform"]=t.localPosition.ToString("R")+t.localRotation.ToString("R")+t.localScale.ToString("R")+t.name+"/"+t.gameObject.activeSelf+"/"+t.gameObject.layer;
   foreach(var component in t.GetComponents<Component>()) {
    if(component==null)throw new InvalidOperationException("Missing script on "+t.name);
    if(component is Transform)continue;
    result[component.GetEntityId().ToString()+"/component"]=EditorJsonUtility.ToJson(component);
   }
  }
  return result;
 }
 static void ValidateDecoration(Transform root) {
  foreach(var c in root.GetComponentsInChildren<Component>(true))
   if(!(c is Transform)&&!(c is SpriteRenderer)&&!(c is WaterFeature))
    throw new InvalidOperationException("Decoration must remain visual only: "+c.GetType().Name);
  foreach(var sr in root.GetComponentsInChildren<SpriteRenderer>(true)) {
   if(sr.sprite==null||sr.sharedMaterial==null||!sr.sharedMaterial.shader.isSupported)
    throw new InvalidOperationException("Invalid art/material: "+sr.name);
   bool transparentWater=sr.GetComponent<WaterFeature>()!=null||sr.sharedMaterial.shader.name=="OneMoreTime/Water Accent";
   if(sr.sortingOrder>=10&&(sr.color.a>.38f||!transparentWater))
    throw new InvalidOperationException("Opaque decoration would cover the goat: "+sr.name);
  }
 }
 static GameObject Put(Transform root,string path,float x,float y) {
  var prefab=AssetDatabase.LoadAssetAtPath<GameObject>(path);
  if(prefab==null)throw new InvalidOperationException("Build the forest expansion first. Missing "+path);
  var go=(GameObject)PrefabUtility.InstantiatePrefab(prefab,root.gameObject.scene);
  go.transform.SetParent(root,false);go.transform.position=new Vector3(x,y,0);return go;
 }
 static GameObject Prop(Transform root,string name,float x,float y,bool background=false,bool flip=false) {
  var go=Put(root,ForestExpansionBuilder.Prefabs+"/"+name+(background?"Background":"")+".prefab",x,y);
  foreach(var sr in go.GetComponentsInChildren<SpriteRenderer>())sr.flipX=flip;
  return go;
 }
 static GameObject Water(Transform root,string name,float x,float y) {
  var go=Put(root,WaterFeatureBuilder.Prefabs+"/"+name+".prefab",x,y);
  foreach(var water in go.GetComponentsInChildren<WaterFeature>())water.Refresh();return go;
 }
 // A face detail sits above the dirt artwork but below all player sprites.
 static void WallDetail(Transform root,string name,float x,float y,bool flip=false) {
  var go=Prop(root,name,x,y,true,flip);
  foreach(var sr in go.GetComponentsInChildren<SpriteRenderer>())sr.sortingOrder=5;
 }

 static void DecorateRoom4(Transform root) {
  // The lift shaft stays open; its working platform and the bridge pickup remain legible.
  Prop(root,"BracedTimber",5,4,true);
  Prop(root,"SmallFern",4.5f,4);
  Prop(root,"HollowStump",8.1f,4);
  Prop(root,"ForestMushrooms",8.7f,4);
  Prop(root,"WindsweptPine",10.7f,4,true);
  Prop(root,"BroadFern",12,4);
  Prop(root,"MeadowFlowers",14,4);
  WallDetail(root,"HangingRoots",9,3.4f);
  WallDetail(root,"ShelfFungus",5.4f,1.3f);
  // Old riverworks behind the bridge; no extra deck or solid stepping stone.
  Prop(root,"BrokenPillar",17.4f,-2.5f,true);
  Water(root,"FineWaterMistBackground",19,-.6f);
  Prop(root,"SmallFern",23.8f,4);
  Prop(root,"CarvedStone",27.7f,4,true);
  Prop(root,"RiverReeds",28.2f,4);
  WallDetail(root,"HangingRoots",26.9f,3.2f);
  // The wheel is an abandoned scenic structure beyond the stream, never a new platform.
  Prop(root,"OldWaterwheel",32.4f,.1f,true);
  Prop(root,"SmallFern",31.2f,6);
  Prop(root,"ForestMushrooms",36,5);
  Water(root,"ImpactFoam",38.9f,1.65f);
  Water(root,"FineWaterMistForeground",39.8f,1.2f);
  // Water accents stay attached to the established channel and waterfall.
  // Save the giant waterfall reveal for the later chapter climax.
  Prop(root,"RockSpire",57,-3,true);
  Prop(root,"CliffCluster",52,-3,true);
  Prop(root,"SmallFern",44.7f,5.6f);
  Prop(root,"MeadowFlowers",46.9f,5.6f);
  // The acquisition room's only whirlpool is its functional crate drain,
  // owned by RoomFourUpdate so scenery refreshes cannot duplicate it.
  // The shallow foreground spray stays below the crate tops and never hides their path.
  Water(root,"FineWaterMistForeground",59.3f,2.5f);
  Prop(root,"RiverReeds",65.1f,5);
  Prop(root,"RockyStreamBank",66.3f,5,true);
  Prop(root,"AlpineShrub",67.6f,5);
  WallDetail(root,"HangingRoots",66.8f,4.3f);
 }

 public static string Capture(int number) {
  if(EditorApplication.isPlaying)throw new InvalidOperationException("Exit Play mode before capturing.");
  for(int i=0;i<SceneManager.sceneCount;i++)if(SceneManager.GetSceneAt(i).isDirty)
   throw new InvalidOperationException("Save scene edits before capture.");
  var setup=EditorSceneManager.GetSceneManagerSetup();
  string path=SceneFolder+"Main_Room0"+number+".unity";
  // A camera sees objects in every loaded scene. Isolate the room so a loaded
  // gallery or another room cannot appear in its review images.
  var scene=EditorSceneManager.OpenScene(path,OpenSceneMode.Single);
  var room=scene.GetRootGameObjects().SelectMany(g=>g.GetComponentsInChildren<MainRoom>(true)).Single();
  var cam=room.view;
  Vector2[] centers=number==1?new[]{new Vector2(12,3.5f),new Vector2(60,8)}:
   number==2?new[]{new Vector2(10,3.5f),new Vector2(29,7)}:
   number==3?new[]{new Vector2(15,7),new Vector2(15,25),new Vector2(22,38)}:
   new[]{new Vector2(11,7),new Vector2(34,5),new Vector2(58,6)};
  var originalPosition=cam.transform.position;float originalSize=cam.orthographicSize,originalAspect=cam.aspect;
  var originalVisual=room.visual.transform.localPosition;
  Directory.CreateDirectory(Evidence);
  try {
   for(int i=0;i<centers.Length;i++) {
    cam.transform.position=new Vector3(centers[i].x,centers[i].y,-10);cam.orthographicSize=5.625f;cam.aspect=16f/9;
    foreach(var backdrop in scene.GetRootGameObjects().SelectMany(g=>g.GetComponentsInChildren<MainRoomBackdrop>()))backdrop.SendMessage("LateUpdate");
    foreach(var backdrop in scene.GetRootGameObjects().SelectMany(g=>g.GetComponentsInChildren<WorldOneBackdrop>()))backdrop.SendMessage("LateUpdate");
    foreach(var feature in scene.GetRootGameObjects().SelectMany(g=>g.GetComponentsInChildren<WaterFeature>()))feature.Refresh();
    var rt=new RenderTexture(1600,900,24);var oldTarget=cam.targetTexture;var oldActive=RenderTexture.active;
    var read=new Texture2D(1600,900,TextureFormat.RGBA32,false);
    try {cam.targetTexture=rt;cam.Render();RenderTexture.active=rt;read.ReadPixels(new Rect(0,0,1600,900),0,0);read.Apply();File.WriteAllBytes(Evidence+"main-room-"+number+"-decorated-"+(i+1)+".png",read.EncodeToPNG());}
    finally {cam.targetTexture=oldTarget;RenderTexture.active=oldActive;Object.DestroyImmediate(read);rt.Release();Object.DestroyImmediate(rt);}
   }
  } finally {
   cam.transform.position=originalPosition;cam.orthographicSize=originalSize;cam.aspect=originalAspect;room.visual.transform.localPosition=originalVisual;
   // Discard capture-only transforms and restore the user's saved scene setup.
   EditorSceneManager.RestoreSceneManagerSetup(setup);
  }
  return "Captured Room "+number+" at its 20-unit gameplay camera width.";
 }
}
}
