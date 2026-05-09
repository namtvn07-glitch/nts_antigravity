using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

/// <summary>
/// Manages dialog creation, pooling, and lifecycle in the UI system.
/// OPTIMIZATION: Implements stack visibility management to auto-hide the previous dialog.
/// </summary>
public class MenuManager : MonoBehaviour
{
    #region Fields & Properties

    public static MenuManager Instance { get; private set; }

    [SerializeField, Tooltip("Main camera for Dialog canvases")]
    private Camera mainCamera;

    [SerializeField, Tooltip("Prefab array for Dialogs")]
    private Dialog[] dialogPrefabs;

    // The root canvas RectTransform, used for calculating animation positions
    private RectTransform selfRect;

    // Use List for stack management (last added is on top)
    private readonly List<Dialog> activeDialogs = new List<Dialog>();
    private readonly Dictionary<Type, ObjectPool<Dialog>> dialogPools = new Dictionary<Type, ObjectPool<Dialog>>();
    private int currentSortingOrder = 100;

    /// <summary>Gets the main camera used for dialog rendering.</summary>
    public Camera MainCamera => mainCamera;

    #endregion

    #region Unity Lifecycle

    private void Awake()
    {
        Instance = this;
        if (mainCamera == null)
            mainCamera = Camera.main;

        // Get the RectTransform of this manager, assuming it's on the root canvas
        selfRect = GetComponent<RectTransform>();
    }

    private void OnDestroy()
    {
        Instance = null;
        foreach (var pool in dialogPools.Values)
            pool.Dispose();
    }

    #endregion

    #region Public Utility Methods

    /// <summary>
    /// Gets the next available sorting order for dialog canvases.
    /// </summary>
    public int GetNextSortingOrder() => ++currentSortingOrder;

    /// <summary>
    /// Calculates the off-screen start/end position for animations relative to the root canvas.
    /// </summary>
    public Vector2 GetStartPosition(RectTransform dialogRect, MoveDirection dir)
    {
        // ... (Logic remains the same: calculation based on selfRect)
        Vector2 startPos = Vector2.zero;
        if (selfRect == null)
        {
            Debug.LogWarning("MenuManager's RectTransform (selfRect) is null. Cannot calculate animation position.");
            return startPos;
        }

        float parentWidth = selfRect.rect.width;
        float parentHeight = selfRect.rect.height;

        float dialogWidth = dialogRect.rect.width;
        float dialogHeight = dialogRect.rect.height;

        switch (dir)
        {
            case MoveDirection.BottomScreenEdge:
                startPos.y = -(parentHeight / 2) - (dialogHeight / 2);
                break;
            case MoveDirection.TopScreenEdge:
                startPos.y = (parentHeight / 2) + (dialogHeight / 2);
                break;
            case MoveDirection.LeftScreenEdge:
                startPos.x = -(parentWidth / 2) - (dialogWidth / 2);
                break;
            case MoveDirection.RightScreenEdge:
                startPos.x = (parentWidth / 2) + (dialogWidth / 2);
                break;
        }

        return startPos;
    }

    /// <summary>
    /// Creates or retrieves a dialog instance from the object pool.
    /// </summary>
    public T CreateDialog<T>() where T : Dialog
    {
        var type = typeof(T);
        if (!dialogPools.ContainsKey(type))
        {
            var prefab = GetPrefab<T>(dialogPrefabs);
            dialogPools[type] = new ObjectPool<Dialog>(
                createFunc: () =>
                {
                    var instance = Instantiate(prefab, transform);
                    instance.name = prefab.name;
                    instance.IsPooled = true;
                    return instance;
                },
                actionOnGet: dialog =>
                {
                    dialog.gameObject.SetActive(true);
                    dialog.IsPooled = false; // Mark as active
                },
                actionOnRelease: dialog =>
                {
                    // Reset all visual state before returning to pool
                    dialog.isShow = false;
                    dialog.IsPooled = true; // Mark as pooled
                    dialog.gameObject.SetActive(false);
                },
                actionOnDestroy: dialog => Destroy(dialog.gameObject),
                defaultCapacity: 5
            );
        }
        return (T)dialogPools[type].Get();
    }

    #endregion

    #region Dialog Stack Management

    /// <summary>
    /// Registers a dialog as active and pushes it to the top of the stack.
    /// </summary>
    public void OpenDialog(Dialog instance)
    {
        // 1. Update the stack list
        if (activeDialogs.Contains(instance))
        {
            activeDialogs.Remove(instance); // Remove to re-add (move to top)
        }
        activeDialogs.Add(instance);

        // 2. Initialization for the new top dialog (Show() will be called in Dialog<T>.Open)
        instance.OnDialogBecameVisible();
    }

    /// <summary>
    /// Closes a dialog, returns it to the object pool, and visually SHOWS the new topmost dialog.
    /// </summary>
    public void CloseDialog(Dialog instance, bool isDestroy = true)
    {
        if (instance == null || !activeDialogs.Contains(instance))
            return;

        // 1. Remove the dialog being closed
        activeDialogs.Remove(instance);

        if (instance.DestroyWhenClosed)
        {
            Destroy(instance.gameObject);
        }
        else
        {
            // 2. Return it to the object pool
            var type = instance.GetType();
            if (dialogPools.ContainsKey(type))
                dialogPools[type].Release(instance);
        }

        // 3. Reset sorting order when no dialogs are active to prevent overflow
        if (activeDialogs.Count == 0)
        {
            currentSortingOrder = 100;
        }
    }

    #endregion

    #region Private Helper Methods

    /// <summary>
    /// Finds a dialog prefab of the specified type.
    /// </summary>
    private T GetPrefab<T>(Dialog[] prefabs) where T : Dialog
    {
        foreach (var prefab in prefabs)
        {
            if (prefab != null && prefab.GetType() == typeof(T))
                return (T)prefab;
        }

        throw new MissingReferenceException($"Prefab not found for type {typeof(T)}");
    }

    #endregion
}