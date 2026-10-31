# 官方參考來源與適用範圍

查核日期：2026-09-18。以下來源只支持工具／素材／交付規格；四關玩法、數值、開發排序和驗收案例是本計畫提出的設計，不是來源作者替本遊戲驗證過的結論。

這份計畫沒有讀取使用者的 Unity 專案或實際下載素材；環境與素材現況需由階段 0 讀取工作區確認。

## [S1] OpenAI：Custom instructions with AGENTS.md

官方文件說明 Codex 如何讀取全域與專案的 AGENTS 指示，以及文件大小限制。計畫因此把簡短規則放在 AGENTS，把完整玩法放在需要明確讀取的文件。

`https://developers.openai.com/codex/guides/agents-md/`

查核時此入口轉向：

`https://learn.chatgpt.com/docs/agent-configuration/agents-md`

## [S2] ansimuz：SunnyLand - Pixel Game Art Assets Pack

作者頁列出素材內容、下載檔案和授權欄。查核時頁面標示 CC0，也存在不同下載與額外內容；不能據此假定所有使用者已取得的檔案、音樂或衍生素材有相同內容與授權。實作盤點以實際取得版本為準。

`https://ansimuz.itch.io/sunny-land-pixel-game-art`

## [S3] Unity：Rigidbody2D.MovePosition

支持「實際位置改變在後續物理更新」「官方建議 FixedUpdate」「適合 kinematic 的使用情境」等 API 細節。不保證移動平台能自動可靠承載玩家，承載仍須由本遊戲實作與測試。

`https://docs.unity3d.com/6000.0/Documentation/ScriptReference/Rigidbody2D.MovePosition.html`

## [S4] Unity：Editor command line arguments reference

支持用命令列啟動 Editor、batchmode、projectPath、executeMethod 和 log 等自動化方式。被 executeMethod 呼叫的方法需為 static 並位於 Editor 範圍。實際命令應依使用者專案的版本核對。

`https://docs.unity3d.com/6000.0/Documentation/Manual/EditorCommandLineArguments.html`

## [S5] Unity Test Framework：Running tests from the command line

支持 runTests、testPlatform、testResults 等測試命令列參數。參考頁為 1.1 系列文件，不是要求把使用者的套件版本改為 1.1；實作時使用實際安裝套件的對應文件。

`https://docs.unity3d.com/Packages/com.unity.test-framework@1.1/manual/reference-command-line.html`

## [S6] Unity：Sprite texture import settings

支持 Sprite、Multiple、Pixels Per Unit 和 Point filtering 等匯入設定的意義。素材切片與 PPU 值仍必須實際盤點，不由本頁決定 Sunny Land 的版本內容。

`https://docs.unity3d.com/6000.0/Documentation/Manual/texture-type-sprite.html`

## [S7] itch.io：Uploading HTML5 games

支持 HTML5／Web 遊戲的 ZIP 交付、index.html、完整資源／相對路徑、檔名大小寫和壓縮等發布注意事項。計畫只要求準備交付包，不授權 Codex 公開發布。

`https://itch.io/docs/creators/html5`

## [S8] Unity：Time.timeScale

文件說明 timeScale 為 0 對 FixedUpdate 和部分等待機制的影響。計畫把定格能力與真正的選單暫停分開，避免把整個世界停止當作留下殘影。

`https://docs.unity3d.com/6000.0/Documentation/ScriptReference/Time-timeScale.html`
