#if UNITY_EDITOR
using System.Collections;
using System.Linq;
using NUnit.Framework;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

namespace OneMoreTime.Tests {
[Category("RewindWaitingRegression"), Category("ChapterOneRegression")]
public class RewindWaitingTests {
 RewindPlaytest replay;
 ChapterOneRoom chapter;

 IEnumerator Load(bool river=false,int number=6,bool fixture=false){
  MainGameProgress.StartOver();chapter=null;
  string name=river?"Main_Room"+number.ToString("00"):"Debug_Rewind";
  // The original river fixture keeps rewind coverage while campaign Room 4 is shutter-only.
  string folder=(fixture||river&&number==6)?"Tests/Fixtures/":"Scenes/";
  EditorSceneManager.LoadSceneInPlayMode("Assets/_OneMoreTime/"+folder+name+".unity",new LoadSceneParameters(LoadSceneMode.Single));
  yield return null;
  replay=Object.FindFirstObjectByType<RewindPlaytest>();replay.AutoTick=false;
  chapter=Object.FindFirstObjectByType<ChapterOneRoom>();
  if(chapter!=null){
   chapter.AutoTick=chapter.room.AutoTick=false;
   for(int i=0;i<300&&chapter.CurrentPhase==ChapterOneRoom.Phase.Entering;i++)Act();
   Assert.AreEqual(ChapterOneRoom.Phase.Playing,chapter.CurrentPhase);
  }
 }
 void Act(bool record=false,float move=0,bool jump=false,bool shutter=false){
  var input=new InputFrame{move=move,jump=jump,held=true,freeze=shutter};
  if(chapter!=null)chapter.Step(input,record);else replay.Step(input,record);
  Physics2D.SyncTransforms();
 }
 void Wait(int ticks){for(int i=0;i<ticks;i++)Act();}
 void Place(Vector2 position){replay.player.ResetAt(position+Vector2.up*.01f);Physics2D.SyncTransforms();Wait(3);}
 void Record(int ticks=60){
  for(int i=0;i<13;i++)Act(record:true);
  Assert.AreEqual(RewindPlaytest.Mode.Recording,replay.State);Act();Wait(ticks);
 }
 void Finish(){
  Act(record:true);Act();
  Assert.AreEqual(RewindPlaytest.Mode.Rewinding,replay.State);
  for(int i=0;i<150&&replay.State==RewindPlaytest.Mode.Rewinding;i++)Act();
  Assert.AreEqual(RewindPlaytest.Mode.Ready,replay.State);Assert.IsTrue(replay.AwaitingReplay);
 }
 void Play(){Act(record:true);Act();Assert.AreEqual(RewindPlaytest.Mode.Playing,replay.State);Assert.IsFalse(replay.AwaitingReplay);}
 CrateSpawner Source=>replay.participants.OfType<CrateSpawner>().Single();

 [UnityTest]public IEnumerator AffectedPlatformsAndPushCratesWaitWhileTheLivePlayerCanMove(){
  yield return Load();Place(new Vector2(6.9f,0));Record(0);
  var crate=replay.crates[0];float initialCrateX=crate.transform.position.x;
  for(int i=0;i<50;i++)Act(move:1);
  Assert.Greater(crate.transform.position.x,initialCrateX+.5f);
  Finish();var platform=replay.platforms.Single();var platformPosition=platform.transform.position;
  var cratePosition=crate.transform.position;var playerPosition=replay.player.Position;
  for(int i=0;i<30;i++)Act(move:-1);
  Assert.Less(replay.player.Position.x,playerPosition.x-1,"Waiting freezes affected scenery, not the live goat.");
  Place(new Vector2(6.9f,0));
  for(int i=0;i<50;i++)Act(move:1);
  Assert.AreEqual(cratePosition,crate.transform.position,"A restored crate cannot be pushed before replay.");
  Assert.AreEqual(platformPosition,platform.transform.position);
  Assert.AreEqual(Vector2.zero,platform.Delta);Assert.AreEqual(Vector2.zero,crate.CarryDelta);
  Play();Wait(30);
  Assert.AreNotEqual(platformPosition,platform.transform.position);
  Assert.Greater(crate.transform.position.x,cratePosition.x+.4f,"Replay resumes the saved crate motion.");
 }

 [UnityTest]public IEnumerator AStandingRiderDoesNotInheritThePlatformsLastRecordedDisplacement(){
  yield return Load();Record();Finish();var platform=replay.platforms.Single();
  Place(new Vector2(platform.solid.bounds.center.x,platform.solid.bounds.max.y));
  Assert.AreSame(platform.solid,replay.player.GroundCollider);
  var position=replay.player.Position;Wait(100);
  Assert.AreSame(platform.solid,replay.player.GroundCollider);
  Assert.That(Vector2.Distance(position,replay.player.Position),Is.LessThan(.001f));
  Assert.AreEqual(Vector2.zero,platform.Delta);
  Play();Wait(20);Assert.Greater(Vector2.Distance(position,replay.player.Position),.3f);
 }

 [UnityTest]public IEnumerator ObjectsOptedOutOfRewindKeepSimulatingWhileWaiting(){
  yield return Load();var platform=replay.platforms.Single();platform.affectedByRewind=false;
  Record();Finish();var position=platform.transform.position;Wait(40);
  Assert.IsTrue(replay.AwaitingReplay);
  Assert.Greater(Vector3.Distance(position,platform.transform.position),.5f);
 }

 [UnityTest]public IEnumerator HoldingRecordReleasesWaitingAndOverwritesTheOldMemory(){
  yield return Load();Record();Finish();var platform=replay.platforms.Single();var position=platform.transform.position;
  for(int i=0;i<12;i++)Act(record:true);
  Assert.IsTrue(replay.AwaitingReplay);Assert.AreEqual(position,platform.transform.position);
  Act(record:true);Assert.AreEqual(RewindPlaytest.Mode.Recording,replay.State);
  Assert.IsFalse(replay.AwaitingReplay);Assert.IsNull(replay.Memory);Act();Wait(20);
  Assert.Greater(Vector3.Distance(position,platform.transform.position),.3f);
 }

 [UnityTest]public IEnumerator RetryKeepsTheMemoryAndItsObjectsWaitingUntilReplay(){
  yield return Load();Record();Finish();var saved=replay.Memory;
  replay.Retry();Assert.AreSame(saved,replay.Memory);Assert.IsTrue(replay.AwaitingReplay);
  var platform=replay.platforms.Single();var position=platform.transform.position;Wait(80);
  Assert.AreEqual(position,platform.transform.position);Play();Wait(20);
  Assert.Greater(Vector3.Distance(position,platform.transform.position),.3f);
 }

 [UnityTest]public IEnumerator RiverLogsStayStillAndNewPlatePressesCannotSpawnBeforeReplay(){
  yield return Load(true);Place(Source.activationPlate.transform.position);
  Assert.AreEqual(1,Source.ActiveCount);Place(chapter.room.start);Record();Finish();
  var log=Source.pool.Single(item=>item.gameObject.activeSelf);var position=log.transform.position;float clock=log.clock;
  Wait(100);Assert.AreEqual(position,log.transform.position);Assert.AreEqual(clock,log.clock);
  Assert.AreEqual(Vector2.zero,log.CarryDelta);
  Place(Source.activationPlate.transform.position);Wait(30);
  Assert.AreEqual(1,Source.ActiveCount,"The restored log source must not create a second log while waiting.");
  Assert.AreEqual(position,log.transform.position);Assert.AreEqual(clock,log.clock);
  Play();Wait(20);
  Assert.Greater(log.transform.position.x,position.x+.6f);
  Assert.Greater(log.clock,clock);
 }

 [UnityTest]public IEnumerator FrozenRiverLogCarriesNoStaleMotionAndCanBeRiddenWhenReplayStarts(){
  yield return Load(true);Place(Source.activationPlate.transform.position);Place(chapter.room.start);Record();Finish();
  var log=Source.pool.Single(item=>item.gameObject.activeSelf);var deck=log.GetComponent<BoxCollider2D>();
  Place(new Vector2(deck.bounds.center.x,deck.bounds.max.y));Assert.AreSame(deck,replay.player.GroundCollider);
  var position=replay.player.Position;Wait(80);
  Assert.AreEqual(ChapterOneRoom.Phase.Playing,chapter.CurrentPhase);
  Assert.That(Vector2.Distance(position,replay.player.Position),Is.LessThan(.001f));
  Assert.AreSame(deck,replay.player.GroundCollider);Assert.AreEqual(Vector2.zero,log.CarryDelta);
  Play();Wait(20);
  Assert.Greater(replay.player.Position.x,position.x+.6f);Assert.AreSame(deck,replay.player.GroundCollider);
 }

 [UnityTest]public IEnumerator ReplacingSavedReplayWithShutterReleasesWaitingObjects(){
  yield return Load(true);Place(Source.activationPlate.transform.position);Place(chapter.room.start);Record();Finish();
  var log=Source.pool.Single(item=>item.gameObject.activeSelf);var position=log.transform.position;
  Act(shutter:true);Assert.IsTrue(chapter.room.shutter.Snapping);
  Assert.IsFalse(replay.AwaitingReplay);Assert.IsNull(replay.Memory);Assert.AreEqual(RewindPlaytest.Mode.Idle,replay.State);
  for(int i=0;i<30&&chapter.room.shutter.Snapping;i++)Act();
  Assert.IsFalse(chapter.room.shutter.Snapping);Wait(25);
  Assert.Greater(log.transform.position.x,position.x+.8f);
 }

 [UnityTest]public IEnumerator ChapterPlateAndPlatformKeepTheirSavedStateWhenThePlayerWalksAway(){
  yield return Load(true);var platform=replay.platforms.Single();
  Place(platform.plate.transform.position);Record(0);Place(chapter.room.start);Wait(60);Finish();
  Assert.IsTrue(platform.plate.Pressed);var position=platform.transform.position;
  Place(chapter.room.start);Wait(100);
  Assert.IsTrue(platform.plate.Pressed,"The chapter's external mechanism evaluation must also honor the waiting state.");
  Assert.AreEqual(position,platform.transform.position);
  Play();Wait(10);Assert.IsFalse(platform.plate.Pressed);
  Assert.Less(platform.transform.position.x,position.x,"After replay starts the released plate lets the deck return normally.");
 }

 [UnityTest]public IEnumerator RecordedCrumbleCountdownWaitsUntilReplayStarts(){
  yield return Load(true,8,fixture:true);
  var bridge=replay.participants.OfType<ChapterCrumbleBridge>().Single();
  Place(new Vector2(bridge.deck.bounds.center.x,bridge.deck.bounds.max.y));
  Assert.IsTrue(bridge.Armed);Record(0);Place(chapter.room.start);Wait(60);
  Assert.IsTrue(bridge.Collapsed);Finish();Assert.IsFalse(bridge.Collapsed);
  float remaining=bridge.Remaining;Assert.Greater(remaining,0);Place(chapter.room.start);Wait(300);
  Assert.AreEqual(remaining,bridge.Remaining);Assert.IsFalse(bridge.Collapsed);Assert.IsTrue(bridge.deck.enabled);
  Play();Wait(40);Assert.IsTrue(bridge.Collapsed);Assert.IsFalse(bridge.deck.enabled);
 }
}
}
#endif
