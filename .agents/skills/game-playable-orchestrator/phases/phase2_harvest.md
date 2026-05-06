# Phase 2: ASSET HARVESTING & SFX (Resource Extraction)

**Your Role**: Asset Optimization Manager

## Instructions
1. **Read Playable Docs**: Use `view_file` to read the local `Playable_GDD.md` and parse the **Required Asset List**.
2. **Budget Verification**: Manually count the assets. If there are >1 BGM, >3 SFX, or >15 Sprites, HALT execution and ask the user to trim `Playable_GDD.md`.
3. **Resource Copying & Missing Asset Detection**: Use `list_dir` to check the source game directory. 
   - If the required asset **exists** in the source: Copy it explicitly to the local `assets/` folder.
   - If the required asset is **missing** or a custom ad-element (e.g., CTA button, pointer hand): Do NOT crash. Construct an order file named `Missing_Ad_Assets.json` inside the project folder detailing the missing asset names, semantic traits, and dimensions.
   - **Generation Delegation**: If you created `Missing_Ad_Assets.json`, **HALT execution** here BEFORE optimization. Tell the user: *"Phát hiện thiếu tài nguyên quảng cáo chuyên biệt! Đã xuất hoá đơn `Missing_Ad_Assets.json`. Bạn có thể dùng `@[/game-art-orchestrator]` để tự động sinh file này. Báo 'Tiếp tục' khi ảnh đã được bổ sung nhé!"*
4. **Media Optimization Pipeline**: (Run ONLY after all assets are present)
   - Run the Python Image Optimizer to compress images to `.webp`.
   - Run the Python Audio Optimizer to compress SFX to `.ogg` or `.mp3`.
5. **Phase Completion**: State: *"Phase 2 Complete. All required textures and SFX are present and optimized within budget limit. Say 'Tiếp tục' for DEV Engineering."*
