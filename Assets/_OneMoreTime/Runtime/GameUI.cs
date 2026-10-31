using UnityEngine;
namespace OneMoreTime {
public static class GameUI {
 public static readonly Color Ink=new Color(.075f,.17f,.16f),Cream=new Color(.99f,.95f,.82f),Mint=new Color(.57f,.88f,.69f),Muted=new Color(.67f,.76f,.68f),Gold=new Color(.92f,.75f,.43f),Border=new Color(.32f,.43f,.33f);
 static GUIStyle label,button;static Font current;public static float Height=>540;
 public static Rect GameplayViewport(float width,float height){
  float scale=Mathf.Min(width/960f,height/540f);
  return new Rect(Mathf.Max(0,width-960*scale)*.5f/width,((height-540*scale)*.5f+37*scale)/height,Mathf.Min(1,960*scale/width),503*scale/height);
 }
 public static void Begin(Font font){float scale=Mathf.Min(Screen.width/960f,Screen.height/540f);GUI.matrix=Matrix4x4.TRS(new Vector3((Screen.width-960*scale)/2,(Screen.height-540*scale)/2,0),Quaternion.identity,Vector3.one*scale);GUI.color=Color.white;if(label==null||current!=font){current=font;label=new GUIStyle(GUI.skin.label){font=font,wordWrap=true,richText=false,padding=new RectOffset(0,0,0,0)};button=new GUIStyle(GUIStyle.none);}}
 public static void Panel(Rect r,Color c){var old=GUI.color;GUI.color=c;GUI.DrawTexture(r,Texture2D.whiteTexture);GUI.color=old;}
 public static void Frame(Rect r,Color? fill=null){Panel(new Rect(r.x+4,r.y+5,r.width,r.height),new Color(.02f,.07f,.06f,.5f));Panel(r,Border);Panel(new Rect(r.x+2,r.y+2,r.width-4,r.height-4),fill??new Color(.075f,.17f,.16f,.96f));Panel(new Rect(r.x+8,r.y,r.width-16,2),Gold);}
 public static void Text(Rect r,string text,int size=18,Color? color=null,TextAnchor align=TextAnchor.UpperLeft){label.fontSize=size;label.normal.textColor=color??Cream;label.alignment=align;GUI.Label(r,text,label);}
 public static bool Button(Rect r,string text,bool primary=false,string key=null){bool hover=r.Contains(Event.current.mousePosition)&&GUI.enabled;var fill=primary?(hover?Cream:Gold):(hover?new Color(.20f,.32f,.26f):Ink);Panel(new Rect(r.x,r.y+4,r.width,r.height),new Color(.025f,.075f,.06f,.8f));Panel(r,primary?new Color(.50f,.38f,.21f):Border);Panel(new Rect(r.x+2,r.y+2,r.width-4,r.height-4),fill);Panel(new Rect(r.x+4,r.y+2,r.width-8,2),primary?new Color(1,.9f,.65f):new Color(.37f,.48f,.35f));Text(new Rect(r.x+18,r.y+3,r.width-(key==null?36:100),r.height-6),text,18,primary?Ink:Cream,TextAnchor.MiddleLeft);if(key!=null)Key(new Rect(r.xMax-65,r.y+10,51,r.height-20),key,primary);return GUI.Button(r,GUIContent.none,button);}
 public static void Key(Rect r,string text,bool dark=false){Panel(new Rect(r.x,r.y+3,r.width,r.height),dark?new Color(.4f,.32f,.2f):new Color(.025f,.065f,.05f));Panel(r,dark?new Color(.53f,.43f,.27f):Border);Panel(new Rect(r.x+1,r.y+1,r.width-2,r.height-3),dark?new Color(.79f,.62f,.34f):new Color(.20f,.29f,.24f));Text(r,text,Mathf.Min(14,(int)r.height-8),dark?Ink:Cream,TextAnchor.MiddleCenter);}
 public static void KeyLabel(float x,float y,string key,string description,float width=110){float kw=key.Length>3?54:key.Length>1?42:27;Key(new Rect(x,y,kw,26),key);Text(new Rect(x+kw+8,y,width,26),description,13,Muted,TextAnchor.MiddleLeft);}
 static Texture2D dialogueFrame,goatPortrait;
 public static bool Dialogue(string text,string next,int textSize=22){
  if(dialogueFrame==null)dialogueFrame=Resources.Load<Texture2D>("Dialogue/OpeningDialogue");
  if(goatPortrait==null)goatPortrait=Resources.Load<Texture2D>("Dialogue/GoatPortrait");
  Panel(new Rect(0,0,960,540),new Color(.025f,.06f,.12f,.35f));
  if(dialogueFrame!=null)GUI.DrawTexture(new Rect(32,276,896,227.47f),dialogueFrame,ScaleMode.StretchToFill);
  if(goatPortrait!=null)GUI.DrawTexture(new Rect(35,278,221,221),goatPortrait,ScaleMode.ScaleToFit);
  Text(new Rect(290,296,126,37),"小山羊",19,new Color(.16f,.15f,.30f),TextAnchor.MiddleCenter);
  Text(new Rect(300,350,584,90),text,textSize,new Color(.16f,.15f,.30f));
  Text(new Rect(302,442,300,25),"Enter / 空白鍵 繼續",12,new Color(.34f,.32f,.48f));
  return Button(new Rect(706,433,180,37),next,true);
 }
 public static void Shade(){Panel(new Rect(0,0,960,540),new Color(.025f,.075f,.065f,.82f));}
 public static void Divider(float x,float y,float width){Panel(new Rect(x,y,width,1),Border);}
 public static void Diamond(float x,float y,Color color){Panel(new Rect(x+3,y,4,10),color);Panel(new Rect(x,y+3,10,4),color);}
}
}
