using UnityEngine;

namespace OneMoreTime.EditorTools {
public static partial class MainRoomDecorationBuilder {
 // Grounded props use their native bottom-center pivot. Large silhouettes stay
 // in the darker scenery layer so they do not read as additional platforms.
 static void DecorateRoom1(Transform root) {
  // A sheltered trail, with open space around the movement and jump prompts.
  Prop(root,"WindsweptPine",1.3f,0,true);
  Prop(root,"SmallFern",.9f,0);
  Prop(root,"MeadowFlowers",6.1f,0);
  Prop(root,"HollowStump",9,0,true);
  Prop(root,"ForestMushrooms",10.4f,0);
  Prop(root,"BroadFern",14.6f,0,true);
  Prop(root,"SmallFern",15.4f,0);
  Prop(root,"MossyFallenLog",21.6f,0,true);
  Prop(root,"ForestMushrooms",22.9f,0);

  // The first shrine remains the brightest and clearest object on its ledge.
  Prop(root,"HangingRoots",25.05f,2.65f,true);
  Prop(root,"MossyRiverRock",27,2.8f,true);
  Prop(root,"MeadowFlowers",28.1f,2.8f);
  Prop(root,"SmallFern",33.2f,2.8f);

  // Fern and rock clusters suggest the rising forest beyond the dry ravine.
  Prop(root,"MossyOutcrop",40.6f,2.8f,true);
  Prop(root,"BroadFern",42.1f,2.8f);
  Prop(root,"WindsweptPine",46.2f,2.8f,true,true);
  Prop(root,"ForestMushrooms",47.7f,2.8f);
  Prop(root,"HollowStump",50.6f,2.8f,true);
  Prop(root,"SmallFern",52.2f,2.8f);
  Prop(root,"MeadowFlowers",54.2f,2.8f);
  Prop(root,"SmallFern",61,5.3f,true);
  Prop(root,"ForestMushrooms",55.6f,7.3f,true);
  Prop(root,"AlpineShrub",61.9f,9.8f,true);

  // More exposed vegetation at the upper path; keep the exit marker visible.
  Prop(root,"HangingRoots",64.9f,12.25f,true);
  Prop(root,"FlatBoulder",67.2f,12.4f,true);
  Prop(root,"AlpineShrub",68.3f,12.4f);
  Prop(root,"MeadowFlowers",71,12.4f);
  Prop(root,"WindsweptPine",73.1f,12.4f,true,true);
  Prop(root,"SmallFern",77.1f,12.4f);
 }

 static void DecorateRoom2(Transform root) {
  // Mossy forest ruins frame the Shutter discovery without covering the pickup.
  Prop(root,"WindsweptPine",.8f,0,true);
  Prop(root,"SmallFern",1.4f,0);
  Prop(root,"MossyRiverRock",5,0,true);
  Prop(root,"ForestMushrooms",6.2f,0);
  Prop(root,"BrokenPillar",8,0,true);
  Prop(root,"BroadFern",9.1f,0,true);
  Prop(root,"MeadowFlowers",10.2f,0);
  Prop(root,"CarvedStone",15.3f,0,true);
  Prop(root,"SmallFern",16.2f,0);
  Prop(root,"HangingRoots",19.05f,4.8f,true);

  // The checkpoint has a quiet clearing; the ravine lip stays unobscured.
  Prop(root,"MossyOutcrop",20.3f,5,true);
  Prop(root,"AlpineShrub",21.5f,5,true);
  Prop(root,"MeadowFlowers",22.2f,5);
  Prop(root,"SmallFern",26.1f,5);
  Prop(root,"HangingRoots",26.9f,4.8f,true,true);

  // The far bank continues the abandoned stone route toward the lift rooms.
  Prop(root,"HangingRoots",36.05f,4.8f,true);
  Prop(root,"ForestMushrooms",37,5);
  Prop(root,"RuinedDoorway",39.6f,5,true);
  Prop(root,"SmallFern",38.2f,5);
  Prop(root,"MeadowFlowers",41.5f,5);
  Prop(root,"WindsweptPine",45.2f,5,true,true);
  Prop(root,"BroadFern",46.8f,5,true);
 }

 static void DecorateRoom3(Transform root) {
  // A forest floor gives way to roots and the remains of mountain lift works.
  Prop(root,"WindsweptPine",.8f,0,true);
  Prop(root,"SmallFern",1.6f,0);
  Prop(root,"HollowStump",5.4f,0,true);
  Prop(root,"ForestMushrooms",6.6f,0);
  Prop(root,"MossyOutcrop",9,0,true);
  Prop(root,"BroadFern",10.2f,0,true);

  // Leave Plate A and the complete shuttle track free of added artwork.
  Prop(root,"SmallFern",17.8f,2.5f);
  Prop(root,"MeadowFlowers",18.2f,2.5f,true);
  Prop(root,"HangingRoots",18.95f,2.3f,true);
  Prop(root,"SmallFern",6.65f,6,true);
  Prop(root,"ForestMushrooms",8.3f,6);
  Prop(root,"HangingRoots",6.05f,5.8f,true);
  Prop(root,"SmallFern",10.25f,11,true);

  // Bracing belongs to the cliff face behind the existing wooden steps.
  WallDetail(root,"BracedTimber",26,13);
  Prop(root,"HangingRoots",24.2f,20.3f,true);

  // The timer landing has no prop on its statue or pressure-plate footprints.
  Prop(root,"SmallFern",20.85f,22,true);
  Prop(root,"HangingRoots",20.95f,21.8f,true);
  Prop(root,"ForestMushrooms",2.7f,25,true);
  Prop(root,"SmallFern",9.7f,25,true);
  Prop(root,"HangingRoots",2.05f,24.8f,true);
  Prop(root,"AlpineShrub",6,31,true);
  Prop(root,"MeadowFlowers",7.2f,31);
  Prop(root,"SmallFern",13.1f,32,true);
  WallDetail(root,"BracedTimber",26,33.4f);

  // Sparse windswept vegetation flanks, rather than fills, the lift entrance.
  Prop(root,"WindsweptPine",27.5f,39,true);
  Prop(root,"AlpineShrub",27,39,true);
  Prop(root,"MeadowFlowers",28.1f,39);
  Prop(root,"HangingRoots",24.05f,38.8f,true);
 }
}
}
