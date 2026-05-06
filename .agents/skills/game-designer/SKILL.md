---
name: game-designer
description: Master orchestrator skill for creating Game Design Documents (GDD) via an autonomous 6-phase pipeline including document generation and UI wireframing.
---
# Game Designer (Super Skill)

You are the Master Orchestrator for the Game Design pipeline.
To prevent context overflow, ensure high-fidelity design output, and provide full department coverage, you MUST run this skill as an autonomous 6-phase sequential pipeline.

## Execution Workflow (Autonomous Pipeline Architecture)

You must read the specific instructions for each phase from the local `subskills/` directory and execute them sequentially according to these explicit rules. 

**STRICT PIPELINE RULES:**
- You MUST run Phase 1, then STOP and wait for human approval.
- Once Phase 1 is approved, you MUST automatically chain Phase 2, Phase 3, Phase 4, Phase 5, and Phase 6 one after the other in continuous autonomous execution. Do not stop until Phase 6 is finished.

---

### 1. Phase 1: GDD Master Generation (HUMAN GATE)
- **Action:** Read instructions from `subskills/1_gdd_core.md`.
- **Execution:** Take the user's raw idea and generate the `[ProjectName]_Master_GDD.md`.
- **Important:** **STOP AND HALT EXECUTION.** You must ask the user: "Do you approve this Master GDD?" Do not proceed to Phase 2 until they say yes.

### 2. Phase 2: Department Breakout (AUTONOMOUS)
- **Action:** Read instructions from `subskills/2_department_breakout.md`.
- **Execution:** Parse the approved Master GDD and automatically write 5 separate markdown files for Art, Dev, AudioVFX, UI, and Game Data.

### 3. Phase 3: UI Wireframing & UX Flow (AUTONOMOUS)
- **Action:** Read instructions from `subskills/3_ui_wireframing.md`.
- **Execution:** Parse the UI Plan from Phase 2. Autonomously generate ASCII layout wireframes and Mermaid UX flowchart diagrams in a new markdown file.

### 4. Phase 4: Cross-Validation & Auto-Correction (AUTONOMOUS)
- **Action:** Read instructions from `subskills/4_cross_validation.md`.
- **Execution:** Self-reflect on all 5 generated breakout files. Detect any missing assets or unlinked events between departments and automatically use file editing tools to add them.

### 5. Phase 5: Integration Map (AUTONOMOUS)
- **Action:** Read instructions from `subskills/5_integration_map.md`.
- **Execution:** Synthesize the validated files into a final `[ProjectName]_Integration_Map.md` dictating the technical event hooks between Code, Art, UI, and Audio.

### 6. Phase 6: Project Hub & Technical Spec (AUTONOMOUS)
- **Action:** Read instructions from `subskills/6_project_synthesis.md`.
- **Execution:** Create `[ProjectName]_Project_Hub.md` and `technical-spec.md`. The Hub acts as the central coordinator and directory for all departments, while the Technical Spec details the architecture, systems, and logic for the dev team.
- **Completion & Integration:** Once generated, you MUST instruct the user to run `/commit` to save all GDD files to version control. Finally, trigger the `/finish` workflow to extract game design patterns and learnings into the project's knowledge base.
