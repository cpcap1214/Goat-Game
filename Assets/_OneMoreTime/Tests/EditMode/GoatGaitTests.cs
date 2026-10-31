using NUnit.Framework;using UnityEngine;
namespace OneMoreTime.Tests {
public class GoatGaitTests {
 [Test]public void FootCycleHasContinuousPositionAndVelocity(){
  const float e=.0001f;
  foreach(float t in new[]{0f,.5f,1f}){
   var left=GoatGait.Foot(t-e);var p=GoatGait.Foot(t);var right=GoatGait.Foot(t+e);
   Assert.Less(Vector2.Distance(left,right),.03f);
   Assert.Less(Vector2.Distance((p-left)/e,(right-p)/e),.5f);
  }
 }
 [Test]public void ContactSlidesBackAndSwingClearsGround(){
  for(int i=0;i<12;i++){var f=GoatGait.Foot(i/24f);Assert.AreEqual(0,f.y);if(i>0)Assert.Less(f.x,GoatGait.Foot((i-1)/24f).x);}
  Assert.Greater(GoatGait.Foot(.75f).y,17);Assert.AreEqual(GoatGait.Foot(0),GoatGait.Foot(1));
 }
 [Test]public void ActiveRunUsesOriginalArtworkInOrder(){
  var lib=Resources.Load<EndingAssets>("Ending/EndingAssets").goat;
  Assert.AreEqual(6,lib.run.Length);
  for(int i=0;i<6;i++)Assert.AreEqual("Goat_"+(i+6).ToString("00"),lib.run[i].name);
 }
}
}
