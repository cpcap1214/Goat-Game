# Chapter 2 flashlight

The flashlight is a Chapter 2 ability prototype, available for testing through **Main Menu → Debug Mode → 14 Chapter 2 Flashlight**. The scene is `Assets/_OneMoreTime/Scenes/Debug_ChapterTwoFlashlight.unity`. Chapter 1 and the intro keep their existing progression; this does not choose a Chapter 2 unlock location or create its final rooms.

Tap **I** with the WASD preset or **V** with the arrow-key preset (or the custom key shown in the playtest HUD) to activate the flashlight. The action is also configurable in **Main Menu → Key Bindings**. The light points in the goat's facing direction. It parts darkness and mist within a six-unit cone, with a small clear area around the goat. The default burst lasts four seconds, then needs three seconds to recharge. Edges and activation/expiration soften visually so the obscurity returns instead of being deleted. Darkness and mist use the same light geometry. The light reveals the existing level and does not change platform collision. Mist uses drifting translucent ribbons; both its animation and flashlight timing use the room simulation clock. Repeated presses cannot extend an active burst, and holding the key does not automatically retrigger it after recharge.

The prototype contains a 48-unit cavern lane with a dry entrance, a darkness section, a central refuge, a mist section and a lit far bank. Existing grass/dirt, wooden platforms, rock formations, roots and ruins provide visual targets for testing. A lower recovery floor makes missed jumps forgiving. The camera retains the main game's 20-unit width and bounds, with no colliding ceiling. Retry restores the test; Escape pauses or returns to the menu. This is an ability sandbox rather than a proposed Chapter 2 layout.

## Authoring

Run **Tools → One More Time → Build Chapter 2 Flashlight Sandbox**, or `OneMoreTime.EditorTools.ChapterTwoFlashlightBuilder.Build()` through the Unity pipeline. The generator owns only `Chapter 2 Flashlight Managed` in the debug scene and three prefabs in `Assets/_OneMoreTime/Prefabs/ChapterTwo/`:

| Prefab | Use |
|---|---|
| `FlashlightAbility.prefab` | Assign the player motor, then enable `unlocked` at the intended Chapter 2 acquisition point. The reusable prefab starts locked. |
| `DarknessVolume.prefab` | Position and size a dark world region; assign its flashlight reference. |
| `MistVolume.prefab` | Position and size a drifting mist region; assign the same flashlight reference. |

Tune `activeDuration`, `cooldownDuration`, `range`, `nearRadius` and `halfAngle` on `FlashlightAbility`. Each obscurity volume exposes its `kind`, `size`, `opacity`, `tint` and shader. Volumes are visual and have no colliders. Keep their assigned shader reference so builds include the shader. The sandbox is the only scene where the ability starts unlocked.

The room driver owns the fixed simulation clock. A later Chapter 2 room should tick the flashlight from its existing simulation owner rather than adding a second movement or ability clock. Pausing must stop its activation and recharge timers. The flashlight does not consume or overwrite the player's shutter or replay memory.

The generator opens its target additively, refuses to rebuild a dirty target scene, preserves unrelated loaded scene state, and updates only its own build-settings entry. Rebuilding replaces its managed contents; place experiments outside that managed root or retain a separate scene before regeneration.

## Verification

Actual compilation, scene generation, play tests and rendered checks are recorded with evidence in `STATUS.md` and `TEST_REPORT.md`. The sandbox is a functional prototype; final Chapter 2 acquisition, puzzles, duration balance and human difficulty testing remain separate design work.

Implementation: `Runtime/FlashlightAbility.cs` contains the opt-in timer and beam query; `Runtime/ChapterTwoObscurity.cs` and `Shaders/ChapterTwoObscurity.shader` render local volumes; `Runtime/ChapterTwoFlashlightPlaytest.cs` owns sandbox input, movement, camera, pause and retry. Existing ten-key saved profiles migrate to the new binding map without losing custom keys.
