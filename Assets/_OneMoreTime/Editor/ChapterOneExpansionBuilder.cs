using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object=UnityEngine.Object;

namespace OneMoreTime.EditorTools {
// Main chapter room numbers follow the campaign order. The acquisition room keeps its dedicated builder.
public static partial class ChapterOneExpansionBuilder {
 const string R="Assets/_OneMoreTime";
 const string SceneDir=R+"/Scenes/";
 public sealed class BuildContext {
  public Transform root,terrain,scenery;
  public MainRoom room;public RewindPlaytest replay;public ChapterOneRoom flow;
  public readonly List<RoomCheckpoint> checkpoints=new List<RoomCheckpoint>();
  public readonly List<RoomPlatform> movers=new List<RoomPlatform>();
  public readonly List<PressurePlate> plates=new List<PressurePlate>();
  public readonly List<LinkedDoor> doors=new List<LinkedDoor>();
  public readonly List<WaterHazard> waters=new List<WaterHazard>();
  public readonly List<WaterCurrent2D> currents=new List<WaterCurrent2D>();
  public readonly List<ChapterEraGroup> eras=new List<ChapterEraGroup>();
 }
 static Transform Group(Transform parent,string name){var g=new GameObject(name);if(parent!=null)g.transform.SetParent(parent,false);return g.transform;}
 static GameObject Prefab(Transform parent,string path,Vector2 position){
  var asset=AssetDatabase.LoadAssetAtPath<GameObject>(path);if(asset==null)throw new InvalidOperationException("Missing prefab "+path);
  var g=(GameObject)PrefabUtility.InstantiatePrefab(asset,parent.gameObject.scene);g.transform.SetParent(parent,false);g.transform.position=position;return g;
 }
 [MenuItem("Tools/One More Time/Build Chapter 1 Rooms 4–8")]
 public static void Build(){for(int n=4;n<=8;n++)BuildRoom(n);ConnectAcquisitionRoom();}

 public static string BuildRoom(int number){
  if(number<4||number>8)throw new ArgumentOutOfRangeException(nameof(number));
  if(EditorApplication.isPlaying)throw new InvalidOperationException("Exit Play mode first.");
  if(number==5){RoomFourBuilder.Build();return "Room 5: recording acquisition and waterway.";}
  string path=SceneDir+"Main_Room0"+number+".unity";
  var previous=SceneManager.GetActiveScene();var scene=SceneManager.GetSceneByPath(path);bool loaded=scene.IsValid()&&scene.isLoaded;
  if(loaded&&scene.isDirty)throw new InvalidOperationException("Save your room edits before rebuilding "+path);
  if(!loaded)scene=File.Exists(path)?EditorSceneManager.OpenScene(path,OpenSceneMode.Additive):EditorSceneManager.NewScene(NewSceneSetup.EmptyScene,NewSceneMode.Additive);
  SceneManager.SetActiveScene(scene);
  try {
   string managed="Room "+number+" Expansion Managed";
   var old=scene.GetRootGameObjects().FirstOrDefault(g=>g.name==managed||g.name.EndsWith(" Expansion Managed")&&g.GetComponent<ChapterOneRoom>()!=null);if(old!=null)Object.DestroyImmediate(old);
   var root=Group(null,managed);var c=new BuildContext{root=root,terrain=Group(root,"Route and mechanisms"),scenery=Group(root,"Forest and waterfall scenery")};
   c.room=root.gameObject.AddComponent<MainRoom>();c.room.AutoTick=false;c.room.enabled=false;c.room.showTips=false;c.room.movementTutorial=false;c.room.font=PresentationBuilder.Font;c.room.statueEyes=new SpriteRenderer[0];
   c.replay=root.gameObject.AddComponent<RewindPlaytest>();c.replay.AutoTick=false;c.replay.externalCamera=c.replay.externalHud=true;c.replay.font=c.room.font;c.replay.platforms=new RoomPlatform[0];c.replay.crates=new RewindCrate[0];
   c.replay.pauseTitle="Room "+number;c.replay.hudFooter="F: past / present   E: shutter   Hold Q: record   Tap Q: finish / replay   Down + Jump: drop";
   c.flow=root.gameObject.AddComponent<ChapterOneRoom>();c.flow.room=c.room;c.flow.replay=c.replay;c.flow.roomNumber=number;c.flow.nextRoomScene=number<8?"Main_Room0"+(number+1):"";c.flow.roomTitle=number==6?"The Broken Crossing":number==4?"Riverworks Ascent":number==7?"The Remembering River":"The Grand Waterfall";
   c.replay.participants=new RewindParticipant[]{c.flow};
   var player=Prefab(root,R+"/Prefabs/Player.prefab",Vector2.zero);c.room.player=c.replay.player=player.GetComponent<PlayerMotor2D>();c.room.visual=c.replay.visual=player.GetComponentInChildren<PlayerVisual>();c.room.player.tuning=AssetDatabase.LoadAssetAtPath<PlayerTuning>(R+"/Config/MainRoomPlayerTuning.asset");c.room.player.worldMask=1<<DemoBuilder.GetLayer();
   var echo=Prefab(root,R+"/Prefabs/FreezeEcho.prefab",Vector2.zero);c.replay.echo=echo.GetComponent<EchoSurface>();echo.name="Recorded goat";echo.SetActive(false);
   var shutter=root.gameObject.AddComponent<ShutterAbility>();c.room.shutter=shutter;shutter.room=c.room;shutter.unlockedOnEntry=true;
   var frozen=Prefab(root,R+"/Prefabs/FreezeEcho.prefab",Vector2.zero);shutter.echo=frozen.GetComponent<EchoSurface>();frozen.name="Shutter goat";frozen.SetActive(false);
   var trace=Group(root,"Recorded player trace");var line=trace.gameObject.AddComponent<LineRenderer>();line.sharedMaterial=AssetDatabase.LoadAssetAtPath<Material>(R+"/Generated/RewindTrace.mat");line.positionCount=0;line.useWorldSpace=true;RewindPlaytest.StyleTrace(line);c.replay.path=trace.gameObject.AddComponent<EchoPath>();
   var camera=Group(root,"Room Camera").gameObject.AddComponent<Camera>();camera.tag="MainCamera";camera.orthographic=true;camera.orthographicSize=5.625f;camera.backgroundColor=new Color(.13f,.25f,.28f);camera.clearFlags=CameraClearFlags.SolidColor;camera.gameObject.AddComponent<AudioListener>();c.room.view=c.replay.view=camera;
   var backdrop=Prefab(c.scenery,R+"/Prefabs/WorldOne/ForestWaterfallBackground.prefab",Vector2.zero).GetComponent<WorldOneBackdrop>();backdrop.view=camera;backdrop.eraSource=c.flow;PresentationBuilder.Audio(root.gameObject);
   switch(number){case 4:Layout4(c);break;case 6:Layout6(c);break;case 7:Layout7(c);break;case 8:Layout8(c);break;}
   c.room.deathY=c.room.roomBounds.yMin-1.5f;c.replay.deathY=c.room.deathY;c.replay.spawn=c.room.start;c.replay.bounds=c.room.roomBounds;
   player.transform.position=c.room.entrance;c.room.visual.Pose(c.room.entrance,1,0,0,Color.white);
   c.room.checkpoints=c.checkpoints.ToArray();c.room.checkpoint=c.checkpoints.Count>0?(Vector2)c.checkpoints[0].transform.position:c.room.start;c.room.waterHazards=c.waters.ToArray();c.room.nextRoomScene=c.flow.nextRoomScene;
   c.replay.platforms=c.movers.ToArray();c.flow.plates=c.plates.ToArray();c.flow.doors=c.doors.ToArray();c.flow.waters=c.waters.ToArray();c.flow.eraGroups=c.eras.ToArray();
   // Side bounds are physical; the upper bound constrains the camera only.
   Boundary(c,"Left boundary",c.room.roomBounds.xMin-.3f);Boundary(c,"Right boundary",c.room.roomBounds.xMax+.3f);
   camera.transform.position=new Vector3(c.room.roomBounds.xMin+10,Mathf.Clamp(c.room.start.y+1,c.room.roomBounds.yMin+5.625f,c.room.roomBounds.yMax-5.625f),-10);
   c.flow.ApplyEra();
   foreach(var f in root.GetComponentsInChildren<WaterFeature>())f.Refresh();
   // Official Rooms 5–6 own their drawing-specific bridge supports and bank clusters.
   // Official Rooms 5–8 own their drawing-specific scenery; no prototype decoration pass.
   ExitSignBuilder.Patch(scene,number);
   Validate(c);
   EditorSceneManager.MarkSceneDirty(scene);EditorSceneManager.SaveScene(scene,path);
   if(!EditorBuildSettings.scenes.Any(s=>s.path==path))EditorBuildSettings.scenes=EditorBuildSettings.scenes.Concat(new[]{new EditorBuildSettingsScene(path,true)}).ToArray();
   string result="Room "+number+": "+c.checkpoints.Count+" checkpoints, "+c.eras.Count+" era spans, "+c.movers.Count+" controlled platforms, "+c.replay.participants.OfType<CrateSpawner>().Count()+" floating sources.";
   Directory.CreateDirectory("Artifacts/Logs");File.AppendAllText("Artifacts/Logs/chapter1-expansion-generation.txt",result+"\n");return result;
  } finally {if(!loaded)EditorSceneManager.CloseScene(scene,true);if(previous.IsValid()&&previous.isLoaded)SceneManager.SetActiveScene(previous);}
 }
 public static void ConnectAcquisitionRoom(){
  var previous=SceneManager.GetActiveScene();string path=RoomFourBuilder.Path;var scene=SceneManager.GetSceneByPath(path);bool loaded=scene.IsValid()&&scene.isLoaded;
  if(loaded&&scene.isDirty)throw new InvalidOperationException("Save Room 5 before connecting its exit.");
  if(!loaded)scene=EditorSceneManager.OpenScene(path,OpenSceneMode.Additive);
  try {var flow=scene.GetRootGameObjects().SelectMany(g=>g.GetComponentsInChildren<Room4AbilityTutorial>()).Single();flow.nextRoomScene="Main_Room06";EditorUtility.SetDirty(flow);EditorSceneManager.MarkSceneDirty(scene);EditorSceneManager.SaveScene(scene);}
  finally {if(!loaded)EditorSceneManager.CloseScene(scene,true);if(previous.IsValid()&&previous.isLoaded)SceneManager.SetActiveScene(previous);}
 }
 static void Boundary(BuildContext c,string name,float x){var t=Group(c.terrain,name);t.position=new Vector2(x,c.room.roomBounds.center.y);t.gameObject.layer=DemoBuilder.GetLayer();t.gameObject.AddComponent<BoxCollider2D>().size=new Vector2(.4f,c.room.roomBounds.height+60);}
 static GameObject Solid(BuildContext c,string name,float left,float right,float top,float bottom){
  var t=Group(c.terrain,name);var rect=Rect.MinMaxRect(left,bottom,right,top);t.position=rect.center;t.gameObject.layer=DemoBuilder.GetLayer();t.gameObject.AddComponent<BoxCollider2D>().size=rect.size;PresentationBuilder.TerrainSkin(t.gameObject,rect,"Ground");return t.gameObject;
 }
 static GameObject Grass(BuildContext c,string name,float x,float top,float width,float depth=6){
  var g=Prefab(c.terrain,GrassyPlatformBuilder.PrefabPath,new Vector2(x,top));g.name=name;var column=g.GetComponent<GrassyColumnVisual>();column.width=width;column.depth=depth;column.resizeTopCollider=true;column.Refresh();return g;
 }
 static GameObject Wood(BuildContext c,string name,float left,float right,float top){
  var t=Group(c.terrain,name);t.position=new Vector2((left+right)/2,top-.09f);t.gameObject.layer=DemoBuilder.GetLayer();t.gameObject.AddComponent<BoxCollider2D>().size=new Vector2(right-left,.18f);t.gameObject.AddComponent<OneWayPlatform>();MainRoomArtBuilder.Wood(t,new Rect(left,top-.18f,right-left,.18f));return t.gameObject;
 }
 static GameObject Bridge(BuildContext c,string name,float left,float right,float top,int era=0){
  var t=Group(c.terrain,name);t.position=new Vector2((left+right)/2,top);t.gameObject.layer=DemoBuilder.GetLayer();var box=t.gameObject.AddComponent<BoxCollider2D>();box.size=new Vector2(right-left,.18f);box.offset=new Vector2(0,-.09f);t.gameObject.AddComponent<OneWayPlatform>();
  var art=Group(t,"Suspension bridge fragment");var sr=art.gameObject.AddComponent<SpriteRenderer>();sr.sprite=AssetDatabase.LoadAssetAtPath<Sprite>(R+"/Generated/WorldOne/BridgeMiddle.asset");sr.sharedMaterial=AssetDatabase.LoadAssetAtPath<Material>(R+"/Generated/WorldOne/WoodenSuspensionBridge.mat");sr.drawMode=SpriteDrawMode.Tiled;sr.size=new Vector2(right-left,sr.sprite.bounds.size.y);sr.sortingOrder=5;
  if(era!=0){var group=t.gameObject.AddComponent<ChapterEraGroup>();group.activeInPast=era==2;group.solids=new Collider2D[]{box};group.artwork=new[]{sr};group.CaptureAppearance();c.eras.Add(group);}
  return t.gameObject;
 }
 static WaterCurrent2D Water(BuildContext c,string name,float left,float right,float surface,float bottom){
  var t=Group(c.terrain,name);t.position=new Vector2(left,bottom);
  var current=t.gameObject.AddComponent<WaterCurrent2D>();current.left=left;current.right=right;current.surfaceY=surface;current.velocity=-2;c.currents.Add(current);
  var art=Prefab(t,WaterFeatureBuilder.Prefabs+"/ForestRapids.prefab",new Vector2(left,bottom));var feature=art.GetComponentInChildren<WaterFeature>();feature.shape=WaterFeature.Shape.Pool;feature.size=new Vector2(right-left,surface-bottom);feature.sortingOrder=-3;feature.Refresh();
  var volume=Group(t,"Deep water — invisible drown volume");volume.position=new Vector2((left+right)/2,(surface+bottom)/2-.2f);var box=volume.gameObject.AddComponent<BoxCollider2D>();box.isTrigger=true;box.size=new Vector2(right-left,Mathf.Max(.25f,surface-bottom-.4f));
  var hazard=volume.gameObject.AddComponent<WaterHazard>();hazard.drowningPrefab=AssetDatabase.LoadAssetAtPath<GameObject>(R+"/Prefabs/WorldOne/GoatDrowning.prefab").GetComponent<WorldOneSequence>();c.waters.Add(hazard);return current;
 }
 static GameObject Fall(BuildContext c,string name,float x,float bottom,float width,float height,bool foreground=false,bool background=false){
  var g=Prefab(c.scenery,WaterFeatureBuilder.Prefabs+"/"+(width>=8?"GrandWaterfall":"NarrowCascade")+".prefab",new Vector2(x,bottom));g.name=name;
  var features=g.GetComponentsInChildren<WaterFeature>();
  for(int i=0;i<features.Length;i++){var f=features[i];f.foreground=foreground;f.background=background;f.opacity=foreground?.25f:background?.7f:1;f.sortingOrder=foreground?22:background?-24:-4;if(f.shape==WaterFeature.Shape.Waterfall)f.size=new Vector2(width,height);else f.size=new Vector2(width+2,2);f.Refresh();}
  foreach(var sr in g.GetComponentsInChildren<SpriteRenderer>())if(sr.GetComponent<WaterFeature>()==null){sr.sortingOrder=foreground?23:background?-23:-3;sr.color=new Color(background?.5f:1,background?.5f:1,background?.5f:1,foreground?.25f:.65f);}
  return g;
 }
 static GameObject Prop(BuildContext c,string name,float x,float y,bool background=true){return Prefab(c.scenery,ForestExpansionBuilder.Prefabs+"/"+name+(background?"Background":"")+".prefab",new Vector2(x,y));}
 static RoomCheckpoint Checkpoint(BuildContext c,string name,float x,float top){var g=Prefab(c.terrain,R+"/Prefabs/WorldOne/StoneGoatCheckpoint.prefab",new Vector2(x,top));g.name=name;var cp=g.GetComponent<RoomCheckpoint>();c.checkpoints.Add(cp);return cp;}
 static PressurePlate Plate(BuildContext c,string name,float x,float top){var t=Group(c.terrain,name);t.position=new Vector2(x,top);var plate=t.gameObject.AddComponent<PressurePlate>();plate.graphic=PresentationBuilder.Image(t,"Brass tread",MainRoomArtBuilder.Art("PressurePlate"),new Vector2(x,top+.06f),new Vector2(1.5f,.25f),7,Color.white).GetComponent<SpriteRenderer>();c.plates.Add(plate);return plate;}
 static RoomPlatform Mover(BuildContext c,string name,float x,float top,float width,Vector2 travel,PressurePlate plate=null,float delay=0){
  var g=Wood(c,name,x-width/2,x+width/2,top);var mover=g.AddComponent<RoomPlatform>();mover.plate=plate;mover.solid=g.GetComponent<BoxCollider2D>();mover.artwork=g.GetComponentsInChildren<SpriteRenderer>();mover.travel=travel;mover.releaseDelay=delay;mover.speed=2;
  if(delay>0){var t=Group(g.transform,"Countdown");t.localPosition=new Vector3(0,-.12f,0);var label=t.gameObject.AddComponent<TextMesh>();label.font=PresentationBuilder.Font;label.fontSize=40;label.characterSize=.065f;label.anchor=TextAnchor.MiddleCenter;label.alignment=TextAlignment.Center;label.text="";t.GetComponent<MeshRenderer>().sharedMaterial=label.font.material;t.GetComponent<MeshRenderer>().sortingOrder=9;mover.counter=label;t.gameObject.SetActive(false);}
  c.movers.Add(mover);return mover;
 }
 static CrateSpawner Source(BuildContext c,string variant,float left,float right,float surface,float spawnX){
  var current=c.currents.LastOrDefault(w=>Mathf.Abs(w.left-left)<.01f&&Mathf.Abs(w.right-right)<.01f);
  if(current==null)throw new InvalidOperationException("Create the matching water channel before its floating source.");
  var source=FloatingTimberBuilder.InstallSource(c.terrain,variant,current,new Vector2(spawnX,surface+.28f),left-2,c.replay);
  source.name=variant+" — recorded stream source";source.spawnInterval=10;source.initialHorizontalVelocity=-2;return source;
 }
 static LinkedDoor Gate(BuildContext c,string name,float x,float floor,float height,PressurePlate plate){
  var t=Group(c.terrain,name);t.position=new Vector2(x,floor+height/2);t.gameObject.layer=DemoBuilder.GetLayer();var box=t.gameObject.AddComponent<BoxCollider2D>();box.size=new Vector2(.35f,height);
  var door=t.gameObject.AddComponent<LinkedDoor>();door.solid=box;door.plate=plate;door.graphic=PresentationBuilder.Image(t,"Old iron bars",MainRoomArtBuilder.Art("OldLiftGate"),t.position,new Vector2(.6f,height),6,Color.white).GetComponent<SpriteRenderer>();c.doors.Add(door);return door;
 }
 static Transform Upgrade(BuildContext c,float x,float y){
  var t=Group(c.terrain,"Reverse replay upgrade — Left Shift");t.position=new Vector2(x,y);
  var sprite=AssetDatabase.LoadAssetAtPath<Sprite>(R+"/Generated/Room4TraceRelic.asset");
  PresentationBuilder.Image(t,"Warm hourglass",sprite,t.position,new Vector2(.8f,.8f),8,new Color(1,.8f,.45f));
  return t;
 }
 static GameObject Cavern(BuildContext c,float x,float floor,float width,float height){
  var t=Group(c.scenery,"Chapter 2 cavern — beyond the waterfall");t.position=new Vector2(x,floor);
  PresentationBuilder.Image(t,"Deep unlit cavern",PresentationBuilder.White(),new Vector2(x,floor+height*.43f),new Vector2(width,height*.86f),1,new Color(.055f,.075f,.105f));
  PresentationBuilder.Image(t,"Cool light deeper inside",PresentationBuilder.White(),new Vector2(x+width*.22f,floor+height*.4f),new Vector2(width*.25f,height*.7f),2,new Color(.11f,.17f,.23f));
  foreach(float side in new[]{-1f,1f}){var rock=Prop(c,"RockSpire",x+side*width*.52f,floor,false);rock.transform.SetParent(t,true);foreach(var sr in rock.GetComponentsInChildren<SpriteRenderer>()){sr.color=new Color(.48f,.56f,.65f);sr.sortingOrder=4;}}
  var overhang=Prop(c,"RockOverhang",x,floor+height,false);overhang.transform.SetParent(t,true);overhang.transform.localScale=new Vector3(width/4,1.4f,1);foreach(var sr in overhang.GetComponentsInChildren<SpriteRenderer>()){sr.color=new Color(.48f,.56f,.65f);sr.sortingOrder=4;}
  return t.gameObject;
 }
 static void Validate(BuildContext c){
  if(c.room.player==null||c.room.view==null||c.replay.path==null||c.replay.echo==null||c.room.shutter==null)throw new InvalidOperationException("Missing core room references.");
  if((c.flow.roomNumber!=4&&c.room.checkpoints.Length==0)||c.flow.exitArea.width<=0)throw new InvalidOperationException("Missing checkpoint or exit.");
  foreach(var t in c.root.GetComponentsInChildren<Transform>(true))foreach(var component in t.GetComponents<Component>())if(component==null)throw new InvalidOperationException("Missing script on "+t.name);
  foreach(var sr in c.root.GetComponentsInChildren<SpriteRenderer>(true))if(sr.sprite!=null&&(sr.sharedMaterial==null||!sr.sharedMaterial.shader.isSupported))throw new InvalidOperationException("Unsupported art "+sr.name);
  if(c.scenery.GetComponentsInChildren<Collider2D>(true).Length!=0)throw new InvalidOperationException("Scenery must not add collision.");
 }
 public static string Capture(int number){
  if(EditorApplication.isPlaying)throw new InvalidOperationException("Exit Play mode first.");
  for(int i=0;i<SceneManager.sceneCount;i++)if(SceneManager.GetSceneAt(i).isDirty)throw new InvalidOperationException("Save scene edits before review capture.");
  var setup=EditorSceneManager.GetSceneManagerSetup();var scene=EditorSceneManager.OpenScene(SceneDir+"Main_Room0"+number+".unity");
  try {
   var flow=Object.FindAnyObjectByType<ChapterOneRoom>();var tutorial=Object.FindAnyObjectByType<Room4AbilityTutorial>();var room=flow!=null?flow.room:tutorial.room;var replay=flow!=null?flow.replay:tutorial.replay;var cam=room.view;var b=room.roomBounds;
   // Seed preview art only; scene pools start empty so the first live tick spawns exactly one log.
   foreach(var source in replay.participants.OfType<CrateSpawner>())source.Tick(0);
   Vector2[] positions=number==5?new[]{new Vector2(10,5),new Vector2(30,5),new Vector2(50,5),new Vector2(60,6)}:number==6?new[]{new Vector2(16,8),new Vector2(36,6),new Vector2(54,4),new Vector2(72,8),new Vector2(80,9)}:number==4?new[]{new Vector2(10,4),new Vector2(28,6),new Vector2(46,6),new Vector2(59,9),new Vector2(86,11)}:number==7?new[]{new Vector2(10,5),new Vector2(10,16),new Vector2(10,29),new Vector2(10,42),new Vector2(10,53),new Vector2(10,61)}:new[]{new Vector2(10,5),new Vector2(34,11),new Vector2(58,19),new Vector2(74,27),new Vector2(89,26),new Vector2(105,40),new Vector2(116,47)};
   Directory.CreateDirectory("Artifacts/Logs");
   for(int i=-1;i<positions.Length;i++){
    cam.aspect=i<0?(number==7?b.width/b.height:Mathf.Max(1,b.width/b.height)):16f/9;cam.orthographicSize=i<0?Mathf.Max(b.height/2,b.width/(2*cam.aspect)):5.625f;
    Vector2 p=i<0?b.center:positions[i];cam.transform.position=new Vector3(p.x,p.y,-10);
    foreach(var bg in Object.FindObjectsByType<WorldOneBackdrop>(FindObjectsSortMode.None))bg.SendMessage("LateUpdate");
    foreach(var feature in Object.FindObjectsByType<WaterFeature>(FindObjectsSortMode.None))feature.Refresh();
    int height=i<0&&number==7?2400:900;int width=i<0?Mathf.RoundToInt(height*cam.aspect):1600;var rt=new RenderTexture(width,height,24);var read=new Texture2D(width,height,TextureFormat.RGBA32,false);var prior=RenderTexture.active;cam.targetTexture=rt;
    try {cam.Render();RenderTexture.active=rt;read.ReadPixels(new Rect(0,0,width,height),0,0);read.Apply();File.WriteAllBytes("Artifacts/Logs/chapter1-room"+number+(i<0?"-overview":"-view"+(i+1))+".png",read.EncodeToPNG());}
    finally {cam.targetTexture=null;RenderTexture.active=prior;Object.DestroyImmediate(read);rt.Release();Object.DestroyImmediate(rt);}
   }
  } finally {EditorSceneManager.RestoreSceneManagerSetup(setup);}
  return "Captured Room "+number;
 }
}
}
