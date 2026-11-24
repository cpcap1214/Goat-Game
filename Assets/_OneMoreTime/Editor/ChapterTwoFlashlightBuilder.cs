using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object=UnityEngine.Object;

namespace OneMoreTime.EditorTools {
/// <summary>Owns only the flashlight sandbox and its new Chapter Two authoring prefabs.</summary>
public static class ChapterTwoFlashlightBuilder {
 const string R="Assets/_OneMoreTime";
 public const string ScenePath=R+"/Scenes/Debug_ChapterTwoFlashlight.unity";
 public const string PrefabDir=R+"/Prefabs/ChapterTwo";
 public const string ManagedRoot="Chapter 2 Flashlight Managed";
 const string ShaderPath=R+"/Shaders/ChapterTwoObscurity.shader";

 [MenuItem("Tools/One More Time/Build Chapter 2 Flashlight Sandbox")]
 public static string Build(){
  if(EditorApplication.isPlaying)throw new InvalidOperationException("Exit Play mode before building the flashlight sandbox.");
  var shader=Require<Shader>(ShaderPath);
  Require<GameObject>(R+"/Prefabs/Player.prefab");
  Require<GameObject>(GrassyPlatformBuilder.PrefabPath);
  Require<PlayerTuning>(R+"/Config/MainRoomPlayerTuning.asset");
  if(PresentationBuilder.Font==null)throw new InvalidOperationException("The existing UI font is missing.");
  var previous=SceneManager.GetActiveScene();var scene=SceneManager.GetSceneByPath(ScenePath);bool loaded=scene.IsValid()&&scene.isLoaded;
  if(loaded&&scene.isDirty)throw new InvalidOperationException("Save your flashlight sandbox edits before rebuilding its managed root.");
  if(!loaded)scene=File.Exists(ScenePath)?EditorSceneManager.OpenScene(ScenePath,OpenSceneMode.Additive):EditorSceneManager.NewScene(NewSceneSetup.EmptyScene,NewSceneMode.Additive);
  SceneManager.SetActiveScene(scene);
  try {
   BuildPrefabs(shader);
   var old=scene.GetRootGameObjects().FirstOrDefault(g=>g.name==ManagedRoot);if(old!=null)Object.DestroyImmediate(old);
   var root=Group(null,ManagedRoot);var terrain=Group(root,"Cavern stepping route");var art=Group(root,"Cavern scenery — no collision");
   var player=Put(root,R+"/Prefabs/Player.prefab",new Vector2(3,0)).GetComponent<PlayerMotor2D>();
   player.tuning=Require<PlayerTuning>(R+"/Config/MainRoomPlayerTuning.asset");player.worldMask=1<<DemoBuilder.GetLayer();
   var visual=player.GetComponentInChildren<PlayerVisual>();visual.Pose(new Vector2(3,0),1,0,0,Color.white);
   var ability=Put(root,PrefabDir+"/FlashlightAbility.prefab",Vector2.zero).GetComponent<FlashlightAbility>();ability.player=player;ability.unlocked=true;
   var camera=Group(root,"Cavern camera — 20 units wide").gameObject.AddComponent<Camera>();camera.tag="MainCamera";camera.orthographic=true;camera.orthographicSize=5.625f;camera.backgroundColor=new Color(.075f,.074f,.09f);camera.clearFlags=CameraClearFlags.SolidColor;camera.transform.position=new Vector3(10,2.625f,-10);camera.gameObject.AddComponent<AudioListener>();
   var playtest=root.gameObject.AddComponent<ChapterTwoFlashlightPlaytest>();playtest.player=player;playtest.visual=visual;playtest.flashlight=ability;playtest.view=camera;playtest.font=PresentationBuilder.Font;playtest.bounds=new Rect(0,-3,48,16);playtest.spawn=new Vector2(3,0);playtest.deathY=-5;playtest.AutoTick=true;

   // A continuous low floor makes this a forgiving ability test, not an authored Chapter 2 room.
   Solid(terrain,"Lower recovery floor",0,48,-2,-5);
   Solid(terrain,"Dry entrance bank",0,8,0,-2);
   Grass(terrain,"Darkness step",12,1.2f,4,5);
   Wood(terrain,"Darkness crossing",17,20,2.8f);
   Grass(terrain,"Central refuge",24,1.2f,4,5);
   Wood(terrain,"Mist crossing",29,32,2.6f);
   Grass(terrain,"Mist descent",36.5f,.7f,4,5);
   Solid(terrain,"Lit far bank",40,48,0,-2);
   Boundary(terrain,"Left boundary",-.3f);Boundary(terrain,"Right boundary",48.3f);

   Zone(root,"Darkness — flashlight clears a temporary cone",PrefabDir+"/DarknessVolume.prefab",new Vector2(15.5f,5.5f),new Vector2(15,19),ability);
   Zone(root,"Mist — flashlight parts the haze",PrefabDir+"/MistVolume.prefab",new Vector2(32.5f,5.5f),new Vector2(15,19),ability);
   Decorate(art);
   Validate(root,playtest);
   EditorSceneManager.MarkSceneDirty(scene);
   if(!EditorSceneManager.SaveScene(scene,ScenePath))throw new InvalidOperationException("Could not save the flashlight sandbox.");
   var entries=EditorBuildSettings.scenes;
   if(!entries.Any(s=>s.path==ScenePath))EditorBuildSettings.scenes=entries.Concat(new[]{new EditorBuildSettingsScene(ScenePath,true)}).ToArray();
   else if(entries.Any(s=>s.path==ScenePath&&!s.enabled))EditorBuildSettings.scenes=entries.Select(s=>s.path==ScenePath?new EditorBuildSettingsScene(ScenePath,true):s).ToArray();
   AssetDatabase.SaveAssets();
   return "Built Chapter 2 flashlight sandbox: two obscurity zones, 48-unit reversible test lane, three reusable prefabs; existing chapters unchanged.";
  } finally {if(!loaded)EditorSceneManager.CloseScene(scene,true);if(previous.IsValid()&&previous.isLoaded)SceneManager.SetActiveScene(previous);}
 }

 static T Require<T>(string path)where T:Object {var asset=AssetDatabase.LoadAssetAtPath<T>(path);if(asset==null)throw new InvalidOperationException("Missing existing asset: "+path);return asset;}
 static Transform Group(Transform parent,string name){var t=new GameObject(name).transform;if(parent!=null)t.SetParent(parent,false);return t;}
 static GameObject Put(Transform parent,string path,Vector2 position){var go=(GameObject)PrefabUtility.InstantiatePrefab(Require<GameObject>(path),parent.gameObject.scene);go.transform.SetParent(parent,false);go.transform.position=position;return go;}
 static void BuildPrefabs(Shader shader){
  Directory.CreateDirectory(PrefabDir);AssetDatabase.Refresh();
  var lamp=new GameObject("Flashlight ability — assign player and unlock in Chapter 2");
  try {var a=lamp.AddComponent<FlashlightAbility>();a.unlocked=false;a.activeDuration=4;a.cooldownDuration=3;a.range=6;a.nearRadius=1.1f;a.halfAngle=32;PrefabUtility.SaveAsPrefabAsset(lamp,PrefabDir+"/FlashlightAbility.prefab");}
  finally {Object.DestroyImmediate(lamp);}
  foreach(var kind in new[]{ChapterTwoObscurity.Kind.Darkness,ChapterTwoObscurity.Kind.Mist}){
   var go=new GameObject(kind+" volume — assign flashlight");
   try {var zone=go.AddComponent<ChapterTwoObscurity>();zone.kind=kind;zone.size=new Vector2(12,12);zone.opacity=kind==ChapterTwoObscurity.Kind.Darkness?.96f:.78f;zone.edgeFeather=kind==ChapterTwoObscurity.Kind.Darkness?.5f:1.5f;zone.tint=kind==ChapterTwoObscurity.Kind.Darkness?new Color(.015f,.014f,.025f):new Color(.46f,.5f,.5f);zone.shader=shader;PrefabUtility.SaveAsPrefabAsset(go,PrefabDir+"/"+kind+"Volume.prefab");}
   finally {Object.DestroyImmediate(go);}
  }
 }
 static void Solid(Transform parent,string name,float left,float right,float top,float bottom){
  var t=Group(parent,name);var rect=Rect.MinMaxRect(left,bottom,right,top);t.position=rect.center;t.gameObject.layer=DemoBuilder.GetLayer();t.gameObject.AddComponent<BoxCollider2D>().size=rect.size;PresentationBuilder.TerrainSkin(t.gameObject,rect,"Ground");
  foreach(var sr in t.GetComponentsInChildren<SpriteRenderer>())sr.color*=new Color(.7f,.68f,.66f,1);
 }
 static void Grass(Transform parent,string name,float x,float top,float width,float depth){
  var g=Put(parent,GrassyPlatformBuilder.PrefabPath,new Vector2(x,top));g.name=name;var c=g.GetComponent<GrassyColumnVisual>();c.width=width;c.depth=depth;c.resizeTopCollider=true;c.soilColor=new Color(.6f,.58f,.57f);c.Refresh();
 }
 static void Wood(Transform parent,string name,float left,float right,float top){
  var t=Group(parent,name);t.position=new Vector2((left+right)/2,top-.09f);t.gameObject.layer=DemoBuilder.GetLayer();t.gameObject.AddComponent<BoxCollider2D>().size=new Vector2(right-left,.18f);t.gameObject.AddComponent<OneWayPlatform>();MainRoomArtBuilder.Wood(t,new Rect(left,top-.18f,right-left,.18f));
 }
 static void Boundary(Transform parent,string name,float x){var t=Group(parent,name);t.position=new Vector2(x,5);t.gameObject.layer=DemoBuilder.GetLayer();t.gameObject.AddComponent<BoxCollider2D>().size=new Vector2(.4f,50);}
 static void Zone(Transform parent,string name,string prefab,Vector2 center,Vector2 size,FlashlightAbility lamp){var g=Put(parent,prefab,center);g.name=name;var z=g.GetComponent<ChapterTwoObscurity>();z.size=size;z.flashlight=lamp;z.RefreshVisual();}
 static GameObject Prop(Transform parent,string type,float x,float y,Vector2 scale,bool background=true){var go=Put(parent,ForestExpansionBuilder.Prefabs+"/"+type+(background?"Background":"")+".prefab",new Vector2(x,y));go.transform.localScale=new Vector3(scale.x,scale.y,1);foreach(var sr in go.GetComponentsInChildren<SpriteRenderer>())sr.color*=new Color(.78f,.72f,.69f,1);return go;}
 static void Decorate(Transform art){
  // Reuse the established pixel-art stone/wood/root kit; no newly invented collision ledges.
  for(int i=0;i<9;i++){
   float x=1+i*6;
   Prop(art,"CliffCluster",x,-2.1f,new Vector2(2.3f,3.1f));
   Prop(art,"RockSpire",x+1.5f,13,new Vector2(1.3f,-1.5f));
   if(i%2==0)Prop(art,"HangingRoots",x+2,10.5f,Vector2.one);
  }
  foreach(float x in new[]{4f,24f,44f}){
   float y=x==24?1.2f:0;
   Prop(art,"AncientArch",x,y,new Vector2(1.2f,1.2f));
   Prop(art,"CarvedStone",x-1.3f,y,new Vector2(.45f,.45f),false);
   Lamp(art,new Vector2(x-1.3f,y+1.1f));
   Prop(art,"ForestMushrooms",x+1.3f,y,new Vector2(.6f,.6f),false);
  }
  Prop(art,"BracedTimber",18.5f,-2,new Vector2(.9f,1.5f));
  Prop(art,"TimberFrame",30.5f,-2,new Vector2(.9f,1.5f));
  Prop(art,"ShelfFungus",11.8f,1.2f,new Vector2(.65f,.65f),false);
  Prop(art,"MossyRiverRock",36.5f,.7f,new Vector2(.5f,.5f),false);
  Prop(art,"SmallFern",6.5f,0,new Vector2(.6f,.6f),false);
 }
 static void Lamp(Transform parent,Vector2 p){
  var root=Group(parent,"Warm refuge lantern");var white=PresentationBuilder.White();
  PresentationBuilder.Image(root,"Iron frame",white,p,new Vector2(.31f,.48f),6,new Color(.19f,.17f,.17f));
  PresentationBuilder.Image(root,"Warm glass",white,p,new Vector2(.18f,.29f),7,new Color(1,.79f,.43f));
  PresentationBuilder.Image(root,"Handle",white,p+new Vector2(0,.29f),new Vector2(.14f,.07f),6,new Color(.34f,.27f,.2f));
 }
 static void Validate(Transform root,ChapterTwoFlashlightPlaytest flow){
  if(flow.player==null||flow.visual==null||flow.view==null||flow.flashlight==null||!flow.flashlight.unlocked)throw new InvalidOperationException("Incomplete flashlight sandbox references.");
  if(root.GetComponentsInChildren<ChapterTwoObscurity>().Length!=2)throw new InvalidOperationException("Expected darkness and mist zones.");
  if(root.Find("Cavern scenery — no collision").GetComponentsInChildren<Collider2D>(true).Length!=0)throw new InvalidOperationException("Cavern decoration must not collide.");
  foreach(var t in root.GetComponentsInChildren<Transform>(true))foreach(var component in t.GetComponents<Component>())if(component==null)throw new InvalidOperationException("Missing script on "+t.name);
  foreach(var s in root.GetComponentsInChildren<SpriteRenderer>(true))if(s.sprite!=null&&(s.sharedMaterial==null||!s.sharedMaterial.shader.isSupported))throw new InvalidOperationException("Unsupported art on "+s.name);
 }
}
}
