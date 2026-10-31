using System;using System.IO;using System.Linq;using System.Collections.Generic;using UnityEditor;using UnityEditor.SceneManagement;using UnityEngine;using UnityEngine.SceneManagement;
namespace OneMoreTime.EditorTools {
public static class PresentationBuilder {
 const string R=DemoBuilder.Root;
 public static Font Font=>AssetDatabase.LoadAssetAtPath<Font>(R+"/Fonts/NotoSansTC-Medium.ttf");
 public static Sprite White(){string tp=R+"/Generated/White.asset";var t=AssetDatabase.LoadAssetAtPath<Texture2D>(tp);if(t==null){t=new Texture2D(2,2);t.SetPixels(new[]{Color.white,Color.white,Color.white,Color.white});t.Apply();AssetDatabase.CreateAsset(t,tp);}return SpriteAsset("WhiteSprite",t,new Rect(0,0,2,2),2);}
 public static Sprite Atlas(string name,Rect rect){var t=AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/SunnyLand Artwork/Environment/Tileset/tileset-sliced.png");return SpriteAsset(name,t,rect,16);}
 static Sprite SpriteAsset(string name,Texture2D t,Rect r,float ppu){string path=R+"/Generated/"+name+".asset";var s=AssetDatabase.LoadAssetAtPath<Sprite>(path);if(s==null){s=UnityEngine.Sprite.Create(t,r,new Vector2(.5f,.5f),ppu,0,SpriteMeshType.FullRect);s.name=name;AssetDatabase.CreateAsset(s,path);}return s;}
 public static GameObject Image(Transform parent,string name,Sprite sprite,Vector2 pos,Vector2 size,int order,Color color){var g=new GameObject(name);g.transform.SetParent(parent);g.transform.position=pos;var sr=g.AddComponent<SpriteRenderer>();sr.sprite=sprite;sr.sortingOrder=order;sr.color=color;if(sprite!=null)g.transform.localScale=new Vector3(size.x/sprite.bounds.size.x,size.y/sprite.bounds.size.y,1);return g;}
 static GameObject Prop(Transform parent,string file,Vector2 feet,float height,int order=-2,Color? tint=null){var s=DemoBuilder.Sprite("Environment/props/"+file+".png");return Image(parent,"Scenery "+file,s,feet+Vector2.up*height/2,new Vector2(height*s.bounds.size.x/s.bounds.size.y,height),order,tint??Color.white);}
 public static GameAudio Audio(GameObject root){var a=root.AddComponent<GameAudio>();a.music=AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/SunnyLand Artwork/Music tracks/under the rainbow.ogg");return a;}
 public static void Background(Transform root){Image(root,"Far Mountains",DemoBuilder.Sprite("Environment/back.png"),new Vector2(16,2),new Vector2(70,43.75f),-40,new Color(.87f,.96f,.94f));var forest=new GameObject("Forest");forest.transform.SetParent(root);for(int i=0;i<9;i++)Image(forest.transform,"Forest trunk "+i,DemoBuilder.Sprite("Environment/middle.png"),new Vector2(-16+i*9, -3.5f+(i%2)*.65f),new Vector2(10,20.91f),-30,new Color(.70f,.88f,.82f));}
 // Isolate tiles from the atlas: UVs on tiled quads must never sample atlas gutters.
 static Sprite TerrainTile(string name,RectInt rect){
  string path=R+"/Generated/"+name+"Texture.asset";var texture=AssetDatabase.LoadAssetAtPath<Texture2D>(path);
  if(texture==null){var source=new Texture2D(2,2);source.LoadImage(File.ReadAllBytes("Assets/SunnyLand Artwork/Environment/Tileset/tileset-sliced.png"));texture=new Texture2D(rect.width,rect.height,TextureFormat.RGBA32,false);texture.SetPixels(source.GetPixels(rect.x,rect.y,rect.width,rect.height));texture.filterMode=FilterMode.Point;texture.wrapMode=TextureWrapMode.Clamp;texture.Apply();AssetDatabase.CreateAsset(texture,path);UnityEngine.Object.DestroyImmediate(source);}
  return SpriteAsset(name,texture,new Rect(0,0,rect.width,rect.height),16);
 }
 public static void TerrainSkin(GameObject go,Rect rect,string name){
  var soil=go.AddComponent<SpriteRenderer>();soil.sprite=TerrainTile("EarthFillSeamless",new RectInt(80,304,16,16));soil.drawMode=SpriteDrawMode.Tiled;soil.size=rect.size;soil.color=new Color(.92f,.85f,.80f);
  float cap=Mathf.Min(1f,rect.height);
  if(name=="Terrain"||name=="Ground"){
   var grass=new GameObject("Continuous grass edge");grass.transform.SetParent(go.transform);grass.transform.position=new Vector2(rect.center.x,rect.yMax-cap/2);var edge=grass.AddComponent<SpriteRenderer>();edge.sprite=TerrainTile("GrassEdgeSeamless",new RectInt(80,336,16,16));edge.drawMode=SpriteDrawMode.Tiled;edge.size=new Vector2(rect.width,cap);edge.sortingOrder=4;
   if(rect.height>2&&rect.width>2){for(float x=rect.xMin+.25f;x<rect.xMax;x+=4.2f)Image(go.transform,"Hanging fern",DemoBuilder.Sprite("Environment/props/bush.png"),new Vector2(x,rect.yMax-.22f),new Vector2(.8f,.55f),5,new Color(.71f,.83f,.52f)).transform.rotation=Quaternion.Euler(0,0,180);}
  }
 }
 public static void Decorate(GameObject root,LevelConfig cfg,Camera camera){
  Background(root.transform);var run=root.GetComponent<RunManager>();run.uiFont=Font;run.audioBus=Audio(root);
  var parallax=root.AddComponent<SceneryMotion>();parallax.view=camera;parallax.layers=new[]{root.transform.Find("Far Mountains"),root.transform.Find("Forest")};parallax.amounts=new[]{.68f,.38f};
  var tracePath=new GameObject("Memory path preview");tracePath.transform.SetParent(root.transform);var lr=tracePath.AddComponent<LineRenderer>();lr.material=new Material(Shader.Find("Sprites/Default"));lr.startColor=lr.endColor=new Color(.59f,1,.79f,.28f);lr.startWidth=lr.endWidth=.035f;lr.positionCount=0;lr.sortingOrder=6;run.path=tracePath.AddComponent<EchoPath>();
  for(int i=0;i<cfg.doors.Length;i++){
   int source=cfg.DoorSource(i);var p=cfg.plates[source];var d=cfg.doors[i];var door=root.transform.Find("Door "+(char)('A'+i)).GetComponent<LinkedDoor>();door.graphic.sprite=White();door.graphic.drawMode=SpriteDrawMode.Sliced;door.graphic.size=d.size;
   var bars=new List<SpriteRenderer>();for(int k=0;k<3;k++)bars.Add(Image(door.transform,"Gate brass rail",White(),new Vector2(d.xMin+.1f+k*.2f,d.center.y),new Vector2(.065f,d.height),7,Color.white).GetComponent<SpriteRenderer>());
   for(int k=0;k<2;k++)Image(root.transform,"Gate stone jamb",Atlas("GateStone",new Rect(272,80,16,16)),new Vector2(k==0?d.xMin-.2f:d.xMax+.2f,d.center.y),new Vector2(.28f,d.height+.25f),5,new Color(.87f,.85f,.76f));
   Image(root.transform,"Gate lintel",White(),new Vector2(d.center.x,d.yMax),new Vector2(1.12f,.18f),8,new Color(.37f,.39f,.29f));
   Prop(root.transform,"rock-1",p-new Vector2(.85f,0),.35f,2);Prop(root.transform,"sign",p-new Vector2(1.05f,0),.75f,-1);
   var wire=new GameObject("Signal "+(char)('A'+i));wire.transform.SetParent(root.transform);var line=wire.AddComponent<LineRenderer>();line.material=new Material(Shader.Find("Sprites/Default"));line.startWidth=line.endWidth=.025f;line.sortingOrder=5;line.positionCount=4;line.SetPositions(new[]{new Vector3(p.x,p.y+.12f),new Vector3(p.x,p.y-.5f),new Vector3(d.center.x,p.y-.5f),new Vector3(d.center.x,d.yMin)});
   var signal=wire.AddComponent<MechanismSignal>();signal.plate=root.transform.Find("Plate "+(char)('A'+source)).GetComponent<PressurePlate>();signal.wire=line;signal.door=door;signal.bars=bars.ToArray();
   Label(root.transform,"Plate letter",((char)('A'+source)).ToString(),p+new Vector2(0,.4f));Label(root.transform,"Gate letter",((char)('A'+source)).ToString(),new Vector2(d.center.x,d.yMin+1.6f));
  }
  foreach(var terrain in cfg.terrain){if(terrain.width<3||terrain.yMax>8)continue;float y=terrain.yMax;
   for(float x=Mathf.Max(terrain.xMin+.4f,0);x<terrain.xMax-.4f;x+=2.35f){bool mechanism=cfg.plates.Any(p=>Mathf.Abs(p.x-x)<1.25f&&Mathf.Abs(p.y-y)<1)||cfg.doors.Any(d=>Mathf.Abs(d.center.x-x)<1.2f);if(mechanism)continue;int seed=Mathf.Abs(Mathf.RoundToInt(x*11));Prop(root.transform,seed%3==0?"shrooms":seed%3==1?"rock-2":"bush",new Vector2(x,y),seed%3==2?.55f:.3f,5,new Color(.92f,1,.85f));}
   if(terrain.width>6&&y<7){float x=terrain.xMin+terrain.width*.32f;Prop(root.transform,cfg.level%2==0?"tree-2":"tree",new Vector2(x,y),3.2f,-6,new Color(.77f,.90f,.72f));if(terrain.width>12)Prop(root.transform,"pine",new Vector2(terrain.xMax-2,y),4.2f,-8,new Color(.62f,.80f,.72f));}
  }
  Prop(root.transform,"wooden-house",new Vector2(cfg.spawn.x-1.9f,cfg.spawn.y),2.0f,-7,new Color(.8f,.86f,.77f));
  Prop(root.transform,"sign",new Vector2(cfg.spawn.x+1,cfg.spawn.y),.8f,3);
  Prop(root.transform,"bush",new Vector2(cfg.goal.xMax+1,cfg.goal.yMin),.7f,4);
  Prop(root.transform,"shrooms",new Vector2(cfg.goal.xMin-.65f,cfg.goal.yMin),.4f,4);
  var goal=root.transform.Find("Goal");Image(root.transform,"Exit canopy",Atlas("GrassTopV2",new Rect(80,336,16,16)),new Vector2(cfg.goal.center.x,goal.GetComponent<SpriteRenderer>().bounds.max.y+.08f),new Vector2(1.2f,.16f),6,Color.white);
 }
 static void Label(Transform root,string name,string text,Vector2 p){var g=new GameObject(name);g.transform.SetParent(root);g.transform.position=p;var m=g.AddComponent<TextMesh>();m.text=text;m.font=Font;m.fontSize=32;m.characterSize=.15f;m.anchor=TextAnchor.MiddleCenter;m.color=new Color(1,.9f,.59f);g.GetComponent<MeshRenderer>().sharedMaterial=Font.material;g.GetComponent<MeshRenderer>().sortingOrder=15;}
 public static void Menus(){foreach(string file in new[]{"OpeningDialogue","GoatPortrait"}){var importer=(TextureImporter)AssetImporter.GetAtPath(R+"/Resources/Dialogue/"+file+".png");importer.textureType=TextureImporterType.Default;importer.alphaIsTransparency=true;importer.mipmapEnabled=false;importer.npotScale=TextureImporterNPOTScale.None;importer.textureCompression=TextureImporterCompression.Uncompressed;importer.filterMode=FilterMode.Point;importer.SaveAndReimport();}foreach(bool ending in new[]{false,true}){string name=ending?"Ending":"MainMenu",path=R+"/Scenes/"+name+".unity";DemoBuilder.BackupScene(path);var prev=SceneManager.GetActiveScene();var loaded=SceneManager.GetSceneByPath(path);bool wasLoaded=loaded.IsValid()&&loaded.isLoaded;var scene=wasLoaded?loaded:File.Exists(path)?EditorSceneManager.OpenScene(path,OpenSceneMode.Additive):EditorSceneManager.NewScene(NewSceneSetup.EmptyScene,NewSceneMode.Additive);SceneManager.SetActiveScene(scene);var old=scene.GetRootGameObjects().FirstOrDefault(g=>g.name=="OMT Managed");if(old!=null)UnityEngine.Object.DestroyImmediate(old);var root=new GameObject("OMT Managed");var c=new GameObject("Camera").AddComponent<Camera>();c.transform.SetParent(root.transform);c.orthographic=true;c.orthographicSize=5.5f;c.transform.position=new Vector3(10,3,-10);c.gameObject.AddComponent<AudioListener>();var screen=root.AddComponent<MenuScreen>();screen.font=Font;screen.artwork=AssetDatabase.LoadAssetAtPath<Texture2D>(R+"/Art/UI/JourneyTitle.png");screen.dialogueFrame=AssetDatabase.LoadAssetAtPath<Texture2D>(R+"/Resources/Dialogue/OpeningDialogue.png");screen.goatPortrait=AssetDatabase.LoadAssetAtPath<Texture2D>(R+"/Resources/Dialogue/GoatPortrait.png");screen.ending=ending;Audio(root);EditorSceneManager.SaveScene(scene,path);if(!wasLoaded)EditorSceneManager.CloseScene(scene,true);SceneManager.SetActiveScene(prev);}}
}
}
