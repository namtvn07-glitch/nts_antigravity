## Core Pattern

### 1. Modern Rendering Pipeling (URP/HDRP) Optimization
Always configure the Universal Render Pipeline (URP) or High Definition Render Pipeline (HDRP) based on target platform requirements.
- **URP**: Prioritize for Mobile, WebGL, and Switch. Use minimal Renderer Features and prioritize Mobile-friendly shaders.
- **HDRP**: Use only for high-end PC/Console targets. Focus on physically based rendering (PBR) and advanced lighting.
- **Camera/Post-Processing**: Avoid using multiple Post-Processing Volumes with global overrides. Instead, use local volumes optimally. 

### 2. Shader Graph over HLSL
- Use **Shader Graph** for prototyping and most visual effects to ensure pipeline compatibility.
- Write raw HLSL shaders ONLY if custom complex effects cannot be achieved efficiently in Shader Graph (e.g., specific compute shaders or custom lighting models).

### 3. Draw Call & Batching Discipline
- **Static Batching/SRP Batcher**: Ensure materials are compatible with the SRP Batcher (use the same shader). 
- **LOD (Level of Detail)**: All high-poly 3D models MUST have LOD Groups set up. Geometry must degrade at distance.
- Use Frustum Culling and Occlusion Culling to prevent rendering invisible objects.

## Common Mistakes & Rationalizations

| Excuse (Rationalization) | Reality Check |
|--------------------------|---------------|
| "I'll just add 5 Realtime Point Lights in this room for better visuals." | Realtime lights break SRP batching and multiply draw calls. Use Light Probes for dynamic bodies, and Bake lighting for static environments. |
| "This character model looks great with 10 different materials on it." | 10 materials = 10 draw calls per character. Merge textures into a single texture atlas (1 material) to allow batching. |
| "I wrote an older Built-In pipeline shader because I found the code online." | Built-In shaders are broken/magenta in URP/HDRP. Upgrade all materials and write shaders exclusively for the active Render Pipeline. |

**Any Red Flags appearing: DELETE CODE, IMPLEMENT BATCHING & URP/HDRP HYGIENE STANDARDS IMMEDIATELY.**
