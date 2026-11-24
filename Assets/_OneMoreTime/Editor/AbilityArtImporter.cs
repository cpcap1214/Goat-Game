using UnityEditor;
using UnityEngine;

namespace OneMoreTime.EditorTools {
// This folder is owned by the camera ability art set. Existing relic imports stay intact.
public sealed class AbilityArtImporter : AssetPostprocessor {
 void OnPreprocessTexture(){
  if(!assetPath.StartsWith("Assets/_OneMoreTime/Resources/AbilityArt/"))return;
  var importer=(TextureImporter)assetImporter;
  importer.textureType=TextureImporterType.Sprite;
  importer.spriteImportMode=SpriteImportMode.Single;
  // Unity scales a Sprite's PPU when maxTextureSize downsamples it. Express the
  // one-unit canvas in source pixels so the imported 64px art stays one unit.
  importer.GetSourceTextureWidthAndHeight(out int sourceWidth,out int sourceHeight);
  importer.spritePixelsPerUnit=Mathf.Max(1,Mathf.Max(sourceWidth,sourceHeight));
  importer.alphaIsTransparency=true;
  importer.mipmapEnabled=false;
  importer.filterMode=FilterMode.Point;
  importer.wrapMode=TextureWrapMode.Clamp;
  importer.textureCompression=TextureImporterCompression.Uncompressed;
  importer.maxTextureSize=64;
  importer.npotScale=TextureImporterNPOTScale.None;
  var settings=new TextureImporterSettings();
  importer.ReadTextureSettings(settings);
  settings.spriteMeshType=SpriteMeshType.FullRect;
  settings.spriteAlignment=(int)SpriteAlignment.Center;
  importer.SetTextureSettings(settings);
 }
}
}
