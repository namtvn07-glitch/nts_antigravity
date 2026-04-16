---
name: compile-artist-style
description: Use this skill specifically when the user requests to "compile", "analyze", or "update" an art style based on a directory of asset images (e.g., "Hãy compile thư mục DarkFantasy"). The skill reads images inside Assets/GameArtist/StyleLibrary/<style_name>/, extracts their visual DNA guidelines using a Vision Language Model, and outputs the result into a DNA_Profile.md file.
---

# Compile Artist Style

## Overview
This skill allows artists to supply their own previous artworks as a visual reference corpus. The agent analyzes these images once and compiles a static textual DNA profile of the artist's style, heavily reducing token cost and runtime generation duration for subsequent image requests.

## Execution Steps

1. **Locate Style Directory:**
   - From the user's prompt, identify the exact style name requested.
   - The target directory MUST be `<workspace>/Assets/GameArtist/StyleLibrary/<style_name>/`.
   - Use your file listing tools (`list_dir`) to verify the directory exists and retrieve the file paths of the image files (`.png`, `.jpg`, `.jpeg`) inside it.

2. **Validate Content:**
   - If the directory contains no valid images, halt and notify the user to drop images into the folder.

3. **Analyze Style (VLM Evaluation):**
   - Use your chat/vision capabilities to analyze those images. Treat them as a batch.
   - You MUST analyze them focusing EXACTLY on these 4 parameters:
     1. **Line art & Shapes:** Line weight, silhouette styling, geometric vs organic flow.
     2. **Color Palette:** Dominant colors, lighting contrast, vibrancy, palette mood.
     3. **Shading & Texture:** Cel-shading vs soft blending, material representation, noise vs smooth.
     4. **Concept Vibe:** General aesthetic feel (e.g. grimdark sci-fi, cute pastel).
   - Formulate a cohesive text string that can be directly used as a powerful Style Modifier for Image Generators (Midjourney/Imagen/Flash Image). The output should start with standard prompt modifiers like: "In the style of... features...".

4. **Output Compiled DNA Profile:**
   - Write the entire result into the following file precisely using your `write_to_file` tool:
     `<workspace>/Assets/GameArtist/StyleLibrary/<style_name>/DNA_Profile.md`
   - You MUST ensure the file is generated cleanly. Wait for the `write_to_file` tool to succeed.

5. **Acknowledge User:**
   - Display a brief confirmation to the user that the style has been successfully compiled and is ready for use in future image generation workflows.
