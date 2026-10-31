using UnityEngine;
namespace OneMoreTime {
public sealed class EchoSurface : MonoBehaviour {
 public PlayerVisual visual;public SpriteRenderer support;
 float fade;public bool Active{get;private set;}public RecordedFrame Pose{get;private set;}
 public void Show(RecordedFrame pose,bool active){fade=0;Pose=pose;Active=active;gameObject.SetActive(true);visual.Pose(pose.position,pose.facing,pose.state,pose.frame,new Color(.4f,.95f,1,active?.72f:.25f));support.enabled=active;}
 public void Finish(){Active=false;support.enabled=false;fade=.22f;}
 public void TickFade(float dt){if(fade<=0)return;fade=Mathf.Max(0,fade-dt);var c=visual.renderer2D.color;c.a=.72f*fade/.22f;visual.renderer2D.color=c;if(fade==0)gameObject.SetActive(false);}
 public void Clear(){fade=0;Active=false;gameObject.SetActive(false);}
}
}
