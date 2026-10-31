using System;using System.IO;using UnityEngine;using UnityEditor;
namespace OneMoreTime.EditorTools {
// Compose the existing Sunny Land tiles along the union silhouette, not every collider.
public static class TerrainArtBuilder {
 const int Ppu=16;const string Root=DemoBuilder.Root;
 public static void Build(Transform parent,LevelConfig config){
  var rects=config.terrain;float minX=float.MaxValue,minY=float.MaxValue,maxX=float.MinValue,maxY=float.MinValue;
  foreach(var r in rects){minX=Mathf.Min(minX,r.xMin);minY=Mathf.Min(minY,r.yMin);maxX=Mathf.Max(maxX,r.xMax);maxY=Mathf.Max(maxY,r.yMax);}
  int ox=Mathf.FloorToInt(minX*Ppu),oy=Mathf.FloorToInt(minY*Ppu),w=Mathf.CeilToInt(maxX*Ppu)-ox,h=Mathf.CeilToInt(maxY*Ppu)-oy;
  var occupied=new bool[w*h];foreach(var r in rects)for(int y=Mathf.RoundToInt(r.yMin*Ppu)-oy;y<Mathf.RoundToInt(r.yMax*Ppu)-oy;y++)for(int x=Mathf.RoundToInt(r.xMin*Ppu)-ox;x<Mathf.RoundToInt(r.xMax*Ppu)-ox;x++)occupied[y*w+x]=true;
  bool Solid(int x,int y)=>x>=0&&y>=0&&x<w&&y<h&&occupied[y*w+x];
  var atlas=new Texture2D(2,2);atlas.LoadImage(File.ReadAllBytes("Assets/SunnyLand Artwork/Environment/Tileset/tileset-sliced.png"));
  Color Sample(int tx,int ty,int x,int depth)=>atlas.GetPixel(tx+x,atlas.height-1-ty-depth);
  var pixels=new Color[w*h];var earth=new Color32(105,63,85,255);
  for(int y=0;y<h;y++)for(int x=0;x<w;x++){
   if(!Solid(x,y))continue;int up=0,left=0,right=0,down=0;
   while(up<16&&Solid(x,y+up+1))up++;while(left<16&&Solid(x-left-1,y))left++;while(right<16&&Solid(x+right+1,y))right++;while(down<16&&Solid(x,y-down-1))down++;
   Color c=earth;int gx=x+ox,gy=y+oy,px=((gx%16)+16)%16,py=15-((gy%16)+16)%16;
   // Only exposed boundaries receive the rocky tiles. Connected colliders share one face.
   if(left<12)c=Sample(16,48,left,py);else if(right<12)c=Sample(16,48,right,py);
   if(down<5)c=Sample(48,80,px,15-down);
   if(up<16)c=Sample(48,16,px,up);
   // Sparse original embedded-stone motifs, separated by broad untextured soil.
   if(up==16&&left==16&&right==16&&down==16){int cx=Mathf.FloorToInt(gx/16f),cy=Mathf.FloorToInt(gy/16f);uint hash=unchecked((uint)(cx*7385609)^((uint)cy*19349663u));hash^=hash>>16;hash*=2246822519u;hash^=hash>>13;if(hash%19==0)c=Sample(80,112,px,py);}
   pixels[y*w+x]=c;
  }
  string path=Root+"/Generated/TerrainLandscape"+config.level+".asset";var texture=AssetDatabase.LoadAssetAtPath<Texture2D>(path);
  if(texture==null){texture=new Texture2D(w,h,TextureFormat.RGBA32,false);AssetDatabase.CreateAsset(texture,path);}else if(texture.width!=w||texture.height!=h)texture.Reinitialize(w,h,TextureFormat.RGBA32,false);
  texture.SetPixels(pixels);texture.filterMode=FilterMode.Point;texture.wrapMode=TextureWrapMode.Clamp;texture.Apply();EditorUtility.SetDirty(texture);
  string sp=Root+"/Generated/TerrainLandscapeSprite"+config.level+".asset";var sprite=AssetDatabase.LoadAssetAtPath<Sprite>(sp);
  // Rebuild native rect, UVs and mesh when the texture dimensions change.
  // Copy into the existing asset so scene/prefab references and its GUID stay valid.
  if(sprite==null||sprite.rect!=new Rect(0,0,w,h)||sprite.texture!=texture){
   var updated=Sprite.Create(texture,new Rect(0,0,w,h),Vector2.zero,Ppu,0,SpriteMeshType.FullRect);
   if(sprite==null){sprite=updated;AssetDatabase.CreateAsset(sprite,sp);}
   else{updated.name=sprite.name;EditorUtility.CopySerialized(updated,sprite);EditorUtility.SetDirty(sprite);UnityEngine.Object.DestroyImmediate(updated);}
  }
  var go=new GameObject("Continuous landscape");go.transform.SetParent(parent);go.transform.position=new Vector2(ox/(float)Ppu,oy/(float)Ppu);go.AddComponent<SpriteRenderer>().sprite=sprite;
  UnityEngine.Object.DestroyImmediate(atlas);
 }
}
}
