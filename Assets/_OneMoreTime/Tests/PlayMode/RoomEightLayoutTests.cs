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
public class RoomEightLayoutTests {
 ChapterOneRoom chapter;
 RewindPlaytest replay;
 PlayerMotor2D Player=>chapter.room.player;
 RoomPlatform Carrier=>chapter.movingCheckpoint.platform;
 RoomCheckpoint Statue=>chapter.movingCheckpoint.checkpoint;
 CrateSpawner[] Sources=>replay.participants.OfType<CrateSpawner>().OrderBy(s=>s.spawnPoint.position.x).ToArray();
 IEnumerator Load(){
  MainGameProgress.StartOver();
  EditorSceneManager.LoadSceneInPlayMode("Assets/_OneMoreTime/Scenes/Main_Room08.unity",new LoadSceneParameters(LoadSceneMode.Single));
  yield return null;
  chapter=Object.FindFirstObjectByType<ChapterOneRoom>();Assert.IsNotNull(chapter);replay=chapter.replay;
  chapter.AutoTick=chapter.room.AutoTick=replay.AutoTick=false;
  for(int i=0;i<200&&chapter.CurrentPhase==ChapterOneRoom.Phase.Entering;i++)Act();
  Assert.AreEqual(ChapterOneRoom.Phase.Playing,chapter.CurrentPhase);
 }
 void Act(float move=0,bool jump=false,bool record=false,bool shutter=false){chapter.Step(new InputFrame{move=move,jump=jump,held=true,freeze=shutter},record);Physics2D.SyncTransforms();}
 void Wait(int ticks){for(int i=0;i<ticks;i++)Act();}
 // Mechanism setups are explicit placements, not a claim of full-route traversal.
 void Place(Vector2 feet){Player.ResetAt(feet+Vector2.up*.01f);Physics2D.SyncTransforms();Wait(4);}
 void GroundAt(Vector2 feet){var s=Player.CaptureRewind();s.position=feet;s.velocity=Vector2.zero;s.grounded=true;s.onEcho=false;s.ground=null;Player.RestoreRewind(s);Physics2D.SyncTransforms();}
 void Near(Vector2 expected,Vector2 actual,float tolerance=.015f){Assert.Less(Vector2.Distance(expected,actual),tolerance);}
 void Record(int ticks=30){for(int i=0;i<13;i++)Act(record:true);Act();Assert.AreEqual(RewindPlaytest.Mode.Recording,replay.State);Wait(ticks);}
 void Finish(){Act(record:true);Act();FinishRewind();}
 void FinishRewind(){for(int i=0;i<150&&replay.State==RewindPlaytest.Mode.Rewinding;i++)Act();Assert.AreEqual(RewindPlaytest.Mode.Ready,replay.State);Assert.IsTrue(replay.AwaitingReplay);}
 void Play(){Act(record:true);Act();Assert.IsFalse(replay.AwaitingReplay);}
 void StartCarrier(){
  var companion=Carrier.requiredPlates[1];chapter.room.shutter.echo.Show(new RecordedFrame{position=companion.transform.position,facing=1,state=0,frame=0,grounded=true},true);
  GroundAt(Carrier.plate.transform.position);Act();
  Assert.IsTrue(chapter.platformReveal.Revealing);
  for(int i=0;i<120&&chapter.platformReveal.Revealing;i++)Act();
  Assert.IsFalse(chapter.platformReveal.Revealing);Act();Assert.IsTrue(Carrier.JourneyStarted);
 }
 void BoardStatue(){Place(new Vector2(Statue.transform.position.x,Carrier.solid.bounds.max.y));Assert.AreSame(Carrier.solid,Player.GroundCollider);Assert.AreSame(Statue,chapter.room.ActiveCheckpoint);}
 void FinishRevival(){Assert.IsTrue(chapter.room.IsReviving);for(int i=0;i<100&&chapter.room.IsReviving;i++)Act();Assert.IsFalse(chapter.room.IsReviving);}
 void Die(string kind){
  if(kind=="fall"){Player.ResetAt(new Vector2(4,replay.deathY-1));Physics2D.SyncTransforms();Act();}
  else if(kind=="water"){
   var water=chapter.waters.First();var box=water.GetComponent<BoxCollider2D>();Player.ResetAt((Vector2)box.bounds.center-Vector2.up*.3f);Physics2D.SyncTransforms();Act();
   for(int i=0;i<150&&chapter.CurrentPhase==ChapterOneRoom.Phase.Drowning;i++)Act();
  }else{replay.SetPaused(true);replay.ResetFromPause();Physics2D.SyncTransforms();}
 }

 [UnityTest]public IEnumerator OpeningJumpsUpOutsideTheBankThenLandsWithoutIntersectingWalls(){
  MainGameProgress.StartOver();EditorSceneManager.LoadSceneInPlayMode("Assets/_OneMoreTime/Scenes/Main_Room08.unity",new LoadSceneParameters(LoadSceneMode.Single));yield return null;
  chapter=Object.FindAnyObjectByType<ChapterOneRoom>();replay=chapter.replay;chapter.AutoTick=chapter.room.AutoTick=replay.AutoTick=false;
  float peak=Player.Position.y;int frames=0;
  while(chapter.CurrentPhase==ChapterOneRoom.Phase.Entering&&frames++<150){
   Act();peak=Mathf.Max(peak,Player.Position.y);
   var size=new Vector2(Player.tuning.width-.04f,Player.tuning.height-.04f);
   var overlaps=Physics2D.OverlapBoxAll(Player.Position+Vector2.up*(Player.tuning.height*.5f),size,0,Player.worldMask);
   Assert.IsEmpty(overlaps.Where(c=>!c.isTrigger&&c.bounds.max.y>Player.Position.y+.001f).ToArray(),"The entrance arc must stay clear of the bank and walls at "+Player.Position+" frame "+frames);
  }
  Assert.AreEqual(ChapterOneRoom.Phase.Playing,chapter.CurrentPhase);Near(chapter.room.start,Player.Position);Assert.Greater(peak,chapter.room.start.y+1);
  foreach(string mistaken in new[]{"08 Low bank red remnant","08 Upper stream red remnant","08 Upper stream orange remnant"})Assert.IsNull(GameObject.Find(mistaken));
  var crossing=GameObject.Find("08 Lower red crossing");Assert.IsNotNull(crossing,"The red span around x=20 is a real bridge, not a whirlpool arrow.");
  var deck=crossing.GetComponent<BoxCollider2D>();Assert.IsTrue(deck.enabled);Assert.IsNotNull(crossing.GetComponent<OneWayPlatform>());
  Assert.That(deck.bounds.min.x,Is.EqualTo(19.7f).Within(.001f));Assert.That(deck.bounds.max.x,Is.EqualTo(24.7f).Within(.001f));Assert.That(deck.bounds.max.y,Is.EqualTo(2.9f).Within(.001f));
 }

 [UnityTest]public IEnumerator EitherPlateAloneCannotStartTheStatueButBothTogetherStartItsOneWayJourney(){
  yield return Load();Assert.AreEqual(2,Carrier.requiredPlates.Length);var second=Carrier.requiredPlates[1];
  Assert.IsNotNull(Carrier.counter);Assert.IsTrue(Carrier.counter.gameObject.activeInHierarchy);Assert.AreEqual("0/2",Carrier.counter.text);Assert.AreEqual(Color.white,Carrier.counter.color);
  GroundAt(second.transform.position);Wait(20);Assert.IsFalse(Carrier.JourneyStarted);Assert.IsFalse(chapter.platformReveal.Shown);
  Assert.AreEqual("1/2",Carrier.counter.text);Assert.AreEqual(Color.white,Carrier.counter.color);
  GroundAt(Carrier.plate.transform.position);Act();Assert.IsTrue(chapter.platformReveal.Revealing);
  for(int i=0;i<120&&chapter.platformReveal.Revealing;i++)Act();Wait(20);Assert.IsFalse(Carrier.JourneyStarted);
  Assert.AreEqual("1/2",Carrier.counter.text);Assert.AreEqual(Color.white,Carrier.counter.color);
  chapter.room.shutter.echo.Show(new RecordedFrame{position=second.transform.position,facing=1,grounded=true},true);Act();Assert.IsTrue(Carrier.JourneyStarted);
  Assert.AreEqual("2/2",Carrier.counter.text);Assert.AreEqual((Color32)new Color(.4f,1,.5f),(Color32)Carrier.counter.color);
  chapter.room.shutter.echo.Clear();GroundAt(chapter.room.start);float progress=Carrier.JourneyProgress;Wait(50);Assert.Greater(Carrier.JourneyProgress,progress);
 }

 [UnityTest]public IEnumerator BothCrateStreamsRunDownstreamAtTwoUnitsEverySixSecondsAndLoopsTakeSixSeconds(){
  yield return Load();var sources=Sources;Assert.AreEqual(2,sources.Length);
  foreach(var source in sources){Assert.AreEqual(-2,source.initialHorizontalVelocity);Assert.AreEqual(-2,source.water.velocity);Assert.AreEqual(6,source.spawnInterval);Assert.IsTrue(source.affectedByRewind);Assert.IsTrue(source.disposal.sinkBeforeDisposal);}
  Act();var crates=sources.Select(s=>s.pool.Single(c=>c.gameObject.activeSelf)).ToArray();var positions=crates.Select(c=>c.transform.position).ToArray();
  Wait(50);for(int i=0;i<crates.Length;i++)Assert.That(crates[i].transform.position.x-positions[i].x,Is.EqualTo(-2).Within(.002f));
  Wait(240);foreach(var source in sources)Assert.AreEqual(1,source.ActiveCount,"No second crate before the six-second interval.");
  Wait(12);foreach(var source in sources){Assert.AreEqual(2,source.ActiveCount);Assert.IsTrue(source.pool.Any(c=>c.gameObject.activeSelf&&!c.Sinking&&Mathf.Abs(c.transform.position.x-source.spawnPoint.position.x)<.15f));}
  foreach(var crate in crates)foreach(var art in crate.GetComponentsInChildren<SpriteRenderer>())Assert.AreEqual(Color.white,art.color);
  var loops=replay.platforms.Where(p=>p.motion==RoomPlatform.Motion.ClosedRoute).ToArray();Assert.AreEqual(2,loops.Length);foreach(var loop in loops)Assert.AreEqual(6,loop.cycleSeconds);
  var starts=loops.Select(p=>p.transform.position).ToArray();Wait(300);for(int i=0;i<loops.Length;i++)Near(starts[i],loops[i].transform.position,.02f);
 }

 [UnityTest]public IEnumerator ArrivalCameraScaffoldsAndForegroundCurtainLeadToTheCaveFinale(){
  yield return Load();Assert.IsTrue(chapter.enterFromBelow);Near(chapter.room.start,Player.Position);
  Assert.AreEqual(new Rect(0,-1,126,58),chapter.room.roomBounds);Assert.AreEqual(2,chapter.room.checkpoints.Length);
  Assert.IsFalse(chapter.FilterAvailable);Assert.IsFalse(replay.ReplayUpgradeUnlocked);Assert.IsTrue(MainGameProgress.BasicReplayUnlocked);Assert.IsTrue(chapter.room.shutter.Unlocked);
  var scenery=chapter.transform.Find("Forest and waterfall scenery");Assert.IsNotNull(scenery);Assert.IsEmpty(scenery.GetComponentsInChildren<Collider2D>(true));
  var beams=scenery.GetComponentsInChildren<Transform>().Where(t=>t.name.EndsWith("scaffold beams")).ToArray();Assert.GreaterOrEqual(beams.Length,5);
  foreach(var beam in beams){Assert.Greater(beam.childCount,0);Assert.IsEmpty(beam.GetComponentsInChildren<Collider2D>(true));}
  var curtain=scenery.GetComponentsInChildren<WaterFeature>().Single(w=>w.name=="Grand Waterfall — foreground curtain");
  Assert.IsTrue(curtain.foreground);Assert.GreaterOrEqual(curtain.size.y,12);Assert.GreaterOrEqual(curtain.size.x,3.5f);
  Assert.Greater(curtain.GetComponent<SpriteRenderer>().sortingOrder,chapter.room.visual.renderer2D.sortingOrder);Assert.LessOrEqual(curtain.GetComponent<SpriteRenderer>().color.a,.38f);
  Assert.IsTrue(scenery.GetComponentsInChildren<Transform>().Any(t=>t.name=="Cavern depth"));
  foreach(var position in new[]{new Vector2(3,4),new Vector2(65,30),new Vector2(125,62)}){
   Player.ResetAt(position);chapter.SendMessage("LateUpdate");var view=chapter.room.view;
   Assert.That(view.orthographicSize*view.aspect*2,Is.EqualTo(20).Within(.001f));
   Assert.LessOrEqual(view.transform.position.y+view.orthographicSize,chapter.room.roomBounds.yMax+.001f);
   Assert.GreaterOrEqual(view.transform.position.x,10);Assert.LessOrEqual(view.transform.position.x,116);
  }
  Assert.Greater(Player.Position.y,chapter.room.roomBounds.yMax);Assert.IsEmpty(Physics2D.RaycastAll(Player.Position,Vector2.up,5,Player.worldMask).Where(h=>!h.collider.isTrigger).ToArray());
  Assert.IsTrue(chapter.chapterFinale);Place(new Vector2(123.5f,47.3f));Assert.AreEqual(ChapterOneRoom.Phase.Exiting,chapter.CurrentPhase);
  Wait(70);Assert.AreEqual(ChapterOneRoom.Phase.Complete,chapter.CurrentPhase);Assert.AreEqual(1,chapter.Fade);Assert.AreEqual("Main_Room08",SceneManager.GetActiveScene().name);
 }

 [UnityTest]public IEnumerator PlateRevealFreezesRecordingPlayerPlatformsAndCratesThenReturnsCamera(){
  yield return Load();Place(chapter.room.checkpoints[0].transform.position);Record(20);GroundAt(Carrier.plate.transform.position);
  chapter.SendMessage("LateUpdate");Vector3 normal=chapter.room.view.transform.position;Vector2 player=Player.Position;
  var platforms=replay.platforms.Select(p=>p.transform.position).ToArray();var sources=Sources;var remaining=sources.Select(s=>s.Remaining).ToArray();
  var crates=sources.SelectMany(s=>s.pool).ToArray();var crateStates=crates.Select(c=>c.Capture()).ToArray();float recorded=replay.RecordSeconds;
  Act(move:1,jump:true,record:true,shutter:true);Assert.IsTrue(chapter.platformReveal.Revealing);
  Wait(26);chapter.SendMessage("LateUpdate");
  Near((Vector2)Carrier.transform.position+Vector2.up,chapter.room.view.transform.position,.02f);Assert.Greater(Vector3.Distance(normal,chapter.room.view.transform.position),5);
  replay.SetPaused(true);var frozenView=chapter.room.view.transform.position;Wait(10);chapter.SendMessage("LateUpdate");Assert.AreEqual(frozenView,chapter.room.view.transform.position);replay.SetPaused(false);
  for(int i=0;i<120&&chapter.platformReveal.Revealing;i++)Act(move:1,record:true);
  Assert.AreEqual(player,Player.Position);Assert.AreEqual(recorded,replay.RecordSeconds);Assert.IsFalse(Carrier.JourneyStarted);Assert.IsFalse(chapter.room.shutter.echo.Active);
  for(int i=0;i<platforms.Length;i++)Assert.AreEqual(platforms[i],replay.platforms[i].transform.position);
  for(int i=0;i<sources.Length;i++)Assert.AreEqual(remaining[i],sources[i].Remaining);
  for(int i=0;i<crates.Length;i++){Assert.AreEqual(crateStates[i].position,(Vector2)crates[i].transform.position);Assert.AreEqual(crateStates[i].active,crates[i].gameObject.activeSelf);}
  chapter.SendMessage("LateUpdate");Near(normal,chapter.room.view.transform.position,.001f);Act();Assert.Greater(replay.RecordSeconds,recorded);Assert.IsFalse(Carrier.JourneyStarted,"One plate can reveal the carrier, but cannot activate it.");
 }

 [UnityTest]public IEnumerator StatueDeckCarriesTheLiveGoatOnceAndParksAfterItsTenSecondJourney(){
  yield return Load();Vector2 home=Carrier.transform.position;Vector2 statueHome=Statue.transform.position;
  Assert.AreEqual(10,Carrier.journeySeconds);StartCarrier();BoardStatue();Wait(250);
  Assert.Greater(Carrier.JourneyProgress,.5f);Assert.IsFalse(Carrier.plate.Pressed);Near(Statue.transform.position,chapter.room.checkpoint);Near(Statue.transform.position,Player.Position,.03f);
  for(int i=0;i<500&&Carrier.InTransit;i++)Act();Near(home+Carrier.travel,Carrier.transform.position);Near(statueHome+Carrier.travel,Statue.transform.position);
  Assert.AreSame(Carrier.solid,Player.GroundCollider);Assert.AreSame(Statue,chapter.room.ActiveCheckpoint);Assert.IsTrue(Carrier.Active);
  Vector2 parked=Player.Position;Wait(150);Near(parked,Player.Position,.003f);Assert.AreEqual(Vector2.zero,Carrier.Delta);Assert.IsFalse(Carrier.InTransit);
 }

 [UnityTest]public IEnumerator FallsDrowningAndPauseResetRestoreMovingStatueBeforeRevivalWithoutLosingMemory(){
  yield return Load();Place(chapter.room.checkpoints[0].transform.position);Record();Finish();var memory=replay.Memory;
  Vector2 home=Carrier.transform.position;Vector2 statueHome=Statue.transform.position;
  foreach(string kind in new[]{"fall","water","pause"}){
   Play();StartCarrier();BoardStatue();Wait(100);Assert.Greater(Vector2.Distance(home,Carrier.transform.position),2);
   Die(kind);Assert.AreSame(memory,replay.Memory);Assert.IsTrue(replay.AwaitingReplay);Assert.IsTrue(chapter.room.IsReviving,kind+" must revive at the reset statue.");
   Near(home,Carrier.transform.position);Near(statueHome,Statue.transform.position);Near(statueHome,chapter.room.checkpoint);Near(statueHome,Player.Position);
   Assert.IsFalse(Carrier.JourneyStarted);Assert.IsFalse(chapter.movingCheckpoint.ResetPending);FinishRevival();Wait(20);Near(home,Carrier.transform.position);Near(statueHome,Player.Position);
  }
 }

 [UnityTest]public IEnumerator OrdinaryRecordingRestoresMovingCheckpointAndHoldsEveryRecordedObjectUntilPlayback(){
  yield return Load();StartCarrier();BoardStatue();
  for(int i=0;i<13;i++)Act(record:true);Assert.AreEqual(RewindPlaytest.Mode.Recording,replay.State);
  Vector2 recordedDeck=(Vector2)Carrier.transform.position-Carrier.Delta;Vector2 recordedStatue=(Vector2)Statue.transform.position-Carrier.Delta;
  Act();Wait(70);Finish();Near(recordedDeck,Carrier.transform.position);Near(recordedStatue,chapter.room.checkpoint);Assert.IsFalse(chapter.room.IsReviving);
  var sources=Sources;var remaining=sources.Select(s=>s.Remaining).ToArray();var crates=sources.SelectMany(s=>s.pool).ToArray();var positions=crates.Select(c=>c.transform.position).ToArray();var player=Player.Position;
  Wait(100);Near(recordedDeck,Carrier.transform.position);Near(player,Player.Position,.003f);Assert.AreEqual(Vector2.zero,Carrier.Delta);Assert.IsTrue(Carrier.JourneyStarted);
  for(int i=0;i<sources.Length;i++)Assert.AreEqual(remaining[i],sources[i].Remaining);
  for(int i=0;i<crates.Length;i++){Assert.AreEqual(positions[i],crates[i].transform.position);Assert.AreEqual(Vector2.zero,crates[i].CarryDelta);foreach(var art in crates[i].GetComponentsInChildren<SpriteRenderer>())Assert.AreEqual(Color.white,art.color);}
  Play();Wait(20);Assert.Greater(Vector2.Distance(recordedDeck,Carrier.transform.position),.3f);Near(Statue.transform.position,chapter.room.checkpoint);Assert.IsFalse(replay.AwaitingReplay);
 }

 [UnityTest]public IEnumerator DeathDuringRecordingResetsTheMovingStatueAfterVisibleRewind(){
  foreach(string kind in new[]{"fall","water","pause"}){
   yield return Load();Vector2 home=Carrier.transform.position;Vector2 statueHome=Statue.transform.position;
   StartCarrier();BoardStatue();Record(40);Assert.Greater(Carrier.JourneyProgress,0);
   Die(kind);Assert.AreEqual(RewindPlaytest.Mode.Rewinding,replay.State);Assert.IsTrue(chapter.movingCheckpoint.ResetPending);var memory=replay.Memory;
   FinishRewind();Assert.AreSame(memory,replay.Memory);Assert.IsNotNull(memory);Near(home,Carrier.transform.position);Near(statueHome,chapter.room.checkpoint);Near(statueHome,Player.Position);
   Assert.IsFalse(chapter.room.IsReviving,"Recording death keeps the visible rewind presentation rather than adding a second revival.");Assert.IsFalse(chapter.movingCheckpoint.ResetPending);
   Wait(50);Near(home,Carrier.transform.position);Assert.IsTrue(replay.AwaitingReplay);
  }
 }
}
}
#endif
