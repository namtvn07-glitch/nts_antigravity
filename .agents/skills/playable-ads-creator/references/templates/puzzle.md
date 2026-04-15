# Template: Puzzle / Sort / Hidden Object

Template Phaser 3 cho game kiểu giải đố — kéo thả phân loại, tìm đồ vật ẩn, hoặc sắp xếp logic.

## Khi nào dùng
- GDD có keyword: puzzle, sort, find, hidden, organize, drag, arrange, spot difference
- Gameplay dựa trên tap/drag từng item vào đúng vị trí
- Thường có time limit hoặc item count cố định

## Pattern A: Drag & Sort (Kéo thả Phân loại)

```javascript
class GameScene extends Phaser.Scene {
    constructor() { super('GameScene'); }

    create() {
        const w = this.scale.width;
        const h = this.scale.height;

        this.bg = this.add.image(w/2, h/2, 'bg').setDisplaySize(w, h);

        // Target zones (drop areas)
        this.zones = [];
        const zoneCount = GameConfig.TARGET_ZONES || 3;
        const zoneWidth = (w * 0.9) / zoneCount;

        for (let i = 0; i < zoneCount; i++) {
            const zx = (w * 0.05) + zoneWidth * i + zoneWidth / 2;
            const zy = h * 0.75;

            const zone = this.add.rectangle(zx, zy, zoneWidth - 10, h * 0.2, 0x333333, 0.4)
                .setStrokeStyle(2, 0x88ff88);
            zone.zoneIndex = i;
            this.zones.push(zone);

            // Zone label
            this.add.text(zx, zy, GameConfig.ZONE_LABELS?.[i] || 'Zone ' + (i+1), {
                fontSize: '16px', color: '#aaa', fontFamily: 'Arial'
            }).setOrigin(0.5);
        }

        // Draggable items
        this.items = [];
        this.itemsCorrect = 0;
        const itemCount = GameConfig.ITEM_COUNT || 6;
        const itemKeys = GameConfig.ITEM_KEYS || ['item_1','item_2','item_3','item_4','item_5','item_6'];
        // Each item has a correct zone
        const correctZones = GameConfig.CORRECT_ZONES || [0,0,1,1,2,2];

        for (let i = 0; i < itemCount; i++) {
            const ix = Phaser.Math.Between(w * 0.1, w * 0.9);
            const iy = Phaser.Math.Between(h * 0.1, h * 0.45);

            const item = this.add.image(ix, iy, itemKeys[i % itemKeys.length])
                .setDisplaySize(this.cellSize(w), this.cellSize(w))
                .setInteractive({ draggable: true });

            item.correctZone = correctZones[i % correctZones.length];
            item.originalX = ix;
            item.originalY = iy;
            item.placed = false;
            this.items.push(item);
        }

        // Drag events
        this.input.on('drag', (pointer, gameObject, dragX, dragY) => {
            gameObject.x = dragX;
            gameObject.y = dragY;
        });

        this.input.on('dragend', (pointer, gameObject) => {
            if (gameObject.placed) return;

            // Check if dropped on correct zone
            let droppedOnZone = false;
            this.zones.forEach((zone, idx) => {
                const bounds = zone.getBounds();
                if (bounds.contains(pointer.x, pointer.y)) {
                    droppedOnZone = true;
                    if (idx === gameObject.correctZone) {
                        // Correct!
                        gameObject.placed = true;
                        gameObject.disableInteractive();
                        this.tweens.add({
                            targets: gameObject,
                            x: zone.x, y: zone.y,
                            scaleX: 0.8, scaleY: 0.8,
                            duration: 200
                        });
                        gameObject.setTint(0x00ff00);
                        this.itemsCorrect++;

                        if (this.itemsCorrect >= itemCount) {
                            this.time.delayedCall(500, () => this.endGame(true));
                        }
                    } else {
                        // Wrong zone - shake and snap back
                        this.tweens.add({
                            targets: gameObject,
                            x: gameObject.originalX, y: gameObject.originalY,
                            duration: 300, ease: 'Back.easeOut'
                        });
                        // Shake effect
                        this.cameras.main.shake(100, 0.005);
                    }
                }
            });

            if (!droppedOnZone) {
                // Snap back if not on any zone
                this.tweens.add({
                    targets: gameObject,
                    x: gameObject.originalX, y: gameObject.originalY,
                    duration: 200
                });
            }
        });

        // Timer
        this.timeLeft = GameConfig.TIME_LIMIT || 20;
        this.timerText = this.add.text(w/2, 15, '⏱ ' + this.timeLeft, {
            fontSize: '22px', color: '#fff', fontFamily: 'Arial'
        }).setOrigin(0.5).setDepth(10);

        this.gameTimer = this.time.addEvent({
            delay: 1000,
            callback: () => {
                this.timeLeft--;
                this.timerText.setText('⏱ ' + this.timeLeft);
                if (this.timeLeft <= 0) this.endGame(false);
            },
            callbackScope: this,
            loop: true
        });

        // Tutorial hand
        if (this.items.length > 0) {
            const firstItem = this.items[0];
            const targetZone = this.zones[firstItem.correctZone];
            const hand = this.add.text(firstItem.x + 15, firstItem.y + 15, '👆', {
                fontSize: '32px'
            }).setDepth(200);
            this.tweens.add({
                targets: hand,
                x: targetZone.x + 15, y: targetZone.y + 15,
                duration: 1200,
                yoyo: true, repeat: 2,
                onComplete: () => hand.destroy()
            });
        }

        // Responsive
        this.scale.on('resize', (gameSize) => {
            this.bg.setPosition(gameSize.width/2, gameSize.height/2);
            this.bg.setDisplaySize(gameSize.width, gameSize.height);
        });
    }

    cellSize(screenWidth) {
        return Math.min(screenWidth * 0.15, 80);
    }

    endGame(isWin) {
        this.gameTimer.remove();
        this.scene.start('EndCardScene', { isWin: isWin });
    }
}
```

## Pattern B: Hidden Object (Tìm đồ vật ẩn)

```javascript
class GameScene extends Phaser.Scene {
    constructor() { super('GameScene'); }

    create() {
        const w = this.scale.width;
        const h = this.scale.height;

        // Full-screen background scene
        this.bg = this.add.image(w/2, h/2, 'bg').setDisplaySize(w, h);

        // Hidden items
        const itemPositions = GameConfig.ITEM_POSITIONS || [
            { key: 'item_1', x: 0.3, y: 0.4 },
            { key: 'item_2', x: 0.7, y: 0.6 },
            { key: 'item_3', x: 0.5, y: 0.25 }
        ];

        this.itemsFound = 0;
        this.totalItems = itemPositions.length;

        // Progress text
        this.progressText = this.add.text(w/2, 20,
            `Found: 0/${this.totalItems}`, {
            fontSize: '20px', color: '#fff', fontFamily: 'Arial',
            backgroundColor: '#000000aa', padding: { x: 10, y: 5 }
        }).setOrigin(0.5).setDepth(10);

        // Place items (slightly transparent / blended into scene)
        itemPositions.forEach(cfg => {
            const item = this.add.image(w * cfg.x, h * cfg.y, cfg.key)
                .setDisplaySize(60, 60)
                .setAlpha(0.6)  // Semi-transparent to "hide"
                .setInteractive();

            item.on('pointerdown', () => {
                item.removeInteractive();
                item.setAlpha(1);
                item.setTint(0x00ff00);

                // Pop effect
                this.tweens.add({
                    targets: item,
                    scaleX: 1.5, scaleY: 1.5,
                    duration: 200, yoyo: true
                });

                // Circle highlight
                const circle = this.add.circle(item.x, item.y, 40)
                    .setStrokeStyle(3, 0x00ff00).setDepth(5);
                this.tweens.add({
                    targets: circle,
                    alpha: 0,
                    duration: 1000,
                    onComplete: () => circle.destroy()
                });

                this.itemsFound++;
                this.progressText.setText(`Found: ${this.itemsFound}/${this.totalItems}`);

                if (this.itemsFound >= this.totalItems) {
                    this.time.delayedCall(800, () => this.endGame(true));
                }
            });
        });

        // Timer
        this.timeLeft = GameConfig.TIME_LIMIT || 15;
        this.timerText = this.add.text(w - 10, 20, this.timeLeft + 's', {
            fontSize: '20px', color: '#ff6666', fontFamily: 'Arial'
        }).setOrigin(1, 0.5).setDepth(10);

        this.time.addEvent({
            delay: 1000,
            callback: () => {
                this.timeLeft--;
                this.timerText.setText(this.timeLeft + 's');
                if (this.timeLeft <= 0) this.endGame(false);
            },
            callbackScope: this,
            loop: true
        });

        // Hint: pulse effect on items periodically
        this.time.addEvent({
            delay: 5000,
            callback: () => {
                // Brief flash on all unfound items
                this.children.getAll().forEach(child => {
                    if (child.input && child.input.enabled) {
                        this.tweens.add({
                            targets: child,
                            alpha: 1, duration: 300,
                            yoyo: true
                        });
                    }
                });
            },
            callbackScope: this,
            loop: true
        });
    }

    endGame(isWin) {
        this.scene.start('EndCardScene', { isWin: isWin });
    }
}
```

## Lưu ý
- **Drag & Sort:** Items scatter ở nửa trên, zones ở nửa dưới — tạo layout tự nhiên cho mobile dọc
- **Hidden Object:** Items setAlpha(0.6) để "ẩn" mà vẫn findable — cân bằng giữa thách thức và không frustrating
- **Hint system:** Mỗi 5 giây flash items chưa tìm — giữ engagement, tránh User bỏ cuộc
- **Snap back tween:** Dùng `Back.easeOut` tạo cảm giác "bật" vui mắt
- **Camera shake on wrong:** Phản hồi visual nhẹ khi sai — không quá aggressive
- Tutorial hand di chuyển từ item đầu tiên đến zone đúng — gợi ý rõ ràng gameplay
