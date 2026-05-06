# Phase 3: UI Wireframe Generation

**Role:** You are the Lead UX/UI Designer. Your job is to visually conceptualize the UI screens defined in the breakout phase.

## Pre-requisite
Phase 2 must be complete. The file `[ProjectName]_UI_Plan.md` must exist.

## Action
1. Read the `[ProjectName]_UI_Plan.md` file.
2. Create a new file named `[ProjectName]_UX_Wireframes.md` in the project directory using the `write_to_file` tool.
3. In this new file, you MUST generate an **ASCII Wireframe** for every distinct UI Screen or Popup listed in the UI Plan. Use Markdown code blocks to draw the layout (Top, Center, Bottom) with exact button names and placements.
4. Below the ASCII wireframes, generate a **Mermaid UX Flowchart** (`graph TD`) mapping the navigation logic between all these screens.

### ASCII Wireframing Rules
- Use structural ASCII art to represent the screen boundaries and elements.
- Example:
  ```text
  +-----------------------+
  | [Back]        [Coins] |
  +-----------------------+
  |                       |
  |      [Monster]        |
  |                       |
  +-----------------------+
  |    (HOLD TO RECORD)   |
  +-----------------------+
  ```
- Ensure every button and text field defined in the UI Plan is present in your ASCII wireframe.

### UX Flowchart Rules
- Use Mermaid syntax.
- Link Screens to Popups and other Screens using clear action labels (e.g., `UI_MainMenu -->|Click Store| UI_Popup_Store`).

---
**CRITICAL:** Ensure that you complete the `[ProjectName]_UX_Wireframes.md` file fully. Do not wait for human approval. After the file is generated, proceed immediately to Phase 4.
