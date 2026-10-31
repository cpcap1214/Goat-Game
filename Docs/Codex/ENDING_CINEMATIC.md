# 回魔法塔結尾動畫

Ending 場景顯示山頂魔法塔背景，沿用既有山羊 idle/run Sprite。11秒：0–6秒由左側走向塔門，6–7秒停留，7–8.5秒縮小淡出進塔，8秒後魔法光點亮起，9–11秒淡入「恭喜通關！」。最後保留「再玩一次」回主選單；Esc／略過動畫直接到入塔完成狀態。主選單和四關玩法不變。

背景：Assets/_OneMoreTime/Resources/Ending/MagicTower.png。內建imagegen生成；原始檔保留在 ~/.codex/generated_images/01a0c35d-4c5f-7090-a8af-4e916a777c17/exec-906e65f2-5c9d-4f30-b4ba-03be8b5a7714.png。

提示詞（生成用原文）：
> Use case: illustration-story. Create a finished pixel-art background for the ending of a side-view 2D goat platform game called Goat Tower Again. Landscape 16:9, 1536x864. Crisp chunky pixel art, sunny mountain fantasy, teal pine forests, distant layered blue mountains, warm golden late-afternoon light, lush grass and weathered stone. A whimsical tall wizard's stone tower occupies the RIGHT third of the image, purple pointed roof, mint magical crystal, glowing amber small windows, ivy, brass details. The tower stands on a mountaintop grassy plateau. IMPORTANT for animation compositing: a clear horizontal grassy walking path stretches from left edge toward the tower entrance at 78% image width; path surface and bottom of the tower entrance are both at 80% image height. Tower entrance is a tall OPEN dark arched doorway, center at 78% width, from 62% to 80% image height, no door leaf. Left half of image is mostly scenic open sky and mountain backdrop with uncluttered space for ending title text. Ground fills bottom 20%. No characters, no goats, no creatures, no lettering, no logo, no UI, no frame. This is a production background layer; keep the open doorway clearly visible and the walking surface flat so an animated goat can walk across and disappear into the doorway.

實際輸出1672×941，門中心經視覺確認在81.3%寬、底部約80%高，EndingAssets使用此座標。點取樣、無壓縮與mipmap，保留原生成圖。EndingAssetBuilder在建置前配置真實動畫資產引用，不手寫GUID。

驗證入口：EndingTests（時間順序／skip／資源引用），EndingAssetBuilder.BuildPreview 建出包含Ending與MainMenu的獨立預覽供瀏覽器檢查。正式版本仍使用原六場景流程。


## 2026-09-22 24格素材在結尾的對齊修正

GUI原先直接把整張256×256圖格放進固定矩形，忽略Sprite.pivot；新圖格的透明留白有位置偏移，所以SpriteRenderer雖已校正，結尾仍會跳動。EndingCinematic.SpriteRect改以Sprite.pivot計算實際绘製位置，確保指定腳底座標在各格相同。跑步相位改由橫向行進距離（64 GUI單位／循環）驅動，進門再追加0.75循環，避免SmoothStep減速時仍固定48格／秒空轉。
