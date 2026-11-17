using UnityEngine;
namespace OneMoreTime {
public sealed class RoomMechanisms:MonoBehaviour {
 public MainRoom room;public PressurePlate[] plates=new PressurePlate[0];public RoomPlatform[] platforms=new RoomPlatform[0];
 public string Hint=>room.player.Position.y<12?"Leave a frozen goat on the plate to run the vertical platform":room.player.Position.y<28?"The timer stays full while the plate is held":"Leave a frozen goat on the final plate, then enter the lift";
 public void Evaluate(){foreach(var p in plates)p.Evaluate(room.player,room.shutter.echo);}
 public void Tick(float dt){Evaluate();foreach(var p in platforms)p.Tick(dt);Physics2D.SyncTransforms();}
 public void ResetState(){foreach(var p in platforms)p.ResetState();Tick(0);}
}
}
