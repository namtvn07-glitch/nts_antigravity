using System.Collections.Generic;
using UnityEngine;

namespace HenryLe.Scripts.AnimationModule
{
	[CreateAssetMenu(fileName = "AnimationPreset", menuName = "Henry/Animation/Animation Preset", order = 1)]
	public class AnimationPresetSO : ScriptableObject
	{
		[Header("Preset Info")]
		public string presetName;
		[TextArea(3, 5)]
		public string description;
		public string category = "Default";

		[Header("Settings")]
		public AutoPlayTrigger autoPlayTrigger = AutoPlayTrigger.None;
		public PlayMode playMode = PlayMode.Sequential;
		public float globalTimeScale = 1f;

		[Header("Animations")]
		public List<AnimationConfig> animations = new List<AnimationConfig>();

		public void ApplyToManager(AnimationManager manager)
		{
			if (manager == null) return;

			manager.autoPlayTrigger = autoPlayTrigger;
			manager.playMode = playMode;
			manager.globalTimeScale = globalTimeScale;

			manager.animations.Clear();
			foreach (var anim in animations)
			{
				manager.animations.Add(anim.Clone());
			}
		}

		public void SaveFromManager(AnimationManager manager)
		{
			if (manager == null) return;

			autoPlayTrigger = manager.autoPlayTrigger;
			playMode = manager.playMode;
			globalTimeScale = manager.globalTimeScale;

			animations.Clear();
			foreach (var anim in manager.animations)
			{
				animations.Add(anim.Clone());
			}
		}
	}
}
