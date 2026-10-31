using NUnit.Framework;using UnityEngine;
namespace OneMoreTime.Tests {
public class RecorderTests {
 [TestCase(3)][TestCase(10)][TestCase(15)][TestCase(60)]public void LatestWindow(int seconds){var r=new Recorder();for(int i=0;i<=seconds*50;i++)Assert.IsTrue(r.Add(new RecordedFrame{time=i*.02f,position=new Vector2(i,4),facing=1}));Assert.LessOrEqual(r.Count,501);Assert.That(r.Duration,Is.EqualTo(Mathf.Min(10,seconds)).Within(.0001f));Assert.That(r.At(0).position.x,Is.EqualTo(Mathf.Max(0,seconds*50-500)));}
 [Test]public void ImmutableAndWaiting(){var r=new Recorder();for(int i=0;i<=200;i++)r.Add(new RecordedFrame{time=i*.02f,position=new Vector2(12,i<=100?0:(i-100)*.01f),grounded=i<=100,state=2,frame=i});var s=r.Save(3);r.Clear();for(int i=0;i<600;i++)r.Add(new RecordedFrame{time=i,position=Vector2.zero});Assert.AreEqual(new Vector2(12,0),s.Sample(1.8f).position);Assert.AreEqual(200,s.Frame(200).frame);Assert.AreEqual(4,s.Duration);}
 [Test]public void InvalidInput(){var r=new Recorder();Assert.IsNull(r.Save(3));Assert.IsFalse(r.Add(new RecordedFrame{time=float.NaN}));Assert.IsFalse(r.Add(new RecordedFrame{position=new Vector2(float.PositiveInfinity,0)}));Assert.IsTrue(r.Add(default));Assert.IsFalse(r.Add(default));Assert.IsNull(r.Save(3));r.Add(new RecordedFrame{time=.2f});Assert.IsNull(r.Save(3));}
}
}
