# Template: Merge / Match-3

Template Phaser 3 cho game kiểu Merge (kéo thả ghép vật phẩm cùng loại) hoặc Match-3 (nối/chọn 3 ô cùng màu).

## Khi nào dùng
- GDD có keyword: merge, match, combine, grid, puzzle-board
- Grid-based gameplay, tap hoặc drag để ghép items
- Mục tiêu: ghép đủ level hoặc clear board trong giới hạn moves/time

## Merge GameScene Code Pattern

```javascript
class GameScene extends Phaser.Scene {
    constructor() { super('GameScene'); }

    create() {
        const w = this.scale.width;
        const h = this.scale.height;

        // Background
        this.bg = this.add.image(w/2, h/2, 'bg').setDisplaySize(w, h);

        // Grid config
        this.cols = GameConfig.GRID_COLS;
        this.rows = GameConfig.GRID_ROWS;
        this.cellSize = Math.min(
            (w * 0.85) / this.cols,
            (h * 0.6) / this.rows
        );
        this.gridOffsetX = (w - this.cols * this.cellSize) / 2 + this.cellSize / 2;
        this.gridOffsetY = h * 0.25 + this.cellSize / 2;

        // Grid data: 2D array of item levels (0 = empty)
        this.grid = [];
        this.gridSprites = [];

        this.initGrid();

        // Move counter
        this.movesLeft = GameConfig.MOVES_LIMIT || 10;
        this.movesText = this.add.text(w/2, 30, 'Moves: ' + this.movesLeft, {
            fontSize: '22px', color: '#fff', fontFamily: 'Arial'
        }).setOrigin(0.5).setDepth(10);

        // Score
        this.score = 0;
        this.scoreText = this.add.text(10, 10, 'Score: 0', {
            fontSize: '20px', color: '#ffcc00', fontFamily: 'Arial'
        }).setDepth(10);

        // Drag state
        this.dragItem = null;
        this.dragStartCell = null;

        // Tutorial hand
        this.showTutorialHand();

        // Responsive
        this.scale.on('resize', (gameSize) => {
            this.bg.setPosition(gameSize.width/2, gameSize.height/2);
            this.bg.setDisplaySize(gameSize.width, gameSize.height);
        });
    }

    initGrid() {
        const itemTypes = ['item_1', 'item_2', 'item_3'];
        for (let r = 0; r < this.rows; r++) {
            this.grid[r] = [];
            this.gridSprites[r] = [];
            for (let c = 0; c < this.cols; c++) {
                const level = Phaser.Math.Between(1, itemTypes.length);
                this.grid[r][c] = level;

                const x = this.gridOffsetX + c * this.cellSize;
                const y = this.gridOffsetY + r * this.cellSize;

                // Vẽ ô nền
                this.add.rectangle(x, y, this.cellSize - 4, this.cellSize - 4, 0x333333, 0.5)
                    .setStrokeStyle(1, 0x666666);

                // Vẽ item
                const sprite = this.add.image(x, y, itemTypes[level - 1])
                    .setDisplaySize(this.cellSize * 0.7, this.cellSize * 0.7)
                    .setInteractive({ draggable: true });

                sprite.gridRow = r;
                sprite.gridCol = c;
                sprite.itemLevel = level;
                this.gridSprites[r][c] = sprite;
            }
        }

        // Drag events
        this.input.on('dragstart', (pointer, gameObject) => {
            this.dragItem = gameObject;
            this.dragStartCell = { r: gameObject.gridRow, c: gameObject.gridCol };
            gameObject.setDepth(100);
            gameObject.setScale(1.2);
        });

        this.input.on('drag', (pointer, gameObject, dragX, dragY) => {
            gameObject.x = dragX;
            gameObject.y = dragY;
        });

        this.input.on('dragend', (pointer, gameObject) => {
            gameObject.setScale(1);
            gameObject.setDepth(0);

            // Find nearest cell
            const dropCell = this.getCellFromXY(pointer.x, pointer.y);

            if (dropCell && this.isAdjacent(this.dragStartCell, dropCell)) {
                this.tryMerge(this.dragStartCell, dropCell);
            } else {
                // Snap back
                this.snapToGrid(gameObject, this.dragStartCell.r, this.dragStartCell.c);
            }

            this.dragItem = null;
            this.dragStartCell = null;
        });
    }

    getCellFromXY(x, y) {
        const c = Math.round((x - this.gridOffsetX) / this.cellSize);
        const r = Math.round((y - this.gridOffsetY) / this.cellSize);
        if (r >= 0 && r < this.rows && c >= 0 && c < this.cols) {
            return { r, c };
        }
        return null;
    }

    isAdjacent(a, b) {
        return Math.abs(a.r - b.r) + Math.abs(a.c - b.c) === 1;
    }

    tryMerge(from, to) {
        const fromLevel = this.grid[from.r][from.c];
        const toLevel = this.grid[to.r][to.c];

        if (fromLevel === toLevel && fromLevel > 0) {
            // Merge thành công!
            const newLevel = fromLevel + 1;
            this.grid[to.r][to.c] = newLevel;
            this.grid[from.r][from.c] = 0;

            // Destroy old sprites
            if (this.gridSprites[from.r][from.c]) this.gridSprites[from.r][from.c].destroy();
            if (this.gridSprites[to.r][to.c]) this.gridSprites[to.r][to.c].destroy();

            // Tạo sprite mới (hoặc upgrade texture nếu có)
            const x = this.gridOffsetX + to.c * this.cellSize;
            const y = this.gridOffsetY + to.r * this.cellSize;

            const itemKey = 'item_' + Math.min(newLevel, 3); // cap at max texture
            const newSprite = this.add.image(x, y, itemKey)
                .setDisplaySize(this.cellSize * 0.7, this.cellSize * 0.7)
                .setInteractive({ draggable: true });
            newSprite.gridRow = to.r;
            newSprite.gridCol = to.c;
            newSprite.itemLevel = newLevel;
            this.gridSprites[to.r][to.c] = newSprite;
            this.gridSprites[from.r][from.c] = null;

            // Scale pop effect
            this.tweens.add({
                targets: newSprite,
                scaleX: 1.3, scaleY: 1.3,
                duration: 150, yoyo: true
            });

            this.score += newLevel * 100;
            this.scoreText.setText('Score: ' + this.score);

            // Check win condition
            if (newLevel >= (GameConfig.MERGE_LEVELS || 5)) {
                this.time.delayedCall(500, () => this.endGame(true));
                return;
            }
        } else {
            // Không merge được, snap back
            this.snapToGrid(this.dragItem, this.dragStartCell.r, this.dragStartCell.c);
        }

        this.movesLeft--;
        this.movesText.setText('Moves: ' + this.movesLeft);
        if (this.movesLeft <= 0) {
            this.time.delayedCall(500, () => this.endGame(false));
        }
    }

    snapToGrid(sprite, r, c) {
        if (!sprite) return;
        const x = this.gridOffsetX + c * this.cellSize;
        const y = this.gridOffsetY + r * this.cellSize;
        this.tweens.add({
            targets: sprite,
            x: x, y: y,
            duration: 150, ease: 'Back.easeOut'
        });
    }

    showTutorialHand() {
        const x1 = this.gridOffsetX + 0 * this.cellSize;
        const y1 = this.gridOffsetY + 0 * this.cellSize;
        const x2 = this.gridOffsetX + 1 * this.cellSize;

        const hand = this.add.text(x1 + 20, y1 + 30, '👆', { fontSize: '32px' })
            .setDepth(200);

        this.tweens.add({
            targets: hand,
            x: x2 + 20,
            duration: 1000,
            yoyo: true, repeat: 2,
            onComplete: () => hand.destroy()
        });
    }

    endGame(isWin) {
        this.scene.start('EndCardScene', { isWin: isWin });
    }
}
```

## Lưu ý
- Grid auto-sizes dựa trên screen width → responsive tự nhiên
- `cellSize` tính từ available space, đảm bảo fit mọi màn hình
- Merge logic: cùng level + adjacent → level+1
- Nếu làm Match-3 thay vì Merge: đổi logic thành "chọn 3 ô cùng loại liền kề" → destroy + fill down
- Tutorial hand tự hủy sau 3 lần lặp — không block gameplay
- Item textures: dùng `item_1`, `item_2`, `item_3` → map từ GAME_ASSETS
