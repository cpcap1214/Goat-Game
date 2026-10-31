# One More Time｜交給 Codex 的完整實作計畫

**先用 Sunny Land 狐狸，做四關「與上一輪的自己合作」的平台解謎 demo。**

這是一套可交給 Codex 執行的規格、分階段提示詞和驗收文件；不是已完成的 Unity 遊戲，不包含素材或字型。尚未對你的專案執行編譯、場景生成或遊玩測試。

## 開始使用

把整個 `OneMoreTime_Codex_Plan` 資料夾放在你的 Unity 專案根目錄，也就是與 `Assets`、`Packages`、`ProjectSettings` 同一層，不要放進 Assets 當成遊戲資產。

```text
你的Unity專案/
  Assets/
  Packages/
  ProjectSettings/
  OneMoreTime_Codex_Plan/
    README.md
    MASTER_PLAN.md
    STAGE_PROMPTS.md
    AGENTS.additions.md
    TEST_MATRIX.md
    STATUS.template.md
    SOURCES.md
```

在這個工作區開啟 Codex，貼上下面的首次任務。從階段 0 開始，依 `STAGE_PROMPTS.md` 自動接續。連續執行全部階段，每階段完成後在 Unity 檢查並自動接續。

沒有既有 Unity 專案時，也可以把本資料夾放在預定的工作區，再讓階段 0 檢查實際環境。不要預設 Codex 雲端環境具備 Unity Editor／授權／Web 模組；缺少工具時，文件要求 Codex 說明確切阻塞，不假裝已經完成遊戲。

## 第一次直接貼這段

```text
請在目前工作區實作 One More Time 的 Unity 2D 四關平台解謎 demo。

先完整閱讀 OneMoreTime_Codex_Plan/MASTER_PLAN.md、STAGE_PROMPTS.md、TEST_MATRIX.md，以及目前適用的 AGENTS.md／AGENTS.override.md。

遊戲先使用 Sunny Land 狐狸暫代山羊。主要能力是定格，以及保存最近最多 10 秒的軌跡。只有一份記憶；準備輪保存後結束本輪，利用輪使用記憶但不能覆寫；不是放出分身後繼續走。

從 STAGE_PROMPTS.md 的「階段 0」開始連續執行：
檢查實際 Unity 專案、版本、套件、輸入系統、Sunny Land 素材和可用執行工具；建立 AUDIT、ASSET_INVENTORY、ARCHITECTURE 初稿與 STATUS。安全合併 AGENTS.additions.md，不覆寫既有指示或使用者修改。

不要只再寫一份計畫，請實際完成階段 0 能執行的工作。不要猜素材位置、Unity MCP 或 Editor 能力；非破壞性決策採規格預設並記錄。缺工具就列出確切阻塞，不聲稱已編譯或測試成功。

完成後回報實際修改、已確認環境、測試／檢查結果及下一階段的前置條件，更新 Docs/Codex/STATUS.md，完成驗證後自動接續階段 1。
```

## 文件內容

| 檔案 | 用途 |
|---|---|
| `MASTER_PLAN.md` | 完整玩法、狀態機、資料規格、角色與殘影承載、四關配置、技術架構與最終完成定義。 |
| `STAGE_PROMPTS.md` | 首次任務、階段 1–7 的完整提示詞，以及新對話接續和修錯提示詞。 |
| `AGENTS.additions.md` | 持續提醒 Codex 的短規則；由它安全合併，不直接覆寫你原本的 AGENTS.md。 |
| `TEST_MATRIX.md` | 環境、移動、定格、軌跡、平台、機關、四關、UI、Web 的具體驗收項目。 |
| `STATUS.template.md` | 讓 Codex 記錄實際進度、阻塞和下一步的範本，所有測試初始為 NOT RUN。 |
| `SOURCES.md` | 官方參考文件與來源適用範圍。 |

## 八個階段

| 階段 | 你應拿到的成果 |
|---|---|
| 0 環境盤點 | 真實專案與素材清單、可用執行方式、明確缺項。 |
| 1 跑跳與素材 | 狐狸可走跳的測試場景、已量測的跳高和跳遠。 |
| 2 定格＋第一關 | 空中定格、重生後踩上殘影、完成第一關。 |
| 3 軌跡＋第三關 | 錄製、回放、搭乘移動殘影與接力跳躍。 |
| 4 機關＋第二關 | 殘影同時壓踏板與墊高，可靠開門通關。 |
| 5 綜合＋第四關 | 玩家安排 A/B 停留時間，配合崩塌平台完成通行。 |
| 6 完整流程 | 主選單、正式四關順序、教學、HUD、美術、聲音與結尾。 |
| 7 回歸與 Web | 真實驗證、Web 建置、可交付 ZIP 和啟動／發布說明。 |

遊玩順序是 1→2→3→4；開發先做 1→3→2→4，是為了先驗證「站在移動殘影上再起跳」是否可靠，避免機關關卡做完才回頭大修核心。

## 不要跳過的檢查

Codex 需要交付能開啟的場景和自動配置入口，不只是 C# 檔案。你主要檢查：第一關留下的位置是否真的可踩、第三關是否能穩定搭乘與起跳、第二關是否能單人衝過門、第四關是否因調整上一輪等待而變得可通過。

`PASS` 必須有真正執行過的證據。`NOT RUN` 是尚未測試，`BLOCKED` 是環境有實際缺項。靜態看過程式不等於 Unity 編譯通過，Editor 通關也不等於 Web 版本通關。

若 Codex 換了新對話，使用 `STAGE_PROMPTS.md` 的「新對話恢復進度」；它會讀 STATUS 和實際專案，而不是重新從零建立。
