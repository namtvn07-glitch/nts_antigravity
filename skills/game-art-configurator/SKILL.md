---
name: game-art-configurator
description: A knowledge management skill to modify, query, or add rules to the Global_DNA.md and automatically vectorize them for the Global GraphRAG system.
---

# Update Design System (Global GraphRAG)

## MANDATORY PRE-CHECK (Global GraphRAG & Learned)
**BEFORE executing any steps below:**
1. **Read learned files**: Use `view_file` on `.agents/learned/art-2d.md` and `.agents/learned/graphrag-architecture.md`
2. **Semantic search** (optional): Run `python .agents/scripts/build_knowledge_graph.py --query "global DNA rules art"` and read `=== QUERY RESULTS ===`
3. **Conflict check**: If a new rule contradicts existing Global DNA, STOP and present the conflict to the user before writing
4. **Declare result**: State `"PRE-CHECK PASSED"` or list conflicts before continuing

## Overview
This skill acts as an Active Knowledge Management Agent for the Master Configuration file governing global aesthetic physics (`Assets/GameArtist/Global_DNA.md`). It ensures modifications are processed by the graph builder script to keep the global GraphRAG database up to date, while actively checking for structural conflict against specific local styles.

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
- Immediately after updating the Markdown, trigger the graph builder script to rebuild the Global GraphRAG database:
  - Run `python .agents/scripts/build_knowledge_graph.py`
- Wait for the script to finish. It will automatically update `.agents/graph_data/vector_index.faiss` and `.agents/graph_data/community_summaries.json`.

### 4. Conflict Resolution (Semantic RAG Search)
- Review the terminal output from the script run. The script automatically calculates Cosine Similarity between your new global rules and all existing local style indices.
- If the script outputs any `[WARNING]` logs about high similarity or conflicts (e.g., Global rule contradicts Style rules), you MUST print out this warning explicitly in your response to the user.

### 5. Acknowledge & Confirm
- Present the updated section explicitly to the user.
- Ask them if they want to revise the rule based on the Conflict warnings (if any).
- If no conflicts, remind the user that the global GraphRAG context is now updated for the `game-art-orchestrator` and other agents.
- **Integration:** Run the `/finish` workflow immediately after to record any structural rule conflicts found into the `docs/learned/` knowledge base.
