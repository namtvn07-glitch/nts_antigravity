# Unity Dev Learnings & Patterns

## Code Architecture & Patterns

- **Prefab Variants for Data-Driven Spawning:** When spawning entities configured via ScriptableObjects (like Themes or Monsters), prefer linking `Prefab Variants` inside the ScriptableObject rather than hardcoding a generic base prefab and injecting properties. This encapsulates hierarchy-specific data (like child `Transforms` or `SpriteRenderers`) entirely within the variant, keeping Manager classes decoupled from visual setup.
- **Event-Driven Architecture & UI Decoupling:** Use `Action<T>` to decouple systems. UI should only act as a listener (e.g., `HUDCoinDisplay` listening to `EconomyManager`) and never contain game logic. Be extremely careful with initialization order: ensure subscribers (`Start` or `OnEnable`) register to events at the correct lifecycle phase so they don't miss events fired by publishers in `Awake` or `Start`.
- **Memory Leaks from Events:** Always remember to unsubscribe (`-=`) from C# events in `OnDisable` or `OnDestroy` to prevent memory leaks and ghost calls when objects are destroyed or scenes are reloaded.

## Gameplay & Physics

- **Physics2D with Orthographic Camera:** Avoid using `Camera.ScreenPointToRay` combined with `Physics2D.Raycast` for drag-and-drop or touch detection. `ScreenPointToRay` sets the Z origin to the camera's `nearClipPlane` (e.g., -10), which can cause 2D casts to miss or behave unpredictably. Instead, use `Camera.ScreenToWorldPoint` to get the XY coordinates and check with `Physics2D.OverlapPoint`.

## Audio

- **Audio Quantization on Mobile:** Prefer using Silence Padding (trimming original audio and adding silence to match the Grid) over Time-Stretch to prevent pitch distortion and maintain Zero Latency.

## Editor & Tooling

- **Automating Repetitive Tasks:** Always write small editor scripts for repetitive tasks like converting `Text` to `TextMeshPro` across multiple prefabs, or using a `SpriteColliderGenerator` to automatically generate physics hitboxes based on sprite graphics at runtime instead of manually adjusting BoxColliders in the editor. This saves time and prevents errors when art assets change.

