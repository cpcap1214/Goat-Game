# Forest & Grand Waterfall expansion kit

This asset kit supports three further rooms without defining their layouts or changing their progression. The current forest background remains in use. New art was generated with built-in imagegen using Sunny Land terrain/tree and the current water atlas as references. Exact prompts are in [FOREST_EXPANSION_PROMPTS.md](FOREST_EXPANSION_PROMPTS.md).

## Contents and intended use

| Group | Assets | Suggested use |
|---|---|---|
| Forest details | 16: ferns, reeds, flowers, mushrooms, shelf fungus, fallen log, stump, vines, roots, mossy rocks, branch | Dense lower forest and river banks |
| Mountain details | 16: arch, pillar, stone slab, doorway, spire, boulder, cliff, overhang, timber frame, bracing, waterwheel, rope post, windswept tree, alpine shrub, canopy, rocky bank | Higher rocky forest and old waterworks |
| Floating timber | Straight log, mossy log, three-log raft, timber plank; each with stream-source prefab | Rideable water routes; current, drop-through and replay support |
| Water features | 11 families: grand/broad/narrow/split falls, rapids, pool, diagonal rapids, spray, foreground curtain/narrow/diagonal streams | River climb and chapter climax |
| New water accents | Impact foam, surface eddy, dripping curtain, fine mist; 4 animation frames each | Falls, banks, foreground droplets and spray |
| Grassy columns | Configurable width/depth, continuous grass cap, vertical dirt sides | Reference-style platform columns, using original terrain pixels |

All 32 scenery props have darker background prefabs and decorative Tile assets. Floating timber has four dark visual-only background variants. Water features and accents have dark/foreground variants. Native terrain has nine foreground and nine darker Tile assets.

## Files

- Source atlases: `Assets/_OneMoreTime/Art/WorldOne/Expansion/`.
- Props, floating platforms and stream sources: `Assets/_OneMoreTime/Prefabs/WorldOne/Expansion/`.
- Water and accent prefabs: `Assets/_OneMoreTime/Prefabs/WorldOne/WaterKit/`.
- Grass platform: `Assets/_OneMoreTime/Prefabs/WorldOne/GrassyColumnPlatform.prefab`.
- Grass background: `Assets/_OneMoreTime/Prefabs/Background/GrassyColumnPlatformBackground.prefab`.
- Native grass/dirt tiles: `Assets/_OneMoreTime/Generated/GrassyColumns/`.
- Generated prop sprites/tiles/materials: `Assets/_OneMoreTime/Generated/WorldOne/Expansion/`.

## Preview

Main Menu → Debug Mode → **08 World 1 Art**, or open `Assets/_OneMoreTime/Scenes/Debug_WorldOneArt.unity` and press Play.

Use **1–6** or **Tab** to switch between original art, forest props, mountain props, water effects, giant-waterfall composition, and floating timber. WASD/arrows pan the camera. Space previews drowning; C previews statue activation; Esc returns to the menu. The gallery displays timber artwork; its functional riding/replay behavior belongs to a room's fixed simulation clock and is verified separately.

## Placement and scale

All new artwork renders at **16 effective pixels per world unit**, matching the existing terrain. Props have explicit world widths and trimmed transparent bounds. Ground props use bottom-center pivots; hanging props use top-center pivots. Their colliders are intentionally absent. Treat arches/overhangs as scenery; add gameplay collision only when a room layout requires it. Decorative Tile assets need the matching material when rendered on a Tilemap to retain pixel scale; prefab placement is simplest.

The grassy column origin is its walking surface. Edit `GrassyColumnVisual.width` and `depth`, then use **Refresh grass column** or change the Inspector field. Only the thin top is a one-way platform. The downward soil is visual, so jumping upward through the cap and down+jump remain possible. Main Rooms 1, 3 and 4 use the new art; Room 2 has no matching grass platform to replace. Layout and collision data are preserved by an explicit migration check.

The grand waterfall defaults to **12 units wide × 22 high**, with separate impact foam and mist. Change the child `WaterFeature.size` rather than Transform scale so pixels stay consistent. Foreground water sorts above the player at reduced opacity; darker variants sort behind terrain. Water artwork alone does not create a death zone/current. Use the existing WaterHazard/WaterCurrent2D components when the layout needs them. See [WATER_EXPANSION.md](WATER_EXPANSION.md).

Floating platforms use the existing pooled `FloatingObject2D` system: visible-width 3–4 units, one-way top, white original colors, gentle bobbing, current and replay carry. Sources default to **2 units/sec leftward, 10-second interval**, up to six objects. Register the source in the room replay participant list; the editor helper wires this automatically. Bounds are world coordinates. See [FLOATING_TIMBER.md](FLOATING_TIMBER.md).

## Rebuild

Run these Unity editor methods in this order, outside Play mode:

1. `GrassyPlatformBuilder.Build()` — native tiles/prefabs and art-only migration of saved main rooms.
2. `WaterFeatureBuilder.Build()` — water features and new accent frames.
3. `FloatingTimberBuilder.Build()` — platforms/backgrounds/source pools.
4. `ForestExpansionBuilder.Build()` — forest/mountain props, decorative tiles and six-section gallery.

Each builder manages its own assets; generated references retain their GUIDs. The gallery builder replaces only `Forest Expansion Exhibits`. It does not generate Rooms 5–7 or change the original room layouts.
