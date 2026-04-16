# Design Specification: Compile Artist Style

## 1. Overview
This spec outlines the addition of a `compile-artist-style` skill and the `StyleLibrary` system to the AI Game Artist Workflow. It allows artists to securely and efficiently inject their personal drawing styles into the Antigravity agent process without uploading or re-analyzing old assets for every image generation command, vastly reducing token consumption and latency.

## 2. Architecture & Structure
The assets for different styles are stored systematically on the disk.
- **Root Style Path**: `Assets/GameArtist/StyleLibrary/`
- **Style Categories**: Each specific style uses its own subdirectory, e.g., `Assets/GameArtist/StyleLibrary/<style_name>/`.
- Users drop image assets (`.png`, `.jpg`) into the respective `<style_name>` folder.
- After processing by the skill, a compiled `DNA_Profile.md` file will reside in that same folder.

## 3. Workflow Integration
### 3.1. New Skill: `compile-artist-style`
- **Trigger**: When the user explicitly requests to "compile", "update", or "analyze" a specific style folder.
- **Process**:
  1. Identifies the targeted `<style_name>` folder inside `Assets/GameArtist/StyleLibrary/`.
  2. Reads all image files within that folder (acting as Few-Shot Visual Prompts).
  3. Dispatches a Vision Language Model (VLM) request (e.g., Gemini 2.5 Flash/Pro) utilizing standard System Instructions to extract 4 defining axes:
     - Line art and Base Shapes
     - Color Palette and Contrasts
     - Shading Techniques and Material Textures
     - Concept Vibe and Core Aesthetic
  4. Transforms the output into a concise textual prompt modifier.
  5. Saves the output to `Assets/GameArtist/StyleLibrary/<style_name>/DNA_Profile.md`.

### 3.2. Updates to existing Generator Skill
- When a user generates an image and explicitly references a `<style_name>`:
  1. Check if `Assets/GameArtist/StyleLibrary/<style_name>/DNA_Profile.md` exists.
  2. If it exists, inject its contents into the Art Director's prompt enhancements, prioritizing structural/color directives to match the DNA Profile.
  3. Image generation continues as standard, with the modified prompt.

## 4. Verification Plan
- A style directory will be populated with 3 sample images.
- Execution of the compile skill must result in a valid `DNA_Profile.md` being created.
- Requesting an image generation utilizing the compiled style must audibly trace the injection of the DNA in the execution logs.
