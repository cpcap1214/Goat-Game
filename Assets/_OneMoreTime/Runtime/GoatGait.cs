using UnityEngine;
namespace OneMoreTime {
public static class GoatGait {
 public static Vector2 Foot(float phase) {
  phase=Mathf.Repeat(phase,1);
  if(phase<.5f)return new Vector2(Mathf.Lerp(27,-27,phase*2),0);
  float u=(phase-.5f)*2;
  return new Vector2(-27-54*u+324*u*u-216*u*u*u,18*Mathf.Pow(Mathf.Sin(Mathf.PI*u),2));
 }
}
}
