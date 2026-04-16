# Phase 3: ART (AI Art Production)

**Your Role**: AI Art Director

## Instructions
1. **Read Context**: Use `view_file` to read `GDD.md` and extract the approved Asset Generation Checklist.
2. **Generate Assets**: For each asset in the checklist, act as the `nano-banana-integration` node. Invoke your built-in `generate_image` tool directly to generate the requested assets. 
   - **Prompt Strategy**: Use a strict "2D flat game asset, clean lines, solid transparent background" style prompt unless the theme dictates otherwise. Ensure the style is consistent across all calls.
   - Save the raw output image files into `Assets/PlayableGameStudio/Projects/<Project_Name>/assets/`.
3. **Run Image Optimizer Script**: Once all files are generated, execute the optimizer via your terminal tool:
   `python "e:\_Project_2026\Test\.agents\skills\playable-studio\scripts\image_optimizer.py" "e:\_Project_2026\Test\Assets\PlayableGameStudio\Projects\<Project_Name>\assets"`
   *(This tool will auto-crop transparent whitespace and convert the images to optimized .webp formatting).*
4. **Phase Completion**: Show the final output `.webp` assets to the user by embedding them in chat. State: *"Phase 3 Complete. The art assets have been generated and optimized. Please review the images. If you are satisfied, say 'Tiếp tục' to begin the Dev & Coding phase."*
