using UnityEngine;

namespace OneMoreTime {
// The statue is a child of the deck. RoomPlatform remains its only movement clock;
// this component keeps the checkpoint/revival anchor in step with that hierarchy.
public sealed class MovingRoomCheckpoint : MonoBehaviour {
 public MainRoom room;
 public RewindPlaytest replay;
 public RoomPlatform platform;
 public RoomCheckpoint checkpoint;
 public bool ResetPending {get;private set;}
 RewindPlaytest subscribedReplay;

 void Awake(){Initialize();}
 void OnEnable(){Initialize();}
 void OnDisable(){Unsubscribe();}
 public void Initialize(){
  // Initializing the deck captures its authored home, before any rewind can move it.
  if(platform!=null)platform.CaptureRewind();
  if(!Application.isPlaying||replay==subscribedReplay)return;
  Unsubscribe();subscribedReplay=replay;
  if(subscribedReplay==null)return;
  subscribedReplay.Died+=MarkDeath;
  subscribedReplay.RetryRestored+=ResetAfterDeath;
  subscribedReplay.WorldRewindFinished+=FinishDeathRewind;
 }
 void Unsubscribe(){
  if(subscribedReplay==null)return;
  subscribedReplay.Died-=MarkDeath;
  subscribedReplay.RetryRestored-=ResetAfterDeath;
  subscribedReplay.WorldRewindFinished-=FinishDeathRewind;
  subscribedReplay=null;
 }
 public void SyncCheckpoint(){
  Initialize();
  if(room!=null&&checkpoint!=null&&room.ActiveCheckpoint==checkpoint)room.checkpoint=checkpoint.transform.position;
 }
 // Water and pause-reset deaths are owned by the room; falling deaths arrive
 // through RewindPlaytest.Died. Defer until rewind/retry finishes restoring state.
 public void MarkDeath(){ResetPending=true;}
 public void FinishDeathRewind(){if(ResetPending)ResetAfterDeath();}
 public void ResetAfterDeath(){
  if(platform==null)return;
  platform.ResetState();platform.plate?.ResetState();
  if(platform.requiredPlates!=null)foreach(var plate in platform.requiredPlates)if(plate!=null)plate.ResetState();
  SyncCheckpoint();
  // A selected moving statue must revive on its reset deck, not at an old
  // recorded position in mid-air. Other checkpoints keep their original retry.
  if(room!=null&&checkpoint!=null&&room.ActiveCheckpoint==checkpoint&&room.player!=null)room.player.ResetAt(room.checkpoint);
  ResetPending=false;Physics2D.SyncTransforms();
 }
}
}
