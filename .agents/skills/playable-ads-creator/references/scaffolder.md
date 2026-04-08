# The Scaffolder - Boilerplate Architecture

Quy định cấu trúc cho file `index.html` duy nhất khi sản xuất Playable Ads bằng Phaser 3.

## Cấu Trúc HTML Tiêu Chuẩn

```html
<!DOCTYPE html>
<html lang="en">
<head>
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0, maximum-scale=1.0, user-scalable=no, viewport-fit=cover">
    <title>Playable Ad</title>
    <style>
        body, html { margin: 0; padding: 0; background-color: #000; overflow: hidden; width: 100%; height: 100%; }
        #game-container { width: 100%; height: 100%; display: flex; justify-content: center; align-items: center; }
        canvas { display: block; touch-action: none; user-select: none; -webkit-user-select: none; }
        #cta-button-overlay {
            position: absolute; bottom: 10%; left: 50%; transform: translateX(-50%);
            display: none; padding: 15px 30px; background: linear-gradient(to bottom, #4CAF50, #2E7D32);
            color: white; font-family: sans-serif; font-weight: bold; font-size: 20px;
            border-radius: 25px; border: 2px solid white; box-shadow: 0 4px 6px rgba(0,0,0,0.3);
            animation: pulse 1.5s infinite; cursor: pointer; text-transform: uppercase;
        }
        @keyframes pulse { 0% { transform: translateX(-50%) scale(1); } 50% { transform: translateX(-50%) scale(1.1); } 100% { transform: translateX(-50%) scale(1); } }
    </style>
    <!-- THƯ VIỆN PHASER 3 MINIFIED -->
    <script src="https://cdn.jsdelivr.net/npm/phaser@3.80.1/dist/phaser.min.js"></script>
    
    <!-- DATA BASE64 NHÚNG VÀO ĐÂY (Bước 1) -->
    <script>
        const GAME_ASSETS = {
            // "bg.png": "data:image/png;base64,iVBORw0KGgo...",
            // "click.mp3": "data:audio/mp3;base64,SUQzBAA..."
        };
    </script>
</head>
<body>
    <div id="game-container"></div>
    <div id="cta-button-overlay" onclick="openStore()">PLAY NOW</div>

    <script>
        // --- CẦU NỐI MẠNG QUẢNG CÁO SẼ ĐƯỢC CHÈN VÀO HÀM NÀY (Bước 4) ---
        function openStore() {
            console.log("Redirecting to store...");
            // Tuỳ nền tảng (Ví dụ AppLovin: mraid.open(url), FB: FbPlayableAd.onCTAClick())
        }

        // --- CÁC SCENE CỦA GAME (Bước 3) ---
        class BootScene extends Phaser.Scene {
            constructor() { super('BootScene'); }
            preload() {
                // Đọc từ base64 thay vì load URL file
                for (let key in GAME_ASSETS) {
                    if(key.endsWith('.png') || key.endsWith('.jpg')) {
                        this.load.image(key, GAME_ASSETS[key]);
                    } else if(key.endsWith('.mp3') || key.endsWith('.wav')) {
                        this.load.audio(key, GAME_ASSETS[key]);
                    }
                }
            }
            create() {
                this.scene.start('GameScene');
            }
        }

        class GameScene extends Phaser.Scene {
            constructor() { super('GameScene'); }
            create() {
                // Game logic
                const bg = this.add.image(this.cameras.main.centerX, this.cameras.main.centerY, 'bg.png');
                bg.setDisplaySize(this.scale.width, this.scale.height); // Responsive bg
                
                // Demo EndGame
                this.time.delayedCall(3000, () => { this.scene.start('EndCardScene'); });
            }
        }

        class EndCardScene extends Phaser.Scene {
            constructor() { super('EndCardScene'); }
            create() {
                this.add.text(this.cameras.main.centerX, this.cameras.main.centerY - 50, "Missions Complete!", { fontSize: '40px', color: '#fff' }).setOrigin(0.5);
                // Hiển thị nút CTA HTML 
                document.getElementById('cta-button-overlay').style.display = 'block';
            }
        }

        // --- CẤU HÌNH PHASER (CHÚ Ý RESPONSIVE) ---
        const config = {
            type: Phaser.AUTO,
            parent: 'game-container',
            transparent: true,
            scale: {
                mode: Phaser.Scale.RESIZE, // Tự động co giãn theo trình duyệt
                autoCenter: Phaser.Scale.CENTER_BOTH,
                width: '100%',
                height: '100%'
            },
            scene: [BootScene, GameScene, EndCardScene]
        };

        const game = new Phaser.Game(config);

        // Xử lý mất focus (Tab ẩn đi / hiển thị lại) không bị đen màn hình
        window.addEventListener('focus', () => {
            if (game && game.sound) { game.sound.context.resume(); }
        });
    </script>
</body>
</html>
```

## Lưu ý dành cho AI Agent
- Luôn sử dụng `Phaser.Scale.RESIZE` để xử lý Responsive. Khi màn hình bị resize, bạn cần cập nhật đối tượng hiển thị (Background full màn, căn giữa màn hình) bằng cách lắng nghe sự kiện `this.scale.on('resize', this.resizeParams, this)`.
- Không sử dụng các tài nguyên ngoài HTML, thay vào đó đọc dữ liệu từ const `GAME_ASSETS` dưới dạng Base64 URI.
