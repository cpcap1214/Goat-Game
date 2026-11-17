using System.IO;
using UnityEditor;
using UnityEngine;
namespace OneMoreTime.EditorTools {
public static class MainRoomArtBuilder {
 const string Dir=DemoBuilder.Root+"/Art/MainRooms";
 static Texture2D canvas;
 static Color C(string hex){ColorUtility.TryParseHtmlString("#"+hex,out var c);return c;}
 static void Box(int x,int y,int w,int h,string color){for(int a=x;a<x+w;a++)for(int b=y;b<y+h;b++)if(a>=0&&a<canvas.width&&b>=0&&b<canvas.height)canvas.SetPixel(a,b,C(color));}
 static void Line(int x,int y,int dx,int dy,int n,string color){for(int i=0;i<n;i++)Box(x+dx*i,y+dy*i,2,2,color);}
 static void Begin(int w,int h){canvas=new Texture2D(w,h,TextureFormat.RGBA32,false);canvas.SetPixels(new Color[w*h]);}
 static Sprite Save(string name){Directory.CreateDirectory(Dir);canvas.Apply();string path=Dir+"/"+name+".png";File.WriteAllBytes(path,canvas.EncodeToPNG());Object.DestroyImmediate(canvas);AssetDatabase.ImportAsset(path);var i=(TextureImporter)AssetImporter.GetAtPath(path);i.textureType=TextureImporterType.Sprite;i.spritePixelsPerUnit=32;i.filterMode=FilterMode.Point;i.mipmapEnabled=false;i.alphaIsTransparency=true;i.textureCompression=TextureImporterCompression.Uncompressed;var settings=new TextureImporterSettings();i.ReadTextureSettings(settings);settings.spriteMeshType=SpriteMeshType.FullRect;i.SetTextureSettings(settings);i.SaveAndReimport();return AssetDatabase.LoadAssetAtPath<Sprite>(path);}
 public static Sprite Art(string name)=>AssetDatabase.LoadAssetAtPath<Sprite>(Dir+"/"+name+".png");
 public static void Generate(){
  Begin(64,16);Box(0,4,64,12,"302e32");Box(1,6,62,9,"81553d");Box(1,13,62,2,"c9945b");Box(0,4,64,2,"4c3630");
  for(int x=0;x<64;x+=16){Box(x,5,1,9,"47352e");Box(x+3,10,10,1,"aa7548");Box(x+6,7,7,1,"5d4031");Box(x+2,5,2,7,"59626a");Box(x+2,11,2,2,"c5bca3");Box(x+2,6,1,1,"252c37");}Box(8,0,5,4,"543e34");Box(50,0,5,4,"543e34");Save("WeatheredWoodPlatform");
  Begin(96,112);
  // Riveted iron cage, worn boards, overhead pulley and separate sliding gate.
  Box(9,8,78,88,"292f38");Box(15,14,66,76,"443e3e");
  for(int x=16;x<81;x+=9){Box(x,16,7,72,"655346");Box(x+1,17,1,68,"8b6d50");Box(x+4,23,1,14,"423b37");}
  Line(18,20,1,1,60,"383e42");Line(18,79,1,-1,60,"383e42");
  Box(5,6,8,91,"323e46");Box(83,6,8,91,"323e46");Box(6,7,2,89,"8a9187");Box(84,7,2,89,"8a9187");
  Box(7,90,82,9,"2d3540");Box(9,95,78,2,"a08d6c");Box(11,90,74,3,"657272");
  Box(2,3,92,8,"2b323c");Box(3,9,90,3,"b58b53");Box(5,6,86,2,"805e40");
  for(int y=17;y<89;y+=17){Box(7,y,3,3,"c5b58c");Box(85,y,3,3,"c5b58c");Box(10,y+4,2,5,"8c5940");}
  Box(38,99,20,10,"303941");Box(41,101,14,8,"938b72");Box(45,103,6,4,"303941");Box(47,109,2,3,"a1a493");
  Box(38,80,20,8,"303941");Box(41,82,14,4,"d3a65b");Box(44,83,8,2,"f0d58b");Save("OldLiftCabin");
  Begin(64,80);Box(0,0,64,3,"2c3640");Box(0,76,64,4,"687b78");
  for(int x=2;x<64;x+=10){Box(x,3,3,73,"303b45");Box(x,4,1,71,"93a39a");}
  for(int y=12;y<72;y+=24){Line(3,y,1,1,24,"657d7b");Line(35,y+23,1,-1,24,"657d7b");}Box(28,35,8,10,"bf9054");Box(31,39,2,4,"343d44");Save("OldLiftGate");
  Begin(32,10);Box(0,0,32,2,"386641");for(int x=0;x<32;x+=2){int h=3+(x*5)%7;Box(x,1,2,h,"579748");Box(x,2,1,h-1,"91bd50");if(x%4==0)Box(x+1,h,1,2,"c3d777");}Save("GrassBlades");
  Begin(48,8);Box(0,0,48,4,"333e43");Box(3,3,42,4,"727f78");Box(5,6,38,2,"d5ad64");for(int x=8;x<42;x+=8)Box(x,4,3,2,"4b554f");Save("PressurePlate");
 }
 public static void Background(GameObject root,MainRoom room){
  if(room.sky!=null)Object.DestroyImmediate(room.sky.gameObject);room.sky=null;
  var old=root.transform.Find("Tutorial scenery");if(old!=null)Object.DestroyImmediate(old.gameObject);
  var backdrop=new GameObject("Tutorial scenery");backdrop.transform.SetParent(root.transform);var b=backdrop.AddComponent<MainRoomBackdrop>();b.view=room.view;
  b.mountains=new GameObject("Mountains").transform;b.mountains.SetParent(backdrop.transform);
  b.forest=new GameObject("Forest").transform;b.forest.SetParent(backdrop.transform);
  for(int x=-2;x<=2;x++)PresentationBuilder.Image(b.mountains,"Tutorial mountains",DemoBuilder.Sprite("Environment/back.png"),new Vector2(x*48,0),new Vector2(48,30),-40,new Color(.87f,.96f,.94f));
  for(int x=-5;x<=8;x++)PresentationBuilder.Image(b.forest,"Tutorial forest",DemoBuilder.Sprite("Environment/middle.png"),new Vector2(x*9,-6),new Vector2(10,20.91f),-30,new Color(.70f,.88f,.82f));
 }
 public static void Grass(Transform parent,Rect rect){
  var g=new GameObject("Grass blades and roots");g.transform.SetParent(parent);g.transform.position=new Vector2(rect.center.x,rect.yMax-.5f);var sr=g.AddComponent<SpriteRenderer>();sr.sprite=AssetDatabase.LoadAssetAtPath<Sprite>(DemoBuilder.Root+"/Generated/GrassEdgeSeamless.asset");sr.drawMode=SpriteDrawMode.Tiled;sr.size=new Vector2(rect.width,1);sr.sortingOrder=5;
  if(Art("GrassBlades")==null)Generate();var fringe=new GameObject("Individual grass blades");fringe.transform.SetParent(g.transform);fringe.transform.position=new Vector2(rect.center.x,rect.yMax-.065f);var blades=fringe.AddComponent<SpriteRenderer>();blades.sprite=Art("GrassBlades");blades.drawMode=SpriteDrawMode.Tiled;blades.size=new Vector2(rect.width,10f/32);blades.sortingOrder=6;
 }
 public static void Wood(Transform parent,Rect rect){var g=new GameObject("Weathered timber");g.transform.SetParent(parent);g.transform.position=new Vector2(rect.center.x,rect.yMax-.25f);var sr=g.AddComponent<SpriteRenderer>();sr.sprite=Art("WeatheredWoodPlatform");sr.drawMode=SpriteDrawMode.Tiled;sr.size=new Vector2(rect.width,.5f);sr.sortingOrder=5;}
}
}
