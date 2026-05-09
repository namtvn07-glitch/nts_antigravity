using UnityEngine;
using System.Collections.Generic;
using DG.Tweening;
using HenryLe.Scripts.AnimationModule;

namespace HenryLe.Scripts.AnimationModule.Examples
{
	/// <summary>
	/// Example script demonstrating staggered animations
	/// </summary>
	public class StaggeredAnimationExample : MonoBehaviour
	{
		[Header("References")]
		public List<GameObject> items;

		[Header("Settings")]
		public float staggerDelay = 0.1f;

		// Example Methods - Use Context Menu (Right-click in Inspector)
		[ContextMenu("1. Fade In")]
		public void Example_FadeIn()
		{
			items.StaggeredFadeIn(0.5f, staggerDelay);
		}

		[ContextMenu("2. Fade Out")]
		public void Example_FadeOut()
		{
			items.StaggeredFadeOut(0.5f, staggerDelay);
		}

		[ContextMenu("3. Pop In")]
		public void Example_PopIn()
		{
			items.StaggeredPopIn(0.6f, staggerDelay);
		}

		[ContextMenu("4. Pop Out")]
		public void Example_PopOut()
		{
			items.StaggeredPopOut(0.4f, staggerDelay);
		}

		[ContextMenu("5. Scale Bounce")]
		public void Example_ScaleBounce()
		{
			items.DOStaggeredScale(Vector3.one * 1.2f, 0.5f, staggerDelay, Ease.OutBounce);
		}

		[ContextMenu("6. From Center")]
		public void Example_FromCenter()
		{
			items.DOStaggeredFromCenter((obj, delay) =>
			{
				obj.transform.DOPunchScale(Vector3.one * 0.3f, 0.5f)
					.SetDelay(delay)
					.SetEase(Ease.OutElastic);
			}, staggerDelay);
		}

		[ContextMenu("7. From Edges")]
		public void Example_FromEdges()
		{
			items.DOStaggeredFromEdges((obj, delay) =>
			{
				obj.transform.DOScale(Vector3.one * 1.1f, 0.3f)
					.SetDelay(delay)
					.SetEase(Ease.OutQuad)
					.SetLoops(2, LoopType.Yoyo);
			}, staggerDelay);
		}

		[ContextMenu("8. Custom Wave Effect")]
		public void Example_WaveEffect()
		{
			for (int i = 0; i < items.Count; i++)
			{
				GameObject item = items[i];
				float delay = i * staggerDelay;

				// Wave: Move up then down
				item.transform.DOMoveY(item.transform.position.y + 0.5f, 0.3f)
					.SetDelay(delay)
					.SetEase(Ease.OutQuad)
					.SetLoops(2, LoopType.Yoyo);
			}
		}

		[ContextMenu("9. Color Rainbow")]
		public void Example_ColorRainbow()
		{
			Color[] rainbowColors = new Color[]
			{
				Color.red,
				new Color(1f, 0.5f, 0f), // Orange
				Color.yellow,
				Color.green,
				Color.cyan,
				Color.blue,
				new Color(0.5f, 0f, 1f)  // Purple
			};

			for (int i = 0; i < items.Count; i++)
			{
				GameObject item = items[i];
				UnityEngine.UI.Image img = item.GetComponent<UnityEngine.UI.Image>();
				if (img != null)
				{
					Color targetColor = rainbowColors[i % rainbowColors.Length];
					float delay = i * staggerDelay;

					img.DOColor(targetColor, 0.5f)
						.SetDelay(delay)
						.SetEase(Ease.OutQuad);
				}
			}
		}

		[ContextMenu("10. Spiral Animation")]
		public void Example_SpiralAnimation()
		{
			for (int i = 0; i < items.Count; i++)
			{
				GameObject item = items[i];
				float delay = i * staggerDelay;

				// Rotate + Scale
				Sequence seq = DOTween.Sequence();
				seq.SetDelay(delay);
				seq.Append(item.transform.DOScale(Vector3.zero, 0f));
				seq.Append(item.transform.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBack));
				seq.Join(item.transform.DORotate(new Vector3(0, 0, 360), 0.5f, RotateMode.FastBeyond360));
			}
		}

		[ContextMenu("Reset All")]
		public void ResetAll()
		{
			foreach (var item in items)
			{
				if (item != null)
				{
					item.transform.localScale = Vector3.one;
					item.transform.localRotation = Quaternion.identity;

					var cg = item.GetComponent<CanvasGroup>();
					if (cg != null) cg.alpha = 1f;

					var img = item.GetComponent<UnityEngine.UI.Image>();
					if (img != null) img.color = Color.white;
				}
			}

			DOTween.KillAll();
		}

		private void OnValidate()
		{
			// Auto-fill children if items list is empty
			if (items.Count == 0)
			{
				for (int i = 0; i < transform.childCount; i++)
				{
					items.Add(transform.GetChild(i).gameObject);
				}
			}
		}
	}
}
