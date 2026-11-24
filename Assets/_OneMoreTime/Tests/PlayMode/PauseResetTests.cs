#if UNITY_EDITOR
using System.Collections;
using System.Linq;
using NUnit.Framework;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

namespace OneMoreTime.Tests {
[Category("ChapterOneRegression")]
public class PauseResetTests {
 const float dt=.02f;

 IEnumerator Load(string scene){
  MainGameProgress.StartOver();
  EditorSceneManager.LoadSceneInPlayMode("Assets/_OneMoreTime/Scenes/"+scene+".unity",new LoadSceneParameters(LoadSceneMode.Single));
  yield return null;
 }
 // Exercise the real pause binding; MainRoom intentionally has no test-only setter.
 void Pause(MainRoom room){
  var fixture=new InputTestFixture();fixture.Setup();KeyBindings.Reload();
  var previous=Keyboard.current;var keyboard=InputSystem.AddDevice<Keyboard>();keyboard.MakeCurrent();
  try{
   room.AutoTick=true;
   InputSystem.QueueStateEvent(keyboard,new KeyboardState());InputSystem.Update();room.SendMessage("Update");
   InputSystem.QueueStateEvent(keyboard,new KeyboardState(KeyBindings.Get(GameAction.Pause)));InputSystem.Update();room.SendMessage("Update");
   Assert.IsTrue(room.Paused);
  }finally{room.AutoTick=false;InputSystem.RemoveDevice(keyboard);if(previous!=null)previous.MakeCurrent();fixture.TearDown();KeyBindings.Reload();}
 }

 [UnityTest] public IEnumerator ResetCancelsExitLiftAndRestartsRevivalAtSavedStatue(){
  yield return Load("Main_Room03");
  var room=Object.FindFirstObjectByType<MainRoom>();room.AutoTick=false;
  void Step(InputFrame command=default){room.Step(command,dt);Physics2D.SyncTransforms();}
  for(int i=0;i<200&&room.Phase==MainRoom.RoomPhase.Entering;i++)Step();
  room.player.ResetAt((Vector2)room.checkpoints[0].transform.position+Vector2.up*.01f);Step();
  Assert.IsTrue(room.CheckpointSaved);Vector2 checkpoint=room.checkpoint;
  room.player.ResetAt(new Vector2(25.5f,39));Step();Step();Step(new InputFrame{freeze=true});
  for(int i=0;i<30&&room.shutter.Snapping;i++)Step();
  for(int i=0;i<100&&room.Phase==MainRoom.RoomPhase.Playing;i++)Step(new InputFrame{move=1});
  Assert.AreEqual(MainRoom.RoomPhase.Exiting,room.Phase);Assert.IsTrue(room.lift.Riding);
  for(int i=0;i<12;i++)Step();
  Pause(room);room.ResetFromPause();
  Assert.IsFalse(room.Paused);Assert.IsFalse(room.lift.Riding);Assert.AreEqual(0,room.Fade);
  Assert.AreEqual(MainRoom.RoomPhase.Playing,room.Phase);Assert.AreEqual(1,room.Deaths);
  Assert.That(Vector2.Distance(checkpoint,room.player.Position),Is.LessThan(.001f));Assert.IsTrue(room.IsReviving);
  room.ResetFromPause();Assert.AreEqual(1,room.Deaths,"A repeated UI call after unpausing must not kill twice");
  for(int i=0;i<10;i++)Step();
  Pause(room);room.ResetFromPause();Assert.AreEqual(2,room.Deaths);
  for(int i=0;i<80&&room.IsReviving;i++)Step();
  Assert.IsFalse(room.IsReviving);Assert.IsTrue(room.visual.renderer2D.enabled);
  Assert.That(Vector2.Distance(checkpoint,room.player.Position),Is.LessThan(.001f));
  Assert.AreEqual("Main_Room03",SceneManager.GetActiveScene().name,"The cancelled lift must not still load the next room");
 }

 [UnityTest] public IEnumerator SharedPauseResetEndsDrowningAndKeepsSavedReplay(){
  yield return Load("Main_Room06");
  var chapter=Object.FindFirstObjectByType<ChapterOneRoom>();chapter.AutoTick=false;
  var room=chapter.room;var replay=chapter.replay;replay.AutoTick=false;
  for(int i=0;i<300&&chapter.CurrentPhase==ChapterOneRoom.Phase.Entering;i++)chapter.Step(default,false);
  room.player.ResetAt((Vector2)room.checkpoints[0].transform.position+Vector2.up*.01f);chapter.Step(default,false);
  Assert.IsTrue(room.CheckpointSaved);
  for(int i=0;i<13;i++)chapter.Step(default,true);
  for(int i=0;i<20;i++)chapter.Step(default,false);
  Assert.AreEqual(RewindPlaytest.Mode.Recording,replay.State);replay.FinishRecording();
  for(int i=0;i<200&&replay.State==RewindPlaytest.Mode.Rewinding;i++)chapter.Step(default,false);
  var memory=replay.Memory;Assert.IsNotNull(memory);
  var water=chapter.waters.First(w=>w!=null&&w.drowningPrefab!=null);
  room.player.ResetAt((Vector2)water.GetComponent<BoxCollider2D>().bounds.center-Vector2.up*.3f);chapter.Step(default,false);
  Assert.AreEqual(ChapterOneRoom.Phase.Drowning,chapter.CurrentPhase);Assert.IsFalse(room.visual.renderer2D.enabled);
  replay.SetPaused(true);replay.ResetFromPause();
  Assert.IsFalse(replay.Paused);Assert.AreEqual(ChapterOneRoom.Phase.Playing,chapter.CurrentPhase);
  Assert.AreSame(memory,replay.Memory);Assert.AreEqual(RewindPlaytest.Mode.Ready,replay.State);
  Assert.That(Vector2.Distance(room.checkpoint,room.player.Position),Is.LessThan(.025f));Assert.IsTrue(room.IsReviving);
  for(int i=0;i<80&&room.IsReviving;i++)chapter.Step(default,false);
  Assert.IsFalse(room.IsReviving);Assert.IsTrue(room.visual.renderer2D.enabled);Assert.AreSame(memory,replay.Memory);
  chapter.Step(default,false);Assert.AreEqual(ChapterOneRoom.Phase.Playing,chapter.CurrentPhase);
 }

 [UnityTest] public IEnumerator ResetDuringAcquisitionLeavesSafePlayableRoomAtNormalSpeed(){
  yield return Load("Main_Room05");
  var tutorial=Object.FindFirstObjectByType<Room4AbilityTutorial>();tutorial.AutoTick=false;
  var replay=tutorial.replay;
  for(int i=0;i<200&&tutorial.CurrentPhase==Room4AbilityTutorial.Phase.Arriving;i++)tutorial.Step(default,false,dt);
  for(int i=0;i<250&&tutorial.CurrentPhase==Room4AbilityTutorial.Phase.Approach;i++)tutorial.Step(new InputFrame{move=1},false,dt);
  for(int i=0;i<500&&tutorial.CurrentPhase!=Room4AbilityTutorial.Phase.AwaitRecord;i++)tutorial.Step(default,false,dt);
  Assert.AreEqual(Room4AbilityTutorial.Phase.AwaitRecord,tutorial.CurrentPhase);Assert.IsTrue(tutorial.Unlocked);
  replay.SetPaused(true);replay.ResetFromPause();
  Assert.IsFalse(replay.Paused);Assert.AreEqual(Room4AbilityTutorial.Phase.Playing,tutorial.CurrentPhase);
  Assert.IsFalse(tutorial.TutorialVisible);Assert.AreEqual(1,tutorial.GameplaySpeed);Assert.AreEqual(0,tutorial.Fade);
  Assert.IsTrue(tutorial.crateSource.spawningEnabled);Assert.IsNull(tutorial.room.lift);
  Assert.IsTrue(tutorial.room.visual.renderer2D.enabled);Assert.IsNull(replay.Memory);
  Assert.That(replay.player.Position.y,Is.EqualTo(4).Within(.02f));Assert.Less(replay.player.Position.x,tutorial.pickupFeet.x-1);
  float x=replay.player.Position.x;
  for(int i=0;i<10;i++)tutorial.Step(new InputFrame{move=1},false,dt);
  Assert.Greater(replay.player.Position.x,x+.1f);Assert.AreEqual(1,tutorial.GameplaySpeed);
  Assert.AreEqual(Room4AbilityTutorial.Phase.Playing,tutorial.CurrentPhase);
 }

 [UnityTest] public IEnumerator LegacyPauseResetUsesRecordingDeathReviewAndPreservesSavedMemory(){
  yield return Load("L04_Timing");
  var game=Object.FindFirstObjectByType<RunManager>();game.AutoTick=false;game.InputSource=new TestPlayerInput();game.ResetRun();
  game.Tick(new InputFrame{trace=true});for(int i=0;i<25;i++)game.Tick(default);
  Assert.IsTrue(game.IsRecording);Assert.Greater(game.Recorder.Duration,.25f);
  float recorded=game.Recorder.Duration;game.SetPaused(true);game.ResetFromPause();
  Assert.AreEqual(FlowState.DeathReview,game.State);Assert.AreEqual(recorded,game.Recorder.Duration);Assert.IsNull(game.Memory);
  game.Tick(new InputFrame{trace=true});var memory=game.Memory;Assert.IsNotNull(memory);
  Assert.AreEqual(FlowState.Playing,game.State);
  game.SetPaused(true);game.ResetFromPause();
  Assert.AreEqual(FlowState.Playing,game.State);Assert.AreSame(memory,game.Memory);
  Assert.AreEqual(game.config.spawn,game.player.Position);Assert.AreEqual(PlaybackState.Ready,game.Playback);
 }

 [UnityTest] public IEnumerator ModernPauseResetFinishesValidRecordingAndRecoversAnEmptyOne(){
  yield return Load("Debug_Rewind");
  var replay=Object.FindFirstObjectByType<RewindPlaytest>();replay.AutoTick=false;
  // A suspended clock can begin recording before any second sample exists.
  for(int i=0;i<13;i++)replay.ChargeWhileSuspended(true);
  Assert.AreEqual(RewindPlaytest.Mode.Recording,replay.State);Assert.AreEqual(0,replay.RecordSeconds);
  replay.SetPaused(true);replay.ResetFromPause();
  Assert.IsFalse(replay.Paused);Assert.AreEqual(RewindPlaytest.Mode.Idle,replay.State);Assert.IsNull(replay.Memory);
  for(int i=0;i<13;i++)replay.Step(default,true);
  for(int i=0;i<30;i++)replay.Step(new InputFrame{move=1},false);
  Assert.AreEqual(RewindPlaytest.Mode.Recording,replay.State);
  replay.SetPaused(true);replay.ResetFromPause();
  Assert.IsFalse(replay.Paused);Assert.AreEqual(RewindPlaytest.Mode.Rewinding,replay.State);Assert.IsNotNull(replay.Memory);
  for(int i=0;i<100&&replay.State==RewindPlaytest.Mode.Rewinding;i++)replay.Step(default,false);
  Assert.AreEqual(RewindPlaytest.Mode.Ready,replay.State);var memory=replay.Memory;
  replay.SetPaused(true);replay.ResetFromPause();
  Assert.AreSame(memory,replay.Memory);Assert.AreEqual(RewindPlaytest.Mode.Ready,replay.State);
 }
}
}
#endif
