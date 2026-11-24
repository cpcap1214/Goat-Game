using System;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.SceneManagement;
using UnityEditor;
using UnityEditor.SceneManagement;
using Object=UnityEngine.Object;
namespace OneMoreTime.EditorTools {
public static class RoomFourBuilder {
 const string R=DemoBuilder.Root;
 public const string Path=R+"/Scenes/"+Room4AbilityTutorial.SceneName+".unity";
 static Transform Group(Transform parent,string name){var g=new GameObject(name);g.transform.SetParent(parent,false);return g.transform;}
 static GameObject Prefab(Transform parent,string path,Vector2 p){var g=(GameObject)PrefabUtility.InstantiatePrefab(AssetDatabase.LoadAssetAtPath<GameObject>(R+"/Prefabs/"+path+".prefab"));g.transform.SetParent(parent,false);g.transform.position=p;return g;}
 static GameObject Land(Transform parent,string name,Rect rect,int layer,bool wood=false){var g=Group(parent,name).gameObject;g.layer=layer;g.transform.position=rect.center;g.AddComponent<BoxCollider2D>().size=rect.size;if(wood){g.AddComponent<OneWayPlatform>();MainRoomArtBuilder.Wood(g.transform,rect);}else PresentationBuilder.TerrainSkin(g,rect,"Ground");return g;}
 static WaterHazard Water(Transform parent,string name,float left,float right,float surface){
  var g=Group(parent,name).gameObject;var grid=g.AddComponent<Grid>();var art=Group(g.transform,"Flowing surface");var map=art.gameObject.AddComponent<Tilemap>();var mr=art.gameObject.AddComponent<TilemapRenderer>();mr.sortingOrder=1;mr.sharedMaterial=AssetDatabase.LoadAssetAtPath<Material>(R+"/Generated/WorldOne/WaterAtlas.mat");
  art.position=new Vector3(left,surface-.5f,0);var tile=AssetDatabase.LoadAssetAtPath<WaterAnimationTile>(R+"/Generated/WorldOne/RiverSurface.asset");for(int x=0;x<Mathf.CeilToInt(right-left);x++)map.SetTile(new Vector3Int(x,0,0),tile);
  var hazard=Group(g.transform,"Deep water volume");hazard.position=new Vector3((left+right)/2,surface-2.4f,0);var box=hazard.gameObject.AddComponent<BoxCollider2D>();box.isTrigger=true;box.size=new Vector2(right-left,4);
  var water=hazard.gameObject.AddComponent<WaterHazard>();water.drowningPrefab=AssetDatabase.LoadAssetAtPath<GameObject>(R+"/Prefabs/WorldOne/GoatDrowning.prefab").GetComponent<WorldOneSequence>();return water;
 }
 static CrateSpawner Spawner(Transform parent,string name,float left,float right,float surface,int count,float speed){
  var g=Group(parent,name).gameObject;var sp=g.AddComponent<CrateSpawner>();sp.maximumActiveCrates=count;sp.initialHorizontalVelocity=speed;
  var water=g.AddComponent<WaterCurrent2D>();water.left=left;water.right=right+1;water.surfaceY=surface;water.velocity=speed;sp.water=water;
  sp.spawnPoint=Group(g.transform,"Crate spawn point");sp.spawnPoint.position=new Vector3(right,surface+.8f,0);
  var disposal=Group(g.transform,"Crate-only disposal boundary");disposal.position=new Vector3(left-.75f,surface,0);var dc=disposal.gameObject.AddComponent<BoxCollider2D>();dc.isTrigger=true;dc.size=new Vector2(1,8);
  sp.disposal=disposal.gameObject.AddComponent<CrateDisposalTrigger>();sp.disposal.owner=sp;sp.disposal.boundaryX=left;
  sp.pool=new FloatingObject2D[count];for(int i=0;i<count;i++){
   var c=Group(g.transform,"Pooled floating crate "+(i+1)).gameObject;c.layer=DemoBuilder.GetLayer();var box=c.AddComponent<BoxCollider2D>();box.size=new Vector2(1.2f,1.2f);c.AddComponent<Rigidbody2D>().bodyType=RigidbodyType2D.Kinematic;c.AddComponent<OneWayPlatform>();
   var f=c.AddComponent<FloatingObject2D>();f.water=water;sp.pool[i]=f;
   PresentationBuilder.Image(c.transform,"Sunny Land crate",DemoBuilder.Sprite("Environment/props/crate.png"),Vector2.zero,new Vector2(1.2f,1.2f),6,Color.white);c.SetActive(false);
  }
  return sp;
 }
 [MenuItem("Tools/One More Time/Build Main Room 5")]
 public static void Build(){
  if(EditorApplication.isPlaying)throw new InvalidOperationException("Exit Play mode first");
  var previous=SceneManager.GetActiveScene();var scene=SceneManager.GetSceneByPath(Path);bool loaded=scene.IsValid()&&scene.isLoaded;
  if(loaded&&scene.isDirty)throw new InvalidOperationException("Save Room 5 before rebuilding");
  if(!loaded)scene=File.Exists(Path)?EditorSceneManager.OpenScene(Path,OpenSceneMode.Additive):EditorSceneManager.NewScene(NewSceneSetup.EmptyScene,NewSceneMode.Additive);
  SceneManager.SetActiveScene(scene);var old=scene.GetRootGameObjects().FirstOrDefault(x=>x.name=="Room 5 Managed");if(old!=null)Object.DestroyImmediate(old);
  var root=new GameObject("Room 5 Managed");var room=root.AddComponent<MainRoom>();room.AutoTick=false;room.enabled=false;room.showTips=false;room.movementTutorial=false;room.font=PresentationBuilder.Font;room.entrance=room.start=new Vector2(2,4);room.statueEyes=new SpriteRenderer[0];room.roomBounds=new Rect(0,-3,68,18);room.exitX=float.MaxValue;
  var replay=root.AddComponent<RewindPlaytest>();replay.AutoTick=false;replay.externalCamera=replay.externalHud=true;replay.font=room.font;replay.pauseTitle="Room 5";replay.hudFooter="Ride the floating crates. Hold Q: record / replace · Tap Q: finish / replay · R: retry";replay.spawn=room.start;replay.platforms=new RoomPlatform[0];replay.crates=new RewindCrate[0];replay.bounds=room.roomBounds;
  var tutorial=root.AddComponent<Room4AbilityTutorial>();tutorial.room=room;tutorial.replay=replay;
  var environment=Group(root.transform,"Environment");var entrance=Group(root.transform,"Entrance");var ability=Group(root.transform,"AbilityTutorial");var crates=Group(root.transform,"CrateSystem");
  var player=Prefab(root.transform,"Player",room.start);room.player=replay.player=player.GetComponent<PlayerMotor2D>();room.player.tuning=AssetDatabase.LoadAssetAtPath<PlayerTuning>(R+"/Config/MainRoomPlayerTuning.asset");room.player.worldMask=1<<DemoBuilder.GetLayer();room.visual=replay.visual=player.GetComponentInChildren<PlayerVisual>();
  var echo=Prefab(root.transform,"FreezeEcho",Vector2.zero);replay.echo=echo.GetComponent<EchoSurface>();echo.SetActive(false);
  var shutter=root.AddComponent<ShutterAbility>();room.shutter=shutter;shutter.room=room;shutter.unlockedOnEntry=true;var frozen=Prefab(root.transform,"FreezeEcho",Vector2.zero);frozen.name="Shutter frozen goat";shutter.echo=frozen.GetComponent<EchoSurface>();frozen.SetActive(false);
  var trace=Group(root.transform,"Recorded player trace");var line=trace.gameObject.AddComponent<LineRenderer>();line.sharedMaterial=AssetDatabase.LoadAssetAtPath<Material>(R+"/Generated/RewindTrace.mat");line.positionCount=0;line.useWorldSpace=true;RewindPlaytest.StyleTrace(line);replay.path=trace.gameObject.AddComponent<EchoPath>();
  var cam=Group(root.transform,"Room 5 Camera").gameObject.AddComponent<Camera>();cam.tag="MainCamera";cam.orthographic=true;cam.orthographicSize=5.24f;cam.transform.position=new Vector3(10,5,-10);cam.gameObject.AddComponent<AudioListener>();room.view=replay.view=cam;
  var bg=Prefab(environment,"WorldOne/ForestWaterfallBackground",Vector2.zero).GetComponent<WorldOneBackdrop>();bg.view=cam;PresentationBuilder.Audio(root);
  int layer=DemoBuilder.GetLayer();
  Land(environment,"Lift landing and approach",new Rect(-2,0,18,4),layer);
  Land(environment,"First pit right bank",new Rect(23,0,6,4),layer);
  Land(environment,"Far right exit bank",new Rect(59,0,11,5),layer);
  var wall=Land(environment,"Left boundary",new Rect(-2.5f,-10,.5f,35),layer);foreach(var sr in wall.GetComponentsInChildren<SpriteRenderer>())Object.DestroyImmediate(sr);
  // No ceiling collider: same rule as Rooms 1–3.
  foreach(var spec in new[]{new Vector3(32,6,3),new Vector3(37,5,1.5f),new Vector3(47,5.6f,4)}){
   var ledge=Prefab(environment,"WorldOne/GrassDirtLedge",new Vector2(spec.x,spec.y));ledge.transform.localScale=new Vector3(spec.z/4,1,1);
  }
  Land(environment,"Upper wood traversal platform",new Rect(40,7.32f,3,.18f),layer,true);
  var exitLip=Prefab(environment,"WorldOne/GrassDirtLedge",new Vector2(58.5f,5));exitLip.transform.localScale=new Vector3(.625f,1,1);
  var pitLedge=Prefab(environment,"WorldOne/GrassDirtLedge",new Vector2(22.5f,2.8f));pitLedge.transform.localScale=new Vector3(.5f,1,1);
  // The diagonal lip is a real slope; the underside remains decorative.
  var slope=Prefab(environment,"WorldOne/GrassDirtLedge",new Vector2(28.8f,3));slope.GetComponent<BoxCollider2D>().enabled=false;Object.DestroyImmediate(slope.GetComponent<OneWayPlatform>());var poly=slope.AddComponent<PolygonCollider2D>();poly.points=new[]{new Vector2(-.8f,1),new Vector2(.8f,-.7f),new Vector2(-.8f,-.7f)};
  tutorial.waters=new[]{Water(environment,"First water pit",16,23,0),Water(environment,"Lower water route",29,40,2),Water(environment,"Upper water channel",40,59,4)};
  var rise=Prefab(environment,"WorldOne/Waterfall",new Vector2(39,1));rise.transform.localScale=new Vector3(.34f,.6f,1);
  var liftObject=Prefab(entrance,"OldMountainLift",new Vector2(2,4));room.lift=liftObject.GetComponent<RoomLift>();room.lift.room=room;room.lift.arrival=true;room.lift.dock=new Vector2(2,4);room.lift.travel=7;room.lift.speed=3;
  var bridgeRoot=Group(environment,"Breakable pickup bridge");bridgeRoot.position=new Vector3(19.5f,4,0);var bridge=bridgeRoot.gameObject.AddComponent<BreakableBridge>();tutorial.bridge=bridge;
  var intact=Prefab(bridgeRoot,"WorldOne/WoodenSuspensionBridge",new Vector2(19.5f,4));intact.transform.localScale=new Vector3(7f/8,1,1);bridge.intact=intact;bridge.deck=intact.GetComponent<BoxCollider2D>();
  bridge.pieces=new SpriteRenderer[4];for(int i=0;i<4;i++){
   var p=Group(bridgeRoot,"Falling timber segment "+(i+1));p.position=new Vector3(16.875f+i*1.75f,4,0);var sr=p.gameObject.AddComponent<SpriteRenderer>();sr.sprite=AssetDatabase.LoadAssetAtPath<Sprite>(R+"/Generated/WorldOne/BridgeMiddle.asset");sr.sharedMaterial=AssetDatabase.LoadAssetAtPath<Material>(R+"/Generated/WorldOne/WoodenSuspensionBridge.mat");sr.sortingOrder=5;p.localScale=new Vector3(.875f,1,1);bridge.pieces[i]=sr;p.gameObject.SetActive(false);
  }
  tutorial.pickup=PresentationBuilder.Image(ability,"Purple Record Replay pickup",PresentationBuilder.White(),new Vector2(19.5f,4.7f),new Vector2(.85f,.85f),10,new Color(.65f,.15f,.82f)).transform;
  var relic=AssetDatabase.LoadAssetAtPath<Texture2D>(R+"/Resources/Relics/TraceHourglass.png");string relicPath=R+"/Generated/Room4TraceRelic.asset";var relicSprite=AssetDatabase.LoadAssetAtPath<Sprite>(relicPath);if(relicSprite==null){relicSprite=Sprite.Create(relic,new Rect(0,0,relic.width,relic.height),new Vector2(.5f,.5f),relic.height);AssetDatabase.CreateAsset(relicSprite,relicPath);}
  PresentationBuilder.Image(tutorial.pickup,"Record hourglass",relicSprite,new Vector2(19.5f,4.7f),new Vector2(.65f,.65f),11,Color.white);
  var blocker=Group(ability,"Invisible anti-bypass wall");blocker.position=new Vector3(20.8f,13,0);blocker.gameObject.layer=layer;tutorial.abilityBlocker=blocker.gameObject.AddComponent<BoxCollider2D>();((BoxCollider2D)tutorial.abilityBlocker).size=new Vector2(.25f,28);
  var source=Spawner(crates,"Upper channel crate source",40,58,4,6,-2);
  var rescue=Spawner(crates,"First pit floating crate",17,19.5f,0,1,0);rescue.spawnInterval=999;
  replay.participants=new RewindParticipant[]{source,rescue};
  // Apply ordinary sprite materials without replacing the art pack's special shaders.
  foreach(var sr in root.GetComponentsInChildren<SpriteRenderer>(true))if(sr.sharedMaterial==null||sr.sharedMaterial.name=="Sprites-Default"){
   if(sr.sprite==null)continue;string asset=AssetDatabase.GetAssetPath(sr.sprite.texture);string path=R+"/Generated/Room4_"+AssetDatabase.AssetPathToGUID(asset)+".mat";var m=AssetDatabase.LoadAssetAtPath<Material>(path);if(m==null){m=new Material(Shader.Find("Sprites/Default"));m.mainTexture=sr.sprite.texture;AssetDatabase.CreateAsset(m,path);}sr.sharedMaterial=m;
  }
  PrefabUtility.SaveAsPrefabAsset(bridgeRoot.gameObject,R+"/Prefabs/Room4BreakableBridge.prefab");
  var reusable=Object.Instantiate(source.pool[0].gameObject);reusable.name="Floating Water Crate";reusable.transform.position=Vector3.zero;reusable.GetComponent<FloatingObject2D>().water=null;reusable.SetActive(true);PrefabUtility.SaveAsPrefabAsset(reusable,R+"/Prefabs/FloatingWaterCrate.prefab");Object.DestroyImmediate(reusable);
  RoomFourUpdate.Patch(scene);
  BridgeScaffoldBuilder.Patch(scene);
  ExitSignBuilder.Patch(scene,5);
  EditorSceneManager.SaveScene(scene,Path);if(!EditorBuildSettings.scenes.Any(s=>s.path==Path))EditorBuildSettings.scenes=EditorBuildSettings.scenes.Concat(new[]{new EditorBuildSettingsScene(Path,true)}).ToArray();
  if(!loaded)EditorSceneManager.CloseScene(scene,true);if(previous.IsValid()&&previous.isLoaded)SceneManager.SetActiveScene(previous);
  ConnectRoomThree();AssetDatabase.SaveAssets();
 }
 static void ConnectRoomThree(){var previous=SceneManager.GetActiveScene();string path=MainRoomContentBuilder.Room3Path;var s=SceneManager.GetSceneByPath(path);bool loaded=s.IsValid()&&s.isLoaded;if(loaded&&s.isDirty)throw new InvalidOperationException("Save Room 3 before connecting its exit");if(!loaded)s=EditorSceneManager.OpenScene(path,OpenSceneMode.Additive);var room=s.GetRootGameObjects().SelectMany(g=>g.GetComponentsInChildren<MainRoom>(true)).Single();room.nextRoomScene="Main_Room04";EditorUtility.SetDirty(room);EditorSceneManager.SaveScene(s,path);if(!loaded)EditorSceneManager.CloseScene(s,true);if(previous.IsValid()&&previous.isLoaded)SceneManager.SetActiveScene(previous);}
}
}
