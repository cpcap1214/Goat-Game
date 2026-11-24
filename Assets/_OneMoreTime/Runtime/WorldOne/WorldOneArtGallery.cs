using UnityEngine;
using UnityEngine.InputSystem;
namespace OneMoreTime {
public sealed class WorldOneArtGallery:MonoBehaviour {
 public StoneCheckpointVisual statue;public Transform exhibits;public Camera view;public Font font;public WorldOneSequence drowning;public PlayerVisual goat;
 public Vector2[] sectionOrigins;public float[] sectionSizes;public string[] sectionNames;
 float horizontal,height;bool sinking;int section;
 StatueRevival revival;Vector2 previewGoatFeet;bool reviving;
 void Start(){drowning.target.enabled=false;}
 void Update(){var k=Keyboard.current;if(k==null)return;
  horizontal+=((k.rightArrowKey.isPressed||k.dKey.isPressed?1:0)-(k.leftArrowKey.isPressed||k.aKey.isPressed?1:0))*Time.deltaTime*8;
  height=height+((k.upArrowKey.isPressed||k.wKey.isPressed?1:0)-(k.downArrowKey.isPressed||k.sKey.isPressed?1:0))*Time.deltaTime*8;
  if(sectionOrigins!=null&&sectionOrigins.Length>0){
   int next=section;if(k.tabKey.wasPressedThisFrame)next=(section+1)%sectionOrigins.Length;
   var keys=new[]{k.digit1Key,k.digit2Key,k.digit3Key,k.digit4Key,k.digit5Key,k.digit6Key};
   for(int i=0;i<keys.Length&&i<sectionOrigins.Length;i++)if(keys[i].wasPressedThisFrame)next=i;
   if(next!=section){if(reviving){revival.Cancel();goat.Pose(previewGoatFeet,1,0,0,Color.white);reviving=false;}section=next;horizontal=height=0;}
  }
  if(sinking)drowning.Tick(Time.deltaTime);
  if(reviving){revival.Tick(Time.deltaTime);if(!revival.Playing){reviving=false;goat.Pose(previewGoatFeet,1,0,0,Color.white);}}
  Vector2 origin=sectionOrigins!=null&&sectionOrigins.Length>section?sectionOrigins[section]:new Vector2(10,5);
  view.transform.position=new Vector3(origin.x+horizontal,height+origin.y,-10);
  if(sectionSizes!=null&&sectionSizes.Length>section)view.orthographicSize=sectionSizes[section];
  if(k.spaceKey.wasPressedThisFrame&&!reviving){drowning.Play();sinking=true;goat.renderer2D.enabled=false;}
  if(sinking&&drowning.Finished){sinking=false;drowning.target.enabled=false;goat.renderer2D.enabled=true;}
  if(k.cKey.wasPressedThisFrame&&!reviving){statue.ResetDisplay();statue.Activate();}
  if(k.vKey.wasPressedThisFrame&&!sinking&&!reviving&&section==0){
   if(revival==null)revival=gameObject.AddComponent<StatueRevival>();
   previewGoatFeet=goat.transform.position;statue.Activate();statue.Tick(1);
   revival.Begin(statue.target,statue,goat,statue.transform.position,1);reviving=revival.Playing;
  }
  if(k.escapeKey.wasPressedThisFrame)SceneFlow.Load(0);
 }
 void OnGUI(){GameUI.Begin(font);GameUI.Panel(new Rect(0,0,960,53),new Color(.025f,.07f,.07f,.9f));GameUI.Text(new Rect(20,10,920,35),"WORLD 1 — THE FOREST & THE GRAND WATERFALL",23,GameUI.Gold);
  if(sectionNames!=null&&sectionNames.Length>section)GameUI.Text(new Rect(20,58,920,30),(section+1)+" / "+sectionNames.Length+"   "+sectionNames[section],18,GameUI.Cream);
  GameUI.Panel(new Rect(0,500,960,40),GameUI.Ink);GameUI.Text(new Rect(18,507,924,25),"1–6/Tab: section   WASD/arrows: pan   Space: drown   C: statue   V: revive (1)   Esc: menu",15,GameUI.Cream,TextAnchor.MiddleCenter);}
}
}
