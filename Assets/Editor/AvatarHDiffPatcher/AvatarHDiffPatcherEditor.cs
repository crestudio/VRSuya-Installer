using System.IO;

using UnityEditor;
using UnityEngine;

using static VRSuya.Core.Translator;

using Avatar = VRSuya.Core.Avatar;
using Animator = UnityEngine.Animator;

/*
 * VRSuya Installer
 * Contact : vrsuya@gmail.com // Twitter : https://twitter.com/VRSuya
 */

namespace VRSuya.Installer {

	public class AvatarHDiffPatcherEditorWindow : EditorWindow {

		public GameObject AvatarGameObject;
		public string HDiffFilePath = string.Empty;

		const float BorderX = 30f;

		[MenuItem("Tools/VRSuya/Installer/AvatarHDiffPatcher", priority = 1000)]
		static void OpenAvatarHDiffPatcherEditorWindow() {
			AvatarHDiffPatcherEditorWindow AppWindow = GetWindowWithRect<AvatarHDiffPatcherEditorWindow>(new Rect(0, 0, 400, 180), true, "VRSuya AvatarHDiffPatcher");
			AppWindow.Initialize();
		}

		void Initialize() {
			Avatar AvatarInstance = new Avatar();
			AvatarGameObject = AvatarInstance.GetAvatarGameObject();
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
				string SelectedPath = EditorUtility.OpenFilePanel("VRSuya AvatarHDiffPatcher", Application.dataPath, "hdiff");
				if (!string.IsNullOrEmpty(SelectedPath)) {
					HDiffFilePath = SelectedPath;
					GUI.FocusControl(null);
				}
			}
			GUILayout.Space(BorderX);
			EditorGUILayout.EndHorizontal();
			EditorGUILayout.Space(EditorGUIUtility.singleLineHeight);
			EditorGUILayout.BeginHorizontal();
			GUILayout.Space(BorderX);
			GUI.backgroundColor = Color.cyan;
			EditorGUI.BeginDisabledGroup(!IsReadyToPatch());
			if (GUILayout.Button(GetTranslatedString("String_Update"), GUILayout.Height(40))) {
				if (RequestAvatarPatch()) {
					Close();
				}
			}
			EditorGUI.EndDisabledGroup();
			GUI.backgroundColor = Color.white;
			GUILayout.Space(BorderX);
			EditorGUILayout.EndHorizontal();
		}

		bool IsReadyToPatch() {
			return AvatarGameObject && !string.IsNullOrEmpty(HDiffFilePath);
		}

		bool RequestAvatarPatch() {
			AvatarFBXHDiffPatcher AvatarPatcherInstance = new AvatarFBXHDiffPatcher();
			string AvatarFilePath = GetAvatarFilePath();
			string OutputAssetPath = AvatarPatcherInstance.RequestHDiffPatch(AvatarFilePath, HDiffFilePath);
			if (string.IsNullOrEmpty(OutputAssetPath)) {
				EditorUtility.DisplayDialog(
					"AvatarFBXHDiffPatcher",
					GetTranslatedString("ERROR_CONSOLE"),
					GetTranslatedString("String_Okay")
				);
				return false;
			}
			EditorUtility.DisplayDialog(
				"AvatarFBXHDiffPatcher",
				$"{string.Format(GetTranslatedString("COMPLETED_PATCH"), AvatarGameObject.name)}\n\n{OutputAssetPath}",
				GetTranslatedString("String_Okay")
			);
			VRSuya.Core.Asset AssetInstance = new VRSuya.Core.Asset();
			AssetInstance.PingAsset(OutputAssetPath);
			return true;
		}

		string GetAvatarFilePath() {
			Animator AvatarAnimator = AvatarGameObject.GetComponent<Animator>();
			if (AvatarAnimator) {
				string AvatarAssetPath = AssetDatabase.GetAssetPath(AvatarAnimator.avatar);
				return GetFilePath(AvatarAssetPath);
			} else {
				EditorUtility.DisplayDialog(
					"AvatarFBXHDiffPatcher",
					GetTranslatedString("NO_OLD_ANIMATOR"),
					GetTranslatedString("String_Okay")
				);
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
	}
}