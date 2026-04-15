# Template: Runner / Dodge

Template Phaser 3 cho game kiểu Endless Runner — màn hình tự cuộn, nhân vật nhảy/đổi lane để né chướng ngại vật.

## Khi nào dùng
- GDD có keyword: runner, run, dodge, jump, lane, obstacle, endless
- Auto-scroll ngang hoặc dọc, nhân vật né hoặc nhảy
- Tap to jump hoặc Swipe to change lane

## GameScene Code Pattern (Tap to Jump — Side Runner)

```javascript
class GameScene extends Phaser.Scene {
    constructor() { super('GameScene'); }

    create() {
        const w = this.scale.width;
        const h = this.scale.height;

        // Scrolling background (2 images for seamless loop)
        this.bg1 = this.add.image(w/2, h/2, 'bg').setDisplaySize(w, h);
        this.bg2 = this.add.image(w + w/2, h/2, 'bg').setDisplaySize(w, h);

        // Ground line
        this.groundY = h * 0.8;
        this.add.rectangle(w/2, this.groundY + 20, w, 4, 0x666666);

        // Player
        this.player = this.physics.add.sprite(w * 0.25, this.groundY, 'player')
            .setScale(0.5);
        this.player.body.setGravityY(GameConfig.GRAVITY || 800);
        this.player.body.setSize(this.player.width * 0.5, this.player.height * 0.7);
        this.player.setCollideWorldBounds(true);

        // Invisible ground collider
        this.ground = this.physics.add.staticImage(w/2, this.groundY + 30, null);
        this.ground.setDisplaySize(w, 10);
        this.ground.refreshBody();
        this.ground.setVisible(false);
        this.physics.add.collider(this.player, this.ground);

        // Obstacles group
        this.obstacles = this.physics.add.group();

        // Tap to jump
        this.input.on('pointerdown', () => {
            if (this.player.body.touching.down || this.player.body.blocked.down) {
                this.player.setVelocityY(GameConfig.JUMP_FORCE || -500);
            }
        });

        // Spawn timer
        this.spawnTimer = this.time.addEvent({
            delay: GameConfig.OBSTACLE_SPAWN_RATE || 1500,
            callback: this.spawnObstacle,
            callbackScope: this,
            loop: true
        });

        // Collision
        this.physics.add.overlap(this.player, this.obstacles, this.hitObstacle, null, this);

        // Timer
        this.timeLeft = GameConfig.GAME_DURATION || 15;
        this.timerText = this.add.text(w/2, 30, this.timeLeft + 's', {
            fontSize: '24px', color: '#fff', fontFamily: 'Arial'
        }).setOrigin(0.5).setDepth(10);

        this.gameTimer = this.time.addEvent({
            delay: 1000,
            callback: () => {
                this.timeLeft--;
                this.timerText.setText(this.timeLeft + 's');
                if (this.timeLeft <= 0) this.endGame(true);
            },
            callbackScope: this,
            loop: true
        });

        // Score by distance
        this.score = 0;
        this.scoreText = this.add.text(10, 10, '0m', {
            fontSize: '20px', color: '#ffcc00', fontFamily: 'Arial'
        }).setDepth(10);

        this.scrollSpeed = GameConfig.SCROLL_SPEED || 300;
        this.isGameOver = false;

        // Tutorial hand
        const hand = this.add.text(w * 0.25, this.groundY - 80, '👆\nTAP', {
            fontSize: '28px', align: 'center'
        }).setOrigin(0.5).setDepth(200);
        this.tweens.add({
            targets: hand,
            y: this.groundY - 120,
            duration: 600,
            yoyo: true, repeat: 3,
            onComplete: () => hand.destroy()
        });

        // Responsive
        this.scale.on('resize', (gameSize) => {
            this.bg1.setDisplaySize(gameSize.width, gameSize.height);
            this.bg2.setDisplaySize(gameSize.width, gameSize.height);
        });
    }

    update(time, delta) {
        if (this.isGameOver) return;

        // Scroll background
        const dx = this.scrollSpeed * delta / 1000;
        this.bg1.x -= dx;
        this.bg2.x -= dx;

        if (this.bg1.x <= -this.scale.width / 2) {
            this.bg1.x = this.bg2.x + this.scale.width;
        }
        if (this.bg2.x <= -this.scale.width / 2) {
            this.bg2.x = this.bg1.x + this.scale.width;
        }

        // Move obstacles left
        this.obstacles.getChildren().forEach(obs => {
            obs.x -= dx;
            if (obs.x < -50) obs.destroy();
        });

        // Update score
        this.score += dx * 0.01;
        this.scoreText.setText(Math.floor(this.score) + 'm');
    }

    spawnObstacle() {
        if (this.isGameOver) return;
        const w = this.scale.width;

        const obs = this.obstacles.create(w + 50, this.groundY, 'obstacle')
            .setScale(0.4);
        obs.body.setAllowGravity(false);
        obs.body.setImmovable(true);
        obs.body.setSize(obs.width * 0.5, obs.height * 0.7);
    }

    hitObstacle(player, obstacle) {
        this.isGameOver = true;
        this.spawnTimer.remove();
        this.gameTimer.remove();
        this.physics.pause();

        // Player crash effect
        player.setTint(0xff0000);
        this.tweens.add({
            targets: player,
            angle: 90, alpha: 0.5,
            duration: 300,
            onComplete: () => {
                this.time.delayedCall(500, () => this.endGame(false));
            }
        });
    }

    endGame(isWin) {
        this.scene.start('EndCardScene', { isWin: isWin });
    }
}
```

## Variant: Lane-Based (3 Lanes, Swipe to Switch)

Nếu GDD yêu cầu lane-based thay vì jump:

```javascript
// Thay đổi trong create():
this.currentLane = 1; // 0, 1, 2
this.lanes = [w * 0.25, w * 0.5, w * 0.75];
this.player.x = this.lanes[this.currentLane];

// Swipe detection
let startX = 0;
this.input.on('pointerdown', (p) => { startX = p.x; });
this.input.on('pointerup', (p) => {
    const dx = p.x - startX;
    if (Math.abs(dx) > 30) {
        if (dx > 0 && this.currentLane < 2) this.currentLane++;
        else if (dx < 0 && this.currentLane > 0) this.currentLane--;
        this.tweens.add({
            targets: this.player,
            x: this.lanes[this.currentLane],
            duration: 150, ease: 'Quad.easeOut'
        });
    }
});

// Trong spawnObstacle(), random lane:
const lane = Phaser.Math.Between(0, 2);
obs.x = this.lanes[lane];
```

## Lưu ý
- Background seamless loop: 2 images đặt cạnh nhau, khi 1 đi hết → nhảy sang phía sau
- Obstacle spawn từ bên phải, di chuyển sang trái bằng dx mỗi frame (không dùng velocity — smoother)
- Ground collision dùng invisible static body — đơn giản hơn tilemap
- Crash effect: tint đỏ + xoay nghiêng + fade → tạo cảm giác "va chạm" mạnh
- Lane variant: swipe threshold 30px để tránh tap nhầm thành swipe
