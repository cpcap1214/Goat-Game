#if UNITY_EDITOR
using System.Collections;
using System.IO;
using System.Linq;
using NUnit.Framework;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

namespace OneMoreTime.Tests {
public class AbilityArtTests {
 GameObject fixture;
 [TearDown] public void Cleanup(){if(fixture!=null)Object.DestroyImmediate(fixture);MainGameProgress.ReturnToMenu();}

 [TestCase(AbilityArt.ShutterCamera)]
 [TestCase(AbilityArt.ReplayTape)]
 [TestCase(AbilityArt.ArcaneReplayTape)]
 [TestCase(AbilityArt.FlashModule)]
 [TestCase(AbilityArt.PastLens)]
 public void ResourcesAreSmallPointFilteredSpritesWithTransparentArtwork(string name){
  var sprite=AbilityArt.Sprite(name);var texture=AbilityArt.Texture(name);
  Assert.IsNotNull(sprite,name+" must load as a Sprite");Assert.IsNotNull(texture);
  Assert.AreSame(texture,sprite.texture);
  Assert.AreEqual(FilterMode.Point,texture.filterMode);Assert.AreEqual(1,texture.mipmapCount);
  Assert.That(texture.width,Is.InRange(1,64));Assert.That(texture.height,Is.InRange(1,64));
  Assert.That(sprite.pixelsPerUnit,Is.EqualTo(64).Within(2),"Effective PPU must match the 64px runtime canvas after Unity downsampling");
  Assert.That(Mathf.Max(sprite.bounds.size.x,sprite.bounds.size.y),Is.EqualTo(1).Within(.02f),"The reusable sprite canvas must be one world unit without a corrective transform");
  // Runtime sprites need not be CPU-readable. Inspect the source without changing
  // its import settings or making gameplay keep an extra readable texture copy.
  var source=new Texture2D(2,2,TextureFormat.RGBA32,false);
  try{
   Assert.IsTrue(ImageConversion.LoadImage(source,File.ReadAllBytes(AssetDatabase.GetAssetPath(texture))));
   var importer=(TextureImporter)AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(texture));
   Assert.AreEqual(Mathf.Max(source.width,source.height),importer.spritePixelsPerUnit,"Source PPU must define the one-unit canvas before import downsampling");
   Assert.AreEqual(TextureImporterNPOTScale.None,importer.npotScale);
   Assert.AreEqual(TextureImporterCompression.Uncompressed,importer.textureCompression);
   var pixels=source.GetPixels32();
   Assert.IsTrue(pixels.Any(p=>p.a==0),name+" needs empty transparent space around the item");
   Assert.IsTrue(pixels.Any(p=>p.a>=250),name+" needs an opaque, readable item silhouette");
   Assert.That((float)texture.width/texture.height,Is.EqualTo((float)source.width/source.height).Within(.04f),"Import must preserve the non-square magic tape's aspect ratio");
  }finally{Object.DestroyImmediate(source);}
 }

 [Test] public void ReplacingArtPreservesRootCollisionAndAspectWithoutDuplicateVisuals(){
  fixture=new GameObject("Authored pickup fixture");var root=fixture.transform;
  root.position=new Vector3(12,7,0);root.localScale=new Vector3(2,3,1);
  var collider=fixture.AddComponent<BoxCollider2D>();collider.size=new Vector2(.76f,.5f);collider.offset=new Vector2(.2f,.1f);collider.isTrigger=true;
  var oldChild=new GameObject("Authored original art").transform;oldChild.SetParent(root,false);oldChild.localPosition=new Vector3(.1f,.3f,0);oldChild.localScale=new Vector3(.2f,.3f,1);
  var original=oldChild.gameObject.AddComponent<SpriteRenderer>();original.sprite=AbilityArt.Sprite(AbilityArt.ShutterCamera);original.sortingOrder=17;
  Physics2D.SyncTransforms();var position=root.position;var scale=root.localScale;var collision=collider.bounds;var originalCenter=original.bounds.center;
  var replacement=AbilityArt.ReplacePickup(root,AbilityArt.ArcaneReplayTape);
  Assert.IsNotNull(replacement);Physics2D.SyncTransforms();
  Assert.AreEqual(position,root.position);Assert.AreEqual(scale,root.localScale);
  Assert.AreEqual(collision,collider.bounds);Assert.AreEqual(new Vector2(.76f,.5f),collider.size);Assert.AreEqual(new Vector2(.2f,.1f),collider.offset);Assert.IsTrue(collider.isTrigger);
  Assert.AreEqual(1,fixture.GetComponentsInChildren<Collider2D>(true).Length);
  Assert.IsFalse(original.enabled);Assert.IsTrue(replacement.enabled);Assert.AreEqual(17,replacement.sortingOrder);
  Assert.That(Vector3.Distance(originalCenter,replacement.bounds.center),Is.LessThan(.001f));
  Assert.That(replacement.bounds.size.x/replacement.bounds.size.y,Is.EqualTo(replacement.sprite.bounds.size.x/replacement.sprite.bounds.size.y).Within(.001f),"Nonuniform authored root scale must not stretch the magic tape");
  int children=root.childCount;
  Assert.AreSame(replacement,AbilityArt.ReplacePickup(root,AbilityArt.ArcaneReplayTape));Assert.AreEqual(children,root.childCount);
  Assert.AreEqual(1,fixture.GetComponentsInChildren<SpriteRenderer>(true).Count(sr=>sr.enabled));
 }

 [UnityTest] public IEnumerator RoomFourShowsTheTapeUntilTheRealAcquisitionHidesThePickup(){
  MainGameProgress.StartOver();
  EditorSceneManager.LoadSceneInPlayMode("Assets/_OneMoreTime/Scenes/Main_Room05.unity",new LoadSceneParameters(LoadSceneMode.Single));
  yield return null;
  var tutorial=Object.FindFirstObjectByType<Room4AbilityTutorial>();Assert.IsNotNull(tutorial);tutorial.AutoTick=false;
  for(int i=0;i<150&&tutorial.CurrentPhase==Room4AbilityTutorial.Phase.Arriving;i++)tutorial.Step(default,false,.02f);
  Assert.AreEqual(Room4AbilityTutorial.Phase.Approach,tutorial.CurrentPhase);
  var art=tutorial.pickup.GetComponentsInChildren<SpriteRenderer>(true).Single(sr=>sr.enabled);
  Assert.AreEqual("Camera ability sprite",art.name);Assert.AreSame(AbilityArt.Sprite(AbilityArt.ReplayTape),art.sprite);Assert.IsTrue(art.gameObject.activeInHierarchy);
  for(int i=0;i<250&&tutorial.CurrentPhase==Room4AbilityTutorial.Phase.Approach;i++)tutorial.Step(new InputFrame{move=1,held=true},false,.02f);
  Assert.IsTrue(tutorial.Unlocked);Assert.AreEqual(Room4AbilityTutorial.Phase.Breaking,tutorial.CurrentPhase);
  Assert.IsFalse(tutorial.pickup.gameObject.activeSelf);Assert.IsFalse(art.gameObject.activeInHierarchy);Assert.IsFalse(tutorial.abilityBlocker.enabled);
 }
}
}
#endif
