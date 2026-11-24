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
public class ChapterBasicTraversalTests {
 ChapterOneRoom chapter;
 PlayerMotor2D player;

 void Step(float move=0,bool jump=false){
  chapter.Step(new InputFrame{move=move,jump=jump,held=true},false);
  Physics2D.SyncTransforms();
 }

 [UnityTest] public IEnumerator HighStreamCrossingNeedsNoDebugUpgradeOrFilter(){
  MainGameProgress.StartOver();
  // This archived crossing tests ordinary raft carry independently of new room layouts.
  EditorSceneManager.LoadSceneInPlayMode("Assets/_OneMoreTime/Tests/Fixtures/Main_Room08.unity",new LoadSceneParameters(LoadSceneMode.Single));
  yield return null;
  chapter=Object.FindFirstObjectByType<ChapterOneRoom>();Assert.IsNotNull(chapter);
  chapter.AutoTick=chapter.room.AutoTick=chapter.replay.AutoTick=false;player=chapter.room.player;
  for(int i=0;i<300&&chapter.CurrentPhase==ChapterOneRoom.Phase.Entering;i++)Step();
  Assert.AreEqual(ChapterOneRoom.Phase.Playing,chapter.CurrentPhase);
  Assert.IsFalse(chapter.DebugAbilities);Assert.IsFalse(chapter.FilterAvailable);Assert.IsFalse(chapter.replay.ReplayUpgradeUnlocked);
  var source=chapter.replay.participants.OfType<CrateSpawner>().OrderBy(s=>s.water.left).First();
  Assert.Greater(source.water.velocity,0,"The ordinary chapter stream must carry toward the next island");
  Assert.That(source.spawnPoint.position.x,Is.LessThan(source.water.left+2));

  // Fixture setup places the goat on the authored high-stream west bank.
  // Walking, falling onto the raft, carrying and dismount use the normal clock.
  player.ResetAt(new Vector2(55.5f,29.51f));Physics2D.SyncTransforms();
  for(int i=0;i<5;i++)Step();
  FloatingObject2D log=null;
  for(int i=0;i<130&&log==null;i++){
   Step(1);
   if(player.GroundCollider!=null)log=player.GroundCollider.GetComponent<FloatingObject2D>();
   Assert.AreEqual(ChapterOneRoom.Phase.Playing,chapter.CurrentPhase,"The bank must connect to the outgoing raft without drowning");
  }
  Assert.IsNotNull(log,"The goat must board a real outgoing raft by walking off the west bank");
  Assert.Contains(log,source.pool);Assert.IsTrue(player.Grounded);
  var deck=log.GetComponent<BoxCollider2D>();float boardX=player.Position.x;
  for(int i=0;i<400&&log.transform.position.x<70.5f;i++){
   Step();
   Assert.AreSame(deck,player.GroundCollider,"The normal motor must remain carried by the log");
   Assert.AreEqual(ChapterOneRoom.Phase.Playing,chapter.CurrentPhase);
  }
  Assert.That(player.Position.x-boardX,Is.GreaterThan(8));
  Assert.That(log.transform.position.x,Is.GreaterThanOrEqualTo(70.5f));

  Step(1,true);Assert.Greater(player.Velocity.y,0);
  for(int i=0;i<100;i++){
   Step(player.Position.x<74?1:0);
   if(i>5&&player.Grounded)break;
  }
  Assert.IsTrue(player.Grounded,"The normal jump must dismount onto the next island");
  Assert.AreNotSame(deck,player.GroundCollider);Assert.That(player.Position.x,Is.InRange(72,77));
  Assert.That(player.Position.y,Is.EqualTo(30.5f).Within(.025f));
  Assert.AreEqual(0,chapter.room.Deaths);Assert.AreEqual(ChapterOneRoom.Phase.Playing,chapter.CurrentPhase);
  Assert.IsFalse(chapter.replay.ReplayUpgradeUnlocked);Assert.IsFalse(chapter.Past);
 }
}
}
#endif
