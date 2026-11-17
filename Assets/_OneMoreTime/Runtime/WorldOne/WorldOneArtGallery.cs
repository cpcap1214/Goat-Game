using UnityEngine;
using UnityEngine.InputSystem;
namespace OneMoreTime {
public sealed class WorldOneArtGallery:MonoBehaviour {
 public StoneCheckpointVisual statue;public Transform exhibits;public Camera view;public Font font;public WorldOneSequence drowning;public PlayerVisual goat;
 float height;bool sinking;
 void Start(){drowning.target.enabled=false;}
 void Update(){var k=Keyboard.current;if(k==null)return;
  height=height+((k.upArrowKey.isPressed?1:0)-(k.downArrowKey.isPressed?1:0))*Time.deltaTime*8;
  // Move the display together; only the backdrop's elevation changes.
  exhibits.localPosition=new Vector3(0,height,0);
  if(sinking)drowning.Tick(Time.deltaTime);
  view.transform.position=new Vector3(10,height+5,-10);
  if(k.spaceKey.wasPressedThisFrame){drowning.Play();sinking=true;goat.renderer2D.enabled=false;}
  if(sinking&&drowning.Finished){sinking=false;drowning.target.enabled=false;goat.renderer2D.enabled=true;}
  if(k.cKey.wasPressedThisFrame){statue.ResetDisplay();statue.Activate();}
  if(k.escapeKey.wasPressedThisFrame)SceneFlow.Load(0);
 }
 void OnGUI(){GameUI.Begin(font);GameUI.Panel(new Rect(0,0,960,53),new Color(.025f,.07f,.07f,.9f));GameUI.Text(new Rect(20,10,920,35),"WORLD 1 — THE FOREST & THE GRAND WATERFALL",23,GameUI.Gold);
  GameUI.Panel(new Rect(0,500,960,40),GameUI.Ink);GameUI.Text(new Rect(18,507,924,25),"↑ / ↓: scroll background   Space: drown   C: activate statue   Esc: menu",16,GameUI.Cream,TextAnchor.MiddleCenter);}
}
}
