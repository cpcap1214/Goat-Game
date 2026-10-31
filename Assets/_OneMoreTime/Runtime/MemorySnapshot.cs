using UnityEngine;
namespace OneMoreTime {
public enum MemoryKind{None,Freeze,Trace}public enum RunPhase{Preparation,Execution}public enum FlowState{Playing,Paused,DeathReview,Completed,AbilityDialogue,AbilityDemonstration}public enum PlaybackState{Ready,Playing,Finished}
public struct RecordedFrame {
 public float time;public Vector2 position;public int facing,state,frame;public bool grounded;
 public RecordedFrame(float t,PlayerMotor2D m){time=t;position=m.Position;facing=m.Facing;grounded=m.Grounded;state=m.VisualState;frame=m.VisualFrame;}
}
public sealed class MemorySnapshot {
 public MemoryKind Kind {get;}public RecordedFrame FreezePose{get;}public int Level{get;}
 readonly RecordedFrame[] frames;
 public int Count=>frames==null?0:frames.Length; public float Duration=>Count<2?0:frames[Count-1].time;
 public RecordedFrame Frame(int index)=>frames[index];
 public MemorySnapshot(RecordedFrame[] source,int level){if(source==null||source.Length<2)throw new System.ArgumentException("Too few samples");frames=(RecordedFrame[])source.Clone();Kind=MemoryKind.Trace;Level=level;}
 public RecordedFrame Sample(float time){if(Count==0)return FreezePose;if(time<=0)return frames[0];if(time>=Duration)return frames[Count-1];int lo=0,hi=Count-1;while(hi-lo>1){int mid=(lo+hi)/2;if(frames[mid].time<=time)lo=mid;else hi=mid;}var f=frames[lo];f.position=Vector2.Lerp(f.position,frames[hi].position,(time-f.time)/(frames[hi].time-f.time));return f;}
 public MemorySnapshot(RecordedFrame pose,int level){Kind=MemoryKind.Freeze;FreezePose=pose;Level=level;}
}
}
