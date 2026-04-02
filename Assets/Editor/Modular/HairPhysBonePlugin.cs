#if MODULAR_AVATAR
using System.Collections.Generic;
using System.Linq;

using UnityEditor;
using UnityEngine;

using VRC.SDK3.Dynamics.PhysBone.Components;

using nadena.dev.ndmf;

using static VRSuya.Core.Translator;

using Object = UnityEngine.Object;

/*
 * VRSuya Modular Component
 * Contact : vrsuya@gmail.com // Twitter : https://twitter.com/VRSuya
 */

[assembly: ExportsPlugin(typeof(VRSuya.Modular.Editor.HairPhysBonePlugin))]

namespace VRSuya.Modular.Editor {

    public class HairPhysBonePlugin : Plugin<HairPhysBonePlugin> {

		public override string QualifiedName => "com.vrsuya.modular.hairphysbone";
		public override string DisplayName => "VRSuya Hair PhysBone";

		protected override void Configure() {
			InPhase(BuildPhase.Optimizing).Run(HairPhysBonePass.Instance);
		}
	}

	public class HairPhysBonePass : Pass<HairPhysBonePass> {

		public override string DisplayName => "Hair PhysBone";

		protected override void Execute(BuildContext TargetBuildContext) {
			HairPhysBone[] HairPhysBoneComponents = TargetBuildContext.AvatarRootObject.GetComponentsInChildren<HairPhysBone>(true);
			if (HairPhysBoneComponents.Length > 0) {
				List<string> TargetPhysBoneNames = new List<string>();
				foreach (HairPhysBone TargetComponent in HairPhysBoneComponents) {
					if (TargetComponent) {
						foreach (string LayerName in TargetComponent.TargetPhysBoneName) {
							if (!string.IsNullOrEmpty(LayerName) && !TargetPhysBoneNames.Contains(LayerName)) {
								TargetPhysBoneNames.Add(LayerName);
							}
						}
					}
				}
				if (TargetPhysBoneNames.Count > 0) {
					TargetBuildContext.AvatarRootObject.TryGetComponent(out Animator TargetAnimator);
					if (TargetAnimator) {
						VRCPhysBone[] HeadPhysBones = TargetAnimator.GetBoneTransform(HumanBodyBones.Head).GetComponentsInChildren<VRCPhysBone>(true);
						foreach (string TargetName in TargetPhysBoneNames) {
							VRCPhysBone TargetVRCPhysBones = HeadPhysBones.FirstOrDefault(Item => Item.gameObject.name == "TargetName");
							if (TargetVRCPhysBones) {
								if (!TargetVRCPhysBones.isAnimated) TargetVRCPhysBones.isAnimated = true;
							}
						}
					}
				}
				foreach (HairPhysBone TargetComponent in HairPhysBoneComponents) {
					if (TargetComponent) Object.DestroyImmediate(TargetComponent);
				}
			}
		}
	}

	[CustomEditor(typeof(HairPhysBone))]
	public class HairPhysBoneEditor : UnityEditor.Editor {

		SerializedProperty SerializedTargetPhysBoneName;

		void OnEnable() {
			SerializedTargetPhysBoneName = serializedObject.FindProperty("TargetPhysBoneName");
		}

		public override void OnInspectorGUI() {
			serializedObject.Update();
			LanguageIndex = EditorGUILayout.Popup(GetTranslatedString("String_Language"), LanguageIndex, LanguageOption);
			EditorGUILayout.Space();
			EditorGUILayout.PropertyField(SerializedTargetPhysBoneName, new GUIContent(GetTranslatedString("String_PhysBoneName")));
			serializedObject.ApplyModifiedProperties();
		}
	}
}
#endif