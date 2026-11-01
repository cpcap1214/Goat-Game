using System;using System.IO;using System.Linq;using UnityEditor;using UnityEditor.SceneManagement;using UnityEngine;using UnityEngine.SceneManagement;
namespace OneMoreTime.EditorTools {
public static class ChapterOneBuilder {
 const string R=DemoBuilder.Root;public const string Path=R+"/Scenes/Chapter01_EchoFoothills.unity";
 static GameObject Image(Transform p,string name,Vector2 pos,Vector2 size,Color color,int order=4)=>PresentationBuilder.Image(p,name,PresentationBuilder.White(),pos,size,order,color);
 static Sprite ImportSky(string name){
  string p=R+"/Art/ChapterOne/"+name+".png";if(!File.Exists(p))return DemoBuilder.Sprite("Environment/back.png");
  AssetDatabase.ImportAsset(p);var i=(TextureImporter)AssetImporter.GetAtPath(p);i.textureType=TextureImporterType.Sprite;i.spriteImportMode=SpriteImportMode.Single;i.spritePixelsPerUnit=64;i.filterMode=FilterMode.Point;i.mipmapEnabled=false;i.textureCompression=TextureImporterCompression.Uncompressed;i.maxTextureSize=2048;i.SaveAndReimport();return AssetDatabase.LoadAssetAtPath<Sprite>(p);
 }
 [MenuItem("Tools/One More Time/Build Chapter One - Nostalgia")]
 public static void Build(){
  if(EditorApplication.isPlaying)throw new Exception("Stop play before building chapter");
  Directory.CreateDirectory("Artifacts/Logs");DemoBuilder.BackupScene(Path);
  var prev=SceneManager.GetActiveScene();var scene=SceneManager.GetSceneByPath(Path);bool loaded=scene.IsValid()&&scene.isLoaded;
  if(!loaded)scene=File.Exists(Path)?EditorSceneManager.OpenScene(Path,OpenSceneMode.Additive):EditorSceneManager.NewScene(NewSceneSetup.EmptyScene,NewSceneMode.Additive);
  SceneManager.SetActiveScene(scene);var old=scene.GetRootGameObjects().FirstOrDefault(x=>x.name=="Chapter One Managed");if(old!=null)UnityEngine.Object.DestroyImmediate(old);
  var root=new GameObject("Chapter One Managed");var game=root.AddComponent<NostalgiaChapter>();game.font=PresentationBuilder.Font;game.audioBus=PresentationBuilder.Audio(root);
  var player=(GameObject)PrefabUtility.InstantiatePrefab(AssetDatabase.LoadAssetAtPath<GameObject>(R+"/Prefabs/Player.prefab"),scene);player.transform.SetParent(root.transform);player.transform.position=ChapterLayout.Checkpoints[0];game.player=player.GetComponent<PlayerMotor2D>();game.visual=player.GetComponentInChildren<PlayerVisual>();
  var cam=new GameObject("Chapter Camera").AddComponent<Camera>();cam.transform.SetParent(root.transform);cam.tag="MainCamera";cam.orthographic=true;cam.orthographicSize=6.1f;cam.transform.position=new Vector3(8,3.2f,-10);cam.clearFlags=CameraClearFlags.SolidColor;cam.backgroundColor=new Color(.15f,.29f,.32f);cam.gameObject.AddComponent<AudioListener>();game.view=cam;
  game.presentSky=PresentationBuilder.Image(root.transform,"PRESENT - overgrown valley",ImportSky("PresentClearSky"),new Vector2(8,4.2f),new Vector2(38,25.33f),-50,Color.white).GetComponent<SpriteRenderer>();
  game.pastSky=PresentationBuilder.Image(root.transform,"PAST - living valley",ImportSky("PastClearSky"),new Vector2(8,4.2f),new Vector2(38,25.33f),-50,Color.white).GetComponent<SpriteRenderer>();
  var common=new GameObject("Shared geography");common.transform.SetParent(root.transform);var present=new GameObject("Present map - ruins and grown branches");present.transform.SetParent(root.transform);var past=new GameObject("Past map - restored bridges and scaffolding");past.transform.SetParent(root.transform);
  int layer=DemoBuilder.GetLayer();int index=0;
  foreach(var block in ChapterLayout.Blocks()){
   var parent=block.era==ChapterEra.Both?common.transform:block.era==ChapterEra.Present?present.transform:past.transform;
   var g=new GameObject($"{index++:00} {block.era} {(block.bridge?"Bridge":"Ground")}");g.transform.SetParent(parent);g.layer=layer;g.transform.position=block.rect.center;var c=g.AddComponent<BoxCollider2D>();c.size=block.rect.size;
   if(!block.bridge)ChapterTerrainArt.Apply(g,block.rect,index-1);
   else{Image(g.transform,"Wood deck",block.rect.center,block.rect.size,block.era==ChapterEra.Past?new Color(.69f,.48f,.26f):new Color(.31f,.44f,.29f));for(float x=block.rect.xMin+.15f;x<block.rect.xMax;x+=.45f)Image(g.transform,"Deck joint",new Vector2(x,block.rect.yMax-.08f),new Vector2(.035f,.13f),new Color(.25f,.21f,.17f));Image(g.transform,"Rope",new Vector2(block.rect.center.x,block.rect.yMax+.24f),new Vector2(block.rect.width,.045f),new Color(.68f,.56f,.38f),-1);}
   var era=g.AddComponent<EraTerrain>();era.block=block;era.solid=c;era.art=g.GetComponentsInChildren<SpriteRenderer>();
   if(block.era==ChapterEra.Past)foreach(var a in era.art)a.color*=new Color(1,.88f,.63f,1);
   if(!block.bridge&&block.rect.width>5&&block.rect.height>1){var spr=DemoBuilder.Sprite("Environment/props/bush.png");for(float x=block.rect.xMin+2;x<block.rect.xMax-1;x+=5.5f)PresentationBuilder.Image(g.transform,"Wildflowers and fern",spr,new Vector2(x,block.rect.yMax+.23f),new Vector2(1.1f,.6f),2,new Color(.75f,.87f,.58f));era.art=g.GetComponentsInChildren<SpriteRenderer>();}
  }
  game.terrain=root.GetComponentsInChildren<EraTerrain>();
  var memory=root.AddComponent<ChapterMemory>();game.memories=memory;memory.game=game;
  var ghost=(GameObject)PrefabUtility.InstantiatePrefab(AssetDatabase.LoadAssetAtPath<GameObject>(R+"/Prefabs/FreezeEcho.prefab"),scene);ghost.transform.SetParent(root.transform);memory.echo=ghost.GetComponent<EchoSurface>();
  memory.freezePad=Image(root.transform,"PAST pressure plate - E freeze",new Vector2(80,.08f),new Vector2(2,.16f),GameUI.Gold,8).GetComponent<SpriteRenderer>();
  memory.padA=Image(root.transform,"Recording signal A",new Vector2(205,.08f),new Vector2(2,.16f),GameUI.Gold,8).GetComponent<SpriteRenderer>();
  memory.padB=Image(root.transform,"Recording signal B",new Vector2(216,.08f),new Vector2(2,.16f),GameUI.Gold,8).GetComponent<SpriteRenderer>();
  memory.console=Image(root.transform,"Stand here and Q playback",new Vector2(230,.08f),new Vector2(2,.16f),GameUI.Cream,8).GetComponent<SpriteRenderer>();
  foreach(float x in new[]{104f,239f}){var gate=Image(root.transform,x==104?"Freeze pressure gate":"Ordered echo signal gate",new Vector2(x,6),new Vector2(.8f,12),new Color(.25f,.43f,.48f),7);gate.layer=layer;var col=gate.AddComponent<BoxCollider2D>();col.size=Vector2.one;if(x==104)memory.freezeGate=col;else memory.traceGate=col;}

  var filter=ImportSky("NostalgiaFilter");game.pickup=PresentationBuilder.Image(root.transform,"Nostalgia filter pickup",filter,new Vector2(7,1.2f),new Vector2(1.1f,1.1f),10,Color.white).transform;game.filterIcon=filter.texture;
  game.deviceArt=new SpriteRenderer[2];for(int i=0;i<2;i++){
   Vector2 p=ChapterLayout.Devices[i];var wheel=new GameObject(i==0?"Ancient waterwheel crank":"Old bell rope");wheel.transform.SetParent(root.transform);
   if(i==0){
    wheel.transform.position=p+Vector2.up*1.15f;game.waterwheel=wheel.transform;
    for(int spoke=0;spoke<8;spoke++){var beam=Image(wheel.transform,"Wheel spoke",p+Vector2.up*1.15f,new Vector2(.10f,2.2f),new Color(.49f,.32f,.18f));beam.transform.rotation=Quaternion.Euler(0,0,spoke*22.5f);}
    for(int paddle=0;paddle<16;paddle++){float angle=paddle*Mathf.PI/8;Vector2 rim=p+Vector2.up*1.15f+new Vector2(Mathf.Cos(angle),Mathf.Sin(angle))*1.1f;var board=Image(wheel.transform,"Water paddle",rim,new Vector2(.35f,.16f),new Color(.62f,.43f,.25f));board.transform.rotation=Quaternion.Euler(0,0,angle*Mathf.Rad2Deg+90);}
   }else{
    Image(root.transform,"Belfry left pillar",p+new Vector2(-1.1f,1.55f),new Vector2(.25f,3.1f),new Color(.43f,.43f,.39f),2);
    Image(root.transform,"Belfry right pillar",p+new Vector2(1.1f,1.55f),new Vector2(.25f,3.1f),new Color(.43f,.43f,.39f),2);
    Image(root.transform,"Belfry crossbeam",p+Vector2.up*3.1f,new Vector2(2.6f,.26f),new Color(.39f,.29f,.21f),3);
    wheel.transform.position=p+Vector2.up*2.9f;game.bell=wheel.transform;
    Image(wheel.transform,"Bronze bell crown",p+Vector2.up*2.55f,new Vector2(.65f,.65f),new Color(.79f,.57f,.24f),4);
    Image(wheel.transform,"Bronze bell skirt",p+Vector2.up*2.2f,new Vector2(.95f,.4f),GameUI.Gold,5);
    Image(wheel.transform,"Bell lip",p+Vector2.up*2,new Vector2(1.15f,.13f),new Color(.95f,.77f,.38f),6);
    Image(root.transform,"Pull rope",p+new Vector2(.7f,1.45f),new Vector2(.06f,2.5f),new Color(.78f,.68f,.43f),7);
   }
   game.deviceArt[i]=Image(root.transform,"E - repair in the past",p+Vector2.up*1.15f,new Vector2(.5f,.5f),GameUI.Gold,8).GetComponent<SpriteRenderer>();
  }
  game.photoArt=new SpriteRenderer[3];for(int i=0;i<3;i++){Vector2 p=ChapterLayout.Photos[i];var g=Image(root.transform,"Photo viewpoint "+(i+1),p+Vector2.up*.6f,new Vector2(.95f,.7f),GameUI.Cream,8);Image(g.transform,"Camera lens",p+Vector2.up*.6f,new Vector2(.4f,.4f),GameUI.Ink,9);Image(g.transform,"Tripod",p+Vector2.up*.15f,new Vector2(.1f,.5f),GameUI.Gold,7);game.photoArt[i]=g.GetComponent<SpriteRenderer>();}
  game.photoGates=new BoxCollider2D[2];float[] gateX={120,253};
  for(int i=0;i<2;i++){var gate=Image(root.transform,"Viewpoint exit - photograph first "+i,new Vector2(gateX[i],7),new Vector2(.18f,26),new Color(.54f,.91f,.88f,.48f),6);gate.layer=layer;var collider=gate.AddComponent<BoxCollider2D>();collider.size=Vector2.one;game.photoGates[i]=collider;}
  game.checkpointArt=new Transform[ChapterLayout.Checkpoints.Length];for(int i=0;i<game.checkpointArt.Length;i++){var p=ChapterLayout.Checkpoints[i];var flag=Image(root.transform,"Checkpoint "+i,p+Vector2.up*.8f,new Vector2(.08f,1.6f),GameUI.Ink,5);Image(flag.transform,"Mint flag",p+new Vector2(.3f,1.35f),new Vector2(.55f,.4f),GameUI.Mint,6);game.checkpointArt[i]=flag.transform;}
  EditorSceneManager.SaveScene(scene,Path);AssetDatabase.SaveAssets();if(!loaded)EditorSceneManager.CloseScene(scene,true);if(prev.IsValid()&&prev.isLoaded)SceneManager.SetActiveScene(prev);
  File.WriteAllText("Artifacts/Logs/chapter-generation.txt",$"Generated {gameName}: {index} collision blocks, two era maps, 11 checkpoints, 2 mechanisms, 3 photos.\n");
 }
 const string gameName="Chapter01_EchoFoothills";
}
}
