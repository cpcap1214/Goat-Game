#if UNITY_EDITOR
using System.Collections;
using System.Linq;
using NUnit.Framework;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

namespace OneMoreTime.Tests {
[Category("ChapterOneRegression")]
public class WhirlpoolCrateTests {
 const float Dt=.02f;
 GameObject fixture;

 CrateSpawner MakeSource(){
  fixture=new GameObject("Whirlpool test source");
  var source=fixture.AddComponent<CrateSpawner>();
  source.water=fixture.AddComponent<WaterCurrent2D>();
  source.water.left=0;source.water.right=30;source.water.surfaceY=0;source.water.velocity=2.5f;
  var spawn=new GameObject("Source point");spawn.transform.SetParent(fixture.transform);spawn.transform.position=new Vector3(5.05f,.35f,0);source.spawnPoint=spawn.transform;
  var drain=new GameObject("Whirlpool");drain.transform.SetParent(fixture.transform);
  source.disposal=drain.AddComponent<CrateDisposalTrigger>();
  source.disposal.GetComponent<BoxCollider2D>().isTrigger=true;
  source.disposal.owner=source;source.disposal.boundaryX=5;source.disposal.disposeToRight=true;source.disposal.sinkBeforeDisposal=true;
  source.initialHorizontalVelocity=2.5f;source.spawnInterval=10;source.maximumActiveCrates=1;
  var prefab=AssetDatabase.LoadAssetAtPath<GameObject>("Assets/_OneMoreTime/Prefabs/FloatingWaterCrate.prefab");
  Assert.IsNotNull(prefab);
  var item=Object.Instantiate(prefab,fixture.transform);var crate=item.GetComponent<FloatingObject2D>();crate.water=source.water;item.SetActive(false);
  source.pool=new[]{crate};return source;
 }

 [TearDown]public void Cleanup(){if(fixture!=null)Object.DestroyImmediate(fixture);fixture=null;}

 [Test]public void WhirlpoolFallsBeforeDisposalAndRestoresItsFullTimeline(){
  var source=MakeSource();source.Tick(0);var crate=source.pool[0];
  Assert.IsTrue(crate.Sinking);Assert.IsTrue(crate.gameObject.activeSelf,"A whirlpool must not pop the crate out immediately.");
  var before=source.Capture();float startY=crate.transform.position.y;
  for(int i=0;i<20;i++)source.Tick(Dt);
  var middle=source.Capture();Vector2 middlePosition=crate.transform.position;
  Assert.IsTrue(crate.gameObject.activeSelf);Assert.Less(middlePosition.y,startY-.3f);
  Assert.That(middlePosition.x,Is.EqualTo(source.disposal.boundaryX).Within(.001f));
  for(int i=0;i<60;i++)source.Tick(Dt);
  var disposed=source.Capture();Assert.IsFalse(crate.gameObject.activeSelf);
  Assert.That(crate.transform.position.y,Is.EqualTo(-source.disposal.sinkDepth).Within(.001f));
  source.Restore(before);
  for(int i=0;i<20;i++)source.Tick(Dt);
  Assert.That(Vector2.Distance(middlePosition,crate.transform.position),Is.LessThan(.0001f),"A restored sinking frame must resume the same fall.");
  source.RestorePlayback(disposed,false);Assert.IsFalse(crate.gameObject.activeSelf);
  source.RestorePlayback(middle,true);Assert.IsTrue(crate.gameObject.activeSelf);Assert.IsTrue(crate.Sinking);
  Assert.AreEqual(Vector2.zero,crate.CarryDelta,"Reappearing pooled objects cannot teleport a rider.");
  source.RestorePlayback(before,true);
  Assert.Greater(crate.CarryDelta.y,0,"Backward playback visibly lifts the crate out of the whirlpool.");
  crate.Restore(new FloatingObject2D.State{active=true,position=new Vector2(1,.35f),velocity=new Vector2(2.5f,0)});
  Assert.IsFalse(crate.Sinking,"Reusing a pooled crate clears the previous drain state.");
 }

 [Test]public void OrdinaryDisposalStillRemovesImmediatelyAndIgnoresForeignCrates(){
  var source=MakeSource();source.disposal.sinkBeforeDisposal=false;source.Tick(0);
  Assert.IsFalse(source.pool[0].gameObject.activeSelf,"Existing room sources keep their immediate boundaries.");
  source.disposal.sinkBeforeDisposal=true;
  var stranger=Object.Instantiate(source.pool[0],fixture.transform);
  stranger.Restore(new FloatingObject2D.State{active=true,position=new Vector2(6,.35f)});
  source.disposal.Check(stranger);
  Assert.IsTrue(stranger.gameObject.activeSelf);Assert.IsFalse(stranger.Sinking,"A drain owns only its source's pool.");
 }

 [UnityTest]public IEnumerator RecordingAndForwardReversePlaybackIncludeTheWhirlpoolFall(){
  MainGameProgress.StartOver();MainGameProgress.GrantReplay();
  EditorSceneManager.LoadSceneInPlayMode("Assets/_OneMoreTime/Scenes/Main_Room05.unity",new LoadSceneParameters(LoadSceneMode.Single));
  yield return null;
  var tutorial=Object.FindFirstObjectByType<Room4AbilityTutorial>();tutorial.AutoTick=false;
  for(int i=0;i<150&&tutorial.CurrentPhase==Room4AbilityTutorial.Phase.Arriving;i++)tutorial.Step(default,false,Dt);
  var replay=tutorial.replay;replay.AutoTick=false;replay.UnlockReplayUpgrade();
  var source=tutorial.crateSource;
  source.water.left=40;source.water.right=50;source.water.velocity=2.5f;
  source.disposal.boundaryX=43;source.disposal.disposeToRight=true;source.disposal.sinkBeforeDisposal=true;
  foreach(var pooled in source.pool)pooled.gameObject.SetActive(false);
  var crate=source.pool[0];crate.Restore(new FloatingObject2D.State{active=true,position=new Vector2(42,source.water.surfaceY+.35f),velocity=new Vector2(2.5f,0)});
  replay.player.ResetAt(new Vector2(14.5f,4));Physics2D.SyncTransforms();replay.SetRetryAnchor();
  var sprites=crate.GetComponentsInChildren<SpriteRenderer>();var colors=sprites.Select(sr=>sr.color).ToArray();
  for(int i=0;i<13;i++)replay.Step(default,true);
  Assert.AreEqual(RewindPlaytest.Mode.Recording,replay.State);
  replay.Step(default,false);
  for(int i=0;i<120;i++)replay.Step(default,false);
  Assert.IsFalse(crate.gameObject.activeSelf,"The recorded period spans the complete fall and disposal.");
  replay.Step(default,true);replay.Step(default,false);
  for(int i=0;i<150&&replay.State==RewindPlaytest.Mode.Rewinding;i++)replay.Step(default,false);
  Assert.AreEqual(RewindPlaytest.Mode.Ready,replay.State);
  Assert.IsTrue(crate.gameObject.activeSelf);Assert.IsFalse(crate.Sinking);
  Vector2 recordingStart=crate.transform.position;
  replay.Step(default,true);replay.Step(default,false);
  bool sawSinking=false;
  for(int i=0;i<200&&replay.State==RewindPlaytest.Mode.Playing;i++){
   replay.Step(default,false);
   if(!sawSinking&&crate.Sinking&&crate.gameObject.activeSelf){
    sawSinking=true;var held=crate.Capture();
    for(int j=0;j<15;j++)replay.Step(default,false,false,true);
    Assert.AreEqual(held.position,(Vector2)crate.transform.position,"Holding playback freezes the drain too.");
   }
  }
  Assert.IsTrue(sawSinking);Assert.IsFalse(crate.gameObject.activeSelf);
  replay.Step(default,false,true);
  bool roseFromDrain=false;
  for(int i=0;i<200&&replay.State==RewindPlaytest.Mode.Playing;i++){
   replay.Step(default,false);roseFromDrain|=crate.gameObject.activeSelf&&crate.Sinking;
  }
  Assert.IsTrue(roseFromDrain);Assert.IsTrue(crate.gameObject.activeSelf);Assert.IsFalse(crate.Sinking);
  Assert.That(Vector2.Distance(recordingStart,crate.transform.position),Is.LessThan(.001f));
  for(int i=0;i<sprites.Length;i++)Assert.AreEqual(colors[i],sprites[i].color);
 }
}
}
#endif
