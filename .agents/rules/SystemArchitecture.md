# Workspace Rules - RAG, Embedding, and Few-Shot Integration

These are the core operational rules for the **GamePipeline** project. All AI agents, scripts, and workflows functioning within this workspace MUST strictly adhere to these specific technical standards.

## 1. RAG (Retrieval-Augmented Generation) Technical Standards
This project relies on a data-driven RAG architecture to enforce rigid consistency across generation pipelines.
*   **Mandatory Context Retrieval:** Never rely on local LLM memory ("zero-shot hallucination") for art styles, game logic, or evaluation rules. Context must be mechanically retrieved from the designated knowledge base (e.g., `Global_DNA.md`, archetypes).
*   **Conflict Resolution:** Implement strict hierarchy during retrieval. Specific local style rules MUST override generic global parameters.
*   **Automated Updates:** If a source knowledge document is edited, the system must synchronize/re-vectorize the data to keep the RAG pipeline accurate.

## 2. Data Embedding & Semantic Search Strategy
*   **Embedding First:** All design documentation, prompt templates, and art assets must be processed through an embedding pipeline to generate vector representations.
*   **Semantic Over Keyword Search:** Context gathering in production scripts (e.g., Orchestrator) MUST rely on Semantic Search algorithms (such as cosine similarity using `sentence-transformers`) to fetch the most contextually relevant constraints instead of rudimentary string matching (like Regex/Grepping).
*   **Data Integrity:** Ensure that vectorized chunks remain logically grouped (e.g., full JSON structures or complete markdown sections) to prevent fragmented context passing.

## 3. Few-Shot Prompting Execution
*   **Zero-Shot is Prohibited:** For any generation tasks (Design logic mapping or Image Generation), you are forbidden to use Zero-Shot prompts.
*   **Dynamic Referencing:** Use the Semantic Search capability to retrieve the **Top-K most relevant examples** and inject them directly into your agent instructions.
*   **Visual Few-Shot:** For Game Art production, the prompt must explicitly include retrieved reference images (as context to the Vision Language Model) aligned with the target aesthetic, coupled with the exact retrieved text rules.
