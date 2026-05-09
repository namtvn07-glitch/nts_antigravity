using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;

namespace HenryLe.Scripts.AnimationModule
{
	[Serializable]
	public class AnimationConfig
	{
		[Header("Target")]
		public GameObject target;

		[Header("Animation Settings")]
		public AnimationType animationType = AnimationType.Scale;
		public bool useCustomEase = false;
		public Ease easeType = Ease.OutQuad;
		public AnimationCurve customCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

		[Header("Timing")]
		public float duration = 1f;
		public float delay = 0f;

		[Header("Values - Scale/Fade")]
		public float fromValue = 0f;
		public float toValue = 1f;
		public Vector3 fromVector = Vector3.zero;
		public Vector3 toVector = Vector3.one;

		[Header("Values - Move/Position")]
		public Vector3 fromPosition;
		public Vector3 toPosition;
		public Vector2 fromAnchoredPosition;
		public Vector2 toAnchoredPosition;

		[Header("Values - Rotation")]
		public Vector3 fromRotation;
		public Vector3 toRotation;

		[Header("Values - Color")]
		public Color fromColor = Color.white;
		public Color toColor = Color.white;

		[Header("Values - Size")]
		public Vector2 fromSize;
		public Vector2 toSize;

		[Header("Path Settings")]
		public Vector3[] pathPoints;
		public PathType pathType = PathType.Linear;
		public int pathResolution = 10;

		[Header("Shake/Punch Settings")]
		public float strength = 1f;
		public int vibrato = 10;
		public float randomness = 90f;

		[Header("Loop Settings")]
		public bool loop = false;
		public int loopCount = -1; // -1 = infinite
		public LoopType loopType = LoopType.Restart;

		[Header("Advanced")]
		public bool useUnscaledTime = false;
		public bool autoKill = true;
		public UpdateType updateType = UpdateType.Normal;

		[Header("Sequence")]
		public bool isParallel = false; // Chạy song song với animation trước
		public float sequenceDelay = 0f; // Delay riêng trong sequence

		[Header("Events")]
		public UnityEvent onStart;
		public UnityEvent onUpdate;
		public UnityEvent onComplete;
		public UnityEvent onKill;

		// Runtime
		[NonSerialized] public Tween activeTween;

		public AnimationConfig Clone()
		{
			return (AnimationConfig)MemberwiseClone();
		}

		public Ease GetEase()
		{
			return useCustomEase ? Ease.INTERNAL_Custom : easeType;
		}
	}
}
