#if UNITY_EDITOR
using System.Linq;using NUnit.Framework;using UnityEditor;using UnityEngine;
namespace OneMoreTime.Tests {
public class ArtTests {
 [TestCase(1,"L01_Freeze")][TestCase(2,"L02_Pressure")][TestCase(3,"L03_Trace")][TestCase(4,"L04_Timing")]
 public void LandscapeSpriteCoversEntireConfiguredLevel(int level,string name){
  var c=AssetDatabase.LoadAssetAtPath<LevelConfig>("Assets/_OneMoreTime/Config/"+name+".asset");
  var s=AssetDatabase.LoadAssetAtPath<Sprite>("Assets/_OneMoreTime/Generated/TerrainLandscapeSprite"+level+".asset");
  int width=Mathf.CeilToInt(c.terrain.Max(r=>r.xMax)*16)-Mathf.FloorToInt(c.terrain.Min(r=>r.xMin)*16);
  int height=Mathf.CeilToInt(c.terrain.Max(r=>r.yMax)*16)-Mathf.FloorToInt(c.terrain.Min(r=>r.yMin)*16);
  Assert.That(s.rect,Is.EqualTo(new Rect(0,0,width,height)),"Sprite crop must grow/shrink with terrain, including the exit bank");
  Assert.That(s.texture.width,Is.EqualTo(width));Assert.That(s.texture.height,Is.EqualTo(height));
  Assert.That(s.bounds.size.x,Is.EqualTo(width/16f).Within(.001f));Assert.That(s.bounds.size.y,Is.EqualTo(height/16f).Within(.001f));
  Assert.That(s.pivot,Is.EqualTo(Vector2.zero));
 }

 [Test]public void GoatHasTwentyFourAlignedFramesAndFoxIsPreserved(){var a=AssetDatabase.LoadAssetAtPath<AnimationLibrary>("Assets/_OneMoreTime/Config/AnimationLibrary.asset");Assert.NotNull(a);var frames=a.idle.Concat(a.run).Concat(a.rise).Concat(a.descent).Concat(a.landing).ToArray();Assert.AreEqual(24,frames.Length);Assert.AreEqual(24,frames.Distinct().Count());foreach(var s in frames){Assert.NotNull(s);Assert.That(s.pixelsPerUnit,Is.EqualTo(170));Assert.That(s.pivot.y,Is.InRange(0,100));}Assert.AreEqual(1,a.visualScale);Assert.AreEqual(Vector2.zero,a.visualOffset);var fox=AssetDatabase.LoadAssetAtPath<AnimationLibrary>("Assets/_OneMoreTime/Config/FoxAnimationLibrary.asset");Assert.AreEqual(4,fox.idle.Length);Assert.AreEqual(6,fox.run.Length);Assert.AreSame(fox.run[1],fox.Get(1,1));Assert.IsTrue(AssetDatabase.GetAssetPath(fox.idle[0]).Contains("SunnyLand Artwork"));Assert.NotNull(AssetDatabase.LoadAssetAtPath<GameObject>("Assets/_OneMoreTime/Prefabs/FoxTestPlayer.prefab"));}
 [Test]public void AirAndLandingFramesClampWhileGroundCyclesLoop(){var a=AssetDatabase.LoadAssetAtPath<AnimationLibrary>("Assets/_OneMoreTime/Config/AnimationLibrary.asset");Assert.AreSame(a.rise[2],a.Get(2,100));Assert.AreSame(a.descent[2],a.Get(3,100));Assert.AreSame(a.landing[5],a.Get(4,100));Assert.AreSame(a.idle[1],a.Get(0,7));Assert.AreSame(a.run[1],a.Get(1,25));}
}
}
#endif
