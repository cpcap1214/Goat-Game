using System.IO;
using UnityEditor;
using UnityEngine;
namespace OneMoreTime.EditorTools {
// Bake a cutout animation from a single model. Frame order is time, never image similarity.
public static class GoatGaitBuilder {
 const int Cell=256;
 public static void Build() {
  string source=DemoBuilder.Root+"/Art/Goat/GoatAnimations.png";
  var original=AssetDatabase.LoadAssetAtPath<Texture2D>(source);
  var src=original.GetPixels(0,original.height-Cell,Cell,Cell);
  var sheet=new Texture2D(Cell*6,Cell*4,TextureFormat.RGBA32,false);
  sheet.SetPixels(new Color[sheet.width*sheet.height]);
  for(int frame=0;frame<24;frame++){
   float phase=frame/24f;int bob=Mathf.RoundToInt(2*(1-Mathf.Cos(phase*4*Mathf.PI)));
   var pixels=new Color[Cell*Cell];
   // Far diagonal pair, then near pair. The same leg artwork keeps volume constant.
   Leg(pixels,src,94,phase+.5f,bob,.68f);
   Leg(pixels,src,171,phase,bob,.68f);
   Leg(pixels,src,70,phase,bob,1);
   Leg(pixels,src,146,phase+.5f,bob,1);
   for(int y=0;y<215;y++)for(int x=0;x<Cell;x++){
    var c=src[(255-y)*Cell+x];if(c.a<.1f)continue;
    int py=y-bob;if(py>=0)Over(pixels,(255-py)*Cell+x,c);
   }
   sheet.SetPixels(frame%6*Cell,(3-frame/6)*Cell,Cell,Cell,pixels);
  }
  sheet.Apply();string path=DemoBuilder.Root+"/Art/Goat/GoatGait24.png";
  File.WriteAllBytes(path,sheet.EncodeToPNG());Object.DestroyImmediate(sheet);
  AssetDatabase.ImportAsset(path,ImportAssetOptions.ForceSynchronousImport);
  var imp=(TextureImporter)AssetImporter.GetAtPath(path);imp.textureType=TextureImporterType.Default;imp.isReadable=true;imp.alphaIsTransparency=true;imp.mipmapEnabled=false;imp.filterMode=FilterMode.Point;imp.textureCompression=TextureImporterCompression.Uncompressed;imp.npotScale=TextureImporterNPOTScale.None;imp.maxTextureSize=2048;imp.SaveAndReimport();
  var texture=AssetDatabase.LoadAssetAtPath<Texture2D>(path);var frames=new Sprite[24];
  for(int i=0;i<24;i++){
   string asset=DemoBuilder.Root+"/Generated/GoatGait24_"+i.ToString("00")+".asset";
   frames[i]=AssetDatabase.LoadAssetAtPath<Sprite>(asset);
   if(frames[i]==null){frames[i]=Sprite.Create(texture,new Rect(i%6*256,(3-i/6)*256,256,256),new Vector2(142f/256,13f/256),170,0,SpriteMeshType.FullRect);frames[i].name="GoatGait24_"+i.ToString("00");AssetDatabase.CreateAsset(frames[i],asset);}
  }
  var lib=AssetDatabase.LoadAssetAtPath<AnimationLibrary>(DemoBuilder.Root+"/Config/AnimationLibrary.asset");lib.run=frames;EditorUtility.SetDirty(lib);AssetDatabase.SaveAssets();
 }
 static void Leg(Color[] dst,Color[] src,float hip,float phase,int bob,float shade){
  var foot=GoatGait.Foot(phase);float top=207-bob,bottom=242-foot.y;
  for(int y=Mathf.FloorToInt(top);y<=Mathf.CeilToInt(bottom);y++){
   float v=Mathf.Clamp01((y-top)/(bottom-top));float center=hip+foot.x*v;
   for(int x=Mathf.FloorToInt(center-14);x<=Mathf.CeilToInt(center+14);x++){
    int sx=Mathf.Clamp(Mathf.RoundToInt(55+(x-center+14)/28f*50),55,105),sy=Mathf.RoundToInt(Mathf.Lerp(207,242,v));
    var c=src[(255-sy)*256+sx];c.r*=shade;c.g*=shade;c.b*=shade;
    if(x>=0&&x<256&&y>=0&&y<256)Over(dst,(255-y)*256+x,c);
   }
  }
 }
 static void Over(Color[] dst,int i,Color c){var old=dst[i];float a=c.a+old.a*(1-c.a);if(a<=0)return;dst[i]=new Color((c.r*c.a+old.r*old.a*(1-c.a))/a,(c.g*c.a+old.g*old.a*(1-c.a))/a,(c.b*c.a+old.b*old.a*(1-c.a))/a,a);}
}
}
