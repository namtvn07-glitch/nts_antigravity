# Goal: Implement `@[/playable-studio]` Skill & Architecture

Implement an Antigravity custom skill acting as a Master Orchestrator, supported by Modular Sub-Guides for each phase. This ensures smooth UX while maintaining strict modularity boundaries. It automates 5 phases integrating JSON, GDD, Python Toolkits, and Phaser 3.

## User Review Required

> [!NOTE]
> Based on your feedback, we've updated the architecture to the **Hybrid Modularity** approach. The single `@[/playable-studio]` skill acts as a shell. It will dynamically load logic from `/phases/` directories.

## Proposed Changes

### Core Skill Implementation
The `playable-studio` master shell.

#### [NEW] .agents/skills/playable-studio/SKILL.md
- **Instruction set**: The root logic. Only knows how to read the Tracker and invoke the appropriate local file inside the `phases/` folder. Acts as a "Router".

### Phase Modularity Logic (The Sub-Guides)
Storing exact prompts to isolate context and prevent AI hallucination.

#### [NEW] .agents/skills/playable-studio/phases/phase1_init.md
#### [NEW] .agents/skills/playable-studio/phases/phase2_design.md
#### [NEW] .agents/skills/playable-studio/phases/phase3_art.md
#### [NEW] .agents/skills/playable-studio/phases/phase4_dev.md
#### [NEW] .agents/skills/playable-studio/phases/phase5_package.md

### Helper Tooling (Python Scripts)
Robust file pipeline processing tools.

#### [NEW] .agents/skills/playable-studio/scripts/image_optimizer.py
- Auto-crops transparent regions using `PIL`.
- Resizes to power-of-two bounds if needed.
- Exports to highly-compressed `.webp`.

#### [NEW] .agents/skills/playable-studio/scripts/packager.py
- Parses the finished `index.html` from DEV mode.
- Fully auto-generates Base64 encoding for `.webp` files.
- Creates `build/index.html` as the < 3MB Release Ad.

## Verification Plan

### Automated Tests
- Test Master Router logic.
- Execute `packager.py` against a sample file to check regex.

### Manual Verification
- Execute `@[/playable-studio] Demo Game`.
- Validate it correctly loads Phase 1 logic and waits.
