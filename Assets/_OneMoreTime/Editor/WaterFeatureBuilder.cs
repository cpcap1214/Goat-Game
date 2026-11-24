using System;
using System.IO;
using UnityEditor;
using UnityEngine;
using Object=UnityEngine.Object;

namespace OneMoreTime.EditorTools {
/// <summary>Owns only the WorldOne/WaterKit assets. Safe to rerun without rebuilding rooms.</summary>
public static class WaterFeatureBuilder {
 const string Root="Assets/_OneMoreTime";
 public const string Prefabs=Root+"/Prefabs/WorldOne/WaterKit";
 const string Generated=Root+"/Generated/WorldOne/WaterKit";
 static Sprite quad;
 static Material waterMaterial;
 public static readonly string[] FeatureNames={"GrandWaterfall","BroadWaterfall","NarrowCascade","SplitCascade","ForestRapids","DeepForestPool","DiagonalRapids","WaterfallSpray","ForegroundWaterCurtain","ForegroundNarrowStream","ForegroundDiagonalStream"};

 [MenuItem("Tools/One More Time/Build Water Expansion Kit")]
 public static void Build() {
  if(EditorApplication.isPlaying)throw new InvalidOperationException("Exit Play mode before building water assets");
  Directory.CreateDirectory(Prefabs);Directory.CreateDirectory(Generated);AssetDatabase.Refresh();
  var atlas=AssetDatabase.LoadAssetAtPath<Texture2D>(Root+"/Art/WorldOne/WaterAtlas.png");
  if(atlas==null)throw new InvalidOperationException("Existing WaterAtlas is required");
  quad=SaveSprite(atlas,"WaterFeatureQuad",new Rect(0,0,atlas.width,atlas.height),Vector2.zero,atlas.width);
  waterMaterial=Material("WaterFeature","OneMoreTime/Water Feature",atlas);
  BuildAccents();
  foreach(string name in FeatureNames) {
   var root=new GameObject(name);
   switch(name) {
    case "GrandWaterfall":
     var grand=Part(root,"Broad falling curtain",WaterFeature.Shape.Waterfall,new Vector2(12,22),Vector2.zero);
     var grandAtlas=GrandAtlas();
     if(grandAtlas!=null){var grandRenderer=grand.GetComponent<SpriteRenderer>();grandRenderer.sprite=SaveSprite(grandAtlas,"GrandWaterfallQuad",new Rect(0,0,grandAtlas.width,grandAtlas.height),Vector2.zero,grandAtlas.width);grandRenderer.sharedMaterial=Material("GrandWaterfall","OneMoreTime/Water Feature",grandAtlas);grand.Refresh();}
     Part(root,"Impact spray",WaterFeature.Shape.Mist,new Vector2(14,2),new Vector2(-1,0),.6f, -4);
     for(int i=0;i<6;i++)Accent(root,"ImpactFoam",new Vector2(1+i*2,0));
     for(int i=0;i<3;i++)Accent(root,"FineWaterMist",new Vector2(2+i*4,.5f));
     break;
    case "BroadWaterfall": Part(root,"Water",WaterFeature.Shape.Waterfall,new Vector2(6,12),Vector2.zero);break;
    case "NarrowCascade": Part(root,"Water",WaterFeature.Shape.Waterfall,new Vector2(1.5f,8),Vector2.zero);break;
    case "SplitCascade":
     Part(root,"Left cascade",WaterFeature.Shape.Waterfall,new Vector2(2,8),Vector2.zero);
     Part(root,"Right cascade",WaterFeature.Shape.Waterfall,new Vector2(1,6),new Vector2(3,0));
     Part(root,"Common spray",WaterFeature.Shape.Mist,new Vector2(5,1.5f),new Vector2(-.5f,0),.6f,-4);
     break;
    case "ForestRapids": Part(root,"Water",WaterFeature.Shape.River,new Vector2(8,1.5f),Vector2.zero);break;
    case "DeepForestPool": Part(root,"Water",WaterFeature.Shape.Pool,new Vector2(10,4),Vector2.zero);Accent(root,"SurfaceEddy",new Vector2(3,3.35f));Accent(root,"SurfaceEddy",new Vector2(7,3.35f));break;
    case "DiagonalRapids": Part(root,"Water",WaterFeature.Shape.Diagonal,new Vector2(6,6),Vector2.zero);break;
    case "WaterfallSpray": Part(root,"Spray",WaterFeature.Shape.Mist,new Vector2(6,2),Vector2.zero,.65f,-4);break;
    case "ForegroundWaterCurtain": Part(root,"Transparent curtain",WaterFeature.Shape.Waterfall,new Vector2(4,14),Vector2.zero,.24f,22,true);break;
    case "ForegroundNarrowStream": Part(root,"Transparent ribbon",WaterFeature.Shape.Waterfall,new Vector2(.75f,10),Vector2.zero,.38f,22,true);break;
    case "ForegroundDiagonalStream": Part(root,"Transparent diagonal",WaterFeature.Shape.Diagonal,new Vector2(6,6),Vector2.zero,.32f,22,true);break;
   }
   Save(root,name);
   foreach(var water in root.GetComponentsInChildren<WaterFeature>()) {water.foreground=false;water.background=true;water.Refresh();}
   foreach(var sr in root.GetComponentsInChildren<SpriteRenderer>())if(sr.GetComponent<WaterFeature>()==null){sr.color=new Color(.5f,.5f,.5f,sr.color.a);sr.sortingOrder=-21;}
   Save(root,name+"Background");Object.DestroyImmediate(root);
  }
  AssetDatabase.SaveAssets();
 }
 static Texture2D GrandAtlas() {
  string path=Root+"/Art/WorldOne/Expansion/GrandWaterfallAtlas.png";if(!File.Exists(path))return null;
  var importer=AssetImporter.GetAtPath(path) as TextureImporter;
  if(importer!=null){importer.textureType=TextureImporterType.Default;importer.sRGBTexture=true;importer.alphaIsTransparency=true;importer.filterMode=FilterMode.Point;importer.mipmapEnabled=false;importer.textureCompression=TextureImporterCompression.Uncompressed;importer.maxTextureSize=4096;importer.SaveAndReimport();}
  return AssetDatabase.LoadAssetAtPath<Texture2D>(path);
 }
 static void Accent(GameObject parent,string name,Vector2 position) {
  var prefab=AssetDatabase.LoadAssetAtPath<GameObject>(Prefabs+"/"+name+".prefab");if(prefab==null)return;
  var child=(GameObject)PrefabUtility.InstantiatePrefab(prefab);child.transform.SetParent(parent.transform,false);child.transform.localPosition=position;
 }
 static WaterFeature Part(GameObject root,string name,WaterFeature.Shape shape,Vector2 size,Vector2 position,float opacity=1,int sorting=-5,bool foreground=false) {
  var child=new GameObject(name);child.transform.SetParent(root.transform,false);child.transform.localPosition=position;
  var sr=child.AddComponent<SpriteRenderer>();sr.sprite=quad;sr.sharedMaterial=waterMaterial;
  var feature=child.AddComponent<WaterFeature>();feature.shape=shape;feature.size=size;feature.opacity=opacity;feature.sortingOrder=sorting;feature.foreground=foreground;feature.Refresh();return feature;
 }
 static void Save(GameObject g,string name) {PrefabUtility.SaveAsPrefabAsset(g,Prefabs+"/"+name+".prefab");}
 static Sprite SaveSprite(Texture2D texture,string name,Rect rect,Vector2 pivot,float ppu) {
  var fresh=Sprite.Create(texture,rect,pivot,ppu,0,SpriteMeshType.FullRect);fresh.name=name;
  string path=Generated+"/"+name+".asset";var old=AssetDatabase.LoadAssetAtPath<Sprite>(path);
  if(old==null){AssetDatabase.CreateAsset(fresh,path);return fresh;}
  EditorUtility.CopySerialized(fresh,old);Object.DestroyImmediate(fresh);return old;
 }
 static Material Material(string name,string shaderName,Texture texture) {
  var shader=Shader.Find(shaderName);if(shader==null)throw new InvalidOperationException("Missing shader "+shaderName);
  string path=Generated+"/"+name+".mat";var material=AssetDatabase.LoadAssetAtPath<Material>(path);
  if(material==null){material=new Material(shader);AssetDatabase.CreateAsset(material,path);}material.shader=shader;material.mainTexture=texture;material.SetFloat("_FramesPerSecond",4);material.SetFloat("_PreviewTime",-1);EditorUtility.SetDirty(material);return material;
 }
 public static void BuildAccents() {
  string path=Root+"/Art/WorldOne/Expansion/WaterAccents.png";
  if(!File.Exists(path))return;
  AssetDatabase.ImportAsset(path,ImportAssetOptions.ForceSynchronousImport);
  var importer=(TextureImporter)AssetImporter.GetAtPath(path);importer.textureType=TextureImporterType.Default;importer.isReadable=true;importer.alphaIsTransparency=true;importer.filterMode=FilterMode.Point;importer.mipmapEnabled=false;importer.textureCompression=TextureImporterCompression.Uncompressed;importer.maxTextureSize=4096;importer.SaveAndReimport();
  var texture=AssetDatabase.LoadAssetAtPath<Texture2D>(path);var material=Material("WaterAccents","OneMoreTime/Water Accent",texture);
  float ppu=texture.width/8f;material.SetVector("_PixelGrid",new Vector4(texture.width/ppu*16,texture.height/ppu*16,0,0));EditorUtility.SetDirty(material);
  string[] names={"ImpactFoam","SurfaceEddy","DripCurtain","FineWaterMist"};
  for(int row=0;row<4;row++) {
   var frames=new Sprite[4];for(int col=0;col<4;col++) {
    int x=Mathf.RoundToInt(col*texture.width/4f),right=Mathf.RoundToInt((col+1)*texture.width/4f),bottom=Mathf.RoundToInt((3-row)*texture.height/4f),top=Mathf.RoundToInt((4-row)*texture.height/4f);
    frames[col]=SaveSprite(texture,names[row]+col,new Rect(x,bottom,texture.width/4,texture.height/4),new Vector2(.5f,0),ppu);
   }
   string tilePath=Generated+"/"+names[row]+"Tile.asset";var tile=AssetDatabase.LoadAssetAtPath<WaterAnimationTile>(tilePath);
   if(tile==null){tile=ScriptableObject.CreateInstance<WaterAnimationTile>();AssetDatabase.CreateAsset(tile,tilePath);}tile.frames=frames;tile.framesPerSecond=4;EditorUtility.SetDirty(tile);
   var g=new GameObject(names[row]);var sr=g.AddComponent<SpriteRenderer>();sr.sprite=frames[0];sr.sharedMaterial=material;sr.sortingOrder=6;
   Save(g,names[row]);sr.sortingOrder=23;sr.color=new Color(1,1,1,.28f);Save(g,names[row]+"Foreground");sr.sortingOrder=-21;sr.color=new Color(.5f,.5f,.5f,1);Save(g,names[row]+"Background");Object.DestroyImmediate(g);
  }
 }
}
}
