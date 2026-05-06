# Phase 1: INGEST (Playable GDD Synthesis)

**Your Role**: Playable Integration Architect

## Instructions
1. **Input Detection**: Extract the `<Source_Game_Directory>` provided by the user.
2. **Context Ingestion**: Use `view_file` to read the primary `GDD.md` and logic files within the Source Game Directory. 
3. **Asset Budget Sentinel (CRITICAL)**: Use `list_dir` on the source game's image and audio directories to verify exact filenames. **You are strictly limited to an Asset Budget of: 1 BGM, 3 SFX, and 15 Sprites MAXIMUM.**
4. **Playable Rule Distillation**: Mentally distill the complex source game mechanics down to a 15-20 second "Hook". 
   - **Game Genre**: You MUST clearly classify the genre (e.g. Puzzle like Numchain, Merge like Drop merge, Simulation like Gun Sim, etc.).
   - **Logic Gameplay**: Define the Gameloop explicitly, define the exact Gameplay limits/bounds, and force clear Win/Lose End Conditions.
   - **UI & Layout Analysis**: You MUST map out the screen explicitly. Define the Layout (Header / Body / Footer). Detail the UI Elements (Button with exact text, Input with exact placeholders, Main text content). Identify the overall Style, and extract the implicit "Hidden visual flow" from the references. Ensure your output provides a complete UI Flow.
   - Force an explicit Call-To-Action (CTA) overlay at the end.
   - If the hook requires more assets than the Budget allows, you MUST simplify the hook.
5. **Output Synthesis**: Write `Playable_GDD.md` directly into the current active Playable Project folder.
   - **Game Genre**: State the classified genre.
   - **Playable Mechanics**: Document the Gameloop, Gameplay Limit, and End Conditions.
   - **UI Flow & Structure**: Output a rigorous UI Flow covering Header/Body/Footer segmentation, explicit Button/Input text payloads, Style, and hidden flow paths.
   - **Required Asset List**: List ONLY the exact filenames needed (respecting the Budget).
6. **Phase Completion**: State: *"Phase 1 Complete. `Playable_GDD.md` synthesized under budget constraints. If approved, say 'Tiếp tục'."*
