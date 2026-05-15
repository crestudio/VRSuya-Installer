#if MODULAR_AVATAR
using System;
using System.Collections.Generic;
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
			InPhase(BuildPhase.Optimizing).Run(FixLocomotionPass.Instance);
		}
	}

	public class FixLocomotionPass : Pass<FixLocomotionPass> {

		public override string DisplayName => "FixLocomotion";

		protected override void Execute(BuildContext TargetBuildContext) {
			FixLocomotion[] FixLocomotionComponents = TargetBuildContext.AvatarRootObject.GetComponentsInChildren<FixLocomotion>(true);
			if (FixLocomotionComponents.Length > 0) {
				Avatar AvatarInstance = new Avatar();
				AnimatorController TargetAnimator = AvatarInstance.GetAnimatorController(TargetBuildContext.AvatarRootObject, VRCAvatarDescriptor.AnimLayerType.Base);
				if (TargetAnimator) {
					AnimationClip TargetAnimationClip = FixLocomotionComponents
						.Select(Item => Item.TargetAnimationClip)
						.FirstOrDefault(Item => Item != null);
					if (TargetAnimationClip) {
						FixLocomotion(TargetAnimator, TargetAnimationClip);
					}
				}
				foreach (FixLocomotion TargetComponent in FixLocomotionComponents) {
					if (TargetComponent) Object.DestroyImmediate(TargetComponent);
				}
			}
		}

		void FixLocomotion(AnimatorController TargetAnimator, AnimationClip TargetAnimationClip) {
			if (TargetAnimator.layers.Length > 0) {
				Animator AnimatorInstance = new Animator();
				AnimatorStateMachine TargetStateMachine = TargetAnimator.layers[0].stateMachine;
				AnimatorState[] AllAnimatorStates = AnimatorInstance.GetAllStates(TargetStateMachine);
				AnimatorState StandingState = GetStandingState(TargetStateMachine, AllAnimatorStates);
				if (StandingState) {
					bool TargetWriteDefaults = GetWriteDefaults(AllAnimatorStates);
					AnimationClip NewStandingClip = GetStandingAnimation(StandingState, TargetAnimationClip);
					AnimatorState ActionState = GetActionState(TargetStateMachine, AllAnimatorStates, NewStandingClip, TargetWriteDefaults);
					bool IsVerify = VerifyTransitions(ActionState.transitions);
					if (!IsVerify) {
						SetStatePosition(TargetStateMachine, StandingState, ActionState);
						AnimatorStateTransition AnyStateToAction_AFK = TargetStateMachine.AddAnyStateTransition(ActionState);
						AnimatorStateTransition AnyStateToAction_Emote = TargetStateMachine.AddAnyStateTransition(ActionState);
						AnimatorStateTransition AnyStateToAction_Wotagei = TargetStateMachine.AddAnyStateTransition(ActionState);
						AnimatorStateTransition ActionToStanding = ActionState.AddTransition(StandingState);
						AddParameters(TargetAnimator);
						SetTransition(AnyStateToAction_AFK, "AFK");
						SetTransition(AnyStateToAction_Emote, "VRCEmote");
						SetTransition(AnyStateToAction_Wotagei, "Wotagei/Action/Type");
						SetTransition(ActionToStanding);
					}
				}
			}
		}

		AnimatorState GetStandingState(AnimatorStateMachine TargetStateMachine, AnimatorState[] AllAnimatorStates) {
			AnimatorState StandingState = AllAnimatorStates.FirstOrDefault(Item => Item.name == "Standing");
			if (StandingState) return StandingState;
			StandingState = AllAnimatorStates.FirstOrDefault(Item => Item.name.Contains("Stand", StringComparison.OrdinalIgnoreCase));
			if (StandingState) return StandingState;
			if (TargetStateMachine.defaultState) {
				return TargetStateMachine.defaultState;
			} else {
				return null;
			}
		}

		AnimatorState GetActionState(AnimatorStateMachine TargetStateMachine, AnimatorState[] AllAnimatorStates, AnimationClip TargetAnimationClip, bool TargetWriteDefaults) {
			AnimatorState ActionState = AllAnimatorStates.FirstOrDefault(Item => Item.name == "Action");
			if (ActionState) {
				if (VerifyTransitions(ActionState.transitions)) {
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

		AnimationClip GetStandingAnimation(AnimatorState TargetStandingState, AnimationClip TargetAnimationClip) {
			if (TargetStandingState.motion && TargetStandingState.motion is BlendTree) {
				BlendTree StandingBlendTree = TargetStandingState.motion as BlendTree;
				ChildMotion[] StandingMotion = StandingBlendTree.children.Where(Item => Item.position == new Vector2(0f, 0f)).ToArray();
				if (StandingMotion.Length > 0) {
					if (StandingMotion[0].motion && StandingMotion[0].motion is AnimationClip) {
						return StandingMotion[0].motion as AnimationClip;
					}
				}
			}
			return TargetAnimationClip;
		}

		bool GetWriteDefaults(AnimatorState[] AllAnimatorStates) {
			bool[] WriteDefaults = AllAnimatorStates.Select(Item => Item.writeDefaultValues).ToArray();
			int WriteDefaultsOffCount = WriteDefaults.Where(Item => Item == false).Count();
			bool ResultWriteDefaults = ((WriteDefaultsOffCount / WriteDefaults.Length) <= 0.5) ? true : false;
			return ResultWriteDefaults;
		}

		bool VerifyTransitions(AnimatorStateTransition[] TargetTransitions) {
			string[] TargetParameters = new string[] { "AFK", "VRCEmote", "Wotagei/Action/Type" };
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

		void AddParameters(AnimatorController TargetAnimator) {
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
			if (IsModifed) {
				TargetAnimator.parameters = NewAnimatorParameters.ToArray();
			}
		}

		void SetTransition(AnimatorStateTransition TargetTransition, string TargetParameter = null) {
			TargetTransition.hasExitTime = false;
			TargetTransition.exitTime = 0;
			TargetTransition.hasFixedDuration = true;
			TargetTransition.duration = 0.25f;
			TargetTransition.offset = 0;
			TargetTransition.interruptionSource = TransitionInterruptionSource.None;
			TargetTransition.canTransitionToSelf = false;
			switch (TargetParameter) {
				case "AFK":
					TargetTransition.AddCondition(AnimatorConditionMode.If, 1f, "AFK");
					break;
				case "VRCEmote":
					TargetTransition.AddCondition(AnimatorConditionMode.Greater, 0f, "VRCEmote");
					break;
				case "Wotagei/Action/Type":
					TargetTransition.AddCondition(AnimatorConditionMode.Greater, 0f, "Wotagei/Action/Type");
					break;
				default:
					TargetTransition.AddCondition(AnimatorConditionMode.IfNot, 0f, "AFK");
					TargetTransition.AddCondition(AnimatorConditionMode.Equals, 0f, "VRCEmote");
					TargetTransition.AddCondition(AnimatorConditionMode.Equals, 0f, "Wotagei/Action/Type");
					break;
			}
		}
	}

	[CustomEditor(typeof(FixLocomotion))]
	public class FixLocomotionEditor : UnityEditor.Editor {

		SerializedProperty SerializedTargetAnimationClip;

		void OnEnable() {
			SerializedTargetAnimationClip = serializedObject.FindProperty("TargetAnimationClip");
		}

		public override void OnInspectorGUI() {
			serializedObject.Update();
			LanguageIndex = EditorGUILayout.Popup(GetTranslatedString("String_Language"), LanguageIndex, LanguageOption);
			EditorGUILayout.Space();
			EditorGUILayout.PropertyField(SerializedTargetAnimationClip, new GUIContent(GetTranslatedString("String_AnimationClip")));
			serializedObject.ApplyModifiedProperties();
		}
	}
}
#endif