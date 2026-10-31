> 使用者已要求恢復原始 6 格動畫；此步態不再由正式動畫庫使用。

# 24 格連續步態重做

舊 GoatRun24 的獨立生成姿勢即使按相似度排序，也沒有連續步態。本次取消該排序，使用原始 GoatAnimations 第一格的固定軀幹與腿部部件，透過 Unity 編輯器烘焙成 24 個依時間排序的姿勢。

- GoatGait.Foot 定義半週接地後移、半週抬脚回收。位置及速度在兩個交界連續，首尾循環。
- 四肢採對角相位，遠側較暗；身體小幅起伏。固定樞紐 (142,13)、170 PPU。
- GoatGaitBuilder.Build 可重建 GoatGait24.png 與 Sprite 引用；GoatArtBuilder.Build / BuildRun24 皆轉用此產生器。
- 原始與舊生成素材保留，但正式 library.run 改用 GoatGait24_00 至 23。
- 關卡、教學、結尾共用 AnimationLibrary；維持原本 48 格/秒、速度比例及展示距離相位。這是步態造型修正，不是實測 FPS 改善，也未改碰撞或角色速度。
- 預覽 Artifacts/QA/gait.html，可播放、暫停及逐格檢查。

測試：EditMode 20/20 PASS，包含新增的循環位置/速度、接地/抬腳與時間順序/固定 pivot。PlayMode 47/48；既有 InputTests.HeldKeysAndResetSuppression 的 Initial E edge 仍 FAIL。四關模擬測試通過。
