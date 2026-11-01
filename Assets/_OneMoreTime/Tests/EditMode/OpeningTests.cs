using NUnit.Framework;using UnityEngine;
namespace OneMoreTime.Tests {
public class OpeningTests {
 [Test]public void OpeningRunsFor27SecondsAndVisitsEachStoryBeat(){
  var s=new OpeningSequence();Assert.AreEqual(0,s.Shot);s.Tick(-1);Assert.AreEqual(0,s.Elapsed);
  s.Tick(7);Assert.AreEqual(1,s.Shot);s.Tick(6);Assert.AreEqual(2,s.Shot);s.Tick(7);Assert.AreEqual(3,s.Shot);
  s.Tick(6.9f);Assert.IsFalse(s.Complete);s.Tick(.2f);Assert.IsTrue(s.Complete);Assert.AreEqual(27,s.Elapsed);
 }
 [Test]public void DialogueAdvancesFromHostToCrowdToGoatWithoutSkippingIntoGameplayEarly(){
  var s=new OpeningSequence();Assert.AreEqual("主持人",s.Speaker);
  for(int i=0;i<8;i++){Assert.AreEqual(i,s.Cue);Assert.IsNotEmpty(s.Line);Assert.IsFalse(s.Complete);s.Next();}
  Assert.IsTrue(s.Complete);Assert.AreEqual(27,s.Elapsed);
 }
 [Test]public void SkipWorksFromEveryShot(){foreach(float t in new[]{0,8,15,24}){var s=new OpeningSequence();s.Tick(t);s.Skip();s.Tick(1);Assert.IsTrue(s.Complete);Assert.AreEqual(27,s.Elapsed);}}
 [Test]public void CinematicHasPrizeAndAnimatedGoat(){Assert.IsNotNull(Resources.Load<Texture2D>("Opening/GTA7Prize"));Assert.IsNotNull(Resources.Load<EndingAssets>("Ending/EndingAssets").goat);}
}
}
