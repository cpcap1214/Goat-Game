using UnityEngine;
namespace OneMoreTime {
public sealed class WaterCurrent2D:MonoBehaviour {
 public float surfaceY=0,left=0,right=10,velocity=-2;
 public bool Contains(float x)=>x>=left&&x<=right;
}
}
