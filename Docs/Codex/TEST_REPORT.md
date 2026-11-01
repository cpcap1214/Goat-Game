# 2026-11-01 修正啟動需求：先顯示開始畫面

使用者澄清要先顯示開始畫面，不要自動進攝影大賽。已撤回 MenuScreen.Awake 的自動 StartJourney，恢復按「開始旅程」／Enter 才播劇情；本機 playModeStartScene 仍指向 MainMenu，避免從第一大關直接啟動。下方上一輪自動播放紀錄已被本修訂取代。

PASS：Unity CLI recompile 完成、0編譯錯誤；真實 Editor 進 Play 後 MainMenu 存在、ending=false、opening=null，確認等待開始操作。Unity 留在開始畫面供使用者操作。NOT RUN：本輪未重跑完整劇情／關卡回歸或 Web 建置；未推送。

---

# 2026-11-01 啟動直接播放劇情

原因：Editor 目前開啟 Chapter01_EchoFoothills 且 playModeStartScene 為空，按 Play 直接進關卡；MainMenu 原先還需按開始旅程才播劇情。

修改：MenuScreen.Awake 在非結尾入口自動 StartJourney，直接播攝影大賽，結束後沿用 LoadChapter。透過 Unity CLI／Pipeline 將本機 Editor 的 playModeStartScene 設為 MainMenu 並開啟該場景；此覆寫是本機 Editor 設定，不是新機 Clone 的預設，其他協作者開 MainMenu 按 Play 即可。直接試玩關卡時需清除本機覆寫。未改角色／記憶／關卡數值。

PASS：`unity recompile --format json` 0 errors、0 compilation warnings；真實 Editor Play 後 wait_for 確認 frameCount 前進，讀取 opening 非空且自動播放到 5.57 秒，攝影大賽截圖已目視（Artifacts/Logs/start-opening.png）。不輸入略過／下一句，自動播放後 wait_for 找到 NostalgiaChapter，active scene 確認 Chapter01_EchoFoothills；證據 Artifacts/Logs/opening-auto-transition.json。測完停止 Play，回到 MainMenu 待使用者按 Play，runInBackground 還原 false。

工具限制：CLI 回報現有 Pipeline 使用舊指令名稱；重編譯時出現工具相容性 console 記錄，本次不升級套件。不將其當成遊戲例外或宣稱 Console 全空。NOT RUN：全玩法回歸、Web 重建、瀏覽器測試；未 commit／push 本輪改動。

---

# 2026-11-01 GitHub 公開協作交接

使用者授權同步目前專案到 GitHub，並改為 public，不需邀請協作者。新增根目錄 CONTRIBUTING.md 與 README 入口，提供 Unity 6000.6.0f1、從零 Clone／Fork、關卡分工與 PR 流程。保留既有玩法、場景及全部未提交的第一大關成果；不在本輪改寫關卡編輯器。

PASS（本機交付檢查）：`git ls-files -co --exclude-standard` 檔案清單與 Python 檢查顯示資產 .meta 完整、無被追蹤的快取／建置輸出、無本機路徑套件依賴；最大檔案約11.4 MiB。以 `git rev-list --objects --all`／`git cat-file --batch` 掃描既有1599個歷史blob及目前檔案，常見私鑰與token樣式未命中；此為有限樣式檢查，不宣稱全面安全稽核。Unity 原有 Force Text 與 Visible Meta Files 設定已確認。

NOT RUN：本輪未重新執行 Unity 編譯、PlayMode 或新手試玩；歷史測試結果不代表全新電腦驗證。關卡生成器重建會覆蓋 managed 內容，且機關含硬編碼座標，已明列於交接教學。下一步由協作者首次匯入／試玩並在 Fork 提交設計；GitHub 遠端與全新 Clone 檢查結果見下。

PASS（遠端交付）：`a316ef9` 已推送至 `origin/main`；`gh repo edit --visibility public` 後遠端為 PUBLIC。停用 Git 全域設定與 credential helper 的全新 `git clone --depth 1` 成功，1584個追蹤檔案 SHA-256 與工作區逐檔一致；無憑證 `curl` 查詢 GitHub API 回傳 `private: false`。Python urllib 的 API 檢查曾因本機 CA 憑證失敗，改以保留 TLS 驗證的 curl 完成，未停用憑證驗證。全新 Clone 未開啟 Unity，不代表已通過新機匯入／編譯。

---

# 2026-10-31 踏板出口鏡頭與晴空背景

使用者回報第一個踏板拉遠後，通過障礙會看不到山羊，且背景與主角顏色接近。

已修正：NostalgiaChapter 的定格庭院／錄影機關鏡頭，以連續權重進出廣角構圖；離開機關時逐漸回到跟隨，並依相機實際 aspect 限制角色在畫面中央安全區。原先第一區固定 x=92 直到玩家 x=121，導致出口前出框。現在不再固定到出口。

已生成並匯入 PresentClearSky.png、PastClearSky.png，現在／過去皆採明亮藍天，移除原灰暗背景染色；兩張原始 Valley 素材保留。第一大關場景以 Unity plugin 重新生成。

PASS：Unity 編譯；PlayMode 63/63（Artifacts/Logs/chapter-camera-sky-tests.json），含正常完整通關與新增兩機關區出口逐段視野／鏡頭連續性測試。測試範圍 x69–131、193–255，每0.25單位檢查腳底在安全區及相鄰鏡頭變化。已檢視現在出口 x116 與過去踏板 x80 的 Game view；圖片在 Artifacts/ChapterCaptures/CameraSkyRevision/UnityCaptureOutput/CameraSky/。

仍待整體新版實際試玩時間，未宣稱4–5分鐘已驗收。沒有推送或發布。Unity 重新開在第一區教學預覽，按 Enter 可繼續。

---

# 2026-10-31 清楚教學與連續岩土（完成實作）

依使用者「操作或提示不清楚」與「下方方塊要連在一起」回饋，加入三段首次進入教學、H重看、依狀態顯示下一步與E/Q動態鍵名。保存／回起點／清除前一份記憶均明確說明；末段錄製和搭乘拆兩頁。教學時固定模擬暫停、關閉清除殘留按鍵，進入同區不重複跳出。

新大關非橋地形與碰撞向下連到y=-22；土層使用世界座標對齊的連續岩層與草緣，取代一格格土塊重複圖。保留解謎必需的橋與斷口。正式資產Art/ChapterOne/Terrain、生成器ChapterTerrainArt；舊四關不變。

PASS：Unity plugin編譯／場景生成，PlayMode62/62，含完整新地形通關及教學暫停／重開；定格教學、接力第二頁與連續地面截圖目視。證據Artifacts/Logs/chapter-guidance-terrain-tests.json。最後岩土色調與提示位置為純視覺微調，另檢查編譯與畫面。

仍待新版首次遊玩時長與理解度回饋，不將程式測試等同趣味或4–5分鐘驗收。未推送或發布。

最終畫面：Artifacts/ChapterCaptures/TerrainRevision/UnityCaptureOutput/Logs/terrain-connected-final.png。Unity 留在定格庭院教學預覽，AutoTick=true；Enter 關閉教學後可操作，H 重看。已恢復 runInBackground=false。

---

# 2026-10-31 記憶機關邊界驗證

PASS：Unity plugin編譯；PlayMode61/61，包含新版完整錄製／回放／接力路線、B→A反向訊號拒絕、中途離開本體控制台中斷序列，以及12種跳躍時點的定格無法代替末段移動回放。修正過去踏板必須由站立在踏板上的殘影觸發，不把本體站在踏板且遠處有任意殘影誤算為完成。證據 Artifacts/Logs/chapter-memory-boundaries.json。

新版首次通關時長及解謎體驗待使用者回饋；原版2分7秒已確認不達時間與趣味要求，不能用舊結果驗收新版。沒有為湊時間降低走速或強制等待。此輪僅規則判定與測試改動，場景幾何不變。

---

# 2026-10-31 記憶解謎改版（新主線）

使用者回饋原版2分7秒且過多跳台無聊。已將樹冠、河道、峰頂三段重複跳台替換為：過去年代定格壓門、錄影A→B搭配本體控制台、搭乘錄下的兩段跳躍登高。E定格、Q錄影／保存／播放／清除、F年代、X互動拍照。沿用單一記憶、最近10秒、等待保留、R保留、錄影死亡選擇；新ChapterMemory重用原Recorder／MemorySnapshot／EchoSurface與唯一PlayerMotor2D承載。

PASS：Unity plugin編譯／場景生成；PlayMode58/58，含整關正常馬達路線（真正錄製、回放、乘坐，無瞬移）、單一定格無法解雙訊號、過去踏板來源、防單跳高崖、10秒與死亡保存。證據 Artifacts/Logs/chapter-memory-tests.json。定格庭院與雙影水閘實際Unity截圖已目視（Artifacts/ChapterCaptures/MemoryRevision/UnityCaptureOutput/Logs）；最後鏡頭／背景填滿與文字微調另經編譯與目視，未再重跑58項玩法測試；局部截圖定位不算手動通關。鏡頭在機關室拉遠，讓本體與殘影能同時看見。

時長：新熟練解法模擬101.0351秒，不宣稱已達首次4–5分鐘；本次先修正缺乏解謎的核心問題，原版2分7秒不再代表新版。原四關保留；未更新Web／未推送。下一步為記憶版遊玩節奏與理解度回饋。

---

# 2026-10-31 實際輸入路徑補驗

PASS：`KeyboardTutorialSimultaneousJumpFilterAndPause` 透過 Unity Input System KeyboardState，呼叫正式 Update／FixedUpdate，驗證教學 Enter、D拾取、D+Space+F同時起跳切換、長按F僅一次、暫停期間位置與時鐘不變、R回旗幟與E啟動過去機關。整體54/54 PlayMode通過，證據 Artifacts/Logs/chapter-input-playmode.json。

原生視窗工具僅支援單一非修飾鍵；D+Space組合會回 keyPressIncludedMultipleNonModifierKeys，並行單鍵呼叫實際序列化。原生試走到古橋後，在第一段跳躍死亡並回到旗幟，因此不能宣稱原生鍵盤完整通關或使用該次時間證明4–5分鐘。Input System測試補足同時按鍵功能，但不代替新手時長驗證。

---

# 2026-10-31 第一大關：回聲山麓（可玩，時長待試玩）

現行主線改為戶外大賽開場→第一大關。實作懷舊濾鏡 F、現在／過去兩套實際碰撞地形與 AI 背景、六區連續平台跳躍、兩機關、三照片及11重生旗幟。舊四小關保留。水車轉動、鐘樓拉繩、取景點出口保護及另一年代平台輪廓已加入。

PASS：Unity plugin 編譯／場景生成；PlayMode 54/54（含完整正常馬達路線）；原生鍵盤拾取、年代切換、跳躍、重生與暫停；各區Unity畫面檢查。完整馬達路線103.9546秒、26切換、0死亡。詳細證據見 CHAPTER_ONE.md。

工具紀錄：最新 Console 仍含 Unity Temp/FSTimeGet 檔案時間斷言及 Pipeline 的 editor status 不存在錯誤；編譯結果無 C# 錯誤，不能宣稱 Console 全空。完整摘錄 Artifacts/Logs/chapter-console.json。最新輸入層與玩法回歸54/54通過，涵蓋最後的輪廓調整。

NOT RUN：新手4–5分鐘時長驗收、整關真實鍵盤通關、新Web建置。已請使用者試玩回報節奏。未推送此輪變更。下一步為首次遊玩節奏調整。

---

# 2026-10-31 Unity 戶外大賽與角色對話（最新）

改為戶外舞台與主持人、觀眾、主角的8段對話框，保留27秒自動播放、Enter/Space下一句及Esc略過。使用 Unity plugin 的 CLI/Pipeline 連線編輯器驗證；使用者登入後授權阻礙已解決，新素材及腳本 metadata 由 Unity 產生。

PASS：Unity 編譯、28/28 EditMode、六場景與字型驗證、實際 Play Mode 畫格前進、自動播放後進 L01、戶外主持人畫面目視檢查。證據 Artifacts/Logs/opening-plugin-tests.json、Artifacts/OpeningCaptures/Logs/opening-outdoor-final.png。NOT RUN：完整四關回歸、Web新版建置、逐鍵互動。詳細範圍及工具錯誤見 OPENING_CINEMATIC.md。Unity 留在第一句暫停以供預覽。

---

# 2026-10-31 攝影大賽前導劇情

已實作 27 秒開場：山羊攝影大賽、旋轉 GTA7 獎品、登山、魔法相機；替換舊三頁對話，保留略過並自動進第一關。附可攜式 HTML 畫面預覽。詳 OPENING_CINEMATIC.md。

PASS：預覽 JavaScript 語法、13 個分鏡時間點 mock canvas 檢查、diff 空白檢查。BLOCKED：Unity 無有效授權（exit 198），編譯／測試／建置均未完成；自動瀏覽器政策拒絕 file://，未進行畫面驗收。NOT RUN：真實播放／略過／進關驗證。新 metadata 待 Unity 正常匯入產生；既有 Web／ZIP 未更新。

---

# 2026-10-31 Git 儲存庫整理

依使用者本次授權整理、提交並推送既有遊戲專案至 origin/main；不發布遊戲網站。加入 README、忽略本機快取與 Artifacts，保留既有素材 metadata、套件設定及既有刪除。未重寫歷史。

PASS：Git 納入檔案檢查、Assets 檔案對應 metadata 檢查、常見秘密金鑰格式掃描（未發現）、單檔大小檢查（無 >=50 MiB）。原生 Unity YAML 的空值行保留，由 .gitattributes 設定空白檢查；僅移除 AbilityDemo.cs 一行尾端空白。

NOT RUN：此次沒有重新執行 Unity 編譯、EditMode、PlayMode、Web 建置或瀏覽器驗證。以下皆為歷史結果，包含既有 InputTests.HeldKeysAndResetSuppression 失敗紀錄。Artifacts 中測試證據與建置包仍保留本機，但不納入 Git。

---

# 2026-09-22 itch.io 上傳包與封面（完成）

交付 Artifacts/itch-release/Goat-Tower-Again-Web.zip、Goat-Tower-Again-Cover-630x500.png、itch-upload-guide.md。使用現有最新Web打包，未改玩法。ZIP 6檔、根目錄index.html、CRC及與Web逐檔一致驗證PASS，約30MiB；未解壓超過itch HTML5限制。封面由內建image_gen依主畫面截圖移除兩按鈕、調整比例，保留生成原圖並以sips輸出630×500，已目視確認。生成提示保存在cover-generation.txt。

NOT RUN：itch.io實站上傳與嵌入測試。本次未上傳或發布；提供欄位填寫、AI揭露與Draft預覽指引。

---

# 2026-09-22 尖刺下方補土地方塊（完成）

SpikeArtBuilder.Create 為前三關共7處坑底尖刺增加1單位高、與尖刺列同寬的土地方塊，頂面貼齊尖刺底部。使用既有 EarthFillSeamless。L04已有地形支撐，未填塞下層通道。基座為視覺地形，不改尖刺傷害區或碰撞。產生器重跑保留此效果；場景备份 Artifacts/SceneBackups/before-spike-foundation。

PASS：正式 Web build 0 errors / 1 warning，ZIP CRC／逐檔一致；四關場景檢查基座數量2/3/2/0。瀏覽器正式Web第一關兩處坑底尖刺與土塊接合顯示確認；截圖Artifacts/spike-foundations.png，Logs/spike-foundation-build.txt。NOT RUN：本輪EditMode/PlayMode及四關完整通關（僅新增視覺基座）。

---

# 2026-09-22 底部操作列不再遮擋尖刺（完成）

RunManager 相機使用 GameUI.GameplayViewport，底部37 UI單位專供不透明操作列，上方503單位顯示世界。沿用相機 orthographicSize，因此原本上下世界範圍仍可見，寬度隨新 viewport 比例調整。依視窗比例置中，其他場景不受影響。玩法、碰撞、尖刺位置不變。

PASS：EditMode24/24，含960×540、1920×1080、1280×800、800×1000分區邊界檢查；正式Web建置成功與ZIP CRC／逐檔一致。Logs/viewport-editmode.xml、viewport-build.txt。瀏覽器正式Web第一關確認兩處坑底尖刺完整顯示於操作列上方；截圖Artifacts/viewport-spikes-visible.png。NOT RUN：本輪PlayMode、全四關通關與其他比例的瀏覽器實測。

---

# 2026-09-22 AI 尖刺與錄製提示刪除（完成）

刪除時間軸未錄製狀態的「移動、跳躍與等待都會錄下來」，其餘教學文案不變。使用內建 image_gen 生成透明銀白金屬尖刺 SteelSpike.png，Unity alpha 包圍框取圖，以單顆重複排列替換四關 9 排舊素材及紅色色調。sortingOrder=6；不改 LevelConfig、傷害判定、平台或動畫。更新 DemoBuilder 的關卡生成流程及 SpikeArtBuilder，避免重建恢復舊素材。素材、提示與重建方式：SPIKE_ART.md。

PASS：EditMode20/20、正式 Web build 0 errors / 1 warning、ZIP CRC／逐檔一致、來源副本一致；四關 LevelConfig 位元組一致。證據 Logs/ai-spike-editmode.xml、ai-spike-web-build.txt、ai-spike-spikes.txt。瀏覽器實際第四關預覽確認銀白三角尖刺清晰顯示，Q 開始錄製與 0.0 / 10.0 秒仍在，指定說明已消失；截圖 Artifacts/ai-spikes-l04.png。正式遊戲 Artifacts/Web、OneMoreTime-Web.zip 已更新。

NOT RUN：本輪 PlayMode 與四關完整瀏覽器通關，玩法及碰撞未改。原 Editor 未派發測試；誤在原目錄啟動的測試 wrapper 在編譯保護等待期間中止，實際測試來自隔離副本。保存場景備份 Artifacts/SceneBackups/before-ai-spikes。

---

# 2026-09-22 恢復原始 6 格走路動畫（完成）

依使用者要求，AnimationLibrary.run 恢復原始 Goat_06–Goat_11 六張 Sprite，RunCycleFrames=6、RunFramesPerSecond=12。GoatArtBuilder 重建預設保留原始動作，不再呼叫 GoatGaitBuilder。正式 Web 的角色、技能示範及結尾共用此動畫庫；保留此前位置補間與腳底對齊修正。

PASS：EditMode 20/20（含原始六張素材順序、循環、示範/結尾定位），正式 Web build 0 errors / 1 warning、ZIP CRC 與逐檔一致。瀏覽器啟動至 L01、拾取懷錶、對話及示範確認原始姿勢。Logs/restore-original-editmode.xml、restore-original-build.txt。NOT RUN：本輪 PlayMode、四關完整瀏覽器通關與結尾播放；既有 PlayMode 首次 E edge 測試問題未處理。QA/gait.html 與 EndingPreview 為上輪歷史預覽，非此次正式 Web。

---

# 2026-09-22 連續山羊步態重做（完成）

以固定身體及四肢部件烘焙 24 格循環，取代獨立生成姿勢及相似度排序。對角四肢交替，接地／抬腳位置及速度連續，固定 Sprite pivot。GoatArtBuilder 重建亦會使用新版本；遊戲、技能示範及結尾共用。原素材保留。實作與重建方式：GOAT_GAIT.md。

PASS：EditMode 20/20；PlayMode 47/48，唯一既有 FAIL 為 InputTests.HeldKeysAndResetSuppression Initial E edge，四關模擬通過。Web build 0 errors / 1 warning；ZIP CRC 與逐檔一致；C# 來源與隔離建置副本一致。Logs/gait-editmode.xml、gait-playmode.xml、gait-web-build.txt。

PASS：瀏覽器預覽播放／逐格首尾銜接、新 Web 懷錶示範、第一關移動／跳躍／定格通關；新 EndingPreview 行走至塔門及進門。截圖 Artifacts/gait-demo-0.png、gait-demo-1.png、gait-ending-1.png、gait-ending-2.png。預覽 http://127.0.0.1:8765/QA/gait.html 。NOT RUN：本次瀏覽器 L02–L04 全程與沙漏示範；未實測 FPS，步態改善不代表效能問題全部解決。

---

# 2026-09-22 技能示範對齊與播放時鐘修正（完成）

AbilityDemo改共用PlayerVisual.SpriteGUIRect按Sprite.pivot對齊，修正24格素材留白造成的換格跳動。展示時鐘由Update/unscaledDeltaTime逐畫面推進，FixedUpdate不再推進，實際世界仍凍結。腳步跟移動距離同步，Trace16秒停步後不空轉，Freeze定格姿勢固定。結尾改用同一對齊函式，行為不變。

PASS：EditMode17/17含示範尺寸對齊；PlayMode47/48，新增展示時鐘不受固定步長推進、120Hz展示dt可前進且不改實際世界之驗證通過。唯一FAIL仍為既有InputTests.HeldKeysAndResetSuppression Initial E edge。報告Logs/ability-smooth-editmode.xml、ability-smooth-playmode.xml。最後的Freeze固定姿勢一行調整於PlayMode之後，經EditMode編譯與Web實際示範驗證。

PASS：Web build 0 errors／1 warning、ZIP CRC／逐檔內容一致、全部C#與建置副本一致。Logs/ability-smooth-build.txt。瀏覽器實際L01懷錶示範播放至結束、回遊戲、L01/L02通關；L03沙漏對話、示範錄製／搭乘／停步／消失、重播及Enter返回未錄製遊戲均確認。截圖Artifacts/ability-smooth-freeze-0.png至2.png、ability-smooth-trace-0.png至2.png。

未以截圖宣稱實測FPS提升；本次未完成L03/L04全關卡瀏覽器通關。原Editor未操作，使用獨立副本建置。

---

# 2026-09-22 結尾24格山羊對齊與步速修正（完成）

修正EndingCinematic以整張圖格固定矩形繪製而忽略Sprite.pivot的錯誤。現在以各Sprite校正的腳底中心計算GUI矩形，消除新素材留白偏移造成的角色跳動。跑步相位隨移動距離推進，接近塔門減速時同步放慢脚步，進門保持連續相位。保留24格素材、原結尾流程與略過操作。

PASS：EditMode17/17（含全部跑步／待機姿勢在進門前後尺寸均對齊指定腳底）；正式Web build 0 errors／1 warning；ZIP CRC及逐檔一致，修改來源與建置副本一致。Logs/ending-anchor-editmode.xml、ending-anchor-build.txt。獨立EndingPreview瀏覽器連續擷取確認沿草地行走、抵達門口、縮小淡出進門及通關畫面；Artifacts/ending-anchor-1.png至4.png。未量測實際FPS，不把對齊修正宣稱為效能提升；本輪未重跑四關玩法。

---

# 2026-09-22 山羊24格跑步（完成）

新增透明GoatRun24.png與24個Sprite資產；修正生成圖格偏移，排列相鄰腿部輪廓形成循環。跑步由6格改為24格，最高48格／秒維持2循環／秒；依實際水平速度調整播放節奏。狐狸保留6格並按邏輯相位映射。示範、結尾與Gallery同步；其餘動作維持原素材，完整動作庫42張。生成說明與來源詳GOAT_RUN24.md。

PASS：EditMode 16/16，含24格引用、循環、總素材42張與狐狸相位檢查；PlayMode 47/48，唯一FAIL仍為既有InputTests.HeldKeysAndResetSuppression的Initial E edge。四關玩法與畫面補間測試PASS。Logs/run24-editmode.xml、run24-playmode.xml。素材24個圖格像素雜湊均不同，沒有重複原6格湊數。

PASS：正式Web build 0 errors／1 warning（Logs/run24-build.txt）；ZIP CRC／逐檔一致及全部C#與建置副本一致。瀏覽器新舊動畫比較頁確認透明背景、大小、腳底對齊與不同姿勢；新版Web拾取、教學、示範、L01移動／跳躍／定格通關。QA/run24.html可播放與逐格查看，截圖Artifacts/run24-comparison.png。NOT RUN：本輪Web完整L02–L04與結尾；循環動作品質仍可透過比較頁由使用者評估。

使用同版本獨立副本建置。原專案的測試命令被既有來源未編譯保護阻擋，未派發Editor測試；實際測試與報告來自獨立副本。

---

# 2026-09-22 移動顯示補間（完成）

角色與殘影在固定模擬位置間進行逐影格補間，相機跟隨相同顯示位置；背景視差明確在相機之後更新。保留50Hz物理／錄製／承載與所有原始座標；回起點重設補間歷史，暫停和對話時呈現當前位置。詳ARCHITECTURE.md。

PASS：PlayMode 48項中47項通過，含新增PresentationInterpolatesWithoutChangingSimulation及四關玩法回歸。唯一FAIL為既有InputTests.HeldKeysAndResetSuppression的Initial E edge，與前次結果相同。報告Artifacts/Logs/movement-interpolation-playmode.xml。正式Web建置0 errors／1 warning，ZIP CRC及逐檔內容一致、修改來源與建置副本一致。報告Artifacts/Logs/movement-interpolation-build.txt。

PASS：瀏覽器開場、L01撿取教學、略過能力示範，鍵盤輸入完成移動／跳躍／定格至通關；證據Artifacts/movement-interpolation-l01.png。NOT RUN：此輪瀏覽器L02–L04、實際GPU／CPU影格耗時分析；不能將補間修正宣稱為實測FPS提升。原Editor未操作，使用同版本獨立副本建置。

---

# 2026-09-22 結尾天空接縫修正（完成）

將 EndingCinematic 的48片半透明遮罩改成單張雙線性連續漸層，消除畫面縮放後的垂直接縫；退出選單時釋放執行期紋理。

PASS：正式Web建置0 errors／1 warning；獨立EndingPreview建置成功，瀏覽器全螢幕1512×949實際完成畫面無原有垂直細線。截圖Artifacts/tower-ending-no-seams.png。ZIP CRC與逐檔內容一致、修改來源與建置副本一致。建置報告Artifacts/Logs/ending-no-seams-build.txt。

本次只驗證結尾顯示，未重跑四關遊玩或完整測試套件。沿用獨立副本建置，未操作原Editor。

---

# 2026-09-22 回到魔法塔結尾動畫（完成）

Ending改為11秒回塔動畫：現有山羊走到魔法塔、停留、縮小淡出入門，門前魔法光點亮起，淡入恭喜通關與再玩一次。Esc／略過動畫跳到已入塔結尾；再玩一次返回MainMenu。背景由內建imagegen生成，資產與完整提示詞見ENDING_CINEMATIC.md。

PASS：15/15 EditMode（含新增3項結尾時間順序、略過與素材引用）；正式Web build 0 errors／1 warning，ZIP CRC／逐檔一致。工作區C#與實際建置副本一致。EndingPreview瀏覽器確認魔法塔背景、完成畫面、魔法光點，點再玩一次回主選單。Artifacts/tower-ending.png；Logs/tower-ending-build.txt、tower-ending-editmode.xml。

驗證界線：本次正式Web通過L01/L02；L03途中瀏覽器尺寸／焦點改變，未完成四關到結尾的完整端到端驗證，不列PASS。動畫逐幀走路／入門對齊尚未完整視覺擷取；時間狀態與資產引用測試PASS。結尾預覽為獨立Ending+MainMenu build，只供驗證，不包含正式ZIP。原Editor暫存檔問題仍未處理，沿用同版本獨立副本建置。

檔案：Runtime/EndingSequence.cs、EndingCinematic.cs、EndingAssets.cs、MenuScreen.cs；Editor/EndingAssetBuilder.cs；Resources/Ending/；Tests/EditMode/EndingTests.cs。沒有改動四關幾何或記憶玩法。

---

# 2026-09-22 拾取後能力自動示範（完成）

對話後新增懷錶16秒／沙漏22秒的遊戲內動畫示範，沿用山羊動畫，顯示按鍵、回起點、定格支撐與軌跡搭乘。可重播、Esc略過、結束後按Enter或換我試試；保留拾取位置。詳ABILITY_DEMO.md。

PASS：新版Web建置0 errors／1 warning、ZIP CRC與逐檔一致、工作區與建置副本來源一致。瀏覽器實際拾取兩道具、對話後進示範、懷錶完整播放／重播／Esc略過、L01／L02通關、沙漏播放結束並點換我試試，Q成功開始玩家自己的錄製。截圖Artifacts/ability-demo-freeze.png、ability-demo-trace.png；Logs/ability-demo-build.txt。

PlayMode：47項中46 PASS、1 FAIL（既有InputTests.HeldKeysAndResetSuppression的Initial E edge）；独立副本重跑結果相同，未修改該鍵盤測試。新增DemonstrationPausesWorldCanReplayAndSkip以及四關玩法測試PASS。完整結果Artifacts/Logs/ability-demo-playmode.xml。NOT RUN：本次Web完整L03/L04通關。

環境：原Editor的Temp/UnityLockfile遺失，工具誤走batch路徑後發現原Editor仍開啟且報暫存檔遺失；未強制關閉。改用/tmp/goat-ability-demo-validation獨立副本完成建置並更新原Web。Tools/omt.py增加macOS程序檢查，避免只靠lockfile再次開啟同專案。原Editor暫存檔提示尚未解決，後續需重啟Editor；Web可正常遊玩。

---

# 2026-09-22 道具教學文案更新（完成）

AbilityTutorial 已替換為使用者提供的懷錶3段／沙漏4段；「阿如果想重來一次就按 R」納入懷錶第3段。保留換行、口吻與原對話框。

PASS：Unity編譯、Web建置／字型驗證、ZIP CRC與逐檔一致；最長23字的行在20字級下保守估算小於584寬。NOT RUN：此次未重新進行瀏覽器畫面與玩法回歸。建置證據Artifacts/Logs/relic-copy-build.txt。

---

# 2026-09-22 道具教學改用開場對話樣式（完成）

GameUI.Dialogue 共用淡紫框、山羊頭像、小山羊名牌、繼續提示及下一句按鈕；開場與兩類道具教學均使用。道具文案保持不變，三行文案使用20字級；結尾頁按鈕為試試看。圖像與原meta移到Resources/Dialogue，保留GUID並更新PresentationBuilder來源路徑。

PASS：Unity編譯、Web建置／場景／字型驗證、ZIP CRC與逐檔一致；瀏覽器開場及L01實際拾取、三段教學、Enter及滑鼠推進、完成後關閉回遊戲。截圖Artifacts/pickup-dialogue.png，建置Artifacts/Logs/dialogue-style-build.txt。NOT RUN：L03道具瀏覽器流程、四關完整回歸；兩道具共用同一繪製函式，玩法狀態與文案未改。

---

# 2026-09-22 結尾補上恭喜通關（完成）

MenuScreen 在結尾左側、再玩一次按鈕上方加入「恭喜通關！」。

PASS：Unity 編譯、Web build、場景與字型驗證；Editor 實際顯示標題與按鈕無重疊，截圖 Artifacts/ending-congratulations.png。Web 與 ZIP 已更新，ZIP CRC／逐檔一致 PASS。NOT RUN：此次未重跑瀏覽器四關流程或玩法回歸。建置紀錄 Artifacts/Logs/ending-congratulations-build.txt。

---

# 2026-09-22 結尾畫面精簡（完成）

MenuScreen 移除結尾標語、謝謝標題、英文副標與分隔線、兩行介紹、04旅程完成框、底部署名列；結尾按鈕改「再玩一次」，刪除結尾旅人手冊，主選單手冊保留。按鈕維持回主選單功能。

PASS：Unity 編譯／Web build／場景與字型驗證；Unity Editor 實際執行 Ending 場景確認僅背景與再玩一次按鈕，點擊後返回主選單，logo 和手冊正常。驗證後停止 Play 並恢復原 SampleScene。ZIP CRC／逐檔一致 PASS；證據 Artifacts/Logs/ending-clean-build.txt、ending-clean-package.json 與本次 CUA Unity 畫面。NOT RUN：本次未重跑 Web 四關至結尾或玩法回歸。

---

# 2026-09-22 主選單使用新版 logo（完成）

主選單以使用者確認的 goat-tower-again-logo-v2.png 取代 GTA／Goat Tower Again 文字標題及分隔線。原圖無修改複製至 Assets/_OneMoreTime/Resources/Branding/GoatTowerAgain.png，SHA-256 一致。LogoMask.shader 在 UI 繪製時將黑色轉為透明遮罩，白色字形與錯位排列保留；兩次 AI 去背輸出有字形缺損，未採用。MenuScreen Awake 載入圖與材質，OnDestroy 釋放材質，位置為原960x540介面的(38,10,340,340)。結尾文案保留。

PASS：Unity 編譯、Web build／字型與場景驗證、瀏覽器主選單實際白字透明顯示及操作說明遮罩層級；ZIP CRC 與逐檔一致。證據 Artifacts/Logs/menu-logo-build.txt、menu-logo-package.json、本次瀏覽器畫面。NOT RUN：本次未重跑玩法回歸（視覺替換）。

---

# 2026-09-22 能力道具、拾取教學與獨立錄製時間軸（完成）

- L01 起點前方加入定格懷錶，拾取解鎖 E；L03 加入回溯沙漏，拾取解鎖 Q。L02/L04 沿用前關能力；直接進場亦可使用相應能力。新旅程重置拾取，當關 R／死亡／清除記憶不要求重撿。
- AI 透明像素道具原圖存放 Assets/_OneMoreTime/Resources/Relics/；固定模擬 Clock 控制上下浮動，僅玩家能拾取。拾取後消失並暫停模擬，顯示小山羊三頁／四頁能力說明，支援 Enter、Space 與滑鼠按鈕。
- L03/L04 獨立頂部時間軸：錄製秒數、10 秒上限與滾動窗口提示、已保存長度、播放進度、播放結束。未恢复被移除的三塊 HUD。
- 開場第三段改為找回掉落的時間道具。既有 E/Q 狀態次序、單一記憶、最近10秒、R 保留及四關地形不變。

PASS：46/46 PlayMode +12/12 EditMode（共58項）；含四關正常解法與新增4項拾取／解鎖／對話停表／持續錄製窗口／播放／重試／殘影不能拾取／新旅程測試。命令 python3 Tools/omt.py test --mode play/edit。Unity Web build Succeeded、0 errors、1既有warning；場景／字型驗證 PASS。

PASS：瀏覽器 L01 起點懷錶、拾取、三頁教學與正常通關；L02 沿用能力並通關；L03 起點沙漏、拾取、四頁教學，實按 Q 觀察 0.2→10.0秒、滿窗提示、保存10秒與播放0.3/10秒。圖像透明且文字不裁切，底部按鍵隨解鎖更新。所有瀏覽器輸入經鍵盤或畫面按鈕，不注入遊戲狀態。NOT RUN：本次 Web 未重新通過 L03/L04（兩關 PlayMode 通關 PASS），未測 Safari/Firefox。證據為本次 CUA 畫面、Artifacts/QA/relics.html、Artifacts/Logs/relic-playmode.xml、relic-editmode.xml、relic-build.txt、relic-package.json。

ZIP 已更新且 CRC／逐檔一致 PASS。素材生成提示與來源：Docs/Codex/RELIC_ART.md；Runtime/AbilityPickup.cs、AbilityTutorial.cs、RunManager.cs；Tests/PlayMode/AbilityTests.cs。無發布或遠端推送。

---

# 2026-09-22 刪除序章標示（完成）

移除 MenuScreen.DrawStory 左上「序章 / 山會記得」。開場三段共用此畫面；對話及略過按鈕保留。

PASS：Unity 編譯、Web build、瀏覽器開場畫面確認文字消失、ZIP CRC 與逐檔一致。證據 Artifacts/Logs/prologue-label-build.txt、prologue-label-package.json 與本次瀏覽器畫面。NOT RUN：玩法回歸（僅移除文字）。

---

# 2026-09-22 遊戲顯示名稱 GTA（完成）

主選單標題改為 GTA、副標題 Goat Tower Again；同步結尾署名、瀏覽器分頁、載入畫面、canvas 無障礙名稱與 Web productName。來源 MenuScreen.cs、Tools/web-shell.html。

PASS：Unity 編譯與 Web build；瀏覽器確認載入名稱、分頁與主選單標題完整顯示；ZIP CRC／逐檔一致。證據 Artifacts/Logs/gta-title-build.txt、gta-title-package.json 與本次瀏覽器畫面。NOT RUN：玩法回歸（僅顯示名稱變更）。

---

# 2026-09-22 移除三塊 HUD（完成）

依最新使用者要求，移除左上關卡資訊、右上記憶狀態（含軌跡進度）與下方教學提示框。四關共用 RunManager.OnGUI，遊玩及暫停均不再繪製。底部按鍵列保留。此要求取代舊規格的三塊 HUD 顯示要求；玩法與記憶機制不變。檔案：Assets/_OneMoreTime/Runtime/RunManager.cs。

PASS：Unity 編譯、Web build、靜態區塊檢查、ZIP CRC 與逐檔一致；瀏覽器 L01 遊玩與暫停畫面確認三框及文字均消失。證據 Artifacts/Logs/hud-clean-build.txt、hud-clean-package.json 與本次瀏覽器畫面。NOT RUN：四關玩法回歸（本次僅刪除顯示區塊）。

---

# 2026-09-22 暫停標題精簡（完成）

RunManager.DrawPause 標題改為「暫停」，刪除上方「ONE MORE TIME / 旅途小歇」。檔案：Assets/_OneMoreTime/Runtime/RunManager.cs。

PASS：Unity 編譯與 Web build、瀏覽器進入 L01 按 Esc 確認暫停標題和移除上方文字、ZIP CRC 與逐檔一致。證據 Artifacts/Logs/pause-copy-build.txt、pause-copy-package.json 與本次瀏覽器畫面。NOT RUN：玩法回歸（此次僅修改文字）。

---

# 2026-09-21 通關文案精簡（完成）

四關共用 RunManager.Completed 視窗標題改為「通關！」，移除 CHAPTER／記憶已抵達與「你與上一輪的自己，一起走到了這裡。」；保留下一段旅程／走向結尾按鈕及 Enter 行為。檔案：Assets/_OneMoreTime/Runtime/RunManager.cs。

PASS：Unity 編譯、Web build、ZIP CRC 與逐檔一致；瀏覽器以既有正常按鍵錄影通過 L01，確認精簡後通關視窗。證據 Artifacts/Logs/completion-copy-build.txt、completion-copy-package.json 與本次瀏覽器畫面。NOT RUN：本次未重跑四關玩法回歸；四關使用同一顯示分支。

---

# 2026-09-21 操作說明與角色名稱微調（完成）

刪除操作說明「先留下記憶，再與過去的自己合作。」；底部按鈕改為「繼續」；對話框「小山羊」文字下移 10 個介面單位。修改 Assets/_OneMoreTime/Runtime/MenuScreen.cs。

PASS：Unity 編譯、場景與字型驗證、Web build；瀏覽器確認提示已移除、繼續按鈕可返回、角色名稱下移且完整顯示。ZIP CRC 及逐檔一致 PASS。證據 Artifacts/Logs/ui-labels-build.txt、ui-labels-package.json 與本次瀏覽器畫面。NOT RUN：玩法回歸測試（本次僅文字與位置調整）。

---

# 2026-09-21 主選單四塊內容移除（完成）

依使用者四张截图，移除主選單左上標語與菱形、右上 04 個記憶篇章與外框、中段兩行介紹、底部署名文字與底條。標題、英文副標、按鈕、插畫及位置保留。變更僅限主選單，結尾畫面及來源授權檔保留。

PASS：Unity 編譯／場景與字型驗證／Web build；瀏覽器重新載入確認四塊均消失。Web/ZIP 已更新、CRC 與逐檔一致 PASS，證據 menu-clean-build.txt、menu-clean-package.json 及本次瀏覽器畫面。僅 UI 文字顯示變更，未重跑玩法測試。

---

# 2026-09-21 開場改為三段指定文案（完成）

MenuScreen.story 使用使用者提供的三段原文，保留換行及標點；刪除舊五段和頁碼。保留 Enter/空白鍵提示、下一句、出發吧及略過操作。

PASS：Unity 編譯／六場景與字形驗證／Web build；瀏覽器逐頁確認三段完整顯示、沒有頁碼、第三段顯示出發吧，Enter 後直接載入 L01。Web 與 ZIP 更新且 CRC/逐檔比對 PASS，證據 Artifacts/Logs/story-three-*、本次瀏覽器畫面工具輸出。僅修改文案，未新增测试或重跑玩法回歸；先前 54 項為歷史結果。

---

# 2026-09-21 後段地形消失：裁切回歸

- 根因：重新設計加長 Texture，但原 Sprite rect/UV/mesh 沒有更新。這是上一輪交付漏驗的顯示錯誤，與角色碰撞或關卡解法無關。
- 重現 PASS：新增四關完整範圍測試，修正前 8 PASS / 4 FAIL，terrain-crop-before.xml；舊裁切 L01 368×224、L02 384×224、L03 384×256、L04 640×224。
- 修正：TerrainArtBuilder 在尺寸變更時更新 Sprite 的原生資料，CopySerialized 保留原資產 GUID；SceneValidator 阻止裁切不完整的場景建置。
- PASS：重新生成後 12/12 EditMode、42/42 PlayMode；四關範圍 496×240、624×240、608×272、704×240；四份 Sprite metadata 未改動。證據 terrain-crop-editmode.xml、terrain-crop-playmode.xml、terrain-crop-scene-validation.txt、terrain-crop-guids-before.json。
- PASS：Web 重新建置、ZIP CRC 與逐檔比對；terrain-crop-build.txt、terrain-crop-package.json。
- 瀏覽器 PASS：透過 terrain-inspect.html 正常鍵盤輸入走到四關終點前，直接確認未被通關面板遮住的地形。四關出口岸皆有完整草皮／土層／邊界，L04 上下層末端及天花板完整；繼續向右後四關各出現通關面板。此頁只刪除時間軸最後 20 個輸入步，不注入遊戲狀態。畫面證據為本次 cua_repl 工具輸出；結果 terrain-crop-browser.json。

---

# 2026-09-21 四關挑戰版驗證

| 項目 | 結果與證據 |
|---|---|
| 真實環境／编譯 | PASS，Unity 6000.6.0f1、Input System 1.20.0、Web 模組存在。透過現有 EditorBridge，未終止 Editor 或啟動第二個 Editor。 |
| PlayMode | PASS 42/42，`python3 Tools/omt.py test --mode play`；redesign-playmode.xml。四關正常解均是真實 InputFrame；單元重置／死亡 fixture 的 ResetAt 不用於正常通關。 |
| EditMode | PASS 8/8，`python3 Tools/omt.py test --mode edit`；redesign-editmode.xml。 |
| 場景與字型 | PASS，`python3 Tools/omt.py validate`；redesign-scene-validation.txt、redesign-font-validation.txt。四關皆一玩家／一殘影／一相機；L04 兩踏板三門；出口落地 gap=0。 |
| 防繞關 | PASS 已測路徑：無記憶、地面／過高定格、懸空壓板、L03 12 組定格時機、L04 單板定格／無等待／未回 A／下層借殘影撞天花板。不是全連續輸入的形式化證明。 |
| Web build | PASS，`python3 Tools/omt.py build-web`；redesign-web-build.txt，0 errors、1 既有 URP warning。其後僅更新 Web 外框及 Editor 建置雜湊算法，遊戲二進位與測試過的 Runtime/Config/Scenes 相同。 |
| 瀏覽器四關流程 | PASS，Codex In-app Browser / Chrome 153，完整 MainMenu→開場略過→L01→L02→L03→L04→Ending；四關各觀察到 CHAPTER 完成畫面。`Artifacts/QA/browser.html` 只派送正常鍵盤事件，無遊戲狀態注入、傳送或通關呼叫。紀錄 redesign-browser-validation.json；截图在本次任務工具輸出，沒有聲稱本機 PNG。 |
| ZIP | PASS，`python3 Tools/omt.py package-web`，CRC、root index、4 引用、WASM magic、6 檔與 Web 逐檔一致；redesign-package-validation.json。 |
| 視窗與錯誤邊界 | 正常窄視窗以 16:9 canvas 留邊，沒有裁掉起點；第四關上下路線及 A/B/A 可見。尺寸 override 造成擷取拼接，重設後驗證，960×540/1920×1080 精確尺寸本次 NOT RUN。首輪舊資源混用發生 Unity 錯誤；完整 bundle hash 刷新後最終通關無新增 Unity 錯誤，無來源 MutationObserver 仍留在主控台。 |
| 新手難度／跨瀏覽器 | NOT RUN，沒有獨立新手測試者；未測 Safari/Firefox。參考解法時間不是初玩時長。 |

首輪 PlayMode 37/39：L01 原落點 x12 太遠，將高台起點調至 x11 後正向解通過；L02 負向測試原在落回踏板後檢查門，修正為離地時觀察，不改門規則。新增地面/過高定格與未回 A 反例後最終 42/42。

本次生成由 ChallengeLayouts 明確更新四 Config 並備份，不以初始化旗標靜默覆寫既有設定；一般 Generate 繼續沿用 Config。未改全局 PlayerTuning、Sunny Land 來源、無關場景或使用者既有刪除。

---

# 2026-09-21 開場劇情

實作：MainMenu 的開始旅程先顯示五段山羊獨白，再進 L01。沿用使用者 SVG 淡紫對話框，新增 AI 像素山羊透明胸像。Enter／Space／下一句推進，Esc／略過劇情直接開始。關卡重試不重播開場，E/Q 手動流程保留。生成器已配置兩個貼圖並驗證引用。

驗證：8 EditMode＋39 PlayMode 全數 PASS；六場景／繁中字形 PASS。Web 實測逐句閱讀、空白鍵／Enter／滑鼠推進、完成後進 L01。最終版滑鼠／Esc 略過均進 L01，略過按鈕單行顯示 PASS，Web／ZIP CRC 與資源一致性 PASS。瀏覽器尺寸覆寫產生擷取異常，重設後以原視窗確認；未將該次異常截圖當成 960×540 驗收。四關通關本次沒有重跑，上一版 controls-* 是歷史證據。最後僅加寬略過按鈕，未重跑玩法測試；已重新編譯、驗證及建置。

證據：Artifacts/Logs/opening-*；本機網址 http://127.0.0.1:8765/Web/ 。Unity 起始場景 Assets/_OneMoreTime/Scenes/MainMenu.unity。

---

# 2026-09-21 E/Q 手動操作版驗證

PASS：8 EditMode、39 PlayMode，共47項，failed=0、skipped=0，退出碼0。證據controls-editmode.xml、controls-playmode.xml。更新舊按鍵語意測試，新增手動起錄／短錄製／Q保存與播放／Q播放中清除／Q結束後清除／未起錄死亡／錄製中E定格／E再次清除回起點，並保留四關正常解、反例與單一記憶回歸。

PASS：實際Web E保存→再次E清除、Q手動開始→保存→播放→清除後等待，HUD及移除重錄按鈕的暫停選單已檢查。controls-pause.png、controls-recording.png、controls-cleared-idle.png。首次Web曾出現function signature mismatch；資源URL加入WASM雜湊版本後載入成功，後續流程以新資源執行。建置／打包已成功，ZIP解壓及逐檔一致性見package-validation.json。PASS：最終Web已從主選單完成四關至結尾，controls-L01-complete.png至controls-L04-complete.png、controls-ending.png；第三關首次第二跳偏早，R保留原手動錄製重試成功。未注入位置、快照或通關方法。

以下為歷史版本驗證，確認視窗／自動錄製舊描述不適用本版。

---

# 2026-09-21 連續地形美術版（最新驗證）

- PASS：生成器／六場景引用檢查；四關Continuous landscape存在，四個出口Sprite底部與地面差值均0.000000。`landscape-scene-validation.txt`。
- PASS：8/8 EditMode、38/38 PlayMode，failed=0、skipped=0，命令退出碼0；`landscape-editmode.xml`、`landscape-playmode.xml`。包含四关正常解法、反例、軌跡承載及重試回歸。
- PASS：Web建置Succeeded，0 errors、1既有URP warning，5.306秒增量；`landscape-web-build.txt`。
- PASS：新版ZIP實際解壓、CRC、index根目錄、四項資源引用、WASM magic、逐檔內容等於Web輸出；`package-validation.json`。
- 最新效能樣本：50次重試2.027ms，物件44固定，Unity allocated memory +25,600 bytes；不宣稱零配置。
- PASS：最終Web逐關畫面與完整按鍵流程：landscape-L01.png至landscape-L04.png、landscape-L03-cliff.png、各關landscape-L0x-complete.png、landscape-ending.png。第三關首次第二跳偏早，R保留同錄製重試成功；沒有位置注入或直接通關。下方舊結果屬歷史版本。

---

# 2026-09-19 地板接縫修正（最新）

已修正圖集取樣造成的透明細線，以及草皮逐塊非等比縮放。PresentationBuilder將土塊／草皮切為獨立16×16 Texture資產，Point、Clamp、無mipmap；草皮用連續Tiled renderer，取消逐片壓扁與末端拉伸。原Sunny Land PNG不變，碰撞體與關卡幾何不變。

PASS：generate／六場景validate／8 EditMode／Web build（0 errors、1既有warning）／ZIP解壓與逐檔比對。瀏覽器960×540及1920×1080畫面已檢查，原本水平透明線消失；第一關正常按鍵通關。證據Artifacts/Logs/terrain-seams-fixed-960.png、terrain-seams-fixed-1920.png、terrain-seams-L01-complete.png。此次未重新跑完整38項PlayMode與四關端到端，上一版結果保留為歷史證據。最新ZIP25,796,460 bytes，雜湊見BUILD.md。

---

# 山羊美術版回歸（2026-09-19，最新）

- PASS：`python3 Tools/omt.py generate`、`validate`，退出碼0；六個正式場景0 Missing Script、每關1 player/camera/echo。證據 `Artifacts/Logs/art-v2-scene-validation.txt`。
- PASS：`python3 Tools/omt.py test --mode edit`，8/8；`--mode play`，38/38；退出碼0，無略過。證據 `art-v2-editmode.xml`、`art-v2-playmode.xml`。新增24幀素材／狐狸保留、動畫序列邊界、動作轉換與定格姿態測試；原四關正常解及防繞關回歸均通過。
- PASS：Unity動畫展示場景實際渲染五種山羊動作與狐狸；`goat-animation-gallery.png`。透明合成、腳底對齊與不同腿部姿勢已觀察。
- PASS：主選單960×540及1920×1080實際畫面（art-v2-mainmenu-960.png／art-v2-mainmenu-1920.png），手冊art-v2-instructions.png。
- PASS：新版Web暫停、兩個音量滑桿拖曳、重新錄製取消／確認；`art-v2-audio-pause.png`、`art-v2-gameplay.png`。不代表主觀音質評估。
- PASS：最終Web增量建置，0 errors／1 warning；重新打包後實際解壓、CRC、根目錄index、4個引用、WASM magic及逐檔比對；`web-build.txt`、`package-validation.json`。
- PASS：新版Web主選單→L01→L02→L03（原軌跡R重試）→L04→Ending，全部僅玩家按鍵；`art-v2-L01-complete.png`至`art-v2-L04-complete.png`、`art-v2-ending.png`。第三關第一趟第二跳較早落下，保留原錄製重試成功，沒有修改玩法或注入快照。
- 效能最新樣本：50次重試2.119ms，物件73固定，配置記憶體532,911,331→532,936,931（+25,600）；30.01/60.01/120.00 FPS下100固定步均x11.75999。
- 已修復：MechanismSignal原先與另一MonoBehaviour同檔，重載場景出現missing reference；已拆成獨立腳本、重新生成，建置前強制驗證場景。背景森林恢復原圖比例。

以下為原玩法基準矩陣與證據；舊畫面／43項測試數屬升級前版本。新版仍保留同一套核心玩法，UI-05獨立新手試玩仍NOT RUN。

---

# 最終驗收紀錄

2026-09-19。P＝真實 Unity PlayMode，E＝EditMode，W＝實際 Chromium Web 操作。下表 PASS 依列出的測試範圍與證據；有限防繞關測試不等於所有連續輸入的形式化證明。只有 UI-05 無獨立新手測試者，維持 NOT RUN。

最終 **37 PlayMode + 6 EditMode = 43 項測試通過，0 failed、0 skipped**。證據 `Artifacts/Logs/final-playmode.xml`、`final-editmode.xml`；其他 stage XML 是歷史，不能代替最終結果。

環境：Apple M4 Pro、macOS 27.0 (26A428)，Unity 6000.6.0f1，URP 17.6.0，Input System 1.20.0；Codex IAB Chromium 153。Safari／Firefox／其他裝置未測。瀏覽器 UA 的 Intel Mac10_15_7 為相容字串，非實際硬體。

執行：`python3 Tools/omt.py test --mode edit`、`test --mode play`、`validate`、`build-web`、`package-web` 最终退出碼皆0。測試在已開啟 Editor 透過 TestRunnerApi 執行，wrapper 依XML summary判定，不把派送成功或0個測試當通過。最後補充測試一度等待編譯逾時（exit1）；透過Unity Refresh後重新執行37項全過。Web runtime未修改，補充案例只在Tests assembly。

所有下列證據檔均位於 `Artifacts/Logs/`，文件位於 `Docs/Codex/`。

| ID | 狀態 | 實際證據與範圍 |
|---|---|---|
| S0-01 | PASS | AUDIT.md、ASSET_INVENTORY.md、LICENSES.md；實際 Editor／素材 metadata／作者授權頁及本地指示比對。 |
| S0-02 | PASS | AUDIT.md、ASSET_INVENTORY.md、LICENSES.md；實際 Editor／素材 metadata／作者授權頁及本地指示比對。 |
| S0-03 | PASS | AUDIT.md、ASSET_INVENTORY.md、LICENSES.md；實際 Editor／素材 metadata／作者授權頁及本地指示比對。 |
| S0-04 | PASS | AUDIT.md、ASSET_INVENTORY.md、LICENSES.md；實際 Editor／素材 metadata／作者授權頁及本地指示比對。 |
| GEN-01 | PASS | idempotence-before.txt 與 idempotence-after.txt 完全一致；生成兩次無重複角色／機關。 |
| GEN-02 | PASS | scene-validation.txt、font-validation.txt、validate.txt；六場景缺脚本 0、引用有效；真實測試與 Web 編譯成功。 |
| GEN-03 | PASS | git diff 與 AUDIT.md；Sunny Land 原素材及 SampleScene 未改；使用第 8 空圖層；Unity Web 的 URP 序列化差異已記錄。 |
| GEN-04 | PASS | scene-validation.txt、font-validation.txt、validate.txt；六場景缺脚本 0、引用有效；真實測試與 Web 編譯成功。 |
| BASE-01 | PASS | P: WallsCeilingAndDeceleration、LongShortAndNoDoubleJump；W: 四關正常按鍵與可見角色位置。 |
| BASE-02 | PASS | P: WallsCeilingAndDeceleration、LongShortAndNoDoubleJump；W: 四關正常按鍵與可見角色位置。 |
| BASE-03 | PASS | P: WallsCeilingAndDeceleration、LongShortAndNoDoubleJump；W: 四關正常按鍵與可見角色位置。 |
| BASE-04 | PASS | P: CoyoteAndBufferWindows、HeldKeysAndResetSuppression；窗口內外與重試舊按鍵。 |
| BASE-05 | PASS | jump-measurement.txt；H=2.92320、D=5.16000、B=.90，固定步 .02。 |
| BASE-06 | PASS | P: ActualRenderRates 與 RenderCadenceDoesNotAlterSimulation；render-rates.txt，位移容差 .015。 |
| BASE-07 | PASS | P: DeathReviewKeepsOnlyLivingFrames、PreparationResetAndDeathDiscard、FreezeCannotOverwriteAndPauseStopsClock；危險與 R/E/Q 同步時只合法轉移。 |
| FREEZE-01 | PASS | P: FreezeNormalSolution、FreezeCannotOverwriteAndPauseStopsClock、HeldKeysAndResetSuppression；保存後立即回出生點，不連續覆寫。 |
| FREEZE-02 | PASS | P: FreezeNormalSolution、FreezeCannotOverwriteAndPauseStopsClock、HeldKeysAndResetSuppression；保存後立即回出生點，不連續覆寫。 |
| FREEZE-03 | PASS | P: EchoDisappearsAndDoesNotPullFromBelow、EchoCeilingAndWalkOff、FreezeNormalSolution；底下不吸附、上方承載、走離失效。 |
| FREEZE-04 | PASS | W: L01 空中／L02 站立定格畫面；生成頂面亮線與 PlayerTuning 固定背寬／背高一致，不隨 sprite 外形。 |
| FREEZE-05 | PASS | P: FreezeNormalSolution、FreezeCannotOverwriteAndPauseStopsClock、HeldKeysAndResetSuppression；保存後立即回出生點，不連續覆寫。 |
| RESET-01 | PASS | P: LongPreparationAndFiftyRetries（50 次 R）、FreezeCannotOverwriteAndPauseStopsClock（10 次死亡）；同一快照／物件數不變。 |
| RESET-02 | PASS | P: PreparationResetAndDeathDiscard；準備 R 清 recorder／Clock，Memory 仍 null。 |
| RESET-03 | PASS | W: E→暫停→重新錄製→保留記憶→仍為利用輪；再確認清除→準備輪；web-rerecord-confirm.png。P: Rerecord 後門板關閉。 |
| RESET-04 | PASS | P: DeathReviewKeepsOnlyLivingFrames、PreparationResetAndDeathDiscard、FreezeCannotOverwriteAndPauseStopsClock；危險與 R/E/Q 同步時只合法轉移。 |
| RESET-05 | PASS | P: HeldKeysAndResetSuppression；W: 失焦／暫停後恢復未自動移動，Q 必須新按。 |
| L01-01 | PASS | P: FreezeNormalSolution；W: web-L01-complete.png，方向／Space／E 真實輸入。 |
| L01-02 | PASS | P: NoMemoryCannotClimb，x=8/10/11.5/12.5；高台4.5>H；LEVELS.md。 |
| TRACE-01 | PASS | E: LatestWindow(3/10/15/60)，固定容量501、最近窗口含端點。 |
| TRACE-02 | PASS | E: ImmutableAndWaiting、LatestWindow；保存後覆寫 ring 不改快照，原世界 x=12 與2秒等待保留。P: TraceNormalSolution。 |
| TRACE-03 | PASS | E: ImmutableAndWaiting、LatestWindow；保存後覆寫 ring 不改快照，原世界 x=12 與2秒等待保留。P: TraceNormalSolution。 |
| TRACE-04 | PASS | E: ImmutableAndWaiting、LatestWindow；保存後覆寫 ring 不改快照，原世界 x=12 與2秒等待保留。P: TraceNormalSolution。 |
| TRACE-05 | PASS | E: InvalidInput；0/1、.2秒、NaN、Infinity、重複時間拒絕。 |
| TRACE-06 | PASS | P: TraceNormalSolution、TraceCannotOverwriteAfterDeathAndRepeatedQ；Ready 不支撐、Q 一次、重試復位。 |
| TRACE-07 | PASS | P: TraceNormalSolution、TraceCannotOverwriteAfterDeathAndRepeatedQ；Ready 不支撐、Q 一次、重試復位。 |
| TRACE-08 | PASS | P: TraceNormalSolution 結束作用停用 + EchoDisappearsAndDoesNotPullFromBelow 承載失效後下落；W: Finished 文字與殘影消散。 |
| TRACE-09 | PASS | P: TraceNormalSolution、TraceCannotOverwriteAfterDeathAndRepeatedQ；Ready 不支撐、Q 一次、重試復位。 |
| TRACE-10 | PASS | P: DeathReviewKeepsOnlyLivingFrames（停留2秒不追加）、PreparationResetAndDeathDiscard；Q保存活著窗口、R丟棄。 |
| TRACE-11 | PASS | P: TraceCannotOverwriteAfterDeathAndRepeatedQ；Trace 利用輪死亡10次仍相同快照。 |
| TRACE-12 | PASS | P: PreparationResetAndDeathDiscard、DeathReviewKeepsOnlyLivingFrames；錄製／播放暫停的 Clock／PlaybackTime 不變。 |
| CARRY-01 | PASS | P: EchoCarry 四組水平／上升／下降／斜向；相對誤差<.005；起跳 v.y=13.36，不繼承平台速度。 |
| CARRY-02 | PASS | P: EchoCarry 四組水平／上升／下降／斜向；相對誤差<.005；起跳 v.y=13.36，不繼承平台速度。 |
| CARRY-03 | PASS | P: EchoDisappearsAndDoesNotPullFromBelow、EchoCeilingAndWalkOff；底下接近、頂面、走離、消失、ResetAt。 |
| CARRY-04 | PASS | P: EchoCarry 四組水平／上升／下降／斜向；相對誤差<.005；起跳 v.y=13.36，不繼承平台速度。 |
| CARRY-05 | PASS | P: EchoDisappearsAndDoesNotPullFromBelow、EchoCeilingAndWalkOff；底下接近、頂面、走離、消失、ResetAt。 |
| CARRY-06 | PASS | P: EchoCannotCarryThroughWorld、EchoCeilingAndWalkOff、LowerRouteCannotRideThroughSeparation；牆與天花板阻擋承載。 |
| L03-01 | PASS | P: TraceNormalSolution；W: web-L03-complete.png，同一記憶 R 重試調整第二跳後通關。 |
| L03-02 | PASS | P: TraceLevelFreezeSweepCannotReachExit；12組上升高度freezeTick=2,4,…24；幾何2H<6.3<2H+B，詳 LEVELS.md。 |
| PLATE-01 | PASS | P: LegalSourcesOnly、PressureNormalSolutionAndReset、TimingNormalSolution；合法grounded來源OR；空中／inactive排除。 |
| PLATE-02 | PASS | P: LegalSourcesOnly、PressureNormalSolutionAndReset、TimingNormalSolution；合法grounded來源OR；空中／inactive排除。 |
| PLATE-03 | PASS | P: LegalSourcesOnly、PressureNormalSolutionAndReset、TimingNormalSolution；合法grounded來源OR；空中／inactive排除。 |
| DOOR-01 | PASS | P: PressureSoloAndAirborneFreezeFail；離板立即關門，空中定格無作用；頂門接天花板。有限路徑測試與幾何，不是全操作窮舉。 |
| DOOR-02 | PASS | P: DoorProtectsOnlyActualOccupant；門洞內保護，離開即關。 |
| DOOR-03 | PASS | P: PressureNormalSolutionAndReset、DoorProtectsOnlyActualOccupant；重置先關，再按保留合法記憶重算。 |
| L02-01 | PASS | P: PressureNormalSolutionAndReset；W: web-L02-complete.png。 |
| L02-02 | PASS | P: PressureSoloAndAirborneFreezeFail；離板立即關門，空中定格無作用；頂門接天花板。有限路徑測試與幾何，不是全操作窮舉。 |
| CRUMBLE-01 | PASS | P: CrumbleIgnoresSideAndEcho、CrumbleTimerResetsWithoutOldCallbacks；只有活角色頂面ground支撐啟動。 |
| CRUMBLE-02 | PASS | P: CrumbleTimerResetsWithoutOldCallbacks、TimingNoMemoryAndCrumblePauseSafePoint；暫停／完成崩塌／R及超過舊倒數無殘留回呼。 |
| CRUMBLE-03 | PASS | P: CrumbleTimerResetsWithoutOldCallbacks、TimingNoMemoryAndCrumblePauseSafePoint；暫停／完成崩塌／R及超過舊倒數無殘留回呼。 |
| L04-01 | PASS | P: TimingNormalSolution；W: web-L04-complete.png，實際保存9.4秒，A/B等待由按鍵產生。 |
| L04-02 | PASS | P: TimingFreezeCannotOpenBothDoors（A/B各一次）、TimingNoMemoryAndCrumblePauseSafePoint（無記憶）。 |
| L04-03 | PASS | P: TimingWithoutWaitingFailsAtRequiredDoors 對照 TimingNormalSolution；初版失敗已藉上層幾何修正。 |
| L04-04 | PASS | P: LowerRouteCannotRideThroughSeparation，腳底<=2.52；門接頂、崩塌下危險帶、外牆；幾何及針對性路徑。 |
| L04-05 | PASS | P: TimingNoMemoryAndCrumblePauseSafePoint；安全區 x25 等4秒仍 y5、Playing。此為隔離fixture，非通關證據。 |
| FLOW-01 | PASS | W: 同一最終建置，主選單→L01→L02→L03（R重試）→L04→Ending；web-L01/02/03/04-complete.png、web-ending.png。 |
| FLOW-02 | PASS | P: 四個 LevelTests 各直接載入正式場景；W: Enter 逐關切換與暫停回主選單；無跨場景記憶。 |
| UI-01 | PASS | W: web-mainmenu-960.png、web-mainmenu-1920.png、web-instructions-1920.png、web-audio-pause-1920.png，四關／結尾截圖；font-validation.txt 掃描全部 runtime 非ASCII 字元。 |
| UI-02 | PASS | W: Freeze 可踩、Trace 僅預覽／可踩／已消散 HUD，加亮線／虛線；四關實際錄製與回放觀察。 |
| UI-03 | PASS | P: 暫停時鐘與输入suppression；W: Esc、P、確認取消、點 QA header 引發失焦暫停、回復無舊輸入。 |
| UI-04 | PASS | W: web-L04-layout.png 與最終 web-L04-complete.png；上／下層、A/B標號連線、固定安全區可辨識。前者為同幾何較早截圖。 |
| UI-05 | NOT RUN | 無獨立新手測試者；未宣稱10–15分鐘首次遊玩時長或理解率。 |
| PERF-01 | PASS | P: LongPreparationAndFiftyRetries；60秒ring=501；50次重試物件22固定，retry-performance.txt。 |
| PERF-02 | PASS | render-rates.txt、retry-performance.txt、browser-cadence.txt；裝置與測量限制見下方。 |
| BUILD-01 | PASS | web-build.txt：Succeeded，0 errors、1 warning；真實data/framework/loader/wasm。 |
| BUILD-02 | PASS | package-validation.json；實際解壓、CRC、全部index引用、WASM magic、逐檔相同；ZIP根目錄index.html。 |
| WEB-01 | PASS | W: HTTP /Web/ 成功載入真實Unity runtime；四關完整運行，無遊戲載入阻塞。主控台警告與無來源例外見下方，沒有隱藏。 |
| WEB-02 | PASS | W: E/Q/R、P/Esc、焦點、逐關切換及兩個音量滑桿操作；web-audio-pause-1920.png。未做外接音訊設備或主觀聽感評估。 |
| WEB-03 | PASS | W: 同一最終建置，主選單→L01→L02→L03（R重試）→L04→Ending；web-L01/02/03/04-complete.png、web-ending.png。 |
| HANDOFF-01 | PASS | STATUS／ARCHITECTURE／LEVELS／TEST_REPORT／BUILD／LICENSES；Artifacts/Web 與實際ZIP，沒有發布或push。 |

## Web 端到端方法與限制

HTTP server：`python3 -m http.server 8765 --bind 127.0.0.1 --directory Artifacts`。外部 `Artifacts/QA/browser.html` iframe 載入不修改的 final Web；只發送 DOM keydown/keyup，從準備輪實際錄製，不讀寫Unity狀態、不傳送角色、不注入快照、不呼叫完成。各關的實際通關畫面及最後Ending已核對。

第三關最初的自動時間軸曾跳太早而失敗，保留原錄製，透過R重試、延後第二跳約 .16秒後成功。成功重試輸入保存於 `solution-L03-retry-browser.json`；當時暫以第一關輸入按鈕載入，因此第三關完成截圖上方寫L01 inputs，遊戲HUD與場景仍是03。原本第一關JSON已還原，QA新增明確Retry L03按鈕。此結果證明可操作解法及重試保留，不代表不熟悉解法的玩家都能立即通關。

機制隔離測試允許 ResetAt 安排牆壁／危險／安全點fixture；四關正常通關與Web解法均只靠玩家輸入，沒有把fixture位置安排冒充通關。

## 效能量測

- 真實Editor渲染目標30/60/120，實測30.01/60.00/119.99 FPS；每次100固定步，x=11.75999，容差.015。不是跨平台位元一致宣稱。
- 50次同步重試合計2.077ms；物件數22固定。Unity allocated memory 528,854,499→528,880,099 bytes（+25,600，單次樣本，不能宣稱零配置或已排除所有長時間洩漏）。
- 瀏覽器結尾畫面5秒RAF：中位16.70ms，p95 18.30ms，298樣本。這是頁面渲染回呼間隔，不冒充Unity Profiler CPU/GPU frame time，也不代表所有裝置。

## 已知非阻塞項與未測項

- Web build 1 warning；瀏覽器提示URP的Edge Adaptive Spatial Upsampling shader被剝除／不支援，PostProcessing pass不執行。本demo不使用該效果，實際場景正常渲染並通關。
- `browser-console.json` 保留幾次載入時無source URL的MutationObserver.observe TypeError。已查本專案HTML／loader／framework無MutationObserver字串，推測屬瀏覽器附加環境，來源仍未確認；不能把它說成已修復或主控台零錯誤。未造成遊戲載入／輸入／四關流程阻塞，未觀察到Unity NullReference或Missing Script。
- UI-05新手試玩NOT RUN；Safari／Firefox、手機觸控、外接音訊設備未测。本規格為鍵盤桌面demo，無需帳號或伺服器。
- 無公開發布、無Git commit/push。後續若要發布，使用者可自行上傳已驗證ZIP。
