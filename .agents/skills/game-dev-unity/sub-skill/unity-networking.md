## Core Pattern

### 1. Authoritative Server Architecture
Never trust the client. Implement a Server Authority model where clients only send Inputs and the Server validates, computes, and replicates the State.
- **NGO (Netcode for GameObjects)**: Use this as the default multiplayer framework for Unity. Mirror / Photon can be alternatives if specifically requested.
- **RPCs vs NetworkVariables**: Use `NetworkVariables` for persistent state data (like Player Health) that latches onto late joiners. Use `RPCs` (ClientRpc/ServerRpc) for transient events like "Play Sound" or "Spawn Particle".

### 2. Bandwidth Optimization & Tick Rate
Multiplayer games die if they send too much data.
- Only sync what is absolutely necessary. Compress floats to bytes if precision isn't strictly required.
- Do not sync data inside `Update()` blindly. Only sync changes.

### 3. Client Prediction & Lag Compensation
- Implement client-side prediction for movement. Do not wait for the server roundtrip to move the player locally, otherwise, the game feels unresponsive. 
- Implement lag compensation/rewind on the server for hit registration (e.g., raycasting back in time based on the client's ping).

## Common Mistakes & Rationalizations

| Excuse (Rationalization) | Reality Check |
|--------------------------|---------------|
| "I'll let the client reduce the enemy's HP to make hits feel instant." | Hackers will instantly set Enemy HP to zero. Clients request damage; Server applies damage. |
| "I'm syncing the exact `transform.position` every frame." | Saturates bandwidth. Sync positions gracefully at a lower network tick rate using interpolation/extrapolation on the client side. |
| "I'm sending a heavy JSON string in my ServerRpc." | JSON is bloated. Use lightweight bitpacking (e.g. `FastBufferWriter`) to send minimal bytes across the network. |

**Any Red Flags appearing: DELETE CODE, SECURE THE SERVER ARCHITECTURE AND MINIMIZE BANDWIDTH.**
