using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace OneMoreTime.EditorTools {
public static class MainRoomBuilder {
 const string Root=DemoBuilder.Root;
 public const string Path=Root+"/Scenes/"+MainRoom.SceneName+".unity";
 static GameObject Image(Transform parent,string name,Vector2 p,Vector2 size,Color color,int order=5)=>PresentationBuilder.Image(parent,name,PresentationBuilder.White(),p,size,order,color);
 [MenuItem("Tools/One More Time/Build Main Room 1")]
 public static void Build(){
  if(EditorApplication.isPlaying)throw new System.InvalidOperationException("Stop Play before building Room 1");
  if(Application.isBatchMode&&string.IsNullOrEmpty(SceneManager.GetActiveScene().path))EditorSceneManager.OpenScene(Root+"/Scenes/MainMenu.unity");
  Directory.CreateDirectory("Artifacts/Logs");DemoBuilder.BackupScene(Path);
  var previous=SceneManager.GetActiveScene();var scene=SceneManager.GetSceneByPath(Path);bool loaded=scene.IsValid()&&scene.isLoaded;
  if(!loaded)scene=File.Exists(Path)?EditorSceneManager.OpenScene(Path,OpenSceneMode.Additive):EditorSceneManager.NewScene(NewSceneSetup.EmptyScene,NewSceneMode.Additive);
  SceneManager.SetActiveScene(scene);
  var old=scene.GetRootGameObjects().FirstOrDefault(x=>x.name=="Room 1 Managed");if(old!=null)Object.DestroyImmediate(old);
  var root=new GameObject("Room 1 Managed");var room=root.AddComponent<MainRoom>();room.deathY=-8.5f;room.font=PresentationBuilder.Font;room.nextRoomScene=MainRoom.RoomTwoScene;PresentationBuilder.Audio(root);
  var p=(GameObject)PrefabUtility.InstantiatePrefab(AssetDatabase.LoadAssetAtPath<GameObject>(Root+"/Prefabs/Player.prefab"),scene);p.transform.SetParent(root.transform);p.transform.position=room.entrance;room.player=p.GetComponent<PlayerMotor2D>();room.visual=p.GetComponentInChildren<PlayerVisual>();
  var mainTuning=DemoBuilder.Asset<PlayerTuning>("MainRoomPlayerTuning");EditorUtility.CopySerialized(room.player.tuning,mainTuning);
  // Semi-implicit 0.02-second simulation: 22 ascending ticks sum to 3 units.
  mainTuning.jumpSpeed=14.178182f;EditorUtility.SetDirty(mainTuning);room.player.tuning=mainTuning;
  var camera=new GameObject("Room Camera").AddComponent<Camera>();camera.transform.SetParent(root.transform);camera.tag="MainCamera";camera.orthographic=true;camera.orthographicSize=10f/(960f/503f);camera.transform.position=new Vector3(10,10f/(960f/503f)*(3.2f/6.1f),-10);camera.clearFlags=CameraClearFlags.SolidColor;camera.backgroundColor=new Color(.48f,.73f,.82f);camera.gameObject.AddComponent<AudioListener>();room.view=camera;
  var sky=AssetDatabase.LoadAssetAtPath<Sprite>(Root+"/Art/ChapterOne/PresentClearSky.png");
  if(sky!=null)room.sky=PresentationBuilder.Image(root.transform,"Open sky",sky,new Vector2(12,4),new Vector2(40,26),-40,Color.white).transform;
  int layer=DemoBuilder.GetLayer();room.player.worldMask=1<<layer;
  Land(root.transform,"Entrance ground",new Rect(-3,-10,28,10),layer);
  Land(root.transform,"Checkpoint ledge",new Rect(25,-10,9.8f,12.8f),layer);
  Land(root.transform,"Far bank",new Rect(39.2f,-10,25.6f,12.8f),layer);
  Land(root.transform,"Exit cliff",new Rect(64.8f,-10,15.6f,22.4f),layer);
  Platform(root.transform,"Lower grass platform",new Rect(58.6f,5.3f-.18f,3,.18f),layer);
  Platform(root.transform,"Middle grass platform",new Rect(54.8f,7.3f-.18f,2.8f,.18f),layer);
  Platform(root.transform,"Upper grass platform",new Rect(59.4f,9.8f-.18f,3.8f,.18f),layer);
  // Bounds exclude offscreen entry/exit corridors, which are used only by transition walks.
  room.entranceBarrier=Solid(root.transform,"Entrance closes after walk-in",new Rect(-.5f,-10,.5f,30),layer).GetComponent<BoxCollider2D>();room.entranceBarrier.enabled=false;
  Solid(root.transform,"Left room boundary",new Rect(-3.5f,-10,.5f,31),layer);
  Solid(root.transform,"Right room boundary",new Rect(80.4f,-10,.5f,31),layer);
  Statue(root.transform,room);
  var sign=DemoBuilder.Sprite("Environment/props/sign.png");if(sign!=null)PresentationBuilder.Image(root.transform,"Exit trail marker",sign,new Vector2(75,12.8f),new Vector2(.7f,.8f),4,Color.white);
  MainRoomArtBuilder.Background(root,room);
  ApplyMaterials(root);
  EditorSceneManager.SaveScene(scene,Path);
  var gameScenes=SceneFlow.Scenes.Concat(new[]{ChapterLayout.Scene,MainRoom.SceneName,MainRoom.RoomTwoScene,MainRoom.RoomThreeScene,MainRoom.LiftLandingScene}).Select(n=>Root+"/Scenes/"+n+".unity").ToArray();
  EditorBuildSettings.scenes=gameScenes.Select(s=>new EditorBuildSettingsScene(s,true)).Concat(EditorBuildSettings.scenes.Where(s=>!gameScenes.Contains(s.path))).ToArray();
  AssetDatabase.SaveAssets();
  // A single layout preview; all saved geometry is generated through Unity APIs.
  var target=new RenderTexture(960,503,24);camera.targetTexture=target;
  room.visual.Pose(room.start,1,0,0,Color.white);UnityEngine.Rendering.RenderPipeline.SubmitRenderRequest(camera,new UnityEngine.Rendering.RenderPipeline.StandardRequest{destination=target});
  var prior=RenderTexture.active;RenderTexture.active=target;var shot=new Texture2D(960,503,TextureFormat.RGB24,false);shot.ReadPixels(new Rect(0,0,960,503),0,0);shot.Apply();File.WriteAllBytes("Artifacts/Logs/room1-preview.png",shot.EncodeToPNG());
  RenderTexture.active=prior;camera.targetTexture=null;Object.DestroyImmediate(shot);target.Release();Object.DestroyImmediate(target);room.visual.transform.position=room.entrance;
  if(!loaded)EditorSceneManager.CloseScene(scene,true);if(previous.IsValid()&&previous.isLoaded)SceneManager.SetActiveScene(previous);
  File.WriteAllText("Artifacts/Logs/room1-generation.txt","Generated Main_Room01: stepped ground, pit, 3 thin grass platforms with non-colliding dirt pillars, goat checkpoint, entry/exit transitions.\n");
 }
 public static void ApplyMaterials(GameObject root){
  // Explicit unlit sprite materials keep this room independent of scene lighting.
  string materials=Root+"/Generated/Room1Materials";Directory.CreateDirectory(materials);AssetDatabase.Refresh();
  foreach(var sr in root.GetComponentsInChildren<SpriteRenderer>(true)){
   if(sr.sprite==null)continue;
   string source=AssetDatabase.GetAssetPath(sr.sprite.texture);string key=AssetDatabase.AssetPathToGUID(source);
   bool grayscale=sr.name=="Grayscale player statue";
   string materialPath=materials+"/Sprite_"+key+(grayscale?"_Grayscale":"")+".mat";
   var material=AssetDatabase.LoadAssetAtPath<Material>(materialPath);
   if(material==null){material=new Material(Shader.Find(grayscale?"OneMoreTime/Checkpoint Grayscale":"Sprites/Default"));material.mainTexture=sr.sprite.texture;AssetDatabase.CreateAsset(material,materialPath);}
   sr.sharedMaterial=material;
  }
 }
 static GameObject Solid(Transform root,string name,Rect rect,int layer){var g=new GameObject(name);g.transform.SetParent(root);g.transform.position=rect.center;g.layer=layer;g.AddComponent<BoxCollider2D>().size=rect.size;return g;}
 static void Land(Transform root,string name,Rect rect,int layer){var g=Solid(root,name,rect,layer);PresentationBuilder.TerrainSkin(g,rect,"Ground");}
 static void Platform(Transform root,string name,Rect rect,int layer){
  var g=Solid(root,name,rect,layer);
  g.AddComponent<OneWayPlatform>();
  var column=new GameObject("Dirt pillar - visual only");column.transform.SetParent(root);var r=new Rect(rect.xMin,-7,rect.width,rect.yMax+7);column.transform.position=r.center;
  PresentationBuilder.TerrainSkin(column,r,"Ground");
  foreach(var renderer in column.GetComponentsInChildren<SpriteRenderer>()){renderer.sortingOrder=-5;renderer.color=new Color(.65f,.73f,.65f);}
  MainRoomArtBuilder.Grass(g.transform,rect);
 }
 public static GameObject Statue(Transform root,MainRoom room){
  var statue=new GameObject("Goat statue checkpoint");statue.transform.SetParent(root);Vector2 p=room.checkpoint;statue.transform.position=p;
  var library=room.visual.library;var sprite=library.idle[0];
  var art=new GameObject("Grayscale player statue");art.transform.SetParent(statue.transform);
  art.transform.position=p+library.visualOffset;art.transform.localScale=Vector3.one*library.visualScale;
  var renderer=art.AddComponent<SpriteRenderer>();renderer.sprite=sprite;renderer.sortingOrder=5;
  // Idle frame 0's visible eye, in sprite pixels; retain the checkpoint indicator.
  Vector2 eyePixel=new Vector2(179,138);
  Vector2 eyePosition=p+library.visualOffset+(eyePixel-sprite.pivot)/sprite.pixelsPerUnit*library.visualScale;
  room.statueEyes=new[]{Image(statue.transform,"Checkpoint eye",eyePosition,new Vector2(8,10)/sprite.pixelsPerUnit*library.visualScale,new Color(.12f,.15f,.15f),8).GetComponent<SpriteRenderer>()};
  return statue;
 }
}
}
