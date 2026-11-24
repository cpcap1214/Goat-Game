#if UNITY_EDITOR
using NUnit.Framework;
using UnityEngine;

namespace OneMoreTime.Tests {
[Category("ChapterOneRegression")]
public class RoomSixMechanismTests {
 const float Dt=.02f;
 GameObject fixture;

 [SetUp]public void Setup(){fixture=new GameObject("Room 6 mechanism fixture");}
 [TearDown]public void Cleanup(){Object.DestroyImmediate(fixture);}
 GameObject Child(string name){var item=new GameObject(name);item.transform.SetParent(fixture.transform);return item;}
 PressurePlate Plate()=>Child("Pressure plate").AddComponent<PressurePlate>();

 RoomPlatform Platform(bool returning){
  var item=Child("Plate platform");item.transform.position=new Vector2(4,7);
  var platform=item.AddComponent<RoomPlatform>();platform.solid=item.AddComponent<BoxCollider2D>();
  platform.solid.size=new Vector2(2,.18f);platform.solid.offset=new Vector2(0,-.09f);
  platform.artwork=new SpriteRenderer[0];platform.plate=Plate();
  platform.travel=new Vector2(6,0);platform.speed=12;platform.returnWhenReleased=returning;
  return platform;
 }

 CrateSpawner Source(bool activated){
  var source=Child("Floating log source").AddComponent<CrateSpawner>();
  source.water=source.gameObject.AddComponent<WaterCurrent2D>();
  source.water.left=0;source.water.right=100;source.water.surfaceY=0;source.water.velocity=2;
  source.spawnPoint=Child("Spawn point").transform;source.spawnPoint.position=new Vector2(2,.35f);
  source.initialHorizontalVelocity=2;source.spawnInterval=10;source.maximumActiveCrates=3;
  if(activated)source.activationPlate=Plate();
  source.pool=new FloatingObject2D[3];
  for(int i=0;i<source.pool.Length;i++){
   var item=Child("Floating log "+i);var log=item.AddComponent<FloatingObject2D>();
   log.water=source.water;item.SetActive(false);source.pool[i]=log;
  }
  return source;
 }

 [Test]public void HeldPlatformStopsAtEndpointAndReturnsSolidWhenReleased(){
  var platform=Platform(true);platform.ResetState();
  Assert.IsTrue(platform.Active);Assert.IsTrue(platform.solid.enabled);
  platform.plate.RestoreRewind(1);
  for(int i=0;i<25;i++)platform.Tick(Dt);
  Assert.That(platform.transform.position.x,Is.EqualTo(10).Within(.001f));
  for(int i=0;i<50;i++)platform.Tick(Dt);
  Assert.That(platform.transform.position.x,Is.EqualTo(10).Within(.001f),"A held plate parks the deck at the arrow, rather than reversing it.");
  platform.plate.RestoreRewind(0);platform.Tick(Dt);
  Assert.Less(platform.Delta.x,0);Assert.IsTrue(platform.Active);Assert.IsTrue(platform.solid.enabled);
  for(int i=0;i<30;i++)platform.Tick(Dt);
  Assert.That(platform.transform.position.x,Is.EqualTo(4).Within(.001f));
  Assert.AreEqual(Vector2.zero,platform.Delta);Assert.IsTrue(platform.solid.enabled);
 }

 [Test]public void PlatformReturnRewindResumesAndCanReverseAgainMidJourney(){
  var platform=Platform(true);platform.plate.RestoreRewind(1);
  for(int i=0;i<25;i++)platform.Tick(Dt);
  platform.plate.RestoreRewind(0);
  for(int i=0;i<5;i++)platform.Tick(Dt);
  var saved=platform.CaptureRewind();
  for(int i=0;i<5;i++)platform.Tick(Dt);
  Vector2 expected=platform.transform.position;
  platform.RestoreRewind(saved);
  for(int i=0;i<5;i++)platform.Tick(Dt);
  Assert.That(Vector2.Distance(expected,platform.transform.position),Is.LessThan(.0001f));
  platform.plate.RestoreRewind(1);platform.Tick(Dt);
  Assert.Greater(platform.Delta.x,0);Assert.IsTrue(platform.solid.enabled);
  platform.ResetState();Assert.That(platform.transform.position.x,Is.EqualTo(4));Assert.IsTrue(platform.Active);
 }

 [Test]public void ExistingShuttlesStillPingPongAndDeactivateOnRelease(){
  var platform=Platform(false);platform.plate.RestoreRewind(1);
  for(int i=0;i<30;i++)platform.Tick(Dt);
  Assert.Less(platform.Delta.x,0,"Default shuttles still reverse while held.");
  platform.plate.RestoreRewind(0);platform.Tick(Dt);
  Assert.IsFalse(platform.Active);Assert.IsFalse(platform.solid.enabled);Assert.AreEqual(Vector2.zero,platform.Delta);
 }

 [Test]public void PlateProducesOneLogPerPressAndReleasedLogsContinueAtTwoUnitsPerSecond(){
  var source=Source(true);
  for(int i=0;i<10;i++)source.Tick(Dt);
  Assert.AreEqual(0,source.ActiveCount);
  source.activationPlate.RestoreRewind(1);source.Tick(Dt);
  Assert.AreEqual(1,source.ActiveCount);float start=source.pool[0].transform.position.x;
  for(int i=0;i<600;i++)source.Tick(Dt);
  Assert.AreEqual(1,source.ActiveCount,"Holding longer than the old interval must not produce another log.");
  Assert.That(source.pool[0].transform.position.x-start,Is.EqualTo(24).Within(.002f));
  source.activationPlate.RestoreRewind(0);start=source.pool[0].transform.position.x;
  for(int i=0;i<50;i++)source.Tick(Dt);
  Assert.That(source.pool[0].transform.position.x-start,Is.EqualTo(2).Within(.002f));
  source.activationPlate.RestoreRewind(1);source.Tick(Dt);
  Assert.AreEqual(2,source.ActiveCount);
 }

 [Test]public void RewindAndPlaybackRestoreThePlateEdgeWithoutDuplicateLogs(){
  var source=Source(true);source.activationPlate.RestoreRewind(1);source.Tick(Dt);
  var held=source.Capture();
  source.activationPlate.RestoreRewind(0);source.Tick(Dt);var released=source.Capture();
  source.activationPlate.RestoreRewind(1);source.Tick(Dt);
  Assert.AreEqual(2,source.ActiveCount);Vector2 secondPosition=source.pool[1].transform.position;
  source.Restore(held);source.Tick(Dt);
  Assert.AreEqual(1,source.ActiveCount,"Restoring a pressed frame must not treat it as a new activation.");
  source.Restore(released);source.Tick(Dt);
  Assert.AreEqual(2,source.ActiveCount);
  Assert.That(Vector2.Distance(secondPosition,source.pool[1].transform.position),Is.LessThan(.0001f));
  source.RestorePlayback(held,true);source.Tick(Dt);
  Assert.AreEqual(1,source.ActiveCount,"Continuing from a replayed pressed frame must retain its edge state.");
 }

 [Test]public void LogCooldownRejectsRapidPressesWithoutQueuingAHeldSpawn(){
  var source=Source(true);source.plateCooldown=4;
  source.activationPlate.RestoreRewind(1);source.Tick(0);
  Assert.AreEqual(1,source.ActiveCount);Assert.That(source.Remaining,Is.EqualTo(4));
  source.activationPlate.RestoreRewind(0);source.Tick(.25f);
  source.activationPlate.RestoreRewind(1);source.Tick(.25f);
  Assert.AreEqual(1,source.ActiveCount,"A fresh press during cooldown must not release another log.");
  Assert.That(source.Remaining,Is.EqualTo(3.5f).Within(.001f),"Rejected presses must not restart the cooldown.");
  source.Tick(4);
  Assert.AreEqual(1,source.ActiveCount,"Holding a rejected press past expiry must not queue an automatic release.");
  Assert.That(source.Remaining,Is.EqualTo(0));
  source.activationPlate.RestoreRewind(0);source.Tick(0);
  source.activationPlate.RestoreRewind(1);source.Tick(0);
  Assert.AreEqual(2,source.ActiveCount);Assert.That(source.Remaining,Is.EqualTo(4));
 }

 [TestCase(false)]
 [TestCase(true)]
 public void RestoredLogCooldownPreservesTimeAndRequiresAFreshPress(bool playback){
  var source=Source(true);source.plateCooldown=4;
  source.activationPlate.RestoreRewind(1);source.Tick(0);
  source.Tick(1);var held=source.Capture();
  source.activationPlate.RestoreRewind(0);source.Tick(.5f);var released=source.Capture();
  source.Tick(4);source.activationPlate.RestoreRewind(1);source.Tick(0);
  Assert.AreEqual(2,source.ActiveCount);
  if(playback)source.RestorePlayback(held,true);else source.Restore(held);
  Assert.AreEqual(1,source.ActiveCount);Assert.That(source.Remaining,Is.EqualTo(3).Within(.001f));
  source.Tick(4);
  Assert.AreEqual(1,source.ActiveCount,"Restoring a held plate must not spawn again when its restored cooldown expires.");
  if(playback)source.RestorePlayback(released,true);else source.Restore(released);
  Assert.That(source.Remaining,Is.EqualTo(2.5f).Within(.001f));
  source.Tick(0);
  Assert.AreEqual(1,source.ActiveCount,"A new edge restored during cooldown still cannot spawn.");
  source.Tick(3);Assert.AreEqual(1,source.ActiveCount);
  source.activationPlate.RestoreRewind(0);source.Tick(0);
  source.activationPlate.RestoreRewind(1);source.Tick(0);
  Assert.AreEqual(2,source.ActiveCount);Assert.That(source.Remaining,Is.EqualTo(4));
 }

 [Test]public void UngatedSourcesKeepTheirIntervalAndRewindCountdown(){
  var source=Source(false);source.Tick(0);Assert.AreEqual(1,source.ActiveCount);
  source.Tick(9);Assert.AreEqual(1,source.ActiveCount);var saved=source.Capture();
  source.Tick(1.01f);Assert.AreEqual(2,source.ActiveCount);
  source.Restore(saved);Assert.AreEqual(1,source.ActiveCount);
  source.Tick(.5f);Assert.AreEqual(1,source.ActiveCount);
  source.Tick(.51f);Assert.AreEqual(2,source.ActiveCount);
 }
}
}
#endif
