# Stage 0 稽核（2026-09-19）

- 專案：/Users/cpcap/Goat Game。Unity 6000.6.0f1 (f7f8ed4d1e24)。現有 Editor 開啟 SampleScene，GUI 已實際讀取。
- Editor：/Applications/Unity/Hub/Editor/6000.6.0f1/Unity.app/Contents/MacOS/Unity。
- Web 模組：/Applications/Unity/Hub/Editor/6000.6.0f1/PlaybackEngines/WebGLSupport 存在。授權 entitlement 在 Logs/Editor.log 正常；最終真實 Web 建置成功，見 web-build.txt。
- URP 17.6.0、Input System 1.20.0（activeInputHandler=1）、Test Framework 1.8.0、uGUI 2.6.0；沿用，不升級。
- 沒有 Unity MCP。使用 Assets/_OneMoreTime/Editor/EditorBridge.cs，Editor 僅接受專案 Artifacts/command.txt 的有限命令，無網路 listener、無全域設定。不啟動第二個 Editor。
- 原始未提交變更：刪除 Assets/Editor/HubForceResolve.cs 與 meta，Packages/packages-lock.json、learn Settings.json 已修改；SunnyLand Artwork 與規格資料夾未追蹤。全部保留。
- 原無適用 AGENTS、Docs/Codex 或本遊戲實作。已安全新增專案 AGENTS，更新四份規格內舊停止規則。
- GUI：cua Unity app 可用；最終 Chromium 153 HTTP 四關至結尾已通過，見 TEST_REPORT.md；環境偵測本身不作為建置通過依據。
