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
	public class AnimationManager : MonoBehaviour
	{
		[Header("Auto Play")]
		public AutoPlayTrigger autoPlayTrigger = AutoPlayTrigger.None;

		[Header("Play Mode")]
		public PlayMode playMode = PlayMode.Sequential;

		[Header("Animations")]
		public List<AnimationConfig> animations = new List<AnimationConfig>();

		[Header("Global Settings")]
		public float globalTimeScale = 1f;
		public bool killOnDisable = true;

		// Runtime
		private Sequence mainSequence;
		private bool isPlaying = false;
		private bool isPaused = false;
		private float currentProgress = 0f;
#if UNITY_EDITOR
		private bool editorManualPreviewActive = false;
		private double editorLastUpdateTime = 0f;
#endif

		// Events
		public event Action OnPlaybackStart;
		public event Action OnPlaybackComplete;
		public event Action<float> OnProgressUpdate;

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
			if (isPlaying && !isPaused)
			{
				Restart();
				return;
			}

			if (isPaused)
			{
				Resume();
				return;
			}

			Kill();
			ResetToFromValues(); // Reset về giá trị FROM trước khi play
			BuildSequence();

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

		public void Pause()
		{
			if (mainSequence != null && isPlaying && !isPaused)
			{
				mainSequence.Pause();
				isPaused = true;
			}
		}

		public void Resume()
		{
			if (mainSequence != null && isPaused)
			{
				mainSequence.Play();
				isPaused = false;
			}
		}

		public void Stop()
		{
			if (mainSequence != null)
			{
				mainSequence.Pause();
				mainSequence.Goto(0);
				isPlaying = false;
				isPaused = false;
				currentProgress = 0f;
			}
		}

		public void Restart()
		{
			Kill();
			Play();
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

			if (animations != null)
			{
				foreach (var anim in animations)
				{
					if (anim != null && anim.activeTween != null && anim.activeTween.IsActive())
					{
						anim.activeTween.Kill();
						anim.activeTween = null;
					}
				}
			}

			isPlaying = false;
			isPaused = false;
			currentProgress = 0f;
		}

		public void Reverse()
		{
			if (mainSequence != null)
			{
				mainSequence.PlayBackwards();
			}
		}

		public void SetProgress(float progress)
		{
			if (mainSequence != null)
			{
				progress = Mathf.Clamp01(progress);
				float duration = mainSequence.Duration();
				mainSequence.Goto(duration * progress);
				currentProgress = progress;
				OnProgressUpdate?.Invoke(progress);
			}
		}

		public void SetTimeScale(float timeScale)
		{
			globalTimeScale = timeScale;
			if (mainSequence != null)
			{
				mainSequence.timeScale = timeScale;
			}
		}

		#endregion

		#region Sequence Building

		private void BuildSequence()
		{
			mainSequence = DOTween.Sequence();

			// Set recyclable to reduce GC allocation
			mainSequence.SetRecyclable(true);
			mainSequence.SetAutoKill(true);

			if (playMode == PlayMode.Sequential)
			{
				BuildSequentialSequence();
			}
			else if (playMode == PlayMode.Parallel)
			{
				BuildParallelSequence();
			}
			else if (playMode == PlayMode.Mixed)
			{
				BuildMixedSequence();
			}

			mainSequence.timeScale = globalTimeScale;
			mainSequence.OnUpdate(() =>
			{
				if (mainSequence != null)
				{
					currentProgress = mainSequence.ElapsedPercentage();
					OnProgressUpdate?.Invoke(currentProgress);
				}
			});
			mainSequence.OnComplete(() =>
			{
				isPlaying = false;
				isPaused = false;
				OnPlaybackComplete?.Invoke();
			});
		}

		private void BuildSequentialSequence()
		{
			foreach (var anim in animations)
			{
				if (anim.target == null) continue;

				Tween tween = CreateTween(anim);
				if (tween != null)
				{
					if (anim.delay > 0)
					{
						mainSequence.AppendInterval(anim.delay);
					}
					mainSequence.Append(tween);
				}
			}
		}

		private void BuildParallelSequence()
		{
			foreach (var anim in animations)
			{
				if (anim.target == null) continue;

				Tween tween = CreateTween(anim);
				if (tween != null)
				{
					if (anim.delay > 0)
					{
						tween.SetDelay(anim.delay);
					}
					mainSequence.Join(tween);
				}
			}
		}

		private void BuildMixedSequence()
		{
			foreach (var anim in animations)
			{
				if (anim.target == null) continue;

				Tween tween = CreateTween(anim);
				if (tween != null)
				{
					if (anim.sequenceDelay > 0)
					{
						mainSequence.AppendInterval(anim.sequenceDelay);
					}

					if (anim.isParallel)
					{
						if (anim.delay > 0)
						{
							tween.SetDelay(anim.delay);
						}
						mainSequence.Join(tween);
					}
					else
					{
						if (anim.delay > 0)
						{
							mainSequence.AppendInterval(anim.delay);
						}
						mainSequence.Append(tween);
					}
				}
			}
		}

		private Tween CreateTween(AnimationConfig config)
		{
			Tween tween = AnimationFactory.CreateAnimation(config);

			if (tween != null)
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

				// Apply loop
				if (config.loop)
				{
					tween.SetLoops(config.loopCount, config.loopType);
				}

				// Apply settings
				tween.SetUpdate(config.updateType, config.useUnscaledTime);
				tween.SetAutoKill(config.autoKill);

				// Apply events
				tween.OnStart(() => config.onStart?.Invoke());
				tween.OnUpdate(() => config.onUpdate?.Invoke());
				tween.OnComplete(() => config.onComplete?.Invoke());
				tween.OnKill(() => config.onKill?.Invoke());

				config.activeTween = tween;
			}

			return tween;
		}

		#endregion

		#region Getters

		public bool IsPlaying => isPlaying;
		public bool IsPaused => isPaused;
		public float CurrentProgress => currentProgress;
		public float TotalDuration => mainSequence?.Duration() ?? 0f;

		#endregion

		#region Reset Methods

		/// <summary>
		/// Reset tất cả targets về giá trị FROM của animation
		/// </summary>
		public void ResetToFromValues()
		{
			if (animations == null) return;

			foreach (var anim in animations)
			{
				if (anim == null || anim.target == null) continue;

				Transform transform = anim.target.transform;
				RectTransform rectTransform = transform as RectTransform;

				switch (anim.animationType)
				{
					case AnimationType.Scale:
						transform.localScale = anim.fromVector;
						break;

					case AnimationType.Move:
						transform.position = anim.fromPosition;
						break;

					case AnimationType.LocalMove:
						transform.localPosition = anim.fromPosition;
						break;

					case AnimationType.AnchoredMove:
						if (rectTransform != null)
							rectTransform.anchoredPosition = anim.fromAnchoredPosition;
						break;

					case AnimationType.Rotation:
						transform.eulerAngles = anim.fromRotation;
						break;

					case AnimationType.LocalRotation:
						transform.localEulerAngles = anim.fromRotation;
						break;

					case AnimationType.Fade:
						ResetFadeValue(anim.target, anim.fromValue);
						break;

					case AnimationType.FadeCanvasGroup:
						var cg = anim.target.GetComponent<CanvasGroup>();
						if (cg != null) cg.alpha = anim.fromValue;
						break;

					case AnimationType.Color:
						ResetColorValue(anim.target, anim.fromColor);
						break;

					case AnimationType.SizeDelta:
						if (rectTransform != null)
							rectTransform.sizeDelta = anim.fromSize;
						break;

					// Path, Shake, Punch không cần reset vì không có FROM value cụ thể
				}
			}
		}

		private void ResetFadeValue(GameObject target, float alpha)
		{
			var spriteRenderer = target.GetComponent<UnityEngine.SpriteRenderer>();
			if (spriteRenderer != null)
			{
				var color = spriteRenderer.color;
				color.a = alpha;
				spriteRenderer.color = color;
				return;
			}

			var image = target.GetComponent<UnityEngine.UI.Image>();
			if (image != null)
			{
				var color = image.color;
				color.a = alpha;
				image.color = color;
				return;
			}

			var text = target.GetComponent<UnityEngine.UI.Text>();
			if (text != null)
			{
				var color = text.color;
				color.a = alpha;
				text.color = color;
			}
		}

		private void ResetColorValue(GameObject target, Color color)
		{
			var spriteRenderer = target.GetComponent<UnityEngine.SpriteRenderer>();
			if (spriteRenderer != null)
			{
				spriteRenderer.color = color;
				return;
			}

			var image = target.GetComponent<UnityEngine.UI.Image>();
			if (image != null)
			{
				image.color = color;
				return;
			}

			var text = target.GetComponent<UnityEngine.UI.Text>();
			if (text != null)
			{
				text.color = color;
				return;
			}

			var renderer = target.GetComponent<Renderer>();
			if (renderer != null && renderer.material != null)
			{
				renderer.material.color = color;
			}
		}

		#endregion

		#region Cleanup

		private void ClearAllEvents()
		{
			// Clear manager events
			OnPlaybackStart = null;
			OnPlaybackComplete = null;
			OnProgressUpdate = null;

			// Clear animation config events
			if (animations != null)
			{
				foreach (var anim in animations)
				{
					if (anim != null)
					{
						anim.onStart?.RemoveAllListeners();
						anim.onUpdate?.RemoveAllListeners();
						anim.onComplete?.RemoveAllListeners();
						anim.onKill?.RemoveAllListeners();
					}
				}
			}
		}

		/// <summary>
		/// Force cleanup all tweens and sequences. Call this if you suspect memory leaks.
		/// </summary>
		public void ForceCleanup()
		{
			Kill();
			ClearAllEvents();

			// Clear animation references
			if (animations != null)
			{
				foreach (var anim in animations)
				{
					if (anim != null)
					{
						anim.activeTween = null;
					}
				}
			}
		}

		#endregion

#if UNITY_EDITOR
		#region Editor Preview Helpers

		// Use reflection so this compiles even if DOTweenEditor assembly is not present
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

			// Fallback: manual tick via EditorApplication.update (for projects without DOTweenEditor assembly)
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
			// ManualUpdate will advance tweens and trigger OnUpdate, updating currentProgress
			mainSequence.ManualUpdate(delta, delta);

			// Auto stop when finished
			if (!mainSequence.IsActive() || !mainSequence.IsPlaying())
			{
				StopEditorPreview();
			}
		}

		#endregion
#endif
	}
}
