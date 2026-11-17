# World 1 — The Forest and The Grand Waterfall

## Preview
Main menu → Debug Mode → **08 World 1 Art / 森林與大瀑布**.
Alternatively open `Assets/_OneMoreTime/Scenes/Debug_WorldOneArt.unity` and press Play.
- Up / Down: scroll the repeating background vertically without a height limit.
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
| Waterfall | Three tiles wide, five tiles high; lip, repeating body and splash. Each type has four frames at 8 fps. Extend its Tilemap as needed. |
| ForestWaterfallBackground | Looping forest/cliff/waterfall background. Assign `view` (or use MainCamera). `repeatWidth` / `repeatHeight` control texture size in world units; `parallax.x/y` control camera-relative speed. Defaults: 30×30 units, 0.2 horizontal / 0.35 vertical speed. `edgeBlend` overlaps joins. Repeats indefinitely in both directions without spawning tiles. |
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
