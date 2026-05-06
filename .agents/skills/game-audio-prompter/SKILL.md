---
name: game-audio-prompter
description: An AI prompt engineering skill that reads game design documents and audio asset lists to automatically construct specialized BGM and SFX prompts for external Audio LLMs.
---

# Game Audio Prompter

## Purpose
This skill acts as a specialized **Game Audio Designer Agent**. It translates technical mechanics and visual art styles described in Game Design Documents (GDD) into precise, well-formatted Audio Generation Prompts suitable for state-of-the-art models like Suno, Udio, ElevenLabs, or ChatGPT Advanced Voice.

## Workflow

When the user invokes the `game-audio-prompter` skill on a specific project, **YOU MUST** rigorously execute the following phases:

### Phase 1: Ingest Local Context
1. Scan the target project directory.
2. Locate and use `view_file` to read the `*_GDD.md` document. Extract:
   - **Core Gameplay Mechanics:** How the game plays (e.g., fast twitch, constant scrolling, deliberate puzzles).
   - **Visual Art Style:** The look (e.g., Line-art, Pixel Art, Hyper-realistic).
   - **Overall Game Vibe:** The emotional tone and target audience (e.g., Hybrid-casual, Mid-core).
3. Locate and use `view_file` to read the `*_Audio_Assets.json` file. Extract the full list of required audio assets, noting their IDs, Layer type (BGM vs SFX), and Loop flags.

### Phase 2: Apply Audio Translation Logic (CRITICAL)
You cannot just copy the description over. You MUST translate visual/mechanic tags into Audio Envelopes and Textures using your LLM inference.
- **Rule of Reverb / Texture:** If the visual is "Line-art" or "Minimalist", the audio must be prompted as "Crisp, zero reverb, bright high-end" to match the clean aesthetic and avoid muddy overlaps. If it's "Atmospheric", prompt for spatial reverb.
- **Rule of Fatigue:** If a game mechanic triggers at high frequency (e.g., shooting 3 times a second), the SFX prompt MUST state: "Fast attack, instant decay, strictly non-fatiguing to the ear."
- **Rule of Loops:** Any asset marked `loop_flag: true` MUST forcefully include the prompt constraint: "MUST be a seamless looping track, no fade-in or fade-out."
- **Rule of Style Matching:** Map instrumentation to the visual era (e.g., Retro/8-bit visuals dictate Synthwave, Chiptune, or basic waveform instruments).
- **Rule of Duration Constraints:** Explicitly enforce a strict time limit for every asset. Typical SFX MUST be constrained to precise millisecond/second limits (e.g., "< 500ms", "Max 1s"). BGM should also have a defined loop length expectation if applicable.

### Phase 3: Generate the Prompt Book
Create a Markdown file named `<ProjectName>_Audio_Prompt_Book.md` in the project's root directory. 
The generated file MUST strictly follow the structural template below.

#### Target File Structure:
```markdown
# 🎵 [Project Name]: Game Audio Generation Prompt Book

*Tài liệu này được định dạng chuyên biệt để bạn sao chép (copy-paste) vào các Audio LLM (ElevenLabs, Udio, Suno, v.v.).*

## 1. System Definition
- **System Prompt:** "Act as a professional Game Audio Designer. You are creating the audio soundscape for a [Genre] game with a [Visual Style] aesthetic. The audio must feel [Translated Vibe]."
- **Global Constraints:** [Note any limits, like "Must be extremely short files under 50kb for Playable Ads", or "No orchestral overlaps"].

## 2. Background Music (BGM) Request
### [BGM Asset ID]
- **Gameplay Context:** [Where/When it plays]
- **Duration Constraint:** [Expected track/loop length, e.g., ~15s to 30s loop]
- **Prompt Formulation:** "[Estimated BPM], [Instruments], [Mood]. [Loop constraints]."

## 3. Sound Effects (SFX) Breakdown
*(Repeat for EVERY SFX found in the JSON - DO NOT truncate unless explicitly instructed)*
### [SFX Asset ID]
- **Gameplay Context:** [What action triggers this sound]
- **Trigger Frequency:** [Low / High / Continuous]
- **Duration Constraint:** [Strict maximum length, e.g., Max 0.5s or Max 2s]
- **Audio Texture Prompt:** "[Attack/Decay envelope], [Reverb level], [Timbre/Pitch]. [Fatigue constraints if high-frequency]."
```

### Phase 4: Finalize & Integration
Once the file is generated, review it structurally. Provide the user with a clickable link to the generated document and ask if they need assistance executing the generated prompts via other tools or APIs.
- **Workflow Hook:** Finally, trigger the `/finish` workflow to log successful audio prompting keywords and formatting techniques into the project's learned documentation.
