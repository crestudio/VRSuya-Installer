#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;

using UnityEditor;
using UnityEngine;

#if MODULAR_AVATAR
using nadena.dev.modular_avatar.core;
#endif

using Avatar = VRSuya.Core.Avatar;

/*
 * VRSuya Installer
 * Contact : vrsuya@gmail.com // Twitter : https://twitter.com/VRSuya
 */

namespace VRSuya.Installer {

    [ExecuteInEditMode]
	public class AnimationOffsetUpdater : ScriptableObject {

        public GameObject AvatarGameObject = null;
        public AnimationClip[] AvatarAnimationClips = new AnimationClip[4];
        public Vector3 AnimationStrength = new Vector3 (1.0f, 1.0f, 1.0f);

		Transform LeftCheekBone = null;
		Transform RightCheekBone = null;
		Vector3 LeftCheekPosition = Vector3.zero;
		Vector3 RightCheekPosition = Vector3.zero;
		Vector3 LeftOriginPosition = Vector3.zero;
		Vector3 RightOriginPosition = Vector3.zero;

		static readonly string[] Cheek_L_Names = new string[] { "Cheek_L", "Cheek.L", "Cheek1_L", "Cheek_Root_L", "Cheek_root_L", "Hoppe.L", "ho_L" };
		static readonly string[] Cheek_R_Names = new string[] { "Cheek_R", "Cheek.R", "Cheek1_R", "Cheek_Root_R", "Cheek_root_R", "Hoppe.R", "ho_R" };

		string StatusCode;

		void OnEnable() {
			Avatar AvatarInstance = new Avatar();
			AvatarGameObject = AvatarInstance.GetAvatarGameObject();
			if (AvatarGameObject) {
				AvatarAnimationClips = GetVRSuyaMogumoguAnimations(AvatarGameObject);
			}
		}

		public string RequestUpdateAnimationClips() {
			if (VerifyVariable()) {
				GetCheekBoneTransforms();
                if (LeftCheekBone && RightCheekBone) {
					GetOriginPositions();
					UpdateAnimationClips();
					StatusCode = "COMPLETED_UPDATE";
				} else {
					StatusCode = "NO_CHEEKBONE";
				}
            }
			return StatusCode;
        }

		bool VerifyVariable() {
            if (!AvatarGameObject) {
				StatusCode = "NO_OLD_AVATAR";
				return false;
			}
			AvatarGameObject.TryGetComponent(out Animator AvatarAnimator);
			if (!AvatarAnimator) {
				StatusCode = "NO_ANIMATOR";
				return false;
			}
			StatusCode = string.Empty;
			return true;
        }

		AnimationClip[] GetVRSuyaMogumoguAnimations(GameObject TargetGameObject) {
			#if MODULAR_AVATAR
			RuntimeAnimatorController MogumoguAnimator = TargetGameObject.GetComponentsInChildren<ModularAvatarMergeAnimator>(true)
				.Where(Item => Item.animator != null)
				.Select(Item => Item.animator)
				.FirstOrDefault(Item => Item.name.Contains("Mogumogu") && !Item.name.Contains("Effect"));
			if (MogumoguAnimator) {
				AnimationClip[] NewMogumoguAnimationClips = MogumoguAnimator.animationClips.Where(Item => Item.name.Contains("Mogumogu")).ToArray();
				Array.Sort(NewMogumoguAnimationClips, (Item1, Item2) => Item1.name.CompareTo(Item2.name));
				return NewMogumoguAnimationClips;
			}
			#endif
			return null;
		}

		void GetCheekBoneTransforms() {
			Transform[] HeadChildTransforms = AvatarGameObject.GetComponent<Animator>().GetBoneTransform(HumanBodyBones.Head).GetComponentsInChildren<Transform>(true);
			LeftCheekBone = HeadChildTransforms.FirstOrDefault(Item => Cheek_L_Names.Contains(Item.name));
			RightCheekBone = HeadChildTransforms.FirstOrDefault(Item => Cheek_R_Names.Contains(Item.name));
			if (LeftCheekBone) LeftCheekPosition = LeftCheekBone.transform.localPosition;
			if (RightCheekBone) RightCheekPosition = RightCheekBone.transform.localPosition;
		}

		void GetOriginPositions() {
			AnimationClip PoseAnimationClip = AvatarAnimationClips.FirstOrDefault(Item => Item.length == 0);
			if (PoseAnimationClip) {
				Vector3 NewLeftOriginPosition = new Vector3(0.0f, 0.0f, 0.0f);
				Vector3 NewRightOriginPosition = new Vector3(0.0f, 0.0f, 0.0f);
				foreach (EditorCurveBinding TargetBinding in AnimationUtility.GetCurveBindings(PoseAnimationClip)) {
					if (TargetBinding.path.Contains(LeftCheekBone.name)) {
						Keyframe[] ExistKeyframes = AnimationUtility.GetEditorCurve(PoseAnimationClip, TargetBinding).keys;
						switch (TargetBinding.propertyName) {
							case "m_LocalPosition.x":
								NewLeftOriginPosition = new Vector3(ExistKeyframes[0].value, NewLeftOriginPosition.y, NewLeftOriginPosition.z);
								break;
							case "m_LocalPosition.y":
								NewLeftOriginPosition = new Vector3(NewLeftOriginPosition.x, ExistKeyframes[0].value, NewLeftOriginPosition.z);
								break;
							case "m_LocalPosition.z":
								NewLeftOriginPosition = new Vector3(NewLeftOriginPosition.x, NewLeftOriginPosition.y, ExistKeyframes[0].value);
								break;
						}
					} else if (TargetBinding.path.Contains(RightCheekBone.name)) {
						Keyframe[] ExistKeyframes = AnimationUtility.GetEditorCurve(PoseAnimationClip, TargetBinding).keys;
						switch (TargetBinding.propertyName) {
							case "m_LocalPosition.x":
								NewRightOriginPosition = new Vector3(ExistKeyframes[0].value, NewRightOriginPosition.y, NewRightOriginPosition.z);
								break;
							case "m_LocalPosition.y":
								NewRightOriginPosition = new Vector3(NewRightOriginPosition.x, ExistKeyframes[0].value, NewRightOriginPosition.z);
								break;
							case "m_LocalPosition.z":
								NewRightOriginPosition = new Vector3(NewRightOriginPosition.x, NewRightOriginPosition.y, ExistKeyframes[0].value);
								break;
						}
					}
				}
				LeftOriginPosition = NewLeftOriginPosition;
				RightOriginPosition = NewRightOriginPosition;
			}
		}

		void UpdateAnimationClips() {
			List<bool> Modified = new List<bool>();
			foreach (AnimationClip TargetAnimationClip in AvatarAnimationClips) {
				if (!TargetAnimationClip) continue;
				if (TargetAnimationClip.length > 0) {
					Modified.Add(UpdateAnimation(TargetAnimationClip));
				} else {
					Modified.Add(UpdatePoseAnimation(TargetAnimationClip));
				}
            }
			if (Modified.Any(Item => true)) {
				AssetDatabase.SaveAssets();
			}
		}

		bool UpdateAnimation(AnimationClip TargetAnimationClip) {
			bool IsModified = false;
			foreach (EditorCurveBinding TargetBinding in AnimationUtility.GetCurveBindings(TargetAnimationClip)) {
				if (TargetBinding.path.Contains(LeftCheekBone.name) || TargetBinding.path.Contains(RightCheekBone.name)) {
					Keyframe[] ExistKeyframes = AnimationUtility.GetEditorCurve(TargetAnimationClip, TargetBinding).keys;
					Keyframe[] NewKeyframes = new Keyframe[ExistKeyframes.Length];
					bool IsLeft = TargetBinding.path.Contains(LeftCheekBone.name);
					switch (TargetBinding.propertyName) {
						case "m_LocalPosition.x":
							for (int Frame = 0; Frame < ExistKeyframes.Length; Frame++) {
								float NewValue = GetAnimationValue("X", ExistKeyframes[Frame].value, IsLeft);
								NewKeyframes[Frame] = new Keyframe(ExistKeyframes[Frame].time, NewValue);
							}
							break;
						case "m_LocalPosition.y":
							for (int Frame = 0; Frame < ExistKeyframes.Length; Frame++) {
								float NewValue = GetAnimationValue("Y", ExistKeyframes[Frame].value, IsLeft);
								NewKeyframes[Frame] = new Keyframe(ExistKeyframes[Frame].time, NewValue);
							}
							break;
						case "m_LocalPosition.z":
							for (int Frame = 0; Frame < ExistKeyframes.Length; Frame++) {
								float NewValue = GetAnimationValue("Z", ExistKeyframes[Frame].value, IsLeft);
								NewKeyframes[Frame] = new Keyframe(ExistKeyframes[Frame].time, NewValue);
							}
							break;
					}
					TargetAnimationClip.SetCurve(TargetBinding.path, typeof(Transform), TargetBinding.propertyName, new AnimationCurve(NewKeyframes));
					IsModified = true;
					EditorUtility.SetDirty(TargetAnimationClip);
				}
			}
			return IsModified;
		}

		bool UpdatePoseAnimation(AnimationClip TargetAnimationClip) {
			bool IsModified = false;
			foreach (EditorCurveBinding TargetBinding in AnimationUtility.GetCurveBindings(TargetAnimationClip)) {
				if (TargetBinding.path.Contains(LeftCheekBone.name) || TargetBinding.path.Contains(RightCheekBone.name)) {
					Keyframe[] OldKeyframes = AnimationUtility.GetEditorCurve(TargetAnimationClip, TargetBinding).keys;
					Keyframe[] NewKeyframes = new Keyframe[OldKeyframes.Length];
					switch (TargetBinding.propertyName) {
						case "m_LocalPosition.x":
							for (int Frame = 0; Frame < OldKeyframes.Length; Frame++) {
								float NewValue = TargetBinding.path.Contains(LeftCheekBone.name) ? LeftCheekPosition.x : RightCheekPosition.x;
								NewKeyframes[Frame] = new Keyframe(OldKeyframes[Frame].time, NewValue);
							}
							break;
						case "m_LocalPosition.y":
							for (int Frame = 0; Frame < OldKeyframes.Length; Frame++) {
								float NewValue = TargetBinding.path.Contains(LeftCheekBone.name) ? LeftCheekPosition.y : RightCheekPosition.y;
								NewKeyframes[Frame] = new Keyframe(OldKeyframes[Frame].time, NewValue);
							}
							break;
						case "m_LocalPosition.z":
							for (int Frame = 0; Frame < OldKeyframes.Length; Frame++) {
								float NewValue = TargetBinding.path.Contains(LeftCheekBone.name) ? LeftCheekPosition.z : RightCheekPosition.z;
								NewKeyframes[Frame] = new Keyframe(OldKeyframes[Frame].time, NewValue);
							}
							break;
					}
					TargetAnimationClip.SetCurve(TargetBinding.path, typeof(Transform), TargetBinding.propertyName, new AnimationCurve(NewKeyframes));
					IsModified = true;
					EditorUtility.SetDirty(TargetAnimationClip);
				}
			}
			return IsModified;
		}

		float GetAnimationValue(string Axis, float Value, bool IsLeft) {
			if (IsLeft) {
				switch (Axis) {
					case "X":
						return LeftCheekPosition.x + ((Value - LeftOriginPosition.x) * AnimationStrength.x);
					case "Y":
						return LeftCheekPosition.y + ((Value - LeftOriginPosition.y) * AnimationStrength.y);
					case "Z":
						return LeftCheekPosition.z + ((Value - LeftOriginPosition.z) * AnimationStrength.z);
				}
			} else {
				switch (Axis) {
					case "X":
						return RightCheekPosition.x + ((Value - RightOriginPosition.x) * AnimationStrength.x);
					case "Y":
						return RightCheekPosition.y + ((Value - RightOriginPosition.y) * AnimationStrength.y);
					case "Z":
						return RightCheekPosition.z + ((Value - RightOriginPosition.z) * AnimationStrength.z);
				}
			}
			return Value;
		}
    }
}
#endif