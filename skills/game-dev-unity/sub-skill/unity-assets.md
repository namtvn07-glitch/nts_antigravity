## Core Pattern

### 1. Dynamic Asset Management (Addressables)
Never load large amounts of assets or static data simultaneously at startup unless strictly necessary.
- **Addressables**: Use the Unity Addressables System to asynchronously load/unload assets, scenes, and prefabs.
- **Resources Folder**: **STRICTLY PROHIBITED** for large/binary files. The `Resources` folder permanently blobs into the application build, destroying load times and memory footprint.
- **Asset Bundles**: Addressables handles bundle management under the hood, but always optimize bundle sizes and group them logically.

### 2. Texture & Audio Compression
Assets dictate memory. The wrong format will crash lower-end devices.
- **Textures**: Use ASTC (Mobile) or BC7 (PC/Console). Ensure sprite atlases are enabled with "Power of Two" (POT) sizes. ALWAYS enable "Crunched Compression" if applicable.
- **Audio**: 
  - Short SFX (< 5 seconds): `Decompress on Load`.
  - Medium SFX/BGM: `Compressed in Memory`.
  - Long Background Music: `Streaming`.

### 3. Circular Dependencies & Asset Streaming
- Prevent assets from referencing each other infinitely. Use GUIDs or Addressable strings instead of direct prefab references in ScriptableObjects if it leads to massive dependency chains loading synchronously.

## Common Mistakes & Rationalizations

| Excuse (Rationalization) | Reality Check |
|--------------------------|---------------|
| "I put all 500 game items in the Resources folder so I can load them easily with Resources.Load." | The app will take 20 seconds to boot on a phone and consume 1GB of RAM instantly. Use Addressables instead. |
| "I left the texture size at 4096x4096 uncompressed because it looks crisp." | It eats 64MB of VRAM for one image. Downscale textures to max necessary resolution and apply ASTC compression. |
| "I passed the heavy Boss Prefab directly into the Main Menu script to spawn it later." | This forces the Boss texture to load while parsing the Main Menu. Use `AssetReference` to load it ONLY when entering the boss level. |

**Any Red Flags appearing: DELETE CODE, REFACTOR TO USE ADDRESSABLES AND EFFICIENT COMPRESSION FORMATS.**
