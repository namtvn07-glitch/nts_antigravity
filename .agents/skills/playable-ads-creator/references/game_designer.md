# Game Designer Reference — Chi tiết Thiết kế GDD

Tài liệu tham chiếu cho Phase 2: Game Design Document. Hướng dẫn cách chuyển hóa Market Gap hoặc concept thô thành GDD chuẩn hóa.

## Nguyên tắc Cốt lõi

### 1. Constraint-Based Design
Playable Ads có giới hạn rất khắt khe — thiết kế phải xuất phát từ giới hạn, không phải từ tham vọng:
- **15-30 giây** cho toàn bộ gameplay
- **1 loại input** duy nhất (Tap, Drag, hoặc Swipe)
- **Không sprite sheet** — chỉ hình tĩnh trượt/xoay/nảy
- **Tối đa 4-5 loại game object** (player, enemy, collectible, obstacle, background)
- **Phải dẫn đến End Card** trong mọi trường hợp

### 2. "3 Giây Đầu Tiên" Rule
Người chơi phải hiểu luật chơi trong 3 giây đầu — không có tutorial dài dòng. Mọi thứ phải trực giác:
- Thấy tay gợi ý → chạm → hiểu ngay phải làm gì
- Nếu mechanic cần giải thích > 5 từ → mechanic quá phức tạp

### 3. Art-Friendly Conversion Table
Khi concept yêu cầu animation phức tạp, tự động chuyển đổi:

| Concept gốc | Chuyển thành | Lý do |
|-------------|-------------|-------|
| Người chạy bộ | Xe/Tàu/Ván trượt lướt | Không cần animation bước chân |
| Chém kiếm | Bắn đạn tự động | Đạn là hình tròn, không cần animation |
| Nhảy múa | Nảy lên xuống (bounce tween) | Chỉ cần scale/y tween |
| Bơi lội | Tàu ngầm/Cá trượt | Không cần animation vẫy tay |
| Bay | Tên lửa/UFO/Chim tĩnh | Rotate theo hướng di chuyển |

### 4. Mechanic → GameConfig Mapping

#### Survivor (Auto-shoot)
```
PLAYER_SPEED: 200-300
GRAVITY: 0
ENEMY_SPAWN_RATE: 600-1200ms
ENEMY_SPEED: 80-150
SHOOT_INTERVAL: 200-500ms
BULLET_SPEED: 400-600
GAME_DURATION: 15-25s
PLAYER_HP: 3-5
```

#### Runner (Side-scroll / Dodge)
```
PLAYER_SPEED: 0 (auto-scroll)
SCROLL_SPEED: 200-400
GRAVITY: 800-1200
JUMP_FORCE: -400 to -600
OBSTACLE_SPAWN_RATE: 1000-2000ms
GAME_DURATION: 15-20s
LANE_COUNT: 2-3 (nếu lane-based)
```

#### Merge / Match-3
```
GRID_COLS: 4-6
GRID_ROWS: 5-7
CELL_SIZE: tính từ screen width
MERGE_LEVELS: 3-5
MOVES_LIMIT: 5-10 (hoặc TIME_LIMIT: 15-20s)
MATCH_MIN: 3
```

#### Puzzle / Sort / Hidden Object
```
ITEM_COUNT: 3-8
TARGET_ZONES: 2-4
DRAG_SNAP: true
TIME_LIMIT: 15-25s
SUCCESS_THRESHOLD: tìm đủ X items
```

## Ví dụ GDD Hoàn chỉnh

**Input:** Genre Hyper-casual, Mechanic: Swarm Survival, Theme: Mèo vs Chuột Zombie

```markdown
# GAME DESIGN DOCUMENT (GDD)

## 1. Thông tin chung
- **Tên Game:** Meow Survivor: Zombie Rats
- **Chủ đề:** Mèo dễ thương cầm súng vs Chuột Zombie
- **Cơ chế cốt lõi:** Auto-shoot Survival

## 2. Core Loop (20s)
1. **Bắt đầu:** Mèo đứng giữa, chuột zombie từ 4 rìa tiến vào
2. **Tương tác:** Hold & Drag di chuyển Mèo (auto-shoot về chuột gần nhất)
3. **Phản hồi:** Chuột trúng đạn biến mất + rớt sao kinh nghiệm

## 3. Game Objects
- **Player (Mèo):** Ảnh tĩnh PNG/WebP. Trượt theo ngón tay.
- **Enemies (Chuột):** Ảnh tĩnh. Spawn từ 4 viền, lao về phía Mèo.
- **Bullets:** Hình tròn nhỏ, bay từ Mèo đến chuột gần nhất.
- **Win/Lose:** Thắng = sống 20s. Thua = chuột chạm Mèo (HP=0).

## 4. GameConfig Parameters
- `PLAYER_SPEED`: 250
- `GRAVITY`: 0
- `ENEMY_SPAWN_RATE`: 800
- `ENEMY_SPEED`: 100
- `SHOOT_INTERVAL`: 300
- `BULLET_SPEED`: 500
- `GAME_DURATION`: 20
- `PLAYER_HP`: 3

## 5. Asset List
- `player_cat.webp` — Mèo cầm súng, nhìn từ trên
- `enemy_rat.webp` — Chuột zombie xanh, nhìn từ trên
- `bullet.webp` — Viên đạn nhỏ tròn vàng
- `bg_street.webp` — Background đường phố tối
- `star.webp` — Ngôi sao kinh nghiệm
```
