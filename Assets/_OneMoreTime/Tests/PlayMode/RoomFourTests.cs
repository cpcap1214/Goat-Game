#if UNITY_EDITOR
using System.Collections;
using System.Linq;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
namespace OneMoreTime.Tests {
[Category("ChapterOneRegression")]
public class RoomFourTests {
 Room4AbilityTutorial t;RewindPlaytest r;const float dt=.02f;
 IEnumerator Load(bool owned=false){MainGameProgress.StartOver();if(owned)MainGameProgress.GrantReplay();EditorSceneManager.LoadSceneInPlayMode("Assets/_OneMoreTime/Scenes/Main_Room05.unity",new LoadSceneParameters(LoadSceneMode.Single));yield return null;t=Object.FindFirstObjectByType<Room4AbilityTutorial>();t.AutoTick=false;r=t.replay;for(int i=0;i<150&&t.CurrentPhase==Room4AbilityTutorial.Phase.Arriving;i++)t.Step(default,false,dt);Assert.IsNull(t.room.lift,"The acquisition room now has a horizontal walk-in");Assert.That(r.player.Position.y,Is.EqualTo(4).Within(.01f));}
 void Step(float move=0,bool jump=false,bool q=false){t.Step(new InputFrame{move=move,jump=jump,held=true},q,dt);}
 void Acquire(){for(int i=0;i<250&&t.CurrentPhase==Room4AbilityTutorial.Phase.Approach;i++)Step(1);Assert.IsTrue(t.Unlocked);for(int i=0;i<500&&t.CurrentPhase!=Room4AbilityTutorial.Phase.AwaitRecord;i++)Step();Assert.AreEqual(Room4AbilityTutorial.Phase.AwaitRecord,t.CurrentPhase);}
 [UnityTest]public IEnumerator AcquisitionPromptsFadeAndChangeAfterRewind(){
  yield return Load();Assert.AreEqual(0,t.TutorialPromptAlpha);Acquire();
  Assert.AreEqual("Hold ["+KeyBindings.Label(GameAction.RecordReplay)+"] to record and rewind",t.TutorialPrompt);
  Assert.AreEqual(0,t.TutorialPromptAlpha,"The recording instruction starts transparent");
  for(int i=0;i<10;i++)Step();float first=t.TutorialPromptAlpha;Assert.That(first,Is.InRange(.05f,.4f));
  r.SetPaused(true);for(int i=0;i<30;i++)Step();Assert.AreEqual(first,t.TutorialPromptAlpha);r.SetPaused(false);
  for(int i=0;i<30;i++)Step();Assert.That(t.TutorialPromptAlpha,Is.EqualTo(.88f).Within(.001f));
  for(int i=0;i<13;i++)Step(q:true);
  for(int i=0;i<120&&t.CurrentPhase!=Room4AbilityTutorial.Phase.Playing;i++)Step();
  Assert.AreEqual(Room4AbilityTutorial.Phase.Playing,t.CurrentPhase);Assert.IsTrue(r.AwaitingReplay);
  Assert.AreEqual("Press ["+KeyBindings.Label(GameAction.RecordReplay)+"] to replay",t.TutorialPrompt);
  Assert.AreEqual(0,t.TutorialPromptAlpha,"The replay instruction gets its own fade after the rewind");
  for(int i=0;i<10;i++)Step();Assert.That(t.TutorialPromptAlpha,Is.InRange(.05f,.4f));
  for(int i=0;i<30;i++)Step();Assert.That(t.TutorialPromptAlpha,Is.EqualTo(.88f).Within(.001f));
  Step(q:true);Step();Assert.AreEqual(RewindPlaytest.Mode.Playing,r.State);
  for(int i=0;i<20;i++)Step();Assert.AreEqual(0,t.TutorialPromptAlpha,"Replay dismisses the hint smoothly");
  for(int i=0;i<250;i++)Step();Assert.AreEqual(0,t.TutorialPromptAlpha,"Finishing the replay does not repeat the hint");
 }
 [UnityTest]public IEnumerator PickupFallWaitsForActualHoldAndRestoresCamera(){
  yield return Load();Assert.IsTrue(t.bridge.deck.enabled);Assert.IsTrue(t.abilityBlocker.enabled);Acquire();Assert.IsTrue(t.bridge.Broken);Assert.IsFalse(t.abilityBlocker.enabled);
  Assert.That(r.player.Position.y,Is.InRange(t.waterSurface+t.holdDistance-.001f,t.waterSurface+t.holdDistance+t.holdTolerance));Assert.AreEqual(0,t.crateSource.ActiveCount);Assert.IsFalse(t.crateSource.spawningEnabled);
  var p=r.player.Position;for(int i=0;i<100;i++)Step(1,true);Assert.AreEqual(p,r.player.Position);Assert.IsTrue(t.TutorialVisible);Assert.AreEqual(RewindPlaytest.Mode.Idle,r.State);
  for(int i=0;i<12;i++)Step(q:true);Assert.IsTrue(t.TutorialVisible);Step(q:true);Assert.AreEqual(RewindPlaytest.Mode.Rewinding,r.State);Assert.IsFalse(t.TutorialVisible);Assert.AreEqual(p,r.player.Position,"No teleport on handoff");
  for(int i=0;i<45;i++)Step();t.SendMessage("LateUpdate");Assert.That(t.CameraSize,Is.EqualTo(10/t.room.view.aspect).Within(.001f));Assert.That(t.GameplaySpeed,Is.EqualTo(1).Within(.001f));
  Assert.IsTrue(t.crateSource.spawningEnabled);Assert.IsTrue(r.AwaitingReplay);Assert.AreEqual(0,t.crateSource.ActiveCount,"The scripted rewind also waits for replay before releasing world objects");
  Assert.That(Vector2.Distance(r.player.Position,t.tutorialReturnGround),Is.LessThan(.03f));Assert.AreEqual(t.tutorialReturnGround,r.Memory.Frame(0).position);Assert.AreEqual(p,r.Memory.Frame(r.Memory.Count-1).position);Assert.IsTrue(t.bridge.Broken);
  Assert.That(Vector2.Distance(r.Memory.Sample(r.Memory.Duration*.6f).position,t.pickupFeet),Is.LessThan(.001f));Assert.AreEqual(4,r.Memory.Sample(r.Memory.Duration*.3f).position.y);
  for(int i=0;i<60;i++)Step();Assert.AreEqual(0,t.crateSource.ActiveCount);Step(q:true);Step();Assert.IsFalse(r.AwaitingReplay);Assert.Greater(t.crateSource.ActiveCount,0);

 }
 [UnityTest]public IEnumerator CratePlaybackFollowsCursorLifetimesPauseReverseAndCarriesPlayer(){
  yield return Load(true);r.UnlockReplayUpgrade();for(int i=0;i<300;i++)Step();
  for(int i=0;i<13;i++)Step(q:true);Step();for(int i=0;i<350;i++)Step();
  Step(q:true);var end=t.crateSource.pool.Select(c=>c.Capture()).ToArray();Step();
  for(int i=0;i<120&&r.State==RewindPlaytest.Mode.Rewinding;i++)Step();Assert.AreEqual(RewindPlaytest.Mode.Ready,r.State);
  var start=t.crateSource.pool.Select(c=>c.Capture()).ToArray();Assert.AreNotEqual(start[0].active,end[0].active,"Recording crosses the first crate's disposal");
  r.Step(default,true);r.Step(default,false);for(int i=0;i<10;i++)r.Step(default,false);
  var crate=t.crateSource.pool.First(c=>c.gameObject.activeSelf);r.player.ResetAt(new Vector2(crate.transform.position.x,crate.GetComponent<BoxCollider2D>().bounds.max.y+.01f));Physics2D.SyncTransforms();r.Step(default,false);Assert.IsTrue(r.player.Grounded);
  float px=r.player.Position.x,cx=crate.transform.position.x;r.Step(default,false);Assert.That(r.player.Position.x-px,Is.EqualTo(crate.transform.position.x-cx).Within(.005f));
  var paused=t.crateSource.pool.Select(c=>c.Capture()).ToArray();for(int i=0;i<20;i++)r.Step(default,false,false,true);
  for(int i=0;i<paused.Length;i++)Assert.AreEqual(paused[i].position,(Vector2)t.crateSource.pool[i].transform.position);
  r.player.ResetAt(new Vector2(14.5f,4));for(int i=0;i<400&&r.State==RewindPlaytest.Mode.Playing;i++)r.Step(default,false);
  for(int i=0;i<end.Length;i++){Assert.AreEqual(end[i].active,t.crateSource.pool[i].gameObject.activeSelf);Assert.That(Vector2.Distance(end[i].position,t.crateSource.pool[i].transform.position),Is.LessThan(.001f));}
  r.Step(default,false,true);for(int i=0;i<400&&r.State==RewindPlaytest.Mode.Playing;i++)r.Step(default,false);Assert.AreEqual(0,r.PlaybackSeconds);
  for(int i=0;i<start.Length;i++){Assert.AreEqual(start[i].active,t.crateSource.pool[i].gameObject.activeSelf);Assert.That(Vector2.Distance(start[i].position,t.crateSource.pool[i].transform.position),Is.LessThan(.001f));}
 }
 [UnityTest]public IEnumerator FloatingCratesRewindVisiblyWithoutTint(){
  yield return Load(true);for(int i=0;i<40;i++)Step();var crate=t.crateSource.pool.First(c=>c.gameObject.activeSelf);
  var sprites=crate.GetComponentsInChildren<SpriteRenderer>();var colors=sprites.Select(sr=>sr.color).ToArray();
  for(int i=0;i<13;i++)Step(q:true);Step();for(int i=0;i<80;i++)Step();
  float end=crate.transform.position.x;Step(q:true);Step();Assert.AreEqual(RewindPlaytest.Mode.Rewinding,r.State);
  Assert.Greater(crate.transform.position.x,end,"Crate must visibly move against the current during rewind");
  for(int i=0;i<10;i++){Step();for(int j=0;j<sprites.Length;j++)Assert.AreEqual(colors[j],sprites[j].color,"Rewinding must not tint a crate");}
 }
 [UnityTest]public IEnumerator CratesFloatCarryDisposeAndRewindTheirLifetimes(){
  yield return Load(true);var s=(CrateSpawner)r.participants[0];for(int i=0;i<200;i++)Step();Assert.That(s.ActiveCount,Is.InRange(1,6));var c=s.pool.First(x=>x.gameObject.activeSelf);Assert.That(c.transform.position.y,Is.InRange(4.1f,4.6f));
  r.player.ResetAt(new Vector2(c.transform.position.x,c.GetComponent<BoxCollider2D>().bounds.max.y+.01f));Physics2D.SyncTransforms();for(int i=0;i<8;i++)Step();Assert.IsTrue(r.player.Grounded);float px=r.player.Position.x;for(int i=0;i<25;i++)Step();Assert.Less(r.player.Position.x,px-.3f,"Player is carried by the current");
  for(int i=0;i<13;i++)Step(q:true);var initial=s.pool.Select(x=>x.Capture()).ToArray();Step();for(int i=0;i<175;i++)Step();Step(q:true);Step();Assert.AreEqual(RewindPlaytest.Mode.Rewinding,r.State);for(int i=0;i<110&&r.State==RewindPlaytest.Mode.Rewinding;i++)Step();Assert.AreEqual(RewindPlaytest.Mode.Ready,r.State);
  for(int i=0;i<initial.Length;i++){Assert.AreEqual(initial[i].active,s.pool[i].gameObject.activeSelf);Assert.That(Vector2.Distance(initial[i].position,s.pool[i].transform.position),Is.LessThan(.1f));}
  c=s.pool.First(x=>x.gameObject.activeSelf);var held=c.Capture();c.transform.position=new Vector2(s.disposal.boundaryX-1,4);s.disposal.Check(c);Assert.IsTrue(c.gameObject.activeSelf,"Disposal callbacks cannot remove a frozen object");c.Restore(held);
  r.Step(default,true);r.Step(default,false);Assert.IsFalse(r.AwaitingReplay);c.transform.position=new Vector2(s.disposal.boundaryX-1,4);s.disposal.Check(c);Assert.IsTrue(c.gameObject.activeSelf);Assert.IsTrue(c.Sinking);
  float sinkStart=c.transform.position.y;for(int i=0;i<10;i++)c.Tick(dt);Assert.Less(c.transform.position.y,sinkStart-.1f);Assert.IsTrue(c.gameObject.activeSelf,"The drain submerges a crate before pooling it");
  for(int i=0;i<100&&c.gameObject.activeSelf;i++)c.Tick(dt);Assert.IsFalse(c.gameObject.activeSelf);
  for(int i=0;i<1500;i++)s.Tick(dt);Assert.That(s.ActiveCount,Is.InRange(1,6));
 }
 [UnityTest]public IEnumerator OnlyTheFunctionalDrainShowsAnEddyAndCratesSinkBeforePooling(){
  yield return Load(true);var source=t.crateSource;var drain=source.disposal;
  Assert.IsTrue(drain.sinkBeforeDisposal);Assert.IsFalse(drain.disposeToRight);Assert.AreEqual(40,drain.boundaryX);
  var eddies=t.GetComponentsInChildren<SpriteRenderer>(true).Where(sr=>sr.sprite!=null&&sr.sprite.name.StartsWith("SurfaceEddy")).ToArray();
  Assert.AreEqual(1,eddies.Length,"Only the actual crate sink should be marked with a whirlpool");
  Assert.That(eddies[0].transform.position.x,Is.EqualTo(drain.boundaryX).Within(.01f));
  Assert.That(eddies[0].transform.position.y,Is.EqualTo(source.water.surfaceY-.38f).Within(.01f));
  Step();var crate=source.pool.First(c=>c.gameObject.activeSelf);var afloat=crate.Capture();
  var sprites=crate.GetComponentsInChildren<SpriteRenderer>();var colors=sprites.Select(sr=>sr.color).ToArray();
  for(int i=0;i<650&&!crate.Sinking;i++)Step();
  Assert.IsTrue(crate.Sinking);Assert.IsTrue(crate.gameObject.activeSelf);Assert.That(crate.transform.position.x,Is.EqualTo(drain.boundaryX).Within(.1f));
  var sinking=crate.Capture();for(int i=0;i<10;i++)Step();
  Assert.IsTrue(crate.gameObject.activeSelf);Assert.Less(crate.transform.position.y,sinking.position.y-.1f);
  foreach(var sprite in sprites){Assert.IsTrue(sprite.sharedMaterial.HasProperty("_WaterlineY"));Assert.AreEqual(4,sprite.sharedMaterial.GetFloat("_WaterlineY"));}
  for(int i=0;i<150&&crate.gameObject.activeSelf;i++)Step();Assert.IsFalse(crate.gameObject.activeSelf,"The submerged crate must return to the pool");
  crate.Restore(sinking);Assert.IsTrue(crate.gameObject.activeSelf);Assert.IsTrue(crate.Sinking);Assert.AreEqual(sinking.position,(Vector2)crate.transform.position);
  crate.Restore(afloat);Assert.IsFalse(crate.Sinking);Assert.AreEqual(afloat.position,(Vector2)crate.transform.position);
  for(int i=0;i<sprites.Length;i++)Assert.AreEqual(colors[i],sprites[i].color,"Sinking and rewind must not tint the crate");
 }
 [UnityTest]public IEnumerator ReachUpperPlatformsWithUnchangedJump(){
  yield return Load(true);r.player.ResetAt(new Vector2(27.8f,4));Physics2D.SyncTransforms();Step();Step();
  void JumpTo(float x,float minimumY){
   Assert.IsTrue(r.player.Grounded);Step(1,true);bool landed=false;
   for(int i=0;i<90;i++){float move=Mathf.Abs(x-r.player.Position.x)<.08f?0:Mathf.Sign(x-r.player.Position.x);Step(move);if(i>10&&r.player.Grounded){landed=true;break;}}
   Assert.IsTrue(landed,"Failed landing near "+x+" actual "+r.player.Position);Assert.GreaterOrEqual(r.player.Position.y,minimumY);
  }
  JumpTo(32,5.9f);JumpTo(36,4.9f);JumpTo(41,7.4f);JumpTo(46,5.5f);
  Assert.AreEqual(Room4AbilityTutorial.Phase.Playing,t.CurrentPhase);
  // The revised eight-second spacing is a replay puzzle, not an auto-jump shortcut.

 }
 [UnityTest]public IEnumerator ReloadSkipsAcquisitionAndNewJourneyResetsIt(){
  yield return Load(true);Assert.AreEqual(Room4AbilityTutorial.Phase.Playing,t.CurrentPhase);Assert.IsFalse(t.pickup.gameObject.activeSelf);Assert.IsFalse(t.abilityBlocker.enabled);Assert.IsTrue(t.bridge.Broken);
  Step(q:true);Step();Assert.AreEqual(RewindPlaytest.Mode.Idle,r.State);MainGameProgress.StartOver();Assert.IsFalse(MainGameProgress.BasicReplayUnlocked);
 }
 [UnityTest]public IEnumerator ReturningToMenuRestartsCompletedAndInterruptedAcquisition(){
  yield return Load();Acquire();
  for(int i=0;i<13;i++)Step(q:true);
  for(int i=0;i<120&&t.CurrentPhase!=Room4AbilityTutorial.Phase.Playing;i++)Step();
  Assert.AreEqual(Room4AbilityTutorial.Phase.Playing,t.CurrentPhase,"Finish acquisition before the first menu exit");
  Assert.IsTrue(t.crateSource.spawningEnabled);Assert.IsNotNull(r.Memory);
  MainGameProgress.GrantReplayUpgrade();MainGameProgress.RecordDeath();
  Assert.AreEqual(1,MainGameProgress.DeathCount);
  for(int visit=0;visit<2;visit++){
   if(visit==1)Assert.AreEqual(Room4AbilityTutorial.Phase.AwaitRecord,t.CurrentPhase,"The second exit interrupts the acquisition animation");
   r.SetPaused(true);SceneFlow.Load(0);yield return null;
   Assert.AreEqual("MainMenu",SceneManager.GetActiveScene().name);
   Assert.IsFalse(MainGameProgress.BasicReplayUnlocked);Assert.IsFalse(MainGameProgress.ReplayUpgradeUnlocked);
   Assert.IsFalse(MainGameProgress.IsMainJourney);Assert.AreEqual(1,MainGameProgress.DeathCount,"Returning to the menu must preserve the last journey's total");
   var menu=Object.FindFirstObjectByType<MenuScreen>();Assert.IsNotNull(menu);menu.SendMessage("OpenTutorial",10);yield return null;
   Assert.AreEqual(Room4AbilityTutorial.SceneName,SceneManager.GetActiveScene().name);
   t=Object.FindFirstObjectByType<Room4AbilityTutorial>();Assert.IsNotNull(t);t.AutoTick=false;r=t.replay;
   for(int i=0;i<150&&t.CurrentPhase==Room4AbilityTutorial.Phase.Arriving;i++)t.Step(default,false,dt);
   Assert.AreEqual(Room4AbilityTutorial.Phase.Approach,t.CurrentPhase);
   Assert.IsTrue(t.pickup.gameObject.activeSelf);Assert.IsTrue(t.abilityBlocker.enabled);
   Assert.IsFalse(t.bridge.Broken);Assert.IsTrue(t.bridge.deck.enabled);
   Assert.IsFalse(t.crateSource.spawningEnabled);Assert.AreEqual(0,t.crateSource.ActiveCount);
   Assert.IsNull(r.Memory);Assert.IsFalse(MainGameProgress.IsMainJourney);
   Acquire();Assert.IsTrue(t.TutorialVisible);
   Assert.AreEqual(1,MainGameProgress.DeathCount,"Replaying the debug acquisition must not change the journey total");
  }
 }
 [UnityTest]public IEnumerator SlowFallHasNoCratesAndNeverTeleportsUp(){
  yield return Load();for(int i=0;i<250&&t.CurrentPhase==Room4AbilityTutorial.Phase.Approach;i++)Step(1);
  float previous=r.player.Position.y;bool sawSlow=false;int slowFrames=0;
  for(int i=0;i<500&&!t.TutorialVisible;i++){
   Step();Assert.AreEqual(0,t.crateSource.ActiveCount);Assert.That(r.player.Position.y,Is.LessThanOrEqualTo(previous+.001f));previous=r.player.Position.y;
   if(t.CurrentPhase==Room4AbilityTutorial.Phase.Falling&&t.GameplaySpeed<.8f&&t.GameplaySpeed>0){sawSlow=true;slowFrames++;}
  }
  Assert.IsTrue(t.TutorialVisible);Assert.IsTrue(sawSlow);Assert.Greater(slowFrames,15);Assert.AreEqual(1,Time.timeScale);
  r.SetPaused(true);for(int i=0;i<50;i++)Step(q:true);Assert.IsTrue(t.TutorialVisible);r.SetPaused(false);Step();for(int i=0;i<13;i++)Step(q:true);Assert.AreEqual(RewindPlaytest.Mode.Rewinding,r.State);
 }
 [UnityTest]public IEnumerator StatueUsesExistingCheckpointAndRetriesPreserveMemory(){
  yield return Load(true);var mark=t.room.checkpoints.Single();Assert.AreEqual(new Vector3(26,4,0),mark.transform.position);Assert.IsFalse(mark.Saved);Assert.Less(mark.stoneVisual.target.sortingOrder,t.room.visual.renderer2D.sortingOrder);
  r.player.ResetAt(new Vector2(26,4));for(int i=0;i<30;i++)Step();Assert.IsTrue(mark.Saved);Assert.IsTrue(t.room.CheckpointSaved);Assert.IsTrue(mark.stoneVisual.Active);mark.stoneVisual.Tick(1);Assert.AreEqual(mark.stoneVisual.frames[4],mark.stoneVisual.target.sprite);
  for(int i=0;i<13;i++)Step(q:true);Step();for(int i=0;i<20;i++)Step(1);Step(q:true);Step();for(int i=0;i<100&&r.State==RewindPlaytest.Mode.Rewinding;i++)Step();var memory=r.Memory;Assert.IsNotNull(memory);
  r.player.ResetAt(new Vector2(35,0));Step();Assert.AreEqual(Room4AbilityTutorial.Phase.Drowning,t.CurrentPhase);for(int i=0;i<200&&t.CurrentPhase==Room4AbilityTutorial.Phase.Drowning;i++)Step();Assert.That(Vector2.Distance(r.player.Position,new Vector2(26,4)),Is.LessThan(.03f));Assert.AreSame(memory,r.Memory);
  Assert.IsTrue(t.room.IsReviving,"Drowning must end with emergence from the saved statue");
  for(int i=0;i<80&&t.room.IsReviving;i++)Step();Assert.IsFalse(t.room.IsReviving);Assert.IsTrue(t.room.visual.renderer2D.enabled);Assert.AreSame(memory,r.Memory);
  r.player.ResetAt(new Vector2(32,6));t.Step(new InputFrame{retry=true},false,dt);Assert.That(Vector2.Distance(r.player.Position,new Vector2(26,4)),Is.LessThan(.03f));Assert.IsTrue(t.room.IsReviving);
 }
 [UnityTest]public IEnumerator TerrainWaterAndLedgesRemainVisualAndConnected(){
  yield return Load();var env=t.transform.Find("Environment");
  foreach(string name in new[]{"Lift landing and approach/Entrance ground","First pit right bank/Old terrain body","Far right exit bank/Old terrain body"}){var fill=env.Find(name);Assert.IsNotNull(fill);Assert.That(fill.GetComponent<SpriteRenderer>().bounds.min.y,Is.EqualTo(t.room.roomBounds.yMin).Within(.01f));Assert.IsNull(fill.GetComponent<Collider2D>());}
  for(int i=1;i<=3;i++){
   var platform=env.Find("Main-style grass platform "+i);var column=platform.GetComponent<GrassyColumnVisual>();Assert.IsNotNull(column);
   Assert.IsNotNull(platform.Find("Rectangular grass column art/Grass left edge"));Assert.IsNotNull(platform.Find("Rectangular grass column art/Soil interior"));
   Assert.IsNotNull(platform.GetComponent<OneWayPlatform>());Assert.AreEqual(1,platform.GetComponentsInChildren<Collider2D>(true).Length);
   Assert.That(column.width,Is.EqualTo(platform.GetComponent<BoxCollider2D>().size.x).Within(.001f));Assert.AreEqual(16,column.grassMiddle.pixelsPerUnit);
  }
  foreach(var water in t.waters){var fill=water.transform.parent.Find("Full depth water fill");Assert.IsNotNull(fill);var map=fill.GetComponent<UnityEngine.Tilemaps.Tilemap>();Assert.Greater(map.GetUsedTilesCount(),0);Assert.IsNull(fill.GetComponent<Collider2D>());Assert.LessOrEqual(fill.position.y,t.room.roomBounds.yMin);}
  Assert.IsNull(env.Find("First pit wall ledge"));Assert.IsNull(env.Find("Diagonal bank wall ledge"));Assert.IsNull(env.Find("Exit bank wall ledge"));
  Assert.IsNotNull(env.Find("First pit right bank/Pure dirt terrain bevel"));Assert.IsNotNull(env.Find("Far right exit bank/Grass terrain bevel"));
  Assert.IsNull(env.Find("Lift landing wood platform"),"The horizontal entrance has no lift landing deck");
  Assert.That(env.Find("Lift landing and approach").GetComponent<BoxCollider2D>().bounds.min.x,Is.EqualTo(-2).Within(.001f));
  var channel=env.Find("Upper water channel/Flowing surface").GetComponent<UnityEngine.Tilemaps.Tilemap>();Assert.AreEqual(24,channel.GetUsedTilesCount()==0?0:channel.cellBounds.size.x);

  Assert.IsNull(t.transform.Find("CrateSystem/First pit floating crate"));
 }
 [UnityTest]public IEnumerator CrateSpeedAndSpacingMatchRevisedLayout(){
  yield return Load(true);var source=t.crateSource;Assert.AreEqual(10,source.spawnInterval);Assert.AreEqual(-2,source.water.velocity);Assert.AreEqual(-2,source.initialHorizontalVelocity);Assert.AreEqual(63.5f,source.spawnPoint.position.x);
  source.Tick(dt);var first=source.pool.First(c=>c.gameObject.activeSelf);float x=first.transform.position.x;
  for(int i=0;i<50;i++)source.Tick(dt);Assert.That(first.transform.position.x,Is.EqualTo(x-2).Within(.005f));
  for(int i=0;i<448;i++)source.Tick(dt);Assert.AreEqual(1,source.ActiveCount);
  for(int i=0;i<3;i++)source.Tick(dt);Assert.AreEqual(2,source.ActiveCount);
 }
 [UnityTest]public IEnumerator RoomThreeExitKeepsExistingLiftAndOnlyChangesDestination(){
  EditorSceneManager.LoadSceneInPlayMode("Assets/_OneMoreTime/Scenes/Main_Room03.unity",new LoadSceneParameters(LoadSceneMode.Single));yield return null;var room=Object.FindFirstObjectByType<MainRoom>();room.AutoTick=false;
  Assert.AreEqual("Main_Room04",room.nextRoomScene);Assert.AreEqual(new Vector2(31,39),room.lift.dock);Assert.That(room.player.tuning.speed,Is.EqualTo(6));Assert.That(room.player.tuning.jumpSpeed,Is.EqualTo(14.178182f).Within(.001f));
 }
 [UnityTest]public IEnumerator RoomFiveExitWalksFadesAndLoadsRoomSixWithProgress(){
  yield return Load(true);Assert.AreEqual("Main_Room06",t.nextRoomScene);
  r.player.ResetAt(new Vector2(t.exitX+.05f,5.01f));Physics2D.SyncTransforms();
  for(int i=0;i<10&&t.CurrentPhase==Room4AbilityTutorial.Phase.Playing;i++)Step();
  Assert.AreEqual(Room4AbilityTutorial.Phase.Exiting,t.CurrentPhase);
  float startX=r.player.Position.x,fade=t.Fade;r.SetPaused(true);
  for(int i=0;i<20;i++)Step(1,true,true);
  Assert.AreEqual(startX,r.player.Position.x);Assert.AreEqual(fade,t.Fade);Assert.AreEqual(Room4AbilityTutorial.Phase.Exiting,t.CurrentPhase);
  r.SetPaused(false);for(int i=0;i<30;i++)Step();
  Assert.Greater(r.player.Position.x,startX+.5f);Assert.That(t.Fade,Is.InRange(.1f,.8f));
  for(int i=0;i<40&&t.CurrentPhase==Room4AbilityTutorial.Phase.Exiting;i++)Step();
  yield return null;
  Assert.AreEqual("Main_Room06",SceneManager.GetActiveScene().name);
  var next=Object.FindFirstObjectByType<ChapterOneRoom>();Assert.IsNotNull(next);next.AutoTick=false;
  Assert.AreEqual(6,next.roomNumber);Assert.IsTrue(MainGameProgress.BasicReplayUnlocked);
  Assert.IsNull(next.replay.Memory,"Each room starts with an empty recording while keeping learned abilities");
 }
}
}
#endif
