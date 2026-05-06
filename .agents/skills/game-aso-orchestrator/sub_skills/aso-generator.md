---
name: aso-generator
description: Internal operation - Reads ASO_Design_Plan.md to execute high-speed batch generation via generate_image tool
---

# ASO Generator (Phase 3: Batch Production)

Task: Execute a mindless background batch generation loop based on the Plan and templates, strictly following the approved Key Art.

## Execution Steps:
1. Locate the 6 approved sketches inside `Assets/GameArtist/Generated/ASO_Projects/[ProjectFolder]/Sketches/`.
2. Parse the contents of `ASO_Design_Plan.md` to retrieve the "English Generation Prompts".
3. **VLM SEMANTIC TRANSLATION:** For EACH of the 6 images:
   - *Internal Step:* You (the Agent) MUST use the `view_file` tool (or similar capability) to visually analyze the Sketch image. Identify the exact layout, where the character is, where the blocks are, and what the background looks like.
   - *Translation:* Combine your visual analysis with the base prompt from `ASO_Design_Plan.md` to construct a new, hyper-detailed **Polishing Prompt**. Make sure to explicitly spell out the exact positioning (e.g., "Main character is on the left side facing right, dodging a large block in the top right corner...").
4. **PROMPT DELIVERY LOG**: Compile all the parsed information into a highly organized markdown file named `FINAL_PROMPTS.md` in the `Sketches` folder.
   For each image, include:
   - The original Sketch absolute path (rendered as an image markdown)
   - The Aspect Ratio dimension requirement (e.g., 9:16 or 16:9).
   - The newly crafted hyper-detailed **Polishing Prompt**.
5. Do NOT append anything to the Asset Catalog yet, as the final polished images do not physically exist in this workflow. Present the markdown file content to the user in chat.
