using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace OneMoreTime.EditorTools {
// Timber uses the established fixed-clock pooled floating-object system. It has
// no independent Update/FixedUpdate, so recording and replay own its motion once.
public static class FloatingTimberBuilder {
 public const string AtlasPath = "Assets/_OneMoreTime/Art/WorldOne/Expansion/FloatingTimber.png";
 public const string GeneratedFolder = "Assets/_OneMoreTime/Generated/WorldOne/Expansion";
 public const string PrefabFolder = "Assets/_OneMoreTime/Prefabs/WorldOne/Expansion";
 public static readonly string[] Variants = { "FloatingLog", "MossyFloatingLog", "BoundLogRaft", "FloatingTimberPlank" };
 static readonly float[] Widths = { 3.5f, 3.5f, 4f, 3f };

 [MenuItem("Tools/One More Time/Build Floating Timber")]
 public static void Build() {
  if (EditorApplication.isPlaying) throw new InvalidOperationException("Exit Play mode before building floating timber.");
  if (!File.Exists(AtlasPath)) throw new FileNotFoundException("Generate the four-column floating timber atlas first.", AtlasPath);
  Directory.CreateDirectory(GeneratedFolder);
  Directory.CreateDirectory(PrefabFolder);
  Directory.CreateDirectory(PrefabFolder + "/Background");
  AssetDatabase.Refresh();
  var importer = (TextureImporter)AssetImporter.GetAtPath(AtlasPath);
  importer.textureType = TextureImporterType.Default;
  importer.isReadable = true;
  importer.alphaIsTransparency = true;
  importer.filterMode = FilterMode.Point;
  importer.mipmapEnabled = false;
  importer.textureCompression = TextureImporterCompression.Uncompressed;
  importer.maxTextureSize = 4096;
  importer.SaveAndReimport();
  var texture = AssetDatabase.LoadAssetAtPath<Texture2D>(AtlasPath);
  var pixels = texture.GetPixels32();
  for (int i = 0; i < Variants.Length; i++) {
   int left = Mathf.RoundToInt(i * texture.width / 4f);
   int right = Mathf.RoundToInt((i + 1) * texture.width / 4f);
   int minX = right, maxX = left, minY = texture.height, maxY = 0;
   for (int y = 0; y < texture.height; y++) for (int x = left; x < right; x++) {
    if (pixels[y * texture.width + x].a < 128) continue;
    minX = Mathf.Min(minX, x); maxX = Mathf.Max(maxX, x);
    minY = Mathf.Min(minY, y); maxY = Mathf.Max(maxY, y);
   }
   if (minX > maxX || minY > maxY) throw new InvalidOperationException("No opaque timber in atlas column " + i);
   int width = maxX - minX + 1, height = maxY - minY + 1, surface = maxY;
   // Ignore twigs/moss above the broad top: collision follows the visible deck.
   for (int y = maxY; y >= minY; y--) {
    int occupied = 0;
    for (int x = minX; x <= maxX; x++) if (pixels[y * texture.width + x].a >= 128) occupied++;
    if (occupied >= width * .7f) { surface = y; break; }
   }
   float ppu = width / Widths[i];
   var fresh = Sprite.Create(texture, new Rect(minX, minY, width, height), new Vector2(.5f, (surface - minY + .5f) / height), ppu, 0, SpriteMeshType.FullRect);
   fresh.name = Variants[i];
   string spritePath = GeneratedFolder + "/" + Variants[i] + ".asset";
   var sprite = AssetDatabase.LoadAssetAtPath<Sprite>(spritePath);
   if (sprite == null) { AssetDatabase.CreateAsset(fresh, spritePath); sprite = fresh; }
   else { EditorUtility.CopySerialized(fresh, sprite); Object.DestroyImmediate(fresh); }
   string materialPath = GeneratedFolder + "/" + Variants[i] + ".mat";
   var material = AssetDatabase.LoadAssetAtPath<Material>(materialPath);
   if (material == null) { material = new Material(Shader.Find("OneMoreTime/Terrain Pixel Scale")); AssetDatabase.CreateAsset(material, materialPath); }
   material.mainTexture = texture;
   material.SetVector("_PixelGrid", new Vector4(texture.width / ppu * 16, texture.height / ppu * 16, 0, 0));
   EditorUtility.SetDirty(material);

   var go = new GameObject(Variants[i]);
   go.layer = DemoBuilder.GetLayer();
   var box = go.AddComponent<BoxCollider2D>();
   box.size = new Vector2(Widths[i] * .9f, .18f);
   box.offset = new Vector2(0, -.09f);
   var body = go.AddComponent<Rigidbody2D>();
   body.bodyType = RigidbodyType2D.Kinematic;
   body.gravityScale = 0;
   body.freezeRotation = true;
   go.AddComponent<OneWayPlatform>();
   var floating = go.AddComponent<FloatingObject2D>();
   floating.floatOffset = i == 3 ? .18f : .28f;
   floating.bobAmplitude = .035f;
   var visual = go.AddComponent<SpriteRenderer>();
   visual.sprite = sprite;
   visual.sharedMaterial = material;
   visual.sortingOrder = 6;
   visual.color = Color.white;
   var prefab = PrefabUtility.SaveAsPrefabAsset(go, PrefabFolder + "/" + Variants[i] + ".prefab");
   Object.DestroyImmediate(go);
   var background = new GameObject(Variants[i] + " Background");
   var backgroundVisual = background.AddComponent<SpriteRenderer>();
   backgroundVisual.sprite = sprite;
   backgroundVisual.sharedMaterial = material;
   backgroundVisual.sortingOrder = -20;
   backgroundVisual.color = new Color(.5f, .5f, .5f, 1);
   PrefabUtility.SaveAsPrefabAsset(background, PrefabFolder + "/Background/" + Variants[i] + "Background.prefab");
   Object.DestroyImmediate(background);
   BuildSource(prefab, Variants[i]);
  }
  AssetDatabase.SaveAssets();
 }

 static void BuildSource(GameObject platformPrefab, string variant) {
  var root = new GameObject(variant + " Stream Source");
  var source = root.AddComponent<CrateSpawner>();
  source.affectedByRewind = true;
  source.spawnInterval = 10;
  source.initialHorizontalVelocity = -2;
  source.maximumActiveCrates = 6;
  var currentObject = new GameObject("Current - configure world bounds and surface");
  currentObject.transform.SetParent(root.transform, false);
  source.water = currentObject.AddComponent<WaterCurrent2D>();
  source.water.left = -10;
  source.water.right = 10;
  source.water.surfaceY = 0;
  source.water.velocity = -2;
  var spawn = new GameObject("Spawn");
  spawn.transform.SetParent(root.transform, false);
  spawn.transform.localPosition = new Vector3(9.5f, .28f, 0);
  source.spawnPoint = spawn.transform;
  var disposal = new GameObject("Disposal");
  disposal.transform.SetParent(root.transform, false);
  disposal.transform.localPosition = new Vector3(-11, 0, 0);
  source.disposal = disposal.AddComponent<CrateDisposalTrigger>();
  source.disposal.owner = source;
  source.disposal.boundaryX = -10;
  var trigger = disposal.GetComponent<BoxCollider2D>();
  trigger.isTrigger = true;
  trigger.size = new Vector2(1, 6);
  source.pool = new FloatingObject2D[source.maximumActiveCrates];
  for (int i = 0; i < source.pool.Length; i++) {
   var item = (GameObject)PrefabUtility.InstantiatePrefab(platformPrefab);
   item.name = variant + " pooled " + (i + 1);
   item.transform.SetParent(root.transform, false);
   source.pool[i] = item.GetComponent<FloatingObject2D>();
   source.pool[i].water = source.water;
   item.SetActive(false);
  }
  PrefabUtility.SaveAsPrefabAsset(root, PrefabFolder + "/" + variant + "Source.prefab");
  Object.DestroyImmediate(root);
 }

 // Call while building a room, before RewindPlaytest.Awake captures its retry state.
 // Current.left/right/surfaceY and disposalX are WORLD coordinates. spawn is also
 // a world position; moving the source root afterwards does not move those bounds.
 // Existing disposal evaluates leftward flow only (matching current room streams).
 // This wires the existing current and participant list; no runtime registration
 // or second physics clock is introduced, and room/crate defaults are unchanged.
 public static CrateSpawner InstallSource(Transform parent, string variant, WaterCurrent2D current, Vector2 spawn, float disposalX, RewindPlaytest replay) {
  if (EditorApplication.isPlaying) throw new InvalidOperationException("Install floating timber in Edit mode before the room clock starts.");
  if (!Variants.Contains(variant)) throw new ArgumentException("Unknown floating timber variant.", nameof(variant));
  if (current == null || replay == null) throw new ArgumentNullException("A room current and replay clock are required.");
  var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(PrefabFolder + "/" + variant + "Source.prefab");
  if (prefab == null) throw new InvalidOperationException("Build Floating Timber first.");
  var root = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
  root.transform.SetParent(parent, false);
  var source = root.GetComponent<CrateSpawner>();
  source.water = current;
  source.initialHorizontalVelocity = current.velocity;
  source.spawnPoint.position = spawn;
  source.disposal.boundaryX = disposalX;
  source.disposal.transform.position = new Vector2(disposalX - 1, current.surfaceY);
  foreach (var item in source.pool) item.water = current;
  replay.participants = (replay.participants ?? new RewindParticipant[0]).Concat(new RewindParticipant[] { source }).ToArray();
  EditorUtility.SetDirty(replay);
  return source;
 }
}
}
