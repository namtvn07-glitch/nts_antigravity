# Walkthrough: AI Playable Studio Skill

The `@[/playable-studio]` skill has been fully synthesized and injected into the `.agents/skills` repository. It is ready for practical use. 

## Architectural Accomplishments

### Modular Shell Architecture
We successfully sidestepped token collision and contextual hallucination by deploying a **Hybrid Router Architecture**. 
The Master `SKILL.md` acts purely as an intersection guard. It dynamically reads one of the 5 specialized Phase Guides stored inside `.agents/skills/playable-studio/phases/`.

### Phase Breakdowns
1. **[Init]**: Intercepts user requests, forces QA choices, and yields `task_input.json`.
2. **[Design]**: Reads JSON, synthesizes <20s GDD constraints, and demands an explicit Image Generation Checklist.
3. **[Art]**: Ingests checklist, cross-pollinates with the pre-existing `@[/nano-banana-integration]` skill, and runs our new optimizer tool to sanitize the `.webp` files.
4. **[Dev]**: Imposes the 3-Scene Architecture Rule (`BootScene`, `PlayScene`, `EndScene`), establishes the `GameConfig` sandbox variable, and outputs raw, editable `index.html`.
5. **[Package]**: Compiles all WebP binaries autonomously using the Python regex logic into a single monolithic `< 3MB` file.

### Backend Python Upgrades
- `image_optimizer.py`: Embedded into the workflow. Actively slices transparent bounding boxes from DALL-E/MidJourney generations using PIL and forces max bounds to 512px.
- `packager.py`: Written and integrated. It guarantees robust compilation into Single-File Ad networks logic without crashing the LLM limits.

## Getting Started
To invoke the studio for the first time, type:
`@[/playable-studio] I want a Flappy bird clone but with a flying spaceship`
