using System;using UnityEngine;
namespace OneMoreTime {
public sealed class Recorder {
 public const int Capacity=501;readonly RecordedFrame[] frames=new RecordedFrame[Capacity];int head,count;
 public int Count=>count;public float Duration=>count<2?0:At(count-1).time-At(0).time;
 public RecordedFrame At(int i){if(i<0||i>=count)throw new ArgumentOutOfRangeException();return frames[(head+i)%Capacity];}
 public void Clear(){head=count=0;}
 public bool Add(RecordedFrame f){if(!float.IsFinite(f.time)||!float.IsFinite(f.position.x)||!float.IsFinite(f.position.y)||(count>0&&f.time<=At(count-1).time))return false;
  if(count==Capacity){head=(head+1)%Capacity;count--;}
  frames[(head+count)%Capacity]=f;count++;
  while(count>1&&f.time-At(0).time>10.0001f){head=(head+1)%Capacity;count--;}return true;
 }
 public MemorySnapshot Save(int level){if(count<2||Duration<.25f)return null;var copy=new RecordedFrame[count];float start=At(0).time;for(int i=0;i<count;i++){copy[i]=At(i);copy[i].time-=start;}return new MemorySnapshot(copy,level);}
}
}
