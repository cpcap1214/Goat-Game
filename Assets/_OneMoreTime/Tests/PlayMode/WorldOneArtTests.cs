#if UNITY_EDITOR
using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
namespace OneMoreTime.Tests {
public class WorldOneArtTests {
 const string Root="Assets/_OneMoreTime/";
 [UnityTest]public IEnumerator ArtLoopsBackgroundAndCheckpoint(){
  EditorSceneManager.LoadSceneInPlayMode(Root+"Scenes/Debug_WorldOneArt.unity",new LoadSceneParameters(LoadSceneMode.Single));yield return null;
  var gallery=Object.FindFirstObjectByType<WorldOneArtGallery>();gallery.enabled=false;
  foreach(string name in new[]{"RiverSurface","WaterfallLip","WaterfallBody","WaterfallSplash"}){var tile=AssetDatabase.LoadAssetAtPath<WaterAnimationTile>(Root+"Generated/WorldOne/"+name+".asset");Assert.AreEqual(4,tile.frames.Length);foreach(var frame in tile.frames)Assert.NotNull(frame);}
  var texture=AssetDatabase.LoadAssetAtPath<Texture2D>(Root+"Art/WorldOne/WaterAtlas.png");Assert.Less(texture.GetPixel(50,texture.height-50).a,.01f,"Air must be genuinely transparent");
  var drowning=gallery.drowning;drowning.Play();var first=drowning.target.sprite;drowning.Tick(.2f);Assert.AreNotSame(first,drowning.target.sprite);drowning.Tick(2);Assert.IsTrue(drowning.Finished);Assert.AreSame(drowning.frames[5],drowning.target.sprite);
  gallery.statue.ResetDisplay();Assert.AreSame(gallery.statue.frames[0],gallery.statue.target.sprite);gallery.statue.Activate();gallery.statue.Tick(.4f);Assert.AreNotSame(gallery.statue.frames[0],gallery.statue.target.sprite);gallery.statue.Tick(2);Assert.IsTrue(gallery.statue.target.sprite==gallery.statue.frames[4]||gallery.statue.target.sprite==gallery.statue.frames[5]);
  var bg=Object.FindFirstObjectByType<WorldOneBackdrop>();foreach(float y in new[]{5f,27f,50f}){gallery.view.transform.position=new Vector3(10,y,-10);bg.SendMessage("LateUpdate");var bounds=bg.scenery.bounds;Assert.LessOrEqual(bounds.min.y,y-gallery.view.orthographicSize+.001f);Assert.GreaterOrEqual(bounds.max.y,y+gallery.view.orthographicSize-.001f);}
 }
 [UnityTest]public IEnumerator BackgroundRepeatsBeyondOldBoundsAndHandlesNegativeTravel(){
  EditorSceneManager.LoadSceneInPlayMode(Root+"Scenes/Debug_WorldOneArt.unity",new LoadSceneParameters(LoadSceneMode.Single));yield return null;
  var gallery=Object.FindFirstObjectByType<WorldOneArtGallery>();gallery.enabled=false;var bg=Object.FindFirstObjectByType<WorldOneBackdrop>();
  Assert.IsTrue(bg.scenery.sharedMaterial.shader.isSupported);
  gallery.view.transform.position=new Vector3(10,5,-10);bg.SendMessage("LateUpdate");var phase=bg.ScrollPhase;
  gallery.view.transform.position+=new Vector3(bg.repeatWidth/bg.parallax.x,bg.repeatHeight/bg.parallax.y,0);bg.SendMessage("LateUpdate");Assert.That(Vector2.Distance(phase,bg.ScrollPhase),Is.LessThan(.0001f));
  foreach(float y in new[]{-1000f,0f,45f,2000f}){gallery.view.transform.position=new Vector3(y,y,-10);bg.SendMessage("LateUpdate");Assert.That(bg.ScrollPhase.x,Is.InRange(0f,1f));Assert.That(bg.ScrollPhase.y,Is.InRange(0f,1f));var b=bg.scenery.bounds;Assert.That(b.center.y,Is.EqualTo(y).Within(.001f));Assert.That(b.size.y,Is.EqualTo(gallery.view.orthographicSize*2).Within(.001f));}
 }
 [UnityTest]public IEnumerator DeepWaterDrownsThenRespawnsWithoutReload(){
  EditorSceneManager.LoadSceneInPlayMode(Root+"Scenes/Main_Room01.unity",new LoadSceneParameters(LoadSceneMode.Single));yield return null;
  var room=Object.FindFirstObjectByType<MainRoom>();room.AutoTick=false;for(int i=0;i<300&&room.Phase==MainRoom.RoomPhase.Entering;i++)room.Step(default,.02f);
  var water=Object.Instantiate(AssetDatabase.LoadAssetAtPath<GameObject>(Root+"Prefabs/WorldOne/DeepWaterHazard.prefab")).GetComponent<WaterHazard>();water.transform.position=room.player.Position;room.waterHazards=new[]{water};Physics2D.SyncTransforms();
  room.Step(default,.02f);Assert.IsTrue(room.IsDrowning);Assert.IsFalse(room.visual.renderer2D.enabled);Assert.AreEqual(1,room.Deaths);water.causesDrowning=false;
  for(int i=0;i<80;i++)room.Step(default,.02f);Assert.IsFalse(room.IsDrowning);Assert.IsTrue(room.visual.renderer2D.enabled);Assert.AreEqual(1,room.Deaths);Assert.AreEqual("Main_Room01",SceneManager.GetActiveScene().name);Assert.That(room.player.Position.x,Is.EqualTo(room.start.x).Within(.01f));
 }
}
}
#endif
