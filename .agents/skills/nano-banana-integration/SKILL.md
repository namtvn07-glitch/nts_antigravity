---
name: nano-banana-integration
description: Use when the user requests generating art, concept art, images, or game assets via Nano Banana Engine, or when running the AI Game Artist workflow. It leverages the builtin generate_image tool to synthesize the image based on templates or prompts, and then explicitly moves the generated result to the workspace's Generated folder. 
---

# Nano Banana Integration Skill

## Overview
This skill implements the Image Worker logic for the AI Game Artist workflow within Antigravity. It abstracts away the external engine API by bridging requests directly to the built-in Gemini image generation capability (`generate_image` tool). 

## Execution Steps

When you decide to trigger this skill to generate an image via Nano Banana:

1. **Formulate the Prompt & Style Override Check (Text DNA & Image Conditioning):**
   - Synthesize the user's intent or the raw prompt. Add robust negative constraints to prevent style deviation (e.g. `"2D premium game environment art style, visual novel background. NO 3D render, NO photorealistic camera lens, NO granular realism"`).
   - **Crucial DNA Style Check:** Identify if a custom Style Template (e.g., `Style_A`, `SampleStyle`, `Semi-Realistic`) is requested in the user's instructions.
   - If a specific Style is detected, use `view_file` to read `<workspace>/Assets/GameArtist/StyleLibrary/<style_name>/DNA_Profile.md` and aggressively inject the DNA text directly into your final prompt.
   - Additionally, you MUST use `list_dir` on the style's folder (`<workspace>/Assets/GameArtist/StyleLibrary/<style_name>`) and pick 1 to 3 actual image paths (`.png`, `.jpg`) to serve as "Visual Anchors".
   - If no custom style is mentioned, simply enhance the prompt with general artistic modifiers.

2. **Generate Image:**
   - Call your built-in `generate_image` tool directly.
   - Set `Prompt` to the comprehensively formulated prompt, merging the artistic modifiers and textual DNA.
   - Set `ImageName` to a descriptive name (e.g., `nano_banana_output`).
   - Set `ImagePaths` to the absolute paths of the original Style template images gathered in Step 1 (max 3 images). This feeds the style perfectly to the generator. If the user provided their own Draft Image, prioritize the draft but try to retain at least one style template image.
   - **Crucial Note:** You MUST successfully execute this tool before proceeding.

3. **Move Artifact to Workspace:**
   - After `generate_image` succeeds, an artifact image file is saved in the Brain/Artifacts folder of the session.
   - Identify the path to the newly generated WebP/PNG image.
   - You MUST use the `run_command` tool (Powershell) to MOVE or COPY that artifact file into the target project workspace folder. The hardcoded target is: `<current_workspace>\Assets\GameArtist\Generated\`.
   - If the `Generated` folder does not exist, create it via `mkdir -Force`.

4. **Response and Verification:**
   - Present the image visually to the user in the chat interface using markdown syntax: `![Generated Image](<absolute_path_to_workspace_generated_file>)`.
   - Confirm that the process was successful.
