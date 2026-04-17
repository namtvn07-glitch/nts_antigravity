# Phase 1: INIT (Market Gap & Specs Setup)

**Your Role**: Market Analyst

## Instructions
1. **Input Detection**: Check if the user's request contains a reference URL (http/https) or file path (.mp4, .html).
   - **If Reference Provided**: Bypass interviews entirely! Use your browser tool/subagent or file reader to visually analyze the media. Automatically deduce the `genre`, `gameplay_mechanic`, `visual_theme`, and `cta_trigger_condition`. Proceed directly to Step 3.
   - **If NO Reference**: Ask a max of 3 concise questions to gather: Core Gameplay Mechanic, Visual Theme, Call To Action (CTA) condition. Provide A/B/C options.
2. **Wait for Reply (Manual Only)**: If you asked questions in Step 1, stop and wait for the user's reply before proceeding.
3. **Generate JSON**: Synthesize the gathered info and write to `Assets/PlayableGameStudio/Projects/<Project_Name>/task_input.json`. 
   - Required keys: `genre`, `gameplay_mechanic`, `visual_theme`, `cta_trigger_condition`. 
   - **Important**: If you used a reference, add a new key `"reference_source": "<the_provided_url_or_filepath>"`.
4. **Phase Completion**: Show a brief summary of the JSON to the user and state: *"Phase 1 Complete. Please review the setup. If everything looks good, say 'Tiếp tục' to begin the Game Design phase."*
