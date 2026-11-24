# Statue revival

Activated main-game checkpoints now revive the goat through a temporary stone shell. This uses the existing statue and goat sprites; no replacement character art, level geometry, prefab rebuild or new collision surfaces were needed.

## Evaluation and chosen implementation

The existing stone goat already supplies cracked stone, green eyes and an aura, so a breakout is a good visual fit. A literal hop or step out is unsafe on the first Room 3 checkpoint: its grass shelf spans x=10–12 and the checkpoint sits at x=11.5. The motor therefore stays at the original spawn point throughout the effect. Only the renderer receives a tiny vertical settling offset. The statue is larger than the live goat; the effect reveals the correctly sized player rather than stretching the player to match it.

The first Unity preview used large shell fragments and the bright aura frame. They hid the goat and made the green outline break into pieces. The refined version uses the cracked stone frame, smaller triangular fragments cropped to the visible sprite, excludes the pedestal, disperses earlier and draws the debris behind the emerging goat. Following the November 23 playtest correction, the broken statue leaves only its pedestal; the full statue no longer returns behind the player. Later respawns briefly recreate a temporary shell for the same breakout while the permanent checkpoint continues to display its base.

Respawn also sets the motor's facing to the statue's direction, including both SpriteRenderer flipping and mirrored parent transforms. This direction persists when normal animation/control resumes. Checkpoint coordinates and collision geometry do not change. Modern checkpoints crop the existing stone texture to its painted plinth while preserving its world-space pivot; the older player-sprite checkpoints use a small gray plinth and hide the separate eye indicator. The texture rectangle itself excludes the goat, so Unity renderer updates cannot reconstruct the statue from its full source texture. Activating another checkpoint, ordinary animation ticks, repeated retries, and interrupted revival do not restore a consumed statue. The art gallery's explicit C reset can still restore its exhibit.

Timing, on the existing fixed simulation clock:

- 0.00–0.16 s: player hidden; eye/crack energy builds in the statue.
- 0.16–0.42 s: shell opens into textured stone pieces; the live goat fades in and settles.
- 0.42–0.64 s: remaining debris vanishes while the pedestal stays in place.
- After 32 fixed ticks: normal control and the room simulation resume.

Gameplay is suspended during these 0.64 seconds, including recorded playback, floating logs and timed mechanisms. Pause freezes the revival itself. Buffered actions are discarded at the boundary so holding record/jump/retry does not produce an accidental action when the goat returns. This is an implementation review and targeted test evaluation; the repeated-death feel still benefits from user playtesting.

## Where it runs

- Main Rooms 1–2: original grayscale goat statues, with green pulse and shell pieces.
- Main Room 3: both original statues, preserving the narrow ledges.
- Main Rooms 4–8: existing cracked stone-goat checkpoints; drowning finishes before revival begins.
- A retry/death before activating any statue uses the original spawn without a statue animation.
- Recording completion, death/retry while recording, world rewind and temporary motor movement for checkpoint-anchor capture do not trigger revival. Saved recordings and shutter snapshots retain their prior reset rules.

The intro, legacy tutorial/EchoFoothills and Chapter 2 flashlight sandbox retain their existing respawn presentation.

## Try it

Open a main room, activate a statue, then die or press the configured Retry key (R by default). For a quick visual preview use **Main menu → Debug Mode → World 1 art preview**, section **1**, and press **V**. C still previews statue activation; Space previews drowning. The gallery demonstration uses the same visual component and returns its display goat afterward.

## Source

- `Assets/_OneMoreTime/Runtime/WorldOne/StatueRevival.cs`: existing-sprite shell, triangular meshes, green energy, fixed-time animation and cleanup.
- `Assets/_OneMoreTime/Runtime/MainRoom.cs`: identifies the saved statue, starts explicit revival and supplies the shared ticking guard.
- `Assets/_OneMoreTime/Runtime/RewindPlaytest.cs`: actual non-recording Retry notification; recording rewind deliberately excluded.
- `Assets/_OneMoreTime/Runtime/ChapterOneRoom.cs` and `Runtime/RoomFour/Room4AbilityTutorial.cs`: suspend each custom room clock during revival.
- `Assets/_OneMoreTime/Runtime/WorldOne/WorldOneArtGallery.cs`: V preview.
- `Assets/_OneMoreTime/Tests/PlayMode/StatueRevivalTests.cs`: integration checks; existing checkpoint/drowning regressions also cover the new lifecycle.

Temporary meshes are built at the start, use the existing atlas/material, add no colliders and are destroyed on finish/cancel/scene unload. The statue animation component retains its original enabled state but displays the persistent pedestal after completion or cancellation. One runtime base sprite is reused per checkpoint and destroyed with that checkpoint, along with the small opaque primitive texture used by legacy checkpoints. Alpha bounds are cached per sprite rather than scanned every tick.

The focused `StatueRevivalTests` include motor/renderer facing after both kinds of mirroring, low pedestal geometry, repeated respawns and cleanup, legacy eye removal, checkpoint changes, cancellation, pause, recorded-memory retention and safe spawn positions. November 23 Unity execution is coordinated in the combined level-fix validation; source inspection alone is not recorded as a Unity test pass.
