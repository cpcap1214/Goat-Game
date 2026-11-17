using UnityEngine;
namespace OneMoreTime {
// Main-game shutter: capture in place. Legacy tutorial memory rules remain separate.
public sealed class ShutterAbility : MonoBehaviour {
 public MainRoom room;
 public bool unlockedOnEntry;
 public Transform pickup;
 public EchoSurface echo;
 public bool Unlocked {get;private set;}
 public bool Snapping {get;private set;}
 public bool HasSnapshot=>echo!=null&&echo.Active;
 public const float SnapDuration=.32f;
 public float SnapProgress=>Mathf.Clamp01(snapTime/SnapDuration);
 public Vector2 Position=>HasSnapshot?echo.Pose.position:Vector2.zero;
 public string Hint=>notice>0?"Shutter unlocked! Jump and press [E] to freeze a copy.":!Unlocked?"Collect the purple camera to unlock Shutter":HasSnapshot?"[E] Replace frozen copy · [R] Retry (keeps copy)":"Jump and press [E] to leave a frozen copy. No reset.";
 float snapTime,notice;
 RecordedFrame captured;
 AudioSource sound;AudioClip click;
 void Awake(){Unlocked=unlockedOnEntry;echo.Clear();sound=gameObject.AddComponent<AudioSource>();sound.playOnAwake=false;
  const int rate=22050;var samples=new float[(int)(rate*.16f)];uint seed=1297;
  for(int i=0;i<samples.Length;i++){seed=1664525*seed+1013904223;float noise=((seed>>8)&65535)/32767.5f-1;float t=i/(float)rate;float env=Mathf.Exp(-t*100)+(t>.065f?Mathf.Exp(-(t-.065f)*125)*.65f:0);samples[i]=noise*env*.6f;}
  click=AudioClip.Create("Synthesized camera shutter",samples.Length,1,rate,false);click.SetData(samples,0);
 }
 void OnDestroy(){if(click!=null)Destroy(click);}
 public void CheckPickup(){if(!Unlocked&&pickup!=null&&Vector2.Distance(room.player.Position,pickup.position)<1.3f){Unlocked=true;pickup.gameObject.SetActive(false);notice=4;}}
 // Returns true while the room simulation must remain frozen, including the finish tick.
 public bool Tick(float dt){notice=Mathf.Max(0,notice-dt);if(!Snapping)return false;snapTime+=dt;
  if(snapTime>=SnapDuration)Snapping=false;
  return true;
 }
 public bool Press(){
  if(!Unlocked||Snapping)return false;
  captured=new RecordedFrame(0,room.player);echo.Show(captured,true);echo.support.enabled=false;
  snapTime=0;Snapping=true;notice=0;sound.PlayOneShot(click,GameAudio.EffectsVolume);return true;
 }
 public void DrawOverlay(){if(!Snapping)return;float t=SnapProgress;
  GUI.matrix=Matrix4x4.identity;
  float close=Mathf.Sin(t*Mathf.PI);float h=Screen.height*.5f*close;
  GameUI.Panel(new Rect(0,0,Screen.width,h),new Color(.02f,.025f,.04f,.9f));
  GameUI.Panel(new Rect(0,Screen.height-h,Screen.width,h),new Color(.02f,.025f,.04f,.9f));

 }
}
}
