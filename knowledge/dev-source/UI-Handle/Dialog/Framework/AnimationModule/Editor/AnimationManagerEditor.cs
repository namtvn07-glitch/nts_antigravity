using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

namespace HenryLe.Scripts.AnimationModule.Editor
{
	[CustomEditor(typeof(AnimationManager))]
	public class AnimationManagerEditor : UnityEditor.Editor
	{
		private AnimationManager manager;
		private SerializedProperty autoPlayTriggerProp;
		private SerializedProperty playModeProp;
		private SerializedProperty animationsProp;
		private SerializedProperty globalTimeScaleProp;
		private SerializedProperty killOnDisableProp;

		private bool showGlobalSettings = true;
		private bool showAnimations = true;
		private Vector2 scrollPosition;

		// Preview
		private Dictionary<int, bool> animationFoldouts = new Dictionary<int, bool>();
		private bool isPreviewMode = false;
		private float previewProgress = 0f;

		// Colors
		private Color headerColor = new Color(0.3f, 0.5f, 0.8f, 0.3f);
		private Color previewColor = new Color(0.2f, 0.8f, 0.2f, 0.3f);

		private void OnEnable()
		{
			manager = (AnimationManager)target;

			autoPlayTriggerProp = serializedObject.FindProperty("autoPlayTrigger");
			playModeProp = serializedObject.FindProperty("playMode");
			animationsProp = serializedObject.FindProperty("animations");
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

			DrawPresetControls();
			EditorGUILayout.Space(5);

			DrawGlobalSettings();
			EditorGUILayout.Space(5);

			DrawAnimationsList();
			EditorGUILayout.Space(5);

			DrawTimelineButton();

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

			EditorGUILayout.LabelField("Animation Manager", titleStyle);

			GUI.backgroundColor = Color.white;
			EditorGUILayout.EndVertical();
		}

		private void DrawPreviewControls()
		{
			EditorGUILayout.BeginVertical(EditorStyles.helpBox);
			GUI.backgroundColor = previewColor;

			EditorGUILayout.LabelField("Preview Controls", EditorStyles.boldLabel);

			GUI.backgroundColor = Color.white;

			if (!Application.isPlaying)
			{
				EditorGUILayout.BeginHorizontal();

				if (GUILayout.Button("Preview", GUILayout.Height(30)))
				{
					PreviewAnimation();
				}

				if (GUILayout.Button("Stop Preview", GUILayout.Height(30)))
				{
					StopPreview();
				}

				EditorGUILayout.EndHorizontal();

				if (isPreviewMode)
				{
					EditorGUILayout.Space(5);
					float newProgress = EditorGUILayout.Slider("Progress", previewProgress, 0f, 1f);
					if (Mathf.Abs(newProgress - previewProgress) > 0.001f)
					{
						previewProgress = newProgress;
						ScrubPreview(previewProgress);
					}
				}
			}
			else
			{
				EditorGUILayout.BeginHorizontal();

				if (GUILayout.Button("Play", GUILayout.Height(25)))
				{
					manager.Play();
				}

				if (GUILayout.Button("Pause", GUILayout.Height(25)))
				{
					manager.Pause();
				}

				if (GUILayout.Button("Resume", GUILayout.Height(25)))
				{
					manager.Resume();
				}

				if (GUILayout.Button("Stop", GUILayout.Height(25)))
				{
					manager.Stop();
				}

				EditorGUILayout.EndHorizontal();

				EditorGUILayout.Space(5);

				EditorGUILayout.BeginHorizontal();
				EditorGUILayout.LabelField("Status:", GUILayout.Width(50));
				string status = manager.IsPlaying ? (manager.IsPaused ? "Paused" : "Playing") : "Stopped";
				EditorGUILayout.LabelField(status, EditorStyles.boldLabel);
				EditorGUILayout.EndHorizontal();

				EditorGUILayout.BeginHorizontal();
				EditorGUILayout.LabelField("Progress:", GUILayout.Width(60));
				EditorGUILayout.Slider(manager.CurrentProgress, 0f, 1f);
				EditorGUILayout.EndHorizontal();
			}

			EditorGUILayout.EndVertical();
		}

		private void DrawPresetControls()
		{
			EditorGUILayout.BeginVertical(EditorStyles.helpBox);
			EditorGUILayout.LabelField("Preset Management", EditorStyles.boldLabel);

			EditorGUILayout.BeginHorizontal();

			if (GUILayout.Button("Save as Preset", GUILayout.Height(25)))
			{
				SaveAsPreset();
			}

			if (GUILayout.Button("Load Preset", GUILayout.Height(25)))
			{
				LoadPreset();
			}

			if (GUILayout.Button("Preset Library", GUILayout.Height(25)))
			{
				OpenPresetLibrary();
			}

			EditorGUILayout.EndHorizontal();
			EditorGUILayout.EndVertical();
		}

		private void DrawGlobalSettings()
		{
			EditorGUILayout.BeginVertical(EditorStyles.helpBox);

			showGlobalSettings = EditorGUILayout.Foldout(showGlobalSettings, "Global Settings", true, EditorStyles.foldoutHeader);

			if (showGlobalSettings)
			{
				EditorGUI.indentLevel++;
				EditorGUILayout.PropertyField(autoPlayTriggerProp, new GUIContent("Auto Play Trigger"));
				EditorGUILayout.PropertyField(playModeProp, new GUIContent("Play Mode"));
				EditorGUILayout.PropertyField(globalTimeScaleProp, new GUIContent("Time Scale"));
				EditorGUILayout.PropertyField(killOnDisableProp, new GUIContent("Kill On Disable"));
				EditorGUI.indentLevel--;
			}

			EditorGUILayout.EndVertical();
		}

		private void DrawAnimationsList()
		{
			EditorGUILayout.BeginVertical(EditorStyles.helpBox);

			showAnimations = EditorGUILayout.Foldout(showAnimations, $"Animations ({animationsProp.arraySize})", true, EditorStyles.foldoutHeader);

			if (showAnimations)
			{
				EditorGUI.indentLevel++;

				// Add/Remove buttons
				EditorGUILayout.BeginHorizontal();
				if (GUILayout.Button("Add Animation", GUILayout.Height(25)))
				{
					animationsProp.InsertArrayElementAtIndex(animationsProp.arraySize);
				}
				if (GUILayout.Button("Clear All", GUILayout.Height(25)))
				{
					if (EditorUtility.DisplayDialog("Clear All Animations", "Are you sure you want to remove all animations?", "Yes", "No"))
					{
						animationsProp.ClearArray();
					}
				}
				EditorGUILayout.EndHorizontal();

				EditorGUILayout.Space(5);

				// Scroll view for animations
				scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition, GUILayout.MaxHeight(500));

				for (int i = 0; i < animationsProp.arraySize; i++)
				{
					DrawAnimationElement(i);
					EditorGUILayout.Space(5);
				}

				EditorGUILayout.EndScrollView();

				EditorGUI.indentLevel--;
			}

			EditorGUILayout.EndVertical();
		}

		private void DrawAnimationElement(int index)
		{
			SerializedProperty animProp = animationsProp.GetArrayElementAtIndex(index);

			if (!animationFoldouts.ContainsKey(index))
			{
				animationFoldouts[index] = false;
			}

			EditorGUILayout.BeginVertical(EditorStyles.helpBox);

			// Header with color coding
			SerializedProperty typeProp = animProp.FindPropertyRelative("animationType");
			AnimationType animType = (AnimationType)typeProp.enumValueIndex;

			GUI.backgroundColor = GetAnimationTypeColor(animType);
			EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);

			animationFoldouts[index] = EditorGUILayout.Foldout(animationFoldouts[index], $"[{index}] {animType}", true);

			if (GUILayout.Button("×", GUILayout.Width(20)))
			{
				animationsProp.DeleteArrayElementAtIndex(index);
				GUI.backgroundColor = Color.white;
				EditorGUILayout.EndHorizontal();
				EditorGUILayout.EndVertical();
				return;
			}

			EditorGUILayout.EndHorizontal();
			GUI.backgroundColor = Color.white;

			if (animationFoldouts[index])
			{
				EditorGUI.indentLevel++;
				DrawAnimationProperties(animProp, animType);
				EditorGUI.indentLevel--;
			}

			EditorGUILayout.EndVertical();
		}

		private void DrawAnimationProperties(SerializedProperty animProp, AnimationType animType)
		{
			// Target
			EditorGUILayout.PropertyField(animProp.FindPropertyRelative("target"));

			// Type
			EditorGUILayout.PropertyField(animProp.FindPropertyRelative("animationType"));

			// Ease settings
			EditorGUILayout.Space(3);
			EditorGUILayout.LabelField("Ease Settings", EditorStyles.boldLabel);
			SerializedProperty useCustomEaseProp = animProp.FindPropertyRelative("useCustomEase");
			EditorGUILayout.PropertyField(useCustomEaseProp);

			if (useCustomEaseProp.boolValue)
			{
				EditorGUILayout.PropertyField(animProp.FindPropertyRelative("customCurve"));
			}
			else
			{
				EditorGUILayout.PropertyField(animProp.FindPropertyRelative("easeType"));
			}

			// Timing
			EditorGUILayout.Space(3);
			EditorGUILayout.LabelField("Timing", EditorStyles.boldLabel);
			EditorGUILayout.PropertyField(animProp.FindPropertyRelative("duration"));
			EditorGUILayout.PropertyField(animProp.FindPropertyRelative("delay"));

			// Type-specific values
			EditorGUILayout.Space(3);
			EditorGUILayout.LabelField("Values", EditorStyles.boldLabel);
			DrawTypeSpecificFields(animProp, animType);

			// Loop settings
			EditorGUILayout.Space(3);
			EditorGUILayout.LabelField("Loop Settings", EditorStyles.boldLabel);
			SerializedProperty loopProp = animProp.FindPropertyRelative("loop");
			EditorGUILayout.PropertyField(loopProp);

			if (loopProp.boolValue)
			{
				EditorGUILayout.PropertyField(animProp.FindPropertyRelative("loopCount"));
				EditorGUILayout.PropertyField(animProp.FindPropertyRelative("loopType"));
			}

			// Sequence settings (for Mixed mode)
			if (manager.playMode == PlayMode.Mixed)
			{
				EditorGUILayout.Space(3);
				EditorGUILayout.LabelField("Sequence Settings", EditorStyles.boldLabel);
				EditorGUILayout.PropertyField(animProp.FindPropertyRelative("isParallel"));
				EditorGUILayout.PropertyField(animProp.FindPropertyRelative("sequenceDelay"));
			}

			// Advanced
			EditorGUILayout.Space(3);
			EditorGUILayout.LabelField("Advanced", EditorStyles.boldLabel);
			EditorGUILayout.PropertyField(animProp.FindPropertyRelative("useUnscaledTime"));
			EditorGUILayout.PropertyField(animProp.FindPropertyRelative("updateType"));

			// Events
			EditorGUILayout.Space(3);
			EditorGUILayout.LabelField("Events", EditorStyles.boldLabel);
			EditorGUILayout.PropertyField(animProp.FindPropertyRelative("onStart"));
			EditorGUILayout.PropertyField(animProp.FindPropertyRelative("onComplete"));
		}

		private void DrawTypeSpecificFields(SerializedProperty animProp, AnimationType animType)
		{
			switch (animType)
			{
				case AnimationType.Scale:
					EditorGUILayout.PropertyField(animProp.FindPropertyRelative("fromVector"), new GUIContent("From Scale"));
					EditorGUILayout.PropertyField(animProp.FindPropertyRelative("toVector"), new GUIContent("To Scale"));
					break;

				case AnimationType.Move:
				case AnimationType.LocalMove:
					EditorGUILayout.PropertyField(animProp.FindPropertyRelative("fromPosition"), new GUIContent("From Position"));
					EditorGUILayout.PropertyField(animProp.FindPropertyRelative("toPosition"), new GUIContent("To Position"));
					break;

				case AnimationType.AnchoredMove:
					EditorGUILayout.PropertyField(animProp.FindPropertyRelative("fromAnchoredPosition"), new GUIContent("From Position"));
					EditorGUILayout.PropertyField(animProp.FindPropertyRelative("toAnchoredPosition"), new GUIContent("To Position"));
					break;

				case AnimationType.Rotation:
				case AnimationType.LocalRotation:
					EditorGUILayout.PropertyField(animProp.FindPropertyRelative("fromRotation"), new GUIContent("From Rotation"));
					EditorGUILayout.PropertyField(animProp.FindPropertyRelative("toRotation"), new GUIContent("To Rotation"));
					break;

				case AnimationType.Fade:
				case AnimationType.FadeCanvasGroup:
					EditorGUILayout.PropertyField(animProp.FindPropertyRelative("fromValue"), new GUIContent("From Alpha"));
					EditorGUILayout.PropertyField(animProp.FindPropertyRelative("toValue"), new GUIContent("To Alpha"));
					break;

				case AnimationType.Color:
					EditorGUILayout.PropertyField(animProp.FindPropertyRelative("fromColor"), new GUIContent("From Color"));
					EditorGUILayout.PropertyField(animProp.FindPropertyRelative("toColor"), new GUIContent("To Color"));
					break;

				case AnimationType.Path:
					EditorGUILayout.PropertyField(animProp.FindPropertyRelative("pathPoints"), new GUIContent("Path Points"), true);
					EditorGUILayout.PropertyField(animProp.FindPropertyRelative("pathType"), new GUIContent("Path Type"));
					EditorGUILayout.PropertyField(animProp.FindPropertyRelative("pathResolution"), new GUIContent("Resolution"));
					break;

				case AnimationType.ShakePosition:
				case AnimationType.ShakeRotation:
				case AnimationType.ShakeScale:
					EditorGUILayout.PropertyField(animProp.FindPropertyRelative("strength"), new GUIContent("Strength"));
					EditorGUILayout.PropertyField(animProp.FindPropertyRelative("vibrato"), new GUIContent("Vibrato"));
					EditorGUILayout.PropertyField(animProp.FindPropertyRelative("randomness"), new GUIContent("Randomness"));
					break;

				case AnimationType.PunchPosition:
				case AnimationType.PunchRotation:
					EditorGUILayout.PropertyField(animProp.FindPropertyRelative("toVector"), new GUIContent("Punch Direction"));
					EditorGUILayout.PropertyField(animProp.FindPropertyRelative("vibrato"), new GUIContent("Vibrato"));
					EditorGUILayout.PropertyField(animProp.FindPropertyRelative("randomness"), new GUIContent("Elasticity"));
					break;

				case AnimationType.PunchScale:
					EditorGUILayout.PropertyField(animProp.FindPropertyRelative("toVector"), new GUIContent("Punch Scale"));
					EditorGUILayout.PropertyField(animProp.FindPropertyRelative("vibrato"), new GUIContent("Vibrato"));
					EditorGUILayout.PropertyField(animProp.FindPropertyRelative("randomness"), new GUIContent("Elasticity"));
					break;

				case AnimationType.SizeDelta:
					EditorGUILayout.PropertyField(animProp.FindPropertyRelative("fromSize"), new GUIContent("From Size"));
					EditorGUILayout.PropertyField(animProp.FindPropertyRelative("toSize"), new GUIContent("To Size"));
					break;
			}
		}

		private Color GetAnimationTypeColor(AnimationType type)
		{
			switch (type)
			{
				case AnimationType.Scale: return new Color(0.3f, 0.8f, 0.3f, 0.3f);
				case AnimationType.Move:
				case AnimationType.LocalMove:
				case AnimationType.AnchoredMove: return new Color(0.3f, 0.5f, 0.8f, 0.3f);
				case AnimationType.Rotation:
				case AnimationType.LocalRotation: return new Color(0.8f, 0.5f, 0.3f, 0.3f);
				case AnimationType.Fade:
				case AnimationType.FadeCanvasGroup: return new Color(0.6f, 0.3f, 0.8f, 0.3f);
				case AnimationType.Color: return new Color(0.8f, 0.3f, 0.5f, 0.3f);
				case AnimationType.Path: return new Color(0.3f, 0.8f, 0.8f, 0.3f);
				case AnimationType.ShakePosition:
				case AnimationType.ShakeRotation:
				case AnimationType.ShakeScale: return new Color(0.8f, 0.8f, 0.3f, 0.3f);
				case AnimationType.PunchPosition:
				case AnimationType.PunchRotation:
				case AnimationType.PunchScale: return new Color(0.8f, 0.3f, 0.3f, 0.3f);
				default: return new Color(0.5f, 0.5f, 0.5f, 0.3f);
			}
		}

		private void DrawTimelineButton()
		{
			EditorGUILayout.BeginVertical(EditorStyles.helpBox);

			if (GUILayout.Button("Open Timeline Editor", GUILayout.Height(35)))
			{
				AnimationTimelineWindow.ShowWindow(manager);
			}

			EditorGUILayout.EndVertical();
		}

		#region Preview Methods

		private void PreviewAnimation()
		{
			if (!Application.isPlaying)
			{
				isPreviewMode = true;
				previewProgress = 0f;
				manager.Play();
				EditorApplication.update += UpdatePreview;
			}
		}

		private void StopPreview()
		{
			if (isPreviewMode)
			{
				manager.Kill();
				isPreviewMode = false;
				previewProgress = 0f;
				EditorApplication.update -= UpdatePreview;
				Repaint();
			}
		}

		private void ScrubPreview(float progress)
		{
			if (!Application.isPlaying && manager != null)
			{
				manager.SetProgress(progress);
				Repaint();
			}
		}

		private void UpdatePreview()
		{
			if (isPreviewMode && manager != null)
			{
				previewProgress = manager.CurrentProgress;
				Repaint();

				if (previewProgress >= 1f && !manager.IsPlaying)
				{
					StopPreview();
				}
			}
		}

		private void OnDisable()
		{
			StopPreview();
		}

		#endregion

		#region Scene Gizmos

		private void OnSceneGUI()
		{
			if (manager == null || manager.animations == null) return;

			foreach (var anim in manager.animations)
			{
				if (anim == null || anim.target == null) continue;
				DrawAnimationGizmos(anim);
			}
		}

		private void DrawAnimationGizmos(AnimationConfig config)
		{
			Transform transform = config.target.transform;

			switch (config.animationType)
			{
				case AnimationType.Move:
				case AnimationType.LocalMove:
					DrawMoveGizmos(transform, config);
					break;

				case AnimationType.Path:
					DrawPathGizmos(transform, config);
					break;

				case AnimationType.AnchoredMove:
					DrawAnchoredMoveGizmos(config);
					break;
			}
		}

		private void DrawMoveGizmos(Transform transform, AnimationConfig config)
		{
			Handles.color = Color.green;
			Handles.SphereHandleCap(0, config.fromPosition, Quaternion.identity, 0.2f, EventType.Repaint);
			Handles.Label(config.fromPosition, "Start");

			Handles.color = Color.red;
			Handles.SphereHandleCap(0, config.toPosition, Quaternion.identity, 0.2f, EventType.Repaint);
			Handles.Label(config.toPosition, "End");

			Handles.color = Color.yellow;
			Handles.DrawDottedLine(config.fromPosition, config.toPosition, 5f);

			DrawArrow(config.fromPosition, config.toPosition);
		}

		private void DrawPathGizmos(Transform transform, AnimationConfig config)
		{
			if (config.pathPoints == null || config.pathPoints.Length < 2) return;

			Handles.color = Color.cyan;
			for (int i = 0; i < config.pathPoints.Length; i++)
			{
				Handles.SphereHandleCap(0, config.pathPoints[i], Quaternion.identity, 0.15f, EventType.Repaint);
				Handles.Label(config.pathPoints[i], $"P{i}");
			}

			Handles.color = Color.yellow;
			for (int i = 0; i < config.pathPoints.Length - 1; i++)
			{
				if (config.pathType == PathType.Linear)
				{
					Handles.DrawLine(config.pathPoints[i], config.pathPoints[i + 1]);
				}
				else
				{
					DrawSmoothCurve(config.pathPoints, i);
				}
			}

			if (config.pathPoints.Length > 0)
			{
				Handles.color = Color.green;
				Handles.SphereHandleCap(0, config.pathPoints[0], Quaternion.identity, 0.25f, EventType.Repaint);
				Handles.Label(config.pathPoints[0], "START", EditorStyles.boldLabel);

				Handles.color = Color.red;
				Handles.SphereHandleCap(0, config.pathPoints[config.pathPoints.Length - 1], Quaternion.identity, 0.25f, EventType.Repaint);
				Handles.Label(config.pathPoints[config.pathPoints.Length - 1], "END", EditorStyles.boldLabel);
			}
		}

		private void DrawAnchoredMoveGizmos(AnimationConfig config)
		{
			RectTransform rectTransform = config.target.transform as RectTransform;
			if (rectTransform == null) return;

			Canvas canvas = config.target.GetComponentInParent<Canvas>();
			if (canvas == null) return;

			Vector3 fromWorld = rectTransform.TransformPoint(config.fromAnchoredPosition);
			Vector3 toWorld = rectTransform.TransformPoint(config.toAnchoredPosition);

			Handles.color = Color.green;
			Handles.SphereHandleCap(0, fromWorld, Quaternion.identity, 10f, EventType.Repaint);

			Handles.color = Color.red;
			Handles.SphereHandleCap(0, toWorld, Quaternion.identity, 10f, EventType.Repaint);

			Handles.color = Color.yellow;
			Handles.DrawDottedLine(fromWorld, toWorld, 5f);
		}

		private void DrawSmoothCurve(Vector3[] points, int index)
		{
			int segments = 20;
			Vector3 lastPoint = points[index];

			for (int i = 1; i <= segments; i++)
			{
				float t = i / (float)segments;
				Vector3 point = Vector3.Lerp(points[index], points[index + 1], t);
				Handles.DrawLine(lastPoint, point);
				lastPoint = point;
			}
		}

		private void DrawArrow(Vector3 from, Vector3 to)
		{
			Vector3 direction = (to - from).normalized;
			Vector3 right = Vector3.Cross(direction, Vector3.up).normalized;
			if (right == Vector3.zero) right = Vector3.Cross(direction, Vector3.forward).normalized;

			Vector3 arrowTip = to;
			Vector3 arrowBase = to - direction * 0.3f;
			Vector3 arrowLeft = arrowBase - right * 0.15f;
			Vector3 arrowRight = arrowBase + right * 0.15f;

			Handles.color = Color.yellow;
			Handles.DrawLine(arrowLeft, arrowTip);
			Handles.DrawLine(arrowRight, arrowTip);
		}

		#endregion

		#region Preset Methods

		private void SaveAsPreset()
		{
			string path = EditorUtility.SaveFilePanelInProject(
				"Save Animation Preset",
				"AnimationPreset",
				"asset",
				"Save animation preset as ScriptableObject"
			);

			if (!string.IsNullOrEmpty(path))
			{
				AnimationPresetSO preset = ScriptableObject.CreateInstance<AnimationPresetSO>();
				preset.presetName = System.IO.Path.GetFileNameWithoutExtension(path);
				preset.SaveFromManager(manager);

				AssetDatabase.CreateAsset(preset, path);
				AssetDatabase.SaveAssets();
				AssetDatabase.Refresh();

				EditorUtility.DisplayDialog("Success", "Preset saved successfully!", "OK");
			}
		}

		private void LoadPreset()
		{
			string path = EditorUtility.OpenFilePanel("Load Animation Preset", "Assets", "asset");

			if (!string.IsNullOrEmpty(path))
			{
				path = "Assets" + path.Replace(Application.dataPath, "");
				AnimationPresetSO preset = AssetDatabase.LoadAssetAtPath<AnimationPresetSO>(path);

				if (preset != null)
				{
					Undo.RecordObject(manager, "Load Preset");
					preset.ApplyToManager(manager);
					EditorUtility.SetDirty(manager);
					serializedObject.Update();
				}
				else
				{
					EditorUtility.DisplayDialog("Error", "Failed to load preset!", "OK");
				}
			}
		}

		private void OpenPresetLibrary()
		{
			PresetLibraryWindow.ShowWindow(manager);
		}

		#endregion
	}
}
