using System.Collections.Generic;
using UnityEngine;

namespace OneMoreTime.EditorTools {
public static partial class ChapterOneExpansionBuilder {
 // IMG_8935: one dot = one unit, waterfall edges x=0 and x=20.
 // The lower cyan entrance line is y=0. Only the drawn decks provide footholds.
 static void Layout7(BuildContext c){
  c.flow.roomTitle="Twin Falls Tower";c.flow.filterEnabled=false;c.flow.replayUpgradeOnEntry=false;
  c.flow.nextRoomScene="Main_Room08";c.flow.exitUpwards=true;c.flow.exitArea=new Rect(8,64.7f,3,5);
  c.room.roomBounds=new Rect(0,-3.5f,20,68.5f);c.room.start=new Vector2(9.5f,0);c.room.entrance=c.room.start-Vector2.up*8;
  c.room.exitX=9.5f;c.room.exitFloor=65;
  Solid(c,"07 Entrance floor west",0,7.8f,0,-4);
  Solid(c,"07 Entrance floor east",11,20,0,-4);
  Wood(c,"07 Arrival lift landing",7.8f,11,0);
  var arrival=Prefab(c.terrain,R+"/Prefabs/OldMountainLift.prefab",c.room.start);
  arrival.name="07 Arrival from Room 6 lift";
  c.room.lift=arrival.GetComponent<RoomLift>();c.room.lift.room=c.room;c.room.lift.dock=c.room.start;
  c.room.lift.arrival=true;c.room.lift.automaticGate=true;c.room.lift.travel=8;c.room.lift.speed=4;
  Grass(c,"07 Lower west grass shelf",4.4f,2.6f,5.2f,6.6f);
  Grass(c,"07 Lower east grass shelf",15.4f,2.6f,5.2f,6.6f);
  // Keep the inward reach and hide the irregular outer silhouettes off-camera.
  Fall(c,"07 West boundary waterfall",-1,-3.5f,2.5f,68.5f);
  Fall(c,"07 East boundary waterfall",18.5f,-3.5f,2.5f,68.5f);

  LoopDeck(c,"07 Center six-second loop",new[]{new Vector2(8,7.6f),new Vector2(8,8.5f),new Vector2(11,8.5f),new Vector2(11,5.6f),new Vector2(8,5.6f)});
  LoopDeck(c,"07 West six-second loop",new[]{new Vector2(2.2f,12.5f),new Vector2(2.2f,10.4f),new Vector2(5.2f,10.4f),new Vector2(5.2f,13.4f),new Vector2(2.2f,13.4f)});
  LoopDeck(c,"07 East six-second loop",new[]{new Vector2(14.2f,12.5f),new Vector2(14.2f,10.4f),new Vector2(17.2f,10.4f),new Vector2(17.2f,13.4f),new Vector2(14.2f,13.4f)});
  Wood(c,"07 West paired plate deck",.8f,4,15.6f);Wood(c,"07 East paired plate deck",15.8f,19,15.6f);
  var west=Plate(c,"07 West simultaneous pressure plate",2.3f,15.6f);
  var east=Plate(c,"07 East simultaneous pressure plate",17.4f,15.6f);
  var paired=Mover(c,"07 Two-plate latched ascent",9.5f,14.5f,5.3f,new Vector2(0,6));
  paired.requiredPlates=new[]{west,east};paired.latchWhenAllPressed=true;
  paired.counter=DeckCount(paired.transform);paired.Tick(0);
  var pairedReveal=c.root.gameObject.AddComponent<RoomPlatformReveal>();pairedReveal.triggeringPlates=new[]{west,east};pairedReveal.platform=paired;c.flow.platformReveal=pairedReveal;
  TravelGuide(c,"07 Paired lift vertical guide",new[]{new Vector2(9.5f,14.5f),new Vector2(9.5f,20.5f)},false);
  Bridge(c,"07 First checkpoint bridge",0,20,22.6f);Checkpoint(c,"07 Lower bridge checkpoint",9.5f,22.6f);

  ShuttleDeck(c,"07 Four-second shuttle 1",11.2f,26.6f,6);
  ShuttleDeck(c,"07 Four-second shuttle 2",8,28.6f,-5);
  ShuttleDeck(c,"07 Four-second shuttle 3",11.2f,30.5f,6);
  ShuttleDeck(c,"07 Four-second shuttle 4",8,32.4f,-5);
  ShuttleDeck(c,"07 Four-second shuttle 5",3,35.7f,4);
  ShuttleDeck(c,"07 Four-second shuttle 6",15,39.6f,-4);
  Bridge(c,"07 Upper checkpoint bridge",0,10,42.6f);Checkpoint(c,"07 Upper bridge checkpoint",7.6f,42.6f);
  var send=Plate(c,"07 Dispatch platform pressure plate",3.3f,42.6f);
  Solid(c,"07 Upper pool west wall",10,11,45.1f,42.6f);
  Solid(c,"07 Upper pool bed",11,20,43.6f,42.6f);
  var current=Water(c,"07 Upper pool",11,20,44.6f,43.6f);
  foreach(var feature in current.GetComponentsInChildren<WaterFeature>())if(feature.shape==WaterFeature.Shape.Pool){feature.size.y+=.5f;feature.Refresh();}
  // Center the narrow slot on the dispatched deck/track. The empty deck
  // passes through; the goat takes the upper route to catch it again.
  Solid(c,"07 Overhead middle block",7.7f,10.8f,48.6f,45.7f);
  Solid(c,"07 Zigzag west wall",12.9f,14,58.8f,48.6f);
  Solid(c,"07 Zigzag east wall",16.9f,18,59.8f,46);
  Wood(c,"07 Zigzag landing",14,17,48.6f);
  Wood(c,"07 Zigzag step 1",16,17.1f,51.8f);
  Wood(c,"07 Zigzag step 2",14,15.1f,53.8f);
  Wood(c,"07 Zigzag step 3",16,17.1f,55.8f);
  Wood(c,"07 Zigzag step 4",14,15.1f,57.8f);
  Wood(c,"07 Upper west small deck",2.8f,4.1f,58.1f);
  Wood(c,"07 Upper middle small deck",6.8f,8.1f,56.1f);
  var sendRoute=new[]{new Vector2(1.2f,45.5f),new Vector2(18.8f,45.5f),new Vector2(18.8f,60),new Vector2(2.2f,60)};
  TravelGuide(c,"07 Dispatch route",sendRoute,false);
  var dispatcher=Group(c.terrain,"07 Four-second platform dispatcher").gameObject.AddComponent<PlatformDispatcher>();
  dispatcher.activationPlate=send;dispatcher.cooldown=4;dispatcher.pool=new RoomPlatform[4];
  for(int i=0;i<dispatcher.pool.Length;i++){
   var deck=Mover(c,"07 Dispatched deck "+(i+1),sendRoute[0].x,sendRoute[0].y,2.2f,Vector2.zero);
   deck.motion=RoomPlatform.Motion.DispatchedRoute;deck.route=Offsets(sendRoute);deck.speed=3;ConfigureDispatchCountdown(deck);deck.Tick(0);dispatcher.pool[i]=deck;
  }
  var participants=new List<RewindParticipant>(c.replay.participants);participants.Add(dispatcher);c.replay.participants=participants.ToArray();

  // Upper-left pocket, exit shaft, and the right upper block follow the cyan outline.
  Solid(c,"07 Upper-left plate landing",4,8,62.8f,60.3f);
  Solid(c,"07 Upper-left shaft wall",7,8,65,62.8f);
  Solid(c,"07 Upper-left outer bank",0,7,65,64.65f);
  Solid(c,"07 Upper-right exit bank",11,20,65,61.7f);
  Wood(c,"07 Exit shaft stepping deck",8,11,63.7f);
  var open=Plate(c,"07 Upper-left trapdoor pressure plate",5.8f,62.8f);open.affectedByRewind=false;
  var trap=Group(c.terrain,"07 Split trapdoor — persistent").gameObject.AddComponent<RoomTrapdoor>();
  trap.plate=open;trap.horizontalCenter=10;trap.targetPoint=new Vector2(9.5f,62.7f);trap.leafTravel=1.5f;
  trap.leftLeaf=Wood(c,"07 Trapdoor west leaf",8,9.5f,62.7f).transform;
  trap.rightLeaf=Wood(c,"07 Trapdoor east leaf",9.5f,11,62.7f).transform;
  // Trapdoor blocks both directions until opened; ordinary wooden decks remain one-way.
  Object.DestroyImmediate(trap.leftLeaf.GetComponent<OneWayPlatform>());Object.DestroyImmediate(trap.rightLeaf.GetComponent<OneWayPlatform>());
  trap.leftCollider=trap.leftLeaf.GetComponent<BoxCollider2D>();trap.rightCollider=trap.rightLeaf.GetComponent<BoxCollider2D>();
  c.flow.trapdoor=trap;
 }
 public static void ConfigureDispatchCountdown(RoomPlatform deck){
  deck.endLifetime=5;
  var t=deck.transform.Find("Endpoint countdown");if(t==null)t=Group(deck.transform,"Endpoint countdown");
  t.localPosition=new Vector3(0,0,-.1f);
  var text=t.GetComponent<TextMesh>();if(text==null)text=t.gameObject.AddComponent<TextMesh>();
  text.font=PresentationBuilder.Font;text.fontSize=40;text.characterSize=.065f;text.anchor=TextAnchor.MiddleCenter;text.alignment=TextAlignment.Center;text.text="";
  var renderer=t.GetComponent<MeshRenderer>();renderer.sharedMaterial=text.font.material;renderer.sortingOrder=10;
  deck.counter=text;t.gameObject.SetActive(false);
 }
 static Vector2[] Offsets(Vector2[] points){var offsets=new Vector2[points.Length];for(int i=0;i<points.Length;i++)offsets[i]=points[i]-points[0];return offsets;}
 static void LoopDeck(BuildContext c,string name,Vector2[] points){
  var deck=Mover(c,name,points[0].x,points[0].y,2.2f,Vector2.zero);deck.motion=RoomPlatform.Motion.ClosedRoute;deck.route=Offsets(points);deck.cycleSeconds=6;deck.Tick(0);TravelGuide(c,name+" track",points,true);
 }
 static void ShuttleDeck(BuildContext c,string name,float x,float y,float travel){var deck=Mover(c,name,x,y,2.2f,new Vector2(travel,0));deck.cycleSeconds=4;deck.Tick(0);TravelGuide(c,name+" track",new[]{new Vector2(x,y),new Vector2(x+travel,y)},false);}
 static TextMesh DeckCount(Transform parent){
  var t=Group(parent,"Simultaneous plate count");t.localPosition=new Vector3(0,.28f,-.1f);var text=t.gameObject.AddComponent<TextMesh>();text.font=PresentationBuilder.Font;text.fontSize=40;text.characterSize=.08f;text.anchor=TextAnchor.MiddleCenter;text.alignment=TextAlignment.Center;
  var renderer=t.GetComponent<MeshRenderer>();renderer.sharedMaterial=text.font.material;renderer.sortingOrder=10;return text;
 }
 static void TravelGuide(BuildContext c,string name,Vector2[] points,bool loop){
  var t=Group(c.scenery,name);var line=t.gameObject.AddComponent<LineRenderer>();line.sharedMaterial=UnityEditor.AssetDatabase.LoadAssetAtPath<Material>(R+"/Generated/RewindTrace.mat");line.useWorldSpace=true;line.positionCount=points.Length;line.loop=loop;line.startWidth=line.endWidth=.035f;line.startColor=line.endColor=new Color(.78f,.73f,.4f,.48f);line.sortingOrder=-12;
  for(int i=0;i<points.Length;i++)line.SetPosition(i,new Vector3(points[i].x,points[i].y-.12f,0));
 }
}
}
