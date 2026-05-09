using System;
using System.Collections;
using DG.Tweening;
using HenryLe.Scripts.UIModule;
using UnityEngine;

/// <summary>
/// Generic base class for dialogs with static accessors and correct instance lifecycle management.
/// </summary>
public abstract class Dialog<T> : Dialog where T : Dialog<T>
{
    public static T Instance { get; private set; }

    protected override void Awake()
    {
        base.Awake(); // Call the base Dialog's Awake

        // Set Instance if it doesn't exist (handles dialogs placed directly in scene)
        if (Instance == null)
        {
            Instance = (T)this;
        }
    }

    protected override void OnEnable()
    {
        base.OnEnable();

        // Re-assign Instance when enabled (in case previous instance was destroyed)
        if (Instance == null || Instance.IsPooled)
        {
            Instance = (T)this;
        }
    }

    protected virtual void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }

    /// <summary>
    /// Opens the dialog: creates/retrieves, configures canvas, pushes to stack, and calls Show.
    /// </summary>
    public static void Open(Action actionOnShowStart = null, Action actionOnShowCompleted = null, Action actionOnHide = null)
    {
        T dialogInstance;

        if (Instance != null && !Instance.IsPooled)
        {
            // Instance already exists and is active, just show it again
            dialogInstance = Instance;
            // Force kill any ongoing Hide tweens so it can successfully Open again
            dialogInstance.rectTransform?.DOKill(complete: false);
        }
        else
        {
            // Create new instance or get from pool
            dialogInstance = MenuManager.Instance.CreateDialog<T>();
            Instance = dialogInstance; // Set static instance reference
        }

        dialogInstance.ConfigureCanvas();
        MenuManager.Instance.OpenDialog(dialogInstance); // Pushes to stack and HIDES old top

        dialogInstance.Show(actionOnShowStart, actionOnShowCompleted, actionOnHide);
    }

    /// <summary>
    /// Hides and Closes the dialog, returns it to the pool, and shows the previous dialog.
    /// </summary>
    public static void Close(Action onHideComplete = null)
    {
        if (Instance == null)
        {
#if UNITY_EDITOR
            Debug.LogWarning($"Trying to close dialog {typeof(T)} but Instance is null");
#endif
            return;
        }

        T instanceToClose = Instance;

        // KHÔNG clear Instance ở đây — chờ animation xong để tránh race condition
        instanceToClose.Hide(() =>
        {
            // Clear Instance SAU KHI animation Hide hoàn thành
            if (Instance == instanceToClose)
                Instance = null;

            onHideComplete?.Invoke();
            MenuManager.Instance?.CloseDialog(instanceToClose);
        });
    }

    /// <summary>
    /// Configures the dialog's canvas for rendering and sorting order.
    /// </summary>
    public void ConfigureCanvas()
    {
        var canvas = GetComponent<Canvas>();
        if (canvas != null)
        {
            // Only configure canvas settings once (except sorting order which always updates)
            if (!isCanvasConfigured)
            {
                // Optimal Rendering Setup: ScreenSpaceCamera
                canvas.renderMode = RenderMode.ScreenSpaceCamera;
                canvas.worldCamera = MenuManager.Instance.MainCamera;
                canvas.overrideSorting = true;
                isCanvasConfigured = true;
            }

            // Always update sorting order to ensure dialog appears on top
            canvas.sortingOrder = MenuManager.Instance.GetNextSortingOrder();
        }
    }

    public override void AutoOnClose()
    {
        Close(null);
    }
}

/// <summary>
/// Abstract base class for all dialogs, containing core components and animation logic (using DOTween).
/// </summary>
[RequireComponent(typeof(RectTransform))]
public abstract class Dialog : MonoBehaviour
{
    public bool hidePreviousDialog = true;
    [Tooltip("Destroy the Game Object when dialog is closed (reduces memory usage)")]
    public bool DestroyWhenClosed = true;
    [Header("COMPONENTS")]
    protected RectTransform rectTransform;

    public TweenData openTween;

    public TweenData closeTween;

    public bool isShow = false;
    public bool IsPooled { get; set; } = true; // Start as pooled
    protected bool isCanvasConfigured = false; // Track if canvas has been set up
    private Action OnHideComplete;

    public bool AutoClosePoup = false;
    public float timeAutoClose = 0;

    protected virtual void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    private IEnumerator DelayClose()
    {
        yield return new WaitForSeconds(timeAutoClose);
        AutoOnClose();
    }

    protected virtual void OnEnable()
    {
        if (AutoClosePoup)
        {
            StartCoroutine(DelayClose());
        }
        // IsPooled flag will be set by MenuManager when retrieved from pool
    }

    protected virtual void OnDisable()
    {
        // Don't set IsPooled here as OnDisable can be called in various scenarios
    }

    public virtual void SetHideAction(Action actionHide)
    {
        OnHideComplete += actionHide; // Dùng += để tránh tích tụ callbacks
    }

    public virtual void OnDialogBecameVisible() { }
    protected virtual void _OnShowCompleted() { }
    protected virtual void _OnHideStart() { }

    public virtual void AutoOnClose()
    {
        // Override this in derived class to call Close()
    }

    /// <summary>
    /// Show the dialog with animation.
    /// </summary>
    public virtual void Show(Action actionOnShowStart = null, Action actionOnShowCompleted = null, Action actionOnHide = null)
    {
        rectTransform.anchoredPosition = Vector2.zero;

        if (isShow)
        {
            isShow = false;
            rectTransform.DOKill(complete: false);
        }

        isShow = true;
        OnHideComplete = actionOnHide;
        rectTransform.DOKill(complete: false);

        actionOnShowStart?.Invoke();

        Action callbackStart = () =>
        {
            actionOnShowCompleted?.Invoke();
            _OnShowCompleted();
        };

        if (openTween != null)
        {
            // Truyền callback trực tiếp vào DOTween Sequence (không dùng Coroutine song song)
            openTween.SetupData(callbackStart, gameObject.name);
        }
        else
        {
            callbackStart?.Invoke();
        }
    }

    /// <summary>
    /// Hide the dialog with animation.
    /// </summary>
    public virtual void Hide(Action actionOnHideCompleted = null)
    {
        if (!isShow)
        {
            OnHideComplete?.Invoke();
            OnHideComplete = null;
            actionOnHideCompleted?.Invoke();
            return;
        }

        _OnHideStart();
        isShow = false;

        rectTransform.DOKill(complete: false);

        Action callbackClose = () =>
        {
            OnHideComplete?.Invoke();
            OnHideComplete = null;
            actionOnHideCompleted?.Invoke();
        };

        if (closeTween != null)
        {
            // Truyền callback trực tiếp vào DOTween Sequence (không dùng Coroutine song song)
            closeTween.SetupData(callbackClose, gameObject.name);
        }
        else
        {
            callbackClose?.Invoke();
        }
    }

}