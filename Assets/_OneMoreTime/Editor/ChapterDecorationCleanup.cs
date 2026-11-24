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
/// <summary>Replaces only the chapter builders' loose forest props, without rebuilding a level.</summary>
public static class ChapterDecorationCleanup {
 public const string ManagedName = "Grounded chapter decoration";
 const string SceneFolder = "Assets/_OneMoreTime/Scenes/";
 const string Evidence = "Artifacts/Logs/chapter-decoration-cleanup.txt";

 struct Placement {
  public string cluster,asset; public Vector2 position; public float scale;
  public Placement(string cluster,string asset,float x,float y,float scale=.65f) {
   this.cluster=cluster;this.asset=asset;position=new Vector2(x,y);this.scale=scale;
  }
 }
 static Placement P(string cluster,string asset,float x,float y,float scale=.65f)=>new Placement(cluster,asset,x,y,scale);

 [MenuItem("Tools/One More Time/Tidy Chapter 1 Decoration")]
 public static void Build() { for(int number=1;number<=8;number++)Apply(number); }

 // Read-only scope review. No object creation/deletion, Undo operations, scene dirtying
 // or saves; unopened rooms are closed again after enumerating their authored state.
 public static string Audit(int number) {
  if(number<1||number>8)throw new ArgumentOutOfRangeException(nameof(number));
  if(EditorApplication.isPlaying)throw new InvalidOperationException("Audit decoration in Edit mode.");
  var previous=SceneManager.GetActiveScene();
  string path=SceneFolder+"Main_Room0"+number+".unity";
  var scene=SceneManager.GetSceneByPath(path);bool loaded=scene.IsValid()&&scene.isLoaded;
  if(!loaded)scene=EditorSceneManager.OpenScene(path,OpenSceneMode.Additive);
  try {
   var room=scene.GetRootGameObjects().SelectMany(g=>g.GetComponentsInChildren<MainRoom>(true)).Single();
   var scenery=room.transform.Find((number<=3||number==5)?MainRoomDecorationBuilder.ManagedName:"Forest and waterfall scenery");
   var old=scenery==null?new Transform[0]:scenery.Cast<Transform>().Where(IsLooseForestProp).ToArray();
   var original=room.transform.Find(ManagedName);
   var solids=Ground(room);var clearings=Clearings(room);
   var supported=new List<string>();var omitted=new List<string>();
   foreach(var p in Layout(number)) {
    var asset=AssetDatabase.LoadAssetAtPath<GameObject>(ForestExpansionBuilder.Prefabs+"/"+p.asset+".prefab");
    var sr=asset==null?null:asset.GetComponent<SpriteRenderer>();
    if(sr==null||sr.sprite==null){omitted.Add(p.asset+": missing root sprite");continue;}
    // Expansion props have a root renderer and their bottom-center pivot is authored
    // into the sprite. Evaluate the intended world bounds without instantiating it.
    var local=sr.sprite.bounds;
    var bounds=new Bounds((Vector3)p.position+local.center*p.scale,local.size*p.scale);
    string issue=PlacementProblem(bounds,p.position.y,solids,clearings);
    string description=p.cluster+" / "+p.asset+" at "+p.position+" scale "+p.scale;
    if(issue==null)supported.Add(description);else omitted.Add(description+": "+issue);
   }
   return "READ-ONLY Room "+number+": "+old.Length+" loose forest props would be replaced; "+
    (original==null?0:original.GetComponentsInChildren<SpriteRenderer>(true).Length)+" existing managed props; "+
    supported.Count+" of "+Layout(number).Length+" proposed grounded placements pass current geometry checks.\n"+
    "Replacement candidates: "+string.Join("; ",old.Select(t=>t.name+" at "+(Vector2)t.position))+"\n"+
    "Supported placements: "+string.Join("; ",supported)+"\n"+
    "Omitted placements: "+(omitted.Count==0?"none":string.Join("; ",omitted))+"\n"+
    "Water, cavern, terrain, mechanisms and custom objects remain outside the proposed change.";
  } finally {
   if(!loaded)EditorSceneManager.CloseScene(scene,true);
   if(previous.IsValid()&&previous.isLoaded)SceneManager.SetActiveScene(previous);
  }
 }

 public static string Apply(int number) {
  if(number<1||number>8)throw new ArgumentOutOfRangeException(nameof(number));
  if(EditorApplication.isPlaying)throw new InvalidOperationException("Exit Play mode before editing scenery.");
  var previous=SceneManager.GetActiveScene();
  var scene=SceneManager.GetSceneByPath(SceneFolder+"Main_Room0"+number+".unity");
  bool loaded=scene.IsValid()&&scene.isLoaded;
  if(loaded&&scene.isDirty)throw new InvalidOperationException("Save the room's current edits before tidying its decoration.");
  if(!loaded)scene=EditorSceneManager.OpenScene(SceneFolder+"Main_Room0"+number+".unity",OpenSceneMode.Additive);
  try {
   var room=scene.GetRootGameObjects().SelectMany(g=>g.GetComponentsInChildren<MainRoom>(true)).Single();
   string report=ApplyToRoom(room,number);
   EditorSceneManager.MarkSceneDirty(scene);EditorSceneManager.SaveScene(scene);
   Directory.CreateDirectory("Artifacts/Logs");File.AppendAllText(Evidence,report+"\n");
   return report;
  } finally {
   if(!loaded)EditorSceneManager.CloseScene(scene,true);
   if(previous.IsValid()&&previous.isLoaded)SceneManager.SetActiveScene(previous);
  }
 }

 // Generation hook: call after route, checkpoints and props have been created, before saving.
 // It also accepts an existing room and does not write it to disk itself.
 public static string ApplyToRoom(MainRoom room,int number) {
  if(room==null)throw new ArgumentNullException(nameof(room));
  if(number<1||number>8)throw new ArgumentOutOfRangeException(nameof(number));
  if(EditorApplication.isPlaying)throw new InvalidOperationException("Decoration cleanup is an Edit mode operation.");
  var original=room.transform.Find(ManagedName);
  var scenery=room.transform.Find((number<=3||number==5)?MainRoomDecorationBuilder.ManagedName:"Forest and waterfall scenery");
  // Only direct expansion-prefab instances belong to the old scatter pass. Nested cavern
  // scenery, all water artwork and any custom or gameplay objects are deliberately retained.
  var old=scenery==null?new Transform[0]:scenery.Cast<Transform>().Where(IsLooseForestProp).ToArray();
  var excluded=new HashSet<Transform>(old);if(original!=null)excluded.Add(original);
  var before=Snapshot(room.gameObject.scene,excluded);
  var root=new GameObject(ManagedName+" (staging)").transform;
  SceneManager.MoveGameObjectToScene(root.gameObject,room.gameObject.scene);root.SetParent(room.transform,false);
  excluded.Add(root);
  var omitted=new List<string>();
  int replacementUndo=-1;
  try {
   Physics2D.SyncTransforms();
   var solids=Ground(room);
   var clearings=Clearings(room);
   foreach(var p in Layout(number)) {
    var asset=AssetDatabase.LoadAssetAtPath<GameObject>(ForestExpansionBuilder.Prefabs+"/"+p.asset+".prefab");
    if(asset==null)throw new InvalidOperationException("Missing scenery prefab: "+p.asset);
    var prop=(GameObject)PrefabUtility.InstantiatePrefab(asset,room.gameObject.scene);
    prop.transform.SetParent(root,false);prop.transform.position=p.position;prop.transform.localScale=Vector3.one*p.scale;
    foreach(var sprite in prop.GetComponentsInChildren<SpriteRenderer>(true)) {
     sprite.sortingOrder=5;sprite.color=new Color(.83f,.9f,.82f,1);
    }
    string problem=PlacementProblem(prop,solids,clearings);
    if(problem!=null) {omitted.Add(p.asset+" at "+p.position+": "+problem);Object.DestroyImmediate(prop);continue;}
    var cluster=root.Find(p.cluster);
    if(cluster==null){cluster=new GameObject(p.cluster).transform;cluster.SetParent(root,false);}
    prop.transform.SetParent(cluster,true);
   }
   if(root.childCount==0)throw new InvalidOperationException("No supported decoration locations remain; the existing scenery was preserved.");
   ValidateRoot(root,solids,clearings);
   AssertUnchanged(before,Snapshot(room.gameObject.scene,excluded));
   int props=root.GetComponentsInChildren<SpriteRenderer>().Length;
   root.name=ManagedName;
   // Commit only after all placement checks pass. Keep deletion in its own Undo
   // group so a final preservation failure restores the actual original objects
   // and their references, rather than leaving a partially edited loaded scene.
   Undo.IncrementCurrentGroup();replacementUndo=Undo.GetCurrentGroup();
   Undo.SetCurrentGroupName("Tidy chapter decoration");
   Undo.RegisterCreatedObjectUndo(root.gameObject,"Create grounded decoration");
   foreach(var loose in old)Undo.DestroyObjectImmediate(loose.gameObject);
   if(original!=null)Undo.DestroyObjectImmediate(original.gameObject);
   AssertUnchanged(before,Snapshot(room.gameObject.scene,excluded));
   Undo.CollapseUndoOperations(replacementUndo);Undo.IncrementCurrentGroup();
   return "PASS Room "+number+": replaced "+old.Length+" loose forest props with "+props+" supported props in "+root.childCount+
    " clusters; checkpoint/plate/mover clearances checked; "+before.Count+" existing records unchanged; no added collision."+
    (omitted.Count==0?string.Empty:" Omitted incompatible placements: "+string.Join("; ",omitted));
  } catch {
   if(replacementUndo>=0){Undo.RevertAllDownToGroup(replacementUndo);Undo.IncrementCurrentGroup();}
   if(root!=null)Object.DestroyImmediate(root.gameObject);
   throw;
  }
 }

 static bool IsLooseForestProp(Transform t) {
  string path=PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(t.gameObject);
  if(!path.StartsWith(ForestExpansionBuilder.Prefabs+"/",StringComparison.Ordinal))return false;
  return t.GetComponentsInChildren<Component>(true).All(c=>c is Transform||c is SpriteRenderer);
 }
 static Collider2D[] Ground(MainRoom room)=>room.GetComponentsInChildren<Collider2D>(true).Where(c=>
  c.enabled&&c.gameObject.activeInHierarchy&&!c.isTrigger&&
  (room.player==null||(room.player.worldMask.value&(1<<c.gameObject.layer))!=0)&&
  c.GetComponentInParent<PlayerMotor2D>()==null&&c.GetComponentInParent<EchoSurface>()==null&&
  c.GetComponentInParent<RoomPlatform>()==null&&c.GetComponentInParent<RoomLift>()==null&&
  c.GetComponentInParent<LinkedDoor>()==null&&c.GetComponentInParent<BreakableBridge>()==null&&
  c.GetComponentInParent<ChapterCrumbleBridge>()==null&&c.GetComponentInParent<RewindCrate>()==null&&
  c.GetComponentInParent<FloatingObject2D>()==null).ToArray();

 static List<Rect> Clearings(MainRoom room) {
  var result=new List<Rect>();
  Action<Vector2,float,float> reserve=(p,halfWidth,height)=>result.Add(new Rect(p.x-halfWidth,p.y-.1f,halfWidth*2,height));
  reserve(room.start,1.35f,2.2f);
  // Older rooms can have a single legacy statue without a RoomCheckpoint component.
  reserve(room.checkpoint,1.35f,2.2f);
  foreach(var cp in room.GetComponentsInChildren<RoomCheckpoint>(true))reserve(cp.transform.position,1.35f,2.2f);
  foreach(var plate in room.GetComponentsInChildren<PressurePlate>(true))reserve(plate.transform.position,1.2f,1.8f);
  foreach(var pickup in room.GetComponentsInChildren<AbilityPickup>(true))reserve(pickup.transform.position,1.2f,2.2f);
  if(room.shutter!=null&&room.shutter.pickup!=null)reserve(room.shutter.pickup.position,1.4f,2.2f);
  foreach(var lift in room.GetComponentsInChildren<RoomLift>(true))reserve(lift.dock,2.35f,4);
  foreach(var mover in room.GetComponentsInChildren<RoomPlatform>(true)) {
   if(mover.solid==null)continue;
   // Disabled plate platforms report an empty physics bounds in Edit mode.
   // Their authored box still reserves the complete travel corridor.
   var box=mover.solid;var scale=box.transform.lossyScale;
   var b=new Bounds(box.transform.TransformPoint(box.offset),new Vector3(Mathf.Abs(box.size.x*scale.x),Mathf.Abs(box.size.y*scale.y),0));
   var end=b.center+(Vector3)mover.travel;
   result.Add(Rect.MinMaxRect(Mathf.Min(b.min.x,end.x-b.extents.x)-.3f,Mathf.Min(b.min.y,end.y-b.extents.y)-.2f,
    Mathf.Max(b.max.x,end.x+b.extents.x)+.3f,Mathf.Max(b.max.y,end.y+b.extents.y)+1.5f));
  }
  var tutorial=room.GetComponent<Room4AbilityTutorial>();
  if(tutorial!=null&&tutorial.pickup!=null)reserve(tutorial.pickup.position,1.4f,2.2f);
  var flow=room.GetComponent<ChapterOneRoom>();
  if(flow!=null)result.Add(flow.exitArea);
  else if(room.exitX<10000)reserve(new Vector2(room.exitX,room.exitFloor),1,2.5f);
  return result;
 }

 static string PlacementProblem(GameObject prop,Collider2D[] ground,List<Rect> clearings) {
  var renderers=prop.GetComponentsInChildren<SpriteRenderer>(true);
  if(renderers.Length==0)return "missing sprite";
  var bounds=renderers[0].bounds;foreach(var sr in renderers)bounds.Encapsulate(sr.bounds);
  return PlacementProblem(bounds,prop.transform.position.y,ground,clearings);
 }
 static string PlacementProblem(Bounds bounds,float floor,Collider2D[] ground,List<Rect> clearings) {
  if(bounds.size.x>3.05f||bounds.size.y>3.2f)return "oversized silhouette";
  // Require the entire visible footprint to rest on an actual static collision surface.
  // Sampling just below and above the baseline also handles the existing polygon banks.
  for(int i=0;i<5;i++) {
   float x=Mathf.Lerp(bounds.min.x+.03f,bounds.max.x-.03f,i/4f);
   if(!ground.Any(c=>c.OverlapPoint(new Vector2(x,floor-.025f))&&!c.OverlapPoint(new Vector2(x,floor+.025f))))
    return "no continuous ground support";
  }
  var footprint=Rect.MinMaxRect(bounds.min.x-.12f,floor,bounds.max.x+.12f,bounds.max.y);
  if(clearings.Any(r=>r.Overlaps(footprint)))return "checkpoint, interaction or moving-route clearing";
  return null;
 }

 static void ValidateRoot(Transform root,Collider2D[] solids,List<Rect> clearings) {
  if(root.GetComponentsInChildren<Component>(true).Any(c=>!(c is Transform)&&!(c is SpriteRenderer)))
   throw new InvalidOperationException("Scenery must contain only transforms and sprite renderers.");
  foreach(Transform cluster in root)foreach(Transform prop in cluster) {
   string issue=PlacementProblem(prop.gameObject,solids,clearings);
   if(issue!=null)throw new InvalidOperationException(prop.name+": "+issue);
   foreach(var sr in prop.GetComponentsInChildren<SpriteRenderer>())
    if(sr.sprite==null||sr.sharedMaterial==null||!sr.sharedMaterial.shader.isSupported||sr.sortingOrder>=10)
     throw new InvalidOperationException("Invalid or obscuring scenery: "+prop.name);
  }
 }

 public static string ValidateRoom(MainRoom room) {
  var root=room.transform.Find(ManagedName);
  if(root==null)throw new InvalidOperationException("The room has not received the decoration cleanup.");
  Physics2D.SyncTransforms();ValidateRoot(root,Ground(room),Clearings(room));
  return "PASS "+room.gameObject.scene.name+": all "+root.GetComponentsInChildren<SpriteRenderer>().Length+" props are supported, behind the player and clear of interactions.";
 }

 static Dictionary<string,string> Snapshot(Scene scene,HashSet<Transform> excluded) {
  var records=new Dictionary<string,string>();
  foreach(var root in scene.GetRootGameObjects())foreach(var t in root.GetComponentsInChildren<Transform>(true)) {
   if(excluded.Any(e=>e!=null&&(t==e||t.IsChildOf(e))))continue;
   records[t.GetEntityId()+"/transform"]=t.localPosition.ToString("R")+t.localRotation.ToString("R")+t.localScale.ToString("R")+t.name+t.gameObject.activeSelf+t.gameObject.layer;
   foreach(var c in t.GetComponents<Component>()) {
    if(c==null)throw new InvalidOperationException("Missing script on "+t.name);
    if(!(c is Transform))records[c.GetEntityId()+"/component"]=EditorJsonUtility.ToJson(c);
   }
  }
  return records;
 }
 static void AssertUnchanged(Dictionary<string,string> before,Dictionary<string,string> after) {
  if(before.Count!=after.Count||before.Any(p=>!after.TryGetValue(p.Key,out var value)||p.Value!=value))
   throw new InvalidOperationException("Decoration changed an object outside its own forest props. Scene was not saved.");
 }

 // Small groups belong to dry ground: fungus beside fallen wood, ferns on damp banks,
 // and sparse shrubs on exposed ridges. Narrow bridge decks and mechanism rests stay empty.
 static Placement[] Layout(int room) {
  switch(room) {
   case 1:return new[]{
    P("Sheltered trail","HollowStump",8.8f,0,.6f),P("Sheltered trail","ForestMushrooms",9.8f,0,.6f),
    P("Forest edge","WindsweptPine",14.5f,0,.55f),P("Forest edge","SmallFern",15.9f,0,.7f),
    P("Far-bank grove","WindsweptPine",46.5f,2.8f,.55f),P("Far-bank grove","SmallFern",48,2.8f,.7f),
    P("High trail","MossyRiverRock",68.5f,12.4f,.55f),P("High trail","AlpineShrub",69.6f,12.4f,.6f)};
   case 2:return new[]{
    P("Forest approach","HollowStump",5.2f,0,.6f),P("Forest approach","ForestMushrooms",6.2f,0,.6f),
    P("Ruins edge","MossyRiverRock",15.2f,0,.6f),P("Ruins edge","SmallFern",16.3f,0,.65f),
    P("Far-bank grove","WindsweptPine",40.3f,5,.55f),P("Far-bank grove","SmallFern",41.7f,5,.7f)};
   case 3:return new[]{
    P("Forest floor","HollowStump",6,0,.6f),P("Forest floor","ForestMushrooms",7,0,.6f),
    P("Lower ledge","SmallFern",6.8f,6,.5f),P("Lower ledge","ForestMushrooms",7.8f,6,.45f),
    P("Exposed upper ledge","AlpineShrub",6,31,.5f),P("Exposed upper ledge","MeadowFlowers",7.2f,31,.55f),
    P("Lift approach","AlpineShrub",27.8f,39,.5f)};
   case 5:return new[]{
    P("Lift-side grove","HollowStump",8,4,.6f),P("Lift-side grove","ForestMushrooms",9,4,.6f),
    P("Dry approach","SmallFern",11.5f,4,.65f),P("Dry approach","MeadowFlowers",12.5f,4,.55f),
    P("River ledge","SmallFern",32.6f,6,.45f),P("Upper bank","MeadowFlowers",46.5f,5.6f,.55f),
    P("Exit riverbank","MossyRiverRock",65.3f,5,.55f),P("Exit riverbank","RiverReeds",66.5f,5,.6f)};
   case 6:return new[]{
    P("West bridge bank","SmallFern",6,9,.55f),P("West bridge bank","ForestMushrooms",6.9f,9,.45f),
    P("East riverbank","MossyRiverRock",62.7f,10,.55f),P("East riverbank","RiverReeds",63.9f,10,.6f),
    P("East riverbank","ForestMushrooms",64.9f,10,.45f)};
   case 4:return new[]{
    P("Damp entrance","BroadFern",7.4f,1,.55f),P("Damp entrance","RiverReeds",8.7f,1,.55f),
    P("Moss shelf","SmallFern",18.3f,8.1f,.5f),P("Moss shelf","ForestMushrooms",19.2f,8.1f,.45f),
    P("Middle rest edge","SmallFern",16.8f,20.9f,.45f),
    P("Upper rest edge","AlpineShrub",16.5f,30.5f,.5f),
    P("Headwater grove","WindsweptPine",24.4f,40,.5f),P("Headwater grove","MeadowFlowers",25.8f,40,.55f)};
   case 7:return new[]{
    P("Arrival bank edge","RiverReeds",.9f,3,.5f),
    P("First island bank","SmallFern",27.4f,3,.4f),
    P("Second island bank","SmallFern",47.4f,3,.4f),
    P("Far-bank driftwood","MossyFallenLog",69,3,.55f),P("Far-bank driftwood","ForestMushrooms",70.2f,3,.5f),
    P("Far-bank driftwood","AlpineShrub",71.2f,3,.55f)};
   case 8:return new[]{
    P("Valley edge","SmallFern",.9f,3,.5f),
    P("Moss ledge","MeadowFlowers",43.5f,25.5f,.55f),
    P("High riverbank","RiverReeds",55.2f,29.5f,.55f),
    P("Far riverbank","AlpineShrub",76,30.5f,.5f),
    P("Alpine ridge","AlpineShrub",88,37,.5f),
    P("Stone approach","MossyRiverRock",96,41,.5f)};
   default:throw new ArgumentOutOfRangeException(nameof(room));
  }
 }
}
}
