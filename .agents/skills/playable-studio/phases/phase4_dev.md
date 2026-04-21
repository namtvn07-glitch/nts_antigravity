# Phase 4: DEV (Playable Engineer)

**Your Role**: Playable Engineer

## Instructions
1. **Context Initialization**: STRICTLY use the `list_dir` tool on the project's `assets/` directory to discover the *actual* physical names of the generated `.webp` files. Read `GDD.md` to refresh memory of the mechanic logic.
2. **Implement Skeleton Architecture**: Write a complete HTML5 file `index.html` (save it in the project root directory) using pure **Vanilla JavaScript and HTML5 Canvas API** (No 3rd-party frameworks like Phaser or PixiJS). To ensure it runs seamlessly across **mobile ad-networks (Facebook, Google Ads, AppLovin)**, the HTML/CSS/JS must be strictly cross-browser compatible, responsive, and contained within a single file after packaging.
   - **Module Division**: Structure the JS logic into clear, separate modules/classes (e.g., `GameEngine`, `InputHandler`, `AssetManager`, `Entities`). Avoid monolithic functions!
   - **Game Loop**: You MUST use `requestAnimationFrame` to ensure a smooth 60 FPS update/render pipeline. Game states should be cleanly separated (e.g., `INIT`, `PLAY`, `END`).
   - **Visual Effects**: Utilize math-based rendering (sin/cos, Canvas Context 2D `arc`, `fill`, `drawImage`) for effects like `drawShockwaves()`, `drawParticles()`, and `drawScorePopups()` instead of heavy sprite animations.
   - **End Card UI**: Use **CSS3** overlaid on top of the `<canvas>` for UI. Specifically, design a "V30 GOLD END CARD" (CTA overlay) using `backdrop-filter` for blur, `linear-gradient`, and CSS animations for premium polish. The logic to trigger this End Card must be derived directly from the `GDD.md`'s End Logic.
   - **ASSET LOADING ARCHITECTURE (CRITICAL)**: Define two global dictionaries:
     `var ASSET_PATHS = { 'player': 'assets/player.webp' };`
     `var ASSET_B64 = {}; // Left empty for DEV mode`
     Write an image loader function that checks if `ASSET_B64` has keys. If so, create `new Image()` and set `.src` to the Base64 string. If empty, load from `ASSET_PATHS`. Wait for all `onload` events firing before starting the game loop. Store loaded images in a global `IMAGES` dictionary to draw in the Canvas.
3. **Internal QA / Execution**: 
   - Serve the directory using Python Server and check the result or check for logic errors via terminal if needed.
   - If you encounter javascript errors, fix them iteratively.
4. **Phase Completion**: Print completion message: *"Phase 4 Complete. The DEV build of the Playable Ad is written and functional. It currently loads loose image files for testing. Please review/test the game rules manually. If it feels right, say 'Tiếp' to package everything into a monolithic Single-File Base64 release."*
