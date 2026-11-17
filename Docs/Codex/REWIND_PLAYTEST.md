# Rewind playtest

Open Main Menu → Debug Mode → **06 Basic Replay / 基礎回放** for forward-only replay, or **07 Upgraded Replay / 進階回放** for direction switching and hold-to-pause. Both retain the final afterimage. Directly opening the scene defaults to basic. You can also open `Assets/_OneMoreTime/Scenes/Debug_Rewind.unity` and press Play.

- Hold Q for **0.5 seconds** to begin recording from the current position and replace any older recording. A clockwise ring fills above the goat while charging. Release Q and continue playing.
- Tap Q to finish, or let the **10-second** recording finish automatically.
- The goat and participating objects visibly rewind at **5x speed** to the state when recording began. The level does not reload.
- Recording adds thicker 42-pixel film strips above/below the game, moving at 180 UI pixels/second with a 0.16-second soft reveal from the left, a warm tint and fine grain.
- The recorded route remains visible as a thin, faint trace. Tap Q to start playback; tapping Q during playback restarts from the beginning, facing forward in time, without replacing the recording or resetting the live player; it can press the gate plate and carry the goat, with its support line hidden. The minimal timeline is only a warm amber bar (display only, no seeking), matching the subtle amber trace and afterimage. Hold Q again for a fresh recording.
- The afterimage stays active at the final frame. In the upgraded version only, hold **Left Shift** to pause only the afterimage; release to resume in the same direction. A quick tap (under 0.25 seconds) switches direction on release. Tap **Left Shift** to switch forward/backward from the current frame; at either endpoint it stops and remains usable as a platform/plate weight. Switching direction resumes it from that endpoint. Before playback, Left Shift selects the direction; Q starts from the corresponding end. The top status text indicates direction and HELD at endpoints. Holding Q for a new recording resets direction to forward.
- A/D or arrows move, Space jumps, Down+Space drops through wooden platforms, Esc pauses and offers a return to the menu. R finishes an active recording; otherwise it restarts this practice room while keeping the trace.

Try recording while pushing a crate, or while riding the moving platform. Finish to see both the player and world return to their earlier state. To test the plate, stand on it while recording for several seconds, finish, move toward the gate, then play the trace.

## Choosing affected objects

In the Hierarchy, select a crate, the moving platform, the pressure plate or the gate. Its gameplay component exposes **Affected By Rewind** (`affectedByRewind`). Enabled objects restore their recorded state; disabled objects retain their end state throughout the reverse animation. This setting has no gameplay label. The two crates have separate fixed floor lanes and are pushed by walking into them.

This prototype is a separate debug entry; the original four tutorials and EchoFoothills remain available. New dynamic object types require explicit snapshot support. Full Rigidbody2D physics history, other scenes, music and arbitrary script state are outside this debug prototype.

Regenerate only this scene with **Tools → One More Time → Build Rewind Playtest**. The goat and echo use existing prefabs; the new material and scene include Unity-generated metadata.

## Later-game upgrade

`RewindPlaytest.ReplayUpgradeUnlocked` defaults to false. A future pickup can call `UnlockReplayUpgrade()` on the controller; no pickup location is assigned yet. Debug entries share the practice scene but select separate ability tiers through `SceneFlow.LoadRewindPlaytest(false/true)`. The launch choice is consumed once and does not leak into later basic sessions.
