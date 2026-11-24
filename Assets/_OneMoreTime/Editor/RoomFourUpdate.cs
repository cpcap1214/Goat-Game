using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using Object=UnityEngine.Object;
namespace OneMoreTime.EditorTools {
// Targeted, repeatable patch: preserves the existing scene root, objects and references.
public static class RoomFourUpdate {
 const string R=DemoBuilder.Root;
 static Transform Child(Transform parent,string name){var t=parent.Find(name);if(t!=null)return t;var g=new GameObject(name);g.transform.SetParent(parent,false);return g.transform;}
 static T Asset<T>(string name)where T:Object=>AssetDatabase.LoadAssetAtPath<T>(R+"/Generated/WorldOne/"+name);
 [MenuItem("Tools/One More Time/Update Existing Room 5")]
 public static void Apply(){
  if(EditorApplication.isPlaying)throw new InvalidOperationException("Exit Play mode before updating Room 5");
  var previous=SceneManager.GetActiveScene();var scene=SceneManager.GetSceneByPath(RoomFourBuilder.Path);bool loaded=scene.IsValid()&&scene.isLoaded;
  if(loaded&&scene.isDirty)throw new InvalidOperationException("Save Room 5 edits first");
  if(!loaded)scene=EditorSceneManager.OpenScene(RoomFourBuilder.Path,OpenSceneMode.Additive);
  Patch(scene);EditorSceneManager.SaveScene(scene);AssetDatabase.SaveAssets();
  if(!loaded)EditorSceneManager.CloseScene(scene,true);if(previous.IsValid()&&previous.isLoaded)SceneManager.SetActiveScene(previous);
 }
 public static void Patch(Scene scene){
  var root=scene.GetRootGameObjects().Single(g=>g.GetComponent<Room4AbilityTutorial>()!=null);var t=root.GetComponent<Room4AbilityTutorial>();var env=root.transform.Find("Environment");
  var soil=AssetDatabase.LoadAssetAtPath<Sprite>(R+"/Generated/EarthFillSeamless.asset");
  float bottom=t.room.roomBounds.yMin;
  ApplyLayout(env,t,bottom);
  UpdateEntrance(scene);
  var texture=AssetDatabase.LoadAssetAtPath<Texture2D>(R+"/Art/WorldOne/WaterAtlas.png");
  var sprite=Asset<Sprite>("FullBlockWater.asset");if(sprite==null){sprite=Sprite.Create(texture,new Rect(16,952,80,80),new Vector2(.5f,.5f),80,0,SpriteMeshType.FullRect);sprite.name="FullBlockWater";AssetDatabase.CreateAsset(sprite,R+"/Generated/WorldOne/FullBlockWater.asset");}
  if(sprite.rect.width!=80){var corrected=Sprite.Create(texture,new Rect(16,952,80,80),new Vector2(.5f,.5f),80,0,SpriteMeshType.FullRect);EditorUtility.CopySerialized(corrected,sprite);Object.DestroyImmediate(corrected);EditorUtility.SetDirty(sprite);}
  var tile=Asset<Tile>("FullBlockWaterTile.asset");if(tile==null){tile=ScriptableObject.CreateInstance<Tile>();tile.name="FullBlockWaterTile";AssetDatabase.CreateAsset(tile,R+"/Generated/WorldOne/FullBlockWaterTile.asset");}tile.sprite=sprite;tile.colliderType=Tile.ColliderType.None;tile.transform=Matrix4x4.identity;EditorUtility.SetDirty(tile);
  var material=Asset<Material>("FullBlockWater.mat");if(material==null){material=new Material(Shader.Find("OneMoreTime/Water Volume"));material.mainTexture=texture;AssetDatabase.CreateAsset(material,R+"/Generated/WorldOne/FullBlockWater.mat");}
  string[] names={"First water pit","Lower water route","Upper water channel"};float[] left={16,28,40},right={23,40,64},surface={0,2,4};
  for(int i=0;i<3;i++){
   var water=env.Find(names[i]);var edge=water.Find("Flowing surface");edge.position=new Vector3(left[i],surface[i]-.5f,0);
   var surfaceMap=edge.GetComponent<Tilemap>();surfaceMap.ClearAllTiles();for(int x=0;x<right[i]-left[i];x++)surfaceMap.SetTile(new Vector3Int(x,0,0),Asset<WaterAnimationTile>("RiverSurface.asset"));
   var fill=Child(water,"Full depth water fill");fill.position=new Vector3(left[i],bottom,0);fill.localScale=Vector3.one;
   var map=fill.GetComponent<Tilemap>();if(map==null)map=fill.gameObject.AddComponent<Tilemap>();var renderer=fill.GetComponent<TilemapRenderer>();if(renderer==null)renderer=fill.gameObject.AddComponent<TilemapRenderer>();renderer.sharedMaterial=material;renderer.sortingOrder=1;map.ClearAllTiles();
   for(int y=0;y<Mathf.CeilToInt(surface[i]-bottom);y++)for(int x=0;x<right[i]-left[i];x++)map.SetTile(new Vector3Int(x,y,0),tile);
   // Opaque volume overlaps the lower half of the surface, never the crest.
   fill.position=new Vector3(left[i],bottom-.45f,0);
   var hazard=water.Find("Deep water volume");var box=hazard.GetComponent<BoxCollider2D>();float low=bottom-3,high=surface[i]-.5f;hazard.position=new Vector3((left[i]+right[i])/2,(low+high)/2,0);box.size=new Vector2(right[i]-left[i],high-low);
  }
  var rescue=root.transform.Find("CrateSystem/First pit floating crate");if(rescue!=null)Object.DestroyImmediate(rescue.gameObject);
  t.crateSource=root.transform.Find("CrateSystem/Upper channel crate source").GetComponent<CrateSpawner>();t.crateSource.spawnInterval=10;t.crateSource.initialHorizontalVelocity=-2;t.crateSource.water.velocity=-2;t.crateSource.water.right=65;t.crateSource.spawnPoint.position=new Vector3(63.5f,4.8f,0);t.crateSource.spawningEnabled=false;t.replay.participants=new RewindParticipant[]{t.crateSource};foreach(var crate in t.crateSource.pool)crate.gameObject.SetActive(false);
  var checkpoint=env.Find("Room 4 stone goat checkpoint");if(checkpoint==null){var g=(GameObject)PrefabUtility.InstantiatePrefab(AssetDatabase.LoadAssetAtPath<GameObject>(R+"/Prefabs/WorldOne/StoneGoatCheckpoint.prefab"),scene);g.name="Room 4 stone goat checkpoint";g.transform.SetParent(env,false);checkpoint=g.transform;}
  checkpoint.position=new Vector3(26,4,0);t.room.checkpoints=new[]{checkpoint.GetComponent<RoomCheckpoint>()};t.room.checkpoint=new Vector2(26,4);
  t.holdDistance=.08f;t.holdTolerance=.015f;t.slowMotionStartDistance=3.6f;t.slowMotionDuration=.5f;t.minimumSpeed=.12f;t.resumeDuration=.45f;
  UpdateCrateDrain(scene);
  EditorUtility.SetDirty(t);EditorUtility.SetDirty(t.room);EditorUtility.SetDirty(t.replay);EditorSceneManager.MarkSceneDirty(scene);
 }
 // Water-only correction for the current acquisition room; does not repaint or
 // regenerate terrain, the pickup sequence, bridge, or checkpoint.
 public static void UpdateCrateDrain(Scene scene){
  var t=scene.GetRootGameObjects().SelectMany(g=>g.GetComponentsInChildren<Room4AbilityTutorial>(true)).Single();
  var source=t.crateSource;var drain=source.disposal;
  drain.sinkBeforeDisposal=true;drain.sinkDepth=2.5f;drain.sinkSpeed=3.5f;
  foreach(var old in t.GetComponentsInChildren<Transform>(true).Where(x=>x.name=="SurfaceEddy").ToArray())
   Object.DestroyImmediate(old.gameObject);
  var env=t.transform.Find("Environment");
  var swirl=env.Find("Crate drain swirl");
  if(swirl==null){
   var prefab=AssetDatabase.LoadAssetAtPath<GameObject>(WaterFeatureBuilder.Prefabs+"/SurfaceEddy.prefab");
   swirl=((GameObject)PrefabUtility.InstantiatePrefab(prefab,scene)).transform;
   swirl.SetParent(env,false);swirl.name="Crate drain swirl";
  }
  swirl.position=new Vector2(drain.boundaryX,source.water.surfaceY-.38f);
  swirl.localScale=new Vector3(1.2f,.6f,1);swirl.GetComponent<SpriteRenderer>().sortingOrder=7;
  PrefabUtility.RecordPrefabInstancePropertyModifications(drain);
  EditorUtility.SetDirty(drain);EditorSceneManager.MarkSceneDirty(scene);
 }
 public static void ApplyAndCaptureReview(){Apply();CaptureReview();}
 // Entrance-only migration for the reordered acquisition room. Keep its authored
 // water, checkpoint, pickup, bridge and cutscene references intact.
 public static void UpdateEntrance(Scene scene){
  var t=scene.GetRootGameObjects().SelectMany(g=>g.GetComponentsInChildren<Room4AbilityTutorial>(true)).Single();
  var root=t.transform;root.name="Room 5 Managed";var env=root.Find("Environment");
  if(t.room.lift!=null){Object.DestroyImmediate(t.room.lift.gameObject);t.room.lift=null;}
  var oldDeck=env.Find("Lift landing wood platform");if(oldDeck!=null)Object.DestroyImmediate(oldDeck.gameObject);
  var approach=env.Find("Lift landing and approach");
  float bottom=t.room.roomBounds.yMin;
  ClearArt(approach);approach.position=new Vector2(7,(bottom+4)/2);approach.localScale=Vector3.one;
  approach.GetComponent<BoxCollider2D>().size=new Vector2(18,4-bottom);
  Decoration(approach,"Entrance ground",new Rect(-2,bottom,18,4-bottom),true);
  t.room.entrance=new Vector2(.5f,4);t.room.start=new Vector2(2,4);t.replay.spawn=t.room.start;
  t.room.player.transform.position=t.room.entrance;t.room.visual.Pose(t.room.entrance,1,0,0,Color.white);
  t.replay.pauseTitle="Room 5";t.nextRoomScene=t.room.nextRoomScene="Main_Room06";
  t.room.view.name="Room 5 Camera";
  EditorUtility.SetDirty(t);EditorUtility.SetDirty(t.room);EditorUtility.SetDirty(t.replay);EditorSceneManager.MarkSceneDirty(scene);
 }
 public static void CaptureReview(){
  var scene=EditorSceneManager.OpenScene(RoomFourBuilder.Path,OpenSceneMode.Single);
  var t=scene.GetRootGameObjects().SelectMany(g=>g.GetComponentsInChildren<Room4AbilityTutorial>()).Single();
  var camera=t.room.view;camera.transform.position=new Vector3(34,4,-10);camera.orthographicSize=10;camera.aspect=3.4f;camera.rect=new Rect(0,0,1,1);
  var background=t.GetComponentInChildren<WorldOneBackdrop>();background.SendMessage("LateUpdate");
  var rt=new RenderTexture(2040,600,24);camera.targetTexture=rt;camera.Render();RenderTexture.active=rt;
  var image=new Texture2D(2040,600,TextureFormat.RGB24,false);image.ReadPixels(new Rect(0,0,2040,600),0,0);image.Apply();System.IO.Directory.CreateDirectory("Artifacts/Logs");System.IO.File.WriteAllBytes("Artifacts/Logs/room4-update-overview.png",image.EncodeToPNG());
  camera.targetTexture=null;RenderTexture.active=null;Object.DestroyImmediate(image);Object.DestroyImmediate(rt);
 }
 static void ClearArt(Transform t){foreach(Transform child in t.Cast<Transform>().ToArray())Object.DestroyImmediate(child.gameObject);var sr=t.GetComponent<SpriteRenderer>();if(sr!=null)sr.enabled=false;}
 static Transform Decoration(Transform parent,string name,Rect rect,bool grass){
  var t=Child(parent,name);t.position=rect.center;t.localScale=Vector3.one;PresentationBuilder.TerrainSkin(t.gameObject,rect,grass?"Ground":"Earth");
  // Keep the tile skin, not the builder's unrelated decorative props.
  foreach(Transform c in t.Cast<Transform>().Where(c=>c.name=="Hanging fern").ToArray())Object.DestroyImmediate(c.gameObject);
  return t;
 }
 static Sprite CornerSprite(bool grass){
  string name=grass?"GrassTerrainCorner.asset":"DirtTerrainCorner.asset";var sprite=Asset<Sprite>(name);if(sprite!=null)return sprite;
  var source=AssetDatabase.LoadAssetAtPath<Sprite>(R+"/Generated/"+(grass?"GrassEdgeSeamless":"EarthFillSeamless")+".asset");
  sprite=Sprite.Create(source.texture,source.rect,new Vector2(1,1),16,0,SpriteMeshType.FullRect);sprite.name=grass?"Grass terrain corner":"Pure dirt terrain corner";
  sprite.OverrideGeometry(new[]{new Vector2(-1,0),Vector2.zero,new Vector2(-1,-1)},new ushort[]{0,1,2});
  AssetDatabase.CreateAsset(sprite,R+"/Generated/WorldOne/"+name);return sprite;
 }
 static void Bank(Transform bank,Vector2 origin,Vector2[] polygon,Rect body,Rect? cap,bool cornerGrass,bool mirror){
  ClearArt(bank);bank.position=Vector3.zero;bank.localScale=Vector3.one;var box=bank.GetComponent<BoxCollider2D>();box.enabled=false;
  var poly=bank.GetComponent<PolygonCollider2D>();if(poly==null)poly=bank.gameObject.AddComponent<PolygonCollider2D>();poly.points=polygon;
  Decoration(bank,"Old terrain body",body,true);if(cap.HasValue)Decoration(bank,"Upper bank cap",cap.Value,true);
  var corner=Child(bank,cornerGrass?"Grass terrain bevel":"Pure dirt terrain bevel");corner.position=origin;var sr=corner.gameObject.AddComponent<SpriteRenderer>();sr.sprite=CornerSprite(cornerGrass);sr.flipX=mirror;sr.sortingOrder=4;sr.color=cornerGrass?Color.white:new Color(.92f,.85f,.80f);
 }
 static void ApplyLayout(Transform env,Room4AbilityTutorial t,float bottom){
  // Only the two diagonal terrain corners drawn by the user. No extra stepping ledges.
  foreach(var sr in env.GetComponentsInChildren<SpriteRenderer>(true).Where(s=>s.sprite==Asset<Sprite>("WallGrassLedge.asset")||s.sprite==Asset<Sprite>("GrassDirtLedge.asset")).ToArray()){
   float x=sr.transform.position.x;if(x>30&&x<50)continue;Object.DestroyImmediate(sr.gameObject);
  }
  var approach=env.Find("Lift landing and approach");ClearArt(approach);approach.position=new Vector2(9.5f,(bottom+4)/2);approach.localScale=Vector3.one;approach.GetComponent<BoxCollider2D>().size=new Vector2(13,4-bottom);
  Decoration(approach,"Entrance ground",new Rect(3,bottom,13,4-bottom),true);
  var landing=Child(env,"Lift landing wood platform");ClearArt(landing);landing.position=new Vector2(1.75f,3.91f);landing.gameObject.layer=DemoBuilder.GetLayer();var landingBox=landing.GetComponent<BoxCollider2D>();if(landingBox==null)landingBox=landing.gameObject.AddComponent<BoxCollider2D>();landingBox.size=new Vector2(2.5f,.18f);if(landing.GetComponent<OneWayPlatform>()==null)landing.gameObject.AddComponent<OneWayPlatform>();MainRoomArtBuilder.Wood(landing,new Rect(.5f,3.82f,2.5f,.18f));
  Bank(env.Find("First pit right bank"),new Vector2(29,3),new[]{new Vector2(23,bottom),new Vector2(28,bottom),new Vector2(28,2),new Vector2(29,3),new Vector2(29,4),new Vector2(23,4)},new Rect(23,bottom,5,4-bottom),new Rect(28,3,1,1),false,false);
  Bank(env.Find("Far right exit bank"),new Vector2(63,5),new[]{new Vector2(64,bottom),new Vector2(70,bottom),new Vector2(70,5),new Vector2(63,5),new Vector2(64,4)},new Rect(64,bottom,6,5-bottom),null,true,true);
  var wood=env.Find("Upper wood traversal platform");ClearArt(wood);wood.position=new Vector2(41,7.41f);wood.GetComponent<BoxCollider2D>().size=new Vector2(3,.18f);MainRoomArtBuilder.Wood(wood,new Rect(39.5f,7.32f,3,.18f));
  // Native Sunny Land grass rims over rectangular, visual-only soil columns.
  var grass=env.GetComponentsInChildren<SpriteRenderer>(true).Where(s=>s.sprite==Asset<Sprite>("GrassDirtLedge.asset")).Select(s=>s.transform).ToArray();
  for(int i=0;i<3;i++){
   string name="Main-style grass platform "+(i+1);var g=env.Find(name);if(g==null)g=grass.OrderBy(x=>x.position.x).ElementAtOrDefault(i);if(g==null)throw new InvalidOperationException("Missing Room 4 grass platform "+i);
   float[] xs={32,36,46},ys={6,5,5.6f},widths={3,1.5f,4};ClearArt(g);g.name=name;g.position=new Vector2(xs[i],ys[i]-.09f);g.localScale=Vector3.one;var box=g.GetComponent<BoxCollider2D>();box.enabled=true;box.offset=Vector2.zero;box.size=new Vector2(widths[i],.18f);
   GrassyPlatformBuilder.Attach(g,new Rect(xs[i]-widths[i]/2,ys[i]-.18f,widths[i],.18f),bottom);
  }
  t.room.roomBounds=new Rect(0,bottom,70,t.room.roomBounds.height);t.replay.bounds=t.room.roomBounds;t.exitX=68;
 }
}
}
