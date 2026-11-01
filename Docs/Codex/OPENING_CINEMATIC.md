# Unity 戶外攝影大賽前導劇情（2026-10-31）

正式入口：Assets/_OneMoreTime/Scenes/MainMenu.unity → Play → 開始旅程。使用既有 MenuScreen 與 Unity IMGUI 呈現，並非 HTML 播放。27 秒自動劇情，Esc／略過按鈕跳至 L01；Enter／Space／下一句推進對話。

戶外草地、樹木、浮雲、彩旗、木舞台；主持人持麥克風介紹攝影大賽及 GTA7 獎品，九隻山羊觀眾帶相機。獎品使用使用者第二張附件，旋轉展示正反面；主角出發登山，相機魔力和殘影銜接第一關。下方對話框包含角色肖像、姓名牌、逐字文字與下一句按鈕；發言者依序是主持人、觀眾們、小山羊。目前為文字對話，沒有新增配音。

| 時間 | 對話 |
| --- | --- |
| 0–3.5 | 主持人：各位攝影高手，歡迎來到山羊世界攝影大賽！ |
| 3.5–7 | 主持人：今年的挑戰：拍下世界上最美的風景！ |
| 7–10 | 主持人：冠軍將獲得……大家期待的《GTA7》！ |
| 10–13 | 觀眾們：咩——！GTA7！我要把它帶回家！ |
| 13–16.5 | 小山羊：最美的風景，一定藏在那座傳說中的山頂。 |
| 16.5–20 | 小山羊：就算是最難攀登的山，我也要拍給大家看！ |
| 20–23.5 | 小山羊：咦？這座山有魔力……我的相機在發光！ |
| 23.5–27 | 小山羊：照片竟然變成真的了？帶著這份力量，出發吧！ |

實作：Runtime/OpeningCinematic.cs、Runtime/MenuScreen.cs、Tests/EditMode/OpeningTests.cs。Unity 已正常匯入素材及新腳本並產生真實 metadata。GTA7Prize 使用 Point、無 mipmap、無壓縮匯入。

依使用者指示使用本機 Unity plugin 0.1.8-beta 的 unity-cli 技能，CLI 更新至 1.0.0-beta.15；以 Pipeline 連線現有 6000.6.0f1 Editor，不升級 Editor。透過 unity command 開場景、編譯、run_tests、eval、wait_for、capture_game_view 操作及驗證。舊的無授權阻礙已因使用者登入 Hub 解決。

PASS：Unity recompile_status completed/failed=false；28/28 EditMode（含4項 OpeningTests）；DemoBuilder.Validate 六場景／字型驗證；Play Mode frameCount 在 wait_for 中確實前進；自然播放後觀察到 L01_Freeze 與 RunManager；Unity Game view 的戶外舞台、主角觀眾與主持人對話框目視確認。

證據：Artifacts/Logs/opening-plugin-tests.json；Artifacts/OpeningCaptures/Logs/opening-outdoor-final.png。最後截圖透過設定劇情第2秒並暫停取得，是定點畫面驗收，非輸入測試。先前自然播放流程另驗證進第一關。

已知：CLI status 曾向舊 Pipeline 發出不存在的 editor status，Console 留下一筆工具錯誤；一次測試 eval 在場景已切換後找不到 MenuScreen，屬驗證呼叫錯誤。未宣稱 Console 無任何錯誤。

NOT RUN：本輪完整四關 PlayMode 回歸、Web 重新建置、鍵盤與按鈕逐項實際操作。略過及下一句狀態邏輯已由 EditMode 驗證。舊 Artifacts/Web、ZIP、HTML 預覽皆非本次戶外版。未發布或推送。既有結尾與能力道具保留，本次只改前導。

交付時 Unity 停在主持人第一句、Play Mode 暫停；按編輯器上方暫停鍵可繼續。原 Application.runInBackground=false 已恢復。
