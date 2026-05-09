# UI Dialog System — Architecture Reference

> Source: `Dialog.cs` (260 lines) + `MenuManager.cs` (206 lines)  
> Pattern category: UI Architecture | Zero-Allocation Object Pooling

## Pattern: Generic Dialog&lt;T&gt; with Static Lifecycle

All dialogs inherit `Dialog<T>` where T is the concrete class:
- Static `Instance` property — set in `Awake/OnEnable`, cleared in `OnDestroy`
- `Open()` and `Close()` are **static** methods on `Dialog<T>`, not instance methods
- Caller never holds a direct reference; always uses `DialogType.Open()` / `DialogType.Close()`

```csharp
// Correct usage:
SettingsDialog.Open();
SettingsDialog.Close();
// Never: dialogRef.Open() — use static accessor
```

## Pattern: MenuManager with ObjectPool&lt;Dialog&gt;

- `MenuManager.Instance.CreateDialog<T>()` retrieves from a per-type `ObjectPool<Dialog>`
- Pool lifecycle flags: `actionOnGet` sets `IsPooled=false`; `actionOnRelease` sets `IsPooled=true`
- Pool keyed by `Type` in `Dictionary<Type, ObjectPool<Dialog>>`
- Sorting order auto-increments via `GetNextSortingOrder()`, resets to 100 when stack empty

## Critical Rules

- **Canvas setup**: `RenderMode.ScreenSpaceCamera` + `overrideSorting = true` (set once via `isCanvasConfigured` flag)
- **Never disable GameObject directly on Close** — wait for DOTween `closeTween` callback to fire `OnHideComplete` before pool release
- **`DestroyWhenClosed = true`**: skips pool, immediately destroys (use only for heavy/rare dialogs)
- **Animation positions**: calculated from root canvas RectTransform via `GetStartPosition(dialogRect, MoveDirection)`, NOT hardcoded pixel offsets
- **DOTween kill guard**: `rectTransform.DOKill(complete: false)` called before every Show/Hide to prevent tween conflicts
- **Race condition guard on Close**: Instance is NOT cleared immediately — cleared only inside the Hide callback lambda after animation completes

## Integration Contract

**Requires (Inspector):**
- `Camera mainCamera` — for Dialog canvas RenderMode.ScreenSpaceCamera
- `Dialog[] dialogPrefabs` — one prefab entry per concrete Dialog type

**Events called by MenuManager:**
- `OnDialogBecameVisible()` — called when dialog becomes top of stack (override for refresh logic)
- `AutoOnClose()` — called by coroutine when `AutoClosePoup = true` after `timeAutoClose` seconds

## Gotcha: Instance null after animation

Do NOT check `Dialog<T>.Instance != null` immediately after calling `Close()`.  
The static Instance is cleared asynchronously inside the DOTween callback — it will still be non-null during the hide animation duration.
