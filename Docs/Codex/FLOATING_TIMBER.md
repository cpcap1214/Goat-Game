# Floating timber assets

Build with `Tools > One More Time > Build Floating Timber` or `OneMoreTime.EditorTools.FloatingTimberBuilder.Build()` after importing `Assets/_OneMoreTime/Art/WorldOne/Expansion/FloatingTimber.png`.

`Assets/_OneMoreTime/Prefabs/WorldOne/Expansion/` contains `FloatingLog` (3.5 units), `MossyFloatingLog` (3.5 units), `BoundLogRaft` (4 units), and `FloatingTimberPlank` (3 units), plus corresponding `Source` prefabs. Each platform has a one-way deck aligned with its visible top, supports down+jump, and uses the existing floating-object carrier. The corresponding prefabs in `Expansion/Background/` are darker visual-only sprites with sorting order -20; they have no collision, physics, or floating-object scripts.

## Room placement

The individual platform prefabs do not run their own simulation clock. Use a source prefab with a `RewindPlaytest` room: add its `CrateSpawner` to the room's `participants` array **before entering Play mode**. Its `affectedByRewind` variable controls rewind participation. The existing crate system also handles logs and rafts, including recorded lifetimes, reverse replay, held replay, and dashed traces, without applying a tint.

Each source defaults to a leftward current of 2 units/second, one spawn every 10 seconds, and six pooled platforms. Its default world bounds are x=-10 to x=10, water surface y=0, spawn=(9.5,0.28), and disposal boundary x=-10. These are templates for placement, not additional content inserted into existing rooms.

`WaterCurrent2D.left`, `right`, `surfaceY`, and `CrateDisposalTrigger.boundaryX` are **world coordinates**. Moving a source's root transform does not update these numeric bounds. Set them to the actual stream, place its spawn transform above that stream, position its disposal trigger beyond the left bank, and keep the trigger's owner set to that source. All pool items must reference the same current. Disposal currently supports leftward streams, matching the existing room setup; backward *replay* still works normally.

For a room builder, prefer `FloatingTimberBuilder.InstallSource(parent, variant, current, spawnWorldPosition, disposalWorldX, replay)`. This assigns the current, all pool references, spawn and disposal positions, and the replay participant automatically. The room's existing `WaterCurrent2D` supplies its world bounds. Avoid moving the source root afterwards without updating the world-coordinate settings. Call the helper once per intended source while constructing the room; it does not replace an existing source.

## Validation

`OneMoreTime.Tests.FloatingTimberTests` covers prefab configuration, raft buoyancy/player carrying/drop-through in Room 4, and log forward/reverse/held replay without tint. Test results are recorded by the main task in `TEST_REPORT.md`; this file does not claim an unrun check passed.
