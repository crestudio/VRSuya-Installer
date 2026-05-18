#if MODULAR_AVATAR
using System.Linq;

using UnityEditor;
using UnityEditor.Animations;

using VRC.SDK3.Avatars.Components;

using nadena.dev.ndmf;

using static VRSuya.Core.Translator;

using Avatar = VRSuya.Core.Avatar;
using Object = UnityEngine.Object;

/*
 * VRSuya Modular Component
 * Contact : vrsuya@gmail.com // Twitter : https://twitter.com/VRSuya
 */

[assembly: ExportsPlugin(typeof(VRSuya.Modular.Editor.RemoveFXMaskPlugin))]

namespace VRSuya.Modular.Editor {

    public class RemoveFXMaskPlugin : Plugin<RemoveFXMaskPlugin> {

		public override string QualifiedName => "com.vrsuya.modular.removefxmask";
		public override string DisplayName => "VRSuya RemoveFXMask";

		protected override void Configure() {
			InPhase(BuildPhase.Generating).Run(RemoveFXMaskPass.Instance);
		}
	}

	public class RemoveFXMaskPass : Pass<RemoveFXMaskPass> {

		public override string DisplayName => "RemoveFXMask";

		protected override void Execute(BuildContext TargetBuildContext) {
			RemoveFXMask[] RemoveFXMaskComponents = TargetBuildContext.AvatarRootObject.GetComponentsInChildren<RemoveFXMask>(true);
			if (RemoveFXMaskComponents.Length > 0) {
				Avatar AvatarInstance = new Avatar();
				AnimatorController TargetAnimator = AvatarInstance.GetAnimatorController(TargetBuildContext.AvatarRootObject, VRCAvatarDescriptor.AnimLayerType.FX);
				if (TargetAnimator) {
					if (RemoveFXMask(TargetAnimator)) {
						AssetDatabase.SaveAssets();
					}
				}
				foreach (RemoveFXMask TargetComponent in RemoveFXMaskComponents) {
					if (TargetComponent) Object.DestroyImmediate(TargetComponent);
				}
			}
		}

		bool RemoveFXMask(AnimatorController TargetAnimator) {
			if (TargetAnimator.layers.Length > 0) {
				int FXMaskCount = TargetAnimator.layers.Count(Item => Item.avatarMask != null);
				if (FXMaskCount > 0) {
					AnimatorControllerLayer[] NewAnimatorLayers = new AnimatorControllerLayer[TargetAnimator.layers.Length];
					for (int Index = 0; Index < TargetAnimator.layers.Length; Index++) {
						AnimatorControllerLayer NewLayer = new AnimatorControllerLayer {
							avatarMask = null,
							blendingMode = TargetAnimator.layers[Index].blendingMode,
							defaultWeight = TargetAnimator.layers[Index].defaultWeight,
							iKPass = TargetAnimator.layers[Index].iKPass,
							name = TargetAnimator.layers[Index].name,
							stateMachine = TargetAnimator.layers[Index].stateMachine,
							syncedLayerAffectsTiming = TargetAnimator.layers[Index].syncedLayerAffectsTiming,
							syncedLayerIndex = TargetAnimator.layers[Index].syncedLayerIndex
						};
						NewAnimatorLayers[Index] = NewLayer;
					}
					TargetAnimator.layers = NewAnimatorLayers;
					EditorUtility.SetDirty(TargetAnimator);
					return true;
				}
			}
			return false;
		}
	}

	[CustomEditor(typeof(RemoveFXMask))]
	public class RemoveFXMaskEditor : UnityEditor.Editor {

		public override void OnInspectorGUI() {
			LanguageIndex = EditorGUILayout.Popup(GetTranslatedString("String_Language"), LanguageIndex, LanguageOption);
			EditorGUILayout.Space();
			EditorGUILayout.HelpBox(GetTranslatedString("String_RemoveFXMask"), MessageType.Info);
		}
	}
}
#endif