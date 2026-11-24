using UnityEngine;
namespace OneMoreTime {
[RequireComponent(typeof(Rigidbody2D),typeof(BoxCollider2D))]
public sealed class FloatingObject2D:MonoBehaviour,IPlayerCarrier {
 public float gravity=18,buoyancyStrength=36,damping=9,floatOffset=.35f,bobAmplitude=.04f,bobFrequency=1.8f;
 public Vector2 velocity;public WaterCurrent2D water;public Vector2 CarryDelta{get;private set;}
 public float clock;Rigidbody2D body;
 public bool Sinking{get;private set;}
 float sinkX,sinkBottom,sinkSpeed,sinkSurface;
 // Clip the submerged part, rather than fading or recolouring the entire crate.
 // Materials are per renderer so a neighbouring floating item stays untouched.
 sealed class SinkVisual {public SpriteRenderer renderer;public Material original,clipped;}
 SinkVisual[] sinkVisuals;
 public struct State {public bool active,sinking;public Vector2 position,velocity;public float clock,sinkX,sinkBottom,sinkSpeed,sinkSurface;}
 void Awake(){body=GetComponent<Rigidbody2D>();body.bodyType=RigidbodyType2D.Kinematic;body.gravityScale=0;body.freezeRotation=true;}
 public void Tick(float dt){
  if(!gameObject.activeSelf)return;
  Vector2 old=transform.position;clock+=dt;
  if(Sinking){
   // The drain uses the same fixed clock as buoyancy and recording. Do not
   // destroy pooled crates: reversing a saved frame must bring them back.
   velocity.x=Mathf.MoveTowards(velocity.x,0,gravity*dt);
   velocity.y=Mathf.MoveTowards(velocity.y,-sinkSpeed,gravity*dt);
   Vector2 submerged=new Vector2(Mathf.MoveTowards(old.x,sinkX,sinkSpeed*dt),Mathf.Max(sinkBottom,old.y+velocity.y*dt));
   CarryDelta=submerged-old;transform.position=submerged;if(body!=null)body.position=submerged;
   if(submerged.y<=sinkBottom){gameObject.SetActive(false);CarryDelta=Vector2.zero;}
   return;
  }
  bool wet=water!=null&&water.Contains(old.x)&&old.y<water.surfaceY+1.5f;
  float acceleration=-gravity;
  if(wet){float target=water.surfaceY+floatOffset+bobAmplitude*Mathf.Sin(clock*bobFrequency);acceleration=buoyancyStrength*(target-old.y)-damping*velocity.y;velocity.x=Mathf.MoveTowards(velocity.x,water.velocity,dt*4);}
  velocity.y=Mathf.Clamp(velocity.y+acceleration*dt,-8,5);
  Vector2 next=old+velocity*dt;CarryDelta=next-old;transform.position=next;if(body!=null)body.position=next;
 }
 public void BeginSink(float centerX,float bottom,float speed){
  if(Sinking||!gameObject.activeSelf)return;
  Sinking=true;sinkX=centerX;sinkBottom=bottom;sinkSpeed=Mathf.Max(.1f,speed);sinkSurface=water!=null?water.surfaceY:transform.position.y;
  velocity.y=Mathf.Min(velocity.y,-.5f);
  RefreshSubmersion();
 }
 public void RestorePlayback(State state,bool carry){Vector2 old=transform.position;bool wasActive=gameObject.activeSelf;Restore(state);CarryDelta=carry&&wasActive&&state.active?state.position-old:Vector2.zero;}
 public void HoldFrame(){CarryDelta=Vector2.zero;}
 public State Capture()=>new State{active=gameObject.activeSelf,position=transform.position,velocity=velocity,clock=clock,sinking=Sinking,sinkX=sinkX,sinkBottom=sinkBottom,sinkSpeed=sinkSpeed,sinkSurface=sinkSurface};
 public void Restore(State s){gameObject.SetActive(s.active);transform.position=s.position;velocity=s.velocity;clock=s.clock;Sinking=s.sinking;sinkX=s.sinkX;sinkBottom=s.sinkBottom;sinkSpeed=s.sinkSpeed;sinkSurface=s.sinkSurface;CarryDelta=Vector2.zero;if(body!=null)body.position=s.position;RefreshSubmersion();}
 void RefreshSubmersion(){
  if(sinkVisuals==null){
   if(!Sinking)return;
   var shader=Resources.Load<Shader>("FloatingSubmersion");if(shader==null)return;
   var renderers=GetComponentsInChildren<SpriteRenderer>(true);sinkVisuals=new SinkVisual[renderers.Length];
   for(int i=0;i<renderers.Length;i++){
    var original=renderers[i].sharedMaterial;
    var clipped=original!=null?new Material(original):new Material(shader);clipped.shader=shader;
    clipped.name="Submerged "+renderers[i].name;clipped.hideFlags=HideFlags.HideAndDontSave;
    clipped.SetFloat("_UsePixelGrid",original!=null&&original.HasProperty("_PixelGrid")?1:0);
    sinkVisuals[i]=new SinkVisual{renderer=renderers[i],original=original,clipped=clipped};
   }
  }
  foreach(var visual in sinkVisuals){
   if(visual.renderer==null)continue;
   visual.clipped.SetFloat("_WaterlineY",sinkSurface);
   visual.renderer.sharedMaterial=Sinking?visual.clipped:visual.original;
  }
 }
 void OnDestroy(){
  if(sinkVisuals==null)return;
  foreach(var visual in sinkVisuals)if(visual.clipped!=null){if(Application.isPlaying)Destroy(visual.clipped);else DestroyImmediate(visual.clipped);}
 }
}
}
