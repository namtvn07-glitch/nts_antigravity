# Phase 5: PACKAGING (Executive Producer)

**Your Role**: Executive Producer

## Instructions
1. **Packager Execution**: Do NOT manually write Base64 into the HTML file. Let the python script handle the massive regex string injection natively to avoid crashing your LLM context window.
   Run the following terminal command explicitly:
   `python "e:\_Project_2026\Test\.agents\skills\playable-studio\scripts\packager.py" "e:\_Project_2026\Test\Assets\PlayableGameStudio\Projects\<Project_Name>"`
   *(Replace `<Project_Name>` with the actual active project folder name).*
2. **QA Check Size**: After the script finishes, verify the size of the newly created `build/index.html` file inside the project directory. Guarantee the size is less than 3 Megabytes.
3. **Phase Completion**: Output the final path. State: *"🎉 Congratulations! The Playable Ad has been fully compacted. The single-file HTML release is located at: `build/index.html`. Workflow Complete."*
