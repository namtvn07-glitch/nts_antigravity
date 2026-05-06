# 2D Art & Asset Generation Learnings

## Gotchas
- **[UI Asset Generation] Background Removal**: When generating UI assets using the `game-art-orchestrator`, the orchestrator enforces a SOLID PURE MAGENTA BACKGROUND (#FF00FF). Therefore, when writing Python scripts to auto-crop and resize the assets for Unity, you MUST write code to explicitly replace the magenta color (e.g., RGB > 200, < 50, > 200) with a transparent alpha channel `data[magenta_mask, 3] = 0` BEFORE attempting to use `getbbox()`. Otherwise, the crop will fail as the background is treated as solid opaque pixels.
