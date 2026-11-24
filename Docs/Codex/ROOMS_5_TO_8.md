# Chapter 1 — Current Rooms 4–8 guide

Updated: 2026-11-24 (room order revision)

Current campaign scope: **Shutter in Room 4; Shutter and basic replay from the Room 5 pickup onward**. Room 4 never grants recording/replay, including direct Debug entry. The official Rooms 4–8 do not grant the past filter or advanced replay. Orange bridge fragments are darker, noncolliding scenery. The dedicated ability debug scenes and EchoFoothills retain their separate test mechanics.

The current order is **Room 3 → Room 4 Riverworks → Room 5 Recording acquisition → Room 6 Broken crossing → Room 7 Twin Falls → Room 8 Grand Waterfall**. Riverworks follows 黑板 12, the broken crossing follows IMG_8932, Room 7 follows IMG_8935, and Room 8 follows its diagonal-waterfall drawing. The original Room 6 became Room 4; original Room 4 became Room 5; original Room 5 became Room 6. Their scene assets were moved to the matching filenames with GUIDs preserved. Rooms 1–3, the tutorial, and the intro retain their layouts.

## Open and edit

- Scenes: `Assets/_OneMoreTime/Scenes/Main_Room04.unity` through `Main_Room08.unity`.
- Open a scene in Unity and press Play, or use **Main menu → Debug Mode → entries 9–13**: 09 Room 4 / 河道升降, 10 Room 5 / 錄影與水路, 11 Room 6 / 斷橋渡河, then the unchanged Room 7 and 8 entries.
- Generator: **Tools → One More Time → Build Chapter 1 Rooms 4–8**. Prefer `ChapterOneExpansionBuilder.BuildRoom(4)`, `BuildRoom(5)`, or `BuildRoom(6)` when only one room needs regeneration.
- Shared assembly and art helpers: `Assets/_OneMoreTime/Editor/ChapterOneExpansionBuilder.cs`.
- Room 4 coordinates: `Assets/_OneMoreTime/Editor/ChapterOneRooms56.cs` (`Layout4`); Room 6 coordinates: `Assets/_OneMoreTime/Editor/RoomFiveLayout.cs` (`Layout6`). These established source filenames are retained.
- Room 5 keeps its dedicated `RoomFourBuilder` / `RoomFourUpdate` / `Room4AbilityTutorial` classes. `BuildRoom(5)` delegates to `RoomFourBuilder.Build()` and writes `Main_Room05.unity`. Menu commands are **Build Main Room 5** and **Update Existing Room 5**; see [the current acquisition guide](ROOM_FOUR.md).
- For a single-room change, call `BuildRoom(number)` in the Editor; the all-room menu command regenerates every room from 4 through 8.
- Room 7 coordinates: `Assets/_OneMoreTime/Editor/RoomSevenLayout.cs`. Room 8 coordinates: `Assets/_OneMoreTime/Editor/RoomEightLayout.cs`; finale art: `RoomEightScenery.cs`.
- Room orchestration: `Assets/_OneMoreTime/Runtime/ChapterOneRoom.cs`; rewind continues using the existing `RewindPlaytest` and floating-object participants.

Rooms 4 and 6–8 use **Room N Expansion Managed**; the acquisition uses **Room 5 Managed**. Expansion rooms have separate **Route and mechanisms** and **Forest and waterfall scenery** children. The generator replaces that managed root on rerun. Keep permanent layout edits in the layout methods, or keep additional manual objects outside the managed root. It refuses to rebuild a loaded dirty target scene. Generated decoration has no colliders.

## Past background artwork

The legacy chapter test fixture supports background filtering, synchronized with the 0.36-second lens transition. The official campaign layouts keep this ability unavailable. Until the new art is chosen, past mode uses a warm, less saturated treatment of the existing forest. The original present image stays intact. Pausing freezes the blend; retry and world rewind follow the restored era.

Select **Forest and waterfall scenery → Forest and Grand Waterfall Background** and inspect **World One Backdrop → Past Filter Background**. `Past Background` accepts the eventual background texture. Leave it empty for the temporary treatment, adjustable with `Past Tint` and `Past Saturation`. Assigned artwork replaces that treatment without an extra recolor. Use the same 3:1 proportions as the present artwork: both share the existing repeat dimensions, parallax, edge blending and 384-cell vertical grid. To use one past image throughout the chapter, assign it on `Assets/_OneMoreTime/Prefabs/WorldOne/ForestWaterfallBackground.prefab`; scene overrides can provide room-specific artwork. The art gallery and acquisition Room 5 remain in present mode because they have no era controller; Room 4 keeps its filter disabled.

The source is `Runtime/WorldOne/WorldOneBackdrop.cs` and `Shaders/LoopingForest.shader`. Existing rooms automatically locate their enclosing `ChapterOneRoom`; future generated rooms explicitly link it. No room regeneration is needed. EchoFoothills retains its existing separate present/past skies.

## Room 4 — Riverworks ascent (黑板 12)

One dot interval equals one unit. The horizontal room is 97 units wide; the river is y=0, x=8–92, with a visible downward spill at its right end. Bounds are x=0–97, y=−6–22.5, with the same 20-unit-wide camera and no physical ceiling. The drawing has no checkpoint statue, so retry uses the entrance bank. No extra stepping ledges were added.

| Feature | Coordinates / behavior |
|---|---|
| Arrival / entrance bank | Lift docks at (2,1), traveling upward 7 units at 3 units/sec from Room 3. One-way wooden landing x=0.5–4 at y=1 leaves the shaft open; solid bank x=4–8 |
| Log-release plate | (6.5,1), one log per fresh press; **4-second cooldown**, hidden during gameplay |
| Log source | (8.5,0.28), normal floating-log sprite, **2 units/sec** rightward; logs keep moving when the plate is released |
| River blocks | x=18–21 and 24–26, top y=3, bottom y=1 |
| Middle stepped island | x=30–36, top y=3, with x=32–35 tower up to y=7 |
| Bridges | solid red x=36–39, 48–51 and 65–68 at y=3; orange x=39–48 is dark, noncolliding scenery |
| Eastern island | x=51–65 at y=3, tower x=62–65 up to y=13 |
| Grass shelves | x=52–57 at y=7; x=58.5–62 at y=5; one-way tops with background dirt below water |
| Return-platform plate | (60.4,3); live goat or Shutter image can hold it; recording is unavailable in this room |
| Swift platform | 2.4 units wide, center (54,10) → (60,10) at **12 units/sec**; parks while held and returns immediately on release; always solid |
| Final timbers | x=87–90 at y=3; x=84–87 at y=6; x=88–89 at y=8.5 |
| Horizontal exit bank | Floor y=12; body x=92–97, with an inward grass bevel x=91–92. Exit trigger x=95–97; walk right and fade into Room 5 |

Bridge scaffolding is decorative. The three wooden approach steps remain unchanged; the former high exit lift/landing is replaced by the beveled horizontal bank at y=12. Shutter remains available, but `ChapterOneRoom.ReplayAvailable` is false in Room 4 in both normal and Debug entry, so holding record cannot start recording and the pause camera has no tape module. The shared replay engine still advances the motor, logs and platforms; their reusable rewind support does not grant the player the ability. No past filter or advanced replay is granted.

Implementation options are `CrateSpawner.activationPlate`, `RoomPlatform.returnWhenReleased`, and `RoomLift.automaticGate`; all default off/null so existing rooms keep their behavior. Log sources without an activation plate retain interval spawning. Room 4 uses `CrateSpawner.plateCooldown = 4`: one activation releases one log, early presses are ignored and holding never repeats, even after the cooldown expires. The timer uses the simulation clock, pauses with gameplay, and has no player-facing display. Other sources default to zero plate cooldown. Logs visibly fall over the right spill before returning to the pool at x=95.

Debug menu: **09 Room 4 / 河道升降**. Scene: `Assets/_OneMoreTime/Scenes/Main_Room04.unity`. Builder: `ChapterOneExpansionBuilder.BuildRoom(4)`; it replaces the Room 4 managed root. Historical pre-reorder backup: `Artifacts/Room 6Layout/Main_Room06.before-layout.unity`.

## Room 5 — Recording acquisition and waterway

Scene: `Assets/_OneMoreTime/Scenes/Main_Room05.unity`; Debug entry **10 Room 5 / 錄影與水路**. The goat now enters horizontally from (0.5,4) and walks to (2,4) with a fade-in. `MainRoom.lift` is null in this room; no arrival lift blocks the left route. The existing acquisition bridge, tape pickup, slow fall, white record prompt, imaginary rewind to the bank, checkpoint and water/crate puzzle remain intact. Exit continues horizontally into Room 6.

Before the pickup, recording is locked. Collecting it unlocks basic replay; the real 0.25-second hold starts the scripted acquisition rewind. Returning to the main menu rearms the pickup and cutscene; ordinary death/retry retains the current journey's progress. The stable legacy-named `Room4AbilityTutorial` keeps its lift fallback for other authored uses but points `SceneName` at `Main_Room05` and `nextRoomScene` at `Main_Room06`.

Rebuild through `ChapterOneExpansionBuilder.BuildRoom(5)` or **Build Main Room 5**; patch through **Update Existing Room 5**. These call the existing `RoomFourBuilder` / `RoomFourUpdate`, which manage **Room 5 Managed**. [ROOM_FOUR.md](ROOM_FOUR.md) contains the current entry instructions above its preserved historical implementation records.

## Room 6 — Broken crossing (IMG_8932)

One dot interval is one world unit. The 90-unit-wide room uses its lowest river surface as y=0; the camera remains 20 units wide and the upper bound is not a ceiling. Start at (2.5,8). The exit bank remains x=82–90 at y=9; an automatic upward lift at (88,9) carries the goat 8 units at 4 units/sec and fades into Room 7.

| Feature | Authored placement |
|---|---|
| Stepped U-shaped river | x=4–26 at y=4; 26–29 at y=3; 29–33 at y=1; 33–74 at y=0; 74–76 at y=3; 76–82 at y=7 |
| West bridge, y=8 | Solid x=5–7, 11–13, 19–20; background x=7–11, 13–19 |
| Plate island | Grass x=20–25 at y=8; pressure plate x=24.3 |
| Moving platform | Two-unit timber deck, center x=29→36 at y=8, back and forth at 2 units/sec; active only while its plate is held, no release timer |
| Central checkpoint | Grass x=43–46 at y=5; statue (44.5,5) |
| Lower bridge, y=5 | Solid x=41–43, 46–49, 60–67; background x=49–60, 67–74 |
| Upper bridge, y=9 | Solid x=66–74; background x=57–66, 74–82 |
| High background remnant | x=62–79 at y=13 |
| Exit lift | Dock (88,9), automatic gate, upward travel 8 units at 4 units/sec to Room 7 |
| Crate source and drain | Spawn at x=33.5; rightward speed 2.5 units/sec; interval 10 seconds; whirlpool at x=60 |

Red spans use existing bridge tiles with one-way top collision. Orange spans are dark background sprites without colliders; they do not become solid. The past filter and advanced replay remain unavailable in the main game. Use the existing shutter/basic record-and-replay mechanics; no extra stepping platforms were inserted to fill the drawn gaps.

Both grassy tops use the existing grass/dirt textures with visual soil columns extending to y=−3, beneath the water. The timber frames form dark scaffold walls below bridge decks. Upper decks share the lower support structure to avoid overlapping walls. All scaffold pieces and dirt below the thin grass collision surface are decorative. Purple diagram connectors are not rendered as gameplay lines; a slim track indicates the shuttle's horizontal travel.

The crate source keeps its authored downstream spawn and drain in both normal and debug entry. At the whirlpool, each crate visibly moves down before returning to its pool. Position, velocity, sinking phase, activation and spawner timing are captured by the existing world-rewind/replay system. Other rooms keep their original disposal behavior. Crates keep their normal colors. The source is marked `affectedByRewind`; the drain's `sinkBeforeDisposal`, `sinkDepth` and `sinkSpeed` fields expose its behavior.

Historical validation (2026-11-23, before renumbering and the new exit lift): seven focused tests for the then-named Room 5 passed, including real shutter → shuttle boarding/carry → checkpoint movement, plate release, crate cadence/sink, decorative collision and camera bounds. The shared chapter regressions total 77 distinct passing tests after updating old multi-checkpoint/filter fixtures. Full-room and gameplay-zoom renders were reviewed. A complete human clear of every long gap and standalone build remain NOT RUN.

Historical backup from that layout update: the previous Room 5 scene was backed up locally in `Artifacts/Room5Layout/Main_Room05.before-layout.unity`. That earlier update retained its then-current GUID/menu links and regenerated only the then-named Room 5. The current scene is `Main_Room06.unity`, Debug entry 11; the later reorder preserves its GUID and adds the lift described above. Those older results do not validate the reordered entrances/exits.

## Room 7 — Twin Falls Tower (official IMG_8935)

Replaces the former horizontal timber river. Bounds x=0–20, y=-3.5–65. The camera stays centered at x=10 with a 20-unit normal view; it follows vertically but stops with its top at y=65, including during the trapdoor reveal and upward exit, so the ends of the walls stay outside view. The upper boundary does not collide with the goat. Waterfalls run down both sides. Entry is the lift arriving upward from Room 6; exit rises through the opened top shaft into Room 8. No past filter or advanced replay is granted here.

| Section | Layout and behavior |
|---|---|
| Arrival | Lift dock (9.5,0), wooden landing, grass shelves at y=2.6 with background dirt |
| Lower loops | Three rectangular routes, each taking **6 seconds for a complete lap** |
| Paired plates | West/east decks at y=15.6. Pressing either plate first freezes gameplay and pans to the center deck, then returns. Both plates must overlap in time to unlock movement from y=14.5 to 20.5; `0/2` and `1/2` white, latched `2/2` green. Once unlocked it stays unlocked through release, retry, and rewind |
| First checkpoint | Bridge across x=0–20 at y=22.6, goat statue at x=9.5 |
| Middle shuttles | Six horizontal routes at y=26.6,28.6,30.5,32.4,35.7,39.6; **4 seconds per complete outward-and-return cycle** |
| Second checkpoint | Bridge x=0–10 at y=42.6, statue x=7.6; small water pool to the right |
| Dispatcher | Plate (3.3,42.6) sends one deck per press; **4-second cooldown**. Decks travel right, upward, then left around the upper wall route at **3 units/sec (75% of the former speed)**. Four reusable deck instances; holding the plate does not repeatedly send decks |
| Upper climb | Left slot spans y=45.1–45.7, centered on the dispatched deck/track. East wall extends down to y=46, leaving only 0.5 unit above the deck, so the 1-unit-tall goat must leave it and climb to catch it later. Two small upper-left decks are at y=56.1; other loop/shuttle timings are unchanged |
| Trapdoor | Plate (5.8,62.8) opens two leaves across the central exit shaft at y=62.7; the stepping deck beyond is y=63.7. Opening freezes gameplay, pans/zooms vertically to the door, then returns. Door stays open and is excluded from rewind; its pressure plate also stays live while recorded objects wait |
| Exit | Upper shaft x=8–11, y>=64.7; airborne upward transition after the trapdoor opens |

Routes use decorative pale travel guides; no control-link lines or unmarked helper ledges. Grass shelf dirt and waterfall art do not add collision. Ordinary wooden platforms can be jumped through from below and dropped through with Down+Jump; the closed trapdoor blocks both directions.

`RoomPlatform.motion`, `route`, `cycleSeconds`, `requiredPlates`, and `latchWhenAllPressed` expose the movement rules. `PlatformDispatcher` owns only launch edges and cooldown; the existing replay clock owns deck movement/carry once per fixed step. Moving decks and dispatcher state rewind and stay frozen until replay starts. The activation latch and trapdoor opening are persistent puzzle progress. The former river replay regression fixtures now live in the archived `Tests/Fixtures/Main_Room08.unity`, excluded from build settings.

## Room 8 — The Grand Waterfall (official drawing)

Bounds x=0–126, y=-1–57. Arrival jumps up from below beside the bank, then arcs over its edge onto the landing without intersecting the solid wall. The camera follows within the room with a 20-unit view; the upper boundary remains open to jumps. Dot spacing follows one world unit. The river climbs in steps from y=0.6 to y=40.3, ending at the cave bank at y=47.3.

| Section | Layout and behavior |
|---|---|
| Lower river | Crates spawn near x=19.2 and flow left at **2 units/sec**, one every **6 seconds**, into the marked drain. The opening red arrows denote whirlpool flow. The separate red span above the next water step is a solid bridge at **x=19.7–24.7, y=2.9**, with decorative beams underneath. The following bridge supports the statue at (31.1,6.9) |
| Loop decks | Two rectangular paths around x=35–40 and x=45–50; **6 seconds per complete cycle** |
| Grass route | Four one-way grassy shelves with background dirt at the drawn heights |
| Upper crate stream | Second source near x=82.8, also leftward **2 units/sec / 6 seconds**, draining near x=70.5. Its flow arrows likewise add no bridge geometry |
| Remote pressure plate | Remote plate at (68.7,29.7) reveals the carrier with a paused camera pan. It and a second plate on the eastern step at (95.25,25.7) must both be held simultaneously to start the trip |
| Traveling checkpoint | Deck starts at (84.75,22.7), carries its statue by (+11,+7) over **10 seconds**, then stays at its destination. Its paired-plate counter is white `0/2` or `1/2`, green `2/2`. Releasing the plate does not reverse it |
| Final ascent | Drawn wooden steps and red/orange bridge fragments, with decorative timber supports extending downward |
| Grand Waterfall | Layered water falls from y=62, beyond the camera top, with an overhead river source. The dark cave has a clear continuous floor rim at y=47.3 and an uncluttered walking corridor |
| Cave exit | Cavern floor y=47.3, exit x>=123. Fades into the existing Chapter 2 threshold/completion screen; Chapter 2 gameplay is still pending |

The traveling statue remains a usable checkpoint while moving. Falling, drowning, and the pause reset return its platform and checkpoint to the original position before revival. Death while recording keeps the visible rewind presentation, then restores that safe home. Ordinary recording completion restores the recorded platform position instead, and affected objects stay frozen until replay is triggered. The saved recording survives death.

All bridge beams, background dirt, cave decoration, mist, and foreground waterfall art are noncolliding. No control-link lines or extra helper platforms were added. Water hazards remain invisible. Crates retain their normal colors and participate in recording/replay. Sinking clips progressively below the waterline and restores correctly during rewind. A fresh shutter copy holds plates immediately, but cannot support the goat until its full body has left the copy; replacing a copy clears stale carried support.

The former Room 8 prototype is preserved only as `Assets/_OneMoreTime/Tests/Fixtures/Main_Room08.unity`, created by Unity's AssetDatabase. It is outside Build Settings and keeps the older filter, crumble, and river traversal regression coverage without adding their geometry to this layout.

## Shared controls and tuning

- Movement follows the saved WASD/arrow-key preset or custom bindings. Jump defaults to Space; drop through one-way platforms with the bound Down + Jump actions.
- Past filter and advanced playback remain in dedicated debug/legacy test content, not the official Rooms 4–8.
- The bound Shutter action places an image; a successful capture replaces a saved replay. Recording and active world rewind must finish first.
- From the Room 5 pickup onward, hold the bound recording key for 0.25 seconds to record; tap it to finish or play/restart a saved recording. Room 4 has Shutter only. Use the saved key bindings shown in pause settings; older E/Q references are legacy examples.
- In the separate upgraded replay debug scene, tap the bound replay-direction key to reverse playback; hold it to pause the cursor (Left Shift in the arrow-key preset).
- Existing goat tuning remains speed 6, fixed-step jump height 3, body width 0.7 and height 1 world unit. Environmental props and decorative waterfalls do not become invisible collision obstacles.

## Route review and validation

A calculation using the existing fixed simulation step (0.02 seconds), initial jump speed 14.178182, and gravity 32 gives a nominal 3.0-unit jump peak. At full horizontal speed, the descending part of a jump reaches a 2.0/2.4/2.5-unit-higher surface after approximately 4.08/3.72/3.60 horizontal units respectively. These are planning estimates, not controller traversal tests; they omit acceleration, human timing, obstruction, and collider overlap details.

Official layouts preserve ability-assisted jumps from the drawings; these are not all ordinary single-jump gaps. The older bank-to-log and moving-platform traversal tests run against the archived prototype and do not establish full traversal of this replacement.

Historical Room 8 verification (before this room-order revision): Unity compilation and generation passed; **129/129 ChapterOneRegression PlayMode tests passed**, including seven real Room 8 integration tests and six moving-checkpoint mechanism tests. Coverage includes crate timing, complete loop cycles, camera bounds/reveal, deck carry/parking, death/reset before revival, ordinary rewind and frozen waiting, scenery collision, and the finale. Mechanism tests use explicit setup placement; a complete human traversal and first-time-player timing remain **NOT RUN**. Evidence: `Artifacts/Logs/room8-regressions.json`, `room8-compilation-final.json`, `room8-generation.json`, and `chapter1-room8-overview.png` / `chapter1-room8-view1.png` through `view7.png`.

Room-order validation is recorded separately in `STATUS.md` and `TEST_REPORT.md`; the historical test counts above are not new test results for this reorder.


## 2026-11-24 Room 5–6 bridge support correction

Room 6’s scaffold groups now use continuous vertical posts and horizontal crosspieces, with straight wood strips sliced from the existing TimberFrame atlas. Complete frame props and their repeated stone bases are no longer tiled down the wall. Room 5’s pickup bridge uses the same purely decorative supports. All terrain/platform collision is unchanged. `BridgeScaffoldBuilder.Apply()` updates only those two saved rooms, and their regular generators use the helper as well. Room 6 retains the single authored whirlpool at x60 and its existing crate sinking behavior. Room 5 removes its extra decorative swirls and adopts the same sinking/clipping system at its upper-channel drain.
