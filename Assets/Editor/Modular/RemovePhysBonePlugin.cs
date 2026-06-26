#if MODULAR_AVATAR
using System;
using System.Collections.Generic;
using System.Linq;

using UnityEditor;

using VRC.SDK3.Dynamics.PhysBone.Components;

using nadena.dev.ndmf;

using VRSuya.Core;
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
				VRCPhysBone[] PrefabPhysBoneComponents = RemovePhysBoneComponents.SelectMany(Item => Item.gameObject.GetComponentsInChildren<VRCPhysBone>(true)).ToArray();
				VRCPhysBone[] AvatarPhysBoneComponents = TargetBuildContext.AvatarRootObject.GetComponentsInChildren<VRCPhysBone>(true)
					.Where(Item => !PrefabPhysBoneComponents.Contains(Item)).ToArray();
				List<VRCPhysBone> TargetPhysBoneComponents = new List<VRCPhysBone>();
				TargetPhysBoneComponents.AddRange(AvatarPhysBoneComponents
					.Where(Item => Item.rootTransform != null)
					.Where(Item => AvatarUtility.CheekBoneNames.Contains(Item.rootTransform.gameObject.name, StringComparer.OrdinalIgnoreCase)));
				TargetPhysBoneComponents.AddRange(AvatarPhysBoneComponents
					.Where(Item => AvatarUtility.CheekBoneNames.Contains(Item.gameObject.name, StringComparer.OrdinalIgnoreCase)));
				foreach (VRCPhysBone TargetComponent in TargetPhysBoneComponents) {
					if (TargetComponent) Object.DestroyImmediate(TargetComponent);
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