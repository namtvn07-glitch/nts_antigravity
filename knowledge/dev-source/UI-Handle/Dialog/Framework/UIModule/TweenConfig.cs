using System;
using UnityEngine;

namespace HenryLe.Scripts.UIModule
{
	[Serializable]
	public class TweenConfig
	{
		public UITweenType tweenType;

		public float from;

		public float to;

		public Vector3 mFrom;

		public Vector3 mTo;

		public float duration;

		public float delay;

		public AnimationCurve curve;

		public TweenConfig Clone()
		{
			return null;
		}
	}
}
