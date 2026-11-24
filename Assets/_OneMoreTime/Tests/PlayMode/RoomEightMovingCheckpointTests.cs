#if UNITY_EDITOR
using NUnit.Framework;
using UnityEngine;

namespace OneMoreTime.Tests {
[Category("ChapterOneRegression")]
public class RoomEightMovingCheckpointTests {
 const float Dt=.02f;
 GameObject fixture;
 PlayerTuning tuning;
 [SetUp]public void Setup(){fixture=new GameObject("Room 8 moving checkpoint fixture");tuning=ScriptableObject.CreateInstance<PlayerTuning>();}
 [TearDown]public void Cleanup(){Object.DestroyImmediate(fixture);Object.DestroyImmediate(tuning);}
 GameObject Child(string name){var child=new GameObject(name);child.transform.SetParent(fixture.transform);return child;}
 RoomPlatform Platform(){
  var item=Child("One-way statue deck");item.transform.position=new Vector2(4,7);item.layer=30;
  var platform=item.AddComponent<RoomPlatform>();platform.solid=item.AddComponent<BoxCollider2D>();
  platform.solid.size=new Vector2(3,.18f);platform.solid.offset=new Vector2(0,-.09f);item.AddComponent<OneWayPlatform>();
  platform.artwork=new[]{item.AddComponent<SpriteRenderer>()};platform.plate=Child("Start plate").AddComponent<PressurePlate>();
  platform.motion=RoomPlatform.Motion.OneWayTravel;platform.travel=new Vector2(11,7);platform.journeySeconds=10;platform.Tick(0);
  return platform;
 }
 PlayerMotor2D Player(){var player=Child("Live goat").AddComponent<PlayerMotor2D>();player.tuning=tuning;player.worldMask=1<<30;player.Init();return player;}
 void Steps(RoomPlatform platform,int count){for(int i=0;i<count;i++)platform.Tick(Dt);}
 void Near(Vector2 expected,Vector2 actual){Assert.That(Vector2.Distance(expected,actual),Is.LessThan(.001f));}
 MovingRoomCheckpoint MovingCheckpoint(RoomPlatform platform){
  // The room controller stays disabled; these checks exercise the anchor API
  // independently of scene bootstrapping, input, and camera presentation.
  var host=Child("Inactive room controller");host.SetActive(false);var room=host.AddComponent<MainRoom>();room.player=Player();
  var mark=Child("Moving goat statue").AddComponent<RoomCheckpoint>();mark.transform.SetParent(platform.transform,false);mark.transform.localPosition=Vector3.zero;
  room.checkpoints=new[]{mark};room.player.ResetAt(mark.transform.position);Assert.IsTrue(room.CheckForCheckpoint());
  var moving=Child("Moving checkpoint binding").AddComponent<MovingRoomCheckpoint>();moving.room=room;moving.platform=platform;moving.checkpoint=mark;moving.Initialize();
  return moving;
 }

 [Test]public void OnePressRunsForTenSecondsAndParksPermanentlyAtTheEndpoint(){
  var platform=Platform();Vector2 home=platform.transform.position;
  Steps(platform,100);Near(home,platform.transform.position);Assert.IsTrue(platform.solid.enabled);Assert.IsFalse(platform.JourneyStarted);
  platform.plate.RestoreRewind(1);platform.Tick(0);platform.plate.RestoreRewind(0);
  Steps(platform,250);Near(home+platform.travel*.5f,platform.transform.position);Assert.IsTrue(platform.InTransit);
  Steps(platform,250);Near(home+platform.travel,platform.transform.position);Assert.IsFalse(platform.InTransit);Assert.AreEqual(1,platform.JourneyProgress);
  Steps(platform,750);Near(home+platform.travel,platform.transform.position);Assert.IsTrue(platform.Active);Assert.IsTrue(platform.solid.enabled);Assert.AreEqual(Vector2.zero,platform.Delta);
  platform.ResetState();Near(home,platform.transform.position);Assert.IsFalse(platform.JourneyStarted);Assert.IsTrue(platform.Active);Assert.AreEqual(Vector2.zero,platform.Delta);
 }

 [Test]public void RewindRestoresTheTriggerAndJourneyWhileWaitingHasNoResidualCarry(){
  var platform=Platform();var unstarted=platform.CaptureRewind();
  platform.plate.RestoreRewind(1);platform.Tick(0);platform.plate.RestoreRewind(0);Steps(platform,100);var saved=platform.CaptureRewind();
  Steps(platform,150);Vector2 expected=platform.transform.position;platform.RestoreRewind(saved);Vector2 held=platform.transform.position;
  for(int i=0;i<200;i++)platform.HoldFrame();Near(held,platform.transform.position);Assert.AreEqual(Vector2.zero,platform.Delta);Assert.IsTrue(platform.InTransit);
  Steps(platform,150);Near(expected,platform.transform.position);
  platform.RestoreRewind(unstarted);Assert.IsFalse(platform.JourneyStarted);Steps(platform,100);Near(unstarted.position,platform.transform.position);
 }

 [Test]public void ExistingMotorCarriesTheGoatExactlyOnceAndJumpDoesNotInheritDeckVelocity(){
  var platform=Platform();var player=Player();player.ResetAt(new Vector2(4,7.01f));Physics2D.SyncTransforms();
  for(int i=0;i<3;i++)player.Step(default,Dt,platforms:new[]{platform});Assert.AreSame(platform.solid,player.GroundCollider);Vector2 start=player.Position;
  platform.plate.RestoreRewind(1);platform.Tick(0);platform.plate.RestoreRewind(0);
  for(int i=0;i<250;i++){platform.Tick(Dt);Physics2D.SyncTransforms();player.Step(default,Dt,platforms:new[]{platform});Physics2D.SyncTransforms();}
  Near(start+platform.travel*.5f,player.Position);Assert.AreSame(platform.solid,player.GroundCollider);
  platform.Tick(Dt);Physics2D.SyncTransforms();player.Step(new InputFrame{jump=true,held=true},Dt,platforms:new[]{platform});
  Assert.IsFalse(player.Grounded);Assert.AreEqual(0,player.Velocity.x,.0001f);Assert.AreEqual(tuning.jumpSpeed-tuning.gravity*Dt,player.Velocity.y,.0001f);
 }

 [Test]public void SelectedStatueFollowsDeckAndDeathReturnsGoatAndStatueToAuthoredHome(){
  var platform=Platform();var binding=MovingCheckpoint(platform);Vector2 home=binding.checkpoint.transform.position;
  platform.plate.RestoreRewind(1);Steps(platform,250);binding.SyncCheckpoint();
  Near(home+platform.travel*.5f,binding.checkpoint.transform.position);Near(binding.checkpoint.transform.position,binding.room.checkpoint);
  binding.room.player.ResetAt(new Vector2(30,-10));binding.MarkDeath();Assert.IsTrue(binding.ResetPending);binding.FinishDeathRewind();
  Near(home,platform.transform.position);Near(home,binding.room.checkpoint);Near(home,binding.room.player.Position);
  Assert.IsTrue(binding.checkpoint.Saved);Assert.AreSame(binding.checkpoint,binding.room.ActiveCheckpoint);Assert.IsFalse(binding.ResetPending);Assert.IsFalse(platform.plate.Pressed);
 }

 [Test]public void MovingStatueResetDoesNotReplaceAnotherSelectedCheckpoint(){
  var platform=Platform();var binding=MovingCheckpoint(platform);var other=Child("Earlier bank statue").AddComponent<RoomCheckpoint>();other.transform.position=new Vector2(-20,0);
  binding.room.checkpoints=new[]{binding.checkpoint,other};binding.room.player.ResetAt(other.transform.position);Assert.IsTrue(binding.room.CheckForCheckpoint());
  platform.plate.RestoreRewind(1);Steps(platform,250);binding.SyncCheckpoint();Near(other.transform.position,binding.room.checkpoint);
  binding.MarkDeath();binding.ResetAfterDeath();Near(other.transform.position,binding.room.player.Position);Near(other.transform.position,binding.room.checkpoint);
  Near(new Vector2(4,7),platform.transform.position);Assert.AreSame(other,binding.room.ActiveCheckpoint);
 }

 [Test]public void FinishingAnOrdinaryRecordingDoesNotResetTheMovingStatue(){
  var platform=Platform();var binding=MovingCheckpoint(platform);platform.plate.RestoreRewind(1);Steps(platform,100);var start=platform.CaptureRewind();
  Steps(platform,200);platform.RestoreRewind(start);binding.SyncCheckpoint();binding.FinishDeathRewind();
  Near(start.position,platform.transform.position);Near(start.position,binding.room.checkpoint);Assert.IsTrue(platform.JourneyStarted);Assert.IsFalse(binding.ResetPending);
 }
}
}
#endif
