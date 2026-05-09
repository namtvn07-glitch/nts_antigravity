using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

namespace HenryLe.Scripts.AnimationModule.Editor
{
	[CustomEditor(typeof(StaggeredAnimationManager))]
	public class StaggeredAnimationManagerEditor : UnityEditor.Editor
	{
		private StaggeredAnimationManager manager;
		private SerializedProperty targetObjectsProp;
		private SerializedProperty animationTemplateProp;
		private SerializedProperty staggerDelayProp;
		private SerializedProperty staggerOrderProp;
		private SerializedProperty playAllSimultaneouslyProp;
		private SerializedProperty autoPlayTriggerProp;
		private SerializedProperty globalTimeScaleProp;
		private SerializedProperty killOnDisableProp;

		private bool showTargetObjects = true;
		private bool showAnimationSettings = true;
		private bool showStaggerSettings = true;

		private Color headerColor = new Color(0.3f, 0.5f, 0.8f, 0.3f);
		private Color previewColor = new Color(0.2f, 0.8f, 0.2f, 0.3f);

		private void OnEnable()
		{
			manager = (StaggeredAnimationManager)target;

			targetObjectsProp = serializedObject.FindProperty("targetObjects");
			animationTemplateProp = serializedObject.FindProperty("animationTemplate");
			staggerDelayProp = serializedObject.FindProperty("staggerDelay");
			staggerOrderProp = serializedObject.FindProperty("staggerOrder");
			playAllSimultaneouslyProp = serializedObject.FindProperty("playAllSimultaneously");
			autoPlayTriggerProp = serializedObject.FindProperty("autoPlayTrigger");
			globalTimeScaleProp = serializedObject.FindProperty("globalTimeScale");
			killOnDisableProp = serializedObject.FindProperty("killOnDisable");
		}

		public override void OnInspectorGUI()
		{
			serializedObject.Update();

			DrawHeader();
			EditorGUILayout.Space(5);

			DrawPreviewControls();
			EditorGUILayout.Space(5);

			DrawTargetObjects();
			EditorGUILayout.Space(5);

			DrawAnimationSettings();
			EditorGUILayout.Space(5);

			DrawStaggerSettings();
			EditorGUILayout.Space(5);

			DrawGlobalSettings();

			serializedObject.ApplyModifiedProperties();
		}

		private void DrawHeader()
		{
			EditorGUILayout.BeginVertical(EditorStyles.helpBox);
			GUI.backgroundColor = headerColor;

			GUIStyle titleStyle = new GUIStyle(EditorStyles.boldLabel)
			{
				fontSize = 14,
				alignment = TextAnchor.MiddleCenter
			};

			EditorGUILayout.LabelField("Staggered Animation Manager", titleStyle);
			EditorGUILayout.LabelField($"Objects: {manager.ObjectCount}", EditorStyles.centeredGreyMiniLabel);

			GUI.backgroundColor = Color.white;
			EditorGUILayout.EndVertical();
		}

		private void DrawPreviewControls()
		{
			EditorGUILayout.BeginVertical(EditorStyles.helpBox);
			GUI.backgroundColor = previewColor;

			EditorGUILayout.LabelField("Controls", EditorStyles.boldLabel);
			GUI.backgroundColor = Color.white;

			EditorGUILayout.BeginHorizontal();

			if (GUILayout.Button("Play", GUILayout.Height(30)))
			{
				manager.Play();
			}

			if (GUILayout.Button("Stop", GUILayout.Height(30)))
			{
				manager.Kill();
			}

			if (GUILayout.Button("Restart", GUILayout.Height(30)))
			{
				manager.Restart();
			}

			EditorGUILayout.EndHorizontal();

			if (Application.isPlaying)
			{
				EditorGUILayout.Space(5);
				EditorGUILayout.BeginHorizontal();
				EditorGUILayout.LabelField("Status:", GUILayout.Width(50));
				EditorGUILayout.LabelField(manager.IsPlaying ? "Playing" : "Stopped", EditorStyles.boldLabel);
				EditorGUILayout.EndHorizontal();
			}

			EditorGUILayout.EndVertical();
		}

		private void DrawTargetObjects()
		{
			EditorGUILayout.BeginVertical(EditorStyles.helpBox);

			showTargetObjects = EditorGUILayout.Foldout(showTargetObjects, $"Target Objects ({manager.ObjectCount})", true, EditorStyles.foldoutHeader);

			if (showTargetObjects)
			{
				EditorGUI.indentLevel++;

				// Buttons
				EditorGUILayout.BeginHorizontal();

				if (GUILayout.Button("Add Empty Slot", GUILayout.Height(25)))
				{
					targetObjectsProp.InsertArrayElementAtIndex(targetObjectsProp.arraySize);
				}

				if (GUILayout.Button("Clear All", GUILayout.Height(25)))
				{
					if (EditorUtility.DisplayDialog("Clear All Objects", "Remove all target objects?", "Yes", "No"))
					{
						targetObjectsProp.ClearArray();
					}
				}

				if (GUILayout.Button("Auto-Fill Children", GUILayout.Height(25)))
				{
					AutoFillChildren();
				}

				EditorGUILayout.EndHorizontal();

				EditorGUILayout.Space(5);

				// List
				EditorGUILayout.PropertyField(targetObjectsProp, true);

				EditorGUI.indentLevel--;
			}

			EditorGUILayout.EndVertical();
		}

		private void DrawAnimationSettings()
		{
			EditorGUILayout.BeginVertical(EditorStyles.helpBox);

			showAnimationSettings = EditorGUILayout.Foldout(showAnimationSettings, "Animation Settings", true, EditorStyles.foldoutHeader);

			if (showAnimationSettings)
			{
				EditorGUI.indentLevel++;

				EditorGUILayout.PropertyField(animationTemplateProp);

				if (animationTemplateProp.objectReferenceValue == null)
				{
					EditorGUILayout.HelpBox("Set an Animation Template to define what animation will play on each object.", MessageType.Info);
				}
				else
				{
					AnimationConfig template = (animationTemplateProp.serializedObject.targetObject as StaggeredAnimationManager).animationTemplate;
					if (template != null)
					{
						EditorGUILayout.Space(3);
						EditorGUILayout.LabelField("Template Preview:", EditorStyles.boldLabel);
						EditorGUILayout.LabelField($"Type: {template.animationType}");
						EditorGUILayout.LabelField($"Duration: {template.duration}s");
						EditorGUILayout.LabelField($"Ease: {template.easeType}");
					}
				}

				EditorGUI.indentLevel--;
			}

			EditorGUILayout.EndVertical();
		}

		private void DrawStaggerSettings()
		{
			EditorGUILayout.BeginVertical(EditorStyles.helpBox);

			showStaggerSettings = EditorGUILayout.Foldout(showStaggerSettings, "Stagger Settings", true, EditorStyles.foldoutHeader);

			if (showStaggerSettings)
			{
				EditorGUI.indentLevel++;

				EditorGUILayout.PropertyField(staggerDelayProp, new GUIContent("Delay Between Objects"));
				EditorGUILayout.PropertyField(staggerOrderProp, new GUIContent("Play Order"));
				EditorGUILayout.PropertyField(playAllSimultaneouslyProp, new GUIContent("Play All Simultaneously"));

				// Help box
				if (playAllSimultaneouslyProp.boolValue)
				{
					EditorGUILayout.HelpBox("All animations start at different times but play simultaneously (overlapping).", MessageType.Info);
				}
				else
				{
					EditorGUILayout.HelpBox("Animations play sequentially - each waits for the previous to complete before starting.", MessageType.Info);
				}

				// Calculate total duration
				if (manager.animationTemplate != null && manager.ObjectCount > 0)
				{
					float totalDuration = CalculateTotalDuration();
					EditorGUILayout.Space(5);
					EditorGUILayout.LabelField($"Estimated Total Duration: {totalDuration:F2}s", EditorStyles.boldLabel);
				}

				// Order visualization
				EditorGUILayout.Space(5);
				DrawOrderVisualization();

				EditorGUI.indentLevel--;
			}

			EditorGUILayout.EndVertical();
		}

		private void DrawGlobalSettings()
		{
			EditorGUILayout.BeginVertical(EditorStyles.helpBox);
			EditorGUILayout.LabelField("Global Settings", EditorStyles.boldLabel);

			EditorGUILayout.PropertyField(autoPlayTriggerProp);
			EditorGUILayout.PropertyField(globalTimeScaleProp);
			EditorGUILayout.PropertyField(killOnDisableProp);

			EditorGUILayout.EndVertical();
		}

		private void DrawOrderVisualization()
		{
			StaggerOrder order = (StaggerOrder)staggerOrderProp.enumValueIndex;

			EditorGUILayout.LabelField("Play Order Preview:", EditorStyles.miniLabel);

			string visualization = "";
			switch (order)
			{
				case StaggerOrder.Forward:
					visualization = "0 → 1 → 2 → 3 → 4 →";
					break;
				case StaggerOrder.Backward:
					visualization = "4 → 3 → 2 → 1 → 0 →";
					break;
				case StaggerOrder.Random:
					visualization = "Random (eg: 2 → 0 → 4 → 1 → 3)";
					break;
				case StaggerOrder.FromCenter:
					visualization = "Center out (eg: 2 → 1 → 3 → 0 → 4)";
					break;
				case StaggerOrder.FromEdges:
					visualization = "Edges in (eg: 0 → 4 → 1 → 3 → 2)";
					break;
			}

			GUIStyle style = new GUIStyle(EditorStyles.helpBox);
			style.normal.textColor = Color.cyan;
			style.alignment = TextAnchor.MiddleCenter;
			style.fontSize = 11;

			EditorGUILayout.LabelField(visualization, style);
		}

		private float CalculateTotalDuration()
		{
			if (manager.animationTemplate == null || manager.ObjectCount == 0)
				return 0f;

			float animDuration = manager.animationTemplate.duration;
			float delay = manager.staggerDelay;
			int count = manager.ObjectCount;

			if (manager.playAllSimultaneously)
			{
				// Longest = last object start time + animation duration
				return (delay * (count - 1)) + animDuration;
			}
			else
			{
				// Sequential = all animations + delays
				return (animDuration * count) + (delay * (count - 1));
			}
		}

		private void AutoFillChildren()
		{
			Transform transform = manager.transform;
			targetObjectsProp.ClearArray();

			for (int i = 0; i < transform.childCount; i++)
			{
				targetObjectsProp.InsertArrayElementAtIndex(i);
				targetObjectsProp.GetArrayElementAtIndex(i).objectReferenceValue = transform.GetChild(i).gameObject;
			}

			serializedObject.ApplyModifiedProperties();
			EditorUtility.SetDirty(manager);
		}
	}
}
