# 關卡設計協作與第一次下載

儲存庫：https://github.com/cpcap1214/Goat-Game

## 第一次取得完整專案

1. 安裝 GitHub Desktop、Unity Hub，並透過 Hub 安裝 **Unity 6000.6.0f1**。僅編輯／試玩不需要 WebGL 模組；要建置 Web 才需額外安裝。
2. 只想試玩：在 GitHub Desktop 使用 File → Clone repository → URL，貼上 `https://github.com/cpcap1214/Goat-Game.git`。
3. 要提交關卡：先登入 GitHub，在上述儲存庫按 **Fork** 建立自己帳號下的副本，再以 GitHub Desktop Clone 自己的 Fork。公開存取不等於有原儲存庫的推送權限。
4. 在 Unity Hub 選 Add project from disk，選 Clone 下來、同時包含 `Assets`、`Packages`、`ProjectSettings` 的根目錄。不要建立空白專案再複製零散檔案。
5. 等待第一次套件下載與素材匯入；需要網路。開啟 `Assets/_OneMoreTime/Scenes/MainMenu.unity` 後按 Play 顯示開始畫面，按「開始旅程」才播放開場劇情。也可直接開啟 `Assets/_OneMoreTime/Scenes/Chapter01_EchoFoothills.unity` 試玩。

沒有原作者電腦上的任何檔案也能使用此流程。`Library`、`Temp` 等由 Unity 重建；歷史測試截圖與建置產物 `Artifacts` 不隨 Clone 提供。

## 每次交付關卡

1. 先儲存並關閉 Unity，再到 GitHub 的 Fork 頁面按 Sync fork 更新自己的 main，接著在 GitHub Desktop Fetch／Pull。若出現衝突，先和專案負責人確認，不要直接覆蓋。
2. 從最新 main 建立工作分支，例如 `level/chapter01-layout`。
3. 開啟 Unity，修改事先約定由你負責的場景。試玩後停止 Play，儲存 Scene 與 Project；Play 模式中的修改通常不會保留。
4. 在 GitHub Desktop 檢查 Changes，提交 Scene、相關資產與全部 `.meta`。不要提交 Library、Temp、Logs、UserSettings 或建置輸出。
5. 填寫修改說明，Commit，Publish branch／Push 到自己的 Fork。
6. 在 GitHub 建立 Pull Request：base 為 `cpcap1214/Goat-Game:main`，head 為自己 Fork 的工作分支。附上改了哪個區域、解法、試玩時間／卡點，交給專案負責人檢查合併。

若未來取得原儲存庫協作者權限，可直接在原儲存庫建立分支與 PR；仍避免兩人同時修改同一個場景／Prefab。

## 分工與目前的編輯限制

- 關卡設計者：謎題順序、節奏、美術擺設、地形設計提案與試玩。
- 專案負責人：角色與記憶功能、機關程式、生成器、整合與建置。
- 移動或重新命名 Assets 內資產請在 Unity Project 視窗操作，保留 `.meta` 與引用。
- 不升級 Unity／套件、不自行改共用角色 Prefab 或 ProjectSettings；需要修改時在 PR 說明並先協調。

**目前第一大關不是完整的拖拉式關卡編輯器。** 請先確認以下限制，再調整機關：

- `Assets/_OneMoreTime/Editor/ChapterOneBuilder.cs` 的 **Build Chapter One - Nostalgia** 會重建正式第一大關的 `Chapter One Managed` 根物件；手動編輯的內容可能被覆蓋。
- 地形初始配置、旗幟／互動／拍照座標在 `Assets/_OneMoreTime/Runtime/ChapterOne/ChapterLayout.cs`。
- 記憶踏板和控制台部分判定座標寫在 `Assets/_OneMoreTime/Runtime/ChapterOne/ChapterMemory.cs`；只移動畫面中的踏板不會同步改變判定位置。
- 若 Editor 已設定 `playModeStartScene` 為 MainMenu，需先清除該本機啟動覆寫，才能直接試玩目前的設計場景。
- 進行手動布局提案前，先在 Unity 中以 Save As 將第一大關另存為 `Assets/_OneMoreTime/Scenes/Design_Chapter01.unity`，讓 Unity 產生 `.meta`。直接開該場景按 Play 做局部檢查；主選單仍進入正式第一大關。
- 不執行生成／重建場景選單來「開啟遊戲」。設計副本不會自動更新主線；修改後由專案負責人同步生成配置與程式判定，再做整合驗證。

## 目前玩法

A/D 或方向鍵移動、Space 跳躍、F 切換年代、E 定格、Q 開始錄影／保存／播放／清除、X 互動／拍照、R 回旗幟、Esc 暫停、H 重看教學。

定格與錄影共用單一記憶；錄影最多保留最近 10 秒並保留等待。死亡／R 保留已保存記憶。關卡重點是記憶與年代機關，避免只增加重複跳台。第一大關 4–5 分鐘為首次遊玩設計目標，尚未完成新手驗收。

更多內容：[第一大關規格](Docs/Codex/CHAPTER_ONE.md)、[目前狀態](Docs/Codex/STATUS.md)、[測試紀錄](Docs/Codex/TEST_REPORT.md)、[素材來源](Docs/Codex/LICENSES.md)。
