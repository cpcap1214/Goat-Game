## 2026-10-31 記憶解謎更新

現行第一大關已加入ChapterMemory：定格庭院、雙影水閘、搭上昨天三種解謎，E/Q記憶、F年代、X互動。場景入口與生成器不變；詳CHAPTER_ONE.md及最新TEST_REPORT.md，以下為舊版歷史。

# 2026-10-31 第一大關更新

Unity 6000.6.0f1。直接開啟 Assets/_OneMoreTime/Scenes/Chapter01_EchoFoothills.unity 後 Play，或由 MainMenu 開始旅程。Tools > One More Time > Build Chapter One - Nostalgia 可重建自己的 Chapter One Managed 根物件，重建前自動備份場景。WebBuilder 已納入新場景，但 Artifacts/Web 尚未重新建置。Unity plugin CLI/Pipeline 為目前驗證入口。

---

# 2026-09-22 回魔法塔結尾

正式Web與ZIP已更新；同版本Unity獨立副本建置，0 errors／1 warning。Artifacts/Logs/tower-ending-build.txt。結尾預覽位於Artifacts/EndingPreview（非正式交付ZIP）。素材及提示詞見ENDING_CINEMATIC.md。

---

# 2026-09-22 能力自動示範建置

Web和ZIP已更新。本次因原Editor出現Temp檔案遺失，使用 /tmp/goat-ability-demo-validation 的Assets／Packages／ProjectSettings／Tools副本，以同版本Unity 6000.6.0f1建置，再複製Web回原專案。建置Succeeded、0 errors、1 warning；Artifacts/Logs/ability-demo-build.txt。原Editor未強制關閉，暫存檔提示仍未解決。

---

# 2026-09-22 結尾精簡版

Web/ZIP 已更新；27271716 bytes，SHA-256 `145f654032dc9e04f539f7a966a4088065fc17ca36b0f0eeb6cda667734bc4f6`。Editor 結尾畫面與回主選單確認 PASS。

---

# 2026-09-22 主選單圖像 logo 版

Web/ZIP 已更新；27273409 bytes，SHA-256 `b0122589b9ead00209033eb54579f5e4b92efc8ba3bc1f18600d1da4e24ae0f1`。主選單與說明視窗瀏覽器確認 PASS。Logo PNG／mask shader 都位於 Resources/Branding，保證 Web 包含。

---

# 2026-09-22 能力道具與時間軸版

Web/ZIP 更新；26710529 bytes；SHA-256 `9a9ee1e6b5d30f603b81a9cfcddc5fd54cc597058822ffbca97286f471df9755`。58測試 PASS、Web 互動與教學視覺確認，完整範圍見 TEST_REPORT。執行 python3 Tools/omt.py build-web，再 python3 Tools/omt.py package-web。資產自 Resources/Relics 載入，專用 importer 與 Unity 自動管理 metadata，不需重生場景。

---

# 2026-09-22 移除序章標示版

Web/ZIP 已更新；26583201 bytes；SHA-256 `964e9f616e4cac67d983886fd229e0f97316cf1afaab93b27da47bf2275480b4`。開場瀏覽器視覺確認 PASS。

---

# 2026-09-22 GTA / Goat Tower Again

Web/ZIP 已更新；26583358 bytes；SHA-256 `7f42f53c591a57f2dde736748c788405d377cd173ffc08db488562ec6f5d7ef5`。本機網址 http://127.0.0.1:8765/Web/ 。

---

# 2026-09-22 三塊 HUD 移除版

Web/ZIP 已更新；26583378 bytes，SHA-256 `8865d351ae803511375020fe1c1fbde8d5f0fd8f1207d716d9f08035524f2e47`。L01 遊玩／暫停畫面驗證 PASS。

---

# 2026-09-22 暫停標題精簡版

Web/ZIP 已更新；26585912 bytes，SHA-256 `24c9e8f0b0eb34b7fa8cf7d37b8c95407d870beb35e38b8b71d2546f77cd81f3`。暫停画面瀏覽器驗證 PASS。

---

# 2026-09-21 通關文案精簡版

Web/ZIP 已更新；26586246 bytes，SHA-256 `a9d442e81bf119a27c95c56f5f16f18fffcf0676ea0feaf419b9851e7a468ad4`。瀏覽器 L01 通關畫面確認 PASS；四關共用同一視窗。

---

# 2026-09-21 操作說明與名稱位置更新

Web/ZIP 已更新，瀏覽器視覺確認 PASS。ZIP 26586329 bytes，SHA-256 `c780069e04da834dd41ef926e9b35965ca5a4ffee84d277b4487abf909caeea3`。CRC 與逐檔一致 PASS；證據 ui-labels-*。

---

# 2026-09-21 主選單精簡版

Web/ZIP 已更新並驗證；26586585 bytes，SHA-256 `84e408cb6efea04584a42d1aedd9bcf204f8b6d7644db7b5111bcaa0fc1a57f5`。四塊指定 UI 移除，瀏覽器確認 PASS。

---

# 2026-09-21 三段開場版

Web/ZIP 已更新；26586713 bytes，SHA-256 `6aa18224cd550dc6fb9d728b0993b8b811d14b0def17f06c66725550b61a216d`。三段原文、無頁碼，逐頁 Web 驗證及進 L01 PASS。證據 story-three-*。

---

# 2026-09-21 地形裁切修正版

Web/ZIP 已重建：26586976 bytes；SHA-256 `80e9fb78e26948fd08604fd5cbca3d40824fce1368c1a9ba89a948bdf5e87159`。`python3 Tools/omt.py generate` 現在會同步更新地形 Texture 與 Sprite 的裁切及網格，並保留原 Sprite GUID。`validate` 增加地形完整覆蓋檢查。後段地形缺失是舊 Sprite 範圍問題，非關卡碰撞缺失。

證據：Artifacts/Logs/terrain-crop-*。最新網址仍為 http://127.0.0.1:8765/Web/ 。

---

# 2026-09-21 四關挑戰版

本機試玩：http://127.0.0.1:8765/Web/ 。起始：Assets/_OneMoreTime/Scenes/MainMenu.unity。四場景名稱沿用 L01_Freeze、L02_Pressure、L03_Trace、L04_Timing。

本次完整 Web 四關→Ending 已實測；42 PlayMode + 8 EditMode PASS。ZIP：26587086 bytes，SHA-256 `9fdfa03191845604016825813178b18be8266f26bf2548ef5a87df26ea42ffdf`，詳 Artifacts/Logs/redesign-package-validation.json。

一般重新生成仍用 `python3 Tools/omt.py generate`；要明確恢复本次四關預設（會先備份 Config），用 `python3 Tools/omt.py challenge-layouts` 或 Unity Tools → One More Time → Apply Four Challenge Layouts。此命令不應用於保留 Inspector 個人調整的普通重建。

建置工具以 Build 全部檔案的組合 SHA-256 作 query version，因此僅改地形／場景資料也會更新快取版本。外框保持 16:9，窄或高視窗留邊；不改遊戲控制和物理。

測試輔助頁 Artifacts/QA/browser.html 已使用新版四關正常輸入；舊第三關特殊時間軸不再用於 Play L03。輔助頁不包含於下載 ZIP。沒有發布或 push。

---

# 2026-09-21 開場劇情版

Web 與 ZIP 已更新；封包驗證 PASS，26580024 bytes，SHA-256 `fa4842bfc9ec5c3d28cd0203d1153ad6d4adfc375730d567e2a36ea5e9cba7e9`。起始 MainMenu → 五段開場 → L01；Enter／Space 推進，Esc 略過。來源 SVG 與生成肖像位於 Art/UI。命令：`python3 Tools/omt.py build-web`、`python3 Tools/omt.py package-web`。證據 opening-web-build.txt、opening-package-validation.json。

---

# 開啟、重建與交付

## Unity

沿用 Unity 6000.6.0f1 (f7f8ed4d1e24)。專案根目錄 /Users/cpcap/Goat Game。
起始場景 Assets/_OneMoreTime/Scenes/MainMenu.unity；四個 L0x 場景也可獨立 Play。

本機 Editor：/Applications/Unity/Hub/Editor/6000.6.0f1/Unity.app/Contents/MacOS/Unity。
Web 模組：/Applications/Unity/Hub/Editor/6000.6.0f1/PlaybackEngines/WebGLSupport。

## 命令

在專案根目錄執行：

```sh
python3 Tools/omt.py audit
python3 Tools/omt.py generate
python3 Tools/omt.py validate
python3 Tools/omt.py test --mode edit
python3 Tools/omt.py test --mode play
python3 Tools/omt.py build-web
python3 Tools/omt.py package-web
```

可用 UNITY_EDITOR 指向另一台機器上的同版本 Editor。Editor 已開啟時使用本專案 EditorBridge，腳本尚未編譯時等待 SHA256 版本一致；若 Unity Auto Refresh 未觸發，在 Assets → Refresh 重新整理。遇到編譯錯誤會逾時回傳非零，不假裝新版本已執行。不要同時送兩個命令。

未開啟時使用 -batchmode。測試使用 TestRunnerApi／-runTests 產生 Artifacts/Logs/*mode.xml，必須確認實際測試數大於 0、failed=0；命令派送成功不代表測試通過。

Unity 選單 Tools → One More Time 提供 Build or Update Demo、Validate Demo、Audit Assets、Open Playground、Open Animation Gallery、Build Web Demo。
生成範圍限 Assets/_OneMoreTime，關卡 OMT Managed root。重新生成前會將既有場景備份到 Artifacts/SceneBackups；已初始化的 LevelConfig 不重設手動調整的幾何。PlayerTuning 數值集中於 Config/PlayerTuning.asset。原有素材、SampleScene、套件版本與既有未提交差異保留。Unity Web 建置另會序列化 URP shader prefilter/runtime settings 及 PlayerSettings preloaded assets；新增 OMTWorld 圖層使用原先空白的第 8 層。

## Web

使用六個正式場景的明確陣列建置，排除 Dev_Playground 和測試 assemblies，不更換全專案 build scene 清單。輸出 Artifacts/Web；ZIP 根目錄為 index.html 與 Build/ 四個必要檔案。自訂滿版外框 Tools/web-shell.html 不依賴 TemplateData；打包排除該未使用目錄及所有 QA／測試。

```sh
python3 -m http.server 8765 --bind 127.0.0.1 --directory Artifacts/Web
```

瀏覽器開啟 http://127.0.0.1:8765/ 。不要直接雙擊 index.html。建置使用未壓縮檔，無需 Content-Encoding；首次建置 IL2CPP/wasm 較慢。完成狀態以 Artifacts/Logs/web-build.txt 與 TEST_REPORT.md 為準。

使用者可將 Artifacts/OneMoreTime-Web.zip 上傳 itch.io 的 HTML5 專案，選擇在瀏覽器執行，建議嵌入尺寸 960×540、允許全螢幕。此任務不登入、不發布、不推送 Git。

## 操作

A/D 或左右鍵移動；Space 跳躍，放開可短跳。E 保存定格並回起點；已有定格時再按 E 清除並回起點重新準備。第三關起 Q：開始錄製 → 保存並回起點 → 播放 → 清除並回起點。進關／重新準備都不自動錄製，需手動 Q；保留最近最多10秒及等待。R 保留已存記憶重試（尚未保存則清空錄製）。Esc/P 暫停、音量及回選單；不再提供重錄按鈕或確認視窗。過關後 Enter 下一關。

音樂 Pascal Belisle，Sunny Land 美術 ansimuz，字型 Noto Sans TC（SIL OFL）。來源與原始授權檔位置見 LICENSES.md。

## 本次產物

2026-09-21 手動E/Q版建置成功（耗時見web-build.txt）、0 errors、1 warning，Artifacts/Logs/web-build.txt。ZIP 25,798,809 bytes，SHA256 `b23bf621605fbb1efe93ed670bcca830c6148672b2386fa1205680083fbb83e0`。解壓、CRC、四個 index 引用、WASM magic 與逐檔雜湊比對通過；見 package-validation.json。

本次已啟動的預覽伺服器根目錄為 Artifacts，因此目前遊戲 URL 是 http://127.0.0.1:8765/Web/ 。若該伺服器仍在執行，不要重複佔用 8765；上方命令供重新啟動時使用。

純按鍵瀏覽器驗證頁：以 Artifacts 為 HTTP 根目錄時，開啟 /QA/browser.html。按 Enter 開始，依序使用各關 Play inputs，確認可見過關畫面後再 Enter。QA 只輸入方向、跳躍、E/Q/R/P，不注入遊戲狀態；L03 使用人工按鍵時間軸錄製，若第二跳太早可按 Retry L03 inputs 使用已通過的保留記憶重試時間軸；其他關依 solution-L0x.json。這些輔助檔不會進入 ZIP。

建置外框依WASM SHA256加入資源版本query，避免更新後混用瀏覽器中的舊framework／data與新WASM。
