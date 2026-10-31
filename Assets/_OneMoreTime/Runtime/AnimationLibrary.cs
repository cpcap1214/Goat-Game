using UnityEngine;
namespace OneMoreTime {
[CreateAssetMenu(menuName="One More Time/Animation Library")]
public sealed class AnimationLibrary : ScriptableObject {
 public const int RunCycleFrames=6;
 public const float RunFramesPerSecond=12;
 public Sprite[] idle,run,rise,descent,landing;public Sprite jump,fall;public float visualScale=5;public Vector2 visualOffset=new Vector2(0,.6f);
 public Sprite Get(int state,int frame){
  if(state==1&&run!=null&&run.Length>0)return run[(Mathf.Abs(frame)%RunCycleFrames)*run.Length/RunCycleFrames];
  var list=state==2?rise:state==3?descent:state==4?landing:state==1?run:idle;
  if(list!=null&&list.Length>0)return list[state>=2?Mathf.Clamp(frame,0,list.Length-1):Mathf.Abs(frame)%list.Length];
  return state==3?fall:jump;
 }
}
}
