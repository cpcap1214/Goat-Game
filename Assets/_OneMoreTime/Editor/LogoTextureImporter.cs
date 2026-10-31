using UnityEditor;using UnityEngine;
namespace OneMoreTime.EditorTools {
public sealed class LogoTextureImporter:AssetPostprocessor {
 void OnPreprocessTexture(){if(assetPath!="Assets/_OneMoreTime/Resources/Branding/GoatTowerAgain.png")return;var t=(TextureImporter)assetImporter;t.textureType=TextureImporterType.Default;t.mipmapEnabled=false;t.filterMode=FilterMode.Bilinear;t.wrapMode=TextureWrapMode.Clamp;t.textureCompression=TextureImporterCompression.Uncompressed;t.maxTextureSize=2048;}
}
}
