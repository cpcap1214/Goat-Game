#if UNITY_EDITOR
using System.Collections;
using System.Linq;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;
using UnityEngine.TestTools;

namespace OneMoreTime.Tests {
public sealed class ChapterTwoObscurityTests {
 [UnityTest] public IEnumerator RenderedBeamClearsOnlyItsAreaAndMistUsesTheManualClock() {
  var shader = AssetDatabase.LoadAssetAtPath<Shader>("Assets/_OneMoreTime/Shaders/ChapterTwoObscurity.shader");
  Assert.NotNull(shader); Assert.IsTrue(shader.isSupported);
  GameObject lightObject = null, volumeObject = null, cameraObject = null;
  RenderTexture target = null; Texture2D read = null;
  try {
   lightObject = new GameObject("Flashlight visual test");
   var light = lightObject.AddComponent<FlashlightAbility>(); light.unlocked = true;
   volumeObject = new GameObject("Local mist test"); volumeObject.layer = 31;
   var volume = volumeObject.AddComponent<ChapterTwoObscurity>();
   volume.shader = shader; volume.flashlight = light; volume.kind = ChapterTwoObscurity.Kind.Mist;
   volume.size = new Vector2(20, 12); volume.opacity = .8f; volume.tint = new Color(.36f,.4f,.42f,1);
   volume.RefreshVisual();
   Assert.IsEmpty(volume.GetComponents<Collider2D>(), "Obscurity must not change platform collision");
   Assert.AreEqual(60, volume.GetComponent<SpriteRenderer>().sortingOrder);

   cameraObject = new GameObject("Obscurity render verification");
   var camera = cameraObject.AddComponent<Camera>(); camera.enabled = false;
   camera.orthographic = true; camera.orthographicSize = 6; camera.aspect = 20f/12;
   camera.transform.position = new Vector3(0,0,-10); camera.cullingMask = 1<<31;
   camera.clearFlags = CameraClearFlags.SolidColor; camera.backgroundColor = Color.clear;
   target = new RenderTexture(320,192,24,RenderTextureFormat.ARGB32,RenderTextureReadWrite.sRGB);
   camera.targetTexture = target; read = new Texture2D(320,192,TextureFormat.RGBA32,false);
   Color32[] Capture() {
    camera.Render(); var previous = RenderTexture.active; RenderTexture.active = target;
    read.ReadPixels(new Rect(0,0,320,192),0,0); read.Apply(); RenderTexture.active = previous;
    return read.GetPixels32();
   }
   int Pixel(float x,float y) => Mathf.FloorToInt((y+6)*16)*320+Mathf.FloorToInt((x+10)*16);
   void MatchesQuery(Color32[] frame, Vector2 point) {
    int index = Pixel(point.x,point.y);
    Vector2 pixelCenter = new Vector2((index%320+.5f)/16-10,(index/320+.5f)/16-6);
    Assert.That((int)frame[index].a,Is.EqualTo(Mathf.RoundToInt(volume.ObscurityAt(pixelCenter)*255)).Within(3),"CPU visibility query must match rendered coverage at "+point);
   }
   var dark = Capture();
   MatchesQuery(dark,new Vector2(3,0));
   var interiorAlphas = dark.Where((pixel,index)=>index%320>=32&&index%320<288&&index/320>=32&&index/320<160).Select(pixel=>(int)pixel.a).ToArray();
   Assert.Greater(interiorAlphas.Max()-interiorAlphas.Min(),70,"Unlit mist needs visible translucent ribbons, not a flat rectangular sheet");
   Assert.IsTrue(light.TryActivate()); light.Tick(.2f); volume.RefreshVisual();
   var lit = Capture();
   Assert.Less(lit[Pixel(3,0)].a,5,"The facing cone must reveal the actual scene");
   Assert.Less(lit[Pixel(0,0)].a,5,"Near light keeps the goat readable");
   Assert.Greater(lit[Pixel(-3,0)].a,40,"Mist remains behind the goat");
   Assert.Greater(lit[Pixel(7,0)].a,40,"Mist beyond beam range remains");
   Assert.Greater(lit[Pixel(3,3)].a,40,"Clearing must be a cone, not the whole volume");
   foreach(var point in new[]{new Vector2(3,0),new Vector2(-3,0),new Vector2(7,0),new Vector2(3,3),new Vector2(5.5f,0),new Vector2(3,1.75f),new Vector2(9.8f,0)}) MatchesQuery(lit,point);
   yield return null;
   CollectionAssert.AreEqual(lit,Capture(),"No simulation tick means no mist movement, including while paused");
   light.Tick(.8f); volume.RefreshVisual();
   Assert.IsFalse(lit.SequenceEqual(Capture()),"Mist should drift when its room clock advances");
   var material = volume.GetComponent<SpriteRenderer>().sharedMaterial;
   material.SetFloat("_BeamFacing",-1);
   var reversed = Capture();
   Assert.Less(reversed[Pixel(-3,0)].a,5); Assert.Greater(reversed[Pixel(3,0)].a,40);
   light.Tick(4); volume.RefreshVisual();
   var returned = Capture();
   Assert.Greater(returned[Pixel(3,0)].a,40,"Mist returns when the temporary light expires");
   MatchesQuery(returned,new Vector2(3,0));
   Assert.AreEqual(0,volume.ObscurityAt(new Vector2(12,0)),"Volume queries do not cover unrelated space");
   volume.kind = ChapterTwoObscurity.Kind.Darkness; volume.RefreshVisual();
   var solidDarkness = Capture();
   Assert.That((int)solidDarkness[Pixel(3,0)].a,Is.EqualTo(204).Within(2),"Darkness keeps its original opacity instead of the mist ribbons");
   MatchesQuery(solidDarkness,new Vector2(3,0));
  } finally {
   if(cameraObject != null) cameraObject.GetComponent<Camera>().targetTexture = null;
   if(read != null) Object.DestroyImmediate(read);
   if(target != null) Object.DestroyImmediate(target);
   if(cameraObject != null) Object.DestroyImmediate(cameraObject);
   if(volumeObject != null) Object.DestroyImmediate(volumeObject);
   if(lightObject != null) Object.DestroyImmediate(lightObject);
  }
 }
}
}
#endif
