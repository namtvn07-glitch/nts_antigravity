## Core Pattern

### 1. Always Unsubscribe Events (Anti-Leak)
If you `+=` anywhere, you MUST `-=` in the opposing lifecycle method.
- `OnEnable` -> Unsubscribe in `OnDisable`.
- `Awake/Start` -> Unsubscribe in `OnDestroy`.

```csharp
// CORRECT:
void OnEnable() { GameManager.OnScoreChanged += UpdateScoreUI; }
void OnDisable() { GameManager.OnScoreChanged -= UpdateScoreUI; }
```

### 2. Zero GC Alloc in Constant Loops
Creating new objects, string concatenations (`"" + var`), or string interpolations (`$"{var}"`) generates garbage that accelerates the C# Garbage Collector spike.
- **STRICTLY PROHIBITED**: Calling `.ToString()` or concatenating strings inside `Update()`.
- **Solution**: Use temporary state variables, `StringBuilder`, or dedicated UI frameworks like TextMeshPro (using char arrays or `SetText()`). Ideally, only update values when an Event triggers instead of checking endlessly inside `Update()`.

### 3. Caching References, No Dynamic Lookups
- Declare and cache references using `GetComponent<T>()` exactly ONE time inside `Awake()`.
- **PROHIBITED**: Calling `GetComponent()`, `.Find()`, or `Camera.main` within `Update()`. 

## Common Mistakes & Rationalizations

| Excuse (Rationalization) | Reality Check |
|--------------------------|---------------|
| "I'm only using string format once in Update, it won't lag." | Many small scripts doing this = Mobile FPS stutter. Zero exceptions! |
| "When leaving the scene, memory is wiped anyway. Why unsubscribe static events?" | Static events persistently hold the GameObject's reference, creating immortal Zombie Objects eating RAM. Delete code and rewrite! |
| "I used `Object.FindObjectOfType` in Start() to save time instead of assigning in Inspector." | Forbidden because it breaks Dependency Injection and wastes initialization time. Use `[SerializeField]` as the standard. |

**Any Red Flags appearing: DELETE CODE, RESTART WITH THESE HYGIENE STANDARDS.**
