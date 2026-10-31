# 山羊與森林美術升級

2026-09-19 新需求：山羊正式取代狐狸；狐狸仍保留測試。美術修改不改跑速、跳高、記憶規則或四關幾何。

## 角色

參考圖：使用者附件 image-1.png，方胖的米白身體、深棕直角、小耳朵、大鼻口、短蹄。內建 image_gen 產生24幀透明PNG，6×4網格；不是以程式畫幾何佔位圖。生成後原檔複製進專案，保留原始生成圖。

- Assets/_OneMoreTime/Art/Goat/GoatAnimations.png：1536×1024。
- 待機6幀、跑步6幀、上升3幀、下落3幀、落地6幀。
- GoatArtBuilder 在Unity中切為24個Sprite子資產檔，以alpha輪廓計算每幀腳底pivot，PPU170，Point filter，無材質背景矩形。
- AnimationLibrary 是正式山羊設定；FoxAnimationLibrary與FoxTestPlayer保留。Dev_Playground按F可切山羊／狐狸。
- 角色視覺時鐘隨固定模擬步進入狀態，空中／落地序列不循環；錄製保存確切state/frame，定格不繼續播放。視覺時鐘不改碰撞或速度。

## 場景

使用原Sunny Land切圖與props，增加草皮邊緣、苔蘚、重複鋪設的原生岩土塊、垂掛蕨葉、樹／松樹、灌木、蘑菇、岩石、起點木屋及路標。遠山與中景森林隨相機產生視差；森林圖以原比例重複配置，避免橫向拉伸。所有新裝飾沒有Collider，不創造額外踏腳或繞關路徑。門的黃銅欄杆與踏板連線在啟動後轉為淡薄的薄荷色，原門洞保護保留。

## 介面

Assets/_OneMoreTime/Art/UI/JourneyTitle.png 為AI生成的森林像素插畫：右側山羊與淡綠記憶、左側深色留白。標題、按鈕、按鍵提示全部為真正可縮放、可操作的Unity UI文字與程式繪製面板，不把不可點的圖當成介面。

配色為墨綠、奶油白、黃銅金、薄荷綠；方角薄邊框、下緣陰影、主操作金色填底。HUD分成左側章節牌、右側記憶牌、底部提示與獨立鍵帽。旅人手冊、暫停、音量、死亡保存、重新錄製確認與通關面板使用相同系統。

## 生成方式與提示詞

方式：內建 image_gen；未使用API金鑰或CLI fallback。以下為最終採用的提示詞規格（英文原意）：

1. **Goat animation sheet / stylized-concept**：production pixel-art goat sprite sheet; exact6columns×4rows,1536×1024,transparent; same right-facing chunky off-white goat with upright dark horns, square muzzle, short dark cloven hooves, tiny tail; restrained pixel palette; row1 six idle/breathing/blink poses, row2 six distinct run gait frames, row3 three ascent plus three descent poses, row4 six landing/recovery poses; consistent scale, full silhouettes, empty gutters, no text/grid/scenery.
2. **Background-extraction edit**：remove brown/gray background, glow, haze and external shadows from the exact sheet; preserve all24 poses/positions and canvas; empty alpha0, opaque character silhouettes, hard pixel edges; no rearrangement. 輸出保有真實透明通道，實際透明合成另以Unity畫面驗證，不以工具的RGB預覽底色判定。
3. **Journey title / stylized-concept**：wide16:9 polished cozy pixel-art forest title background; goat identity from reference, full-body at lower-right facing left; subtle mint ghost behind; golden grass, terracotta banks, mossy stones, ferns, oak trees, wildflowers, pale teal mountains and warm morning light; left45% low-detail dark teal forest negative space for title/menu; no generated text/buttons/logos; opaque image.

生成来源文件位於本機 ~/.codex/generated_images/01a0b94d-03d9-7541-97cf-559aa673707f/；專案只引用上方Assets內複本。原Sunny Land素材、音樂與字型授權仍見LICENSES.md。

## 最終驗證

46項Unity測試通過；六場景引用檢查通過。實際動畫展示見Artifacts/Logs/goat-animation-gallery.png；最終地形見art-v2-gameplay.png；新版四關完整按鍵通關至結尾。主選單960×540與1920×1080、按鍵說明、暫停／音量及重錄確認已操作核對。Web與ZIP已更新，證據詳TEST_REPORT.md。

## 2026-09-21 參考圖地形修訂

依使用者提供的Sunny Land範例：地形內部以原素材的深紫棕色土層為主，內嵌石塊稀疏散布，草皮與岩石只出現在外緣。TerrainArtBuilder以四關各自所有terrain矩形的聯集計算外輪廓，再用Sunny Land原切片組成整片景觀；相接矩形不各自畫邊、不把岩壁圖鋪满內部。Point/Clamp、16 PPU，原始PNG不改動。這是Unity素材組版，沒有新生成AI圖片。

四關出口門使用Sprite實際高度計算位置，使renderer.bounds.min.y等於goal.yMin，門頂也貼齊；SceneValidator逐關檢查間隙。第一關草地高台、第二關機關高牆、第三關高崖、第四關上下層與連續頂棚均使用新外觀；原地形碰撞／崩塌區／記憶玩法保留。


## 2026-09-21 開場劇情
使用者提供的 `/Users/cpcap/Downloads/第一個過場動畫.svg` 原樣保存在 `Assets/_OneMoreTime/Art/UI/OpeningDialogue.svg`；ImageMagick 將其透明圖層光柵化為同目錄 PNG（1166×296），遊戲等比例呈現。未重畫對話框。
`GoatPortrait.png` 由內建 imagegen 依現有 GoatAnimations.png 生成：單隻奶油色方形山羊、直立深棕角、方吻、朝右三分之四胸像、好奇而有希望的神情、像素風、透明背景、無文字／邊框。來源 exec-a965e5d8-d54d-40aa-8c27-f91605089ddf.png。
開場以五段獨白交代暴雨沖斷回家的路、發現記憶、與過去的自己合作。Enter／Space／下一句推進，Esc／略過劇情直接進 L01。每次从主選單開始皆可重看，R 不重播劇情。沒有增加第五關或改動 E/Q。
