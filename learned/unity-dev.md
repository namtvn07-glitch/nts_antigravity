# Unity Dev Learnings & Patterns

## Code Architecture & Patterns

- **Prefab Variants for Data-Driven Spawning:** When spawning entities configured via ScriptableObjects (like Themes or Monsters), prefer linking `Prefab Variants` inside the ScriptableObject rather than hardcoding a generic base prefab and injecting properties. This encapsulates hierarchy-specific data (like child `Transforms` or `SpriteRenderers`) entirely within the variant, keeping Manager classes decoupled from visual setup.
- **Event-Driven Architecture & UI Decoupling:** Use `Action<T>` to decouple systems. UI should only act as a listener (e.g., `HUDCoinDisplay` listening to `EconomyManager`) and never contain game logic. Be extremely careful with initialization order: ensure subscribers (`Start` or `OnEnable`) register to events at the correct lifecycle phase so they don't miss events fired by publishers in `Awake` or `Start`.
- **Memory Leaks from Events:** Always remember to unsubscribe (`-=`) from C# events in `OnDisable` or `OnDestroy` to prevent memory leaks and ghost calls when objects are destroyed or scenes are reloaded.
- **[Gotcha] Component State Bypass:** When an object uses a wrapper/controller script (e.g. `MonsterController`) to manage inner component logic (e.g. `QuantizedAudioPlayer`) and state flags (`isSinging`), UI scripts MUST call methods on the wrapper controller, NEVER directly on the nested component. Direct component calls bypass the parent's state management, causing event-driven game logic dependent on those states to silently fail.
- **[Gotcha] ScriptableObjects and Resources.Load:** When attempting to dynamically load ScriptableObjects (or any asset) at runtime using `Resources.Load<T>("Path")`, the asset MUST be placed inside a folder named `Resources` (e.g., `Assets/Resources/Data/...`). If placed elsewhere (e.g. `Assets/Scripts/Data/`), `Resources.Load` will fail and return null, which can break fallback loading logic.

## Gameplay & Physics

- **Physics2D with Orthographic Camera:** Avoid using `Camera.ScreenPointToRay` combined with `Physics2D.Raycast` for drag-and-drop or touch detection. `ScreenPointToRay` sets the Z origin to the camera's `nearClipPlane` (e.g., -10), which can cause 2D casts to miss or behave unpredictably. Instead, use `Camera.ScreenToWorldPoint` to get the XY coordinates and check with `Physics2D.OverlapPoint`.
- **[Gotcha] Physics2D OverlapCircleNonAlloc:** `Physics2D.OverlapCircleNonAlloc` is obsolete in newer Unity versions. Use `Physics2D.OverlapCircle` with a `ContactFilter2D` instead for non-allocating, filtered overlap queries.
- **[Gotcha] Raycasts Inside Colliders:** By default, Unity's `Physics2D.queriesStartInColliders` is true. If a raycast source (e.g., Laser) is positioned inside a solid collider (like Sand), it will hit the collider immediately. To shoot through triggers (like Portals/Acid) while still hitting solid objects, use `ContactFilter2D` with `useTriggers = false` instead of just a LayerMask.
- **[Pattern] Puzzle Mechanics - Hazard Neutralization:** When dynamic objects (e.g., Boulders) interact with hazards (e.g., AcidPool triggers), don't just disable the hazard's trigger. Set the dynamic object's `bodyType` to `RigidbodyType2D.Static` so it stops falling through the trigger and transforms into a solid bridge for the player.

## Audio

- **Audio Quantization on Mobile:** Prefer using Silence Padding (trimming original audio and adding silence to match the Grid) over Time-Stretch to prevent pitch distortion and maintain Zero Latency.
- **[Gotcha] Audio Muting & Pausing:** Avoid using `Time.timeScale = 0f` or `AudioListener.pause = true` just to mute background music or pause a game loop, as it stops UI animations, coroutines, and newly generated audio playback. Use event-driven direct muting (`audioSource.mute = true`) and `WaitForSecondsRealtime` (or `Time.unscaledDeltaTime`) instead.

## Editor & Tooling

- **Automating Repetitive Tasks:** Always write small editor scripts for repetitive tasks like converting `Text` to `TextMeshPro` across multiple prefabs, or using a `SpriteColliderGenerator` to automatically generate physics hitboxes based on sprite graphics at runtime instead of manually adjusting BoxColliders in the editor. This saves time and prevents errors when art assets change.
- **[Gotcha] UI Button Listeners in Editor Scripts:** Do not use `button.onClick.AddListener(() => ...)` dynamically within Editor setup scripts. Lambda listeners do not serialize into the scene file, so the buttons will be broken at runtime. Instead, assign the `Button` references to `[SerializeField]` properties using `SerializedObject` and call `AddListener` in `Awake/Start()` of a MonoBehaviour.
- **[Gotcha] Editor Scripting Asset Paths:** When using `AssetDatabase.LoadAssetAtPath<T>` in Editor scripts, ensure the string path matches exactly (e.g., `Assets/Scripts/Data/...`). If the path is wrong, it returns null without throwing an exception, which can silently break runtime components that depend on those injected ScriptableObjects.

