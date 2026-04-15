# The Scaffolder - Boilerplate Architecture v2.0

Cấu trúc chuẩn cho file `index.html` duy nhất khi sản xuất Playable Ads bằng Phaser 3.
Phiên bản này bao gồm GameConfig, Win/Lose End Card, responsive framework, và CTA tích hợp.

## Cấu Trúc HTML Tiêu Chuẩn

```html
<!DOCTYPE html>
<html lang="en">
<head>
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0, maximum-scale=1.0, user-scalable=no, viewport-fit=cover">
    <title>Playable Ad</title>
    <style>
        * { margin: 0; padding: 0; box-sizing: border-box; }
        body, html {
            background-color: #000; overflow: hidden;
            width: 100%; height: 100%;
            -webkit-touch-callout: none;
            -webkit-user-select: none;
            user-select: none;
        }
        #game-container {
            width: 100%; height: 100%;
            display: flex; justify-content: center; align-items: center;
            position: relative;
        }
        canvas {
            display: block; touch-action: none;
            user-select: none; -webkit-user-select: none;
        }
        /* CTA Overlay Button — hiện khi End Card */
        #cta-overlay {
            position: absolute; bottom: 12%; left: 50%;
            transform: translateX(-50%);
            display: none; z-index: 1000;
            padding: 16px 40px;
            background: linear-gradient(135deg, #4CAF50, #2E7D32);
            color: #fff; font-family: 'Arial Black', Arial, sans-serif;
            font-weight: 900; font-size: 22px; text-transform: uppercase;
            border-radius: 30px; border: 3px solid rgba(255,255,255,0.6);
            box-shadow: 0 6px 20px rgba(0,0,0,0.4);
            animation: ctaPulse 1.2s ease-in-out infinite;
            cursor: pointer; letter-spacing: 1px;
        }
        @keyframes ctaPulse {
            0%, 100% { transform: translateX(-50%) scale(1); box-shadow: 0 6px 20px rgba(0,0,0,0.4); }
            50% { transform: translateX(-50%) scale(1.08); box-shadow: 0 8px 30px rgba(76,175,80,0.5); }
        }
        #cta-overlay:active { transform: translateX(-50%) scale(0.95); }
    </style>

    <!-- PHASER 3 CDN (minified) -->
    <script src="https://cdn.jsdelivr.net/npm/phaser@3.80.1/dist/phaser.min.js"></script>

    <!-- GAME CONFIG — Tất cả thông số gameplay tập trung tại đây -->
    <script>
        const GameConfig = {
            // === GAMEPLAY ===
            PLAYER_SPEED: 250,
            GRAVITY: 0,
            ENEMY_SPAWN_RATE: 800,
            ENEMY_SPEED: 100,
            SHOOT_INTERVAL: 300,
            BULLET_SPEED: 500,
            GAME_DURATION: 20,
            PLAYER_HP: 3,

            // === END CARD ===
            WIN_HEADLINE: "YOU WIN!",
            WIN_CTA: "PLAY MORE!",
            LOSE_HEADLINE: "GAME OVER!",
            LOSE_CTA: "TRY AGAIN!",

            // === STORE LINK ===
            STORE_URL: "https://play.google.com/store/apps/details?id=com.your.bundle",

            // === PLATFORM (applovin | meta | google | mraid_bypass) ===
            PLATFORM: "mraid_bypass"
        };
    </script>

    <!-- ASSET DATA (Base64) — Inject từ asset_transformer output -->
    <script>
        const GAME_ASSETS = {
            // "player.webp": "data:image/webp;base64,...",
            // "enemy.webp": "data:image/webp;base64,...",
            // "bg.webp": "data:image/webp;base64,...",
        };
    </script>
</head>
<body>
    <div id="game-container"></div>
    <div id="cta-overlay" onclick="openStore()">DOWNLOAD</div>

    <script>
        // === PLATFORM BRIDGE ===
        function openStore() {
            const url = GameConfig.STORE_URL;
            switch (GameConfig.PLATFORM) {
                case 'applovin':
                case 'ironsource':
                    if (typeof mraid !== 'undefined') mraid.open(url);
                    else window.open(url, '_blank');
                    break;
                case 'meta':
                    if (typeof FbPlayableAd !== 'undefined') FbPlayableAd.onCTAClick();
                    else console.log('FbPlayableAd CTA click');
                    break;
                case 'google':
                    window.open(url, '_blank');
                    break;
                default: // mraid_bypass
                    if (typeof mraid !== 'undefined') mraid.open(url);
                    else window.open(url, '_blank');
            }
            console.log('[CTA] Store link triggered:', url);
        }

        // === BOOT SCENE (Load assets từ Base64) ===
        class BootScene extends Phaser.Scene {
            constructor() { super('BootScene'); }

            preload() {
                // Loading bar
                const w = this.scale.width;
                const h = this.scale.height;
                const bar = this.add.rectangle(w/2, h/2, w * 0.6, 20, 0x222222);
                const fill = this.add.rectangle(w/2 - w*0.3, h/2, 0, 16, 0x4CAF50).setOrigin(0, 0.5);

                this.load.on('progress', (v) => { fill.width = w * 0.6 * v; });
                this.load.on('complete', () => { bar.destroy(); fill.destroy(); });

                // Load từ GAME_ASSETS
                for (const [key, dataUri] of Object.entries(GAME_ASSETS)) {
                    const ext = key.split('.').pop().toLowerCase();
                    const cleanKey = key.replace(/\.[^.]+$/, ''); // remove extension for key
                    if (['png','jpg','jpeg','webp','gif'].includes(ext)) {
                        this.textures.addBase64(cleanKey, dataUri);
                    } else if (['mp3','wav','ogg'].includes(ext)) {
                        this.load.audio(cleanKey, dataUri);
                    }
                }
            }

            create() {
                this.scene.start('GameScene');
            }
        }

        // === GAME SCENE (Logic gameplay — thay thế theo template) ===
        class GameScene extends Phaser.Scene {
            constructor() { super('GameScene'); }

            create() {
                const w = this.scale.width;
                const h = this.scale.height;

                // Background (nếu có)
                if (this.textures.exists('bg')) {
                    this.bg = this.add.image(w/2, h/2, 'bg').setDisplaySize(w, h);
                } else {
                    this.bg = this.add.rectangle(w/2, h/2, w, h, 0x1a1a2e);
                }

                // Placeholder — thay bằng logic từ template
                this.add.text(w/2, h/2, 'Game Logic Here', {
                    fontSize: '24px', color: '#fff', fontFamily: 'Arial'
                }).setOrigin(0.5);

                // Demo: end game after duration
                this.time.delayedCall(GameConfig.GAME_DURATION * 1000, () => {
                    this.scene.start('EndCardScene', { isWin: true });
                });

                // Responsive handler
                this.scale.on('resize', (gameSize) => {
                    if (this.bg && this.bg.setPosition) {
                        this.bg.setPosition(gameSize.width/2, gameSize.height/2);
                        this.bg.setDisplaySize(gameSize.width, gameSize.height);
                    }
                });
            }
        }

        // === END CARD SCENE (Win/Lose + CTA) ===
        class EndCardScene extends Phaser.Scene {
            constructor() { super('EndCardScene'); }

            create(data) {
                const w = this.scale.width;
                const h = this.scale.height;
                const isWin = data.isWin !== false;

                // Dark overlay
                this.add.rectangle(w/2, h/2, w, h, 0x000000, 0.8);

                // Headline
                const headline = isWin ? GameConfig.WIN_HEADLINE : GameConfig.LOSE_HEADLINE;
                const headlineColor = isWin ? '#4CAF50' : '#ff4444';
                this.add.text(w/2, h * 0.3, headline, {
                    fontSize: Math.min(w * 0.08, 36) + 'px',
                    color: headlineColor,
                    fontFamily: 'Arial',
                    fontStyle: 'bold',
                    align: 'center',
                    wordWrap: { width: w * 0.85 }
                }).setOrigin(0.5);

                // Sub text
                const subText = isWin
                    ? 'Unlock more levels!'
                    : 'Can you do better?';
                this.add.text(w/2, h * 0.42, subText, {
                    fontSize: Math.min(w * 0.05, 20) + 'px',
                    color: '#cccccc',
                    fontFamily: 'Arial', align: 'center'
                }).setOrigin(0.5);

                // In-canvas CTA button
                const ctaText = isWin ? GameConfig.WIN_CTA : GameConfig.LOSE_CTA;
                const btn = this.add.text(w/2, h * 0.6, ctaText, {
                    fontSize: Math.min(w * 0.065, 28) + 'px',
                    color: '#ffffff',
                    fontFamily: 'Arial',
                    fontStyle: 'bold',
                    backgroundColor: '#4CAF50',
                    padding: { x: 30, y: 14 }
                }).setOrigin(0.5).setInteractive();

                // Pulse
                this.tweens.add({
                    targets: btn,
                    scaleX: 1.08, scaleY: 1.08,
                    duration: 700, yoyo: true, repeat: -1,
                    ease: 'Sine.easeInOut'
                });

                btn.on('pointerdown', () => openStore());

                // Also show HTML overlay
                const overlay = document.getElementById('cta-overlay');
                if (overlay) {
                    overlay.textContent = ctaText;
                    overlay.style.display = 'block';
                }
            }
        }

        // === PHASER INIT ===
        const game = new Phaser.Game({
            type: Phaser.AUTO,
            parent: 'game-container',
            transparent: true,
            scale: {
                mode: Phaser.Scale.RESIZE,
                autoCenter: Phaser.Scale.CENTER_BOTH,
                width: '100%',
                height: '100%'
            },
            physics: {
                default: 'arcade',
                arcade: { gravity: { y: GameConfig.GRAVITY }, debug: false }
            },
            scene: [BootScene, GameScene, EndCardScene],
            input: { activePointers: 2 }
        });

        // Handle visibility change (tab switching)
        document.addEventListener('visibilitychange', () => {
            if (!document.hidden && game && game.sound) {
                game.sound.context.resume();
            }
        });
    </script>
</body>
</html>
```

## Lưu ý Quan trọng cho AI Agent

### Responsive Framework
- Sử dụng `Phaser.Scale.RESIZE` — canvas tự co giãn theo viewport
- Mỗi Scene phải lắng nghe `this.scale.on('resize', callback)` để reposition objects
- Background luôn `setDisplaySize(width, height)` để full-screen
- Text size dùng `Math.min(w * ratio, maxPx)` để scale theo màn hình

### Asset Loading (BootScene)
- `textures.addBase64(key, dataUri)` cho ảnh — key không có extension
- `load.audio(key, dataUri)` cho audio
- Loading bar tự tính progress

### GameConfig Pattern
- Mọi thông số gameplay đều nằm trong `GameConfig`
- User có thể edit nhanh mà không cần đọc code game
- End Card text cũng cấu hình từ GameConfig
- Platform/Store URL cũng cấu hình tại đây

### CTA Dual Layer
- In-canvas button (Phaser text) — luôn hoạt động
- HTML overlay (#cta-overlay) — backup, hiện khi End Card
- Cả 2 đều gọi `openStore()`

### Platform Bridge
- `openStore()` tự detect platform từ `GameConfig.PLATFORM`
- Default: `mraid_bypass` — hoạt động với hầu hết ad networks
- Không thêm `<script src="mraid.js">` trừ khi chắc chắn platform
