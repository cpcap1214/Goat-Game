using NUnit.Framework;
using UnityEngine;
using UnityEngine.InputSystem;

namespace OneMoreTime.Tests {
public sealed class ControlPromptTests {
 bool hadPreferences;string preferences;
 [SetUp]public void SetUp(){hadPreferences=PlayerPrefs.HasKey(KeyBindings.PreferencesKey);preferences=PlayerPrefs.GetString(KeyBindings.PreferencesKey);KeyBindings.ApplyPreset(0);}
 [TearDown]public void TearDown(){if(hadPreferences)PlayerPrefs.SetString(KeyBindings.PreferencesKey,preferences);else PlayerPrefs.DeleteKey(KeyBindings.PreferencesKey);PlayerPrefs.Save();KeyBindings.Reload();}
 [Test]public void PresetsResolveLegacyInstructionsAtDisplayTime(){
  Assert.AreEqual("Hold [K] to record; J shutter; L filter; W interact; Right Shift reverse",ControlPrompt.Format("Hold [Q] to record; E shutter; F filter; X interact; Left Shift reverse"));
  Assert.AreEqual("Hold [K] to record and rewind",ControlPrompt.Format("Hold [Q] to record and rewind"));
  Assert.AreEqual("Press [K] to replay",ControlPrompt.Format("Press [Q] to replay"));
  KeyBindings.ApplyPreset(1);
  Assert.AreEqual("Hold [X] to record; Z shutter; C filter; Up Arrow interact; Left Shift reverse",ControlPrompt.Format("Hold [Q] to record; E shutter; F filter; X interact; Left Shift reverse"));
  Assert.AreEqual("Press [X] to replay",ControlPrompt.Format("Press [Q] to replay"));
 }
 [Test]public void KeySwapsAreSinglePass(){
  KeyBindings.SetBinding(GameAction.Shutter,Key.Q);KeyBindings.SetBinding(GameAction.RecordReplay,Key.E);
  Assert.AreEqual("Q / E",ControlPrompt.Format("E / Q"));
  Assert.AreEqual("Q / E",ControlPrompt.Key("E / Q"));
  KeyBindings.SetBinding(GameAction.Shutter,Key.A);
  Assert.AreEqual("A",ControlPrompt.Key("E"),"A custom action key must not be reinterpreted as movement.");
 }
 [Test]public void MovementInstructionsChangeWithoutChangingPlateNames(){
  KeyBindings.ApplyPreset(1);
  Assert.AreEqual("Press [Left Arrow] [Right Arrow] to move",ControlPrompt.Format("Press [A] [D] or [←] [→] to move"));
  Assert.AreEqual("Left Arrow / Right Arrow: move; [Down Arrow] + [Space]",ControlPrompt.Format("A / D: move; [↓] + [Space]"));
  KeyBindings.SetBinding(GameAction.Jump,Key.B);
  Assert.AreEqual("Down Arrow + B: drop",ControlPrompt.Format("Down + Jump: drop"));
  Assert.AreEqual("A → B; A、B; WORLD 1; REC",ControlPrompt.Format("A → B; A、B; WORLD 1; REC"));
  Assert.AreEqual("← / →",ControlPrompt.CompactKey("A D"));
 }
 [Test]public void ChineseControlsAndFixedDialogueKeysRemainDistinct(){
  Assert.AreEqual("按 J 留下自己，再按 K 保存；Space跳躍",ControlPrompt.Format("按 E 留下自己，再按 Q 保存；空白鍵跳躍"));
  Assert.AreEqual("Enter / H",ControlPrompt.Key("Enter / H"));
  Assert.IsNull(ControlPrompt.Format(null));Assert.AreEqual("",ControlPrompt.Format(""));
 }
}
}
