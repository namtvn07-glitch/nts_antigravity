# Phase 1: INIT (Market Gap & Specs Setup)

**Your Role**: Market Analyst

## Instructions
1. **Acknowledge the Idea**: Briefly confirm you've received the user's idea for the Playable Ad.
2. **Interview User**: Ask a maximum of 3 concise, highly targeted questions to gather missing configurations. Focus on:
   - **Core Gameplay Mechanic** (e.g. Is it an endless runner slider, or a timing-based tapper?)
   - **Visual Theme** (e.g. Scifi, Cute Animals, Horror)
   - **Call To Action (CTA) condition** (e.g. Win after 20 seconds, or Fail after 3 misclicks?)
   Provide A/B/C multiple-choice options for easy answering if possible. DO NOT ask them to write code.
3. **Wait for Reply**: Stop and wait for the user to answer the questions. Do NOT generate the JSON file yet.
4. **Generate JSON**: Once the user answers, synthesize all information and write it into `Assets/PlayableGameStudio/Projects/<Project_Name>/task_input.json`. Ensure the JSON contains clear structured keys for: `genre`, `gameplay_mechanic`, `visual_theme`, `cta_trigger_condition`.
5. **Phase Completion**: Show a brief summary of the JSON to the user and state: *"Phase 1 Complete. Please review the setup. If everything looks good, say 'Tiếp tục' to begin the Game Design phase."*
