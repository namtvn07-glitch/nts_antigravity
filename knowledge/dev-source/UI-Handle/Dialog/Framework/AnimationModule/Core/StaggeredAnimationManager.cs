using System;
using System.Collections.Generic;
using System.Reflection;
using DG.Tweening;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace HenryLe.Scripts.AnimationModule
{
	/// <summary>
	/// Manager cho staggered animations - play animations cho nhiều objects với delay giữa chúng
	/// </summary>
	public class StaggeredAnimationManager : MonoBehaviour
	{
		[Header("Target Objects")]
		[Tooltip("List các objects sẽ được animate")]
		public List<GameObject> targetObjects = new List<GameObject>();

		[Header("Animation Settings")]
		[Tooltip("Template animation config (sẽ được copy cho mỗi object)")]
		public AnimationConfig animationTemplate;

		[Header("Stagger Settings")]
		[Tooltip("Delay giữa mỗi object (seconds)")]
		public float staggerDelay = 0.1f;

		[Tooltip("Thứ tự play animation")]
		public StaggerOrder staggerOrder = StaggerOrder.Forward;

		[Tooltip("Play tất cả cùng lúc sau khi delay, thay vì chờ animation trước complete")]
		public bool playAllSimultaneously = false;

		[Header("Auto Play")]
		public AutoPlayTrigger autoPlayTrigger = AutoPlayTrigger.None;

		[Header("Global Settings")]
		public float globalTimeScale = 1f;
		public bool killOnDisable = true;

		// Runtime
		private Sequence mainSequence;
		private List<Tween> activeTweens = new List<Tween>();
		private bool isPlaying = false;
#if UNITY_EDITOR
		private bool editorManualPreviewActive = false;
		private double editorLastUpdateTime = 0f;
#endif

		// Events
		public event Action OnPlaybackStart;
		public event Action OnPlaybackComplete;
		public event Action<int, GameObject> OnObjectAnimationStart; // index, object
		public event Action<int, GameObject> OnObjectAnimationComplete; // index, object

		#region Unity Lifecycle

		private void Awake()
		{
			if (autoPlayTrigger == AutoPlayTrigger.OnAwake)
			{
				Play();
			}
		}

		private void Start()
		{
			if (autoPlayTrigger == AutoPlayTrigger.OnStart)
			{
				Play();
			}
		}

		private void OnEnable()
		{
			if (autoPlayTrigger == AutoPlayTrigger.OnEnable)
			{
				Play();
			}
		}

		private void OnDisable()
		{
			if (killOnDisable)
			{
				Kill();
			}
		}

		private void OnDestroy()
		{
			Kill();
			ClearAllEvents();
		}

		#endregion

		#region Public Methods

		public void Play()
		{
			if (targetObjects == null || targetObjects.Count == 0)
			{
				Debug.LogWarning("StaggeredAnimationManager: No target objects!");
				return;
			}

			if (animationTemplate == null)
			{
				Debug.LogWarning("StaggeredAnimationManager: No animation template!");
				return;
			}

			Kill();
			BuildStaggeredSequence();

			if (mainSequence != null)
			{
#if UNITY_EDITOR
				if (!Application.isPlaying)
				{
					StartEditorPreview(mainSequence);
				}
#endif
				mainSequence.Play();
				isPlaying = true;
				OnPlaybackStart?.Invoke();
			}
		}

		public void Kill()
		{
			if (mainSequence != null && mainSequence.IsActive())
			{
				mainSequence.Kill();
				mainSequence = null;
			}

#if UNITY_EDITOR
			if (!Application.isPlaying)
			{
				StopEditorPreview();
			}
#endif

			foreach (var tween in activeTweens)
			{
				if (tween != null && tween.IsActive())
				{
					tween.Kill();
				}
			}

			activeTweens.Clear();
			isPlaying = false;
		}

		public void Restart()
		{
			Kill();
			Play();
		}

		#endregion

		#region Sequence Building

		private void BuildStaggeredSequence()
		{
			mainSequence = DOTween.Sequence();
			mainSequence.SetRecyclable(true);
			mainSequence.SetAutoKill(true);
			mainSequence.timeScale = globalTimeScale;

			activeTweens.Clear();

			// Get ordered list
			List<GameObject> orderedObjects = GetOrderedObjects();

			if (playAllSimultaneously)
			{
				// Tất cả play cùng lúc nhưng start times khác nhau
				BuildSimultaneousStagger(orderedObjects);
			}
			else
			{
				// Sequential stagger (chờ animation trước complete)
				BuildSequentialStagger(orderedObjects);
			}

			mainSequence.OnComplete(() =>
			{
				isPlaying = false;
				OnPlaybackComplete?.Invoke();
			});
		}

		private void BuildSimultaneousStagger(List<GameObject> orderedObjects)
		{
			for (int i = 0; i < orderedObjects.Count; i++)
			{
				GameObject obj = orderedObjects[i];
				if (obj == null) continue;

				// Tạo config riêng cho object này
				AnimationConfig config = animationTemplate.Clone();
				config.target = obj;

				// Tạo tween
				Tween tween = AnimationFactory.CreateAnimation(config);
				if (tween != null)
				{
					// Apply settings
					ApplyConfigToTween(tween, config);

					// Delay = staggerDelay * index
					float delay = staggerDelay * i;
					tween.SetDelay(delay);

					// Add events
					int index = i; // Capture for closure
					tween.OnStart(() => OnObjectAnimationStart?.Invoke(index, obj));
					tween.OnComplete(() => OnObjectAnimationComplete?.Invoke(index, obj));

					// Join vào sequence (tất cả play parallel)
					if (i == 0)
					{
						mainSequence.Append(tween);
					}
					else
					{
						mainSequence.Join(tween);
					}

					activeTweens.Add(tween);
				}
			}
		}

		private void BuildSequentialStagger(List<GameObject> orderedObjects)
		{
			for (int i = 0; i < orderedObjects.Count; i++)
			{
				GameObject obj = orderedObjects[i];
				if (obj == null) continue;

				// Add stagger delay
				if (i > 0)
				{
					mainSequence.AppendInterval(staggerDelay);
				}

				// Tạo config riêng cho object này
				AnimationConfig config = animationTemplate.Clone();
				config.target = obj;

				// Tạo tween
				Tween tween = AnimationFactory.CreateAnimation(config);
				if (tween != null)
				{
					// Apply settings
					ApplyConfigToTween(tween, config);

					// Add events
					int index = i; // Capture for closure
					tween.OnStart(() => OnObjectAnimationStart?.Invoke(index, obj));
					tween.OnComplete(() => OnObjectAnimationComplete?.Invoke(index, obj));

					// Append vào sequence
					mainSequence.Append(tween);
					activeTweens.Add(tween);
				}
			}
		}

		private void ApplyConfigToTween(Tween tween, AnimationConfig config)
		{
			// Apply ease
			if (config.useCustomEase && config.customCurve != null)
			{
				tween.SetEase(config.customCurve);
			}
			else
			{
				tween.SetEase(config.easeType);
			}

			// Apply loop (nếu cần)
			if (config.loop)
			{
				tween.SetLoops(config.loopCount, config.loopType);
			}

			// Apply settings
			tween.SetUpdate(config.updateType, config.useUnscaledTime);
			tween.SetAutoKill(config.autoKill);
		}

		private List<GameObject> GetOrderedObjects()
		{
			List<GameObject> result = new List<GameObject>(targetObjects);

			switch (staggerOrder)
			{
				case StaggerOrder.Forward:
					// Giữ nguyên thứ tự
					break;

				case StaggerOrder.Backward:
					result.Reverse();
					break;

				case StaggerOrder.Random:
					// Shuffle
					for (int i = 0; i < result.Count; i++)
					{
						int randomIndex = UnityEngine.Random.Range(i, result.Count);
						GameObject temp = result[i];
						result[i] = result[randomIndex];
						result[randomIndex] = temp;
					}
					break;

				case StaggerOrder.FromCenter:
					result = GetCenterOutOrder();
					break;

				case StaggerOrder.FromEdges:
					result = GetEdgesInOrder();
					break;
			}

			return result;
		}

		private List<GameObject> GetCenterOutOrder()
		{
			List<GameObject> result = new List<GameObject>();
			int count = targetObjects.Count;
			int center = count / 2;

			result.Add(targetObjects[center]);

			for (int i = 1; i <= center; i++)
			{
				// Add left
				if (center - i >= 0)
					result.Add(targetObjects[center - i]);

				// Add right
				if (center + i < count)
					result.Add(targetObjects[center + i]);
			}

			return result;
		}

		private List<GameObject> GetEdgesInOrder()
		{
			List<GameObject> result = new List<GameObject>();
			int left = 0;
			int right = targetObjects.Count - 1;

			while (left <= right)
			{
				result.Add(targetObjects[left]);
				if (left != right)
					result.Add(targetObjects[right]);

				left++;
				right--;
			}

			return result;
		}

		#endregion

		#region Cleanup

		private void ClearAllEvents()
		{
			OnPlaybackStart = null;
			OnPlaybackComplete = null;
			OnObjectAnimationStart = null;
			OnObjectAnimationComplete = null;
		}

		public void ForceCleanup()
		{
			Kill();
			ClearAllEvents();
			activeTweens.Clear();
		}

		#endregion

		#region Helper Methods

		/// <summary>
		/// Add object vào list và play nếu đang chạy
		/// </summary>
		public void AddObject(GameObject obj)
		{
			if (!targetObjects.Contains(obj))
			{
				targetObjects.Add(obj);
			}
		}

		/// <summary>
		/// Remove object khỏi list
		/// </summary>
		public void RemoveObject(GameObject obj)
		{
			targetObjects.Remove(obj);
		}

		/// <summary>
		/// Clear tất cả objects
		/// </summary>
		public void ClearObjects()
		{
			targetObjects.Clear();
		}

		/// <summary>
		/// Set objects từ array
		/// </summary>
		public void SetObjects(GameObject[] objects)
		{
			targetObjects = new List<GameObject>(objects);
		}

		/// <summary>
		/// Set objects từ list
		/// </summary>
		public void SetObjects(List<GameObject> objects)
		{
			targetObjects = new List<GameObject>(objects);
		}

		#endregion

#if UNITY_EDITOR
		#region Editor Preview Helpers

		private static readonly MethodInfo preparePreviewMethod =
			System.Type.GetType("DG.DOTweenEditor.DOTweenEditorPreview, DOTweenEditor")
				?.GetMethod("PrepareTweenForPreview", BindingFlags.Public | BindingFlags.Static);

		private static readonly MethodInfo startPreviewMethod =
			System.Type.GetType("DG.DOTweenEditor.DOTweenEditorPreview, DOTweenEditor")
				?.GetMethod("Start", BindingFlags.Public | BindingFlags.Static);

		private static readonly MethodInfo stopPreviewMethod =
			System.Type.GetType("DG.DOTweenEditor.DOTweenEditorPreview, DOTweenEditor")
				?.GetMethod("Stop", BindingFlags.Public | BindingFlags.Static);

		private void StartEditorPreview(Sequence seq)
		{
			if (seq == null) return;
			try
			{
				if (preparePreviewMethod != null && startPreviewMethod != null)
				{
					preparePreviewMethod.Invoke(null, new object[] { seq });
					startPreviewMethod.Invoke(null, null);
					return;
				}
			}
			catch { /* ignore reflection errors */ }

			// Fallback: manual tick via EditorApplication.update when DOTweenEditor is absent
			if (!editorManualPreviewActive)
			{
				seq.SetUpdate(UpdateType.Manual, true);
				editorManualPreviewActive = true;
				editorLastUpdateTime = EditorApplication.timeSinceStartup;
				EditorApplication.update += EditorManualUpdate;
			}
		}

		private void StopEditorPreview()
		{
			try
			{
				stopPreviewMethod?.Invoke(null, null);
			}
			catch { /* ignore reflection errors */ }

			if (editorManualPreviewActive)
			{
				editorManualPreviewActive = false;
				EditorApplication.update -= EditorManualUpdate;
			}
		}

		private void EditorManualUpdate()
		{
			if (!editorManualPreviewActive || mainSequence == null) return;
			double now = EditorApplication.timeSinceStartup;
			float delta = (float)(now - editorLastUpdateTime);
			editorLastUpdateTime = now;

			mainSequence.ManualUpdate(delta, delta);

			if (!mainSequence.IsActive() || !mainSequence.IsPlaying())
			{
				StopEditorPreview();
			}
		}

		#endregion
#endif

		#region Getters

		public bool IsPlaying => isPlaying;
		public int ObjectCount => targetObjects?.Count ?? 0;

		#endregion
	}

	/// <summary>
	/// Thứ tự play staggered animation
	/// </summary>
	public enum StaggerOrder
	{
		Forward = 0,      // 0 → 1 → 2 → 3
		Backward = 1,     // 3 → 2 → 1 → 0
		Random = 2,       // Random order
		FromCenter = 3,   // Center → Edges (2 → 1 → 3 → 0 → 4)
		FromEdges = 4     // Edges → Center (0 → 4 → 1 → 3 → 2)
	}
}
