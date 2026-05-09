# GraphRAG Architecture and Context Retrieval

## Pre-computed Context (Zero Latency Strategy)
Instead of executing LLM operations during runtime, the project utilizes a **Pre-computed GraphRAG Architecture**.

1. **The Brain Builder (`scripts/build_knowledge_graph.py`)**:
   - Runs offline. Extracts entities via rule-based Markdown parsing (headings → Category nodes, `**[Tag]**` → Tagged nodes, backtick terms → Technical nodes).
   - Runs the Louvain algorithm to cluster entities into Communities.
   - Generates extractive Community Summaries (no LLM required — concatenates node labels grouped by type).
   - **Outputs**:
     - `knowledge_graph.json`: Raw node/edge graph data.
     - `community_summaries.json`: Extracted text clusters.
     - `interactive_mindmap.html`: Interactive vis.js visualization.
   
2. **FAISS Vector Search**:
   - Community Summaries are embedded via local `SentenceTransformer` (`all-MiniLM-L6-v2`) into `vector_index.faiss`.
   - Retrieval scripts (like `retrieve_orchestrator_context.py`) vector-search this FAISS index instantly — zero API costs.
   - Fallback: TF-IDF if SentenceTransformer unavailable; text-based top-3 if FAISS fails.
   - *Requires:* `faiss-cpu` and `sentence-transformers` in Python env.

3. **CLI Query**:
   - `python build_knowledge_graph.py --query "Physics2D collision"` rebuilds + runs semantic search in one command.

## Developer Rules
- **DO NOT** add runtime LLM calls to any retrieval script. Rely on the pre-computed FAISS index.
- After adding/modifying `.md` files in `rules/`, `learned/`, or `knowledge/`, re-run `build_knowledge_graph.py`.
- The `/finish` workflow auto-triggers this rebuild (Step 3.7).
