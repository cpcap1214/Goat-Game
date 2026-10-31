using NUnit.Framework;using UnityEngine;
namespace OneMoreTime.Tests {
public class ViewportTests {
 [TestCase(960,540)][TestCase(1920,1080)][TestCase(1280,800)][TestCase(800,1000)]
 public void WorldEndsAboveControlsAndRetainsFullCameraHeight(int width,int height){
  var rect=GameUI.GameplayViewport(width,height);float scale=Mathf.Min(width/960f,height/540f);
  float controlsTopFromBottom=(height-540*scale)*.5f+37*scale;
  Assert.That(rect.yMin*height,Is.EqualTo(controlsTopFromBottom).Within(.001f));
  Assert.That(rect.height*height,Is.EqualTo(503*scale).Within(.001f));
  Assert.That(rect.xMin,Is.GreaterThanOrEqualTo(0));Assert.That(rect.xMax,Is.LessThanOrEqualTo(1));
 }
}
}
