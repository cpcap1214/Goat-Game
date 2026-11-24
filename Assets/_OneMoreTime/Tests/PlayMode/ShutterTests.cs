#if UNITY_EDITOR
using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
namespace OneMoreTime.Tests {
[Category("ChapterOneRegression")]
public class ShutterTests {
 MainRoom room;
 void Step(float move=0,bool jump=false,bool freeze=false,bool retry=false){room.Step(new InputFrame{move=move,jump=jump,held=true,freeze=freeze,retry=retry},.02f);Physics2D.SyncTransforms();}
 void Walk(float x){for(int i=0;i<700&&Mathf.Abs(room.player.Position.x-x)>.08f;i++)Step(Mathf.Sign(x-room.player.Position.x));for(int i=0;i<8;i++)Step();Assert.That(room.player.Position.x,Is.EqualTo(x).Within(.3f));}
 void Jump(float x,float y){Step(0,true);for(int i=0;i<160;i++){float dx=x-room.player.Position.x;Step(Mathf.Abs(dx)>.06f?Mathf.Sign(dx):0);if(room.player.Grounded&&i>5)break;}Assert.That(room.player.Position.y,Is.EqualTo(y).Within(.04f),"landing "+room.player.Position);}
 void Snap(){
  var position=room.player.Position;var velocity=room.player.Velocity;int deaths=room.Deaths;
  Step(freeze:true);Assert.IsTrue(room.shutter.Snapping);Assert.AreEqual(position,room.shutter.Position);
  for(int i=0;i<30&&room.shutter.Snapping;i++){Step(1,true,true,true);Assert.AreEqual(position,room.player.Position);Assert.AreEqual(velocity,room.player.Velocity);}
  Assert.IsFalse(room.shutter.Snapping);Assert.IsTrue(room.shutter.HasSnapshot);Assert.AreEqual(position,room.shutter.Position);Assert.AreEqual(deaths,room.Deaths);Assert.IsFalse(room.shutter.echo.support.enabled);
 }
 void LandOnCopy(){for(int i=0;i<90&&!room.player.OnEcho;i++)Step();Assert.IsTrue(room.player.OnEcho,"land on frozen copy at "+room.shutter.Position+" from "+room.player.Position);}
 [UnityTest]public IEnumerator RoomTwoShutterFreezesInPlaceAndReachesCheckpoint(){
  EditorSceneManager.LoadSceneInPlayMode("Assets/_OneMoreTime/Scenes/"+MainRoom.RoomTwoScene+".unity",new LoadSceneParameters(LoadSceneMode.Single));yield return null;
  room=Object.FindAnyObjectByType<MainRoom>();room.AutoTick=false;
  for(int i=0;i<160&&room.Phase==MainRoom.RoomPhase.Entering;i++)Step();Assert.IsFalse(room.shutter.Press());
  Walk(12.4f);Assert.IsTrue(room.shutter.Unlocked);Assert.IsFalse(room.shutter.pickup.gameObject.activeSelf);
  Walk(17.5f);Step(jump:true);for(int i=0;i<6;i++)Step();Snap();LandOnCopy();Jump(20,5);Walk(24.3f);Assert.IsTrue(room.CheckpointSaved);
  var saved=room.shutter.Position;Step(retry:true);Assert.AreEqual(saved,room.shutter.Position);Assert.That(room.player.Position.x,Is.EqualTo(24.3f).Within(.01f));
  Walk(28.5f);for(int i=0;i<100&&room.Deaths==0;i++)Step();Assert.AreEqual(1,room.Deaths);Assert.AreEqual(saved,room.shutter.Position);Assert.IsTrue(room.shutter.Unlocked);
  Assert.IsTrue(room.IsReviving);for(int i=0;i<80&&room.IsReviving;i++)Step();Assert.IsFalse(room.IsReviving);
  var before=room.player.Position;Snap();Assert.AreEqual(before,room.shutter.Position);Assert.AreNotEqual(saved,room.shutter.Position);Assert.AreEqual(before,room.player.Position);
  // Exit animation fixture only: the exact 9-unit sketch pit has not been validated as crossable.
  room.player.ResetAt(new Vector2(37,5));
  for(int i=0;i<200&&room.Phase==MainRoom.RoomPhase.Playing;i++)Step(1);Assert.AreEqual(MainRoom.RoomPhase.Exiting,room.Phase);
  for(int i=0;i<100&&room.Phase==MainRoom.RoomPhase.Exiting;i++)Step();Assert.AreEqual(MainRoom.RoomPhase.Complete,room.Phase);Assert.Greater(room.player.Position.x,room.roomBounds.xMax);
 }
 [UnityTest]public IEnumerator RoomOneExitLoadsRoomTwo(){
  EditorSceneManager.LoadSceneInPlayMode("Assets/_OneMoreTime/Scenes/"+MainRoom.SceneName+".unity",new LoadSceneParameters(LoadSceneMode.Single));yield return null;
  room=Object.FindAnyObjectByType<MainRoom>();room.AutoTick=false;for(int i=0;i<160&&room.Phase==MainRoom.RoomPhase.Entering;i++)Step();
  // Transition-only fixture; full Room 1 traversal is covered by MainRoomTests.
  room.player.ResetAt(new Vector2(room.exitX,room.exitFloor));for(int i=0;i<100&&room.Phase!=MainRoom.RoomPhase.Complete;i++)Step();yield return null;
  Assert.AreEqual(MainRoom.RoomTwoScene,SceneManager.GetActiveScene().name);Assert.IsNotNull(Object.FindAnyObjectByType<ShutterAbility>());
 }
 [UnityTest]public IEnumerator ShutterWorksAfterRoomFiveAcquisitionCreatesItsFirstReplay(){
  MainGameProgress.StartOver();
  EditorSceneManager.LoadSceneInPlayMode("Assets/_OneMoreTime/Scenes/Main_Room05.unity",new LoadSceneParameters(LoadSceneMode.Single));yield return null;
  var tutorial=Object.FindFirstObjectByType<Room4AbilityTutorial>();tutorial.AutoTick=false;room=tutorial.room;var replay=tutorial.replay;
  for(int i=0;i<180&&tutorial.CurrentPhase==Room4AbilityTutorial.Phase.Arriving;i++)tutorial.Step(default,false,.02f);
  for(int i=0;i<250&&tutorial.CurrentPhase==Room4AbilityTutorial.Phase.Approach;i++)tutorial.Step(new InputFrame{move=1},false,.02f);
  for(int i=0;i<500&&tutorial.CurrentPhase!=Room4AbilityTutorial.Phase.AwaitRecord;i++)tutorial.Step(default,false,.02f);
  Assert.AreEqual(Room4AbilityTutorial.Phase.AwaitRecord,tutorial.CurrentPhase);
  for(int i=0;i<13;i++)tutorial.Step(default,true,.02f);
  for(int i=0;i<120&&tutorial.CurrentPhase!=Room4AbilityTutorial.Phase.Playing;i++)tutorial.Step(default,false,.02f);
  Assert.AreEqual(Room4AbilityTutorial.Phase.Playing,tutorial.CurrentPhase);Assert.IsNotNull(replay.Memory);Assert.IsFalse(replay.ReplayUpgradeUnlocked);
  var position=room.player.Position;var velocity=room.player.Velocity;
  tutorial.Step(new InputFrame{freeze=true},false,.02f);
  Assert.IsTrue(room.shutter.Snapping,"The acquisition's saved replay must not permanently disable shutter");
  Assert.IsTrue(room.shutter.HasSnapshot);Assert.IsNull(replay.Memory);Assert.IsFalse(replay.echo.Active);
  Assert.AreEqual(RewindPlaytest.Mode.Idle,replay.State);Assert.AreEqual(position,room.player.Position);Assert.AreEqual(velocity,room.player.Velocity);
  for(int i=0;i<25&&room.shutter.Snapping;i++)tutorial.Step(default,false,.02f);
  tutorial.Step(new InputFrame{retry=true},false,.02f);
  Assert.AreEqual(position,room.shutter.Position,"Retry retains the selected shutter memory");Assert.IsNull(replay.Memory);
 }
 [UnityTest]public IEnumerator UpgradedReplayCanSwitchToShutterAndBackWithoutTwoMemories(){
  MainGameProgress.BeginDebug();
  EditorSceneManager.LoadSceneInPlayMode("Assets/_OneMoreTime/Scenes/Main_Room06.unity",new LoadSceneParameters(LoadSceneMode.Single));yield return null;
  var chapter=Object.FindFirstObjectByType<ChapterOneRoom>();chapter.AutoTick=false;room=chapter.room;var replay=chapter.replay;
  for(int i=0;i<300&&chapter.CurrentPhase==ChapterOneRoom.Phase.Entering;i++)chapter.Step(default,false);
  Assert.AreEqual(ChapterOneRoom.Phase.Playing,chapter.CurrentPhase);
  // Explicit developer unlock exercises the same upgraded replay engine used in Debug Mode.
  replay.UnlockReplayUpgrade();
  for(int i=0;i<13;i++)chapter.Step(default,true);chapter.Step(default,false);
  for(int i=0;i<45;i++)chapter.Step(default,false);
  chapter.Step(new InputFrame{freeze=true},false);
  Assert.AreEqual(RewindPlaytest.Mode.Recording,replay.State);Assert.IsFalse(room.shutter.HasSnapshot,"Do not discard an unfinished recording");
  chapter.Step(default,true);chapter.Step(default,false);Assert.AreEqual(RewindPlaytest.Mode.Rewinding,replay.State);
  var memory=replay.Memory;chapter.Step(new InputFrame{freeze=true},false);
  Assert.AreSame(memory,replay.Memory);Assert.IsFalse(room.shutter.HasSnapshot,"World rewind must finish before a shutter capture");
  for(int i=0;i<120&&replay.State==RewindPlaytest.Mode.Rewinding;i++)chapter.Step(default,false);
  chapter.Step(default,true);chapter.Step(default,false);for(int i=0;i<15;i++)chapter.Step(default,false);
  chapter.Step(default,false,false,true,true);
  Assert.IsTrue(replay.PlaybackBackwards);Assert.IsTrue(replay.PlaybackHeld);Assert.IsTrue(replay.echo.Active);
  replay.SetPaused(true);Assert.IsFalse(replay.TryUseShutter(room.shutter));
  Assert.AreSame(memory,replay.Memory);Assert.IsFalse(room.shutter.HasSnapshot);
  replay.SetPaused(false);var position=room.player.Position;var velocity=room.player.Velocity;
  chapter.Step(new InputFrame{freeze=true},false);
  Assert.IsTrue(room.shutter.Snapping);Assert.IsTrue(room.shutter.HasSnapshot);Assert.IsNull(replay.Memory);Assert.IsFalse(replay.echo.Active);
  Assert.AreEqual(RewindPlaytest.Mode.Idle,replay.State);Assert.IsFalse(replay.PlaybackBackwards);Assert.IsFalse(replay.PlaybackHeld);
  Assert.IsTrue(replay.ReplayUpgradeUnlocked,"Changing memory type must not revoke an acquired debug upgrade");
  Assert.AreEqual(position,room.player.Position);Assert.AreEqual(velocity,room.player.Velocity);
  Assert.AreEqual(0,replay.path.GetComponent<LineRenderer>().positionCount);
  var traces=replay.transform.Find("Object replay traces");Assert.IsTrue(traces==null||!traces.gameObject.activeInHierarchy,"Discarded object paths must disappear with their replay");
  for(int i=0;i<25&&room.shutter.Snapping;i++)chapter.Step(default,false);
  for(int i=0;i<13;i++)chapter.Step(default,true);
  Assert.AreEqual(RewindPlaytest.Mode.Recording,replay.State);Assert.IsNull(replay.Memory);Assert.IsFalse(room.shutter.HasSnapshot);
  chapter.Step(default,false);for(int i=0;i<20;i++)chapter.Step(default,false);
  chapter.Step(default,true);chapter.Step(default,false);
  for(int i=0;i<120&&replay.State==RewindPlaytest.Mode.Rewinding;i++)chapter.Step(default,false);
  Assert.AreEqual(RewindPlaytest.Mode.Ready,replay.State);Assert.IsNotNull(replay.Memory);Assert.IsFalse(room.shutter.HasSnapshot);
 }
}
}
#endif
