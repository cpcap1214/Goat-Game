using UnityEngine;using UnityEngine.InputSystem;
namespace OneMoreTime {
public sealed class MenuScreen:MonoBehaviour {
 public Font font;public Texture2D artwork;public bool ending;public Texture2D dialogueFrame,goatPortrait;bool instructions;OpeningCinematic opening;bool enteringLevel;bool tutorials;
 EndingCinematic endingCinematic;
 Texture2D titleLogo;Material titleLogoMaterial;
 readonly KeyBindingMenu keyBindingMenu=new KeyBindingMenu();
 void Awake(){if(ending){endingCinematic=new EndingCinematic();return;}titleLogo=Resources.Load<Texture2D>("Branding/GoatTowerAgain");var shader=Resources.Load<Shader>("Branding/LogoMask");if(titleLogo!=null&&shader!=null)titleLogoMaterial=new Material(shader);}
 void OnDestroy(){endingCinematic?.Dispose();opening?.Dispose();if(titleLogoMaterial!=null)Destroy(titleLogoMaterial);}
 void StartJourney(){if(ending)SceneFlow.Load(0);else{MainGameProgress.StartOver();opening?.Dispose();opening=new OpeningCinematic();}}
 void EnterLevel(){if(enteringLevel)return;enteringLevel=true;SceneFlow.LoadMainRoom();}
 void ReturnToMenu(){tutorials=false;instructions=false;keyBindingMenu.Close();}
 void OpenTutorial(int index){if(enteringLevel)return;MainGameProgress.BeginDebug();enteringLevel=true;if(index==14){Time.timeScale=1;UnityEngine.SceneManagement.SceneManager.LoadScene("Debug_ChapterTwoFlashlight");}else if(index>=9&&index<=13){Time.timeScale=1;UnityEngine.SceneManagement.SceneManager.LoadScene("Main_Room0"+(index-5));}else if(index==8)SceneFlow.LoadWorldOneGallery();else if(index==6||index==7)SceneFlow.LoadRewindPlaytest(index==7);else if(index==5)SceneFlow.LoadChapter();else SceneFlow.Load(index);}
 void Update(){var k=Keyboard.current;if(ending){endingCinematic.Sequence.Tick(Time.unscaledDeltaTime);if(k!=null){if(k.escapeKey.wasPressedThisFrame)endingCinematic.Sequence.Skip();else if(endingCinematic.Sequence.Complete&&k.enterKey.wasPressedThisFrame)StartJourney();}return;}if(enteringLevel)return;
  if(keyBindingMenu.IsOpen){keyBindingMenu.Tick();return;}
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
  if(keyBindingMenu.IsOpen){keyBindingMenu.Draw();return;}
  GameUI.Panel(new Rect(0,0,475,540),new Color(.025f,.10f,.12f,.16f));
  if(ending)GameUI.Text(new Rect(60,170,400,80),"恭喜通關！",48,GameUI.Cream,TextAnchor.MiddleLeft);
  if(!ending&&titleLogoMaterial!=null&&Event.current.type==EventType.Repaint){Graphics.DrawTexture(new Rect(38,10,340,340),titleLogo,titleLogoMaterial);}
  GUI.enabled=!instructions;
  if(GameUI.Button(new Rect(60,334,300,43),ending?"再玩一次":"開始旅程",true,"Enter"))StartJourney();
  if(GameUI.Button(new Rect(60,389,300,36),"玩法測試 / Debug Mode"))tutorials=true;
  if(!ending&&GameUI.Button(new Rect(60,435,300,36),"旅人手冊 / 操作說明"))instructions=true;
  if(!ending&&GameUI.Button(new Rect(60,481,300,36),"按鍵設定 / Key Bindings"))keyBindingMenu.Open();
  GUI.enabled=true;
  if(instructions){GameUI.Shade();GameUI.Frame(new Rect(160,32,640,476));GameUI.Text(new Rect(190,52,580,40),"旅人手冊 / Controls",28);GameUI.Divider(190,102,580);
   string[] keys={KeyBindings.Label(GameAction.MoveLeft)+" / "+KeyBindings.Label(GameAction.MoveRight),KeyBindings.Label(GameAction.Jump),KeyBindings.Label(GameAction.MoveDown)+" + "+KeyBindings.Label(GameAction.Jump),KeyBindings.Label(GameAction.Shutter),KeyBindings.Label(GameAction.RecordReplay),KeyBindings.Label(GameAction.Filter),KeyBindings.Label(GameAction.ReplayDirection),KeyBindings.Label(GameAction.Interact),KeyBindings.Label(GameAction.Flashlight),KeyBindings.Label(GameAction.Retry)+" / Esc"};
   string[] hints={"左右移動","跳躍；放開可短跳","向下穿過單向平台","定格；再次按下替換影像","長按 0.25 秒錄影；短按保存／回放／重播","懷舊濾鏡：切換現在與過去","進階回放：短按換方向，按住暫停","操作機關、拍下風景","第二章手電筒：暫時驅散黑暗與霧氣","保留記憶重試／暫停"};
   for(int i=0;i<keys.Length;i++){GameUI.Key(new Rect(190,116+i*31,148,25),keys[i]);GameUI.Text(new Rect(354,114+i*31,416,29),hints[i],15,GameUI.Cream,TextAnchor.MiddleLeft);}
   GameUI.Text(new Rect(190,426,580,23),"顯示目前按鍵；能力隨關卡解鎖，舊教學保留原流程。",13,GameUI.Muted);
   if(GameUI.Button(new Rect(190,460,580,32),"繼續 / Back",true))instructions=false;
  }
 }
 void DrawTutorials(){
  GameUI.Shade();GameUI.Frame(new Rect(110,28,740,484));
  GameUI.Text(new Rect(145,48,670,42),"玩法測試 / Debug",32,GameUI.Gold);
  GameUI.Text(new Rect(145,94,670,30),"舊教學、第一章房間與第二章能力測試",18);
  string[] labels={"01  定格記憶","02  踏板與門","03  錄影與回放","04  時序與崩塌平台","05  EchoFoothills / 回聲山麓","06  Basic Replay / 基礎回放","07  Upgraded Replay / 進階回放","08  World 1 Art / 森林與大瀑布","09  Room 4 / 河道升降","10  Room 5 / 錄影與水路","11  Room 6 / 斷橋渡河","12  Room 7 / 雙瀑機關","13  Room 8 / 大瀑布終章","14  Chapter 2 / 手電筒"};
  for(int i=0;i<labels.Length;i++)if(GameUI.Button(new Rect(145+(i/7)*345,132+(i%7)*42,325,36),labels[i]))OpenTutorial(i+1);
  if(GameUI.Button(new Rect(145,450,670,40),"回到主選單",false,"Esc"))ReturnToMenu();
 }
 void DrawStory(){
  opening.Draw();
  if(GameUI.Button(new Rect(726,30,194,36),"略過劇情",false,"Esc"))EnterLevel();
 }

}
}
