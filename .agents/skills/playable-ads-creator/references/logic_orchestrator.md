# Logic Orchestrator - Trình điều phối Gameplay

## Giới hạn cốt lõi
Playable Ads rất khác thường so với Game App đầy đủ:
1. **Thời lượng chơi ngắn**: Khoảng 15 - 30 giây.
2. **Quy tắc win/lose đơn giản**: Thắng sau 2-3 lượt nhấp, hoặc thua cố ý để khiêu khích tải game.
3. **Luôn phải dẫn về Endcard**.

## Các Pattern Thường Gặp
Bạn phải hướng dẫn logic Phaser của bạn theo các block mẫu này thay vì tự sáng tạo ra các vòng lặp game dài dòng.

### 1. The "Hidden Object" Pattern (Tìm đồ)
Kịch bản: Có 3 item bị ẩn trong một background lộn xộn.
Code Pattern:
```javascript
// Trong create() của GameScene
let itemsFound = 0;
const MAX_ITEMS = 3;

let hidden1 = this.add.image(100, 200, 'item1.png').setInteractive();
hidden1.on('pointerdown', function() {
    this.removeInteractive();
    this.setTint(0x00ff00); // Đổi màu để đánh dấu tìm thấy
    
    // Phát âm thanh
    this.scene.sound.play('click.mp3'); 

    itemsFound++;
    if(itemsFound === MAX_ITEMS) {
        this.scene.time.delayedCall(1000, () => {
            this.scene.scene.start('EndCardScene');
        });
    }
});
```

### 2. The "Swipe To Action" Pattern (Vuốt / Kéo)
Kịch bản: Kéo vũ khí hoặc kéo nhân vật đi qua mê cung.
Code Pattern:
```javascript
// Trong create() của GameScene
let player = this.add.image(centerX, centerY, 'character.png').setInteractive();
this.input.setDraggable(player);

this.input.on('drag', function (pointer, gameObject, dragX, dragY) {
    gameObject.x = dragX;
    gameObject.y = dragY;

    // Check hit box thủ công thay vì nạp Physics engine mạnh để tiết kiệm kb
    let distance = Phaser.Math.Distance.Between(gameObject.x, gameObject.y, targetX, targetY);
    if(distance < 50) {
        // Hoàn thành nhiệm vụ
        gameObject.x = targetX;
        gameObject.y = targetY;
        this.input.setDraggable(player, false);
        this.scene.scene.start('EndCardScene');
    }
});
```

### 3. Hướng dẫn ban đầu (Tutorial Hand)
- LUÔN luôn tạo một hình ảnh cái tay (`hand.png`) và sử dụng Tween animation di chuyển nó gợi ý cho người chơi.

```javascript
let hand = this.add.image(player.x + 50, player.y + 50, 'hand.png').setScale(0.5);
this.tweens.add({
    targets: hand,
    y: hand.y + 100,
    x: hand.x + 50,
    duration: 1000,
    yoyo: true,
    repeat: -1
});

// Khi người chơi nhấp (Tap to start), ẩn cái tay đi.
this.input.on('pointerdown', () => {
    hand.setVisible(false);
    hand.destroy();
});
```

## Quản lý Responsive Scaling
Khi Game bị quay màn hình hoặc size WebView thay đổi:
```javascript
// Trong create()
this.scale.on('resize', this.resizeGame, this);

// Thêm hàm resizeGame vào GameScene class
resizeGame(gameSize) {
    let width = gameSize.width;
    let height = gameSize.height;

    // Căn chỉnh lại các Object UI ví dụ Background
    this.bg.setPosition(width/2, height/2);
    this.bg.setDisplaySize(width, height);
}
```

Hãy ưu tiên Performance và Logic ít điều kiện rẽ nhánh nhất có thể. Tận dụng Tweens để nội suy UI thay vì dùng Update Loop.
