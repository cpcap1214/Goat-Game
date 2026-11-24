# World 1 — The Forest and The Grand Waterfall

## Preview
Main menu → Debug Mode → **08 World 1 Art / 森林與大瀑布**.
Alternatively open `Assets/_OneMoreTime/Scenes/Debug_WorldOneArt.unity` and press Play.
- Arrow keys / WASD: pan the repeating background horizontally and vertically without limits. Preview camera size is 7 (about 24.9 units wide at 16:9); exhibits remain visible while panning.
- Space: replay the six-frame drowning animation.
- C: replay the stone checkpoint activation.
- Esc: main menu.

## Assets
Source PNGs: `Assets/_OneMoreTime/Art/WorldOne/`.
Generated sprites, materials and animated TileBase assets: `Assets/_OneMoreTime/Generated/WorldOne/`.
Reusable prefabs: `Assets/_OneMoreTime/Prefabs/WorldOne/`.

| Asset | Use |
|---|---|
| FlowingRiver | Six sample 1-unit animated surface tiles; extend its Tilemap with RiverSurface.asset. |
| Waterfall | Three tiles wide, five tiles high; lip, repeating body and splash. Each type has four poses at 4 fps (one-second cycle), blended continuously with the Looping Water shader, including the last-to-first transition. All tiles share a clock. Extend its Tilemap as needed. |
| SidewaysWaterfall | Right-flowing horizontal waterfall: lip, body and impact foam. SidewaysWaterfallLip/Body/Splash TileBase assets rotate the matching vertical art 90°. Rotate the prefab 180° for leftward flow. |
| ForestWaterfallBackground | Looping forest/cliff/waterfall background. Assign `view` (or use MainCamera). `repeatWidth` / `repeatHeight` control texture size in world units; `parallax.x/y` control camera-relative speed. Defaults: 62.7×62.7 units, 0.2 horizontal / 0.35 vertical speed. `edgeBlend` overlaps joins. Repeats indefinitely in both directions without spawning tiles. |
| GoatDrowning | Six frames at 6 fps, non-looping; ends on ripples. |
| DeepWaterHazard | Invisible trigger volume with `causesDrowning` toggle and a drowning prefab reference. Assign placed volumes to `MainRoom.waterHazards`. Decorative water has no collision or death effect. |
| StoneGoatCheckpoint | Gray goat statue; RoomCheckpoint.Save() starts cracks/green eyes/aura; last two frames loop gently. Add placed checkpoints to `MainRoom.checkpoints`. |

All textures use point filtering, no mipmaps and uncompressed alpha. Water cells are approximately one Unity unit; an inset excludes atlas edge bleed. The generated water is tile-based; inspect joins at the final placement/scale when building the waterfall landscape. The static distant waterfall is part of the scrolling backdrop; foreground waterfall tiles animate. The original finite ForestWaterfall.png is retained; the prefab now uses ForestWaterfallLoop.png with the Looping Forest shader. Its art has no skyline/valley endpoint, allowing endless climbs.

Drowning hides the live goat while its visual animation plays, then restores it at the existing saved checkpoint (or room start). It increments deaths once, does not reload the scene, and preserves the existing respawn/reset rules. Pause also pauses this sequence. No lethal water volumes or new terrain were inserted into the existing room layouts.

The new scenery and statue are reusable assets in the gallery; existing main-room backgrounds and checkpoint placements remain available for the upcoming World 1 layout pass. This preview is an asset gallery, not a new playable level.

## Rebuild
Exit Play mode, then **Tools → One More Time → Build World One Art Preview**.
The builder uses Unity's asset/scene APIs, preserves asset references, manages only its named gallery root, refuses to rebuild a dirty loaded gallery, and appends the gallery to Build Settings without changing the existing scene order.

## Generation provenance
Tool: built-in `image_gen.imagegen`, using the existing Sunny Land tileset and GoatAnimations.png as reference images. Originals remain in the Codex generated-images folder. Output copied into the project. No external assets purchased.

Prompts / direction supplied:
- Water: production side-view pixel-art 4×4 atlas; four sequential frames per row for river surface, waterfall lip, vertical body, impact foam; teal/turquoise/ivory palette, transparent air, seamless repeat, no rocks/scenery/text/grid. Match Sunny Land chunky pixels. A follow-up image edit explicitly removed painted white/checkerboard air and grid lines while preserving blue water and cream foam.
- Background: tall portrait forest and grand waterfall, vertically scrolling from dense shaded valley and low lake through layered forest/cliffs to high mountains and pale sky; distant subdued scenery, no foreground platforms or characters, pixel clusters matching Sunny Land, opaque.
- Drowning: exact existing right-facing cream/tan goat with upright brown horns and dark hooves; six frames in 3×2 cells, splash/paddle/chest submerged/head/horn tips/ripples, fixed waterline, nonviolent, transparent background. The revised sheet keeps the goat horizontal and fully hides submerged limbs. Unity pivots are measured from each frame's dense turquoise waterline to prevent vertical jumps; see DROWNING_REDRAW_PROMPT.md for the exact replacement prompt.
- Stone checkpoint (created last): exact goat silhouette carved from gray granite with plinth; six 3×2 frames, dormant black eye → cracks → green cracks/eye → bright aura → two quiet aura frames. Gray stone remains gray. No labels/background. Unity pivots align the stone plinth independently of surrounding glow.

### Revised water loops
WaterAtlas.png was redrawn with more stable crests/streaks. Keep the supplied WaterAtlas.mat on water TilemapRenderers: it blends premultiplied colors across consecutive atlas cells at 4 poses/second. A generic sprite material only shows the discrete TileBase animation. The material expects the un-packed 4×4 atlas; do not include this texture in a SpriteAtlas. Exact prompt: WATER_LOOP_REDRAW_PROMPT.md. Sideways prefab appears above the goat/statue in the debug gallery.

### Background pixel scale (2026-11-18)
The loop backdrop is zoomed 2.09× relative to its previous 30-unit repeat. Reference is the tutorial forest layer: middle.png has 176 pixels displayed across 10 units (17.6 pixels/unit). With 12% edge overlap the 1254-pixel loop uses 62.7 units per repeat: 1254 × 0.88 / 17.6. This matches texture sampling density; the painted pixel-cluster detail remains that of the generated art. Camera zoom and gameplay coordinates are unchanged.

## Wooden suspension bridge

Drag `Assets/_OneMoreTime/Prefabs/WorldOne/WoodenSuspensionBridge.prefab` into a room. Position its root at the desired walking surface. Set **Middle Tiles** to change length: each segment is 2 units wide, plus two 2-unit ends. The collider and endpoints update automatically. Keep the existing platform layer. Ropes/posts have no collision; the deck uses the project one-way platform behavior. Preview: Debug Mode → World 1 Art.

Source: `Art/WorldOne/WoodenSuspensionBridge.png`. Separate reusable sprites: `Generated/WorldOne/BridgeLeftEnd.asset`, `BridgeMiddle.asset`, `BridgeRightEnd.asset`. Artwork generated with built-in imagegen; prompt archived in `Docs/Codex/WOODEN_BRIDGE_PROMPT.md`.

## Inward tapered grass/dirt ledge
Use `Assets/_OneMoreTime/Prefabs/WorldOne/GrassDirtLedge.prefab` for a 4-unit-wide one-way platform with a downward triangular underside. Root position is the walking surface. The underside is visual only, consistent with grassy platforms. `Generated/WorldOne/GrassDirtLedge.asset` is the standalone sprite; `GrassDirtLedgeTile.asset` is a decorative tile with collision disabled (use the prefab for gameplay collision). Source image: `Art/WorldOne/GrassDirtLedge.png`. Preview under the bridge in Debug World 1 Art. Built-in imagegen prompt: `Docs/Codex/GRASS_LEDGE_PROMPT.md`.

## 2026-11-18 Pixel-scale correction (supersedes earlier zoom guidance)
Original terrain uses 16 source pixels per world unit. World One sprite materials now sample at the same native grid; water uses 16×16 cells per one-unit tile. Bridge source import preserves full dimensions so equal thirds stay equal. Forest background uses a 384-cell grid at 16 pixels/unit, with a 21.12-unit repeat after edge overlap, replacing the incorrect 62.7-unit enlargement. Original source images remain intact. Use the generated prefab materials; dropping a raw high-resolution PNG into a room bypasses the pixel-grid rendering. Keep prefab transform scale at 1 for matching density.

Current background selection: **ForestWaterfallLoop.png**, the Tuesday Nov 17 ~02:26 version, restored at user request. Pixel-grid correction and loop edge handling remain enabled. ForestWaterfallPixel.png is retained but unused.

## Wide forest background
Current background is ForestWaterfallWide.png: a 3:1 panorama extending the selected Tuesday composition with new scenery on both sides. The original square is retained. Horizontal repeat is now 63.36 units (3× 21.12), vertical repeat remains 21.12; the shader uses a 1152×384 native sampling grid to preserve square pixels and the existing scenery scale. Both-axis wrapping and edge overlap remain enabled. Generated center retains the composition but is not pixel-identical. Prompt: WIDE_BACKGROUND_PROMPT.md.

## Coordinated water remake — 2026-11-18
Replaced WaterAtlas.png in place with simpler blue/cream pixel clusters referenced against the original Sunny Land terrain. River, waterfall lip/body/splash and sideways variants share the same four-frame sheet. WaterCommon.cginc applies one 4 fps clock, a six-color palette, 16-pixel tile sampling and a continuous final-to-first blend; transparency is thresholded to remove matte speckles. Full-depth water now also animates from this sheet.

New `Generated/WorldOne/DiagonalStream.asset` and `Prefabs/WorldOne/DiagonalStream.prefab`: descending approximately 45-degree wedge with a naturally animated surface, derived from the river frames. Use its supplied prefab, or put the tile on a separate TilemapRenderer with `Generated/WorldOne/DiagonalWater.mat`. Reflect the prefab horizontally for the opposite slope. No collider is added; existing level water hazards remain responsible for gameplay. The prefab is displayed to the right of the bridge in Debug_WorldOneArt. Room geometry is unchanged.

`_PreviewTime` is a material-only deterministic render-test control; leave at -1 for the shared game clock. Exact built-in imagegen prompt and source references: WATER_REMAKE_PROMPT.md. Recording hold threshold and on-screen prompts changed to 0.25 seconds (13 fixed 20-ms ticks, i.e. first simulation tick at/after threshold).

## 2026-11-19 Expansion for the next three rooms

The art preview now has six sections, selected with 1–6 or Tab. Added forest/mountain props, rideable floating logs and raft, modular giant waterfall, foreground streams, new animated water accents, dark variants and reference-style rectangular grass columns. Full inventory, paths and placement instructions: [FOREST_EXPANSION.md](FOREST_EXPANSION.md). Built-in imagegen provenance: [FOREST_EXPANSION_PROMPTS.md](FOREST_EXPANSION_PROMPTS.md).


## Wooden exit signs — 2026-11-24

Source reference: user-provided `/Users/wesley/Downloads/IMG_1114.JPG`. Built-in image_gen was used (not CLI) to isolate/adapt the upper-right wooden sign, followed by a matching upward variant. Final source files: `Assets/_OneMoreTime/Art/WorldOne/WoodenExitSign.png` and `WoodenExitSignUp.png`. Full generated sources remain intact; Unity imports them at 64px, uncompressed, point-filtered, no mipmaps. Sprite slices crop transparent padding with the pivot at the post's foot; reusable prefabs are in `Assets/_OneMoreTime/Prefabs/WorldOne/`.

Placement via `ExitSignBuilder`: Room 1 (74.2,12.4), Room 2 (41.6,5), Room 3 (28.5,39), Room 4 (94.2,12), Room 5 (66.5,5), Room 6 (85.7,9), Room 7 (9.5,63.7), Room 8 (116.7,47.3). Room 7 uses the upward variant at 75% scale to fit its top camera boundary; other arrows point right. Layer order 5 keeps all signs behind the player and foreground water. No colliders. To reapply without rebuilding rooms, use Tools → One More Time → Place Main Room Exit Signs.

Right-arrow prompt:

> Use case: background-extraction. Asset type: transparent pixel-art sprite for a Unity side-scrolling platformer. Input image: edit target sprite sheet. Isolate ONLY the right-pointing wooden arrow sign in the upper-right of the supplied sheet. Preserve its exact simple warm brown wood, horizontal wood grain, stepped jagged arrow silhouette, upright single post, and flat front-on pixel-art style. Remove every other object and remove the baked checkerboard completely. Output one sign, centered, with genuine transparent alpha around it, no checkerboard, no shadow outside the sprite, no ground, no scenery, no text. Make edges clean and crisp on a regular pixel grid (about 48 by 48 logical pixels for the complete sign), with flat discrete pixel color clusters, no blur, no anti-aliasing, no painted fine detail. Keep the right-pointing arrow and upright post recognizable and close to the original. Use a square canvas with modest transparent margins.

Upward-arrow prompt (edit target: generated right-arrow image):

> Use case: precise-object-edit. Asset type: matching pixel-art directional exit sign. Edit target: the supplied wooden right-arrow sign. Make one upward-pointing wooden arrow sign instead, standing on an upright wooden post: rotate only the arrow-shaped board so it points straight up, retain the supporting post extending straight down underneath the board. Keep same warm brown shades, dark outline, blocky pixel wood grain, exact same simple art style. Full object centered on square canvas with modest transparent margin. Genuine transparent alpha background, no black or checkerboard background, no ground, no shadow, no text or other objects. Crisp regular pixel grid with about 48 by 48 logical pixels for the complete sprite. This is the matching upward exit variant for the same game.
