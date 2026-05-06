---
trigger: always_on
---

# Project Specific Rules: Unity MCP

This document contains rules, code conventions, and lessons learned specifically for this Unity Game project.
The AI system (Antigravity) will prioritize reading this file in combination with the Global GEMINI.md before writing or reviewing code.

## 1. Project Architecture

- **Data Layer:** (Example: Prefer using ScriptableObjects for data storage instead of static JSON).
- **Logic Layer:** (Example: Manage logic via Managers/Singletons or Dependency Injection).
- **UI Layer:** (Example: Do not write game logic in UI scripts; UI should only listen to events to update visuals).

## 2. Code Conventions
- **Inspector Variables:** Use `[SerializeField] private` instead of `public`.
- **Naming:** 
  - Classes and Methods use `PascalCase`.
  - Private variables use `camelCase` or `_camelCase`.
- **Performance:** 
  - Cache `GetComponent` in `Awake()` or `Start()`. Never use it inside `Update()`.
  - Limit `Instantiate/Destroy` in the gameplay loop; use Object Pooling.

## 3. Learned Patterns & Gotchas
> *This section will be automatically updated by the `/finish` command when new lessons are discovered during work.*

- (Example) Always unsubscribe from events (`-=`) in `OnDisable` or `OnDestroy` to prevent memory leaks.
- **[Gotcha] Audio Muting & Pausing:** Avoid using `Time.timeScale = 0f` or `AudioListener.pause = true` just to mute background music or pause a game loop, as it stops UI animations, coroutines, and newly generated audio playback. Use event-driven direct muting (`audioSource.mute = true`) and `WaitForSecondsRealtime` (or `Time.unscaledDeltaTime`) instead.
- **[Gotcha] UI Button Listeners in Editor Scripts:** Do not use `button.onClick.AddListener(() => ...)` dynamically within Editor setup scripts. Lambda listeners do not serialize into the scene file, so the buttons will be broken at runtime. Instead, assign the `Button` references to `[SerializeField]` properties using `SerializedObject` and call `AddListener` in `Awake/Start()` of a MonoBehaviour.
- **[Gotcha] Component State Bypass:** When an object uses a wrapper/controller script (e.g. `MonsterController`) to manage inner component logic (e.g. `QuantizedAudioPlayer`) and state flags (`isSinging`), UI scripts MUST call methods on the wrapper controller, NEVER directly on the nested component. Direct component calls bypass the parent's state management, causing event-driven game logic dependent on those states to silently fail.
