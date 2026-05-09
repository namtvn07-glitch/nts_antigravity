using System;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace HenryLe.Scripts.AnimationModule
{
	/// <summary>
	/// Extension methods để dễ dàng tạo staggered animations
	/// </summary>
	public static class AnimationExtensions
	{
		/// <summary>
		/// Play staggered scale animation cho list objects
		/// </summary>
		public static Sequence DOStaggeredScale(this IEnumerable<GameObject> objects, Vector3 to, float duration, float staggerDelay = 0.1f, Ease ease = Ease.OutQuad)
		{
			Sequence seq = DOTween.Sequence();
			seq.SetRecyclable(true);

			float delay = 0f;
			foreach (var obj in objects)
			{
				if (obj != null)
				{
					var tween = obj.transform.DOScale(to, duration)
						.SetEase(ease)
						.SetDelay(delay)
						.SetRecyclable(true);

					seq.Join(tween);
					delay += staggerDelay;
				}
			}

			return seq;
		}

		/// <summary>
		/// Play staggered fade animation cho list objects
		/// </summary>
		public static Sequence DOStaggeredFade(this IEnumerable<GameObject> objects, float to, float duration, float staggerDelay = 0.1f, Ease ease = Ease.OutQuad)
		{
			Sequence seq = DOTween.Sequence();
			seq.SetRecyclable(true);

			float delay = 0f;
			foreach (var obj in objects)
			{
				if (obj != null)
				{
					CanvasGroup canvasGroup = obj.GetComponent<CanvasGroup>();
					if (canvasGroup == null)
						canvasGroup = obj.AddComponent<CanvasGroup>();

					var tween = canvasGroup.DOFade(to, duration)
						.SetEase(ease)
						.SetDelay(delay)
						.SetRecyclable(true);

					seq.Join(tween);
					delay += staggerDelay;
				}
			}

			return seq;
		}

		/// <summary>
		/// Play staggered move animation cho list objects
		/// </summary>
		public static Sequence DOStaggeredMove(this IEnumerable<GameObject> objects, Vector3[] positions, float duration, float staggerDelay = 0.1f, Ease ease = Ease.OutQuad)
		{
			Sequence seq = DOTween.Sequence();
			seq.SetRecyclable(true);

			float delay = 0f;
			int index = 0;

			foreach (var obj in objects)
			{
				if (obj != null && index < positions.Length)
				{
					var tween = obj.transform.DOMove(positions[index], duration)
						.SetEase(ease)
						.SetDelay(delay)
						.SetRecyclable(true);

					seq.Join(tween);
					delay += staggerDelay;
					index++;
				}
			}

			return seq;
		}

		/// <summary>
		/// Play staggered anchored position animation cho list UI objects
		/// </summary>
		public static Sequence DOStaggeredAnchorPos(this IEnumerable<GameObject> objects, Vector2[] positions, float duration, float staggerDelay = 0.1f, Ease ease = Ease.OutQuad)
		{
			Sequence seq = DOTween.Sequence();
			seq.SetRecyclable(true);

			float delay = 0f;
			int index = 0;

			foreach (var obj in objects)
			{
				if (obj != null && index < positions.Length)
				{
					RectTransform rect = obj.GetComponent<RectTransform>();
					if (rect != null)
					{
						var tween = rect.DOAnchorPos(positions[index], duration)
							.SetEase(ease)
							.SetDelay(delay)
							.SetRecyclable(true);

						seq.Join(tween);
					}

					delay += staggerDelay;
					index++;
				}
			}

			return seq;
		}

		/// <summary>
		/// Quick helper: Fade in list objects với stagger
		/// </summary>
		public static Sequence StaggeredFadeIn(this IEnumerable<GameObject> objects, float duration = 0.5f, float staggerDelay = 0.1f)
		{
			// Set all alpha to 0 first
			foreach (var obj in objects)
			{
				if (obj != null)
				{
					CanvasGroup canvasGroup = obj.GetComponent<CanvasGroup>();
					if (canvasGroup == null)
						canvasGroup = obj.AddComponent<CanvasGroup>();
					canvasGroup.alpha = 0f;
				}
			}

			return objects.DOStaggeredFade(1f, duration, staggerDelay, Ease.OutQuad);
		}

		/// <summary>
		/// Quick helper: Fade out list objects với stagger
		/// </summary>
		public static Sequence StaggeredFadeOut(this IEnumerable<GameObject> objects, float duration = 0.5f, float staggerDelay = 0.1f)
		{
			return objects.DOStaggeredFade(0f, duration, staggerDelay, Ease.InQuad);
		}

		/// <summary>
		/// Quick helper: Scale pop in với stagger
		/// </summary>
		public static Sequence StaggeredPopIn(this IEnumerable<GameObject> objects, float duration = 0.5f, float staggerDelay = 0.1f)
		{
			// Set all scale to 0 first
			foreach (var obj in objects)
			{
				if (obj != null)
				{
					obj.transform.localScale = Vector3.zero;
				}
			}

			return objects.DOStaggeredScale(Vector3.one, duration, staggerDelay, Ease.OutBack);
		}

		/// <summary>
		/// Quick helper: Scale pop out với stagger
		/// </summary>
		public static Sequence StaggeredPopOut(this IEnumerable<GameObject> objects, float duration = 0.5f, float staggerDelay = 0.1f)
		{
			return objects.DOStaggeredScale(Vector3.zero, duration, staggerDelay, Ease.InBack);
		}

		/// <summary>
		/// Play animation với delay từ center ra ngoài
		/// </summary>
		public static Sequence DOStaggeredFromCenter<T>(this List<T> objects, Action<T, float> animateCallback, float staggerDelay = 0.1f) where T : Component
		{
			Sequence seq = DOTween.Sequence();
			seq.SetRecyclable(true);

			int center = objects.Count / 2;
			float delay = 0f;

			// Center first
			if (objects.Count > 0 && objects[center] != null)
			{
				animateCallback(objects[center], delay);
			}

			// Expand outwards
			for (int i = 1; i <= center; i++)
			{
				delay += staggerDelay;

				// Left
				if (center - i >= 0 && objects[center - i] != null)
				{
					animateCallback(objects[center - i], delay);
				}

				// Right
				if (center + i < objects.Count && objects[center + i] != null)
				{
					animateCallback(objects[center + i], delay);
				}
			}

			return seq;
		}

		/// <summary>
		/// Play animation với delay từ edges vào center
		/// </summary>
		public static Sequence DOStaggeredFromEdges<T>(this List<T> objects, Action<T, float> animateCallback, float staggerDelay = 0.1f) where T : Component
		{
			Sequence seq = DOTween.Sequence();
			seq.SetRecyclable(true);

			int left = 0;
			int right = objects.Count - 1;
			float delay = 0f;

			while (left <= right)
			{
				if (objects[left] != null)
				{
					animateCallback(objects[left], delay);
				}

				if (left != right && objects[right] != null)
				{
					animateCallback(objects[right], delay);
				}

				left++;
				right--;
				delay += staggerDelay;
			}

			return seq;
		}

		/// <summary>
		/// Play animation với delay từ center ra ngoài (GameObject overload)
		/// </summary>
		public static Sequence DOStaggeredFromCenter(this List<GameObject> objects, Action<GameObject, float> animateCallback, float staggerDelay = 0.1f)
		{
			Sequence seq = DOTween.Sequence();
			seq.SetRecyclable(true);

			int center = objects.Count / 2;
			float delay = 0f;

			// Center first
			if (objects.Count > 0 && objects[center] != null)
			{
				animateCallback(objects[center], delay);
			}

			// Expand outwards
			for (int i = 1; i <= center; i++)
			{
				delay += staggerDelay;

				// Left
				if (center - i >= 0 && objects[center - i] != null)
				{
					animateCallback(objects[center - i], delay);
				}

				// Right
				if (center + i < objects.Count && objects[center + i] != null)
				{
					animateCallback(objects[center + i], delay);
				}
			}

			return seq;
		}

		/// <summary>
		/// Play animation với delay từ edges vào center (GameObject overload)
		/// </summary>
		public static Sequence DOStaggeredFromEdges(this List<GameObject> objects, Action<GameObject, float> animateCallback, float staggerDelay = 0.1f)
		{
			Sequence seq = DOTween.Sequence();
			seq.SetRecyclable(true);

			int left = 0;
			int right = objects.Count - 1;
			float delay = 0f;

			while (left <= right)
			{
				if (objects[left] != null)
				{
					animateCallback(objects[left], delay);
				}

				if (left != right && objects[right] != null)
				{
					animateCallback(objects[right], delay);
				}

				left++;
				right--;
				delay += staggerDelay;
			}

			return seq;
		}
	}
}
