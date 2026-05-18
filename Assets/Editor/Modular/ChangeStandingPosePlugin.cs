#if MODULAR_AVATAR
using System;
using System.Linq;

using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;

using VRC.SDK3.Avatars.Components;

using nadena.dev.ndmf;

using static VRSuya.Core.Translator;

using Animator = VRSuya.Core.Animator;
using Avatar = VRSuya.Core.Avatar;
using Object = UnityEngine.Object;

/*
 * VRSuya Modular Component
 * Contact : vrsuya@gmail.com // Twitter : https://twitter.com/VRSuya
 */

[assembly: ExportsPlugin(typeof(VRSuya.Modular.Editor.ChangeStandingPosePlugin))]

namespace VRSuya.Modular.Editor {

    public class ChangeStandingPosePlugin : Plugin<ChangeStandingPosePlugin> {

		public override string QualifiedName => "com.vrsuya.modular.changestandingpose";
		public override string DisplayName => "VRSuya ChangeStandingPose";

		protected override void Configure() {
			InPhase(BuildPhase.Optimizing).Run(ChangeStandingPosePass.Instance);
		}
	}

	public class ChangeStandingPosePass : Pass<ChangeStandingPosePass> {

		public override string DisplayName => "ChangeStandingPose";

		protected override void Execute(BuildContext TargetBuildContext) {
			ChangeStandingPose[] ChangeStandingPoseComponents = TargetBuildContext.AvatarRootObject.GetComponentsInChildren<ChangeStandingPose>(true);
			if (ChangeStandingPoseComponents.Length > 0) {
				Avatar AvatarInstance = new Avatar();
				AnimatorController BaseAnimator = AvatarInstance.GetAnimatorController(TargetBuildContext.AvatarRootObject, VRCAvatarDescriptor.AnimLayerType.Base);
				AnimatorController ActionAnimator = AvatarInstance.GetAnimatorController(TargetBuildContext.AvatarRootObject, VRCAvatarDescriptor.AnimLayerType.Action);
				if (BaseAnimator && ActionAnimator) {
					AnimationClip TargetAnimationClip = AvatarInstance.GetStandingAnimation(BaseAnimator);
					if (TargetAnimationClip) {
						ChangeStandingPose(ActionAnimator, TargetAnimationClip);
						AssetDatabase.SaveAssets();
					}
				}
				foreach (ChangeStandingPose TargetComponent in ChangeStandingPoseComponents) {
					if (TargetComponent) Object.DestroyImmediate(TargetComponent);
				}
			}
		}

		void ChangeStandingPose(AnimatorController TargetAnimator, AnimationClip StandAnimationClip) {
			if (TargetAnimator.layers.Length > 0) {
				string[] TargetAnimationNames = new string[] { "proxy_stand_still", "VRSuya_Wotagei_Wotagei_Stand" };
				Animator AnimatorInstance = new Animator();
				AnimatorState[] TargetAnimatorStates = AnimatorInstance.GetAllAnimatorStates(TargetAnimator)
					.Where(Item => Item.motion != null)
					.Where(Item => TargetAnimationNames.Contains(Item.motion.name))
					.ToArray();
				foreach (AnimatorState TargetAnimatorState in TargetAnimatorStates) {
					TargetAnimatorState.motion = StandAnimationClip;
					EditorUtility.SetDirty(TargetAnimatorState);
				}
			}
		}
	}

	[CustomEditor(typeof(ChangeStandingPose))]
	public class ChangeStandingPoseEditor : UnityEditor.Editor {

		public override void OnInspectorGUI() {
			LanguageIndex = EditorGUILayout.Popup(GetTranslatedString("String_Language"), LanguageIndex, LanguageOption);
			EditorGUILayout.Space();
			EditorGUILayout.HelpBox(GetTranslatedString("String_ChangeStandingPose"), MessageType.Info);
		}
	}
}
#endif