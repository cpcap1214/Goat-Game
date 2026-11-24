using UnityEngine;
namespace OneMoreTime {
// The replay clock advances pooled decks; this participant only handles press edges/cooldown.
public sealed class PlatformDispatcher:RewindParticipant {
 public PressurePlate activationPlate;
 public RoomPlatform[] pool=new RoomPlatform[0];
 [Min(0)]public float cooldown=4;
 public float Remaining{get;private set;}
 public override bool HasTransformHistory=>false;
 bool plateWasPressed;
 sealed class State {public float remaining;public bool pressed;}
 public override void Tick(float dt){
  Remaining=Mathf.Max(0,Remaining-dt);if(Remaining<.0001f)Remaining=0;
  bool pressed=activationPlate!=null&&activationPlate.Pressed;
  if(pressed&&!plateWasPressed&&Remaining<=0){
   foreach(var platform in pool)if(platform!=null&&platform.Launch()){Remaining=cooldown;break;}
  }
  plateWasPressed=pressed;
 }
 public override object Capture()=>new State{remaining=Remaining,pressed=plateWasPressed};
 public override void Restore(object value){var state=(State)value;Remaining=state.remaining;plateWasPressed=state.pressed;}
}
}
