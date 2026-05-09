# Art Generation Patterns

> Domain: game-art-compiler | game-art-orchestrator  
> Source: Accumulated from production sessions

## Gotcha: Magenta Background Removal — Basic BFS Fails on Holes

- AI-generated assets use **SOLID PURE MAGENTA (#FF00FF)** background
- Basic edge-seeded Flood Fill (BFS) fails on objects with enclosed holes (padlocks, portals, rings)
- **Fix**: BFS seed queue must scan the **entire image**, including interior pure-magenta pixels  
  Relaxed threshold: `R > 180, B > 180, G < 90` to account for AI generation compression noise

```python
# Correct: scan all pixels for BFS seeds
seeds = [(r, c) for r in range(h) for c in range(w)
         if img[r, c, 0] > 180 and img[r, c, 2] > 180 and img[r, c, 1] < 90]
```

## Gotcha: Crop Fails When Background Not Pre-Removed

- `getbbox()` treats magenta as solid opaque content → crop box = full image
- **Fix**: Replace magenta with transparent alpha BEFORE calling `getbbox()`:  
  `data[magenta_mask, 3] = 0`

## Rule: Mobile VRAM Optimization — Hard Size Limits

Strictly enforce before Unity import:
| Asset Type | Max Size |
|-----------|---------|
| Icons | 256px |
| Buttons | 512px |
| Panels | 1024px |

Use `Image.Resampling.LANCZOS` for high-quality downscale (PIL).

## Rule: Single Asset per Generation

Always inject prompt constraint:  
`"Generate EXACTLY ONE single asset in the center. DO NOT generate multiple variants, character sheets, split views. ONLY ONE FIGURE."`

## Rule: Generation Background Constraint

For isolated assets (Characters, Items, UI, Obstacles, VFX):  
`"SOLID PURE MAGENTA BACKGROUND (#FF00FF), no gradients, no shadows on background, no floor."`
