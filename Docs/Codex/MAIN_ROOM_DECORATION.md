# Chapter 1 main-room decoration

## 2026-11-23 cleanup prepared — NOT APPLIED

The grounded-decoration cleanup tool is implemented and its read-only Unity audit has completed. **No cleanup changes have been saved to any room.** Automatic approval review rejected the proposed all-eight-room save as broader than the current-level request; the requested room scope is awaiting the user's answer. Existing saved scenery remains in place.

The proposed revision replaces scattered large props with small groups that belong to their surroundings: mushrooms beside fallen wood, ferns and reeds on riverbanks, and sparse shrubs on high ledges. Bridge decks, checkpoints and mechanism terraces receive clear space. Examples removed by the proposal include Room 5's unsupported timber frame and Room 8's large frame/bracing around a checkpoint and pressure plate.

### Audited scope

These counts come from the real Editor's read-only `ChapterDecorationCleanup.Audit(number)`, recorded in `Artifacts/Logs/chapter-decoration-audit.json`. They describe a proposed replacement, not an applied result. “Existing” counts only loose forest-expansion props eligible for replacement; water artwork and the nested cavern are excluded.

| Room | Existing loose props | Proposed locations | Locations passing geometry and clearance checks | Intended composition |
|---|---:|---:|---:|---|
| `Main_Room01` | 29 | 8 | 8 | Trail-side wood and mushrooms, two small groves, a sparse upper trail |
| `Main_Room02` | 22 | 6 | 4 | A quiet forest approach, mossy stone and fern pair, one far-bank tree |
| `Main_Room03` | 28 | 7 | 7 | Forest-floor group and small plants on selected static ledges |
| `Main_Room04` | 25 | 8 | 8 | Lift-side grove, dry approach and restrained riverbank plants |
| `Main_Room05` | 17 | 5 | 5 | Two west-bank plants and a three-prop east-bank group |
| `Main_Room06` | 18 | 8 | 8 | Damp entrance, moss shelf and sparse rest-ledge/headwater vegetation |
| `Main_Room07` | 18 | 6 | 6 | Small bank-edge plants and one far-bank driftwood group |
| `Main_Room08` | 21 | 6 | 6 | Sparse plants on high banks and exposed ridges |
| **Total** | **178** | **54** | **52** | |

Room 2's proposed stump at `(5.2, 0)` and fern at `(41.7, 5)` overlap reserved clearings. The helper omits both instead of moving them into another unreviewed location. All other proposed placements passed the audit. The audit reported zero existing managed cleanup props across all eight rooms; it did not apply any replacements.

### Tool behavior and preservation

`Assets/_OneMoreTime/Editor/ChapterDecorationCleanup.cs` owns the proposed **Grounded chapter decoration** child. It replaces only direct forest-expansion prefab instances under **Chapter 1 expansion scenery** in Rooms 1–4 or **Forest and waterfall scenery** in Rooms 5–8. Nested cavern art, water artwork, terrain, gameplay components and other custom objects remain outside this replacement. An expansion-prefab instance with extra components is retained.

Every proposed prop uses five samples across its visible footprint to check static solid support just below the baseline and empty space just above it. Moving platforms, floating objects, breakable bridges, doors, lifts, echoes and players cannot count as supporting terrain. The helper reserves clearings around starts, checkpoints, pickups, plates, lifts, exits and the full travel of moving platforms, including disabled plate-controlled platforms. Oversized silhouettes are rejected. New props contain only transforms and sprite renderers and draw behind the player.

The write path stages and validates new props before replacing old ones. It compares all non-decoration serialized components, local transforms, active states and layers before and after the change. Final replacement uses a dedicated Unity Undo group so a failure after deletion can restore the original objects and references. These preservation and rollback checks are implemented; the read-only audit does not exercise their mutation paths.

The entry points are:

- `Audit(number)`: inspect one room without creating/deleting objects, marking scenes dirty or saving. It reports candidate names, proposed placements and rejected locations.
- `Apply(number)`: after scope approval, replace and save one clean room. It refuses loaded scenes with unsaved edits.
- `ApplyToRoom(room, number)`: generation hook that validates and replaces decoration in memory; the caller handles saving.
- `ValidateRoom(room)`: validate an already-applied managed decoration root.

`MainRoomDecorationBuilder.Apply` and `ChapterOneExpansionBuilder.BuildRoom` now call the cleanup hook before saving future generated decoration. Running those write paths would apply the prepared layout; they have not been run for this cleanup. The direct all-room menu is **Tools → One More Time → Tidy Chapter 1 Decoration** and is also pending scope approval.

### Verification status

- **PASS:** real Unity read-only audit of all eight saved scenes; 52 supported placements out of 54 candidates; 178 existing loose forest props identified.
- **PASS:** source whitespace check (`git diff --check`).
- **NOT APPLIED:** scene replacements and saves; scope approval remains pending after the automatic review rejection.
- **NOT RUN:** applied-scene preservation/idempotence checks, Undo rollback exercise, final cleanup renders and human visual/playthrough review.

## Historical initial decoration pass — 2026-11-19

The following records the earlier saved Rooms 1–4 decoration pass. Its counts and review claims describe that initial pass, not the prepared cleanup above. Later Rooms 5–8 were added separately; see [ROOMS_5_TO_8.md](ROOMS_5_TO_8.md).

The initial pass added forest-expansion scenery without redesigning the routes. The intro, tutorial levels and existing backgrounds were retained.

### Initial scenes and themes

| Scene | Decoration | Groups |
|---|---|---:|
| `Assets/_OneMoreTime/Scenes/Main_Room01.unity` | Sheltered forest trail, ferns, mushrooms, mossy rocks, a clear checkpoint area, and sparse vegetation along the upper path | 29 |
| `Assets/_OneMoreTime/Scenes/Main_Room02.unity` | Moss-covered stone ruins around the Shutter discovery, with open space at the pickup and checkpoint | 22 |
| `Assets/_OneMoreTime/Scenes/Main_Room03.unity` | Roots, vegetation, timber details inside the cliff face, and a windswept lift landing | 28 |
| `Assets/_OneMoreTime/Scenes/Main_Room04.unity` | Riverbank plants, abandoned waterworks, wall roots, surface eddies, impact foam, and subtle foreground spray | 33 |

At that stage the giant waterfall was reserved for a later climax. The initial decoration pass did not add rideable logs or other stepping surfaces.

### Initial builder and ownership

The original implementation was split between:

- `Assets/_OneMoreTime/Editor/MainRoomDecorationBuilder.cs`: scene handling, prefab placement, Room 4 composition, preservation checks, and review captures.
- `Assets/_OneMoreTime/Editor/MainRoomDecorationLayouts.cs`: Rooms 1–3 compositions.

`MainRoomDecorationBuilder.Apply(number)` handles one room from 1 to 4; `Build()` handles all four. The menu remains **Tools → One More Time → Decorate Main Rooms With Forest Expansion**, but it now also invokes the prepared cleanup described above. The forest expansion prefabs must exist; see [FOREST_EXPANSION.md](FOREST_EXPANSION.md) for their inventory.

The original builder owned the **Chapter 1 expansion scenery** child. Rebuilding replaces that child; separate manual scenery belongs outside the managed subtrees. The tool refuses loaded rooms with unsaved changes.

### Initial gameplay and visual limits

Added scenery has no collision, platforms, hazards, current simulation, or rewind participation. Existing routes, checkpoints, pressure plates, timers, ability configuration, movement, and crate cadence are preserved. Large background props are darker; opaque scenery stays behind the goat. Foreground water is translucent and placed below important riding surfaces.

Timber face details in Room 3 sit inside the solid cliff silhouette, avoiding false ledges. The decorative beam between Room 4's grass columns was removed because it resembled a new platform over water. Eddies sit on the established water surfaces. No new red connection lines or tip boxes were introduced.

The initial pass retained native prop dimensions and materials. The prepared cleanup intentionally scales its selected props down. Both use the existing expansion materials; blanket material replacement would discard the expansion's pixel-scale and water shaders.

### Initial validation evidence

Before saving, the builder compares the existing objects' serialized components and local transform values against their pre-decoration state. The managed scenery subtree is excluded from that comparison. It also rejects scenery containing components other than transforms, sprite renderers, and the visual-only `WaterFeature` component.

Repeated preservation checks passed for the initial four-room pass. Its saved group counts were **29 / 22 / 28 / 33**; Room 4's total includes eight water groups in addition to 25 forest props. Gameplay-scale captures were inspected with each room loaded in isolation. Those images do not show the proposed cleanup.

Evidence:

- `Artifacts/Logs/main-room-decoration-validation.txt`
- `Artifacts/Logs/main-room-1-decorated-1.png` and `-2.png`
- `Artifacts/Logs/main-room-2-decorated-1.png` and `-2.png`
- `Artifacts/Logs/main-room-3-decorated-1.png` through `-3.png`
- `Artifacts/Logs/main-room-4-decorated-1.png` through `-3.png`

PlayMode test results are recorded separately in [TEST_REPORT.md](TEST_REPORT.md). Preservation checks and render review do not substitute for a complete human playthrough or standalone-build verification.

### Historical follow-up proposals

These were proposals written after the initial pass, not the current chapter specification. Rooms 5–8 subsequently implemented the floating-timber routes and Grand Waterfall. The current correction request reserves the advanced replay and filter abilities for debug mode, superseding the acquisition/era proposals below.

1. **Floating-timber route:** design a new room around recording and riding logs or rafts, using the existing water-current and replay systems.
2. **Ruined waterworks puzzle:** combine pressure plates with past/present terrain so the filter changes the route or mechanism state meaningfully.
3. **Grand Waterfall finale:** build a vertical climax using the giant waterfall, foreground spray, clear rest areas, and a chapter-ending sequence.
4. **Later replay upgrade:** introduce reverse playback through an in-game upgrade pickup and a focused teaching puzzle. The upgraded ability currently has a separate debug playtest.
5. **Save and resume:** persist chapter checkpoints and earned abilities across application restarts; current main-room progression is session-based.
6. **Audio and playtesting:** add distance-based waterfall sound and forest ambience, then test complete routes with new players to tune pacing, checkpoint spacing, and visual readability.

The initial pass ended with Room 4's placeholder completion screen. That historical limit was superseded when Rooms 5–8 were connected; it is not the current scene flow.
