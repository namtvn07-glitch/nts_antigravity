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

Global patterns that apply across the entire project (regardless of domain) should be placed here.

For specific domains (like Unity Development, Game Design, 2D Art, UI, etc.), the rules and gotchas are stored in the `.agents/learned/` folder:
- **`unity-dev.md`**: Unity specific gotchas, physics, audio, memory management.
- **`game-designer.md`**: Game design rules.
- **`art-2d.md`**: Art generation, formatting, sizing.

*Note: The `/finish` workflow will automatically extract new patterns and place them in the correct file according to this structure.*
