# Template: Survivor / Auto-shoot

Template Phaser 3 cho game kiểu Vampire Survivors — nhân vật tự bắn, di chuyển bằng drag, kẻ địch spawn từ rìa màn hình.

## Khi nào dùng
- GDD có keyword: survivor, auto-shoot, swarm, horde, wave
- Player di chuyển tự do (top-down), đạn tự bay
- Enemies spawn liên tục từ rìa

## GameScene Code Pattern

```javascript
class GameScene extends Phaser.Scene {
    constructor() { super('GameScene'); }

    create() {
        const w = this.scale.width;
        const h = this.scale.height;

        // Background
        this.bg = this.add.image(w/2, h/2, 'bg').setDisplaySize(w, h);

        // Player
        this.player = this.physics.add.sprite(w/2, h/2, 'player').setScale(0.5);
        this.player.setCollideWorldBounds(true);
        this.player.body.setSize(
            this.player.width * 0.6,
            this.player.height * 0.6
        );
        this.playerHP = GameConfig.PLAYER_HP;

        // Groups
        this.enemies = this.physics.add.group();
        this.bullets = this.physics.add.group();

        // Drag to move
        this.input.on('pointermove', (pointer) => {
            if (pointer.isDown) {
                this.player.x = Phaser.Math.Clamp(pointer.x, 30, w - 30);
                this.player.y = Phaser.Math.Clamp(pointer.y, 30, h - 30);
            }
        });

        // Auto-shoot timer
        this.shootTimer = this.time.addEvent({
            delay: GameConfig.SHOOT_INTERVAL,
            callback: this.autoShoot,
            callbackScope: this,
            loop: true
        });

        // Enemy spawn timer
        this.spawnTimer = this.time.addEvent({
            delay: GameConfig.ENEMY_SPAWN_RATE,
            callback: this.spawnEnemy,
            callbackScope: this,
            loop: true
        });

        // Game duration timer
        this.timeLeft = GameConfig.GAME_DURATION;
        this.timerText = this.add.text(w/2, 30, this.timeLeft + 's',
            { fontSize: '24px', color: '#fff', fontFamily: 'Arial' }
        ).setOrigin(0.5).setDepth(10);

        this.gameTimer = this.time.addEvent({
            delay: 1000,
            callback: () => {
                this.timeLeft--;
                this.timerText.setText(this.timeLeft + 's');
                if (this.timeLeft <= 0) {
                    this.endGame(true); // WIN
                }
            },
            callbackScope: this,
            loop: true
        });

        // Collisions
        this.physics.add.overlap(this.bullets, this.enemies, this.hitEnemy, null, this);
        this.physics.add.overlap(this.player, this.enemies, this.hitPlayer, null, this);

        // HP display
        this.hpText = this.add.text(10, 10, 'HP: ' + this.playerHP,
            { fontSize: '20px', color: '#ff4444', fontFamily: 'Arial' }
        ).setDepth(10);

        // Responsive
        this.scale.on('resize', (gameSize) => {
            this.bg.setPosition(gameSize.width/2, gameSize.height/2);
            this.bg.setDisplaySize(gameSize.width, gameSize.height);
        });
    }

    autoShoot() {
        // Find nearest enemy
        let nearest = null;
        let minDist = Infinity;
        this.enemies.getChildren().forEach(enemy => {
            if (!enemy.active) return;
            let dist = Phaser.Math.Distance.Between(
                this.player.x, this.player.y, enemy.x, enemy.y
            );
            if (dist < minDist) { minDist = dist; nearest = enemy; }
        });

        if (!nearest) return;

        let bullet = this.bullets.create(this.player.x, this.player.y, 'bullet');
        bullet.setScale(0.3);
        this.physics.moveToObject(bullet, nearest, GameConfig.BULLET_SPEED);

        // Auto-destroy bullet after 2s
        this.time.delayedCall(2000, () => { if (bullet.active) bullet.destroy(); });
    }

    spawnEnemy() {
        const w = this.scale.width;
        const h = this.scale.height;
        // Random edge
        let x, y;
        const side = Phaser.Math.Between(0, 3);
        switch(side) {
            case 0: x = Phaser.Math.Between(0, w); y = -30; break;
            case 1: x = w + 30; y = Phaser.Math.Between(0, h); break;
            case 2: x = Phaser.Math.Between(0, w); y = h + 30; break;
            case 3: x = -30; y = Phaser.Math.Between(0, h); break;
        }
        let enemy = this.enemies.create(x, y, 'enemy').setScale(0.4);
        enemy.body.setSize(enemy.width * 0.6, enemy.height * 0.6);
        this.physics.moveToObject(enemy, this.player, GameConfig.ENEMY_SPEED);
    }

    hitEnemy(bullet, enemy) {
        bullet.destroy();
        // Flash effect
        enemy.setTint(0xffffff);
        this.time.delayedCall(100, () => { enemy.destroy(); });
    }

    hitPlayer(player, enemy) {
        enemy.destroy();
        this.playerHP--;
        this.hpText.setText('HP: ' + this.playerHP);
        // Flash red
        player.setTint(0xff0000);
        this.time.delayedCall(200, () => { player.clearTint(); });

        if (this.playerHP <= 0) {
            this.endGame(false); // LOSE
        }
    }

    endGame(isWin) {
        this.shootTimer.remove();
        this.spawnTimer.remove();
        this.gameTimer.remove();
        this.physics.pause();
        this.scene.start('EndCardScene', { isWin: isWin });
    }
}
```

## EndCardScene Pattern (Win/Lose dual)

```javascript
class EndCardScene extends Phaser.Scene {
    constructor() { super('EndCardScene'); }

    create(data) {
        const w = this.scale.width;
        const h = this.scale.height;
        const isWin = data.isWin;

        // Dark overlay
        this.add.rectangle(w/2, h/2, w, h, 0x000000, 0.8).setDepth(0);

        // Headline
        const headline = isWin
            ? GameConfig.WIN_HEADLINE || 'YOU SURVIVED!'
            : GameConfig.LOSE_HEADLINE || 'GAME OVER!';
        this.add.text(w/2, h * 0.3, headline, {
            fontSize: '32px', color: '#fff', fontFamily: 'Arial',
            fontStyle: 'bold', align: 'center', wordWrap: { width: w * 0.8 }
        }).setOrigin(0.5).setDepth(1);

        // CTA Button
        const ctaText = isWin
            ? GameConfig.WIN_CTA || 'PLAY MORE!'
            : GameConfig.LOSE_CTA || 'TRY AGAIN!';

        const btn = this.add.text(w/2, h * 0.6, ctaText, {
            fontSize: '28px', color: '#fff', fontFamily: 'Arial',
            fontStyle: 'bold', backgroundColor: '#4CAF50',
            padding: { x: 30, y: 15 }
        }).setOrigin(0.5).setInteractive().setDepth(1);

        // Pulse animation
        this.tweens.add({
            targets: btn,
            scaleX: 1.1, scaleY: 1.1,
            duration: 800, yoyo: true, repeat: -1, ease: 'Sine.easeInOut'
        });

        btn.on('pointerdown', () => { openStore(); });

        // Also show HTML CTA overlay
        const ctaOverlay = document.getElementById('cta-button-overlay');
        if (ctaOverlay) {
            ctaOverlay.textContent = ctaText;
            ctaOverlay.style.display = 'block';
        }
    }
}
```

## Lưu ý
- Enemy `moveToObject` hướng về player tại thời điểm spawn — enemy bay thẳng, không tracking liên tục (tiết kiệm CPU)
- Nếu muốn enemy tracking liên tục, thêm `update()` với `this.physics.moveToObject` mỗi frame — nhưng cân nhắc performance
- Bullet tự hủy sau 2s để tránh leak memory
- Hitbox nên setSize ~60% kích thước sprite để "cảm giác" công bằng hơn
