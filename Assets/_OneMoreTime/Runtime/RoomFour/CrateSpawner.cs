using UnityEngine;
namespace OneMoreTime {
public sealed class CrateSpawner:RewindParticipant {
 [Min(.1f)]public float spawnInterval=10;
 public bool spawningEnabled=true;
 [Tooltip("Optional: produce one floating item per plate press. Holding the plate never repeats; released items keep moving.")] public PressurePlate activationPlate;
 [Min(0),Tooltip("Minimum seconds between successful plate releases. Presses during cooldown are ignored; no gameplay indicator.")] public float plateCooldown;
 public float initialHorizontalVelocity=-2;
 [Min(1)]public int maximumActiveCrates=6;
 public Transform spawnPoint;public WaterCurrent2D water;public CrateDisposalTrigger disposal;
 public FloatingObject2D[] pool=new FloatingObject2D[0];
 public float Remaining{get;private set;}public int ActiveCount{get{int n=0;foreach(var c in pool)if(c.gameObject.activeSelf)n++;return n;}}
 bool plateWasPressed;
 // Physics trigger callbacks must respect the same waiting state as Tick.
 public bool AwaitingReplay=>affectedByRewind&&GetComponentInParent<RewindPlaytest>() is RewindPlaytest replay&&replay.AwaitingReplay;
 sealed class State {public float remaining;public bool plateWasPressed;public FloatingObject2D.State[] crates;}
 public override void Tick(float dt){
  if(!spawningEnabled)return;
  if(activationPlate!=null){
   Remaining=Mathf.Max(0,Remaining-dt);
   bool pressed=activationPlate.Pressed;
   if(pressed&&!plateWasPressed&&Remaining<=0&&Spawn())Remaining=plateCooldown;
   plateWasPressed=pressed;
  }else{
   Remaining-=dt;
   if(Remaining<=0){Spawn();Remaining=spawnInterval;}
  }
  foreach(var c in pool)if(c.gameObject.activeSelf){c.Tick(dt);if(disposal!=null)disposal.Check(c);}
 }
 bool Spawn(){if(ActiveCount>=maximumActiveCrates)return false;foreach(var c in pool)if(!c.gameObject.activeSelf){c.water=water;c.Restore(new FloatingObject2D.State{active=true,position=spawnPoint.position,velocity=new Vector2(initialHorizontalVelocity,0)});return true;}return false;}
 public override void HoldFrame(){foreach(var c in pool)c.HoldFrame();}
 public void Dispose(FloatingObject2D crate){if(AwaitingReplay)return;foreach(var c in pool)if(c==crate){c.gameObject.SetActive(false);return;}}
 public void RestorePlayback(object value,bool carry){var s=(State)value;Remaining=s.remaining;plateWasPressed=s.plateWasPressed;for(int i=0;i<pool.Length;i++)pool[i].RestorePlayback(s.crates[i],carry);}
 public override object Capture(){var s=new State{remaining=Remaining,plateWasPressed=plateWasPressed,crates=new FloatingObject2D.State[pool.Length]};for(int i=0;i<pool.Length;i++)s.crates[i]=pool[i].Capture();return s;}
 public override void Restore(object value){var s=(State)value;Remaining=s.remaining;plateWasPressed=s.plateWasPressed;for(int i=0;i<pool.Length;i++)pool[i].Restore(s.crates[i]);}
}
}
