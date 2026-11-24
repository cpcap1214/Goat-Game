#if UNITY_EDITOR
using System.Collections;
using System.Linq;
using NUnit.Framework;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

namespace OneMoreTime.Tests {
[Category("ChapterOneRegression")]
public class StatueRevivalTests {
 MainRoom room;
 ChapterOneRoom chapter;
 RewindPlaytest replay;

 IEnumerator Load(int number){
  MainGameProgress.StartOver();
  EditorSceneManager.LoadSceneInPlayMode("Assets/_OneMoreTime/Scenes/Main_Room"+number.ToString("00")+".unity",new LoadSceneParameters(LoadSceneMode.Single));
  yield return null;
  room=Object.FindFirstObjectByType<MainRoom>();Assert.IsNotNull(room);room.AutoTick=false;
  chapter=Object.FindFirstObjectByType<ChapterOneRoom>();replay=chapter!=null?chapter.replay:null;
  if(chapter!=null){chapter.AutoTick=false;replay.AutoTick=false;}
  for(int i=0;i<300&&(chapter!=null?chapter.CurrentPhase==ChapterOneRoom.Phase.Entering:room.Phase==MainRoom.RoomPhase.Entering);i++)Step();
  Assert.IsFalse(room.IsReviving,"Entry is not a death or retry");
 }
 void Step(InputFrame command=default,bool held=false){
  if(chapter!=null)chapter.Step(command,held);else room.Step(command,.02f);
  Physics2D.SyncTransforms();
 }
 void SaveCheckpoint(Vector2 position){
  room.player.ResetAt(position+Vector2.up*.01f);Physics2D.SyncTransforms();Step();
  Assert.IsTrue(room.CheckpointSaved);Assert.IsFalse(room.IsReviving,"Saving a checkpoint must not play its respawn animation");
 }
 void FinishRevival(){
  Assert.IsTrue(room.IsReviving);
  Vector2 spawn=room.player.Position;
  for(int i=0;i<80&&room.IsReviving;i++){
   Step();Assert.That(Vector2.Distance(room.player.Position,spawn),Is.LessThan(.001f),"The visual breakout must not move the motor off its safe spawn");
  }
  Assert.IsFalse(room.IsReviving);Assert.IsTrue(room.visual.renderer2D.enabled);
  Assert.AreEqual(Color.white,room.visual.renderer2D.color);
 }
 void Record(){
  for(int i=0;i<13;i++)Step(default,true);
  Step();Assert.AreEqual(RewindPlaytest.Mode.Recording,replay.State);
  for(int i=0;i<15;i++)Step();
 }
 void FinishRewind(){
  Assert.AreEqual(RewindPlaytest.Mode.Rewinding,replay.State);
  for(int i=0;i<200&&replay.State==RewindPlaytest.Mode.Rewinding;i++){
   Assert.IsFalse(room.IsReviving,"World rewind must not be mistaken for a respawn");Step();
  }
  Assert.AreEqual(RewindPlaytest.Mode.Ready,replay.State);Assert.IsNotNull(replay.Memory);Assert.IsFalse(room.IsReviving);
 }

 [Test] public void PedestalVerticesRemainInsideTheirSourceRectForFractionalPixelScales(){
  foreach(bool painted in new[]{false,true})foreach(float ppu in new[]{1.443f,170f,209f}){
   var root=new GameObject("Pedestal geometry fixture");
   var source=Sprite.Create(Texture2D.whiteTexture,new Rect(0,0,2,2),new Vector2(.50717705f,.21212122f),ppu,0,SpriteMeshType.FullRect);
   try{
    var renderer=root.AddComponent<SpriteRenderer>();renderer.sprite=source;
    var stone=root.AddComponent<StoneCheckpointVisual>();stone.target=renderer;stone.frames=new[]{source};stone.hasPedestal=painted;
    stone.LeaveBase();var pedestal=renderer.sprite;
    Assert.AreNotSame(source,pedestal);
    foreach(var vertex in pedestal.vertices){
     Vector2 pixel=vertex*pedestal.pixelsPerUnit+pedestal.pivot;
     Assert.That(pixel.x,Is.GreaterThanOrEqualTo(-.0001f).And.LessThanOrEqualTo(pedestal.rect.width+.0001f));
     Assert.That(pixel.y,Is.GreaterThanOrEqualTo(-.0001f).And.LessThanOrEqualTo(pedestal.rect.height+.0001f));
    }
    float expectedWidth=source.rect.width/ppu*(painted?1:.8f);
    float expectedHeight=painted?source.rect.height/ppu*.14f:expectedWidth*.16f;
    var vertices=pedestal.vertices;
    Assert.AreEqual(expectedWidth,vertices.Max(v=>v.x)-vertices.Min(v=>v.x),.00001f,"Pixel-space geometry must preserve the intended world width");
    Assert.AreEqual(expectedHeight,vertices.Max(v=>v.y)-vertices.Min(v=>v.y),.00001f,"The pedestal must have its intended low height, not a second PPU conversion");
    LogAssert.NoUnexpectedReceived();
   }finally{Object.DestroyImmediate(root);Object.DestroyImmediate(source);}
  }
 }

 [UnityTest] public IEnumerator LegacyStatueRevivesAfterDeathAndRestoresTheLivingGoat(){
  yield return Load(1);
  Step(new InputFrame{retry=true});Assert.IsFalse(room.IsReviving,"Retry before finding a statue has no statue to emerge from");
  Assert.IsEmpty(room.checkpoints,"This test intentionally covers the legacy single-statue room");
  SaveCheckpoint(room.checkpoint);
  int colliders=room.GetComponentsInChildren<Collider2D>(true).Length;
  room.player.ResetAt(new Vector2(room.checkpoint.x,room.deathY-1));Step();
  Assert.AreEqual(1,room.Deaths);Assert.IsTrue(room.IsReviving);Assert.IsNotNull(room.Revival);
  Assert.That(Vector2.Distance(room.player.Position,room.checkpoint),Is.LessThan(.001f));
  Assert.AreEqual(colliders,room.GetComponentsInChildren<Collider2D>(true).Length,"Stone fragments are visual, never new collision surfaces");
  FinishRevival();
  float x=room.player.Position.x;for(int i=0;i<8;i++)Step(new InputFrame{move=1});
  Assert.Greater(room.player.Position.x,x+.1f,"Movement must return when the emergence finishes");
  Assert.IsTrue(room.CheckpointSaved);Assert.Greater(room.statueEyes[0].color.g,.9f);
  Assert.IsFalse(room.statueEyes[0].enabled,"The eye must not float over the broken pedestal");
  Assert.IsTrue(room.statueEyes[0].transform.parent.GetComponentInChildren<StoneCheckpointVisual>().BaseOnly);
 }

 [UnityTest] public IEnumerator NarrowRoomThreeCheckpointKeepsItsSpawnAndRepeatedRevivesDoNotAccumulateDebris(){
  yield return Load(3);
  var marker=room.checkpoints[0];SaveCheckpoint(marker.transform.position);
  Vector2 spawn=room.checkpoint;
  int colliders=room.GetComponentsInChildren<Collider2D>(true).Length;
  int activeRenderers=-1;
  int warmedObjectCount=0;
  for(int cycle=0;cycle<4;cycle++){
   room.player.ResetAt(room.start);Step(new InputFrame{retry=true});
   Assert.That(Vector2.Distance(room.player.Position,spawn),Is.LessThan(.001f));
   FinishRevival();
   Assert.That(Vector2.Distance(room.player.Position,spawn),Is.LessThan(.001f),"The narrow ledge cannot afford a sideways exit from its statue");
   Assert.AreEqual(colliders,room.GetComponentsInChildren<Collider2D>(true).Length);
   int visible=room.GetComponentsInChildren<SpriteRenderer>().Count(r=>r.enabled);
   if(cycle==0)activeRenderers=visible;else Assert.AreEqual(activeRenderers,visible,"Repeated respawns must leave only the same pedestal, never another shell or eye");
   Assert.IsTrue(marker.stoneVisual.BaseOnly);
   // The first revival may add its controller; deferred destruction must clear temporary art.
   yield return null;
   int count=room.GetComponentsInChildren<Transform>(true).Length;
   if(cycle==0)warmedObjectCount=count;else Assert.AreEqual(warmedObjectCount,count,"Repeated deaths must not accumulate effect objects");
  }
  Assert.IsTrue(marker.Saved);Assert.IsTrue(room.visual.renderer2D.enabled);
 }

 [UnityTest] public IEnumerator RespawnFacesTheStatueAndLeavesItsPedestalAcrossRepeatedDeaths(){
  yield return Load(6);
  var checkpoint=room.checkpoints[0];var stone=checkpoint.stoneVisual;
  SaveCheckpoint(checkpoint.transform.position);
  Sprite fullStatue=stone.IntactSprite;
  for(int cycle=0;cycle<3;cycle++){
   // Right-facing art can be mirrored on the renderer, the hierarchy, or both.
   stone.target.flipX=cycle!=1;
   checkpoint.transform.localScale=new Vector3(cycle==0?1:-1,1,1);
   int expected=cycle==2?1:-1;
   room.player.Step(new InputFrame{move=-expected},.02f);
   room.player.ResetAt(room.start);
   Step(new InputFrame{retry=true});
   Assert.IsTrue(room.IsReviving);Assert.AreEqual(expected,room.player.Facing);
   Assert.AreEqual(expected<0,room.visual.renderer2D.flipX);
   FinishRevival();
   Assert.AreEqual(expected,room.player.Facing,"The first ordinary pose must retain the statue's facing");
   Assert.IsTrue(stone.BaseOnly);Assert.IsTrue(stone.target.enabled);
   Assert.AreNotSame(fullStatue,stone.target.sprite);
   var vertices=stone.target.sprite.vertices;
   Assert.Less(vertices.Max(v=>v.y)-vertices.Min(v=>v.y),fullStatue.bounds.size.y*.25f,"Only the low painted base may remain visible");
   var pedestal=stone.target.sprite;stone.Tick(10);Step();
   Assert.AreSame(pedestal,stone.target.sprite,"Idle checkpoint animation must not reconstruct the goat");
  }
 }

 [UnityTest] public IEnumerator BrokenBasePersistsWhenAnotherCheckpointIsActivatedAndRevivalIsCancelled(){
  yield return Load(7);
  var first=room.checkpoints[0];var second=room.checkpoints[1];
  SaveCheckpoint(first.transform.position);Step(new InputFrame{retry=true});FinishRevival();
  Assert.IsTrue(first.stoneVisual.BaseOnly);Assert.IsFalse(second.stoneVisual.BaseOnly);
  SaveCheckpoint(second.transform.position);
  Assert.IsTrue(first.stoneVisual.BaseOnly,"Saving another statue cannot rebuild a statue already used for revival");
  Step(new InputFrame{retry=true});Assert.IsTrue(room.IsReviving);
  float progress=room.Revival.Progress;
  replay.SetPaused(true);for(int i=0;i<20;i++)Step();
  Assert.AreEqual(progress,room.Revival.Progress);Assert.IsFalse(second.stoneVisual.BaseOnly,"Pause freezes the pre-break shell");
  replay.SetPaused(false);
  room.Revival.Cancel();
  Assert.IsFalse(room.IsReviving);Assert.IsTrue(first.stoneVisual.BaseOnly);Assert.IsTrue(second.stoneVisual.BaseOnly);
  Assert.IsTrue(second.stoneVisual.target.enabled,"Cancelling presentation after respawn must still leave the base");
  Assert.IsTrue(room.visual.renderer2D.enabled);
  Step(new InputFrame{retry=true});FinishRevival();Assert.IsTrue(second.stoneVisual.BaseOnly);
 }

 [UnityTest] public IEnumerator PedestalRemainsCroppedAfterRenderedFramesAndCheckpointAnimationUpdates(){
  yield return Load(6);var marker=room.checkpoints[0];var stone=marker.stoneVisual;
  SaveCheckpoint(marker.transform.position);var source=stone.IntactSprite;
  Step(new InputFrame{retry=true});FinishRevival();
  var pedestal=stone.target.sprite;var rect=pedestal.rect;
  Assert.Less(rect.height,source.rect.height*.25f,"The source rectangle itself must exclude the goat, even if Unity rebuilds its mesh");
  for(int frame=0;frame<4;frame++){
   stone.Tick(1);yield return null;
   Assert.AreSame(pedestal,stone.target.sprite);Assert.AreEqual(rect,stone.target.sprite.rect);
   var vertices=stone.target.sprite.vertices;
   Assert.Less(vertices.Max(v=>v.y)-vertices.Min(v=>v.y),source.bounds.size.y*.25f,"Renderer updates must not regenerate a complete statue");
  }
  yield return Load(1);SaveCheckpoint(room.checkpoint);Step(new InputFrame{retry=true});FinishRevival();
  stone=room.statueEyes[0].transform.parent.GetComponentInChildren<StoneCheckpointVisual>();
  pedestal=stone.target.sprite;
  for(int frame=0;frame<3;frame++){
   stone.Tick(1);yield return null;
   Assert.AreSame(pedestal,stone.target.sprite);
   var vertices=pedestal.vertices;float width=vertices.Max(v=>v.x)-vertices.Min(v=>v.x),height=vertices.Max(v=>v.y)-vertices.Min(v=>v.y);
   Assert.AreEqual(.16f,height/width,.0001f,"The legacy pedestal must remain a low stone block after rendering");
   Assert.IsFalse(room.statueEyes[0].enabled);
  }
 }

 [UnityTest] public IEnumerator ChapterRetryAndDeathReviveAtTheStatueWithoutErasingSavedReplay(){
  yield return Load(6);SaveCheckpoint(room.checkpoints[0].transform.position);
  Record();replay.FinishRecording();FinishRewind();var memory=replay.Memory;
  room.player.ResetAt(room.start);Step(new InputFrame{retry=true});
  Assert.IsTrue(room.IsReviving);Assert.AreSame(memory,replay.Memory);
  Assert.That(Vector2.Distance(room.player.Position,room.checkpoint),Is.LessThan(.025f));
  float progress=room.Revival.Progress;
  replay.SetPaused(true);for(int i=0;i<80;i++)Step(new InputFrame{move=1,jump=true,retry=true},true);
  Assert.AreEqual(progress,room.Revival.Progress,"Pausing must freeze the entire revival");
  replay.SetPaused(false);FinishRevival();
  room.player.ResetAt(new Vector2(room.checkpoint.x,replay.deathY-1));Step();
  Assert.IsTrue(room.IsReviving,"Falling below the death plane uses the same statue revival as retry");
  Assert.AreSame(memory,replay.Memory);Assert.AreEqual(RewindPlaytest.Mode.Ready,replay.State);
  Assert.That(Vector2.Distance(room.player.Position,room.checkpoint),Is.LessThan(.025f));
  FinishRevival();Assert.AreSame(memory,replay.Memory);
 }

 [UnityTest] public IEnumerator RecordingCompletionRecordingDeathAndCheckpointAnchorCaptureNeverRevive(){
  yield return Load(7);SaveCheckpoint(room.checkpoints[0].transform.position);
  // The room temporarily moves the motor to its checkpoint to capture a safe retry anchor.
  // Saving a second statue while moving exercises that path, not just direct SetRetryAnchor.
  room.player.ResetAt((Vector2)room.checkpoints[1].transform.position+Vector2.up*.01f);
  Step(new InputFrame{move=1});Assert.IsTrue(room.checkpoints[1].Saved);Assert.IsFalse(room.IsReviving);
  replay.SetRetryAnchor();Assert.IsFalse(room.IsReviving);
  Record();replay.FinishRecording();FinishRewind();
  Record();room.player.ResetAt(new Vector2(room.checkpoint.x,replay.deathY-1));Step();
  Assert.IsFalse(room.IsReviving,"Death while recording saves/rewinds the recording instead of performing a statue retry");
  FinishRewind();
  Record();Step(new InputFrame{retry=true});Assert.IsFalse(room.IsReviving);
  FinishRewind();
 }

 [UnityTest] public IEnumerator RevivalConsumesTheRoomClockAndIgnoresActionsUntilTheGoatIsReady(){
  yield return Load(8);SaveCheckpoint(room.checkpoints[0].transform.position);
  Step(new InputFrame{retry=true});Assert.IsTrue(room.IsReviving);
  var source=replay.participants.OfType<CrateSpawner>().First();
  var log=source.pool.First(c=>c.gameObject.activeSelf);Vector3 logPosition=log.transform.position;
  Vector2 spawn=room.player.Position;bool past=chapter.Past;
  for(int i=0;i<15;i++){
   chapter.Step(new InputFrame{move=1,jump=true,freeze=true,retry=true},true,true,true,true);
   Assert.IsTrue(room.IsReviving);Assert.AreEqual(spawn,room.player.Position);
   Assert.AreEqual(logPosition,log.transform.position,"Floating obstacles must share the paused revival clock");
   Assert.AreEqual(past,chapter.Past,"Filter edges during emergence must not switch collision state");
   Assert.AreEqual(RewindPlaytest.Mode.Idle,replay.State,"Holding record during emergence must not start a recording");
  }
  Assert.Greater(room.Revival.Progress,0,"Ignored retry input must not restart the revival each tick");
  FinishRevival();Assert.AreEqual(logPosition,log.transform.position,"The completion tick must also be consumed before ordinary simulation resumes");
  Step();Assert.That(Vector3.Distance(log.transform.position,logPosition),Is.GreaterThan(.01f),"Floating obstacles resume on the next ordinary tick");
 }
}
}
#endif
