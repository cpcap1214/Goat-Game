# Forest and Grand Waterfall expansion — built-in imagegen

Five original atlases generated with the built-in image_gen tool. Existing Sunny Land tree/tiles and WaterAtlas supplied as style references; originals retained. Project copies: Assets/_OneMoreTime/Art/WorldOne/Expansion/. Sprite slicing and native pixel-scale rendering are handled by Unity builders.

## forest

Use case: stylized-concept. Asset type: a production 2D side-view pixel-art SPRITE ATLAS for a forest mountain climbing platformer, matching the supplied Sunny Land tree and terrain palette. References are style only, not edit targets.
Generate a single coherent transparent 4 by 4 sprite sheet, 16 isolated distinct objects, precisely one centered object per equal square cell, no grid lines, no text, no labels, no ground plane, no drop shadows. Every sprite completely contained with 12% transparent padding on every side. Square canvas.
Read cells left to right top to bottom:
row1: broad fern clump; small fern clump; riverbank reed clump; flowering meadow tuft with tiny cream/yellow flowers.
row2: red-brown mushroom cluster; pale shelf fungus cluster attached to a small bark piece; moss-covered fallen log; hollow broken stump.
row3: tall hanging leafy vine; paired hanging roots; arching exposed tree root; low tangled roots with moss.
row4: round mossy river rock; stacked wet river stones; jagged mossy rock outcrop; leafy branch reaching sideways.
Style strict crisp low-resolution pixel art, chunky deliberate clusters, about 48x48 logical pixels per cell, no smooth gradients or antialiasing, no tiny noisy detail. Match warm ochre/terracotta bark, plum-brown deepest shadows, deep green forest foliage, lime grass highlights and muted moss of the references. Flat side-view game sprites; natural irregular silhouettes; consistent upper-left lighting; moderate dark outlines. Actual transparent alpha background, not checkerboard. Do not add water backdrops or scenery between sprites.

## mountain

Use case: stylized-concept. Asset type: production transparent 4x4 sprite atlas for a forest/mountain/waterfall 2D side-view platformer. Supplied tree and terrain are STYLE references.
16 separate decorative assets, one per equal square cell, uniform 4 columns by4 rows, all objects completely contained with 12% clear padding, no text/grid/background/shadow. Row1: moss-covered ancient stone arch; broken square stone pillar; cracked carved stone slab with no writing; small ruined stone doorway.
Row2: tall jagged mountainside rock spire; wide flat mossy boulder; angular cliff-face rock cluster; hanging rocky overhang with small vines.
Row3: upright weathered timber support frame; cross-braced mine support beam; old wooden waterwheel; weathered bridge support post with coiled rope.
Row4: bent windswept mountain pine; small alpine shrub; long leafy hanging canopy branch; rocky stream-bank clump with small reeds.
Strict crisp 2D pixel art matching Sunny Land. About48x48 logical pixels each cell, flat side-view, no perspective scene. Warm ochre/terracotta wood, plum-brown rock shadows, moss/lime/deep green vegetation, subdued gray-brown stone. Limited palette, distinct chunky pixels, consistent upper-left light, moderate dark outlines. No gradients, no glow, no tiny noisy texture. Actual transparent alpha. Decorative environment art with clear silhouettes for three consecutive forest-climb rooms.

## water

Use case: stylized-concept. Asset type: animated pixel-art water accents SPRITE SHEET for a forest mountain platformer. Reference WaterAtlas is the exact STYLE/PALETTE reference, not to be changed.
Create a square 4 columns x4 rows transparent atlas. Each row is ONE effect in four consecutive animation frames left-to-right, all in exactly equal cells, fixed anchoring/size, 1-second loop, frame4 transitions back naturally to frame1. Keep every effect wholly inside each cell, 10% transparent padding.
Row1: low wide waterfall impact foam surge, ivory foam crest over muted teal/slate water, never a tall explosion.
Row2: shallow surface eddy/ripple, side-view wide oval rippling foam around center.
Row3: hanging water drip curtain, four slender thin falling strands with scattered droplets, fine stream foreground overlay, mostly transparent.
Row4: low spreading waterfall mist/spray, sparse pixel clusters of pale warm mint/cream, thin and translucent not solid cloud.
Palette MUST stay identical to provided atlas: #344F69 deep slate, #477F9C blue-gray, #65A7B7 desaturated teal, #94C8C6 light mint, #E3E6C0 cream foam, #304B59 dark shadow. No saturated cyan, no new purple/green colors, no warm brown, no characters. About32x32 logical pixels per cell, deliberate crisp chunky pixel clusters and small bounded particles, no painterly shading, no smooth blur. True transparent alpha background, no checkerboard. Animation shapes maintain volume/position, subtle coherent travel, no changing camera or objects.

## timber

Use case: stylized-concept. Asset type: transparent floating timber platform sprites for a forest river pixel-art game, side-on view.
A wide horizontal 4-column ONE ROW atlas, precisely four equal square cells, canvas aspect 4:1 if possible or wide3:1. One isolated object centered in each cell, no text or grid.
Left to right: 1 a long horizontal cylindrical floating log with clear cut tree rings at left end and stripped warm bark; 2 a long moss-topped floating log with small snapped branch stubs below the top; 3 a flat small raft of three parallel logs lashed with two hemp ropes, seen from side with slight top visible but clearly horizontal playable deck; 4 a long weathered flat wooden plank with irregular chipped ends.
All top surfaces are relatively flat and legible as platforms. Keep all four objects same horizontal baseline in center cell height, roughly70-80% of cell width, roughly20-30% of cell height. No water painted underneath (water rendered separately), no leaves sticking high into standing space, no decorations or props above them. Actual alpha transparent surrounding space, no fake checkerboard, no shadow floor.
Style matching supplied Sunny Land terrain: strict low-resolution crisp deliberate pixel clusters, about48 logical pixels across each log; ochre/copper highlights, terracotta bark, dark plum-brown outlines and shadow, moss/deep green accents. Moderate dark outline, warm upper-left lighting, no smooth gradients or noisy texture. Distinct clean silhouettes, all isolated with12% padding.


## grand waterfall

Use case: stylized-concept. Asset type: GRAND WATERFALL animation atlas for a pixel-art forest mountain platformer. Reference1 existing WaterAtlas defines palette and 4x4 arrangement. Create an additional atlas for a much larger majestic powerful waterfall, do not copy its dark sparse body.
Exactly4columns x4rows on square canvas, each row has4 consecutive animationframes of one seamlessly repeatable1unit tile. No gaps or borders. Rows lefttoright:
row1 shallow river surface, waving cream foam along top, slate water beneath, transparent air in top quarter.
row2 waterfall top lip, broad scalloping white-cream/mint water rolling over edge into downwardflow, transparent small airspace only topedge, tile horizontally seamlessly.
row3 FULL OPAQUE body of grand waterfall, fast flowing broad vertical bands of pale mint and warm cream occupy at least55percent area, interrupted bluegray channels and changing shorter light streaks. NO AIR or gaps. Tile seamlessly on ALL FOUR EDGES, with consistent brightnessanddensity across frames. No horizontal lines, no crossbars, no checkerboard. Flow travels down continuously acrossframes loop4to1. A powerful luminouscurtain made of chunky deliberatepixels.
row4 impact foam, low irregular cream/mint froth and small spray at top against transparent air, slatepoolbottom. Keep overallshapeconsistent fourframes, tile horizontally.
Use EXACT limited palette #344F69 deepestslate, #477F9C bluegray, #65A7B7 desaturatedteal, #94C8C6 pale mint, #E3E6C0 warmfoamcream, #304B59 shadow. No brightcyan! Strict lowres crisp pixelart, about32logicalpixelspercell, no gradients, painterlyshading, antialiasing or fine noise. Actual alpha transparent where air requested. Atlas only, no scenery/text/grid. Cell boundaries at preciselyquarters, samecellsize and composition in everyframe.
