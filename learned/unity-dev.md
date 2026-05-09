# Unity Dev Learnings & Patterns

## Code Architecture & Patterns

- **[Pattern] Prefab Variants for Data-Driven Spawning:** When spawning entities configured via ScriptableObjects (like Themes or Monsters), prefer linking `Prefab Variants` inside the ScriptableObject rather than hardcoding a generic base prefab and injecting properties. This encapsulates hierarchy-specific data (like child `Transforms` or `SpriteRenderers`) entirely within the variant, keeping Manager classes decoupled from visual setup.
- **[Gotcha] Component State Bypass:** When an object uses a wrapper/controller script (e.g. `MonsterController`) to manage inner component logic (e.g. `QuantizedAudioPlayer`) and state flags (`isSinging`), UI scripts MUST call methods on the wrapper controller, NEVER directly on the nested component. Direct component calls bypass the parent's state management, causing event-driven game logic dependent on those states to silently fail.
- **[Gotcha] ScriptableObjects and Resources.Load:** When attempting to dynamically load ScriptableObjects (or any asset) at runtime using `Resources.Load<T>("Path")`, the asset MUST be placed inside a folder named `Resources` (e.g., `Assets/Resources/Data/...`). If placed elsewhere (e.g. `Assets/Scripts/Data/`), `Resources.Load` will fail and return null, which can break fallback loading logic.

## Gameplay & Physics

- **[Gotcha] Physics2D with Orthographic Camera:** Avoid using `Camera.ScreenPointToRay` combined with `Physics2D.Raycast` for touch detection. `ScreenPointToRay` sets the Z origin to the camera's `nearClipPlane` (e.g., -10), causing 2D casts to miss. Use `Camera.ScreenToWorldPoint` + `Physics2D.OverlapPoint` instead. Note: `OverlapCircleNonAlloc` is obsolete — use `Physics2D.OverlapCircle` with `ContactFilter2D`.
- **[Gotcha] Raycasts Inside Colliders:** By default, Unity's `Physics2D.queriesStartInColliders` is true. If a raycast source (e.g., Laser) is positioned inside a solid collider (like Sand), it will hit the collider immediately. To shoot through triggers (like Portals/Acid) while still hitting solid objects, use `ContactFilter2D` with `useTriggers = false` instead of just a LayerMask.
- **[Pattern] Puzzle Mechanics - Hazard Neutralization:** When dynamic objects (e.g., Boulders) interact with hazards (e.g., AcidPool triggers), don't just disable the hazard's trigger. Set the dynamic object's `bodyType` to `RigidbodyType2D.Static` so it stops falling through the trigger and transforms into a solid bridge for the player.
- **[Gotcha] Missing Tags on Prefabs:** If gameplay items (hazards, powerups) silently stop interacting without throwing errors, verify their Prefab Tag assignments. Reverting or updating prefabs can wipe custom tags (e.g., `Player`, `Boulder`), breaking all `CompareTag()` logic.

## Audio

- **[Gotcha] Audio Muting & Pausing:** Avoid using `Time.timeScale = 0f` or `AudioListener.pause = true` just to mute background music or pause a game loop, as it stops UI animations, coroutines, and newly generated audio playback. Use event-driven direct muting (`audioSource.mute = true`) and `WaitForSecondsRealtime` (or `Time.unscaledDeltaTime`) instead.

## Editor & Tooling

- **[Gotcha] UI Button Listeners in Editor Scripts:** Do not use `button.onClick.AddListener(() => ...)` dynamically within Editor setup scripts. Lambda listeners do not serialize into the scene file, so the buttons will be broken at runtime. Instead, assign the `Button` references to `[SerializeField]` properties using `SerializedObject` and call `AddListener` in `Awake/Start()` of a MonoBehaviour.
- **[Gotcha] SerializedObjectNotCreatableException:** If thrown by `ButtonEditor.OnEnable()`, indicates the `Navigation` or `AnimationTriggers` array inside a UI `Selectable` prefab is corrupted. Fix: reimport the affected prefab.
