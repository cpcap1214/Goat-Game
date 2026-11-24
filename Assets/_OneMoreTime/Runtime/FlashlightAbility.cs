using UnityEngine;

namespace OneMoreTime {
// Chapter 2 opt-in ability. The room owns input and calls Tick on its simulation
// clock; there is no second Update/FixedUpdate or global light/volume registry.
public sealed class FlashlightAbility : MonoBehaviour {
 public PlayerMotor2D player;
 public bool unlocked;
 [Min(.1f)] public float activeDuration=4;
 [Min(0)] public float cooldownDuration=3;
 [Min(.2f)] public float range=6;
 [Min(.1f)] public float nearRadius=1.1f;
 [Range(5,80)] public float halfAngle=32;
 public float Remaining {get;private set;}
 public float CooldownRemaining {get;private set;}
 public float Clock {get;private set;}
 public bool Active=>unlocked&&Remaining>0;
 public bool Ready=>unlocked&&Remaining<=0&&CooldownRemaining<=0;
 public int Facing=>player!=null?player.Facing:1;
 public Vector2 Origin=>player!=null?player.Position+Vector2.up*(player.tuning!=null?player.tuning.height*.62f:.7f):(Vector2)transform.position;
 public float Strength=>Active?Mathf.Min(Mathf.SmoothStep(0,1,elapsed/.12f),Mathf.SmoothStep(0,1,Remaining/.35f)):0;
 public float Charge=>Active?Mathf.Clamp01(Remaining/Mathf.Max(.1f,activeDuration)):1-Mathf.Clamp01(CooldownRemaining/Mathf.Max(.01f,cooldownDuration));
 float elapsed;

 public bool TryActivate(){
  if(!isActiveAndEnabled||!Ready)return false;
  Remaining=Mathf.Max(.1f,activeDuration);elapsed=0;return true;
 }
 public void Tick(float dt){
  if(!isActiveAndEnabled||dt<=0||float.IsNaN(dt)||float.IsInfinity(dt))return;
  Clock+=dt;
  if(!unlocked){Remaining=CooldownRemaining=elapsed=0;return;}
  if(Remaining>0){
   float consumed=Mathf.Min(Remaining,dt);elapsed+=consumed;Remaining-=consumed;dt-=consumed;
   if(Remaining<.00001f){Remaining=0;CooldownRemaining=Mathf.Max(0,cooldownDuration);}
  }
  if(Remaining==0)CooldownRemaining=Mathf.Max(0,CooldownRemaining-dt);
 }
 public void ResetAbility(){Remaining=CooldownRemaining=elapsed=Clock=0;}
 void OnDisable(){ResetAbility();}

 // Shared with ChapterTwoObscurity.shader. This is a visibility query only;
 // level designers may use it for later puzzles without changing collision.
 public float ExposureAt(Vector2 point){
  if(!isActiveAndEnabled||Strength<=0)return 0;
  point=new Vector2((Mathf.Floor(point.x*16)+.5f)/16,(Mathf.Floor(point.y*16)+.5f)/16);
  Vector2 delta=point-Origin;float distance=delta.magnitude;
  float near=1-SmoothRange(nearRadius*.65f,nearRadius,distance);
  float direction=distance>.0001f?delta.x*Facing/distance:1;
  float cone=SmoothRange(Mathf.Cos(halfAngle*Mathf.Deg2Rad),Mathf.Cos(halfAngle*.75f*Mathf.Deg2Rad),direction);
  float reach=1-SmoothRange(range*.78f,range,distance);
  return Mathf.Max(near,cone*reach)*Strength;
 }
 static float SmoothRange(float a,float b,float value)=>Mathf.SmoothStep(0,1,Mathf.InverseLerp(a,b,value));
}
}
