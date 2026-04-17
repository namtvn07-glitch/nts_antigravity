# Phase 2: DESIGN (Game Design & Logistics)

**Your Role**: Lead Game Designer & UA Strategist

## Instructions
1. **Read Task Input**: Use `view_file` (or similar tools) to read `task_input.json` in the current project directory.
2. **Draft GDD**: Analyze the `task_input.json`. **If `"reference_source"` exists**, use your tools (like browser subagent) to deeply analyze the source media again to observe UI layouts, pacing, screen transitions, and detailed logic. Then, write a Game Design Document (`GDD.md`) and save it to the project directory.
   - **Design Constraints (CRITICAL)**: Convert the reference/idea into a Playable Ad format: Enforce a Core Gameplay Loop of `< 20s`. Mechanics must be solely single-tap or swipe logic. Limit heavy physics simulations (rely on simple gravity, velocity, or tweens). Define the End Screen (CTA) clearly.
   - **Asset Generation Checklist**: Explicitly list all required 2D image assets expected to be built (e.g., `background.png`, `player.png`, `obstacle.png`, `cta_button.png`). ONLY propose distinct standard still images. Avoid complex sprite sheet animations to simplify and standardize the AI Generation process.
3. **Phase Completion**: Present the Asset Checklist to the user directly in the chat. Post the message: *"Phase 2 Complete. Here is the Asset Generation Checklist. Please review the listed characters and items. If you approve, say 'Tiếp tục' to invoke the Art Workflow."*
