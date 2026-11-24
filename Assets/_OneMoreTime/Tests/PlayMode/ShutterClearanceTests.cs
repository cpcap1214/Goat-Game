#if UNITY_EDITOR
using System.Collections;
using NUnit.Framework;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

namespace OneMoreTime.Tests {
[Category("ChapterOneRegression")]
public class ShutterClearanceTests {
 MainRoom room;ChapterOneRoom chapter;
 PlayerMotor2D Player=>room.player;
 ShutterAbility Shutter=>room.shutter;
 IEnumerator Load(int number){
  MainGameProgress.StartOver();
  EditorSceneManager.LoadSceneInPlayMode("Assets/_OneMoreTime/Scenes/Main_Room"+number.ToString("00")+".unity",new LoadSceneParameters(LoadSceneMode.Single));yield return null;
  room=Object.FindFirstObjectByType<MainRoom>();chapter=Object.FindFirstObjectByType<ChapterOneRoom>();room.AutoTick=false;
  if(chapter!=null){chapter.AutoTick=false;chapter.replay.AutoTick=false;}
  for(int i=0;i<300&&(chapter!=null?chapter.CurrentPhase==ChapterOneRoom.Phase.Entering:room.Phase==MainRoom.RoomPhase.Entering);i++)Act();
  if(!Shutter.Unlocked){Player.ResetAt(Shutter.pickup.position);Shutter.CheckPickup();}
  Assert.IsTrue(Shutter.Unlocked);Player.ResetAt(room.start+Vector2.up*.01f);Physics2D.SyncTransforms();for(int i=0;i<6;i++)Act();
 }
 void Act(bool jump=false,bool freeze=false,bool record=false){
  var command=new InputFrame{held=true,jump=jump,freeze=freeze};
  if(chapter!=null)chapter.Step(command,record);else room.Step(command,.02f);
  Physics2D.SyncTransforms();
 }
 void Snap(){
  var before=Player.Position;Act(freeze:true);Assert.IsTrue(Shutter.Snapping);Assert.IsTrue(Shutter.HasSnapshot);
  for(int i=0;i<25&&Shutter.Snapping;i++)Act();
  Assert.IsFalse(Shutter.Snapping);Assert.AreEqual(before,Player.Position);Assert.IsTrue(Shutter.echo.SupportPendingClearance);Assert.IsFalse(Shutter.SupportReady);
 }

 [UnityTest]public IEnumerator ReplacingCopyUnderStationaryFeetNeverRaisesGoatInEitherMainClock(){
  foreach(int roomNumber in new[]{2,5}){
   yield return Load(roomNumber);Snap();float floor=Player.Position.y;
   Act(jump:true);for(int i=0;i<140&&!Player.OnEcho;i++)Act();
   Assert.IsTrue(Player.OnEcho,"A normal jump fully clears and then lands on the frozen goat.");
   Assert.That(Player.Position.y,Is.EqualTo(floor+Player.tuning.backHeight).Within(.02f));Assert.IsTrue(Shutter.SupportReady);
   float highest=Player.Position.y;
   for(int replacement=0;replacement<8;replacement++){
    float before=Player.Position.y;Snap();Assert.IsFalse(Player.OnEcho,"The old carrier must be released at replacement.");
    Act();Assert.LessOrEqual(Player.Position.y,before+.001f,"Replacing a copy without moving cannot climb its new back height.");
    Assert.IsFalse(Player.OnEcho);Assert.IsFalse(Shutter.SupportReady);
   }
   Assert.LessOrEqual(Player.Position.y,highest+.001f);
  }
 }

 [UnityTest]public IEnumerator WholeBodyMustClearTheNewCopyAndAnArmedCopyStaysUsableOnReturn(){
  yield return Load(6);Snap();Vector2 captured=Shutter.Position;
  float horizontalClearance=(Player.tuning.width+Mathf.Max(Player.tuning.width,Player.tuning.backWidth))*.5f;
  Player.ResetAt(captured+Vector2.right*(horizontalClearance-.01f));Assert.IsFalse(Shutter.SupportReady,"Partial horizontal separation is still overlap.");
  Player.ResetAt(captured+Vector2.right*(horizontalClearance+.02f));Assert.IsTrue(Shutter.SupportReady);Assert.IsFalse(Shutter.echo.SupportPendingClearance);
  Player.ResetAt(captured);Assert.IsTrue(Shutter.SupportReady,"Clearing arms this copy permanently until replacement.");
  Snap();Player.ResetAt(captured+Vector2.up*(Player.tuning.height-.01f));Assert.IsFalse(Shutter.SupportReady,"The goat's feet must clear the top of the captured body.");
  Player.ResetAt(captured+Vector2.up*(Player.tuning.height+.02f));Assert.IsTrue(Shutter.SupportReady);
  Player.ResetAt(captured);Snap();Player.ResetAt(captured-Vector2.up*(Player.tuning.height-.01f));Assert.IsFalse(Shutter.SupportReady);
  Player.ResetAt(captured-Vector2.up*(Player.tuning.height+.02f));Assert.IsTrue(Shutter.SupportReady,"Falling entirely below the copy also clears it.");
 }

 [UnityTest]public IEnumerator NewlyCapturedCopyHoldsItsPressurePlateWhilePlayerSupportIsBlocked(){
  yield return Load(6);var plate=chapter.plates[0];var state=Player.CaptureRewind();state.position=plate.transform.position;state.velocity=Vector2.zero;state.grounded=true;state.onEcho=false;state.ground=null;Player.RestoreRewind(state);Physics2D.SyncTransforms();
  Act(freeze:true);Assert.IsTrue(Shutter.Snapping);Assert.IsTrue(Shutter.echo.Active);Assert.IsTrue(Shutter.echo.Pose.grounded);Assert.IsFalse(Shutter.SupportReady);
  plate.Evaluate(Player,Shutter.echo);Assert.AreEqual(2,plate.Sources,"Live goat and frozen copy both count without waiting for support to arm.");
  Player.ResetAt((Vector2)plate.transform.position+Vector2.right*3);plate.Evaluate(Player,Shutter.echo);Assert.AreEqual(1,plate.Sources);Assert.IsTrue(plate.Pressed);
 }

 [UnityTest]public IEnumerator RecordedReplayRemainsImmediatelyUsableAndSwitchingToShutterStartsNewClearance(){
  yield return Load(6);var replay=chapter.replay;
  for(int i=0;i<13;i++)Act(record:true);Act();for(int i=0;i<20;i++)Act();Assert.AreEqual(RewindPlaytest.Mode.Recording,replay.State);
  Act(record:true);Act();for(int i=0;i<100&&replay.State==RewindPlaytest.Mode.Rewinding;i++)Act();Assert.AreEqual(RewindPlaytest.Mode.Ready,replay.State);
  Act(record:true);Act();Assert.IsTrue(replay.echo.Active);Player.ResetAt(replay.echo.Pose.position);
  Assert.IsTrue(replay.echo.CanSupport(Player));Assert.IsFalse(replay.echo.SupportPendingClearance,"Recorded replay keeps its existing support semantics.");
  Snap();Assert.IsFalse(replay.echo.Active);Assert.IsNull(replay.Memory);Assert.IsTrue(Shutter.echo.Active);Assert.IsFalse(Shutter.SupportReady);
 }
}
}
#endif
