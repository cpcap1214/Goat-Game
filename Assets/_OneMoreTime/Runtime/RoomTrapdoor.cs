using UnityEngine;

namespace OneMoreTime {
// ChapterOneRoom owns this clock. The reveal deliberately does not enter rewind snapshots.
public sealed class RoomTrapdoor : MonoBehaviour {
 public const bool affectedByRewind=false;
 public PressurePlate plate;
 public Transform leftLeaf,rightLeaf;
 public BoxCollider2D leftCollider,rightCollider;
 public Vector2 targetPoint;
 public float horizontalCenter=10;
 [Min(0)]public float leafTravel=1.5f;
 [Min(.1f)]public float revealOrthographicSize=3.8f;
 [Min(.01f)]public float panDuration=.45f,openDuration=.5f,holdDuration=.6f,returnDuration=.45f;
 public bool Triggered{get;private set;}
 public bool Revealing{get;private set;}
 public bool Opened{get;private set;}
 public float OpenAmount{get;private set;}
 public float RevealProgress=>Triggered?Mathf.Clamp01(elapsed/TotalDuration):0;

 Vector3 leftClosed,rightClosed;
 float elapsed;
 bool initialized;
 float PanDuration=>Mathf.Max(.01f,panDuration);
 float OpenDuration=>Mathf.Max(.01f,openDuration);
 float HoldDuration=>Mathf.Max(0,holdDuration);
 float ReturnDuration=>Mathf.Max(.01f,returnDuration);
 float TotalDuration=>PanDuration+OpenDuration+HoldDuration+ReturnDuration;

 void Initialize(){
  if(initialized)return;
  initialized=true;
  if(leftLeaf!=null)leftClosed=leftLeaf.localPosition;
  if(rightLeaf!=null)rightClosed=rightLeaf.localPosition;
 }

 // True freezes gameplay for this entire fixed step, including the final return frame.
 // This must run before player, platform, recording and world clocks advance.
 public bool Tick(float dt){
  Initialize();
  if(!Triggered){
   if(plate==null||!plate.Pressed)return false;
   Triggered=true;Revealing=true;
  }
  if(!Revealing)return false;
  elapsed=Mathf.Min(TotalDuration,elapsed+Mathf.Max(0,dt));
  OpenAmount=Mathf.Clamp01((elapsed-PanDuration)/OpenDuration);
  float opening=Mathf.SmoothStep(0,1,OpenAmount);
  if(leftLeaf!=null)leftLeaf.localPosition=leftClosed+Vector3.left*(leafTravel*opening);
  if(rightLeaf!=null)rightLeaf.localPosition=rightClosed+Vector3.right*(leafTravel*opening);
  if(OpenAmount>=1){
   Opened=true;
   if(leftCollider!=null)leftCollider.enabled=false;
   if(rightCollider!=null)rightCollider.enabled=false;
  }
  if(elapsed>=TotalDuration)Revealing=false;
  return true;
 }

 // The 20-unit shaft retains its horizontal center even during the reveal's zoom.
 public void ApplyCamera(Camera view,Vector3 normalPosition,float normalSize){
  if(view==null)return;
  float focus=0;
  if(Revealing){
   float returnStart=PanDuration+OpenDuration+HoldDuration;
   focus=elapsed<PanDuration?Mathf.SmoothStep(0,1,elapsed/PanDuration):
    elapsed<returnStart?1:1-Mathf.SmoothStep(0,1,(elapsed-returnStart)/ReturnDuration);
  }
  var destination=new Vector3(horizontalCenter,targetPoint.y,normalPosition.z);
  normalPosition.x=horizontalCenter;
  view.transform.position=Vector3.Lerp(normalPosition,destination,focus);
  view.orthographicSize=Mathf.Lerp(normalSize,Mathf.Min(normalSize,revealOrthographicSize),focus);
 }
}
}
