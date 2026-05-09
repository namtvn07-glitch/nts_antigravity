using DG.Tweening;
using UnityEngine;

namespace HenryLe.Scripts.UIModule
{
	public class UITweenElement : MonoBehaviour
	{
		public TweenData tweenData;

		public bool playOnAwake;

		private void OnEnable()
		{
			if(playOnAwake)
			{
				Play();
            }	

		}

		public void Play()
		{
			tweenData.SetupData();
		}
	}
}
