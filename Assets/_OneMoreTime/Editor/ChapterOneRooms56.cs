using UnityEngine;

namespace OneMoreTime.EditorTools {
public static partial class ChapterOneExpansionBuilder {
 // Official 黑板 12 layout: one dot interval = one unit, river surface y=0.
 // Red bridges collide; orange bridges are scenery. No extra stepping ledges.
 static void Layout4(BuildContext c) {
  c.room.roomBounds=new Rect(0,-6,97,28.5f);
  c.room.start=new Vector2(2,1);c.room.entrance=c.room.start-Vector2.up*7;
  c.room.exitX=96;c.room.exitFloor=12;
  c.flow.roomTitle="Riverworks Ascent";
  c.flow.filterEnabled=false;c.flow.replayUpgradeOnEntry=false;
  c.flow.exitArea=new Rect(95,12,2,3);

  // Leave the arrival shaft open below its one-way landing, matching Room 3's lift.
  Wood(c,"Room 4 arrival lift landing",.5f,4,1);
  Solid(c,"River entrance bank",4,8,1,-7);
  var arrival=Prefab(c.terrain,R+"/Prefabs/OldMountainLift.prefab",c.room.start);
  arrival.name="Room 4 arrival lift from Room 3";
  c.room.lift=arrival.GetComponent<RoomLift>();c.room.lift.room=c.room;c.room.lift.dock=c.room.start;
  c.room.lift.arrival=true;c.room.lift.automaticGate=true;c.room.lift.plate=null;
  c.room.lift.travel=7;c.room.lift.speed=3;
  Solid(c,"First river block",18,21,3,1);
  Solid(c,"Second river block",24,26,3,1);
  Solid(c,"Middle island foundation",30,36,3,1);
  Solid(c,"Middle island tower",32,35,7,3);
  Solid(c,"Eastern island foundation",51,65,3,1);
  Solid(c,"Eastern island tower",62,65,13,3);

  DrawingBridge(c,"Middle red bridge",36,39,3,false);
  DrawingBridge(c,"Middle orange bridge",39,48,3,true);
  DrawingBridge(c,"East red approach",48,51,3,false);
  DrawingBridge(c,"East red departure",65,68,3,false);
  Grass(c,"Upper grass shelf",54.5f,7,5,10);
  Grass(c,"Tower grass shelf",60.25f,5,3.5f,8);

  var river=Water(c,"Long downstream river",8,92,0,-7);
  river.velocity=2;
  foreach(var feature in river.GetComponentsInChildren<WaterFeature>()) {
   feature.size+=Vector2.up*.5f;feature.Refresh();
  }
  Fall(c,"River end spill",91.8f,-8,.6f,8);

  var release=Plate(c,"Log release pressure plate",6.5f,1);
  var source=FloatingTimberBuilder.InstallSource(c.terrain,"FloatingLog",river,
   new Vector2(8.5f,.28f),95,c.replay);
  source.name="Room 4 plate-released logs";
  source.activationPlate=release;source.plateCooldown=4;source.initialHorizontalVelocity=2;
  source.disposal.disposeToRight=true;source.disposal.boundaryX=95;
  // Let logs fall over the drawn river end before their pool slot is released.
  source.disposal.transform.position=new Vector2(96,-8);

  var plate=Plate(c,"Return-platform pressure plate",60.4f,3);
  var platform=Mover(c,"Swift return platform",54,10,2.4f,new Vector2(6,0),plate);
  platform.returnWhenReleased=true;platform.speed=12;platform.releaseDelay=0;
  var rail=Group(c.scenery,"Swift platform travel rail — decorative");
  PresentationBuilder.Image(rail,"Faded travel guide",MainRoomArtBuilder.Art("WeatheredWoodPlatform"),
   new Vector2(57,9.86f),new Vector2(6,.06f),-15,new Color(.5f,.48f,.32f,.55f));

  Wood(c,"Lower exit approach",87,90,3);
  Wood(c,"Middle exit approach",84,87,6);
  Wood(c,"Upper exit approach",88,89,8.5f);
  var exitBank=Solid(c,"Room 4 beveled horizontal exit bank",92,97,12,-7);
  Object.DestroyImmediate(exitBank.GetComponent<BoxCollider2D>());
  var outline=exitBank.AddComponent<PolygonCollider2D>();
  var points=new[]{new Vector2(92,-7),new Vector2(97,-7),new Vector2(97,12),new Vector2(91,12),new Vector2(92,11)};
  for(int i=0;i<points.Length;i++)points[i]-=(Vector2)exitBank.transform.position;
  outline.points=points;
  var bevel=Group(exitBank.transform,"Left inward grass terrain bevel");bevel.position=new Vector2(91,12);
  var bevelArt=bevel.gameObject.AddComponent<SpriteRenderer>();
  bevelArt.sprite=UnityEditor.AssetDatabase.LoadAssetAtPath<Sprite>(R+"/Generated/WorldOne/GrassTerrainCorner.asset");
  bevelArt.flipX=true;bevelArt.sortingOrder=4;
  // The drawing has no statue: retries use the entrance bank.
  var fern=Prop(c,"SmallFern",5,1);fern.transform.localScale=Vector3.one*.45f;
 }
}
}
