# Camera ability art

Five transparent sprites now share plain charcoal housings, silver edges and neutral white highlights. The magic lens alone keeps a luminous teal spiral and etched runes. Created with the built-in image generation tool; source PNGs are preserved in `Assets/_OneMoreTime/Resources/AbilityArt/`. The Unity importer makes small, Point-filtered, uncompressed sprites without mipmaps. Exact generation prompts are in [CAMERA_ABILITY_ART_PROMPTS.md](CAMERA_ABILITY_ART_PROMPTS.md).

| Asset | Design | In-game use |
|---|---|---|
| `ShutterCamera.png` | Charcoal and silver camera with blue optical lens | Room 2 Shutter pickup, original freeze tutorial, pause camera body |
| `ReplayTape.png` | Plain two-spool cassette with a side connector | Room 4 recording pickup, original trace tutorial, attached recording module |
| `ArcaneReplayTape.png` | Matching cassette with a small teal reverse symbol | Debug Room 7 upgraded replay pickup; replaces the ordinary cassette on the pause camera |
| `FlashModule.png` | Neutral white diffuser with a silver hot-shoe mount | Flashlight sandbox pause camera; reusable Chapter 2 module sprite |
| `PastLens.png` | Silver-rimmed filter with teal runes and luminous memory spiral | EchoFoothills filter pickup; mounted over the pause camera's optical lens |

The bottom-left of each gameplay pause panel shows the assembled camera. Its cassette plugs into the left edge, the flashlight foot meets the top hot shoe, and the magic filter aligns over the optical lens. Both cassette variants use the same socket. Normalized source rectangles in CameraEquipment crop transparent padding during UI drawing; the source PNGs and pickup sprites remain whole. Only acquired, locally available modules appear, including the shutter camera body. Before the Room 2 pickup the assembly is empty. Chapter 1 has only shutter and basic replay; upgraded replay, filter and flashlight are restricted to their debug contexts. The advanced cassette replaces the basic cassette instead of adding a second recording power. The main-game death counter stays at the opposite bottom-right. Nothing is added to the live gameplay HUD.

`CameraEquipment.For` reads the owning room's actual unlock/availability state, including the special locked-before-pickup state in Room 4. Debug play does not inherit unrelated campaign unlock flags. The display does not grant abilities or affect recording, collisions, camera movement or input.

`AbilityArt.ReplacePickup` adds one visual child to existing pickups, disables their previous renderers, and preserves roots, pickup positions, colliders and collection logic. Existing scene assets and old artwork remain intact. A one-time runtime replacement also works for future scenes generated with the existing builders. There is no new flashlight acquisition location; that remains for a future Chapter 2 layout.

Open Main Menu → Debug Mode → 09 Room 4 for the cassette acquisition; Room 7 for the advanced cassette, EchoFoothills for the magic lens, or Chapter 2 / Flashlight for the top-mounted light. Press Esc to inspect the camera. Sprite resources can be reused directly from `AbilityArt` without adding another copy to the project.

The folder-specific `AbilityArtImporter` limits runtime textures to 64 pixels on the longest side. Source pixels-per-unit equals the source image's longest dimension; Unity's import scaling then keeps the full sprite canvas approximately one world unit. This avoids inflated world bounds when importing the larger generation originals. Aspect ratio, alpha and original source files are preserved.

Validation: real Unity compilation and ten focused PlayMode tests passed (seven art/import/pickup tests and three equipment/progression tests). Actual Game-view captures were reviewed for the normal Room 5 camera, all-module assembly and Room 4 cassette pickup. The all-module capture uses temporary Play-mode preview state, not a change to campaign progression. See `Artifacts/Logs/camera-equipment-room5.png`, `camera-equipment-all-modules.png` and `camera-ability-room4-pickup.png`. No standalone build or complete human playthrough was run.
