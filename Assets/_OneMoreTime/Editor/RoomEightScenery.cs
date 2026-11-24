using UnityEditor;
using UnityEngine;

namespace OneMoreTime.EditorTools {
public static partial class ChapterOneExpansionBuilder {
 // Scenery only: the official drawing's decks, water hazards and cave floor
 // remain owned by Layout8. Every new child is below the scenery root.
 static void RoomEightFinaleScenery(BuildContext c) {
  var root=Group(c.scenery,"08 Grand Waterfall and Chapter 2 cave scenery");
  ShadeCaveThreshold(c);

  // A continuous empty passage is more readable than a row of props at goat
  // height. The darkness starts behind the waterfall and continues offscreen.
  CaveShade(root,"Cavern depth",114.75f,126.5f,47.2f,62.5f,new Color(.055f,.063f,.075f),-19);
  CaveShade(root,"Shadow behind falling water",114.4f,114.8f,47.2f,54.6f,new Color(.095f,.115f,.12f),-19);
  CaveShade(root,"Recessed cave mouth",114.75f,118.2f,47.2f,55.3f,new Color(.095f,.11f,.115f),-18);
  CaveShade(root,"Middle chamber",118.2f,122.2f,47.2f,55.3f,new Color(.073f,.088f,.102f),-18);
  CaveShade(root,"Light fading deeper inside",122.2f,126.5f,47.2f,55.3f,new Color(.055f,.068f,.083f),-18);

  // Keep native prop scale and the shared 16-pixel material. Repeated pieces,
  // rather than a stretched arch, join the forest cliff to the long cave.
  CaveRock(c,root,"CliffCluster",114.4f,43.4f,-12,new Color(.39f,.46f,.45f),true);
  CaveRock(c,root,"RockOverhang",116.6f,55.5f,-11,new Color(.43f,.49f,.48f));
  CaveRock(c,root,"RockOverhang",120.45f,55.55f,-11,new Color(.34f,.4f,.42f),true);
  CaveRock(c,root,"RockOverhang",124.3f,55.5f,-11,new Color(.27f,.33f,.37f));
  // Three pixel-high stone edging sits entirely beneath the real floor at
  // y=47.3. It makes the walkable surface explicit without adding any ledge.
  CaveShade(root,"Continuous cave floor rim",115.1f,126,47.1125f,47.3f,new Color(.24f,.28f,.29f),3);
  CaveShade(root,"One-pixel lit stone edge",115.1f,126,47.2375f,47.3f,new Color(.43f,.47f,.46f),4);

  // Sparse vegetation survives at the entrance, then gives way to damp bare
  // rock. Hanging foliage stays behind the goat and clear of the exit.
  CaveRock(c,root,"AlpineShrub",108.4f,40.3f,-13,new Color(.62f,.71f,.59f));
  CaveRock(c,root,"MossyRiverRock",112.2f,40.1f,-13,new Color(.59f,.67f,.6f));
  CaveRock(c,root,"HangingRoots",115.65f,54.25f,-10,new Color(.4f,.47f,.42f));

  // Broad shaded water gives scale behind the route. The clear foreground
  // ribbons share the same water shader and clock, preserving animation phase
  // and pixel size while leaving the goat visible through the curtain.
  // All curtains continue beyond the highest possible camera edge. Their
  // shared crest connects to the river above the cave instead of starting
  // halfway down the player's view.
  const float waterfallBottom=40.3f,waterfallCrest=62;
  float waterfallHeight=waterfallCrest-waterfallBottom;
  FinaleWater(root,"Grand Waterfall — shaded depth",WaterFeature.Shape.Waterfall,
   new Vector2(108.7f,waterfallBottom),new Vector2(6.4f,waterfallHeight),.72f,-16,false,true,true);
  FinaleWater(root,"Grand Waterfall — foreground curtain",WaterFeature.Shape.Waterfall,
   new Vector2(111.5f,waterfallBottom),new Vector2(3.6f,waterfallHeight),.37f,22,true,false,true);
  FinaleWater(root,"Grand Waterfall — middle current ribbon",WaterFeature.Shape.Waterfall,
   new Vector2(112.6f,waterfallBottom),new Vector2(1.35f,waterfallHeight),.12f,23,true,false,false);
  FinaleWater(root,"Grand Waterfall — near edge ribbon",WaterFeature.Shape.Waterfall,
   new Vector2(111.3f,waterfallBottom),new Vector2(.55f,waterfallHeight),.36f,23,true,false,false);
  FinaleWater(root,"Grand Waterfall — far edge ribbon",WaterFeature.Shape.Waterfall,
   new Vector2(114.8f,waterfallBottom),new Vector2(.55f,waterfallHeight),.34f,23,true,false,false);
  FinaleWater(root,"River above the hidden cavern",WaterFeature.Shape.River,
   new Vector2(111.5f,waterfallCrest-.75f),new Vector2(15,.75f),.32f,22,true,false,false);
  FinaleWater(root,"Grand Waterfall — rolling impact spray",WaterFeature.Shape.Mist,
   new Vector2(107.5f,40.2f),new Vector2(10.5f,2.4f),.35f,24,true,false,false);
  FinaleWater(root,"Mist drifting across the cavern mouth",WaterFeature.Shape.Mist,
   new Vector2(111.1f,47.3f),new Vector2(4.5f,1.2f),.12f,24,true,false,false);

  for(int i=0;i<4;i++) {
   FinaleAccent(root,"ImpactFoam",new Vector2(109.2f+i*1.7f,40.3f),.3f,24);
   FinaleAccent(root,"FineWaterMist",new Vector2(109.7f+i*1.65f,40.6f+(i%2)*.25f),.2f,25);
  }
  FinaleAccent(root,"DripCurtain",new Vector2(116.2f,53.6f),.12f,-9);
 }

 static void ShadeCaveThreshold(BuildContext c) {
  var threshold=c.terrain.Find("08 Cavern threshold");
  if(threshold==null)return;
  // Solid() contributes default grass and little ferns. Strip only those
  // automatic decorations from this cave floor, leaving the entrance's
  // deliberately placed roots, moss and rock props intact.
  for(int i=threshold.childCount-1;i>=0;i--) {
   var child=threshold.GetChild(i);
   if(child.name=="Continuous grass edge"||child.name=="Hanging fern")
    Object.DestroyImmediate(child.gameObject);
  }
  // Retain the established terrain texture and collision. The unequal tint
  // channels neutralize its orange highlights into damp charcoal stone.
  foreach(var sr in threshold.GetComponentsInChildren<SpriteRenderer>())
   sr.color=new Color(.23f,.43f,.64f,1);
 }

 static void CaveShade(Transform parent,string name,float left,float right,float bottom,float top,Color color,int order) {
  PresentationBuilder.Image(parent,name,PresentationBuilder.White(),new Vector2((left+right)/2,(bottom+top)/2),new Vector2(right-left,top-bottom),order,color);
 }

 static void CaveRock(BuildContext c,Transform parent,string asset,float x,float y,int order,Color color,bool flip=false) {
  var g=Prop(c,asset,x,y,false);g.transform.SetParent(parent,true);
  g.name="Cave transition — "+asset;
  foreach(var sr in g.GetComponentsInChildren<SpriteRenderer>()) {sr.sortingOrder=order;sr.color=color;sr.flipX=flip;}
 }

 static WaterFeature FinaleWater(Transform parent,string name,WaterFeature.Shape shape,Vector2 position,Vector2 size,float opacity,int order,bool foreground,bool background,bool grand) {
  string folder=R+"/Generated/WorldOne/WaterKit/";
  var g=Group(parent,name);g.position=position;
  var sr=g.gameObject.AddComponent<SpriteRenderer>();
  sr.sprite=AssetDatabase.LoadAssetAtPath<Sprite>(folder+(grand?"GrandWaterfallQuad":"WaterFeatureQuad")+".asset");
  sr.sharedMaterial=AssetDatabase.LoadAssetAtPath<Material>(folder+(grand?"GrandWaterfall":"WaterFeature")+".mat");
  var water=g.gameObject.AddComponent<WaterFeature>();
  water.shape=shape;water.size=size;water.opacity=opacity;water.sortingOrder=order;water.foreground=foreground;water.background=background;water.Refresh();
  return water;
 }

 static void FinaleAccent(Transform parent,string name,Vector2 position,float opacity,int order) {
  var g=Prefab(parent,WaterFeatureBuilder.Prefabs+"/"+name+".prefab",position);
  g.name="Grand Waterfall accent — "+name;
  foreach(var sr in g.GetComponentsInChildren<SpriteRenderer>()) {sr.color=new Color(1,1,1,opacity);sr.sortingOrder=order;}
 }

 // Official Room 8 bridges need support art down to their own nearby water
 // level, not the fixed Room 5 base. These beams never create collision.
 static void RoomEightBridgeScaffold(BuildContext c,string name,float left,float right,float top,float bottom) {
  var root=Group(c.scenery,name+" — scaffold beams");
  var template=AssetDatabase.LoadAssetAtPath<GameObject>(ForestExpansionBuilder.Prefabs+"/TimberFrameBackground.prefab").GetComponent<SpriteRenderer>();
  float nativeWidth=template.sprite.bounds.size.x,nativeHeight=template.sprite.bounds.size.y;
  int bays=Mathf.Max(1,Mathf.CeilToInt((right-left)/nativeWidth));
  float width=(right-left)/bays;
  for(int bay=0;bay<bays;bay++)for(float y=top-.14f;y>bottom;y-=nativeHeight) {
   float height=Mathf.Min(nativeHeight,y-bottom);
   var frame=Group(root,"Damp timber support");frame.position=new Vector2(left+(bay+.5f)*width,y-height);
   frame.localScale=new Vector3(width/nativeWidth,height/nativeHeight,1);
   var sr=frame.gameObject.AddComponent<SpriteRenderer>();sr.sprite=template.sprite;sr.sharedMaterial=template.sharedMaterial;
   sr.sortingOrder=-17;sr.color=new Color(.37f,.42f,.39f,.88f);
  }
 }
}
}
