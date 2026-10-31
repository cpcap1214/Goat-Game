using NUnit.Framework;using UnityEngine;
namespace OneMoreTime.Tests {
public class EndingTests {
 [Test]public void EveryEndingPoseUsesItsCalibratedFootAnchor(){
  var a=Resources.Load<EndingAssets>("Ending/EndingAssets").goat;Vector2 feet=new Vector2(780,432);
  foreach(var list in new[]{a.run,a.idle})foreach(var sprite in list)foreach(float scale in new[]{78/256f,48/256f,.28f}){
   var rect=PlayerVisual.SpriteGUIRect(sprite,feet,scale);
   Assert.That(rect.x+sprite.pivot.x*scale,Is.EqualTo(feet.x).Within(.001f));
   Assert.That(rect.y+(sprite.rect.height-sprite.pivot.y)*scale,Is.EqualTo(feet.y).Within(.001f));
  }
 }

 [Test]public void RunUsesOriginalSixFramesAndLoops(){
  var a=Resources.Load<EndingAssets>("Ending/EndingAssets").goat;
  Assert.AreEqual(6,a.run.Length);var seen=new System.Collections.Generic.HashSet<Sprite>();
  for(int i=0;i<6;i++){Assert.IsNotNull(a.run[i]);Assert.IsTrue(seen.Add(a.run[i]));Assert.AreSame(a.run[i],a.Get(1,i));Assert.AreSame(a.run[i],a.Get(1,i+6));}
  Assert.AreEqual(12,AnimationLibrary.RunFramesPerSecond);
 }

 [Test]public void GoatArrivesBeforeEnteringAndTitleAppearsAfterEntry(){
  var s=new EndingSequence();Assert.IsFalse(s.Complete);Assert.AreEqual(0,s.Reveal);
  s.Tick(6);Assert.AreEqual(1,s.Walk);Assert.AreEqual(0,s.Enter);Assert.AreEqual(0,s.Reveal);
  s.Tick(2.5f);Assert.AreEqual(1,s.Enter);Assert.AreEqual(0,s.Reveal);
  s.Tick(2.5f);Assert.IsTrue(s.Complete);Assert.AreEqual(1,s.Reveal);
  s.Tick(100);Assert.AreEqual(EndingSequence.Duration,s.Elapsed);
 }
 [Test]public void SkipAlwaysEndsAtReturnedHomeState(){foreach(float elapsed in new[]{0f,4f,8f}){var s=new EndingSequence();s.Tick(elapsed);s.Skip();Assert.IsTrue(s.Complete);Assert.AreEqual(1,s.Enter);Assert.AreEqual(1,s.Reveal);}}
 [Test]public void EndingResourcesIncludeTowerAndAnimatedGoat(){var a=Resources.Load<EndingAssets>("Ending/EndingAssets");Assert.IsNotNull(a);Assert.IsNotNull(a.background);Assert.IsNotNull(a.goat);Assert.Greater(a.goat.run.Length,1);foreach(var s in a.goat.run)Assert.IsNotNull(s);}
}
}
