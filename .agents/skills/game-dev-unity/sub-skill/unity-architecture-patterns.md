## Core Pattern

### 1. Anti-Singleton Pattern
Except for true core system managers (like AudioManager), do not turn everything into a Singleton.
- **Harm of Singletons**: Causes tight coupling, hard to test, hidden bugs due to Load/Destroy execution order.
- **Alternatives**: Use Unity Events (Event-Driven), Dependency Injection (via Constructor or ScriptableObject), or the Observer Pattern.

### 2. Static Data Must Use ScriptableObjects (Data-Driven Pattern)
Use ScriptableObjects (SOs) for mostly static data or to share references:
- Monsters (HP, Damage) 
- Items (Name, Icon, Level)
**STRICTLY PROHIBITED**: Hardcoding massive arrays, tuples, or string configuration data inside a C# MonoBehaviour class.

```csharp
// CORRECT:
[CreateAssetMenu(fileName = "NewWeapon", menuName = "Data/WeaponBase")]
public class WeaponData : ScriptableObject {
    public string weaponName;
    public float baseDamage;
}
```

### 3. Decoupling Logic and View (Model vs View)
- Components handling calculations (Model/Command) MUST NOT directly change the Alpha, Scale, or Text of a UI Component.
- They should only emit an event: `OnHealthChanged?.Invoke(float newHealth)`.
- The `HealthUI` Component (View) listening to that event will independently update the Text/Slider.

## Common Mistakes & Rationalizations

| Excuse (Rationalization) | Reality Check |
|--------------------------|---------------|
| "Turn it into a Singleton so it's easy to call from another class: `GameManager.Instance.Score += 1;`." | The root of Spaghetti Code. Instead, use a ScriptableObject containing an Event, or a static MessageBus system. |
| "Since it's just a prototype, I put HP Logic and HP Bar UI inside 1 class `Enemy.cs`." | When you need to add a screen flash effect, you will break the `Enemy` class. Separate Model and View from draft 1. |
| "There are only 3 weapons, I'll just hardcode them into an array." | Expanding the system later will take 10x the effort. Use Data-Driven immediately (ScriptableObject). |

**Any Red Flags appearing: DELETE CODE, SEPARATE CLASSES BY PROPER MODEL INSTEAD OF LUMPING THEM TOGETHER.**
