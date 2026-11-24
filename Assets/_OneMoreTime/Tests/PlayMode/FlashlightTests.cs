#if UNITY_EDITOR
using System.Collections;
using System.Reflection;
using NUnit.Framework;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

namespace OneMoreTime.Tests {
public class FlashlightTests {
 GameObject source;
 FlashlightAbility light;
 [SetUp] public void Setup(){source=new GameObject("Flashlight timing test");light=source.AddComponent<FlashlightAbility>();}
 [TearDown] public void Teardown(){if(source!=null)Object.DestroyImmediate(source);}

 [Test] public void LockedByDefaultAndCannotRefreshOrSkipRecharge(){
  Assert.IsFalse(light.TryActivate());Assert.AreEqual(0,light.Strength);
  light.unlocked=true;Assert.IsTrue(light.TryActivate());light.Tick(1);
  Assert.IsFalse(light.TryActivate());Assert.That(light.Remaining,Is.EqualTo(3).Within(.001));
  light.Tick(3);Assert.IsFalse(light.Active);Assert.AreEqual(0,light.Strength);
  Assert.That(light.CooldownRemaining,Is.EqualTo(3).Within(.001));Assert.IsFalse(light.TryActivate());
  light.Tick(2.98f);Assert.IsFalse(light.TryActivate());light.Tick(.02f);Assert.IsTrue(light.Ready);Assert.IsTrue(light.TryActivate());
 }
 [Test] public void FixedClockExpiresAndSmoothlyReturnsTheDarkness(){
  light.unlocked=true;light.TryActivate();Assert.AreEqual(0,light.Strength);
  for(int i=0;i<6;i++)light.Tick(.02f);Assert.That(light.Strength,Is.GreaterThan(.99));
  for(int i=6;i<190;i++)light.Tick(.02f);Assert.That(light.Strength,Is.InRange(.5f,.7f));
  for(int i=190;i<200;i++)light.Tick(.02f);Assert.IsFalse(light.Active);Assert.AreEqual(0,light.ExposureAt(Vector2.right*2));
  for(int i=0;i<151;i++)light.Tick(.02f);Assert.IsTrue(light.Ready);
  light.TryActivate();light.Tick(float.NaN);light.Tick(float.PositiveInfinity);light.Tick(-1);Assert.IsFalse(float.IsNaN(light.Clock));
 }
 [Test] public void ConeClearsNearbyAndAheadButNotBehindOrBeyondItsRange(){
  light.unlocked=true;light.TryActivate();light.Tick(.2f);
  Assert.That(light.ExposureAt(Vector2.right*2),Is.GreaterThan(.99));
  Assert.That(light.ExposureAt(Vector2.left*.3f),Is.GreaterThan(.99));
  Assert.AreEqual(0,light.ExposureAt(Vector2.left*2));Assert.AreEqual(0,light.ExposureAt(Vector2.right*7));Assert.AreEqual(0,light.ExposureAt(Vector2.up*3));
  light.enabled=false;Assert.AreEqual(0,light.Strength);Assert.AreEqual(0,light.ExposureAt(Vector2.right*2));
  light.enabled=true;Assert.IsTrue(light.Ready);
 }

 [UnityTest] public IEnumerator SandboxPauseRetryFacingAndBoundsUseOneClock(){
  EditorSceneManager.LoadSceneInPlayMode("Assets/_OneMoreTime/Scenes/Debug_ChapterTwoFlashlight.unity",new LoadSceneParameters(LoadSceneMode.Single));yield return null;
  var room=Object.FindFirstObjectByType<ChapterTwoFlashlightPlaytest>();Assert.NotNull(room);room.AutoTick=false;room.SetPaused(false);room.Retry();
  Assert.IsTrue(room.flashlight.unlocked);
  room.Step(default,true);for(int i=0;i<10;i++)room.Step(default);
  var clock=room.flashlight.Clock;var remaining=room.flashlight.Remaining;var position=room.player.Position;
  room.SetPaused(true);for(int i=0;i<100;i++)room.Step(new InputFrame{move=1},true);
  Assert.AreEqual(clock,room.flashlight.Clock);Assert.AreEqual(remaining,room.flashlight.Remaining);Assert.AreEqual(position,room.player.Position);
  room.SetPaused(false);room.Step(new InputFrame{move=-1});Assert.AreEqual(-1,room.flashlight.Facing);
  Assert.That(room.flashlight.ExposureAt(room.flashlight.Origin+Vector2.left*2),Is.GreaterThan(.99));
  Assert.AreEqual(0,room.flashlight.ExposureAt(room.flashlight.Origin+Vector2.right*2));
  room.Step(new InputFrame{retry=true});Assert.AreEqual(room.spawn,room.player.Position);Assert.IsTrue(room.flashlight.Ready);Assert.AreEqual(0,room.flashlight.Strength);
  room.player.ResetAt(new Vector2(room.spawn.x,room.bounds.yMax+5));room.FrameCamera();Assert.That(room.view.transform.position.y+room.view.orthographicSize,Is.LessThanOrEqualTo(room.bounds.yMax+.001));
  Assert.That(room.view.orthographicSize*room.view.aspect*2,Is.EqualTo(20).Within(.001));
  var volumes=Object.FindObjectsByType<ChapterTwoObscurity>(FindObjectsSortMode.None);Assert.AreEqual(2,volumes.Length);
  foreach(var volume in volumes){Assert.AreSame(room.flashlight,volume.flashlight);Assert.IsNull(volume.GetComponent<Collider2D>());Assert.NotNull(volume.shader);Assert.IsTrue(volume.shader.isSupported);}
 }

 [UnityTest] public IEnumerator RemappedKeyboardActivatesOnlyOnFreshPressAndHoldingDoesNotRepeat(){
  EditorSceneManager.LoadSceneInPlayMode("Assets/_OneMoreTime/Scenes/Debug_ChapterTwoFlashlight.unity",new LoadSceneParameters(LoadSceneMode.Single));yield return null;
  var room=Object.FindFirstObjectByType<ChapterTwoFlashlightPlaytest>();room.AutoTick=true;room.enabled=false;room.SetPaused(false);room.Retry();
  bool saved=PlayerPrefs.HasKey(KeyBindings.PreferencesKey);string preferences=PlayerPrefs.GetString(KeyBindings.PreferencesKey,"");
  var fixture=new InputTestFixture();fixture.Setup();
  try{
   var keyboard=InputSystem.AddDevice<Keyboard>();keyboard.MakeCurrent();KeyBindings.ApplyPreset(0);KeyBindings.SetBinding(GameAction.Flashlight,Key.B);
   var update=typeof(ChapterTwoFlashlightPlaytest).GetMethod("Update",BindingFlags.NonPublic|BindingFlags.Instance);
   var fixedUpdate=typeof(ChapterTwoFlashlightPlaytest).GetMethod("FixedUpdate",BindingFlags.NonPublic|BindingFlags.Instance);
   void Frame(params Key[] keys){InputSystem.QueueStateEvent(keyboard,new KeyboardState(keys));InputSystem.Update();update.Invoke(room,null);fixedUpdate.Invoke(room,null);}
   Frame(Key.I);Assert.IsFalse(room.flashlight.Active,"Old preset key must not activate after rebinding");Frame();
   Frame(Key.B);Assert.IsTrue(room.flashlight.Active);
   for(int i=0;i<360;i++)Frame(Key.B);
   Assert.IsTrue(room.flashlight.Ready);Assert.IsFalse(room.flashlight.Active,"Holding must not retrigger after cooldown");
   Frame();Frame(Key.B);Assert.IsTrue(room.flashlight.Active);
   Frame(Key.Escape);Assert.IsTrue(room.Paused);float remaining=room.flashlight.Remaining;
   for(int i=0;i<20;i++)Frame(Key.B);Assert.AreEqual(remaining,room.flashlight.Remaining);
  } finally {
   fixture.TearDown();if(saved)PlayerPrefs.SetString(KeyBindings.PreferencesKey,preferences);else PlayerPrefs.DeleteKey(KeyBindings.PreferencesKey);PlayerPrefs.Save();KeyBindings.Reload();
  }
 }
}
}
#endif
