using NUnit.Framework;
using UnityEngine;
using UnityEngine.InputSystem;

namespace OneMoreTime.Tests {
public class KeyBindingMenuTests {
 bool hadPreferences; string preferences;
 [SetUp] public void SetUp() {
  hadPreferences=PlayerPrefs.HasKey(KeyBindings.PreferencesKey);
  preferences=PlayerPrefs.GetString(KeyBindings.PreferencesKey, "");
  KeyBindings.ApplyPreset(0);
 }
 [TearDown] public void TearDown() {
  if(hadPreferences)PlayerPrefs.SetString(KeyBindings.PreferencesKey,preferences);
  else PlayerPrefs.DeleteKey(KeyBindings.PreferencesKey);
  PlayerPrefs.Save();KeyBindings.Reload();
 }
 [Test] public void EscapeCancelsCaptureWithoutClosingSettingsOrChangingKey() {
  var menu=new KeyBindingMenu();menu.Open();menu.BeginCapture(GameAction.Jump);
  Assert.IsFalse(menu.TryCapture(Key.Escape));Assert.IsTrue(menu.IsOpen);
  Assert.IsNull(menu.CapturingAction);Assert.AreEqual(Key.Space,KeyBindings.Get(GameAction.Jump));
  menu.Close();Assert.IsFalse(menu.IsOpen);
 }
 [Test] public void ReservedKeyCanBeCorrectedAndSuccessfulCapturePersists() {
  var menu=new KeyBindingMenu();menu.Open();menu.BeginCapture(GameAction.Shutter);
  Assert.IsFalse(menu.TryCapture(Key.Enter));Assert.AreEqual(GameAction.Shutter,menu.CapturingAction);
  Assert.IsNotEmpty(menu.Status);Assert.IsTrue(menu.TryCapture(Key.V));
  Assert.IsNull(menu.CapturingAction);KeyBindings.Reload();Assert.AreEqual(Key.V,KeyBindings.Get(GameAction.Shutter));
 }
 [Test] public void FlashlightCanBeCapturedAndSavedWithoutChangingOtherAbilities() {
  var menu=new KeyBindingMenu();menu.Open();menu.BeginCapture(GameAction.Flashlight);
  Assert.AreEqual(GameAction.Flashlight,menu.CapturingAction);Assert.IsTrue(menu.TryCapture(Key.B));
  KeyBindings.Reload();Assert.AreEqual(Key.B,KeyBindings.Get(GameAction.Flashlight));
  Assert.AreEqual(Key.J,KeyBindings.Get(GameAction.Shutter));Assert.AreEqual(Key.K,KeyBindings.Get(GameAction.RecordReplay));
  Assert.AreEqual(Key.L,KeyBindings.Get(GameAction.Filter));
 }
 [Test] public void DuplicateCaptureSwapsAndClosingCancelsPendingCapture() {
  var menu=new KeyBindingMenu();menu.Open();menu.BeginCapture(GameAction.RecordReplay);
  Assert.IsTrue(menu.TryCapture(Key.J));Assert.AreEqual(Key.K,KeyBindings.Get(GameAction.Shutter));
  Assert.AreEqual(Key.J,KeyBindings.Get(GameAction.RecordReplay));
  menu.BeginCapture(GameAction.Filter);menu.Close();Assert.IsFalse(menu.TryCapture(Key.B));
  Assert.AreEqual(Key.L,KeyBindings.Get(GameAction.Filter));
  menu.Open();menu.BeginCapture(GameAction.Pause);Assert.IsNull(menu.CapturingAction);
 }
}
}
