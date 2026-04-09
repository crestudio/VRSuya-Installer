#if MODULAR_AVATAR
using System;
using System.Linq;

using UnityEditor;
using UnityEngine;

using nadena.dev.ndmf;

using static VRSuya.Core.Translator;

using Object = UnityEngine.Object;
using VRC.Dynamics;

/*
 * VRSuya Modular Component
 * Contact : vrsuya@gmail.com // Twitter : https://twitter.com/VRSuya
 */

[assembly: ExportsPlugin(typeof(VRSuya.Modular.Editor.ConstraintConnectorPlugin))]

namespace VRSuya.Modular.Editor {

    public class ConstraintConnectorPlugin : Plugin<ConstraintConnectorPlugin> {

		public override string QualifiedName => "com.vrsuya.modular.constraintconnector";
		public override string DisplayName => "VRSuya ConstraintConnector";

		protected override void Configure() {
			InPhase(BuildPhase.Resolving).Run(ConstraintConnectorPass.Instance);
		}
	}

	public class ConstraintConnectorPass : Pass<ConstraintConnectorPass> {

		public override string DisplayName => "ConstraintConnector";

		protected override void Execute(BuildContext TargetBuildContext) {
			ConstraintConnector[] ConstraintConnectors = TargetBuildContext.AvatarRootObject.GetComponentsInChildren<ConstraintConnector>(true);
			if (ConstraintConnectors.Length > 0) {
				TargetBuildContext.AvatarRootObject.TryGetComponent(out Animator TargetAnimator);
				if (TargetAnimator) {
					foreach (ConstraintConnector TargetComponent in ConstraintConnectors) {
						if (!TargetComponent) continue;
						if (TargetComponent.LeftHandConstraint) {
							Transform LeftHandTransform = TargetAnimator.GetBoneTransform(HumanBodyBones.LeftHand);
							if (TargetComponent.LeftHandConstraint.Sources.Count > 0) {
								VRCConstraintSource NewConstraintSource = new VRCConstraintSource {
									SourceTransform = LeftHandTransform,
									Weight = TargetComponent.LeftHandConstraint.Sources[0].Weight,
									ParentPositionOffset = TargetComponent.LeftHandConstraint.Sources[0].ParentPositionOffset,
									ParentRotationOffset = TargetComponent.LeftHandConstraint.Sources[0].ParentRotationOffset
								};
								TargetComponent.LeftHandConstraint.Sources[0] = NewConstraintSource;
							} else {
								VRCConstraintSource NewConstraintSource = new VRCConstraintSource {
									SourceTransform = LeftHandTransform,
									Weight = 1.0f
								};
								TargetComponent.LeftHandConstraint.Sources.Add(NewConstraintSource);
							}
						}
						if (TargetComponent.RightHandConstraint) {
							Transform RightHandTransform = TargetAnimator.GetBoneTransform(HumanBodyBones.RightHand);
							if (TargetComponent.RightHandConstraint.Sources.Count > 0) {
								VRCConstraintSource NewConstraintSource = new VRCConstraintSource {
									SourceTransform = RightHandTransform,
									Weight = TargetComponent.RightHandConstraint.Sources[0].Weight,
									ParentPositionOffset = TargetComponent.RightHandConstraint.Sources[0].ParentPositionOffset,
									ParentRotationOffset = TargetComponent.RightHandConstraint.Sources[0].ParentRotationOffset
								};
								TargetComponent.RightHandConstraint.Sources[0] = NewConstraintSource;
							} else {
								VRCConstraintSource NewConstraintSource = new VRCConstraintSource {
									SourceTransform = RightHandTransform,
									Weight = 1.0f
								};
								TargetComponent.RightHandConstraint.Sources.Add(NewConstraintSource);
							}
						}
					}
				}
				foreach (ConstraintConnector TargetComponent in ConstraintConnectors) {
					if (TargetComponent) Object.DestroyImmediate(TargetComponent);
				}
			}
		}
	}

	[CustomEditor(typeof(ConstraintConnector))]
	public class ConstraintConnectorEditor : UnityEditor.Editor {

		SerializedProperty SerializedLeftHandConstraint;
		SerializedProperty SerializedRightHandConstraint;

		void OnEnable() {
			SerializedLeftHandConstraint = serializedObject.FindProperty("LeftHandConstraint");
			SerializedRightHandConstraint = serializedObject.FindProperty("RightHandConstraint");
		}

		public override void OnInspectorGUI() {
			serializedObject.Update();
			LanguageIndex = EditorGUILayout.Popup(GetTranslatedString("String_Language"), LanguageIndex, LanguageOption);
			EditorGUILayout.Space(EditorGUIUtility.singleLineHeight);
			EditorGUILayout.PropertyField(SerializedLeftHandConstraint, new GUIContent(GetTranslatedString("String_LeftHand")));
			EditorGUILayout.PropertyField(SerializedRightHandConstraint, new GUIContent(GetTranslatedString("String_RightHand")));
			serializedObject.ApplyModifiedProperties();
		}
	}
}
#endif