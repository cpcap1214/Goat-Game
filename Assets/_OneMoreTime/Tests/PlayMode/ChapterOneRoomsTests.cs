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
public class ChapterOneRoomsTests {
 ChapterOneRoom chapter;
 RewindPlaytest replay;

 IEnumerator Load(int number,bool keepProgress=false,bool debug=false,bool fixture=false){
  if(!keepProgress){if(debug)MainGameProgress.BeginDebug();else MainGameProgress.StartOver();}
  string folder=fixture?"Tests/Fixtures/":"Scenes/";
  EditorSceneManager.LoadSceneInPlayMode("Assets/_OneMoreTime/"+folder+"Main_Room"+number.ToString("00")+".unity",new LoadSceneParameters(LoadSceneMode.Single));
  yield return null;
  chapter=Object.FindFirstObjectByType<ChapterOneRoom>();Assert.IsNotNull(chapter);
  chapter.AutoTick=false;chapter.room.AutoTick=false;chapter.replay.AutoTick=false;
  replay=chapter.replay;
  for(int i=0;i<300&&chapter.CurrentPhase==ChapterOneRoom.Phase.Entering;i++)chapter.Step(default,false);
  Assert.AreEqual(ChapterOneRoom.Phase.Playing,chapter.CurrentPhase,"Walk/lift entry did not finish");
 }
 void Record(int ticks=45){
  for(int i=0;i<13;i++)chapter.Step(default,true);
  Assert.AreEqual(RewindPlaytest.Mode.Recording,replay.State);
  chapter.Step(default,false);
  for(int i=0;i<ticks;i++)chapter.Step(default,false);
 }
 void Finish(){
  chapter.Step(default,true);chapter.Step(default,false);
  Assert.AreEqual(RewindPlaytest.Mode.Rewinding,replay.State);
  for(int i=0;i<200&&replay.State==RewindPlaytest.Mode.Rewinding;i++)chapter.Step(default,false);
  Assert.AreEqual(RewindPlaytest.Mode.Ready,replay.State);Assert.IsNotNull(replay.Memory);
 }
 [UnityTest] public IEnumerator FourRoomsHaveOneClockAuthoredCheckpointsAndOpenBoundedCameras(){
  foreach(int number in new[]{4,6,7,8}){
   yield return Load(number);
   Assert.AreEqual(number,chapter.roomNumber);Assert.AreEqual(number>=6,MainGameProgress.BasicReplayUnlocked);Assert.AreEqual(number>=6,chapter.ReplayAvailable);
   Assert.IsFalse(chapter.room.enabled);Assert.IsFalse(chapter.room.AutoTick);Assert.IsFalse(replay.AutoTick);
   Assert.Contains(chapter,replay.participants);
   if(number==4)Assert.IsEmpty(chapter.room.checkpoints,"The river layout does not add a statue");
   else Assert.IsNotEmpty(chapter.room.checkpoints);
   Assert.IsTrue(chapter.room.checkpoints.All(c=>c!=null&&c.stoneVisual!=null));
   Assert.That(replay.deathY,Is.LessThan(chapter.room.roomBounds.yMin));
   Assert.IsNotNull(chapter.room.shutter);Assert.IsTrue(chapter.room.shutter.Unlocked);
   chapter.SendMessage("LateUpdate");var camera=chapter.room.view;
   Assert.That(camera.orthographicSize*camera.aspect*2,Is.EqualTo(20).Within(.001f));
   chapter.room.player.ResetAt(new Vector2(chapter.room.roomBounds.center.x,chapter.room.roomBounds.yMax+3));
   chapter.SendMessage("LateUpdate");float top=camera.transform.position.y+camera.orthographicSize;
   Assert.That(top,Is.LessThanOrEqualTo(chapter.room.roomBounds.yMax+.001f));Assert.Greater(chapter.room.player.Position.y,top);
   var ceilingHits=Physics2D.RaycastAll(chapter.room.player.Position,Vector2.up,5,chapter.room.player.worldMask);
   Assert.IsEmpty(ceilingHits.Where(hit=>!hit.collider.isTrigger).ToArray(),"The camera boundary must not add a ceiling");
  }
 }
 [UnityTest] public IEnumerator FilterChangesBackgroundBridgesAndWorldRewindRestoresEra(){
  yield return Load(8,debug:true,fixture:true);Assert.IsNotEmpty(chapter.eraGroups);Assert.IsFalse(chapter.Past);
  var artwork=chapter.eraGroups.SelectMany(g=>g.artwork).ToArray();var colors=artwork.Select(a=>a.color).ToArray();
  foreach(var g in chapter.eraGroups)Assert.IsTrue(g.solids.All(c=>c.enabled!=g.activeInPast));
  Record(5);Assert.IsTrue(chapter.TrySwitchEra());Assert.IsTrue(chapter.Past);
  foreach(var g in chapter.eraGroups)Assert.IsTrue(g.solids.All(c=>c.enabled==g.activeInPast));
  foreach(var g in chapter.eraGroups.Where(g=>g.activeInPast))for(int i=0;i<g.artwork.Length;i++)Assert.AreEqual(g.activeColors[i],g.artwork[i].color,"Active bridges must recover their authored colors after Editor serialization");
  for(int i=0;i<30;i++)chapter.Step(default,false);
  Finish();Assert.IsFalse(chapter.Past,"Fast world rewind must restore the recorded starting era");
  foreach(var g in chapter.eraGroups)Assert.IsTrue(g.solids.All(c=>c.enabled!=g.activeInPast));
  for(int i=0;i<artwork.Length;i++)Assert.AreEqual(colors[i],artwork[i].color,"State-only rewind must not tint the entire room");
 }
 [UnityTest] public IEnumerator FilterRefusesToMaterializeABridgeInsideThePlayer(){
  yield return Load(8,debug:true,fixture:true);
  var group=chapter.eraGroups.First(g=>g.activeInPast);
  var solid=group.solids.OfType<BoxCollider2D>().First();
  Vector2 center=solid.transform.TransformPoint(solid.offset);
  replay.player.ResetAt(center-Vector2.up*replay.player.tuning.height*.5f);
  Assert.IsTrue(group.WouldEmbed(new Rect(replay.player.Position-new Vector2(replay.player.tuning.width*.5f,0),new Vector2(replay.player.tuning.width,replay.player.tuning.height))));
  Assert.IsFalse(chapter.TrySwitchEra());Assert.IsFalse(chapter.Past);Assert.IsFalse(solid.enabled);
 }
 [UnityTest] public IEnumerator CheckpointRetryKeepsSavedReplayAndRestoresSafeState(){
  yield return Load(7);
  var marker=chapter.room.checkpoints[0];
  replay.player.ResetAt((Vector2)marker.transform.position+Vector2.up*.01f);
  for(int i=0;i<6;i++)chapter.Step(default,false);
  Assert.IsTrue(marker.Saved);Assert.IsTrue(chapter.room.CheckpointSaved);
  Record();Finish();var memory=replay.Memory;
  replay.player.ResetAt(new Vector2(marker.transform.position.x,replay.deathY-1));chapter.Step(default,false);
  Assert.That(Vector2.Distance(replay.player.Position,marker.transform.position),Is.LessThan(.04f));Assert.AreSame(memory,replay.Memory);
  for(int i=0;i<80&&chapter.room.IsReviving;i++)chapter.Step(default,false);
  Assert.IsFalse(chapter.room.IsReviving);Assert.IsTrue(chapter.room.visual.renderer2D.enabled);
  replay.player.ResetAt(chapter.room.start);chapter.Step(new InputFrame{retry=true},false);
  Assert.That(Vector2.Distance(replay.player.Position,marker.transform.position),Is.LessThan(.04f));Assert.AreSame(memory,replay.Memory);
 }
 [UnityTest] public IEnumerator BothShutterAndReplayCanHoldTheReturnPlatformPressurePlate(){
  yield return Load(6,fixture:true);Assert.IsNotEmpty(chapter.plates);
  var platform=replay.platforms.Single(p=>p.plate!=null);var plate=platform.plate;var frozen=chapter.room.shutter.echo;
  var pose=new RecordedFrame{position=plate.transform.position,grounded=true,facing=1};
  frozen.Show(pose,true);frozen.support.enabled=false;chapter.Step(default,false);Assert.IsTrue(plate.Pressed);
  Assert.IsTrue(platform.Active);
  frozen.Clear();replay.echo.Show(pose,true);replay.echo.support.enabled=false;
  chapter.Step(default,false);Assert.IsTrue(plate.Pressed,"Replay must press plates through the same room clock");Assert.IsTrue(platform.Active);
  replay.echo.Clear();chapter.Step(default,false);Assert.IsFalse(plate.Pressed);
 }
 [UnityTest] public IEnumerator FloatingTimberReplaysBothDirectionsWithTheDebugUpgrade(){
  yield return Load(8,debug:true,fixture:true);
  Assert.IsTrue(replay.ReplayUpgradeUnlocked);Assert.IsTrue(MainGameProgress.ReplayUpgradeUnlocked);
  replay.player.ResetAt(chapter.room.start);for(int i=0;i<5;i++)chapter.Step(default,false);
  var source=replay.participants.OfType<CrateSpawner>().First();
  Assert.AreEqual(-2,source.water.velocity);Assert.AreEqual(10,source.spawnInterval);Assert.IsTrue(source.affectedByRewind);
  Record(75);var item=source.pool.First(c=>c.gameObject.activeSelf);var art=item.GetComponentsInChildren<SpriteRenderer>().First();var color=art.color;
  Finish();float initial=item.transform.position.x;
  chapter.Step(default,true);chapter.Step(default,false);for(int i=0;i<25;i++)chapter.Step(default,false);
  float forward=item.transform.position.x;Assert.Less(forward,initial-.5f);
  for(int i=0;i<10;i++)chapter.Step(default,false,false,false,true);Assert.That(item.transform.position.x,Is.EqualTo(forward).Within(.001f));
  chapter.Step(default,false,false,true);for(int i=0;i<15;i++)chapter.Step(default,false);
  Assert.Greater(item.transform.position.x,forward+.4f);Assert.AreEqual(color,art.color);
  yield return Load(8,true,fixture:true);Assert.IsTrue(replay.ReplayUpgradeUnlocked,"The debug upgrade survives a room reload");
 }
 [UnityTest] public IEnumerator WaterfallCavernFinishesChapterWithoutInventingAChapterTwoScene(){
  yield return Load(8);Assert.IsTrue(chapter.chapterFinale);
  var foreground=chapter.GetComponentsInChildren<WaterFeature>(true).Where(f=>f.foreground).ToArray();Assert.IsNotEmpty(foreground);
  Assert.IsTrue(foreground.All(f=>f.GetComponents<Collider2D>().Length==0));
  chapter.nextRoomScene="ChapterTwoNotImplemented";
  var area=chapter.exitArea;replay.player.ResetAt(new Vector2(area.center.x,area.yMin+2));
  Physics2D.SyncTransforms();
  for(int i=0;i<100&&chapter.CurrentPhase==ChapterOneRoom.Phase.Playing;i++)chapter.Step(default,false);
  Assert.AreEqual(ChapterOneRoom.Phase.Exiting,chapter.CurrentPhase);
  for(int i=0;i<100&&chapter.CurrentPhase==ChapterOneRoom.Phase.Exiting;i++)chapter.Step(default,false);
  Assert.AreEqual(ChapterOneRoom.Phase.Complete,chapter.CurrentPhase);Assert.AreEqual(1,chapter.Fade);
  Assert.AreEqual("Main_Room08",SceneManager.GetActiveScene().name);
 }
 [UnityTest] public IEnumerator RecordedApproachingLogCanBeBoardedReversedAndRiddenToTheNextBank(){
  yield return Load(8,debug:true,fixture:true);
  // Setup only: place the goat on the authored high-stream west bank.
  // Recording, boarding, upstream carry and the final jump all use the room clock.
  replay.player.ResetAt(new Vector2(55.5f,29.51f));Physics2D.SyncTransforms();
  for(int i=0;i<5;i++)chapter.Step(default,false);
  Assert.IsTrue(replay.ReplayUpgradeUnlocked);
  var source=replay.participants.OfType<CrateSpawner>().First();
  var log=source.pool.First(c=>c.gameObject.activeSelf);var deck=log.GetComponent<BoxCollider2D>();
  Record(0);
  for(int i=0;i<400&&log.transform.position.x>57.9f;i++)chapter.Step(default,false);
  Assert.AreEqual(RewindPlaytest.Mode.Recording,replay.State);Assert.That(log.transform.position.x,Is.InRange(57.8f,57.95f));
  Finish();Assert.Greater(log.transform.position.x,68.5f,"The saved route must include the far side of the authored high stream");

  chapter.Step(default,true);chapter.Step(default,false);
  for(int i=0;i<400&&log.transform.position.x>59.1f;i++)chapter.Step(default,false);
  // Walk off the bank onto the arriving log; do not teleport onto its collider.
  for(int i=0;i<80&&replay.player.GroundCollider!=deck;i++)chapter.Step(new InputFrame{move=1},false);
  Assert.AreEqual(ChapterOneRoom.Phase.Playing,chapter.CurrentPhase,"Boarding must stay above the drowning volume");
  Assert.AreSame(deck,replay.player.GroundCollider);Assert.IsTrue(replay.player.Grounded);
  for(int i=0;i<5;i++)chapter.Step(default,false);
  float boardX=replay.player.Position.x;
  chapter.Step(default,false,false,true);Assert.IsTrue(replay.PlaybackBackwards);
  for(int i=0;i<400&&log.transform.position.x<68.5f;i++)chapter.Step(default,false);
  Assert.That(replay.player.Position.x-boardX,Is.GreaterThan(9),"Reverse replay must carry the live goat across most of the stream");
  Assert.AreSame(deck,replay.player.GroundCollider);Assert.IsTrue(replay.player.Grounded);

  chapter.Step(new InputFrame{move=1,jump=true,held=true},false);
  bool landed=false;
  for(int i=0;i<100;i++){
   chapter.Step(new InputFrame{move=replay.player.Position.x<73?1:0,held=true},false);
   if(i>5&&replay.player.Grounded){landed=true;break;}
  }
  Assert.IsTrue(landed,"The ordinary three-unit jump must reach the far bank from the reversed log");
  Assert.AreEqual(ChapterOneRoom.Phase.Playing,chapter.CurrentPhase);
  Assert.That(replay.player.Position.x,Is.InRange(72,77));Assert.That(replay.player.Position.y,Is.EqualTo(30.5f).Within(.025f));
  Assert.AreNotSame(deck,replay.player.GroundCollider);
 }
 [UnityTest] public IEnumerator CollapsedBridgeRestoresItsDeckCountdownAndDebrisWithTheWorld(){
  yield return Load(8,fixture:true);
  var bridge=replay.participants.OfType<ChapterCrumbleBridge>().Single();
  var deck=bridge.deck;var position=new Vector2(deck.bounds.center.x,deck.bounds.max.y+.01f);
  replay.player.ResetAt(position);Physics2D.SyncTransforms();
  for(int i=0;i<3;i++)chapter.Step(default,false);
  Assert.IsTrue(bridge.Armed);Assert.IsFalse(bridge.Collapsed);
  Record(0);float remaining=bridge.Remaining;Assert.Greater(remaining,.1f);
  replay.player.ResetAt(chapter.room.start);
  for(int i=0;i<60;i++)chapter.Step(default,false);
  Assert.IsTrue(bridge.Collapsed);Assert.IsFalse(deck.enabled);
  Finish();Assert.IsFalse(bridge.Collapsed);Assert.IsTrue(deck.enabled);
  Assert.That(bridge.Remaining,Is.EqualTo(remaining).Within(.05f));
  Assert.IsTrue(bridge.artwork.All(a=>a.color.a>.9f),"Rewind must restore the bridge pieces as well as collision");
 }
}
}
#endif
