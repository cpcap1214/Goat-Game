using System;using System.IO;using System.Linq;using UnityEditor;using UnityEditor.Build.Reporting;using UnityEngine;
namespace OneMoreTime.EditorTools {
public static class WebBuilder {
 [MenuItem("Tools/One More Time/Build Web Demo")]
 public static void Build(){
  if(EditorApplication.isPlaying)throw new Exception("Stop Play before building");
  DemoBuilder.Validate();
  if(!BuildPipeline.IsBuildTargetSupported(BuildTargetGroup.WebGL,BuildTarget.WebGL))throw new Exception("Web Build Support unavailable");
  var compression=PlayerSettings.WebGL.compressionFormat;var width=PlayerSettings.defaultWebScreenWidth;var height=PlayerSettings.defaultWebScreenHeight;
  try{PlayerSettings.WebGL.compressionFormat=WebGLCompressionFormat.Disabled;PlayerSettings.defaultWebScreenWidth=960;PlayerSettings.defaultWebScreenHeight=540;
   var report=BuildPipeline.BuildPlayer(new BuildPlayerOptions{scenes=SceneFlow.Scenes.Concat(new[]{ChapterLayout.Scene}).Select(s=>DemoBuilder.Root+"/Scenes/"+s+".unity").ToArray(),locationPathName="Artifacts/Web",target=BuildTarget.WebGL,options=BuildOptions.None});
   File.WriteAllText("Artifacts/Logs/web-build.txt",$"result={report.summary.result}; errors={report.summary.totalErrors}; warnings={report.summary.totalWarnings}; bytes={report.summary.totalSize}; duration={report.summary.totalTime}; Unity={Application.unityVersion}");
   if(report.summary.result!=BuildResult.Succeeded)throw new Exception("Web build failed; see Logs/Editor.log");
   string shell=File.ReadAllText("Tools/web-shell.html");foreach(var item in new[]{new[]{"__LOADER__","*.loader.js"},new[]{"__DATA__","*.data"},new[]{"__FRAMEWORK__","*.framework.js"},new[]{"__WASM__","*.wasm"}})shell=shell.Replace(item[0],Path.GetFileName(Directory.GetFiles("Artifacts/Web/Build",item[1]).Single()));using(var hash=System.Security.Cryptography.SHA256.Create()){shell=shell.Replace("__VERSION__",BitConverter.ToString(hash.ComputeHash(Directory.GetFiles("Artifacts/Web/Build").OrderBy(p=>p,StringComparer.Ordinal).SelectMany(File.ReadAllBytes).ToArray())).Replace("-","").Substring(0,16));}File.WriteAllText("Artifacts/Web/index.html",shell);
  }finally{PlayerSettings.WebGL.compressionFormat=compression;PlayerSettings.defaultWebScreenWidth=width;PlayerSettings.defaultWebScreenHeight=height;}
 }
}
}
