#if UNITY_EDITOR
using NUnit.Framework;
using UnityEngine;

namespace OneMoreTime.Tests {
[Category("ChapterOneRegression")]
public class RoomTrapdoorTests {
 GameObject fixture;
 RoomTrapdoor door;
 Vector3 leftStart,rightStart;
 [SetUp]public void Setup(){
  fixture=new GameObject("Room 7 trapdoor fixture");
  door=fixture.AddComponent<RoomTrapdoor>();
  door.plate=Child("Upper-left pressure plate").AddComponent<PressurePlate>();
  door.plate.affectedByRewind=false;
  door.leftLeaf=Child("Left leaf").transform;door.leftLeaf.localPosition=new Vector3(8.75f,62,0);
  door.rightLeaf=Child("Right leaf").transform;door.rightLeaf.localPosition=new Vector3(10.25f,62,0);
  door.leftCollider=door.leftLeaf.gameObject.AddComponent<BoxCollider2D>();
  door.rightCollider=door.rightLeaf.gameObject.AddComponent<BoxCollider2D>();
  door.targetPoint=new Vector2(9.5f,62);door.leafTravel=1.5f;
  leftStart=door.leftLeaf.localPosition;rightStart=door.rightLeaf.localPosition;
 }
 [TearDown]public void Cleanup(){Object.DestroyImmediate(fixture);}
 GameObject Child(string name){var item=new GameObject(name);item.transform.SetParent(fixture.transform);return item;}

 [Test]public void RevealFreezesFirstThenOpensBothHalvesAndLatchesAfterRelease(){
  Assert.IsFalse(door.Tick(.02f));Assert.IsFalse(door.Triggered);
  door.plate.RestoreRewind(1);
  Assert.IsTrue(door.Tick(.2f));Assert.IsTrue(door.Revealing);
  Assert.AreEqual(leftStart,door.leftLeaf.localPosition);Assert.AreEqual(rightStart,door.rightLeaf.localPosition);
  Assert.IsTrue(door.leftCollider.enabled);Assert.IsTrue(door.rightCollider.enabled);
  door.plate.RestoreRewind(0);door.Tick(.5f);
  Assert.Less(door.leftLeaf.localPosition.x,leftStart.x);Assert.Greater(door.rightLeaf.localPosition.x,rightStart.x);
  Assert.IsFalse(door.Opened);
  door.Tick(.3f);Assert.IsTrue(door.Opened);Assert.IsTrue(door.Revealing);
  Assert.That(door.leftLeaf.localPosition.x,Is.EqualTo(leftStart.x-1.5f).Within(.0001f));
  Assert.That(door.rightLeaf.localPosition.x,Is.EqualTo(rightStart.x+1.5f).Within(.0001f));
  Assert.IsFalse(door.leftCollider.enabled);Assert.IsFalse(door.rightCollider.enabled);
  Assert.IsTrue(door.Tick(1.1f),"The completion frame still freezes all gameplay clocks.");
  Assert.IsFalse(door.Revealing);Assert.IsTrue(door.Opened);
  Assert.IsFalse(door.Tick(.02f));
 }

 [Test]public void PlateOrRewindStateChangesCannotCloseOrRepeatTheReveal(){
  door.plate.RestoreRewind(1);door.Tick(3);
  Assert.IsTrue(door.Opened);Assert.IsFalse(RoomTrapdoor.affectedByRewind);
  Assert.IsNull(door.GetComponent<RewindParticipant>());
  for(int i=0;i<3;i++){
   door.plate.ResetState();Assert.IsFalse(door.Tick(.02f));
   door.plate.RestoreRewind(2);Assert.IsFalse(door.Tick(.02f));
   Assert.IsTrue(door.Opened);Assert.IsFalse(door.leftCollider.enabled);Assert.IsFalse(door.rightCollider.enabled);
  }
 }

 [Test]public void CameraFocusesVerticallyAndZoomsThenReturnsWithoutHorizontalTravel(){
  var view=Child("Camera").AddComponent<Camera>();view.orthographic=true;
  var normal=new Vector3(10,52,-10);const float normalSize=5.625f;
  door.plate.RestoreRewind(1);door.Tick(.225f);
  door.ApplyCamera(view,normal,normalSize);
  Assert.That(view.transform.position.x,Is.EqualTo(10));
  Assert.That(view.transform.position.y,Is.EqualTo(57).Within(.0001f));
  Assert.That(view.orthographicSize,Is.EqualTo((normalSize+3.8f)/2).Within(.0001f));
  door.Tick(.725f);door.ApplyCamera(view,normal,normalSize);
  Assert.AreEqual(new Vector3(10,62,-10),view.transform.position);
  Assert.That(view.orthographicSize,Is.EqualTo(3.8f));
  door.Tick(.825f);door.ApplyCamera(view,normal,normalSize);
  Assert.That(view.transform.position.x,Is.EqualTo(10));
  Assert.That(view.transform.position.y,Is.EqualTo(57).Within(.0001f));
  door.Tick(.2251f);door.ApplyCamera(view,normal,normalSize);
  Assert.AreEqual(normal,view.transform.position);Assert.That(view.orthographicSize,Is.EqualTo(normalSize));
 }

 [Test]public void SharedClockControlsEveryRevealPhaseWithoutAutonomousUpdates(){
  door.plate.RestoreRewind(1);door.Tick(.45f);
  Assert.AreEqual(0,door.OpenAmount);Assert.IsTrue(door.Revealing);
  for(int i=0;i<25;i++)door.Tick(.02f);
  Assert.That(door.OpenAmount,Is.EqualTo(1).Within(.0001f));
  // A larger fixed-step catch-up still completes exactly at the open endpoint.
  door.Tick(10);Assert.That(door.RevealProgress,Is.EqualTo(1));
  Assert.IsTrue(door.Opened);Assert.IsFalse(door.Revealing);
  Assert.AreEqual(leftStart+Vector3.left*1.5f,door.leftLeaf.localPosition);
  Assert.AreEqual(rightStart+Vector3.right*1.5f,door.rightLeaf.localPosition);
 }
}
}
#endif
