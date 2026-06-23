using System.IO;

using UnityEditor;
using UnityEngine;

using VRSuya.Core;
using static VRSuya.Core.Translator;

using Avatar = VRSuya.Core.Avatar;
using Animator = UnityEngine.Animator;

/*
 * VRSuya Installer
 * Contact : vrsuya@gmail.com // Twitter : https://twitter.com/VRSuya
 */

namespace VRSuya.Installer {

	public class HDiffPatcherEditorWindow : EditorWindow {

		public GameObject AvatarGameObject;
		public string HDiffFilePath = string.Empty;
		public bool ReplaceModel = true;
		public bool SilenceMode = false;

		const float BorderX = 30f;

		[MenuItem("Tools/VRSuya/Installer/HDiffPatcher", priority = 1000)]
		static void CreateWindow() {
			HDiffPatcherEditorWindow AppWindow = GetWindowWithRect<HDiffPatcherEditorWindow>(new Rect(0, 0, 400, 200), true, "VRSuya HDiffPatcher");
			AppWindow.Initialize();
		}

		void Initialize() {
			AvatarGameObject = Avatar.GetAvatarGameObject();
		}

		void OnGUI() {
			EditorGUILayout.Space(EditorGUIUtility.singleLineHeight);
			EditorGUILayout.BeginHorizontal();
			GUILayout.Space(BorderX);
			EditorGUIUtility.labelWidth = 100f;
			LanguageIndex = EditorGUILayout.Popup(GetTranslatedString("String_Language"), LanguageIndex, LanguageOption);
			GUILayout.Space(BorderX);
			EditorGUILayout.EndHorizontal();
			EditorGUILayout.Space(EditorGUIUtility.singleLineHeight);
			EditorGUILayout.BeginHorizontal();
			GUILayout.Space(BorderX);
			AvatarGameObject = (GameObject)EditorGUILayout.ObjectField(GetTranslatedString("String_Avatar"), AvatarGameObject, typeof(GameObject), true);
			GUILayout.Space(BorderX);
			EditorGUILayout.EndHorizontal();
			EditorGUILayout.BeginHorizontal();
			GUILayout.Space(BorderX);
			EditorGUILayout.LabelField(GetTranslatedString("String_PatchData"), GUILayout.Width(99));
			HDiffFilePath = EditorGUILayout.TextField(HDiffFilePath);
			if (GUILayout.Button(GetTranslatedString("String_Browse"), GUILayout.Width(72))) {
				string TargetPath = EditorUtility.OpenFilePanel("VRSuya HDiffPatcher", Application.dataPath, "hdiff");
				if (!string.IsNullOrEmpty(TargetPath)) {
					HDiffFilePath = TargetPath;
					GUI.FocusControl(null);
				}
			}
			GUILayout.Space(BorderX);
			EditorGUILayout.EndHorizontal();
			EditorGUILayout.BeginHorizontal();
			GUILayout.Space(BorderX);
			ReplaceModel = EditorGUILayout.ToggleLeft(GetTranslatedString("String_ReplaceAfterPatch"), ReplaceModel);
			GUILayout.Space(BorderX);
			EditorGUILayout.EndHorizontal();
			EditorGUILayout.Space(EditorGUIUtility.singleLineHeight);
			EditorGUILayout.BeginHorizontal();
			GUILayout.Space(BorderX);
			GUI.backgroundColor = Color.cyan;
			EditorGUI.BeginDisabledGroup(!IsReadyToPatch());
			if (GUILayout.Button(GetTranslatedString("String_Update"), GUILayout.Height(40))) {
				if (RequestAvatarPatch()) Close();
			}
			EditorGUI.EndDisabledGroup();
			GUI.backgroundColor = Color.white;
			GUILayout.Space(BorderX);
			EditorGUILayout.EndHorizontal();
		}

		bool IsReadyToPatch() {
			return AvatarGameObject && !string.IsNullOrEmpty(HDiffFilePath);
		}

		public bool RequestAvatarPatch() {
			string OutputAssetPath = ApplyAvatarPatch();
			if (!string.IsNullOrEmpty(OutputAssetPath) && ReplaceModel) {
				if (ReplaceAvatarModel(OutputAssetPath)) {
					return true;
				}
			}
			return false;
		}

		string ApplyAvatarPatch() {
			HDiffPatcher HDiffPatcherInstance = new HDiffPatcher();
			string AvatarFilePath = GetAvatarFilePath();
			string OutputAssetPath = HDiffPatcherInstance.RequestHDiffPatch(AvatarFilePath, HDiffFilePath);
			if (string.IsNullOrEmpty(OutputAssetPath)) {
				if (!SilenceMode) {
					EditorUtility.DisplayDialog(
						"VRSuya HDiffPatcher",
						GetTranslatedString("ERROR_CONSOLE"),
						GetTranslatedString("String_Okay")
					);
				}
				return null;
			}
			if (!ReplaceModel && !SilenceMode) {
				EditorUtility.DisplayDialog(
					"VRSuya HDiffPatcher",
					$"{string.Format(GetTranslatedString("COMPLETED_PATCH"), AvatarGameObject.name)}\n\n{OutputAssetPath}",
					GetTranslatedString("String_Okay")
				);
			}
			Asset.PingAsset(OutputAssetPath);
			return OutputAssetPath;
		}

		string GetAvatarFilePath() {
			Animator AvatarAnimator = AvatarGameObject.GetComponent<Animator>();
			if (AvatarAnimator) {
				string AvatarAssetPath = AssetDatabase.GetAssetPath(AvatarAnimator.avatar);
				return GetFilePath(AvatarAssetPath);
			} else {
				if (!SilenceMode) {
					EditorUtility.DisplayDialog(
						"AvatarFBXHDiffPatcher",
						GetTranslatedString("NO_OLD_ANIMATOR"),
						GetTranslatedString("String_Okay")
					);
				}
				return string.Empty;
			}
		}

		string GetFilePath(string TargetAssetPath) {
			if (string.IsNullOrEmpty(TargetAssetPath)) return string.Empty;
			string UnityProjectAssetsPath = Application.dataPath;
			if (TargetAssetPath.StartsWith("Assets", System.StringComparison.Ordinal)) {
				string AssetSubPath = TargetAssetPath.Substring("Assets".Length);
				string FullFilePath = UnityProjectAssetsPath + AssetSubPath;
				return Path.GetFullPath(FullFilePath);
			} else {
				string UnityProjectPath = Path.GetDirectoryName(UnityProjectAssetsPath);
				string FullFilePath = Path.Combine(UnityProjectPath, TargetAssetPath);
				return Path.GetFullPath(FullFilePath);
			}
		}

		bool ReplaceAvatarModel(string TargetAssetPath) {
			GameObject NewAvatarGameObject = AssetDatabase.LoadAssetAtPath<GameObject>(TargetAssetPath);
			AvatarRebuilder AvatarRebuilderInstance = new AvatarRebuilder(AvatarGameObject, NewAvatarGameObject);
			string StatusString = AvatarRebuilderInstance.RequestRebuildAvatar();
			if (StatusString == "COMPLETED") {
				if (!SilenceMode) {
					EditorUtility.DisplayDialog(
						"VRSuya HDiffPatcher",
						$"{string.Format(GetTranslatedString("COMPLETED_PATCH"), AvatarGameObject.name)}\n\n{TargetAssetPath}",
						GetTranslatedString("String_Okay")
					);
				}
				return true;
			} else {
				if (!SilenceMode) {
					EditorUtility.DisplayDialog(
						"VRSuya HDiffPatcher",
						GetTranslatedString(StatusString),
						GetTranslatedString("String_Okay")
					);
				}
				return false;
			}
		}
	}
}