# Project-Specific Unity Operational Rules

> **Note:** Architectural constraints (like Single-Scene, UI Dialog Pattern) and runtime gotchas (like physics/audio bugs) are explicitly excluded from this file. The agent MUST rely on the GraphRAG query results from the PRE-CHECK to fetch the latest context dynamically.

## 1. MCP Live Editor Verification (MANDATORY)

- **Live Verification over Theory**: NEVER blindly write C# scripts and assume they work. GameObjects are driven by complex serialization, hierarchy, and strict execution orders. You MUST use MCP capabilities (e.g., `manage_editor` action=play) to verify your logic, physics, and references in the real engine environment.
- **Do not claim a fix works "in theory."** Evidence of testing (Console output or Screenshots) must be provided in your `walkthrough.md` or escalation report.

## 2. YAML Parsing Prohibition

- **Static Reading is Banned**: NEVER attempt to read or parse `.unity` or `.prefab` YAML files as static text (via `view_file`, `cat`, or `grep_search`) to understand scene hierarchy, references, or components. YAML serialization contains raw GUIDs and massive bloat that causes hallucinations.
- **Solution**: Always use Unity MCP tools (e.g., `manage_scene`, `manage_gameobject`, `manage_components`) to query the live Editor for scene hierarchies, GameObject details, and component states.

## 3. Mobile Performance Hygiene

- **Canvas Batching Enforcement**: Whenever generating UI via code, ALWAYS disable `Raycast Target` on non-interactive elements (Images, Text) to save CPU on mobile.
- **VRAM Strict Limits**: If generating or assigning UI assets, enforce strict resizing (Icons <= 256px, Buttons <= 512px, Panels <= 1024px) and apply ASTC compression. Do not use uncompressed 4K textures.
