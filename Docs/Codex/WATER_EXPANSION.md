# Forest and Grand Waterfall water expansion

The expansion supplies decorative, animated water for the next three rooms without changing existing layouts, hazards, current velocities, or crate timing. Rebuild with **Tools → One More Time → Build Water Expansion Kit**, or `OneMoreTime.EditorTools.WaterFeatureBuilder.Build()`.

Assets are generated under `Assets/_OneMoreTime/Prefabs/WorldOne/WaterKit/` and `Assets/_OneMoreTime/Generated/WorldOne/WaterKit/`. Existing water assets and their GUIDs are retained.

## Water features

| Prefab | World size | Intended use |
| --- | --- | --- |
| GrandWaterfall | 12 × 22, plus 14 × 2 spray | Chapter-climax waterfall, continuous curtain with upper lip and impact foam |
| BroadWaterfall | 6 × 12 | Tall climbing-room cascade |
| NarrowCascade | 1.5 × 8 | Thin fall between rocky banks |
| SplitCascade | Two falls, 2 × 8 and 1 × 6 | Separated water ribbons with shared impact spray |
| ForestRapids | 8 × 1.5 | Shallow horizontal stream |
| DeepForestPool | 10 × 4 | Wide pool with animated surface and submerged fill |
| DiagonalRapids | 6 × 6 | Descending stream, natural waving edge |
| WaterfallSpray | 6 × 2 | Low broken spray at an impact zone |
| ForegroundWaterCurtain | 4 × 14 | Translucent water in front of the player, 24% opacity |
| ForegroundNarrowStream | 0.75 × 10 | Thin foreground ribbon, 38% opacity |
| ForegroundDiagonalStream | 6 × 6 | Foreground sloping stream, 32% opacity |

Every feature has a `Background` prefab variant with half-brightness sprites and a sorting order below −20. Foreground versions sort above the goat at 22. These objects contain **no colliders**: add the existing `WaterHazard` and `WaterCurrent2D` separately where the layout calls for danger or current. The foreground layer is decorative.

Each water child has a **Water Feature** component. Change `Size` in world units instead of scaling its Transform to preserve the 16-pixel world grid. The origin is the bottom-left corner; the broad grand-waterfall spray extends one unit on either side. `Stream Thickness` adjusts the diagonal band. Repeated body sections join across their upper/lower edges and mirrored horizontal tiles share the same edge sample. The four-frame, 4-fps animation and six-color palette are the same as the existing streams. The grand waterfall uses `Expansion/GrandWaterfallAtlas.png` when available for its wider, brighter curtain; all other feature bodies retain the existing source atlas. Lip/body/foam use one clock, with a continuous last-to-first blend.

## Newly drawn accents

`Art/WorldOne/Expansion/WaterAccents.png` supplies four rows of four frames: **ImpactFoam**, **SurfaceEddy**, **DripCurtain**, and **FineWaterMist**. Each has a normal, `Foreground`, and `Background` prefab, plus a four-frame animated Tile asset. The `Water Accent` material synchronizes all instances to the same 4-fps clock and renders 16 visible pixels per unit. Cell pivots and bounds remain fixed throughout the animation. Prefabs are two units wide; arrange repeated accents along a wide waterfall rather than stretching their pixels. Foreground accents use 28% opacity.

For the Tile Palette, assign `Generated/WorldOne/WaterKit/WaterAccents.mat` to the destination Tilemap Renderer. The provided prefabs already carry this material.

## Verification entry point

`OneMoreTime.Tests.WaterFeatureTests` verifies generated variants, collider-free decoration, layer/opacity safety, resizable bounds, shader animation visibility and exact one-second loop closure, plus the new accent frame alignment. Run through the real Unity PlayMode runner after building assets; the main task's test report records the actual outcome. Tests do not replace a human review of final room composition.

Shader palette selection compares sRGB texture colors against sRGB palette swatches, then converts the chosen color once for the render pipeline. A GPU swatch assertion protects against the double-linearization bug that previously made the water too dark. Accent sprites preserve four equal-size frame rectangles even when the source sheet dimensions are not divisible by four.
