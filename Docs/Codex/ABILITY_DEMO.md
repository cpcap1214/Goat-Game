# 道具自動示範

對話結束後，L01懷錶和L03沙漏會進入 `AbilityDemonstration`，顯示獨立的教學舞台。沿用正式山羊動畫與薄荷綠殘影，以簡化地形呈現能力因果；示範是UI動畫，不注入正式關卡的玩家、記憶、錄製或機關狀態。

- 懷錶16秒：靠近高台、跳起按E、留下定格並回起點、從上方落到定格背上、再次跳上高台。
- 沙漏22秒：Q開始、等待與走動、Q保存回起點、Q播放、跳上等待的殘影、搭乘並起跳、殘影消失。
- 每段同步顯示按鍵和說明；重播重置示範時間，Esc或略過結束。播放完成後停在最後一幕，按Enter、Space或「換我試試」繼續。
- 示範結束保留拾取位置和解鎖能力，清空輸入，避免結束按鍵洩漏至遊戲；當關重試不再播放。進入新旅程重新拾取仍會顯示。
- 示範使用固定步0.02秒自己的時間；正式關卡Clock、角色、機關與Recorder不前進。

實作：Runtime/AbilityDemo.cs、RunManager.cs、MemorySnapshot.cs。
測試：AbilityTests.DemonstrationPausesWorldCanReplayAndSkip涵蓋兩種道具，檢查世界停表、位置、記憶、錄製、重播、略過與繼續。


## 2026-09-22 示範播放平滑化

示範時鐘由RunManager.Update透過AdvancePresentation(Time.unscaledDeltaTime)推進；FixedUpdate/Tick只在示範時直接返回，不再以50Hz推進動畫。測試可直接給定展示dt，仍不改任何實際世界狀態。

GUI角色改共用PlayerVisual.SpriteGUIRect，使用各Sprite.pivot校正透明留白；結尾亦改共用同一函式。跑步相位隨x移動距離計算，軌跡16秒停步後不再空轉；定格殘影永遠採相同姿勢。
