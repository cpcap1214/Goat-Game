#if UNITY_EDITOR
using System.Linq;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;

namespace OneMoreTime.Tests {
[Category("ChapterOneRegression")]
public class FloatingSubmersionTests {
 [TestCase("Assets/_OneMoreTime/Prefabs/FloatingWaterCrate.prefab",0f)]
 [TestCase("Assets/_OneMoreTime/Prefabs/WorldOne/Expansion/FloatingLog.prefab",4.3f)]
 public void SinkingClipsOnlySubmergedPixelsAndRewindRestoresThem(string prefabPath,float surface){
  var fixture=new GameObject("Submersion render fixture");
  var target=new RenderTexture(128,128,24,RenderTextureFormat.ARGB32,RenderTextureReadWrite.sRGB);
  var read=new Texture2D(128,128,TextureFormat.RGBA32,false);
  var previous=RenderTexture.active;
  Camera camera=null;
  try {
   var prefab=AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);Assert.IsNotNull(prefab);
   var item=Object.Instantiate(prefab,fixture.transform);var floating=item.GetComponent<FloatingObject2D>();
   var water=fixture.AddComponent<WaterCurrent2D>();water.surfaceY=surface;floating.water=water;
   var sprites=item.GetComponentsInChildren<SpriteRenderer>(true);
   foreach(var sprite in sprites)sprite.gameObject.layer=30;
   var originals=sprites.Select(s=>s.sharedMaterial).ToArray();var colors=sprites.Select(s=>s.color).ToArray();
   floating.Restore(new FloatingObject2D.State{active=true,position=new Vector2(5,surface)});
   var position=(Vector2)item.transform.position;position.y+=surface-sprites[0].bounds.center.y;
   floating.Restore(new FloatingObject2D.State{active=true,position=position});var initial=floating.Capture();
   var cameraObject=new GameObject("Submersion camera");cameraObject.transform.SetParent(fixture.transform);
   camera=cameraObject.AddComponent<Camera>();camera.enabled=false;camera.orthographic=true;camera.orthographicSize=2;
   camera.transform.position=new Vector3(5,surface,-10);camera.backgroundColor=Color.black;
   camera.clearFlags=CameraClearFlags.SolidColor;camera.cullingMask=1<<30;camera.targetTexture=target;
   Color32[] Capture(){camera.Render();RenderTexture.active=target;read.ReadPixels(new Rect(0,0,128,128),0,0);read.Apply();RenderTexture.active=previous;return read.GetPixels32();}
   bool Visible(Color32 c)=>c.r>2||c.g>2||c.b>2;
   var before=Capture();
   Assert.Greater(before.Take(128*63).Count(Visible),8,"The fixture must show original pixels below the waterline.");
   Assert.Greater(before.Skip(128*65).Count(Visible),8,"Part of the fixture must remain above the waterline.");
   floating.BeginSink(5,surface-2.5f,3.5f);var sinking=floating.Capture();
   var clipped=Capture();
   Assert.AreEqual(0,clipped.Take(128*63).Count(Visible),"Every pixel below the water surface should disappear.");
   CollectionAssert.AreEqual(before.Skip(128*65).ToArray(),clipped.Skip(128*65).ToArray(),"The visible top keeps its texture, pixel scale and original colour.");
   Assert.IsTrue(item.activeSelf,"Submersion must not prematurely dispose the physics object.");
   Assert.IsTrue(item.GetComponent<BoxCollider2D>().enabled);
   for(int i=0;i<sprites.Length;i++)Assert.AreEqual(colors[i],sprites[i].color);

   for(int i=0;i<30;i++)floating.Tick(.02f);
   Assert.IsTrue(item.activeSelf,"The crate/log remains pooled and active until its actual disposal depth.");
   Assert.Less(item.transform.position.y,position.y-.5f);
   Assert.AreEqual(0,Capture().Count(Visible),"The submerged sprite must be hidden while its sink continues.");
   floating.RestorePlayback(sinking,true);
   CollectionAssert.AreEqual(clipped,Capture(),"Reversing the timeline restores the exact partly submerged image.");
   floating.Restore(initial);
   CollectionAssert.AreEqual(before,Capture(),"Rewinding to before the whirlpool restores all original pixels.");
   for(int i=0;i<sprites.Length;i++)Assert.AreSame(originals[i],sprites[i].sharedMaterial);

   floating.BeginSink(5,surface-2.5f,3.5f);for(int i=0;i<100;i++)floating.Tick(.02f);
   Assert.IsFalse(item.activeSelf);
   floating.Restore(initial);
   CollectionAssert.AreEqual(before,Capture(),"A reused pooled item cannot retain the previous waterline mask.");
  } finally {
   RenderTexture.active=previous;if(camera!=null)camera.targetTexture=null;
   Object.DestroyImmediate(fixture);target.Release();Object.DestroyImmediate(target);Object.DestroyImmediate(read);
  }
 }
}
}
#endif
