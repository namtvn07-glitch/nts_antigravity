## Core Pattern

### 1. Robust Audio Pooling
Dozens of simultaneous overlapping sounds will clip, lag, and generate massive Garbage Collector (GC) pressure if instantiated or destroyed dynamically.
- **AudioSource Object Pooling**: Instead of `Instantiate` -> `Play` -> `Destroy`, use an Object Pool of Audio Sources. Grab an available AudioSource, play the clip, and return it to the pool when finished.
- Limit max simultaneous voices using Unity's `Max Real Voices` setting.

### 2. AudioMixer & Routing Hygiene
- **Never route audio directly to the AudioListener.**
- ALL AudioSources MUST route into a designated Group on an `AudioMixer` (e.g., Master -> SFX, Master -> BGM, Master -> UI). This allows for centralized volume control, ducking, and global snapshots.

### 3. Spatial Audio & Optimization
- **3D vs 2D**: Ensure UI and BGM are strictly 2D (`Spatial Blend` = 0). For 3D environment sounds, properly configure `Min Distance`, `Max Distance`, and use a logarithmic rolloff curve.
- **Disable when Silent**: Audio sources should pause or disable themselves when muted or completely out of range to save CPU cycles. 

## Common Mistakes & Rationalizations

| Excuse (Rationalization) | Reality Check |
|--------------------------|---------------|
| "I wrote an extension method to `AudioSource.PlayClipAtPoint` for convenience." | `PlayClipAtPoint` creates a ghost GameObject and destroys it immediately, allocating massive GC. Write a proper Audio Pool system instead. |
| "I didn't bother using an AudioMixer, I just looped through all objects to change their volume." | Unscalable and inefficient. Use an AudioMixer and expose a parameter for Volume to change everything instantly. |
| "All sound files are loaded as 3D audio, even the menu clicks." | Spatial sound requires extra CPU processing. Explicitly set UI and global sounds to 2D. |

**Any Red Flags appearing: DELETE CODE, REFACTOR TO USE AUDIO POOLS AND THE AUDIOMIXER.**
