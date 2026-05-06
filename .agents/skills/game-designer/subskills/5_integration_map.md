# Phase 5: Integration Map Generation

**Role:** You are the Lead Systems Integrator. Your job is to define exactly how the disparate assets (Art, Audio, UI) hook into the Code logic.

## Pre-requisite
Phase 4 (Validation) must be complete.

## Action
Read the validated Dev, UI, Art, and AudioVFX files.
Create a final document named `[ProjectName]_Integration_Map.md` using the `write_to_file` tool.

## Output Structure
This document must explicitly map Event Triggers to their respective outputs across departments.

### 1. [SYSTEM_HOOKS]
Create a breakdown linking Action -> Code -> VFX -> SFX -> UI.
**Example:**
- **Trigger:** `OnPlayerTakeDamage`
  - **Dev:** `HealthSystem.TakeDamage()`
  - **Art/Anim:** `Player_Anim_Hurt` plays, screen shake triggered.
  - **VFX:** `VFX_Hit_Sparks` spawned at impact point.
  - **SFX:** `SFX_Impact_Flesh_01` plays.
  - **UI:** `UI_HUD_HealthBar` flashes red and value decreases.

### 2. [SCENE_HIERARCHY]
Provide a high-level representation of what the main Unity Scene hierarchy should look like to accommodate these systems (e.g., `Managers`, `UI_Canvas`, `Environment`, `Player`).

---
**CRITICAL:** Once the Integration Map is written, you MUST automatically proceed to Phase 6: Project Hub & Technical Spec.
