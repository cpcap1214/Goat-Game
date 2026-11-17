using UnityEngine;
using UnityEngine.InputSystem;
namespace OneMoreTime {
[System.Serializable] public struct InputFrame { public float move; public bool jump, held, drop, freeze, trace, retry; }
public interface IPlayerInput { InputFrame Consume(); void Clear(); }
public sealed class KeyboardPlayerInput : IPlayerInput {
 InputFrame frame; bool suppressed;
 public void Poll(bool continuousOnly=false) {
  var k=Keyboard.current; if(k==null)return;
  if(suppressed) { if(!k.spaceKey.isPressed&&!k.qKey.isPressed&&!k.eKey.isPressed&&!k.rKey.isPressed&&!k.aKey.isPressed&&!k.dKey.isPressed&&!k.leftArrowKey.isPressed&&!k.rightArrowKey.isPressed)suppressed=false; return; }
  frame.move=(k.dKey.isPressed||k.rightArrowKey.isPressed?1:0)-(k.aKey.isPressed||k.leftArrowKey.isPressed?1:0);
  frame.held=k.spaceKey.isPressed;
  if(continuousOnly){ClearActions();return;}
  frame.drop|=k.spaceKey.wasPressedThisFrame&&k.downArrowKey.isPressed;
  frame.jump|=k.spaceKey.wasPressedThisFrame; frame.held=k.spaceKey.isPressed;
  frame.freeze|=k.eKey.wasPressedThisFrame;frame.trace|=k.qKey.wasPressedThisFrame;frame.retry|=k.rKey.wasPressedThisFrame;
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
