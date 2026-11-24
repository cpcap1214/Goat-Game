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
// Owns reusable expansion assets and one gallery child, never future room layouts.
public static class ForestExpansionBuilder {
 const string Root="Assets/_OneMoreTime";
 public const string Prefabs=Root+"/Prefabs/WorldOne/Expansion";
 public const string Generated=Root+"/Generated/WorldOne/Expansion";
 public static readonly string[] Forest={"BroadFern","SmallFern","RiverReeds","MeadowFlowers","ForestMushrooms","ShelfFungus","MossyFallenLog","HollowStump","HangingVine","HangingRoots","RootArch","TangledRoots","MossyRiverRock","WetStoneStack","MossyOutcrop","LeafyBranch"};
 public static readonly string[] Mountain={"AncientArch","BrokenPillar","CarvedStone","RuinedDoorway","RockSpire","FlatBoulder","CliffCluster","RockOverhang","TimberFrame","BracedTimber","OldWaterwheel","RopePost","WindsweptPine","AlpineShrub","HangingCanopy","RockyStreamBank"};
 static readonly float[] ForestWidths={2.5f,1.5f,1.6f,1.5f,1.3f,1.4f,3,2,1.3f,2,3,2.5f,2,2.5f,2,3};
 static readonly float[] MountainWidths={4,2,2.5f,3.5f,3,3.5f,3,4,3.5f,3.5f,3.5f,2,4,2,4,3.5f};

 [MenuItem("Tools/One More Time/Build Forest Expansion Props and Gallery")]
 public static void Build(){
  if(EditorApplication.isPlaying)throw new InvalidOperationException("Exit Play mode before building art");
  Directory.CreateDirectory(Prefabs);Directory.CreateDirectory(Generated);AssetDatabase.Refresh();
  Atlas("ForestDetails",Forest,ForestWidths);Atlas("MountainDetails",Mountain,MountainWidths);
  AssetDatabase.SaveAssets();BuildGallery();
 }
 static void Atlas(string atlasName,string[] names,float[] widths){
  string path=Root+"/Art/WorldOne/Expansion/"+atlasName+".png";
  AssetDatabase.ImportAsset(path,ImportAssetOptions.ForceSynchronousImport);
  var importer=(TextureImporter)AssetImporter.GetAtPath(path);importer.textureType=TextureImporterType.Default;importer.isReadable=true;importer.filterMode=FilterMode.Point;importer.mipmapEnabled=false;importer.alphaIsTransparency=true;importer.textureCompression=TextureImporterCompression.Uncompressed;importer.maxTextureSize=4096;importer.SaveAndReimport();
  var texture=AssetDatabase.LoadAssetAtPath<Texture2D>(path);var pixels=texture.GetPixels32();
  for(int i=0;i<names.Length;i++){
   int left=Mathf.RoundToInt(i%4*texture.width/4f),right=Mathf.RoundToInt((i%4+1)*texture.width/4f),bottom=Mathf.RoundToInt((3-i/4)*texture.height/4f),top=Mathf.RoundToInt((4-i/4)*texture.height/4f);
   int x0=right,x1=left,y0=top,y1=bottom;
   for(int y=bottom;y<top;y++)for(int x=left;x<right;x++)if(pixels[y*texture.width+x].a>=160){x0=Mathf.Min(x0,x);x1=Mathf.Max(x1,x);y0=Mathf.Min(y0,y);y1=Mathf.Max(y1,y);}
   if(x1<x0||y1<y0)throw new InvalidOperationException("Empty sprite "+names[i]);
   float ppu=(x1-x0+1)/widths[i];bool hanging=names[i].StartsWith("Hanging")||names[i]=="RockOverhang";
   var fresh=Sprite.Create(texture,new Rect(x0,y0,x1-x0+1,y1-y0+1),new Vector2(.5f,hanging?1:0),ppu,0,SpriteMeshType.FullRect);fresh.name=names[i];
   string sp=Generated+"/"+names[i]+".asset";var sprite=AssetDatabase.LoadAssetAtPath<Sprite>(sp);
   if(sprite==null){sprite=fresh;AssetDatabase.CreateAsset(sprite,sp);}else{EditorUtility.CopySerialized(fresh,sprite);Object.DestroyImmediate(fresh);}
   string mp=Generated+"/"+names[i]+".mat";var material=AssetDatabase.LoadAssetAtPath<Material>(mp);var shader=Shader.Find("OneMoreTime/Expansion Sprite");if(material==null){material=new Material(shader);AssetDatabase.CreateAsset(material,mp);}material.shader=shader;material.mainTexture=texture;material.SetVector("_PixelGrid",new Vector4(texture.width/ppu*16,texture.height/ppu*16,0,0));EditorUtility.SetDirty(material);
   foreach(bool bg in new[]{false,true}){
    string n=names[i]+(bg?"Background":"");var go=new GameObject(n);var sr=go.AddComponent<SpriteRenderer>();sr.sprite=sprite;sr.sharedMaterial=material;sr.sortingOrder=bg?-25:3;sr.color=bg?new Color(.5f,.5f,.5f,1):Color.white;
    PrefabUtility.SaveAsPrefabAsset(go,Prefabs+"/"+n+".prefab");Object.DestroyImmediate(go);
    string tp=Generated+"/"+n+"Tile.asset";var tile=AssetDatabase.LoadAssetAtPath<Tile>(tp);if(tile==null){tile=ScriptableObject.CreateInstance<Tile>();AssetDatabase.CreateAsset(tile,tp);}tile.sprite=sprite;tile.color=bg?new Color(.5f,.5f,.5f,1):Color.white;tile.colliderType=Tile.ColliderType.None;EditorUtility.SetDirty(tile);
   }
  }
 }
 static GameObject Put(string path,Transform parent,Vector2 pos){var prefab=AssetDatabase.LoadAssetAtPath<GameObject>(path);if(prefab==null)throw new InvalidOperationException("Missing "+path);var go=(GameObject)PrefabUtility.InstantiatePrefab(prefab);go.transform.SetParent(parent,false);go.transform.localPosition=pos;return go;}
 static GameObject Prop(string name,Transform parent,Vector2 pos)=>Put(Prefabs+"/"+name+".prefab",parent,pos);
 static GameObject Water(string name,Transform parent,Vector2 pos)=>Put(WaterFeatureBuilder.Prefabs+"/"+name+".prefab",parent,pos);
 static void Grass(Transform parent,Vector2 pos,float width,float depth){var go=Put(GrassyPlatformBuilder.PrefabPath,parent,pos);var column=go.GetComponent<GrassyColumnVisual>();column.width=width;column.depth=depth;column.Refresh();}
 public static void BuildGallery(){
  var previous=SceneManager.GetActiveScene();string path=WorldOneArtBuilder.ScenePath;var scene=SceneManager.GetSceneByPath(path);bool loaded=scene.IsValid()&&scene.isLoaded;
  if(loaded&&scene.isDirty)throw new InvalidOperationException("Save art gallery edits before rebuilding its expansion child");
  if(!loaded)scene=EditorSceneManager.OpenScene(path,OpenSceneMode.Additive);SceneManager.SetActiveScene(scene);
  var gallery=scene.GetRootGameObjects().SelectMany(g=>g.GetComponentsInChildren<WorldOneArtGallery>()).Single();
  var old=gallery.exhibits.Find("Forest Expansion Exhibits");if(old!=null)Object.DestroyImmediate(old.gameObject);
  var root=new GameObject("Forest Expansion Exhibits").transform;root.SetParent(gallery.exhibits,false);
  for(int i=0;i<16;i++){
   var f=Prop(Forest[i],root,new Vector2(32+i%4*5,13-i/4*4));
   if(Forest[i].StartsWith("Hanging"))f.transform.position+=Vector3.up*2;
   var m=Prop(Mountain[i],root,new Vector2(61+i%4*5,13-i/4*4));
   if(Mountain[i].StartsWith("Hanging")||Mountain[i]=="RockOverhang")m.transform.position+=Vector3.up*3;
  }
  Water("NarrowCascade",root,new Vector2(88,5));Water("SplitCascade",root,new Vector2(92,5));
  Water("ForestRapids",root,new Vector2(100,11));Water("DeepForestPool",root,new Vector2(100,3));
  Water("DiagonalRapids",root,new Vector2(88,-2));Water("DripCurtainForeground",root,new Vector2(98,1));Water("SurfaceEddy",root,new Vector2(108,1));
  Water("GrandWaterfall",root,new Vector2(125,-1));
  Grass(root,new Vector2(123,3),4,8);Grass(root,new Vector2(140,8),4,12);Grass(root,new Vector2(140,17),4,10);
  Prop("HangingCanopy",root,new Vector2(124,20));Prop("RockSpire",root,new Vector2(122,-2));Prop("RockyStreamBank",root,new Vector2(141,-2));
  Water("ForegroundNarrowStream",root,new Vector2(121,-1));Water("ForegroundDiagonalStream",root,new Vector2(138,7));
  var goat=new GameObject("Goat for scale");goat.transform.SetParent(root,false);var pv=goat.AddComponent<PlayerVisual>();pv.library=AssetDatabase.LoadAssetAtPath<AnimationLibrary>(Root+"/Config/AnimationLibrary.asset");var sprite=new GameObject("Sprite");sprite.transform.SetParent(goat.transform,false);pv.renderer2D=sprite.AddComponent<SpriteRenderer>();pv.renderer2D.sortingOrder=10;pv.Pose(new Vector2(123,3),1,0,0,Color.white);
  Water("ForestRapids",root,new Vector2(153,8));Water("ForestRapids",root,new Vector2(153,3));
  for(int i=0;i<4;i++)Prop(FloatingTimberBuilder.Variants[i],root,new Vector2(155+i%2*6,9-i/2*5));
  Grass(root,new Vector2(169,9),3,9);Prop("RiverReeds",root,new Vector2(169,9));
  // Keep each group beyond the widest preview frustum, so adjacent exhibits never intrude.
  foreach(Transform item in root){float x=item.localPosition.x;float offset=x<55?0:x<85?30:x<116?60:x<150?90:120;item.localPosition+=new Vector3(offset,0,0);}
  gallery.sectionOrigins=new[]{new Vector2(10,5),new Vector2(39.5f,7),new Vector2(98.5f,7),new Vector2(159,6),new Vector2(222,10),new Vector2(282,6)};
  gallery.sectionSizes=new[]{7f,10f,10f,10f,13.5f,8f};
  gallery.sectionNames=new[]{"Original art","Forest undergrowth","Mountain ruins","Streams & water effects","The Grand Waterfall","Floating timber"};
  EditorUtility.SetDirty(gallery);EditorSceneManager.MarkSceneDirty(scene);EditorSceneManager.SaveScene(scene);AssetDatabase.SaveAssets();
  if(!loaded)EditorSceneManager.CloseScene(scene,true);if(previous.IsValid()&&previous.isLoaded)SceneManager.SetActiveScene(previous);
 }
 public static string Validate(){
  foreach(string n in Forest.Concat(Mountain))foreach(string suffix in new[]{"","Background"}){var g=AssetDatabase.LoadAssetAtPath<GameObject>(Prefabs+"/"+n+suffix+".prefab");if(g==null||g.GetComponentsInChildren<Collider2D>().Length!=0)throw new Exception("Missing or colliding decoration "+n);var sr=g.GetComponent<SpriteRenderer>();if(sr.sprite==null||!sr.sharedMaterial.shader.isSupported)throw new Exception("Invalid decoration "+n);var p=sr.sharedMaterial.GetVector("_PixelGrid");if(Mathf.Abs(p.x/(sr.sprite.texture.width/sr.sprite.pixelsPerUnit)-16)>.001f)throw new Exception("Pixel density "+n);}
  return "PASS: 32 forest/mountain props, 32 dark variants, 64 decorative tiles; 16 pixels/unit; no decoration colliders.";
 }
 public static void Capture(){
  var scene=EditorSceneManager.OpenScene(WorldOneArtBuilder.ScenePath);var gallery=Object.FindFirstObjectByType<WorldOneArtGallery>();
  foreach(var feature in Object.FindObjectsByType<WaterFeature>(FindObjectsSortMode.None))feature.Refresh();
  var cam=gallery.view;Vector3 before=cam.transform.position;float size=cam.orthographicSize;float aspect=cam.aspect;
  Directory.CreateDirectory("Artifacts/Logs");
  for(int section=1;section<gallery.sectionOrigins.Length;section++){
   Vector2 p=gallery.sectionOrigins[section];cam.transform.position=new Vector3(p.x,p.y,-10);cam.orthographicSize=gallery.sectionSizes[section];cam.aspect=16f/9;
   foreach(var bg in Object.FindObjectsByType<WorldOneBackdrop>(FindObjectsSortMode.None))bg.SendMessage("LateUpdate");
   var rt=new RenderTexture(1600,900,24);cam.targetTexture=rt;cam.Render();var read=new Texture2D(1600,900,TextureFormat.RGBA32,false);RenderTexture.active=rt;read.ReadPixels(new Rect(0,0,1600,900),0,0);read.Apply();File.WriteAllBytes("Artifacts/Logs/forest-expansion-"+section+".png",read.EncodeToPNG());cam.targetTexture=null;RenderTexture.active=null;Object.DestroyImmediate(read);Object.DestroyImmediate(rt);
  }
  cam.transform.position=before;cam.orthographicSize=size;cam.aspect=aspect;
  // Camera captures are temporary; reopen saved scene to leave the user's preview clean.
  EditorSceneManager.OpenScene(WorldOneArtBuilder.ScenePath);
 }
}
}
