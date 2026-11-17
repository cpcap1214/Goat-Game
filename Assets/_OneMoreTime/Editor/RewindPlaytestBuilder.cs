using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
namespace OneMoreTime.EditorTools {
public static class RewindPlaytestBuilder {
 const string Root=DemoBuilder.Root;public const string Path=Root+"/Scenes/Debug_Rewind.unity";
 static GameObject Block(Transform parent,string name,Rect rect,int layer,bool wood=false){
  var go=new GameObject(name);go.transform.SetParent(parent);go.transform.position=rect.center;go.layer=layer;go.AddComponent<BoxCollider2D>().size=rect.size;
  if(wood){go.AddComponent<OneWayPlatform>();MainRoomArtBuilder.Wood(go.transform,rect);}else PresentationBuilder.TerrainSkin(go,rect,"Ground");return go;
 }
 [MenuItem("Tools/One More Time/Build Rewind Playtest")]
 public static void Build(){
  if(EditorApplication.isPlaying)throw new System.InvalidOperationException("Exit Play first");
  if(Application.isBatchMode&&string.IsNullOrEmpty(SceneManager.GetActiveScene().path))EditorSceneManager.OpenScene(Root+"/Scenes/MainMenu.unity");
  var previous=SceneManager.GetActiveScene();if(File.Exists(Path))DemoBuilder.BackupScene(Path);
  var scene=SceneManager.GetSceneByPath(Path);bool loaded=scene.IsValid()&&scene.isLoaded;
  if(!loaded)scene=File.Exists(Path)?EditorSceneManager.OpenScene(Path,OpenSceneMode.Additive):EditorSceneManager.NewScene(NewSceneSetup.EmptyScene,NewSceneMode.Additive);
  SceneManager.SetActiveScene(scene);var old=scene.GetRootGameObjects().FirstOrDefault(x=>x.name=="Rewind playtest Managed");if(old!=null)Object.DestroyImmediate(old);
  var root=new GameObject("Rewind playtest Managed");var test=root.AddComponent<RewindPlaytest>();test.font=PresentationBuilder.Font;int layer=DemoBuilder.GetLayer();
  var goat=(GameObject)PrefabUtility.InstantiatePrefab(AssetDatabase.LoadAssetAtPath<GameObject>(Root+"/Prefabs/Player.prefab"),scene);goat.transform.SetParent(root.transform);goat.transform.position=test.spawn;test.player=goat.GetComponent<PlayerMotor2D>();test.visual=goat.GetComponentInChildren<PlayerVisual>();test.player.tuning=AssetDatabase.LoadAssetAtPath<PlayerTuning>(Root+"/Config/MainRoomPlayerTuning.asset");test.player.worldMask=1<<layer;
  var ghost=(GameObject)PrefabUtility.InstantiatePrefab(AssetDatabase.LoadAssetAtPath<GameObject>(Root+"/Prefabs/FreezeEcho.prefab"),scene);ghost.name="Rewind playback goat";
  ghost.transform.SetParent(root.transform);test.echo=ghost.GetComponent<EchoSurface>();ghost.SetActive(false);
  var camera=new GameObject("Rewind room camera");camera.transform.SetParent(root.transform);test.view=camera.AddComponent<Camera>();test.view.tag="MainCamera";test.view.orthographic=true;test.view.orthographicSize=5.625f;test.view.transform.position=new Vector3(10,4,-10);test.view.backgroundColor=new Color(.24f,.42f,.48f);test.view.clearFlags=CameraClearFlags.SolidColor;camera.AddComponent<AudioListener>();
  PresentationBuilder.Background(root.transform);PresentationBuilder.Audio(root);
  Block(root.transform,"Practice floor",new Rect(-3,-3,46,3),layer);
  Block(root.transform,"Left boundary",new Rect(-3.5f,-3,.5f,20),layer);Block(root.transform,"Right boundary",new Rect(43,-3,.5f,20),layer);
  test.crates=new RewindCrate[2];for(int i=0;i<2;i++){
   float x=i==0?8:11;var go=new GameObject("Pushable crate "+(i+1));go.transform.SetParent(root.transform);go.transform.position=new Vector2(x,.6f);go.layer=layer;
   var crate=go.AddComponent<RewindCrate>();crate.solid=go.AddComponent<BoxCollider2D>();crate.solid.size=new Vector2(1.2f,1.2f);crate.minX=i==0?6:10.5f;crate.maxX=i==0?9:13;
   PresentationBuilder.Image(go.transform,"Wooden crate",MainRoomArtBuilder.Art("WeatheredWoodPlatform"),go.transform.position,new Vector2(1.2f,1.2f),5,new Color(.9f,.85f,.7f));test.crates[i]=crate;
  }
  var plateObject=new GameObject("Pressure plate");plateObject.transform.SetParent(root.transform);plateObject.transform.position=new Vector2(17,0);test.plate=plateObject.AddComponent<PressurePlate>();test.plate.graphic=PresentationBuilder.Image(plateObject.transform,"Plate artwork",MainRoomArtBuilder.Art("PressurePlate"),new Vector2(17,.08f),new Vector2(1.5f,.25f),6,Color.white).GetComponent<SpriteRenderer>();
  var gate=Block(root.transform,"Plate gate",new Rect(23,0,.5f,3),layer);test.door=gate.AddComponent<LinkedDoor>();test.door.plate=test.plate;test.door.solid=gate.GetComponent<BoxCollider2D>();test.door.graphic=gate.GetComponent<SpriteRenderer>();
  var platform=Block(root.transform,"Moving platform",new Rect(28,1.82f,3,.18f),layer,true);var moving=platform.AddComponent<RoomPlatform>();moving.solid=platform.GetComponent<BoxCollider2D>();moving.artwork=platform.GetComponentsInChildren<SpriteRenderer>();moving.travel=new Vector2(5,3);moving.speed=1.5f;test.platforms=new[]{moving};
  Block(root.transform,"Upper perch",new Rect(35,5.82f,4,.18f),layer,true);
  var trace=new GameObject("Recorded player trace");trace.transform.SetParent(root.transform);var line=trace.AddComponent<LineRenderer>();line.sharedMaterial=AssetDatabase.LoadAssetAtPath<Material>(Root+"/Generated/RewindTrace.mat");
  if(line.sharedMaterial==null){line.sharedMaterial=new Material(Shader.Find("Sprites/Default"));AssetDatabase.CreateAsset(line.sharedMaterial,Root+"/Generated/RewindTrace.mat");}
  line.useWorldSpace=true;line.positionCount=0;RewindPlaytest.StyleTrace(line);test.path=trace.AddComponent<EchoPath>();test.echo.support.enabled=false;
  MainRoomBuilder.ApplyMaterials(root);EditorSceneManager.SaveScene(scene,Path);EditorBuildSettings.scenes=EditorBuildSettings.scenes.Where(x=>x.path!=Path).Concat(new[]{new EditorBuildSettingsScene(Path,true)}).ToArray();AssetDatabase.SaveAssets();
  if(!loaded)EditorSceneManager.CloseScene(scene,true);if(previous.IsValid()&&previous.isLoaded)SceneManager.SetActiveScene(previous);
 }
 public static void RefreshPresentation(){
  if(EditorApplication.isPlaying)throw new System.InvalidOperationException("Exit Play first");
  var previous=SceneManager.GetActiveScene();var scene=SceneManager.GetSceneByPath(Path);bool loaded=scene.IsValid()&&scene.isLoaded;if(!loaded)scene=EditorSceneManager.OpenScene(Path,OpenSceneMode.Additive);
  DemoBuilder.BackupScene(Path);var room=scene.GetRootGameObjects().SelectMany(x=>x.GetComponentsInChildren<RewindPlaytest>(true)).Single();RewindPlaytest.StyleTrace(room.path.GetComponent<LineRenderer>());room.echo.support.enabled=false;EditorSceneManager.SaveScene(scene,Path);
  if(!loaded)EditorSceneManager.CloseScene(scene,true);if(previous.IsValid()&&previous.isLoaded)SceneManager.SetActiveScene(previous);
 }
}
}
