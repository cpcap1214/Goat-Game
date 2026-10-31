using UnityEngine;
namespace OneMoreTime {
// Developer-only scene: uses the same sprite libraries and PlayerVisual as the game.
public sealed class AnimationGallery:MonoBehaviour {
 public Font font;public PlayerVisual[] actors;float clock;
 void Update(){clock+=Time.deltaTime;for(int i=0;i<actors.Length;i++){int state=i==5?1:i;int length=state==0?6:state==1?AnimationLibrary.RunCycleFrames:state==2||state==3?3:6;int frame=(int)(clock*(state==0?6:state==1?AnimationLibrary.RunFramesPerSecond:10))%length;actors[i].Pose(new Vector2(-7.5f+i*3,0),1,state,frame,Color.white);actors[i].renderer2D.transform.localScale*=2.2f;}}
 void OnGUI(){GameUI.Begin(font);GameUI.Text(new Rect(30,35,900,50),"山羊 / 動畫檢視",32);GameUI.Text(new Rect(32,90,900,35),"正式角色  ·  跑步 24 幀  ·  共用腳底基準  /  狐狸保留作測試",16,GameUI.Muted);string[] names={"待機","奔跑","起跳","下落","落地","狐狸測試"};for(int i=0;i<6;i++){GameUI.Frame(new Rect(18+i*155,391,144,53));GameUI.Text(new Rect(20+i*155,399,140,37),names[i],18,GameUI.Cream,TextAnchor.MiddleCenter);}GameUI.Text(new Rect(32,490,900,25),"實際 SpriteRenderer / AnimationLibrary 顯示；不改變角色碰撞與能力。",13,GameUI.Muted);}
}
}
