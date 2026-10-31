#if UNITY_EDITOR
using System.Collections;using NUnit.Framework;using UnityEngine;using UnityEngine.TestTools;using UnityEditor.SceneManagement;using UnityEngine.SceneManagement;
namespace OneMoreTime.Tests {
public class AbilityTests {
 RunManager game;
 IEnumerator Load(string scene){EditorSceneManager.LoadSceneInPlayMode("Assets/_OneMoreTime/Scenes/"+scene+".unity",new LoadSceneParameters(LoadSceneMode.Single));yield return null;game=Object.FindAnyObjectByType<RunManager>();game.AutoTick=false;game.InputSource=new TestPlayerInput();game.ResetRun();}
 void Step(InputFrame f=default,int count=1){for(int i=0;i<count;i++){game.AdvancePresentation(.02f);game.Tick(f);f.freeze=f.trace=f.jump=f.retry=false;}}
 void Collect(){game.player.ResetAt(game.Relic.Anchor-Vector2.up*.5f);Step();Assert.AreEqual(FlowState.AbilityDialogue,game.State);}
 void Read(){int guard=0;while(game.State==FlowState.AbilityDialogue&&guard++<10)game.AdvanceAbilityDialogue();game.FinishAbilityDemo();Assert.AreEqual(FlowState.Playing,game.State);}
 [UnityTest]public IEnumerator PresentationInterpolatesWithoutChangingSimulation(){
  yield return Load("L04_Timing");
  Step(default,2);Vector2 before=game.player.Position;Step(new InputFrame{move=1});Vector2 after=game.player.Position;
  Assert.Greater(after.x,before.x);
  foreach(float alpha in new[]{0f,.25f,.5f,.75f,1f}){
   game.PresentFrame(alpha);Assert.That(Vector2.Distance(game.visual.transform.position,Vector2.Lerp(before,after,alpha)),Is.LessThan(.0001f));
   Assert.AreEqual(after,game.player.Position);Assert.AreEqual(after,(Vector2)game.player.transform.position);
   Assert.That(game.view.transform.position.x,Is.EqualTo(Mathf.Clamp(game.visual.transform.position.x,game.config.cameraMin,game.config.cameraMax)).Within(.0001f));
  }
  Step(new InputFrame{trace=true});Step(new InputFrame{move=1},10);Step(new InputFrame{trace=true});
  Step(new InputFrame{trace=true});Vector2 oldEcho=game.echo.Pose.position;Step();Vector2 newEcho=game.echo.Pose.position;
  var saved=game.Memory;float playback=game.PlaybackTime;game.PresentFrame(.5f);
  Assert.That(Vector2.Distance(game.echo.visual.transform.position,Vector2.Lerp(oldEcho,newEcho,.5f)),Is.LessThan(.0001f));
  Assert.AreEqual(newEcho,game.echo.Pose.position);Assert.AreEqual(playback,game.PlaybackTime);Assert.AreSame(saved,game.Memory);
  game.ResetRun();game.PresentFrame(0);Assert.AreEqual(game.player.Position,(Vector2)game.visual.transform.position);
  Assert.AreEqual(game.echo.Pose.position,(Vector2)game.echo.visual.transform.position);
 }
 [UnityTest]public IEnumerator DemonstrationPausesWorldCanReplayAndSkip(){
  foreach(string scene in new[]{"L01_Freeze","L03_Trace"}){
   yield return Load(scene);Collect();while(game.State==FlowState.AbilityDialogue)game.AdvanceAbilityDialogue();
   Assert.AreEqual(FlowState.AbilityDemonstration,game.State);float demoTime=game.Demonstration.Time;for(int i=0;i<10;i++)game.Tick(default);Assert.AreEqual(demoTime,game.Demonstration.Time,"Fixed steps must not advance the presentation clock");game.AdvancePresentation(1f/120);Assert.That(game.Demonstration.Time,Is.EqualTo(demoTime+1f/120).Within(.00001f));float clock=game.Clock;Vector2 pos=game.player.Position;
   Step(new InputFrame{move=1,freeze=true,trace=true,retry=true},1200);
   Assert.IsTrue(game.Demonstration.Finished);Assert.AreEqual(clock,game.Clock);Assert.AreEqual(pos,game.player.Position);Assert.IsNull(game.Memory);Assert.IsFalse(game.IsRecording);
   game.SetPaused(true);Assert.AreEqual(FlowState.AbilityDemonstration,game.State);
   game.Demonstration.Restart();Assert.AreEqual(0,game.Demonstration.Time);Step(default,20);Assert.IsFalse(game.Demonstration.Finished);
   game.FinishAbilityDemo();Assert.AreEqual(FlowState.Playing,game.State);Assert.IsNull(game.Demonstration);Assert.AreEqual(pos,game.player.Position);Assert.IsTrue(game.Relic.Collected);
   Step();Assert.Greater(game.Clock,clock);
  }
 }
 [UnityTest]public IEnumerator FreezePickupLocksInputPausesAndSurvivesRetry(){yield return Load("L01_Freeze");Assert.IsFalse(game.FreezeUnlocked);Step(new InputFrame{freeze=true});Assert.IsNull(game.Memory);var original=game.Relic.transform.position;Step(default,20);Assert.AreNotEqual(original,game.Relic.transform.position);Collect();Assert.IsTrue(game.FreezeUnlocked);float clock=game.Clock;Vector2 pos=game.player.Position;Step(new InputFrame{move=1,freeze=true,retry=true},50);game.SetPaused(true);Assert.AreEqual(FlowState.AbilityDialogue,game.State);Assert.AreEqual(clock,game.Clock);Assert.AreEqual(pos,game.player.Position);Assert.IsNull(game.Memory);Read();Step(new InputFrame{freeze=true});Assert.NotNull(game.Memory);var memory=game.Memory;Step(new InputFrame{retry=true});Assert.AreSame(memory,game.Memory);Assert.IsTrue(game.Relic.Collected);Assert.IsFalse(game.Relic.GetComponent<SpriteRenderer>().enabled);game.player.ResetAt(new Vector2(2,-6));Step();Assert.IsTrue(game.FreezeUnlocked);Assert.AreEqual(FlowState.Playing,game.State);Assert.AreSame(memory,game.Memory);}
 [UnityTest]public IEnumerator TracePickupTimelineRollingWindowAndPlayback(){yield return Load("L03_Trace");Assert.IsTrue(game.FreezeUnlocked);Assert.IsFalse(game.TraceUnlocked);Step(new InputFrame{trace=true});Assert.IsFalse(game.IsRecording);Collect();Read();Step(new InputFrame{trace=true});Step(default,125);Assert.That(game.TraceTimelineSeconds,Is.EqualTo(2.5f).Within(.03f));game.SetPaused(true);float before=game.TraceTimelineSeconds;Step(default,100);Assert.AreEqual(before,game.TraceTimelineSeconds);game.SetPaused(false);Step(default,500);Assert.That(game.TraceTimelineSeconds,Is.EqualTo(10).Within(.03f));Step(new InputFrame{trace=true});Assert.NotNull(game.Memory);Assert.That(game.TraceTimelineSeconds,Is.EqualTo(game.Memory.Duration));Step(new InputFrame{trace=true});Step(default,25);Assert.That(game.TraceTimelineSeconds,Is.EqualTo(.5f).Within(.03f));Step(default,550);Assert.AreEqual(PlaybackState.Finished,game.Playback);Assert.That(game.TraceTimelineSeconds,Is.EqualTo(game.Memory.Duration));Assert.IsFalse(game.echo.Active);Step(new InputFrame{retry=true});Assert.IsTrue(game.TraceUnlocked);Assert.AreEqual(PlaybackState.Ready,game.Playback);Assert.IsTrue(game.Relic.Collected);}
 [UnityTest]public IEnumerator EchoCannotCollectAndUncollectedRelicSurvivesDeath(){yield return Load("L03_Trace");game.echo.Show(new RecordedFrame{position=game.Relic.Anchor},true);Step();Assert.IsFalse(game.Relic.Collected);game.player.ResetAt(new Vector2(2,-6));Step();Assert.IsFalse(game.TraceUnlocked);Assert.IsFalse(game.Relic.Collected);Collect();Read();Assert.IsTrue(game.TraceUnlocked);}
 [UnityTest]public IEnumerator LaterLevelsInheritAndNewJourneyRequiresPickup(){yield return Load("L02_Pressure");Assert.IsTrue(game.FreezeUnlocked);Assert.IsFalse(game.TraceUnlocked);Assert.IsNull(game.Relic);yield return Load("L04_Timing");Assert.IsTrue(game.FreezeUnlocked);Assert.IsTrue(game.TraceUnlocked);Assert.IsNull(game.Relic);yield return Load("L01_Freeze");Assert.IsFalse(game.FreezeUnlocked);Assert.IsFalse(game.TraceUnlocked);Assert.IsNotNull(game.Relic);}
}
}
#endif
