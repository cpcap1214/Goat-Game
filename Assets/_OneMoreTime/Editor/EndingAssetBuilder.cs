using System.IO;using UnityEditor;using UnityEditor.Build;using UnityEditor.Build.Reporting;using UnityEngine;
namespace OneMoreTime.EditorTools {
public sealed class EndingAssetBuilder:IPreprocessBuildWithReport {
 public int callbackOrder=>0;
 public void OnPreprocessBuild(BuildReport report){Ensure();}
 public static void BuildPreview(){
  Ensure();var compression=PlayerSettings.WebGL.compressionFormat;try{PlayerSettings.WebGL.compressionFormat=WebGLCompressionFormat.Disabled;
  var report=BuildPipeline.BuildPlayer(new BuildPlayerOptions{scenes=new[]{"Assets/_OneMoreTime/Scenes/Ending.unity","Assets/_OneMoreTime/Scenes/MainMenu.unity"},locationPathName="Artifacts/EndingPreview",target=BuildTarget.WebGL,options=BuildOptions.None});
  if(report.summary.result!=UnityEditor.Build.Reporting.BuildResult.Succeeded)throw new BuildFailedException("Ending preview failed");}finally{PlayerSettings.WebGL.compressionFormat=compression;}
 }
 public static void Ensure(){
  const string root="Assets/_OneMoreTime";const string path=root+"/Resources/Ending/EndingAssets.asset";
  var importer=(TextureImporter)AssetImporter.GetAtPath(root+"/Resources/Ending/MagicTower.png");
  if(importer!=null){importer.textureType=TextureImporterType.Default;importer.mipmapEnabled=false;importer.npotScale=TextureImporterNPOTScale.None;importer.textureCompression=TextureImporterCompression.Uncompressed;importer.filterMode=FilterMode.Point;importer.maxTextureSize=2048;importer.SaveAndReimport();}
  var background=AssetDatabase.LoadAssetAtPath<Texture2D>(root+"/Resources/Ending/MagicTower.png");
  var goat=AssetDatabase.LoadAssetAtPath<AnimationLibrary>(root+"/Config/AnimationLibrary.asset");
  if(background==null||goat==null)throw new BuildFailedException("Missing magic tower ending assets");
  var data=AssetDatabase.LoadAssetAtPath<EndingAssets>(path);
  if(data==null){data=ScriptableObject.CreateInstance<EndingAssets>();AssetDatabase.CreateAsset(data,path);}
  data.background=background;data.goat=goat;EditorUtility.SetDirty(data);AssetDatabase.SaveAssets();
 }
}
}
