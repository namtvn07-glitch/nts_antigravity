## Core Pattern

### 1. Differentiate FixedUpdate and Update
- The `FixedUpdate()` loop is the standard Physics frequency. `Update()` is for drawing frames based on device FPS.
**Rule:**
Gathering input (Jump/Shoot button) -> CATCH inside `Update()`. (Because FixedUpdate runs sparsely and can miss input). Use a flag variable.
Applying force to Rigidbody -> MUST be placed inside `FixedUpdate()`.

```csharp
// CORRECT:
bool jumpRequested = false;
void Update() { 
   if(Input.GetKeyDown(KeyCode.Space)) jumpRequested = true; 
}
void FixedUpdate() {
   if(jumpRequested) {
      rb.AddForce(Vector3.up * 10f, ForceMode.Impulse);
      jumpRequested = false; // reset flag
   }
}
```

### 2. Strictly prohibit manipulating `transform.position` on Dynamic Rigidbodies
Moving a dynamic Rigidbody by tweaking `transform.position = ...` in code breaks Physics Interpolation and the collision tree scanning.
- You must use: `Rigidbody.MovePosition()`, `.velocity`, or `.AddForce()`.
- Exception: A Kinematic Rigidbody (Is Kinematic = true) can be moved with `MovePosition()` smoothly and carries a lighter collision burden.

### 3. Optimized Raycasting
- **NEVER** fire a blind raycast using `Physics.Raycast(...)` without a Layer Mask (only shoot on specific layers). It will scan the entire Scene, including UI Canvases if colliders are on.
- In large loops, you MUST use `Physics.RaycastNonAlloc` (or OverlapSphereNonAlloc) to scan and fill an existing Array instead of creating a brand new array (Garbage) continuously.

### 4. Careful with Collision Matrices
Check Tag strings using `CompareTag("Enemy")` instead of `gameObject.name == "Enemy"` or `.tag == ...` inside OnTriggerEnter. `CompareTag` is much faster.

## Common Mistakes & Rationalizations

| Excuse (Rationalization) | Reality Check |
|--------------------------|---------------|
| "My Update is fast, it's more convenient to link Rigidbody here than splitting into FixedUpdate." | The character will stutter and phase through walls at high speeds. Move code to FixedUpdate immediately! |
| "Use `new WaitForSeconds(0.1f)` to delay raycast checks." | Generates GC Garbage! Declare static WaitForSeconds or count Time.deltaTime instead. |
| "I'll just add RigidBody to the parent for easy catching, ignore child colliders." | Compound Colliders: If you change the Scale of a parent with compound colliders, the Physics engine Re-Bakes the Mesh Collision heavily. Use cautiously. |

**Any Red Flags appearing: DELETE CODE, AUDIT EVERY RIGIDBODY TO ENSURE IT DOES NOT DESYNC UPDATE.**
