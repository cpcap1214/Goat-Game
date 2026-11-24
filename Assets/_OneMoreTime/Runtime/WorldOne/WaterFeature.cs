using UnityEngine;

namespace OneMoreTime {
/// <summary>Resizable, decorative water. A separate WaterHazard/current supplies gameplay.</summary>
[ExecuteAlways, DisallowMultipleComponent, RequireComponent(typeof(SpriteRenderer))]
public sealed class WaterFeature : MonoBehaviour {
 public enum Shape { Waterfall, River, Pool, Diagonal, Mist }
 public Shape shape;
 [Tooltip("Size in world units. The texture stays at 16 visible pixels per unit.")]
 public Vector2 size = new Vector2(6, 12);
 [Range(.25f, 4)] public float streamThickness = 1;
 [Tooltip("Draw over the goat, with transparency, for a foreground stream.")]
 public bool foreground;
 public bool background;
 [Range(0, 1)] public float opacity = 1;
 public int sortingOrder = -5;
 MaterialPropertyBlock properties;
 SpriteRenderer target;
 bool dirty=true;

 public void Refresh() {
  dirty=false;
  target = GetComponent<SpriteRenderer>();
  size = new Vector2(Mathf.Max(.25f, size.x), Mathf.Max(.25f, size.y));
  target.drawMode = SpriteDrawMode.Sliced;
  target.size = size;
  target.sortingOrder = foreground ? Mathf.Max(20, sortingOrder) : background ? Mathf.Min(-20, sortingOrder) : sortingOrder;
  float shade = background ? .5f : 1;
  // The main gameplay silhouette remains readable through foreground water.
  target.color = new Color(shade, shade, shade, foreground ? Mathf.Min(.38f, opacity) : opacity);
  if (properties == null) properties = new MaterialPropertyBlock();
  target.GetPropertyBlock(properties);
  properties.SetVector("_Size", new Vector4(size.x, size.y, 0, 0));
  properties.SetFloat("_Shape", (float)shape);
  properties.SetFloat("_Thickness", streamThickness);
  target.SetPropertyBlock(properties);
 }
 void OnEnable() { dirty=true; }
 void OnValidate() { size=new Vector2(Mathf.Max(.25f,size.x),Mathf.Max(.25f,size.y));dirty=true; }
 void LateUpdate() { if(dirty)Refresh(); }
}
}
