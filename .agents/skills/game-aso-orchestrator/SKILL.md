---
name: game-aso-orchestrator
description: Automates the planning and production of ASO (App Store Optimization) assets. Use the command @/game-aso-orchestrator [store_link] to trigger.
---

# Game ASO Orchestrator

You are the Master Orchestrator. You interact exclusively with the user and manage backend operations.

## Execution Workflow (3 Phases Workflow):

**Phase 1: Analyst & RAG Planning**
1. Invoke the subskill at `sub_skills/aso-analyst.md` to automatically extract the Vibe, trigger the RAG script `retrieve_aso_style.py`, and consult the User.
2. The process pauses for the User to select a Style.
3. Synthesize the context and export the `ASO_Design_Plan.md` file. Ask the User for final approval of the Plan.

**Phase 2: Sketching & Composition Approval**
1. **CRITICAL:** Start by establishing the Key Art (Feature Graphic) using the internal `generate_image` tool. Use Bounding Box layout and Character Anchor `ImagePaths` to establish the frame and entity correctly. Prompt constraint: *"Focus ONLY on Step 1: Sketching & Silhouettes. Low detail."*
2. Present the sketch: `![Key Art Sketch](absolute/path/with/forward/slashes.png)` (CRITICAL: You MUST replace backslashes with forward slashes `/` for the path to render correctly) and EXPLICITLY ASK THE USER: *"Do you approve this Sketch?"*
3. If approved, SEQUENTIALLY generate the App Store Icon sketch (1:1 format, no bounding box padded logic needed for Icon) AND the remaining 5 Screenshot sketches (using `generate_image` tool + Bounding Boxes + Character Anchor).
4. Present ALL 6 remaining sketches (Icon + 5 Screenshots) together in the chat (or a carousel, ensuring ALL paths use forward slashes `/`) and EXPLICITLY ASK: *"Do you approve all of these sketches?"*
5. Once fully approved, use `run_command` tool to create `Assets/GameArtist/Generated/ASO_Projects/[ProjectFolder]/Sketches/` and move ALL 7 sketches (1 Key Art + 1 Icon + 5 Screenshots) there.

**Phase 3: Semantic Translation & Final Generation**
1. Trigger `sub_skills/aso-generator.md`. This sub-skill will take the approved Sketches, interpret their structures semantically via VLM, and merge them into hyper-detailed Prompts.
2. Output a definitive Markdown file combining the Sketches and corresponding Polishing Prompts, and ask the User to explicitly approve them ("chốt").
3. Once approved, use the `generate_image` tool to automatically render ALL final assets. Because the `generate_image` tool operates on a 1:1 aspect ratio, you MUST instruct the prompt to scale the actual content down to fit within the 1:1 frame while preserving its original aspect ratio, leaving the remaining empty areas (excess/padding) as transparent.
4. **Integration & Completion:** Once all ASO assets are successfully rendered and moved to their final directory, trigger the `/finish` workflow to document effective ASO prompt structures. Remind the user to run `/commit` to save the assets to version control.
