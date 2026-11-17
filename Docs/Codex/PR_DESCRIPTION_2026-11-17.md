# 主線房間、回放能力與 World 1 森林瀑布素材

這個 PR 將獨立主線 Room 1–3、保留的舊關卡入口，以及可試玩的新版回放能力整合到原始 repo。開場保留，基本與進階回放分開，方便後續安排能力取得順序。

## World 1 本次更新

- 新增四種動畫水面／瀑布 tiles、可重複使用 prefabs，以及雙軸循環森林背景與 parallax。
- 重畫六幀山羊溺水動畫，加入可選用的水域死亡與原地關卡重生流程。
- 新增會裂開、亮綠眼及綠色光暈的石化山羊檢查點。
- Debug Mode 新增 08 World 1 Art；↑/↓ 捲動、Space 溺水、C 雕像。
- 素材與使用說明：`Docs/Codex/WORLD_ONE_ART.md`。本次 update log：`Docs/Codex/UPDATE_LOG_2026-11-17.md`。
- WorldOneArtTests **3/3 PASS**；已在真實 Unity Editor 檢查動畫與背景接縫。素材尚未重新佈置到既有主線房間。

## 分支先前 Update log

- 新增主線房間、檢查點、單向平台、壓力板、限時／移動平台與升降機；開放房間上界但維持相機邊界。
- Debug Mode 保留舊四關與 EchoFoothills；新增 06 Basic Replay、07 Upgraded Replay。
- 長按 Q 0.5 秒錄影，最多 10 秒；保存後可視快速倒帶世界至錄影起點，物件使用 `affectedByRewind` 選擇是否還原。
- 回放殘影停留於末幀；途中按 Q 從開頭正向重播。進階版支援 Left Shift 正倒帶與按住暫停。
- 新增蓄力圓環、加厚快速膠卷及左側淡入、暖色殘影／軌跡／倒帶，以及底部進度條與時間指示點。
- EchoFoothills 的 F 切換加入鏡片動畫與細微懷舊色調、顆粒和暗角。
- 完整更新紀錄：`Docs/Codex/UPDATE_LOG_2026-11-16.md`。

## 先前驗證紀錄（本次未全部重跑）

- Unity 6000.6.0f1 編譯通過。
- 最新 RewindPlaytestTests：10/10 PASS。
- PastLensTransitionAndRespawn：1/1 PASS。
- 先前主線上界測試：2/2 PASS；Motor：20/20 PASS（歷史結果，未於本次全部重跑）。
- `git diff --check` PASS。

## 試玩及已知限制

從 Main Menu → Debug Mode 選擇 06 或 07；濾鏡測試使用 05 EchoFoothills，拾取鏡片後按 F。

新版回放仍為 Debug_Rewind 原型，尚未放入主線能力取得流程；後續道具可呼叫 `UnlockReplayUpgrade()`。不會自動還原任意 Rigidbody2D／腳本狀態。尚未完成最新 Web build 或整體人工通關。既有 Room 3／其他歷史測試問題詳見 `Docs/Codex/TEST_REPORT.md`，回放測試通過不代表所有舊問題已解決。
