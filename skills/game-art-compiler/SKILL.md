---
name: game-art-compiler
description: Use this skill specifically when the user requests to "compile", "analyze", or "update" an art style based on a directory of asset images. The skill acts as a Data Ingestion pipeline that vectorizes style images using `sentence-transformers` and outputs specialized rules (`Generation_DNA.md` and `Evaluation_Rules.json`) explicitly designed to connect with the `game-art-orchestrator`.
---

# Compile Artist Style V2

## Overview
This skill acts as a robust Data Ingestion Pipeline for your game artist workflows. It transforms a loose directory of reference images into a mathematically structured Vector schema (`style_index.json`) and specific rulesets meant to decouple Generation mechanisms from strict Evaluation heuristics.

## Execution Steps

### 1. Locate Style Directory & Legacy Cleanup
- Identify the exact style name requested.
- Target directory: `<workspace>/Assets/GameArtist/StyleLibrary/<style_name>/`
- Verify the directory exists via `list_dir`.
- **LEGACY CLEANUP**: If a file named `DNA_Profile.md` exists in this folder, you MUST delete it. It is a deprecated V1 format. Do NOT leave it behind, as it will pollute the system.

### 2. VLM Semantic Tagging (RAG-Optimized & Dynamic Context)
- Process all valid image files (`.png`, `.jpg`, `.jpeg`) inside the folder.
- Execute a VLM check (e.g. via Gemini/Flash) for EACH image.
- **Pass 1: Asset Categorization:** The VLM must classify the image into one of: `[Character]`, `[Environment]`, `[Prop/Interactable]`, or `[UI]`.
- **Pass 2: Contextual Extraction & Visual DNA:** Ask the VLM to produce a highly dense, searchable string (Embedding-Ready). It must explicitly extract:
  1. **Visual DNA:** Shape Language, Material, Complexity Level / Rarity, and Color Palette.
  2. **Game Design Principles (Based on Category):**
     - `[Character]`: Silhouette Readability, Body Alignment, Proportion Rhythms.
     - `[Environment]`: Guiding Lines, Depth Contrast (Foreground vs. Background), Scale perception.
     - `[Prop/Interactable]`: Affordance (e.g., sharp/red = danger, round/bright = collectible).
- Also generate a **RAG Hook Tag** (e.g. `"tag: UI_Icon, material: metallic, rarity: epic, visibility: high_contrast"`).
- Keep a JSON array in memory or disk containing: `[{"filename":"...", "semantic_metadata":"..."}]`

### 3. Generate Local Embeddings & Archetype Schema
- Save the JSON array from Step 2 into a temporary file or pass it to our embedding script.
- Execute the script: `python scripts/create_embeddings.py <style_directory>` (assuming you have passed the captions JSON path or adapted the script to handle inputs).
- **The Script will automatically**: Use `sentence-transformers` locally to calculate text embeddings, find the mathematical centroid, assign `is_archetype: true` to the 2 images closest to the centroid, and output the absolute `style_index.json` to the target folder.

### 4. Synthesize DNA and Rule Files
- Collect the macro conclusions from analyzing the full image batch and separate them into two strict files optimized for RAG retrieval and Semantic Search:
  
  **A. `Generation_DNA.md`**
  Write this file using `write_to_file` into the style directory. It must use strict nested Markdown headings (for text-splitters) containing the following FOUR pillars:
  - **`# I. VISUAL DNA`**: Consistency rules, Shape Language, Color Script (Rarity), and Material Polish.
  - **`# II. PRODUCT LOGIC`**: Readability (3-Second Rule), Progression Logic (Level upgrades), The Juice (Animation states), and UI Interaction rules (if applicable).
  - **`# III. TECHNICAL EXCELLENCE`**: Mesh / Topology rules, Texel Density, Modular design / Recolor limits.
  - **`# IV. GAMEPLAY AFFORDANCE & DESIGN LOGIC`**: Synthesized rules from the dynamic context extraction (e.g., silhouette rules, guiding lines, affordance logic).
  Include positive anchors and strict negative safeguards under these headings.
  
  **B. `Evaluation_Rules.json`**
  Write this file using `write_to_file` into the style directory. It must conform to this schema:
  ```json
  {
    "semantic_metadata": {
      "supported_types": ["asset", "ui"],
      "vector_keywords": ["list", "of", "dense", "visual", "keywords"]
    },
    "evaluation_criteria": {
      "visual_dna": ["Are shapes consistent?", "Does it match standard rarity palettes?"],
      "product_logic": ["Does it pass the 3-second readability rule?"],
      "technical_excellence": ["Are details readable at thumbnail size?"],
      "gameplay_affordance": ["Does the shape communicate its gameplay function?", "Is it distinct from the background (Visual Hierarchy)?"]
    },
    "forbidden_elements": ["List of anti-patterns"]
  }
  ```

### 5. Acknowledge Target & Finish
- Present the updated section explicitly to the user to confirm that the changes were securely registered.
- Remind the user that these global rules will now forcefully apply to the next `game-art-orchestrator` execution.
- **Integration:** Automatically run the `/finish` workflow to document any data ingestion gotchas or conflicts identified during the compilation process.
