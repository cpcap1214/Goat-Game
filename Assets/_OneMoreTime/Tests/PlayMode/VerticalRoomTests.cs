#if UNITY_EDITOR
using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
namespace OneMoreTime.Tests {
public class VerticalRoomTests {
 MainRoom room;
 void Step(float move=0,bool jump=false,bool freeze=false,bool drop=false,bool retry=false){room.Step(new InputFrame{move=move,jump=jump,held=true,freeze=freeze,drop=drop,retry=retry},.02f);Physics2D.SyncTransforms();}
 void Place(Vector2 p){room.player.ResetAt(p);Physics2D.SyncTransforms();Step();Step();}
 void Snap(){var p=room.player.Position;Step(freeze:true);Assert.AreEqual(p,room.shutter.Position);for(int i=0;i<30&&room.shutter.Snapping;i++)Step();Assert.IsFalse(room.shutter.echo.support.enabled);}
 IEnumerator Load(){EditorSceneManager.LoadSceneInPlayMode("Assets/_OneMoreTime/Scenes/"+MainRoom.RoomThreeScene+".unity",new LoadSceneParameters(LoadSceneMode.Single));yield return null;room=Object.FindAnyObjectByType<MainRoom>();room.AutoTick=false;for(int i=0;i<160&&room.Phase==MainRoom.RoomPhase.Entering;i++)Step();Step();}
 [UnityTest]public IEnumerator HeldKeyboardMovementSurvivesShutter(){
  yield return Load();Assert.IsFalse(room.showTips);Assert.IsEmpty(room.Prompt);Assert.AreEqual(0,room.GetComponentsInChildren<LineRenderer>(true).Length);Assert.AreEqual(31,room.lift.dock.x);
  var previous=Keyboard.current;var keyboard=InputSystem.AddDevice<Keyboard>();keyboard.MakeCurrent();room.AutoTick=true;
  void Keys(params Key[] keys){InputSystem.QueueStateEvent(keyboard,new KeyboardState(keys));InputSystem.Update();room.SendMessage("Update");room.SendMessage("FixedUpdate");Physics2D.SyncTransforms();}
  try{
   Keys();for(int i=0;i<10;i++)Keys(Key.D);Assert.Greater(room.player.Velocity.x,0);
   Keys(Key.D,Key.E);Assert.IsTrue(room.shutter.Snapping);var captured=room.shutter.Position;
   for(int i=0;i<24;i++)Keys(Key.D,Key.E);Assert.IsFalse(room.shutter.Snapping);Assert.Greater(room.player.Position.x,captured.x+.1f);Assert.AreEqual(captured,room.shutter.Position,"Held E must not repeat");
   Keys(Key.D);Keys(Key.D,Key.E);Assert.IsTrue(room.shutter.Snapping);for(int i=0;i<28;i++)Keys(Key.A);Assert.Less(room.player.Velocity.x,0,"Direction can change during snap");
   Keys(Key.A,Key.E);Assert.IsTrue(room.shutter.Snapping);for(int i=0;i<28;i++)Keys();Assert.AreEqual(0,room.player.Velocity.x,"Released movement must not remain latched");
  }finally{room.AutoTick=false;InputSystem.RemoveDevice(keyboard);if(previous!=null)previous.MakeCurrent();}
 }
 [UnityTest]public IEnumerator ShuttleCarriesBothDirectionsAndAllowsJumpAndDrop(){
  yield return Load();var p=room.mechanisms.platforms[0];Assert.IsFalse(p.Active);
  // Fixtures isolate mechanisms; these are not a claim of a full-room solution.
  Place(new Vector2(12.5f,2.5f));Snap();Step();Assert.IsTrue(p.Active);
  Place(new Vector2(16,p.solid.bounds.max.y+.01f));Assert.AreEqual(p.solid,room.player.GroundCollider);
  float min=100,max=-100;for(int i=0;i<280;i++){Step();float y=p.solid.bounds.max.y;min=Mathf.Min(min,y);max=Mathf.Max(max,y);Assert.That(room.player.Position.y-y,Is.InRange(-.01f,.03f));}
  Assert.That(max,Is.EqualTo(10).Within(.05f));Assert.That(min,Is.EqualTo(5).Within(.05f));
  Step(jump:true);Assert.That(room.player.Velocity.y,Is.EqualTo(room.player.tuning.jumpSpeed-room.player.tuning.gravity*.02f).Within(.001f));
  Place(new Vector2(16,p.solid.bounds.max.y+.01f));Step(jump:true,drop:true);for(int i=0;i<12;i++)Step();Assert.Less(room.player.Position.y,p.solid.bounds.max.y-.1f);
  Step(retry:true);Assert.IsTrue(room.shutter.HasSnapshot);Assert.That(p.solid.bounds.max.y,Is.EqualTo(5).Within(.02f));
 }
 [UnityTest]public IEnumerator TimerHoldsCountsDownRechargesAndExpires(){
  yield return Load();var p=room.mechanisms.platforms[1];Assert.IsFalse(p.Active);Assert.IsFalse(p.counter.gameObject.activeSelf);Assert.That(Vector3.Distance(p.counter.transform.position,p.artwork[0].bounds.center),Is.LessThan(.01f));Assert.That(p.counter.characterSize,Is.EqualTo(.065f));
  Place(new Vector2(19.5f,22));for(int i=0;i<50;i++)Step();Assert.AreEqual(3,p.Remaining);Snap();Place(new Vector2(18,22));for(int i=0;i<160;i++)Step();Assert.AreEqual(3,p.Remaining);
  Snap();for(int i=0;i<50;i++)Step();Assert.That(p.Remaining,Is.EqualTo(2).Within(.03f));
  float remaining=p.Remaining;Snap();Assert.AreEqual(remaining,p.Remaining,"shutter pauses countdown");
  Place(new Vector2(19.5f,22));Step();Assert.AreEqual(3,p.Remaining);Assert.AreEqual("3.0",p.counter.text);
  Place(new Vector2(17,25));for(int i=0;i<160;i++)Step();Assert.IsFalse(p.Active);Assert.IsFalse(p.solid.enabled);Assert.AreEqual("",p.counter.text);Assert.IsFalse(p.counter.gameObject.activeSelf);Assert.AreNotEqual(p.solid,room.player.GroundCollider);
 }
 [UnityTest]public IEnumerator SharedPlateControlsIndependentThreeAndTwentySecondTimers(){
  yield return Load();var shortTimer=room.mechanisms.platforms[1];var longTimer=room.mechanisms.platforms[2];
  Assert.AreSame(shortTimer.plate,longTimer.plate);Assert.AreEqual(39,room.lift.dock.y);Assert.AreEqual(44,room.roomBounds.yMax);
  Place(new Vector2(19.5f,22));Assert.AreEqual(3,shortTimer.Remaining);Assert.AreEqual(20,longTimer.Remaining);
  Place(new Vector2(13,32));for(int i=0;i<160;i++)Step();
  Assert.IsFalse(shortTimer.Active);Assert.IsTrue(longTimer.Active);Assert.That(longTimer.Remaining,Is.InRange(16.7f,16.9f));
  Assert.IsFalse(shortTimer.counter.gameObject.activeSelf);Assert.IsTrue(longTimer.counter.gameObject.activeSelf);
  for(int i=0;i<850;i++)Step();Assert.IsFalse(longTimer.Active);Assert.IsFalse(longTimer.solid.enabled);Assert.IsFalse(longTimer.counter.gameObject.activeSelf);Assert.AreEqual("",longTimer.counter.text);
 }
 [UnityTest]public IEnumerator UpperRouteExceedsDirectSingleFreezeJumpReach(){
  yield return Load();var upper=room.transform.Find("Revised upper route");
  float Top(string name){var box=upper.Find(name).GetComponent<BoxCollider2D>();return box.transform.position.y+box.offset.y+box.size.y/2;}
  Assert.That(Top("Left route wood step"),Is.EqualTo(27.5f).Within(.001f));
  Assert.That(Top("High left grass"),Is.EqualTo(31).Within(.001f));
  Assert.That(Top("High middle grass"),Is.EqualTo(32).Within(.001f));
  Assert.That(Top("Twenty second wood platform"),Is.EqualTo(35).Within(.001f));
  // Conservative bound: two full jumps plus a frozen goat's back, starting
  // on the 3-second ledge. This even grants a perfect apex capture/landing.
  float velocity=room.player.tuning.jumpSpeed,height=0,apex=0;
  while(velocity>0){velocity-=room.player.tuning.gravity*.02f;height+=velocity*.02f;apex=Mathf.Max(apex,height);}
  float directReach=25+apex*2+room.player.tuning.backHeight;
  Assert.Greater(Top("High middle grass"),directReach+.05f);
  Assert.Greater(Top("Twenty second wood platform"),directReach);
  Assert.Less(Top("High left grass")-Top("Left route wood step"),apex*2+room.player.tuning.backHeight);
 }
 [UnityTest]public IEnumerator BothStatuesSaveAndPitRespawnsBelowView(){
  yield return Load();Assert.AreEqual(2,room.checkpoints.Length);Place(new Vector2(11.5f,11));Assert.IsTrue(room.checkpoints[0].Saved);Place(new Vector2(17.6f,22));Assert.IsTrue(room.checkpoints[1].Saved);Snap();
  room.player.ResetAt(new Vector2(21.5f,-6.4f));room.SendMessage("LateUpdate");Assert.Less(room.deathY,room.view.transform.position.y-room.view.orthographicSize-1);Step();Assert.AreEqual(0,room.Deaths);
  for(int i=0;i<20&&room.Deaths==0;i++)Step();Assert.AreEqual(1,room.Deaths);Assert.That(room.player.Position.x,Is.EqualTo(17.6f).Within(.02f));Assert.That(room.player.Position.y,Is.EqualTo(22).Within(.03f));Assert.IsTrue(room.shutter.HasSnapshot);
  Assert.Greater(room.checkpoints[0].eyes[0].color.g,.9f);Assert.Greater(room.checkpoints[1].eyes[0].color.g,.9f);
 }
 [UnityTest]public IEnumerator FinalPlateOpensLiftAndTransfersUpward(){
  yield return Load();Assert.IsFalse(room.lift.Open);Place(new Vector2(25.5f,39));Snap();
  for(int i=0;i<100&&room.Phase==MainRoom.RoomPhase.Playing;i++)Step(1);Assert.AreEqual(MainRoom.RoomPhase.Exiting,room.Phase);float y=room.player.Position.y;for(int i=0;i<12;i++)Step(-1,true);Assert.Greater(room.player.Position.y,y);
  for(int i=0;i<150&&room.Phase!=MainRoom.RoomPhase.Complete;i++)Step();yield return null;Assert.AreEqual(MainRoom.LiftLandingScene,SceneManager.GetActiveScene().name);room=Object.FindAnyObjectByType<MainRoom>();room.AutoTick=false;for(int i=0;i<160&&room.Phase==MainRoom.RoomPhase.Entering;i++)Step();Assert.AreEqual(MainRoom.RoomPhase.Playing,room.Phase);Assert.That(room.player.Position.y,Is.EqualTo(0).Within(.02f));
 }
}
}
#endif
