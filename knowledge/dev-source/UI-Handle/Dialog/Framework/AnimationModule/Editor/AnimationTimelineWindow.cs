using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

namespace HenryLe.Scripts.AnimationModule.Editor
{
	public class AnimationTimelineWindow : EditorWindow
	{
		private static AnimationManager targetManager;
		private Vector2 scrollPosition;
		private Vector2 timelineScrollPosition;

		// Timeline settings
		private float timelineZoom = 100f; // pixels per second
		private float timelineOffset = 0f;
		private float snapInterval = 0.1f;
		private bool snapToGrid = true;

		// Selection
		private int selectedAnimationIndex = -1;
		private bool isDragging = false;
		private float dragStartTime = 0f;

		// Colors
		private Color timelineBackgroundColor = new Color(0.2f, 0.2f, 0.2f);
		private Color timelineGridColor = new Color(0.3f, 0.3f, 0.3f);
		private Color timeMarkerColor = new Color(0.8f, 0.8f, 0.2f);
		private Color selectionColor = new Color(0.3f, 0.6f, 1f);

		// Playback
		private bool isPlaying = false;
		private float playbackTime = 0f;
		private double lastUpdateTime = 0f;

		[MenuItem("Window/Henry/Animation Timeline")]
		public static void ShowWindow()
		{
			AnimationTimelineWindow window = GetWindow<AnimationTimelineWindow>("Animation Timeline");
			window.minSize = new Vector2(600, 400);
			window.Show();
		}

		public static void ShowWindow(AnimationManager manager)
		{
			targetManager = manager;
			ShowWindow();
		}

		private void OnGUI()
		{
			if (targetManager == null)
			{
				DrawNoTargetGUI();
				return;
			}

			DrawToolbar();
			EditorGUILayout.Space(5);

			DrawTimelineControls();
			EditorGUILayout.Space(5);

			DrawTimeline();
			EditorGUILayout.Space(5);

			DrawSelectedAnimationDetails();

			// Handle playback
			if (isPlaying && !Application.isPlaying)
			{
				UpdatePlayback();
			}
		}

		private void DrawNoTargetGUI()
		{
			EditorGUILayout.Space(50);
			EditorGUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();

			EditorGUILayout.BeginVertical();
			EditorGUILayout.LabelField("No Animation Manager Selected", EditorStyles.boldLabel);
			EditorGUILayout.Space(10);

			targetManager = (AnimationManager)EditorGUILayout.ObjectField("Target Manager", targetManager, typeof(AnimationManager), true);

			if (targetManager != null)
			{
				Repaint();
			}

			EditorGUILayout.EndVertical();

			GUILayout.FlexibleSpace();
			EditorGUILayout.EndHorizontal();
		}

		private void DrawToolbar()
		{
			EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);

			targetManager = (AnimationManager)EditorGUILayout.ObjectField(targetManager, typeof(AnimationManager), true, GUILayout.Width(200));

			GUILayout.FlexibleSpace();

			if (GUILayout.Button("Refresh", EditorStyles.toolbarButton, GUILayout.Width(60)))
			{
				Repaint();
			}

			EditorGUILayout.EndHorizontal();
		}

		private void DrawTimelineControls()
		{
			EditorGUILayout.BeginVertical(EditorStyles.helpBox);

			// Playback controls
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField("Playback", EditorStyles.boldLabel, GUILayout.Width(70));

			GUI.backgroundColor = isPlaying ? Color.red : Color.green;
			if (GUILayout.Button(isPlaying ? "Stop" : "Play", GUILayout.Width(60), GUILayout.Height(25)))
			{
				TogglePlayback();
			}
			GUI.backgroundColor = Color.white;

			if (GUILayout.Button("Reset", GUILayout.Width(60), GUILayout.Height(25)))
			{
				ResetPlayback();
			}

			GUILayout.FlexibleSpace();

			float totalDuration = CalculateTotalDuration();
			EditorGUILayout.LabelField($"Total Duration: {totalDuration:F2}s", GUILayout.Width(130));

			EditorGUILayout.EndHorizontal();

			// Progress bar
			EditorGUILayout.Space(5);
			Rect progressRect = EditorGUILayout.GetControlRect(GUILayout.Height(20));
			DrawProgressBar(progressRect, totalDuration);

			// Zoom controls
			EditorGUILayout.Space(5);
			EditorGUILayout.BeginHorizontal();

			EditorGUILayout.LabelField("Zoom", GUILayout.Width(50));
			timelineZoom = EditorGUILayout.Slider(timelineZoom, 20f, 300f);

			GUILayout.Space(20);

			snapToGrid = EditorGUILayout.Toggle("Snap to Grid", snapToGrid, GUILayout.Width(120));
			if (snapToGrid)
			{
				EditorGUILayout.LabelField("Interval", GUILayout.Width(50));
				snapInterval = EditorGUILayout.FloatField(snapInterval, GUILayout.Width(50));
			}

			EditorGUILayout.EndHorizontal();

			EditorGUILayout.EndVertical();
		}

		private void DrawTimeline()
		{
			EditorGUILayout.BeginVertical(EditorStyles.helpBox);
			EditorGUILayout.LabelField("Timeline", EditorStyles.boldLabel);

			Rect timelineRect = GUILayoutUtility.GetRect(position.width - 20, 300);
			DrawTimelineBackground(timelineRect);
			DrawTimelineGrid(timelineRect);
			DrawAnimationBlocks(timelineRect);
			DrawTimeMarker(timelineRect);

			HandleTimelineInput(timelineRect);

			EditorGUILayout.EndVertical();
		}

		private void DrawTimelineBackground(Rect rect)
		{
			EditorGUI.DrawRect(rect, timelineBackgroundColor);
		}

		private void DrawTimelineGrid(Rect rect)
		{
			float totalDuration = CalculateTotalDuration();
			float maxTime = Mathf.Max(totalDuration, 5f);

			// Draw vertical grid lines
			for (float time = 0; time <= maxTime; time += snapInterval)
			{
				float x = rect.x + time * timelineZoom + timelineOffset;
				if (x >= rect.x && x <= rect.xMax)
				{
					Handles.color = timelineGridColor;
					Handles.DrawLine(new Vector3(x, rect.y), new Vector3(x, rect.yMax));

					// Draw time labels
					GUIStyle labelStyle = new GUIStyle(EditorStyles.miniLabel);
					labelStyle.normal.textColor = Color.gray;
					GUI.Label(new Rect(x - 20, rect.y, 40, 15), $"{time:F1}s", labelStyle);
				}
			}
		}

		private void DrawAnimationBlocks(Rect rect)
		{
			if (targetManager == null || targetManager.animations == null) return;

			float yOffset = rect.y + 30;
			float blockHeight = 25;
			float spacing = 5;

			for (int i = 0; i < targetManager.animations.Count; i++)
			{
				AnimationConfig anim = targetManager.animations[i];
				if (anim == null) continue;

				float startTime = CalculateAnimationStartTime(i);
				float duration = anim.duration;

				float blockX = rect.x + startTime * timelineZoom + timelineOffset;
				float blockWidth = duration * timelineZoom;
				float blockY = yOffset + i * (blockHeight + spacing);

				Rect blockRect = new Rect(blockX, blockY, blockWidth, blockHeight);

				// Draw block
				Color blockColor = GetAnimationTypeColor(anim.animationType);
				if (i == selectedAnimationIndex)
				{
					blockColor = selectionColor;
				}

				EditorGUI.DrawRect(blockRect, blockColor);
				EditorGUI.DrawRect(new Rect(blockRect.x, blockRect.y, blockRect.width, 1), Color.black);
				EditorGUI.DrawRect(new Rect(blockRect.x, blockRect.yMax - 1, blockRect.width, 1), Color.black);
				EditorGUI.DrawRect(new Rect(blockRect.x, blockRect.y, 1, blockRect.height), Color.black);
				EditorGUI.DrawRect(new Rect(blockRect.xMax - 1, blockRect.y, 1, blockRect.height), Color.black);

				// Draw label
				GUIStyle labelStyle = new GUIStyle(EditorStyles.miniLabel);
				labelStyle.normal.textColor = Color.white;
				labelStyle.alignment = TextAnchor.MiddleLeft;
				labelStyle.padding = new RectOffset(5, 5, 0, 0);

				string label = $"{i}: {anim.animationType} ({duration:F2}s)";
				GUI.Label(blockRect, label, labelStyle);

				// Handle selection
				if (Event.current.type == EventType.MouseDown && blockRect.Contains(Event.current.mousePosition))
				{
					selectedAnimationIndex = i;
					isDragging = true;
					dragStartTime = startTime;
					Event.current.Use();
					Repaint();
				}
			}
		}

		private void DrawTimeMarker(Rect rect)
		{
			if (isPlaying || playbackTime > 0)
			{
				float markerX = rect.x + playbackTime * timelineZoom + timelineOffset;

				if (markerX >= rect.x && markerX <= rect.xMax)
				{
					Handles.color = timeMarkerColor;
					Handles.DrawLine(new Vector3(markerX, rect.y), new Vector3(markerX, rect.yMax));

					// Draw marker head
					Vector3[] triangle = new Vector3[3]
					{
						new Vector3(markerX - 5, rect.y + 15),
						new Vector3(markerX + 5, rect.y + 15),
						new Vector3(markerX, rect.y + 25)
					};
					Handles.DrawAAConvexPolygon(triangle);
				}
			}
		}

		private void DrawProgressBar(Rect rect, float totalDuration)
		{
			EditorGUI.DrawRect(rect, new Color(0.3f, 0.3f, 0.3f));

			if (totalDuration > 0)
			{
				float progress = playbackTime / totalDuration;
				Rect progressFillRect = new Rect(rect.x, rect.y, rect.width * progress, rect.height);
				EditorGUI.DrawRect(progressFillRect, new Color(0.3f, 0.6f, 1f));
			}

			GUIStyle labelStyle = new GUIStyle(EditorStyles.boldLabel);
			labelStyle.alignment = TextAnchor.MiddleCenter;
			labelStyle.normal.textColor = Color.white;
			GUI.Label(rect, $"{playbackTime:F2}s", labelStyle);
		}

		private void DrawSelectedAnimationDetails()
		{
			if (selectedAnimationIndex < 0 || targetManager == null || selectedAnimationIndex >= targetManager.animations.Count)
			{
				return;
			}

			EditorGUILayout.BeginVertical(EditorStyles.helpBox);
			EditorGUILayout.LabelField("Selected Animation Details", EditorStyles.boldLabel);

			AnimationConfig anim = targetManager.animations[selectedAnimationIndex];

			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField("Index:", GUILayout.Width(60));
			EditorGUILayout.LabelField(selectedAnimationIndex.ToString(), EditorStyles.boldLabel);
			EditorGUILayout.EndHorizontal();

			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField("Type:", GUILayout.Width(60));
			EditorGUILayout.LabelField(anim.animationType.ToString(), EditorStyles.boldLabel);
			EditorGUILayout.EndHorizontal();

			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField("Duration:", GUILayout.Width(60));
			EditorGUILayout.LabelField($"{anim.duration:F2}s", EditorStyles.boldLabel);
			EditorGUILayout.EndHorizontal();

			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField("Delay:", GUILayout.Width(60));
			EditorGUILayout.LabelField($"{anim.delay:F2}s", EditorStyles.boldLabel);
			EditorGUILayout.EndHorizontal();

			EditorGUILayout.Space(5);

			if (GUILayout.Button("Focus in Inspector", GUILayout.Height(25)))
			{
				Selection.activeGameObject = targetManager.gameObject;
			}

			EditorGUILayout.EndVertical();
		}

		private void HandleTimelineInput(Rect rect)
		{
			Event e = Event.current;

			// Handle dragging
			if (isDragging)
			{
				if (e.type == EventType.MouseDrag)
				{
					// Could implement dragging animations on timeline here
					Repaint();
				}
				else if (e.type == EventType.MouseUp)
				{
					isDragging = false;
					Repaint();
				}
			}

			// Handle scrolling
			if (e.type == EventType.ScrollWheel && rect.Contains(e.mousePosition))
			{
				timelineOffset += e.delta.y * 10f;
				e.Use();
				Repaint();
			}

			// Click to set playback time
			if (e.type == EventType.MouseDown && e.button == 0 && rect.Contains(e.mousePosition))
			{
				float clickX = e.mousePosition.x - rect.x - timelineOffset;
				playbackTime = Mathf.Max(0, clickX / timelineZoom);

				if (snapToGrid)
				{
					playbackTime = Mathf.Round(playbackTime / snapInterval) * snapInterval;
				}

				if (targetManager != null)
				{
					targetManager.SetProgress(playbackTime / CalculateTotalDuration());
				}

				e.Use();
				Repaint();
			}
		}

		private void TogglePlayback()
		{
			isPlaying = !isPlaying;

			if (isPlaying)
			{
				lastUpdateTime = EditorApplication.timeSinceStartup;
				EditorApplication.update += OnEditorUpdate;

				if (targetManager != null && !Application.isPlaying)
				{
					targetManager.Play();
				}
			}
			else
			{
				EditorApplication.update -= OnEditorUpdate;

				if (targetManager != null && !Application.isPlaying)
				{
					targetManager.Pause();
				}
			}
		}

		private void ResetPlayback()
		{
			playbackTime = 0f;
			isPlaying = false;
			EditorApplication.update -= OnEditorUpdate;

			if (targetManager != null && !Application.isPlaying)
			{
				targetManager.Stop();
			}

			Repaint();
		}

		private void OnEditorUpdate()
		{
			if (isPlaying)
			{
				UpdatePlayback();
			}
		}

		private void UpdatePlayback()
		{
			double currentTime = EditorApplication.timeSinceStartup;
			float deltaTime = (float)(currentTime - lastUpdateTime);
			lastUpdateTime = currentTime;

			playbackTime += deltaTime;

			float totalDuration = CalculateTotalDuration();
			if (playbackTime >= totalDuration)
			{
				playbackTime = totalDuration;
				isPlaying = false;
				EditorApplication.update -= OnEditorUpdate;
			}

			if (targetManager != null && totalDuration > 0)
			{
				targetManager.SetProgress(playbackTime / totalDuration);
			}

			Repaint();
		}

		private float CalculateAnimationStartTime(int index)
		{
			if (targetManager == null || targetManager.animations == null) return 0f;

			float startTime = 0f;

			if (targetManager.playMode == PlayMode.Sequential)
			{
				for (int i = 0; i < index; i++)
				{
					if (targetManager.animations[i] != null)
					{
						startTime += targetManager.animations[i].delay + targetManager.animations[i].duration;
					}
				}

				if (index < targetManager.animations.Count)
				{
					startTime += targetManager.animations[index].delay;
				}
			}
			else if (targetManager.playMode == PlayMode.Parallel)
			{
				if (index < targetManager.animations.Count)
				{
					startTime = targetManager.animations[index].delay;
				}
			}
			else if (targetManager.playMode == PlayMode.Mixed)
			{
				for (int i = 0; i < index; i++)
				{
					AnimationConfig anim = targetManager.animations[i];
					if (anim != null)
					{
						if (!anim.isParallel)
						{
							startTime += anim.delay + anim.duration + anim.sequenceDelay;
						}
					}
				}

				if (index < targetManager.animations.Count)
				{
					AnimationConfig currentAnim = targetManager.animations[index];
					if (currentAnim.isParallel && index > 0)
					{
						startTime = CalculateAnimationStartTime(index - 1);
					}
					startTime += currentAnim.delay;
				}
			}

			return startTime;
		}

		private float CalculateTotalDuration()
		{
			if (targetManager == null || targetManager.animations == null || targetManager.animations.Count == 0)
				return 0f;

			float totalDuration = 0f;

			if (targetManager.playMode == PlayMode.Sequential)
			{
				foreach (var anim in targetManager.animations)
				{
					if (anim != null)
					{
						totalDuration += anim.delay + anim.duration;
					}
				}
			}
			else if (targetManager.playMode == PlayMode.Parallel)
			{
				foreach (var anim in targetManager.animations)
				{
					if (anim != null)
					{
						float animDuration = anim.delay + anim.duration;
						if (animDuration > totalDuration)
						{
							totalDuration = animDuration;
						}
					}
				}
			}
			else if (targetManager.playMode == PlayMode.Mixed)
			{
				float currentTime = 0f;
				float maxParallelTime = 0f;

				foreach (var anim in targetManager.animations)
				{
					if (anim != null)
					{
						if (anim.isParallel)
						{
							float parallelEndTime = currentTime + anim.delay + anim.duration;
							if (parallelEndTime > maxParallelTime)
							{
								maxParallelTime = parallelEndTime;
							}
						}
						else
						{
							currentTime = Mathf.Max(currentTime, maxParallelTime);
							currentTime += anim.sequenceDelay + anim.delay + anim.duration;
							maxParallelTime = currentTime;
						}
					}
				}

				totalDuration = Mathf.Max(currentTime, maxParallelTime);
			}

			return totalDuration;
		}

		private Color GetAnimationTypeColor(AnimationType type)
		{
			switch (type)
			{
				case AnimationType.Scale: return new Color(0.3f, 0.8f, 0.3f);
				case AnimationType.Move:
				case AnimationType.LocalMove:
				case AnimationType.AnchoredMove: return new Color(0.3f, 0.5f, 0.8f);
				case AnimationType.Rotation:
				case AnimationType.LocalRotation: return new Color(0.8f, 0.5f, 0.3f);
				case AnimationType.Fade:
				case AnimationType.FadeCanvasGroup: return new Color(0.6f, 0.3f, 0.8f);
				case AnimationType.Color: return new Color(0.8f, 0.3f, 0.5f);
				case AnimationType.Path: return new Color(0.3f, 0.8f, 0.8f);
				case AnimationType.ShakePosition:
				case AnimationType.ShakeRotation:
				case AnimationType.ShakeScale: return new Color(0.8f, 0.8f, 0.3f);
				case AnimationType.PunchPosition:
				case AnimationType.PunchRotation:
				case AnimationType.PunchScale: return new Color(0.8f, 0.3f, 0.3f);
				default: return new Color(0.5f, 0.5f, 0.5f);
			}
		}

		private void OnDestroy()
		{
			EditorApplication.update -= OnEditorUpdate;
		}
	}
}
