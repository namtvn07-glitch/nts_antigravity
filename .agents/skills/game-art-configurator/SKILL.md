---
name: game-art-configurator
description: A knowledge management skill to modify, query, or add rules to the Global_DNA.md and automatically vectorize them for the RAG system.
---

# Update Design System (RAG)

## Overview
This skill acts as an Active Knowledge Management Agent for the Master Configuration file governing global aesthetic physics (`Assets/GameArtist/Global_DNA.md`). It ensures modifications are processed by a vectorization script to keep the global RAG database up to date, while actively checking for structural conflict against specific local styles.

## Execution Steps

### 1. Parse Request & Locate File
- Navigate to `<workspace>/Assets/GameArtist/Global_DNA.md`.
- Read the file content.
- Identify the explicit modification requested (e.g. "Add a new rule that UI must occupy 80% of canvas").

### 2. Update Markdown
- Determine the correct Heading (e.g. `# III. DIMENSION RULES`) for the new rule based on its semantic meaning.
- If no heading fits, create a new top-level heading.
- Inject the new rule as a bullet point or text under that heading.
- Write the updated file back to `Assets/GameArtist/Global_DNA.md` using `write_to_file`.

### 3. Data Embedding (Vectorization)
- Immediately after updating the Markdown, trigger the embedding script to rebuild the Global RAG database:
  - Run `python scripts/create_global_embeddings.py`
- Wait for the script to finish. It will automatically overwrite `Assets/GameArtist/global_index.json`.

### 4. Conflict Resolution (Semantic RAG Search)
- Review the terminal output from the script run. The script automatically calculates Cosine Similarity between your new global rules and all existing local style indices.
- If the script outputs any `[WARNING]` logs about high similarity or conflicts (e.g., Global rule contradicts Style rules), you MUST print out this warning explicitly in your response to the user.

### 5. Acknowledge & Confirm
- Present the updated section explicitly to the user.
- Ask them if they want to revise the rule based on the Conflict warnings (if any).
- If no conflicts, remind the user that the global RAG context is now updated for the `game-art-orchestrator`.
- **Integration:** Run the `/finish` workflow immediately after to record any structural rule conflicts found into the `docs/learned/` knowledge base.
