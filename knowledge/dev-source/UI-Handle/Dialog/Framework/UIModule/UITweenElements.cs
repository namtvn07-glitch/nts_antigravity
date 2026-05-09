using DG.Tweening;
using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

namespace HenryLe.Scripts.UIModule
{
	public class UITweenElements : MonoBehaviour
	{
		public List<TweenData> tweenDatas;

		public bool playOnAwake;
		public bool UseSequence;

		private void OnEnable()
		{
			if(playOnAwake)
			{
				Play();
            }	

		}

		public void Play()
		{
			foreach(var it in tweenDatas)
			{
                it.SetupData();
            }	

          //  tweenDatas.SetupData();
		}
	}
}
