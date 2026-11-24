## 2026-10-31 記憶解謎更新

現行第一大關已加入ChapterMemory：定格庭院、雙影水閘、搭上昨天三種解謎，E/Q記憶、F年代、X互動。場景入口與生成器不變；詳CHAPTER_ONE.md及最新TEST_REPORT.md，以下為舊版歷史。

# 2026-10-31 第一大關更新

現行第一大關為 Chapter01_EchoFoothills，六區連續384單位，兩年代、兩機關、三照片、11旗幟。完整正常馬達路線已通過；首次遊玩4–5分鐘尚待試玩。以下四小關保留為舊設計。詳 CHAPTER_ONE.md。

---

# 2026-09-22 能力拾取入口

L01：spawn+(1.7,0.85) 定格懷錶，拾取前 E 鎖定。L03 同位置為回溯沙漏，拾取前 Q 鎖定。L02 已有 E，L04 已有 E/Q；關卡平台、出口與原解法不變。拾取開啟教學並停止模擬，閱讀完畢才繼續。R／死亡保留當關解鎖。

---

# 2026-09-21 四關挑戰版

本次明確需求取代舊白盒的座標與難度，保留同一份記憶、兩輪數值、10 秒及 E/Q 手動流程。生成來源：`Assets/_OneMoreTime/Editor/ChallengeLayouts.cs`；`Apply Four Challenge Layouts` 是明確的重置配置命令，先備份原 Config；一般 Generate 仍保留 Inspector 修改。

| 關卡 | 新解題結構 | 失敗與重新思考 |
|---|---|---|
| L01 留在半途 | 起岸 x≤7，第一高台 x11–18／y4.3，出口岸 x21–29。朝裂谷跳出，留下中等高度定格；下一輪登上殘影，再跨到高台，最後越過第二道缺口。 | 地面定格高度不足；最高點定格踩不到；太遠的位置下一輪無法登上。R 可保留位置反覆試跳。 |
| L02 一個位置，兩個任務 | 先跳過入口缺口，登上 y1.4 石座；在 x12.8 的 A 踏板定格。同一殘影開啟封頂門，並把起跳面升至 y2.3，才能登上 y4.9。門後連續兩道 3 單位缺口。 | 懸空定格無法壓板；在踏板外定格無法開門。普通角色離板立即關門。 |
| L03 搭上移動的昨天 | 左岸 x≤12，右岸 x16–22，高崖 x23／y6.3。左岸 Q 開始，留上車等待、跳過裂谷、走到右岸最右、等候、再長跳、落地 Q 保存。利用輪搭乘這段移動，再從最後一跳高處起跳。 | 省略等待難以上車；太早下車會失去接力高度；單一定格無法登上高崖。最後不是永久平台。 |
| L04 等我，再回來 | 下層 A(x6)→B(x16)→A，分別等候；上層三門 x11／25／35 對應 A／B／A。兩段斷橋分成離散崩塌平台；中間與末段各有永久安全島。 | 只錄 A→B 會困在最後 A 門；不等待的短窗口難以匹配上層。不能一直向右走，需在斷橋上跳躍，在草地等候。 |

L04 在下層 A 站妥再開始 Q，可將整段往返留在 10 秒內。第三扇門重用 A，沒有新能力或新記憶欄位。`doorSources` 明確配置每門來源；線路及門上的 A/B 字母與實際引用一致。

驗證：新版 42/42 PlayMode、8/8 EditMode PASS；四關正常解皆透過 InputFrame，無位置傳送。正常軌跡：L03 約 7.92 秒，L04 約 9.38 秒。完整輸入時間軸見 Artifacts/Logs/solution-L01.json 至 solution-L04.json；這是已知解法的快速執行時間，不是新玩家解謎時間。

負向測試：L01 無記憶／地面定格／過高定格；L02 單人離板／懸空定格；L03 12 組定格時機；L04 無記憶／分別定格 A 或 B／無等待／只 A→B 未返回／下層借殘影撞分隔。有限路徑測試不代表窮舉所有操作；新手難度評估尚未執行。

---

# 關卡與實測

Unity 6000.6.0f1 PlayMode MotorTests.MeasureJump，dt=0.02。實測 H=2.92320、D=5.16000（全速平地長跳）、B=.90。證據 Artifacts/Logs/jump-measurement.txt 與 playmode.xml。

L01 初始高台 y=4.5 > H；地面到 x=13，建議 x=12、腳底 y≈1.5 定格，背面≈2.4；第二跳可達高台。正常輸入解法與四個起跳點的無記憶挑戰通過（final-playmode.xml）。

L02：高台 3.5 > H，踏板位於 (10.8,0)，背面 .9 + H = 3.8232 可登台。門 x=13，頂部與 y=7 天花板相接。正常輸入解法與空中離板測試通過（stage4-playmode.xml）。

L03：平台高 6.3，H×2=5.8464 < 6.3 < H×2+B=6.7464。實際錄製：走到 x≈12.7、等 2 秒、長跳、落地等 .5 秒、Q。利用輪到搭乘區後 Q，等殘影抵達後跳上，接近頂峰再次跳向右方。真實 IPlayerInput 語義輸入通關（stage3-playmode.xml），不注入快照。12 組固定記憶高度掃描通過，範圍詳見下方。

L04 最終版：出生 (2,5)，下落入口 x=3–5，上層 y=5，下層 y=0。實體隔板 x=5–37、y=3.5–4.5，阻止殘影由門後返回上層。A/B=(6,0)/(18,0)，門 x=16/30 延伸至 y=10 天花板，出口 x=34。崩塌 x=19–23，下面 y=4.45–4.73 危險條，不侵入下層錄製空間；x=23–30 為永久安全點。正常錄製 A 等約 2 秒、B 等約 3.2 秒，總長少於 10 秒。

## Stage 7 的真實修正

過短等待的測試揭露：舊 L04 的上層路程太短，快速經過 A/B 也能完成。沒有降低測試要求；改成門 A x=16、門 B x=30、出口 x=34，崩塌 x=19–23、安全區 x=23–30；分隔與外牆延伸至 x=37。下層 A/B 與角色數值不變。修正版正常等待通關、無刻意等待失敗，見 stage7-playmode.xml 的 TimingNormalSolution／TimingWithoutWaitingFailsAtRequiredDoors。

L03 防繞關掃描：freezeTick=2,4,…,24，在實際長跳不同高度按 E，利用輪嘗試踩背再向出口長跳；12 組均未通關。結合固定頂面可達性上限 2H+約 .02 < 6.3。這是針對性測試與幾何界限，不聲稱枚舉所有連續輸入。

## 2026-11-18 Main Room 4

Room 3 lift now enters `Assets/_OneMoreTime/Scenes/Main_Room04.unity`. Debug Mode entry 09 opens it directly. Basic replay acquisition breaks the bridge and holds the falling player near water until recording actually starts. The remainder is a water/platform route with six pooled, rewindable crates spawning every 3 seconds and drifting left at 1.3 units/s. Room dimensions and platform spacing preserve current player tuning. See [ROOM_FOUR.md](ROOM_FOUR.md) for coordinates, tuning, assets, regeneration, first-time persistence and validation. Rooms 1–3 layouts remain unchanged; Room 5 is not authored.

### Room 4 revision: checkpoint and water volumes
The statue now sits at (26,4), three units left of the first bank's right edge (*6). Main ground masses and grassy columns have visual soil down to y=-3. Three water sections have full-depth non-colliding tile fills; lethal volumes begin below the surface. Wall ledges project horizontally from the banks. The first-time fall slows before holding 0.08 units above water; actual Q recording releases the hold. There is no first-pit crate; the upper-channel source starts only after tutorial completion. See ROOM_FOUR.md for exact changes and TEST_REPORT.md for 8/8 Room 4 tests and remaining batch regression failures.

### Room 4 latest dot-grid sketch
Supersedes previous wall-step arrangement: only *6 pure-dirt bank bevel and far-right grassy bank bevel. Main-room-style grass caps/pillars, wooden lift landing x0.5–3, entrance dirt begins x3. Upper channel x40–64, crate source x63.5, speed -1 unit/s, interval 8 s, limit 6. Details and validation: ROOM_FOUR.md latest correction and TEST_REPORT.md (9/9 focused tests).


## 2026-11-19 Main Rooms 5–8 first pass

Room 4 now continues through `Main_Room05` → `Main_Room06` → `Main_Room07` → `Main_Room08`. Debug entries 10–13 open these rooms directly. This supersedes the older “Room 5 is not authored” note above.

- **5 — Broken crossing:** 70-unit-wide U-shaped river and alternating present/past suspension-bridge remnants, with two shared checkpoint spans.
- **6 — Twin falls:** central climb from y=1 to y=40, waterfalls framing both sides, a shutter-powered moving platform, and three checkpoint rests.
- **7 — Remembering river:** three 15-unit channels crossed by recording and reversing floating logs/rafts. Reverse replay unlocks on the first bank. Sources use 2 units/s flow and a 10-second interval.
- **8 — Grand Waterfall:** 120-unit diagonal ascent, era bridges, shutter lift, collapsing bridge, timed platform, replay raft, final plate/gate and cavern behind a foreground waterfall. Cooler rock and a dark interior mark the Chapter 2 threshold.

These are editable provisional layouts for later detailed drawings. Chapter 2 currently has an ending/teaser only. Design, route coordinates and intended solutions: [ROOMS_5_TO_8.md](ROOMS_5_TO_8.md).


## 2026-11-23 Room 5 complete layout

Replaced the provisional Room 5 with IMG_8932 at one dot interval per unit: a 90-unit-wide stepped U river; solid red bridge fragments and noncolliding orange background spans; two grassy islands with soil reaching below water; one central checkpoint; a plate-only horizontal shuttle; and crates flowing right at 2.5 units/sec every 10 seconds into a visible sinking whirlpool. The main game retains shutter/basic replay only. Dark timber scaffolding is decorative and adds no collision. No extra stepping ledges were added. The 20-unit camera, open top and Room 4 → 5 → 6 links remain. Coordinates and source: [ROOMS_5_TO_8.md](ROOMS_5_TO_8.md#room-5--broken-crossing-complete-drawing), `Editor/RoomFiveLayout.cs`. This replaces the older Room 5 first-pass description.

## 2026-11-23 Room 6 official layout

`Main_Room06.unity` now follows 黑板 12, replacing the old twin-waterfall vertical prototype. River x=8–92 at y=0; the 97-unit room retains a 20-unit camera and open upper boundary. Entrance plate releases one 2-unit/sec floating log per activation; the eastern plate moves a solid deck six units right at 12 units/sec and releases it back home. Red bridge fragments collide, orange spans/scaffolding are background only, and the two grass columns use thin one-way tops with noncolliding dirt. Only marked platform locations were authored. The drawing has no statue, so retry uses the entrance. Final landing y=15.3 leads to an automatic upward lift and Room 7. Full coordinates, runtime variables and debug entry are in `ROOMS_5_TO_8.md`; 89/89 Unity regressions passed, full human route/balance playtest pending.

## 2026-11-23 Room 7 official tower layout

`Main_Room07.unity` replaces the horizontal river with IMG_8935: a 20-unit-wide vertical shaft, side waterfalls, three 6-second loops, a permanently latched two-plate ascent with 0/2–2/2 counter, six 4-second shuttles, two checkpoint bridges, and a 4-second-cooldown dispatched route. The upper-left plate opens the split trapdoor with a paused camera reveal. The door remains open through rewind/retry; moving decks still rewind and wait for replay. Entry arrives by lift from Room 6; the open top shaft fades upward into Room 8. See `ROOMS_5_TO_8.md` for coordinates and editable variables.

## 2026-11-23 Official Room 8 — The Grand Waterfall

`Assets/_OneMoreTime/Scenes/Main_Room08.unity` replaces the provisional finale with the supplied diagonal ascent. Two leftward crate streams run at 2 units/sec with 6-second spawning; two rectangular decks take 6 seconds per lap. The remote high plate reveals a statue carrier that travels (+11,+7) in 10 seconds and parks. Death returns the carrier and selected statue to its safe home. Red bridges collide; orange remnants and their downward beams are decorative. Four grassy shelves use the existing background dirt columns. The 12-unit foreground waterfall hides a dark cave exit at y=47.3.

Open the scene and press Play, or use Main Menu → Debug Mode → 13 Room 8. It follows Room 7 and ends at the existing Chapter 2 threshold. The normal camera stays 20 units wide and bounded, with no head-blocking upper collider. Only shutter/basic replay are available. Builder coordinates: `Editor/RoomEightLayout.cs`; scenery: `Editor/RoomEightScenery.cs`. Full human traversal remains unmeasured; automated mechanism/scene coverage is documented in TEST_REPORT.

### 2026-11-24 Room 8 correction

Arrival now rises beside the bank then lands from above. Removed bridges mistakenly inferred from the whirlpool/current arrows. Both plates at (68.7,29.7) and (95.25,25.7) must be held together to launch the statue carrier; the first still provides its camera reveal. Cave floor y47.3 has a clear rim and no floor-height clutter; the waterfall extends to y62 above the camera's maximum top.
