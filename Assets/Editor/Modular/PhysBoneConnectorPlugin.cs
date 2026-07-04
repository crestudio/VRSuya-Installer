#if MODULAR_AVATAR
using System;
using System.Collections.Generic;
using System.Linq;

using UnityEditor;
using UnityEngine;

using nadena.dev.ndmf;

using VRSuya.Core;
using static VRSuya.Core.Translator;

using Object = UnityEngine.Object;

/*
 * VRSuya Modular Component
 * Contact : vrsuya@gmail.com // Twitter : https://twitter.com/VRSuya
 */

[assembly: ExportsPlugin(typeof(VRSuya.Modular.Editor.PhysBoneConnectorPlugin))]

namespace VRSuya.Modular.Editor {

    public class PhysBoneConnectorPlugin : Plugin<PhysBoneConnectorPlugin> {

		public override string QualifiedName => "com.vrsuya.modular.physboneconnector";
		public override string DisplayName => "VRSuya PhysBoneConnector";

		protected override void Configure() {
			InPhase(BuildPhase.Optimizing).Run(PhysBoneConnectorPass.Instance);
		}
	}

	public class PhysBoneConnectorPass : Pass<PhysBoneConnectorPass> {

		public override string DisplayName => "PhysBoneConnector";

		protected override void Execute(BuildContext TargetBuildContext) {
			PhysBoneConnector[] PhysBoneConnectors = TargetBuildContext.AvatarRootObject.GetComponentsInChildren<PhysBoneConnector>(true);
			if (PhysBoneConnectors.Length > 0) {
				TargetBuildContext.AvatarRootObject.TryGetComponent(out Animator TargetAnimator);
				if (TargetAnimator) {
					foreach (PhysBoneConnector TargetComponent in PhysBoneConnectors) {
						if (!TargetComponent) continue;
						if (TargetComponent.TargetType == PhysBoneType.Cheek) {
							Transform[] HeadTransforms = TargetAnimator.GetBoneTransform(HumanBodyBones.Head).GetComponentsInChildren<Transform>(true);
							Transform Cheek_L_Transform = HeadTransforms.FirstOrDefault(Item => AvatarUtility.CheekLeftBoneNames.Contains(Item.name));
							Transform Cheek_R_Transform = HeadTransforms.FirstOrDefault(Item => AvatarUtility.CheekRightBoneNames.Contains(Item.name));
							if (Cheek_L_Transform && TargetComponent.TargetCheek_L) {
								TargetComponent.TargetCheek_L.rootTransform = Cheek_L_Transform;
								EditorUtility.SetDirty(TargetComponent.TargetCheek_L);
							}
							if (Cheek_R_Transform && TargetComponent.TargetCheek_R) {
								TargetComponent.TargetCheek_R.rootTransform = Cheek_R_Transform;
								EditorUtility.SetDirty(TargetComponent.TargetCheek_R);
							}
						} else {
							Transform[] Foot_L_Transform = TargetAnimator.GetBoneTransform(HumanBodyBones.LeftFoot).GetComponentsInChildren<Transform>(true);
							Transform[] Foot_R_Transform = TargetAnimator.GetBoneTransform(HumanBodyBones.RightFoot).GetComponentsInChildren<Transform>(true);
							Transform Toe_L_Transform = TargetAnimator.GetBoneTransform(HumanBodyBones.LeftToes);
							Transform Toe_R_Transform = TargetAnimator.GetBoneTransform(HumanBodyBones.RightToes);
							List<Transform> Toe_L_ChildTransforms = new List<Transform>();
							List<Transform> Toe_R_ChildTransforms = new List<Transform>();
							if (!Toe_L_Transform) Toe_L_Transform = Foot_L_Transform.FirstOrDefault(Item => AvatarUtility.ToeLeftBoneNames.Contains(Item.name, StringComparer.OrdinalIgnoreCase));
							if (!Toe_R_Transform) Toe_R_Transform = Foot_R_Transform.FirstOrDefault(Item => AvatarUtility.ToeRightBoneNames.Contains(Item.name, StringComparer.OrdinalIgnoreCase));
							if (Toe_L_Transform) Toe_L_ChildTransforms = Toe_L_Transform.GetComponentsInChildren<Transform>(true).Where(Item => Item.parent == Toe_L_Transform).ToList();
							if (Toe_R_Transform) Toe_R_ChildTransforms = Toe_R_Transform.GetComponentsInChildren<Transform>(true).Where(Item => Item.parent == Toe_R_Transform).ToList();
							Transform ThumbToe1_L_Transform = Foot_L_Transform.FirstOrDefault(Item => AvatarUtility.ToeBoneDictionary["ThumbToe1_L"].Contains(Item.name));
							Transform ThumbToe1_R_Transform = Foot_R_Transform.FirstOrDefault(Item => AvatarUtility.ToeBoneDictionary["ThumbToe1_R"].Contains(Item.name));
							Transform IndexToe1_L_Transform = Foot_L_Transform.FirstOrDefault(Item => AvatarUtility.ToeBoneDictionary["IndexToe1_L"].Contains(Item.name));
							Transform IndexToe1_R_Transform = Foot_R_Transform.FirstOrDefault(Item => AvatarUtility.ToeBoneDictionary["IndexToe1_R"].Contains(Item.name));
							Transform MiddleToe1_L_Transform = Foot_L_Transform.FirstOrDefault(Item => AvatarUtility.ToeBoneDictionary["MiddleToe1_L"].Contains(Item.name));
							Transform MiddleToe1_R_Transform = Foot_R_Transform.FirstOrDefault(Item => AvatarUtility.ToeBoneDictionary["MiddleToe1_R"].Contains(Item.name));
							Transform RingToe1_L_Transform = Foot_L_Transform.FirstOrDefault(Item => AvatarUtility.ToeBoneDictionary["RingToe1_L"].Contains(Item.name));
							Transform RingToe1_R_Transform = Foot_R_Transform.FirstOrDefault(Item => AvatarUtility.ToeBoneDictionary["RingToe1_R"].Contains(Item.name));
							Transform LittleToe1_L_Transform = Foot_L_Transform.FirstOrDefault(Item => AvatarUtility.ToeBoneDictionary["LittleToe1_L"].Contains(Item.name));
							Transform LittleToe1_R_Transform = Foot_R_Transform.FirstOrDefault(Item => AvatarUtility.ToeBoneDictionary["LittleToe1_R"].Contains(Item.name));
							if (Toe_L_Transform && TargetComponent.TargetToe_L) {
								TargetComponent.TargetToe_L.rootTransform = Toe_L_Transform;
								if (Toe_L_ChildTransforms.Count > 0) TargetComponent.TargetToe_L.ignoreTransforms.AddRange(Toe_L_ChildTransforms);
								EditorUtility.SetDirty(TargetComponent.TargetToe_L);
							}
							if (Toe_R_Transform && TargetComponent.TargetToe_R) {
								TargetComponent.TargetToe_R.rootTransform = Toe_R_Transform;
								if (Toe_R_ChildTransforms.Count > 0) TargetComponent.TargetToe_R.ignoreTransforms.AddRange(Toe_R_ChildTransforms);
								EditorUtility.SetDirty(TargetComponent.TargetToe_R);
							}
							if (ThumbToe1_L_Transform && TargetComponent.TargetThumbToe1_L) {
								TargetComponent.TargetThumbToe1_L.rootTransform = ThumbToe1_L_Transform;
								EditorUtility.SetDirty(TargetComponent.TargetThumbToe1_L);
							}
							if (ThumbToe1_R_Transform && TargetComponent.TargetThumbToe1_R) {
								TargetComponent.TargetThumbToe1_R.rootTransform = ThumbToe1_R_Transform;
								EditorUtility.SetDirty(TargetComponent.TargetThumbToe1_R);
							}
							if (IndexToe1_L_Transform && TargetComponent.TargetIndexToe1_L) {
								TargetComponent.TargetIndexToe1_L.rootTransform = IndexToe1_L_Transform;
								EditorUtility.SetDirty(TargetComponent.TargetIndexToe1_L);
							}
							if (IndexToe1_R_Transform && TargetComponent.TargetIndexToe1_R) {
								TargetComponent.TargetIndexToe1_R.rootTransform = IndexToe1_R_Transform;
								EditorUtility.SetDirty(TargetComponent.TargetIndexToe1_R);
							}
							if (MiddleToe1_L_Transform && TargetComponent.TargetMiddleToe1_L) {
								TargetComponent.TargetMiddleToe1_L.rootTransform = MiddleToe1_L_Transform;
								EditorUtility.SetDirty(TargetComponent.TargetMiddleToe1_L);
							}
							if (MiddleToe1_R_Transform && TargetComponent.TargetMiddleToe1_R) {
								TargetComponent.TargetMiddleToe1_R.rootTransform = MiddleToe1_R_Transform;
								EditorUtility.SetDirty(TargetComponent.TargetMiddleToe1_R);
							}
							if (RingToe1_L_Transform && TargetComponent.TargetRingToe1_L) {
								TargetComponent.TargetRingToe1_L.rootTransform = RingToe1_L_Transform;
								EditorUtility.SetDirty(TargetComponent.TargetRingToe1_L);
							}
							if (RingToe1_R_Transform && TargetComponent.TargetRingToe1_R) {
								TargetComponent.TargetRingToe1_R.rootTransform = RingToe1_R_Transform;
								EditorUtility.SetDirty(TargetComponent.TargetRingToe1_R);
							}
							if (LittleToe1_L_Transform && TargetComponent.TargetLittleToe1_L) {
								TargetComponent.TargetLittleToe1_L.rootTransform = LittleToe1_L_Transform;
								EditorUtility.SetDirty(TargetComponent.TargetLittleToe1_L);
							}
							if (LittleToe1_R_Transform && TargetComponent.TargetLittleToe1_R) {
								TargetComponent.TargetLittleToe1_R.rootTransform = LittleToe1_R_Transform;
								EditorUtility.SetDirty(TargetComponent.TargetLittleToe1_R);
							}
						}
					}
				}
				foreach (PhysBoneConnector TargetComponent in PhysBoneConnectors) {
					if (TargetComponent) Object.DestroyImmediate(TargetComponent);
				}
			}
		}
	}

	[CustomEditor(typeof(PhysBoneConnector))]
	public class PhysBoneConnectorEditor : UnityEditor.Editor {

		SerializedProperty SerializedTargetType;

		SerializedProperty SerializedTargetCheek_L;
		SerializedProperty SerializedTargetCheek_R;

		SerializedProperty SerializedTargetToe_L;
		SerializedProperty SerializedTargetToe_R;

		SerializedProperty SerializedTargetThumbToe1_L;
		SerializedProperty SerializedTargetThumbToe1_R;
		SerializedProperty SerializedTargetIndexToe1_L;
		SerializedProperty SerializedTargetIndexToe1_R;
		SerializedProperty SerializedTargetMiddleToe1_L;
		SerializedProperty SerializedTargetMiddleToe1_R;
		SerializedProperty SerializedTargetRingToe1_L;
		SerializedProperty SerializedTargetRingToe1_R;
		SerializedProperty SerializedTargetLittleToe1_L;
		SerializedProperty SerializedTargetLittleToe1_R;

		void OnEnable() {
			SerializedTargetType = serializedObject.FindProperty("TargetType");

			SerializedTargetCheek_L = serializedObject.FindProperty("TargetCheek_L");
			SerializedTargetCheek_R = serializedObject.FindProperty("TargetCheek_R");

			SerializedTargetToe_L = serializedObject.FindProperty("TargetToe_L");
			SerializedTargetToe_R = serializedObject.FindProperty("TargetToe_R");

			SerializedTargetThumbToe1_L = serializedObject.FindProperty("TargetThumbToe1_L");
			SerializedTargetThumbToe1_R = serializedObject.FindProperty("TargetThumbToe1_R");
			SerializedTargetIndexToe1_L = serializedObject.FindProperty("TargetIndexToe1_L");
			SerializedTargetIndexToe1_R = serializedObject.FindProperty("TargetIndexToe1_R");
			SerializedTargetMiddleToe1_L = serializedObject.FindProperty("TargetMiddleToe1_L");
			SerializedTargetMiddleToe1_R = serializedObject.FindProperty("TargetMiddleToe1_R");
			SerializedTargetRingToe1_L = serializedObject.FindProperty("TargetRingToe1_L");
			SerializedTargetRingToe1_R = serializedObject.FindProperty("TargetRingToe1_R");
			SerializedTargetLittleToe1_L = serializedObject.FindProperty("TargetLittleToe1_L");
			SerializedTargetLittleToe1_R = serializedObject.FindProperty("TargetLittleToe1_R");
		}

		public override void OnInspectorGUI() {
			serializedObject.Update();
			LanguageIndex = EditorGUILayout.Popup(GetTranslatedString("String_Language"), LanguageIndex, LanguageOption);
			EditorGUILayout.Space(EditorGUIUtility.singleLineHeight);
			SerializedTargetType.enumValueIndex = EditorGUILayout.Popup(GetTranslatedString("String_PhysBoneType"), SerializedTargetType.enumValueIndex, GetPhysBoneOption());
			EditorGUILayout.Space();
			PhysBoneType TargetType = (PhysBoneType)SerializedTargetType.enumValueIndex;
			if (TargetType == 0) {
				using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox)) {
					EditorGUILayout.PropertyField(SerializedTargetCheek_L, new GUIContent(GetTranslatedString("String_LeftCheek")));
					EditorGUILayout.PropertyField(SerializedTargetCheek_R, new GUIContent(GetTranslatedString("String_RightCheek")));
				}
			} else {
				using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox)) {
					EditorGUILayout.PropertyField(SerializedTargetToe_L, new GUIContent(GetTranslatedString("String_LeftToe")));
					EditorGUILayout.PropertyField(SerializedTargetToe_R, new GUIContent(GetTranslatedString("String_RightToe")));
				}
				using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox)) {
					EditorGUILayout.PropertyField(SerializedTargetThumbToe1_L, new GUIContent(GetTranslatedString("String_LeftThumbToe")));
					EditorGUILayout.PropertyField(SerializedTargetThumbToe1_R, new GUIContent(GetTranslatedString("String_RightThumbToe")));
				}
				using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox)) {
					EditorGUILayout.PropertyField(SerializedTargetIndexToe1_L, new GUIContent(GetTranslatedString("String_LeftIndexToe")));
					EditorGUILayout.PropertyField(SerializedTargetIndexToe1_R, new GUIContent(GetTranslatedString("String_RightIndexToe")));
				}
				using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox)) {
					EditorGUILayout.PropertyField(SerializedTargetMiddleToe1_L, new GUIContent(GetTranslatedString("String_LeftMiddleToe")));
					EditorGUILayout.PropertyField(SerializedTargetMiddleToe1_R, new GUIContent(GetTranslatedString("String_RightMiddleToe")));
				}
				using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox)) {
					EditorGUILayout.PropertyField(SerializedTargetRingToe1_L, new GUIContent(GetTranslatedString("String_LeftRingToe")));
					EditorGUILayout.PropertyField(SerializedTargetRingToe1_R, new GUIContent(GetTranslatedString("String_RightRingToe")));
				}
				using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox)) {
					EditorGUILayout.PropertyField(SerializedTargetLittleToe1_L, new GUIContent(GetTranslatedString("String_LeftLittleToe")));
					EditorGUILayout.PropertyField(SerializedTargetLittleToe1_R, new GUIContent(GetTranslatedString("String_RightLittleToe")));
				}
			}
			serializedObject.ApplyModifiedProperties();
		}
	}
}
#endif