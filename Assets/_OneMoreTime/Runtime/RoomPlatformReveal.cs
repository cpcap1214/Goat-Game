using UnityEngine;
namespace OneMoreTime {
// Room-owned reveal: advances presentation only while the shared gameplay clock is paused.
public sealed class RoomPlatformReveal:MonoBehaviour {
 public PressurePlate plate;public RoomPlatform platform;
 [Tooltip("Optional additional triggers. Pressing any plate reveals the target; this does not activate the platform or change its required plates.")]
 public PressurePlate[] triggeringPlates=new PressurePlate[0];
 public float panSeconds=.45f,holdSeconds=.7f,returnSeconds=.45f;
 public bool Revealing{get;private set;}public bool Shown{get;private set;}
 float elapsed;Vector3 focus;
 public void ResetReveal(){Revealing=Shown=false;elapsed=0;}
 bool TriggerPressed(){
  if(plate!=null&&plate.Pressed)return true;
  if(triggeringPlates!=null)foreach(var trigger in triggeringPlates)if(trigger!=null&&trigger.Pressed)return true;
  return false;
 }
 public bool Tick(float dt){
  if(!Shown){if(platform==null||!TriggerPressed())return false;Shown=Revealing=true;elapsed=0;focus=platform.transform.position+Vector3.up;}
  if(!Revealing)return false;
  elapsed+=Mathf.Max(0,dt);if(elapsed>=panSeconds+holdSeconds+returnSeconds)Revealing=false;
  return true;
 }
 public void ApplyCamera(Camera view,Vector3 normal,Rect bounds){
  if(!Revealing)return;
  float a=elapsed<panSeconds?Mathf.SmoothStep(0,1,elapsed/Mathf.Max(.01f,panSeconds)):elapsed<panSeconds+holdSeconds?1:1-Mathf.SmoothStep(0,1,(elapsed-panSeconds-holdSeconds)/Mathf.Max(.01f,returnSeconds));
  float h=view.orthographicSize,w=h*view.aspect;
  Vector3 target=new Vector3(bounds.width<=2*w?bounds.center.x:Mathf.Clamp(focus.x,bounds.xMin+w,bounds.xMax-w),bounds.height<=2*h?bounds.center.y:Mathf.Clamp(focus.y,bounds.yMin+h,bounds.yMax-h),normal.z);
  view.transform.position=Vector3.Lerp(normal,target,a);
 }
}
}
