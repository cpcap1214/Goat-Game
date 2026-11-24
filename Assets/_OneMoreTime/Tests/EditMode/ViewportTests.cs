using NUnit.Framework;using UnityEngine;
namespace OneMoreTime.Tests {
public class ViewportTests {
 [TestCase(960,540)][TestCase(1920,1080)][TestCase(1280,800)][TestCase(800,1000)]
 public void WorldFillsLetterboxedViewWithoutReservingAControlsBar(int width,int height){
  var rect=GameUI.GameplayViewport(width,height);float scale=Mathf.Min(width/960f,height/540f);
  float bottomMargin=(height-540*scale)*.5f;
  Assert.That(rect.yMin*height,Is.EqualTo(bottomMargin).Within(.001f));
  Assert.That(rect.height*height,Is.EqualTo(540*scale).Within(.001f));
  Assert.That(rect.width*width/(rect.height*height),Is.EqualTo(16f/9f).Within(.001f));
  Assert.That(rect.xMin,Is.GreaterThanOrEqualTo(0));Assert.That(rect.xMax,Is.LessThanOrEqualTo(1));
 }
}
}
