using UnityEngine;
namespace OneMoreTime {
public sealed class CrumblingPlatform:MonoBehaviour {
 public BoxCollider2D solid;public SpriteRenderer graphic;public float delay=.65f;public float Remaining{get;private set;}=-1;public bool Collapsed{get;private set;}
 public void Tick(PlayerMotor2D player,float dt){if(Collapsed)return;if(Remaining<0&&player.Grounded&&player.GroundCollider==solid)Remaining=delay;if(Remaining>=0){Remaining-=dt;if(graphic!=null)graphic.color=Mathf.FloorToInt(Remaining*18)%2==0?new Color(1,.45f,.3f):Color.white;if(Remaining<=0){Collapsed=true;solid.enabled=false;if(graphic!=null)graphic.enabled=false;}}}
 public void ResetState(){Remaining=-1;Collapsed=false;solid.enabled=true;if(graphic!=null){graphic.enabled=true;graphic.color=Color.white;}}
}
}
