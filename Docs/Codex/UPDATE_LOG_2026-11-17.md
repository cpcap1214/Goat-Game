# Update log — 2026-11-17

目標：原始 repo `cpcap1214/Goat-Game`。本次新增 World 1「森林與大瀑布」素材及可試玩的展示場景。分支先前的主線、回放與濾鏡更新另見 UPDATE_LOG_2026-11-16.md。

## World 1 美術與動畫
- 新增流水、瀑布頂端、瀑布本體、落水泡沫四種動畫 TileBase，每種四幀，以及可延伸的河流／瀑布 prefab。
- 新增森林峽谷背景；可在水平及垂直方向無限循環，支援可調 parallax、重複尺寸及接縫混合。保留原本的有限高度風景圖。
- 重畫六幀溺水動畫：維持山羊正常身形、隱藏沉入水中的四肢、縮小水花，並以水面對齊 sprite pivot。
- 新增可選用的 DeepWaterHazard，播放溺水後依既有檢查點規則重生，不重新載入場景。
- 新增石化山羊檢查點：灰色石像啟動時裂開，裂縫與眼睛發綠光，綠色光暈持續循環。

## 開發與試玩
- Debug Mode 新增 08 World 1 Art；↑/↓ 無限捲動背景、Space 預覽溺水、C 預覽雕像啟動。
- 提供 Unity builder、sprites、materials、prefabs、場景與使用指南 WORLD_ONE_ART.md；AI 圖片提示詞另附文件。
- 沒有把新素材強制佈置進現有房間，既有關卡地形、開場及能力流程保留。

## 驗證
- PASS：Unity 6000.6.0f1 真實 Editor 編譯及資產生成。
- PASS：WorldOneArtTests 3/3（動畫、檢查點、溺水重生、背景循環及相機覆蓋）。
- PASS：Unity 實際畫面檢查溺水六幀及背景接縫；git diff --check。
- NOT RUN：獨立版本建置及完整人工通關。背景接縫採混合過渡，非逐像素重畫拼接。
