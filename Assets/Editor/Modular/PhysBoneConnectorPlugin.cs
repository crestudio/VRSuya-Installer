#if MODULAR_AVATAR
using System;
using System.Linq;

using UnityEditor;
using UnityEngine;

using nadena.dev.ndmf;

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
			InPhase(BuildPhase.Resolving).Run(PhysBoneConnectorPass.Instance);
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
							string[] Cheek_L_Names = new string[] { "Cheek.L", "Cheek1_L", "Cheek_Root_L", "Cheek_root_L", "Hoppe.L" };
							string[] Cheek_R_Names = new string[] { "Cheek.R", "Cheek1_R", "Cheek_Root_R", "Cheek_root_R", "Hoppe.R" };
							Transform[] HeadTransforms = TargetAnimator.GetBoneTransform(HumanBodyBones.Head).GetComponentsInChildren<Transform>(true);
							Transform Cheek_L_Transform = HeadTransforms.FirstOrDefault(Item => Cheek_L_Names.Contains(Item.name));
							Transform Cheek_R_Transform = HeadTransforms.FirstOrDefault(Item => Cheek_R_Names.Contains(Item.name));
							if (Cheek_L_Transform && TargetComponent.TargetCheek_L) {
								TargetComponent.TargetCheek_L.rootTransform = Cheek_L_Transform;
							}
							if (Cheek_R_Transform && TargetComponent.TargetCheek_R) {
								TargetComponent.TargetCheek_R.rootTransform = Cheek_R_Transform;
							}
						} else {
							Transform[] Toe_L_Transform = TargetAnimator.GetBoneTransform(HumanBodyBones.LeftToes).GetComponentsInChildren<Transform>(true);
							Transform[] Toe_R_Transform = TargetAnimator.GetBoneTransform(HumanBodyBones.RightToes).GetComponentsInChildren<Transform>(true);
							Transform ThumbToe1_L_Transform = Toe_L_Transform.FirstOrDefault(Item => Item.name == "ThumbToe1_L" || Item.name == "Toe_Thumb_Proximal_L");
							Transform ThumbToe1_R_Transform = Toe_R_Transform.FirstOrDefault(Item => Item.name == "ThumbToe1_R" || Item.name == "Toe_Thumb_Proximal_R");
							Transform IndexToe1_L_Transform = Toe_L_Transform.FirstOrDefault(Item => Item.name == "IndexToe1_L" || Item.name == "Toe_Index_Proximal_L");
							Transform IndexToe1_R_Transform = Toe_R_Transform.FirstOrDefault(Item => Item.name == "IndexToe1_R" || Item.name == "Toe_Index_Proximal_R");
							Transform MiddleToe1_L_Transform = Toe_L_Transform.FirstOrDefault(Item => Item.name == "MiddleToe1_L" || Item.name == "Toe_Middle_Proximal_L");
							Transform MiddleToe1_R_Transform = Toe_R_Transform.FirstOrDefault(Item => Item.name == "MiddleToe1_R" || Item.name == "Toe_Middle_Proximal_R");
							Transform RingToe1_L_Transform = Toe_L_Transform.FirstOrDefault(Item => Item.name == "RingToe1_L" || Item.name == "Toe_Ring_Proximal_L");
							Transform RingToe1_R_Transform = Toe_R_Transform.FirstOrDefault(Item => Item.name == "RingToe1_R" || Item.name == "Toe_Ring_Proximal_R");
							Transform LittleToe1_L_Transform = Toe_L_Transform.FirstOrDefault(Item => Item.name == "LittleToe1_L" || Item.name == "Toe_Little_Proximal_L");
							Transform LittleToe1_R_Transform = Toe_R_Transform.FirstOrDefault(Item => Item.name == "LittleToe1_R" || Item.name == "Toe_Little_Proximal_R");
							if (ThumbToe1_L_Transform && TargetComponent.TargetThumbToe1_L) {
								TargetComponent.TargetThumbToe1_L.rootTransform = ThumbToe1_L_Transform;
							}
							if (ThumbToe1_R_Transform && TargetComponent.TargetThumbToe1_R) {
								TargetComponent.TargetThumbToe1_R.rootTransform = ThumbToe1_R_Transform;
							}
							if (IndexToe1_L_Transform && TargetComponent.TargetIndexToe1_L) {
								TargetComponent.TargetIndexToe1_L.rootTransform = IndexToe1_L_Transform;
							}
							if (IndexToe1_R_Transform && TargetComponent.TargetIndexToe1_R) {
								TargetComponent.TargetIndexToe1_R.rootTransform = IndexToe1_R_Transform;
							}
							if (MiddleToe1_L_Transform && TargetComponent.TargetMiddleToe1_L) {
								TargetComponent.TargetMiddleToe1_L.rootTransform = MiddleToe1_L_Transform;
							}
							if (MiddleToe1_R_Transform && TargetComponent.TargetMiddleToe1_R) {
								TargetComponent.TargetMiddleToe1_R.rootTransform = MiddleToe1_R_Transform;
							}
							if (RingToe1_L_Transform && TargetComponent.TargetRingToe1_L) {
								TargetComponent.TargetRingToe1_L.rootTransform = RingToe1_L_Transform;
							}
							if (RingToe1_R_Transform && TargetComponent.TargetRingToe1_R) {
								TargetComponent.TargetRingToe1_R.rootTransform = RingToe1_R_Transform;
							}
							if (LittleToe1_L_Transform && TargetComponent.TargetLittleToe1_L) {
								TargetComponent.TargetLittleToe1_L.rootTransform = LittleToe1_L_Transform;
							}
							if (LittleToe1_R_Transform && TargetComponent.TargetLittleToe1_R) {
								TargetComponent.TargetLittleToe1_R.rootTransform = LittleToe1_R_Transform;
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
			EditorGUILayout.PropertyField(SerializedTargetType, new GUIContent(GetTranslatedString("String_PhysBoneType")));
			EditorGUILayout.Space();
			PhysBoneType TargetType = (PhysBoneType)SerializedTargetType.enumValueIndex;
			if (TargetType == 0) {
				using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox)) {
					EditorGUILayout.PropertyField(SerializedTargetCheek_L, new GUIContent(GetTranslatedString("String_LeftCheek")));
					EditorGUILayout.PropertyField(SerializedTargetCheek_R, new GUIContent(GetTranslatedString("String_RightCheek")));
				}
			} else {
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