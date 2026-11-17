using UnityEngine;using UnityEngine.InputSystem;
namespace OneMoreTime {
public sealed class MenuScreen:MonoBehaviour {
 public Font font;public Texture2D artwork;public bool ending;public Texture2D dialogueFrame,goatPortrait;bool instructions;OpeningCinematic opening;bool enteringLevel;bool tutorials;
 EndingCinematic endingCinematic;
 Texture2D titleLogo;Material titleLogoMaterial;
 void Awake(){if(ending){endingCinematic=new EndingCinematic();return;}titleLogo=Resources.Load<Texture2D>("Branding/GoatTowerAgain");var shader=Resources.Load<Shader>("Branding/LogoMask");if(titleLogo!=null&&shader!=null)titleLogoMaterial=new Material(shader);}
 void OnDestroy(){endingCinematic?.Dispose();opening?.Dispose();if(titleLogoMaterial!=null)Destroy(titleLogoMaterial);}
 void StartJourney(){if(ending)SceneFlow.Load(0);else{opening?.Dispose();opening=new OpeningCinematic();}}
 void EnterLevel(){if(enteringLevel)return;enteringLevel=true;SceneFlow.LoadMainRoom();}
 void ReturnToMenu(){tutorials=false;instructions=false;}
 void OpenTutorial(int index){if(enteringLevel)return;enteringLevel=true;if(index==8)SceneFlow.LoadWorldOneGallery();else if(index==6||index==7)SceneFlow.LoadRewindPlaytest(index==7);else if(index==5)SceneFlow.LoadChapter();else SceneFlow.Load(index);}
 void Update(){var k=Keyboard.current;if(ending){endingCinematic.Sequence.Tick(Time.unscaledDeltaTime);if(k!=null){if(k.escapeKey.wasPressedThisFrame)endingCinematic.Sequence.Skip();else if(endingCinematic.Sequence.Complete&&k.enterKey.wasPressedThisFrame)StartJourney();}return;}if(enteringLevel)return;
  if(tutorials){if(k!=null&&k.escapeKey.wasPressedThisFrame)ReturnToMenu();return;}
  if(opening!=null){opening.Sequence.Tick(Time.unscaledDeltaTime);if(k!=null){if(k.escapeKey.wasPressedThisFrame)opening.Sequence.Skip();else if(k.enterKey.wasPressedThisFrame||k.spaceKey.wasPressedThisFrame)opening.Sequence.Next();}if(opening.Sequence.Complete)EnterLevel();return;}
  if(k==null)return;
  if(k.escapeKey.wasPressedThisFrame)instructions=false;
  else if(k.enterKey.wasPressedThisFrame&&!instructions)StartJourney();
 }
 void OnGUI(){
  GUI.matrix=Matrix4x4.identity;if(ending||opening!=null)GameUI.Panel(new Rect(0,0,Screen.width,Screen.height),new Color(.03f,.08f,.12f));if(!ending&&opening==null&&artwork!=null)GUI.DrawTexture(new Rect(0,0,Screen.width,Screen.height),artwork,ScaleMode.ScaleAndCrop);GameUI.Begin(font);
  if(ending){endingCinematic.Draw();if(endingCinematic.Sequence.Complete&&GameUI.Button(new Rect(60,353,300,51),"再玩一次",true,"Enter"))StartJourney();return;}
  if(opening!=null){DrawStory();return;}
  if(tutorials){DrawTutorials();return;}
  GameUI.Panel(new Rect(0,0,475,540),new Color(.025f,.10f,.12f,.16f));
  if(ending)GameUI.Text(new Rect(60,170,400,80),"恭喜通關！",48,GameUI.Cream,TextAnchor.MiddleLeft);
  if(!ending&&titleLogoMaterial!=null&&Event.current.type==EventType.Repaint){Graphics.DrawTexture(new Rect(38,10,340,340),titleLogo,titleLogoMaterial);}
  GUI.enabled=!instructions;
  if(GameUI.Button(new Rect(60,340,300,51),ending?"再玩一次":"開始旅程",true,"Enter"))StartJourney();
  if(GameUI.Button(new Rect(60,405,300,43),"玩法測試 / Debug Mode"))tutorials=true;
  if(!ending&&GameUI.Button(new Rect(60,462,300,43),"旅人手冊 / 操作說明"))instructions=true;
  GUI.enabled=true;
  if(instructions){GameUI.Shade();GameUI.Frame(new Rect(238,50,484,438));GameUI.Text(new Rect(270,78,420,40),"旅人手冊",30);GameUI.Divider(270,165,420);
   string[] keys={"A D","Space","F","E / Q","X","R / Esc"};string[] hints={"左右移動，也可使用方向鍵","跳躍；放開可短跳","懷舊濾鏡：切換現在與過去","定格／錄影；再次按下保存、播放或清除","操作機關、拍下風景","保留記憶重試／暫停"};for(int i=0;i<6;i++){GameUI.Key(new Rect(270,185+i*36,60,25),keys[i]);GameUI.Text(new Rect(350,183+i*36,330,29),hints[i],15,GameUI.Cream,TextAnchor.MiddleLeft);}if(GameUI.Button(new Rect(270,423,420,40),"繼續",true))instructions=false;
  }
 }
 void DrawTutorials(){
  GameUI.Shade();GameUI.Frame(new Rect(210,28,540,484));
  GameUI.Text(new Rect(245,48,470,42),"玩法測試",32,GameUI.Gold);
  GameUI.Text(new Rect(245,96,470,30),"選擇練習，熟悉山羊的記憶能力。",18);
  string[] labels={"01  定格記憶","02  踏板與門","03  錄影與回放","04  時序與崩塌平台","05  EchoFoothills / 回聲山麓","06  Basic Replay / 基礎回放","07  Upgraded Replay / 進階回放","08  World 1 Art / 森林與大瀑布"};
  for(int i=0;i<labels.Length;i++)if(GameUI.Button(new Rect(245,128+i*38,470,33),labels[i]))OpenTutorial(i+1);
  if(GameUI.Button(new Rect(245,450,470,40),"回到主選單",false,"Esc"))ReturnToMenu();
 }
 void DrawStory(){
  opening.Draw();
  if(GameUI.Button(new Rect(726,30,194,36),"略過劇情",false,"Esc"))EnterLevel();
 }

}
}
