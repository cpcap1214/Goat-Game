using System.Linq;
using UnityEditor;
using UnityEngine;

namespace OneMoreTime.EditorTools {
public static partial class ChapterOneExpansionBuilder {
 // IMG_8932: one grid interval is one unit. Lowest river surface is y=0.
 // Orange spans are permanently decorative: this room grants neither the lens
 // nor reverse replay. Red spans, grass tops and the powered shuttle are playable.
 static void Layout6(BuildContext c) {
  c.room.roomBounds=new Rect(0,-2,90,26);
  c.room.entrance=new Vector2(.8f,8);c.room.start=new Vector2(2.5f,8);
  c.room.exitX=88;c.room.exitFloor=9;
  c.flow.roomTitle="Broken Bridges";
  c.flow.exitArea=new Rect(87,9,2,3);c.flow.filterEnabled=false;
  c.flow.replayUpgradeOnEntry=false;
  Solid(c,"West forest bank",0,5,8,-3);
  Solid(c,"East forest bank",82,90,9,-3);

  Water(c,"West high river",4,26,4,-3);
  Water(c,"First descending step",26,29,3,-3);
  Water(c,"Second descending step",29,33,1,-3);
  var lower=Water(c,"Wide lower bend",33,74,0,-3);
  Water(c,"First ascending step",74,76,3,-3);
  Water(c,"East high river",76,82,7,-3);
  // The shared river strip has transparent space above its painted crest.
  // Align the visible water with the authored buoyancy surface, keeping each
  // pool bottom and invisible drowning volume in their original positions.
  foreach(var current in c.currents)foreach(var feature in current.GetComponentsInChildren<WaterFeature>()) {
   feature.size+=Vector2.up*.5f;feature.Refresh();
  }
  Fall(c,"West one-unit spill",25.85f,3,.3f,1);
  Fall(c,"West two-unit spill",28.85f,1,.3f,2);
  Fall(c,"West final spill",32.85f,0,.3f,1);
  Fall(c,"East lower join",73.85f,0,.3f,3);
  Fall(c,"East upper join",75.85f,3,.3f,4);
  foreach(var water in c.currents)water.velocity=2.5f;

  DrawingBridge(c,"West red span 1",5,7,8,false);
  DrawingBridge(c,"West orange span 1",7,11,8,true);
  DrawingBridge(c,"West red span 2",11,13,8,false);
  DrawingBridge(c,"West orange span 2",13,19,8,true);
  DrawingBridge(c,"West red span 3",19,20,8,false);
  Grass(c,"Plate grass island",22.5f,8,5,11);
  var plate=Plate(c,"Crossing pressure plate",24.3f,8);
  var shuttle=Mover(c,"Plate-powered horizontal shuttle",29,8,2,new Vector2(7,0),plate);
  shuttle.releaseDelay=0;shuttle.speed=2;
  // A slim track helps communicate the horizontal travel; no control-link lines.
  var track=Group(c.scenery,"Shuttle travel rail — decorative");
  PresentationBuilder.Image(track,"Faded brass rail",MainRoomArtBuilder.Art("WeatheredWoodPlatform"),
   new Vector2(32.5f,7.86f),new Vector2(7,.07f),-15,new Color(.5f,.48f,.32f,.65f));

  DrawingBridge(c,"Lower red approach",41,43,5,false);
  Grass(c,"Checkpoint grass island",44.5f,5,3,8);
  Checkpoint(c,"Central river checkpoint",44.5f,5);
  DrawingBridge(c,"Lower red span 1",46,49,5,false);
  DrawingBridge(c,"Lower orange span 1",49,60,5,true);
  DrawingBridge(c,"Lower red span 2",60,67,5,false);
  DrawingBridge(c,"Lower orange span 2",67,74,5,true);
  DrawingBridge(c,"Upper orange approach",57,66,9,true);
  DrawingBridge(c,"Upper red crossing",66,74,9,false);
  DrawingBridge(c,"Upper orange exit",74,82,9,true);
  DrawingBridge(c,"High orange remnant",62,79,13,true);

  RoomSixCrates(c,lower);
  var eddy=Prefab(c.scenery,WaterFeatureBuilder.Prefabs+"/SurfaceEddy.prefab",new Vector2(60,-.38f));
  eddy.name="Whirlpool — crate drain";eddy.transform.localScale=new Vector3(1.25f,.6f,1);
  var eddyArt=eddy.GetComponent<SpriteRenderer>();eddyArt.sortingOrder=7;
  Fall(c,"Whirlpool downward water",59.6f,-3,.8f,3);

  // Small, grounded clusters only. Leave the narrow routes and mechanisms clear.
  var fern=Prop(c,"SmallFern",4,8);fern.transform.localScale=Vector3.one*.45f;
  var reeds=Prop(c,"RiverReeds",83.2f,9);reeds.transform.localScale=Vector3.one*.55f;
  var rock=Prop(c,"MossyRiverRock",84.5f,9);rock.transform.localScale=Vector3.one*.55f;

  var liftObject=Prefab(c.terrain,R+"/Prefabs/OldMountainLift.prefab",new Vector2(88,9));
  liftObject.name="Room 6 upward exit lift";
  c.room.lift=liftObject.GetComponent<RoomLift>();c.room.lift.room=c.room;c.room.lift.dock=new Vector2(88,9);
  c.room.lift.arrival=false;c.room.lift.automaticGate=true;c.room.lift.plate=null;
  c.room.lift.travel=8;c.room.lift.speed=4;
 }

 static void DrawingBridge(BuildContext c,string name,float left,float right,float top,bool background) {
  if(background) {
   var root=Group(c.scenery,name+" — background only");
   root.position=new Vector2((left+right)/2,top);
   var art=root.gameObject.AddComponent<SpriteRenderer>();
   art.sprite=AssetDatabase.LoadAssetAtPath<Sprite>(R+"/Generated/WorldOne/BridgeMiddle.asset");
   art.sharedMaterial=AssetDatabase.LoadAssetAtPath<Material>(R+"/Generated/WorldOne/WoodenSuspensionBridge.mat");
   art.drawMode=SpriteDrawMode.Tiled;art.size=new Vector2(right-left,art.sprite.bounds.size.y);
   art.sortingOrder=-14;art.color=new Color(.42f,.46f,.43f,.8f);
  } else Bridge(c,name,left,right,top);
  BridgeScaffold(c,name,left,right,top);
 }

 static void BridgeScaffold(BuildContext c,string name,float left,float right,float top) {
  // Upper decks share the lower scaffold instead of stacking overlapping walls.
  float bottom=top>=12?9:top>=9&&right<=74?5:-3;
  BridgeScaffoldBuilder.Build(c.scenery,name,left,right,top,bottom);
 }

 static void RoomSixCrates(BuildContext c,WaterCurrent2D current) {
  var root=Group(c.terrain,"Room 6 crate source");var source=root.gameObject.AddComponent<CrateSpawner>();
  source.water=current;source.spawnInterval=10;source.initialHorizontalVelocity=2.5f;source.maximumActiveCrates=6;
  source.spawnPoint=Group(root,"Crates appear here");source.spawnPoint.position=new Vector2(33.5f,.28f);
  var drain=Group(root,"Whirlpool disposal");drain.position=new Vector2(60,0);
  source.disposal=drain.gameObject.AddComponent<CrateDisposalTrigger>();
  source.disposal.owner=source;source.disposal.disposeToRight=true;source.disposal.boundaryX=60;
  source.disposal.sinkBeforeDisposal=true;source.disposal.sinkDepth=2.5f;source.disposal.sinkSpeed=3.5f;
  var trigger=drain.GetComponent<BoxCollider2D>();trigger.isTrigger=true;trigger.size=new Vector2(.6f,5);
  source.pool=new FloatingObject2D[6];
  for(int i=0;i<source.pool.Length;i++) {
   var item=Prefab(root,R+"/Prefabs/FloatingWaterCrate.prefab",source.spawnPoint.position);
   item.name="Whirlpool crate "+(i+1);var floating=item.GetComponent<FloatingObject2D>();
   floating.water=current;source.pool[i]=floating;item.SetActive(false);
  }
  c.replay.participants=c.replay.participants.Concat(new RewindParticipant[]{source}).ToArray();
 }
}
}
