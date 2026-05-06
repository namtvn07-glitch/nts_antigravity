## Core Pattern: Interacting with Unity via MCP (Model Context Protocol)

### 1. Live Inspection Over Static Reading
**NEVER** attempt to read or parse `.unity` or `.prefab` YAML files as static text to understand the scene hierarchy, references, or components.
- **STRICTLY PROHIBITED**: Running `cat`, `view_file`, or `grep_search` on `*.unity` or `*.prefab` files to deduce GameObject logic or serialize state.
- **Solution**: Always use Unity MCP tools to query the live Editor for scene hierarchies, GameObject details, and component states.

### 2. Verify Before Modifying (Live Editor Testing)
Instead of blindly writing C# scripts and assuming they work correctly, you must use MCP capabilities to enter Play Mode and verify your code in the real engine environment.
- **PROHIBITED**: Claiming a fix works "in theory" just by modifying the C# file, especially for physics, UI layout, lifecycle events, and inter-component references.
- **Solution**: Use MCP to trigger Play Mode, or execute C# reflection/commands via MCP to verify logic in real-time.

### 3. Log Retrieval via MCP Console
Do not hunt for `Editor.log` or `Player.log` files manually on the file system if there is a more direct way.
- **Solution**: Rely on the MCP-provided tools or console stream (if available) to read compilation or runtime errors synchronously.

## Common Mistakes & Rationalizations

| Excuse (Rationalization) | Reality Check |
|--------------------------|---------------|
| "Reading the .unity YAML file is faster and saves MCP tool calls." | YAML serialization contains raw GUIDs, FileIDs, and massive bloat that is impossible to safely parse for deep references. You will make errors. Use the MCP live query! |
| "I'll just write the C# script and assume it works since the logic seems simple." | Unity components have hidden Editor-specific behaviors, strict execution orders, and serialization rules. You MUST use MCP to verify it live. |
| "I'll use grep_search to find where this GameObject is instantiated." | GameObjects are driven by Scenes, Prefabs, and runtime instantiations. Static text search misses dynamic references. Query the live Editor via MCP. |

**Any sign of these Red Flags: STOP, DELETE YOUR ASSUMPTIONS, AND USE MCP TOOLS TO VERIFY LIVE STATE.**
