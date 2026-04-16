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
   - **Crucial DNA Style Check:** Identify if a custom Style Template (e.g., `Style_A`, `SampleStyle`, `Semi-Realistic`) is requested. If yes, use `view_file` to read `<workspace>/Assets/GameArtist/StyleLibrary/<style_name>/DNA_Profile.md` to extract the DNA text. Also use `list_dir` on the style folder to pick 1 to 3 actual image paths (`.png`, `.jpg`) as "Visual Anchors".
   - **Prompt Engineering Structure:** You MUST structure the prompt to prioritize the *Medium and Style* BEFORE the *Subject*. For example: `"A 2D digital illustration in premium [Style Name] game art style, depicting [SUBJECT]."`.
   - **Handling Real-World Bias:** If the subject is a real-world location (e.g., a famous landmark), the generator will bias heavily towards photos. You must counteract this by aggressively injecting terms like `"digital painting"`, `"concept art"`, `"hand-drawn brushstrokes"`.
   - **Positive Stylistic Assertions over Negative Prompts:** Instead of using negative words that can confuse the model (e.g., `"NO 3D render, NO photorealism"`), use overwhelming positive modifiers to lock the style (e.g., `"flat 2D illustration, stylized painterly texture, game engine asset"`).
   - Inject the extracted **DNA text** into this well-structured prompt.

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
