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

/*
 * VRSuya Modular Component
 * Contact : vrsuya@gmail.com // Twitter : https://twitter.com/VRSuya
 */

[assembly: ExportsPlugin(typeof(VRSuya.Modular.Editor.FixFacialAnimationPlugin))]

namespace VRSuya.Modular.Editor {

    public class FixFacialAnimationPlugin : Plugin<FixFacialAnimationPlugin> {

		public override string QualifiedName => "com.vrsuya.modular.fixfacialanimation";
		public override string DisplayName => "VRSuya FixFacialAnimation";

		protected override void Configure() {
			InPhase(BuildPhase.Optimizing).Run(FixFacialAnimationPass.Instance);
		}
	}

	public class FixFacialAnimationPass : Pass<FixFacialAnimationPass> {

		public override string DisplayName => "FixFacialAnimation";

		static readonly string[] TargetStateNames = new string[] { "Sleeping_Animation", "Sleeping_NoAnimation", "WakeUp" };

		protected override void Execute(BuildContext TargetBuildContext) {
			FixFacialAnimation[] FixFacialAnimationComponents = TargetBuildContext.AvatarRootObject.GetComponentsInChildren<FixFacialAnimation>(true);
			if (FixFacialAnimationComponents.Length > 0) {
				AnimatorController TargetAnimator = AvatarUtility.GetAnimatorController(TargetBuildContext.AvatarRootObject, VRCAvatarDescriptor.AnimLayerType.FX);
				if (TargetAnimator) {
					bool IsModified = false;
					foreach (FixFacialAnimation TargetComponent in FixFacialAnimationComponents) {
						if (!TargetComponent) continue;
						if (FixAnimationClip(
								TargetBuildContext.AvatarRootObject,
								TargetComponent.TargetAnimationClips,
								TargetComponent.TargetBlendshapes,
								TargetComponent.AddBlinkBlendshape
							)) {
							IsModified = true;
						}
						if (TargetComponent.AddLayerControl) {
							if (FixAnimator(TargetAnimator, TargetComponent.TargetLayerIndexs)) {
								IsModified = true;
							}
						}
					}
					if (IsModified) {
						AssetDatabase.SaveAssets();
					}
				}
				foreach (FixFacialAnimation TargetComponent in FixFacialAnimationComponents) {
					if (TargetComponent) Object.DestroyImmediate(TargetComponent);
				}
			}
		}

		bool FixAnimationClip(GameObject AvatarGameObject, AnimationClip[] TargetAnimationClips, string[] TargetBlendshapes, bool AddBlinkBlendshape) {
			GameObject HeadGameObject = AvatarUtility.GetHeadGameObject(AvatarGameObject);
			VRCAvatarDescriptor TargetAvatarDescriptor = AvatarGameObject.GetComponent<VRCAvatarDescriptor>();
			SkinnedMeshRenderer HeadSkinnedMeshRenderer = TargetAvatarDescriptor.customEyeLookSettings.eyelidsSkinnedMesh;
			string FaceMeshName = (HeadSkinnedMeshRenderer) ? HeadSkinnedMeshRenderer.name : "Body";
			AnimationClip[] AvatarAnimationClips = AnimatorHelper.GetAllAvatarAnimationClips(AvatarGameObject);
			string[] NewBlendshapeNames = TargetBlendshapes;
			AnimationCurve TargetCurve = new AnimationCurve();
			Keyframe TargetKeyframe = new Keyframe(0f, 0f);
			TargetCurve.AddKey(TargetKeyframe);
			bool IsModified = false;
			if (AddBlinkBlendshape && TargetAvatarDescriptor && HeadSkinnedMeshRenderer) {
				int[] BlinkBlendshapeIndex = TargetAvatarDescriptor.customEyeLookSettings.eyelidsBlendshapes;
				if (BlinkBlendshapeIndex.Length > 0) {
					List<string> BlinkBlendshapeNames = new List<string>();
					foreach (int TargetIndex in BlinkBlendshapeIndex) {
						BlinkBlendshapeNames.Add(HeadSkinnedMeshRenderer.sharedMesh.GetBlendShapeName(TargetIndex));
					}
					NewBlendshapeNames = NewBlendshapeNames.Concat(BlinkBlendshapeNames).Distinct().ToArray();
				}
			}
			foreach (AnimationClip TargetAnimationClip in TargetAnimationClips) {
				AnimationClip BuildContextAnimationClip = AvatarAnimationClips.FirstOrDefault(Item => Item.name == TargetAnimationClip.name);
				if (!BuildContextAnimationClip) continue;
				string[] AnimationBlendshapeNames = AnimationUtility.GetCurveBindings(BuildContextAnimationClip)
					.Where(Item => Item != null)
					.Where(Item => Item.type == typeof(SkinnedMeshRenderer))
					.Where(Item => Item.path == FaceMeshName)
					.Select(Item => Item.propertyName.Remove(0, 11))
					.ToArray();
				string[] FinalBlendshapeNames = NewBlendshapeNames.Except(AnimationBlendshapeNames).ToArray();
				if (FinalBlendshapeNames.Length == 0) continue;
				foreach (string TargetName in FinalBlendshapeNames) {
					BuildContextAnimationClip.SetCurve(FaceMeshName, typeof(SkinnedMeshRenderer), $"blendShape.{TargetName}", TargetCurve);
				}
				EditorUtility.SetDirty(BuildContextAnimationClip);
				IsModified = true;
			}
			if (IsModified) {
				AssetDatabase.SaveAssets();
			}
			return false;
		}

		bool FixAnimator(AnimatorController TargetAnimator, int[] TargetLayerIndexs) {
			bool IsModified = false;
			AnimatorState[] TargetStates = AnimatorHelper.GetAllAnimatorStates(TargetAnimator)
				.Where(Item => TargetStateNames.Contains(Item.name))
				.ToArray();
			foreach (AnimatorState TargetState in TargetStates) {
				if (!TargetState) continue;
				VRCAnimatorLayerControl[] TargetVRCLayerControls = TargetState.behaviours
					.Where(Item => Item != null)
					.Where(Item => Item is VRCAnimatorLayerControl)
					.Select(Item => Item as VRCAnimatorLayerControl)
					.ToArray();
				int[] AnimatorLayerIndexs = TargetVRCLayerControls.Select(Item => Item.layer).ToArray();
				int[] FinalLayerIndex = TargetLayerIndexs.Except(AnimatorLayerIndexs).ToArray();
				if (FinalLayerIndex.Length == 0) continue;
				foreach (int TargetIndex in FinalLayerIndex) {
					VRCAnimatorLayerControl NewLayerControl = TargetState.AddStateMachineBehaviour<VRCAnimatorLayerControl>();
					NewLayerControl.playable = VRC.SDKBase.VRC_AnimatorLayerControl.BlendableLayer.FX;
					NewLayerControl.layer = TargetIndex;
					NewLayerControl.goalWeight = (TargetState.name.Contains("Sleeping")) ? 0f : 1f;
					NewLayerControl.blendDuration = (TargetState.name.Contains("Sleeping")) ? 0.5f : 3f;
					EditorUtility.SetDirty(TargetState);
					IsModified = true;
				}
			}
			return IsModified;
		}
	}

	[CustomEditor(typeof(FixFacialAnimation))]
	public class FixFacialAnimationEditor : UnityEditor.Editor {

		SerializedProperty SerializedTargetAnimationClips;
		SerializedProperty SerializedAddBlinkBlendshape;
		SerializedProperty SerializedTargetBlendshapes;
		SerializedProperty SerializedAddLayerControl;
		SerializedProperty SerializedTargetLayerIndexs;

		string[] AvatarBlendshapes = new string[0];
		string[] ComponentBlendshapes = new string[0];

		void OnEnable() {
			SerializedTargetAnimationClips = serializedObject.FindProperty("TargetAnimationClips");
			SerializedAddBlinkBlendshape = serializedObject.FindProperty("AddBlinkBlendshape");
			SerializedTargetBlendshapes = serializedObject.FindProperty("TargetBlendshapes");
			SerializedAddLayerControl = serializedObject.FindProperty("AddLayerControl");
			SerializedTargetLayerIndexs = serializedObject.FindProperty("TargetLayerIndexs");
		}

		public override void OnInspectorGUI() {
			serializedObject.Update();
			GetSerializedBlendshapeList();
			LanguageIndex = EditorGUILayout.Popup(GetTranslatedString("String_Language"), LanguageIndex, LanguageOption);
			EditorGUILayout.Space();
			EditorGUILayout.PropertyField(SerializedTargetAnimationClips, new GUIContent(GetTranslatedString("String_AnimationClip")));
			using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox)) {
				EditorGUI.indentLevel++;
				EditorGUILayout.PropertyField(SerializedAddBlinkBlendshape, new GUIContent(GetTranslatedString("String_AddBlink")));
				EditorGUILayout.PropertyField(SerializedTargetBlendshapes, new GUIContent(GetTranslatedString("String_BlendShape")));
				DrawAvatarBlendshapeGUI();
				EditorGUI.indentLevel--;
			}
			using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox)) {
				EditorGUI.indentLevel++;
				EditorGUILayout.PropertyField(SerializedAddLayerControl, new GUIContent(GetTranslatedString("String_AddLayerControl")));
				EditorGUILayout.PropertyField(SerializedTargetLayerIndexs, new GUIContent(GetTranslatedString("String_LayerIndex")));
				EditorGUI.indentLevel--;
			}
			serializedObject.ApplyModifiedProperties();
		}

		void DrawAvatarBlendshapeGUI() {
			if (AvatarBlendshapes.Length > 0) {
				using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox)) {
					EditorGUI.indentLevel++;
					foreach (string TargetBlendshape in AvatarBlendshapes) {
						EditorGUILayout.BeginHorizontal();
						EditorGUILayout.LabelField(TargetBlendshape);
						if (GUILayout.Button(GetTranslatedString("String_Add"), GUILayout.Width(72))) {
							RequestAddBlendshape(TargetBlendshape);
						}
						EditorGUILayout.EndHorizontal();
					}
					EditorGUI.indentLevel--;
				}
			}
			if (GUILayout.Button(GetTranslatedString("String_GetAvatarData"))) {
				RequestUpdateBlendshapeList();
			}
		}

		void RequestUpdateBlendshapeList() {
			AvatarBlendshapes = new string[0];
			FixFacialAnimation TargetComponentInstance = (FixFacialAnimation)target;
			if (!TargetComponentInstance) return;
			GameObject AvatarGameObject = AvatarUtility.GetAvatarGameObject(TargetComponentInstance.gameObject);
			if (!AvatarGameObject) return;
			GameObject HeadGameObject = AvatarUtility.GetHeadGameObject(AvatarGameObject);
			if (!HeadGameObject) return;
			SkinnedMeshRenderer HeadSkinnedMeshRenderer = HeadGameObject.GetComponent<SkinnedMeshRenderer>();
			if (!HeadSkinnedMeshRenderer) return;
			if (!HeadSkinnedMeshRenderer.sharedMesh) return;
			int HeadBlendshapeCount = HeadSkinnedMeshRenderer.sharedMesh.blendShapeCount;
			if (HeadBlendshapeCount == 0) return;
			List<string> ActiveBlendshapeNames = new List<string>();
			for (int Index = 0; Index < HeadBlendshapeCount; Index++) {
				if (HeadSkinnedMeshRenderer.GetBlendShapeWeight(Index) > 0f) {
					string TargetBlendshapeName = HeadSkinnedMeshRenderer.sharedMesh.GetBlendShapeName(Index);
					if (!ComponentBlendshapes.Contains(TargetBlendshapeName)) {
						ActiveBlendshapeNames.Add(HeadSkinnedMeshRenderer.sharedMesh.GetBlendShapeName(Index));
					}
				}
			}
			AvatarBlendshapes = ActiveBlendshapeNames.OrderBy(Item => Item, StringComparer.Ordinal).ToArray();
		}

		void RequestAddBlendshape(string TargetBlendshape) {
			if (SerializedTargetBlendshapes == null || !SerializedTargetBlendshapes.isArray) return;
			int CurrentArraySize = SerializedTargetBlendshapes.arraySize;
			SerializedTargetBlendshapes.arraySize++;
			SerializedProperty NewSerializedProperty = SerializedTargetBlendshapes.GetArrayElementAtIndex(CurrentArraySize);
			NewSerializedProperty.stringValue = TargetBlendshape;
			List<string> NewAvatarBlendshapes = AvatarBlendshapes.ToList();
			NewAvatarBlendshapes.Remove(TargetBlendshape);
			AvatarBlendshapes = NewAvatarBlendshapes.ToArray();
		}

		void GetSerializedBlendshapeList() {
			if (SerializedTargetBlendshapes == null || !SerializedTargetBlendshapes.isArray) return;
			List<string> SerializedBlendshapes = new List<string>();
			for (int Index = 0; Index < SerializedTargetBlendshapes.arraySize; Index++) {
				SerializedProperty TargetSerializedProperty = SerializedTargetBlendshapes.GetArrayElementAtIndex(Index);
				SerializedBlendshapes.Add(TargetSerializedProperty.stringValue);
			}
			ComponentBlendshapes = SerializedBlendshapes.Distinct().ToArray();
		}
	}
}
#endif