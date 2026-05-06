## Core Pattern

### 1. Canvas Batching and Separation
Changing properties (Scale, Position, Text String, Color) of 1 element on a Canvas forces EVERYTHING on that Canvas to recalculate and redraw (Rebuild).
**Solution**:
- Separate Dynamic UI (constantly changing HUD Text, Joystick, HP Slider) into its own Canvas.
- Separate Static UI (Backgrounds, fixed frames, title text) into a separate Canvas. This can be organized as sub-canvases on a main Parent.

### 2. Disable "Raycast Target" for structural elements
By default, Unity enables Raycast Target = true for new Images and Texts. This bottlenecks the CPU calculating interactive rays across the screen.
- **MANDATORY**: Uncheck Raycast Target in the inspector or when generating components via script for background images and text without Buttons.
- Periodically scan for forgotten raycast targets.

### 3. Disable "Pixel Perfect" on Dynamic Canvases
For objects like HP sliders that change size smoothly or moving text, Pixel Perfect requires constant pixel-level alignment on the GPU. Turn it off in the dynamic master Canvas.

### 4. Mandatory empty "Event Camera" for World Space Canvases
Typical error: Creating an HP bar above an Enemy. Render mode is set to World Space but the Event Camera field is left empty. Unity will automatically search for Camera.main every single frame to check events. You MUST set the Event Camera reference manually or via script.

## Common Mistakes & Rationalizations

| Excuse (Rationalization) | Reality Check |
|--------------------------|---------------|
| "Put everything in the Main Canvas so the Parent-Child codebase looks cleaner." | Clean in the Editor but kills mobile CPU when everything rebuilds just because the Timer text changed. Split into Sub-Canvases. |
| "Ignore the Raycast Target tick, as long as it doesn't cause errors." | If there are too many overlapping UI nodes, your clicks won't register (because the background image blocks the raycast). Disable them all! |
| "Add a LayoutGroup component for the Score Text to auto size." | Layout Groups are extremely heavy for auto-scaling text every time a child changes. If you can disable it or write fixed sizes, do it. |

**Any Red Flags appearing: DELETE CODE, RE-NORMALIZE YOUR RAYCASTS AND CANVASES.**
