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

[assembly: ExportsPlugin(typeof(VRSuya.Modular.Editor.DisablePhysBonePlugin))]

namespace VRSuya.Modular.Editor {

    public class DisablePhysBonePlugin : Plugin<DisablePhysBonePlugin> {

		public override string QualifiedName => "com.vrsuya.modular.disablephysbone";
		public override string DisplayName => "VRSuya Disable PhysBone";

		protected override void Configure() {
			InPhase(BuildPhase.Optimizing).Run(DisablePhysBonePass.Instance);
		}
	}

	public class DisablePhysBonePass : Pass<DisablePhysBonePass> {

		public override string DisplayName => "Disable PhysBone";

		protected override void Execute(BuildContext TargetBuildContext) {
			DisablePhysBone[] DisablePhysBoneComponents = TargetBuildContext.AvatarRootObject.GetComponentsInChildren<DisablePhysBone>(true);
			if (DisablePhysBoneComponents.Length > 0) {
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
				foreach (DisablePhysBone TargetComponent in DisablePhysBoneComponents) {
					if (TargetComponent) Object.DestroyImmediate(TargetComponent);
				}
			}
		}
	}

	[CustomEditor(typeof(DisablePhysBone))]
	public class DisablePhysBoneEditor : UnityEditor.Editor {

		public override void OnInspectorGUI() {
			LanguageIndex = EditorGUILayout.Popup(GetTranslatedString("String_Language"), LanguageIndex, LanguageOption);
			EditorGUILayout.Space();
			EditorGUILayout.HelpBox(GetTranslatedString("String_DisablePhysBone"), MessageType.Info);
		}
	}
}
