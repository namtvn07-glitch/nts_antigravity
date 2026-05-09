# Game Design Document Contracts

> Domain: game-designer | game-audio-prompter | game-dev-unity  
> Purpose: Define cross-skill data contracts to prevent silent failures

## Contract: GDD → game-audio-prompter

`game-audio-prompter` reads `*_GDD.md` and `*_Audio_Assets.json`. Required sections:

| GDD Section | Maps to Audio Output |
|------------|----------------------|
| "Core Gameplay Mechanics" | SFX trigger frequency, fatigue rules |
| "Visual Art Style" | Audio texture (line-art → crisp/zero-reverb; atmospheric → spatial reverb) |
| "Overall Game Vibe" | Instrumentation, BPM range, emotional tone |

`*_Audio_Assets.json` must include per-entry: `id`, `layer` (BGM/SFX), `loop_flag`.

## Contract: GDD → game-dev-unity

`game-dev-unity` reads `*_Master_GDD.md` and `*_Integration_Map.md`. Required sections:

| Document | Required Content |
|---------|-----------------|
| Master GDD | System architecture, data structures (ScriptableObject schemas) |
| Integration Map | Event hooks between Code, Art, UI, and Audio systems |
| Technical Spec | API surface, manager responsibilities, state machine definitions |

## Rule: Data Sync — GDD ↔ Dev Enums

Game Data configurations defined in GDD **MUST exactly match** C# Enum declarations in Dev Architecture.  
Mismatches cause silent runtime failures — verify during `/review` before marking task complete.

## Rule: Prerequisite Order

Skills MUST be invoked in this order or will fail silently:

```
game-designer → game-art-compiler/game-art-configurator → game-art-orchestrator
game-designer → game-audio-prompter
game-designer → game-dev-unity
game-dev-unity → game-aso-orchestrator
game-dev-unity → game-playable-orchestrator
```

Run `python .agents/scripts/check_prerequisites.py <skill> <project_path>` to validate before invocation.
