#if MODULAR_AVATAR
using System;
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

[assembly: ExportsPlugin(typeof(VRSuya.Modular.Editor.RemovePhysBonePlugin))]

namespace VRSuya.Modular.Editor {

    public class RemovePhysBonePlugin : Plugin<RemovePhysBonePlugin> {

		public override string QualifiedName => "com.vrsuya.modular.removephysbone";
		public override string DisplayName => "VRSuya RemovePhysBone";

		protected override void Configure() {
			InPhase(BuildPhase.Optimizing).Run(RemovePhysBonePass.Instance);
		}
	}

	public class RemovePhysBonePass : Pass<RemovePhysBonePass> {

		public override string DisplayName => "RemovePhysBone";

		protected override void Execute(BuildContext TargetBuildContext) {
			RemovePhysBone[] RemovePhysBoneComponents = TargetBuildContext.AvatarRootObject.GetComponentsInChildren<RemovePhysBone>(true);
			if (RemovePhysBoneComponents.Length > 0) {
				TargetBuildContext.AvatarRootObject.TryGetComponent(out Animator TargetAnimator);
				if (TargetAnimator) {
					string[] Cheek_L_Names = new string[] { "Cheek.L", "Cheek1_L", "Cheek_Root_L", "Cheek_root_L", "Hoppe.L" };
					string[] Cheek_R_Names = new string[] { "Cheek.R", "Cheek1_R", "Cheek_Root_R", "Cheek_root_R", "Hoppe.R" };
					Transform[] HeadTransforms = TargetAnimator.GetBoneTransform(HumanBodyBones.Head).GetComponentsInChildren<Transform>(true);
					Transform Cheek_L_Transform = HeadTransforms.FirstOrDefault(Item => Cheek_L_Names.Contains(Item.name));
					Transform Cheek_R_Transform = HeadTransforms.FirstOrDefault(Item => Cheek_R_Names.Contains(Item.name));
					VRCPhysBone[] Cheek_PhysBoneComponents =
						Cheek_L_Transform.GetComponentsInChildren<VRCPhysBone>(true)
						.Concat(Cheek_R_Transform.GetComponentsInChildren<VRCPhysBone>(true))
						.ToArray();
					if (Cheek_PhysBoneComponents.Length > 0) {
						foreach (VRCPhysBone TargetComponent in Cheek_PhysBoneComponents) {
							if (TargetComponent) Object.DestroyImmediate(TargetComponent);
						}
					}
				}
				foreach (RemovePhysBone TargetComponent in RemovePhysBoneComponents) {
					if (TargetComponent) Object.DestroyImmediate(TargetComponent);
				}
			}
		}
	}

	[CustomEditor(typeof(RemovePhysBone))]
	public class RemovePhysBoneEditor : UnityEditor.Editor {

		public override void OnInspectorGUI() {
			LanguageIndex = EditorGUILayout.Popup(GetTranslatedString("String_Language"), LanguageIndex, LanguageOption);
			EditorGUILayout.Space();
			EditorGUILayout.HelpBox(GetTranslatedString("String_RemovePhysBone"), MessageType.Info);
		}
	}
}
#endif