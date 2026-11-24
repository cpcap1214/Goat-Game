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
public class FloatingTimberTests {
 const string Folder = "Assets/_OneMoreTime/Prefabs/WorldOne/Expansion/";
 static readonly string[] Variants = { "FloatingLog", "MossyFloatingLog", "BoundLogRaft", "FloatingTimberPlank" };
 Room4AbilityTutorial tutorial;
 RewindPlaytest replay;
 CrateSpawner source;
 const float Dt = .02f;

 IEnumerator LoadWithTimber(string variant = "FloatingLog") {
  MainGameProgress.StartOver(); MainGameProgress.GrantReplay();
  EditorSceneManager.LoadSceneInPlayMode("Assets/_OneMoreTime/Scenes/Main_Room05.unity", new LoadSceneParameters(LoadSceneMode.Single));
  yield return null;
  tutorial = Object.FindFirstObjectByType<Room4AbilityTutorial>();
  tutorial.AutoTick = false;
  replay = tutorial.replay;
  for (int i = 0; i < 150 && tutorial.CurrentPhase == Room4AbilityTutorial.Phase.Arriving; i++) tutorial.Step(default, false, Dt);
  var oldSource = tutorial.crateSource;
  foreach (var old in oldSource.pool) Object.DestroyImmediate(old.gameObject);
  source = oldSource.gameObject.AddComponent<CrateSpawner>();
  source.water = oldSource.water;
  source.spawnPoint = oldSource.spawnPoint;
  source.disposal = oldSource.disposal;
  source.disposal.owner = source;
  replay.participants = replay.participants.Select(p => p == oldSource ? source : p).ToArray();
  tutorial.crateSource = source;
  Object.DestroyImmediate(oldSource);
  var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(Folder + variant + ".prefab");
  Assert.IsNotNull(prefab, "Run FloatingTimberBuilder.Build first.");
  source.pool = new FloatingObject2D[6];
  for (int i = 0; i < source.pool.Length; i++) {
   var item = Object.Instantiate(prefab, source.transform);
   source.pool[i] = item.GetComponent<FloatingObject2D>();
   source.pool[i].water = source.water;
   item.SetActive(false);
  }
  replay.SetRetryAnchor();
  replay.UnlockReplayUpgrade();
 }

 [Test] public void TimberPrefabsHaveAlignedOneWayDecksAndCompleteSources() {
  foreach (string variant in Variants) {
   var platform = AssetDatabase.LoadAssetAtPath<GameObject>(Folder + variant + ".prefab");
   Assert.IsNotNull(platform, variant);
   Assert.IsNotNull(platform.GetComponent<OneWayPlatform>());
   Assert.IsNotNull(platform.GetComponent<FloatingObject2D>());
   var box = platform.GetComponent<BoxCollider2D>();
   Assert.That(box.size.x, Is.InRange(2.699f, 3.601f));
   Assert.That(box.offset.y + box.size.y / 2, Is.EqualTo(0).Within(.0001f));
   Assert.AreEqual(RigidbodyType2D.Kinematic, platform.GetComponent<Rigidbody2D>().bodyType);
   var visual = platform.GetComponent<SpriteRenderer>();
   Assert.IsNotNull(visual.sprite);
   Assert.AreEqual(Color.white, visual.color);
   Assert.AreEqual("OneMoreTime/Terrain Pixel Scale", visual.sharedMaterial.shader.name);
   var background = AssetDatabase.LoadAssetAtPath<GameObject>(Folder + "Background/" + variant + "Background.prefab");
   Assert.IsNotNull(background);
   Assert.IsEmpty(background.GetComponentsInChildren<MonoBehaviour>(true));
   Assert.IsEmpty(background.GetComponentsInChildren<Collider2D>(true));
   Assert.IsEmpty(background.GetComponentsInChildren<Rigidbody2D>(true));
   Assert.AreEqual(new Color(.5f, .5f, .5f, 1), background.GetComponent<SpriteRenderer>().color);
   Assert.AreEqual(-20, background.GetComponent<SpriteRenderer>().sortingOrder);
   var stream = AssetDatabase.LoadAssetAtPath<GameObject>(Folder + variant + "Source.prefab").GetComponent<CrateSpawner>();
   Assert.IsTrue(stream.affectedByRewind);
   Assert.AreEqual(10, stream.spawnInterval);
   Assert.AreEqual(-2, stream.water.velocity);
   Assert.AreEqual(6, stream.pool.Length);
   Assert.AreSame(stream, stream.disposal.owner);
   Assert.IsTrue(stream.pool.All(item => item.water == stream.water && !item.gameObject.activeSelf));
  }
 }

 [UnityTest] public IEnumerator TimberFloatsCarriesPlayerAndAllowsDropThrough() {
  yield return LoadWithTimber("BoundLogRaft");
  for (int i = 0; i < 60; i++) replay.Step(default, false);
  var item = source.pool.First(x => x.gameObject.activeSelf);
  Assert.That(item.transform.position.y, Is.InRange(4.1f, 4.6f));
  var deck = item.GetComponent<BoxCollider2D>();
  replay.player.ResetAt(new Vector2(item.transform.position.x, deck.bounds.max.y + .01f));
  Physics2D.SyncTransforms();
  for (int i = 0; i < 3; i++) replay.Step(default, false);
  Assert.IsTrue(replay.player.Grounded);
  Assert.AreSame(deck, replay.player.GroundCollider);
  float playerX = replay.player.Position.x, timberX = item.transform.position.x;
  for (int i = 0; i < 15; i++) replay.Step(default, false);
  Assert.That(replay.player.Position.x - playerX, Is.EqualTo(item.transform.position.x - timberX).Within(.01f));
  replay.Step(new InputFrame { drop = true, jump = true }, false);
  for (int i = 0; i < 25; i++) replay.Step(default, false);
  Assert.Less(replay.player.Position.y + replay.player.tuning.height, deck.bounds.min.y);
 }

 [UnityTest] public IEnumerator TimberFollowsForwardReverseHeldReplayAndKeepsItsColors() {
  yield return LoadWithTimber();
  for (int i = 0; i < 50; i++) replay.Step(default, false);
  for (int i = 0; i < 13; i++) replay.Step(default, true);
  replay.Step(default, false);
  for (int i = 0; i < 80; i++) replay.Step(default, false);
  var item = source.pool.First(x => x.gameObject.activeSelf);
  var sprite = item.GetComponent<SpriteRenderer>();
  float endX = item.transform.position.x;
  replay.Step(default, true); replay.Step(default, false);
  for (int i = 0; i < 100 && replay.State == RewindPlaytest.Mode.Rewinding; i++) {
   replay.Step(default, false);
   Assert.AreEqual(Color.white, sprite.color);
  }
  Assert.AreEqual(RewindPlaytest.Mode.Ready, replay.State);
  Assert.Greater(item.transform.position.x, endX + 2);
  replay.Step(default, true); replay.Step(default, false);
  for (int i = 0; i < 25; i++) replay.Step(default, false);
  float forward = item.transform.position.x;
  for (int i = 0; i < 20; i++) replay.Step(default, false, false, true);
  Assert.AreEqual(forward, item.transform.position.x);
  replay.Step(default, false, true);
  for (int i = 0; i < 15; i++) replay.Step(default, false);
  Assert.Greater(item.transform.position.x, forward + .5f);
  Assert.AreEqual(Color.white, sprite.color);
  Assert.Greater(replay.transform.Find("Object replay traces").GetComponentsInChildren<LineRenderer>().Length, 0);
 }
}
}
#endif
