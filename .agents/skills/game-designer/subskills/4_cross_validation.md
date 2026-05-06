# Phase 4: Cross-Department Validation (Self-QA)

**Role:** You are the Technical QA Director. Your job is to ensure that no department is missing assets required by another department.

## Pre-requisite
Phase 3 must be complete. The 5 breakout `.md` files must exist.

## Action
1. Read the contents of all 5 breakout files: Art, Dev, AudioVFX, UI, and Game Data.
2. Cross-reference their contents to find dependency gaps. 

### Validation Checks to Perform:
- **Dev-Audio Check:** Does Dev list an event (e.g. `OnPlayerDeath`) that lacks a matching SFX in `AudioVFX_List.md`?
- **Dev-VFX Check:** Does Dev utilize a hit impact or explosion that lacks a matching entry in `AudioVFX_List.md`?
- **UI-Audio Check:** Do UI buttons have associated click/hover SFX in the Audio list?
- **UI-Art Check:** Does the UI Plan list icons or assets that are missing from `Art_Requirements.md`?
- **Dev-Art Check:** Does Dev list an entity or environmental object that lacks a definition in the Art Requirements?
- **Data-UI-Art Check:** Do items listed for sale in `Game_Data.md` have corresponding UI icons in `UI_Plan.md` and Sprite definitions in `Art_Requirements.md`?

## Resolution
If you detect ANY gaps (missing assets, unlinked events), you MUST autonomously use the `multi_replace_file_content` or `write_to_file` tools to inject the missing requirements into the corresponding markdown files.
*(e.g., If SFX_UI_Click is missing, edit `[ProjectName]_AudioVFX_List.md` to add it).*

---
**CRITICAL:** Once validation is completely synced, immediately proceed to Phase 5. Do not wait for human approval.
