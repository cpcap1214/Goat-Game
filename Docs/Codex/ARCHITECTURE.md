## Replay ability tiers (2026-11-16)

Debug menu 06 selects basic and 07 selects upgraded replay using the same Debug_Rewind practice scene. SceneFlow passes a one-use upgrade request consumed in Awake; direct scene loads default to basic. `ReplayUpgradeUnlocked` gates both direction toggles and playback hold. `UnlockReplayUpgrade()` enables the later-game upgrade without replacing the recording. Both tiers retain endpoint afterimages. The future pickup location and cross-room save integration remain unassigned.

## Rewind debug prototype (2026-11-14)

`RewindPlaytest` is the opt-in controller for `Debug_Rewind`; the legacy tutorials and EchoFoothills retain their original memory flow. At 50 Hz, a Q hold reaching 0.5 seconds clears the previous echo/path and captures the recording's initial world frame at the player's current location. Releasing the initiating hold does not finish recording. A later Q tap, the 10-second/501-frame limit, or recording death starts a 5x visible reverse pass. No scene reload occurs. At the end, the exact first world frame is restored; tap Q to replay the original world-coordinate goat trace. Hold Q to overwrite even during playback. Pause freezes the clock; R during recording finishes/rewinds, while R outside recording restores the debug room's initial world and retains the saved trace.

World frames contain full `PlayerMotor2D.RewindState` (position, velocity, facing, animation phase, grounding/coyote/buffer/drop state), `RoomPlatform.RewindState` (position, home, endpoint direction/distance, timer, activation and previous carrying data), crate positions, plate pressure-source count and door state. These objects are the dynamic simulation participants in the debug scene. Music is not rewound. Playback ghosts reuse EchoSurface/MemorySnapshot and contain no live input/controller. PlayerMotor2D remains the sole owner of rider displacement. EchoPath stays visible after recording; playback clamps at either endpoint with an active stationary echo. Holding Left Shift freezes only the replay cursor while world simulation continues. A tap under 0.25 seconds toggles direction on release; releasing a longer hold resumes the same direction. Left Shift taps toggle the signed playback clock from the current cursor and resumes a held endpoint. Input presses are buffered until the fixed tick and cleared on pause; recording resets direction to forward. This supersedes the legacy one-shot fade only in Debug_Rewind.

`affectedByRewind` is a public Inspector bool on `RewindCrate`, `RoomPlatform`, `PressurePlate` and `LinkedDoor`. False skips restoration during the reverse pass, holding that object's end state while the rest of the world reverses; it resumes normal simulation afterward. An explicit R restart still restores all debug objects. There are no in-game affected-object labels. Additional dynamic object types must provide their own state capture/restore and be registered in the controller before they are rewindable; arbitrary scripts/Rigidbody2D objects are not automatically recorded.

Scene generation: `Tools > One More Time > Build Rewind Playtest` (`RewindPlaytestBuilder.Build`). The builder manages only the debug root/scene, its trace material, and its Build Settings entry. Existing player/echo prefabs, art and the Input System are reused.

## Main-room mechanisms (2026-11-03)

`MainRoom.Step` evaluates pressure sources, ticks `RoomPlatform` once, syncs world transforms, then advances `PlayerMotor2D`. The motor owns platform carrying (excluding the carrying collider from its own displacement casts), relative-top landing, jumping without inherited velocity, and drop-through. Platforms do not move the player independently. `RoomPlatform` supports optional endpoint travel and release-delay countdown; disabled platforms remain faintly visible with no collision. No Update/coroutine timer runs outside the room clock. `RoomCheckpoint[]` stores multiple once-activated statues; `checkpoint` becomes the newest saved feet position. Retry/death retain Shutter and reset mechanisms, then reevaluate preserved pressure sources.

## 2026-10-31 記憶解謎更新

現行第一大關已加入ChapterMemory：定格庭院、雙影水閘、搭上昨天三種解謎，E/Q記憶、F年代、X互動。場景入口與生成器不變；詳CHAPTER_ONE.md及最新TEST_REPORT.md，以下為舊版歷史。

# 2026-10-31 第一大關更新

新主線使用獨立 NostalgiaChapter／EraTerrain／ChapterLayout，沿用 PlayerMotor2D 與 Input System。現在與過去透過固定步切換碰撞，不重設玩家位置速度；另一年代重疊玩家時拒絕切換。照片及裝置跨重生保留；新關不啟用舊 E/Q 記憶。詳 CHAPTER_ONE.md。

---

# 2026-09-22 能力道具與時間軸

RunManager 在 Awake 依關卡建立一個 AbilityPickup：L01 為 Freeze、L03 為 Trace，位於 spawn + (1.7, 0.85)。L02 繼承 Freeze；L04 繼承兩能力。每關直接開啟仍可玩，不依賴跨場景靜態狀態。回到 L01 開始新旅程會重新拾取。道具在本關 ResetRun／R／死亡／清除記憶後保持已拾取；無關 Scene 與配置資產無須重生。

拾取判定僅使用 live player 的身體矩形；echo 不觸發。上下浮動由 RunManager 固定步 Clock 驅動，停頓與教學不前進。圖片由 Resources/Relics 載入，在場景內建立 SpriteRenderer，銷毀時釋放 runtime Sprite；圖片本體由 Resources 管理。專用 importer 僅管理此資料夾，透明、Point、256px。

FlowState.AbilityDialogue 暫停模擬。Enter／Space／按鈕逐頁，關閉時清空並抑制已按住輸入，防止跳躍／技能穿透。AbilityTutorial 集中保存兩套文案。道具解鎖與單一 MemorySnapshot 分開；既有 E/Q 互斥、10 秒窗口與 R 保留規則不變。

独立時間軸只在 L03/L04 且 Trace 解鎖後顯示，讀取 Recorder.Duration 或 Memory.Duration／PlaybackTime，不依系統時間另算。錄製上限10秒、保存後總長、播放進度與結束均有文字；不恢復已移除的關卡／提示面板。

---

# 2026-09-21 關卡挑戰版補充

`ChallengeLayouts.Apply` 是明確的配置遷移入口，備份四個 Config 後用 Unity API 更新，最後呼叫既有 DemoBuilder。一般 Generate 不自動重設 layout，保留 Inspector 編輯。

`LevelConfig.doorSources` 可為空（舊一對一）或按門索引提供踏板索引。L04 使用 [0,1,0]：A、B、A；DemoBuilder、PresentationBuilder、SceneValidator 共用 DoorSource，所以碰撞機關、連線與字母不分歧。沒有新感應規則、持續開門延遲或第二份記憶。

`cameraY` 由關卡配置決定；角色 motor、錄製、支撐、定格／軌跡状态機保持原行為。Web 外框固定 16:9；完整 Build 檔案雜湊保證場景資料變更也更新 query version。

---

# 架構與重要決策

Unity 6000.6.0f1、URP 17.6.0、Input System 1.20.0，沿用原專案。OneMoreTime.Runtime 為獨立 assembly；Tests 與 Editor 分離，Web 排除測試 assemblies。

PlayerMotor2D 是唯一位移來源：kinematic Rigidbody2D、腳底原點、固定 BoxCollider2D、Physics2D.BoxCast 分軸掃掠 OMTWorld。固定步 .02 秒；殘影承載 delta 也走相同 world cast，只套用一次，起跳不繼承平台速度。單向頂面不建立側面實體碰撞。PlayerTuning 集中速度、跳高、重力、coyote、buffer、背部尺寸；同一份資產供四關使用。

KeyboardPlayerInput 在 Update 鎖存按鍵邊緣，FixedUpdate 消耗；Clear 後須先放開舊按鍵。IPlayerInput 支援真實語義輸入測試。RunManager 集中準備／利用輪、Playing／Paused／DeathReview／Completed 與 Ready／Playing／Finished；危險優先於 R，再 E/Q，最後出口。ResetRun 清除輸入、錄製時鐘、支撐、回放次數及全部機關；保留唯一 Memory。Rerecord 才刪除 Memory。新場景不繼承快照。

Recorder 用固定 501 樣本 ring buffer 保留最近最多 10 秒（50 Hz，含端點）；MemorySnapshot 複製資料並只提供值讀取，保留世界座標、等待、姿勢、方向。Ready 僅預覽；Q 後按保存姿態插值，不重新模擬按鍵。Finished 立即停用支撐／踏板並在 .22 秒內淡出；不可循環。

PressurePlate 只認合法 grounded feet，玩家與殘影來源 OR；LinkedDoor 立即响应，但已有角色在門洞中時延後實體關閉。CrumblingPlatform 只認活角色 GroundCollider，以固定步倒數，不使用 coroutine，所以重試不留下舊回呼。

六個正式場景由 SceneFlow 串接，Dev_Playground 不進建置。GameUI 採 960×540 邏輯座標的等比置中 IMGUI，無 Canvas／EventSystem 依賴；HUD 同時使用文字、圖示與支撐亮線。Noto Sans TC Medium 500 實例化自官方可變字型。Sunny Land Sprite／背景／音樂引用原素材；GameAudio 產生七個原創短音效、提供獨立音量。

DemoBuilder 只管理 Assets/_OneMoreTime 與場景 OMT Managed root。additive 生成保留使用者原開啟場景／未存內容；既有場景先備份到 Artifacts/SceneBackups，保留非 managed roots。已初始化 LevelConfig 不覆寫手動幾何；Prefab、Scene、ScriptableObject 路徑穩定，保留 GUID。重跑生成前後數量一致，六場景各一相機，四關各一玩家／殘影。

EditorBridge 在現有 Editor 輪詢白名單命令；Tools/omt.py 等待全部本遊戲 C# 的編譯 SHA256 一致才派送，避免新腳本編譯失敗卻執行舊版本。TestRunnerApi 輸出真實 XML 計數，零測試不能通過。WebBuilder 明確列六場景、不改原 build scene 清單，產出後套用滿版 web-shell.html。Unity 建置造成的 URP prefilter/runtime serialization 已記錄，原有未提交刪除與套件差異未覆寫。

## 2026-09-21 地形外觀

TerrainArtBuilder僅負責視覺：將LevelConfig.terrain轉成16 PPU的聯集遮罩，在外輪廓放Sunny Land草皮／岩石，內部用原素材色及稀疏石塊。每關輸出一個Generated/TerrainLandscapeN Texture/Sprite，原BoxCollider2D仍由各Rect建立，沒有斜坡或能力變更。出口Goal Sprite按實際bounds高度落地；SceneValidator驗證四關gap為0。

## 2026-09-21 手動錄製狀態

RunManager.IsRecording與Memory分開：Preparation可為尚未錄製或錄製中；Q首次設定recordingStart並採第一幀，以相對時鐘記錄，第二次保存。ResetRun一律停止錄製並清空buffer；保留Memory時回Ready。E僅清除既有Freeze，Q於Trace Playing/Finished清除，均透過Rerecord→ResetRun回起點；互異記憶鍵不覆寫。只有IsRecording時準備轮死亡進DeathReview；尚未錄製死亡直接重置。移除confirm狀態及Pause中的重錄入口。


開場劇情：MenuScreen 使用局部 storyPage，主選單開始時切入五頁獨白。劇情 UI 與關卡不共存，完成／略過才呼叫 SceneFlow.Load(1)，因此沒有角色錄製時鐘或記憶副作用。素材由 PresentationBuilder.Menus 配置、SceneValidator 檢查。原 SVG 另存，PNG 保留原比例。


## 2026-09-22 畫面補間

RunManager 在每次遊玩模擬前保留角色與殘影的位置，LateUpdate 用 (Time.time-Time.fixedTime)/Time.fixedDeltaTime 在前後兩次位置間補間。只寫入 PlayerVisual 子物件，碰撞主物件、PlayerMotor2D.Position、EchoSurface.Pose 和 Recorder 維持原本50Hz。相機使用角色的補間位置；SceneryMotion 於 RunManager 之後更新，避免視差晚一個影格。回起點時重設前後位置，暫停、對話與非自動模擬時顯示當前位置，防止跨重生位置拖曳。此方法有最多一個固定步長（20ms）的顯示延遲，不改物理速度。


## 2026-09-22 操作列與世界視野分區

RunManager.LateUpdate 依 GameUI.GameplayViewport 設定攝影機 normalized rect。960×540 UI 的底部37單位專供操作列，上方503單位顯示世界；保持 orthographicSize，因此不裁掉原本世界上下界。矩形使用與 GameUI 相同的等比縮放與置中，適用寬/高視窗。操作列改為不透明底色。其他選單場景不使用此 viewport。


## 2026-11-19 Main Rooms 5–8

`ChapterOneRoom` orchestrates the new room sequence, entry/exit fades, the past-view lens, checkpoints, hazards, and plate/door evaluation. It disables the old `MainRoom` and `RewindPlaytest` automatic loops: `RewindPlaytest.Step` remains the single 0.02-second simulation clock for the player, moving platforms, floating sources, and rewind participants. Shared movement tuning is unchanged.

`ChapterEraGroup` keeps authored colors/sorting in serialized baseline arrays, so entering, reloading and rewinding never repeatedly darkens a bridge. Inactive spans retain a dark background image but disable collision; switching refuses to embed the player in an emerging span. `ChapterOneRoom` snapshots era/plate/door state as a state-only `RewindParticipant`. The new `HasTransformHistory` property defaults to true; state-only controllers and collapsing bridges opt out of decorative transform paths/tints while retaining rewind state.

`ChapterCrumbleBridge` records its warning timer, collision and falling artwork. Floating timber uses the existing `CrateSpawner` pools and replay carry; sources remain empty in the saved scenes and spawn one item on the first live tick. Preview rendering seeds temporary items without saving them. The reverse-playback pickup in Room 7 uses session-level `MainGameProgress.ReplayUpgradeUnlocked`; Room 8 enables that upgrade for independent debug testing. No disk save or Chapter 2 scene is introduced.

The partial `ChapterOneExpansionBuilder` owns only `Room N Expansion Managed` in Scenes 05–08 and the Room 4 destination reference. Layout coordinates are separated into `ChapterOneRooms56.cs` and `ChapterOneRooms78.cs`. Rebuilding replaces each managed root and preserves other roots; loaded dirty target scenes are rejected.


## 2026-11-23 Chapter ability scope and shared memory switching

`MainGameProgress.IsDebugMode` is initialized only by the Debug menu and cleared by New Game or menu return. Chapter Rooms 5–8 capture that scope in `ChapterOneRoom`: normal play hides the advanced cassette, masks reverse/hold input, disables the filter, enables both authored bridge-span sets and sends floating sources downstream from the left bank. Debug entry preserves the original filter and upstream/reverse layouts. Runtime defaults do not rewrite serialized scenes. `CrateDisposalTrigger.disposeToRight` supports the downstream endpoint while retaining the previous leftward default.

`CameraEquipment.For` derives every visible module from the local acquisition state and chapter scope. The camera body itself requires the shutter pickup; before acquiring any ability, the assembly is empty. The top-left replay status/tips panel has been removed from `RewindPlaytestHUD`.

`RewindPlaytest.TryUseShutter` centralizes switching from a saved replay to a shutter image for both Room 4 and later chapter rooms. Only a successful shutter capture discards replay memory, paths and playback controls; recording, world rewind and pause reject the switch. Player position/velocity and the owned upgrade are retained. Starting another recording still replaces shutter, keeping a single memory slot.

## 2026-11-23 Waiting between world rewind and replay

`RewindPlaytest.AwaitingReplay` is true when a saved memory is Ready and its echo is inactive. This deliberately excludes an active echo parked at either playback endpoint. The fixed simulation clock holds all affected platforms/crates/participants while waiting; player simulation and nonaffected objects continue. Plate/door evaluation in both the replay driver and `ChapterOneRoom` respects the same per-object flags. The saved snapshot is not reapplied to the player each tick.

`RoomPlatform.HoldFrame()` clears carrier delta and updates the stationary top plane without evaluating activation, changing shuttle direction, or consuming time. `RewindParticipant.HoldFrame()` supplies an optional equivalent hook; floating sources use it to clear each object's carrier delta. `Tick(0)` is intentionally not used because it can still spawn items or change mechanism state. Crate disposal callbacks also respect waiting. All velocities, bob clocks, activation edges, timers and recorded frames are preserved for resumption.

A replay key tap releases the wait via the existing Play transition; holding to record again and successful shutter replacement clear the saved memory and release it too. Retry preserves memory and re-enters waiting. The Room 4 imaginary acquisition has no recorded crate movement, but now also waits before its ordinary source simulation starts. No camera/UI/ability-tier changes were introduced.

### Room 7 tower mechanisms (2026-11-23)

`RoomPlatform` retains its default plate-controlled shuttle behavior and adds explicit `ClosedRoute`/`DispatchedRoute` modes, local waypoint offsets, full-cycle durations, simultaneous plate requirements, and a permanent activation latch. Motion and collider carry remain owned by the shared replay clock; route progress, pooled deck existence, press edges and cooldown are captured in rewind snapshots. `PlatformDispatcher` only launches from an active pool and never advances a deck a second time. Waiting for replay calls existing hold behavior.

`RoomTrapdoor` deliberately does not implement `RewindParticipant`; its source plate sets `affectedByRewind=false`. `ChapterOneRoom` evaluates the triggering plate and consumes the entire reveal step before any gameplay/recording clocks. The door separates its leaves, disables their colliders when fully open, and overlays a vertical camera pan/zoom while x remains 10. The once-only opening persists through retry and world rewind. `exitUpwards` gates the airborne exit on the opened door and animates the goat upward before loading the next room.

## 2026-11-23 Room 8 traveling checkpoint and reveal

`RoomPlatform.Motion.OneWayTravel` appends a serialized enum value, preserving older motion modes. A plate latches the trip, `journeySeconds` controls its linear duration, and the platform parks solid at the endpoint. Its existing rewind snapshot contains journey state; the existing simulation/carry path remains the only movement owner.

`MovingRoomCheckpoint` parents the statue to the deck and synchronizes `MainRoom.ActiveCheckpoint` by object identity. `RewindPlaytest.RetryRestored` runs after world restoration but before the ordinary `Retried` revival callback; this allows the platform, statue and player to return home before revival begins. `WorldRewindFinished` handles recording deaths after their visible rewind. Ordinary rewinds do not invoke the death reset. Saved memory is retained.

`RoomPlatformReveal` is driven by `ChapterOneRoom.Step`, using only a presentation timer while gameplay simulation, recording, platform movement and floating-object clocks are paused. It pans to the remote carrier and returns, respects pause, and resets on death. Decorative water shaders retain their existing rendering-time animation. Room 8's `enterFromBelow` handles the upward arrival without modifying Room 7's exit.

`RoomEightLayout` owns official geometry; `RoomEightScenery` owns noncolliding scaffold and waterfall/cave art. The original Room 8 scene was copied by AssetDatabase into `Tests/Fixtures/Main_Room08.unity` to retain earlier era/crumble/river tests; it is excluded from Build Settings.

### 2026-11-24 Shutter clearance and sinking visuals

`EchoSurface.WaitForPlayerToExit` captures the new shutter body's bounds. `CanSupport` arms it only after the complete live player body exits; `Active` remains available to pressure plates. Both MainRoom and replay's shutter fallback consult this gate. `PlayerMotor2D.ReleaseEchoSupport` clears stale support/coyote state when replacing a copy, preventing upward snapping. Recorded replay echoes retain their existing support rules.

`FloatingObject2D` snapshots the whirlpool's waterline and applies a cached per-renderer `FloatingSubmersion` shader only while sinking. World-space clipping hides pixels below the surface without recoloring the visible sprite; restoring a nonsinking state restores its original material. Pool lifetime and collision remain driven by existing simulation. The shader is in Resources so builds include it.


## 2026-11-24 Current room order and acquisition gate

This section supersedes room numbers and destination references in the earlier dated architecture records. Physical scene assets were moved with GUIDs preserved: the former Room 6 riverworks is `Scenes/Main_Room04.unity`, the former Room 4 acquisition is `Scenes/Main_Room05.unity`, and the former Room 5 broken crossing is `Scenes/Main_Room06.unity`. Campaign order remains 1 → 2 → 3 → 4 → 5 → 6 → 7 → 8. Debug entries 9/10/11 now open riverworks/acquisition/bridges respectively; entries 12/13 remain Rooms 7/8.

`ChapterOneExpansionBuilder.BuildRoom(4)` builds riverworks through `Layout4` in the retained `ChapterOneRooms56.cs`. `BuildRoom(6)` builds the broken crossing through `Layout6` in the retained `RoomFiveLayout.cs`. `BuildRoom(5)` delegates to `RoomFourBuilder.Build()`; stable legacy `RoomFour*` class/script/prefab names avoid breaking references. Its output and managed root are now `Main_Room05.unity` / `Room 5 Managed`, and the Editor menu commands read **Build Main Room 5** and **Update Existing Room 5**. The broader generator command is **Build Chapter 1 Rooms 4–8**. Builders continue to reject unsaved target-scene edits and only manage their own generated content.

`ChapterOneRoom.ReplayAvailable` requires `roomNumber > 4` and the session's basic-replay unlock. Room 4 does not grant that unlock, and both normal and Debug entry gate held-record, replay-direction/hold input, replay HUD and the pause camera's cassette. It retains the shared `RewindPlaytest.Step` simulation for movement, logs and platforms, and Shutter still works. Merely using that backend does not grant the player recording. The acquisition remains the explicit unlock point in Room 5; later rooms may grant basic replay on direct entry for independent playtesting. Existing past/advanced ability scope is unchanged.

Room 4 receives Room 3's lift at dock **(2,1)**, traveling **7 units at 3 units/sec**. Its one-way landing x=0.5–4 stays open underneath; solid entrance bank begins at x=4 and ends at x=8. The three final timber steps are unchanged. A bank with top y=12, body x=92–97 and inward bevel x=91–92 replaces the old high exit lift. Trigger x=95–97 starts the horizontal exit to Room 5.

`Room4AbilityTutorial.SceneName` is now `Main_Room05`, with `nextRoomScene=Main_Room06`. When `room.lift` is null, the controller starts at **(0.5,4)** and walks to **(2,4)** while fading in; once arrived it captures the retry anchor and starts Approach or Playing according to acquisition ownership. Its optional lift branch remains compatible. Tape pickup, bridge collapse, slow fall, 0.25-second record hold, imaginary rewind and menu-return reset retain their previous behavior. Room 6's new automatic exit lift docks at **(88,9)** and travels **8 units at 4 units/sec**, fading into Room 7.

The current user-facing layout guide is `ROOMS_5_TO_8.md` despite its retained filename. `ROOM_FOUR.md` begins with current Room 5 instructions and preserves its older dated implementation/test records below a superseded notice. Test results for this reorder are recorded separately in `STATUS.md` and `TEST_REPORT.md`.
