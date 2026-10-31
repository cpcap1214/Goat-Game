using System;using System.Linq;using UnityEditor;using UnityEditor.SceneManagement;using UnityEngine;
namespace OneMoreTime.EditorTools {
public static class SpikeArtBuilder {
 public static Sprite EnsureSprite(){
  const string path=DemoBuilder.Root+"/Art/Hazards/SteelSpike.png";
  AssetDatabase.ImportAsset(path,ImportAssetOptions.ForceSynchronousImport);
  var im=(TextureImporter)AssetImporter.GetAtPath(path);im.textureType=TextureImporterType.Default;im.isReadable=true;im.alphaIsTransparency=true;im.mipmapEnabled=false;im.filterMode=FilterMode.Point;im.textureCompression=TextureImporterCompression.Uncompressed;im.maxTextureSize=2048;im.SaveAndReimport();
  var t=AssetDatabase.LoadAssetAtPath<Texture2D>(path);var p=t.GetPixels32();int x0=t.width,y0=t.height,x1=0,y1=0;
  for(int y=0;y<t.height;y++)for(int x=0;x<t.width;x++)if(p[y*t.width+x].a>128){x0=Math.Min(x0,x);y0=Math.Min(y0,y);x1=Math.Max(x1,x);y1=Math.Max(y1,y);}
  if(x0>=x1||y0>=y1)throw new Exception("Empty spike artwork");
  string asset=DemoBuilder.Root+"/Generated/SteelSpike.asset";var sprite=AssetDatabase.LoadAssetAtPath<Sprite>(asset);
  if(sprite==null){sprite=Sprite.Create(t,new Rect(x0,y0,x1-x0+1,y1-y0+1),Vector2.one*.5f,100,0,SpriteMeshType.FullRect);sprite.name="SteelSpike";AssetDatabase.CreateAsset(sprite,asset);}
  return sprite;
 }
 public static void Create(Transform parent,Rect rect,Sprite sprite){
  var row=new GameObject("Hazard");row.transform.SetParent(parent);row.transform.position=rect.center;
  // Pit spikes need a visible earth foundation down to the adjoining cliff bottom.
  // Raised L04 spikes already sit on terrain; do not fill the passage below them.
  if(rect.yMin<0){
   var foundation=new GameObject("Spike earth foundation");foundation.transform.SetParent(row.transform);
   var block=new Rect(rect.xMin,rect.yMin-1,rect.width,1);foundation.transform.position=block.center;
   PresentationBuilder.TerrainSkin(foundation,block,"Hazard foundation");
   foundation.GetComponent<SpriteRenderer>().sortingOrder=2;
  }
  int count=Mathf.Max(1,Mathf.CeilToInt(rect.width/(rect.height*1.05f)));float width=rect.width/count;
  for(int i=0;i<count;i++){
   var g=new GameObject("Steel spike");g.transform.SetParent(row.transform,false);g.transform.localPosition=new Vector3(-rect.width*.5f+width*(i+.5f),0,0);
   g.transform.localScale=new Vector3(width/sprite.bounds.size.x,rect.height/sprite.bounds.size.y,1);
   var sr=g.AddComponent<SpriteRenderer>();sr.sprite=sprite;sr.color=Color.white;sr.sortingOrder=6;
  }
 }
 public static void BuildPreview(){
  var compression=PlayerSettings.WebGL.compressionFormat;
  try{PlayerSettings.WebGL.compressionFormat=WebGLCompressionFormat.Disabled;
   var report=BuildPipeline.BuildPlayer(new BuildPlayerOptions{scenes=new[]{DemoBuilder.Root+"/Scenes/L04_Timing.unity"},locationPathName="Artifacts/SpikePreview",target=BuildTarget.WebGL,options=BuildOptions.None});
   if(report.summary.result!=UnityEditor.Build.Reporting.BuildResult.Succeeded)throw new Exception("Spike preview build failed");
  }finally{PlayerSettings.WebGL.compressionFormat=compression;}
 }
 public static void UpdateScenes(){
  var sprite=EnsureSprite();int count=0;
  foreach(var name in new[]{"L01_Freeze","L02_Pressure","L03_Trace","L04_Timing"}){
   string path=DemoBuilder.Root+"/Scenes/"+name+".unity";DemoBuilder.BackupScene(path);
   var previous=UnityEngine.SceneManagement.SceneManager.GetActiveScene();var scene=UnityEngine.SceneManagement.SceneManager.GetSceneByPath(path);bool wasLoaded=scene.IsValid()&&scene.isLoaded;if(!wasLoaded)scene=EditorSceneManager.OpenScene(path,OpenSceneMode.Additive);var run=scene.GetRootGameObjects().SelectMany(g=>g.GetComponentsInChildren<RunManager>()).Single();
   foreach(var child in run.transform.Cast<Transform>().Where(t=>t.name=="Hazard").ToArray())UnityEngine.Object.DestroyImmediate(child.gameObject);
   foreach(var r in run.config.hazards){Create(run.transform,r,sprite);count++;}
   EditorSceneManager.SaveScene(scene);if(!wasLoaded)EditorSceneManager.CloseScene(scene,true);UnityEngine.SceneManagement.SceneManager.SetActiveScene(previous);
  }
  AssetDatabase.SaveAssets();System.IO.File.WriteAllText("Artifacts/Logs/spikes.txt","Updated "+count+" hazard rows; original LevelConfig collision rectangles preserved.");
 }
}
}
