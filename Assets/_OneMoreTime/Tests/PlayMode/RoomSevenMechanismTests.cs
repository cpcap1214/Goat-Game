#if UNITY_EDITOR
using NUnit.Framework;
using UnityEngine;

namespace OneMoreTime.Tests {
[Category("ChapterOneRegression")]
public class RoomSevenMechanismTests {
 const float Dt=.02f;
 GameObject fixture;
 [SetUp]public void Setup(){fixture=new GameObject("Room 7 mechanism fixture");}
 [TearDown]public void Cleanup(){Object.DestroyImmediate(fixture);}
 GameObject Child(string name){var item=new GameObject(name);item.transform.SetParent(fixture.transform);return item;}
 PressurePlate Plate()=>Child("Pressure plate").AddComponent<PressurePlate>();
 RoomPlatform Platform(RoomPlatform.Motion motion=RoomPlatform.Motion.PingPong){
  var item=Child("Moving deck");item.transform.position=new Vector2(4,7);
  var platform=item.AddComponent<RoomPlatform>();platform.solid=item.AddComponent<BoxCollider2D>();
  platform.solid.size=new Vector2(2,.18f);platform.solid.offset=new Vector2(0,-.09f);
  platform.artwork=new[]{item.AddComponent<SpriteRenderer>()};platform.motion=motion;
  return platform;
 }
 void Steps(RoomPlatform platform,int count){for(int i=0;i<count;i++)platform.Tick(Dt);}
 void Near(Vector2 expected,RoomPlatform platform){Assert.That(Vector2.Distance(expected,platform.transform.position),Is.LessThan(.001f));}

 [Test]public void ClosedRouteCompletesOneLapInSixSecondsAndPreservesCorners(){
  var platform=Platform(RoomPlatform.Motion.ClosedRoute);
  platform.route=new[]{Vector2.zero,new Vector2(0,3),new Vector2(3,3),new Vector2(3,0)};platform.cycleSeconds=6;
  Steps(platform,75);Near(new Vector2(4,10),platform);
  Steps(platform,75);Near(new Vector2(7,10),platform);
  Steps(platform,75);Near(new Vector2(7,7),platform);
  Steps(platform,75);Near(new Vector2(4,7),platform);
  Assert.IsTrue(platform.Active);Assert.IsTrue(platform.solid.enabled);
 }

 [Test]public void PingPongIntervalIncludesTheOutwardAndReturnLegs(){
  var platform=Platform();platform.travel=new Vector2(5,0);platform.cycleSeconds=4;
  Steps(platform,100);Near(new Vector2(9,7),platform);
  Steps(platform,50);Near(new Vector2(6.5f,7),platform);Assert.Less(platform.Delta.x,0);
  Steps(platform,50);Near(new Vector2(4,7),platform);
 }

 [Test]public void BothPlatesMustOverlapAndUnlockedDeckStaysGreenThroughReleaseAndRewind(){
  var platform=Platform();var left=Plate();var right=Plate();
  platform.requiredPlates=new[]{left,right};platform.latchWhenAllPressed=true;
  platform.counter=Child("0/2 label").AddComponent<TextMesh>();platform.travel=new Vector2(0,4);
  platform.Tick(0);var locked=platform.CaptureRewind();
  Assert.IsFalse(platform.Active);Assert.AreEqual("0/2",platform.counter.text);Assert.AreEqual(Color.white,platform.counter.color);
  left.RestoreRewind(1);platform.Tick(Dt);
  Assert.IsFalse(platform.Active);Assert.AreEqual("1/2",platform.counter.text);
  left.RestoreRewind(0);right.RestoreRewind(1);platform.Tick(Dt);
  Assert.IsFalse(platform.ActivationLatched,"Separate visits must not accumulate toward simultaneous activation.");
  left.RestoreRewind(1);platform.Tick(Dt);
  Assert.IsTrue(platform.ActivationLatched);Assert.IsTrue(platform.Active);Assert.AreEqual("2/2",platform.counter.text);
  Assert.Greater(platform.counter.color.g,platform.counter.color.r);
  left.RestoreRewind(0);right.RestoreRewind(0);platform.Tick(Dt);
  Assert.IsTrue(platform.Active);Assert.AreEqual("2/2",platform.counter.text);
  platform.RestoreRewind(locked);Assert.IsTrue(platform.Active);Assert.AreEqual("2/2",platform.counter.text);
  platform.ResetState();Assert.IsFalse(platform.ActivationLatched);Assert.IsFalse(platform.Active);Assert.AreEqual("0/2",platform.counter.text);
 }

 [TestCase(0)][TestCase(1)]public void EitherPairedPlateRevealsWithoutAdvancingOrLatchingTheDeck(int first){
  var platform=Platform();var left=Plate();var right=Plate();
  platform.requiredPlates=new[]{left,right};platform.latchWhenAllPressed=true;platform.travel=new Vector2(0,4);
  platform.counter=Child("Pair counter").AddComponent<TextMesh>();platform.Tick(0);
  var reveal=Child("Paired plate reveal").AddComponent<RoomPlatformReveal>();reveal.platform=platform;
  reveal.triggeringPlates=new[]{left,right};
  Assert.IsFalse(reveal.Tick(Dt));Assert.IsFalse(reveal.Shown);
  reveal.triggeringPlates[first].RestoreRewind(1);
  Assert.IsTrue(reveal.Tick(Dt));Assert.IsTrue(reveal.Revealing);Assert.IsTrue(reveal.Shown);
  Vector2 before=platform.transform.position;
  // ChapterOneRoom returns before the gameplay tick during the reveal, including
  // its finishing frame. Presentation itself must never tick or latch the deck.
  left.RestoreRewind(1);right.RestoreRewind(1);
  for(int i=0;i<120&&reveal.Revealing;i++)Assert.IsTrue(reveal.Tick(Dt));
  Assert.IsFalse(reveal.Revealing);Assert.IsFalse(platform.ActivationLatched);Assert.AreEqual("0/2",platform.counter.text);Near(before,platform);
  Assert.IsFalse(reveal.Tick(Dt),"A held plate must not replay the reveal once it has finished.");
  platform.Tick(Dt);
  Assert.IsTrue(platform.ActivationLatched);Assert.AreEqual("2/2",platform.counter.text);Assert.Greater(platform.counter.color.g,platform.counter.color.r);
  reveal.ResetReveal();Assert.IsFalse(reveal.Shown);Assert.IsTrue(reveal.Tick(Dt),"A reset allows the next attempt to reveal the linked platform again.");
 }

 [Test]public void SinglePlateRevealRemainsCompatibleAndOptionalTriggersMayBeNull(){
  var reveal=Child("Single plate reveal").AddComponent<RoomPlatformReveal>();
  reveal.plate=Plate();reveal.platform=Platform();reveal.triggeringPlates=null;
  Assert.IsFalse(reveal.Tick(Dt));reveal.plate.RestoreRewind(1);
  Assert.IsTrue(reveal.Tick(Dt));Assert.IsTrue(reveal.Revealing);
  reveal.ResetReveal();reveal.plate.RestoreRewind(0);reveal.triggeringPlates=new PressurePlate[]{null};
  Assert.IsFalse(reveal.Tick(Dt));
 }

 [Test]public void RouteRewindAndWaitingKeepTheDeckStillWithoutStaleCarry(){
  var platform=Platform(RoomPlatform.Motion.ClosedRoute);
  platform.route=new[]{Vector2.zero,new Vector2(0,3),new Vector2(3,3),new Vector2(3,0)};platform.cycleSeconds=6;
  Steps(platform,40);var saved=platform.CaptureRewind();Steps(platform,100);Vector2 expected=platform.transform.position;
  platform.RestoreRewind(saved);Vector2 waiting=platform.transform.position;
  for(int i=0;i<100;i++)platform.HoldFrame();
  Near(waiting,platform);Assert.AreEqual(Vector2.zero,platform.Delta);Assert.IsTrue(platform.solid.enabled);
  Steps(platform,100);Near(expected,platform);
 }

 PlatformDispatcher Dispatcher(){
  var source=Child("Platform dispatcher").AddComponent<PlatformDispatcher>();source.activationPlate=Plate();source.cooldown=4;
  source.pool=new RoomPlatform[3];
  for(int i=0;i<source.pool.Length;i++){
   var platform=Platform(RoomPlatform.Motion.DispatchedRoute);platform.route=new[]{Vector2.zero,new Vector2(2,0),new Vector2(2,2),new Vector2(-2,2)};platform.speed=2;platform.Tick(0);source.pool[i]=platform;
  }
  return source;
 }
 void Tick(PlatformDispatcher source,float dt){foreach(var platform in source.pool)platform.Tick(dt);source.Tick(dt);}
 int Active(PlatformDispatcher source){int count=0;foreach(var platform in source.pool)if(platform.InTransit)count++;return count;}

 [Test]public void DispatcherKeepsFinishedDeckSolidUntilAFreshPressReusesIt(){
  var source=Dispatcher();Tick(source,0);Assert.AreEqual(0,Active(source));
  source.activationPlate.RestoreRewind(1);Tick(source,Dt);Assert.AreEqual(1,Active(source));
  for(int i=0;i<50;i++)Tick(source,Dt);
  source.activationPlate.RestoreRewind(0);Tick(source,Dt);source.activationPlate.RestoreRewind(1);Tick(source,Dt);
  Assert.AreEqual(1,Active(source),"A new press before four seconds cannot dispatch a second deck.");
  for(int i=0;i<210;i++)Tick(source,Dt);
  Assert.AreEqual(0,Active(source),"The first deck stops at its endpoint, and holding does not repeatedly dispatch.");
  var finished=source.pool[0];Near(new Vector2(2,9),finished);
  Assert.IsTrue(finished.Active);Assert.IsTrue(finished.solid.enabled);Assert.IsTrue(finished.artwork[0].enabled);
  source.activationPlate.RestoreRewind(0);
  for(int i=0;i<500;i++)Tick(source,Dt);
  Near(new Vector2(2,9),finished);Assert.IsTrue(finished.Active);Assert.AreEqual(Vector2.zero,finished.Delta);
  source.activationPlate.RestoreRewind(0);Tick(source,Dt);source.activationPlate.RestoreRewind(1);Tick(source,Dt);
  Assert.AreEqual(1,Active(source));Near(new Vector2(4,7),finished);Assert.IsTrue(finished.solid.enabled);
  Assert.AreEqual(Vector2.zero,finished.Delta,"Redispatch must not carry a rider along the teleport back to the start.");
 }

 [Test]public void WaitingDispatchedDeckSurvivesRewindAndResetClearsIt(){
  var source=Dispatcher();source.activationPlate.RestoreRewind(1);Tick(source,Dt);
  for(int i=0;i<250;i++)Tick(source,Dt);
  var deck=source.pool[0];var waiting=deck.CaptureRewind();var sourceState=source.Capture();
  Assert.IsTrue(deck.Active);Assert.IsFalse(deck.InTransit);
  source.activationPlate.RestoreRewind(0);Tick(source,Dt);source.activationPlate.RestoreRewind(1);Tick(source,Dt);
  Assert.IsTrue(deck.InTransit);
  source.Restore(sourceState);deck.RestoreRewind(waiting);
  for(int i=0;i<100;i++){source.HoldFrame();deck.HoldFrame();}
  Near(new Vector2(2,9),deck);Assert.IsTrue(deck.Active);Assert.IsTrue(deck.solid.enabled);Assert.IsTrue(deck.artwork[0].enabled);Assert.IsFalse(deck.InTransit);
  Tick(source,Dt);Near(new Vector2(2,9),deck);Assert.IsFalse(deck.InTransit,"Restoring a held launch plate must not dispatch again.");
  deck.ResetState();Assert.IsFalse(deck.Active);Assert.IsFalse(deck.solid.enabled);Assert.IsFalse(deck.artwork[0].enabled);Near(new Vector2(4,7),deck);
 }

 [Test]public void DispatchRewindRestoresLifetimesCooldownAndPressEdgeWithoutDuplicateLaunch(){
  var source=Dispatcher();source.activationPlate.RestoreRewind(1);Tick(source,Dt);
  for(int i=0;i<50;i++)Tick(source,Dt);
  var sourceState=source.Capture();var states=new RoomPlatform.RewindState[source.pool.Length];
  for(int i=0;i<states.Length;i++)states[i]=source.pool[i].CaptureRewind();
  for(int i=0;i<250;i++)Tick(source,Dt);
  Assert.AreEqual(0,Active(source));
  source.Restore(sourceState);for(int i=0;i<states.Length;i++)source.pool[i].RestoreRewind(states[i]);
  float remaining=source.Remaining;Vector2 restored=source.pool[0].transform.position;
  for(int i=0;i<100;i++){source.HoldFrame();foreach(var platform in source.pool)platform.HoldFrame();}
  Assert.That(source.Remaining,Is.EqualTo(remaining));Near(restored,source.pool[0]);
  Tick(source,Dt);Assert.AreEqual(1,Active(source));Assert.Less(source.Remaining,remaining);
  Assert.IsFalse(source.pool[1].InTransit,"Restoring a held press cannot create a duplicate platform.");
 }
}
}
#endif
