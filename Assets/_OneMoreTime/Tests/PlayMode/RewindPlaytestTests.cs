#if UNITY_EDITOR
using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using UnityEditor.SceneManagement;
namespace OneMoreTime.Tests {
public class RewindPlaytestTests {
 RewindPlaytest room;
 IEnumerator Load(bool upgraded=true){EditorSceneManager.LoadSceneInPlayMode("Assets/_OneMoreTime/Scenes/Debug_Rewind.unity",new LoadSceneParameters(LoadSceneMode.Single));yield return null;room=Object.FindAnyObjectByType<RewindPlaytest>();room.AutoTick=false;if(upgraded)room.UnlockReplayUpgrade();}
 void Step(bool q=false,float move=0,bool jump=false){room.Step(new InputFrame{move=move,jump=jump,held=true},q);}
 void Hold(){for(int i=0;i<25;i++)Step(true);Assert.AreEqual(RewindPlaytest.Mode.Recording,room.State);Step();}
 void Finish(){Step(true);Step();Assert.AreEqual(RewindPlaytest.Mode.Rewinding,room.State);for(int i=0;i<110&&room.State==RewindPlaytest.Mode.Rewinding;i++)Step();Assert.AreEqual(RewindPlaytest.Mode.Ready,room.State);}
 [UnityTest]public IEnumerator HoldThresholdReleaseAndOverwrite(){
  yield return Load();for(int i=0;i<24;i++)Step(true);Assert.AreEqual(RewindPlaytest.Mode.Idle,room.State);Assert.IsTrue(room.Charging);Assert.That(room.HoldProgress,Is.EqualTo(.96f).Within(.001f));Step(true);Assert.AreEqual(RewindPlaytest.Mode.Recording,room.State);Assert.IsFalse(room.Charging);Step();Assert.AreEqual(RewindPlaytest.Mode.Recording,room.State);
  for(int i=0;i<50;i++)Step(move:1);Finish();var memory=room.Memory;Assert.NotNull(memory);Assert.Greater(room.path.GetComponent<LineRenderer>().positionCount,2);
  Step(true);Step();Assert.AreEqual(RewindPlaytest.Mode.Playing,room.State);Assert.IsFalse(room.echo.support.enabled);Assert.Greater(room.PlaybackProgress,0);Assert.AreEqual(RewindPlaytest.ReplayColor,room.echo.visual.renderer2D.color);Assert.Greater(room.path.GetComponent<LineRenderer>().startColor.r,room.path.GetComponent<LineRenderer>().startColor.b);
  for(int i=0;i<25;i++)Step(true);Assert.AreEqual(RewindPlaytest.Mode.Recording,room.State);Assert.IsNull(room.Memory);Assert.IsFalse(room.echo.Active);Assert.AreEqual(0,room.path.GetComponent<LineRenderer>().positionCount);Step();Assert.AreEqual(RewindPlaytest.Mode.Recording,room.State);
 }
 [UnityTest]public IEnumerator TapQRestartsMidPlaybackWithoutReplacingMemory(){
  yield return Load(false);Hold();for(int i=0;i<80;i++)Step(move:1);Finish();var saved=room.Memory;
  Step(true);Step();for(int i=0;i<35;i++)Step();var position=room.player.Position;
  Step(true);Step();Assert.AreSame(saved,room.Memory);Assert.AreEqual(RewindPlaytest.Mode.Playing,room.State);Assert.That(room.PlaybackSeconds,Is.EqualTo(.02f).Within(.001f));Assert.AreEqual(position,room.player.Position);
  room.UnlockReplayUpgrade();for(int i=0;i<35;i++)Step();room.Step(default,false,true);Assert.IsTrue(room.PlaybackBackwards);
  Step(true);Step();Assert.IsFalse(room.PlaybackBackwards);Assert.That(room.PlaybackSeconds,Is.EqualTo(.02f).Within(.001f));Assert.AreSame(saved,room.Memory);
 }
 [UnityTest]public IEnumerator ReverseRestoresRecordingStartAndMovingObjects(){
  yield return Load();room.player.ResetAt(new Vector2(6.9f,0));Step();Hold();var start=room.Memory;var original=room.player.Position;var crate=room.crates[0].transform.position;var platform=room.platforms[0].CaptureRewind();
  for(int i=0;i<80;i++)Step(move:1);Assert.Greater(room.crates[0].transform.position.x,crate.x+.5f);float endX=room.player.Position.x;
  Step(true);Step();Assert.AreEqual(RewindPlaytest.Mode.Rewinding,room.State);Assert.AreEqual(RewindPlaytest.ReplayColor,room.visual.renderer2D.color);Assert.Greater(room.player.Position.x,original.x+.5f,"Visible rewind must take multiple ticks");
  for(int i=0;i<110&&room.State==RewindPlaytest.Mode.Rewinding;i++)Step();Assert.AreEqual(RewindPlaytest.Mode.Ready,room.State);
  Assert.AreEqual(Color.white,room.visual.renderer2D.color);Assert.That(room.player.Position.x,Is.EqualTo(original.x).Within(.001f));Assert.AreNotEqual(room.spawn.x,room.player.Position.x);Assert.That(Vector3.Distance(crate,room.crates[0].transform.position),Is.LessThan(.001f));
  // The two ticks after the first captured frame are deliberately excluded.
  Assert.That(Vector2.Distance(platform.position,room.platforms[0].transform.position),Is.LessThan(.07f));
 }
 [UnityTest]public IEnumerator OptOutLeavesObjectAtItsEndPosition(){
  yield return Load();room.crates[0].affectedByRewind=false;room.player.ResetAt(new Vector2(6.9f,0));Step();Hold();for(int i=0;i<60;i++)Step(move:1);var end=room.crates[0].transform.position;Finish();Assert.AreEqual(end,room.crates[0].transform.position);
 }
 [UnityTest]public IEnumerator TimeLimitPauseAndPersistentPlayback(){
  yield return Load();Hold();room.SetPaused(true);float time=room.RecordSeconds;var p=room.platforms[0].transform.position;for(int i=0;i<30;i++)Step();Assert.AreEqual(time,room.RecordSeconds);Assert.AreEqual(p,room.platforms[0].transform.position);room.SetPaused(false);
  for(int i=0;i<510&&room.State==RewindPlaytest.Mode.Recording;i++)Step();Assert.AreEqual(RewindPlaytest.Mode.Rewinding,room.State);Assert.That(room.Memory.Duration,Is.EqualTo(10).Within(.001f));Assert.AreEqual(501,room.Memory.Count);
  for(int i=0;i<110&&room.State==RewindPlaytest.Mode.Rewinding;i++)Step();Step(true);Step();Assert.AreEqual(RewindPlaytest.Mode.Playing,room.State);
  float progress=room.PlaybackProgress;room.SetPaused(true);for(int i=0;i<20;i++)Step();Assert.AreEqual(progress,room.PlaybackProgress);room.SetPaused(false);
  for(int i=0;i<510;i++){Step();Assert.IsFalse(room.echo.support.enabled);}Assert.AreEqual(RewindPlaytest.Mode.Ready,room.State);Assert.IsTrue(room.echo.Active);Assert.AreEqual(1,room.PlaybackProgress);
 }
 [UnityTest]public IEnumerator DirectionSwitchResumesFromEndpointsAndCurrentFrame(){
  yield return Load();Hold();for(int i=0;i<90;i++)Step(move:1);Finish();Step(true);Step();
  for(int i=0;i<35;i++)Step();float cursor=room.PlaybackSeconds;
  room.Step(default,false,true);Assert.IsTrue(room.PlaybackBackwards);Assert.That(room.PlaybackSeconds,Is.EqualTo(cursor-.02f).Within(.001f));
  for(int i=0;i<100;i++)Step();Assert.AreEqual(0,room.PlaybackSeconds);Assert.IsTrue(room.echo.Active);Assert.AreEqual(room.Memory.Sample(0).position,room.echo.Pose.position);
  room.Step(default,false,true);Assert.IsFalse(room.PlaybackBackwards);Assert.Greater(room.PlaybackSeconds,0);
  for(int i=0;i<150;i++)Step();Assert.AreEqual(room.Memory.Duration,room.PlaybackSeconds);Assert.IsTrue(room.echo.Active);Assert.AreEqual(room.Memory.Sample(room.Memory.Duration).position,room.echo.Pose.position);
  room.Step(default,false,true);Assert.Less(room.PlaybackSeconds,room.Memory.Duration);Assert.IsFalse(room.echo.support.enabled);
  float paused=room.PlaybackSeconds;room.SetPaused(true);room.Step(default,false,true);Assert.IsTrue(room.PlaybackBackwards);Assert.AreEqual(paused,room.PlaybackSeconds);room.SetPaused(false);
 }
 [UnityTest]public IEnumerator HoldingReplayFreezesOnlyEchoAndResumesBothDirections(){
  yield return Load();Hold();for(int i=0;i<100;i++)Step(move:1);Finish();Step(true);Step();for(int i=0;i<30;i++)Step();
  for(int direction=0;direction<2;direction++){
   float cursor=room.PlaybackSeconds;var pose=room.echo.Pose.position;var platform=room.platforms[0].transform.position;
   for(int i=0;i<20;i++)room.Step(default,false,false,true);
   Assert.IsTrue(room.PlaybackHeld);Assert.AreEqual(cursor,room.PlaybackSeconds);Assert.AreEqual(pose,room.echo.Pose.position);Assert.IsTrue(room.echo.Active);Assert.AreNotEqual(platform,room.platforms[0].transform.position);
   Step();Assert.IsFalse(room.PlaybackHeld);Assert.That(room.PlaybackSeconds,Is.EqualTo(cursor+(direction==0?.02f:-.02f)).Within(.001f));
   if(direction==0)room.Step(default,false,true);
  }
 }
 [UnityTest]public IEnumerator DebugEntriesSelectSeparateAbilityTiers(){
  SceneFlow.LoadRewindPlaytest(true);yield return null;
  room=Object.FindAnyObjectByType<RewindPlaytest>();room.AutoTick=false;Assert.IsTrue(room.ReplayUpgradeUnlocked);
  SceneFlow.LoadRewindPlaytest(false);yield return null;
  room=Object.FindAnyObjectByType<RewindPlaytest>();room.AutoTick=false;Assert.IsFalse(room.ReplayUpgradeUnlocked);
 }
 [UnityTest]public IEnumerator BasicReplayIgnoresShiftUntilUpgradeUnlocked(){
  yield return Load(false);Assert.IsFalse(room.ReplayUpgradeUnlocked);Hold();for(int i=0;i<80;i++)Step(move:1);Finish();Step(true);Step();
  float cursor=room.PlaybackSeconds;room.Step(default,false,true,true);Assert.IsFalse(room.PlaybackBackwards);Assert.IsFalse(room.PlaybackHeld);Assert.Greater(room.PlaybackSeconds,cursor);
  room.UnlockReplayUpgrade();cursor=room.PlaybackSeconds;room.Step(default,false,false,true);Assert.IsTrue(room.PlaybackHeld);Assert.AreEqual(cursor,room.PlaybackSeconds);
  room.Step(default,false,true);Assert.IsTrue(room.PlaybackBackwards);Assert.Less(room.PlaybackSeconds,cursor);
  yield return Load(false);Assert.IsFalse(room.ReplayUpgradeUnlocked,"Upgrade must not leak into a fresh basic room");
 }
 [UnityTest]public IEnumerator ReplayOperatesPlateAndGateWithoutResettingPlayer(){
  yield return Load();room.player.ResetAt(new Vector2(17,0));Step();Hold();for(int i=0;i<100;i++)Step();Finish();Assert.IsTrue(room.plate.Pressed);Assert.IsTrue(room.door.Open);
  room.player.ResetAt(new Vector2(20,0));Step();Assert.IsFalse(room.door.Open);Step(true);Step();Assert.IsTrue(room.echo.Active);Assert.IsTrue(room.plate.Pressed);Assert.IsTrue(room.door.Open);Assert.That(room.player.Position.x,Is.EqualTo(20).Within(.001f));
  for(int i=0;i<110;i++)Step();Assert.IsTrue(room.echo.Active);Assert.IsTrue(room.door.Open);
 }
}
}
#endif
