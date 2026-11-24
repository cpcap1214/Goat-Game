#if UNITY_EDITOR
using System.Collections;
using NUnit.Framework;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

namespace OneMoreTime.Tests {
[Category("ChapterOneRegression")]
public class CameraEquipmentTests {
 IEnumerator Load(string scene,bool fixture=false){
  string folder=fixture?"Tests/Fixtures/":"Scenes/";
  EditorSceneManager.LoadSceneInPlayMode("Assets/_OneMoreTime/"+folder+scene+".unity",new LoadSceneParameters(LoadSceneMode.Single));
  yield return null;
 }
 [SetUp] public void ResetProgress(){MainGameProgress.ReturnToMenu();}
 [TearDown] public void ClearProgress(){MainGameProgress.ReturnToMenu();}

 [UnityTest] public IEnumerator MainCameraAddsOnlyModulesAfterTheirActualPickup(){
  yield return Load("Main_Room02");
  var room=Object.FindFirstObjectByType<MainRoom>();room.AutoTick=false;
  Assert.AreEqual(CameraModules.None,CameraEquipment.For(room));
  room.player.ResetAt(room.shutter.pickup.position);room.shutter.CheckPickup();
  Assert.AreEqual(CameraModules.Shutter,CameraEquipment.For(room));
  yield return Load("Main_Room05");
  var tutorial=Object.FindFirstObjectByType<Room4AbilityTutorial>();tutorial.AutoTick=false;
  Assert.IsFalse((CameraEquipment.For(tutorial)&CameraModules.Replay)!=0,"The replay component exists before the pickup but is still locked");
  MainGameProgress.GrantReplay();
  Assert.IsTrue((CameraEquipment.For(tutorial)&CameraModules.Replay)!=0);
  Assert.IsFalse((CameraEquipment.For(tutorial)&CameraModules.AdvancedReplay)!=0);
 }

 [UnityTest] public IEnumerator ReplayDebugUsesItsLocalUpgradeInsteadOfLeakingCampaignProgress(){
  MainGameProgress.GrantReplayUpgrade();
  yield return Load("Debug_Rewind");
  var replay=Object.FindFirstObjectByType<RewindPlaytest>();replay.AutoTick=false;
  Assert.AreEqual(CameraModules.Replay,CameraEquipment.For(replay));
  replay.UnlockReplayUpgrade();
  Assert.AreEqual(CameraModules.Replay|CameraModules.AdvancedReplay,CameraEquipment.For(replay));
 }

 [UnityTest] public IEnumerator FilterAndFlashModulesFollowTheirOwnRoomAvailability(){
  MainGameProgress.BeginDebug();
  yield return Load("Main_Room08",fixture:true);
  var chapter=Object.FindFirstObjectByType<ChapterOneRoom>();chapter.AutoTick=false;
  Assert.IsTrue((CameraEquipment.For(chapter)&CameraModules.PastLens)!=0);
  Assert.IsTrue((CameraEquipment.For(chapter)&CameraModules.Replay)!=0);
  chapter.filterEnabled=false;
  Assert.IsFalse((CameraEquipment.For(chapter)&CameraModules.PastLens)!=0);
  yield return Load("Debug_ChapterTwoFlashlight");
  var flashlight=Object.FindFirstObjectByType<ChapterTwoFlashlightPlaytest>();flashlight.AutoTick=false;
  Assert.AreEqual(CameraModules.Flashlight,CameraEquipment.For(flashlight));
  flashlight.flashlight.unlocked=false;
  Assert.AreEqual(CameraModules.None,CameraEquipment.For(flashlight));
 }
}
}
#endif
