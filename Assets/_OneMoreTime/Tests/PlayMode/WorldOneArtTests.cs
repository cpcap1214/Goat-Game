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
  foreach(string name in new[]{"RiverSurface","WaterfallLip","WaterfallBody","WaterfallSplash","DiagonalStream"}){var tile=AssetDatabase.LoadAssetAtPath<WaterAnimationTile>(Root+"Generated/WorldOne/"+name+".asset");Assert.AreEqual(4,tile.frames.Length);foreach(var frame in tile.frames)Assert.NotNull(frame);}
  var waterMaterial=AssetDatabase.LoadAssetAtPath<Material>(Root+"Generated/WorldOne/WaterAtlas.mat");Assert.AreEqual("OneMoreTime/Looping Water",waterMaterial.shader.name);Assert.IsTrue(waterMaterial.shader.isSupported);Assert.AreEqual(4,waterMaterial.GetFloat("_FramesPerSecond"));
  var sideways=AssetDatabase.LoadAssetAtPath<WaterAnimationTile>(Root+"Generated/WorldOne/SidewaysWaterfallBody.asset");var vertical=AssetDatabase.LoadAssetAtPath<WaterAnimationTile>(Root+"Generated/WorldOne/WaterfallBody.asset");CollectionAssert.AreEqual(vertical.frames,sideways.frames);var tileData=new UnityEngine.Tilemaps.TileData();sideways.GetTileData(Vector3Int.zero,null,ref tileData);Assert.That(Vector3.Distance(tileData.transform.MultiplyVector(Vector3.down),Vector3.right),Is.LessThan(.001f));Assert.AreEqual(UnityEngine.Tilemaps.Tile.ColliderType.None,tileData.colliderType);
  var texture=AssetDatabase.LoadAssetAtPath<Texture2D>(Root+"Art/WorldOne/WaterAtlas.png");Assert.Less(texture.GetPixel(50,texture.height-50).a,.01f,"Air must be genuinely transparent");
  foreach(string asset in new[]{"WoodenSuspensionBridge","GrassDirtLedge","StoneGoatCheckpoint"}){
   var prefab=AssetDatabase.LoadAssetAtPath<GameObject>(Root+"Prefabs/WorldOne/"+asset+".prefab");
   foreach(var sr in prefab.GetComponentsInChildren<SpriteRenderer>()){
    Assert.AreEqual("OneMoreTime/Terrain Pixel Scale",sr.sharedMaterial.shader.name);
    Assert.IsTrue(sr.sharedMaterial.shader.isSupported);
    var grid=sr.sharedMaterial.GetVector("_PixelGrid");
    Assert.That(grid.x/(sr.sprite.texture.width/sr.sprite.pixelsPerUnit),Is.EqualTo(16).Within(.001f));
    Assert.That(grid.y/(sr.sprite.texture.height/sr.sprite.pixelsPerUnit),Is.EqualTo(16).Within(.001f));
   }
  }
  var drowning=gallery.drowning;drowning.Play();var first=drowning.target.sprite;drowning.Tick(.2f);Assert.AreNotSame(first,drowning.target.sprite);drowning.Tick(2);Assert.IsTrue(drowning.Finished);Assert.AreSame(drowning.frames[5],drowning.target.sprite);
  gallery.statue.ResetDisplay();Assert.AreSame(gallery.statue.frames[0],gallery.statue.target.sprite);gallery.statue.Activate();gallery.statue.Tick(.4f);Assert.AreNotSame(gallery.statue.frames[0],gallery.statue.target.sprite);gallery.statue.Tick(2);Assert.IsTrue(gallery.statue.target.sprite==gallery.statue.frames[4]||gallery.statue.target.sprite==gallery.statue.frames[5]);
  var bg=Object.FindFirstObjectByType<WorldOneBackdrop>();foreach(float y in new[]{5f,27f,50f}){gallery.view.transform.position=new Vector3(10,y,-10);bg.SendMessage("LateUpdate");var bounds=bg.scenery.bounds;Assert.LessOrEqual(bounds.min.y,y-gallery.view.orthographicSize+.001f);Assert.GreaterOrEqual(bounds.max.y,y+gallery.view.orthographicSize-.001f);}
 }
 [UnityTest]public IEnumerator BackgroundRepeatsBeyondOldBoundsAndHandlesNegativeTravel(){
  EditorSceneManager.LoadSceneInPlayMode(Root+"Scenes/Debug_WorldOneArt.unity",new LoadSceneParameters(LoadSceneMode.Single));yield return null;
  var gallery=Object.FindFirstObjectByType<WorldOneArtGallery>();gallery.enabled=false;var bg=Object.FindFirstObjectByType<WorldOneBackdrop>();
  Assert.IsTrue(bg.scenery.sharedMaterial.shader.isSupported);
  Assert.That(bg.repeatWidth/bg.repeatHeight,Is.EqualTo(3).Within(.001f));
  Assert.That(bg.scenery.sprite.texture.width/(float)bg.scenery.sprite.texture.height,Is.EqualTo(3).Within(.001f));
  gallery.view.transform.position=new Vector3(10,5,-10);bg.SendMessage("LateUpdate");var phase=bg.ScrollPhase;
  gallery.view.transform.position+=new Vector3(bg.repeatWidth/bg.parallax.x,bg.repeatHeight/bg.parallax.y,0);bg.SendMessage("LateUpdate");Assert.That(Vector2.Distance(phase,bg.ScrollPhase),Is.LessThan(.0001f));
  foreach(float y in new[]{-1000f,0f,45f,2000f}){gallery.view.transform.position=new Vector3(y,y,-10);bg.SendMessage("LateUpdate");Assert.That(bg.ScrollPhase.x,Is.InRange(0f,1f));Assert.That(bg.ScrollPhase.y,Is.InRange(0f,1f));var b=bg.scenery.bounds;Assert.That(b.center.y,Is.EqualTo(y).Within(.001f));Assert.That(b.size.y,Is.EqualTo(gallery.view.orthographicSize*2).Within(.001f));}
 }
 [UnityTest]public IEnumerator WaterShaderCycleAndDiagonalSilhouette(){
  var go=new GameObject("Water render test");go.layer=31;
  var sr=go.AddComponent<SpriteRenderer>();
  sr.sprite=AssetDatabase.LoadAssetAtPath<WaterAnimationTile>(Root+"Generated/WorldOne/RiverSurface.asset").frames[0];
  var cameraObject=new GameObject("Water test camera");var camera=cameraObject.AddComponent<Camera>();camera.enabled=false;camera.orthographic=true;camera.orthographicSize=.5f;camera.aspect=1;camera.transform.position=new Vector3(0,0,-10);camera.cullingMask=1<<31;camera.clearFlags=CameraClearFlags.SolidColor;camera.backgroundColor=Color.clear;
  var target=new RenderTexture(64,64,24);camera.targetTexture=target;
  var read=new Texture2D(64,64,TextureFormat.RGBA32,false);
  Color32[] Capture(Material material,float time){material.SetFloat("_PreviewTime",time);sr.sharedMaterial=material;camera.Render();RenderTexture.active=target;read.ReadPixels(new Rect(0,0,64,64),0,0);read.Apply();RenderTexture.active=null;return read.GetPixels32();}
  foreach(string name in new[]{"WaterAtlas","FullBlockWater","DiagonalWater"}){
   var material=new Material(AssetDatabase.LoadAssetAtPath<Material>(Root+"Generated/WorldOne/"+name+".mat"));Assert.IsTrue(material.shader.isSupported,name);
   var first=Capture(material,0);var last=Capture(material,1);CollectionAssert.AreEqual(first,last,name+" must close the one-second loop exactly");
   var next=Capture(material,.25f);Assert.IsFalse(System.Linq.Enumerable.SequenceEqual(first,next),name+" must animate");
   if(name=="DiagonalWater"){Assert.Less(first[56*64+56].a,5,"Air above slope");Assert.Greater(first[8*64+8].a,250,"Water below slope");}
   Object.DestroyImmediate(material);
  }
  var diagonal=AssetDatabase.LoadAssetAtPath<WaterAnimationTile>(Root+"Generated/WorldOne/DiagonalStream.asset");var river=AssetDatabase.LoadAssetAtPath<WaterAnimationTile>(Root+"Generated/WorldOne/RiverSurface.asset");CollectionAssert.AreEqual(river.frames,diagonal.frames);Assert.AreEqual(river.framesPerSecond,diagonal.framesPerSecond);
  camera.targetTexture=null;Object.DestroyImmediate(read);Object.DestroyImmediate(target);Object.DestroyImmediate(cameraObject);Object.DestroyImmediate(go);yield return null;
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
