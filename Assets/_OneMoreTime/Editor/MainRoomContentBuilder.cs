using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
namespace OneMoreTime.EditorTools {
public static class MainRoomContentBuilder {
 const string R=DemoBuilder.Root;
 public const string Room3Path=R+"/Scenes/"+MainRoom.RoomThreeScene+".unity",LandingPath=R+"/Scenes/"+MainRoom.LiftLandingScene+".unity";
 static GameObject Solid(Transform parent,string name,Rect rect,int layer,bool wood=false){var g=new GameObject(name);g.transform.SetParent(parent);g.transform.position=rect.center;g.layer=layer;g.AddComponent<BoxCollider2D>().size=rect.size;if(wood){g.AddComponent<OneWayPlatform>();MainRoomArtBuilder.Wood(g.transform,rect);}else PresentationBuilder.TerrainSkin(g,rect,"Ground");return g;}
 static PressurePlate Plate(Transform parent,Vector2 p,string name){var g=new GameObject(name);g.transform.SetParent(parent);g.transform.position=p;var plate=g.AddComponent<PressurePlate>();plate.graphic=PresentationBuilder.Image(g.transform,"Brass tread",MainRoomArtBuilder.Art("PressurePlate"),p+Vector2.up*.06f,new Vector2(1.5f,.25f),7,Color.white).GetComponent<SpriteRenderer>();return plate;}
 static RoomLift Lift(Transform parent,Vector2 dock,int layer){
  var g=new GameObject("Old mountain lift");g.transform.SetParent(parent);var lift=g.AddComponent<RoomLift>();lift.dock=dock;
  lift.cabin=new GameObject("Moving cabin").transform;lift.cabin.SetParent(g.transform);lift.cabin.position=dock;
  PresentationBuilder.Image(lift.cabin,"Weathered iron and timber cabin",MainRoomArtBuilder.Art("OldLiftCabin"),dock+Vector2.up*1.5f,new Vector2(3,3.5f),2,Color.white);
  lift.gate=PresentationBuilder.Image(lift.cabin,"Sliding iron gate",MainRoomArtBuilder.Art("OldLiftGate"),dock+Vector2.up*1.3f,new Vector2(2,2.5f),8,Color.white).transform;
  var gate=new GameObject("Gate collision - left entrance");gate.transform.SetParent(lift.cabin);gate.layer=layer;gate.transform.position=dock+new Vector2(-1.05f,1.2f);lift.gateCollider=gate.AddComponent<BoxCollider2D>();lift.gateCollider.size=new Vector2(.18f,2.4f);
  for(int i=-1;i<=1;i+=2)PresentationBuilder.Image(g.transform,"Lift guide rail",PresentationBuilder.White(),dock+new Vector2(i*1.6f,5),new Vector2(.1f,13),-1,new Color(.24f,.32f,.34f));
  return lift;
 }
 [MenuItem("Tools/One More Time/Build Main Room Art and Room 3")]
 public static void Build(){
  if(EditorApplication.isPlaying)throw new System.InvalidOperationException("Stop Play before generating rooms");
  Directory.CreateDirectory("Artifacts/Logs");MainRoomArtBuilder.Generate();int layer=DemoBuilder.GetLayer();
  var holder=new GameObject("Reusable props");var lift=Lift(holder.transform,Vector2.zero,layer);MainRoomBuilder.ApplyMaterials(holder);PrefabUtility.SaveAsPrefabAsset(lift.gameObject,R+"/Prefabs/OldMountainLift.prefab");
  var wood=Solid(holder.transform,"WoodenPlatform",new Rect(-1,-.18f,2,.18f),layer,true);MainRoomBuilder.ApplyMaterials(wood);PrefabUtility.SaveAsPrefabAsset(wood,R+"/Prefabs/WoodenPlatform.prefab");Object.DestroyImmediate(holder);
  Refresh(MainRoomBuilder.Path);Refresh(RoomTwoBuilder.Path);
  BuildRoom(false);BuildRoom(true);
  Connect(RoomTwoBuilder.Path,MainRoom.RoomThreeScene);
  var paths=new[]{Room3Path,LandingPath};EditorBuildSettings.scenes=EditorBuildSettings.scenes.Where(s=>!paths.Contains(s.path)).Concat(paths.Select(p=>new EditorBuildSettingsScene(p,true))).ToArray();AssetDatabase.SaveAssets();
 }
 public static void RefreshPresentation(){MainRoomArtBuilder.Generate();Refresh(MainRoomBuilder.Path);Refresh(RoomTwoBuilder.Path);}
 [MenuItem("Tools/One More Time/Remove Main Room Ceilings")]
 public static void RemoveMainRoomCeilings(){
  if(EditorApplication.isPlaying)throw new System.InvalidOperationException("Exit Play first");
  foreach(string path in new[]{MainRoomBuilder.Path,RoomTwoBuilder.Path,Room3Path,LandingPath}){
   if(!File.Exists(path))continue;DemoBuilder.BackupScene(path);var previous=SceneManager.GetActiveScene();var scene=SceneManager.GetSceneByPath(path);bool loaded=scene.IsValid()&&scene.isLoaded;
   if(!loaded)scene=EditorSceneManager.OpenScene(path,OpenSceneMode.Additive);
   var room=scene.GetRootGameObjects().SelectMany(x=>x.GetComponentsInChildren<MainRoom>(true)).Single();
   foreach(string name in new[]{"Room ceiling","Ceiling"}){var ceiling=room.transform.Find(name);if(ceiling!=null)Object.DestroyImmediate(ceiling.gameObject);}
   EditorSceneManager.SaveScene(scene,path);if(!loaded)EditorSceneManager.CloseScene(scene,true);if(previous.IsValid()&&previous.isLoaded)SceneManager.SetActiveScene(previous);
  }
 }
 static void Refresh(string path){
  if(!File.Exists(path))return;DemoBuilder.BackupScene(path);var previous=SceneManager.GetActiveScene();var scene=SceneManager.GetSceneByPath(path);bool loaded=scene.IsValid()&&scene.isLoaded;if(!loaded)scene=EditorSceneManager.OpenScene(path,OpenSceneMode.Additive);
  var room=scene.GetRootGameObjects().SelectMany(x=>x.GetComponentsInChildren<MainRoom>(true)).Single();room.deathY=Mathf.Min(room.roomBounds.yMin-1.5f,-4.5f);MainRoomArtBuilder.Background(room.gameObject,room);
  foreach(var platform in room.GetComponentsInChildren<OneWayPlatform>()){var top=platform.transform.Find("Grass platform top");if(top!=null)Object.DestroyImmediate(top.gameObject);var grass=platform.transform.Find("Grass blades and roots");if(grass!=null)Object.DestroyImmediate(grass.gameObject);var box=platform.GetComponent<BoxCollider2D>();var p=(Vector2)platform.transform.position+box.offset;MainRoomArtBuilder.Grass(platform.transform,new Rect(p-box.size/2,box.size));}
  MainRoomBuilder.ApplyMaterials(room.gameObject);EditorSceneManager.SaveScene(scene,path);if(!loaded)EditorSceneManager.CloseScene(scene,true);if(previous.IsValid()&&previous.isLoaded)SceneManager.SetActiveScene(previous);
 }
 static void Connect(string path,string next){var previous=SceneManager.GetActiveScene();var scene=SceneManager.GetSceneByPath(path);bool loaded=scene.IsValid()&&scene.isLoaded;if(!loaded)scene=EditorSceneManager.OpenScene(path,OpenSceneMode.Additive);var room=scene.GetRootGameObjects().SelectMany(x=>x.GetComponentsInChildren<MainRoom>(true)).Single();room.nextRoomScene=next;EditorUtility.SetDirty(room);EditorSceneManager.SaveScene(scene,path);if(!loaded)EditorSceneManager.CloseScene(scene,true);if(previous.IsValid()&&previous.isLoaded)SceneManager.SetActiveScene(previous);}
 static void BuildRoom(bool arrival){
  string path=arrival?LandingPath:Room3Path;DemoBuilder.BackupScene(path);var previous=SceneManager.GetActiveScene();var scene=SceneManager.GetSceneByPath(path);bool loaded=scene.IsValid()&&scene.isLoaded;if(!loaded)scene=File.Exists(path)?EditorSceneManager.OpenScene(path,OpenSceneMode.Additive):EditorSceneManager.NewScene(NewSceneSetup.EmptyScene,NewSceneMode.Additive);SceneManager.SetActiveScene(scene);
  string rootName=arrival?"Lift landing Managed":"Room 3 Managed";var old=scene.GetRootGameObjects().FirstOrDefault(x=>x.name==rootName);if(old!=null)Object.DestroyImmediate(old);
  var root=new GameObject(rootName);var room=root.AddComponent<MainRoom>();room.font=PresentationBuilder.Font;room.movementTutorial=false;room.showTips=arrival;room.roomBounds=arrival?new Rect(0,-3,20,15):new Rect(0,-5,32,49);room.exitX=float.MaxValue;room.deathY=arrival?-4.5f:-6.5f;room.checkpoint=arrival?new Vector2(-100,-100):new Vector2(12,5);room.statueEyes=new SpriteRenderer[0];PresentationBuilder.Audio(root);
  var player=(GameObject)PrefabUtility.InstantiatePrefab(AssetDatabase.LoadAssetAtPath<GameObject>(R+"/Prefabs/Player.prefab"),scene);player.transform.SetParent(root.transform);room.player=player.GetComponent<PlayerMotor2D>();room.player.tuning=AssetDatabase.LoadAssetAtPath<PlayerTuning>(R+"/Config/MainRoomPlayerTuning.asset");room.visual=player.GetComponentInChildren<PlayerVisual>();int layer=DemoBuilder.GetLayer();room.player.worldMask=1<<layer;
  var camera=new GameObject("Room Camera").AddComponent<Camera>();camera.transform.SetParent(root.transform);camera.tag="MainCamera";camera.orthographic=true;camera.orthographicSize=10f/(960f/503f);camera.transform.position=new Vector3(10,3,-10);camera.clearFlags=CameraClearFlags.SolidColor;camera.backgroundColor=new Color(.35f,.6f,.65f);camera.gameObject.AddComponent<AudioListener>();room.view=camera;MainRoomArtBuilder.Background(root,room);
  var shutter=root.AddComponent<ShutterAbility>();shutter.room=room;shutter.unlockedOnEntry=true;room.shutter=shutter;var echo=(GameObject)PrefabUtility.InstantiatePrefab(AssetDatabase.LoadAssetAtPath<GameObject>(R+"/Prefabs/FreezeEcho.prefab"),scene);echo.transform.SetParent(root.transform);shutter.echo=echo.GetComponent<EchoSurface>();shutter.echo.support.enabled=false;echo.SetActive(false);
  if(arrival)Solid(root.transform,"Safe floor",new Rect(-3,-4,26,4),layer);
  else {Solid(root.transform,"Entrance ground",new Rect(-3,-9,22,9),layer);Solid(root.transform,"Tall right cliff",new Rect(24,-9,12,48),layer);}
  foreach(float x in new[]{-3.5f,arrival?20f:32.5f}){var wall=new GameObject("Room side wall");wall.transform.SetParent(root.transform);wall.layer=layer;wall.transform.position=new Vector2(x,18);wall.AddComponent<BoxCollider2D>().size=new Vector2(.5f,54);}
  if(arrival){room.entrance=room.start=new Vector2(16,0);room.lift=Lift(root.transform,new Vector2(16,0),layer);room.lift.room=room;room.lift.arrival=true;}
  else {
   Grass(root.transform,"Lower plate terrace",11,19,2.5f,layer);
   Grass(root.transform,"Left grass ledge",6,9,6,layer);
   Grass(root.transform,"First checkpoint grass",10,12,11,layer);
   var first=Checkpoint(root.transform,room,new Vector2(11.5f,11));
   Solid(root.transform,"Wood step at 14",new Rect(13,13.82f,2,.18f),layer,true);
   Solid(root.transform,"Right wood step at 16",new Rect(21,15.82f,3,.18f),layer,true);
   Solid(root.transform,"Short right step at 18.5",new Rect(23,18.32f,1,.18f),layer,true);
   Grass(root.transform,"Upper checkpoint grass",17,21,22,layer);
   var second=Checkpoint(root.transform,room,new Vector2(17.6f,22));room.checkpoints=new[]{first,second};room.statueEyes=new SpriteRenderer[0];
   var lowerPlate=Plate(root.transform,new Vector2(12.5f,2.5f),"Plate A - vertical shuttle");
   var mover=Controlled(root.transform,"Vertical shuttle",new Rect(15,4.82f,2,.18f),layer,lowerPlate,0,new Vector2(0,5));
   Track(root.transform,new Vector2(16,5),new Vector2(16,10));
   var timerPlate=Plate(root.transform,new Vector2(19.5f,22),"Plate B - three second platform");
   var upper=UpperRoute(root.transform,layer,timerPlate);
   var gatePlate=Plate(root.transform,new Vector2(25.5f,39),"Lift gate pressure plate");
   room.mechanisms=root.AddComponent<RoomMechanisms>();room.mechanisms.room=room;room.mechanisms.plates=new[]{lowerPlate,timerPlate,gatePlate};room.mechanisms.platforms=new[]{mover,upper[0],upper[1]};
   room.lift=Lift(root.transform,new Vector2(31,39),layer);room.lift.room=room;room.lift.plate=gatePlate;room.nextRoomScene=MainRoom.LiftLandingScene;
  }
  MainRoomBuilder.ApplyMaterials(root);EditorSceneManager.SaveScene(scene,path);Preview(room,arrival?"lift-landing":"room3",arrival?new Vector2(10,3):new Vector2(16,16.5f),arrival?6:22.5f);
  if(!loaded)EditorSceneManager.CloseScene(scene,true);if(previous.IsValid()&&previous.isLoaded)SceneManager.SetActiveScene(previous);
 }
 // One shared definition for new rooms and the scoped scene upgrade.
 static RoomPlatform[] UpperRoute(Transform parent,int layer,PressurePlate plate){
  var root=new GameObject("Revised upper route");root.transform.SetParent(parent);
  Grass(root.transform,"Far left upper grass",2,4,25,layer);
  Grass(root.transform,"Middle upper grass",9,11,25,layer);
  Solid(root.transform,"Left route wood step",new Rect(3,27.32f,2,.18f),layer,true);
  Grass(root.transform,"High left grass",5,8,31,layer);
  Grass(root.transform,"High middle grass",12,14,32,layer);
  var shortTimer=Controlled(root.transform,"Three second wood platform",new Rect(16,24.82f,2,.18f),layer,plate,3,Vector2.zero);
  var longTimer=Controlled(root.transform,"Twenty second wood platform",new Rect(18,34.82f,2,.18f),layer,plate,20,Vector2.zero);
  Solid(root.transform,"Upper right wood step",new Rect(23,35.82f,1,.18f),layer,true);
  return new[]{shortTimer,longTimer};
 }
 [MenuItem("Tools/One More Time/Update Room 3 Upper Section")]
 public static void UpdateRoomThreeUpperSection(){
  if(EditorApplication.isPlaying)throw new System.InvalidOperationException("Exit Play first");
  DemoBuilder.BackupScene(Room3Path);var previous=SceneManager.GetActiveScene();var scene=SceneManager.GetSceneByPath(Room3Path);bool loaded=scene.IsValid()&&scene.isLoaded;
  if(!loaded)scene=EditorSceneManager.OpenScene(Room3Path,OpenSceneMode.Additive);SceneManager.SetActiveScene(scene);
  var room=scene.GetRootGameObjects().SelectMany(x=>x.GetComponentsInChildren<MainRoom>(true)).Single();int layer=DemoBuilder.GetLayer();
  var timerPlate=room.mechanisms.platforms[1].plate;var mover=room.mechanisms.platforms[0];
  foreach(string name in new[]{"Revised upper route","Three second wood platform","Upper right wood step"}){var old=room.transform.Find(name);if(old!=null)Object.DestroyImmediate(old.gameObject);}
  var upper=UpperRoute(room.transform,layer,timerPlate);room.mechanisms.platforms=new[]{mover,upper[0],upper[1]};
  // Extend the cliff upwards while keeping its original collider and all lower objects intact.
  var cliff=room.transform.Find("Tall right cliff");
  foreach(Transform child in cliff){if(child.name=="Continuous grass edge"||child.name=="Hanging fern")child.position=new Vector3(child.position.x,child.name=="Continuous grass edge"?38.5f:38.78f,child.position.z);}
  var extension=room.transform.Find("Upper cliff extension");if(extension!=null)Object.DestroyImmediate(extension.gameObject);
  if(cliff.GetComponent<BoxCollider2D>().bounds.max.y<39){var g=new GameObject("Upper cliff extension");g.transform.SetParent(room.transform);g.layer=layer;g.transform.position=new Vector2(30,36);g.AddComponent<BoxCollider2D>().size=new Vector2(12,6);PresentationBuilder.TerrainSkin(g,new Rect(24,33,12,6),"Earth");}
  var lift=room.lift;var delta=new Vector2(31,39)-lift.dock;lift.transform.position+=(Vector3)delta;lift.dock=new Vector2(31,39);
  var gatePlate=lift.plate;gatePlate.transform.position=new Vector2(25.5f,39);
  room.roomBounds=new Rect(0,-5,32,49);EditorUtility.SetDirty(room);EditorUtility.SetDirty(room.mechanisms);EditorUtility.SetDirty(lift);
  MainRoomBuilder.ApplyMaterials(room.gameObject);EditorSceneManager.SaveScene(scene,Room3Path);
  if(!loaded)EditorSceneManager.CloseScene(scene,true);if(previous.IsValid()&&previous.isLoaded)SceneManager.SetActiveScene(previous);
 }
 public static void ApplyRoomThreePolish(){
  if(EditorApplication.isPlaying)throw new System.InvalidOperationException("Exit Play first");DemoBuilder.BackupScene(Room3Path);var previous=SceneManager.GetActiveScene();var scene=SceneManager.GetSceneByPath(Room3Path);bool loaded=scene.IsValid()&&scene.isLoaded;if(!loaded)scene=EditorSceneManager.OpenScene(Room3Path,OpenSceneMode.Additive);
  var room=scene.GetRootGameObjects().SelectMany(x=>x.GetComponentsInChildren<MainRoom>(true)).Single();room.showTips=false;
  foreach(var cable in room.GetComponentsInChildren<LineRenderer>(true).Where(x=>x.name=="Copper control cable").ToArray())Object.DestroyImmediate(cable.gameObject);
  var lift=room.lift;var delta=new Vector2(31,lift.dock.y)-lift.dock;lift.transform.position+=(Vector3)delta;lift.dock=new Vector2(31,lift.dock.y);EditorUtility.SetDirty(lift);
  foreach(var platform in room.mechanisms.platforms){if(platform.counter==null)continue;platform.counter.transform.position=platform.artwork[0].bounds.center;platform.counter.characterSize=.065f;platform.counter.text="";platform.counter.gameObject.SetActive(false);EditorUtility.SetDirty(platform.counter);}
  EditorUtility.SetDirty(room);EditorSceneManager.SaveScene(scene,Room3Path);if(!loaded)EditorSceneManager.CloseScene(scene,true);if(previous.IsValid()&&previous.isLoaded)SceneManager.SetActiveScene(previous);
 }
 public static void BuildRoomThreeLayout(){if(EditorApplication.isPlaying)throw new System.InvalidOperationException("Exit Play mode first");Directory.CreateDirectory("Artifacts/Logs");Refresh(MainRoomBuilder.Path);Refresh(RoomTwoBuilder.Path);BuildRoom(false);Connect(RoomTwoBuilder.Path,MainRoom.RoomThreeScene);AssetDatabase.SaveAssets();}
 static void Grass(Transform root,string name,float left,float right,float top,int layer){var g=new GameObject(name);g.transform.SetParent(root);g.transform.position=new Vector2((left+right)/2,top-.09f);g.layer=layer;g.AddComponent<BoxCollider2D>().size=new Vector2(right-left,.18f);g.AddComponent<OneWayPlatform>();MainRoomArtBuilder.Grass(g.transform,new Rect(left,top-.18f,right-left,.18f));var pillar=new GameObject("Dirt pillar - decoration only");pillar.transform.SetParent(root);var rect=new Rect(left,-5,right-left,top+5);pillar.transform.position=rect.center;PresentationBuilder.TerrainSkin(pillar,rect,"Earth");foreach(var sr in pillar.GetComponentsInChildren<SpriteRenderer>()){sr.sortingOrder=-5;sr.color=new Color(.55f,.65f,.6f);}}
 static RoomCheckpoint Checkpoint(Transform root,MainRoom room,Vector2 p){room.checkpoint=p;var g=MainRoomBuilder.Statue(root,room);var c=g.AddComponent<RoomCheckpoint>();c.eyes=room.statueEyes;return c;}
 static RoomPlatform Controlled(Transform root,string name,Rect rect,int layer,PressurePlate plate,float delay,Vector2 travel){var g=Solid(root,name,rect,layer,true);var p=g.AddComponent<RoomPlatform>();p.plate=plate;p.solid=g.GetComponent<BoxCollider2D>();p.artwork=g.GetComponentsInChildren<SpriteRenderer>();p.releaseDelay=delay;p.travel=travel;p.solid.enabled=false;foreach(var sr in p.artwork)sr.color=new Color(.55f,.7f,.7f,.22f);
  if(delay>0){var label=new GameObject("Countdown seconds");label.transform.SetParent(g.transform);label.transform.position=new Vector2(rect.center.x,rect.yMax-.25f);p.counter=label.AddComponent<TextMesh>();p.counter.font=PresentationBuilder.Font;p.counter.fontSize=64;p.counter.characterSize=.065f;p.counter.anchor=TextAnchor.MiddleCenter;p.counter.text="";label.SetActive(false);var mr=label.GetComponent<MeshRenderer>();mr.sharedMaterial=PresentationBuilder.Font.material;mr.sortingOrder=12;}
  return p;
 }
 static void Track(Transform root,Vector2 a,Vector2 b){PresentationBuilder.Image(root,"Pale yellow shuttle track",PresentationBuilder.White(),(a+b)/2,new Vector2(.035f,(b-a).magnitude),-2,new Color(.86f,.83f,.52f));}
 public static void Preview(MainRoom room,string name,Vector2 center,float size){var camera=room.view;var pos=camera.transform.position;float old=camera.orthographicSize;camera.transform.position=new Vector3(center.x,center.y,-10);camera.orthographicSize=size;int width=size>10?800:1200,height=size>10?1000:630;var target=new RenderTexture(width,height,24);camera.targetTexture=target;room.visual.Pose(room.start,1,0,0,Color.white);var b=room.GetComponentInChildren<MainRoomBackdrop>();b.SendMessage("LateUpdate");UnityEngine.Rendering.RenderPipeline.SubmitRenderRequest(camera,new UnityEngine.Rendering.RenderPipeline.StandardRequest{destination=target});var prior=RenderTexture.active;RenderTexture.active=target;var shot=new Texture2D(width,height,TextureFormat.RGB24,false);shot.ReadPixels(new Rect(0,0,width,height),0,0);shot.Apply();File.WriteAllBytes("Artifacts/Logs/"+name+"-preview.png",shot.EncodeToPNG());RenderTexture.active=prior;camera.targetTexture=null;Object.DestroyImmediate(shot);target.Release();Object.DestroyImmediate(target);camera.transform.position=pos;camera.orthographicSize=old;}
}
}
