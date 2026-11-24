using UnityEngine;

namespace OneMoreTime {
// A short warning, a falling bridge, then a forgiving rebuild. The existing room
// recorder snapshots this state on its own clock, including during fast rewind.
public sealed class ChapterCrumbleBridge : RewindParticipant {
 public PlayerMotor2D player;
 public Collider2D deck;
 public SpriteRenderer[] artwork=new SpriteRenderer[0];
 [Min(.1f)]public float collapseDelay=.65f;
 [Min(.5f)]public float rebuildDelay=5;
 public bool Armed {get;private set;}
 public bool Collapsed {get;private set;}
 public float Remaining {get;private set;}
 public override bool HasTransformHistory=>false;
 Vector3[] homes;Quaternion[] rotations;Color[] colors;
 float brokenTime;
 struct State {public bool armed,collapsed;public float remaining,brokenTime;}
 void Initialize(){
  if(homes!=null)return;
  homes=new Vector3[artwork.Length];rotations=new Quaternion[artwork.Length];colors=new Color[artwork.Length];
  for(int i=0;i<artwork.Length;i++)if(artwork[i]!=null){homes[i]=artwork[i].transform.localPosition;rotations[i]=artwork[i].transform.localRotation;colors[i]=artwork[i].color;}
 }
 public override void Tick(float dt){
  Initialize();
  if(Collapsed){brokenTime+=dt;if(brokenTime>=rebuildDelay){Collapsed=Armed=false;Remaining=brokenTime=0;}}
  else {
   if(!Armed&&player!=null&&player.Grounded&&player.GroundCollider==deck){Armed=true;Remaining=collapseDelay;}
   if(Armed){Remaining=Mathf.Max(0,Remaining-dt);if(Remaining<=0){Collapsed=true;brokenTime=0;}}
  }
  Present();
 }
 public override object Capture(){Initialize();return new State{armed=Armed,collapsed=Collapsed,remaining=Remaining,brokenTime=brokenTime};}
 public override void Restore(object value){Initialize();var state=(State)value;Armed=state.armed;Collapsed=state.collapsed;Remaining=state.remaining;brokenTime=state.brokenTime;Present();}
 void Present(){
  if(deck!=null)deck.enabled=!Collapsed;
  for(int i=0;i<artwork.Length;i++)if(artwork[i]!=null){
   var art=artwork[i];float fall=Mathf.Min(brokenTime,1.3f),side=i%2==0?-1:1;
   art.transform.localPosition=homes[i]+(Collapsed?new Vector3(side*fall*.15f,-4*fall*fall,0):Vector3.zero);
   art.transform.localRotation=rotations[i]*Quaternion.Euler(0,0,Collapsed?side*fall*8:0);
   var color=colors[i];if(Collapsed)color.a*=1-Mathf.Clamp01((brokenTime-.25f)/.8f);
   else if(Armed)color=Color.Lerp(color,new Color(1,.65f,.35f,color.a),.25f*(1-Remaining/collapseDelay));
   art.color=color;
  }
 }
}
}
