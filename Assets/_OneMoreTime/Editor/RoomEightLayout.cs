using System.Linq;
using UnityEngine;
namespace OneMoreTime.EditorTools {
public static partial class ChapterOneExpansionBuilder {
 // Official Room 8 dot grid: one dot interval = one unit. Blue steps are water,
 // red bridge fragments collide, orange remnants and timber supports are background.
 static void Layout8(BuildContext c){
  c.flow.roomTitle="The Grand Waterfall";c.flow.chapterFinale=true;c.flow.nextRoomScene="";
  c.flow.filterEnabled=false;c.flow.replayUpgradeOnEntry=false;c.flow.enterFromBelow=true;
  c.flow.exitArea=new Rect(123,47.3f,2.8f,5);
  c.room.roomBounds=new Rect(0,-1,126,58);c.room.start=new Vector2(4.5f,1);c.room.entrance=new Vector2(2.2f,-3.5f);
  c.room.exitX=124;c.room.exitFloor=47.3f;
  Solid(c,"08 Arrival ledge from Room 7",3,7,1,-2);

  float[] edges={7,19.7f,27.7f,28.6f,29.7f,37.7f,40.7f,52.6f,59.6f,63.4f,70.4f,83.4f,94.3f,96.3f,97.3f,101.2f,103.5f,115.1f};
  float[] heights={.6f,1.9f,2.9f,3.9f,5.7f,6.7f,7.7f,9.9f,11.7f,14.8f,17.8f,21.8f,24.2f,27.2f,29.1f,33.3f,40.3f};
  for(int i=0;i<heights.Length;i++){
   var current=Water(c,"08 Ascending water step "+(i+1),edges[i],edges[i+1],heights[i],-2);current.velocity=-2;
   foreach(var feature in current.GetComponentsInChildren<WaterFeature>()){feature.size.y+=.5f;feature.Refresh();}
   if(i>0)Fall(c,"08 Water step spill "+i,edges[i]-.13f,heights[i-1],.26f,heights[i]-heights[i-1]);
  }
  RoomEightCrates(c,c.currents[0],19.2f,7.7f);
  RoomEightCrates(c,c.currents[10],82.8f,70.5f);

  // The short red span above the second water step is a bridge. Only the
  // entrance arrows below it mark the crate flow and whirlpool.
  RoomEightBridge(c,"08 Lower red crossing",19.7f,24.7f,2.9f,-1.1f,false);
  RoomEightBridge(c,"08 First statue bridge",29.7f,33.8f,6.9f,2.7f,false);
  Checkpoint(c,"08 Lower river statue",31.1f,6.9f);
  LoopDeck(c,"08 Lower six-second loop",new[]{new Vector2(34.7f,11.75f),new Vector2(34.7f,10.8f),new Vector2(39.7f,10.8f),new Vector2(39.7f,13.8f),new Vector2(34.7f,13.8f)});
  LoopDeck(c,"08 Upper six-second loop",new[]{new Vector2(44.7f,14.75f),new Vector2(44.7f,13.8f),new Vector2(49.7f,13.8f),new Vector2(49.7f,16.8f),new Vector2(44.7f,16.8f)});
  Grass(c,"08 High west grass shelf",53.9f,18.75f,3,11);
  Grass(c,"08 Low grass shelf",56.4f,12.8f,2.5f,6);
  Grass(c,"08 Middle grass shelf",60.8f,15.8f,3,7);
  Grass(c,"08 Narrow east grass shelf",64.9f,17.8f,1.4f,6);

  Wood(c,"08 Remote switch deck",67.2f,70.5f,29.7f);
  Wood(c,"08 High middle deck",74.3f,76.6f,29.7f);
  Wood(c,"08 High east deck",80.2f,82.6f,29.7f);
  Wood(c,"08 Return step upper",85.1f,87.5f,27.6f);
  Wood(c,"08 Return step lower",82,84.4f,25.2f);
  var plate=Plate(c,"08 Moving statue pressure plate",68.7f,29.7f);
  var secondPlate=Plate(c,"08 East simultaneous pressure plate",95.25f,25.7f);
  var deck=Mover(c,"08 One-way statue carrier",84.75f,22.7f,3.1f,new Vector2(11,7),plate);
  deck.requiredPlates=new[]{plate,secondPlate};
  deck.counter=DeckCount(deck.transform);
  deck.motion=RoomPlatform.Motion.OneWayTravel;deck.journeySeconds=10;deck.Tick(0);
  var statue=Checkpoint(c,"08 Traveling goat statue",84.75f,22.7f);statue.transform.SetParent(deck.transform,true);
  var checkpoint=c.root.gameObject.AddComponent<MovingRoomCheckpoint>();checkpoint.room=c.room;checkpoint.replay=c.replay;checkpoint.platform=deck;checkpoint.checkpoint=statue;c.flow.movingCheckpoint=checkpoint;
  var reveal=c.root.gameObject.AddComponent<RoomPlatformReveal>();reveal.plate=plate;reveal.platform=deck;c.flow.platformReveal=reveal;
  TravelGuide(c,"08 One-way statue ascent guide",new[]{new Vector2(84.75f,22.7f),new Vector2(95.75f,29.7f)},false);

  Wood(c,"08 Water stair lower deck",92,94.2f,23.7f);
  Wood(c,"08 Water stair middle deck",94.2f,96.3f,25.7f);
  Wood(c,"08 High water corner deck",99.1f,101.5f,33.6f);
  Wood(c,"08 High water wall step",102,103.4f,36.6f);
  Wood(c,"08 Last timber before the falls",99.9f,101.6f,38.6f);
  RoomEightBridge(c,"08 Falls lower red bridge",103.25f,107.8f,41.6f,37.3f,false);
  RoomEightBridge(c,"08 Falls lower orange bridge",107.8f,111,41.6f,37.3f,true);
  RoomEightBridge(c,"08 Falls upper orange bridge",103.25f,105.2f,44.6f,41.6f,true);
  RoomEightBridge(c,"08 Falls upper red bridge",105.2f,107.4f,44.6f,41.6f,false);
  var cave=Solid(c,"08 Cavern threshold",115.1f,126,47.3f,-2);
  var grass=cave.transform.Find("Continuous grass edge");if(grass!=null)Object.DestroyImmediate(grass.gameObject);
  RoomEightFinaleScenery(c);
 }
 static void RoomEightBridge(BuildContext c,string name,float left,float right,float top,float bottom,bool background){
  if(background){
   var t=Group(c.scenery,name+" — background only");t.position=new Vector2((left+right)/2,top);
   var sr=t.gameObject.AddComponent<SpriteRenderer>();sr.sprite=UnityEditor.AssetDatabase.LoadAssetAtPath<Sprite>(R+"/Generated/WorldOne/BridgeMiddle.asset");sr.sharedMaterial=UnityEditor.AssetDatabase.LoadAssetAtPath<Material>(R+"/Generated/WorldOne/WoodenSuspensionBridge.mat");sr.drawMode=SpriteDrawMode.Tiled;sr.size=new Vector2(right-left,sr.sprite.bounds.size.y);sr.sortingOrder=-14;sr.color=new Color(.42f,.46f,.43f,.8f);
  }else Bridge(c,name,left,right,top);
  RoomEightBridgeScaffold(c,name,left,right,top,bottom);
 }
 static void RoomEightCrates(BuildContext c,WaterCurrent2D current,float sourceX,float drainX){
  var root=Group(c.terrain,"08 Six-second crate stream "+sourceX);var source=root.gameObject.AddComponent<CrateSpawner>();
  source.water=current;source.spawnInterval=6;source.initialHorizontalVelocity=-2;source.maximumActiveCrates=4;
  source.spawnPoint=Group(root,"Right-bank crate source");source.spawnPoint.position=new Vector2(sourceX,current.surfaceY+.28f);
  var drain=Group(root,"Left whirlpool drain");drain.position=new Vector2(drainX,current.surfaceY);
  source.disposal=drain.gameObject.AddComponent<CrateDisposalTrigger>();source.disposal.owner=source;source.disposal.disposeToRight=false;source.disposal.boundaryX=drainX;
  source.disposal.sinkBeforeDisposal=true;source.disposal.sinkDepth=2.5f;source.disposal.sinkSpeed=3.5f;
  var box=drain.GetComponent<BoxCollider2D>();box.isTrigger=true;box.size=new Vector2(.5f,5);
  source.pool=new FloatingObject2D[4];
  for(int i=0;i<source.pool.Length;i++){var g=Prefab(root,R+"/Prefabs/FloatingWaterCrate.prefab",source.spawnPoint.position);g.name="08 Rewindable water crate "+i;source.pool[i]=g.GetComponent<FloatingObject2D>();source.pool[i].water=current;g.SetActive(false);}
  c.replay.participants=c.replay.participants.Concat(new RewindParticipant[]{source}).ToArray();
  var eddy=Prefab(c.scenery,WaterFeatureBuilder.Prefabs+"/SurfaceEddy.prefab",new Vector2(drainX,current.surfaceY-.38f));eddy.name="08 Crate drain swirl";eddy.transform.localScale=new Vector3(1.2f,.6f,1);eddy.GetComponent<SpriteRenderer>().sortingOrder=7;
 }
}
}
