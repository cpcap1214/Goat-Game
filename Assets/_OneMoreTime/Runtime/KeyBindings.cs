using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.SceneManagement;

namespace OneMoreTime {
public enum GameAction {MoveLeft,MoveRight,MoveUp,MoveDown,Jump,Shutter,RecordReplay,Filter,ReplayDirection,Interact,Retry,Pause,Flashlight}

// One keyboard map for both legacy tutorials and the current main rooms. Up also
// means interact; it is deliberately one slot, not two independently bound keys.
public static class KeyBindings {
 public const string PreferencesKey="OneMoreTime.KeyboardBindings.v1";
 static readonly GameAction[] editable={GameAction.MoveLeft,GameAction.MoveRight,GameAction.MoveUp,GameAction.MoveDown,GameAction.Jump,GameAction.Shutter,GameAction.RecordReplay,GameAction.Filter,GameAction.ReplayDirection,GameAction.Retry,GameAction.Flashlight};
 static readonly HashSet<Key> suppressed=new HashSet<Key>();
 static Key[] bindings;
 static Keyboard suppressedKeyboard;
 public static int Revision {get;private set;}
 [Serializable] sealed class SavedMap {public int version=2;public int[] keys;}

 [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
 static void Initialize(){Reload();SceneManager.sceneLoaded-=SceneLoaded;SceneManager.sceneLoaded+=SceneLoaded;SuppressUntilRelease();}
 static void SceneLoaded(Scene scene,LoadSceneMode mode){SuppressUntilRelease();}
 static GameAction Canonical(GameAction action)=>action==GameAction.Interact?GameAction.MoveUp:action;
 static bool ValidAction(GameAction action)=>Enum.IsDefined(typeof(GameAction),action);
 static void EnsureLoaded(){if(bindings==null)Reload();}
 static Key[] Preset(int index){
  var map=new Key[Enum.GetValues(typeof(GameAction)).Length];
  map[(int)GameAction.MoveLeft]=index==0?Key.A:Key.LeftArrow;
  map[(int)GameAction.MoveRight]=index==0?Key.D:Key.RightArrow;
  map[(int)GameAction.MoveUp]=index==0?Key.W:Key.UpArrow;
  map[(int)GameAction.MoveDown]=index==0?Key.S:Key.DownArrow;
  map[(int)GameAction.Jump]=Key.Space;
  map[(int)GameAction.Shutter]=index==0?Key.J:Key.Z;
  map[(int)GameAction.RecordReplay]=index==0?Key.K:Key.X;
  map[(int)GameAction.Filter]=index==0?Key.L:Key.C;
  map[(int)GameAction.ReplayDirection]=index==0?Key.RightShift:Key.LeftShift;
  map[(int)GameAction.Interact]=map[(int)GameAction.MoveUp];
  map[(int)GameAction.Retry]=Key.R;map[(int)GameAction.Pause]=Key.Escape;
  map[(int)GameAction.Flashlight]=index==0?Key.I:Key.V;
  return map;
 }
 public static string PresetName(int index)=>index==0?"WASD · JKL/I · Right Shift · Space":index==1?"Arrows · ZXC/V · Left Shift · Space":"Custom";
 public static int CurrentPresetIndex {
  get{EnsureLoaded();for(int p=0;p<2;p++){var preset=Preset(p);bool same=true;foreach(var action in editable)if(bindings[(int)action]!=preset[(int)action]){same=false;break;}if(same)return p;}return -1;}
 }
 public static Key Get(GameAction action){EnsureLoaded();return ValidAction(action)?bindings[(int)Canonical(action)]:Key.None;}
 public static string Label(GameAction action)=>KeyLabel(Get(action));
 public static string KeyLabel(Key key){
  switch(key){case Key.LeftArrow:return "Left Arrow";case Key.RightArrow:return "Right Arrow";case Key.UpArrow:return "Up Arrow";case Key.DownArrow:return "Down Arrow";
   case Key.LeftShift:return "Left Shift";case Key.RightShift:return "Right Shift";case Key.LeftCtrl:return "Left Ctrl";case Key.RightCtrl:return "Right Ctrl";
   case Key.LeftAlt:return "Left Alt";case Key.RightAlt:return "Right Alt";case Key.LeftMeta:return "Left Command";case Key.RightMeta:return "Right Command";
   case Key.Escape:return "Esc";case Key.Backquote:return "`";case Key.Quote:return "'";case Key.Semicolon:return ";";case Key.Comma:return ",";case Key.Period:return ".";
   case Key.Slash:return "/";case Key.Backslash:return "\\";case Key.LeftBracket:return "[";case Key.RightBracket:return "]";case Key.Minus:return "-";case Key.Equals:return "=";
   default:var name=key.ToString();return name.StartsWith("Digit",StringComparison.Ordinal)?name.Substring(5):name;
  }
 }
 public static bool IsBindable(Key key)=>key!=Key.None&&key!=Key.Escape&&key!=Key.Enter&&key!=Key.NumpadEnter&&key!=Key.H&&Enum.IsDefined(typeof(Key),key);
 public static bool SetBinding(GameAction action,Key key){
  EnsureLoaded();if(!ValidAction(action)||action==GameAction.Pause||!IsBindable(key))return false;
  action=Canonical(action);var previous=Get(action);
  foreach(var other in editable)if(other!=action&&Get(other)==key){bindings[(int)other]=previous;break;}
  bindings[(int)action]=key;bindings[(int)GameAction.Interact]=bindings[(int)GameAction.MoveUp];
  Save();Revision++;SuppressUntilRelease();return true;
 }
 public static void ApplyPreset(int index){if(index<0||index>1)throw new ArgumentOutOfRangeException(nameof(index));bindings=Preset(index);Save();Revision++;SuppressUntilRelease();}
 static void Save(){var data=new SavedMap{keys=new int[editable.Length]};for(int i=0;i<editable.Length;i++)data.keys[i]=(int)Get(editable[i]);PlayerPrefs.SetString(PreferencesKey,JsonUtility.ToJson(data));PlayerPrefs.Save();}
 // The PlayerPrefs name stays stable. Version 1 had ten editable actions;
 // version 2 appends Flashlight so existing custom keys keep their meaning.
 public static void Reload(){
  bindings=Preset(0);suppressed.Clear();suppressedKeyboard=Keyboard.current;
  InputSystem.onAfterUpdate-=ReleaseSuppressedKeys;InputSystem.onAfterUpdate+=ReleaseSuppressedKeys;
  if(PlayerPrefs.HasKey(PreferencesKey))try{
   var data=JsonUtility.FromJson<SavedMap>(PlayerPrefs.GetString(PreferencesKey));
   bool legacy=data!=null&&data.version==1&&data.keys!=null&&data.keys.Length==editable.Length-1;
   bool current=data!=null&&data.version==2&&data.keys!=null&&data.keys.Length==editable.Length;
   if(legacy||current){
    var seen=new HashSet<Key>();bool valid=true;
    foreach(int raw in data.keys){var key=(Key)raw;if(!IsBindable(key)||!seen.Add(key)){valid=false;break;}}
    if(valid){
     for(int i=0;i<data.keys.Length;i++)bindings[(int)editable[i]]=(Key)data.keys[i];
     bindings[(int)GameAction.Interact]=bindings[(int)GameAction.MoveUp];
     if(legacy){bindings[(int)GameAction.Flashlight]=MigratedFlashlightKey(seen);Save();}
    }
   }
  }catch(ArgumentException){/* Old/corrupt preferences must never prevent play. */}
  Revision++;
 }
 static Key MigratedFlashlightKey(HashSet<Key> used){
  bool arrows=bindings[(int)GameAction.MoveLeft]==Key.LeftArrow&&bindings[(int)GameAction.MoveRight]==Key.RightArrow;
  var preferred=arrows?new[]{Key.V,Key.I,Key.T,Key.G,Key.B}:new[]{Key.I,Key.V,Key.T,Key.G,Key.B};
  foreach(var key in preferred)if(!used.Contains(key))return key;
  // A custom map may already use both new preset keys. Pick a free key without
  // moving any saved action, even when every preferred fallback is occupied.
  foreach(Key key in Enum.GetValues(typeof(Key)))if(IsBindable(key)&&!used.Contains(key))return key;
  return Key.None; // Unreachable: ten legacy bindings cannot exhaust the keyboard.
 }
 static void ReleaseSuppressedKeys(){
  if(suppressed.Count==0)return;
  var keyboard=Keyboard.current;if(keyboard!=suppressedKeyboard||keyboard==null){suppressed.Clear();suppressedKeyboard=keyboard;return;}
  // Entry animations and menus may not poll gameplay at all. Observe releases
  // on the existing Input System clock so the next fresh press still works.
  foreach(var key in keyboard.allKeys)if(!key.isPressed)suppressed.Remove(key.keyCode);
 }
 public static void SuppressUntilRelease(){
  // Refresh the snapshot: a captured key may have been released while the menu
  // was open and gameplay never polled it. That old latch must not eat a new press.
  var keyboard=Keyboard.current;suppressed.Clear();suppressedKeyboard=keyboard;
  if(keyboard==null)return;
  foreach(var key in keyboard.allKeys)if(key.isPressed)suppressed.Add(key.keyCode);
 }
 static KeyControl Control(GameAction action){
  var keyboard=Keyboard.current;if(keyboard==null)return null;
  if(keyboard!=suppressedKeyboard){suppressed.Clear();suppressedKeyboard=keyboard;}
  var key=Get(action);if(key==Key.None)return null;var control=keyboard[key];
  if(suppressed.Contains(key)){if(!control.isPressed)suppressed.Remove(key);return null;}
  return control;
 }
 public static bool IsHeld(GameAction action)=>Control(action)?.isPressed??false;
 public static bool WasPressed(GameAction action)=>Control(action)?.wasPressedThisFrame??false;
 public static bool WasReleased(GameAction action)=>Control(action)?.wasReleasedThisFrame??false;
 // Used by the adapter's reset latch, independent of menu suppression.
 public static bool AnyGameplayKeyHeld(){EnsureLoaded();var keyboard=Keyboard.current;if(keyboard==null)return false;foreach(var action in editable)if(keyboard[Get(action)].isPressed)return true;return false;}
}
}
