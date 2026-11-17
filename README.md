# GTA · Goat Tower Again

Unity 2D 平台解謎遊戲，專案代號 One More Time。主線 Room 1 已依新布局建立，從開場接入；現有「回聲山麓」與四個記憶解謎小關改由主選單「教學關卡 / Tutorials」進入。回聲山麓內容：扮演攝影山羊，使用懷舊濾鏡切換現在與過去，留下定格、錄影回放分工，搭乘自己的跳躍登山拍照。原四個記憶解謎小關保留作獨立測試。

## 多人協作與第一次下載

**第一次參與請先看 [關卡設計協作教學](CONTRIBUTING.md)。** 不需要預先持有任何檔案：Clone 即可取得完整原始專案。沒有推送權限的協作者使用 Fork → 工作分支 → Pull Request 交付修改。第一大關部分機關仍由程式座標與生成器管理，編輯限制也在教學中說明。

## 開啟專案

1. 使用 Unity Hub 安裝 **Unity 6000.6.0f1**，加入此儲存庫的根目錄。
2. 等待 Unity 匯入素材與套件。
3. 開啟 `Assets/_OneMoreTime/Scenes/MainMenu.unity`，按 Play 顯示開始畫面；按「開始旅程」後播放攝影大賽劇情，再進入 Room 1，山羊走入鏡頭後可操作。現有關卡請從「教學關卡 / Tutorials」選擇。

Web 建置另需該版本的 WebGL Build Support。

## 第一大關操作

A / D 或方向鍵移動、Space 跳躍、F 切換年代、E 定格、Q 錄影／保存／播放、X 操作機關／拍照、R 回到最近旗幟、Esc 暫停，H 重看目前機關教學。三個機關入口會自動顯示短教學，E/Q 鍵名隨狀態改變。開場有主持人對話框；Enter／Space 下一句、Esc 略過。

第一大關場景：`Assets/_OneMoreTime/Scenes/Chapter01_EchoFoothills.unity`。可直接開啟後按 Play；主選單「教學關卡 / Tutorials」第五項也會進入；「開始旅程」不再進入此關。目標首次遊玩約 4–5 分鐘，尚待新手試玩確認；原版使用者實測 2 分 7 秒；記憶解謎改版的已知解法自動路線約 101 秒，首次遊玩需重測。詳見 [第一大關設計與驗證](Docs/Codex/CHAPTER_ONE.md)。

## 舊四小關操作

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

## 主線 Room 1

場景：`Assets/_OneMoreTime/Scenes/Main_Room01.unity`。A/D 或左右方向鍵移動、Space 跳躍、下方向鍵＋Space 穿過草平台往下落、R 回檢查點、Esc 暫停。靠近山羊石像後眼睛變綠；落坑回到石像。出口走出鏡頭並淡出，接入 Room 2。Room 2 紫色相機解鎖 E 快門：立即在原位留下山羊，每次 E 取代舊影像；短暫停表與快門聲、無白閃光，支撐面不可見。Room 2 依一點距離一單位配置，9 單位坑的完整通關尚未驗證。完整標記慣例與編輯入口見 [ROOM_LAYOUTS.md](Docs/Codex/ROOM_LAYOUTS.md)。

## 主線 Room 3 與升降機

`Assets/_OneMoreTime/Scenes/Main_Room03.unity` 可直接開啟按 Play（已解鎖 E）。已按 IMG_8884.jpg 重建：留下定格山羊踩住下方踏板，啟動垂直往返平台；上方踏板控制 3 秒平台（踩住維持 3.0，離開倒數），包含兩個灰色山羊檢查點；頂部踏板開啟老升降機門，入內後向上升起並淡入下一房間的空白抵達區。Room 2 出口已接入 Room 3。主線共用教學的森林山景，Room 1 草平台改用草葉貼圖；素材與可重用 Prefab 詳見 `Docs/Codex/ROOM_LAYOUTS.md`。
