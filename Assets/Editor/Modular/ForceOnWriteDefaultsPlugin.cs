#if MODULAR_AVATAR
using System.Linq;

using UnityEngine;
using UnityEditor;
using UnityEditor.Animations;

using VRC.SDK3.Avatars.Components;

using nadena.dev.ndmf;

using VRSuya.Core;
using static VRSuya.Core.Translator;

/*
 * VRSuya Modular Component
 * Contact : vrsuya@gmail.com // Twitter : https://twitter.com/VRSuya
 */

[assembly: ExportsPlugin(typeof(VRSuya.Modular.Editor.ForceOnWriteDefaultsPlugin))]

namespace VRSuya.Modular.Editor {

    public class ForceOnWriteDefaultsPlugin : Plugin<ForceOnWriteDefaultsPlugin> {

		public override string QualifiedName => "com.vrsuya.modular.forceonwritedefaults";
		public override string DisplayName => "VRSuya ForceOnWriteDefaults";

		protected override void Configure() {
			InPhase(BuildPhase.Generating).Run(ForceOnWriteDefaultsPass.Instance);
		}
	}

	public class ForceOnWriteDefaultsPass : Pass<ForceOnWriteDefaultsPass> {

		public override string DisplayName => "ForceOnWriteDefaults";

		protected override void Execute(BuildContext TargetBuildContext) {
			ForceOnWriteDefaults[] ForceOnWriteDefaultsComponents = TargetBuildContext.AvatarRootObject.GetComponentsInChildren<ForceOnWriteDefaults>(true);
			if (ForceOnWriteDefaultsComponents.Length > 0) {
				AnimatorController TargetAnimator = AvatarUtility.GetAnimatorController(TargetBuildContext.AvatarRootObject, VRCAvatarDescriptor.AnimLayerType.FX);
				if (TargetAnimator) {
					if (ForceOnWriteDefaults(TargetAnimator)) {
						AssetDatabase.SaveAssets();
					}
				}
				foreach (ForceOnWriteDefaults TargetComponent in ForceOnWriteDefaultsComponents) {
					if (TargetComponent) Object.DestroyImmediate(TargetComponent);
				}
			}
		}

		bool ForceOnWriteDefaults(AnimatorController TargetAnimator) {
			if (TargetAnimator.layers.Length > 0) {
				AnimatorState[] WDOffStates = AnimatorHelper.GetAllAnimatorStates(TargetAnimator)
					.Where(Item => Item != null)
					.Where(Item => Item.writeDefaultValues == false)
					.ToArray();
				if (WDOffStates.Length > 0) {
					foreach (AnimatorState TargetState in WDOffStates) {
						TargetState.writeDefaultValues = true;
						EditorUtility.SetDirty(TargetState);
					}
					return true;
				}
			}
			return false;
		}
	}

	[CustomEditor(typeof(ForceOnWriteDefaults))]
	public class ForceOnWriteDefaultsEditor : UnityEditor.Editor {

		public override void OnInspectorGUI() {
			LanguageIndex = EditorGUILayout.Popup(GetTranslatedString("String_Language"), LanguageIndex, LanguageOption);
			EditorGUILayout.Space();
			EditorGUILayout.HelpBox(GetTranslatedString("String_ForceOnWriteDefaults"), MessageType.Warning);
		}
	}
}
#endif