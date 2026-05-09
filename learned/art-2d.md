# 2D Art & Asset Generation Learnings

## Gotchas
- **[UI Asset Generation] Background Removal**: When generating UI assets using the `game-art-orchestrator`, the orchestrator enforces a SOLID PURE MAGENTA BACKGROUND (#FF00FF). Therefore, when writing Python scripts to auto-crop and resize the assets for Unity, you MUST write code to explicitly replace the magenta color (e.g., RGB > 200, < 50, > 200) with a transparent alpha channel `data[magenta_mask, 3] = 0` BEFORE attempting to use `getbbox()`. Otherwise, the crop will fail as the background is treated as solid opaque pixels.
- **[UI Asset Generation] Enclosed Background Removal**: Using a basic edge-based Flood Fill (BFS) to remove magenta backgrounds fails on objects with holes (e.g., Padlocks, Portals). To fix this, the BFS seed queue must scan the entire image and include all "pure" magenta pixels (using a relaxed threshold like `R > 180, B > 180, G < 90` to account for AI generation noise). This allows the algorithm to clear enclosed holes while still maintaining edge anti-aliasing.

## Best Practices
- **[UI Optimization] Mobile VRAM Limits**: Raw AI-generated UI assets are typically 1024x1024. Before importing into Unity, strictly resize them to optimize memory: Icons to max `256px`, Buttons to max `512px`, and Panels to max `1024px`. Use high-quality resampling (e.g., `Image.Resampling.LANCZOS` in PIL).
