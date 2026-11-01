#if UNITY_EDITOR
using System.Collections;using System.Collections.Generic;using System.Linq;using NUnit.Framework;using UnityEngine;using UnityEngine.TestTools;using UnityEditor.SceneManagement;using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;using UnityEngine.InputSystem.LowLevel;
namespace OneMoreTime.Tests {
public class ChapterOneTests {
 NostalgiaChapter g;int steps;
 [UnityTest]public IEnumerator KeyboardTutorialSimultaneousJumpFilterAndPause(){
  EditorSceneManager.LoadSceneInPlayMode("Assets/_OneMoreTime/Scenes/Chapter01_EchoFoothills.unity",new LoadSceneParameters(LoadSceneMode.Single));yield return null;
  g=Object.FindFirstObjectByType<NostalgiaChapter>();g.enabled=false;
  var previous=Keyboard.current;var keyboard=InputSystem.AddDevice<Keyboard>();keyboard.MakeCurrent();
  System.Action<Key[]> pump=keys=>{InputSystem.QueueStateEvent(keyboard,new KeyboardState(keys));InputSystem.Update();g.SendMessage("Update");g.SendMessage("FixedUpdate");};
  try{
   pump(new[]{Key.Enter});Assert.AreEqual(-1,g.Tutorial);pump(new Key[0]);
   for(int i=0;i<40&&g.Tutorial<0;i++)pump(new[]{Key.D});Assert.IsTrue(g.Unlocked);Assert.AreEqual(1,g.Tutorial);
   pump(new Key[0]);pump(new[]{Key.Enter});pump(new Key[0]);pump(new[]{Key.Enter});pump(new Key[0]);Assert.AreEqual(-1,g.Tutorial);
   var start=g.player.Position;int switches=g.Switches;
   pump(new[]{Key.D,Key.Space,Key.F});for(int i=0;i<12;i++)pump(new[]{Key.D,Key.Space,Key.F});
   Assert.IsTrue(g.Past);Assert.AreEqual(switches+1,g.Switches,"Held F must switch only once");Assert.Greater(g.player.Position.y,start.y+.5f);Assert.Greater(g.player.Position.x,start.x+.5f);
   pump(new Key[0]);pump(new[]{Key.Escape});Assert.IsTrue(g.Paused);var paused=g.player.Position;var elapsed=g.Elapsed;
   for(int i=0;i<10;i++)pump(new[]{Key.D,Key.F,Key.Space});Assert.AreEqual(paused,g.player.Position);Assert.AreEqual(elapsed,g.Elapsed);
   pump(new Key[0]);pump(new[]{Key.Escape});pump(new Key[0]);pump(new[]{Key.R});Assert.IsFalse(g.Paused);Assert.IsFalse(g.Past);Assert.AreEqual(ChapterLayout.Checkpoints[g.Checkpoint],g.player.Position);
   pump(new Key[0]);g.player.ResetAt(ChapterLayout.Devices[0]);pump(new[]{Key.F});pump(new Key[0]);pump(new[]{Key.X});Assert.IsTrue(g.Devices[0],"Keyboard X must activate past mechanism");
  }finally{InputSystem.RemoveDevice(keyboard);if(previous!=null)previous.MakeCurrent();}
 }
 IEnumerator Load(){EditorSceneManager.LoadSceneInPlayMode("Assets/_OneMoreTime/Scenes/Chapter01_EchoFoothills.unity",new LoadSceneParameters(LoadSceneMode.Single));yield return null;g=Object.FindFirstObjectByType<NostalgiaChapter>();g.AutoTick=false;g.AdvanceTutorial();Step(default,4);}
 void Step(InputFrame f,int n=1,bool flip=false){for(int i=0;i<n;i++){g.Tick(f,flip&&i==0);steps++;while(g.Guidance.IsOpen)g.AdvanceGuidance();f.jump=f.freeze=f.trace=f.retry=false;if(g.Tutorial>=0){g.AdvanceTutorial();if(g.Tutorial>=0)g.AdvanceTutorial();}Assert.Less(steps,20000,"Route exceeded fixed-step budget");}}
 void Walk(float x){for(int i=0;i<500&&Mathf.Abs(g.player.Position.x-x)>.12f;i++)Step(new InputFrame{move=Mathf.Sign(x-g.player.Position.x)});Step(default,8);Assert.That(g.player.Position.x,Is.EqualTo(x).Within(.4f),"Walk destination "+x+" current "+g.player.Position);}
 void Era(bool past){if(g.Past!=past)Assert.IsTrue(g.TrySwitch(),"Switch blocked at "+g.player.Position);}
 void Hop(float targetX,float targetY,ChapterEra era){
  var support=g.terrain.Where(t=>t.solid.enabled&&Mathf.Abs(t.block.rect.yMax-g.player.Position.y)<.15f&&g.player.Position.x>=t.block.rect.xMin-.3f&&g.player.Position.x<=t.block.rect.xMax+.3f).OrderByDescending(t=>t.block.rect.xMax).FirstOrDefault();
  Assert.IsNotNull(support,"No takeoff support at "+g.player.Position);
  float takeoff=Mathf.Min(support.block.rect.xMax-.2f,targetX-1);if(takeoff>g.player.Position.x+.1f)Walk(takeoff);
  int deaths=g.Deaths;Step(new InputFrame{jump=true,held=true,move=1});
  if(era!=ChapterEra.Both)Era(era==ChapterEra.Past);
  for(int i=0;i<120;i++){float dx=targetX-g.player.Position.x;Step(new InputFrame{held=true,move=Mathf.Abs(dx)>.12f?Mathf.Sign(dx):0});if(i>4&&g.player.Grounded)break;}
  Step(default,5);Assert.AreEqual(deaths,g.Deaths,"Died hopping to "+targetX);Assert.That(g.player.Position.y,Is.EqualTo(targetY).Within(.08f),"Hop Y at "+targetX);Walk(targetX);
 }
 void JumpTo(float x,int ticks=65){Step(new InputFrame{jump=true,held=true,move=Mathf.Sign(x-g.player.Position.x)});for(int i=1;i<ticks;i++)Step(new InputFrame{held=true,move=Mathf.Abs(g.player.Position.x-x)>.08f?Mathf.Sign(x-g.player.Position.x):0});Step(default,6);}
 [UnityTest]public IEnumerator FullChapterNormalMotorRoute(){
  yield return Load();Walk(7);Assert.IsTrue(g.Unlocked);Era(true);Walk(36);Walk(49);Era(false);Walk(74);
  Era(true);Walk(80);Step(new InputFrame{freeze=true});Assert.IsNotNull(g.memories.Memory);Assert.IsTrue(g.memories.FreezeSolved);Walk(115);g.Interact();Assert.IsTrue(g.Photos[0]);Walk(132);
  Walk(137);Era(true);Walk(165);g.Interact();Assert.IsTrue(g.Devices[0]);Era(false);Walk(189);
  Walk(195);Step(new InputFrame{freeze=true});Assert.IsNull(g.memories.Memory);Walk(205);Step(new InputFrame{trace=true});Step(default,50);Walk(216);Step(default,50);Step(new InputFrame{trace=true});Assert.IsNotNull(g.memories.Memory);Walk(230);Step(new InputFrame{trace=true});Step(default,250);Assert.IsTrue(g.memories.TraceSolved);Walk(248);Era(false);g.Interact();Assert.IsTrue(g.Photos[1]);Walk(266);
  for(int i=0;i<4;i++)Hop(270.8f+i*5,1.8f+i*1.8f,i%2==0?ChapterEra.Past:ChapterEra.Present);
  Hop(290,7.2f,ChapterEra.Both);Era(true);g.Interact();Assert.IsTrue(g.Devices[1]);
  Hop(299,5,ChapterEra.Present);Hop(308,3,ChapterEra.Past);Hop(317,0,ChapterEra.Both);Walk(328);
  Step(new InputFrame{trace=true});Assert.IsNull(g.memories.Memory);Walk(331.4f);Step(new InputFrame{trace=true});Step(default,100);JumpTo(336.3f);Walk(341.7f);Step(default,90);Step(new InputFrame{jump=true,held=true});Step(new InputFrame{held=true},55);Step(default,25);Step(new InputFrame{trace=true});
  Walk(331.4f);Step(new InputFrame{trace=true});JumpTo(331.4f);Assert.IsTrue(g.player.OnEcho,"board recorded self");
  for(int i=0;i<400&&(g.memories.echo.Pose.position.x<341||g.memories.echo.Pose.position.y<2.8f);i++)Step(default);
  Assert.IsTrue(g.player.OnEcho,"ride to launch");Step(new InputFrame{jump=true,held=true});Step(new InputFrame{held=true},13);Step(new InputFrame{held=true,move=1},55);Assert.Greater(g.player.Position.y,6.2f);Walk(380);Era(false);g.Interact();
  Assert.IsTrue(g.Complete);Assert.AreEqual(0,g.Deaths);Assert.Greater(g.Switches,10);System.IO.File.WriteAllText("Artifacts/Logs/chapter-route.txt",$"PASS simulated normal motor route; seconds={g.Elapsed}; switches={g.Switches}; deaths={g.Deaths}; checkpoints={g.Checkpoint}; photos=3\n");
 }
 [UnityTest]public IEnumerator CameraKeepsGoatVisibleAcrossPuzzleExits(){
  yield return Load();
  foreach(float start in new[]{69f,193f}){
   Vector3 prior=Vector3.zero;float priorZoom=0;bool first=true;
   for(float x=start;x<=start+62;x+=.25f){
    g.player.ResetAt(new Vector2(x,0));g.SendMessage("LateUpdate");
    var feet=g.view.WorldToViewportPoint(g.player.Position);
    var head=g.view.WorldToViewportPoint(g.player.Position+Vector2.up);
    Assert.That(feet.x,Is.InRange(.26f,.74f),"Goat leaves safe camera area at "+x);
    Assert.That(feet.y,Is.InRange(.1f,.8f));Assert.Less(head.y,.9f);
    if(!first){Assert.Less(Vector3.Distance(prior,g.view.transform.position),.8f,"Camera snap at "+x);Assert.Less(Mathf.Abs(priorZoom-g.view.orthographicSize),.3f);}
    prior=g.view.transform.position;priorZoom=g.view.orthographicSize;first=false;
   }
  }
 }
 [UnityTest]public IEnumerator FreezeGateRequiresSavedPastSelf(){yield return Load();Walk(7);g.player.ResetAt(new Vector2(65,0));Step(default,3);g.player.ResetAt(new Vector2(80,0));Step(default,3);Step(new InputFrame{freeze=true});Assert.IsFalse(g.memories.FreezeSolved,"Present snapshot must not power past plate");Step(new InputFrame{freeze=true});g.player.ResetAt(new Vector2(80,0));g.TrySwitch();Step(default,3);Step(new InputFrame{freeze=true});Assert.IsTrue(g.memories.FreezeSolved);Assert.IsFalse(g.memories.freezeGate.enabled);var saved=g.memories.Memory;Step(new InputFrame{trace=true});Assert.AreSame(saved,g.memories.Memory,"Q cannot replace freeze");g.Respawn();Assert.AreSame(saved,g.memories.Memory);}
 [UnityTest]public IEnumerator OneFrozenSignalCannotUnlockOrderedGate(){yield return Load();g.player.ResetAt(new Vector2(205,0));Step(default,3);Step(new InputFrame{freeze=true});g.player.ResetAt(new Vector2(230,0));Step(default,150);Assert.AreEqual(1,g.memories.Signals);Assert.IsFalse(g.memories.TraceSolved);Assert.IsTrue(g.memories.traceGate.enabled);}
 [UnityTest]public IEnumerator RecordingDeathOffersOnlyLivingFramesAndRetryKeepsSnapshot(){yield return Load();g.player.ResetAt(new Vector2(65,0));Step(default,3);Step(new InputFrame{trace=true});Step(default,600);Assert.That(g.memories.Recorder.Duration,Is.EqualTo(10).Within(.03));int count=g.memories.Recorder.Count;g.player.ResetAt(new Vector2(65,-6));Step(default);Assert.IsTrue(g.memories.DeathReview);Assert.AreEqual(count,g.memories.Recorder.Count);Step(new InputFrame{trace=true});var saved=g.memories.Memory;Assert.IsNotNull(saved);Assert.Greater(saved.Frame(saved.Count-1).position.y,-1);Step(new InputFrame{retry=true});Assert.AreSame(saved,g.memories.Memory);Assert.IsFalse(g.memories.Recording);Assert.AreEqual(PlaybackState.Ready,g.memories.Playback);Step(new InputFrame{trace=true});Step(default,550);Assert.AreEqual(PlaybackState.Finished,g.memories.Playback);Assert.IsFalse(g.memories.echo.Active);}
 [UnityTest]public IEnumerator RelayCliffCannotBeReachedBySoloJump(){yield return Load();g.player.ResetAt(new Vector2(341.7f,0));Step(default,3);JumpTo(346);Assert.Less(g.player.Position.x,343);Assert.IsFalse(g.Complete);}
 [UnityTest]public IEnumerator ReversedSignalsDoNotUnlockGate(){yield return Load();g.player.ResetAt(new Vector2(216,0));Step(default,3);Step(new InputFrame{trace=true});Step(default,30);Walk(205);Step(default,30);Step(new InputFrame{trace=true});g.player.ResetAt(new Vector2(230,0));Step(default,3);Step(new InputFrame{trace=true});Step(default,350);Assert.IsFalse(g.memories.TraceSolved);Assert.IsTrue(g.memories.traceGate.enabled);}
 [UnityTest]public IEnumerator LeavingConsoleBreaksSignalSequence(){yield return Load();g.player.ResetAt(new Vector2(205,0));Step(default,3);Step(new InputFrame{trace=true});Step(default,30);Walk(216);Step(default,30);Step(new InputFrame{trace=true});g.player.ResetAt(new Vector2(230,0));Step(default,3);Step(new InputFrame{trace=true});Step(default,80);Assert.AreEqual(1,g.memories.Signals);g.player.ResetAt(new Vector2(227,0));Step(default,3);Assert.AreEqual(0,g.memories.Signals);g.player.ResetAt(new Vector2(230,0));Step(default,250);Assert.IsFalse(g.memories.TraceSolved);}
 [UnityTest]public IEnumerator RelayCannotBeBypassedWithReachableFreeze(){yield return Load();for(int heightTick=2;heightTick<=24;heightTick+=2){if(g.memories.Memory!=null)Step(new InputFrame{freeze=true});g.player.ResetAt(new Vector2(341.7f,0));Step(default,3);Step(new InputFrame{jump=true,held=true});Step(new InputFrame{held=true},heightTick);Step(new InputFrame{freeze=true,held=true});g.player.ResetAt(new Vector2(341.7f,0));Step(default,3);JumpTo(341.7f);JumpTo(347);Assert.Less(g.player.Position.x,343,"Freeze ascent tick "+heightTick);}}
 [UnityTest]public IEnumerator RoomGuidancePausesAndCanBeReopened(){yield return Load();g.player.ResetAt(new Vector2(65,0));g.Tick(default);Assert.IsTrue(g.Guidance.IsOpen);float elapsed=g.Elapsed;var position=g.player.Position;g.Tick(new InputFrame{move=1,freeze=true,trace=true},true);Assert.AreEqual(elapsed,g.Elapsed);Assert.AreEqual(position,g.player.Position);Assert.IsNull(g.memories.Memory);Assert.IsFalse(g.memories.Recording);g.AdvanceGuidance();Assert.IsFalse(g.Guidance.IsOpen);g.Tick(default);Assert.IsFalse(g.Guidance.IsOpen,"Room introduction must not repeatedly interrupt");g.Guidance.OpenAt(330);Assert.IsTrue(g.Guidance.HasNext);g.AdvanceGuidance();Assert.IsTrue(g.Guidance.IsOpen);Assert.AreEqual(1,g.Guidance.Page);g.AdvanceGuidance();Assert.IsFalse(g.Guidance.IsOpen);}
 [UnityTest]public IEnumerator ViewpointExitOpensOnlyAfterPresentDayPhoto(){yield return Load();Walk(7);Assert.IsTrue(g.photoGates[0].enabled);g.player.ResetAt(ChapterLayout.Photos[0]);Era(true);g.Interact();Assert.IsFalse(g.Photos[0]);Assert.IsTrue(g.photoGates[0].enabled);Era(false);g.Interact();Assert.IsTrue(g.Photos[0]);Assert.IsFalse(g.photoGates[0].enabled);g.Respawn();Assert.IsFalse(g.photoGates[0].enabled);}
 [UnityTest]public IEnumerator AirborneFilterPreservesMomentum(){yield return Load();Walk(7);Step(new InputFrame{jump=true,held=true,move=1},5);var position=g.player.Position;var velocity=g.player.Velocity;Assert.IsTrue(g.TrySwitch());Assert.AreEqual(position,g.player.Position);Assert.AreEqual(velocity,g.player.Velocity);}
 [UnityTest]public IEnumerator FilterCannotPlacePlayerInsideOppositeEraRock(){yield return Load();Walk(7);Era(true);g.player.ResetAt(new Vector2(41,0));Assert.IsFalse(g.TrySwitch());Assert.IsTrue(g.Past);}
 [UnityTest]public IEnumerator RetryKeepsMechanismsAndPhotosButClearsVelocity(){yield return Load();Walk(7);Era(true);g.player.ResetAt(ChapterLayout.Devices[0]);g.Interact();Assert.IsTrue(g.Devices[0]);Era(false);g.player.ResetAt(ChapterLayout.Photos[0]);g.Interact();Assert.IsTrue(g.Photos[0]);g.Respawn();Assert.IsTrue(g.Photos[0]);Assert.IsTrue(g.Devices[0]);Assert.IsFalse(g.Past);Assert.AreEqual(Vector2.zero,g.player.Velocity);Assert.IsTrue(g.Unlocked);}
}
}
#endif
