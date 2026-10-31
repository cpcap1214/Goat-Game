# GTA · Goat Tower Again

Unity 2D 四關平台解謎遊戲，專案代號 One More Time。扮演小山羊，留下定格或行動軌跡，在下一輪與過去的自己合作過關。

## 開啟專案

1. 使用 Unity Hub 安裝 **Unity 6000.6.0f1**，加入此儲存庫的根目錄。
2. 等待 Unity 匯入素材與套件。
3. 開啟 `Assets/_OneMoreTime/Scenes/MainMenu.unity`，按 Play。

Web 建置另需該版本的 WebGL Build Support。

## 操作

| 按鍵 | 功能 |
| --- | --- |
| A / D、方向鍵 | 移動 |
| Space | 跳躍，短按可降低高度 |
| E | 保存定格並回起點；已有定格時清除並重新準備 |
| Q（第三、四關） | 依序開始錄製、保存並回起點、播放、清除並回起點 |
| R | 重試，保留已保存記憶；清除尚未保存的錄製 |
| Esc / P | 暫停 |

只有一份記憶，定格與軌跡互斥。軌跡保留最近最多 10 秒，包含等待時間；須手動開始錄製。

## 專案結構

- `Assets/_OneMoreTime/`：程式、場景、Prefab、設定、山羊美術與測試。
- `Assets/SunnyLand Artwork/`：環境美術、測試狐狸及音樂。
- `Packages/`、`ProjectSettings/`：Unity 依賴與專案設定。
- `Tools/`：生成、驗證、測試與 Web 打包工具。
- `Docs/Codex/`：架構、素材來源、測試紀錄與建置說明。
- `OneMoreTime_Codex_Plan/`：原始規格；後續修訂以 `AGENTS.md` 與狀態文件為準。

## 驗證與 Web 建置

在專案根目錄執行，需 Python 3 與上述 Unity 版本：

```sh
python3 Tools/omt.py validate
python3 Tools/omt.py test --mode edit
python3 Tools/omt.py test --mode play
python3 Tools/omt.py build-web
python3 Tools/omt.py package-web
```

非預設安裝位置可設定 `UNITY_EDITOR`。詳細流程見 [BUILD.md](Docs/Codex/BUILD.md)。建置輸出為 `Artifacts/Web/` 與 `Artifacts/OneMoreTime-Web.zip`；這些產物不納入 Git。

## 測試狀態

歷史驗證範圍與已知問題見 [STATUS.md](Docs/Codex/STATUS.md) 和 [TEST_REPORT.md](Docs/Codex/TEST_REPORT.md)。既有紀錄包含 PlayMode 的 `InputTests.HeldKeysAndResetSuppression` 失敗，不能視為所有測試皆通過。2026-10-31 的整理只驗證 Git 交付，沒有重新執行 Unity 編譯、遊玩或瀏覽器測試。歷史報告引用的 `Artifacts/` 證據保留在原工作機，不隨 clone 下載。

## 素材來源

Sunny Land：ansimuz；音樂：Pascal Belisle；Noto Sans TC 字型：SIL OFL。山羊及部分介面插畫為本專案生成素材。完整來源與授權紀錄見 [LICENSES.md](Docs/Codex/LICENSES.md)，字型授權原文隨素材保留。
