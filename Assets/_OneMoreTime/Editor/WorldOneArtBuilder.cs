using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;
using Object=UnityEngine.Object;
namespace OneMoreTime.EditorTools {
public static class WorldOneArtBuilder {
 const string Root="Assets/_OneMoreTime";
 public const string ScenePath=Root+"/Scenes/Debug_WorldOneArt.unity";
 static Texture2D Texture(string name){
  string path=Root+"/Art/WorldOne/"+name+".png";AssetDatabase.ImportAsset(path,ImportAssetOptions.ForceSynchronousImport);
  var t=(TextureImporter)AssetImporter.GetAtPath(path);t.textureType=TextureImporterType.Default;t.isReadable=true;t.alphaIsTransparency=true;t.filterMode=FilterMode.Point;t.mipmapEnabled=false;t.textureCompression=TextureImporterCompression.Uncompressed;t.maxTextureSize=2048;t.SaveAndReimport();return AssetDatabase.LoadAssetAtPath<Texture2D>(path);
 }
 static Sprite Sprite(Texture2D t,string name,Rect rect,Vector2 pivot,float ppu){
  string path=Root+"/Generated/WorldOne/"+name+".asset";var fresh=UnityEngine.Sprite.Create(t,rect,pivot,ppu,0,SpriteMeshType.FullRect);fresh.name=name;var existing=AssetDatabase.LoadAssetAtPath<Sprite>(path);
  if(existing==null){AssetDatabase.CreateAsset(fresh,path);return fresh;}EditorUtility.CopySerialized(fresh,existing);Object.DestroyImmediate(fresh);return existing;
 }
 static WorldOneSequence Sequence(string name,Sprite[] frames,float fps,bool loop){var g=new GameObject(name);var s=g.AddComponent<WorldOneSequence>();s.target=g.AddComponent<SpriteRenderer>();s.target.sortingOrder=10;s.frames=frames;s.framesPerSecond=fps;s.loop=loop;s.target.sprite=frames[0];return s;}
 static void Materials(GameObject g){foreach(var renderer in g.GetComponentsInChildren<Renderer>(true)){
  Texture texture=null;if(renderer is SpriteRenderer sr&&sr.sprite!=null)texture=sr.sprite.texture;else if(renderer is TilemapRenderer)texture=AssetDatabase.LoadAssetAtPath<Texture2D>(Root+"/Art/WorldOne/WaterAtlas.png");if(texture==null)continue;
  string path=Root+"/Generated/WorldOne/"+texture.name+".mat";var material=AssetDatabase.LoadAssetAtPath<Material>(path);if(material==null){material=new Material(Shader.Find("Sprites/Default"));AssetDatabase.CreateAsset(material,path);}if(texture.name=="ForestWaterfallLoop")material.shader=Shader.Find("OneMoreTime/Looping Forest");material.mainTexture=texture;renderer.sharedMaterial=material;EditorUtility.SetDirty(material);
 }}
 static GameObject Save(GameObject g,string name){Materials(g);var p=PrefabUtility.SaveAsPrefabAsset(g,Root+"/Prefabs/WorldOne/"+name+".prefab");Object.DestroyImmediate(g);return p;}
 static GameObject Instance(GameObject prefab,Transform parent,Vector2 position){var g=(GameObject)PrefabUtility.InstantiatePrefab(prefab);g.transform.SetParent(parent);g.transform.localPosition=position;return g;}
 // Slice only the drowning sheet, preserving existing sprite/prefab references.
 // Use the dense turquoise surface row as the pivot, so submersion never bobs at a row change.
 public static GameObject BuildDrowning(){
  if(EditorApplication.isPlaying)throw new InvalidOperationException("Exit Play mode first");
  var texture=Texture("GoatDrowning");var frames=new Sprite[6];
  for(int i=0;i<6;i++){
   int x=Mathf.RoundToInt(i%3*texture.width/3f),right=Mathf.RoundToInt((i%3+1)*texture.width/3f);
   int h=texture.height/2,bottom=texture.height-(i/3+1)*h,w=right-x;
   var pixels=texture.GetPixels(x,bottom,w,h);int waterline=0,best=0;
   for(int y=0;y<h/2;y++){int count=0;for(int px=0;px<w;px++){var c=pixels[y*w+px];if(c.a>.7f&&c.g>c.r*1.3f&&c.b>c.r*1.3f&&c.g>.3f)count++;}if(count>best){best=count;waterline=y;}}
   if(best<w*.1f)throw new InvalidOperationException("Missing drowning waterline in frame "+i);
   frames[i]=Sprite(texture,"GoatDrown"+i,new Rect(x,bottom,w,h),new Vector2(.5f,waterline/(float)h),w/2f);
  }
  var prefab=Save(Sequence("Goat Drowning",frames,6,false).gameObject,"GoatDrowning");AssetDatabase.SaveAssets();return prefab;
 }
 [MenuItem("Tools/One More Time/Build World One Art Preview")]
 public static void Build(){
  if(EditorApplication.isPlaying)throw new InvalidOperationException("Exit Play mode first");
  Directory.CreateDirectory(Root+"/Generated/WorldOne");Directory.CreateDirectory(Root+"/Prefabs/WorldOne");AssetDatabase.Refresh();
  var water=Texture("WaterAtlas");var tiles=new WaterAnimationTile[4];string[] names={"RiverSurface","WaterfallLip","WaterfallBody","WaterfallSplash"};
  for(int row=0;row<4;row++){
   var frames=new Sprite[4];for(int col=0;col<4;col++){int x=Mathf.RoundToInt(col*water.width/4f),right=Mathf.RoundToInt((col+1)*water.width/4f);int top=Mathf.RoundToInt(water.height-row*water.height/4f),bottom=Mathf.RoundToInt(water.height-(row+1)*water.height/4f);frames[col]=Sprite(water,names[row]+col,new Rect(x+2,bottom+2,right-x-4,top-bottom-4),new Vector2(.5f,.5f),right-x-4);}
   string path=Root+"/Generated/WorldOne/"+names[row]+".asset";var tile=AssetDatabase.LoadAssetAtPath<WaterAnimationTile>(path);if(tile==null){tile=ScriptableObject.CreateInstance<WaterAnimationTile>();AssetDatabase.CreateAsset(tile,path);}tile.frames=frames;tile.framesPerSecond=8;EditorUtility.SetDirty(tile);tiles[row]=tile;
  }
  var river=new GameObject("Flowing River Tiles");river.AddComponent<Grid>();var riverMap=new GameObject("Surface");riverMap.transform.SetParent(river.transform);var map=riverMap.AddComponent<Tilemap>();riverMap.AddComponent<TilemapRenderer>().sortingOrder=5;for(int x=0;x<6;x++)map.SetTile(new Vector3Int(x,0,0),tiles[0]);var riverPrefab=Save(river,"FlowingRiver");
  var falls=new GameObject("Animated Waterfall Tiles");falls.AddComponent<Grid>();var fg=new GameObject("Water");fg.transform.SetParent(falls.transform);var fm=fg.AddComponent<Tilemap>();fg.AddComponent<TilemapRenderer>().sortingOrder=4;
  var splashObject=new GameObject("Impact foam");splashObject.transform.SetParent(falls.transform);splashObject.transform.localPosition=new Vector3(0,.5f,0);var splash=splashObject.AddComponent<Tilemap>();splashObject.AddComponent<TilemapRenderer>().sortingOrder=5;
  for(int x=0;x<3;x++){fm.SetTile(new Vector3Int(x,4,0),tiles[1]);for(int y=1;y<4;y++)fm.SetTile(new Vector3Int(x,y,0),tiles[2]);splash.SetTile(new Vector3Int(x,0,0),tiles[3]);}var waterfallPrefab=Save(falls,"Waterfall");
  var bg=Texture("ForestWaterfallLoop");var bs=Sprite(bg,"ForestWaterfall",new Rect(0,0,bg.width,bg.height),new Vector2(.5f,.5f),100);var bgo=new GameObject("Forest and Grand Waterfall Background");var back=bgo.AddComponent<WorldOneBackdrop>();back.scenery=bgo.AddComponent<SpriteRenderer>();back.scenery.sprite=bs;back.scenery.sortingOrder=-100;var backgroundPrefab=Save(bgo,"ForestWaterfallBackground");
  var drowningPrefab=BuildDrowning();
  var hazard=new GameObject("Deep Water Drowning Volume");var hv=hazard.AddComponent<WaterHazard>();hazard.GetComponent<BoxCollider2D>().isTrigger=true;hazard.GetComponent<BoxCollider2D>().size=new Vector2(6,1);hv.drowningPrefab=drowningPrefab.GetComponent<WorldOneSequence>();Save(hazard,"DeepWaterHazard");
  var st=Texture("StoneGoat");var sf=new Sprite[6];for(int i=0;i<6;i++){
   int x=Mathf.RoundToInt(i%3*st.width/3f),right=Mathf.RoundToInt((i%3+1)*st.width/3f),bottom=st.height-(i/3+1)*st.height/2,w=right-x,h=st.height/2;var pixels=st.GetPixels(x,bottom,w,h);int minY=h,minX=w,maxX=0;
   for(int y=0;y<h;y++)for(int px=0;px<w;px++){var c=pixels[y*w+px];if(c.a>.8f&&Mathf.Abs(c.r-c.g)<.07f&&Mathf.Abs(c.g-c.b)<.07f&&c.r>.15f){minY=Mathf.Min(minY,y);minX=Mathf.Min(minX,px);maxX=Mathf.Max(maxX,px);}}
   sf[i]=Sprite(st,"StoneGoat"+i,new Rect(x,bottom,w,h),new Vector2((minX+maxX)*.5f/w,minY/(float)h),w/2f);
  }
  var stone=new GameObject("Stone Goat Checkpoint");var sv=stone.AddComponent<StoneCheckpointVisual>();sv.target=stone.AddComponent<SpriteRenderer>();sv.target.sprite=sf[0];sv.target.sortingOrder=10;sv.frames=sf;stone.AddComponent<RoomCheckpoint>().stoneVisual=sv;var statuePrefab=Save(stone,"StoneGoatCheckpoint");
  var previous=SceneManager.GetActiveScene();var scene=SceneManager.GetSceneByPath(ScenePath);bool loaded=scene.IsValid()&&scene.isLoaded;
  if(loaded&&scene.isDirty)throw new InvalidOperationException("Save the art preview before rebuilding it");
  if(!loaded)scene=File.Exists(ScenePath)?EditorSceneManager.OpenScene(ScenePath,OpenSceneMode.Additive):EditorSceneManager.NewScene(NewSceneSetup.EmptyScene,NewSceneMode.Additive);
  SceneManager.SetActiveScene(scene);var old=scene.GetRootGameObjects().FirstOrDefault(x=>x.name=="World One Art Managed");if(old!=null)Object.DestroyImmediate(old);
  var root=new GameObject("World One Art Managed");var gallery=root.AddComponent<WorldOneArtGallery>();gallery.font=PresentationBuilder.Font;
  var cam=new GameObject("Art Preview Camera");cam.transform.SetParent(root.transform);gallery.view=cam.AddComponent<Camera>();gallery.view.tag="MainCamera";gallery.view.orthographic=true;gallery.view.orthographicSize=5.625f;cam.transform.position=new Vector3(10,5,-10);cam.AddComponent<AudioListener>();
  var display=new GameObject("Exhibits");display.transform.SetParent(root.transform);gallery.exhibits=display.transform;
  var backdrop=Instance(backgroundPrefab,root.transform,Vector2.zero).GetComponent<WorldOneBackdrop>();backdrop.view=gallery.view;
  Instance(waterfallPrefab,display.transform,new Vector2(3,2));Instance(riverPrefab,display.transform,new Vector2(3,1));
  gallery.drowning=Instance(drowningPrefab,display.transform,new Vector2(11,3)).GetComponent<WorldOneSequence>();gallery.drowning.autoPlay=false;
  var goat=new GameObject("Goat preview");goat.transform.SetParent(display.transform);gallery.goat=goat.AddComponent<PlayerVisual>();var goatArt=new GameObject("Goat sprite");goatArt.transform.SetParent(goat.transform);gallery.goat.renderer2D=goatArt.AddComponent<SpriteRenderer>();gallery.goat.renderer2D.sortingOrder=10;gallery.goat.library=AssetDatabase.LoadAssetAtPath<AnimationLibrary>(Root+"/Config/AnimationLibrary.asset");gallery.goat.Pose(new Vector2(11,3),1,0,0,Color.white);
  gallery.statue=Instance(statuePrefab,display.transform,new Vector2(16,3)).GetComponent<StoneCheckpointVisual>();
  Materials(root);EditorSceneManager.SaveScene(scene,ScenePath);EditorBuildSettings.scenes=EditorBuildSettings.scenes.Where(x=>x.path!=ScenePath).Concat(new[]{new EditorBuildSettingsScene(ScenePath,true)}).ToArray();AssetDatabase.SaveAssets();if(!loaded)EditorSceneManager.CloseScene(scene,true);if(previous.IsValid()&&previous.isLoaded)SceneManager.SetActiveScene(previous);
 }
}
}
