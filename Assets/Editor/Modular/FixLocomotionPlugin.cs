#if MODULAR_AVATAR
using System;
using System.Collections.Generic;
using System.Linq;

using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;

using VRC.SDK3.Avatars.Components;

using nadena.dev.ndmf;

using VRSuya.Core;
using static VRSuya.Core.Translator;

using Object = UnityEngine.Object;
using Random = System.Random;

/*
 * VRSuya Modular Component
 * Contact : vrsuya@gmail.com // Twitter : https://twitter.com/VRSuya
 */

[assembly: ExportsPlugin(typeof(VRSuya.Modular.Editor.FixLocomotionPlugin))]

namespace VRSuya.Modular.Editor {

    public class FixLocomotionPlugin : Plugin<FixLocomotionPlugin> {

		public override string QualifiedName => "com.vrsuya.modular.fixlocomotion";
		public override string DisplayName => "VRSuya FixLocomotion";

		protected override void Configure() {
			InPhase(BuildPhase.Generating).Run(FixLocomotionPass.Instance);
		}
	}

	public class FixLocomotionPass : Pass<FixLocomotionPass> {

		public override string DisplayName => "FixLocomotion";

		protected override void Execute(BuildContext TargetBuildContext) {
			FixLocomotion[] FixLocomotionComponents = TargetBuildContext.AvatarRootObject.GetComponentsInChildren<FixLocomotion>(true);
			if (FixLocomotionComponents.Length > 0) {
				AnimatorController TargetAnimator = AvatarUtility.GetAnimatorController(TargetBuildContext.AvatarRootObject, VRCAvatarDescriptor.AnimLayerType.Base);
				if (TargetAnimator) {
					string[] DefaultParameters = new string[] { "AFK", "VRCEmote", "Wotagei/Action/Type" };
					AnimationClip TargetAnimationClip = FixLocomotionComponents
						.Select(Item => Item.TargetAnimationClip)
						.FirstOrDefault(Item => Item != null);
					string[] TargetParameters = FixLocomotionComponents
						.SelectMany(Item => Item.TargetParameters)
						.Where(Item => !DefaultParameters.Contains(Item, StringComparer.OrdinalIgnoreCase))
						.Where(Item => !string.IsNullOrEmpty(Item))
						.ToArray();
					if (FixLocomotion(TargetAnimator, TargetAnimationClip, TargetParameters)) {
						AssetDatabase.SaveAssets();
					}
				}
				foreach (FixLocomotion TargetComponent in FixLocomotionComponents) {
					if (TargetComponent) Object.DestroyImmediate(TargetComponent);
				}
			}
		}

		bool FixLocomotion(AnimatorController TargetAnimator, AnimationClip TargetAnimationClip, string[] TargetParameters) {
			if (TargetAnimator.layers.Length > 0) {
				AnimatorStateMachine TargetStateMachine = TargetAnimator.layers[0].stateMachine;
				AnimatorState[] AllAnimatorStates = AnimatorHelper.GetAllStates(TargetStateMachine);
				AnimatorState StandingState = AnimatorHelper.GetStandingState(TargetAnimator);
				if (StandingState) {
					bool TargetWriteDefaults = AnimatorHelper.IsAnimatorWriteDefaults(TargetAnimator);
					AnimationClip NewStandingClip = AvatarUtility.GetStandingAnimation(TargetAnimator);
					if (!NewStandingClip) NewStandingClip = TargetAnimationClip;
					AnimatorState ActionState = GetActionState(TargetStateMachine, AllAnimatorStates, NewStandingClip, TargetWriteDefaults, TargetParameters);
					bool IsVerify = VerifyTransitions(ActionState.transitions, TargetParameters);
					if (!IsVerify) {
						SetStatePosition(TargetStateMachine, StandingState, ActionState);
						AnimatorStateTransition AnyStateToAction_AFK = TargetStateMachine.AddAnyStateTransition(ActionState);
						AnimatorStateTransition AnyStateToAction_Emote = TargetStateMachine.AddAnyStateTransition(ActionState);
						AnimatorStateTransition AnyStateToAction_Wotagei = TargetStateMachine.AddAnyStateTransition(ActionState);
						AnimatorStateTransition ActionToStanding = ActionState.AddTransition(StandingState);
						AddParameters(TargetAnimator, TargetParameters);
						SetTransition(AnyStateToAction_AFK, "AFK");
						SetTransition(AnyStateToAction_Emote, "VRCEmote");
						SetTransition(AnyStateToAction_Wotagei, "Wotagei/Action/Type");
						SetTransition(ActionToStanding, string.Empty, TargetParameters);
						foreach (string TargetParameter in TargetParameters) {
							AnimatorStateTransition AnyStateToAction_NewParameter = TargetStateMachine.AddAnyStateTransition(ActionState);
							SetTransition(AnyStateToAction_NewParameter, TargetParameter);
						}
						EditorUtility.SetDirty(TargetAnimator);
						return true;
					}
				}
			}
			return false;
		}

		AnimatorState GetActionState(AnimatorStateMachine TargetStateMachine, AnimatorState[] AllAnimatorStates, AnimationClip TargetAnimationClip, bool TargetWriteDefaults, string[] TargetNewParameters) {
			AnimatorState ActionState = AllAnimatorStates.FirstOrDefault(Item => Item.name == "Action");
			if (ActionState) {
				if (VerifyTransitions(ActionState.transitions, TargetNewParameters)) {
					return ActionState;
				} 
			}
			string NewStateName = "Action";
			if (ActionState) {
				Random RandomInstance = new Random();
				NewStateName = $"Action_{RandomInstance.Next(1000, 10000)}";
			}
			ActionState = TargetStateMachine.AddState(NewStateName);
			ActionState.motion = TargetAnimationClip;
			ActionState.writeDefaultValues = TargetWriteDefaults;
			return ActionState;
		}

		bool VerifyTransitions(AnimatorStateTransition[] TargetTransitions, string[] TargetNewParameters) {
			string[] TargetParameters = new string[] { "AFK", "VRCEmote", "Wotagei/Action/Type" }.Concat(TargetNewParameters).ToArray();
			foreach (AnimatorStateTransition TargetTransition in TargetTransitions) {
				string[] AllParameters = TargetTransition.conditions.Select(Item => Item.parameter).ToArray();
				bool IsAllContained = TargetParameters.All(item => AllParameters.Contains(item));
				if (IsAllContained) {
					return true;
				}
			}
			return false;
		}

		void SetStatePosition(AnimatorStateMachine TargetStateMachine, AnimatorState StandingState, AnimatorState ActionState) {
			ChildAnimatorState[] AllStates = TargetStateMachine.states;
			Vector3 StandingPosition = Vector3.zero;
			bool HasStanding = false;
			for (int Index = 0; Index < AllStates.Length; Index++) {
				if (AllStates[Index].state == StandingState) {
					StandingPosition = AllStates[Index].position;
					HasStanding = true;
					break;
				}
			}
			if (HasStanding) {
				for (int Index = 0; Index < AllStates.Length; Index++) {
					if (AllStates[Index].state == ActionState) {
						AllStates[Index].position = new Vector3(StandingPosition.x, StandingPosition.y - 100f, StandingPosition.z);
						break;
					}
				}
			}
			TargetStateMachine.states = AllStates;
		}

		void AddParameters(AnimatorController TargetAnimator, string[] TargetNewParameters) {
			List<AnimatorControllerParameter> NewAnimatorParameters = TargetAnimator.parameters.ToList();
			List<string> ParameterNames = TargetAnimator.parameters.Select(Item => Item.name).ToList();
			bool IsModifed = false;
			if (!ParameterNames.Contains("AFK")) {
				AnimatorControllerParameter AFKParameter = new AnimatorControllerParameter {
					name = "AFK",
					type = AnimatorControllerParameterType.Bool,
					defaultBool = false
				};
				NewAnimatorParameters.Add(AFKParameter);
				IsModifed = true;
			}
			if (!ParameterNames.Contains("VRCEmote")) {
				AnimatorControllerParameter VRCEmoteParameter = new AnimatorControllerParameter {
					name = "VRCEmote",
					type = AnimatorControllerParameterType.Int,
					defaultInt = 0
				};
				NewAnimatorParameters.Add(VRCEmoteParameter);
				IsModifed = true;
			}
			if (!ParameterNames.Contains("Wotagei/Action/Type")) {
				AnimatorControllerParameter WotageiParameter = new AnimatorControllerParameter {
					name = "Wotagei/Action/Type",
					type = AnimatorControllerParameterType.Int,
					defaultInt = 0
				};
				NewAnimatorParameters.Add(WotageiParameter);
				IsModifed = true;
			}
			foreach (string TargetParameter in TargetNewParameters) {
				AnimatorControllerParameter NewParameter = new AnimatorControllerParameter {
					name = TargetParameter,
					type = AnimatorControllerParameterType.Bool,
					defaultBool = false
				};
				NewAnimatorParameters.Add(NewParameter);
				IsModifed = true;
			}
			if (IsModifed) {
				TargetAnimator.parameters = NewAnimatorParameters.ToArray();
			}
		}

		void SetTransition(AnimatorStateTransition TargetTransition, string TargetParameter, string[] TargetParameters = null) {
			TargetTransition.hasExitTime = false;
			TargetTransition.exitTime = 0;
			TargetTransition.hasFixedDuration = true;
			TargetTransition.duration = 0.25f;
			TargetTransition.offset = 0;
			TargetTransition.interruptionSource = TransitionInterruptionSource.None;
			TargetTransition.canTransitionToSelf = false;
			if (TargetParameter == "AFK") {
				TargetTransition.AddCondition(AnimatorConditionMode.If, 1f, "AFK");
			} else if (TargetParameter == "VRCEmote") {
				TargetTransition.AddCondition(AnimatorConditionMode.Greater, 0f, "VRCEmote");
			} else if (TargetParameter == "Wotagei/Action/Type") {
				TargetTransition.AddCondition(AnimatorConditionMode.Greater, 0f, "Wotagei/Action/Type");
			} else if (!string.IsNullOrEmpty(TargetParameter)) {
				TargetTransition.AddCondition(AnimatorConditionMode.If, 1f, TargetParameter);
			} else {
				TargetTransition.AddCondition(AnimatorConditionMode.IfNot, 0f, "AFK");
				TargetTransition.AddCondition(AnimatorConditionMode.Equals, 0f, "VRCEmote");
				TargetTransition.AddCondition(AnimatorConditionMode.Equals, 0f, "Wotagei/Action/Type");
				foreach (string TargetNewParameter in TargetParameters) {
					TargetTransition.AddCondition(AnimatorConditionMode.IfNot, 0f, TargetNewParameter);
				}
			}
		}

	}

	[CustomEditor(typeof(FixLocomotion))]
	public class FixLocomotionEditor : UnityEditor.Editor {

		SerializedProperty SerializedTargetAnimationClip;
		SerializedProperty SerializedTargetParameters;

		void OnEnable() {
			SerializedTargetAnimationClip = serializedObject.FindProperty("TargetAnimationClip");
			SerializedTargetParameters = serializedObject.FindProperty("TargetParameters");
		}

		public override void OnInspectorGUI() {
			serializedObject.Update();
			LanguageIndex = EditorGUILayout.Popup(GetTranslatedString("String_Language"), LanguageIndex, LanguageOption);
			EditorGUILayout.Space();
			EditorGUILayout.PropertyField(SerializedTargetAnimationClip, new GUIContent(GetTranslatedString("String_AnimationClip")));
			EditorGUILayout.PropertyField(SerializedTargetParameters, new GUIContent(GetTranslatedString("String_Parameter")));
			serializedObject.ApplyModifiedProperties();
		}
	}
}
#endif