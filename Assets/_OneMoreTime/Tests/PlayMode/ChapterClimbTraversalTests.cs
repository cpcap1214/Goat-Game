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
public class ChapterClimbTraversalTests {
 ChapterOneRoom chapter;
 PlayerMotor2D player;

 IEnumerator Load() {
  MainGameProgress.StartOver();
  // Preserve the original climb as a fixed motor/ability regression fixture.
  EditorSceneManager.LoadSceneInPlayMode("Assets/_OneMoreTime/Tests/Fixtures/Main_Room08.unity",
   new LoadSceneParameters(LoadSceneMode.Single));
  yield return null;
  chapter = Object.FindFirstObjectByType<ChapterOneRoom>();
  Assert.IsNotNull(chapter);
  chapter.AutoTick = chapter.room.AutoTick = chapter.replay.AutoTick = false;
  player = chapter.room.player;
  for (int i = 0; i < 300 && chapter.CurrentPhase == ChapterOneRoom.Phase.Entering; i++) Step();
  Assert.AreEqual(ChapterOneRoom.Phase.Playing, chapter.CurrentPhase);
 }

 void Step(float move = 0, bool jump = false, bool freeze = false) {
  chapter.Step(new InputFrame { move = move, jump = jump, held = true, freeze = freeze }, false);
  Physics2D.SyncTransforms();
 }

 void Place(Vector2 feet) {
  player.ResetAt(feet + Vector2.up * .01f);
  Physics2D.SyncTransforms();
  for (int i = 0; i < 3; i++) Step();
  Assert.IsTrue(player.Grounded, "The fixture must begin on an actual authored platform");
 }

 float SteerTo(float x) => Mathf.Abs(x - player.Position.x) > .18f ? Mathf.Sign(x - player.Position.x) : 0;

 BoxCollider2D Surface(string name) => chapter.GetComponentsInChildren<BoxCollider2D>(true).Single(c => c.name == name);

 [UnityTest] public IEnumerator FrozenPlateLetsGoatMountRideAndLeaveServicePlatform() {
  yield return Load();
  var shuttle = chapter.replay.platforms.Single(p => p.name == "08 Plate-powered vertical service platform");
  var arrival = Surface("08 Crumbling upper service landing");
  var plate = shuttle.plate;

  // Set up only the starting rest. Mounting, carrying and the exit jump below
  // use real input through the same chapter clock as gameplay.
  Place(plate.transform.position);
  Step(freeze: true);
  Assert.IsTrue(chapter.room.shutter.HasSnapshot);
  for (int i = 0; i < 30 && chapter.room.shutter.Snapping; i++) Step();
  Assert.IsFalse(chapter.room.shutter.Snapping);
  Assert.IsTrue(chapter.room.shutter.echo.Pose.grounded);

  Step(1, jump: true);
  Assert.Greater(player.Velocity.y, 0);
  for (int i = 0; i < 100 && player.GroundCollider != shuttle.solid; i++) Step(SteerTo(27));
  Assert.AreSame(shuttle.solid, player.GroundCollider, "The goat must jump onto the moving platform from the plate rest");
  Assert.IsTrue(plate.Pressed, "The frozen goat must keep the plate active after the player leaves");
  float boardedAt = player.Position.y;

  for (int i = 0; i < 220 && shuttle.solid.bounds.max.y < 17.3f; i++) {
   Step();
   Assert.AreSame(shuttle.solid, player.GroundCollider);
   Assert.That(player.Position.y, Is.EqualTo(shuttle.solid.bounds.max.y).Within(.025f));
  }
  Assert.That(player.Position.y, Is.GreaterThan(boardedAt + 3.5f), "The real motor must be carried up the shaft");
  Assert.That(shuttle.solid.bounds.max.y, Is.GreaterThanOrEqualTo(17.3f));

  Step(1, jump: true);
  Assert.Greater(player.Velocity.y, 0);
  for (int i = 0; i < 110 && player.GroundCollider != arrival; i++) Step(SteerTo(arrival.bounds.center.x));
  Assert.AreSame(arrival, player.GroundCollider, "The top of the shuttle must connect to the arrival beam");
  Assert.That(player.Position.y, Is.EqualTo(arrival.bounds.max.y).Within(.025f));
  Assert.AreEqual(ChapterOneRoom.Phase.Playing, chapter.CurrentPhase);
 }

 [UnityTest] public IEnumerator HighStreamBankReachesForestLedgeWithNormalJump() {
  yield return Load();
  var balcony = Surface("08 High stream east bank");
  var beam = Surface("08 Last forest ledge");
  Place(new Vector2(balcony.bounds.max.x - .4f, balcony.bounds.max.y));
  Assert.AreSame(balcony, player.GroundCollider);
  Assert.IsFalse(chapter.room.shutter.HasSnapshot);
  Assert.IsNull(chapter.replay.Memory);

  Step(1, jump: true);
  Assert.Greater(player.Velocity.y, 0);
  for (int i = 0; i < 100 && player.GroundCollider != beam; i++) Step(SteerTo(beam.bounds.min.x + .8f));
  Assert.AreSame(beam, player.GroundCollider, "The 2.5-unit rise from the stream bank must be reachable without an afterimage");
  Assert.That(player.Position.y, Is.EqualTo(beam.bounds.max.y).Within(.025f));
  Assert.AreEqual(ChapterOneRoom.Phase.Playing, chapter.CurrentPhase);
 }
}
}
#endif
