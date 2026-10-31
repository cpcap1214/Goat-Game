using UnityEditor;using UnityEngine;
namespace OneMoreTime.EditorTools {
public sealed class RelicTextureImporter:AssetPostprocessor {
 void OnPreprocessTexture(){if(!assetPath.StartsWith("Assets/_OneMoreTime/Resources/Relics/"))return;var t=(TextureImporter)assetImporter;t.textureType=TextureImporterType.Default;t.alphaIsTransparency=true;t.mipmapEnabled=false;t.filterMode=FilterMode.Point;t.wrapMode=TextureWrapMode.Clamp;t.textureCompression=TextureImporterCompression.Uncompressed;t.maxTextureSize=256;}
}
}
