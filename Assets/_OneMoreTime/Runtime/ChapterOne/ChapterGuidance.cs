namespace OneMoreTime {
// Instruction cards are separate from gameplay time and never consume an ability key.
public sealed class ChapterGuidance {
 readonly bool[] seen=new bool[3];
 public int Room{get;private set;}=-1;public int Page{get;private set;}
 public bool IsOpen=>Room>=0;public bool HasNext=>Room==2&&Page==0;
 public void Check(float x){int room=x>=315?2:x>=192&&x<256?1:x>=64&&x<121?0:-1;if(room>=0&&!seen[room])Open(room);}
 public void OpenAt(float x)=>Open(x>=315?2:x>=192?1:0);
 void Open(int room){Room=room;Page=0;seen[room]=true;}
 public void Advance(){if(HasNext)Page++;else Close();}
 public void Close(){Room=-1;Page=0;}
 public string Title=>Room==0?"定格：讓另一個你留守":Room==1?"錄影：兩個你分工":Page==0?"接力 1 / 2：先錄一段路線":"接力 2 / 2：搭上自己的回放";
 public string Goal=>Room==0?"目標：留下殘影壓住踏板，本體穿過石門。":Room==1?"目標：回放依序踩 A、B，本體留在白色控制台。":Page==0?"目標：錄下「等候 → 跳過斷口 → 原地長跳」。":"目標：讓殘影載你移動，再從它背上起跳。";
 public string[] Steps=>Room==0?new[]{"按 F 切到「過去」，走上金色踏板。","站穩後按 E：保存定格，並自動回到旗幟。","回到現在後向右走。殘影會替你持續壓住踏板。"}:Room==1?new[]{"若左上顯示已保存定格，先按 E 清除。","走到 A，按 Q 開始錄影；停一下，再走到 B。","在 B 停一下，按 Q 保存。你會自動回到旗幟。","本體走上白色控制台，再按 Q 播放，站著等門開。"}:Page==0?new[]{"先清除舊錄影：Q 播放後再按 Q，再走到左岸標記。","按 Q 開始錄影，原地等約 2 秒，留時間給自己上車。","跳到中間小島，等約 2 秒，再原地長按空白鍵跳高。","落地後按 Q 保存；你會回到旗幟，錄影會留下來。"}:new[]{"走回左岸的錄影起點，按 Q 播放。","立刻原地跳上藍綠色殘影，放開方向鍵讓它載你。","殘影在中島跳高時，再按空白鍵，接著向右登上高崖。","失敗按 R：保留同一份錄影，再按 Q 重播。"};
 public string Reminder=>"E 再按一次＝清除定格　｜　Q 播放後再按＝清除錄影\nR＝保留已保存記憶重試　｜　X＝操作機關／拍照　｜　H＝重看教學";
}
}
