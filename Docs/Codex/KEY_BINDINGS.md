# Keyboard settings

Open **Pause → Options / Key Bindings** (or **Main Menu → 按鍵設定 / Key Bindings**). Choose a preset or click an action's key and press a replacement. Changes save immediately through PlayerPrefs and survive room changes and game restarts. A new profile starts with preset 1. Settings stay inside the paused game: Esc cancels a pending rebind, then returns to the pause panel; another fresh Esc resumes play.

| Action | Preset 1 | Preset 2 |
|---|---|---|
| Move left / right | A / D | Left / Right Arrow |
| Up / interact | W | Up Arrow |
| Down | S | Down Arrow |
| Jump | Space | Space |
| Shutter | J | Z |
| Record / finish / replay | K | X |
| Past filter | L | C |
| Reverse / hold replay | Right Shift | Left Shift |
| Flashlight (Chapter 2) | I | V |
| Retry | R | R |
| Pause | Esc | Esc |

Down + Jump drops through one-way platforms. The main-game recording hold remains 0.25 seconds. Tap the selected reverse key to change playback direction; hold it to pause playback after obtaining the upgrade. Old tutorial levels retain their own memory rules, using the selected physical keys.

Conflicting assignments swap the two actions' keys, so an action cannot accidentally become unbound. Up and interact share one binding. Esc cancels an unfinished capture and remains the fixed pause/back key. Enter, numpad Enter and H are reserved for existing menu/guide controls; dialogue continuation still accepts Enter/Space. Re-select either preset to restore it. The art gallery retains its separate preview shortcuts. The flashlight has its own editable binding and is available in **Debug Mode → 14 Chapter 2 / 手電筒**; adding the binding does not unlock it in Chapter 1.

The persistent bottom gameplay key guide is removed. The world now fills the full 16:9 gameplay viewport; recording film strips, replay timeline and lens effects use its full height. Contextual tutorials, serialized room hints and the recording charge ring still display current bindings. Formatting happens at the control-prompt draw boundary, without rewriting scenes, changing plate A/B identifiers or remapping already-resolved custom labels a second time.

## Implementation

- `Assets/_OneMoreTime/Runtime/KeyBindings.cs`: one map over the existing Unity Input System; unique binding validation, safe defaults for malformed saved data, conflict swaps and held-key suppression. PlayerPrefs key remains `OneMoreTime.KeyboardBindings.v1`; the saved payload is now version 2. Ten-action version 1 profiles migrate without changing existing controls. The new flashlight key defaults to I (WASD) or V (arrows); if a custom profile already uses those keys, migration chooses a free fallback and saves it.
- `Assets/_OneMoreTime/Runtime/ControlsMenu.cs`: the shared `KeyBindingMenu` view, capture cancellation and preset selection. Main menu, main-room pause controllers, legacy tutorials/EchoFoothills and ability debug rooms create it directly; existing scenes need no new serialized references. Each simulation owner routes settings input before pause/gameplay input.
- `PlayerInput.cs` and room controllers resolve movement, jump/drop, ability edges and held record/reverse keys through the same map.
- `ControlPrompt.cs` and the opt-in `GameUI.Control*` methods adapt legacy authored prompts without changing fixed menu/intro controls.

Held inputs are suppressed during binding changes, scene entry and pause transitions. Input System release updates clear suppression even while a cutscene or menu is not polling gameplay, preserving the next fresh keypress.

Automated coverage is in PlayMode `KeyBindingTests` / `InputTests` and EditMode `KeyBindingMenuTests` / `ControlPromptTests`. The keyboard tests use the installed Input System test fixture so real gameplay press/release counters are exercised. See `TEST_REPORT.md` for actual run results and limitations.
