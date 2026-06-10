#if MODULAR_AVATAR
using System.Collections.Generic;
using System.Linq;

using UnityEditor;
using UnityEngine;

using nadena.dev.ndmf;

using static VRSuya.Core.RenameStruct;
using static VRSuya.Core.Translator;

using Object = UnityEngine.Object;

/*
 * VRSuya Modular Component
 * Contact : vrsuya@gmail.com // Twitter : https://twitter.com/VRSuya
 */

[assembly: ExportsPlugin(typeof(VRSuya.Modular.Editor.AnimationClipRenamerPlugin))]

namespace VRSuya.Modular.Editor {

    public class AnimationClipRenamerPlugin : Plugin<AnimationClipRenamerPlugin> {

		public override string QualifiedName => "com.vrsuya.modular.animationcliprenamer";
		public override string DisplayName => "VRSuya AnimationClipRenamer";

		protected override void Configure() {
			InPhase(BuildPhase.Optimizing).Run(AnimationClipRenamerPass.Instance);
		}
	}

	public class AnimationClipRenamerPass : Pass<AnimationClipRenamerPass> {

		public override string DisplayName => "AnimationClipRenamer";

		protected override void Execute(BuildContext TargetBuildContext) {
			AnimationClipRenamer[] AnimationClipRenamerComponents = TargetBuildContext.AvatarRootObject.GetComponentsInChildren<AnimationClipRenamer>(true);
			if (AnimationClipRenamerComponents.Length > 0) {
				VRSuya.Core.Avatar AvatarInstance = new VRSuya.Core.Avatar();
				AnimationClip[] AvatarAnimationClips = AvatarInstance.GetAllAvatarAnimationClips(TargetBuildContext.AvatarRootObject);
				foreach (AnimationClipRenamer TargetComponent in AnimationClipRenamerComponents) {
					if (!TargetComponent) continue;
					if (TargetComponent.TargetAnimationClips.Length == 0) continue;
					if (TargetComponent.TargetPathRenameList.Count == 0 && TargetComponent.TargetBlendshapeRenameList.Count == 0) continue;
					foreach (AnimationClip TargetAnimationClip in TargetComponent.TargetAnimationClips) {
						AnimationClip BuildContextAnimationClip = AvatarAnimationClips.FirstOrDefault(Item => Item.name == TargetAnimationClip.name);
						if (BuildContextAnimationClip) {
							bool IsModified = false;
							foreach (RenameExpression TargetExpression in TargetComponent.TargetPathRenameList) {
								if (ReplacePath(BuildContextAnimationClip, TargetExpression)) IsModified = true;
							}
							foreach (RenameExpression TargetExpression in TargetComponent.TargetBlendshapeRenameList) {
								if (ReplaceBlendshape(BuildContextAnimationClip, TargetExpression)) IsModified = true;
							}
							if (IsModified) AssetDatabase.SaveAssets();
						}
					}
				}
				foreach (AnimationClipRenamer TargetComponent in AnimationClipRenamerComponents) {
					if (TargetComponent) Object.DestroyImmediate(TargetComponent);
				}
			}
		}

		bool ReplacePath(AnimationClip TargetAnimationClip, RenameExpression TargetExpression) {
			if (TargetAnimationClip) {
				List<string> AnimationPathList = AnimationUtility.GetCurveBindings(TargetAnimationClip)
					.Where(Item => !string.IsNullOrEmpty(Item.path))
					.Select(Item => Item.path)
					.ToList();
				if (AnimationPathList.Contains(TargetExpression.Before)) {
					List<EditorCurveBinding> TargetBindingList = AnimationUtility.GetCurveBindings(TargetAnimationClip)
						.Where(Item => Item.path.Contains(TargetExpression.Before))
						.ToList();
					foreach (EditorCurveBinding TargetBinding in TargetBindingList) {
						string NewPath = TargetBinding.path.Replace(TargetExpression.Before, TargetExpression.After);
						AnimationCurve TargetCurve = AnimationUtility.GetEditorCurve(TargetAnimationClip, TargetBinding);
						TargetAnimationClip.SetCurve(NewPath, TargetBinding.type, TargetBinding.propertyName, TargetCurve);
						TargetAnimationClip.SetCurve(TargetBinding.path, TargetBinding.type, TargetBinding.propertyName, null);
					}
					EditorUtility.SetDirty(TargetAnimationClip);
					return true;
				}
			}
			return false;
		}

		bool ReplaceBlendshape(AnimationClip TargetAnimationClip, RenameExpression TargetExpression) {
			if (TargetAnimationClip) {
				List<string> AnimationBlendshapeList = AnimationUtility.GetCurveBindings(TargetAnimationClip)
					.Where(Item => Item.type == typeof(SkinnedMeshRenderer))
					.Select(Item => Item.propertyName.Remove(0, 11))
					.ToList();
				if (AnimationBlendshapeList.Contains(TargetExpression.Before)) {
					List<EditorCurveBinding> TargetBindingList = AnimationUtility.GetCurveBindings(TargetAnimationClip)
						.Where(Item => Item.type == typeof(SkinnedMeshRenderer))
						.Where(Item => Item.propertyName.Remove(0, 11) == TargetExpression.Before)
						.ToList();
					foreach (EditorCurveBinding TargetBinding in TargetBindingList) {
						AnimationCurve TargetCurve = AnimationUtility.GetEditorCurve(TargetAnimationClip, TargetBinding);
						TargetAnimationClip.SetCurve(TargetBinding.path, TargetBinding.type, $"blendShape.{TargetExpression.After}", TargetCurve);
						TargetAnimationClip.SetCurve(TargetBinding.path, TargetBinding.type, TargetBinding.propertyName, null);
					}
					EditorUtility.SetDirty(TargetAnimationClip);
					return true;
				}
			}
			return false;
		}
	}

	[CustomEditor(typeof(AnimationClipRenamer))]
	public class AnimationClipRenamerEditor : UnityEditor.Editor {

		SerializedProperty SerializedTargetAnimationClips;
		SerializedProperty SerializedTargetPathRenameList;
		SerializedProperty SerializedTargetBlendshapeRenameList;

		void OnEnable() {
			SerializedTargetAnimationClips = serializedObject.FindProperty("TargetAnimationClips");
			SerializedTargetPathRenameList = serializedObject.FindProperty("TargetPathRenameList");
			SerializedTargetBlendshapeRenameList = serializedObject.FindProperty("TargetBlendshapeRenameList");
		}

		public override void OnInspectorGUI() {
			AnimationClipRenamer AnimationClipRenamerInstance = (AnimationClipRenamer)target;
			serializedObject.Update();
			LanguageIndex = EditorGUILayout.Popup(GetTranslatedString("String_Language"), LanguageIndex, LanguageOption);
			EditorGUILayout.Space();
			EditorGUILayout.PropertyField(SerializedTargetAnimationClips, new GUIContent(GetTranslatedString("String_AnimationClip")));
			EditorGUILayout.LabelField(GetTranslatedString("String_Path"), EditorStyles.boldLabel);
			if (SerializedTargetPathRenameList.arraySize > 0) {
				using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox)) {
					for (int Index = 0; Index < SerializedTargetPathRenameList.arraySize; Index++) {
						SerializedProperty PathProperty = SerializedTargetPathRenameList.GetArrayElementAtIndex(Index);
						SerializedProperty BeforeProperty = PathProperty.FindPropertyRelative("Before");
						SerializedProperty AfterProperty = PathProperty.FindPropertyRelative("After");
						EditorGUILayout.BeginHorizontal();
						EditorGUILayout.PropertyField(BeforeProperty, new GUIContent(string.Empty));
						EditorGUILayout.PropertyField(AfterProperty, new GUIContent(string.Empty));
						if (GUILayout.Button("-")) {
							AnimationClipRenamerInstance.TargetPathRenameList.RemoveAt(Index);
						}
						EditorGUILayout.EndHorizontal();
					}
				}
			}
			EditorGUILayout.BeginHorizontal();
			if (GUILayout.Button(GetTranslatedString("String_Add"))) {
				AnimationClipRenamerInstance.TargetPathRenameList.Add(new RenameExpression());
			}
			if (GUILayout.Button(GetTranslatedString("String_Remove"))) {
				if (AnimationClipRenamerInstance.TargetPathRenameList.Count > 0) {
					AnimationClipRenamerInstance.TargetPathRenameList.RemoveAt(AnimationClipRenamerInstance.TargetPathRenameList.Count - 1);
				}
			}
			EditorGUILayout.EndHorizontal();
			EditorGUILayout.LabelField(GetTranslatedString("String_BlendShape"), EditorStyles.boldLabel);
			if (SerializedTargetBlendshapeRenameList.arraySize > 0) {
				using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox)) {
					for (int Index = 0; Index < SerializedTargetBlendshapeRenameList.arraySize; Index++) {
						SerializedProperty BlendShapeProperty = SerializedTargetBlendshapeRenameList.GetArrayElementAtIndex(Index);
						SerializedProperty BeforeProperty = BlendShapeProperty.FindPropertyRelative("Before");
						SerializedProperty AfterProperty = BlendShapeProperty.FindPropertyRelative("After");
						EditorGUILayout.BeginHorizontal();
						EditorGUILayout.PropertyField(BeforeProperty, new GUIContent(string.Empty));
						EditorGUILayout.PropertyField(AfterProperty, new GUIContent(string.Empty));
						if (GUILayout.Button("-")) {
							AnimationClipRenamerInstance.TargetBlendshapeRenameList.RemoveAt(Index);
						}
						EditorGUILayout.EndHorizontal();
					}
				}
			}
			EditorGUILayout.BeginHorizontal();
			if (GUILayout.Button(GetTranslatedString("String_Add"))) {
				AnimationClipRenamerInstance.TargetBlendshapeRenameList.Add(new RenameExpression());
			}
			if (GUILayout.Button(GetTranslatedString("String_Remove"))) {
				if (AnimationClipRenamerInstance.TargetBlendshapeRenameList.Count > 0) {
					AnimationClipRenamerInstance.TargetBlendshapeRenameList.RemoveAt(AnimationClipRenamerInstance.TargetBlendshapeRenameList.Count - 1);
				}
			}
			EditorGUILayout.EndHorizontal();
			serializedObject.ApplyModifiedProperties();
		}
	}
}
#endif