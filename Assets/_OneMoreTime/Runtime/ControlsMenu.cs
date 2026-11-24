using UnityEngine;
using UnityEngine.InputSystem;

namespace OneMoreTime {
// Shared by the main menu and pause menus; no scene references are required.
public sealed class KeyBindingMenu {
 static readonly GameAction[] Actions = {
  GameAction.MoveLeft, GameAction.MoveRight, GameAction.MoveUp,
  GameAction.MoveDown, GameAction.Jump, GameAction.Retry,
  GameAction.Shutter, GameAction.RecordReplay, GameAction.Filter,
  GameAction.ReplayDirection, GameAction.Flashlight, GameAction.Pause
 };
 static readonly string[] ActionNames = {
  "向左 / Left", "向右 / Right", "向上・互動 / Up・Interact",
  "向下 / Down", "跳躍 / Jump", "重試 / Retry",
  "定格 / Shutter", "錄影・回放 / Record・Replay", "濾鏡 / Filter",
  "倒播・暫停 / Reverse・Hold", "手電筒 / Flashlight (Ch. 2)", "暫停 / Pause"
 };
 public bool IsOpen { get; private set; }
 public GameAction? CapturingAction { get; private set; }
 public string Status { get; private set; }
 int captureStartedFrame;

 public void Open() {
  IsOpen = true;
  CapturingAction = null;
  Status = null;
  KeyBindings.SuppressUntilRelease();
 }

 public void Close() {
  CapturingAction = null;
  IsOpen = false;
  KeyBindings.SuppressUntilRelease();
 }

 public void BeginCapture(GameAction action) {
  if (!IsOpen || action == GameAction.Pause) return;
  CapturingAction = action;
  captureStartedFrame = Time.frameCount;
  Status = null;
 }

 // Shared by the keyboard listener and focused tests of cancellation/validation.
 public bool TryCapture(Key key) {
  if (!IsOpen || !CapturingAction.HasValue) return false;
  if (key == Key.Escape) {
   CapturingAction = null;
   Status = "已取消 / Cancelled";
   KeyBindings.SuppressUntilRelease();
   return false;
  }
  if (!KeyBindings.IsBindable(key)) {
   Status = "此按鍵保留給選單，請選另一個 / This key is reserved for menus.";
   return false;
  }
  var action = CapturingAction.Value;
  if (!KeyBindings.SetBinding(action, key)) return false;
  CapturingAction = null;
  Status = "已儲存；如有重複，按鍵已互換 / Saved; conflicting bindings swap.";
  KeyBindings.SuppressUntilRelease();
  return true;
 }

 public void Tick() {
  if (!IsOpen) return;
  var keyboard = Keyboard.current;
  if (keyboard == null) return;
  if (CapturingAction.HasValue) {
   // Ignore any input from the click/frame that began capture.
   if (Time.frameCount <= captureStartedFrame) return;
   if (keyboard.escapeKey.wasPressedThisFrame) {
    TryCapture(Key.Escape);
    return;
   }
   foreach (var key in keyboard.allKeys) {
    if (!key.wasPressedThisFrame) continue;
    TryCapture(key.keyCode);
    break;
   }
   return;
  }
  if (keyboard.escapeKey.wasPressedThisFrame) Close();
 }

 public void Draw() {
  if (!IsOpen) return;
  GameUI.Shade();
  GameUI.Frame(new Rect(60, 20, 840, 500));
  GameUI.Text(new Rect(88, 36, 784, 40), "按鍵設定 / Key Bindings", 28, GameUI.Gold);
  string mode = KeyBindings.CurrentPresetIndex < 0
   ? "自訂 / Custom" : KeyBindings.PresetName(KeyBindings.CurrentPresetIndex);
  GameUI.Text(new Rect(88, 78, 784, 24), "目前 / Current: " + mode, 15, GameUI.Muted);
  bool previousEnabled = GUI.enabled;
  GUI.enabled = previousEnabled && !CapturingAction.HasValue;
  if (GameUI.Button(new Rect(88, 112, 376, 42), "1  WASD · JKL/I · R Shift · Space", KeyBindings.CurrentPresetIndex == 0)) ApplyPreset(0);
  if (GameUI.Button(new Rect(496, 112, 376, 42), "2  Arrows · ZXC/V · L Shift · Space", KeyBindings.CurrentPresetIndex == 1)) ApplyPreset(1);
  GUI.enabled = previousEnabled;
  GameUI.Text(new Rect(88, 165, 784, 25), "點選按鍵更改；向下＋跳躍穿過平台 / Click a key to rebind. Down + Jump: drop through.", 14, GameUI.Muted);
  GameUI.Divider(88, 196, 784);
  for (int i = 0; i < Actions.Length; i++) {
   float x = i < 6 ? 88 : 496;
   float y = 210 + (i < 6 ? i : i - 6) * 34;
   var action = Actions[i];
   bool capturing = CapturingAction == action;
   GameUI.Text(new Rect(x, y, 258, 28), ActionNames[i], 16,
    capturing ? GameUI.Gold : GameUI.Cream, TextAnchor.MiddleLeft);
   var keyRect = new Rect(x + 268, y, 108, 27);
   bool fixedKey = action == GameAction.Pause;
   GameUI.Key(keyRect, capturing ? "輸入 / Press…" : KeyBindings.Label(action), capturing);
   GUI.enabled = previousEnabled && !fixedKey && !CapturingAction.HasValue;
   if (GUI.Button(keyRect, GUIContent.none, GUIStyle.none)) BeginCapture(action);
   GUI.enabled = previousEnabled;
  }
  string help = CapturingAction.HasValue
   ? "按新鍵；Esc 取消。Enter／H 保留給選單。 / Press a key; Esc cancels. Enter/H are reserved."
   : "自動儲存；重複按鍵會互換。Esc 固定為暫停。 / Auto-saved; duplicate keys swap. Esc stays Pause.";
  GameUI.Text(new Rect(88, 421, 784, 41), Status ?? help, 14,
   CapturingAction.HasValue ? GameUI.Gold : GameUI.Muted);
  if (GameUI.Button(new Rect(88, 472, 784, 32), "返回 / Back", false, "Esc")) Close();
 }

 public void ApplyPreset(int index) {
  if (!IsOpen || CapturingAction.HasValue) return;
  KeyBindings.ApplyPreset(index);
  KeyBindings.SuppressUntilRelease();
  Status = "已套用並儲存 / Preset applied and saved";
 }
}
}
