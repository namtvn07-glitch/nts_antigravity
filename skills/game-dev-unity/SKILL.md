---
name: game-dev-unity
description: Use when starting any Unity game development task, architecture design, optimizing rendering, scripting, multi-platform deployment, or performance profiling.
---

# Unity Master Skill Router & OS

## Overview
This is the core operating system (OS) and routing skill for Unity development. The AI is strictly prohibited from writing code, designing systems, or modifying architecture without adhering to the standards defined in the specialized sub-skills below.

<HARD-GATE>
BEFORE WRITING ANY C# CODE OR MODIFYING UNITY SYSTEMS, YOU MUST:
1. **Read Learned File:** Use `view_file` on `.agents/learned/unity-dev.md` to load project-specific rules.
2. **Verify Prerequisites:** Run `python .agents/scripts/check_prerequisites.py game-dev-unity <project_path>` — if FAIL, request GDD from game-designer first.
3. **Query Context (RAG):** Run `python .agents/scripts/build_knowledge_graph.py --query "<task_keywords>"` and read `=== QUERY RESULTS ===` for architectural constraints.
4. **Load Sub-Skills:** Use `view_file` tool to read `.agents/skills/game-dev-unity/sub-skill/unity-rules.md`.
5. **Declare Result:** Explicitly state `"PRE-CHECK PASSED"` and list which rules from sub-skills and GraphRAG will influence your implementation.

Failure to follow these steps will result in code rejection and a severe penalty.
</HARD-GATE>

## Task Scope Triage (3 Tracks)
Upon receiving a request, evaluate its complexity and declare your track in your first response:

- **Hotfix Track (Local Fixes & Tweaks):** For minor bug fixes, parameter tweaks, or isolated changes (< 10 lines, single file).
  - *Workflow:* Pre-check $\rightarrow$ Edit Code $\rightarrow$ Verify via Console $\rightarrow$ Trigger `/finish`.
  - *Skip:* No `/plan` required.

- **Feature Track (Moderate Features):** For components, UI panels, or mechanics (10-100 lines, 1-3 files).
  - *Workflow:* Pre-check $\rightarrow$ Read `unity-rules.md` $\rightarrow$ Write Code $\rightarrow$ **Verify via MCP Play Mode** $\rightarrow$ Trigger `/finish`.
  - *Skip:* No `/plan` required.

- **Architecture Track (New Systems & Refactoring):** For complex structural changes, new core systems, or 3+ files.
  - *Workflow:* Trigger `/plan` (wait for approval) $\rightarrow$ Trigger `/execute` $\rightarrow$ **Verify via MCP Play Mode** $\rightarrow$ Trigger `/review` $\rightarrow$ Trigger `/finish`.

## Verify via MCP (MANDATORY for Feature + Architecture tracks)
Instead of blindly writing C# scripts and assuming they work correctly, you must verify them:
1. Enter Play Mode via MCP (`manage_editor` action=play)
2. Read console for errors (`read_console`)
3. If errors exist $\rightarrow$ fix $\rightarrow$ re-enter Play Mode (this counts as 1 Strike, see 3-Strike Rule below).
4. If clean $\rightarrow$ take a screenshot via `manage_camera` for visual sanity check.
5. Exit Play Mode (`manage_editor` action=stop).

## Structured 3-Strike Error Handling Rule
When using `/debug`, verifying via MCP, or facing compile/runtime errors:
If you fail to resolve an error after **3 consecutive attempts**, YOU MUST STOP. Do not guess blindly. You must output the following escalation report to the user:

```markdown
## Escalation Report (3-Strike)
**Task:** [what was being attempted]
**Attempts:**
1. [what was tried] -> [result/error]
2. [what was tried] -> [result/error]  
3. [what was tried] -> [result/error]
**Stack Trace:** [paste relevant console stack trace here]
**Hypothesis:** [best guess at root cause based on MCP query or RAG knowledge]
**Request:** [specific question for user to unblock]
```
