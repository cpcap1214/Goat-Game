using System.Text.RegularExpressions;

namespace OneMoreTime {
// Legacy scene instructions keep their authored text. Resolve only at a control
// prompt's draw boundary so binding swaps never cascade into another action.
public static class ControlPrompt {
 const string Tokens=@"(?<move>\[A\]\s*\[D\]\s+or\s+\[←\]\s*\[→\]|A\s*/\s*D|A\s+D)|(?<all>WASD)|(?<drop>Down\s*\+\s*Jump)|(?<down>\[↓\]|Down(?:\s*Arrow)?)|(?<key>Left\s+Shift|Right\s+Shift|Shift|SPACE|Space|空白鍵|Escape|Esc|[EQFXR])";
 static readonly Regex PromptTokens=new Regex(@"(?<![A-Za-z0-9_])(?:"+Tokens+@")(?![A-Za-z0-9_])",RegexOptions.CultureInvariant);
 static readonly Regex KeyTokens=new Regex(@"(?<![A-Za-z0-9_])(?:"+Tokens+@"|(?<direction>[WASD]))(?![A-Za-z0-9_])",RegexOptions.CultureInvariant);
 public static string Format(string text)=>string.IsNullOrEmpty(text)?text:PromptTokens.Replace(text,Resolve);
 public static string Key(string text)=>string.IsNullOrEmpty(text)?text:KeyTokens.Replace(text,Resolve);
 public static string CompactKey(string text){
  var resolved=Key(text);if(string.IsNullOrEmpty(resolved))return resolved;
  return resolved.Replace("Left Arrow","←").Replace("Right Arrow","→").Replace("Up Arrow","↑").Replace("Down Arrow","↓").Replace("Left Shift","LShift").Replace("Right Shift","RShift").Replace("Left Command","LCmd").Replace("Right Command","RCmd");
 }
 static string Resolve(Match match){
  if(match.Groups["move"].Success){
   var pair=KeyBindings.Label(GameAction.MoveLeft)+" / "+KeyBindings.Label(GameAction.MoveRight);
   return match.Value.StartsWith("[")?"["+KeyBindings.Label(GameAction.MoveLeft)+"] ["+KeyBindings.Label(GameAction.MoveRight)+"]":pair;
  }
  if(match.Groups["all"].Success)return KeyBindings.Label(GameAction.MoveUp)+" / "+KeyBindings.Label(GameAction.MoveLeft)+" / "+KeyBindings.Label(GameAction.MoveDown)+" / "+KeyBindings.Label(GameAction.MoveRight);
  if(match.Groups["drop"].Success)return KeyBindings.Label(GameAction.MoveDown)+" + "+KeyBindings.Label(GameAction.Jump);
  if(match.Groups["down"].Success)return match.Value.StartsWith("[")?"["+KeyBindings.Label(GameAction.MoveDown)+"]":KeyBindings.Label(GameAction.MoveDown);
  switch(match.Value){
   case "A":return KeyBindings.Label(GameAction.MoveLeft);
   case "D":return KeyBindings.Label(GameAction.MoveRight);
   case "W":return KeyBindings.Label(GameAction.MoveUp);
   case "S":return KeyBindings.Label(GameAction.MoveDown);
   case "E":return KeyBindings.Label(GameAction.Shutter);
   case "Q":return KeyBindings.Label(GameAction.RecordReplay);
   case "F":return KeyBindings.Label(GameAction.Filter);
   case "X":return KeyBindings.Label(GameAction.Interact);
   case "R":return KeyBindings.Label(GameAction.Retry);
   case "SPACE":case "Space":case "空白鍵":return KeyBindings.Label(GameAction.Jump);
   case "Escape":case "Esc":return KeyBindings.Label(GameAction.Pause);
   default:return KeyBindings.Label(GameAction.ReplayDirection);
  }
 }
}
}
