using UnityEngine;

namespace OneMoreTime.EditorTools {
public static partial class ChapterOneExpansionBuilder {
 static void Layout8Prototype(BuildContext c) {
  c.flow.roomNumber=8;
  c.flow.roomTitle="The Grand Waterfall";
  c.flow.nextRoomScene=string.Empty;
  c.flow.chapterFinale=true;
  c.flow.filterEnabled=true;
  c.flow.replayUpgradeOnEntry=true;
  c.flow.exitArea=new Rect(112,47.3f,5,7);
  c.room.roomBounds=new Rect(0,-9,120,70);
  c.room.entrance=new Vector2(.5f,3);
  c.room.start=new Vector2(3,3);
  c.room.deathY=-9;
  c.room.exitX=114;
  c.room.exitFloor=47.5f;
  c.replay.spawn=c.room.start;
  c.replay.bounds=c.room.roomBounds;

  Solid(c,"08 Valley entrance",0,7,3,-10);
  Checkpoint(c,"08 Ascent entrance checkpoint",4.5f,3);
  Water(c,"08 Waterfall basin below the ascent",7,105,-3,-9);

  // The old bridge is visible in the other era; the route climbs diagonally.
  Bridge(c,"08 Present bridge stump",8,12,5,1);
  Bridge(c,"08 Past suspension bridge",13,18,7,2);
  Grass(c,"08 Lower waterworks terrace",22.5f,9,5,11);
  Checkpoint(c,"08 Waterworks checkpoint",21,9);
  var liftPlate=Plate(c,"08 Shutter plate — hold the service lift",23.5f,9);
  var serviceLift=Mover(c,"08 Plate-powered vertical service platform",27,10,3,new Vector2(0,7.5f),liftPlate);
  serviceLift.speed=2;
  var crumbleDeck=Bridge(c,"08 Crumbling upper service landing",28.5f,32.5f,19.5f);
  var crumble=crumbleDeck.AddComponent<ChapterCrumbleBridge>();crumble.player=c.room.player;crumble.deck=crumbleDeck.GetComponent<BoxCollider2D>();crumble.artwork=crumbleDeck.GetComponentsInChildren<SpriteRenderer>();
  var participants=new System.Collections.Generic.List<RewindParticipant>(c.replay.participants);participants.Add(crumble);c.replay.participants=participants.ToArray();
  Grass(c,"08 Timer rest terrace",35.5f,21.5f,5,9);
  Checkpoint(c,"08 High waterworks checkpoint",34.3f,21.5f);
  var timerPlate=Plate(c,"08 Six-second bridge pressure plate",36.7f,21.5f);
  Mover(c,"08 Timed crossing — six seconds after release",39.5f,23.5f,3,Vector2.zero,timerPlate,6);
  Grass(c,"08 Upper moss ledge",43,25.5f,3.5f,7);
  Bridge(c,"08 Past aqueduct span",45.8f,49.5f,27.5f,2);
  Grass(c,"08 High stream west bank",53.5f,29.5f,6,9);
  Checkpoint(c,"08 High stream checkpoint",52.5f,29.5f);

  // Reuse the river-recording idea at altitude, with safe banks at both ends.
  Water(c,"08 Suspended high stream — rewind the raft",56.5f,72,28,22);
  Source(c,"BoundLogRaft",56.5f,72,28,70.3f);
  Grass(c,"08 High stream east bank",74.5f,30.5f,5,9);
  Checkpoint(c,"08 Final ridge checkpoint",74,30.5f);
  Grass(c,"08 Last forest ledge",80,33,3,7);
  Bridge(c,"08 Present ridge bridge",82,86,35,1);
  Grass(c,"08 Sparse alpine ledge",88,37,3,7);
  Bridge(c,"08 Past ridge bridge",90,94,39,2);
  Grass(c,"08 Cold stone approach ledge",96,41,3,7);
  Grass(c,"08 Cavern gate terrace",100,43,3,7);
  var cavePlate=Plate(c,"08 Final shutter plate — cavern entrance",100,43);
  Wood(c,"08 Cavern gate landing",102,105,45.5f);
  Gate(c,"08 Old cavern gate",103.6f,45.5f,4,cavePlate);
  Solid(c,"08 Cavern threshold",105,120,47.5f,28);
  Checkpoint(c,"08 Behind the waterfall checkpoint",106.2f,47.5f);
  Cavern(c,113,47.5f,7,8);

  // A dense green valley gradually gives way to exposed stone and sparse plants.
  Prop(c,"BroadFern",2,3);
  Prop(c,"ForestMushrooms",5.7f,3);
  Prop(c,"RootArch",22,9);
  Prop(c,"HangingRoots",22,8.6f);
  Prop(c,"OldWaterwheel",24.5f,9);
  Prop(c,"TimberFrame",30.4f,19.5f);
  Prop(c,"BracedTimber",35.5f,21.5f);
  Prop(c,"MossyRiverRock",33.5f,21.5f);
  Prop(c,"HangingVine",31,20);
  Prop(c,"MeadowFlowers",43.2f,25.5f);
  Prop(c,"RiverReeds",54.8f,29.5f);
  Prop(c,"BrokenPillar",52,29.5f);
  Prop(c,"RockyStreamBank",73.3f,30.5f);
  Prop(c,"AlpineShrub",76,30.5f);
  Prop(c,"WindsweptPine",79.8f,33);
  Prop(c,"MossyOutcrop",87.7f,37);
  CoolRock(Prop(c,"RockSpire",95.7f,41));
  CoolRock(Prop(c,"CliffCluster",99.8f,43));
  CoolRock(Prop(c,"CarvedStone",107.5f,47.5f));
  CoolRock(Prop(c,"RockOverhang",113,55.3f));
  CoolRock(Prop(c,"BrokenPillar",118,47.5f));

  Fall(c,"08 Lower distant cascade",16,-4,2,21,false,true);
  Fall(c,"08 Waterworks overflow",31.5f,-3,2.3f,32,false,true);
  Fall(c,"08 High stream overflow",57,0,3,28,false,true);
  Fall(c,"08 Grand Waterfall distant curtain",100,2,10,56,false,true);
  Fall(c,"08 Foreground waterfall concealing the cavern",109.5f,18,5.5f,40,true);
 }

 static void CoolRock(GameObject prop) {
  // Keep the source sprite and pixel density; shift only the distant stone tint.
  foreach(var renderer in prop.GetComponentsInChildren<SpriteRenderer>())
   renderer.color*=new Color(.78f,.87f,1,1);
 }
}
}
