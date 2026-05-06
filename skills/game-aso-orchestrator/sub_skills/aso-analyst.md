---
name: aso-analyst
description: Internal operation - Used to scrape store link data and generate ASO_Design_Plan.md
---

# ASO Analyst

Task: Scrape store link data, perform semantic RAG analysis, and draft the art design plan.

## Execution Steps:
1. Receive the link and start the `browser_subagent`. Observe ONLY the Above-the-fold content (Header, Logo, First Screenshot). Extract the Core Vibe, Color Palette, and Art Style. Extract Text Descriptions if visible in the DOM.
2. Execute the python script: `python .agents/skills/game-aso-orchestrator/scripts/retrieve_aso_style.py "<Vibe text>"` to fetch the Top 3 matching Styles from the RAG system (Semantic Search).
3. Present the Top 3 Styles in the chat and ask the User: "What is the Project/Folder name for this ASO? Which Style from the Top 3 do you want to use as the core DNA?"
4. Pause the execution and WAIT for the User's response (Review-Gate 1).
5. After the User selects a Style, read the `Generation_DNA.md` file of that specific Style. Combine this DNA with your initial analysis to formulate and write the results into `ASO_Design_Plan.md`. STRICTLY lock down the "English Generation Prompts" and clearly state the static dimensions: Icon (512x512), Feature (1024x500), Screenshot 1-5 (900x1600 for portrait, or 1600x900 for landscape). Also record the `ProjectFolder` variable to serve as the storage ID.
> **CRITICAL RULE:** The plan must explicitly mandate the use of `generate_image`, and no text/typography should be hallucinated in the image.
> The plan must define a Key Art (typically Feature Graphic) to be isolated in Phase 2 as the absolute Anchor.
> **NARRATIVE & CAMERA CONSTRAINT:** You MUST establish a "Global Camera & Environment Prefix" (e.g., *2.5D orthographic side-scroller, deep space bg*) for all Screenshots to ensure they look like the same game. The 5 screenshots must follow a sequential gameplay narrative (Player Journey).
