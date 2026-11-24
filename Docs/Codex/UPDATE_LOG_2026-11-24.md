# Chapter 1 update — 2026-11-24

Extends The Forest & The Grand Waterfall from three main rooms to eight connected rooms, using the supplied layouts. The riverworks room now comes before the recording unlock, while the original tutorial and debug content remain available.

## Rooms and progression

- Adds the official Rooms 4–8, their checkpoints, lift/walk transitions, plate-driven platforms, floating-object routes, and waterfall cave finale.
- Moves the former Room 6 to Room 4; recording is acquired in the new Room 5 and the bridge route becomes Room 6.
- Keeps past filter and advanced replay unavailable in the main rooms; their playtests remain in Debug Mode.
- Adds simultaneous-plate counters, camera reveals, a persistent split trapdoor, and a moving checkpoint platform.
- Room 7's boundary waterfalls extend inward and outside the camera edges. Its dispatched platform now shows a centered five-second countdown after arrival, then disappears. The upper-west small deck is two units higher.

## Abilities and interaction

- Uses a 0.25-second recording hold; rewind restores participating objects and freezes them until replay starts.
- Includes floating crates/logs in replay, subtle dashed object traces, source cooldowns, and submerged whirlpool disposal.
- Prevents stationary shutter use from repeatedly lifting the player through the afterimage collider.
- Adds a lens transition and past-background support, plus a Chapter 2 flashlight debug prototype.
- Adds statue-linked revival, pixel ability pickups, and the assembled camera display in the pause menu.

## Art and presentation

- Adds coordinated flowing-water, diagonal-stream, foreground-waterfall, and grand-waterfall assets with looping animation support.
- Extends the looping forest background and adds forest/mountain scenery, floating timber, grass/dirt columns, inward ledges, and background variants.
- Adds wooden bridge pieces with continuous posts and crosspieces, plus wooden directional signs at the main-room exits.
- Sizes ability pickups to roughly 90% of the goat and replaces Room 2's shutter pickup with the camera sprite.
- Adds saved keybinding presets/custom bindings, pause Options, reset, and a main-game death count; removes the bottom keybinding bar.
- Includes twelve replay puzzle designs and development/update documentation.

## Validation status

No new tests were run for this PR, following the user's request to handle playtesting personally. Earlier revisions have recorded passes for 155 ChapterOneRegression checks, four ControlPrompt checks, seven AbilityArt checks, and 22 RoomSeven checks. These are historical results, not validation of the final submitted tree.

The latest Room 7 countdown and platform-height changes have not been playtested. Existing RoomSeven test assertions for the previous indefinite endpoint wait and old platform height remain unchanged and need updating. A complete human chapter playthrough and standalone build have not been performed for this update.

Local screenshots/logs and the local run-in-background setting are excluded from the PR. Unity-generated sprite assets, materials, prefabs, scene metadata, and fixtures are included because the project depends on them.
