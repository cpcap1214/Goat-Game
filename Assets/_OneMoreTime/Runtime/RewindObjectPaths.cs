using System.Collections.Generic;
using UnityEngine;
namespace OneMoreTime {
// Visual-only paths use the same samples as the recording, never drive objects.
public sealed class RewindObjectPaths {
 public static readonly Color TraceColor=new Color(.72f,.66f,.55f,.2f);
 public static readonly Color RewindTint=new Color(.88f,.82f,.71f,1);
 sealed class Track {public Transform target;public readonly List<Vector3?> points=new List<Vector3?>();}
 readonly List<Track> tracks=new List<Track>();
 readonly Dictionary<SpriteRenderer,Color> originalColors=new Dictionary<SpriteRenderer,Color>();
 GameObject root;Material material;Texture2D dash;bool tinted;
 public void Begin(RewindPlaytest room){
  Tint(false);ClearLines();tracks.Clear();originalColors.Clear();
  var targets=new HashSet<Transform>();
  foreach(var p in room.platforms)if(p.affectedByRewind)targets.Add(p.transform);
  foreach(var c in room.crates)if(c.affectedByRewind)targets.Add(c.transform);
  foreach(var p in room.participants)if(p.affectedByRewind&&p.HasTransformHistory){if(p is CrateSpawner sp){foreach(var c in sp.pool)targets.Add(c.transform);}else targets.Add(p.transform);}
  foreach(var target in targets){tracks.Add(new Track{target=target});if(target.GetComponent<FloatingObject2D>()!=null||target.GetComponent<RewindCrate>()!=null)continue;foreach(var sr in target.GetComponentsInChildren<SpriteRenderer>(true))originalColors[sr]=sr.color;}
  Sample();
 }
 public void Sample(){foreach(var t in tracks)t.points.Add(t.target!=null&&t.target.gameObject.activeInHierarchy?(Vector3?)t.target.position:null);}
 public void Tint(bool rewinding){
  if(!rewinding&&!tinted)return;
  if(rewinding&&!tinted){var renderers=new List<SpriteRenderer>(originalColors.Keys);foreach(var sr in renderers)if(sr!=null)originalColors[sr]=sr.color;}
  foreach(var pair in originalColors)if(pair.Key!=null)pair.Key.color=rewinding?pair.Value*RewindTint:pair.Value;
  tinted=rewinding;
 }
 public void Show(Transform parent){
  ClearLines();root=new GameObject("Object replay traces");root.transform.SetParent(parent,false);
  if(material==null){dash=new Texture2D(16,1,TextureFormat.RGBA32,false);dash.name="Replay dash pattern";dash.wrapMode=TextureWrapMode.Repeat;dash.filterMode=FilterMode.Point;for(int x=0;x<16;x++)dash.SetPixel(x,0,x<9?Color.white:Color.clear);dash.Apply();material=new Material(Shader.Find("Sprites/Default"));material.mainTexture=dash;material.mainTextureScale=new Vector2(3,1);}
  foreach(var track in tracks){var points=new List<Vector3>();foreach(var point in track.points){
   if(!point.HasValue||(points.Count>0&&Vector3.Distance(points[points.Count-1],point.Value)>2)){Draw(points);points.Clear();}
   if(point.HasValue)points.Add(point.Value);
  }Draw(points);}
 }
 void Draw(List<Vector3> points){
  if(points.Count<2)return;float length=0;for(int i=1;i<points.Count;i++)length+=Vector3.Distance(points[i-1],points[i]);if(length<.05f)return;
  var go=new GameObject("Dashed object path");go.transform.SetParent(root.transform,false);var line=go.AddComponent<LineRenderer>();line.useWorldSpace=true;line.sharedMaterial=material;line.textureMode=LineTextureMode.Tile;line.textureScale=new Vector2(3,1);line.startWidth=line.endWidth=.018f;line.startColor=line.endColor=TraceColor;line.sortingOrder=1;line.positionCount=points.Count;line.SetPositions(points.ToArray());
 }
 void ClearLines(){if(root!=null){root.SetActive(false);Object.Destroy(root);root=null;}}
 public void Clear(){Tint(false);ClearLines();tracks.Clear();originalColors.Clear();}
 public void Dispose(){Clear();if(material!=null)Object.Destroy(material);if(dash!=null)Object.Destroy(dash);}
}
}
