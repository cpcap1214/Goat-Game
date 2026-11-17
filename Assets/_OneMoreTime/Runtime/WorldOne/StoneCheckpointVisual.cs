using UnityEngine;
namespace OneMoreTime {
// The collider/checkpoint location belongs to the room; this component only animates art.
public sealed class StoneCheckpointVisual:MonoBehaviour {
 public SpriteRenderer target;public Sprite[] frames;
 public bool Active{get;private set;}
 float clock;
 public void Activate(){if(Active)return;Active=true;clock=0;}
 public void ResetDisplay(){Active=false;clock=0;Show();}
 void Update(){Tick(Time.deltaTime);}
 public void Tick(float dt){if(Active)clock+=Mathf.Max(0,dt);Show();}
 void Show(){if(target==null||frames==null||frames.Length<6)return;int index=!Active?0:clock<.8f?Mathf.Min(3,1+Mathf.FloorToInt(clock*4)):4+Mathf.FloorToInt((clock-.8f)*3)%2;target.sprite=frames[index];}
}
}
