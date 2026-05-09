using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace HenryLe.Scripts.AnimationModule
{
	public static class AnimationFactory
	{
		public static Tween CreateAnimation(AnimationConfig config)
		{
			if (config.target == null) return null;

			Transform transform = config.target.transform;
			RectTransform rectTransform = transform as RectTransform;

			switch (config.animationType)
			{
				case AnimationType.Scale:
					return CreateScaleAnimation(transform, config);

				case AnimationType.Move:
					return CreateMoveAnimation(transform, config);

				case AnimationType.LocalMove:
					return CreateLocalMoveAnimation(transform, config);

				case AnimationType.AnchoredMove:
					if (rectTransform != null)
						return CreateAnchoredMoveAnimation(rectTransform, config);
					break;

				case AnimationType.Rotation:
					return CreateRotationAnimation(transform, config);

				case AnimationType.LocalRotation:
					return CreateLocalRotationAnimation(transform, config);

				case AnimationType.Fade:
					return CreateFadeAnimation(config.target, config);

				case AnimationType.FadeCanvasGroup:
					return CreateFadeCanvasGroupAnimation(config.target, config);

				case AnimationType.Color:
					return CreateColorAnimation(config.target, config);

				case AnimationType.Path:
					return CreatePathAnimation(transform, config);

				case AnimationType.ShakePosition:
					return CreateShakePositionAnimation(transform, config);

				case AnimationType.ShakeRotation:
					return CreateShakeRotationAnimation(transform, config);

				case AnimationType.ShakeScale:
					return CreateShakeScaleAnimation(transform, config);

				case AnimationType.PunchPosition:
					return CreatePunchPositionAnimation(transform, config);

				case AnimationType.PunchRotation:
					return CreatePunchRotationAnimation(transform, config);

				case AnimationType.PunchScale:
					return CreatePunchScaleAnimation(transform, config);

				case AnimationType.SizeDelta:
					if (rectTransform != null)
						return CreateSizeDeltaAnimation(rectTransform, config);
					break;

				case AnimationType.Active:
					return CreateActiveAnimation(config.target, config);
			}

			return null;
		}

		/// <summary>
		/// Optimize tween to reduce GC allocation and prevent memory leaks
		/// </summary>
		private static Tween OptimizeTween(Tween tween)
		{
			if (tween != null)
			{
				tween.SetRecyclable(true);
			}
			return tween;
		}

		#region Scale Animations

		private static Tween CreateScaleAnimation(Transform transform, AnimationConfig config)
		{
			transform.localScale = config.fromVector;
			return OptimizeTween(transform.DOScale(config.toVector, config.duration));
		}

		#endregion

		#region Move Animations

		private static Tween CreateMoveAnimation(Transform transform, AnimationConfig config)
		{
			transform.position = config.fromPosition;
			return OptimizeTween(transform.DOMove(config.toPosition, config.duration));
		}

		private static Tween CreateLocalMoveAnimation(Transform transform, AnimationConfig config)
		{
			transform.localPosition = config.fromPosition;
			return OptimizeTween(transform.DOLocalMove(config.toPosition, config.duration));
		}

		private static Tween CreateAnchoredMoveAnimation(RectTransform rectTransform, AnimationConfig config)
		{
			rectTransform.anchoredPosition = config.fromAnchoredPosition;
			return OptimizeTween(rectTransform.DOAnchorPos(config.toAnchoredPosition, config.duration));
		}

		#endregion

		#region Rotation Animations

		private static Tween CreateRotationAnimation(Transform transform, AnimationConfig config)
		{
			transform.eulerAngles = config.fromRotation;
			return OptimizeTween(transform.DORotate(config.toRotation, config.duration));
		}

		private static Tween CreateLocalRotationAnimation(Transform transform, AnimationConfig config)
		{
			transform.localEulerAngles = config.fromRotation;
			return OptimizeTween(transform.DOLocalRotate(config.toRotation, config.duration));
		}

		#endregion

		#region Fade Animations

		private static Tween CreateFadeAnimation(GameObject target, AnimationConfig config)
		{
			// Try SpriteRenderer
			SpriteRenderer spriteRenderer = target.GetComponent<SpriteRenderer>();
			if (spriteRenderer != null)
			{
				Color color = spriteRenderer.color;
				color.a = config.fromValue;
				spriteRenderer.color = color;
				return OptimizeTween(spriteRenderer.DOFade(config.toValue, config.duration));
			}

			// Try Image
			Image image = target.GetComponent<Image>();
			if (image != null)
			{
				Color color = image.color;
				color.a = config.fromValue;
				image.color = color;
				return OptimizeTween(image.DOFade(config.toValue, config.duration));
			}

			// Try Text
			Text text = target.GetComponent<Text>();
			if (text != null)
			{
				Color color = text.color;
				color.a = config.fromValue;
				text.color = color;
				return OptimizeTween(text.DOFade(config.toValue, config.duration));
			}

			return null;
		}

		private static Tween CreateFadeCanvasGroupAnimation(GameObject target, AnimationConfig config)
		{
			CanvasGroup canvasGroup = target.GetComponent<CanvasGroup>();
			if (canvasGroup == null)
			{
				canvasGroup = target.AddComponent<CanvasGroup>();
			}

			canvasGroup.alpha = config.fromValue;
			return OptimizeTween(canvasGroup.DOFade(config.toValue, config.duration));
		}

		#endregion

		#region Color Animations

		private static Tween CreateColorAnimation(GameObject target, AnimationConfig config)
		{
			// Try SpriteRenderer
			SpriteRenderer spriteRenderer = target.GetComponent<SpriteRenderer>();
			if (spriteRenderer != null)
			{
				spriteRenderer.color = config.fromColor;
				return OptimizeTween(spriteRenderer.DOColor(config.toColor, config.duration));
			}

			// Try Image
			Image image = target.GetComponent<Image>();
			if (image != null)
			{
				image.color = config.fromColor;
				return OptimizeTween(image.DOColor(config.toColor, config.duration));
			}

			// Try Text
			Text text = target.GetComponent<Text>();
			if (text != null)
			{
				text.color = config.fromColor;
				return OptimizeTween(text.DOColor(config.toColor, config.duration));
			}

			// Try Material
			Renderer renderer = target.GetComponent<Renderer>();
			if (renderer != null && renderer.material != null)
			{
				renderer.material.color = config.fromColor;
				return OptimizeTween(renderer.material.DOColor(config.toColor, config.duration));
			}

			return null;
		}

		#endregion

		#region Path Animations

		private static Tween CreatePathAnimation(Transform transform, AnimationConfig config)
		{
			if (config.pathPoints == null || config.pathPoints.Length < 2)
				return null;

			DG.Tweening.PathType pathType = config.pathType == PathType.Linear ? DG.Tweening.PathType.Linear : DG.Tweening.PathType.CatmullRom;
			return OptimizeTween(transform.DOPath(config.pathPoints, config.duration, pathType, PathMode.Full3D, config.pathResolution));
		}

		#endregion

		#region Shake Animations

		private static Tween CreateShakePositionAnimation(Transform transform, AnimationConfig config)
		{
			return OptimizeTween(transform.DOShakePosition(config.duration, config.strength, config.vibrato, config.randomness));
		}

		private static Tween CreateShakeRotationAnimation(Transform transform, AnimationConfig config)
		{
			return OptimizeTween(transform.DOShakeRotation(config.duration, config.strength, config.vibrato, config.randomness));
		}

		private static Tween CreateShakeScaleAnimation(Transform transform, AnimationConfig config)
		{
			return OptimizeTween(transform.DOShakeScale(config.duration, config.strength, config.vibrato, config.randomness));
		}

		#endregion

		#region Punch Animations

		private static Tween CreatePunchPositionAnimation(Transform transform, AnimationConfig config)
		{
			return OptimizeTween(transform.DOPunchPosition(config.toVector, config.duration, config.vibrato, config.randomness));
		}

		private static Tween CreatePunchRotationAnimation(Transform transform, AnimationConfig config)
		{
			return OptimizeTween(transform.DOPunchRotation(config.toRotation, config.duration, config.vibrato, config.randomness));
		}

		private static Tween CreatePunchScaleAnimation(Transform transform, AnimationConfig config)
		{
			return OptimizeTween(transform.DOPunchScale(config.toVector, config.duration, config.vibrato, config.randomness));
		}

		#endregion

		#region Size Animations

		private static Tween CreateSizeDeltaAnimation(RectTransform rectTransform, AnimationConfig config)
		{
			rectTransform.sizeDelta = config.fromSize;
			return OptimizeTween(rectTransform.DOSizeDelta(config.toSize, config.duration));
		}

		#endregion

		#region Active Animation

		private static Tween CreateActiveAnimation(GameObject target, AnimationConfig config)
		{
			target.SetActive(false);
			Sequence seq = DOTween.Sequence();
			seq.SetRecyclable(true);
			seq.AppendCallback(() => target.SetActive(true));
			seq.AppendInterval(config.duration);
			return seq;
		}

		#endregion
	}
}
