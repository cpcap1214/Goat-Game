# Shared main-room upper boundary

Main_Room01, Main_Room02, Main_Room03 and Main_LiftLanding have no physical ceiling. The camera remains clamped to `roomBounds`; the player can jump above the visible top without extending the camera beyond the designed room frame. Side boundaries remain collidable.

---

# Room 3 revision — 黑板 9.png (2026-11-03)

Upper route revised: left wooden step x=3..5/y=27.5; high left grass x=5..8/y=31; high middle grass x=12..14/y=32; upper timed wood x=18..20/y=35, release delay 20 seconds; right wood x=23..24/y=36. Existing 3-second platform and lower section unchanged. Lift/exit stays y=39, dock x=31. Both timers use the same plate, with hidden red links and small counters hidden at zero. Intended to prevent the direct single-freeze jump from the y=25 ledge; complete traversal not tested.

---

# 2026-11-03 Room 3 upper route — 黑板 8.png

Updated only the route above the y=22 timer/checkpoint terrace. Existing terrace, plate, statues, lower platforms, shuttle, pit/death threshold and entrance remain intact. One dot spacing is one unit. Added grass ledges x=2..4/y=25, x=9..11/y=25, x=5..8/y=29 and x=12..14/y=30. The existing plate controls both the relocated 3-second platform x=16..18/y=25 and new 15-second platform x=18..20/y=33. Pressure holds each at its own full time; release counts down independently. Counters remain small, centered and hidden at zero. No red lines or tips were added.

Upper right step is x=23..24/y=35; exit floor and gate plate rise to y=39; room ceiling rises to y=44. Lift dock is (31,39), retaining the user's previous +2 horizontal shift. Original cliff collider remains unchanged; a 6-unit extension adds the new top. Grass remains one-way with decorative dirt pillars. Existing upward lift transition retained.

The scoped Unity builder UpdateRoomThreeUpperSection saves only Main_Room03 and is safe to rerun; full generation shares UpperRoute. ApplyRoomThreePolish now preserves the lift height. Open Assets/_OneMoreTime/Scenes/Main_Room03.unity and press Play. Editor menu: Tools > One More Time > Update Room 3 Upper Section.

PASS: actual Unity compilation and saved scene generation. Recompile polling briefly lost transport during domain reload; scene eval and PlayMode tests subsequently executed successfully. PASS: VerticalRoomTests 6/6, including independent 3/15-second timers, shutter keyboard continuity, lower shuttle, checkpoints/death and raised lift transfer. Evidence: Artifacts/Logs/room3-upper-tests.json. PASS: serialized comparison confirms 12 existing objects and all their own components unchanged, including original cliff collider (room3-upper-preservation.json). PASS: git diff --check. NOT RUN: complete new-route traversal, manual playtest or Web rebuild. No runtime gameplay scripts changed in this update; no push/publication.

---

# Main-game room layouts

## Shared sketch legend (user supplied, 2026-11-02)

Each gap between adjacent background dots is **1 Unity unit**, per the user's latest clarification. Apply this scale to new layouts; retain explicitly requested Room 1 dimension overrides. These labels apply to future layout drawings. The sketch is a spatial reference; the user's accompanying description defines the behavior.

- **0 — light gray outline:** room boundary and camera framing. The outline is not a decorative white wall.
- **1 — entrance:** scripted goat walk into the camera, then show the left/right movement prompt.
- **2 — jump prompt:** show the jump key near the marked approach.
- **3 — goat statue:** proximity checkpoint; its eye changes from dark to green when saved. Death returns the player to the latest statue.
- **4 — red pit:** lethal fall zone; respawn the player.
- **5 — green grass platforms:** thin one-way platforms (jump through from below, Down+Space to drop). Dirt pillars extend downward visually, behind the player; pillars have no collision.
- **6 — exit arrow:** lock input and animate walking offscreen while fading out. The next room uses a matching fade-in and walk-in.

## Room 1

Reference: user's `黑板 4.png` (2026-11-02). Existing goat controls/tuning unchanged. Scene: `Assets/_OneMoreTime/Scenes/Main_Room01.unity`.

Room bounds are now 76.4 × 27 units (x=0..76.4, y=-7..20). The tutorial-style camera follows with interpolation and look-ahead, but shows exactly 20 horizontal units. It holds at the room edges during entry/exit.

Ground starts at y=0 and rises to y=2.8. The checkpoint is (30.8, 2.8). The pit spans x=34.8..39.2: 4.4 units wide, explicitly exempted from the stage's 2× horizontal expansion. The far bank extends to x=64.8. Grass surfaces are y=5.3, 7.3, 9.8: rises of +2.5, +2, +2.5 from the bank. Exit surface is y=12.4, per user confirmation. Grass is one-way: pass through below and land from above. Hold Down Arrow and press Space while standing on grass to drop through that platform; lower platforms still catch the player. Pillars remain visual only; the pit death zone remains invisible and now triggers at y=-8.5, below the room/camera edge.

The main room gets a separate PlayerTuning asset, preserving tutorial tuning. For the existing 0.02-second motor and gravity=32, jumpSpeed=14.178182 produces a 3-unit held-jump apex. Calculated same-height horizontal travel is 5.28 units at full speed=6, or 5.04 from rest (acceleration=60); higher landing surfaces shorten this. Collider: width 0.7, height 1. Sprite art dimensions vary by animation. These latest distances are fixed-step code calculations, not an updated Unity playtest.

The revised scene has now been generated and saved through Unity; the earlier Editor-access block is resolved. Camera width is 20 units (superseding the previous 10-unit request).
Start Journey → existing photography competition intro → Room 1 walk-in → A/D or arrow-key prompt → Space prompt → statue/pit/climb → automatic walk-out/fade. Esc pauses; R resets to the active checkpoint. The new room introduces movement only; existing E/Q/F practice remains in the independent Tutorials section.

Room 1 now transitions to Main_Room02. MainRoom initializes each room with a fade-in/walk-in. Room 2 now transitions to Room 3. The lift at the top of Room 3 leads upward into an empty arrival landing reserved for the next layout.

`Tools > One More Time > Build Main Room 1` regenerates only `Room 1 Managed` in this scene, preserves other root objects, and backs up the previous scene. Do not regenerate after manual edits within that root without transferring those edits to the builder. The builder adds the main menu, tutorials, ending and Room 1 to Build Settings, retaining other scene entries. The custom Web builder also includes Room 1.

## Room 2 — Shutter

Reference: `黑板 7.png`. Approximate hand-drawn edges snapped to the 1-unit dot grid: bounds x=0..44, y=-2..9; approach y=0 to x=19; checkpoint plateau x=19..27, y=5; pit x=27..36 (9 wide), invisible death threshold y=-4.5; exit bank x=36 onward, y=5. Gray goat checkpoint at (24.3,5); purple camera pickup at (12.4,0.55). Camera remains 20 units wide.

Purple item unlocks E. E captures the player's world position, pose and facing; the frozen goat is replaced immediately and a 0.32-second dark shutter blink and click pauses room simulation without resetting the player or room. Its support marker is invisible; the goat itself remains visible and provides one-way support. Every E press replaces the previous copy in place; there is no separate clear step and no white flash. R/death preserve the copy and unlock. Legacy tutorial E/Q rules remain unchanged.

The first 5-unit rise is verified using the copy. **The exact 9-unit pit with a ceiling 4 units above the ledges has not been validated as crossable** with the current 3-unit jump and single copy; dimensions are preserved pending user preference. Exit tests place the goat on the far bank explicitly and do not count as a full room solution.

Open `Assets/_OneMoreTime/Scenes/Main_Room02.unity` and press Play for direct testing, or enter via Room 1. Regenerate with `Tools > One More Time > Build Main Room 2 - Shutter`. Generation manages only `Room 2 Managed`, backs up scenes and connects Room 1.

## Shared room art (2026-11-03)

Main Rooms 1–3 and the lift arrival reuse the tutorial's Sunny Land `back.png` mountains and `middle.png` forest, with camera-following horizontal and vertical parallax. Room 1 grass platforms now use tiled grass/roots with a separate uneven blade fringe, preserving the thin one-way colliders and decorative dirt pillars.

New original pixel sprites live in `Assets/_OneMoreTime/Art/MainRooms/`: `OldLiftCabin.png` (96×112), `OldLiftGate.png` (64×80), `WeatheredWoodPlatform.png` (64×16), `PressurePlate.png` (48×8), `GrassBlades.png` (32×10). Drawn by the project's deterministic C# art builder; no external generated artwork. Point-filtered, 32 pixels/unit, transparent PNG. The cabin has riveted iron columns, weathered timber, cross-bracing and an overhead pulley; its gate is a separate sliding sprite. Reusable prefabs: `OldMountainLift.prefab`, `WoodenPlatform.prefab`. Assign a MainRoom and PressurePlate when using the lift in a new room.

## Room 3 — user layout (IMG_8884.jpg, replaces the draft)

One adjacent dot gap = 1 Unity unit. Hand-drawn edges are rounded to the grid or half-unit positions. Bounds x=0..32, y=-5..38. Camera width remains 20. The intro/tutorials and Room 1/2 layouts are preserved; Room 2 still transitions into Room 3.

| Feature | X range / point | Surface Y |
|---|---|---|
| Entry ground | -3..19 | 0 |
| Lower grass / Plate A | 11..19 / 12.5 | 2.5 |
| Left grass | 6..9 | 6 |
| Vertical shuttle | 15..17 | 5 ↔ 10 |
| First statue grass / statue | 10..12 / 11.5 | 11 |
| Wood step | 13..15 | 14 |
| Right wood step | 21..24 | 16 |
| Short right step | 23..24 | 18.5 |
| Upper statue grass / statue | 17..21 / 17.6 | 22 |
| Plate B | 19.5 | 22 |
| Three-second platform | 19..21 | 24.5 |
| Upper right step | 22..24 | 29 |
| Right cliff / lift gate plate / lift | 24..36 / 25.5 / 31 | 33 |
| Pit | 19..24 | Death below -6.5 |

Yellow-green marks are pressure plates. Red diagram lines specify control links only; the visible red cables have been removed. Pale yellow line is the shuttle track. Unlinked yellow platforms use ordinary wooden-platform sprites. Yellow box is the upward lift.

Plate A runs the shuttle at 2 units/second between both ends of the 5-unit vertical track. Releasing the plate deactivates its collision and stops it in place; repressing resumes its direction. A grounded frozen goat can hold a plate. The motor carries the live goat exactly once and gives no extra takeoff velocity. Moving platforms remain one-way and allow Down+Space.

Plate B's marked platform is active while the plate is held and its displayed countdown stays at 3.0. After all valid sources leave, the countdown runs for 3 seconds; repressing refills it. At zero the collision switches off and the art returns to a translucent inactive state. The timer number is half its original size, centered on the wooden platform itself, and hidden entirely at zero. Shutter/pause stop mechanism motion and countdowns. R/death reset mechanisms to their initial states, then reevaluate any preserved snapshot.

Grass surfaces have textured blades, thin one-way collision, and dark dirt pillars extending downward with **no pillar collision**. Both statues are grayscale goat sprites; eyes turn green on first proximity activation. Respawn uses the most recently newly activated statue, without reverting to an older statue merely by passing it again. Death areas have no visible renderer. Room 3's pit trigger is 1.5 units below its lower outline; Room 1 and 2 thresholds also move offscreen to -8.5 and -4.5 respectively so the goat disappears before respawning.

The final nearby plate opens the lift gate. A frozen goat holds it while the player boards. The gate closes, the cabin/player rise, then fade into Main_LiftLanding. That destination remains an empty safe arrival area awaiting another layout.

Scene: `Assets/_OneMoreTime/Scenes/Main_Room03.unity`. Direct Play starts with Shutter unlocked. All gameplay tip boxes are disabled in this room. Shutter preserves held movement and jump-hold input; direction changes and releases during the pause are applied when play resumes. The lift has moved two units right to x=31. To regenerate only this layout plus existing-room presentation/death thresholds, call `MainRoomContentBuilder.BuildRoomThreeLayout()` through Unity, or use the existing Build Main Room Art and Room 3 menu to rebuild the full managed content. Managed roots are backed up before regeneration.

Validation covers mechanics with isolated position fixtures, **not a full traversal of the new drawing**. No geometry was silently shortened to make gaps easier. The draft room's old full-route test is superseded.
