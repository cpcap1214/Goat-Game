#if UNITY_EDITOR
using System.Collections;
using System.Linq;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;
using UnityEngine.TestTools;

namespace OneMoreTime.Tests {
public sealed class WaterFeatureTests {
 const string Root="Assets/_OneMoreTime/";
 const string Prefabs=Root+"Prefabs/WorldOne/WaterKit/";
 const string Generated=Root+"Generated/WorldOne/WaterKit/";
 [Test] public void WaterKitHasNonCollidingForegroundAndDarkBackgroundVersions() {
  string[] names={"GrandWaterfall","BroadWaterfall","NarrowCascade","SplitCascade","ForestRapids","DeepForestPool","DiagonalRapids","WaterfallSpray","ForegroundWaterCurtain","ForegroundNarrowStream","ForegroundDiagonalStream"};
  var atlas=AssetDatabase.LoadAssetAtPath<Texture2D>(Root+"Art/WorldOne/WaterAtlas.png");
  foreach(string name in names)foreach(string suffix in new[]{"","Background"}) {
   var prefab=AssetDatabase.LoadAssetAtPath<GameObject>(Prefabs+name+suffix+".prefab");Assert.NotNull(prefab,name+suffix);
   Assert.IsEmpty(prefab.GetComponentsInChildren<Collider2D>(true),name+" must remain decorative");
   foreach(var water in prefab.GetComponentsInChildren<WaterFeature>(true)) {
    var sr=water.GetComponent<SpriteRenderer>();
    if(name=="GrandWaterfall"&&water.shape==WaterFeature.Shape.Waterfall){Assert.IsTrue(sr.sharedMaterial.mainTexture==atlas||sr.sharedMaterial.mainTexture.name=="GrandWaterfallAtlas");}
    else Assert.AreEqual(atlas,sr.sharedMaterial.mainTexture);
    Assert.AreEqual("OneMoreTime/Water Feature",sr.sharedMaterial.shader.name);
    Assert.AreEqual(4,sr.sharedMaterial.GetFloat("_FramesPerSecond"));
    if(suffix=="Background") {Assert.IsTrue(water.background);Assert.That(sr.color.r,Is.EqualTo(.5f));Assert.LessOrEqual(sr.sortingOrder,-20);}
    else if(name.StartsWith("Foreground")) {Assert.IsTrue(water.foreground);Assert.That(sr.color.a,Is.LessThanOrEqualTo(.38f));Assert.Greater(sr.sortingOrder,10);}
   }
  }
  var giant=AssetDatabase.LoadAssetAtPath<GameObject>(Prefabs+"GrandWaterfall.prefab").GetComponentsInChildren<WaterFeature>();
  Assert.IsTrue(giant.Any(x=>x.shape==WaterFeature.Shape.Waterfall&&x.size==new Vector2(12,22)));
  Assert.IsTrue(giant.Any(x=>x.shape==WaterFeature.Shape.Mist));
 }
 [UnityTest] public IEnumerator WaterFeaturesResizeWithoutChangingPixelsAndCloseTheirAnimationLoop() {
  var instance=Object.Instantiate(AssetDatabase.LoadAssetAtPath<GameObject>(Prefabs+"NarrowCascade.prefab"));
  var feature=instance.GetComponentInChildren<WaterFeature>();var sr=feature.GetComponent<SpriteRenderer>();sr.gameObject.layer=31;
  var material=new Material(sr.sharedMaterial);sr.sharedMaterial=material;
  feature.size=new Vector2(2,2);feature.Refresh();
  Assert.That(sr.bounds.size.x,Is.EqualTo(2).Within(.001));Assert.That(sr.bounds.size.y,Is.EqualTo(2).Within(.001));
  var properties=new MaterialPropertyBlock();sr.GetPropertyBlock(properties);Assert.AreEqual(2,properties.GetVector("_Size").x);
  var cameraObject=new GameObject("Water kit render verification");var camera=cameraObject.AddComponent<Camera>();camera.enabled=false;camera.orthographic=true;camera.orthographicSize=1;camera.aspect=1;camera.transform.position=new Vector3(1,1,-10);camera.cullingMask=1<<31;camera.clearFlags=CameraClearFlags.SolidColor;camera.backgroundColor=Color.clear;
  var target=new RenderTexture(128,128,24,RenderTextureFormat.ARGB32,RenderTextureReadWrite.sRGB);camera.targetTexture=target;var read=new Texture2D(128,128,TextureFormat.RGBA32,false);
  Color32[] Capture(float time) {material.SetFloat("_PreviewTime",time);camera.Render();RenderTexture.active=target;read.ReadPixels(new Rect(0,0,128,128),0,0);read.Apply();RenderTexture.active=null;return read.GetPixels32();}
  foreach(WaterFeature.Shape shape in System.Enum.GetValues(typeof(WaterFeature.Shape))) {
   feature.shape=shape;feature.Refresh();var first=Capture(0);var closure=Capture(1);
   CollectionAssert.AreEqual(first,closure,shape+" must close the same one-second loop as existing water");
   Assert.IsTrue(first.Any(c=>c.a>8),shape+" should render visible pixels");
   Assert.IsFalse(first.SequenceEqual(Capture(.25f)),shape+" should animate");
  }
  feature.shape=WaterFeature.Shape.Waterfall;feature.foreground=true;feature.opacity=1;feature.Refresh();Assert.LessOrEqual(Capture(0).Max(c=>c.a),100,"Foreground water must keep the goat visible");
  // Regression for double-linearizing the palette: a known swatch must survive
  // sRGB texture sampling, nearest-palette selection and render-target encoding.
  var swatch=new Texture2D(64,64,TextureFormat.RGBA32,false);swatch.filterMode=FilterMode.Point;
  var expected=new Color32(101,167,183,255);swatch.SetPixels32(Enumerable.Repeat(expected,64*64).ToArray());swatch.Apply();
  var swatchSprite=Sprite.Create(swatch,new Rect(0,0,64,64),Vector2.zero,64,0,SpriteMeshType.FullRect);
  sr.sprite=swatchSprite;material.mainTexture=swatch;feature.foreground=false;feature.opacity=1;feature.Refresh();
  var actual=Capture(0)[64*128+64];
  Assert.That((int)actual.r,Is.EqualTo((int)expected.r).Within(3),"Water palette red");
  Assert.That((int)actual.g,Is.EqualTo((int)expected.g).Within(3),"Water palette green");
  Assert.That((int)actual.b,Is.EqualTo((int)expected.b).Within(3),"Water palette blue");
  Object.DestroyImmediate(swatchSprite);Object.DestroyImmediate(swatch);
  camera.targetTexture=null;Object.DestroyImmediate(read);Object.DestroyImmediate(target);Object.DestroyImmediate(cameraObject);Object.DestroyImmediate(instance);Object.DestroyImmediate(material);yield return null;
 }
 [Test] public void NewWaterAccentsHaveFourAlignedFramesAndMatchingLayerVariants() {
  foreach(string name in new[]{"ImpactFoam","SurfaceEddy","DripCurtain","FineWaterMist"}) {
   var tile=AssetDatabase.LoadAssetAtPath<WaterAnimationTile>(Generated+name+"Tile.asset");Assert.NotNull(tile,name);Assert.AreEqual(4,tile.frames.Length);Assert.AreEqual(4,tile.framesPerSecond);
   var bounds=tile.frames[0].bounds.size;foreach(var frame in tile.frames) {Assert.AreEqual(bounds,frame.bounds.size);Assert.AreEqual(tile.frames[0].pivot,frame.pivot);}
   foreach(string suffix in new[]{"","Foreground","Background"}) {
    var prefab=AssetDatabase.LoadAssetAtPath<GameObject>(Prefabs+name+suffix+".prefab");Assert.NotNull(prefab);
    var sr=prefab.GetComponent<SpriteRenderer>();Assert.AreEqual("OneMoreTime/Water Accent",sr.sharedMaterial.shader.name);Assert.AreEqual(4,sr.sharedMaterial.GetFloat("_FramesPerSecond"));Assert.IsTrue(sr.sharedMaterial.shader.isSupported);
    if(suffix=="Foreground"){Assert.Greater(sr.sortingOrder,10);Assert.Less(sr.color.a,.4f);}
    if(suffix=="Background"){Assert.Less(sr.sortingOrder,0);Assert.AreEqual(.5f,sr.color.r);}
   }
  }
 }
}
}
#endif
