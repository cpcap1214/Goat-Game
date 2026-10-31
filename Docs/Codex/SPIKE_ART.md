# AI 尖刺素材

使用內建 image_gen（imagegen 技能），不是 CLI。生成原始圖保存到 Assets/_OneMoreTime/Art/Hazards/SteelSpike.png，RGBA 1254 × 1254。Unity Sprite 依 alpha > 128 的包圍框取圖，原圖及 alpha 不變；四關 9 排危險區域使用重複單顆尖刺，移除紅色染色，以排序 6 保持可辨識。碰撞範圍與 LevelConfig 不變。

最終生成提示：

Use case: stylized-concept. Asset: transparent pixel-art hazard sprite for a side-view 2D platformer. Create ONE single upward-facing deadly steel spike, centered, orthographic front/side elevation, no perspective. A very sharp tall triangular silver-white metal blade with a dark charcoal thick pixel outline, bright ivory left face and blue-gray right face, on a short dark rectangular metal base. Simple extremely readable silhouette at 24 pixels tall, chunky clean pixel art matching a colorful forest platform game. Shape occupies most of square canvas with small transparent margins. True transparent RGBA background, no ground, no scenery, no glow, no shadow outside sprite, no lettering, no extra spikes. Single spike only, broad triangular base, needle point at top.

更新入口：SpikeArtBuilder.UpdateScenes；之後 DemoBuilder.BuildLevel 亦使用新素材。


2026-09-22：Create 為坑底（yMin < 0）的尖刺新增 Spike earth foundation，使用既有 EarthFillSeamless 土塊，寬度等同尖刺列，頂面對齊尖刺底部，高1單位。共L01兩處、L02三處、L03兩處；L04已有地面，不向下填入通道。此為視覺基座，未新增碰撞，原本尖刺致死區及玩法不變。
