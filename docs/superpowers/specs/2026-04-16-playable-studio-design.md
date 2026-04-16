# AI Playable Studio: System Design Specification (V2.1 - Optimized)

## Overview
This specification details the architecture and workflow for an Antigravity custom skill `@[/playable-studio]`. The skill automates the production of HTML5 Playable Ads (using Phaser 3) through a multi-agent orchestration pipeline.

## Architectural Constraints
- **Execution Environment**: Google Antigravity Local Workspace
- **Output Constraints**: 
  - A single `index.html` file containing HTML, logic, and base64 encoded graphical assets.
  - Total output size MUST be strictly `< 3MB`.
- **Modularity**: Implemented as a Phase-Gated execution workflow (State Machine) to allow user validation and reduce hallucination risk.

## Workspace Strategy
Each playable game project forces isolating outputs into a dedicated project directory:
`Assets/PlayableGameStudio/Projects/<Project_Name>/`

State Tracking:
- The system will write a `pipeline_state.md` (or `.studio_state` tracker file) in the project directory. 
- Tracking ensures seamless transitions between distinct conversation phases following a user's "Continue" approval. (Agent MUST actively read the raw files and directories state, not relying on context memory).

## Phase-Gated Execution Workflow

### Phase 1: INIT (Market Gap & Specs Setup)
- **Role**: Market Analyst
- **Action**: The skill interviews the user (maximum 3 concise, multiple-choice or short-form questions) focusing on: genre, core mechanic, and CTA placement. 
- **Output**: Generates `task_input.json`.
- **Gate**: Displays JSON summary. Waits for User to provide approval to proceed to Phase 2.

### Phase 2: DESIGN (Game Design & Logistics)
- **Role**: Lead Game Designer & UA Strategist
- **Action**: Reads `task_input.json` and drafts a concise Game Design Document.
  - Imposes tight constraints: Core gameplay loop `< 20s`, straightforward mechanics (tap/swipe), simplified physics mapping (speed, gravity).
  - Explicitly structures an **Asset Generation Checklist** (e.g., `player.png`, `obstacle.png`, `cta.png`).
- **Output**: Generates `GDD.md`.
- **Gate**: Displays the Asset Checklist. Haults and waits for User to validate the list to avoid asset waste in Phase 3.

### Phase 3: ART (AI Art Production)
- **Role**: AI Art Director
- **Action**: 
  - Cross-references the approved Asset Checklist from `GDD.md`.
  - Autonomously delegates art generation payload to `@[/nano-banana-integration]` with a "2D Game Ads" style.
  - **Asset Post-Processing**: Activates a Python script (Pillow) to Auto-Crop transparent pixels from the visual assets and resize them into predictable boundaries (e.g., 128x128). Converts output into `.webp` formatting to comply with size constraints.
- **Output**: Populates `Assets/PlayableGameStudio/Projects/<Project_Name>/assets/` with optimized `.webp` files.
- **Gate**: Presents final visual assets. Waits for User to approve the image styles before initiating the code phase.

### Phase 4: DEV & QA (Playable Engineer)
- **Role**: Playable Engineer
- **Action**:
  - Employs a strict **Skeleton Architecture** forcing Phaser 3 into 3 distinct scenes: `BootScene`, `PlayScene`, and `EndScene` to prevent hallucination of the flow.
  - Generates HTML logic loading physical files from the `assets/` directory (DO NOT inject Base64 yet to ensure IDE performance).
  - Forces a customizable `GameConfig` object at the top of the file.
  - Automatically starts a local Python HTTP server, and uses Antigravity Browser Tool/Subagent to load the newly written Playable Ad. Verifies visual loading and actively scans for critical Console JavaScript violations.
- **Output**: The finalized `index.html` (separated assets mode).
- **Gate**: User reviews the browser Playable Ad experience.

### Phase 5: PACKAGING (Producer Release)
- **Role**: Executive Producer
- **Action**: 
  - Once DEV is approved, runs a `packager.py` script.
  - The script parses `index.html`, converts all assigned `.webp` dependencies to Base64 strings, and overwrites the load functions to create a monolithic Single-File payload.
- **Output**: Final `build/index.html` (< 3MB) ready for Ad Network distribution.
