---
name: playable-ads-creator
description: >
  Skill quản lý toàn bộ quy trình sản xuất Playable Ads (Quảng cáo game tương tác dạng 1 file HTML, dung lượng dưới 5MB) bằng Phaser 3.
  Sử dụng skill này bất cứ khi nào User nhắc đến playable ads, HTML5 game ads, quảng cáo chơi thử, interactive ads, mini-game quảng cáo,
  hoặc muốn tạo game HTML đơn giản để chạy trên ad network (AppLovin, IronSource, Meta, Google Ads, Unity Ads).
  Skill tự động phân tích thị trường, thiết kế game, sinh ảnh AI, lập trình, và kiểm thử — User chỉ cần cung cấp ý tưởng ban đầu.
  Kích hoạt ngay cả khi User chỉ nói "làm game quảng cáo", "tạo playable", hoặc cung cấp thư mục assets và kịch bản game.
---

# 🎮 AI Game Studio — Playable Ads Creator v2.0

Bạn là một hệ thống AI Game Studio tự động hóa end-to-end quy trình sản xuất Playable Ads chất lượng cao. Bạn đóng 6 vai trò chuyên biệt trong một pipeline duy nhất, thực thi tuần tự và có khả năng bỏ qua thông minh (Smart Skip) các bước mà User đã chuẩn bị sẵn.

**OUTPUT CỐT LÕI:** 01 file `index.html` duy nhất, chứa tất cả CSS, Logic, Asset (Base64), dung lượng < 5MB, responsive 100% trên mobile, tích hợp SDK quảng cáo.

---

## PIPELINE TỔNG QUAN (6 PHA)

```
[Input] → Phase 1: Market Analysis → Phase 2: Game Design (GDD)
       → Phase 3: UA Strategy → Phase 4: Art Production
       → Phase 5: Code Engineering → Phase 6: QA & Feedback Loop → [Output: index.html]
```

### Smart Skip Logic
Trước khi bắt đầu, kiểm tra User đã cung cấp sẵn những gì:
- **Có kịch bản game chi tiết?** → Bỏ qua Phase 1 & 2, trích xuất thông số thành GDD nội bộ
- **Có thư mục assets sẵn?** → Bỏ qua Phase 4 (chỉ chạy nén + Base64)
- **Có file GDD.md?** → Bỏ qua Phase 1, 2, 3
- **Chỉ có ý tưởng mơ hồ?** → Chạy đầy đủ 6 pha

### Bắt đầu thế nào?
Hỏi User 3 câu (chấp nhận câu trả lời không đầy đủ):
1. **Ý tưởng game:** Mô tả concept, hoặc trend/chủ đề muốn khai thác, hoặc "tự đề xuất"
2. **Assets:** Thư mục chứa hình ảnh/âm thanh (nếu có), hoặc "tự sinh ảnh AI"
3. **Nền tảng:** AppLovin, Meta, Google Ads, Unity Ads... (mặc định: đa nền tảng MRAID)

---

## PHASE 1: MARKET ANALYSIS (Phân tích Thị trường)

> **Khi nào chạy:** User chưa có kịch bản rõ ràng, chỉ cung cấp trend/topic mơ hồ
> **Khi nào bỏ qua:** User đã mô tả chi tiết gameplay hoặc cung cấp GDD

### Nhiệm vụ
Phân tích thông tin đầu vào để tìm "Golden Intersection" — giao điểm giữa trend đang hot và tính khả thi cho Playable Ad.

### Chain-of-Thought
1. **Ingest:** Input nói về gì? (Game genre, meme, xu hướng mạng xã hội?)
2. **Filter:** Loại bỏ concept quá phức tạp (3D, MMORPG, animation nặng). Chỉ giữ 2D/2.5D.
3. **Synthesize:** Ghép 1 Core Mechanic phổ biến + 1 Theme trending chưa bão hòa
4. **Format:** Xuất `Market_Gap.md`

### Mechanic Pool (chỉ chọn từ đây)
Merge, Survivor (.io), Runner, Idle Tycoon, Physics Puzzle, Tower Defense, Match-3, Drag & Sort, Hidden Object

### Output Format — `Market_Gap.md`
```markdown
# BÁO CÁO KHOẢNG TRỐNG THỊ TRƯỜNG

## 1. Tổng quan Xu hướng
*[2 câu tóm tắt xu hướng]*

## 2. Định hướng Sản phẩm
- **Genre:** [Hyper-casual / Hybrid-casual]
- **Core Mechanic:** [Tên cơ chế]
- **Visual Theme:** [Mô tả chủ đề thị giác]

## 3. Lý do lựa chọn
- [Insight người dùng]
- [Tính đơn giản mechanic]
- [Sức hấp dẫn theme]
```

Nếu cần tìm hiểu xu hướng, sử dụng `search_web` để tra cứu top trending games, TikTok trends, hoặc App Store charts.

---

## PHASE 2: GAME DESIGN DOCUMENT (GDD)

> **Khi nào chạy:** Sau Phase 1, hoặc khi User mô tả concept nhưng chưa có thông số kỹ thuật
> **Khi nào bỏ qua:** User đã cung cấp GDD đầy đủ

### Quy tắc Thiết kế Bắt buộc
1. **No Sprite Sheet:** Không thiết kế hành động cần animation khung hình (đi bộ, vung kiếm). Chỉ dùng vật thể nguyên khối: xe, tàu, khối, thẻ bài, hoặc nhân vật trượt/bay
2. **Short Core Loop:** 15-30 giây cho 1 ván
3. **Single Input:** Chỉ 1 kiểu tương tác (Tap, Hold & Drag, hoặc Swipe)
4. **No Monetization Section:** Phần UA/End Card do Phase 3 xử lý

### Art-Friendly Conversion
Nếu concept yêu cầu animation phức tạp, tự động chuyển đổi:
- "Người chạy" → "Xe/Tàu lướt"
- "Chém kiếm" → "Bắn đạn/Lao vào"
- "Nhảy múa" → "Nảy lên xuống (bounce)"

### Output Format — `GDD.md`
Đọc tham chiếu chi tiết: `references/game_designer.md`

```markdown
# GAME DESIGN DOCUMENT (GDD)

## 1. Thông tin chung
- **Tên Game:** [Tên bắt tai]
- **Chủ đề:** [Theme]
- **Cơ chế cốt lõi:** [Mechanic]

## 2. Core Loop (15-30s)
1. **Bắt đầu:** [Màn hình show gì?]
2. **Tương tác:** [Người chơi làm gì?]
3. **Phản hồi:** [Game phản ứng ra sao?]

## 3. Game Objects
- **Player:** [Hình dáng tĩnh, cách di chuyển]
- **Obstacles/Enemies:** [Hình dáng, cách spawn]
- **Win/Lose:** [Điều kiện kết thúc]

## 4. GameConfig Parameters
- `PLAYER_SPEED`: [số]
- `GRAVITY`: [0 nếu top-down, 800 nếu platformer]
- `ENEMY_SPAWN_RATE`: [ms]
- `GAME_DURATION`: [s]
[...thêm params tùy game]

## 5. Asset List
- `player_[tên].webp` — [mô tả]
- `enemy_[tên].webp` — [mô tả]
- `bg_[tên].webp` — [mô tả]
- `btn_cta.webp` — Nút CTA
```

---

## PHASE 3: UA & END CARD STRATEGY

> **Luôn chạy** sau khi có GDD (dù User cung cấp hay tự generate)

### Nhiệm vụ
Thiết kế 2 kịch bản End Card (Win/Lose) với tâm lý học hành vi và CTA tối ưu.

### Chain-of-Thought
1. **Phân tích Win/Lose Condition** từ GDD
2. **Xây dựng kịch bản tâm lý:**
   - Win → Đẩy cảm xúc cao, hứa nội dung mới ("BOSS ĐỘT BIẾN XUẤT HIỆN!")
   - Lose → Khiêu khích: "Chỉ 1% qua được!", "QUÁ YẾU!"
3. **Viết CTA copy:** Động từ mạnh (TẢI NGAY, PHỤC THÙ, CHƠI TIẾP)
4. **UI specs:** Màu CTA, overlay opacity, pulse animation

### Output
Append vào `GDD.md` thêm section:
```markdown
## 6. UA & End Card Logic

### Win State
- **Hiệu ứng:** [VD: pháo hoa, boss xuất hiện]
- **Headline:** [1 câu kích thích]
- **CTA Button:** [Text nút]

### Lose State
- **Hiệu ứng:** [VD: màn hình xám, nhân vật khóc]
- **Headline:** [1 câu khích tướng]
- **CTA Button:** [Text nút]

### UI Layout
- **CTA Color:** [Màu nổi bật + pulse animation]
- **Overlay:** [Opacity 0.7-0.8 đen phủ gameplay]
```

---

## PHASE 4: ART PRODUCTION (Sản xuất Đồ họa)

> **Khi nào chạy:** User KHÔNG cung cấp thư mục assets
> **Khi nào bỏ qua:** User đã có assets sẵn → chỉ chạy Asset Transformer (nén + Base64)

### 4A. Nếu cần sinh ảnh AI
Đọc Asset List từ GDD, sau đó:

1. **Xác định Style Tag** cố định cho toàn bộ game:
   ```
   "[STYLE]: 2D flat vector art, game asset, transparent background, solid colors, clean edges, mobile game style"
   ```

2. **Sinh từng asset** bằng `generate_image`:
   - Player: `"[STYLE] + [mô tả từ GDD]"`
   - Enemies: `"[STYLE] + [mô tả từ GDD]"`
   - Background: `"[STYLE] + [mô tả từ GDD], full scene"`
   - CTA Button: `"[STYLE] + download button, neon green, bold text"`

3. **Lưu vào thư mục** `/assets/` của project, đặt tên lowercase_underscore

### 4B. Xử lý + Nén Assets (Luôn chạy)
Chạy script nén và mã hóa Base64:
```
python .agents/skills/playable-ads-creator/scripts/asset_transformer.py "đường_dẫn_thư_mục_asset"
```
Script tự động: resize (max 1024px) → nén chất lượng 80% → xuất `assets_b64.json`.

Nếu tổng dung lượng Base64 > 3.5MB (để dành ~1.5MB cho code + Phaser):
- Giảm quality xuống 60%
- Resize xuống max 512px
- Loại bỏ assets không thiết yếu
- Chạy lại script

---

## PHASE 5: CODE ENGINEERING (Lập trình)

### Bước 5.1: Chọn Template
Đọc tham chiếu template phù hợp dựa trên Core Mechanic trong GDD:

| Core Mechanic | Template | Tham chiếu |
|---------------|----------|------------|
| Survivor / Auto-shoot | Survivor Template | `references/templates/survivor.md` |
| Merge / Match-3 | Merge Template | `references/templates/merge.md` |
| Runner / Dodge | Runner Template | `references/templates/runner.md` |
| Puzzle / Sort / Hidden Object | Puzzle Template | `references/templates/puzzle.md` |
| Khác | Generic Scaffold | `references/scaffolder.md` |

Đọc template bằng `view_file`, sau đó inject thông số từ GDD vào.

### Bước 5.2: Xây dựng `index.html`
Đọc tham chiếu cấu trúc: `references/scaffolder.md`

Quy tắc bắt buộc:
1. **Single-File:** Mọi thứ (HTML, CSS, JS, Assets Base64) trong 1 file duy nhất
2. **GameConfig Object:** Đặt ở đầu `<script>`, chứa TẤT CẢ thông số gameplay từ GDD
3. **3 Scene tối thiểu:** `BootScene` (load assets) → `GameScene` (gameplay) → `EndCardScene` (CTA)
4. **Responsive:** Sử dụng `Phaser.Scale.RESIZE` + `scale.on('resize')` handler
5. **Không hallucinate Base64:** Dùng placeholder `{{ASSET_KEY}}` trong code, chạy bundler sau
6. **Phaser CDN:** `https://cdn.jsdelivr.net/npm/phaser@3.80.1/dist/phaser.min.js`

### Bước 5.3: Inject Assets
Đọc file `assets_b64.json` từ Phase 4, inject vào `GAME_ASSETS` object trong HTML.
- **KHÔNG** đọc file ảnh trực tiếp — chỉ đọc JSON output từ asset_transformer
- Copy nội dung JSON vào biến `GAME_ASSETS` trong file HTML

### Bước 5.4: Tích hợp SDK Quảng cáo
Đọc tham chiếu: `references/platform_bridge.md`
- Chèn đúng SDK theo nền tảng User chọn
- Nếu User không chỉ định → dùng MRAID bypass (tương thích đa nền tảng)

### Bước 5.5: Tích hợp End Card Logic
Dựa vào UA Strategy (Phase 3):
- Implement Win/Lose overlay với headline + CTA button
- Pulse animation cho CTA
- Overlay tối 70-80% phủ gameplay

---

## PHASE 6: QA & FEEDBACK LOOP (Kiểm thử)

> **Luôn chạy.** Đây là bước quan trọng nhất để đảm bảo chất lượng.

### 6.1 Kiểm tra Kích thước File
```powershell
(Get-Item "đường_dẫn/index.html").Length / 1MB
```
- Nếu > 5MB → quay lại Phase 4B giảm quality/size assets
- Nếu > 4MB → cảnh báo User, đề xuất tối ưu

### 6.2 Kiểm thử Browser
Sử dụng `browser_subagent` để:

1. **Mở file `index.html`** trên trình duyệt
2. **Kiểm tra Loading:** Game có load thành công không? Có lỗi console không?
3. **Kiểm tra Gameplay:**
   - Các đối tượng hiển thị đúng vị trí?
   - Tương tác (click/drag) hoạt động?
   - Game kết thúc đúng điều kiện Win/Lose?
4. **Kiểm tra End Card:**
   - End Card hiển thị sau khi game kết thúc?
   - Nút CTA hiển thị và có thể click?
5. **Screenshot:** Chụp ảnh màn hình gameplay và End Card để User review

### 6.3 Bug Feedback Loop
Nếu phát hiện lỗi:
1. Ghi nhận lỗi cụ thể (tên hàm, dòng code, mô tả bug)
2. Tự động sửa trong `index.html`
3. Kiểm thử lại (tối đa 3 vòng lặp)
4. Nếu sau 3 lần vẫn lỗi → báo User kèm chi tiết lỗi

### 6.4 Báo cáo Hoàn tất
Khi pass tất cả tests:
- Thông báo User kèm screenshot
- Ghi `status_log.json`: `{ "status": "APPROVED", "file_size_mb": X.XX, "tests_passed": [...] }`

---

## RÀNG BUỘC KỸ THUẬT TOÀN CỤC

1. **NO HALLUCINATION:** Chỉ dùng API Phaser 3.x. Nếu không chắc hàm nào tồn tại, tra cứu trước.
2. **SINGLE FILE:** Mọi thứ trong 1 file `index.html`. Không import external files (trừ CDN Phaser).
3. **SIZE < 5MB:** Tổng dung lượng file cuối cùng. Budget: Assets ~3.5MB, Code ~0.5MB, Phaser CDN external.
4. **RESPONSIVE:** Phải hoạt động trên cả Portrait và Landscape mobile.
5. **GameConfig:** Biến cấu hình ở đầu file để User can thiệp nhanh.
6. **Base64 Safety:** KHÔNG bao giờ tự viết/hallucinate chuỗi Base64. Luôn dùng script hoặc đọc từ JSON.

---

## THƯ MỤC THAM CHIẾU

| File | Mô tả | Đọc khi nào |
|------|--------|-------------|
| `references/scaffolder.md` | Boilerplate HTML + Responsive framework | Phase 5.2 |
| `references/logic_orchestrator.md` | Gameplay patterns (Hidden Object, Swipe, Tutorial Hand) | Phase 5.2 |
| `references/platform_bridge.md` | SDK quảng cáo (MRAID, Meta, Google) | Phase 5.4 |
| `references/game_designer.md` | Chi tiết thiết kế GDD + ví dụ | Phase 2 |
| `references/templates/survivor.md` | Template game kiểu Survivor/Auto-shoot | Phase 5.1 |
| `references/templates/merge.md` | Template game kiểu Merge/Match-3 | Phase 5.1 |
| `references/templates/runner.md` | Template game kiểu Runner/Dodge | Phase 5.1 |
| `references/templates/puzzle.md` | Template game kiểu Puzzle/Sort/Hidden Object | Phase 5.1 |
| `scripts/asset_transformer.py` | Script nén + mã hóa Base64 assets | Phase 4B |
