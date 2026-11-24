# 2026-11-16 Q restarts active replay

Tap Q during playback to restart from time zero in the forward direction, preserving the saved recording and live player/world. Holding Q still records a replacement after 0.5 seconds. Both debug tiers supported. PASS: Unity compilation; replay suite 10/10 including mid-forward/backward restart, preserved memory/player and existing hold behavior; git diff --check. First runner returned stale running status after exiting; retry passed. Evidence: Artifacts/Logs/replay-restart-tests.json. Manual keyboard playtest NOT RUN.

# 2026-11-16 Replay playhead marker

Added a warm cream circular playhead with a dark amber outline at the timeline cursor. It follows existing clamped progress in forward/backward playback and fast rewind, and stays still while held/paused. Both debug tiers use it. Display-only indicator; no drag seeking added. PASS: Unity compilation, git diff --check. No gameplay tests rerun for this drawing-only change; manual visual playtest NOT RUN.

# 2026-11-16 Replay bar placement

Replay timeline thickness increased from 4 to 10 UI pixels; y moved from 458 to 486, leaving 7 pixels above the gameplay viewport bottom/footer. Applies to both debug replay tiers; amber styling and width retained. PASS: git diff --check. Layout-only change; no gameplay tests rerun.

# 2026-11-16 Fast-rewind tint fix

RewindPlaytest.Pose now uses the shared amber ReplayColor during the fast world rewind instead of the leftover cyan tint. Live goat returns to white after rewind. Applies to both basic/upgraded debug entries. PASS: Unity compilation, git diff --check, replay suite 9/9 including rewind tint and restored normal color assertions. Initial targeted test runner failed to report completion; full-suite retry passed. Evidence: Artifacts/Logs/rewind-tint-test.json. NOT RUN: manual visual playtest.

# 2026-11-16 Faster recording and warm minimal replay UI

Both replay tiers now charge in 0.5 seconds. Recording film strips are 42 UI pixels high (was 24), travel at 180 pixels/second (was 28), and reveal with a soft left-to-right fade over 0.16 seconds. Header shifted down to avoid the thicker strips. Timeline reduced to only a warm amber track/fill, matching the thin trace and afterimage. Past lens unchanged per clarification.

PASS: Unity compilation; RewindPlaytestTests 9/9 including 24-tick/25-tick hold boundary and warm ghost/trace assertions; git diff --check. First runner attempt hit an Editor Play mode state error; retry from Edit mode passed. Evidence: Artifacts/Logs/replay-warm-ui-tests.json. Recording and playback screenshots inspected in Artifacts/Logs/replay-warm-captures. Final strip seam cleanup compiled. NOT RUN: human full-speed input/animation playtest. Playtest entries: Debug Mode 06 Basic Replay / 07 Upgraded Replay.

# 2026-11-16 Past lens presentation

EchoFoothills F era switching now triggers a 0.36-second circular glass lens animation (expanding into past, contracting into present). Past view has subtle warm tint, fine grain and edge darkening beneath the HUD. Collision switches remain immediate; blocked switches do not trigger the effect. Respawn clears the effect; pause freezes its fixed-step clock.

PASS: Unity compilation (zero errors/warnings), PastLensTransitionAndRespawn 1/1, git diff --check. Evidence: Artifacts/Logs/past-lens-tests.json. Keyboard regression filter returned zero tests, so NOT RUN. Captured game-view images in Artifacts/Logs/past-filter-captures; static visuals inspected, full-speed human animation/keyboard playtest NOT RUN. Final rim-opacity correction compiled successfully. Try Main Menu > Debug Mode > 05 EchoFoothills, collect the lens and press F.

# 2026-11-16 Separate basic and upgraded replay

Main Menu > Debug Mode now offers 06 Basic Replay and 07 Upgraded Replay. Both use Debug_Rewind with separate ability tiers; basic supports forward playback and endpoint persistence, while upgraded adds Left Shift reversal/hold pause. `ReplayUpgradeUnlocked` defaults false; `UnlockReplayUpgrade()` is the explicit later-pickup hook. No later-game pickup location or cross-room progression persistence was assigned.

PASS: Unity compilation, 9/9 live RewindPlaytestTests, git diff --check. New tests exercise both real SceneFlow entry routes, basic ignoring Shift, unlocking on the existing recording, and no upgrade leakage on reload. Evidence: Artifacts/Logs/replay-tier-tests.json. NOT RUN: manual menu/keyboard playtest. Next: user playtest of entries 06 and 07.

# 2026-11-16 Hold Left Shift to pause replay

In Debug_Rewind, Left Shift immediately stops the replay cursor while held; the player/world keep simulating and the echo stays active. Release a hold to resume the same direction. A quick tap under 0.25 seconds toggles direction on release. HUD/timeline show PAUSED.

PASS: Unity compilation with zero errors/warnings; live Pipeline RewindPlaytestTests 7/7 passed, including held cursor/pose, continued moving-platform simulation, and resuming both directions. Evidence: Artifacts/Logs/rewind-hold-tests.json. PASS: git diff --check. NOT RUN: human keyboard playtest. The initial test-launch permission review timed out; the permitted retry succeeded. Next: user playtest via Main Menu > Debug Mode > 06 Rewind.

# 2026-11-16 Persistent bidirectional replay

Debug_Rewind afterimages now stay active at the final frame, including support and pressure-plate interaction. Left Shift toggles forward/backward from the current cursor and resumes from a held endpoint; it never loops automatically. The timeline shows direction/HELD. Hold Q still overwrites after 1 second. Entry: Main Menu > Debug Mode > 06 Rewind.

PASS: live Unity compilation (0 errors/warnings); `unity command run_tests --mode playmode --filter RewindPlaytestTests --async_tests` plus `test_status`: 6/6 passed. Coverage includes mid-playback reversal, both exact endpoints, persistent plate/gate, pause, overwrite and world rewind. Evidence: Artifacts/Logs/rewind-direction-tests.json. PASS: git diff --check. NOT RUN: human keyboard playtest or Web build. Next: user playtest.

# 2026-11-15 Rewind presentation validation

PASS: live Unity Pipeline `run_tests --mode playmode --filter RewindPlaytestTests --async_tests`, followed by `test_status`: 5 total, 5 passed. Updated checks cover the 1-second threshold, charge state, hidden ghost support renderer, and replay progress freezing during pause and reaching 1 at completion. Evidence: Artifacts/Logs/rewind-presentation-tests.json.

PASS: final `recompile_status` completed with failed=false and errors=[] after a domain-reload connection interruption. PASS: live Game view inspection at 1280x720 of charge circle, recording film strips/filter, subtle trace and mid-playback timeline. Fixed ring placement and stale footer rendering found during inspection. Captures: Artifacts/Logs/rewind-presentation-captures/captured-assets/Logs/rewind-{charge,recording,playback}.png. Visual checks use scripted simulation steps, not human keyboard playtesting. NOT RUN: Web build/browser validation.

# 2026-11-14 Rewind playtest

PASS: `unity run /Users/wesley/Documents/GitHub/Goat-Game --editor-version 6000.6.0f1 --no-tail --log-file Artifacts/Logs/rewind-debug-build.log -- -executeMethod OneMoreTime.EditorTools.RewindPlaytestBuilder.Build` completed with exit 0 after handling batch mode's initial untitled scene. The real Editor produced Debug_Rewind.unity, its material/metadata and enabled Build Settings entry.

PASS: `unity test /Users/wesley/Documents/GitHub/Goat-Game --editor-version 6000.6.0f1 --mode PlayMode --filter OneMoreTime.Tests.RewindPlaytestTests --output Artifacts/Logs/rewind-debug-tests.xml --format json`: 5 executed, 5 passed. Tests exercise the 1.5-second threshold, release of the initiating hold, overwrite during playback, persistent path, multi-tick reverse, restoration at the current recording start, crate/platform reversal, `affectedByRewind=false`, paused clock, 501-frame/10-second cap, playback expiration, and ghost-operated plate/gate without live-player teleporting.

PASS: same command with filter `OneMoreTime.Tests.MotorTests` and output `Artifacts/Logs/rewind-motor-regression.xml`: 20 executed, 20 passed. PASS: git diff --check. NOT RUN: manual keyboard playtesting, rendered UI inspection, Web build/browser validation. These results do not supersede the older unrelated Room 3 regression failures.

---

# 2026-11-03 Open upper boundary regression

PASS: `unity run ... -executeMethod OneMoreTime.EditorTools.MainRoomContentBuilder.RemoveMainRoomCeilings` saved all four main scenes using Unity 6000.6.0f1. PASS: `unity test ... --mode PlayMode --filter OneMoreTime.Tests.MainRoomTests` executed 2 tests, 2 passed, 0 failed. `EveryMainRoomHasOpenTopAndBoundedCamera` loaded Main_Room01, Main_Room02, Main_Room03 and Main_LiftLanding, confirmed no ceiling object, placed the goat above the visible frame, and confirmed the camera top stayed at or below `roomBounds.yMax`. `RoomOneRouteCheckpointPitAndExit` also passed. Evidence: `Artifacts/Logs/open-top-main-room-tests.xml`. NOT RUN: human playtest or Web build.

---

# 2026-11-03 Upstream PR validation

Target: cpcap1214/Goat-Game main, from W35L3YL0/Goat-Game. Includes existing commit ee84146 plus the explicit EchoFoothills debug-menu label and current documentation updates. Original tutorial/EchoFoothills scenes remain preserved.

Latest real Unity VerticalRoomTests: 7 executed, 5 PASS, 2 FAIL (Artifacts/Logs/room3-pr-tests.json). HeldKeyboardMovementSurvivesShutter failed at line 23 (expected Snapping=true). SharedPlateControlsIndependentThreeAndTwentySecondTimers expected 20 but the current serialized scene has releaseDelay=15. UpperRouteExceedsDirectSingleFreezeJumpReach PASS. Preserve the current scene state for review; do not claim all tests pass. Earlier entries describe previous validation snapshots. No full-route/manual/Web validation in this PR preparation.

---

# 2026-11-03 EchoFoothills visible in Debug Mode

Renamed the existing fifth Debug Mode entry to `05 EchoFoothills / 回聲山麓`. It continues to call SceneFlow.LoadChapter and load Chapter01_EchoFoothills. Original scene is retained in Build Settings; its F filter toggle, E/Q memory and X interaction logic remain intact. PASS: Unity recompile_status completed, no errors, compilationFailed=false. PASS: static routing/build-settings inspection and git diff --check. NOT RUN: clicking the menu in a manual playtest.

Prior Room 3 黑板 9 update is saved: high grass y=31/32, new left wood step x=3..5 at y=27.5, upper timed platform y=35 with 20 seconds, right step y=36. Lift remains y=39. Lower objects preserved (Artifacts/Logs/room3-board9-preservation.json). Initial tests: 6 PASS, 1 FAIL because the new geometry assertion read empty bounds from a disabled collider. Assertion corrected to use transform/size; compiled, but rerun pending after conversation interruption. No claim of completed full-route or shortcut playtest. Evidence: Artifacts/Logs/room3-board9-tests.json.

---

# 2026-11-03 Room 3 upper route — 黑板 8.png

Updated only the route above the y=22 timer/checkpoint terrace. Existing terrace, plate, statues, lower platforms, shuttle, pit/death threshold and entrance remain intact. One dot spacing is one unit. Added grass ledges x=2..4/y=25, x=9..11/y=25, x=5..8/y=29 and x=12..14/y=30. The existing plate controls both the relocated 3-second platform x=16..18/y=25 and new 15-second platform x=18..20/y=33. Pressure holds each at its own full time; release counts down independently. Counters remain small, centered and hidden at zero. No red lines or tips were added.

Upper right step is x=23..24/y=35; exit floor and gate plate rise to y=39; room ceiling rises to y=44. Lift dock is (31,39), retaining the user's previous +2 horizontal shift. Original cliff collider remains unchanged; a 6-unit extension adds the new top. Grass remains one-way with decorative dirt pillars. Existing upward lift transition retained.

The scoped Unity builder UpdateRoomThreeUpperSection saves only Main_Room03 and is safe to rerun; full generation shares UpperRoute. ApplyRoomThreePolish now preserves the lift height. Open Assets/_OneMoreTime/Scenes/Main_Room03.unity and press Play. Editor menu: Tools > One More Time > Update Room 3 Upper Section.

PASS: actual Unity compilation and saved scene generation. Recompile polling briefly lost transport during domain reload; scene eval and PlayMode tests subsequently executed successfully. PASS: VerticalRoomTests 6/6, including independent 3/15-second timers, shutter keyboard continuity, lower shuttle, checkpoints/death and raised lift transfer. Evidence: Artifacts/Logs/room3-upper-tests.json. PASS: serialized comparison confirms 12 existing objects and all their own components unchanged, including original cliff collider (room3-upper-preservation.json). PASS: git diff --check. NOT RUN: complete new-route traversal, manual playtest or Web rebuild. No runtime gameplay scripts changed in this update; no push/publication.

---

# 2026-11-03 Room 3 cleanup and shutter input fix

Removed Room 3's red control cables and all gameplay tip boxes (including checkpoint/drop-through/ability hints). Plate links still function. Moved the entire lift and gate/rails exactly +2 X, dock (29,33) → (31,33), leaving its plate in place. Countdown is centered over the wooden platform's visible middle, character size .13 → .065, and its text object is hidden with empty text at zero. Updated both the scene and generator; ApplyRoomThreePolish patches only the requested objects/settings without regenerating the layout.

Fixed main-game shutter movement interruption: MainRoom no longer calls the release-all-keys reset suppression during a shutter. KeyboardPlayerInput polls held movement/Space continuously while discarding action edges during the pause. Held movement resumes, direction changes/releases during the shutter are honored, and held E does not retrigger. Legacy reset/pause/death suppression remains unchanged.

PASS: Unity compile verified through recompile_status (completed, no errors, compilationFailed=false); initial polling disconnected during domain reload. PASS: real Editor ApplyRoomThreePolish saved Main_Room03 with backups. PASS: VerticalRoomTests 5/5, including Input System keyboard events through MainRoom.Update/FixedUpdate for hold/reverse/release/E-no-repeat, presentation assertions (no tips/cables, lift x=31, centered smaller hidden-zero counter), countdown, shuttle, statues/death and lift transition. Evidence Artifacts/Logs/room3-polish-tests.json. PASS: git diff --check. NOT RUN: new Web build, full-layout traversal or manual keyboard playtest.

Scene: Assets/_OneMoreTime/Scenes/Main_Room03.unity. Runtime: PlayerInput.cs, MainRoom.cs, RoomPlatform.cs. Builder: MainRoomContentBuilder.cs. No publish/push.

---

# 2026-11-03 Room 3 replaced with IMG_8884.jpg

Replaced the draft with the user's dot-grid drawing: width 32, upper cliff y=33, pit x=19..24, four grass ledges, two grayscale goat checkpoint statues, wood steps, a plate-driven vertical shuttle (top y=5↔10), a 3-second release-delay platform at y=24.5 and the plate-gated upward lift. Red control cables and pale yellow track indicate connections. Grass remains textured/one-way/drop-through with decorative, non-colliding dirt pillars. Death areas remain invisible; Room 1/2/3 thresholds are now -8.5/-4.5/-6.5 so the player falls out of view before respawning.

RoomPlatform advances on MainRoom's fixed clock. PlayerMotor2D owns all rider displacement, including rising/descending relative landings, jump-off without inherited speed and Down+Space. Plate pressure from player or grounded frozen goat holds the timed platform at 3.0; release counts down, re-press refills, zero disables collision and dims art. Shutter/pause stop both motion and countdown. Retry/death retain the copy and reset mechanism state before reevaluating pressure. Both statues illuminate green and respawn uses the newest activated checkpoint. Lift/arrival transition retained.

PASS: real Unity compile (`recompile_status`: completed, failed=false, compilationFailed=false). Initial status transport dropped during domain reload; subsequent status verified success. PASS: `unity command eval 'OneMoreTime.EditorTools.MainRoomContentBuilder.BuildRoomThreeLayout(); return "Room 3 reference layout saved";' --timeout 120000 --format json`. Scene generation uses Unity APIs and backs up managed scenes. PASS: VerticalRoomTests 4/4: shuttle both directions/jump/drop/retry, timer hold/release/recharge/shutter pause/expiry, two statues/offscreen death/respawn, final plate/lift/upward arrival. PASS: MotorTests 20/20, including legacy echo carrying, collision, jumping and one-way/drop-through. Total 24 PASS. The first MotorTests attempt discovered zero tests; requesting EditorUtility.RequestScriptReload refreshed discovery and the final 20 tests executed.

Evidence: Artifacts/Logs/room3-layout-tests.json, room3-motor-regression.json, room3-preview.png. PASS: one rendered layout review and git diff --check. NOT RUN: full traversal of the new sketch, manual keyboard playtest, Web/browser rebuild. Mechanism tests use explicit starting-position fixtures; do not treat the draft room's previous full-route success as validation of this layout. No gaps were silently shortened.

Play: Assets/_OneMoreTime/Scenes/Main_Room03.unity. Full geometry/legend in ROOM_LAYOUTS.md. New runtime files: RoomPlatform.cs, RoomCheckpoint.cs; updated MainRoom, RoomMechanisms, PlayerMotor2D and MainRoomContentBuilder. Existing opening/tutorials preserved. No publish/push. Next action is user playtesting this actual layout; the next room remains an empty lift arrival.

---

# 2026-11-03 Shared backgrounds, lift/wood art, grass blades, Room 3

Completed all five requested items, including the optional-last vertical room. Rooms 1–3 and the arrival landing share the tutorial mountain/forest artwork with vertical/horizontal scenery motion. Room 1 grass platforms use tiled grass/roots and irregular individual grass blades; collision dimensions and decorative dirt pillars are preserved. Original pixel PNGs and reusable OldMountainLift/WoodenPlatform prefabs were generated through Unity APIs.

Room 3 connects from Room 2: checkpoint at y=5, frozen-goat plate activates a y=7.5 shelf, a frozen-copy jump bridges y=10 to y=14.5, upper shelves lead to y=19.5. Final plate opens the lift entrance; a frozen goat can keep it open. Boarding latches the ride, locks movement, closes the gate and moves cabin/passenger upward 7 units before fading into Main_LiftLanding. Arrival animates upward from below and releases the goat onto an empty safe floor awaiting the next layout. Shutter is already unlocked in Room 3. Existing intro/tutorial scenes were not regenerated.

PASS: final `unity recompile --format json` returned zero errors/warnings and compilationFailed=false. PASS: `unity command eval 'OneMoreTime.EditorTools.MainRoomContentBuilder.Build(); return "Final art and rooms saved";' --timeout 120000 --format json` saved sprites, prefabs, real scenes and Build Settings. PASS: `run_tests --mode playmode --filter VerticalRoomTests --async_tests` 2/2 (full input-driven climb and upward scene transfer; plate/gate and replacement-release check). PASS: MainRoomTests 1/1, full Room 1 route/camera/checkpoint/pit/exit regression. PASS: rendered art review, git diff --check. Evidence: Artifacts/Logs/vertical-room-tests.json, main-room-art-regression.json, room3-preview.png, lift-landing-preview.png.

NOT RUN: separate ShutterTests regression attempts returned zero tests; recorded in shutter-art-regression.json, not a pass. Previous shutter tests remain historical evidence. No new Web/browser build, human keyboard/audio playtest, or Room 2 pit crossing test. Its exact 9-unit layout is retained.

An automatic approval review briefly rejected editor_stop as if it terminated Unity. Source inspection of Pipeline PlayModeCommands.cs:28–38 proved it only sets EditorApplication.isPlaying=false; retry with that evidence was approved. Editor was never terminated. No remaining approval block, remote push or publication.

Play directly: Assets/_OneMoreTime/Scenes/Main_Room03.unity. Regenerate: Tools > One More Time > Build Main Room Art and Room 3. Art: Assets/_OneMoreTime/Art/MainRooms; reusable prefabs: Assets/_OneMoreTime/Prefabs/OldMountainLift.prefab and WoodenPlatform.prefab. Detailed dimensions/design notes: ROOM_LAYOUTS.md. Next room remains an empty arrival landing pending user layout.

---

# 2026-11-02 Room 2 dot-grid layout and Shutter

Implemented Main_Room02 with 1 unit per adjacent dot: 5-unit rise, 8-unit checkpoint plateau, 9-unit invisible pit, purple pickup and gray goat checkpoint. Room 1 transitions to Room 2; opening and independent tutorials preserved. Camera width remains 20 units.

Latest user correction: every E immediately replaces the old frozen goat at the current position/pose, without resetting player, velocity, checkpoint or level. A 0.32-second dark shutter blink and click pauses room simulation; white flash removed. Frozen goat remains visible; its support marker is invisible. Death/R retain the snapshot and unlock.

PASS: `unity recompile --format json`, zero errors/warnings. PASS: `unity command eval 'OneMoreTime.EditorTools.RoomTwoBuilder.Build(); return "Room 2 updated";' --timeout 120000 --format json`; saved scene and build entry through real Editor. PASS: `unity command run_tests --mode playmode --filter ShutterTests --async_tests --format json`, 2/2. Tests verify lock/pickup, exact immediate replacement, pause position/velocity, first 5-unit rise, checkpoint, death/R persistence, invisible support and Room 1 transition. Evidence: Artifacts/Logs/shutter-tests.json, room2-generation.txt, room2-preview.png.

NOT RUN: full Room 2 pit traversal (9-unit gap and low ceiling raise a reachability concern; exact layout preserved pending user preference). Exit animation test deliberately starts on the far bank, not a full solution. NOT RUN: additional MainRoomTests filter returned zero tests; evidence room1-shutter-regression.json. NOT RUN: new Web build, browser and manual keyboard/audio playtest. No publish or push.

Open Assets/_OneMoreTime/Scenes/Main_Room02.unity and press Play, or proceed from Room 1. Details: ROOM_LAYOUTS.md. Room 3 remains unspecified.

---

# 2026-11-02 下方向鍵＋跳躍穿下草平台

KeyboardPlayerInput 新增 DownArrow＋Space 單次 drop 事件；PlayerMotor2D 僅在站立於 OneWayPlatform 時暫時忽略該 Collider、清空起跳緩衝與 coyote，不向上跳。下降仍碰撞其他平台／地面，離開原平台或 ResetAt 後恢復碰撞。不修改世界 Collider 的 enabled，避免影響其他角色。草平台上新增對應按鍵提示，README 與 ROOM_LAYOUTS 同步。

PASS：Unity CLI recompile，0編譯錯誤。PASS：Pipeline `run_tests --mode playmode --filter MotorTests --async_tests`，20/20（含穿下後落到下一平台、跳回原平台、重置恢復、實體地板與既有承載回歸）。結果 Artifacts/Logs/drop-through-motor.json。NOT RUN：Pipeline 個別方法與 InputTests 類別篩選均回傳 total=0，不視為通過；新按鍵測試已加入，保留 Artifacts/Logs/drop-through-input.json。
NOT RUN：Web 建置與人工按鍵試玩。無場景布局或資產引用變更，不需重建房間。未推送或發布。

---

# 2026-11-02 待處理布局已套用，鏡頭寬20單位

先前 automatic approval usage limit 阻擋已解除。Unity 真實 Editor 已重新生成並保存 Main_Room01，包含全部待處理布局：場寬76.4、坑寬4.4、平台5.3/7.3/9.8、出口12.4、主線跳高3、草平台由下可穿越、教學式跟隨鏡頭。最新鏡頭水平寬20單位取代先前10單位要求。

PASS：`unity run . --no-tail --log-file Artifacts/Logs/room1-resize.log -- -executeMethod OneMoreTime.EditorTools.MainRoomBuilder.Build`。PASS：`unity test . --mode PlayMode --filter 'OneMoreTime.Tests.MainRoomTests;OneMoreTime.Tests.MotorTests' --output Artifacts/Logs/room1-resize-tests.xml --format json`，19/19、0失敗。包含新版整關真正馬達通關、坑死亡／存檔、出口動畫、20單位視寬斷言、單向平台由下穿越與下降著地，以及既有角色碰撞／承載測試。

PASS：Unity 渲染入口預覽 Artifacts/Logs/room1-preview.png 與 git diff --check。新 MainRoomPlayerTuning 資產與 meta 由 Unity 生成，教學共用 PlayerTuning 保持原數值。
NOT RUN：Web重建／瀏覽器與人工整關試玩。沒有推送或發布。

---

# 2026-11-02 Room 1 尺寸／鏡頭／單向平台修訂（來源完成，場景待生成）

最新使用者修訂取代上一輪高度要求：跳高3單位、三草平台依前一層 +2.5/+2/+2.5、全場寬度2倍、鏡頭水平10單位。使用者已確認坑寬維持4.4、出口y=12.4。場寬76.4、地面第二層y=2.8、草平台y=5.3/7.3/9.8；入口／石像／提示／出口與邊界同步更新。

MainRoom 改為教學式固定步插值、前視跟隨、邊界限制與入出場鎖鏡；主線獨立 PlayerTuning（jumpSpeed=14.178182），不改教學共享資產。OneWayPlatform marker 由 PlayerMotor2D BoxCast 過濾，僅下降且腳底原在平台頂上時碰撞；普通實體地形及舊關不加此 marker。新增單向平台驗證案例，更新主線路線測試。

PASS：Python 以相同0.02秒半隱式積分計算，最高3.000000、同高度落地跑動5.28／靜止起跳5.04單位、滯空0.88秒。碰撞尺寸0.7寬×1高。PASS：git diff --check。
BLOCKED：Unity Pipeline 已找到 running Editor，但後續命令被 automatic approval review 以 usage limit 拒絕；沒有繞過拒絕。最新場景生成、Unity編譯／PlayMode／渲染全部 NOT RUN。現有 Main_Room01.unity 尚為前一次布局，不宣稱已更新。待工具可用或使用者在 Editor 執行 Tools > One More Time > Build Main Room 1，再跑 MainRoomTests 與 OneWayPlatformPassesUpwardAndSupportsDescendingFeet。未推送／未發布。

---

# 2026-11-02 隱藏坑底死亡區

依使用者要求，從 Room 1 生成器移除 Pit danger 紅色條與 Pit ember 裝飾。MainRoom 的 deathY 與死亡／檢查點重生邏輯不變。

PASS：Unity CLI 真實 Editor 重新生成與保存；場景不再包含 Pit danger／Pit ember，deathY 仍為 -3.1。命令：`unity run . --no-tail --log-file Artifacts/Logs/room1-invisible-pit.log -- -executeMethod OneMoreTime.EditorTools.MainRoomBuilder.Build`。PASS：git diff --check。NOT RUN：此次純視覺移除，未重跑玩法測試或 Web 建置。

---

# 2026-11-02 檢查點石像改為灰階玩家

依使用者要求，Room 1 石像改用現有玩家 idle 第 0 幀、同一縮放與腳底 pivot，移除積木造型與底座。專用 CheckpointGrayscale shader 即時轉灰階，原玩家圖檔不變。綠眼仍由既有檢查點狀態控制，位置對齊 idle sprite 的眼睛。

PASS：Unity CLI 真實 Editor 生成／編譯與 RenderPipeline 渲染，目視 room1-preview.png 確認石像為玩家灰階圖。命令：`unity run . --no-tail --log-file Artifacts/Logs/room1-statue.log -- -executeMethod OneMoreTime.EditorTools.MainRoomBuilder.Build`。PASS：git diff --check。NOT RUN：此次僅視覺替換，未重跑關卡路線或 Web 建置。

---

# 2026-11-02 主線 Room 1：使用者手繪布局

已實作 `Assets/_OneMoreTime/Scenes/Main_Room01.unity`，開始旅程保留攝影大賽開場，完成／略過後接入新房間。Tutorials 五個既有關卡保持獨立且可進入。

Room 1：固定全房鏡頭／等比留邊、淡入與自動走入、A/D／方向鍵移動提示、Space 跳躍提示、山羊石像接近存檔與綠眼、落坑死亡回檢查點、三個草平台與無碰撞背景土柱、上層出口走出鏡頭並淡出。沿用原 PlayerMotor2D 與數值，沒有改記憶玩法。Room 2 未指定，淡出後顯示 To be continued；`nextRoomScene` 留空，供之後銜接。

生成器 `Assets/_OneMoreTime/Editor/MainRoomBuilder.cs` 使用 Unity API 保存真正 Scene／meta，僅管理 Room 1 Managed 根物件。新房間使用獨立 unlit sprite 材質，修正預設 lit 材質預覽出現錯誤貼圖的問題；沒有改使用者原 URP 設定。Build Settings 加入遊戲入口、教學及 Room 1，保留 SampleScene；WebBuilder 也包含新房間。

PASS：Unity CLI `unity run . --no-tail --log-file Artifacts/Logs/room1-build.log -- -executeMethod OneMoreTime.EditorTools.MainRoomBuilder.Build` 真實 Editor 編譯／生成。
PASS：`unity test . --mode PlayMode --filter 'RoomOneRouteCheckpointPitAndExit;MainJourneyKeepsIntroAndLoadsRoomOne;TutorialMenuLoadsAllFiveExistingLevels' --output Artifacts/Logs/room1-tests.xml --format json`，3/3。含實際 motor 輸入走完整路線、綠眼、落坑重生、淡出走出邊界，無傳送到終點；主選單開場接 Room 1、五個教學入口也通過。最終材質修正後重做生成／渲染，未重複無關機關測試。
PASS：目視 `Artifacts/Logs/room1-preview.png`（Unity RenderPipeline 渲染，非手繪模擬）。PASS：`git diff --check`。

初次生成因 batch 啟動的未命名場景不能加法建場而失敗，已限定 batch 模式先開 MainMenu 解決。初次路線測試從中層平台正下方起跳被頂住，改為由最低平台右側跳上後正常通過；沒有為測試降低碰撞或更改角色能力。
NOT RUN：人工鍵盤整關試玩、遊戲 HUD 視覺檢查、Web 建置／瀏覽器。此次依使用者要求僅進行必要路線驗證，不做完整舊關回歸。

標記 0–6 的後續共用規則及配置見 `Docs/Codex/ROOM_LAYOUTS.md`。下一步等待 Room 2 布局。未推送或發布。

---

# 2026-11-02 教學區與主線分離

使用者要求現有關卡移至主選單獨立教學區；新主線暫留空，等待後續布局指示。MainMenu 新增「教學關卡 / Tutorials」，可個別進入 L01–L04 與 Chapter01_EchoFoothills。原場景路徑、生成器、機關及關卡串接保留；沒有搬動資產或重建關卡。

「開始旅程」保留完整攝影大賽 OpeningCinematic。自然播完、逐句完成或略過後，顯示無關卡的主線等待畫面，可按 Esc／按鈕返回主選單；不再自動載入回聲山麓。等待畫面使用 MenuScreen 狀態，沒有新建主線關卡或擅自設計布局。主選單手冊顯示時停用背後按鈕，避免點穿。

PASS：Unity CLI 真實 Editor 編譯與 PlayMode 2/2（`unity test . --mode PlayMode --filter 'MainJourneyKeepsIntroAndEndsWithoutLoadingATutorial;TutorialMenuLoadsAllFiveExistingLevels' --output Artifacts/Logs/tutorial-menu.xml --format json`）。驗證開場建立、主線等待畫面無場景跳轉、回選單可重新播放，以及五個教學選項各自載入原場景與相應控制器、最後返回 MainMenu。PASS：`git diff --check`；原五個關卡及開場腳本無修改。

先前誤把新流程測試放在 EditMode，因執行期 cinematic 的 Destroy 呼叫而 FAIL；已將該測試移到 PlayMode 後通過，沒有更動開場內容。該次既有四項 OpeningTests（27秒、逐句推進、略過、素材）全部 PASS；歷史失敗報告 Artifacts/Logs/tutorial-opening.xml 保留。

NOT RUN：本次未手動目視新版選單、未重跑完整機關回歸或 Web 建置。互動 Editor 的 Pipeline server 未連上；改用 Unity CLI batch 真實 Editor 執行測試成功，不將其當作人工試玩。下一步：等待使用者提供主線布局。沒有推送或發布。

---

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

## 2026-11-17 — Forest and Grand Waterfall art
- PASS — Unity real Editor asset generation (`unity command eval 'OneMoreTime.EditorTools.WorldOneArtBuilder.Build(); ...'`); PNG imports, sprites, TileBase assets, prefabs and gallery scene saved with Unity APIs.
- PASS — `unity command run_tests --mode playmode --filter WorldOneArtTests --async_tests --format json`: 2/2 passed, 0 failed (0.23 s reported by runner).
  - ArtLoopsBackgroundAndCheckpoint: 4 frames per tile, transparent water air, drowning progression/final frame, stone activation/idle, background covers view at low/middle/high elevations.
  - DeepWaterDrownsThenRespawnsWithoutReload: live sprite hidden during sequence, one death, restored at start, same scene remains loaded.
- PASS — camera-render inspection for water transparency, background, goat/stone relative size, drowning frame; screenshots in `Artifacts/Logs/worldone-*.png` (local evidence, not runtime assets).
- Initial compile caught unsupported NUnit `Is.AnyOf`; replaced with supported assertion and reran successfully. CLI recompile wrapper dropped its connection during domain reload; subsequent Editor eval/test execution confirmed compilation rather than assuming wrapper success.
- Initial water sheet had opaque white air; regenerated transparency with imagegen and verified alpha=0 in Unity. Impact foam adjusted to meet waterfall body in final prefab.
- NOT RUN — full game regression, standalone player build, and human playthrough of a new World 1 level. This task delivers reusable art and a debug gallery; no new room layout was requested.

### 2026-11-17 — Redrawn drowning sequence
- PASS: revised sprite sheet imported and existing six Sprite assets updated through Unity `WorldOneArtBuilder.BuildDrowning()`; frame pivots measured from opaque turquoise surface rows.
- PASS: inspected actual camera render of all six frames, including the three opening poses; horizontal silhouette, hidden submerged limbs, aligned surface. Evidence: `Artifacts/Logs/drowning-redraw.png`.
- PASS: `unity command run_tests --mode playmode --filter WorldOneArtTests --async_tests --format json`, 2/2 passed, 0 failed, runner duration 0.3 s. Sequence completion and same-room drowning respawn preserved.

### 2026-11-17 — Looping background
- PASS: Unity builder saved updated background sprite/material/prefab/gallery, shader supported by Editor renderer.
- PASS: `unity command run_tests --mode playmode --filter WorldOneArtTests --async_tests --format json`: 3 passed / 0 failed, duration 0.35s. Checks full-repeat phase equivalence on both axes, negative/high travel (-1000 to 2000), camera coverage and existing animation/respawn behaviors.
- PASS: actual camera render inspected with both wrap boundaries centered (`Artifacts/Logs/background-loop-wrap.png`). Edge overlap avoids a hard image cut; overlapping scenery has a soft mist-like blend in the joining strips.
- NOT RUN: standalone build; extended human gameplay session.

### 2026-11-17 — Art preview navigation
PASS: real Unity compilation and saved preview camera orthographic size 7. Arrow/WASD movement uses existing preview Update input path on both axes. No new automated tests added for this small preview-only adjustment; manual keyboard interaction not verified.

### 2026-11-17 — Water loop revision
- PASS: real Unity compilation and builder import of replacement atlas/material/sideways tiles/prefab/gallery.
- PASS: WorldOneArtTests 3/3, 0 failed, duration 0.38s. Expanded checks cover Looping Water shader support, 4 poses/second, shared source frames and 90-degree side-flow orientation, no tile collision.
- PASS: actual camera render checked for vertical/horizontal waterfall assemblies, transparency and preview layout (`Artifacts/Logs/water-loop-redraw.png`).
- Temporal smoothing uses shader interpolation around all four transitions; frame count remains four, cycle is now one second. Snapshot inspection is not a human review of motion quality. No standalone player build run.

### 2026-11-18 — Background zoom
PASS: original forest scale verified from PresentationBuilder (176px over 10 units). Unity API saved loop dimensions 62.6999969 and returned target density 17.6px/unit. Actual camera render inspected (`Artifacts/Logs/background-pixel-scale.png`). No new automated tests for this visual configuration change; full regression not rerun. Automatic approval initially misclassified editor_stop as terminating Unity; read-only inspection showed Editor was already in Edit mode, so no stop action was needed.

## 2026-11-18 Wooden suspension bridge

Created transparent bridge art and separate BridgeLeftEnd, BridgeMiddle and BridgeRightEnd sprites. WoodenSuspensionBridge prefab uses a tiled middle and adjustable middleTiles (default 2, total width 8 units). Deck collision follows the configured width; ropes/posts are visual only. Uses the existing OneWayPlatform convention for jumping through and dropping down. Added beneath the goat/statue in Debug_WorldOneArt. Existing main-room layouts unchanged.

PASS: real Unity compilation/build via WorldOneArtBuilder.Build; prefab resize to 12 units and surface/one-way marker assertions; alpha corner zero; rendered preview inspected (Artifacts/Logs/wooden-bridge.png); git diff --check. Recompile wrapper disconnected during domain reload, subsequent compiled builder executed successfully. NOT RUN: manual goat traversal.

## 2026-11-18 Inward tapered grass/dirt ledge
Created GrassDirtLedge.png with a broad grassy walking surface and inverted triangular soil underside, referencing the current Sunny Land palette. Added standalone sprite, decorative Tile asset (no tile collider), and 4-unit-wide one-way platform prefab; origin is the grass surface. Added preview below the bridge. PASS: Unity compilation zero errors/warnings, WorldOneArtBuilder.Build, prefab width/one-way/alpha assertions, actual camera screenshot inspection (Artifacts/Logs/grass-ledge.png). NOT RUN: manual traversal.

## 2026-11-18 Pixel density and forest palette correction
Replaced the oversized blurry background with ForestWaterfallPixel.png, then revised its excessive cyan cast to muted green foliage, earth-gray cliffs and blue-white water per user feedback. Added Terrain Pixel Scale shader/materials for World One sprites at 16 effective pixels/world unit, matching original terrain; water uses 16×16 sampling per tile. Background samples a 384-cell grid at 16 pixels/unit, 21.12-unit repeat including edge overlap, instead of 62.7. Preserves source textures and platform geometry. Raised importer max size to 4096 to avoid rounding unequal bridge thirds.
PASS: Unity compilation (0 errors/warnings), real asset rebuild, WorldOneArtTests 3/3 with new pixel-density assertions, loop/animation/drowning regression, git diff --check, final green-palette Unity render inspection (Artifacts/Logs/pixel-scale-green.png). Initial density test caught bridge import downscaling and passed after fixing import dimensions. Palette-only final edit imported and visually verified after the passing suite. NOT RUN: full human playthrough or standalone build. Built-in imagegen prompts: PIXEL_SCALE_BACKGROUND_PROMPT.md and FOREST_PALETTE_PROMPT.md.

## 2026-11-18 Restore Tuesday background selection
User selected the background completed Tuesday around 02:26. Restored ForestWaterfallLoop.png (generated Nov 17 02:23:45, copied into project 02:24:36) in the builder, shared background sprite and prefab/gallery. Retained the corrected 16-pixel rendering grid, 21.12-unit repeat and overlapping-edge loop shader. Later background image retained as an unused alternative. PASS: Unity compilation zero errors/warnings; asset rebuild and assertion of ForestWaterfallLoop texture reference; git diff --check. No new loop or gameplay test run for this texture selection change; prior 3/3 suite passed.

## 2026-11-18 Wide background extension
Created ForestWaterfallWide.png via built-in imagegen outpainting: 3:1 panorama with new scenery on both sides of the selected Tuesday composition. Original square retained. Builder/prefab/gallery use new texture; horizontal repeat 63.36 units, vertical 21.12. Aspect-correct shader grid 1152×384 maintains prior pixel scale, with both-axis overlap wrapping. Generated center is composition-preserving, not pixel-identical. PASS: Unity compilation zero errors/warnings, asset rebuild, WorldOneArtTests 3/3 (including 3:1 texture/world aspect and full-cycle wrap/negative travel), source visual inspection, git diff --check. NOT RUN: human seam inspection during scrolling or standalone build. Prompt: WIDE_BACKGROUND_PROMPT.md.

## 2026-11-18 Room 4 implementation and validation

Implemented Main_Room04 from the supplied layout: existing Room 3 lift connection, basic replay pickup and anti-bypass wall, segmented bridge destruction, gravity fall and camera zoom, suspended tutorial waiting for actual 0.5-second Q recording activation, water/platform route, rewindable floating crate pool, and room completion. Existing movement tuning and Room 1–3 layouts are preserved. Full file inventory, reused assets, Inspector defaults, persistence, assumptions and rebuild instructions: [ROOM_FOUR.md](ROOM_FOUR.md). No manual scene setup required; Main Menu → Debug Mode → 09 Room 4.

PASS — real Unity compilation, zero errors/warnings; RoomFourTests 5/5; full PlayMode suite 98/99; entrance/fall/composited tutorial screenshot inspection. Tests cover real recording activation, camera restoration, natural landing, unchanged-jump traversal through the crate route, buoyancy/carry, spawn/despawn rewind, bounded pool, reload/new-journey semantics and lift wiring.
FAIL (pre-existing, outside Room 4 scope) — VerticalRoomTests.SharedPlateControlsIndependentThreeAndTwentySecondTimers expects 20 seconds, but the unchanged Room 3 scene has 15 seconds. Confirmed the same 15-second serialized value in HEAD; left unchanged to preserve Room 3.
NOT RUN — standalone build and full human playthrough. Automated traversal is not a substitute for human difficulty testing. Unlock persistence is session/journey only; no disk save system was introduced. A stationary rescue crate supports the first recording handoff, and Room 4 ends with a placeholder completion screen because Room 5 is undefined.

Commands: `unity recompile --format json`; `unity command run_tests --mode playmode --filter OneMoreTime.Tests.RoomFourTests --async_tests --format json`; `unity command run_tests --mode playmode --filter OneMoreTime.Tests --async_tests --format json`; `unity command test_status --format json`. Two unfiltered discovery attempts returned zero tests and were not counted as passes. Evidence: `Artifacts/Logs/room4-full-playmode-final.json` (99 total, 98 passed), `room4-entrance.png`, `room4-tutorial.png`, `room4-tutorial-ui.png`. Earlier initial suite results are retained separately. All changes are local; nothing pushed or published.

## 2026-11-18 Room 4 visual / checkpoint / slow-motion revision

Updated the existing scene in place, not a rebuild. Added the existing stone-goat checkpoint at (26,4), three units before the *6 bank edge as clarified by the user. Added decorative soil columns and full-depth water using reusable atlas-sliced assets and Water Volume shader; wall ledges now extend horizontally from their banks. Removed the first-pit rescue crate and gated later crate spawning until actual recording activation. Near-water slowdown eases to 0.12 over 0.5 s, freezes at approximately 0.08 units above water, and restores speed in 0.45 s after the real 0.5-second Q hold. Global time and movement tuning are unchanged. Existing checkpoint save/green-eye/crack animation and retry semantics are reused.

PASS: real Unity batch compilation and targeted scene patch; final RoomFourTests **8/8**; rendered overview inspected with no gap between water surface and body; git diff --check. Room 3 scene diff is identical to its pre-update state. Earlier two new-test failures (late slowdown onset and owned-entry speed ramp) were fixed before the final run.

Full PlayMode regression: **97/102 PASS, 5 FAIL**. One is the previously documented Room 3 15-versus-20-second timer mismatch. Four are keyboard-edge tests: ChapterOneTests.KeyboardTutorialSimultaneousJumpFilterAndPause; InputTests.DownJumpIsConsumedOnce; InputTests.HeldKeysAndResetSuppression; VerticalRoomTests.HeldKeyboardMovementSurvivesShutter. The two unchanged InputTests also fail in an isolated batch run without loading Room 4 (0/2); synthetic held state is visible but expected pressed-frame edges are not. This indicates a batch input-fixture issue, but its exact cause and interactive verification remain unresolved. Unrelated input mechanics/tests were not changed to mask these failures.

NOT RUN: interactive keyboard playthrough, human difficulty assessment, standalone build. Interactive Editor launch exited without establishing Pipeline; validation used the installed Unity 6000.6.0f1 through unity-cli batch commands instead. No claim of live interactive validation. Source compilation succeeded; existing obsolete-API warnings/bridge OnValidate warnings were observed during initial imports.

Commands: `unity run . --log-file /tmp/room4-update-final-editor.log --no-tail --format json -- -executeMethod OneMoreTime.EditorTools.RoomFourUpdate.ApplyAndCaptureReview`; `unity test . --mode PlayMode --filter OneMoreTime.Tests.RoomFourTests --output Artifacts/Logs/room4-update-final-tests.xml --format json`; full suite uses `--filter OneMoreTime.Tests`; isolated keyboard check uses `--filter OneMoreTime.Tests.InputTests`.

Evidence: Artifacts/Logs/room4-update-final-tests.xml, room4-update-full-tests.xml, room4-update-input-isolation.xml, room4-update-overview.png, room4-update-final-editor.log. Exact file/script/asset/scene-object inventory and tuning: [ROOM_FOUR.md](ROOM_FOUR.md). No prefab files or source PNGs changed in this revision. No manual setup needed; open Main_Room04 or Main Menu → Debug Mode → 09 Room 4. All changes local.

## 2026-11-18 Room 4 latest sketch correction
Removed unrequested pit/wall stepping ledges; only the two drawn bank bevels remain, using triangular sprites sampled from the existing dirt/grass tiles and bank polygon outlines. Rebuilt grassy-platform visuals using the actual older main-room grass-cap and muted decorative-pillar code. Added the wooden lift landing/open shaft. Upper stream extended to 24 units (x40–64), with synchronized surface/fill/hazard/current/source. Crates drift left at 1 unit/s and spawn every 8 seconds, limit six. See ROOM_FOUR.md latest section for coordinates and exact changed-file inventory.

PASS: real interactive Unity/Pipeline compilation (0 errors/warnings), in-place patch and repeated patch without duplicate scene objects, rendered overview inspection, RoomFourTests 9/9, git diff --check. A first traversal test identified an unreachable wooden-step edge; moving that existing step half a unit left to x39.5…42.5 resolved it without extra platforms or movement changes. Dedicated test verifies displacement of one unit in one simulated second and second spawn at eight seconds. Other focused tests verify statue/retry, tutorial slowdown/hold, platform access, crate rewind/carry/disposal, and lift wiring. NOT RUN: full human replay-puzzle completion, full regression rerun and standalone build; previous full-suite limitations remain documented above.

Evidence: Artifacts/Logs/room4-layout-tests.json, room4-layout-overview.png. Commands: unity recompile; unity command eval invoking RoomFourUpdate.ApplyAndCaptureReview; unity command run_tests --mode playmode --filter OneMoreTime.Tests.RoomFourTests --async_tests. User's running Editor was preserved; Play mode was exited for edits, then the saved Room 4 scene reopened. No remote push.

## 2026-11-18 Water remake / 0.25-second hold
PASS — `unity recompile --format json`: zero errors/warnings. `unity command run_tests --mode playmode --filter OneMoreTime.Tests.RewindPlaytestTests --async_tests --format json`: 10/10. `WorldOneArtTests` with the same command: 4/4. New GPU render check verifies animated changes, 0.0/1.0-second image equality for all three water shaders, and transparent air/opaque water across the diagonal. Shared frame references guarantee the diagonal's palette/animation phases derive from the river. Existing drowning, statue and scrolling-background tests also pass. Screenshot: water-remake-preview.png.

Initial submerged-water animation check failed; sampling band was corrected and the suite then passed. Several subsequent runner calls reported zero tests; those are NOT passes. Test discovery recovered after requesting script compilation. No standalone build, full gameplay regression or human playthrough claimed. Exact result JSONs: Artifacts/Logs/water-rewind-tests.json and water-art-tests.json. `git diff --check` passed.

Final RoomFourTests: PASS 9/9, including the 0.25-second tutorial handoff, crate cadence, checkpoint/retry and existing route. Evidence: Artifacts/Logs/water-room4-tests.json.

## 2026-11-18 Imaginary cutscene rewind / background variants
PASS — unity recompile: zero errors/warnings. RoomFourTests 9/9 via real Editor/Pipeline, including actual Q threshold, no handoff teleport, final safe-bank position, synthetic trace endpoints, bridge remains broken, crate activation, statue behind player and existing Room 4 behavior. Results: Artifacts/Logs/room4-imaginary-rewind-tests.json.
PASS — Editor asset assertions: 41 background prefabs have no Collider2D/MonoBehaviour and renderer order -20. All generated background tiles have no collider and dark tint; original water animation data is delegated. Initial required-component removal issue on bridges corrected before final validation. Evidence: background-variant-validation.txt. git diff --check passed. NOT RUN: full human visual/timing playtest, full suite and standalone build.

## 2026-11-18 Flow / natural acquisition path / object traces
PASS — RoomFourTests 9/9: crate displacement 2 units in one simulated second, second crate after 10 seconds, acquisition trace passes pickupFeet at deck height, exact original start/end snapshots, bank return and existing Room 4 regressions. PASS — RewindPlaytestTests 10/10: dashed tiled object line renderer with repeating texture, lower alpha/narrower width than player path; world restore/opt-out, replay direction and input behavior preserved. Commands: `unity command run_tests --mode playmode --filter OneMoreTime.Tests.RoomFourTests --async_tests --format json` and same with RewindPlaytestTests. Evidence: room4-flow-natural-rewind-tests.json, rewind-object-trace-tests.json in Artifacts/Logs.

Initial curved-path test found sub-pixel endpoint roundoff; endpoint snapshots now explicitly retain their exact coordinates. Empty combined/second-suite runs were discarded; requested script compilation to recover discovery. Unity compilation passed; occasional status RPC disconnects during domain reload were followed by successful compiled tests. git diff --check PASS. NOT RUN: human visual playthrough, full regression and standalone build.

## 2026-11-19 Crates participate in replay, no tint
PASS — RoomFourTests 11/11, RewindPlaytestTests 10/10 through real Editor/Pipeline. New tests record across crate despawn/spawn, verify exact end/start states under forward/backward playback, held playback freezes crate positions, moving replay crates carry the player by their per-tick displacement, automatic rewind visibly reverses their movement, and crate sprite colors stay unchanged. Existing replay control and opt-out regressions pass. Commands: `unity command run_tests --mode playmode --filter OneMoreTime.Tests.RoomFourTests --async_tests --format json`, then same with RewindPlaytestTests. Script compilation requested between suites to refresh discovery. Evidence: Artifacts/Logs/crate-replay-room4-tests.json and crate-replay-controls-tests.json.
PASS — Unity compilation confirmed up to date with no errors; git diff --check. A recompile-status request disconnected during domain reload; subsequent Editor verification and compiled tests succeeded. NOT RUN — full human playthrough, full regression suite and standalone build.

## 2026-11-19 Forest expansion validation

- PASS — Unity 6000.6.0f1/Pipeline compilation: 0 errors and 0 warnings after adapting the migration validator to GetEntityId and EditorJsonUtility. The user's Editor process was preserved.
- PASS — GrassyPlatformBuilder.Validate and ForestExpansionBuilder.Validate: native 16-pixel rectangular terrain, one-way top/background collision invariants, 32 props, 32 dark variants and 64 decorative Tile assets. Evidence: Artifacts/Logs/forest-expansion-validation.txt.
- PASS — Main Rooms 1–4 migration: 3/0/8/3 grass columns respectively; collider counts 11/7/23/25 and serialized data unchanged. Evidence: Artifacts/Logs/grassy-column-migration.txt. Room 2 was left unchanged.
- PASS — WaterFeatureTests 3/3. GPU checks verify that all five shapes animate and close the one-second loop, foreground opacity stays at or below 38%, and a known palette swatch renders correctly. Also checks prefab layers/collision and fixed accent bounds. Evidence: Artifacts/Logs/water-expansion-tests.json. An initial frame-alignment failure caused by the 1254-pixel sheet not dividing evenly into quarters was fixed with equal frame rectangles.
- PASS — FloatingTimberTests 3/3: all four platforms/source pools/backgrounds, actual raft carrying/drop-through, and log automatic rewind/forward/reverse/held playback without tint. Evidence: Artifacts/Logs/floating-timber-tests.json. An initial width assertion failed on floating-point rounding (2.69999981); fixed its tolerance without changing gameplay dimensions.
- Visual review found overly dark water caused by sRGB/linear palette comparison. Corrected the shared shader and added a GPU color assertion. The new grand-waterfall atlas supplies broader light bands. Initial WaterFeature OnValidate warnings were fixed by deferring refresh.
- Commands: `unity recompile --format json`; `unity command eval` invoking the four builders and validators; `unity command run_tests --mode playmode --filter OneMoreTime.Tests.WaterFeatureTests --async_tests --format json` (also FloatingTimberTests). A large initial build exceeded the RPC's five-second timeout but completed; explicit validators checked the resulting assets. A recompile status call disconnected during domain reload; subsequent compilation and test results establish the actual status.
- NOT RUN: standalone build, new-room human playthrough (Rooms 5–7 are not created), and the full unrelated regression suite.

Final regression: PASS WorldOneArtTests **4/4**, preserving original water/background loops, pixel scale, drowning and checkpoint activation; evidence: Artifacts/Logs/forest-expansion-art-regression.json. PASS RoomFourTests **11/11**, including unchanged traversal, water/current/crate timing, replay and the new grass hierarchy; evidence: Artifacts/Logs/forest-expansion-room4-tests.json. Total **21/21** targeted tests passed. Repeated GrassyPlatformBuilder.ApplyToMainRooms confirmed the same 14 columns and unchanged collider serialization; Room 2 has no diff. Final real Unity renders inspected at Artifacts/Logs/forest-expansion-1.png through -5.png, including the bright giant waterfall, transparent foreground streams, rectangular grass columns, props and timber. `git diff --check` passed. The art gallery was reopened after tests.

## 2026-11-19 Main-room expansion decoration

- PASS: real Unity 6000.6.0f1/Pipeline compilation of the new Editor decorator (recompile reported zero errors/warnings). One intermediate recompile-status RPC disconnected during domain reload; subsequent compilation and executed checks completed normally.
- PASS: art-only scene migration and repeat application. Rooms 1–4 have 29 / 22 / 28 / 33 managed decoration groups. All 1,008 pre-existing component/transform records remained unchanged. The new layer contains only Transform, SpriteRenderer, and visual-only WaterFeature components: no additional collision, hazards, input, plates, platforms, or rewind objects.
- PASS: normal 20-unit-width Unity camera renders reviewed for all four rooms. Captures isolate each room from other loaded scenes; initial additive captures that included the art gallery were discarded and replaced. Review moved Room 3 braces inside the cliff, removed Room 4's misleading beam/unanchored cascade, and aligned eddies with the water surface.
- PASS: existing MainRoomTests 2/2, including route/checkpoint/pit/exit and bounded cameras/open room tops. Evidence: `Artifacts/Logs/main-room-decoration-playmode.json`.
- PASS: existing RoomFourTests 11/11, including crate forward/reverse/held playback, visible rewind, spawn cadence, water/platform traversal, acquisition, checkpoints and lift wiring. Evidence: `Artifacts/Logs/main-room-decoration-room4-tests.json`. Total **13/13 targeted PlayMode tests**.
- NOT RUN: complete human playthrough, full unrelated regression suite, standalone build. No new level or difficulty balance is claimed.

Commands: `unity recompile --format json`; `unity command eval 'OneMoreTime.EditorTools.MainRoomDecorationBuilder.Build(); return "done";' --format json`; per-room `MainRoomDecorationBuilder.Capture(1)` through `(4)`; `unity command run_tests --mode playmode --filter OneMoreTime.Tests.MainRoomTests --async_tests --format json`; same command with `OneMoreTime.Tests.RoomFourTests`.

Preservation log: `Artifacts/Logs/main-room-decoration-validation.txt`. Render files: `Artifacts/Logs/main-room-1-decorated-1.png` through `main-room-4-decorated-3.png` (Rooms 1/2 have two views, Rooms 3/4 three). Usage and ownership: [MAIN_ROOM_DECORATION.md](MAIN_ROOM_DECORATION.md).


## 2026-11-19 Rooms 5–8 implementation

- **PASS — real Unity compilation:** 6000.6.0f1, running Editor/Pipeline port 7800; final compilation has zero errors/warnings. Initial `title`/`roomTitle` integration error was corrected before building.
- **PASS — scene generation:** `ChapterOneExpansionBuilder.BuildRoom(5..8)` creates scene assets and enabled build entries; core refs, checkpoints, supported sprite materials, missing scripts and non-colliding scenery validated. Repeated generation replaces the one managed root; Room 4 only receives its destination reference.
- **PASS — visual review:** overview and normal 20-unit-camera renders in all four rooms. Moved Room 6 framing waterfalls inward after the first capture placed them outside the gameplay view. Uses the existing forest/water/wood/grass art. Room 8's cavern and foreground curtain are visible environmental cues for the next area.
- **PASS — ChapterOneRoomsTests 9/9:** one simulation clock, bounded cameras/open tops, era collision/safe switching/rewind restoration and unchanged art colors; checkpoint/retry retains memory; plates work with shutter and replay; floating timber forward/backward/held playback and later-upgrade persistence; actual approaching-log boarding, upstream reverse carry and final bank jump; collapse timer/debris/collision restoration; finale does not try to load a nonexistent Chapter 2 scene.
- **PASS — ChapterClimbTraversalTests 2/2:** actual E placement and jump onto Room 6's plate-driven shuttle, carried ascent, arrival-beam jump; tight east-to-west normal jump. Tests reposition only to the relevant starting dry platform and use `ChapterOneRoom.Step` thereafter.
- **PASS — RoomFourTests 12/12:** prior water/acquisition/crate/checkpoint regressions and real exit walk/fade/load into Room 5 with preserved unlock progress. Pause freezes transition movement and the pause menu draws above the transition fade.
- **PASS — source initialization review:** saved scene pools remain empty; first live tick spawns a single item. Preview-only seeding avoids a duplicated initial log caused by saving an active pool item without its runtime spawn timer.
- **PASS — whitespace:** `git diff --check`.
- **NOT RUN:** complete human playthrough of all four new routes, standalone build, full unrelated regression suite. Existing historical failures above were not changed or claimed fixed.

Total **23/23 targeted tests**. Evidence: `Artifacts/Logs/chapter1-rooms-playmode.json`, `chapter1-climb-traversal-tests.json`, `chapter1-room4-transition-tests.json`. Initial 8/8 suite retained separately as `chapter1-new-rooms-tests-initial.json`; it is not counted twice. One climb-suite discovery returned zero tests, was discarded, and rerun after `AssetDatabase.Refresh`/script compilation. The refresh RPC timed out during reload; the subsequent actual test run completed normally.

Commands: `unity recompile --format json`; `unity command eval` invoking `ChapterOneExpansionBuilder.BuildRoom`, `ConnectRoomFour`, and `Capture`; `unity command run_tests --mode playmode --filter OneMoreTime.Tests.ChapterOneRoomsTests --async_tests --format json`, with separate filters for `ChapterClimbTraversalTests` and `RoomFourTests`. Review renders: `Artifacts/Logs/chapter1-room5-overview.png` through `chapter1-room8-view4.png`. New scene paths, provisional layouts and regeneration ownership: [ROOMS_5_TO_8.md](ROOMS_5_TO_8.md).

Final scene audit: PASS, exactly one managed root in each room, all four scenes enabled in build settings, and saved floating pools empty. Evidence: `Artifacts/Logs/chapter1-scene-validation.txt`. The multi-scene audit exceeded the five-second RPC response timeout but wrote the full validation file and finished opening Room 5; a separate read-only state check confirmed completion.

## 2026-11-19 Replay puzzle design library

- PASS — source audit of recording/restart/reverse/hold, endpoint support, plate eligibility, shared floating-object playback, live platform timers, manual era switching and checkpoint handling. Source references are linked in [REPLAY_PUZZLE_LIBRARY.md](REPLAY_PUZZLE_LIBRARY.md).
- PASS — twelve unique design cards P01–P12, each with layout, recording target, solution, recovery and implementation boundary. Nine use existing mechanism types; P09/P10/P12 explicitly require new logic. Single memory, 0.25-second hold, ten-second recording limit and later reverse upgrade are retained.
- PASS — local document links, card inventory and whitespace checks. These checks validate documentation, not Unity behavior or puzzle solvability.
- NOT RUN — Unity compilation, scene generation, automated traversal, human difficulty/bypass testing or standalone build for these designs. No executable changes were made in this design task. Prior room test results do not validate these twelve proposed layouts.

Review explicitly considers permanent endpoints, repeated Q, E staircase alternatives, recorded object lifetimes and the fact that normal machinery keeps running during held playback. Proposed dimensions and schedules remain prototype targets, not measured or proven solutions.


## 2026-11-19 Keyboard settings

- **PASS — real Editor compilation:** Unity 6000.6.0f1/Input System 1.20.0, running Editor/Pipeline port 7800. Final recompile reports zero errors/warnings. An initial new-file source-registration issue was resolved by moving the plain C# menu class to `ControlsMenu.cs` through AssetDatabase; its real Unity-generated metadata was preserved.
- **PASS — EditMode suite 35/35:** all existing EditMode tests plus four prompt-formatting tests and three key-capture/menu tests. Covers both presets, Chinese prompts, preserved plate labels, non-cascading custom swaps, Esc capture cancellation, reserved keys, and capture save/close behavior.
- **PASS — KeyBindingTests 11/11 (PlayMode):** both presets, configurable keyboard reads, persistence/reload, conflict swapping, invalid preferences, reserved keys, obsolete key isolation, and suppression/release behavior across menus/entry animations. The real Debug_Rewind scene integration drives custom Tab/Right Ctrl keys through controller Update/FixedUpdate: 0.24 s does not record, 0.26 s records, release continues recording, a short press saves, multi-tick visible rewind finishes, replay starts, held playback freezes its cursor/echo while live machinery continues, and release resumes it.
- **PASS — InputTests 2/2 (PlayMode):** one-shot Down+Jump consumption and held-input/reset suppression.
- **PASS — legacy keyboard regressions 2/2 (PlayMode):** `ChapterOneTests.KeyboardTutorialSimultaneousJumpFilterAndPause` and `VerticalRoomTests.HeldKeyboardMovementSurvivesShutter` use the selected bindings and isolated keyboard fixtures.
- **PASS — rendered UI review:** actual MainMenu and keybinding overlay drawn at 1280×720. Both preset buttons, all action labels, current bindings and Back fit the existing game UI. The overlay was opened through its runtime API for the capture; capture/cancellation/conflict logic was tested automatically, not claimed as a manual mouse playthrough.
- **PASS — whitespace:** `git diff --check`.
- **NOT RUN:** standalone build, full human playthrough of every room, or the entire unrelated PlayMode suite.

Final total: **50/50 tests**, counted once. Evidence: `Artifacts/Logs/keybindings-editor-tests.json` (35), `keybindings-input-tests.json` (11), `keybindings-adapter-tests.json` (2), `keybindings-room-keyboard-tests.json` (2), and `keybindings-menu.png`. Initial Editor run was 40/45: five synthetic keyboard edge tests failed because Editor input updates did not exercise the gameplay press/release counters. Those tests were moved to PlayMode and the installed InputTestFixture without weakening their assertions; all passed, with an additional real-room integration test. Initial evidence retained separately in `keybindings-editor-tests-initial.json`. A zero-discovery adapter run was discarded; RequestScriptCompilation refreshed discovery before the actual 2/2 run.

Commands: `unity recompile --format json`; `unity command run_tests --mode editor --filter OneMoreTime.EditTests --filter_type assembly --async_tests --format json`; separate PlayMode filters `OneMoreTime.Tests.KeyBindingTests`, `OneMoreTime.Tests.InputTests`, and `Keyboard`; `unity command capture_game_view --source screen --save_path Artifacts/Logs/keybindings-menu.png`. Pipeline writes that screenshot under `Assets/Artifacts/Logs`; it was copied to the evidence directory. Test preferences are restored by fixture teardown. MainMenu is left clean and open in Edit mode. Implementation and controls: [KEY_BINDINGS.md](KEY_BINDINGS.md).


## 2026-11-19 Chapter 2 flashlight

- **PASS — compilation:** running Unity 6000.6.0f1/Input System 1.20.0/Pipeline, final recompile returned zero errors/warnings. No package or Unity-version changes.
- **PASS — generation and repeat application:** generator produces the new debug scene and three prefab assets through Unity APIs. Repeated generation leaves one managed root, exactly two volumes and the enabled debug build entry, while preserving unrelated scenes. Ability prefab is locked, sandbox ability unlocked, shader refs serialized. Initial build exceeded the five-second RPC response timeout but finished; subsequent scene tests and successful repeat build confirmed completion. No timeout was counted as a successful command by itself.
- **PASS — five FlashlightTests:** lock defaults, activation/expiry/recharge, refusal to extend or spam bursts, fade timing on fixed ticks, invalid dt guards, nearby/facing/range masking, custom physical key input, holding without repeat, pause/retry and input suppression, camera width/bounds, and scene volume/reference wiring. Integration loads the real debug scene and invokes controller Update/FixedUpdate with InputTestFixture; it is not just a direct state test.
- **PASS — ChapterTwoObscurityTests 1/1:** GPU-rendered alpha/query parity, clear near/facing cone, obscurity behind/out of range/off-angle, soft volume edges, changed facing, expiry, paused manual-clock stability, and actual mist density variation. Final rerun includes the wisp refinement and unchanged darkness opacity. An empty discovery run was discarded and rerun after script compilation; no zero-test result is treated as verification.
- **PASS — KeyBindingTests 15/15:** both presets, rebind/conflict/suppression/persistence behavior, old ten-action preset migration, custom I/V collision fallback without losing bindings, and the existing custom-key recording/visible rewind/replay/hold integration.
- **PASS — EditMode suite 36/36:** prior checks plus menu capture of the new flashlight action; includes dynamic prompts and custom binding swaps/cancellation.
- **PASS — game-view review:** 1280×720 Unity captures of darkness unlit/lit and final mist. Initial flat mist was refined to drifting translucent ribbons and wider feathering before final GPU rerun/render review. HUD shows active/recharge time and the current physical key. Captures use runtime API positioning/activation, not a claimed manual full-lane playthrough.
- **PASS — whitespace:** `git diff --check`.
- **NOT RUN:** standalone build, entire unrelated PlayMode suite, complete human playthrough or final Chapter 2 balance/acquisition/puzzle tests.

Total **57 distinct tests passed**: 36 EditMode + 15 keybinding + 5 flashlight + 1 GPU. The `Flashlight` substring run contains seven results because it also selects two binding tests; those two are not counted again. Evidence: `Artifacts/Logs/flashlight-playmode-tests.json`, `flashlight-keybindings-tests.json`, `flashlight-editor-tests.json`, `flashlight-obscurity-gpu-tests.json`. Final shader/visual changes were recompiled and GPU-tested; gameplay/keybinding source was unchanged after its successful tests.

Commands: `unity recompile --format json`; `unity command eval 'return OneMoreTime.EditorTools.ChapterTwoFlashlightBuilder.Build();' --format json`; `unity command run_tests --mode playmode --filter Flashlight --async_tests --format json`; separate filters `ChapterTwoObscurityTests` and `OneMoreTime.Tests.KeyBindingTests`; `unity command run_tests --mode editor --filter OneMoreTime.EditTests --filter_type assembly --async_tests --format json`; `unity command capture_game_view --source screen --save_path Artifacts/Logs/flashlight-...png --format json`. Capture files initially land under `Assets/Artifacts/Logs` and were copied to the evidence directory. Final reviewed captures: `flashlight-dark-off.png`, `flashlight-dark-on.png`, `flashlight-mist-on-final.png`; pre-refinement mist captures remain named `flashlight-mist-off.png` / `flashlight-mist-on.png`.

Source/authoring guide: [CHAPTER_TWO_FLASHLIGHT.md](CHAPTER_TWO_FLASHLIGHT.md). Existing chapter scenes were not regenerated. The new sandbox is open clean in Edit mode, normal AutoTick restored by leaving the review Play session; nothing pushed or published.

## 2026-11-19 Top-left filter lens transition

- **PASS — actual Editor compilation:** Unity 6000.6.0f1/Pipeline reports zero errors/warnings.
- **PASS — existing PlayMode tests 6/6:** `PastLens` filter 1/1 covers transition completion and respawn; `Filter` filter 5/5 covers simultaneous keyboard actions, airborne momentum, safe collision switching, era/rewind restoration and embedding refusal. Evidence: `Artifacts/Logs/lens-swing-transition-tests.json` and `lens-swing-filter-tests.json`.
- **PASS — runtime reversal check:** applying for 0.09 seconds then switching back retains the current tint amount and gives 0.09 seconds of reverse travel; another 0.09 seconds reaches zero remaining time and zero past amount. Checked through Editor eval; not counted as an NUnit test.
- **PASS — actual Game-view review:** Main_Room05 captures at 0.09, 0.18 and 0.29 seconds show upper-left entry, rotation into place, then expanding glass. HUD stays upright and outside the clipped lens effect. Captures use controlled runtime timing, not a claimed manual playthrough: `Artifacts/Logs/lens-swing-entry.png`, `lens-swing-middle.png`, `lens-swing-zoom.png`.
- **PASS — whitespace:** `git diff --check`.
- **NOT RUN:** standalone build, full unrelated suite or complete human playthrough. Existing room geometry/camera settings were not changed.

Commands: `unity recompile --format json`; `unity command run_tests --mode playmode --filter PastLens --async_tests --format json`; same command with `--filter Filter`; `unity command capture_game_view --source screen --save_path Artifacts/Logs/lens-swing-...png --format json`. Test discovery was refreshed with RequestScriptCompilation between runs. Captures initially land under `Assets/Artifacts/Logs` and were copied to the evidence directory. The temporary Play session was stopped, restoring normal ticking; Main_Room05 remains open in Edit mode.


## 2026-11-19 Past filter background

- **PASS — compilation and shader import:** running Unity 6000.6.0f1/Pipeline; compilation completed with zero errors/warnings and ShaderUtil reported no Looping Forest shader messages. Final recompile was up to date.
- **PASS — PastBackgroundTests 3/3:** real Room 5 integration verifies incremental blending follows the lens, pause stops it, reversing mid-transition does not jump, collision refusal keeps present mode, recording/world rewind/retry restore the correct era and preserve the memory. Isolated GPU rendering verifies the temporary treatment differs visibly, replacement artwork actually renders, and turning the filter off restores identical present pixels. Texture, scroll phase, span and geometry remain stable.
- **PASS — existing loop regression 1/1:** `WorldOneArtTests.BackgroundRepeatsBeyondOldBoundsAndHandlesNegativeTravel` checks the wide aspect ratio, repeated camera travel, negative/far positions, and camera-filling bounds.
- **PASS — visual review:** 1280×720 actual Room 5 Game-view captures of present and past were inspected. Used runtime API activation and controlled ticks, not a full manual playthrough. Background visibly becomes warm/faded without shifting; bridges follow their existing era states.
- **PASS — whitespace:** `git diff --check`.
- **NOT RUN:** standalone build, entire unrelated suite or full human playthrough. Final past illustration is intentionally pending the user's choice.

Commands: `unity recompile --format json`; `unity command run_tests --mode playmode --filter PastBackground --async_tests --format json`; same command with `--filter BackgroundRepeatsBeyondOldBoundsAndHandlesNegativeTravel`; `unity command capture_game_view --source screen --save_path Artifacts/Logs/filter-background-present.png --format json` and `filter-background-past.png`. Discovery was refreshed between suites with RequestScriptCompilation. Evidence: `Artifacts/Logs/filter-background-tests.json` (3), `filter-background-loop-test.json` (1), `filter-background-present.png`, `filter-background-past.png`. A preview eval was interrupted by a script reload; its retry confirmed past=true and blend=1 before the final past capture. Screenshots initially landed under `Assets/Artifacts/Logs` and were copied to the evidence directory. Tests finished in Edit mode with Main_Room05 clean/open; no scene regeneration or remote operations.


## 2026-11-19 Statue revival

- **PASS — actual Unity compilation:** Unity 6000.6.0f1/Pipeline, zero errors/warnings after final input cleanup. Evidence: `Artifacts/Logs/statue-revival-compilation.json`.
- **PASS — revival integration:** `Statue` filter 7/7, including five new integration tests plus existing Room 3/4 statue checks. Verifies original Room 1 statues, exact narrow Room 3 spawn, no added colliders, renderer restoration, four-repeat cleanup, checkpoint activation versus revival, saved replay retention, pause, recording completion/death/retry exclusions, ignored actions and floating-world suspension through the final tick. Room 4 covers drowning followed by revival and preserved memory.
- **PASS — checkpoint regressions:** `Checkpoint` filter 8/8. Includes Rooms 5–8 scene wiring, chapter checkpoint retry, Room 1 traversal/pit/exit, Room 2 Shutter, and existing art/checkpoint checks. Three tests overlap the Statue filter and are counted only once. Initial run was 7/8 because the existing Shutter test immediately pressed Shutter on the death tick while the new revival intentionally blocked input. It now explicitly asserts revival, waits for completion, then runs all prior snapshot and exit assertions. Initial result retained as `statue-revival-checkpoint-tests-initial.json`.
- **PASS — replay regressions:** `RewindPlaytestTests` 10/10, including hold threshold/overwrite, visible world reversal, object opt-out, plate operation, upgrade separation, reverse/hold/endpoints, Q restart and persistent playback.
- **PASS — Game-view evaluation:** actual Main_Room05 captures at 1280×720, controlled fixed ticks at 0.12 s (charge), 0.30 s (breakout), and after completion. Initial bulky, green-edged fragments were replaced with smaller cropped stone fragments, behind-player sorting and earlier fade; final breakout leaves the goat clearly visible. Final render restores the player in front of the persistent statue. These are runtime API/timing captures, not a claimed manual full-room playthrough.
- **PASS — source review and whitespace:** reviewed explicit retry event ownership, pause and cancellation/mesh cleanup. Corrected wrapper input cleanup to clear retained movement as well as action edges, preventing stale movement during multiple FixedUpdates before Update. `git diff --check` passes.
- **NOT RUN:** standalone build, full unrelated PlayMode suite, manual art-gallery keyboard walkthrough, complete human playthrough or repeated-death pacing study.

Final total: **22 distinct tests passed** (7 + 8 + 10 minus 3 overlaps). Evidence: `Artifacts/Logs/statue-revival-tests.json`, `statue-revival-checkpoint-tests.json`, `statue-revival-replay-tests.json`. Final reviewed images: `statue-revive-charge-final.png`, `statue-revive-breakout-final.png`, `statue-revive-settled-final.png`; initial preview images remain without the `-final` suffix. Capture files initially land under `Assets/Artifacts/Logs` and were copied to `Artifacts/Logs`.

Commands: `unity recompile --format json`; `unity command run_tests --mode playmode --filter Statue --async_tests --format json`, repeated with filters `Checkpoint` and `RewindPlaytestTests`; `unity command eval` for controlled runtime timing; `unity command capture_game_view --source screen --save_path Artifacts/Logs/statue-revive-...png --format json`. Test discovery refreshed with RequestScriptCompilation between runs. Temporary Play review was stopped, restoring normal automatic ticking; Main_Room05 remains clean/open in Edit mode. Usage and evaluation: [STATUE_REVIVAL.md](STATUE_REVIVAL.md). No scene regeneration, push or publication.


## 2026-11-19 Pause-menu reset

- **PASS — real Unity compilation:** Unity 6000.6.0f1/Pipeline. Successful compile response reported zero errors/warnings. Two CLI compile-status polls lost their connection during Editor reload; neither failed response was treated as compilation evidence. The persisted final compile status reports completed/no errors, and the new five-test assembly subsequently ran successfully. Evidence: `Artifacts/Logs/pause-reset-compilation.json`.
- **PASS — PauseResetTests 5/5:** Room 3 real pause keyboard edge during exit-lift travel, cancellation of travel, exact checkpoint restoration, death counting, repeated-call guard and reset during revival; Room 5 shared HUD reset while drowning with saved replay preserved; Room 4 AwaitRecord reset clears tutorial/slow-motion state and resumes movement/crate spawning safely; original L04 recording death review and subsequent saved-memory reset; replay debug single-frame recovery plus valid recording death → visible rewind → Ready and retained memory.
- **PASS — Game-view review:** 1280×720 Main_Room05 pause menu shows Resume, Reset and Back to main menu with no overlap/clipping. Captured with runtime pause API, not a claimed mouse walkthrough. `Artifacts/Logs/pause-reset-menu.png` (copied from Pipeline's `Assets/Artifacts/Logs` output).
- **PASS — code review / whitespace:** reset dispatch uses the owning room director so drowning/entry/exit/tutorial phases cannot remain active after respawn; lift is docked with matching gate state, existing shutter snapshot stays intact; `git diff --check` passes.
- **NOT RUN:** standalone build, full unrelated test suite, individual mouse activation of every menu or full human playthrough. EchoFoothills and flashlight reset paths were source-reviewed and compiled; their new button behavior is not separately PlayMode-tested in this run.

Commands: `unity recompile --format json`; `unity command run_tests --mode playmode --filter PauseResetTests --async_tests --format json`; `unity command eval` to pause the running room; `unity command capture_game_view --source screen --save_path Artifacts/Logs/pause-reset-menu.png --format json`. Evidence: `Artifacts/Logs/pause-reset-tests.json` (5/5). Runtime sources: MainRoom.cs, RewindPlaytest.cs / RewindPlaytestHUD.cs, ChapterOneRoom.cs, RoomFour/Room4AbilityTutorial.cs, RoomLift.cs, ShutterAbility.cs, RunManager.cs, ChapterOne/NostalgiaChapter.cs and ChapterTwoFlashlightPlaytest.cs. New tests: `Assets/_OneMoreTime/Tests/PlayMode/PauseResetTests.cs`. Temporary Play session stopped after review; Main_Room05 restored to Edit mode, normal automatic ticking unchanged. No scene regeneration or remote operations.


## 2026-11-19 Main-game death counter

- **PASS — actual Unity compilation:** Unity 6000.6.0f1/Pipeline completed successfully; no compilation errors. Evidence: `Artifacts/Logs/death-counter-compilation.json`.
- **PASS — DeathCounterTests 4/4:** cumulative Room 1 → Room 5 deaths and New Game reset; ordinary Retry/recording completion excluded while a recording death survives world rewind; Room 4 water death counts once even when pause Reset skips drowning, with a later living-player Reset counted separately; actual Debug menu launch of Room 4 leaves the journey total unchanged under water deaths and Reset.
- **PASS — Game-view review:** real Main_Room05 at 1280×720 shows the cream code-drawn pixel skull and `x 3` below the buttons at the pause panel's bottom-right. Three runtime Reset calls produced the count. Unpausing removes the counter entirely. Captures use controlled runtime APIs, not a claimed manual mouse/full-room playthrough. Evidence: `Artifacts/Logs/death-counter-paused.png`, `death-counter-gameplay.png`, copied from Pipeline's `Assets/Artifacts/Logs` output.
- **PASS — review / whitespace:** independent review of main-session gating, scene transitions, death-versus-retry events, drowning deduplication, rewind exclusion and pause-only rendering found no issue. `git diff --check` passes.
- **NOT RUN:** standalone build, full unrelated regression suite or complete human playthrough.

Commands: `unity recompile --format json`; `unity command run_tests --mode playmode --filter DeathCounterTests --async_tests --format json`; `unity command eval` for controlled Reset/pause calls; `unity command capture_game_view --source screen --save_path Artifacts/Logs/death-counter-...png --format json`; `unity command editor_stop --format json`. The last command only exits Play mode; it does not terminate Unity. Main_Room05 is left in Edit mode. Runtime ownership: `RoomFour/MainGameProgress.cs` stores the session total; `MenuScreen.cs` / `SceneFlow.cs` gate main-game versus debug/tutorial sessions; `MainRoom.cs`, `RewindPlaytest.cs`, `ChapterOneRoom.cs`, `RoomFour/Room4AbilityTutorial.cs` report deaths; `GameUI.cs` / `RewindPlaytestHUD.cs` render the pause statistic. Tests: `Assets/_OneMoreTime/Tests/PlayMode/DeathCounterTests.cs`. No scene generation, push or publication.


## 2026-11-19 Room 4 menu-return reset

- **PASS — actual Unity compilation:** Unity 6000.6.0f1/Pipeline, zero errors/warnings. Evidence: `Artifacts/Logs/room4-menu-reset-compilation.json`.
- **PASS — RoomFourTests 13/13:** new `ReturningToMenuRestartsCompletedAndInterruptedAcquisition` completes the cutscene, exits through the actual `SceneFlow.Load(0)` menu path, enters Debug Room 4 through `MenuScreen.OpenTutorial(9)`, verifies the pickup/blocker/intact bridge/no crates/no memory, and reaches the acquisition prompt again. Repeats the exit while the cutscene is awaiting record. Both returns clear replay unlock flags, retain the death total and leave debug tracking disabled. Existing tests verify pickup/hold/rewind/camera, checkpoint death/retry, owned reload and Room 4 → 5 progression, crate replay/current and terrain references.
- **PASS — DeathCounterTests 4/4:** main-room accumulation/New Game reset, debug exclusion, recording death versus normal rewind and drowning deduplication remain correct. The first second-suite request returned zero discovered tests; this was not counted as a pass. Refreshing discovery with `CompilationPipeline.RequestScriptCompilation()` then rerunning produced all four passing tests.
- **PASS — source review and whitespace:** all runtime menu exits use `SceneFlow.Load(0)`; clearing both replay flags preserves their dependency, while deaths/room changes do not use this reset. `git diff --check` passes.
- **NOT RUN:** standalone build, full unrelated suite or manual visual/mouse walkthrough. No animation assets or timings changed, so no new render comparison was needed.

Commands: `unity recompile --format json`; `unity command run_tests --mode playmode --filter RoomFourTests --async_tests --format json`; same command with `--filter DeathCounterTests`; `unity command eval` to refresh discovery and inspect final Editor state. Evidence: `Artifacts/Logs/room4-menu-reset-tests.json` (13), `room4-menu-reset-death-tests.json` (4), `room4-menu-reset-compilation.json`. No scenes regenerated, push or publication.


## 2026-11-19 Camera ability art and pause equipment

- **PASS — real Unity compilation:** Unity 6000.6.0f1/Pipeline completed successfully; no compilation errors. Evidence: `Artifacts/Logs/camera-ability-compilation.json`.
- **PASS — AbilityArtTests 7/7:** five resource cases check transparent artwork, small Point-filtered textures, no mipmaps, source aspect and approximately one-unit world canvas. Replacement integration checks original root/collision preservation, correct aspect under nonuniform parent scaling, sorting, disabled old visuals and idempotency. Actual Room 4 acquisition shows then hides the cassette through existing pickup logic. The first run was 2/7: source PPU=64 caused oversized imported bounds. Deriving source PPU from source dimensions fixed the issue; final results cover all five sprites. Initial evidence remains in `camera-ability-art-tests-initial.json`.
- **PASS — CameraEquipmentTests 3/3:** actual Room 2/4 pickup gating; local basic/advanced replay separation despite unrelated campaign upgrade flags; filter and flashlight availability follow their owning room. Equipment rendering does not grant abilities.
- **PASS — actual Game-view review:** normal Room 5 assembly, fully upgraded assembly and Room 4 cassette pickup inspected at 1280×720. Adjusted the lens anchor to sit over the optical lens and enlarged the camera after the first preview. The all-module view uses temporary runtime preview flags/components; no campaign or scene progression change. Final screenshots: `camera-equipment-room5.png`, `camera-equipment-all-modules.png`, `camera-ability-room4-pickup.png`, copied from Pipeline's `Assets/Artifacts/Logs` output to `Artifacts/Logs`.
- **PASS — whitespace:** `git diff --check`.
- **NOT RUN:** standalone build, full unrelated suite, complete human playthrough or individual mouse walkthrough of every pause context. No scene regeneration or remote operations.

Commands: `unity recompile --format json`; `unity command run_tests --mode playmode --filter AbilityArtTests --async_tests --format json`; same command with `--filter CameraEquipmentTests`; `unity command eval` for import refresh and controlled preview state; `unity command capture_game_view --source screen --save_path Artifacts/Logs/camera-...png --format json`; `unity command editor_stop --format json`. The first combined regex filter discovered zero tests and was not counted as a pass; separate filters produced all ten. Discovery was refreshed between filters with RequestScriptCompilation. A forced-reimport eval exceeded its response timeout, but actual import completion and final asset properties were verified by the passing tests. The temporary Play session was stopped, leaving Main_Room05 in Edit mode. Exact generated-art prompts and usage: [CAMERA_ABILITY_ART_PROMPTS.md](CAMERA_ABILITY_ART_PROMPTS.md), [CAMERA_ABILITY_ART.md](CAMERA_ABILITY_ART.md).


## 2026-11-19 Plain modular ability sprites

Replaced all five PNG resources in `Assets/_OneMoreTime/Resources/AbilityArt/` with charcoal/silver equipment, preserving filenames and GUIDs. The magic lens retains a luminous teal spiral and etched runes per the follow-up request. `CameraEquipment.cs` now crops source padding for the pause display and uses shared cassette mounting geometry, a top flash shoe and an aligned optical filter, with visible silver connector pieces. Upgrades assemble visually; no crafting interface or ability-rule changes were introduced.

PASS: Unity 6000.6.0f1 compilation, seven AbilityArtTests (imports/transparency/scale, pickup preservation and Room 4 acquisition), `git diff --check`, actual all-module pause-screen visual review. Pipeline's stale connection was restored through Window → Pipeline Stop/Start Server without closing the Editor. NOT RUN: CameraEquipmentTests returned zero discovered tests; this is not a pass. Standalone build and full human playthrough were not run.

Evidence: `Artifacts/Logs/plain-ability-compilation.json`, `plain-ability-art-tests.json`, `plain-camera-equipment-tests.json`; Game-view capture `Assets/Artifacts/Logs/plain-camera-all-modules.png`. Open `Assets/_OneMoreTime/Scenes/Main_Room05.unity`, Play, then Esc to inspect acquired modules. All-module review used temporary Play Mode unlocks only; Editor restored to Edit mode. Next: normal pickup/playtest review; no scene regeneration, remote push or publishing.


## 2026-11-23 Chapter UI, ability scope, shutter and checkpoint fixes

- **PASS — Unity compilation:** actual Unity 6000.6.0f1/Pipeline compilation completed with zero errors/warnings reported. Evidence: `Artifacts/Logs/chapter-fixes-compilation.json`.
- **PASS — 67/67 PlayMode tests:** `ChapterOneRegression` category covers CameraEquipment (3), ChapterAbilityScope (3), ChapterBasicTraversal (1), ChapterClimbTraversal (2), ChapterOneRooms (9), DeathCounter (4), PastBackground (3), PauseReset (5), RewindPlaytest (11), RoomFour (13), Shutter (4), and StatueRevival (9). Evidence: `Artifacts/Logs/chapter-regressions-tests.json`. Tests exercise actual room loads and fixed-step gameplay, including Room 7's first downstream crossing from its real entrance without teleporting or advanced powers. They do not constitute a complete human traversal of all rooms.
- **PASS — Game-view review:** ordinary Room 5 has no top-left tips panel or filter control; pause shows only shutter and its attached basic cassette. Respawn restores the authored right-facing direction from a left-facing setup, removes the statue body and leaves its painted base. Modern and legacy base persistence also receives real-frame test coverage. Evidence: `Artifacts/Logs/chapter-fixes-gameplay.png`, `chapter-fixes-equipment.png`, `chapter-fixes-statue-before.png`, `chapter-fixes-statue-after-final.png`, `chapter-fixes-base-visible-final.png`, `chapter-fixes-visual-state.json`. Review uses controlled runtime calls, not a claimed full mouse/keyboard playthrough.
- **PASS — independent source review:** campaign/debug transition boundaries, local assembly availability, single-memory switching, route span activation and downstream disposal reviewed. `git diff --check` passes.
- **PASS — read-only decoration audit:** 178 loose managed props identified; 52/54 proposed placements supported by existing ground and clear of interactions. Per-room accepted counts: 8/4/7/8/5/8/6/6. Two Room 2 placements were omitted for clearances. Evidence: `Artifacts/Logs/chapter-decoration-audit.json`. No geometry or scene object was changed by this audit.
- **BLOCKED / NOT APPLIED — saved decoration cleanup:** automatic approval review rejected a loop saving Rooms 1–8 because the user said “current level,” leaving multi-room scope unclear. A concrete 178→52 cleanup was prepared, audited and presented for scope clarification. No scene cleanup has been saved; application, idempotency, rollback and visual validation remain NOT RUN.
- **NOT RUN:** standalone build, complete human chapter traversal or the full unrelated test suite.

Failure history: the initial combined run was 53/66. Twelve failures exposed pedestal mesh coordinate handling; the remaining failure was an older debug test assuming campaign replay carried into debug. After corrections, 66/66 passed, but Game-view review caught the full statue reappearing after mesh regeneration. Replacing mesh clipping with a cropped Sprite rectangle and adding an across-frame test produced the final 67/67 pass. Initial failure evidence is retained in `chapter-regressions-tests-initial.json`. Empty per-filter discovery attempts were not counted as passes. One final run setup stalled in Unity's `ExitPlayModeTask`; it was cancelled, actual Edit Mode was confirmed, and the successful 67-test run was started cleanly.

Commands: `unity recompile --json`; `unity command run_tests --mode playmode --filter ChapterOneRegression --filter_type category --async_tests true --json`; `unity command test_status --json`; `unity command eval` for controlled scene/state review and read-only `ChapterDecorationCleanup.Audit`; `unity command capture_game_view --source screen --save_path Artifacts/Logs/chapter-fixes-….png --json`; `unity command editor_stop --json`. Pipeline captures were copied from its `Assets/Artifacts/Logs` output into `Artifacts/Logs`. Background ticking was enabled only in the temporary review session; saved project settings were not changed.

Final Editor state: Main_Room05 in Edit Mode, no scene save/regeneration, original automatic ticking retained. No push or publishing. Current source rules: [ROOMS_5_TO_8.md](ROOMS_5_TO_8.md), [CAMERA_ABILITY_ART.md](CAMERA_ABILITY_ART.md), [STATUE_REVIVAL.md](STATUE_REVIVAL.md), [MAIN_ROOM_DECORATION.md](MAIN_ROOM_DECORATION.md).


## 2026-11-23 Room 5 complete drawing and whirlpool

- **PASS — Unity compilation:** Unity 6000.6.0f1/Pipeline, final compile reported zero errors/warnings. Two earlier CLI status polls disconnected during domain reload; they were not treated as success. Persisted compile status and subsequent test runs confirmed successful compilation. Evidence: `Artifacts/Logs/room5-layout-compilation.json`.
- **PASS — seven RoomFiveLayoutTests:** normal/debug Room 5 has only basic abilities and seven permanent background bridge spans without colliders; two grassy islands have only thin one-way top collision, soil reaches below water and scaffold walls add no collision; central checkpoint saves; actual shutter input holds the plate and replacing its image releases it immediately; an actual motor-input route boards/rides the shuttle, jumps to the lower red approach and walks to the checkpoint without teleporting after initial plate setup; crates move 2.5 units in one second and spawn ten seconds apart; the real source sends crates to a visible downward sink; the camera is 20 units wide and its upper bound is nonphysical.
- **PASS — three WhirlpoolCrateTests:** captured drain trajectory resumes deterministically, backward playback visibly restores a sunk crate, reuse clears sinking state, old immediate-disposal sources remain unchanged, foreign crates are ignored, and actual recording/world rewind/forward/reverse/held playback include the sink without a color change.
- **PASS — chapter regressions, 77 distinct final passing tests:** category run initially passed 75/77. Two old statue tests indexed a second Room 5 checkpoint; the supplied layout intentionally has one. Moved only those multi-statue fixtures to Room 6; all nine StatueRevivalTests then passed. Existing filter/era fixtures were moved to explicit Debug Room 8 because the replacement Room 5 intentionally has no era groups. No assertions were removed to grant unavailable abilities or add an extra statue. Final regeneration changed only water-art alignment; the seven Room 5 tests passed again. Evidence: `room5-layout-regressions-initial.json`, `room5-layout-statue-regressions.json`, `room5-layout-tests.json`.
- **PASS — render review:** full-room and five 20-unit-wide Editor camera renders checked for the supplied geometry, bridge foreground/background distinction, soil, supports and stepped water. Initial overlapping supports were changed to share lower scaffold walls. Actual 1280×720 Game view checked a live crate entering the whirlpool; the goat was positioned for inspection, so this is not a claimed manual traversal. The final water-art offset corrects the painted crest relative to buoyancy; final overview renders were reinspected. `room5-layout-overview.png`, `room5-layout-view1.png` through `view5.png`, and `room5-whirlpool-gameview.png` (before that final art alignment).
- **PASS — generation/preservation:** repeated managed-root builds retain one checkpoint, one shuttle, one source and zero era groups. Scene and EditorBuildSettings hashes changed only for `Main_Room05.unity`; see `Artifacts/Room5Layout/preservation-check.json`. Old scene backup and source drawing are in that directory. No manual Unity YAML or GUID edits. Final Editor state: Room 5, clean, not playing, AutoTick=true.
- **PASS — whitespace:** `git diff --check`.
- **NOT RUN:** standalone build, full unrelated suite, complete human room traversal or long-gap difficulty/bypass study. Traversal evidence covers the plate → shuttle → checkpoint section, not a complete room-clear proof.

Commands: `unity recompile --format json`; `unity command eval 'return OneMoreTime.EditorTools.ChapterOneExpansionBuilder.BuildRoom(5);' --format json`; `unity command run_tests --mode playmode --filter ChapterOneRegression --filter_type category --async_tests --format json`; focused reruns with `--filter StatueRevivalTests` and `--filter RoomFiveLayoutTests`; `unity command eval 'return OneMoreTime.EditorTools.ChapterOneExpansionBuilder.Capture(5);' --format json`; `unity command capture_game_view --source screen --save_path Artifacts/Logs/room5-whirlpool-gameview.png --format json`; `unity command editor_stop --format json`. An initial zero-test discovery result was discarded; compilation refresh produced the real runs above. No other rooms regenerated and no remote operations.

## 2026-11-23 Official Room 6 layout

| Check | Result | Evidence |
|---|---|---|
| Actual Editor compilation | PASS — zero errors/warnings, Unity 6000.6.0f1 | `Artifacts/Logs/room6-layout-final-compilation.json` |
| Scoped generation | PASS — Room 6, 0 statues, 1 controlled platform, 1 log source | `Artifacts/Logs/room6-layout-generation.json` |
| Final ChapterOneRegression PlayMode run | PASS — 89/89, 10.96 sec | `Artifacts/Logs/room6-layout-final-regressions.json` |
| New RoomSixMechanismTests | PASS — 6 tests: endpoint parking, solid return, midpoint reversal/restore, old pingpong behavior, one log per press at 2 units/sec, rewind/playback edge state and interval-source regression | Included in final regression results |
| New RoomSixLayoutTests | PASS — 6 tests: authored collision/ability/camera policy; actual source cadence and motion; log boarding and carry; shutter-held endpoint and returning rider; recording→rewind→replay log birth; automatic lift ascent/fade into actual Room 7 | Included in final regression results |
| Checkpoints / prior layouts | PASS — absent-statue fallback fixed; legacy statue tests and Room 7 multi-statue tests pass; old traversal coverage moved to existing Room 8 fixtures | Included in final regression results |
| Visual review | PASS — actual Game view at log plate, overview, and mechanism/lift closeups inspected; five 20-unit-scale views captured | `room6-log-gameview.png`, `chapter1-room6-overview.png`, `chapter1-room6-view1.png`–`view5.png` in `Artifacts/Logs` |
| Scope preservation | PASS — only Main_Room06 scene differs, other scenes/build settings unchanged | `Artifacts/Room6Layout/preservation-check.json` |
| Whitespace check | PASS | `git diff --check` |
| Full human traversal / jump balance / standalone build | NOT RUN | Mechanism tests and controlled captures do not replace full human playtesting |

Commands: `/Users/wesley/.unity/bin/unity recompile --format json`; `unity command eval 'return OneMoreTime.EditorTools.ChapterOneExpansionBuilder.BuildRoom(6);' --format json`; `unity command run_tests --mode playmode --filter ChapterOneRegression --filter_type category --async_tests --format json`, with actual completion read from `Temp/pipeline_test_status.json` (not the initial zero-result acknowledgement). Visuals used `ChapterOneExpansionBuilder.Capture(6)` and `capture_game_view --source screen`. One initial 89/89 run preceded the final phantom-checkpoint fix; the final 89/89 run covers the completed code.

## 2026-11-23 Frozen world objects awaiting replay

- **PASS** — actual Unity compilation, 0 errors / 0 warnings: `Artifacts/Logs/rewind-wait-compilation.json`.
- **PASS** — **99/99** ChapterOneRegression PlayMode tests, 8.82 seconds: `Artifacts/Logs/rewind-wait-regressions.json`.
- **PASS** — ten new `RewindWaitingTests`: affected platforms/push crates stay fixed while the goat moves; no stale carry for platform/log riders; nonaffected platform keeps simulating; overwrite/retry/shutter lifecycle; Room 6 log position, bob clock and spawning freeze; replay resumes log carrying; chapter plate/controller state holds; Room 8 crumble countdown resumes only after replay.
- **PASS** — existing Room 4 imaginary-acquisition test now explicitly checks zero spawning while waiting and spawning on replay; disposal check verifies frozen objects cannot be removed; existing gate test checks preserved rewound state before playback. Existing upgraded reverse/hold and endpoint behaviors remain covered.
- **PASS** — `git diff --check`; no scene/prefab writes were needed.
- **NOT RUN** — full human chapter playthrough and standalone build.

Commands: `unity recompile --format json`; `unity command run_tests --mode playmode --filter ChapterOneRegression --filter_type category --async_tests --format json`. Results above are from completed `Temp/pipeline_test_status.json`, not the initial asynchronous acknowledgement.

## 2026-11-23 — Room 7 official layout verification

| Check | Result | Evidence |
|---|---|---|
| Real Editor compile | PASS — 0 errors, 0 warnings | `Artifacts/Logs/room7-compilation.json` |
| Scene generation | PASS — 2 checkpoints, 14 moving/pool platforms, no era spans or obsolete floating sources | `room7-generation.json` |
| Chapter PlayMode suite | PASS — 116/116, 0 skipped | `room7-regressions.json` |
| Loop/shuttle periods and rewind | PASS — exact 6s full laps and 4s full round trips, carry delta cleared during waiting | `RoomSevenMechanismTests`, `RoomSevenLayoutTests` |
| Dual plate unlock | PASS — simultaneous live goat + shutter, count/color, permanent latch after release/rewind/retry | Same fixtures |
| Dispatch | PASS — one per press, 4s cooldown, endpoint cleanup/reuse, recording lifetimes and waiting | Same fixtures |
| Trapdoor and camera | PASS — gameplay/recording/decks freeze, pause works, camera x stays 10, zoom returns, opening excluded from rewind | `RoomTrapdoorTests`, `RoomSevenLayoutTests` |
| Room transitions | PASS — Room 6 lift arrives into 7, opened shaft accepts airborne upward exit into 8 | `RoomSixLayoutTests`, `RoomSevenLayoutTests` |
| River replay regression preservation | PASS — original basic/reverse board/carry/dismount and revival clock coverage moved to Room 8 high stream | `ChapterBasicTraversalTests`, `ChapterOneRoomsTests`, `StatueRevivalTests` |
| Visual review | PASS — actual Editor overview + live counter/reveal screenshots inspected | `chapter1-room7-overview.png`, `room7-paired-live.png`, `room7-trapdoor-reveal.png` |
| Scene scope | PASS — only Room 7 changed; all other scenes and build settings retain pre-turn SHA256 | `room7-scene-scope.json` |
| Human full-route playthrough | NOT RUN — mechanism tests use explicit setup positions; full difficulty assessment still requires playing the tower | — |

Executed through Unity CLI Pipeline: `unity recompile --format json`, `unity command eval 'return OneMoreTime.EditorTools.ChapterOneExpansionBuilder.BuildRoom(7);' --format json`, and `unity command run_tests --mode playmode --filter ChapterOneRegression --filter_type category --async_tests --format json`. The completed test-status file, not the initial asynchronous acknowledgement, supplies the 116/116 result. An initial run was 108/109 due to a stale Room 7 floating-source test; it was migrated to Room 8, then the full suite passed.

## 2026-11-23 Official Room 8 verification

| Check | Result | Evidence |
|---|---|---|
| Actual Editor compilation and only-Room-8 generation | PASS; final reload status has no compile errors. Existing deprecated Unity API warnings remain | `room8-compilation-final.json`, `room8-generation.json` |
| Chapter regressions | PASS — **129/129**, including 13 new Room 8 tests | `room8-regressions.json` |
| Final cave/water visual adjustment | PASS — regenerated Room 8 and reran **7/7** scene tests | `room8-final-layout-tests.json` |
| Two crate streams / loops | PASS — 2 units/sec left, 6-second spawns, normal crate colors, both complete loop cycles 6 seconds | `RoomEightLayoutTests` |
| Remote plate reveal | PASS — pans to carrier and back; motor, recording, decks, sources and floating crates stay still; pause respected | `RoomEightLayoutTests` |
| Traveling statue | PASS — one-way 10 seconds, carry, endpoint parking, active checkpoint follows; ordinary recording/rewind holds correctly | `RoomEightMovingCheckpointTests`, `RoomEightLayoutTests` |
| Death/reset | PASS — fall, water, pause reset and all recording-death paths home the carrier before revival or after visible rewind, preserve memory | Same two test classes |
| Existing scene protection | PASS — only Main_Room08 differs from pre-task hashes; build settings unchanged | `room8-scene-preservation.json` |
| Art and cave readability | PASS — overview and 20-unit camera captures inspected; darkened cave threshold, removed incidental grass, strengthened foreground curtain | `chapter1-room8-overview.png`, `chapter1-room8-view7.png` |
| End-to-end human route, difficulty, standalone build | NOT RUN | Mechanism setups use explicit placement; they are not a complete player traversal |

Commands ran through the connected Unity 6000.6.0f1 Editor/Pipeline: `unity recompile --format json`; `unity command eval 'return OneMoreTime.EditorTools.ChapterOneExpansionBuilder.BuildRoom(8);'`; `unity command run_tests --mode playmode --filter ChapterOneRegression --filter_type category --async_tests`; final `--filter RoomEightLayoutTests`. Completed result files supply pass counts, not async launch acknowledgements. One compile client timed out after domain reload; the independent status and updated builder confirmed compilation, and a subsequent `unity recompile` returned up_to_date with no errors.

The old Room 8 prototype was copied through Unity AssetDatabase to `Assets/_OneMoreTime/Tests/Fixtures/Main_Room08.unity`. Legacy era, crumble and raft traversal checks use that fixture with their original assertions; official Room 8 checks use the real new scene. The fixture is absent from build settings and menus. Room 7 upward exit and the current Room 8 finale remain covered against the real scenes.

Final rendered Play-mode camera check: goat remains legible through the foreground curtain (`Artifacts/Logs/room8-foreground-camera.png`). A screen-source capture returned the previous arrival backbuffer, so the current camera was rendered directly; this image excludes overlay HUD. Unity was returned to Edit mode with Main_Room08 open.

## 2026-11-24 Room 8 corrections and shutter collision

- **PASS — 137/137 ChapterOneRegression PlayMode tests** through the live Unity Editor. Evidence: `Artifacts/Logs/room8-fixes-final-tests.json`.
- **PASS:** entrance arc stays outside bank/walls and lands on the existing floor; two plates are required simultaneously; single remote plate still triggers its camera reveal; carrier keeps its 10-second one-way/death reset behavior.
- **PASS:** new shutter copy cannot support the player until its full body clears it; stationary repeated replacements do not climb; cleared copies can be landed on; plate holding is immediate; normal replay support remains intact. Four new real-scene tests cover MainRoom and chapter simulation paths.
- **PASS:** GPU render tests for crate/log submersion verify clipping below the waterline, unchanged pixels above, fully hidden but still active sinking, exact visual restoration on rewind, and pooled reuse.
- **PASS:** room-only generation, opening/cave render review, scene hash protection and whitespace check. No new helper ledges. Waterfall continues above camera, clear cave floor remains y47.3.
- Initial run passed 136/137: the new entrance overlap query counted resting-floor contact at the final landing. All prior airborne frames cleared; the assertion now permits floor contact while rejecting body/wall penetration. Final full run passed. A zero-test discovery result after script refresh was discarded and rerun. Compile client timed out around domain reload; independent status reported no errors and actual updated scene/test execution succeeded.
- **NOT RUN:** full human chapter traversal or standalone build. Final artifacts: `room8-fixes-generation.json`, `room8-fixes-compile-status.json`, `room8-fixes-scene-scope.json`, `chapter1-room8-view1.png`, `chapter1-room8-view7.png`.


## 2026-11-24 Pause keybinding options

- **PASS — 19/19 KeyBindingTests PlayMode:** selected preset/custom keys, preference persistence, no held-input leaks, and four new real-scene pause-settings cases in Main_Room01, Main_Room04, Main_Room08 and Debug_Rewind. Escape cancels capture, closes settings while retaining pause, and resumes only from pause. Player, moving platforms and floating-object source clocks remain frozen during capture; new movement/record keys work after resume.
- **PASS — 36/36 EditMode tests:** shared menu capture/conflict rules, control prompts, and full-height 16:9 viewport without the old reserved footer.
- **PASS — Unity compilation:** independent Pipeline recompile_status reports idle, failed=false, no errors; new integration tests execute the new code. The recompile client itself timed out after 40s around domain reload, so its timeout is not counted as a pass.
- Commands: `unity recompile --timeout 40 --format json`; `unity command recompile_status --format json`; `unity command run_tests --mode playmode --filter KeyBindingTests --async_tests --format json`; `unity command run_tests --mode editmode --async_tests --format json`. Saved results are completed reports, not launch acknowledgements.
- Evidence: `Artifacts/Logs/pause-controls-compile-status.json`, `pause-controls-playmode-tests.json`, `pause-controls-editmode-tests.json`. PASS: Room 8 pause panel and keybinding settings captured and visually inspected (`pause-controls-menu.png`, `pause-controls-settings.png`); preset buttons and all binding rows are legible, no footer strip remains. No scene generation/level layout changes.
- **NOT RUN:** standalone build and full gameplay regression suite; focused input/UI tests were used for this change.


## 2026-11-24 Room 8 lower bridge restored

- **PASS:** saved real one-way bridge at x=19.7–24.7, y=2.9 plus decorative scaffold beams; restored through the existing Unity bridge helper. Entrance whirlpool arrows remain free of bridges. Builder updated so future regeneration preserves the restored span.
- **PASS — 9/9 RoomEightLayoutTests** in live Unity PlayMode, including the restored bridge's exact bounds and collision. The first post-refresh discovery returned zero tests and was discarded; completed rerun passed.
- **PASS:** Unity compile status idle/no errors, whitespace check, and scene/build-setting hashes: only Main_Room08 changed.
- Evidence: `Artifacts/Logs/room8-bridge-restored.json`, `room8-bridge-restore-tests.json`, `room8-bridge-restore-compile.json`, `room8-bridge-restore-scope.json`. Command: `unity command run_tests --mode playmode --filter RoomEightLayoutTests --async_tests --format json`. Full traversal/build NOT RUN for this local layout correction.


## 2026-11-24 Room 7 route / camera and Room 8 counter

- **PASS — 147 distinct ChapterOneRegression checks:** full suite initially 146/147; subsequent RoomEightLayoutTests 9/9 after correcting only a new Color-vs-Color32 assertion. Combined latest-result summary: `Artifacts/Logs/room7-route-update-verification.json`; complete source runs: `room7-route-update-regressions-initial.json`, `room7-route-update-room8-tests.json`. This is coverage across two runs, not a claim that the first full run passed unchanged.
- **PASS:** either Room 7 paired plate triggers the one-time platform reveal; player, recording and platforms stay frozen; both plates are still required for the permanent unlock. Legacy Room 8 single reveal trigger is preserved.
- **PASS:** four dispatched decks move at 3 units/sec, cooldown remains 4; their collider fits the centered left slot and passes under the lowered east wall, while motor/carry simulation blocks the goat at both obstacles. Two small decks top y56.1, trapdoor y62.7, following step y63.7.
- **PASS:** normal width stays 20 and horizontal center stays x10; camera top never exceeds y65 even during trapdoor focus/zoom or a jump above the room. No ceiling collision added.
- **PASS:** Room 8 carrier displays white 0/2 and 1/2, green 2/2, needs both plates, and retains its one-way trip. Restored x19.7–24.7 lower bridge remains present.
- **PASS:** actual Unity compilation (idle, failed=false, no errors), generation of only Rooms 7 and 8, saved-scene SHA256 scope, and whitespace check. Static Editor route renders inspected; their fixed capture camera positions are not evidence for runtime camera clamping, which is covered by the PlayMode checks.
- Runner recovery: the first launch stalled before reporting any tests and was cancelled, then restarted from Edit mode. It is not counted as a pass. The earlier compile client timed out around domain reload; independent status and execution of updated code provide compilation evidence.
- Commands: `unity command run_tests --mode playmode --filter ChapterOneRegression --filter_type category --async_tests --format json`; `unity command run_tests --mode playmode --filter RoomEightLayoutTests --async_tests --format json`; `unity command recompile_status --format json`. Only completed reports supply counts.
- **NOT RUN:** uninterrupted human traversal, complete difficulty assessment, standalone build. Unity left in clean Edit mode on Main_Room07; evidence `room7-route-update-ready.json`. No other scenes or build settings saved.


## 2026-11-24 Main rooms 4–6 reordered

- **PASS — 150/150 ChapterOneRegression PlayMode tests**, zero failures/skips: `Artifacts/RoomOrder/chapter-regressions.json`. Includes Room 4 campaign/debug recording lock and shutter, Room 5 pickup/cutscene, remapped controls/equipment/death checks, Room 4→5 horizontal exit, Room 5→6 walk transition and Room 6→7 lift transition.
- **PASS — 1/1 VerticalRoomTests.FinalPlateOpensLiftAndTransfersUpward**, actual Room 3 lift arrival into new Room 4: `Artifacts/RoomOrder/room3-lift-transition.json`. Two zero-discovery attempts were discarded; after refreshing discovery the completed exact-filter run passed. Total: 151 distinct passing checks across these two completed runs.
- **PASS — actual Unity compilation:** recompile_status idle, failed=false, compilationFailed=false, no errors (`Artifacts/RoomOrder/compilation.json`); updated code/scenes executed in the live Editor.
- **PASS — migration/scope:** old 6→4, 4→5, 5→6 scene GUIDs preserved through Unity AssetDatabase.MoveAsset. SHA256 comparison identifies only Main_Room04/05/06, their metadata, and EditorBuildSettings among existing scene/build files; intro/tutorial/Rooms 1–3/7–8 unchanged (`scene-scope.json`, `migration.json`). Old river rewind-only checks use the Unity-created Tests/Fixtures/Main_Room06 scene, absent from Build Settings; actual Room 4 ability lock has dedicated checks.
- **PASS — visual review:** new Room 4 lift arrival/right bank, Room 5 walk-in and Room 6 exit lift rendered using the real scene cameras (`room4-capture.json`, `room5-room6-captures.json`; images under `Artifacts/Logs/chapter1-room4/5/6-view*.png`). These static renders are not claimed as continuous gameplay.
- Commands: `unity command run_tests --mode playmode --filter ChapterOneRegression --filter_type category --async_tests --format json`; exact-filter PlayMode run for `VerticalRoomTests.FinalPlateOpensLiftAndTransfersUpward`; `unity command recompile_status --format json`; `git diff --check`. Completed results, rather than launch acknowledgements, provide the pass counts.
- **NOT RUN:** uninterrupted human chapter traversal, first-player balance/timing and standalone build. Editor left on clean Main_Room04 (`Artifacts/RoomOrder/ready.json`). No push or publication.


## 2026-11-24 Room 4 log cooldown

- **PASS — 17/17 focused RoomSix PlayMode tests**, zero failures/skips. Tests keep their historical names but use current Main_Room04 for gameplay. New cases prove rejection during cooldown, no queued spawn while held through expiry, a successful fresh press afterward, and rewind/playback restoration of countdown plus plate-edge state. Existing source, movement, boarding, return-platform and ability-lock cases also pass.
- **PASS — actual Unity compilation**, no errors (`Artifacts/Room4LogCooldown/compilation.json`), saved Room 4 plateCooldown=4/speed=2 (`scene-update.json`), scene/build-setting hash scope (only Main_Room04 changed), `git diff --check`. No cooldown text, bar, or other gameplay UI was added.
- Evidence: `Artifacts/Room4LogCooldown/completed-report.json`, `scene-scope.json`. Command: `unity command run_tests --mode playmode --filter RoomSix --async_tests --format json`.
- Runner recovery: initial launch waited on Unity's scene-save dialog, which was resolved without discarding changes; that launch was cancelled. A zero-discovery retry is not counted as success. After `list_tests` refreshed discovery, all 17 executed and passed. Recompile client's reload timeout is not counted as success; independent compile status and new test execution verify the compiled code.
- **NOT RUN:** full human traversal and standalone build; focused tests are appropriate for this timing-only change.


## 2026-11-24 Room 5–6 bridge / prompt / drain update

- **PASS — 155/155 ChapterOneRegression PlayMode tests**, no failures/skips (`Artifacts/Room56Polish/playmode-tests.json`). New tests verify 0→0.88 eased prompt opacity, pause behavior, transition from record/rewind to replay wording, dismissal after use, single functional drain swirl, actual crate sinking/pooling, waterline clipping/material restoration and unchanged tint. Existing acquisition/replay lifetime checks now allow time for complete sinking; no gameplay assertions were removed.
- **PASS — 4/4 ControlPromptTests EditMode**, including WASD preset K and arrows preset X for the new wording (`editmode-tests.json`).
- **PASS — real Unity compile status**, idle/no errors (`compilation.json`); actual updated code executed. The recompile client's domain-reload timeout is not itself counted as success.
- **PASS — targeted scene patch and visual review:** 23 Room 5 and 29 Room 6 collider components were compared before/after and unchanged. Saved renders show continuous uprights/crossbars and no repeated timber-frame stone feet. Existing bitmap art was reused through two Unity Sprite slices, not replaced. Evidence: `scene-update.json`, `captures.json`, `Artifacts/Logs/chapter1-room5-view2.png`, `chapter1-room6-view1.png`, `chapter1-room6-view3.png`.
- **PASS — scene/build hashes:** only Main_Room05/06 changed (`scene-scope.json`); Room 4 cooldown and all other saved scenes/build settings retained. `git diff --check` passed.
- Commands: `unity command run_tests --mode playmode --filter ChapterOneRegression --filter_type category --async_tests --format json`; `unity command run_tests --mode editmode --filter ControlPromptTests --async_tests --format json`. Initial PlayMode setup threw inside Unity Test Framework PlayModeRunTask after domain reload, before reporting checks; cancelled it, restored saved Room 5, and reran. Only completed reports provide counts.
- **NOT RUN:** uninterrupted human traversal, full difficulty tuning, standalone build. Clean Main_Room05 left open (`ready.json`).


## 2026-11-24 Wooden exit signs

- **PASS — live Unity compilation:** recompile_status reports idle, failed=false, compilationFailed=false, errors empty; helper executed successfully. Initial recompile client timed out during domain reload, so that response alone was not treated as success. Evidence: Artifacts/ExitSigns/compile-status.json.
- **PASS — targeted placement:** each of Rooms 1–8 was patched twice, with exactly one exit sign and no sign colliders. All pre-existing Collider2D and MonoBehaviour serialized values compared equal before/after; a failed comparison would abort before saving. Evidence: placement.json. No room rebuild was run.
- **PASS — rendered review:** room1-exit.png through room8-exit.png inspected at a 20-unit-wide view. Post feet align with the authored ground; Room 7 arrow fits below the camera top; Room 8 sign is readable on the cave floor beyond the water. Actual transparent alpha confirmed; 64px point-filtered Unity imports; sorting order 5 below goat and foreground water. Evidence: render-review.json. Goat positions in captures were posed for scale only and not saved.
- **PASS — scope:** scene-scope.json shows changes only to Main_Room01–08 among existing scenes/build settings. No tutorial/intro/debug scene or gameplay edits. git diff --check passed.
- **NOT RUN:** new gameplay suite, human traversal, standalone build; this change adds decorative SpriteRenderers only. Saved scene setup restored cleanly.


## 2026-11-24 Ability pickup sizing

- **PASS:** live Unity compilation reports no errors; `AbilityArtTests` 7/7 passed, including artwork loading/transparency, preserved collision/aspect/root, duplicate prevention and actual recording-item acquisition. Command: `unity command run_tests --mode playmode --filter AbilityArtTests --async_tests --format json`. Evidence: Artifacts/AbilityPickupSize/playmode-tests.json and compile-status.json.
- **PASS:** Room 2 replacement applied twice with stable 1.35 × 1.35 sprite canvas (the user revised the original doubled-size request to about 90% of the goat), unchanged pickup root position/scale and ShutterAbility serialization. New camera saved in Main_Room02; legacy purple renderers disabled. A visual-only 0.35-unit lift clears the grass. RoomTwoBuilder retains this placement for future generation. Evidence: room2-update.json, clearance.json, final-size.json, room2-pickup.png.
- **PASS:** git diff --check. No bitmap/import/UI changes or new tests were needed. **NOT RUN:** human full traversal or standalone build. Initial scene patch was blocked by the user's active Play preview; stopped Play mode, then saved successfully.


## 2026-11-24 Room 7 waterfall width

- **PASS:** Unity compilation idle/no errors; both saved WaterFeature widths are 1.5 units after reload, with roots at x=0 and18.5. Prefab overrides recorded for sizes/renderers/transforms.
- **PASS:** 119 pre-existing Collider2D and non-water MonoBehaviour serialized values unchanged; no collision or hazard expansion.
- **PASS:** normal 20-unit-wide view inspected; two wider edge curtains are visible inward. Height/outer room edges unchanged. RoomSevenLayout generator matches saved scene.
- Evidence: Artifacts/Room7WaterfallWidth/update.json, compile-status.json, verification.json, preview.png. git diff --check passed.
- **NOT RUN:** new gameplay tests, manual traversal, standalone build (art-only sizing change).


## 2026-11-24 Room 7 outer waterfall coverage

PASS: both saved waterfalls reload at width2.5, west root−1/east18.5; interior edges remain1.5/18.5. Twenty-unit camera render inspected: water covers both outer screen edges. Unity compiled without errors; 119 collision/gameplay components preserved. Builder matches saved scene; git diff --check passed. Evidence: Artifacts/Room7WaterfallEdges/{update,verification,compile-status}.json and preview.png. NOT RUN: gameplay suite, human traversal or standalone build (decorative adjustment only).


## 2026-11-24 Room 7 endpoint waiting

- **PASS:** real Unity compilation, no errors. The completion step now clears InTransit while retaining Active, preserving the completed deck's collider and art. Existing Launch resets position and carry delta only on a valid new launch-plate press.
- **PASS:** `unity command run_tests --mode playmode --filter RoomSeven --async_tests --format json`: 22/22 checks passed. Covers the goat standing on the saved Room 7 endpoint, fresh-press reuse, held/early press rejection, rewind of moving and parked decks, reset, existing camera/reveal mechanisms and room transitions.
- Initial run: 21/22; the new scene assertion compared the platform's center with its top surface (0.09-unit difference). Corrected the assertion to collider bounds; completed rerun passed. Evidence: Artifacts/Room7PlatformWaiting/first-run.json and playmode-tests.json.
- **PASS:** git diff --check. No saved scene or art edits were necessary. **NOT RUN:** uninterrupted human traversal or standalone build.


## 2026-11-24 Room 7 five-second countdown

NOT RUN: tests, gameplay verification and standalone build, per the user's explicit instruction to add the features and leave testing to them. Runtime/builder and Main_Room07 updated for five-second endpoint expiry with a centered countdown and a two-unit increase to the upper-west small deck. Existing tests retain the previous endpoint-wait expectations; no test edits made this turn.
