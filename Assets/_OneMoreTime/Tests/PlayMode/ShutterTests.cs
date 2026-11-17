#if UNITY_EDITOR
using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
namespace OneMoreTime.Tests {
public class ShutterTests {
 MainRoom room;
 void Step(float move=0,bool jump=false,bool freeze=false,bool retry=false){room.Step(new InputFrame{move=move,jump=jump,held=true,freeze=freeze,retry=retry},.02f);Physics2D.SyncTransforms();}
 void Walk(float x){for(int i=0;i<700&&Mathf.Abs(room.player.Position.x-x)>.08f;i++)Step(Mathf.Sign(x-room.player.Position.x));for(int i=0;i<8;i++)Step();Assert.That(room.player.Position.x,Is.EqualTo(x).Within(.3f));}
 void Jump(float x,float y){Step(0,true);for(int i=0;i<160;i++){float dx=x-room.player.Position.x;Step(Mathf.Abs(dx)>.06f?Mathf.Sign(dx):0);if(room.player.Grounded&&i>5)break;}Assert.That(room.player.Position.y,Is.EqualTo(y).Within(.04f),"landing "+room.player.Position);}
 void Snap(){
  var position=room.player.Position;var velocity=room.player.Velocity;int deaths=room.Deaths;
  Step(freeze:true);Assert.IsTrue(room.shutter.Snapping);Assert.AreEqual(position,room.shutter.Position);
  for(int i=0;i<30&&room.shutter.Snapping;i++){Step(1,true,true,true);Assert.AreEqual(position,room.player.Position);Assert.AreEqual(velocity,room.player.Velocity);}
  Assert.IsFalse(room.shutter.Snapping);Assert.IsTrue(room.shutter.HasSnapshot);Assert.AreEqual(position,room.shutter.Position);Assert.AreEqual(deaths,room.Deaths);Assert.IsFalse(room.shutter.echo.support.enabled);
 }
 void LandOnCopy(){for(int i=0;i<90&&!room.player.OnEcho;i++)Step();Assert.IsTrue(room.player.OnEcho,"land on frozen copy at "+room.shutter.Position+" from "+room.player.Position);}
 [UnityTest]public IEnumerator RoomTwoShutterFreezesInPlaceAndReachesCheckpoint(){
  EditorSceneManager.LoadSceneInPlayMode("Assets/_OneMoreTime/Scenes/"+MainRoom.RoomTwoScene+".unity",new LoadSceneParameters(LoadSceneMode.Single));yield return null;
  room=Object.FindAnyObjectByType<MainRoom>();room.AutoTick=false;
  for(int i=0;i<160&&room.Phase==MainRoom.RoomPhase.Entering;i++)Step();Assert.IsFalse(room.shutter.Press());
  Walk(12.4f);Assert.IsTrue(room.shutter.Unlocked);Assert.IsFalse(room.shutter.pickup.gameObject.activeSelf);
  Walk(17.5f);Step(jump:true);for(int i=0;i<6;i++)Step();Snap();LandOnCopy();Jump(20,5);Walk(24.3f);Assert.IsTrue(room.CheckpointSaved);
  var saved=room.shutter.Position;Step(retry:true);Assert.AreEqual(saved,room.shutter.Position);Assert.That(room.player.Position.x,Is.EqualTo(24.3f).Within(.01f));
  Walk(28.5f);for(int i=0;i<100&&room.Deaths==0;i++)Step();Assert.AreEqual(1,room.Deaths);Assert.AreEqual(saved,room.shutter.Position);Assert.IsTrue(room.shutter.Unlocked);
  var before=room.player.Position;Snap();Assert.AreEqual(before,room.shutter.Position);Assert.AreNotEqual(saved,room.shutter.Position);Assert.AreEqual(before,room.player.Position);
  // Exit animation fixture only: the exact 9-unit sketch pit has not been validated as crossable.
  room.player.ResetAt(new Vector2(37,5));
  for(int i=0;i<200&&room.Phase==MainRoom.RoomPhase.Playing;i++)Step(1);Assert.AreEqual(MainRoom.RoomPhase.Exiting,room.Phase);
  for(int i=0;i<100&&room.Phase==MainRoom.RoomPhase.Exiting;i++)Step();Assert.AreEqual(MainRoom.RoomPhase.Complete,room.Phase);Assert.Greater(room.player.Position.x,room.roomBounds.xMax);
 }
 [UnityTest]public IEnumerator RoomOneExitLoadsRoomTwo(){
  EditorSceneManager.LoadSceneInPlayMode("Assets/_OneMoreTime/Scenes/"+MainRoom.SceneName+".unity",new LoadSceneParameters(LoadSceneMode.Single));yield return null;
  room=Object.FindAnyObjectByType<MainRoom>();room.AutoTick=false;for(int i=0;i<160&&room.Phase==MainRoom.RoomPhase.Entering;i++)Step();
  // Transition-only fixture; full Room 1 traversal is covered by MainRoomTests.
  room.player.ResetAt(new Vector2(room.exitX,room.exitFloor));for(int i=0;i<100&&room.Phase!=MainRoom.RoomPhase.Complete;i++)Step();yield return null;
  Assert.AreEqual(MainRoom.RoomTwoScene,SceneManager.GetActiveScene().name);Assert.IsNotNull(Object.FindAnyObjectByType<ShutterAbility>());
 }
}
}
#endif
