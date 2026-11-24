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
public class PastBackgroundTests {
 ChapterOneRoom chapter;
 WorldOneBackdrop backdrop;

 IEnumerator Load(){
  MainGameProgress.BeginDebug();
  // Main-room layouts no longer require past bridges; keep their filter coverage here.
  EditorSceneManager.LoadSceneInPlayMode("Assets/_OneMoreTime/Tests/Fixtures/Main_Room08.unity",new LoadSceneParameters(LoadSceneMode.Single));
  yield return null;
  chapter=Object.FindFirstObjectByType<ChapterOneRoom>();Assert.IsNotNull(chapter);
  chapter.AutoTick=false;chapter.replay.AutoTick=false;chapter.replay.SetPaused(false);
  for(int i=0;i<300&&chapter.CurrentPhase==ChapterOneRoom.Phase.Entering;i++)chapter.Step(default,false);
  Assert.AreEqual(ChapterOneRoom.Phase.Playing,chapter.CurrentPhase);
  backdrop=chapter.GetComponentInChildren<WorldOneBackdrop>();Assert.IsNotNull(backdrop);
  Assert.AreSame(chapter,backdrop.eraSource,"Existing room scenes must discover their own filter controller");
  Refresh();
 }
 void Step(int count){for(int i=0;i<count;i++)chapter.Step(default,false);Refresh();}
 void Refresh(){backdrop.SendMessage("LateUpdate");}
 float BackgroundAmount=>backdrop.scenery.sharedMaterial.GetFloat("_PastAmount");

 [UnityTest] public IEnumerator BackgroundFollowsLensPauseAndMidTransitionReversal(){
  yield return Load();
  var texture=backdrop.scenery.sprite.texture;
  var phase=backdrop.ScrollPhase;
  var span=backdrop.scenery.sharedMaterial.GetVector("_RepeatSpan");
  var scale=backdrop.scenery.transform.localScale;
  Assert.AreEqual(0,BackgroundAmount);
  Assert.IsTrue(chapter.TrySwitchEra());Step(5);
  Assert.That(BackgroundAmount,Is.GreaterThan(0).And.LessThan(1));
  Assert.That(BackgroundAmount,Is.EqualTo(chapter.Lens.PastAmount).Within(.0001f));
  float mid=BackgroundAmount;
  chapter.replay.SetPaused(true);Step(30);
  Assert.AreEqual(mid,BackgroundAmount,"The background transition must pause with the lens and world");
  chapter.replay.SetPaused(false);
  Assert.IsTrue(chapter.TrySwitchEra());Refresh();
  Assert.AreEqual(mid,BackgroundAmount,"Reversing a switch cannot jump the background blend");
  Step(20);Assert.AreEqual(0,BackgroundAmount);
  Assert.IsTrue(chapter.TrySwitchEra());Step(20);Assert.AreEqual(1,BackgroundAmount);
  Assert.AreSame(texture,backdrop.scenery.sprite.texture,"The present sprite must remain intact");
  Assert.AreEqual(phase,backdrop.ScrollPhase);
  Assert.AreEqual(span,backdrop.scenery.sharedMaterial.GetVector("_RepeatSpan"));
  Assert.AreEqual(scale,backdrop.scenery.transform.localScale);
 }

 [UnityTest] public IEnumerator RejectedSwitchWorldRewindAndRetryKeepBackgroundInTheCorrectEra(){
  yield return Load();
  Vector2 start=chapter.room.player.Position;
  var group=chapter.eraGroups.First(g=>g.activeInPast);
  var solid=group.solids.OfType<BoxCollider2D>().First();
  Vector2 center=solid.transform.TransformPoint(solid.offset);
  chapter.room.player.ResetAt(center-Vector2.up*chapter.room.player.tuning.height*.5f);
  Assert.IsFalse(chapter.TrySwitchEra());Refresh();
  Assert.AreEqual(0,BackgroundAmount,"A collision-rejected filter switch must not alter the background");
  chapter.room.player.ResetAt(start);
  for(int i=0;i<13;i++)chapter.Step(default,true);
  chapter.Step(default,false);
  Assert.AreEqual(RewindPlaytest.Mode.Recording,chapter.replay.State);
  Assert.IsTrue(chapter.TrySwitchEra());Step(30);Assert.AreEqual(1,BackgroundAmount);
  chapter.replay.FinishRecording();
  for(int i=0;i<200&&chapter.replay.State==RewindPlaytest.Mode.Rewinding;i++)chapter.Step(default,false);
  Assert.AreEqual(RewindPlaytest.Mode.Ready,chapter.replay.State);
  Assert.IsFalse(chapter.Past);Step(20);Assert.AreEqual(0,BackgroundAmount);
  var recording=chapter.replay.Memory;
  Assert.IsTrue(chapter.TrySwitchEra());Step(20);Assert.AreEqual(1,BackgroundAmount);
  chapter.Step(new InputFrame{retry=true},false);Step(20);
  Assert.IsFalse(chapter.Past);Assert.AreEqual(0,BackgroundAmount);
  Assert.AreSame(recording,chapter.replay.Memory,"A background change must not discard the saved recording");
 }

 [UnityTest] public IEnumerator RenderedPastPlaceholderAndOptionalArtPreserveThePresentAndLoopPhase(){
  yield return Load();
  var cameraObject=new GameObject("Past background render camera");
  var camera=cameraObject.AddComponent<Camera>();camera.enabled=false;camera.orthographic=true;camera.orthographicSize=5;camera.aspect=1;
  camera.transform.position=new Vector3(10,5,-10);camera.cullingMask=1<<31;camera.clearFlags=CameraClearFlags.SolidColor;camera.backgroundColor=Color.magenta;
  var target=new RenderTexture(64,64,24);camera.targetTexture=target;
  var read=new Texture2D(64,64,TextureFormat.RGBA32,false);
  var replacement=new Texture2D(32,16,TextureFormat.RGBA32,false){filterMode=FilterMode.Point,wrapMode=TextureWrapMode.Repeat};
  replacement.SetPixels(Enumerable.Repeat(new Color(.12f,.76f,.21f,1),32*16).ToArray());replacement.Apply();
  Camera originalView=backdrop.view;int originalLayer=backdrop.scenery.gameObject.layer;Texture2D originalPast=backdrop.pastBackground;
  var previousTarget=RenderTexture.active;
  Color32[] Capture(){Refresh();camera.Render();RenderTexture.active=target;read.ReadPixels(new Rect(0,0,64,64),0,0);read.Apply();RenderTexture.active=previousTarget;return read.GetPixels32();}
  try {
   backdrop.view=camera;backdrop.scenery.gameObject.layer=31;backdrop.pastBackground=null;
   var present=Capture();var phase=backdrop.ScrollPhase;var bounds=backdrop.scenery.bounds;
   Assert.IsTrue(chapter.TrySwitchEra());chapter.Lens.Tick(PastLensEffect.Duration);
   var placeholder=Capture();
   Assert.IsFalse(present.SequenceEqual(placeholder),"Past mode must visibly change the scenery even before new art is selected");
   backdrop.pastBackground=replacement;
   var alternate=Capture();
   Assert.IsFalse(placeholder.SequenceEqual(alternate),"Assigned past art must actually render instead of the placeholder");
   Assert.AreEqual(1,backdrop.scenery.sharedMaterial.GetFloat("_HasPastBackground"));
   Assert.AreSame(replacement,backdrop.scenery.sharedMaterial.GetTexture("_PastTex"));
   Assert.AreEqual(phase,backdrop.ScrollPhase);Assert.AreEqual(bounds,backdrop.scenery.bounds);
   Assert.IsTrue(chapter.TrySwitchEra());chapter.Lens.Tick(PastLensEffect.Duration);
   CollectionAssert.AreEqual(present,Capture(),"Turning the filter off must restore the exact present render");
   Assert.AreEqual(phase,backdrop.ScrollPhase);
  } finally {
   backdrop.view=originalView;backdrop.scenery.gameObject.layer=originalLayer;backdrop.pastBackground=originalPast;
   RenderTexture.active=previousTarget;camera.targetTexture=null;
   Object.DestroyImmediate(read);Object.DestroyImmediate(replacement);target.Release();Object.DestroyImmediate(target);Object.DestroyImmediate(cameraObject);
  }
 }
}
}
#endif
