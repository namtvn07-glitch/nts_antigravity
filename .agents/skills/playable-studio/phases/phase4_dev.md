# Phase 4: DEV (Playable Engineer)

**Your Role**: Playable Engineer

## Instructions
1. **Context Initialization**: STRICTLY use the `list_dir` tool on the project's `assets/` directory to discover the *actual* physical names of the generated `.webp` files. Read `GDD.md` to refresh memory of the mechanic logic.
2. **Implement Skeleton Architecture**: Write a complete HTML5 file `index.html` (save it in the project root directory) integrating the Phaser 3 CDN.
   - You MUST force the code into 3 explicit scenes: `BootScene` (where loaders are), `PlayScene` (core mechanics and physics), and `EndScene` (CTA rendering).
   - Expose a `var GameConfig = { speed: 200, gravity: 800, cta_url: 'https://store...' };` configuration block at the very top of `<script>`.
   - **ASSET LOADING ARCHITECTURE (CRITICAL)**: You must define two dictionaries globally above the classes:
     `var ASSET_PATHS = { 'player': 'assets/player.webp' };`
     `var ASSET_B64 = {}; // Left empty for DEV mode`
     In `BootScene.preload()`, check if `ASSET_B64` has keys. If so, loop and use `this.textures.addBase64(key, ASSET_B64[key])` and add a `this.textures.on('addtexture', ...)` listener to start the PlayScene when all are loaded (since base64 decoding is async). If `ASSET_B64` is empty, just loop and `this.load.image(key, ASSET_PATHS[key])` normally, and start the scene in `BootScene.create()`. This universally prevents the "Local data URIs are not supported" error in Phaser.
3. **Internal QA / Execution**: 
   - Serve the directory using Python Server and check the result or check for logic errors via terminal if needed.
   - If you encounter javascript errors, fix them iteratively.
4. **Phase Completion**: Print completion message: *"Phase 4 Complete. The DEV build of the Playable Ad is written and functional. It currently loads loose image files for testing. Please review/test the game rules manually. If it feels right, say 'Tiếp' to package everything into a monolithic Single-File Base64 release."*
