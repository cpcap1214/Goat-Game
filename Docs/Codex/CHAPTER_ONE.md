# 2026-10-31 操作教學與連續地形修訂

使用者回饋操作提示不清楚、地形像單格方塊且不好看。新增ChapterGuidance：三個機關第一次進入時顯示教學卡，暫停模擬、不消耗操作；Enter翻頁／開始，Esc關閉，H或「操作教學」按鈕可隨時重看。末段拆成錄製與搭乘兩頁。下方E/Q動態標示目前動作，場內另顯示依位置、記憶種類與回放狀態選出的下一步，包含舊錄影清除、水車／鐘樓、拍照與失敗重試。

新增ChapterTerrainArt，以世界座標生成連續草緣及岩層剖面，取代16px單格反覆貼圖。所有非橋地形向下延伸至y=-22，圖像與碰撞一致；相接地面共用世界座標紋理，深土層不再懸空。保留橋、斷口及年代機關的必要間隙。只改新大關的生成器／地形，不改舊四關素材。生成PNG與meta位於Art/ChapterOne/Terrain，由Unity匯入；它們是程序生成的像素素材，非外部圖片。

本輪Unity編譯／場景生成、62/62完整PlayMode回歸通過；定格與接力教學、連續岩土畫面已目視。最後色調與提示位置微調另驗編譯／畫面。

---

# 2026-10-31 試玩後改版：以記憶解謎取代重複跳台

使用者實測原版2分7秒，並指出遊戲全部是平台跳躍、很無聊。此回饋優先於單純延長路線。新方向為三個不同的記憶解謎空間；保留古橋及鐘樓的少量年代跳躍。

1. **定格庭院**：在過去站上踏板，E保存自己並回旗幟；本體切回現在穿越石門。離開踏板就關門，單靠本體不能跑過去；過去的石牆迫使玩家換年代。
2. **雙影水閘**：Q開始錄影，依序站A和B，Q保存；本體站在遠端白色控制台，Q播放。錄影踩A再踩B時，本體必須持續留在控制台，石門才解鎖。單一定格不能同時完成兩段訊號。
3. **搭上昨天**：錄下等候、跳上中島、等候、原地長跳。保存後回左岸，播放並跳上自己的殘影；乘著記錄移動，在它跳起時再從背上起跳，登上單次跳躍到不了的高崖。

**操作更新**：E定格／清除定格，Q開始錄影／保存回旗幟／播放／清除，F切年代，X世界互動與拍照，R保留已存記憶回旗幟，Esc暫停。僅一份記憶；錄影最近10秒，等待保留，不自動開始。錄影中死亡才提供保存死前軌跡選擇。

使用原有MemorySnapshot、Recorder、EchoSurface與PlayerMotor2D承載，新增ChapterMemory協調第一大關，不啟用第二套角色位移。新回歸61/61通過，包含整關真正錄製、回放與承載；定格／水閘Unity畫面已檢查；下方為此前版本歷史，不能用來代替本次改版證據。4–5分鐘仍需在新玩法完成後重新評估，不沿用原版測試時間。

---

# 第一大關：回聲山麓

2026-10-31 使用者新授權取代四小關作為目前主線；舊四關及單一記憶機制原檔保留。第一大關只實作附圖的「懷舊濾鏡：地圖恢復成以前樣貌」，不先做廣角／閃光燈。目標新玩家約4–5分鐘；未經完整試玩前不宣稱已達成。

## 路線與規則

主選單 → 27秒戶外大賽開場 → Chapter01_EchoFoothills。384單位的連續關卡，六區：山腳入口、歲月樹冠、沉睡水車、退潮河道、鐘樓迴廊、雙時峰頂。三處取景點、兩個在過去操作的機關、11個重生旗幟。

A/D移動、Space跳躍、F切換年代、E互動／拍照、R重生、Esc暫停。開始拾取發光鏡片後解鎖濾鏡。現在與過去使用相同世界座標及玩家能力、不同碰撞地形；淡色輪廓預告另一年代平台。切換不重置位置或速度；若另一年代的牆會包住玩家則拒絕切換。機關啟動與已拍照片在死亡／R後保留，回到最近安全旗幟並恢復現在。

地形：過去完整古橋／現在岩石封路／過去出口障礙；交錯的現在樹枝和過去腳手架；在過去啟動水車，使現在長橋升起；河道兩年代高低平台交替；過去鐘樓機關解鎖現在石橋；最後混合攀登，在現在拍下峰頂。最後完成條件要求三張照片、兩個機關。

## 實作與生成

- Runtime/ChapterOne/ChapterLayout.cs：雙年代碰撞地形與目標位置。
- Runtime/ChapterOne/EraTerrain.cs：切換碰撞、另一年代輪廓。
- Runtime/ChapterOne/NostalgiaChapter.cs：固定步模擬、互動、重生、教學、HUD。
- Editor/ChapterOneBuilder.cs：只重建 Chapter One Managed，使用真實Unity API生成場景／meta；場景重建先備份。
- Scenes/Chapter01_EchoFoothills.unity：獨立第一大關。
- Tests/PlayMode/ChapterOneTests.cs：角色馬達整關路線、防卡牆、持久進度。

Unity plugin：以 unity-cli 技能、unity command eval 執行 ChapterOneBuilder.Build()。主選單改為 SceneFlow.LoadChapter；WebBuilder 包含新場景，舊場景仍可獨立測試。

## AI素材

使用內建 imagegen，非外部API。正式資產：Art/ChapterOne/PresentValley.png、PastValley.png。

現在提示：wide pixel-art mountainous forest valley, abandoned weathered stone aqueduct, ruined watermill, giant old cedar, cyan-blue daylight, no characters or UI, layered mountains and clear gameplay middle area。
過去提示：edit same camera/geography; restore aqueduct, tower and working watermill; younger cedar and less overgrowth; warm amber morning light, maintained sandstone; preserve pixel style and no characters/text。

兩張背景由同一構圖衍生。它們是遠景，不充當碰撞；真正可玩的兩張年代地圖由EraBlock分別定義與生成。

## 驗證進度（2026-10-31）

PASS：Unity plugin 編譯與真實場景生成；完整 PlayMode 54/54，包含6項新大關測試以及舊關共享馬達回歸。完整正常馬達路線無瞬移，完成兩機關、三照片、26次切換、0次死亡，模擬遊玩時間103.9546秒。修正共面地板角點造成水平移動卡住的問題。

PASS：真實鍵盤 Enter 進教學、D走近拾取、F現在／過去切換、Space跳起、R回(3,0)、Esc暫停／恢復。Unity截圖檢視入口、樹冠、河道、水車、鐘樓與峰頂。後段截圖使用定位工具，只證明視覺，不算手動通關。水車啟動後轉動，鐘樓有獨立鐘體、支柱與拉繩。兩個取景點出口在拍照後才放行，避免下落後漏拍而無法完成。

證據：`Artifacts/Logs/chapter-input-playmode.json`、`chapter-route.txt`，以及 `Artifacts/ChapterCaptures/` 下的截圖。Artifacts 不加入 Git。

NOT RUN：新手完整試玩時長、完整鍵盤手動通關、更新 Web 建置。4–5分鐘仍是首次遊玩目標；104秒是知道解法且不失誤的自動路線，不得標成新手時長。已請使用者試玩回報時間及卡點，待據此調整。

## 素材補充

`Art/ChapterOne/NostalgiaFilter.png`：透明背景黃銅古董相機濾鏡、琥珀鏡片、皮革繫帶及細小符文；由內建 imagegen 生成，用於拾取物與教學框。三項素材皆已匯入為 Point-filter Sprite；背景無碰撞。

## 目前下一步

確認首次遊玩節奏，必要時調整檢查點及提示。現行場景可玩到完成畫面；不把自動通關與局部截圖等同新手4–5分鐘驗收。

## 完成條件稽核

| 要求 | 現況與證據 |
| --- | --- |
| 使用Unity plugin實作驗證 | PASS：unity-cli/Pipeline連線6000.6.0f1、編譯成功、生成真實Scene |
| 第一大關完整可通關 | PASS：正常馬達整關測試，三照片兩機關完成；54項回歸通過 |
| 懷舊濾鏡與兩張地圖 | PASS：兩背景、分開的Present/Past地形群組、切換碰撞、空中動量保留與卡牆拒絕 |
| 解謎與平台跳躍 | PASS：古橋雙年代阻擋、水車／古鐘開橋、交替年代攀登 |
| 四小關暫時擱置 | PASS：主選單進新大關；舊場景保留且測試仍執行 |
| 約4–5分鐘 | 待驗證：熟悉解法自動路線104秒；尚無首次玩家完整試玩時間 |
| 可直接預覽 | PASS：Unity真實Game View，起點教學Enter開始 |

剩餘需要首次玩家的實際時間與卡點回饋；不因程式／測試通過而宣告整個時長目標已驗收。
