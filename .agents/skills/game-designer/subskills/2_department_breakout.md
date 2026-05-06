# Phase 2: Department Breakout Generation

**Role:** You are the Lead Producer breaking down the verified GDD into actionable departmental specifications.

## Pre-requisite
You MUST have obtained human approval for Phase 1 (`[ProjectName]_Master_GDD.md`) before running this phase.

## Action
Read the `[ProjectName]_Master_GDD.md`. Autonomously extract and formulate requirements specifically for Art, Dev, Audio/VFX, UI, and Game Data.
Use the `write_to_file` tool to create five distinct Markdown files in the project folder.

### File 1: `[ProjectName]_Art_Requirements.md`
- A highly detailed, structured checklist/table listing EVERY Environment asset, Prop, Character, UI Element, and VFX Texture.
- Specific animation states required per entity with clear transition logic.
- Visual style guidelines extracted from the concept.

### File 2: `[ProjectName]_Dev_Requirements.md`
- **CRITICAL RULE:** Do NOT design architecture, singletons, or specific script names. Leave "How to build it" to the developers.
- Define "Logic Requirements": Game Rules, State Flow, Input bindings, and Output expectations.
- Define formulas, collision rules, and physics requirements.

### File 3: `[ProjectName]_AudioVFX_List.md`
- Complete SFX list (e.g. `SFX_Jump`, `SFX_Hit`).
- Complete BGM list (e.g. `BGM_MainTheme`).
- Associated VFX (e.g. `VFX_Explosion_Particle`).

### File 4: `[ProjectName]_UI_Plan.md`
- Detailed hierarchical list of all UI Screens and Popups.
- For each screen, list buttons, text fields, and essential UX flow.
- Ensure screen names are uniquely identifiable (e.g. `UI_Screen_MainMenu`).

### File 5: `[ProjectName]_Game_Data.md`
- Provide spreadsheet-like data lists.
- List of entities with their attributes (e.g., Shop prices, unlock costs, Monster stats).
- Currency drop rates, progression requirements, and generation formulas.

---
**CRITICAL:** Do not stop or wait for approval after creating these 5 files. Immediately proceed to Phase 3.
