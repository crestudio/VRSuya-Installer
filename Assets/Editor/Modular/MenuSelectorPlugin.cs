#if MODULAR_AVATAR
using UnityEditor;
using UnityEngine;

using VRC.SDK3.Avatars.ScriptableObjects;

using nadena.dev.modular_avatar.core;
using nadena.dev.ndmf;

using VRSuya.Core;
using static VRSuya.Core.Translator;

/*
 * VRSuya Modular Component
 * Contact : vrsuya@gmail.com // Twitter : https://twitter.com/VRSuya
 */

[assembly: ExportsPlugin(typeof(VRSuya.Modular.Editor.MenuSelectorPlugin))]

namespace VRSuya.Modular.Editor {

    public class MenuSelectorPlugin : Plugin<MenuSelectorPlugin> {

		public override string QualifiedName => "com.vrsuya.modular.menuselector";
		public override string DisplayName => "VRSuya MenuSelector";

		protected override void Configure() {
			InPhase(BuildPhase.Resolving).Run(MenuSelectorPass.Instance);
		}
	}

	public class MenuSelectorPass : Pass<MenuSelectorPass> {

		public override string DisplayName => "MenuSelector";

		protected override void Execute(BuildContext TargetBuildContext) {
			MenuSelector[] MenuSelectorComponents = TargetBuildContext.AvatarRootObject.GetComponentsInChildren<MenuSelector>(true);
			foreach (MenuSelector TargetComponent in MenuSelectorComponents) {
				if (TargetComponent) {
					GameObject ParentGameObject = TargetComponent.gameObject;
					ModularAvatarMenuInstaller MenuInstallerComponent = ParentGameObject.GetComponent<ModularAvatarMenuInstaller>();
					if (!MenuInstallerComponent) continue;
					if (!MenuInstallerComponent.menuToAppend) continue;
					string TargetLanguage = GetLanguage(TargetComponent.TargetMenuType);
					VRCExpressionsMenu NewMenu = GetExpressionsMenu(MenuInstallerComponent.menuToAppend, TargetLanguage);
					if (NewMenu) {
						if (MenuInstallerComponent.menuToAppend != NewMenu) {
							MenuInstallerComponent.menuToAppend = NewMenu;
							EditorUtility.SetDirty(TargetComponent);
						}
					}
					Object.DestroyImmediate(TargetComponent);
				}
			}
		}

		string GetLanguage(int TargetLanguageType) {
			switch (TargetLanguageType) {
				case 0:
					switch (Application.systemLanguage) {
						case SystemLanguage.Korean:
							return "KO";
						case SystemLanguage.Japanese:
							return "JA";
						default:
							return "EN";
					}
				case 1:
					return "EN";
				case 2:
					return "KO";
				case 3:
					return "JA";
				default:
					return "EN";
			}
		}

		VRCExpressionsMenu GetExpressionsMenu(VRCExpressionsMenu TargetMenu, string TargetLanguage) {
			string MenuAssetName = AssetUtility.GetAssetName(AssetDatabase.GetAssetPath(TargetMenu), true);
			string MenuAssetPath = AssetUtility.GetDirectoryPath(AssetDatabase.GetAssetPath(TargetMenu));
			string TargetMenuAssetName = GetSubstringString(MenuAssetName, 2) + TargetLanguage;
			if (string.IsNullOrEmpty(TargetMenuAssetName)) return null;
			string[] AssetGUIDs = AssetDatabase.FindAssets($"{TargetMenuAssetName} t:VRCExpressionsMenu", new[] { MenuAssetPath });
			if (AssetGUIDs.Length > 0) {
				VRCExpressionsMenu NewExpressionsMenu = AssetDatabase.LoadAssetAtPath<VRCExpressionsMenu>(AssetDatabase.GUIDToAssetPath(AssetGUIDs[0]));
				if (NewExpressionsMenu && NewExpressionsMenu is VRCExpressionsMenu) {
					return NewExpressionsMenu;
				}
			}
			return null;
		}

		string GetSubstringString(string TargetString, int TargetLength) {
			if (string.IsNullOrEmpty(TargetString)) return string.Empty;
			if (TargetString.Length <= TargetLength) return string.Empty;
			int RemainingLength = TargetString.Length - TargetLength;
			return TargetString.Substring(0, RemainingLength);
		}
	}

	[CustomEditor(typeof(MenuSelector))]
	public class MenuSelectorEditor : UnityEditor.Editor {

		SerializedProperty SerializedTargetMenuType;

		void OnEnable() {
			SerializedTargetMenuType = serializedObject.FindProperty("TargetMenuType");
		}

		public override void OnInspectorGUI() {
			serializedObject.Update();
			LanguageIndex = EditorGUILayout.Popup(GetTranslatedString("String_Language"), LanguageIndex, LanguageOption);
			EditorGUILayout.Space();
			SerializedTargetMenuType.intValue = EditorGUILayout.Popup(GetTranslatedString("String_MenuLanguage"), SerializedTargetMenuType.intValue, GetLanguageOption());
			serializedObject.ApplyModifiedProperties();
		}
	}
}
#endif