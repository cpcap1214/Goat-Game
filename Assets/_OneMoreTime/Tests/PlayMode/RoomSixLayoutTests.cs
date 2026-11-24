#if UNITY_EDITOR
using System.Collections;
using System.Linq;
using NUnit.Framework;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

namespace OneMoreTime.Tests {
[Category("ChapterOneRegression")]
public class RoomSixLayoutTests {
 ChapterOneRoom chapter;
 RewindPlaytest replay;
 PlayerMotor2D Player=>chapter.room.player;
 CrateSpawner Source=>replay.participants.OfType<CrateSpawner>().Single();
 IEnumerator Load(bool debug=false,bool rewindFixture=false){
  if(debug){MainGameProgress.BeginDebug();MainGameProgress.GrantReplay();}else MainGameProgress.StartOver();
  string scene=rewindFixture?"Tests/Fixtures/Main_Room06":"Scenes/Main_Room04";
  EditorSceneManager.LoadSceneInPlayMode("Assets/_OneMoreTime/"+scene+".unity",new LoadSceneParameters(LoadSceneMode.Single));
  yield return null;
  chapter=Object.FindFirstObjectByType<ChapterOneRoom>();replay=chapter.replay;
  chapter.AutoTick=chapter.room.AutoTick=replay.AutoTick=false;
  for(int i=0;i<300&&chapter.CurrentPhase==ChapterOneRoom.Phase.Entering;i++)Act();
  Assert.AreEqual(ChapterOneRoom.Phase.Playing,chapter.CurrentPhase);
 }
 void Act(float move=0,bool jump=false,bool freeze=false,bool record=false){
  chapter.Step(new InputFrame{move=move,jump=jump,held=true,freeze=freeze},record);Physics2D.SyncTransforms();
 }
 void Wait(int ticks){for(int i=0;i<ticks;i++)Act();}
 void Place(Vector2 p){Player.ResetAt(p+Vector2.up*.01f);Physics2D.SyncTransforms();Wait(3);}
 float Steer(float x)=>Mathf.Abs(x-Player.Position.x)>.12f?Mathf.Sign(x-Player.Position.x):0;

 [UnityTest]public IEnumerator RiverGeometryUsesOnlyShutterAndDecorativeOrangeBridge(){
  yield return Load();
  Assert.AreEqual(new Rect(0,-6,97,28.5f),chapter.room.roomBounds);
  Assert.IsFalse(chapter.FilterAvailable);Assert.IsFalse(replay.ReplayUpgradeUnlocked);
  Assert.IsTrue(chapter.room.shutter.Unlocked);Assert.IsFalse(MainGameProgress.BasicReplayUnlocked);Assert.IsFalse(chapter.ReplayAvailable);
  Assert.IsEmpty(chapter.room.checkpoints,"No statue was marked on this drawing");
  Assert.IsFalse(chapter.room.CheckpointSaved,"The entrance is a retry anchor, not an invisible statue");
  Assert.AreNotEqual("Checkpoint saved",chapter.Notice);
  Assert.AreEqual(2,chapter.plates.Length);Assert.AreEqual(1,replay.platforms.Length);
  var orange=chapter.GetComponentsInChildren<Transform>().Single(t=>t.name=="Middle orange bridge — background only");
  Assert.IsEmpty(orange.GetComponentsInChildren<Collider2D>(true));Assert.Less(orange.GetComponent<SpriteRenderer>().color.grayscale,.6f);
  var grass=chapter.GetComponentsInChildren<GrassyColumnVisual>();Assert.AreEqual(2,grass.Length);
  foreach(var column in grass){
   var solids=column.GetComponentsInChildren<Collider2D>(true);Assert.AreEqual(1,solids.Length);
   Assert.IsNotNull(solids[0].GetComponent<OneWayPlatform>());
   Assert.Less(column.GetComponentsInChildren<SpriteRenderer>().Min(s=>s.bounds.min.y),-.5f);
  }
  Assert.IsEmpty(chapter.transform.Find("Forest and waterfall scenery").GetComponentsInChildren<Collider2D>(true));
  var camera=chapter.room.view;yield return null;
  Assert.That(camera.orthographicSize*camera.aspect*2,Is.EqualTo(20).Within(.01f));
  Place(new Vector2(45,chapter.room.roomBounds.yMax+1));Act(jump:true);
  yield return null;
  Assert.LessOrEqual(camera.transform.position.y+camera.orthographicSize,chapter.room.roomBounds.yMax+.01f);
  Assert.Greater(Player.Position.y,chapter.room.roomBounds.yMax,"The top edge limits the camera, never the goat");
 }

 [UnityTest]public IEnumerator OneLogPerPressTravelsAtTwoUnitsAndContinuesAfterRelease(){
  yield return Load();Wait(60);Assert.AreEqual(0,Source.ActiveCount);
  Assert.That(Source.plateCooldown,Is.EqualTo(4),"Saved Room 4 uses a four-second log cooldown.");
  Place(Source.activationPlate.transform.position);Assert.AreEqual(1,Source.ActiveCount);
  var log=Source.pool.Single(c=>c.gameObject.activeSelf);
  float x=log.transform.position.x;Wait(100);
  Assert.That(log.transform.position.x-x,Is.EqualTo(4).Within(.001f));
  Assert.AreEqual(1,Source.ActiveCount,"Holding the plate never repeats a spawn");
  Place(chapter.room.start);x=log.transform.position.x;Wait(50);
  Assert.That(log.transform.position.x-x,Is.EqualTo(2).Within(.001f));
  Place(Source.activationPlate.transform.position);Assert.AreEqual(1,Source.ActiveCount,"Re-entering the plate before four seconds must be rejected.");
  Assert.Greater(Source.Remaining,0);
  Wait(Mathf.CeilToInt(Source.Remaining/.02f)+1);
  Assert.AreEqual(1,Source.ActiveCount,"Staying on the plate past expiry does not spawn a queued log.");
  Place(chapter.room.start);Place(Source.activationPlate.transform.position);Assert.AreEqual(2,Source.ActiveCount);
  foreach(var sr in log.GetComponentsInChildren<SpriteRenderer>())Assert.AreEqual(Color.white,sr.color);
 }

 [UnityTest]public IEnumerator ReleasedLogCanBeBoardedAndCarriesTheLiveGoat(){
  yield return Load();Place(Source.activationPlate.transform.position);
  var log=Source.pool.Single(c=>c.gameObject.activeSelf);var surface=log.GetComponent<BoxCollider2D>();
  Act(1,jump:true);
  for(int i=0;i<100&&Player.GroundCollider!=surface;i++)Act(Steer(log.transform.position.x));
  Assert.AreSame(surface,Player.GroundCollider,"The log must be boardable from the entrance bank");
  float x=Player.Position.x;
  for(int i=0;i<40;i++){Act();Assert.AreSame(surface,Player.GroundCollider);}
  Assert.That(Player.Position.x-x,Is.EqualTo(1.6f).Within(.06f));
  Assert.IsFalse(Source.activationPlate.Pressed);
 }

 [UnityTest]public IEnumerator FrozenPlateHoldsSwiftEndpointAndReleaseReturnsHome(){
  yield return Load();var platform=replay.platforms.Single();
  Assert.IsTrue(platform.returnWhenReleased);Assert.IsTrue(platform.solid.enabled);
  float home=platform.transform.position.x;Place(platform.plate.transform.position);
  Act(freeze:true);for(int i=0;i<30&&chapter.room.shutter.Snapping;i++)Act();
  Place(new Vector2(52,3));Wait(30);
  Assert.IsTrue(platform.plate.Pressed);
  Assert.That(platform.transform.position.x,Is.EqualTo(home+6).Within(.001f));
  // Stand on the physical endpoint, then clear the statue holding the plate.
  Place(new Vector2(home+6,10));Assert.AreSame(platform.solid,Player.GroundCollider);
  chapter.room.shutter.echo.Clear();Wait(30);
  Assert.IsFalse(platform.plate.Pressed);Assert.IsTrue(platform.solid.enabled);
  Assert.That(platform.transform.position.x,Is.EqualTo(home).Within(.001f));
  Assert.AreSame(platform.solid,Player.GroundCollider,"Returning motion carries riders once, using the existing motor");
  Assert.That(Player.Position.x,Is.EqualTo(home).Within(.03f));
 }

 [UnityTest]public IEnumerator RecordedReleaseRewindsTheLogAndReplaysItsAppearance(){
  // Mechanism fixture retains recording coverage; campaign Room 4 must keep it locked.
  yield return Load(rewindFixture:true);
  for(int i=0;i<13;i++)Act(record:true);Act();
  Assert.AreEqual(RewindPlaytest.Mode.Recording,replay.State);
  for(int i=0;i<90&&Player.Position.x<6.4f;i++)Act(1);
  Wait(80);Assert.AreEqual(1,Source.ActiveCount);
  Act(record:true);Act();
  for(int i=0;i<150&&replay.State==RewindPlaytest.Mode.Rewinding;i++)Act();
  Assert.AreEqual(RewindPlaytest.Mode.Ready,replay.State);Assert.AreEqual(0,Source.ActiveCount);
  Act(record:true);Act();
  for(int i=0;i<150&&Source.ActiveCount==0;i++)Act();
  Assert.AreEqual(1,Source.ActiveCount,"Forward playback restores the log's recorded birth");
  var log=Source.pool.Single(c=>c.gameObject.activeSelf);float x=log.transform.position.x;Wait(20);
  Assert.That(log.transform.position.x-x,Is.EqualTo(.8f).Within(.05f));
 }

 [UnityTest]public IEnumerator RiverRoomRejectsRecordButAllowsShutterInCampaignAndDebug(){
  for(int mode=0;mode<2;mode++){
   yield return Load(debug:mode==1);
   Assert.AreEqual(4,chapter.roomNumber);Assert.IsFalse(chapter.ReplayAvailable);
   Assert.AreEqual(CameraModules.Shutter,CameraEquipment.For(chapter));
   var lift=chapter.room.lift;Assert.IsNotNull(lift);Assert.IsTrue(lift.arrival);
   Assert.AreEqual(new Vector2(2,1),lift.dock);Assert.IsFalse(lift.Riding);
   Assert.That(Player.Position.y,Is.EqualTo(1).Within(.02f));
   for(int i=0;i<50;i++)Act(record:true);
   Act();Assert.AreEqual(RewindPlaytest.Mode.Idle,replay.State);Assert.IsNull(replay.Memory);
   Assert.AreEqual(0,replay.HoldProgress);Assert.IsFalse(replay.ReplayUpgradeUnlocked);
   Act(freeze:true);Assert.IsTrue(chapter.room.shutter.HasSnapshot);
   for(int i=0;i<30&&chapter.room.shutter.Snapping;i++)Act();
   Assert.IsFalse(chapter.room.shutter.Snapping);Assert.AreEqual(ChapterOneRoom.Phase.Playing,chapter.CurrentPhase);
  }
 }

 [UnityTest]public IEnumerator RiverExitWalksIntoTheReplayAcquisitionRoom(){
  yield return Load();Assert.AreEqual("Main_Room05",chapter.nextRoomScene);
  Assert.IsTrue(chapter.room.lift.arrival,"The left lift is only an arrival, never the right exit");
  Place(new Vector2(95.5f,12));Assert.AreEqual(ChapterOneRoom.Phase.Exiting,chapter.CurrentPhase);
  float x=Player.Position.x;Wait(25);
  Assert.Greater(Player.Position.x,x+.5f);Assert.Greater(chapter.Fade,0);
  for(int i=0;i<150&&chapter!=null&&chapter.CurrentPhase==ChapterOneRoom.Phase.Exiting;i++)Act();
  yield return null;
  Assert.AreEqual("Main_Room05",SceneManager.GetActiveScene().name);
  var next=Object.FindFirstObjectByType<Room4AbilityTutorial>();Assert.IsNotNull(next);next.AutoTick=false;
  Assert.IsNull(next.room.lift,"The acquisition room continues the horizontal walk-in");
  Assert.IsFalse(MainGameProgress.BasicReplayUnlocked);Assert.IsFalse(next.Unlocked);
  for(int i=0;i<200&&next.CurrentPhase==Room4AbilityTutorial.Phase.Arriving;i++)next.Step(default,false,.02f);
  Assert.AreEqual(Room4AbilityTutorial.Phase.Approach,next.CurrentPhase);
  Assert.That(next.room.player.Position.y,Is.EqualTo(4).Within(.02f));
 }

}
}
#endif
