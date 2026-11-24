# Room 5 — Record & Replay acquisition (current guide)

Updated: **2026-11-24**. This file keeps its original name for existing links. The original Room 4 acquisition scene is now **Room 5**, while the former Room 6 riverworks scene is now Room 4 and the former Room 5 broken crossing is now Room 6. Scene assets were moved to their new filenames with their GUIDs preserved; legacy `RoomFour*` filenames/classes and reusable prefab references remain intentionally stable.

- Open `Assets/_OneMoreTime/Scenes/Main_Room05.unity`, or **Main menu → Debug Mode → 10 Room 5 / 錄影與水路**.
- Current progression: **Room 3 lift → Room 4 riverworks → Room 5 acquisition → Room 6 bridges → Room 7 → Room 8**. Debug entries 9/10/11 are riverworks/acquisition/bridges respectively.
- Room 5 enters horizontally from **(0.5,4)** and walks to **(2,4)** with a fade-in; `MainRoom.lift` is null. Its exit leads to `Main_Room06`. The existing pickup, bridge collapse, slow fall, checkpoint, water route and imaginary rewind remain intact.
- Rebuild only acquisition with **Tools → One More Time → Build Main Room 5** or `ChapterOneExpansionBuilder.BuildRoom(5)` (delegates to the stable `RoomFourBuilder`). Apply the existing in-place patch through **Update Existing Room 5** (`RoomFourUpdate.Apply`). The managed root is **Room 5 Managed**.
- `Room4AbilityTutorial.SceneName` is `Main_Room05`; its next scene is `Main_Room06`. The controller supports the new horizontal entry and retains its optional lift-arrival branch for compatibility.
- Room 4 has Shutter only and never auto-grants recording, including Debug entry. Room 5's tape pickup grants basic replay. Hold the bound recording key for **0.25 seconds** at the prompt to start the imaginary rewind; the existing natural bank → pickup → fall trace plays backward to the safe bank. Normal recording/replay then uses the shared simulation clock.
- Returning to the main menu clears the acquisition unlock so its animation plays again next entry. Deaths/retries preserve progress and saved memory. Menu return preserves the death count; New Game resets it.

The full current geometry and entry/exit coordinates for the reordered rooms are in [the Rooms 4–8 guide](ROOMS_5_TO_8.md). New validation belongs to `STATUS.md` / `TEST_REPORT.md`; no new test result is asserted here.

## Historical implementation records — superseded numbering

**Everything below records the original Room 4 work from 2026-11-18 onward.** Old scene/menu numbers, lift entry, completion placeholder, hold times, crate settings and test counts are historical, not current setup instructions. Later dated sections supersede earlier behavior; the guide above supersedes their room numbers and entry/exit wiring. The records and evidence paths are preserved for traceability.

## Original Room 4 — Record & Replay water route

Open `Assets/_OneMoreTime/Scenes/Main_Room04.unity`, or Main Menu → Debug Mode → **09 Room 4**. Normal progression now goes from Room 3's existing lift directly to Room 4. No manual scene setup is required. Rebuild only this room with **Tools → One More Time → Build Main Room 4**. The builder refuses unsaved Room 4 / Room 3 edits. It manages only the `Room 4 Managed` root and changes Room 3's next-scene reference; existing landing scene remains available.

## Layout and reused content

The supplied drawing is interpreted at existing player scale: entrance dock (2,4), approach ground x=-2…16, breakable bridge x=16…23 at y=4, pickup at (19.5,4.7), first pit surface y=0 and wall-attached exit ledge, bank x=23…29, lower water x=29…40 at y=2, vertical waterfall at x=39, upper water x=40…59 at y=4, crate source x=58, exit bank x=59…70 at y=5. The route ends at x=64 with a “more to come” screen because Room 5 is not defined.

Reused: Player and FreezeEcho prefabs, MainRoomPlayerTuning, RoomLift and OldMountainLift, basic RewindPlaytest recording/rewind/trace/replay and HUD, ShutterAbility, WorldOne backdrop, river/waterfall animation tiles and shader, GoatDrowning and WaterHazard, WoodenSuspensionBridge and its sliced sprites, GrassDirtLedge, WeatheredWoodPlatform, Sunny Land terrain/crate sprite, TraceHourglass relic, GameUI/font/audio. No new raster artwork was created.

Movement remains speed 6, jumpSpeed 14.178182, gravity 32, body 0.7×1 units; fixed step is 0.02 seconds. No Room 1–3 geometry changes. Room 3 receives only destination wiring plus Unity serialization of already-existing default fields.

## Tutorial and persistence

The existing lift ascends seven units while carrying the player and fades in. The purple pickup grants the basic replay ability; an invisible tall wall prevents bypass. Crossing its pickup zone disables that wall and suppresses movement/actions for the scripted fall. Bridge collision turns off after 0.25 seconds, and four independent timber segments fall, rotate, separate and fade. The player falls using the existing motor, not Rigidbody gravity or a teleport.

Camera zoom interpolates from the normal 20-unit-wide framing to orthographic size 3. During the fall, selective simulation speed eases from 1 to 0.12 over 0.5 seconds, starting 3.6 units above the water. This room's short four-unit fall requires starting early enough to visibly decelerate. The existing motor receives smaller integration steps; global Time.timeScale is never changed. The actual falling position is held about 0.08 units above the water (0.015 tolerance), without teleporting upward. Water death is suppressed during the sequence. The prompt and charge ring wait for the real replay engine to enter Recording after a full 0.5-second Q hold; an already-held Q must be released first.

Recording resumes movement, water consequences and the right-side crate puzzle. Simulation speed eases back to 1 over 0.45 seconds using the shared fixed recording clock; camera zoom restores over 0.7 seconds, even if drowning begins during recovery. The first-pit rescue crate has been removed. There is no automatic safe-bank teleport or checkpoint: the stone goat at (26,4), three units left of the bank edge at *6, must actually be approached. MainRoom.CheckForCheckpoint uses the existing proximity/save behavior and RoomCheckpoint.Save activates the existing cracked green-eye/aura animation. Its location becomes the replay retry anchor, so deaths/R retain saved memory and return to the statue once activated. Before that, the entrance anchor remains in force. Death while actively recording still follows the existing visible rewind behavior.

`MainGameProgress.BasicReplayUnlocked` is **session/journey state**, matching the project's main-room progression scope; it is not a new disk save. Scene reloads, menu visits and R preserve it. Starting a new journey resets it. An already-owned arrival skips the pickup/wall/cutscene and loads the bridge broken. Basic replay only: no Shift upgrade is granted. Shutter E remains available when replay memory is not occupying the single-memory slot; starting a recording clears the shutter copy.

The Input System asset contains generic Player/UI actions but no Record action. Existing live gameplay already polls Q via `Keyboard.current`; Room 4 uses that same control, not a second action map. The prompt describes its actual 0.5-second binding.

## Floating crates and rewind

CrateSpawner owns a fixed pool of six upper-channel crates and a spawn countdown. It advances on the replay simulation clock; no competing FixedUpdate or uncontrolled Rigidbody simulation moves them. Each FloatingObject2D has a kinematic Rigidbody2D and one-way collider, with a damped spring toward the current's waterline, gravity outside the flotation band, subtle bobbing and constrained rotation. Left current accelerates toward the configured velocity. The player motor applies carrier delta once through collision-aware movement, preserving jump velocity and drop-through behavior.

CrateDisposalTrigger filters FloatingObject2D instances belonging to its owner; it cannot delete the player. Explicit boundary checks support fixed-step tests as well as trigger callbacks. Pooling prevents allocation accumulation. RewindParticipant snapshots include every pool slot's active flag, position, velocity and bob phase plus spawn time. Rewinding therefore restores spawned/despawned crates and time until the next spawn. `affectedByRewind` on the spawner labels the pool as rewindable. The existing debug crate system remains unchanged.

## Inspector defaults

| Setting | Default |
|---|---|
| Bridge pieces / delay | 4 / 0.25 s |
| Segment lateral impulse / gravity | 1.8 / 10 |
| Fade delay / duration | 0.35 / 1.1 s |
| Zoom target / in / out | size 3 / 0.65 s / 0.7 s |
| Hold distance / tolerance | 0.08 / 0.015 units above water |
| Slowdown start / duration / minimum speed | 3.6 units above water / 0.5 s / 0.12 |
| Speed restoration | 0.45 s |
| Checkpoint | (26,4), 3 units left of the bank edge |
| Pickup radius | 0.6 horizontal units |
| Upper crate interval / limit | 8 s / 6 |
| Initial horizontal velocity / water current | -1 / -1 units/s |
| Upper spawn point / disposal x | (63.5,4.8) / 40 |
| Buoyancy spring / damping / dry gravity | 36 / 9 / 18 |
| Float center offset / bob amplitude / angular frequency | 0.35 / 0.04 / 1.8 |
| Rescue pool | one crate, current 0, respawn interval 999 s |

The pool size is six authored slots; maximumActiveCrates can cap it further. Add slots to the pool array to support a larger pool. Position spawnPoint / disposal and water limits in Inspector. Breakable pieces are authored references, so their count can be changed with corresponding sprites. Both new reusable prefabs are saved: `Room4BreakableBridge.prefab` and `FloatingWaterCrate.prefab`; assign a WaterCurrent2D when using a standalone floating crate.

## Changed-file inventory (this task only)

New scripts (each has a Unity-generated `.meta`):
- Assets/_OneMoreTime/Runtime/RoomFour/RewindParticipant.cs
- Assets/_OneMoreTime/Runtime/RoomFour/WaterCurrent2D.cs
- Assets/_OneMoreTime/Runtime/RoomFour/FloatingObject2D.cs
- Assets/_OneMoreTime/Runtime/RoomFour/CrateDisposalTrigger.cs
- Assets/_OneMoreTime/Runtime/RoomFour/CrateSpawner.cs
- Assets/_OneMoreTime/Runtime/RoomFour/BreakableBridge.cs
- Assets/_OneMoreTime/Runtime/RoomFour/MainGameProgress.cs
- Assets/_OneMoreTime/Runtime/RoomFour/Room4AbilityTutorial.cs
- Assets/_OneMoreTime/Editor/RoomFourBuilder.cs
- Assets/_OneMoreTime/Tests/PlayMode/RoomFourTests.cs
- Assets/_OneMoreTime/Runtime/RoomFour.meta (folder metadata)

Existing scripts changed:
- Assets/_OneMoreTime/Runtime/RewindPlaytest.cs — optional external clock/presentation, nullable mechanisms, snapshot participants, suspended charging and retry anchor.
- Assets/_OneMoreTime/Runtime/RewindPlaytestHUD.cs — configurable room-specific footer/pause title; original debug defaults retained.
- Assets/_OneMoreTime/Runtime/PlayerMotor2D.cs — opt-in moving-crate carry through IPlayerCarrier, no tuning changes.
- Assets/_OneMoreTime/Runtime/MenuScreen.cs — Room 4 debug entry and new-journey progression reset.
- Assets/_OneMoreTime/Editor/MainRoomContentBuilder.cs — future Room 3 rebuilds retain Room 4 destination.
- Assets/_OneMoreTime/Tests/PlayMode/VerticalRoomTests.cs — lift integration now expects Room 4 instead of placeholder landing.

Scenes, prefabs and generated assets:
- Assets/_OneMoreTime/Scenes/Main_Room04.unity (+ .meta), new managed scene.
- Assets/_OneMoreTime/Scenes/Main_Room03.unity, destination/reference serialization only.
- Assets/_OneMoreTime/Prefabs/Room4BreakableBridge.prefab (+ .meta).
- Assets/_OneMoreTime/Prefabs/FloatingWaterCrate.prefab (+ .meta).
- Assets/_OneMoreTime/Generated/Room4TraceRelic.asset (+ .meta), sprite referencing the existing hourglass texture.
- ProjectSettings/EditorBuildSettings.asset, Room 4 appended.

Documentation: Docs/Codex/ROOM_FOUR.md, STATUS.md, TEST_REPORT.md, LEVELS.md. Evidence lives under Artifacts/Logs/room4-*. Existing uncommitted art work from earlier requests was preserved and is not part of this Room 4 change inventory.

## 2026-11-18 Room 4 implementation and validation

Implemented Main_Room04 from the supplied layout: existing Room 3 lift connection, basic replay pickup and anti-bypass wall, segmented bridge destruction, gravity fall and camera zoom, suspended tutorial waiting for actual 0.5-second Q recording activation, water/platform route, rewindable floating crate pool, and room completion. Existing movement tuning and Room 1–3 layouts are preserved. Full file inventory, reused assets, Inspector defaults, persistence, assumptions and rebuild instructions: [ROOM_FOUR.md](ROOM_FOUR.md). No manual scene setup required; Main Menu → Debug Mode → 09 Room 4.

PASS — real Unity compilation, zero errors/warnings; RoomFourTests 5/5; full PlayMode suite 98/99; entrance/fall/composited tutorial screenshot inspection. Tests cover real recording activation, camera restoration, natural landing, unchanged-jump traversal through the crate route, buoyancy/carry, spawn/despawn rewind, bounded pool, reload/new-journey semantics and lift wiring.
FAIL (pre-existing, outside Room 4 scope) — VerticalRoomTests.SharedPlateControlsIndependentThreeAndTwentySecondTimers expects 20 seconds, but the unchanged Room 3 scene has 15 seconds. Confirmed the same 15-second serialized value in HEAD; left unchanged to preserve Room 3.
NOT RUN — standalone build and full human playthrough. Automated traversal is not a substitute for human difficulty testing. Unlock persistence is session/journey only; no disk save system was introduced. At this initial validation stage, a rescue crate supported the first recording handoff; it was removed by the subsequent revision below. Room 4 ends with a placeholder completion screen because Room 5 is undefined.

Commands: `unity recompile --format json`; `unity command run_tests --mode playmode --filter OneMoreTime.Tests.RoomFourTests --async_tests --format json`; `unity command run_tests --mode playmode --filter OneMoreTime.Tests --async_tests --format json`; `unity command test_status --format json`. Two unfiltered discovery attempts returned zero tests and were not counted as passes. Evidence: `Artifacts/Logs/room4-full-playmode-final.json` (99 total, 98 passed), `room4-entrance.png`, `room4-tutorial.png`, `room4-tutorial-ui.png`. Earlier initial suite results are retained separately. All changes are local; nothing pushed or published.

## Room 4 visual and tutorial revision (2026-11-18)

Applied to the existing scene with **Tools → One More Time → Update Existing Room 4** (`RoomFourUpdate.Apply`). This patches existing objects in place, is safe to rerun, and refuses unsaved scene edits. The original build command also invokes the patch for future regeneration; this update did not rebuild the room or modify other scenes.

Ground bodies extend to room boundary y=-3. The original colliders remain intact; added dirt pillars/fills are decorative. The first-pit, diagonal-bank and exit-bank ledges now use a horizontal half-slice of the existing triangular ledge, anchored flush to their wall. Their grass tops use the existing one-way collision convention.

`FullBlockWater.asset` is a new reusable opaque sprite slice from the existing WaterAtlas.png, paired with `FullBlockWaterTile.asset` and `FullBlockWater.mat`. The Water Volume shader samples the same atlas interior on a 16-pixel/world-unit grid, mirrors the sample at repeat boundaries and softens interior detail. It does not stretch the surface sprite or create new raster artwork. Water fills extend below the room bottom and overlap the surface's lower edge. They have no colliders. The deep-water trigger tops are 0.5 units below nominal surface; WaterHazard's existing body test means drowning begins after the feet sink about 0.8 units, followed by the existing drowning animation.

### Exact files changed by this revision

- Assets/_OneMoreTime/Runtime/MainRoom.cs — extract existing checkpoint proximity/save logic into a callable method; same behavior in existing rooms.
- Assets/_OneMoreTime/Runtime/RoomFour/Room4AbilityTutorial.cs — selective slowdown, near-water hold, recovery, crate gate, checkpoint retry integration.
- Assets/_OneMoreTime/Runtime/RoomFour/CrateSpawner.cs — opt-in spawning gate; existing default remains enabled.
- Assets/_OneMoreTime/Editor/RoomFourBuilder.cs — retain revision when explicitly rebuilding in future.
- Assets/_OneMoreTime/Editor/RoomFourUpdate.cs and .meta — new in-place scene patch and rendered-review utility.
- Assets/_OneMoreTime/Shaders/WaterVolume.shader and .meta — new reusable full-depth water material shader.
- Assets/_OneMoreTime/Generated/WorldOne/FullBlockWater.asset and .meta — new full-block sprite slice.
- Assets/_OneMoreTime/Generated/WorldOne/FullBlockWaterTile.asset and .meta — new non-colliding water Tile.
- Assets/_OneMoreTime/Generated/WorldOne/FullBlockWater.mat and .meta — new tile material.
- Assets/_OneMoreTime/Generated/WorldOne/WallGrassLedge.asset and .meta — new half-slice of existing ledge art.
- Assets/_OneMoreTime/Scenes/Main_Room04.unity — patched scene, original root/reference identity preserved.
- Assets/_OneMoreTime/Tests/PlayMode/RoomFourTests.cs — revised acquisition test; added slowdown, checkpoint and visual/collider tests.
- Docs/Codex/ROOM_FOUR.md, STATUS.md, TEST_REPORT.md, LEVELS.md — revision and validation records.

Scene objects changed: three ground masses gain `Dirt body to room bottom`; three grassy traversal platforms gain decorative dirt columns; three bank ledges are wall-anchored; each water section gains `Full depth water fill` and lower/deeper `Deep water volume`; `Flowing surface` heights align with their nominal waterline; `First pit floating crate` is removed; upper-channel source starts disabled; `Room 4 stone goat checkpoint` is an instance of the unchanged existing StoneGoatCheckpoint prefab. No prefab files or source PNGs were modified by this revision. Room 1–3 scenes and global player tuning are unchanged.

The latest layout image was not attached to the revision text. The user's clarification placed the statue on the ground left of *6, three units from the edge; all other positions use the existing Room 4 layout. Human difficulty testing remains necessary for the revised first-pit recording/replay sequence without the removed rescue crate.

## 2026-11-18 Room 4 visual / checkpoint / slow-motion revision

Updated the existing scene in place, not a rebuild. Added the existing stone-goat checkpoint at (26,4), three units before the *6 bank edge as clarified by the user. Added decorative soil columns and full-depth water using reusable atlas-sliced assets and Water Volume shader; wall ledges now extend horizontally from their banks. Removed the first-pit rescue crate and gated later crate spawning until actual recording activation. Near-water slowdown eases to 0.12 over 0.5 s, freezes at approximately 0.08 units above water, and restores speed in 0.45 s after the real 0.5-second Q hold. Global time and movement tuning are unchanged. Existing checkpoint save/green-eye/crack animation and retry semantics are reused.

PASS: real Unity batch compilation and targeted scene patch; final RoomFourTests **8/8**; rendered overview inspected with no gap between water surface and body; git diff --check. Room 3 scene diff is identical to its pre-update state. Earlier two new-test failures (late slowdown onset and owned-entry speed ramp) were fixed before the final run.

Full PlayMode regression: **97/102 PASS, 5 FAIL**. One is the previously documented Room 3 15-versus-20-second timer mismatch. Four are keyboard-edge tests: ChapterOneTests.KeyboardTutorialSimultaneousJumpFilterAndPause; InputTests.DownJumpIsConsumedOnce; InputTests.HeldKeysAndResetSuppression; VerticalRoomTests.HeldKeyboardMovementSurvivesShutter. The two unchanged InputTests also fail in an isolated batch run without loading Room 4 (0/2); synthetic held state is visible but expected pressed-frame edges are not. This indicates a batch input-fixture issue, but its exact cause and interactive verification remain unresolved. Unrelated input mechanics/tests were not changed to mask these failures.

NOT RUN: interactive keyboard playthrough, human difficulty assessment, standalone build. Interactive Editor launch exited without establishing Pipeline; validation used the installed Unity 6000.6.0f1 through unity-cli batch commands instead. No claim of live interactive validation. Source compilation succeeded; existing obsolete-API warnings/bridge OnValidate warnings were observed during initial imports.

Commands: `unity run . --log-file /tmp/room4-update-final-editor.log --no-tail --format json -- -executeMethod OneMoreTime.EditorTools.RoomFourUpdate.ApplyAndCaptureReview`; `unity test . --mode PlayMode --filter OneMoreTime.Tests.RoomFourTests --output Artifacts/Logs/room4-update-final-tests.xml --format json`; full suite uses `--filter OneMoreTime.Tests`; isolated keyboard check uses `--filter OneMoreTime.Tests.InputTests`.

Evidence: Artifacts/Logs/room4-update-final-tests.xml, room4-update-full-tests.xml, room4-update-input-isolation.xml, room4-update-overview.png, room4-update-final-editor.log. Exact file/script/asset/scene-object inventory and tuning: [ROOM_FOUR.md](ROOM_FOUR.md). No prefab files or source PNGs changed in this revision. No manual setup needed; open Main_Room04 or Main Menu → Debug Mode → 09 Room 4. All changes local.

## Latest layout correction — 2026-11-18

This section supersedes earlier terrain and crate placement descriptions. Applied the user's latest dot-grid sketch to the existing scene. Only the two marked bank bevels remain; the unrequested first-pit stepping ledge and separate wall-step objects are removed. *6 is a pure-dirt triangular terrain corner at (29,3), reaching inward to (28,2); the far-right bank has its grassy corner at (63,5), sloping to (64,4). Each is part of its bank's polygon collision outline, not a new stepping platform. Reusable DirtTerrainCorner.asset and GrassTerrainCorner.asset use custom triangular sprite geometry with the exact existing 16 px/unit EarthFillSeamless / GrassEdgeSeamless textures. No new painted texture or source PNG changes.

Grassy platforms now use MainRoomArtBuilder.Grass and Room 3's muted, decorative soil-pillar construction, with one-way grass colliders. Centers/grass tops: (32,6), (36,5), (46,5.6); widths 3, 1.5, 4. Wooden step spans x=39.5…42.5 at y=7.5, matching the drawing while retaining a reachable jump from the short grass platform. No movement/jump tuning changes.

Upper stream now spans x=40…64 (24 units); lower water starts at x=28. Surface Tilemaps, fill Tilemaps, drowning volumes, current bounds and crate source are updated together. Crate speed is exactly 1 unit/second leftwards; interval 8 seconds; maximum remains six. Source is (63.5,4.8); disposal remains x=40. The longer stream with wider crate gaps is a replay puzzle; the old test's automatic no-replay jump-to-exit shortcut is no longer an acceptance requirement. Upper-platform access and crate lifetime/rewind/carry are tested independently. Full human puzzle playthrough remains NOT RUN.

Lift arrives at a one-way wooden landing spanning x=0.5…3 at y=4. Entrance dirt begins at x=3; there is no dirt column in the lift shaft. Room width is 70 units and exit trigger is x=68.

Changed files in this correction: Assets/_OneMoreTime/Editor/RoomFourUpdate.cs; Assets/_OneMoreTime/Scenes/Main_Room04.unity; Assets/_OneMoreTime/Tests/PlayMode/RoomFourTests.cs; new Generated/WorldOne/DirtTerrainCorner.asset and GrassTerrainCorner.asset with their Unity-generated .meta files; Docs/Codex/ROOM_FOUR.md, STATUS.md, TEST_REPORT.md, LEVELS.md. No prefab files, runtime movement scripts, other room scenes or original art textures changed in this correction.

## Imaginary acquisition rewind / background variants — 2026-11-18
At the near-water suspension, holding Q for 0.25 seconds now creates a scripted four-second imaginary trace from the solid left bank at (14.5,4) to the suspended position. The existing 5x rewind presents that backwards in 0.8 seconds, directly returning to the bank without rebuilding the scene or restoring the broken bridge. The synthetic trace remains available for playback; normal subsequent recordings still capture actual simulation. Retry anchor moves to the safe bank; crates activate after this cinematic completes. The cutscene suppresses the replay HUD and uses only white bottom text, fading in over 0.35 seconds and out over 0.3 seconds during rewind. Ordinary gameplay/pause HUD remains.

StoneGoatCheckpoint uses sorting order 5 (player 10), including runtime initialization and the saved prefab. Existing older statues already use orders 5/8.

Background assets: `Assets/_OneMoreTime/Generated/Background` (dark tiles) and `Assets/_OneMoreTime/Prefabs/Background` (41 decorative platform/water/bridge variants, including scene-built grassy platforms). Tint is 50% RGB, full original alpha. Prefabs use sorting order -20; place background tilemaps at -20 too. Original materials and water animation are preserved; animated water tiles need the same material as their foreground counterpart. No collisions or gameplay scripts in background prefabs. Foreground assets/layout remain unchanged. Rebuild through Tools → One More Time → Build Background Variants. Builder preserves existing generated asset GUIDs.

## Flow and replay-path refinement — 2026-11-18
Upper-channel current and crate entry velocity are now -2 units/s (leftward), with a 10-second spawn interval. Scene values and rebuild/default settings agree. The acquisition memory walks at bank/deck height from (14.5,4) to pickupFeet, then follows an accelerating fall to the suspended position. Rewind traverses that path in reverse, reaching the pickup before returning along the deck to the bank; the memory remains four seconds and rewind presentation 0.8 seconds.

RewindObjectPaths samples opted-in crates, moving platforms and pooled participant objects alongside actual recording ticks. Saved object paths are thin, low-opacity warm-gray dashed lines, distinct from the orange player trace. Paths break at inactive lifetimes/teleports, and stationary paths are omitted. Rewind applies a muted tint only temporarily, restoring original sprite colors afterward. A replacement recording clears old object paths; opt-out flags remain authoritative. This is a visual addition to world rewind, not a change to normal replay's world-state semantics.

## Crates follow replay — 2026-11-19
Recorded crates now follow the saved replay cursor, including forward playback, upgraded backward playback, holding playback, restarting with Q, and stopping at either endpoint. Floating-crate active state, position, velocity, bob clock and spawner countdown come from the same recorded world frame. While playback owns these crates, their normal current/spawner simulation is skipped to prevent double movement. Opt-out flags still allow ordinary simulation. Player carrying uses the crate's actual per-tick replay displacement; restarting or respawning does not apply a teleport-sized carry delta. Starting another recording releases playback control.

Crates keep original sprite colors during both automatic rewind and replay; dashed traces remain. Other moving-platform tint is unchanged. The imaginary acquisition trace contains no real crate recording, so it does not take control of newly spawned crates. Basic replay still excludes Left Shift until its upgrade is unlocked; the upgraded debug room supports reverse and hold.


## Menu-return acquisition reset — 2026-11-19

Returning to the main menu now clears the session replay unlocks. Re-entering Room 4, including through Debug Mode → 09 Room 4, restores its pickup, blocker, intact bridge and full acquisition sequence (collapse, slow fall, record prompt and imaginary rewind). This also applies when leaving halfway through the sequence. Deaths, retries and ordinary room-to-room transitions keep their existing progress. The cumulative death count is preserved on menu return and still resets only with New Game. The reset lives in `MainGameProgress.ReturnToMenu`, called by `SceneFlow.Load(0)`; no scene, geometry or animation assets changed.


## 2026-11-24 Current Room 5 presentation update

The acquisition prompt is now “Hold [K] to record and rewind”, followed by “Press [K] to replay” once the scripted rewind reaches the left bank. The displayed key follows the current control preset/custom binding. Plain white text eases in over 0.7 seconds to 88% opacity, fades out over 0.3 seconds, pauses with gameplay, and the replay hint dismisses once used.

Removed the four decorative SurfaceEddy instances. One functional swirl sits at x40, y3.62, on the upper channel drain. Crates use the later rooms’ sinking behavior: drift at the existing 2 units/sec, sink at the drain, clip below water y4, then return to their pool at depth2.5. Spawn interval remains10 seconds and sinking remains reversible. The pickup bridge now has purely decorative continuous timber posts and horizontal crosspieces beneath it. Its collision and collapse sequence are unchanged.

Targeted editor updates: `RoomFourUpdate.UpdateCrateDrain(scene)` and `BridgeScaffoldBuilder.Patch(scene)`; the regular builders retain these changes.
