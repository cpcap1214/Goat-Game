# Chapter 1 — Twelve record/replay puzzle designs

Prepared 2026-11-19. **Design library, not twelve newly built scenes.** These are alternatives and modules for the user's later detailed layouts. They do not replace the provisional Rooms 5–8. Layout dimensions and recording times below are prototype targets, not verified solutions or measured player completion times.

The theme is a forest's abandoned river crossings and waterworks, gradually becoming the machinery beneath the Grand Waterfall. Complexity comes from planning what the recorded goat does while the live goat does something else, choosing replay direction, and understanding which parts of the world keep running.

## Current rules these designs use

- Hold Q for 0.25 seconds to begin a new recording in place. Its first ten seconds are the limit; it automatically finishes at ten seconds. All schedules below start at the first recorded frame, **after** the hold, and leave some margin under that limit.
- Finishing visibly rewinds the captured world to the recording's starting state. It does not send the goat to the level entrance. Put the recording start on safe ground.
- Tap Q to play or restart. Restarting does not reset the live goat. New recordings replace the old one. There is only one saved timeline.
- The replay goat remains at its last frame. A grounded endpoint can hold a plate indefinitely, and the goat can stand on its back. Moving and paused echoes also provide one-way support.
- The later upgrade adds Left Shift: tap to reverse; hold to stop the playback cursor. Basic puzzles cannot assume this upgrade.
- Recorded crates and floating-source pools follow the same cursor as the echo, including direction, holding and spawn/despawn history. They are **not independent tracks**. They keep their original colors and subtle dashed traces.
- Normal moving platforms, release timers, collapsing bridges, doors and F selection remain live during playback. Shift does not freeze these. Only the automatic rewind restores their captured history. F changes during recording are not replayed as commands.
- Current plates accept grounded goats/echoes, not crates or logs. Current doors each reference one plate. AND locks, ordered readers and pumps are explicitly marked as new mechanisms below.
- E and a saved Q trace are not simultaneous helpers in these main rooms. Starting a recording removes the shutter copy; E is blocked while a trace exists. None of these solutions requires both.
- Jump height is 3 units, horizontal speed 6 units/s, collider 0.7 × 1 unit. Log flow is 2 units/s and new logs arrive every 10 seconds. Use 2–2.5-unit ordinary rises as a starting design margin; test actual jumps before finalizing.
- R/death after recording returns to the current checkpoint and retains the recording. R **during** recording finishes it. A new recording begins wherever the goat currently stands, so a clearly marked safe recording ledge matters.

### Readability and fairness

Use existing wood, grass/dirt columns, stone goats, dark inactive bridges, water and foreground-waterfall art. Plate relationships should be readable through carved matching symbols, moving winch ropes and local indicator lights; do not add the removed red connection lines. Timed platforms retain their small centered countdown, which disappears at zero. Water death volumes start below the surface, with fall death below the camera floor. Camera bounds never become invisible ceilings.

All dimensions are rough footprints, not instructions to add invisible barriers. If a gate must block passage, draw a real gatehouse or cliff opening with matching collision. Enclose only the local structure, not the top of the room. Clearly distinguish decorative dark columns from walkable grass tops.

Endpoint parking, Q restarts and E staircase movement are existing powers. Do not silently disable them to rescue a puzzle. Pure traversal modules may deliberately accept alternate shutter solutions. Puzzles intended to require replay need physical route separation and several remote actions; their bypass resistance still requires playtesting.

## Selection guide

| ID | Puzzle | Main insight | Ability tier | Suggested location | New runtime mechanism? |
|---|---|---|---|---|---|
| P01 | The Two Tollhouses | Record useful waiting in two places | Basic | Room 5 approach | No |
| P02 | A Bridge from Yesterday | A recorded path survives a change of era | Basic + F | Room 5 broken bridge | No |
| P03 | The Winchkeeper | The echo operates machinery while you ride | Basic | Room 6 lower climb | No |
| P04 | The Relay Climb | Plate release time differs from recording time | Basic | Room 6 upper climb | No |
| P05 | Against the Current | Reverse a log's journey into transport | Upgraded | Room 7 introduction | No |
| P06 | Two Different Clocks | Park the recording; let the lift keep moving | Upgraded | Room 7 middle | No |
| P07 | The Timber Convoy | One cursor controls several vehicles together | Upgraded | Room 7 harder crossing | No |
| P08 | Falling Upward | Record a descent to obtain a climbing route | Upgraded | Room 8 lower shaft | No |
| P09 | The Staggered Locks | Two bodies must meet different pairs of conditions | Upgraded | Room 8 waterworks | AND gate with release grace |
| P10 | The Scrambled Sluice | Select which recorded contacts count, in a new order | Upgraded | Room 8 optional challenge | Sampled sequence reader/latch |
| P11 | Let the Old Memory Go | Replace a useful recording only after reaching safety | Basic; upgrade for harder variant | Room 6 or Room 8 rest | No |
| P12 | The Grand Waterfall Engine | Repeated playback motion drives lasting world progress | Upgraded | Room 8 cavern finale | Stroke counter, routing and powered outputs |

Use these as a library. A first chapter pass could select P01/P02 for Room 5, P03/P04 for Room 6, P05/P06 for Room 7 and P11/P12 for Room 8. P07–P10 are harder substitutes or side routes, not a requirement to cram all twelve into the current rooms. P12 needs its listed mechanism work before it can replace the existing finale.

## P01 — The Two Tollhouses

**Question for the player:** How can someone operate the lower controls while I cross the upper bridge?

**Layout:** Roughly 20 × 9 units. A safe lower maintenance path contains plate A and plate B, approximately 6 units apart. The upper broken bridge has gate A, a permanent waiting island, then gate B. The upper starting perch is reachable before playback. The two control pads are not on the upper route.

```text
upper:  start ── gate A ── safe island ── gate B ── exit
lower:          [A] ───── 6 units ───── [B]
```

**Recording target — 5.8 seconds:** Stand on A for 0–2.2 s; walk to B during 2.2–3.4 s; stand on B until 5.8 s and finish. A and B each light their matching gate when tested during exploration.

**Playback solution:** Reposition onto the upper starting perch after automatic rewind. Play. Cross gate A while the echo holds A, wait on the island, then cross B when the echo arrives. Its grounded endpoint may keep B open; that is an intended reward for ending the clip usefully.

**Why it is a puzzle:** A recording of running straight across the pads produces poor windows. The useful information is the waiting, not the distance traveled. A single stationary image cannot service both remote gates while the live goat remains on the upper route.

**Failure and recovery:** A missed window leaves the goat on the island. Q restart supplies another attempt; no drowning is required. Checkpoint before the upper entry.

**Build notes / checks:** Existing plates and individual `LinkedDoor` components suffice. Use real enclosed gate openings to prevent hopping over their tops. Check both windows with normal acceleration, prevent a live goat on the upper route from reaching a lower pad through its floor, and retain the door's existing anti-crush behavior.

## P02 — A Bridge from Yesterday

**Question:** Can I travel along a bridge that no longer exists?

**Layout:** A 12–15-unit missing middle span in the bottom of Room 5's U. Past view reveals a continuous old bridge; present view reveals the route beyond it. Permanent dry banks at both ends are available in both eras. Keep the replay path free of new present-day walls.

```text
past:     west bank ═════ old bridge ═════ east bank
present:  west bank ~~~~~ open river ~~~~~ exit ledge
memory:             ··· walking goat ···
```

**Recording target — 5.8 seconds:** In past view, wait at the west end for 0–1.2 s; walk across during 1.2–4.4 s; wait safely on the east bank until 5.8 s. Finish. Automatic rewind restores the starting era as well as position.

**Playback solution:** Switch to present on the permanent west bank. Play and board the echo during its initial wait. Ride its fixed world-space route over the absent bridge, then jump to the present exit. Do not expect the recording to press F for the player.

**Why it is a puzzle:** The memory describes where the goat went, not which bridge currently supports it. This makes the past-view filter and replay interact in a visible, thematic way.

**Failure and recovery:** Bank checkpoint and a below-surface water hazard. A failed boarding can be retried with the saved trace. Replaying from a dry bank avoids attempting to ride a restart teleport.

**Build notes / checks:** Existing era groups and echo carrying suffice. Keep the inactive bridge dark and visibly incomplete. This is a free-form traversal puzzle: a clever sequence of E placements may also cross it under current rules. Accept that alternate route, or place this as an optional/reward route; do not claim it is a strict replay lock. Verify live geometry does not block a passenger while the echo passes through it.

## P03 — The Winchkeeper

**Question:** Who keeps the winch running after I step onto the lift, and who opens the gate above?

**Layout:** About 18 × 13 units. Lower plate A powers a 3-unit-wide lift traveling 4.5 units vertically at 2 units/s. Lower plate B, 6 units away, opens an upper gate. A permanent upper landing lies before the gate. Include a dry boarding shelf next to the lift's bottom position.

**Recording target — 7 seconds:** Start on neutral ground. Reach A during 0–0.4 s; hold A during 0.4–3.4 s; walk to B during 3.4–4.6 s; hold B until 7 s and finish.

**Playback solution:** After rewind, wait on the boarding shelf and play. Board when A activates the lift. Get off near its first upper pass onto the permanent landing. The echo then leaves A and reaches B, letting the live goat through the upper gate.

**Why it is a puzzle:** The helper changes jobs. Leaving a permanent shutter on A can run the lift, but cannot also operate the remote lower B pad once the live goat is upstairs.

**Failure and recovery:** Missing the lift's top pass should return the goat toward the boarding shelf, not instantly kill it. A top landing prevents waiting for B on a platform that disappears when A is released. Put a checkpoint beyond the gate.

**Build notes / checks:** Existing `RoomPlatform`, plates and door. The mover ping-pongs while active and holds its position when inactive; it does not automatically return home. Recording must start before A is pressed so the captured starting phase is understandable. Measure the first top-pass timing, boarding jump and gate window. Do not assume Shift pauses the lift in a later upgraded variant.

## P04 — The Relay Climb

**Question:** How much time should my recorded self spend on each control?

**Layout:** A 20 × 15-unit ruined staircase with a safe control path below it. A operates the first pair of climbing platforms with a 3-second release grace. B operates the next pair with a 6-second grace. A permanent landing separates the two sections. Normal rises are approximately 2.2–2.5 units.

**Recording target — 5.6 seconds:** Reach A by 0.2 s; hold it until 2 s; travel to B during 2–3.2 s; hold B until 5.2 s; step onto a neutral end pad and finish at about 5.6 s.

**Playback solution:** Climb the first section while the echo holds A and during its release grace. Reach the permanent landing around the handoff to B. Use B's longer grace to complete the upper section. End on the permanent exit, not on a fading platform.

**Why it is a puzzle:** Recording duration and platform lifetime are different clocks. A brief pad visit may be enough if it happens at the right point; a very long wait at the wrong pad can be worse.

**Failure and recovery:** The intermediate landing allows Q restart without returning to the room entrance. If a player misses a timer, lower recovery shelves should catch them where feasible. Small centered numbers show the actual remaining platform time and disappear at zero.

**Build notes / checks:** Existing release delays suffice. The numbers above are a tuning seed, not a proven route. Ending the clip on B instead is a valid easier solution; it makes the second section permanent while leaving the A section's timing meaningful. Repeated Q restarts can refresh the sequence; treat that as recovery. Verify this module is not just P01 with different art: its challenge must be choosing dwell time to cover a vertical route, with safe places to observe the countdowns.

## P05 — Against the Current

**Question:** Can I turn the river's motion into a ride in the opposite direction?

**Layout:** A 15-unit channel, a safe bank on either side, and one approaching log. Grant the reverse upgrade on the first bank. Retain 2 units/s flow and 10-second spawn cadence. This formalizes the working Room 7 first-pass crossing as a reusable teaching puzzle.

**Recording target — about 7 seconds:** Wait safely on the bank while a log moves from near the far side to near the boarding point. Finish before it is disposed of or leaves useful water. The recording needs to contain both the near-bank boarding position and the far-bank dismount position.

**Playback solution:** Play forward until the log approaches. Board, tap Shift to reverse, and ride its recorded journey upstream. Hold Shift if needed while preparing the far-bank jump. Finish on the far bank; the parked endpoint is safe to use.

**Why it is a puzzle:** The recording captures the object as well as the goat. The player deliberately records an apparently unhelpful direction because the upgraded replay can reverse it.

**Failure and recovery:** A checkpoint on each bank. Missing a log means waiting or replaying, not rebuilding a whole route. Q restart should occur while standing on a bank, because its object repositioning is not meant to carry the goat across the river.

**Build notes / checks:** Existing floating-source playback suffices. The already built Room 7 has a passing actual-controller test of the underlying boarding/reverse-carry/dismount pattern; this proposed puzzle's final dimensions are still subject to its own playtest. Do not let the tutorial recording include the chosen log's disappearance before its boarding endpoint. Do not tint the log to communicate replay.

## P06 — Two Different Clocks

**Question:** What keeps moving when I hold the replay still?

**Layout:** Roughly 22 × 14 units. A log crosses a river beside a continuously moving vertical service platform. The upper exit is reachable from the service platform, but not directly from the log. A clearly visible mooring post marks the useful midstream boarding position. The recorded log's two endpoints lie away from this post.

**Recording target — 7–8 seconds:** Capture the log passing the mooring post and continuing beyond it. The goat can wait safely throughout the recording. No recorded interaction is required to power the normal lift; it runs continuously.

**Playback solution:** Ride or reverse the log to the marked position, hold Shift, and wait. The log stops while the live service platform continues its cycle. When the platform reaches boarding height, jump onto it, release Shift after boarding, and ride upward.

**Why it is a puzzle:** There are two clocks. Pausing the transport recording creates a stable place to wait for machinery that is not part of cursor playback. Trying to reverse the lift along with the log will not work.

**Failure and recovery:** Give the mooring area a low rescue dock or a short return to the bank. The normal lift's full movement should be visible from the waiting position. Avoid a jump that requires reacting within one or two frames.

**Build notes / checks:** Existing floating source plus an always-active `RoomPlatform`. Do not add an invisible world pause. A deliberately shorter replacement recording ending beside the mooring post is a valid alternative to holding Shift. Verify that both objects remain readable and that the held log does not drift while the live lift moves.

## P07 — The Timber Convoy

**Question:** How do I use a second recorded vehicle when moving its timeline also moves the first?

**Layout:** Two staggered river terraces with one permanent dry transfer island. The lower log connects the entrance to the island; an upper raft connects the island to the exit. Both currents still move left at 2 units/s. Their starting positions differ so the two useful island arrivals occur at different timestamps. There is no direct jump from the first log's farthest point to the exit.

```text
upper:                   raft ←──────────── far bank
                         [dry transfer island]
lower:  entrance ───── log ←──────────
```

**Recording target — 8.5 seconds:** Record both pools together from safety. A useful initial phase sketch is lower log x=20−2τ, upper raft x=32−2τ, with the dry transfer near x=17. These are path-planning coordinates; the terrain and water hazards must be fitted around safe boarding surfaces. At τ≈1.5 the lower log is near the transfer; at τ≈7.5 the upper raft is near it.

**Playback solution:** Board the lower log when it reaches the entrance, reverse to take it to the transfer island, and dismount. Resume forward playback while waiting on the island until the upper raft arrives. Board that raft, then reverse again to reach the far exit. Every direction change affects both recorded vehicles.

**Why it is a puzzle:** There is one shared timeline, not two independently controllable ferries. The fixed island lets the player stop being attached to one object while advancing the world recording to another useful arrangement.

**Failure and recovery:** Put a checkpoint on the dry transfer island, with enough space to stand away from either vehicle. Starting playback after a retry may reposition both craft; wait safely for the correct one. Losing the now-unused first log later in the recording is acceptable; losing the second before boarding is not.

**Build notes / checks:** Existing multiple `CrateSpawner` participants suffice. This needs a geometry prototype: dry approaches must not cross the upper stream's lethal volume while boarding, and the vertical transfer jumps must remain within the actual jump envelope. Do not draw a safe pier inside an overlapping water hazard. Verify phase offsets after rewind, retry and Q restart; do not fix a bad phase by giving each craft an independent clock.

## P08 — Falling Upward

**Question:** Can I record a safe downward route and use it as an upward route?

**Layout:** A waterfall shaft about 14 × 18 units. Three one-way maintenance perches form a descending route, each roughly 2–2.5 units lower than the previous. The player can explore the descent first. A separate upper exit is reached by jumping from the rising echo. Provide a dry bottom boarding shelf and unobstructed headroom along the recorded path.

**Recording target — 6.5 seconds:** Wait on the top perch for 0–0.8 s; descend to the middle by about 2 s; wait until 2.8 s; descend to the lower perch by about 4 s; wait until 4.8 s; descend to the bottom by about 5.7 s; wait and finish at 6.5 s. Keep landings in the recording rather than making one fast, unreadable free fall.

**Playback solution:** After automatic rewind, take the normal descent to the bottom boarding shelf without changing the recording. Select reverse before playback so the echo begins at the bottom endpoint. Board during its reversed bottom wait, ride upward, and use Shift holds at useful rests before making the final jump.

**Why it is a puzzle:** The recorded fall becomes transport. Resting on the way down creates deliberate boarding and decision windows on the way up.

**Failure and recovery:** A bottom checkpoint makes boarding repeatable. Keep the shaft's water curtains visual unless their hazard is explicitly visible and avoid placing a lethal volume through the intended ride. Do not force the player to catch a maximum-speed falling echo.

**Build notes / checks:** Existing echo support and reverse playback. The echo can pass through geometry, but its live passenger cannot; use one-way perches and test every rising segment for head/side obstruction. E staircase alternatives are possible in an open shaft under current rules. Treat this as an expressive traversal challenge unless a later drawing supplies additional cooperative conditions.

## P09 — The Staggered Locks

**Question:** What if my helper must visit the controls in a different order from mine?

**Layout:** A lower control walk with pads A, B, C and an upper zigzag route containing three paired locks. Upper stations are encountered in order B, A, C. Each lock has a local live-goat pad and requires its matching remote lower pad simultaneously. Two carved halves on each arch show which condition is missing. The lower stations are physically separated from the upper route.

**Recording target — 8 seconds:** Reach A by 0.3 s and wait through 1.8 s; walk to B by 2.8 s and wait through 4.3 s; walk to C by 5.3 s and wait through 7 s; step to a neutral endpoint and finish by 8 s. Size the inter-pad walks to fit these budgets with normal acceleration.

**Playback solution:** At upper B, play forward until the echo holds lower B, then hold Shift while the two halves engage. Cross the released gate. At upper A, reverse back to A and repeat. At upper C, play forward again to C. The required cursor path is B → A → C, not a single start-to-end run.

**Why it is a puzzle:** The player is navigating a recording as well as a physical route. Neither one stationary echo nor blindly following the original plate order serves all three locks.

**Failure and recovery:** Each lock leads onto permanent ground. Reaching a wrong timestamp merely leaves a gate closed. Use a small activation dwell of about 0.5 s and 2 s of release grace so stepping off the live pad to cross is comfortable.

**New mechanism required:** An AND gate combining two existing pressure plates, with release grace and the same occupied-door safety as `LinkedDoor`. Capture its grace/open state for automatic rewind and retry. No extra echo, source-specific plate or new player ability is required. The normal current door cannot implement this by assigning two references to its single plate field.

**Checks:** A single pad never opens the arch; Shift hold preserves the remote occupancy but does not stop the gate's release grace; an endpoint cannot accidentally overlap two lower pads; Q restart has readable recovery. Prototype the upper route so a live goat cannot simply walk down to every remote pad between locks.

## P10 — The Scrambled Sluice

**Question:** What if passing a control and choosing to operate it are two different things?

**Layout:** About 20 × 17 units. A descending maintenance chute has contact pads A → B → C. A distant safe master-valve station is accessible after rewind. The exit's stone diagram reads C (release pressure) → A (unlock) → B (engage). The chute's drops make ascent inconvenient, but the actual requirement is simultaneous operation: the live goat must operate the master while the helper occupies a reader. Each pad has a matching carved symbol, so the diagram does not depend on colors alone.

**Recording target — 7.5 seconds:** Pause on A for 0–1 s; descend to B by 2.8 s and pause until 3.8 s; descend to C by 5.8 s; wait grounded on C and finish at 7.5 s. Record actual contact at every pad, not an airborne crossing.

**Playback solution:** Stand at the master valve. Select backward playback before pressing Q, hold at the C endpoint and press X at the console to register C. Reverse toward A, passing B without operating the console. Hold on A and press X. Then play forward to B and press X again. The completed sequence releases the sluice's latch; the live goat can leave the master and pass the exit.

**Why it is a puzzle:** The recorded goat supplies a position, while the live goat chooses when that position counts. A single forward or backward run does not perform the requested sequence automatically. X is a present-time interaction at the console, not an input being replayed from the recording.

**Failure and recovery:** A wrong reader resets partial progress with an obvious local indicator, without killing the player. The master is permanent safe ground; the saved clip can be restarted. The completed exit remains latched for that attempt.

**New mechanism required:** A sampled sequence reader with master-console eligibility and a completion latch. On a fresh live X press, sample the replay echo's current grounded pad, requiring a brief stable dwell. The console must only accept an active saved replay, not the live goat, shutter, path line or recording in progress. Reset partial progress on a playback restart/new recording. Add explicit interaction and playback-start/restart integration; current runtime does not expose this whole puzzle behavior as a ready-made component. Capture reader/latch state for automatic rewind and normal retry.

**Checks:** Holding X or standing on C cannot submit repeated entries; passing B without X leaves the sequence untouched; a wrong submission clears only partial progress, not the clip; leaving the console cancels incomplete progress. Q restart cannot carry partial completion across a discontinuity. Reversed falling samples must reach the real reader positions without requiring the live goat to follow. There is no countdown: the challenge is choosing and ordering useful moments.

## P11 — Let the Old Memory Go

**Question:** When is it safe to replace the recording that brought me here?

**Layout:** Two connected challenges separated by a broad permanent checkpoint ledge. The first recording powers an entry lift; the second must run between two upper controls that did not appear in the first clip. The old lift is visible below the checkpoint so the consequence of clearing its helper can be seen safely.

**First recording — about 4 seconds:** Walk onto the lower lift plate, wait grounded, and finish there. Replay parks the endpoint on the plate while the live goat rides to the intermediate ledge.

**Second recording — about 6.8 seconds:** Once fully on the permanent ledge, begin a new recording from its safe anchor. Hold upper A for 0.4–2.6 s, walk to upper B during 2.6–3.8 s, then wait there until 6.8 s and finish. A and B operate the second section's two separate mechanisms. This is two clips used successively, never a fourteen-second clip or two stored memories.

**Playback solution:** Use the first endpoint to reach the checkpoint. Step completely off the lift before holding Q again. The old echo disappears and its lift deactivates, but the goat stands safely on stone. Record the upper procedure, rewind to that ledge, then use the second echo to leave.

**Why it is a puzzle:** A useful old solution can become the obstacle to learning the next one. The player must recognize a stable place from which they can afford to replace it. The visual disappearance of the lower support makes the one-memory rule concrete.

**Failure and recovery:** Save the checkpoint before overwriting. Starting a new recording while still on the disappearing lift should cause a short recoverable fall, not erase all room progress. A new clip must begin with the goat grounded on the permanent anchor.

**Build notes / checks:** Existing checkpoint and recording replacement behavior. Test R both after the second recording and after a failed attempt to begin it: the latest saved recording must remain usable from the checkpoint. Do not place a checkpoint on the powered lift itself. Do not add a confirmation dialog or secretly preserve the previous trace. An upgraded variant can replace the first lift with a replay raft, while retaining the same safe-handoff principle.

## P12 — The Grand Waterfall Engine

**Question:** Can a short recording do useful work repeatedly while I direct the result elsewhere?

**Layout:** A large old waterworks at the final waterfall. A lower maintenance path contains two piston-contact plates A and B, about 6 units apart. The upper live-goat route contains a left reservoir selector, an intermediate walkway, and a right reservoir selector. Existing waterwheel, timber, stone and waterfall art explain the mechanism. Two physical gauges show progress; the cavern remains behind the foreground curtain.

```text
live route:  [left selector] ── powered walkway ── [right selector] ── cavern
machine:          left gauge                         right gauge
recording:                  [A] ⇄ [B]
```

**Recording target — 3.6 seconds:** Wait grounded on A for 0–1 s; walk to B during 1–2.3 s; wait on B until 3.6 s and finish. It is deliberately short: the later work comes from moving back and forth through this same recording, not from exceeding ten seconds.

**Playback solution:** Stand on the left selector and play A → B, completing one pump stroke. Reverse B → A for a second stroke. The left gauge reaches two and opens a permanent intermediate walkway. Hold playback while moving along it to the right selector. Resume A → B → A → B to supply three strokes to the right gauge. This opens the cavern gate and activates the final approach. Release the selector and walk behind the waterfall.

**Why it is a puzzle:** Cursor motion is reversible, but the machine's useful work can accumulate in the live world. The player routes that work while the echo acts as a reciprocating operator. This creates a finale about coordinating a system rather than performing another longer jump sequence.

**Failure and recovery:** Start with gauges that do not leak. A stroke without a selected reservoir is visibly vented and produces no progress; the clip remains available. The first completed walkway stays open for the attempt so changing selectors is safe. A checkpoint before the engine avoids repeating the entire diagonal ascent. No timer is necessary to make the base puzzle interesting.

**New mechanisms required:** A debounced stroke reader accepting complete continuous A→B or B→A traversals; a live-selector router; two integer progress gauges; and outputs for the walkway and final gate. New playback restarts/recording replacement must reset the partial stroke so teleporting between endpoints cannot pump for free. These components must implement rewind capture: automatic rewind restores the recording-start gauge state, while ordinary forward/reverse playback leaves completed pump work live. The progress is machinery state, not a second recording. Actual variable-depth water is not required; animated channels and gauges can show routing while collision and water hazards remain fixed.

**Checks:** Holding at one endpoint generates no strokes; one traversal gives exactly one stroke; Q restart is not a stroke; moving off the selector cannot silently route a stroke to both reservoirs; reverse playback does not drain completed charges; recording a new clip correctly rewinds to the gauge state at that new recording's start. A harder optional variant can add slow leakage later, but only after a measured solution demonstrates a fair margin.

## Implementation and validation handoff

These are design proposals. **No scenes, runtime scripts, prefabs or controls were changed for this library.** Existing Rooms 5–8 and their earlier 23 passing tests are separate work, not evidence that all twelve designs have been built or solved.

- **Source review: PASS.** Current control semantics, single memory, echo support, plate criteria, shared object cursor, live timers, era switching and retry behavior were checked against the current source and existing test code.
- **Design inventory: PASS.** Twelve distinct cards, each with a layout, recording target, playback procedure, failure/recovery and implementation boundary. Nine use existing mechanic types; P09/P10/P12 require the explicitly described mechanism work.
- **Unity compilation / generation / puzzle traversal: NOT RUN for this library.** Documentation only; no implementation was introduced to compile or playtest.
- **Bypass resistance, difficulty and completion times: NOT VERIFIED.** In particular, E staircase movement, endpoint parking, Q restarts and late re-recording must be checked honestly. Accept creative alternate solutions when they preserve the intended learning; never impose an invisible exception to an ability.

For each selected prototype: first build the route with a safe observation point; demonstrate a complete legal recording and execution with actual player input; test R and repeated Q at every rest; test held/reversed endpoints and object lifetimes; then tune timing and decorate. Include negative checks specific to the puzzle rather than assuming that greater height or distance proves replay is required. Keep normal room deaths below camera bounds and avoid unrequested hidden ledges.

### Source references

- [Current recording, rewind, restart and playback clock](../../Assets/_OneMoreTime/Runtime/RewindPlaytest.cs)
- [Main-room controls, plate evaluation, eras and checkpoint handling](../../Assets/_OneMoreTime/Runtime/ChapterOneRoom.cs)
- [Moving platform activation and live release timers](../../Assets/_OneMoreTime/Runtime/RoomPlatform.cs)
- [Plate occupancy](../../Assets/_OneMoreTime/Runtime/PressurePlate.cs) and [single-plate gate](../../Assets/_OneMoreTime/Runtime/LinkedDoor.cs)
- [Floating source lifetime/replay](../../Assets/_OneMoreTime/Runtime/RoomFour/CrateSpawner.cs)
- [Crumbling bridge's live clock and rewind state](../../Assets/_OneMoreTime/Runtime/ChapterCrumbleBridge.cs)
- [Current first-pass Rooms 5–8](ROOMS_5_TO_8.md)
