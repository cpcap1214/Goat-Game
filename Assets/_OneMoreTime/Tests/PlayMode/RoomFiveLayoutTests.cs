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
public class RoomFiveLayoutTests {
 ChapterOneRoom chapter;
 RewindPlaytest replay;

 IEnumerator Load(bool debug=false) {
  if(debug)MainGameProgress.BeginDebug();else MainGameProgress.StartOver();
  EditorSceneManager.LoadSceneInPlayMode("Assets/_OneMoreTime/Scenes/Main_Room06.unity",new LoadSceneParameters(LoadSceneMode.Single));
  yield return null;
  chapter=Object.FindFirstObjectByType<ChapterOneRoom>();
  Assert.IsNotNull(chapter);
  replay=chapter.replay;
  chapter.AutoTick=false;chapter.room.AutoTick=false;replay.AutoTick=false;
  for(int i=0;i<300&&chapter.CurrentPhase==ChapterOneRoom.Phase.Entering;i++)chapter.Step(default,false);
  Assert.AreEqual(ChapterOneRoom.Phase.Playing,chapter.CurrentPhase,"The authored walk-in must reach the starting bank");
 }

 void Step(int ticks) { for(int i=0;i<ticks;i++)chapter.Step(default,false); }
 void Act(float move=0,bool jump=false,bool freeze=false) {
  chapter.Step(new InputFrame{move=move,jump=jump,held=true,freeze=freeze},false);
  Physics2D.SyncTransforms();
 }
 void FinishShutterSnap() {
  for(int i=0;i<30&&chapter.room.shutter.Snapping;i++)Act();
  Assert.IsFalse(chapter.room.shutter.Snapping);
 }
 float SteerTo(float x)=>Mathf.Abs(x-replay.player.Position.x)>.15f?Mathf.Sign(x-replay.player.Position.x):0;
 CrateSpawner Source()=>replay.participants.OfType<CrateSpawner>().Single();
 FloatingObject2D FirstCrate() {
  var source=Source();
  for(int i=0;i<5&&source.ActiveCount==0;i++)Step(1);
  Assert.AreEqual(1,source.ActiveCount,"The stream should begin with a single crate");
  return source.pool.Single(c=>c.gameObject.activeSelf);
 }

 [UnityTest] public IEnumerator AuthoredRoomUsesBasicAbilitiesAndDoesNotIntroduceAnEraRoute() {
  for(int mode=0;mode<2;mode++) {
   yield return Load(mode==1);
   Assert.AreEqual(new Rect(0,-2,90,26),chapter.room.roomBounds);
   Assert.That(chapter.room.start.x,Is.EqualTo(2.5f).Within(.01f));
   Assert.That(chapter.room.start.y,Is.EqualTo(8).Within(.01f));
   Assert.IsTrue(chapter.room.shutter.Unlocked);
   Assert.IsTrue(MainGameProgress.BasicReplayUnlocked);
   Assert.IsFalse(chapter.FilterAvailable,"Orange spans are background scenery in this layout, not an era-switch puzzle");
   Assert.IsFalse(chapter.TrySwitchEra());
   Assert.IsFalse(replay.ReplayUpgradeUnlocked);
   Assert.IsEmpty(chapter.eraGroups);
   Assert.AreEqual("Main_Room07",chapter.nextRoomScene);
   var orange=chapter.GetComponentsInChildren<Transform>().Where(t=>t.name.EndsWith(" — background only")).ToArray();
   Assert.AreEqual(7,orange.Length);
   foreach(var span in orange) {
    Assert.IsEmpty(span.GetComponentsInChildren<Collider2D>(true));
    var art=span.GetComponent<SpriteRenderer>();Assert.IsNotNull(art);
    Assert.Less(art.sortingOrder,0);Assert.Less(art.color.grayscale,.6f);
   }
  }
 }

 [UnityTest] public IEnumerator GrassColumnsHaveOnlyTopCollisionAndTheirSoilExtendsBelowWater() {
  yield return Load();
  var columns=chapter.GetComponentsInChildren<GrassyColumnVisual>();
  Assert.AreEqual(2,columns.Length,"Only the two grass islands in the supplied layout are added");
  foreach(var column in columns) {
   float top=column.transform.position.y+column.topOffset;
   float surface=top>7?4:0;
   Assert.That(top,top>7?Is.EqualTo(8).Within(.01f):Is.EqualTo(5).Within(.01f));
   var colliders=column.GetComponentsInChildren<Collider2D>(true);
   Assert.AreEqual(1,colliders.Length,"Dirt extending down the column must remain visual only");
   var deck=colliders[0] as BoxCollider2D;Assert.IsNotNull(deck);
   Assert.IsNotNull(deck.GetComponent<OneWayPlatform>());
   Assert.That(deck.size.y,Is.EqualTo(.18f).Within(.001f));
   var soil=column.GetComponentsInChildren<SpriteRenderer>().Where(s=>s.enabled&&s.sortingOrder<0).ToArray();
   Assert.IsNotEmpty(soil);
   Assert.Less(soil.Min(s=>s.bounds.min.y),surface-.2f,"Background dirt must disappear beneath the nearby waterline");
  }
  var scenery=chapter.transform.Find("Forest and waterfall scenery");Assert.IsNotNull(scenery);
  Assert.IsEmpty(scenery.GetComponentsInChildren<Collider2D>(true),"Bridge scaffolding and scenery must not become walls");
  var supports=scenery.GetComponentsInChildren<Transform>().Where(t=>t.name.EndsWith(" — scaffold wall art")).ToArray();
  Assert.IsNotEmpty(supports);
  foreach(var support in supports) {
   var frames=support.GetComponentsInChildren<SpriteRenderer>();Assert.IsNotEmpty(frames);
   Assert.IsEmpty(support.GetComponentsInChildren<OneWayPlatform>(true));
  }
  Assert.That(supports.SelectMany(s=>s.GetComponentsInChildren<SpriteRenderer>()).Min(s=>s.bounds.min.y),Is.LessThanOrEqualTo(-2.9f),"Upper bridge supports share the lower scaffold, which continues below the river");
  Assert.AreEqual(1,chapter.room.checkpoints.Length);
  var checkpoint=chapter.room.checkpoints.Single();
  Assert.That(checkpoint.transform.position.x,Is.EqualTo(44.5f).Within(.01f));
  Assert.That(checkpoint.transform.position.y,Is.EqualTo(5).Within(.01f));
  replay.player.ResetAt((Vector2)checkpoint.transform.position+Vector2.up*.01f);Physics2D.SyncTransforms();
  Step(6);Assert.IsTrue(checkpoint.Saved);Assert.IsTrue(chapter.room.CheckpointSaved);
 }

 [UnityTest] public IEnumerator ShuttleRunsOnlyWhileThePlayerOrShutterHoldsItsPlate() {
  yield return Load();
  var plate=chapter.plates.Single();var platform=replay.platforms.Single();
  Assert.AreSame(plate,platform.plate);Assert.AreEqual(0,platform.releaseDelay);Assert.IsNull(platform.counter);
  Assert.That(platform.travel.x,Is.EqualTo(7).Within(.01f));Assert.AreEqual(0,platform.travel.y);
  Step(1);Assert.IsFalse(platform.Active);Assert.IsFalse(platform.solid.enabled);
  replay.player.ResetAt((Vector2)plate.transform.position+Vector2.up*.01f);Physics2D.SyncTransforms();
  Step(6);Assert.IsTrue(plate.Pressed);Assert.IsTrue(platform.Active);Assert.IsTrue(platform.solid.enabled);
  Act(freeze:true);Assert.IsTrue(chapter.room.shutter.HasSnapshot);FinishShutterSnap();
  Assert.IsTrue(chapter.room.shutter.echo.Pose.grounded,"The actual shutter must preserve contact with the plate");
  replay.player.ResetAt(chapter.room.start);Physics2D.SyncTransforms();
  float before=platform.transform.position.x;
  Step(100);Assert.IsTrue(plate.Pressed);Assert.Greater(platform.transform.position.x,before+1);
  // Replacing the snapshot away from the plate is the real player's release action.
  Act(freeze:true);FinishShutterSnap();Step(1);
  Assert.IsFalse(plate.Pressed);Assert.IsFalse(platform.Active);Assert.IsFalse(platform.solid.enabled);
  Assert.AreEqual(0,platform.Remaining);
  Vector2 stopped=platform.transform.position;Step(20);
  Assert.AreEqual(stopped,(Vector2)platform.transform.position,"Releasing the plate stops the shuttle immediately without a timer");
 }

 [UnityTest] public IEnumerator ShutterOnPlateLetsGoatBoardRideAndLeaveShuttleForTheCheckpoint() {
  yield return Load();
  var player=replay.player;var platform=replay.platforms.Single();var plate=platform.plate;
  var arrival=chapter.GetComponentsInChildren<BoxCollider2D>().Single(c=>c.name=="Lower red approach");
  var checkpoint=chapter.room.checkpoints.Single();
  // Setup only: begin at the authored plate. All subsequent movement, carrying,
  // shutter capture and checkpoint activation use actual gameplay input/clock.
  player.ResetAt((Vector2)plate.transform.position+Vector2.up*.01f);Physics2D.SyncTransforms();
  Step(6);Assert.IsTrue(player.Grounded);Assert.IsTrue(plate.Pressed);
  Act(freeze:true);FinishShutterSnap();
  Assert.IsTrue(chapter.room.shutter.HasSnapshot);Assert.IsTrue(chapter.room.shutter.echo.Pose.grounded);

  // Use the grass island's right edge to catch the shuttle as it moves away.
  for(int i=0;i<30&&player.Position.x<25.1f;i++)Act(1);
  Assert.IsTrue(player.Grounded,"The takeoff must still be on the grass island");
  Act(1,jump:true);Assert.Greater(player.Velocity.y,0);
  for(int i=0;i<100&&player.GroundCollider!=platform.solid;i++)Act(SteerTo(platform.solid.bounds.center.x));
  Assert.AreSame(platform.solid,player.GroundCollider,"A real jump must mount the moving shuttle from the plate island");
  Assert.IsTrue(plate.Pressed,"The frozen goat must keep the shuttle active while the player rides");
  float boardedAt=player.Position.x;
  for(int i=0;i<220&&platform.solid.bounds.center.x<35.75f;i++) {
   Act();Assert.AreSame(platform.solid,player.GroundCollider);
  }
  Assert.Greater(player.Position.x,boardedAt+3,"The platform must carry the live goat horizontally");
  Assert.GreaterOrEqual(platform.solid.bounds.center.x,35.75f);

  // Walk toward the deck's front before jumping down onto the marked red span.
  for(int i=0;i<30&&player.Position.x<platform.solid.bounds.center.x+.5f;i++) {
   Act(1);Assert.AreSame(platform.solid,player.GroundCollider);
  }
  Act(1,jump:true);Assert.Greater(player.Velocity.y,0);
  for(int i=0;i<110&&player.GroundCollider!=arrival;i++)Act(SteerTo(41.9f));
  Assert.AreSame(arrival,player.GroundCollider,"The shuttle must connect to the lower red bridge without an extra ledge");
  Assert.That(player.Position.y,Is.EqualTo(5).Within(.025f));
  for(int i=0;i<80&&!checkpoint.Saved;i++)Act(SteerTo(checkpoint.transform.position.x));
  Assert.IsTrue(checkpoint.Saved);Assert.IsTrue(chapter.room.CheckpointSaved);
  Assert.AreEqual(ChapterOneRoom.Phase.Playing,chapter.CurrentPhase);
  Assert.AreEqual(0,MainGameProgress.DeathCount,"The route must finish without a hidden death or respawn");
 }

 [UnityTest] public IEnumerator CratesTravelAtTwoPointFiveUnitsPerSecondAndSpawnTenSecondsApart() {
  yield return Load();
  var source=Source();Assert.AreEqual(10,source.spawnInterval);Assert.AreEqual(2.5f,source.water.velocity);
  Assert.AreEqual(2.5f,source.initialHorizontalVelocity);
  Assert.That(source.spawnPoint.position.x,Is.EqualTo(33.5f).Within(.01f));
  var first=FirstCrate();float start=first.transform.position.x;
  float remaining=source.Remaining;
  Step(50);Assert.That(first.transform.position.x-start,Is.EqualTo(2.5f).Within(.01f));
  Assert.AreEqual(1,source.ActiveCount);
  int ticks=50;
  while(source.ActiveCount==1&&ticks<505){Step(1);ticks++;}
  Assert.AreEqual(2,source.ActiveCount,"The second crate should appear before the first reaches the whirlpool");
  Assert.That(ticks*RewindPlaytest.StepSeconds,Is.EqualTo(remaining).Within(.041f));
  var second=source.pool.Single(c=>c.gameObject.activeSelf&&c!=first);
  Assert.That(second.transform.position.x,Is.InRange(33.5f,33.61f));
 }

 [UnityTest] public IEnumerator TheAuthoredWhirlpoolPullsCratesDownBeforeRemovingThem() {
  yield return Load();var source=Source();var crate=FirstCrate();
  Assert.IsTrue(source.disposal.sinkBeforeDisposal);Assert.IsTrue(source.disposal.disposeToRight);
  Assert.That(source.disposal.boundaryX,Is.EqualTo(60).Within(.01f));
  var eddies=chapter.GetComponentsInChildren<SpriteRenderer>(true).Where(sr=>sr.sprite!=null&&sr.sprite.name.StartsWith("SurfaceEddy")).ToArray();
  Assert.AreEqual(1,eddies.Length,"Room 6 retains only its authored crate drain");
  Assert.That(eddies[0].transform.position.x,Is.EqualTo(source.disposal.boundaryX).Within(.01f));
  for(int i=0;i<600&&!crate.Sinking;i++)Step(1);
  Assert.IsTrue(crate.Sinking);Assert.IsTrue(crate.gameObject.activeSelf,"The crate must visibly descend instead of vanishing at the surface");
  Assert.That(crate.transform.position.x,Is.EqualTo(60).Within(.1f));
  float surfaceHeight=crate.transform.position.y;Step(10);
  Assert.IsTrue(crate.gameObject.activeSelf);Assert.Less(crate.transform.position.y,surfaceHeight-.1f);
  for(int i=0;i<150&&crate.gameObject.activeSelf;i++)Step(1);
  Assert.IsFalse(crate.gameObject.activeSelf,"The whirlpool must eventually return the crate to the pool");
 }

 [UnityTest] public IEnumerator CameraKeepsTwentyUnitWidthAndAnOpenUpperBoundary() {
  yield return Load();var room=chapter.room;
  room.player.ResetAt(new Vector2(room.roomBounds.center.x,room.roomBounds.yMax+3));
  Physics2D.SyncTransforms();chapter.SendMessage("LateUpdate");
  Assert.That(room.view.orthographicSize*room.view.aspect*2,Is.EqualTo(20).Within(.001f));
  Assert.That(room.view.transform.position.y+room.view.orthographicSize,Is.LessThanOrEqualTo(room.roomBounds.yMax+.001f));
  Assert.Greater(room.player.Position.y,room.view.transform.position.y+room.view.orthographicSize);
  var hits=Physics2D.RaycastAll(room.player.Position,Vector2.up,5,room.player.worldMask);
  Assert.IsEmpty(hits.Where(h=>!h.collider.isTrigger).ToArray(),"The camera boundary must not stop the goat's head");
 }
 [UnityTest] public IEnumerator BridgeExitLiftAscendsIntoRoomSeven() {
  yield return Load();var lift=chapter.room.lift;
  Assert.IsNotNull(lift);Assert.IsFalse(lift.arrival);Assert.IsTrue(lift.automaticGate);Assert.IsNull(lift.plate);
  Assert.AreEqual(new Vector2(88,9),lift.dock);Assert.AreEqual("Main_Room07",chapter.nextRoomScene);
  replay.player.ResetAt(lift.dock+Vector2.up*.01f);Physics2D.SyncTransforms();Step(3);
  Assert.AreEqual(ChapterOneRoom.Phase.Exiting,chapter.CurrentPhase);Assert.IsTrue(lift.Riding);
  float y=replay.player.Position.y;Step(50);
  Assert.Greater(replay.player.Position.y,y+3.8f);Assert.Greater(chapter.Fade,0);
  for(int i=0;i<150&&chapter!=null&&chapter.CurrentPhase==ChapterOneRoom.Phase.Exiting;i++)Act();
  yield return null;
  Assert.AreEqual("Main_Room07",SceneManager.GetActiveScene().name);
  var next=Object.FindFirstObjectByType<ChapterOneRoom>();Assert.IsNotNull(next);next.AutoTick=false;
  Assert.AreEqual(7,next.roomNumber);Assert.IsTrue(MainGameProgress.BasicReplayUnlocked);
 }

}
}
#endif
