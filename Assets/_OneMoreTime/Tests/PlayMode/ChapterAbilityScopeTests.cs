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
public class ChapterAbilityScopeTests {
 [TearDown] public void ResetSession(){MainGameProgress.ReturnToMenu();}
 IEnumerator Load(int number,bool fixture=false){
  string folder=fixture?"Tests/Fixtures/":"Scenes/";
  EditorSceneManager.LoadSceneInPlayMode("Assets/_OneMoreTime/"+folder+"Main_Room"+number.ToString("00")+".unity",new LoadSceneParameters(LoadSceneMode.Single));
  yield return null;
 }
 [UnityTest] public IEnumerator CampaignReplayStartsAfterAcquisitionAndNoRoomRequiresTheFilter(){
  MainGameProgress.StartOver();
  foreach(int number in new[]{4,6,7,8}){
   yield return Load(number);
   var chapter=Object.FindFirstObjectByType<ChapterOneRoom>();chapter.AutoTick=false;
   Assert.IsFalse(chapter.DebugAbilities);Assert.IsFalse(chapter.FilterAvailable);
   Assert.IsFalse(chapter.TrySwitchEra());Assert.IsFalse(chapter.Past);
   Assert.IsFalse(chapter.replay.ReplayUpgradeUnlocked);
   if(chapter.replayUpgradePickup!=null)Assert.IsFalse(chapter.replayUpgradePickup.gameObject.activeInHierarchy);
   foreach(var group in chapter.eraGroups)Assert.IsTrue(group.solids.All(s=>s.enabled),"Chapter route must remain usable without a filter");
   Assert.AreEqual(number>=6,chapter.ReplayAvailable);
   Assert.AreEqual(number>=6?CameraModules.Shutter|CameraModules.Replay:CameraModules.Shutter,CameraEquipment.For(chapter));
   foreach(var source in chapter.replay.participants.OfType<CrateSpawner>()){
    if(number==8){
     Assert.AreEqual(-2,source.water.velocity,"The official finale's stream flows left, as drawn");
     Assert.AreEqual(-2,source.initialHorizontalVelocity);Assert.AreEqual(6,source.spawnInterval);
     Assert.IsFalse(source.disposal.disposeToRight);
     Assert.Greater(source.spawnPoint.position.x,source.water.right-2);
    }else{
     Assert.Greater(source.water.velocity,0);Assert.IsTrue(source.disposal.disposeToRight);
     Assert.Less(source.spawnPoint.position.x,source.water.left+2);
    }
   }
   MainGameProgress.GrantReplayUpgrade();Assert.IsFalse(MainGameProgress.ReplayUpgradeUnlocked,"Campaign cannot grant a debug upgrade");
  }
 }
 [UnityTest] public IEnumerator DebugChapterRetainsItsFilterAndUpgradeButNewJourneyClearsThem(){
  MainGameProgress.BeginDebug();yield return Load(8,fixture:true);
  var chapter=Object.FindFirstObjectByType<ChapterOneRoom>();chapter.AutoTick=false;
  Assert.IsTrue(chapter.DebugAbilities);Assert.IsTrue(chapter.FilterAvailable);Assert.IsTrue(chapter.TrySwitchEra());
  Assert.IsTrue(chapter.replay.ReplayUpgradeUnlocked,"The debug finale supplies the advanced replay variant on entry");
  yield return Load(8,fixture:true);
  chapter=Object.FindFirstObjectByType<ChapterOneRoom>();chapter.AutoTick=false;
  Assert.IsTrue(chapter.replay.ReplayUpgradeUnlocked);
  Assert.AreEqual(CameraModules.Shutter|CameraModules.Replay|CameraModules.AdvancedReplay|CameraModules.PastLens,CameraEquipment.For(chapter));
  MainGameProgress.StartOver();yield return Load(8,fixture:true);
  chapter=Object.FindFirstObjectByType<ChapterOneRoom>();chapter.AutoTick=false;
  Assert.AreEqual(CameraModules.Shutter|CameraModules.Replay,CameraEquipment.For(chapter));
 }
 [UnityTest] public IEnumerator CameraAssemblyTracksRealAcquisitionAndExcludesUnrelatedTools(){
  MainGameProgress.StartOver();yield return Load(2);
  var room=Object.FindFirstObjectByType<MainRoom>();room.AutoTick=false;
  Assert.AreEqual(CameraModules.None,CameraEquipment.For(room));
  room.gameObject.AddComponent<FlashlightAbility>().unlocked=true;
  Assert.AreEqual(CameraModules.None,CameraEquipment.For(room),"A debug component must not appear in the chapter's camera");
  room.player.ResetAt(room.shutter.pickup.position);room.shutter.CheckPickup();
  Assert.AreEqual(CameraModules.Shutter,CameraEquipment.For(room));
  yield return Load(5);
  var tutorial=Object.FindFirstObjectByType<Room4AbilityTutorial>();tutorial.AutoTick=false;
  Assert.AreEqual(CameraModules.Shutter,CameraEquipment.For(tutorial));
  MainGameProgress.GrantReplay();Assert.AreEqual(CameraModules.Shutter|CameraModules.Replay,CameraEquipment.For(tutorial));
 }
}
}
#endif
