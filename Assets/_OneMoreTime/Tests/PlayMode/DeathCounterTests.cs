#if UNITY_EDITOR
using System.Collections;
using NUnit.Framework;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

namespace OneMoreTime.Tests {
[Category("ChapterOneRegression")]
public class DeathCounterTests {
 const float dt=.02f;
 MainRoom room;
 ChapterOneRoom chapter;
 Room4AbilityTutorial tutorial;
 RewindPlaytest replay;

 [SetUp] public void StartJourney(){MainGameProgress.StartOver();}
 [TearDown] public void EndJourney(){MainGameProgress.EndJourney();}

 IEnumerator Load(int number){
  EditorSceneManager.LoadSceneInPlayMode("Assets/_OneMoreTime/Scenes/Main_Room"+number.ToString("00")+".unity",new LoadSceneParameters(LoadSceneMode.Single));
  yield return null;
  PrepareRoom();
 }
 void PrepareRoom(){
  room=Object.FindFirstObjectByType<MainRoom>();Assert.IsNotNull(room);room.AutoTick=false;
  chapter=Object.FindFirstObjectByType<ChapterOneRoom>();
  tutorial=Object.FindFirstObjectByType<Room4AbilityTutorial>();
  replay=room.GetComponent<RewindPlaytest>();
  if(chapter!=null)chapter.AutoTick=false;
  if(tutorial!=null)tutorial.AutoTick=false;
  if(replay!=null)replay.AutoTick=false;
  for(int i=0;i<300&&Entering;i++)Step();
  Assert.IsFalse(Entering,"The room must finish its entry before counting deaths");
 }
 bool Entering=>chapter!=null?chapter.CurrentPhase==ChapterOneRoom.Phase.Entering:
  tutorial!=null?tutorial.CurrentPhase==Room4AbilityTutorial.Phase.Arriving:room.Phase==MainRoom.RoomPhase.Entering;
 void Step(InputFrame command=default,bool held=false){
  if(chapter!=null)chapter.Step(command,held);
  else if(tutorial!=null)tutorial.Step(command,held,dt);
  else room.Step(command,dt);
  Physics2D.SyncTransforms();
 }
 void Fall(){room.player.ResetAt(new Vector2(room.start.x,room.deathY-1));Step();}
 void Record(){
  for(int i=0;i<13;i++)Step(default,true);
  Step();
  for(int i=0;i<15;i++)Step();
  Assert.AreEqual(RewindPlaytest.Mode.Recording,replay.State);
 }
 void FinishRewind(){
  Assert.AreEqual(RewindPlaytest.Mode.Rewinding,replay.State);
  for(int i=0;i<200&&replay.State==RewindPlaytest.Mode.Rewinding;i++)Step();
  Assert.AreEqual(RewindPlaytest.Mode.Ready,replay.State);
 }

 [UnityTest] public IEnumerator DeathsAccumulateAcrossMainRoomsAndNewGameClearsThem(){
  yield return Load(1);
  Assert.IsTrue(MainGameProgress.IsMainJourney);Assert.AreEqual(0,MainGameProgress.DeathCount);
  Fall();Assert.AreEqual(1,MainGameProgress.DeathCount);
  Step(new InputFrame{retry=true});Assert.AreEqual(1,MainGameProgress.DeathCount,"Ordinary R retry is not another death");
  yield return Load(6);
  Assert.AreEqual(1,MainGameProgress.DeathCount,"Changing rooms must preserve the journey total");
  Fall();Assert.AreEqual(2,MainGameProgress.DeathCount);
  MainGameProgress.StartOver();
  Assert.IsTrue(MainGameProgress.IsMainJourney);Assert.AreEqual(0,MainGameProgress.DeathCount,"New Game starts a fresh count");
 }

 [UnityTest] public IEnumerator RecordingCompletionDoesNotCountButRecordingDeathSurvivesWorldRewind(){
  yield return Load(6);
  Step(new InputFrame{retry=true});
  Record();Step(default,true);Step();FinishRewind();
  Assert.AreEqual(0,MainGameProgress.DeathCount,"Saving a recording and rewinding normally are not deaths");
  Step(new InputFrame{retry=true});Assert.AreEqual(0,MainGameProgress.DeathCount);
  Record();Fall();
  Assert.AreEqual(1,MainGameProgress.DeathCount,"Crossing the death plane counts even when recording saves the last moments");
  FinishRewind();
  Assert.AreEqual(1,MainGameProgress.DeathCount,"Restoring recorded world state must not rewind the journey statistic");
  Assert.IsNotNull(replay.Memory);
 }

 [UnityTest] public IEnumerator WaterDeathCountsOnceWhenPauseResetSkipsTheDrowningAnimation(){
  MainGameProgress.GrantReplay();yield return Load(5);
  room.player.ResetAt(new Vector2(35,0));Step();
  Assert.AreEqual(Room4AbilityTutorial.Phase.Drowning,tutorial.CurrentPhase);
  Assert.AreEqual(1,MainGameProgress.DeathCount);
  for(int i=0;i<8;i++)Step();
  Assert.AreEqual(1,MainGameProgress.DeathCount,"Each drowning frame belongs to the same death");
  replay.SetPaused(true);replay.ResetFromPause();
  Assert.AreEqual(Room4AbilityTutorial.Phase.Playing,tutorial.CurrentPhase);
  Assert.AreEqual(1,MainGameProgress.DeathCount,"Reset while already drowning must not count the same death twice");
  replay.SetPaused(true);replay.ResetFromPause();
  Assert.AreEqual(2,MainGameProgress.DeathCount,"A later intentional reset of the living player is a death");
 }

 [UnityTest] public IEnumerator LaunchingTheSameRoomFromDebugDoesNotAffectTheJourneyCount(){
  yield return Load(1);Fall();Assert.AreEqual(1,MainGameProgress.DeathCount);
  MainGameProgress.GrantReplay();
  EditorSceneManager.LoadSceneInPlayMode("Assets/_OneMoreTime/Scenes/MainMenu.unity",new LoadSceneParameters(LoadSceneMode.Single));
  yield return null;
  var menu=Object.FindFirstObjectByType<MenuScreen>();Assert.IsNotNull(menu);
  menu.SendMessage("OpenTutorial",10);
  yield return null;
  Assert.AreEqual("Main_Room05",SceneManager.GetActiveScene().name);
  PrepareRoom();
  Assert.IsFalse(MainGameProgress.IsMainJourney,"A main-room scene entered through Debug is not the campaign journey");
  Assert.IsTrue(MainGameProgress.IsDebugMode);
  Assert.IsFalse(tutorial.Unlocked,"Debug entry must not inherit the previous journey's replay pickup");
  for(int i=0;i<250&&tutorial.CurrentPhase==Room4AbilityTutorial.Phase.Approach;i++)Step(new InputFrame{move=1});
  for(int i=0;i<500&&tutorial.CurrentPhase!=Room4AbilityTutorial.Phase.AwaitRecord;i++)Step();
  Assert.AreEqual(Room4AbilityTutorial.Phase.AwaitRecord,tutorial.CurrentPhase);
  for(int i=0;i<13;i++)Step(default,true);
  for(int i=0;i<150&&tutorial.CurrentPhase!=Room4AbilityTutorial.Phase.Playing;i++)Step();
  Assert.AreEqual(Room4AbilityTutorial.Phase.Playing,tutorial.CurrentPhase);
  room.player.ResetAt(new Vector2(35,0));Step();
  Assert.AreEqual(Room4AbilityTutorial.Phase.Drowning,tutorial.CurrentPhase);
  Assert.AreEqual(1,MainGameProgress.DeathCount);
  replay.SetPaused(true);replay.ResetFromPause();
  replay.SetPaused(true);replay.ResetFromPause();
  Assert.AreEqual(1,MainGameProgress.DeathCount,"Debug hazards and instant resets must leave the journey total untouched");
 }
}
}
#endif
