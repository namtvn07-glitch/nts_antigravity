using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace HenryLe.Scripts.AnimationModule.Editor
{
	public class PresetLibraryWindow : EditorWindow
	{
		private static AnimationManager targetManager;
		private List<AnimationPresetSO> allPresets = new List<AnimationPresetSO>();
		private List<AnimationPresetSO> filteredPresets = new List<AnimationPresetSO>();
		private Dictionary<string, bool> categoryFoldouts = new Dictionary<string, bool>();

		private Vector2 scrollPosition;
		private string searchQuery = "";
		private string selectedCategory = "All";

		private AnimationPresetSO selectedPreset = null;
		private bool showPresetDetails = false;

		// Colors
		private Color headerColor = new Color(0.3f, 0.5f, 0.8f, 0.3f);
		private Color selectedColor = new Color(0.3f, 0.7f, 1f, 0.5f);

		[MenuItem("Window/Henry/Animation Preset Library")]
		public static void ShowWindow()
		{
			PresetLibraryWindow window = GetWindow<PresetLibraryWindow>("Preset Library");
			window.minSize = new Vector2(500, 400);
			window.Show();
		}

		public static void ShowWindow(AnimationManager manager)
		{
			targetManager = manager;
			ShowWindow();
		}

		private void OnEnable()
		{
			RefreshPresetList();
		}

		private void OnGUI()
		{
			DrawHeader();
			EditorGUILayout.Space(5);

			DrawTargetManager();
			EditorGUILayout.Space(5);

			DrawSearchAndFilter();
			EditorGUILayout.Space(5);

			EditorGUILayout.BeginHorizontal();

			// Left panel - Preset list
			EditorGUILayout.BeginVertical(GUILayout.Width(position.width * 0.5f));
			DrawPresetList();
			EditorGUILayout.EndVertical();

			// Right panel - Preset details
			EditorGUILayout.BeginVertical();
			DrawPresetDetails();
			EditorGUILayout.EndVertical();

			EditorGUILayout.EndHorizontal();
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

			EditorGUILayout.LabelField("Animation Preset Library", titleStyle);

			GUI.backgroundColor = Color.white;
			EditorGUILayout.EndVertical();
		}

		private void DrawTargetManager()
		{
			EditorGUILayout.BeginVertical(EditorStyles.helpBox);

			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField("Target Manager:", GUILayout.Width(100));
			targetManager = (AnimationManager)EditorGUILayout.ObjectField(targetManager, typeof(AnimationManager), true);
			EditorGUILayout.EndHorizontal();

			EditorGUILayout.EndVertical();
		}

		private void DrawSearchAndFilter()
		{
			EditorGUILayout.BeginVertical(EditorStyles.helpBox);

			// Search bar
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField("Search:", GUILayout.Width(60));
			string newSearch = EditorGUILayout.TextField(searchQuery);

			if (newSearch != searchQuery)
			{
				searchQuery = newSearch;
				FilterPresets();
			}

			if (GUILayout.Button("Clear", GUILayout.Width(50)))
			{
				searchQuery = "";
				FilterPresets();
			}

			EditorGUILayout.EndHorizontal();

			// Category filter
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField("Category:", GUILayout.Width(60));

			List<string> categories = GetAllCategories();
			int currentIndex = categories.IndexOf(selectedCategory);
			if (currentIndex < 0) currentIndex = 0;

			int newIndex = EditorGUILayout.Popup(currentIndex, categories.ToArray());
			if (newIndex != currentIndex)
			{
				selectedCategory = categories[newIndex];
				FilterPresets();
			}

			EditorGUILayout.EndHorizontal();

			// Toolbar
			EditorGUILayout.BeginHorizontal();

			if (GUILayout.Button("Refresh", GUILayout.Height(25)))
			{
				RefreshPresetList();
			}

			if (GUILayout.Button("Create New Preset", GUILayout.Height(25)))
			{
				CreateNewPreset();
			}

			EditorGUILayout.EndHorizontal();

			EditorGUILayout.LabelField($"Found: {filteredPresets.Count} presets", EditorStyles.miniLabel);

			EditorGUILayout.EndVertical();
		}

		private void DrawPresetList()
		{
			EditorGUILayout.BeginVertical(EditorStyles.helpBox);
			EditorGUILayout.LabelField("Presets", EditorStyles.boldLabel);

			scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

			if (filteredPresets.Count == 0)
			{
				EditorGUILayout.HelpBox("No presets found. Create a new preset or adjust search filters.", MessageType.Info);
			}
			else
			{
				foreach (var preset in filteredPresets)
				{
					if (preset == null) continue;

					DrawPresetItem(preset);
					EditorGUILayout.Space(2);
				}
			}

			EditorGUILayout.EndScrollView();
			EditorGUILayout.EndVertical();
		}

		private void DrawPresetItem(AnimationPresetSO preset)
		{
			EditorGUILayout.BeginVertical(EditorStyles.helpBox);

			bool isSelected = selectedPreset == preset;
			if (isSelected)
			{
				GUI.backgroundColor = selectedColor;
			}

			EditorGUILayout.BeginHorizontal();

			// Preset info
			EditorGUILayout.BeginVertical();

			GUIStyle nameStyle = new GUIStyle(EditorStyles.boldLabel);
			if (isSelected) nameStyle.normal.textColor = Color.cyan;

			EditorGUILayout.LabelField(preset.presetName, nameStyle);

			if (!string.IsNullOrEmpty(preset.category))
			{
				EditorGUILayout.LabelField($"[{preset.category}]", EditorStyles.miniLabel);
			}

			if (!string.IsNullOrEmpty(preset.description))
			{
				GUIStyle descStyle = new GUIStyle(EditorStyles.wordWrappedMiniLabel);
				descStyle.normal.textColor = Color.gray;
				EditorGUILayout.LabelField(preset.description, descStyle);
			}

			EditorGUILayout.LabelField($"Animations: {preset.animations.Count}", EditorStyles.miniLabel);

			EditorGUILayout.EndVertical();

			// Buttons
			EditorGUILayout.BeginVertical(GUILayout.Width(80));

			if (GUILayout.Button("Select", GUILayout.Height(20)))
			{
				selectedPreset = preset;
				showPresetDetails = true;
				Repaint();
			}

			if (targetManager != null)
			{
				if (GUILayout.Button("Apply", GUILayout.Height(20)))
				{
					ApplyPresetToManager(preset);
				}
			}

			if (GUILayout.Button("Edit", GUILayout.Height(20)))
			{
				Selection.activeObject = preset;
				EditorGUIUtility.PingObject(preset);
			}

			EditorGUILayout.EndVertical();

			EditorGUILayout.EndHorizontal();

			GUI.backgroundColor = Color.white;
			EditorGUILayout.EndVertical();
		}

		private void DrawPresetDetails()
		{
			EditorGUILayout.BeginVertical(EditorStyles.helpBox);
			EditorGUILayout.LabelField("Preset Details", EditorStyles.boldLabel);

			if (selectedPreset == null)
			{
				EditorGUILayout.HelpBox("Select a preset to view details", MessageType.Info);
			}
			else
			{
				EditorGUILayout.BeginVertical(EditorStyles.helpBox);

				EditorGUILayout.LabelField("Name:", EditorStyles.boldLabel);
				EditorGUILayout.LabelField(selectedPreset.presetName);

				EditorGUILayout.Space(5);

				EditorGUILayout.LabelField("Category:", EditorStyles.boldLabel);
				EditorGUILayout.LabelField(selectedPreset.category);

				EditorGUILayout.Space(5);

				EditorGUILayout.LabelField("Description:", EditorStyles.boldLabel);
				EditorGUILayout.LabelField(selectedPreset.description, EditorStyles.wordWrappedLabel);

				EditorGUILayout.Space(10);

				EditorGUILayout.LabelField("Settings:", EditorStyles.boldLabel);
				EditorGUILayout.LabelField($"Auto Play: {selectedPreset.autoPlayTrigger}");
				EditorGUILayout.LabelField($"Play Mode: {selectedPreset.playMode}");
				EditorGUILayout.LabelField($"Time Scale: {selectedPreset.globalTimeScale}");

				EditorGUILayout.Space(10);

				EditorGUILayout.LabelField($"Animations ({selectedPreset.animations.Count}):", EditorStyles.boldLabel);

				Vector2 animScrollPos = EditorGUILayout.BeginScrollView(Vector2.zero, GUILayout.MaxHeight(200));

				for (int i = 0; i < selectedPreset.animations.Count; i++)
				{
					var anim = selectedPreset.animations[i];
					if (anim != null)
					{
						Color animColor = GetAnimationTypeColor(anim.animationType);
						GUI.backgroundColor = animColor;
						EditorGUILayout.BeginVertical(EditorStyles.helpBox);
						GUI.backgroundColor = Color.white;

						EditorGUILayout.LabelField($"{i + 1}. {anim.animationType}", EditorStyles.boldLabel);
						EditorGUILayout.LabelField($"Duration: {anim.duration}s, Delay: {anim.delay}s", EditorStyles.miniLabel);
						EditorGUILayout.LabelField($"Ease: {anim.easeType}", EditorStyles.miniLabel);

						EditorGUILayout.EndVertical();
						EditorGUILayout.Space(3);
					}
				}

				EditorGUILayout.EndScrollView();

				EditorGUILayout.Space(10);

				// Action buttons
				if (targetManager != null)
				{
					if (GUILayout.Button("Apply to Manager", GUILayout.Height(30)))
					{
						ApplyPresetToManager(selectedPreset);
					}
				}

				if (GUILayout.Button("Duplicate Preset", GUILayout.Height(25)))
				{
					DuplicatePreset(selectedPreset);
				}

				if (GUILayout.Button("Delete Preset", GUILayout.Height(25)))
				{
					DeletePreset(selectedPreset);
				}

				EditorGUILayout.EndVertical();
			}

			EditorGUILayout.EndVertical();
		}

		private void RefreshPresetList()
		{
			allPresets.Clear();

			string[] guids = AssetDatabase.FindAssets("t:AnimationPresetSO");
			foreach (string guid in guids)
			{
				string path = AssetDatabase.GUIDToAssetPath(guid);
				AnimationPresetSO preset = AssetDatabase.LoadAssetAtPath<AnimationPresetSO>(path);
				if (preset != null)
				{
					allPresets.Add(preset);
				}
			}

			FilterPresets();
		}

		private void FilterPresets()
		{
			filteredPresets.Clear();

			foreach (var preset in allPresets)
			{
				if (preset == null) continue;

				// Category filter
				if (selectedCategory != "All" && preset.category != selectedCategory)
					continue;

				// Search filter
				if (!string.IsNullOrEmpty(searchQuery))
				{
					string query = searchQuery.ToLower();
					bool matchName = preset.presetName.ToLower().Contains(query);
					bool matchDesc = preset.description.ToLower().Contains(query);
					bool matchCategory = preset.category.ToLower().Contains(query);

					if (!matchName && !matchDesc && !matchCategory)
						continue;
				}

				filteredPresets.Add(preset);
			}

			// Sort by name
			filteredPresets = filteredPresets.OrderBy(p => p.presetName).ToList();
		}

		private List<string> GetAllCategories()
		{
			HashSet<string> categories = new HashSet<string> { "All" };

			foreach (var preset in allPresets)
			{
				if (preset != null && !string.IsNullOrEmpty(preset.category))
				{
					categories.Add(preset.category);
				}
			}

			return categories.OrderBy(c => c).ToList();
		}

		private void ApplyPresetToManager(AnimationPresetSO preset)
		{
			if (targetManager == null || preset == null)
			{
				EditorUtility.DisplayDialog("Error", "Target manager or preset is null!", "OK");
				return;
			}

			Undo.RecordObject(targetManager, "Apply Preset");
			preset.ApplyToManager(targetManager);
			EditorUtility.SetDirty(targetManager);

			EditorUtility.DisplayDialog("Success", $"Preset '{preset.presetName}' applied successfully!", "OK");
		}

		private void CreateNewPreset()
		{
			string path = EditorUtility.SaveFilePanelInProject(
				"Create Animation Preset",
				"NewAnimationPreset",
				"asset",
				"Create a new animation preset"
			);

			if (!string.IsNullOrEmpty(path))
			{
				AnimationPresetSO newPreset = ScriptableObject.CreateInstance<AnimationPresetSO>();
				newPreset.presetName = System.IO.Path.GetFileNameWithoutExtension(path);
				newPreset.category = "Default";

				if (targetManager != null)
				{
					newPreset.SaveFromManager(targetManager);
				}

				AssetDatabase.CreateAsset(newPreset, path);
				AssetDatabase.SaveAssets();
				AssetDatabase.Refresh();

				RefreshPresetList();

				Selection.activeObject = newPreset;
				EditorGUIUtility.PingObject(newPreset);
			}
		}

		private void DuplicatePreset(AnimationPresetSO preset)
		{
			if (preset == null) return;

			string originalPath = AssetDatabase.GetAssetPath(preset);
			string directory = System.IO.Path.GetDirectoryName(originalPath);
			string fileName = System.IO.Path.GetFileNameWithoutExtension(originalPath);
			string newPath = AssetDatabase.GenerateUniqueAssetPath($"{directory}/{fileName}_Copy.asset");

			AnimationPresetSO duplicate = ScriptableObject.CreateInstance<AnimationPresetSO>();
			duplicate.presetName = preset.presetName + " (Copy)";
			duplicate.description = preset.description;
			duplicate.category = preset.category;
			duplicate.autoPlayTrigger = preset.autoPlayTrigger;
			duplicate.playMode = preset.playMode;
			duplicate.globalTimeScale = preset.globalTimeScale;

			foreach (var anim in preset.animations)
			{
				if (anim != null)
				{
					duplicate.animations.Add(anim.Clone());
				}
			}

			AssetDatabase.CreateAsset(duplicate, newPath);
			AssetDatabase.SaveAssets();
			AssetDatabase.Refresh();

			RefreshPresetList();

			selectedPreset = duplicate;
			EditorGUIUtility.PingObject(duplicate);
		}

		private void DeletePreset(AnimationPresetSO preset)
		{
			if (preset == null) return;

			if (EditorUtility.DisplayDialog("Delete Preset", $"Are you sure you want to delete '{preset.presetName}'?", "Yes", "No"))
			{
				string path = AssetDatabase.GetAssetPath(preset);
				AssetDatabase.DeleteAsset(path);
				AssetDatabase.Refresh();

				if (selectedPreset == preset)
				{
					selectedPreset = null;
				}

				RefreshPresetList();
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
	}
}
