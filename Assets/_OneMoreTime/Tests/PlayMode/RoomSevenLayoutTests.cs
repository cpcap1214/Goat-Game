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
public class RoomSevenLayoutTests {
 ChapterOneRoom chapter;
 RewindPlaytest replay;
 PlayerMotor2D Player=>chapter.room.player;
 PlatformDispatcher Dispatcher=>replay.participants.OfType<PlatformDispatcher>().Single();
 RoomPlatform Paired=>replay.platforms.Single(p=>p.latchWhenAllPressed);
 float arrivalStart;

 IEnumerator Load(){
  MainGameProgress.StartOver();
  EditorSceneManager.LoadSceneInPlayMode("Assets/_OneMoreTime/Scenes/Main_Room07.unity",new LoadSceneParameters(LoadSceneMode.Single));
  yield return null;
  chapter=Object.FindFirstObjectByType<ChapterOneRoom>();Assert.IsNotNull(chapter);
  replay=chapter.replay;chapter.AutoTick=chapter.room.AutoTick=replay.AutoTick=false;
  arrivalStart=Player.Position.y;
  for(int i=0;i<300&&chapter.CurrentPhase==ChapterOneRoom.Phase.Entering;i++)Act();
  Assert.AreEqual(ChapterOneRoom.Phase.Playing,chapter.CurrentPhase);
 }
 void Act(float move=0,bool jump=false,bool record=false,bool shutter=false){
  chapter.Step(new InputFrame{move=move,jump=jump,held=true,freeze=shutter},record);Physics2D.SyncTransforms();
 }
 void Wait(int ticks){for(int i=0;i<ticks;i++)Act();}
 BoxCollider2D Box(string name)=>chapter.transform.Find("Route and mechanisms/"+name).GetComponentInChildren<BoxCollider2D>();
 void FinishPlatformReveal(){for(int i=0;i<120&&chapter.platformReveal.Revealing;i++)Act();Assert.IsFalse(chapter.platformReveal.Revealing);Act();}
 // Explicit placement isolates mechanisms; these checks do not claim a full traversal.
 void Place(Vector2 position){Player.ResetAt(position+Vector2.up*.01f);Physics2D.SyncTransforms();Wait(6);}
 void GroundAt(Vector2 position){
  var state=Player.CaptureRewind();state.position=position;state.velocity=Vector2.zero;
  state.grounded=true;state.onEcho=false;state.ground=null;Player.RestoreRewind(state);Physics2D.SyncTransforms();
 }
 void Record(int ticks=30){for(int i=0;i<13;i++)Act(record:true);Act();Assert.AreEqual(RewindPlaytest.Mode.Recording,replay.State);Wait(ticks);}
 void Finish(){
  Act(record:true);Act();
  for(int i=0;i<150&&replay.State==RewindPlaytest.Mode.Rewinding;i++)Act();
  Assert.AreEqual(RewindPlaytest.Mode.Ready,replay.State);Assert.IsTrue(replay.AwaitingReplay);
 }
 void Reveal(){
  GroundAt((Vector2)chapter.trapdoor.plate.transform.position-Vector2.up*.058f);Act();
  Assert.IsTrue(chapter.trapdoor.Revealing);
  for(int i=0;i<120&&chapter.trapdoor.Revealing;i++)Act();
  Assert.IsTrue(chapter.trapdoor.Opened);Assert.IsFalse(chapter.trapdoor.Revealing);
 }

 [UnityTest]public IEnumerator OfficialTowerArrivesByLiftAndKeepsItsTwentyUnitCameraFixedHorizontally(){
  yield return Load();
  Assert.AreEqual(new Rect(0,-3.5f,20,68.5f),chapter.room.roomBounds);
  Assert.IsTrue(chapter.room.lift.arrival);Assert.IsTrue(chapter.room.lift.automaticGate);
  Assert.Less(arrivalStart,0);Assert.That(Player.Position.y,Is.EqualTo(0).Within(.01f));
  Assert.AreEqual(2,chapter.room.checkpoints.Length);Assert.AreEqual(4,chapter.plates.Length);
  Assert.IsFalse(chapter.FilterAvailable);Assert.IsFalse(replay.ReplayUpgradeUnlocked);
  Assert.IsTrue(chapter.room.shutter.Unlocked);Assert.IsTrue(MainGameProgress.BasicReplayUnlocked);
  Assert.AreEqual(3,replay.platforms.Count(p=>p.motion==RoomPlatform.Motion.ClosedRoute&&p.cycleSeconds==6));
  Assert.AreEqual(6,replay.platforms.Count(p=>p.motion==RoomPlatform.Motion.PingPong&&p.cycleSeconds==4));
  foreach(var position in new[]{new Vector2(2,3),new Vector2(18,40),new Vector2(10,70)}){
   Player.ResetAt(position);chapter.SendMessage("LateUpdate");
   var camera=chapter.room.view;
   Assert.That(camera.transform.position.x,Is.EqualTo(10).Within(.001f));
   Assert.That(camera.orthographicSize*camera.aspect*2,Is.EqualTo(20).Within(.001f));
   Assert.LessOrEqual(camera.transform.position.y+camera.orthographicSize,chapter.room.roomBounds.yMax+.001f);
   Assert.GreaterOrEqual(camera.transform.position.y-camera.orthographicSize,chapter.room.roomBounds.yMin-.001f);
  }
  Assert.Greater(Player.Position.y,chapter.room.roomBounds.yMax);
  Assert.IsEmpty(Physics2D.RaycastAll(Player.Position,Vector2.up,5,Player.worldMask).Where(h=>!h.collider.isTrigger).ToArray());
 }

 [UnityTest]public IEnumerator AuthoredLoopsTakeSixSecondsAndBackAndForthDecksTakeFourSeconds(){
  yield return Load();
  var loops=replay.platforms.Where(p=>p.motion==RoomPlatform.Motion.ClosedRoute).ToArray();
  var shuttles=replay.platforms.Where(p=>p.cycleSeconds==4).ToArray();
  var loopStarts=loops.Select(p=>p.transform.position).ToArray();
  var shuttleStarts=shuttles.Select(p=>p.transform.position).ToArray();
  Wait(100);
  for(int i=0;i<shuttles.Length;i++)Assert.That(Vector3.Distance(shuttleStarts[i],shuttles[i].transform.position),Is.EqualTo(shuttles[i].travel.magnitude).Within(.01f));
  Wait(100);
  for(int i=0;i<shuttles.Length;i++)Assert.Less(Vector3.Distance(shuttleStarts[i],shuttles[i].transform.position),.01f);
  Wait(100);
  for(int i=0;i<loops.Length;i++)Assert.Less(Vector3.Distance(loopStarts[i],loops[i].transform.position),.01f);
 }

 [UnityTest]public IEnumerator LiveGoatAndShutterUnlockTheTwoPlateDeckPermanently(){
  yield return Load();var paired=Paired;var west=paired.requiredPlates[0];var east=paired.requiredPlates[1];
  Assert.AreEqual("0/2",paired.counter.text);Assert.AreEqual(Color.white,paired.counter.color);Assert.IsFalse(paired.Active);
  var beforeUnlock=paired.CaptureRewind();
  Place(new Vector2(west.transform.position.x,15.6f));
  Assert.IsTrue(chapter.platformReveal.Revealing);FinishPlatformReveal();
  Assert.AreEqual("1/2",paired.counter.text);Assert.AreEqual(Color.white,paired.counter.color);Assert.IsFalse(paired.Active);
  Act(shutter:true);for(int i=0;i<30&&chapter.room.shutter.Snapping;i++)Act();
  Assert.IsTrue(chapter.room.shutter.echo.Active);Assert.IsTrue(chapter.room.shutter.echo.Pose.grounded);
  Place(new Vector2(east.transform.position.x,15.6f));
  Assert.IsTrue(west.Pressed);Assert.IsTrue(east.Pressed);Assert.IsTrue(paired.ActivationLatched);Assert.IsTrue(paired.Active);
  Assert.AreEqual("2/2",paired.counter.text);Assert.Greater(paired.counter.color.g,paired.counter.color.r);
  chapter.room.shutter.echo.Clear();Place(chapter.room.start);
  Assert.IsFalse(west.Pressed);Assert.IsFalse(east.Pressed);Assert.IsTrue(paired.Active);
  paired.RestoreRewind(beforeUnlock);
  Assert.IsTrue(paired.ActivationLatched);Assert.IsTrue(paired.Active);Assert.AreEqual("2/2",paired.counter.text);
  replay.Retry();Act();
  Assert.IsTrue(paired.ActivationLatched);Assert.IsTrue(paired.solid.enabled);Assert.AreEqual("2/2",paired.counter.text);
 }

 [UnityTest]public IEnumerator DispatcherEnforcesFourSecondPressCooldownAndWaitsWithRecordedWorld(){
  yield return Load();var source=Dispatcher;Assert.AreEqual(4,source.cooldown);
  Place(new Vector2(source.activationPlate.transform.position.x,42.6f));
  Assert.AreEqual(1,source.pool.Count(p=>p.InTransit));Wait(210);
  Assert.AreEqual(1,source.pool.Count(p=>p.InTransit),"Holding the plate does not send additional decks.");
  Place(new Vector2(7.6f,42.6f));Place(new Vector2(source.activationPlate.transform.position.x,42.6f));
  Assert.AreEqual(2,source.pool.Count(p=>p.InTransit));
  Place(new Vector2(7.6f,42.6f));Place(new Vector2(source.activationPlate.transform.position.x,42.6f));
  Assert.AreEqual(2,source.pool.Count(p=>p.InTransit),"A second press inside the four-second cooldown does not launch.");
  Place(new Vector2(7.6f,42.6f));Record(20);Finish();
  float remaining=source.Remaining;Assert.Greater(remaining,0);
  var positions=source.pool.Select(p=>p.transform.position).ToArray();Wait(80);
  Assert.AreEqual(remaining,source.Remaining);Assert.IsTrue(replay.AwaitingReplay);
  for(int i=0;i<positions.Length;i++)Assert.AreEqual(positions[i],source.pool[i].transform.position);
  Act(record:true);Act();Wait(10);
  Assert.IsFalse(replay.AwaitingReplay);Assert.Less(source.Remaining,remaining);
  Assert.Greater(Vector3.Distance(positions[0],source.pool[0].transform.position),.2f);
 }

 [UnityTest]public IEnumerator FinishedDispatchedDeckSupportsTheGoatUntilItsLaunchPlateIsPressedAgain(){
  yield return Load();var source=Dispatcher;
  Place(new Vector2(source.activationPlate.transform.position.x,42.6f));Wait(1000);
  var deck=source.pool[0];var endpoint=new Vector2(2.2f,60);
  Assert.IsFalse(deck.InTransit);Assert.IsTrue(deck.Active);Assert.IsTrue(deck.solid.enabled);
  Assert.IsTrue(deck.artwork.All(art=>art.enabled));
  Assert.That(deck.solid.bounds.center.x,Is.EqualTo(endpoint.x).Within(.001f));
  Assert.That(deck.solid.bounds.max.y,Is.EqualTo(endpoint.y).Within(.001f));
  GroundAt(endpoint);Wait(100);
  Assert.IsTrue(Player.Grounded);Assert.That(Vector2.Distance(endpoint,Player.Position),Is.LessThan(.05f));
  Assert.IsTrue(deck.Active);Assert.AreEqual(Vector2.zero,deck.Delta);
  Place(new Vector2(source.activationPlate.transform.position.x,42.6f));
  Assert.IsTrue(deck.InTransit);Assert.IsTrue(deck.Active);
  Assert.That(deck.solid.bounds.max.y,Is.EqualTo(45.5f).Within(.001f));
  Assert.That(deck.transform.position.x,Is.InRange(1.2f,1.6f));
 }

 [UnityTest]public IEnumerator TrapdoorRevealPausesRecordingPlayerAndEveryDeckThenRestoresTheCamera(){
  yield return Load();Place(new Vector2(7.6f,42.6f));Record(10);
  var trap=chapter.trapdoor;Assert.IsFalse(trap.plate.affectedByRewind);
  GroundAt((Vector2)trap.plate.transform.position-Vector2.up*.058f);
  chapter.SendMessage("LateUpdate");float normalSize=chapter.room.view.orthographicSize;
  var position=Player.Position;var platforms=replay.platforms.Select(p=>p.transform.position).ToArray();
  float recorded=replay.RecordSeconds;
  Act(move:1,jump:true,record:true,shutter:true);Assert.IsTrue(trap.Revealing);
  for(int i=0;i<25;i++)Act(move:1,record:true);
  chapter.SendMessage("LateUpdate");
  Assert.That(chapter.room.view.transform.position.x,Is.EqualTo(10));
  Assert.That(chapter.room.view.transform.position.y,Is.EqualTo(Mathf.Min(trap.targetPoint.y,chapter.room.roomBounds.yMax-chapter.room.view.orthographicSize)).Within(.01f));
  Assert.Less(chapter.room.view.orthographicSize,normalSize);
  float revealProgress=trap.RevealProgress;replay.SetPaused(true);Wait(10);
  Assert.AreEqual(revealProgress,trap.RevealProgress,"The player's pause menu must pause the reveal as well.");
  replay.SetPaused(false);
  for(int i=0;i<120&&trap.Revealing;i++){Act(move:1,record:true);chapter.SendMessage("LateUpdate");Assert.LessOrEqual(chapter.room.view.transform.position.y+chapter.room.view.orthographicSize,65.001f);}
  Assert.IsTrue(trap.Opened);Assert.AreEqual(position,Player.Position);Assert.AreEqual(recorded,replay.RecordSeconds);
  for(int i=0;i<platforms.Length;i++)Assert.AreEqual(platforms[i],replay.platforms[i].transform.position);
  Assert.IsFalse(chapter.room.shutter.echo.Active,"A shutter press during the reveal must be ignored.");
  chapter.SendMessage("LateUpdate");Assert.That(chapter.room.view.orthographicSize,Is.EqualTo(normalSize).Within(.001f));
  Assert.That(chapter.room.view.transform.position.x,Is.EqualTo(10));
  Act();Assert.Greater(replay.RecordSeconds,recorded);
 }

 [UnityTest]public IEnumerator EitherPairedPlateRevealsOnceBeforeCountersOrRecordedGameplayAdvance(){
  for(int trigger=0;trigger<2;trigger++){
   yield return Load();Record(10);
   var reveal=chapter.platformReveal;Assert.IsNotNull(reveal);Assert.AreSame(Paired,reveal.platform);
   CollectionAssert.AreEquivalent(Paired.requiredPlates,reveal.triggeringPlates);
   GroundAt((Vector2)Paired.requiredPlates[trigger].transform.position-Vector2.up*.058f);
   var player=Player.Position;float recorded=replay.RecordSeconds;
   var positions=replay.platforms.Select(p=>p.transform.position).ToArray();
   Act(move:1,jump:true,record:true,shutter:true);Assert.IsTrue(reveal.Revealing);
   for(int i=0;i<120&&reveal.Revealing;i++){
    Assert.AreEqual("0/2",Paired.counter.text,"The counter waits for the camera to return.");
    Assert.IsFalse(Paired.ActivationLatched);Act(move:1,record:true);
   }
   Assert.IsTrue(reveal.Shown);Assert.IsFalse(reveal.Revealing);
   Assert.AreEqual(player,Player.Position);Assert.AreEqual(recorded,replay.RecordSeconds);
   for(int i=0;i<positions.Length;i++)Assert.AreEqual(positions[i],replay.platforms[i].transform.position);
   Assert.IsFalse(chapter.room.shutter.echo.Active);
   Act();Assert.AreEqual("1/2",Paired.counter.text);Assert.Greater(replay.RecordSeconds,recorded);
   GroundAt((Vector2)Paired.requiredPlates[1-trigger].transform.position-Vector2.up*.058f);Act();
   Assert.IsFalse(reveal.Revealing,"The other plate cannot repeat the one-time reveal.");
  }
 }

 [UnityTest]public IEnumerator DispatchedDeckFitsTheCenteredSlotButItsRiderCannotPassThroughTheWalls(){
  yield return Load();var source=Dispatcher;Assert.AreEqual(4,source.cooldown);
  foreach(var item in source.pool)Assert.AreEqual(3,item.speed,"Only dispatched decks slow from four to three units per second.");
  var deck=source.pool[0];Assert.IsTrue(deck.Launch());Physics2D.SyncTransforms();
  var west=Box("07 Upper pool west wall");var middle=Box("07 Overhead middle block");var east=Box("07 Zigzag east wall");
  Assert.That(west.bounds.max.y,Is.EqualTo(45.1f).Within(.001f));Assert.That(middle.bounds.min.y,Is.EqualTo(45.7f).Within(.001f));
  Assert.That(deck.solid.bounds.center.y,Is.EqualTo((west.bounds.max.y+middle.bounds.min.y)/2).Within(.02f));
  Assert.Greater(deck.solid.bounds.min.y,west.bounds.max.y);Assert.Less(deck.solid.bounds.max.y,middle.bounds.min.y);
  Assert.That(east.bounds.min.y,Is.EqualTo(46).Within(.001f));
  Assert.Less(deck.solid.bounds.max.y,east.bounds.min.y);Assert.Greater(deck.solid.bounds.max.y+Player.tuning.height,east.bounds.min.y);
  var start=deck.transform.position;deck.Tick(1);Assert.That(deck.transform.position.x-start.x,Is.EqualTo(3).Within(.001f));
  foreach(var obstacle in new[]{middle,east}){
   var state=deck.CaptureRewind();state.position=new Vector2(obstacle.bounds.min.x-1.3f,state.home.y);state.distance=state.position.x-state.home.x;
   state.delta=Vector2.zero;state.active=state.wasActive=state.inTransit=true;state.previousTop=state.position.y+deck.solid.size.y/2;
   deck.RestoreRewind(state);Physics2D.SyncTransforms();Player.ResetAt(new Vector2(state.position.x,deck.solid.bounds.max.y+.01f));
   for(int i=0;i<5;i++)Player.Step(default,.02f,false,default,default,new[]{deck});
   Assert.AreSame(deck.solid,Player.GroundCollider);
   float furthest=Player.Position.x;
   for(int i=0;i<110&&deck.transform.position.x<obstacle.bounds.max.x+.5f;i++){
    deck.Tick(.02f);Physics2D.SyncTransforms();Player.Step(default,.02f,false,default,default,new[]{deck});furthest=Mathf.Max(furthest,Player.Position.x);
   }
   Assert.Greater(deck.transform.position.x,obstacle.bounds.max.x+.45f,"The empty deck passes the obstruction.");
   Assert.LessOrEqual(furthest,obstacle.bounds.min.x-Player.tuning.width/2+.02f,"Platform carry must not push the goat through the wall.");
  }
  foreach(string name in new[]{"07 Upper west small deck","07 Upper middle small deck"})Assert.That(Box(name).bounds.max.y,Is.EqualTo(56.1f).Within(.001f));
  Assert.That(chapter.trapdoor.leftCollider.bounds.max.y,Is.EqualTo(62.7f).Within(.001f));
  Assert.That(Box("07 Exit shaft stepping deck").bounds.max.y,Is.EqualTo(63.7f).Within(.001f));
 }

 [UnityTest]public IEnumerator JumpingAboveTheUpperBankKeepsTheCameraAtItsTopBoundary(){
  yield return Load();GroundAt(new Vector2(2,65));chapter.SendMessage("LateUpdate");
  var camera=chapter.room.view;float fixedY=camera.transform.position.y,apex=Player.Position.y;
  Act(jump:true);
  for(int i=0;i<55;i++){
   Act();apex=Mathf.Max(apex,Player.Position.y);chapter.SendMessage("LateUpdate");
   Assert.That(camera.transform.position.y,Is.EqualTo(fixedY).Within(.001f));
   Assert.That(camera.transform.position.y+camera.orthographicSize,Is.EqualTo(65).Within(.001f));
  }
  Assert.Greater(apex,67.5f,"The camera boundary must not become a ceiling for the goat.");
 }

 [UnityTest]public IEnumerator TrapdoorCanOpenWhileWaitingForReplayAndSurvivesRestoringTheClosedRecording(){
  yield return Load();Place(new Vector2(7.6f,42.6f));Record();Finish();
  Assert.IsTrue(replay.AwaitingReplay);Assert.IsFalse(chapter.trapdoor.Opened);
  Reveal();Assert.IsTrue(replay.AwaitingReplay);
  replay.Retry();Act();
  Assert.IsTrue(replay.AwaitingReplay);Assert.IsTrue(chapter.trapdoor.Opened);
  Assert.IsFalse(chapter.trapdoor.leftCollider.enabled);Assert.IsFalse(chapter.trapdoor.rightCollider.enabled);
  GroundAt((Vector2)chapter.trapdoor.plate.transform.position-Vector2.up*.058f);Act();
  Assert.IsFalse(chapter.trapdoor.Revealing,"Restored plate states must never repeat the one-time camera reveal.");
 }

 [UnityTest]public IEnumerator OpenedShaftAcceptsAnAirborneExitAndTransitionsUpIntoRoomEight(){
  yield return Load();Assert.AreEqual("Main_Room08",chapter.nextRoomScene);Assert.IsTrue(chapter.exitUpwards);
  Player.ResetAt(new Vector2(9.5f,64.8f));Physics2D.SyncTransforms();Act();
  Assert.AreEqual(ChapterOneRoom.Phase.Playing,chapter.CurrentPhase,"A closed trapdoor cannot be bypassed by an airborne exit trigger.");
  Reveal();Player.ResetAt(new Vector2(9.5f,64.8f));Physics2D.SyncTransforms();Act();
  Assert.AreEqual(ChapterOneRoom.Phase.Exiting,chapter.CurrentPhase);Assert.IsFalse(Player.Grounded);
  float start=Player.Position.y;Wait(25);
  Assert.That(Player.Position.y-start,Is.EqualTo(1.5f).Within(.001f));Assert.Greater(chapter.Fade,0);
  for(int i=0;i<100&&chapter!=null&&chapter.CurrentPhase==ChapterOneRoom.Phase.Exiting;i++)Act();
  yield return null;Assert.AreEqual("Main_Room08",SceneManager.GetActiveScene().name);
  var next=Object.FindFirstObjectByType<ChapterOneRoom>();next.AutoTick=false;
 }
}
}
#endif
