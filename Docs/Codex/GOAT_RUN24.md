> 已由 2026-09-22 GOAT_GAIT.md 的連續部件步態取代；以下為舊實作紀錄。

# 山羊跑步24格

正式素材 Assets/_OneMoreTime/Art/Goat/GoatRun24.png，1536×1024、6×4格、RGBA透明；保留原24格綜合動作素材。由內建imagegen參考原山羊生成，第一版混入跳躍姿势未採用；採第二版直立跑步。

每格以不透明像素包圍框中心X與腳底Y生成Sprite pivot，修正生成圖格逐欄／逐列偏移。GoatArtBuilder.BuildRun24產生24個真正不同矩形引用；以相鄰腿部輪廓差異安排封閉循環順序，非重複原6張。完整動作庫現有42張：待機6、跑步24、上升3、下落3、落地6。

邏輯跑步循環24格，以最高48格/秒播放（2循環/秒），依水平速度降速；不改角色物理速度。狐狸6張透過同一24格邏輯相位映射，維持原節奏。能力示範、結尾與AnimationGallery同步更新取樣方式。

可在 http://127.0.0.1:8765/QA/run24.html 比較新舊循環並逐格查看；QA不包含在正式ZIP。

生成指示：Use ONLY the TOP LEFT goat as a character reference. Discard the reference sheet layout and ALL its poses. Draw a NEW animation sprite sheet 1536x1024 pixels: 6 columns x4 rows of equal256x256 cells. Every cell shows the SAME upright right-facing beige pixel goat RUNNING. 24 consecutive frames of ONE run loop. CRITICAL: torso and head identical upright orientation in ALL 24 frames, NO tilted goats, NO jumping goats, NO crouching goats, NO idle frames. Only the four short legs move incrementally through one trot cycle, with tiny body bob. Last frame leads smoothly back into first. Render actual fully TRANSPARENT RGBA background, not brown, not a glow, not checkerboard illustration. Crisp pixel art, flat dark outline, beige compact square body, pale face, two upright dark horns, beard, white square eye. Entire goat always centered within each cell, same size, constant proportions, horns top at50px, hooves near230px, margin30px each side. NO text. Same character at24 distinct evenly spaced cycle phases. Rows do NOT represent different actions; ALL FOUR ROWS ARE RUNNING.
