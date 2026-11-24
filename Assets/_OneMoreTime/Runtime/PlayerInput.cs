using UnityEngine;
using UnityEngine.InputSystem;
namespace OneMoreTime {
[System.Serializable] public struct InputFrame { public float move; public bool jump, held, drop, freeze, trace, retry; }
public interface IPlayerInput { InputFrame Consume(); void Clear(); }
public sealed class KeyboardPlayerInput : IPlayerInput {
 InputFrame frame; bool suppressed;int bindingRevision=KeyBindings.Revision;
 public void Poll(bool continuousOnly=false) {
  var k=Keyboard.current; if(k==null){frame=default;return;}
  if(bindingRevision!=KeyBindings.Revision){frame=default;bindingRevision=KeyBindings.Revision;}
  if(suppressed) { if(!KeyBindings.AnyGameplayKeyHeld())suppressed=false; return; }
  frame.move=(KeyBindings.IsHeld(GameAction.MoveRight)?1:0)-(KeyBindings.IsHeld(GameAction.MoveLeft)?1:0);
  frame.held=KeyBindings.IsHeld(GameAction.Jump);
  if(continuousOnly){ClearActions();return;}
  frame.drop|=KeyBindings.WasPressed(GameAction.Jump)&&KeyBindings.IsHeld(GameAction.MoveDown);
  frame.jump|=KeyBindings.WasPressed(GameAction.Jump);
  frame.freeze|=KeyBindings.WasPressed(GameAction.Shutter);frame.trace|=KeyBindings.WasPressed(GameAction.RecordReplay);frame.retry|=KeyBindings.WasPressed(GameAction.Retry);
 }
 public InputFrame Consume(){var f=frame;frame.jump=frame.drop=frame.freeze=frame.trace=frame.retry=false;return f;}
 // Shutter pauses discard button edges, but keep live movement and jump-hold state.
 public void ClearActions(){frame.jump=frame.drop=frame.freeze=frame.trace=frame.retry=false;}
 public void Clear(){frame=default;suppressed=true;}
}
public sealed class TestPlayerInput : IPlayerInput {
 public InputFrame frame;
 public InputFrame Consume(){var f=frame;frame.jump=frame.drop=frame.freeze=frame.trace=frame.retry=false;return f;}
 public void Clear(){frame=default;}
}
}
