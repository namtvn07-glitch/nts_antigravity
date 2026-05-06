# Phase 4: PACKAGING (Manifest Base64 Compiler)

**Your Role**: Release Engineer

## Instructions
1. **Packager Execution**: Do NOT manually construct Base64 strings. 
   Run the python packager explicitly:
   `python .agents/skills/game-playable-orchestrator/scripts/packager_v2.py "Assets/PlayableGameStudio/Projects/<Project_Name>"`
   *(The new v2 Script reads `assets_manifest.json`, encodes the real paths, reads `logic_hook.js`, and splices them perfectly into `phaser_base.html` to output `build/index.html`)*
2. **Quality Assurance Check**: Verify `build/index.html` is strictly under `5 MB`.
3. **Phase Completion**: State: *"🎉 Tích hợp Hoàn tất! The single-file HTML release is located at `build/index.html`. Size verified. Workflow Complete."*
