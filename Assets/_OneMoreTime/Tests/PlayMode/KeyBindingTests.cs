using NUnit.Framework;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
#if UNITY_EDITOR
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using UnityEditor.SceneManagement;
#endif

namespace OneMoreTime.Tests {
public class KeyBindingTests : InputTestFixture {
 Keyboard keyboard;bool hadPreferences;string savedPreferences;
 static readonly GameAction[] LegacyActions={GameAction.MoveLeft,GameAction.MoveRight,GameAction.MoveUp,GameAction.MoveDown,GameAction.Jump,GameAction.Shutter,GameAction.RecordReplay,GameAction.Filter,GameAction.ReplayDirection,GameAction.Retry};
 [SetUp]public override void Setup(){
  base.Setup();
  hadPreferences=PlayerPrefs.HasKey(KeyBindings.PreferencesKey);savedPreferences=PlayerPrefs.GetString(KeyBindings.PreferencesKey,"");
  keyboard=InputSystem.AddDevice<Keyboard>();keyboard.MakeCurrent();
  PlayerPrefs.DeleteKey(KeyBindings.PreferencesKey);KeyBindings.Reload();
 }
 [TearDown]public override void TearDown(){
  base.TearDown();
  if(hadPreferences)PlayerPrefs.SetString(KeyBindings.PreferencesKey,savedPreferences);else PlayerPrefs.DeleteKey(KeyBindings.PreferencesKey);
  PlayerPrefs.Save();KeyBindings.Reload();
 }
 void Keys(params Key[] keys){InputSystem.QueueStateEvent(keyboard,new KeyboardState(keys));InputSystem.Update();}
 [Test]public void FreshProfileUsesWasdJklRightShift(){
  Assert.AreEqual(0,KeyBindings.CurrentPresetIndex);Assert.AreEqual(Key.A,KeyBindings.Get(GameAction.MoveLeft));Assert.AreEqual(Key.D,KeyBindings.Get(GameAction.MoveRight));
  Assert.AreEqual(Key.W,KeyBindings.Get(GameAction.Interact));Assert.AreEqual(Key.S,KeyBindings.Get(GameAction.MoveDown));Assert.AreEqual(Key.Space,KeyBindings.Get(GameAction.Jump));
  Assert.AreEqual(Key.J,KeyBindings.Get(GameAction.Shutter));Assert.AreEqual(Key.K,KeyBindings.Get(GameAction.RecordReplay));Assert.AreEqual(Key.L,KeyBindings.Get(GameAction.Filter));
  Assert.AreEqual(Key.I,KeyBindings.Get(GameAction.Flashlight));Assert.AreEqual(Key.RightShift,KeyBindings.Get(GameAction.ReplayDirection));Assert.AreEqual(Key.R,KeyBindings.Get(GameAction.Retry));Assert.AreEqual(Key.Escape,KeyBindings.Get(GameAction.Pause));
 }
 [Test]public void ArrowPresetRoutesMovementDropAndAbilities(){
  KeyBindings.ApplyPreset(1);var adapter=new KeyboardPlayerInput();
  Keys(Key.RightArrow,Key.DownArrow,Key.Space,Key.Z,Key.X,Key.C,Key.V,Key.LeftShift);adapter.Poll();var frame=adapter.Consume();
  Assert.AreEqual(1,frame.move);Assert.IsTrue(frame.jump);Assert.IsTrue(frame.drop);Assert.IsTrue(frame.held);Assert.IsTrue(frame.freeze);Assert.IsTrue(frame.trace);
  Assert.IsTrue(KeyBindings.WasPressed(GameAction.Filter));Assert.IsTrue(KeyBindings.WasPressed(GameAction.Flashlight));Assert.AreEqual(Key.V,KeyBindings.Get(GameAction.Flashlight));Assert.IsTrue(KeyBindings.IsHeld(GameAction.ReplayDirection));Assert.AreEqual(Key.UpArrow,KeyBindings.Get(GameAction.Interact));
  Assert.AreEqual(1,KeyBindings.CurrentPresetIndex);
 }
 [Test]public void RebindingSwapsConflictsAndPersistsAfterReload(){
  Assert.IsTrue(KeyBindings.SetBinding(GameAction.RecordReplay,Key.J));
  Assert.AreEqual(Key.J,KeyBindings.Get(GameAction.RecordReplay));Assert.AreEqual(Key.K,KeyBindings.Get(GameAction.Shutter));Assert.AreEqual(-1,KeyBindings.CurrentPresetIndex);
  KeyBindings.Reload();Assert.AreEqual(Key.J,KeyBindings.Get(GameAction.RecordReplay));Assert.AreEqual(Key.K,KeyBindings.Get(GameAction.Shutter));
  Assert.IsTrue(KeyBindings.SetBinding(GameAction.Interact,Key.Space));Assert.AreEqual(Key.Space,KeyBindings.Get(GameAction.MoveUp));Assert.AreEqual(Key.W,KeyBindings.Get(GameAction.Jump));
 }
 int[] SaveLegacyProfile(){
  var keys=new int[LegacyActions.Length];for(int i=0;i<keys.Length;i++)keys[i]=(int)KeyBindings.Get(LegacyActions[i]);
  PlayerPrefs.SetString(KeyBindings.PreferencesKey,"{\"version\":1,\"keys\":["+string.Join(",",keys)+"]}");return keys;
 }
 [TestCase(0,Key.I)] [TestCase(1,Key.V)]
 public void LegacyPresetMigratesAndRemainsRecognized(int preset,Key flashlight){
  KeyBindings.ApplyPreset(preset);var previous=SaveLegacyProfile();KeyBindings.Reload();
  for(int i=0;i<previous.Length;i++)Assert.AreEqual((Key)previous[i],KeyBindings.Get(LegacyActions[i]));
  Assert.AreEqual(flashlight,KeyBindings.Get(GameAction.Flashlight));Assert.AreEqual(preset,KeyBindings.CurrentPresetIndex);
  StringAssert.Contains("\"version\":2",PlayerPrefs.GetString(KeyBindings.PreferencesKey));
  KeyBindings.Reload();Assert.AreEqual(flashlight,KeyBindings.Get(GameAction.Flashlight));
 }
 [Test]public void LegacyCustomBindingsKeepOccupiedFlashlightDefaultsAndGetFreeFallback(){
  KeyBindings.SetBinding(GameAction.Jump,Key.I);KeyBindings.SetBinding(GameAction.Filter,Key.V);
  var previous=SaveLegacyProfile();KeyBindings.Reload();
  for(int i=0;i<previous.Length;i++)Assert.AreEqual((Key)previous[i],KeyBindings.Get(LegacyActions[i]),LegacyActions[i].ToString());
  Assert.AreEqual(Key.T,KeyBindings.Get(GameAction.Flashlight));Assert.AreEqual(-1,KeyBindings.CurrentPresetIndex);
  KeyBindings.Reload();Assert.AreEqual(Key.T,KeyBindings.Get(GameAction.Flashlight));
 }
 [Test]public void FlashlightRebindingSwapsAndPersistsWithGameplaySuppression(){
  Keys(Key.J);KeyBindings.SetBinding(GameAction.Flashlight,Key.J);
  Assert.AreEqual(Key.I,KeyBindings.Get(GameAction.Shutter));Assert.IsFalse(KeyBindings.WasPressed(GameAction.Flashlight));
  Keys();Keys(Key.J);Assert.IsTrue(KeyBindings.WasPressed(GameAction.Flashlight));
  KeyBindings.Reload();Assert.AreEqual(Key.J,KeyBindings.Get(GameAction.Flashlight));Assert.AreEqual(Key.I,KeyBindings.Get(GameAction.Shutter));
 }
 [Test]public void ReservedAndInvalidKeysCannotReplaceBindings(){
  foreach(var key in new[]{Key.None,Key.Escape,Key.Enter,Key.NumpadEnter,Key.H,(Key)9999})Assert.IsFalse(KeyBindings.SetBinding(GameAction.Jump,key));
  Assert.IsFalse(KeyBindings.SetBinding(GameAction.Pause,Key.P));Assert.AreEqual(Key.Escape,KeyBindings.Get(GameAction.Pause));Assert.AreEqual(Key.Space,KeyBindings.Get(GameAction.Jump));
 }
 [Test]public void CorruptOrDuplicatePreferencesFallBackToCompletePreset(){
  foreach(var json in new[]{"not-json","{\"version\":1,\"keys\":[1,1,1,1,1,1,1,1,1,1]}","{\"version\":77,\"keys\":[]}"}){
   PlayerPrefs.SetString(KeyBindings.PreferencesKey,json);Assert.DoesNotThrow(KeyBindings.Reload);Assert.AreEqual(0,KeyBindings.CurrentPresetIndex);
  }
 }
 [Test]public void CaptureAndMenuExitDoNotLeakHeldRecordOrMove(){
  Keys(Key.D,Key.K);KeyBindings.SuppressUntilRelease();var adapter=new KeyboardPlayerInput();adapter.Poll();var frame=adapter.Consume();
  Assert.AreEqual(0,frame.move);Assert.IsFalse(frame.trace);Assert.IsFalse(KeyBindings.IsHeld(GameAction.RecordReplay));
  Keys();adapter.Poll();Assert.IsFalse(KeyBindings.IsHeld(GameAction.RecordReplay));
  Keys(Key.D,Key.K);adapter.Poll();frame=adapter.Consume();Assert.AreEqual(1,frame.move);Assert.IsTrue(frame.trace);Assert.IsTrue(KeyBindings.IsHeld(GameAction.RecordReplay));
 }
 [Test]public void RemappedHoldAndReleaseUseNewPhysicalKeys(){
  KeyBindings.SetBinding(GameAction.RecordReplay,Key.Tab);KeyBindings.SetBinding(GameAction.ReplayDirection,Key.RightCtrl);
  Keys(Key.Tab,Key.RightCtrl);Assert.IsTrue(KeyBindings.IsHeld(GameAction.RecordReplay));Assert.IsTrue(KeyBindings.WasPressed(GameAction.ReplayDirection));
  InputSystem.Update();Assert.IsTrue(KeyBindings.IsHeld(GameAction.RecordReplay));Assert.IsFalse(KeyBindings.WasPressed(GameAction.ReplayDirection));
  Keys();Assert.IsFalse(KeyBindings.IsHeld(GameAction.RecordReplay));Assert.IsTrue(KeyBindings.WasReleased(GameAction.ReplayDirection));
 }
 [Test]public void CapturedKeyReleasedInsideMenuWorksOnFirstGameplayPress(){
  Keys(Key.Tab);KeyBindings.SetBinding(GameAction.RecordReplay,Key.Tab);
  Keys(); // Menu capture is finished; gameplay does not poll this release.
  KeyBindings.SuppressUntilRelease(); // Closing the menu refreshes held keys.
  Keys(Key.Tab);Assert.IsTrue(KeyBindings.IsHeld(GameAction.RecordReplay));Assert.IsTrue(KeyBindings.WasPressed(GameAction.RecordReplay));
 }
 [Test]public void ReleaseDuringEntryAnimationDoesNotEatNextPress(){
  Keys(Key.K);KeyBindings.SuppressUntilRelease();Keys();
  // No gameplay polling or menu-close callback occurs during this interval.
  Keys(Key.K);Assert.IsTrue(KeyBindings.WasPressed(GameAction.RecordReplay));Assert.IsTrue(KeyBindings.IsHeld(GameAction.RecordReplay));
 }
 [Test]public void UnselectedLegacyKeysDoNotTriggerGameplay(){
  Keys(Key.E,Key.Q,Key.F,Key.LeftShift,Key.LeftArrow);var adapter=new KeyboardPlayerInput();adapter.Poll();var frame=adapter.Consume();
  Assert.AreEqual(0,frame.move);Assert.IsFalse(frame.freeze);Assert.IsFalse(frame.trace);Assert.IsFalse(KeyBindings.IsHeld(GameAction.Filter));Assert.IsFalse(KeyBindings.IsHeld(GameAction.ReplayDirection));
 }
#if UNITY_EDITOR
 [UnityTest,Category("ChapterOneRegression")]public IEnumerator MainRoomPauseSettingsCaptureEscapeAndApplyWithoutUnpausing(){yield return ExercisePauseSettings("Main_Room01");}
 [UnityTest,Category("ChapterOneRegression")]public IEnumerator RoomFivePauseSettingsCaptureEscapeAndApplyWithoutUnpausing(){yield return ExercisePauseSettings("Main_Room05");}
 [UnityTest,Category("ChapterOneRegression")]public IEnumerator ShutterOnlyRoomPauseSettingsPreserveTheReplayLock(){yield return ExercisePauseSettings("Main_Room04");}
 [UnityTest,Category("ChapterOneRegression")]public IEnumerator ChapterRoomPauseSettingsCaptureEscapeAndApplyWithoutUnpausing(){yield return ExercisePauseSettings("Main_Room08");}
 [UnityTest,Category("ChapterOneRegression")]public IEnumerator ReplayDebugPauseSettingsCaptureEscapeAndApplyWithoutUnpausing(){yield return ExercisePauseSettings("Debug_Rewind");}
 IEnumerator ExercisePauseSettings(string scene){
  MainGameProgress.StartOver();if(scene==Room4AbilityTutorial.SceneName)MainGameProgress.GrantReplay();
  EditorSceneManager.LoadSceneInPlayMode("Assets/_OneMoreTime/Scenes/"+scene+".unity",new LoadSceneParameters(LoadSceneMode.Single));yield return null;
  var chapter=Object.FindAnyObjectByType<ChapterOneRoom>();var main=Object.FindAnyObjectByType<MainRoom>();var replay=Object.FindAnyObjectByType<RewindPlaytest>();var tutorial=Object.FindAnyObjectByType<Room4AbilityTutorial>();
  MonoBehaviour owner;PlayerMotor2D player;KeyBindingMenu controls;System.Func<bool> paused;
  if(chapter!=null){
   chapter.enabled=false;chapter.AutoTick=true;chapter.room.AutoTick=chapter.replay.AutoTick=false;
   for(int i=0;i<400&&chapter.CurrentPhase==ChapterOneRoom.Phase.Entering;i++)chapter.Step(default,false);
   Assert.AreEqual(ChapterOneRoom.Phase.Playing,chapter.CurrentPhase);
   owner=chapter;player=chapter.room.player;controls=chapter.replay.Controls;paused=()=>chapter.replay.Paused;
  }else if(tutorial!=null){
   tutorial.enabled=false;tutorial.AutoTick=true;tutorial.room.AutoTick=tutorial.replay.AutoTick=false;
   for(int i=0;i<300&&tutorial.CurrentPhase==Room4AbilityTutorial.Phase.Arriving;i++)tutorial.Step(default,false,.02f);
   Assert.AreEqual(Room4AbilityTutorial.Phase.Playing,tutorial.CurrentPhase);
   owner=tutorial;player=tutorial.room.player;controls=tutorial.replay.Controls;paused=()=>tutorial.replay.Paused;
  }else if(main!=null){
   main.enabled=false;main.AutoTick=true;
   for(int i=0;i<400&&main.Phase==MainRoom.RoomPhase.Entering;i++)main.Step(default,.02f);
   Assert.AreEqual(MainRoom.RoomPhase.Playing,main.Phase);
   owner=main;player=main.player;controls=main.Controls;paused=()=>main.Paused;
  }else{
   Assert.IsNotNull(replay);replay.enabled=false;replay.AutoTick=true;
   owner=replay;player=replay.player;controls=replay.Controls;paused=()=>replay.Paused;
  }
  keyboard.MakeCurrent();
  void Pump(params Key[] keys){Keys(keys);owner.SendMessage("Update");owner.SendMessage("FixedUpdate");}
  Pump();Pump(Key.Escape);Assert.IsTrue(paused());
  Vector2 frozen=player.Position;
  var movers=replay!=null?replay.platforms:new RoomPlatform[0];
  var positions=new Vector3[movers.Length];for(int i=0;i<movers.Length;i++)positions[i]=movers[i].transform.position;
  var sources=Object.FindObjectsByType<CrateSpawner>(FindObjectsSortMode.None);var sourceTimers=new float[sources.Length];
  for(int i=0;i<sources.Length;i++)sourceTimers[i]=sources[i].Remaining;
  void Frozen(){
   Assert.IsTrue(paused(),"Settings input must leave gameplay paused.");Assert.AreEqual(frozen,player.Position);
   for(int i=0;i<movers.Length;i++)Assert.AreEqual(positions[i],movers[i].transform.position,"Settings cannot advance a moving platform.");
   for(int i=0;i<sources.Length;i++)Assert.AreEqual(sourceTimers[i],sources[i].Remaining,"Settings cannot advance floating-object sources.");
  }
  controls.Open();controls.BeginCapture(GameAction.Jump);
  Keys();yield return null; // The listener intentionally ignores the click's frame.
  Pump(Key.Escape);Assert.IsNull(controls.CapturingAction);Assert.IsTrue(controls.IsOpen);Frozen();
  Assert.AreEqual(Key.Space,KeyBindings.Get(GameAction.Jump),"Cancelling leaves the original binding intact.");
  Pump();Pump(Key.Escape);Assert.IsFalse(controls.IsOpen);Frozen();
  Pump();Pump(Key.Escape);Assert.IsFalse(paused(),"Only Escape from the pause panel resumes play.");

  Pump();Pump(Key.Escape);Assert.IsTrue(paused());frozen=player.Position;
  for(int i=0;i<movers.Length;i++)positions[i]=movers[i].transform.position;
  for(int i=0;i<sources.Length;i++)sourceTimers[i]=sources[i].Remaining;
  controls.Open();controls.ApplyPreset(1);
  Assert.AreEqual(1,KeyBindings.CurrentPresetIndex);Assert.AreEqual(Key.X,KeyBindings.Get(GameAction.RecordReplay));
  controls.BeginCapture(GameAction.MoveRight);Keys();yield return null;
  Pump(Key.B);Assert.IsNull(controls.CapturingAction);Assert.AreEqual(Key.B,KeyBindings.Get(GameAction.MoveRight));Frozen();
  for(int i=0;i<25;i++)Pump(Key.B,Key.Space,Key.X);
  Frozen();if(replay!=null)Assert.AreEqual(RewindPlaytest.Mode.Idle,replay.State,"Binding/testing keys while paused cannot begin recording.");
  Pump();Pump(Key.Escape);Assert.IsFalse(controls.IsOpen);Frozen();
  Pump();Pump(Key.Escape);Assert.IsFalse(paused());Pump();
  var resumed=player.Position;
  for(int i=0;i<9;i++)Pump(Key.RightArrow);
  Assert.That(player.Position.x,Is.EqualTo(resumed.x).Within(.002f),"The replaced movement key must no longer move the goat.");
  for(int i=0;i<9;i++)Pump(Key.B);
  Assert.Greater(player.Position.x,resumed.x+.1f,"The newly captured key must work immediately after resuming.");
  if(replay!=null){
   Pump();for(int i=0;i<13;i++)Pump(Key.X);
   Assert.AreEqual(chapter!=null&&!chapter.ReplayAvailable?RewindPlaytest.Mode.Idle:RewindPlaytest.Mode.Recording,replay.State,"Rebound recording follows the room's ability gate after closing pause settings.");
  }
 }
 [UnityTest]public IEnumerator CustomKeysDriveRecordingVisibleRewindReplayAndPlaybackHold(){
  EditorSceneManager.LoadSceneInPlayMode("Assets/_OneMoreTime/Scenes/Debug_Rewind.unity",new LoadSceneParameters(LoadSceneMode.Single));yield return null;
  var room=Object.FindAnyObjectByType<RewindPlaytest>();Assert.IsNotNull(room);
  // Exercise the real keyboard-facing Update/FixedUpdate methods without a
  // second automatic simulation tick from the player loop.
  room.enabled=false;room.AutoTick=true;room.UnlockReplayUpgrade();
  KeyBindings.SetBinding(GameAction.RecordReplay,Key.Tab);KeyBindings.SetBinding(GameAction.ReplayDirection,Key.RightCtrl);
  void Pump(params Key[] keys){Keys(keys);room.SendMessage("Update");room.SendMessage("FixedUpdate");}
  Pump(); // Release any scene-entry suppression before the first gesture.
  for(int i=0;i<12;i++)Pump(Key.Tab);
  Assert.AreEqual(RewindPlaytest.Mode.Idle,room.State,"The custom record key must still respect the 0.25-second hold");
  Assert.That(room.HoldProgress,Is.EqualTo(.96f).Within(.001f));
  Pump(Key.Tab);Assert.AreEqual(RewindPlaytest.Mode.Recording,room.State);
  var recordingStart=room.player.Position;Pump();Assert.AreEqual(RewindPlaytest.Mode.Recording,room.State,"Releasing the initial hold must not finish the clip");
  for(int i=0;i<45;i++)Pump(KeyBindings.Get(GameAction.MoveRight));
  Assert.Greater(room.player.Position.x,recordingStart.x+1);
  Pump(Key.Tab);Pump();
  Assert.AreEqual(RewindPlaytest.Mode.Rewinding,room.State);
  Assert.Greater(room.player.Position.x,recordingStart.x+.5f,"Finishing should visibly rewind over several ticks, not teleport immediately");
  for(int i=0;i<60&&room.State==RewindPlaytest.Mode.Rewinding;i++)Pump();
  Assert.AreEqual(RewindPlaytest.Mode.Ready,room.State);Assert.IsNotNull(room.Memory);
  Assert.That(Vector2.Distance(recordingStart,room.player.Position),Is.LessThan(.001f));
  Pump(Key.Tab);Pump();Assert.AreEqual(RewindPlaytest.Mode.Playing,room.State);
  for(int i=0;i<8;i++)Pump();
  float cursor=room.PlaybackSeconds;var echoPosition=room.echo.Pose.position;var platformPosition=room.platforms[0].transform.position;
  for(int i=0;i<20;i++)Pump(Key.RightCtrl);
  Assert.IsTrue(room.PlaybackHeld);Assert.AreEqual(cursor,room.PlaybackSeconds);Assert.AreEqual(echoPosition,room.echo.Pose.position);
  Assert.AreNotEqual(platformPosition,room.platforms[0].transform.position,"The remapped hold pauses the replay timeline, not live machinery");
  Pump();Assert.IsFalse(room.PlaybackHeld);Assert.AreNotEqual(cursor,room.PlaybackSeconds,"Releasing the custom hold key must resume the cursor");
 }
#endif
}
}
