namespace OneMoreTime {
public static class AbilityTutorial {
 public static string Name(MemoryKind kind)=>kind==MemoryKind.Freeze?"時間暫停 · 定格懷錶":"時間回溯 · 回溯沙漏";
 public static readonly string[] Freeze={
 "我的定格懷錶！原來跟我一起摔下來了\n我記得按一下 E 就可以留下一隻定格的自己",
 "使用方式應該是在想要留下定格的地方按 E\n然後我會回到起點，但剛才的自己會留在原地\n應該就能踩在上面",
 "已有定格時，再按 E 就會清除並回到起點\n阿如果想重來一次就按 R"};
 public static readonly string[] Trace={
 "找到回溯沙漏了！這次連我的軌跡都能錄下來，爽啦\n按 Q 開始錄製，移動、跳躍和等待都會記住",
 "再按 Q 保存軌跡，我會回到起點\n但是只保留最近 10 秒，有點鳥",
 "準備好後按 Q，讓過去的我重走一次剛剛的軌跡\n可以踩著牠移動，也能讓牠去壓踏板\n阿記得回放只會播一次，結束後殘影就會消失",
 "播放後再按 Q 就可以清除軌跡，回起點重新準備\nR 重試會保留軌跡，記得定格和軌跡只能留一種喔"};
 public static string[] Pages(MemoryKind kind)=>kind==MemoryKind.Freeze?Freeze:Trace;
}
}
