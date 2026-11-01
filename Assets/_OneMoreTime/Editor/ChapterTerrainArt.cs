using System.IO;using UnityEditor;using UnityEngine;
namespace OneMoreTime.EditorTools {
// One world-aligned cutaway per connected land mass, without a repeating square tile border.
public static class ChapterTerrainArt {
 const string Folder=DemoBuilder.Root+"/Art/ChapterOne/Terrain";const int Ppu=16;
 public static void Apply(GameObject parent,Rect rect,int index){
  Directory.CreateDirectory(Folder);int width=Mathf.CeilToInt(rect.width*Ppu),height=Mathf.CeilToInt(rect.height*Ppu);
  var image=new Texture2D(width,height,TextureFormat.RGBA32,false);var pixels=new Color32[width*height];
  Color[] earth={Hex(0x352f2b),Hex(0x41372e),Hex(0x514334),Hex(0x5d4d39),Hex(0x705e43),Hex(0x82704e)};
  for(int y=0;y<height;y++)for(int x=0;x<width;x++){
   float wx=rect.xMin+x/(float)Ppu,wy=rect.yMin+y/(float)Ppu,depth=rect.yMax-wy;
   float wave=Mathf.Sin(wx*.43f)*.23f+Mathf.Sin(wx*1.13f)*.10f;
   float layer=Mathf.Repeat(wy+wave,2.7f);float grain=Mathf.PerlinNoise(wx*.42f,wy*.65f);
   int shade=layer<.12f?1:layer<.25f?4:layer<1.2f?3:2;
   Color c=Color.Lerp(earth[shade],earth[Mathf.Min(shade+1,5)],grain*.28f);
   if(Mathf.PerlinNoise(wx*1.6f,wy*1.9f)>.73f)c*=.83f;
   // Irregular roots and grass fringe share world X across adjoining pieces.
   float root=.26f+.13f*Mathf.Sin(wx*2.8f)+.08f*Mathf.Sin(wx*6.3f);
   if(depth<.08f)c=Hex(0xadc775);
   else if(depth<.20f)c=Hex(0x7c9f50);
   else if(depth<root+.14f)c=Hex(0x506b3e);
   else if(depth<root+.22f)c=Hex(0x303f31);
   float vein=Mathf.PerlinNoise(wx*.16f,wy*.24f);
   if(depth>1&&vein>.62f&&grain>.55f)c=Color.Lerp(c,Hex(0x8c805f),.28f);
   c.a=1;pixels[y*width+x]=c;
  }
  image.SetPixels32(pixels);image.Apply();string path=Folder+"/Stratum"+index.ToString("00")+".png";File.WriteAllBytes(path,image.EncodeToPNG());Object.DestroyImmediate(image);
  AssetDatabase.ImportAsset(path);var importer=(TextureImporter)AssetImporter.GetAtPath(path);importer.textureType=TextureImporterType.Sprite;importer.spriteImportMode=SpriteImportMode.Single;importer.spritePixelsPerUnit=Ppu;importer.filterMode=FilterMode.Point;importer.mipmapEnabled=false;importer.textureCompression=TextureImporterCompression.Uncompressed;importer.maxTextureSize=2048;importer.SaveAndReimport();
  var renderer=parent.AddComponent<SpriteRenderer>();renderer.sprite=AssetDatabase.LoadAssetAtPath<Sprite>(path);renderer.sortingOrder=0;
 }
 static Color Hex(int rgb)=>new Color(((rgb>>16)&255)/255f,((rgb>>8)&255)/255f,(rgb&255)/255f);
}
}
