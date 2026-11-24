using UnityEngine;

namespace OneMoreTime {
/// <summary>A local visual darkness or mist volume, cleared by the live goat's flashlight.</summary>
[ExecuteAlways, DisallowMultipleComponent, RequireComponent(typeof(SpriteRenderer))]
public sealed class ChapterTwoObscurity : MonoBehaviour {
 public enum Kind { Darkness, Mist }
 public Kind kind;
 public FlashlightAbility flashlight;
 [Tooltip("Local rectangular coverage in units. This does not create collision or damage.")]
 public Vector2 size = new Vector2(20, 12);
 [Range(0, 1)] public float opacity = .93f;
 public Color tint = new Color(.065f, .078f, .105f, 1);
 [Min(0)] public float edgeFeather = .5f;
 public int sortingOrder = 60;
 [Tooltip("Serialized reference keeps the obscurity shader available in builds.")]
 public Shader shader;

 SpriteRenderer target;
 Texture2D ownedTexture;
 Sprite ownedSprite;
 Material ownedMaterial;

 /// <summary>Effective visual coverage, with the same pixel grid and beam as the shader.</summary>
 public float ObscurityAt(Vector2 worldPoint) {
  Vector2 local = transform.InverseTransformPoint(worldPoint);
  Vector2 half = SafeSize() * .5f;
  float edgeDistance = Mathf.Min(half.x - Mathf.Abs(local.x), half.y - Mathf.Abs(local.y));
  if(edgeDistance <= 0) return 0;
  float feather = edgeFeather > 0 ? Mathf.SmoothStep(0, 1, edgeDistance / edgeFeather) : 1;
  bool available = flashlight != null && flashlight.isActiveAndEnabled;
  float exposure = available ? flashlight.ExposureAt(worldPoint) : 0;
  float density = kind == Kind.Mist ? MistDensity(worldPoint, available ? flashlight.Clock : 0) : 1;
  return Mathf.Clamp01(opacity) * Mathf.Clamp01(tint.a) * feather * density * (1 - exposure);
 }

 // Mirrors mistDensity in the shader. Two warped, slowly moving ribbons give
 // mist actual transparent gaps; the darkness volume keeps its original opacity.
 static float MistDensity(Vector2 point, float clock) {
  point = new Vector2((Mathf.Floor(point.x*16)+.5f)/16, (Mathf.Floor(point.y*16)+.5f)/16);
  float warp = Mathf.Sin(point.x*.47f+clock*.12f)*1.25f
   + Mathf.Sin(point.x*.19f-point.y*.32f-clock*.09f)*.7f;
  float ribbon = .5f+.5f*Mathf.Sin(point.y*1.7f+warp-clock*.2f);
  float curl = .5f+.5f*Mathf.Sin(point.y*.8f-point.x*.17f
   + Mathf.Sin(point.x*.25f+point.y*.21f+clock*.08f)*1.3f+clock*.12f);
  return .22f+.53f*ribbon*ribbon+.25f*curl*curl*curl;
 }

 /// <summary>Refresh immediately after a manual simulation tick or before a render capture.</summary>
 public void RefreshVisual() {
  if(!isActiveAndEnabled) return;
  if(target == null) target = GetComponent<SpriteRenderer>();
  var selectedShader = shader != null ? shader : Shader.Find("OneMoreTime/Chapter Two Obscurity");
  if(selectedShader == null) { target.enabled = false; return; }
  if(ownedMaterial == null || ownedMaterial.shader != selectedShader) {
   Release(ownedMaterial);
   ownedMaterial = new Material(selectedShader) { name = "Chapter 2 obscurity (instance)", hideFlags = HideFlags.HideAndDontSave };
  }
  if(ownedSprite == null) {
   ownedTexture = new Texture2D(1, 1, TextureFormat.RGBA32, false) {
    name = "Chapter 2 obscurity white pixel", filterMode = FilterMode.Point,
    wrapMode = TextureWrapMode.Clamp, hideFlags = HideFlags.HideAndDontSave
   };
   ownedTexture.SetPixel(0, 0, Color.white); ownedTexture.Apply(false, true);
   ownedSprite = Sprite.Create(ownedTexture, new Rect(0, 0, 1, 1), new Vector2(.5f, .5f), 1, 0, SpriteMeshType.FullRect);
   ownedSprite.name = "Chapter 2 obscurity quad";
   ownedSprite.hideFlags = HideFlags.HideAndDontSave;
  }
  target.sprite = ownedSprite;
  target.sharedMaterial = ownedMaterial;
  target.drawMode = SpriteDrawMode.Sliced;
  target.size = SafeSize();
  target.color = Color.white;
  target.sortingOrder = sortingOrder;
  target.enabled = true;

  ownedMaterial.SetColor("_Tint", tint);
  ownedMaterial.SetVector("_Size", new Vector4(target.size.x, target.size.y, 0, 0));
  ownedMaterial.SetFloat("_Opacity", Mathf.Clamp01(opacity));
  ownedMaterial.SetFloat("_EdgeFeather", Mathf.Max(0, edgeFeather));
  ownedMaterial.SetFloat("_Kind", (int)kind);
  bool available = flashlight != null && flashlight.isActiveAndEnabled;
  Vector2 origin = available ? flashlight.Origin : Vector2.zero;
  ownedMaterial.SetVector("_BeamOrigin", new Vector4(origin.x, origin.y, 0, 0));
  ownedMaterial.SetFloat("_BeamFacing", available ? flashlight.Facing : 1);
  ownedMaterial.SetFloat("_BeamRange", available ? Mathf.Max(.01f, flashlight.range) : 6);
  ownedMaterial.SetFloat("_NearRadius", available ? Mathf.Max(.01f, flashlight.nearRadius) : 1.1f);
  ownedMaterial.SetFloat("_HalfAngle", available ? Mathf.Clamp(flashlight.halfAngle, 1, 89) : 32);
  ownedMaterial.SetFloat("_Strength", available ? Mathf.Clamp01(flashlight.Strength) : 0);
  // Never use shader _Time: pausing the room must also stop the drifting mist.
  ownedMaterial.SetFloat("_Clock", available ? flashlight.Clock : 0);
 }

 Vector2 SafeSize() => new Vector2(Mathf.Max(.0625f, size.x), Mathf.Max(.0625f, size.y));
 void OnEnable() { RefreshVisual(); }
 void LateUpdate() { RefreshVisual(); }
 void OnDisable() { Cleanup(); }
 void OnDestroy() { Cleanup(); }
 void Cleanup() {
  if(target != null) {
   if(target.sharedMaterial == ownedMaterial) target.sharedMaterial = null;
   if(target.sprite == ownedSprite) target.sprite = null;
  }
  Release(ownedMaterial); Release(ownedSprite); Release(ownedTexture);
  ownedMaterial = null; ownedSprite = null; ownedTexture = null;
 }
 static void Release(Object owned) {
  if(owned == null) return;
  if(Application.isPlaying) Destroy(owned); else DestroyImmediate(owned);
 }
}
}
