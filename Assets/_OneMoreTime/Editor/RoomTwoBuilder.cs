using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
namespace OneMoreTime.EditorTools {
public static class RoomTwoBuilder {
 const string Root=DemoBuilder.Root;
 public const string Path=Root+"/Scenes/"+MainRoom.RoomTwoScene+".unity";
 static GameObject Image(Transform parent,string name,Vector2 pos,Vector2 size,Color color,int order=5)=>PresentationBuilder.Image(parent,name,PresentationBuilder.White(),pos,size,order,color);
 [MenuItem("Tools/One More Time/Build Main Room 2 - Shutter")]
 public static void Build(){
  if(EditorApplication.isPlaying)throw new System.InvalidOperationException("Stop Play before building Room 2");
  if(Application.isBatchMode&&string.IsNullOrEmpty(SceneManager.GetActiveScene().path))EditorSceneManager.OpenScene(Root+"/Scenes/MainMenu.unity");
  Directory.CreateDirectory("Artifacts/Logs");DemoBuilder.BackupScene(Path);
  var previous=SceneManager.GetActiveScene();var scene=SceneManager.GetSceneByPath(Path);bool loaded=scene.IsValid()&&scene.isLoaded;
  if(!loaded)scene=File.Exists(Path)?EditorSceneManager.OpenScene(Path,OpenSceneMode.Additive):EditorSceneManager.NewScene(NewSceneSetup.EmptyScene,NewSceneMode.Additive);
  SceneManager.SetActiveScene(scene);var old=scene.GetRootGameObjects().FirstOrDefault(x=>x.name=="Room 2 Managed");if(old!=null)Object.DestroyImmediate(old);
  var root=new GameObject("Room 2 Managed");var room=root.AddComponent<MainRoom>();room.font=PresentationBuilder.Font;room.movementTutorial=false;room.nextRoomScene=MainRoom.RoomThreeScene;room.checkpoint=new Vector2(24.3f,5f);room.roomBounds=new Rect(0,-2,44,11);room.exitX=43.2f;room.exitFloor=5f;room.deathY=-4.5f;PresentationBuilder.Audio(root);
  var player=(GameObject)PrefabUtility.InstantiatePrefab(AssetDatabase.LoadAssetAtPath<GameObject>(Root+"/Prefabs/Player.prefab"),scene);player.transform.SetParent(root.transform);player.transform.position=room.entrance;room.player=player.GetComponent<PlayerMotor2D>();room.visual=player.GetComponentInChildren<PlayerVisual>();
  var tuning=AssetDatabase.LoadAssetAtPath<PlayerTuning>(Root+"/Config/MainRoomPlayerTuning.asset");if(tuning==null)throw new System.InvalidOperationException("Build Room 1 first to create its shared main-game tuning");room.player.tuning=tuning;
  var camera=new GameObject("Room Camera").AddComponent<Camera>();camera.transform.SetParent(root.transform);camera.tag="MainCamera";camera.orthographic=true;camera.orthographicSize=10f/(960f/503f);camera.transform.position=new Vector3(10,2.75f,-10);camera.clearFlags=CameraClearFlags.SolidColor;camera.backgroundColor=new Color(.48f,.73f,.82f);camera.gameObject.AddComponent<AudioListener>();room.view=camera;
  var sky=AssetDatabase.LoadAssetAtPath<Sprite>(Root+"/Art/ChapterOne/PresentClearSky.png");if(sky!=null)room.sky=PresentationBuilder.Image(root.transform,"Open sky",sky,new Vector2(22,4),new Vector2(50,26),-40,Color.white).transform;
  int layer=DemoBuilder.GetLayer();room.player.worldMask=1<<layer;
  Land(root.transform,"Shutter approach",new Rect(-3,-6,22,6),layer);
  Land(root.transform,"Raised checkpoint ledge",new Rect(19,-6,8,11),layer);
  Land(root.transform,"Exit bank",new Rect(36,-6,12,11),layer);
  room.entranceBarrier=Solid(root.transform,"Entrance closes after walk-in",new Rect(-.5f,-6,.5f,17),layer).GetComponent<BoxCollider2D>();room.entranceBarrier.enabled=false;
  Solid(root.transform,"Left boundary",new Rect(-3.5f,-6,.5f,18),layer);Solid(root.transform,"Right boundary",new Rect(48,-6,.5f,18),layer);
  MainRoomBuilder.Statue(root.transform,room);
  var ability=root.AddComponent<ShutterAbility>();ability.room=room;room.shutter=ability;
  var pickup=new GameObject("Purple camera - unlock Shutter");pickup.transform.SetParent(root.transform);pickup.transform.position=new Vector2(12.4f,.55f);ability.pickup=pickup.transform;
  Image(pickup.transform,"Camera body",new Vector2(12.4f,.55f),new Vector2(1,.7f),new Color(.64f,.17f,.78f));
  Image(pickup.transform,"Camera top",new Vector2(12.16f,.96f),new Vector2(.3f,.14f),new Color(.85f,.48f,.95f));
  Image(pickup.transform,"Lens rim",new Vector2(12.5f,.55f),new Vector2(.43f,.43f),new Color(.86f,.65f,1),6);
  Image(pickup.transform,"Camera lens",new Vector2(12.5f,.55f),new Vector2(.24f,.24f),GameUI.Ink,7);
  var echo=(GameObject)PrefabUtility.InstantiatePrefab(AssetDatabase.LoadAssetAtPath<GameObject>(Root+"/Prefabs/FreezeEcho.prefab"),scene);echo.transform.SetParent(root.transform);echo.name="Shutter frozen player";ability.echo=echo.GetComponent<EchoSurface>();ability.echo.support.enabled=false;echo.SetActive(false);
  var sign=DemoBuilder.Sprite("Environment/props/sign.png");if(sign!=null)PresentationBuilder.Image(root.transform,"Exit trail marker",sign,new Vector2(42.7f,5.4f),new Vector2(.7f,.8f),4,Color.white);
  MainRoomArtBuilder.Background(root,room);
  MainRoomBuilder.ApplyMaterials(root);EditorSceneManager.SaveScene(scene,Path);AssetDatabase.SaveAssets();
  // One overview render, separate from the saved 20-unit gameplay camera.
  var original=camera.transform.position;float size=camera.orthographicSize;camera.transform.position=new Vector3(22,4,-10);camera.orthographicSize=7;
  var target=new RenderTexture(1440,458,24);camera.targetTexture=target;room.visual.Pose(room.start,1,0,0,Color.white);
  UnityEngine.Rendering.RenderPipeline.SubmitRenderRequest(camera,new UnityEngine.Rendering.RenderPipeline.StandardRequest{destination=target});
  var prior=RenderTexture.active;RenderTexture.active=target;var shot=new Texture2D(1440,458,TextureFormat.RGB24,false);shot.ReadPixels(new Rect(0,0,1440,458),0,0);shot.Apply();File.WriteAllBytes("Artifacts/Logs/room2-preview.png",shot.EncodeToPNG());RenderTexture.active=prior;camera.targetTexture=null;Object.DestroyImmediate(shot);target.Release();Object.DestroyImmediate(target);camera.transform.position=original;camera.orthographicSize=size;room.visual.transform.position=room.entrance;
  if(!loaded)EditorSceneManager.CloseScene(scene,true);if(previous.IsValid()&&previous.isLoaded)SceneManager.SetActiveScene(previous);
  ConnectRoomOne();
  if(!EditorBuildSettings.scenes.Any(x=>x.path==Path))EditorBuildSettings.scenes=EditorBuildSettings.scenes.Concat(new[]{new EditorBuildSettingsScene(Path,true)}).ToArray();
  File.WriteAllText("Artifacts/Logs/room2-generation.txt","Room 2 saved: purple Shutter pickup, dot grid: 5-unit ledge, 8-unit checkpoint plateau, invisible 9-unit pit, exit. Room 1 connected.\n");
 }
 static void ConnectRoomOne(){
  string path=MainRoomBuilder.Path;DemoBuilder.BackupScene(path);var scene=SceneManager.GetSceneByPath(path);bool loaded=scene.IsValid()&&scene.isLoaded;if(!loaded)scene=EditorSceneManager.OpenScene(path,OpenSceneMode.Additive);
  var room=scene.GetRootGameObjects().SelectMany(x=>x.GetComponentsInChildren<MainRoom>(true)).Single();room.nextRoomScene=MainRoom.RoomTwoScene;EditorUtility.SetDirty(room);EditorSceneManager.SaveScene(scene,path);if(!loaded)EditorSceneManager.CloseScene(scene,true);
 }
 static GameObject Solid(Transform root,string name,Rect rect,int layer){var g=new GameObject(name);g.transform.SetParent(root);g.transform.position=rect.center;g.layer=layer;g.AddComponent<BoxCollider2D>().size=rect.size;return g;}
 static void Land(Transform root,string name,Rect rect,int layer){PresentationBuilder.TerrainSkin(Solid(root,name,rect,layer),rect,"Ground");}
}
}
