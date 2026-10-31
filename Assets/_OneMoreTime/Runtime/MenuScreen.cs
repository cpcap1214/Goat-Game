using UnityEngine;using UnityEngine.InputSystem;
namespace OneMoreTime {
public sealed class MenuScreen:MonoBehaviour {
 public Font font;public Texture2D artwork;public bool ending;public Texture2D dialogueFrame,goatPortrait;bool instructions;int storyPage=-1;bool enteringLevel;
 EndingCinematic endingCinematic;
 Texture2D titleLogo;Material titleLogoMaterial;
 void Awake(){if(ending){endingCinematic=new EndingCinematic();return;}titleLogo=Resources.Load<Texture2D>("Branding/GoatTowerAgain");var shader=Resources.Load<Shader>("Branding/LogoMask");if(titleLogo!=null&&shader!=null)titleLogoMaterial=new Material(shader);}
 void OnDestroy(){endingCinematic?.Dispose();if(titleLogoMaterial!=null)Destroy(titleLogoMaterial);}
 readonly string[] story={
 "完蛋了，剛剛在我的魔法塔做實驗。\n一不小心沒站穩就掉到山谷下了",
 "還好我是 GOAT，不會受傷\n但是我還是要爬回山上，有夠麻煩",
 "對了，我的時間道具也一起掉下來了\n先撿回來，再爬回去吧！"};
 void StartJourney(){if(ending)SceneFlow.Load(0);else storyPage=0;}
 void NextStory(){if(storyPage<story.Length-1)storyPage++;else EnterLevel();}
 void EnterLevel(){if(enteringLevel)return;enteringLevel=true;SceneFlow.Load(1);}
 void Update(){var k=Keyboard.current;if(ending){endingCinematic.Sequence.Tick(Time.unscaledDeltaTime);if(k!=null){if(k.escapeKey.wasPressedThisFrame)endingCinematic.Sequence.Skip();else if(endingCinematic.Sequence.Complete&&k.enterKey.wasPressedThisFrame)StartJourney();}return;}if(k==null||enteringLevel)return;
  if(storyPage>=0){if(k.escapeKey.wasPressedThisFrame)EnterLevel();else if(k.enterKey.wasPressedThisFrame||k.spaceKey.wasPressedThisFrame)NextStory();return;}
  if(k.escapeKey.wasPressedThisFrame)instructions=false;
  else if(k.enterKey.wasPressedThisFrame&&!instructions)StartJourney();
 }
 void OnGUI(){
  GUI.matrix=Matrix4x4.identity;if(ending)GameUI.Panel(new Rect(0,0,Screen.width,Screen.height),new Color(.03f,.08f,.12f));if(!ending&&artwork!=null)GUI.DrawTexture(new Rect(0,0,Screen.width,Screen.height),artwork,ScaleMode.ScaleAndCrop);GameUI.Begin(font);
  if(ending){endingCinematic.Draw();if(endingCinematic.Sequence.Complete&&GameUI.Button(new Rect(60,353,300,51),"再玩一次",true,"Enter"))StartJourney();return;}
  if(storyPage>=0){DrawStory();return;}
  GameUI.Panel(new Rect(0,0,475,540),new Color(.025f,.10f,.12f,.16f));
  if(ending)GameUI.Text(new Rect(60,170,400,80),"恭喜通關！",48,GameUI.Cream,TextAnchor.MiddleLeft);
  if(!ending&&titleLogoMaterial!=null&&Event.current.type==EventType.Repaint){Graphics.DrawTexture(new Rect(38,10,340,340),titleLogo,titleLogoMaterial);}
  if(GameUI.Button(new Rect(60,353,300,51),ending?"再玩一次":"開始旅程",true,"Enter"))StartJourney();
  if(!ending&&GameUI.Button(new Rect(60,420,300,43),"旅人手冊 / 操作說明"))instructions=true;
  if(instructions){GameUI.Shade();GameUI.Frame(new Rect(238,50,484,438));GameUI.Text(new Rect(270,78,420,40),"旅人手冊",30);GameUI.Divider(270,165,420);
   string[] keys={"A D","Space","E","Q","R","Esc"};string[] hints={"左右移動，也可使用方向鍵","跳躍；放開可短跳","定格；已有定格時再按可重新準備","開始／保存／播放／重新準備","保留記憶，重新嘗試","暫停與音量設定"};for(int i=0;i<6;i++){GameUI.Key(new Rect(270,185+i*36,60,25),keys[i]);GameUI.Text(new Rect(350,183+i*36,330,29),hints[i],15,GameUI.Cream,TextAnchor.MiddleLeft);}if(GameUI.Button(new Rect(270,423,420,40),"繼續",true))instructions=false;
  }
 }
 void DrawStory(){
  if(GameUI.Dialogue(story[storyPage],storyPage==story.Length-1?"出發吧":"下一句"))NextStory();
  if(GameUI.Button(new Rect(726,30,194,36),"略過劇情",false,"Esc"))EnterLevel();
 }

}
}
