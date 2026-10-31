using System;using System.IO;using System.Linq;using UnityEditor;using UnityEngine;
namespace OneMoreTime.EditorTools {
public static class GoatArtBuilder {
 public static AnimationLibrary Build(){
  const string image=DemoBuilder.Root+"/Art/Goat/GoatAnimations.png";
  AssetDatabase.ImportAsset(image,ImportAssetOptions.ForceSynchronousImport);
  var importer=(TextureImporter)AssetImporter.GetAtPath(image);importer.textureType=TextureImporterType.Default;importer.isReadable=true;importer.alphaIsTransparency=true;importer.filterMode=FilterMode.Point;importer.mipmapEnabled=false;importer.textureCompression=TextureImporterCompression.Uncompressed;importer.maxTextureSize=2048;importer.SaveAndReimport();
  var texture=AssetDatabase.LoadAssetAtPath<Texture2D>(image);var frames=new Sprite[24];var report=new System.Text.StringBuilder();int cell=texture.width/6;
  for(int i=0;i<24;i++){int x=(i%6)*cell,y=texture.height-(i/6+1)*cell;var pixels=texture.GetPixels(x,y,cell,cell);int minX=cell,minY=cell,maxX=0,maxY=0,opaque=0;
   for(int py=0;py<cell;py++)for(int px=0;px<cell;px++)if(pixels[py*cell+px].a>.6f){minX=Math.Min(minX,px);maxX=Math.Max(maxX,px);minY=Math.Min(minY,py);maxY=Math.Max(maxY,py);opaque++;}
   if(opaque<2000||minX<2||maxX>=cell-2)throw new Exception("Invalid/cropped goat frame "+i);
   string path=DemoBuilder.Root+"/Generated/Goat_"+i.ToString("00")+".asset";var sprite=AssetDatabase.LoadAssetAtPath<Sprite>(path);
   if(sprite==null){sprite=UnityEngine.Sprite.Create(texture,new Rect(x,y,cell,cell),new Vector2((minX+maxX)*.5f/cell,minY/(float)cell),170,0,SpriteMeshType.FullRect);sprite.name="Goat_"+i.ToString("00");AssetDatabase.CreateAsset(sprite,path);}frames[i]=sprite;report.AppendLine($"frame={i}; bounds={minX},{minY}..{maxX},{maxY}; opaque={opaque}; pivot={sprite.pivot}");
  }
  var library=DemoBuilder.Asset<AnimationLibrary>("AnimationLibrary");library.idle=frames.Take(6).ToArray();library.run=frames.Skip(6).Take(6).ToArray();library.rise=frames.Skip(12).Take(3).ToArray();library.descent=frames.Skip(15).Take(3).ToArray();library.landing=frames.Skip(18).Take(6).ToArray();library.jump=frames[13];library.fall=frames[16];library.visualScale=1;library.visualOffset=Vector2.zero;EditorUtility.SetDirty(library);File.WriteAllText("Artifacts/Logs/goat-art-validation.txt",report.ToString());
  string title=DemoBuilder.Root+"/Art/UI/JourneyTitle.png";AssetDatabase.ImportAsset(title,ImportAssetOptions.ForceSynchronousImport);var ti=(TextureImporter)AssetImporter.GetAtPath(title);ti.textureType=TextureImporterType.Default;ti.mipmapEnabled=false;ti.filterMode=FilterMode.Bilinear;ti.maxTextureSize=2048;ti.SaveAndReimport();AssetDatabase.SaveAssets();return library;
 }
 public static void RestoreOriginalRun(){ Build(); }

}
}
