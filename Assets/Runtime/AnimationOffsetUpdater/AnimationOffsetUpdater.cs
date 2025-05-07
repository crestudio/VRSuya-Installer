#if UNITY_EDITOR
using System;
using System.Linq;

using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;

using VRC.SDK3.Avatars.Components;

/*
 * VRSuya Animation Offset Updater for Mogumogu Project
 * Contact : vrsuya@gmail.com // Twitter : https://twitter.com/VRSuya
 */

namespace com.vrsuya.animationoffsetupdater {

    [ExecuteInEditMode]
	[AddComponentMenu("VRSuya/VRSuya Animation Offset Updater")]
	public class AnimationOffsetUpdater : MonoBehaviour {

        public GameObject AvatarGameObject = null;
        public AnimationClip[] AvatarAnimationClips = new AnimationClip[4];
        public Vector3 AnimationStrength = new Vector3 (1.0f, 1.0f, 1.0f);

		public string TargetAvatarAuthorName;
		private AvatarAuthor TargetAvatarAuthorType = AvatarAuthor.General;
		public AvatarAuthor[] AvatarAuthors = (AvatarAuthor[])Enum.GetValues(typeof(AvatarAuthor));
		private string[] TargetCheekBoneNames;

        private Transform[] AvatarCheekBoneTransforms = new Transform[0];
        public Vector3 AnimationOriginPosition = new Vector3 (0.0f, 0.0f, 0.0f);
        public Vector3 AvatarOriginPosition = new Vector3(0.0f, 0.0f, 0.0f);

        public string StatusCode;

		public enum AvatarAuthor {
			General,
			ChocolateRice,
			JINGO,
			Komado,
			Plusone
		}

		void OnEnable() {
            if (!AvatarGameObject) AvatarGameObject = this.gameObject;
			if (AvatarGameObject) {
				VRSuya.Core.Avatar AvatarInstance = new VRSuya.Core.Avatar();
				AnimatorController AvatarFXLayer = AvatarInstance.GetAnimatorController(AvatarGameObject, VRCAvatarDescriptor.AnimLayerType.FX);
				AvatarAnimationClips = GetVRSuyaMogumoguAnimations(AvatarFXLayer);
			}
		}

		/// <summary>아바타와 애니메이션으로부터 볼 본의 원점을 구합니다</summary>
		public void UpdateOriginPositions() {
			ClearVariable();
			if (TargetAvatarAuthorName != null) TargetAvatarAuthorType = (AvatarAuthor)Enum.Parse(typeof(AvatarAuthor), TargetAvatarAuthorName);
			TargetCheekBoneNames = GetTargetCheekBoneNames();
            if (VerifyVariable()) {
				AvatarCheekBoneTransforms = GetCheekBoneTransforms();
				if (AvatarCheekBoneTransforms.Length >= 2) {
					AvatarAnimationClips = ReorderAnimationClips();
					GetOriginPositions();
					StatusCode = "COMPLETED_GETPOSITION";
				} else {
					StatusCode = "NO_CHEEKBONE";
				}
            }
            return;
        }

		/// <summary>
		/// 본 프로그램의 메인 세팅 로직입니다.
		/// </summary>
		public void UpdateAnimationOffset() {
			ClearVariable();
			if (TargetAvatarAuthorName != null) TargetAvatarAuthorType = (AvatarAuthor)Enum.Parse(typeof(AvatarAuthor), TargetAvatarAuthorName);
			TargetCheekBoneNames = GetTargetCheekBoneNames();
			if (Array.TrueForAll(AvatarAnimationClips, TargetAnimationClip => !TargetAnimationClip)) {
				VRSuya.Core.Avatar AvatarInstance = new VRSuya.Core.Avatar();
				AnimatorController AvatarFXLayer = AvatarInstance.GetAnimatorController(AvatarGameObject, VRCAvatarDescriptor.AnimLayerType.FX);
				AvatarAnimationClips = GetVRSuyaMogumoguAnimations(AvatarFXLayer);
			}
			if (VerifyVariable()) {
				AvatarCheekBoneTransforms = GetCheekBoneTransforms();
                if (AvatarCheekBoneTransforms.Length >= 2) {
					AvatarAnimationClips = ReorderAnimationClips();
					UpdateAnimationKeyframes();
					StatusCode = "COMPLETED_UPDATE";
				} else {
					StatusCode = "NO_CHEEKBONE";
				}
            }
            return;
        }

		/// <summary>찾아야 하는 볼 본의 이름를 조회해서 반환합니다.</summary>
		/// <returns>목표 볼 본의 이름</returns>
		private string[] GetTargetCheekBoneNames() {
            switch (TargetAvatarAuthorType) {
				case AvatarAuthor.ChocolateRice:
					TargetCheekBoneNames = new string[] { "Hoppe.L", "Hoppe.R" };
					break;
				case AvatarAuthor.JINGO:
					TargetCheekBoneNames = new string[] { "Cheek_root_L", "Cheek_root_R" };
					break;
				case AvatarAuthor.Komado:
					TargetCheekBoneNames = new string[] { "Cheek_Root_L", "Cheek_Root_R" };
					break;
				case AvatarAuthor.Plusone:
					TargetCheekBoneNames = new string[] { "Cheek.L", "Cheek.R" };
					break;
				default:
					TargetCheekBoneNames = new string[] { "Cheek1_L", "Cheek1_R" };
					break;
            }
            return TargetCheekBoneNames;
        }

		/// <summary>아바타의 현재 상태를 검사하여 패치가 가능한지 확인합니다.</summary>
		/// <returns>업데이트 가능 여부</returns>
		private bool VerifyVariable() {
            if (!AvatarGameObject) {
                AvatarGameObject = this.gameObject;
            }
			AvatarGameObject.TryGetComponent(typeof(Animator), out Component Animator);
			if (!Animator) {
				StatusCode = "NO_ANIMATOR";
				return false;
			}
            if (Array.TrueForAll(AvatarAnimationClips, TargetAnimationClip => !TargetAnimationClip)) {
				StatusCode = "NO_CLIPS";
				return false;
            }
            return true;
        }

		/// <summary>상태를 초기화 합니다.</summary>
		private void ClearVariable() {
			AnimationOriginPosition = new Vector3(0.0f, 0.0f, 0.0f);
			AvatarOriginPosition = new Vector3(0.0f, 0.0f, 0.0f);
			StatusCode = null;
            return;
        }


		/// <summary>FX 레이어에서 모구모구 애니메이션을 찾아서 리스트로 반환 합니다.</summary>
		/// <returns>모구모구 애니메이션 클립 어레이</returns>
		private AnimationClip[] GetVRSuyaMogumoguAnimations(AnimatorController TargetAnimatorController) {
			AnimationClip[] newAnimationClips = new AnimationClip[4];
			if (TargetAnimatorController) {
				newAnimationClips = TargetAnimatorController.animationClips.Where(TargetAnimationClip => TargetAnimationClip.name.Contains("Mogumogu")).ToArray();
				Array.Sort(newAnimationClips, (TargetAnimationClip1, TargetAnimationClip2) => TargetAnimationClip1.name.CompareTo(TargetAnimationClip2.name));
			}
			return newAnimationClips;
		}

		/// <summary>아바타의 볼 트랜스폼의 위치를 가져옵니다.</summary>
		/// <returns>아바타의 볼 원점 Position</returns>
		private Transform[] GetCheekBoneTransforms() {
			Transform[] CheekTransforms = new Transform[0];
            Transform[] ChildTransforms = AvatarGameObject.GetComponent<Animator>().GetBoneTransform(HumanBodyBones.Head).GetComponentsInChildren<Transform>(true);
			CheekTransforms = ChildTransforms.Where((HeadChildTransform) => Array.Exists(TargetCheekBoneNames, TargetBoneName => HeadChildTransform.name == TargetBoneName)).ToArray();
            return CheekTransforms;
        }

		/// <summary>아바타와 애니메이션에서 볼의 원점 기준을 가져옵니다</summary>
		private void GetOriginPositions() {
			AnimationClip PoseAnimationClip = Array.Find(AvatarAnimationClips, TargetAnimationClip => TargetAnimationClip.length == 0);
            if (PoseAnimationClip) {
				foreach (EditorCurveBinding Binding in AnimationUtility.GetCurveBindings(PoseAnimationClip)) {
					if (Array.Exists(TargetCheekBoneNames, BoneName => Binding.path.Contains(BoneName))) {
						AnimationOriginPosition = GetAnimationOriginTransform(Binding.path);
						break;
					}
				}
			}
			AvatarOriginPosition = AvatarCheekBoneTransforms[0].localPosition;
			return;
        }

		/// <summary>포즈 애니메이션에서 볼 원점의 위치를 가져옵니다.</summary>
		/// <returns>포즈 애니메이션 볼 원점 Position</returns>
		private Vector3 GetAnimationOriginTransform(string TargetAnimationPath) {
            AnimationClip PoseAnimationClip = Array.Find(AvatarAnimationClips, AnimationClip => AnimationClip.length == 0);
			Vector3 newOriginTransform = new Vector3(0.0f, 0.0f, 0.0f);
            if (PoseAnimationClip) {
				foreach (EditorCurveBinding Binding in AnimationUtility.GetCurveBindings(PoseAnimationClip)) {
					if (Binding.path.Contains(TargetAnimationPath)) {
						Keyframe[] Keyframes = AnimationUtility.GetEditorCurve(PoseAnimationClip, Binding).keys;
						switch (Binding.propertyName) {
							case "m_LocalPosition.x":
								newOriginTransform = new Vector3(Keyframes[0].value, newOriginTransform.y, newOriginTransform.z);
								break;
							case "m_LocalPosition.y":
								newOriginTransform = new Vector3(newOriginTransform.x, Keyframes[0].value, newOriginTransform.z);
								break;
							case "m_LocalPosition.z":
								newOriginTransform = new Vector3(newOriginTransform.x, newOriginTransform.y, Keyframes[0].value);
								break;
						}
					}
				}
			}
            return newOriginTransform;
        }

		/// <summary>아바타에서 볼 원점의 위치를 가져옵니다.</summary>
		/// <returns>아바타 볼 원점 Position</returns>
		private Vector3 GetAvatarOriginTransform(string TargetAnimationPath) {
            Transform TargetCheekTransform = Array.Find(AvatarCheekBoneTransforms, CheekTransform => TargetAnimationPath.Contains(CheekTransform.name));
			Vector3 newOriginTransform = new Vector3(0.0f, 0.0f, 0.0f);
            if (TargetCheekTransform) newOriginTransform = TargetCheekTransform.localPosition;
            return newOriginTransform;
        }

		/// <summary>포즈 애니메이션 클립들을 Array 맨 뒤로 이동합니다</summary>
		private AnimationClip[] ReorderAnimationClips() {
            AnimationClip[] newAnimationClips = new AnimationClip[AvatarAnimationClips.Length];
            int newStartIndex = 0;
            int newEndIndex = AvatarAnimationClips.Length - 1;
			foreach (AnimationClip AnimationClip in AvatarAnimationClips) {
                if (AnimationClip.length != 0) {
                    newAnimationClips[newStartIndex] = AnimationClip;
                    newStartIndex++;
                } else {
                    newAnimationClips[newEndIndex] = AnimationClip;
                    newEndIndex--;

				}
            }
            return newAnimationClips;
        }

		/// <summary>애니메이션 클립들을 업데이트 합니다</summary>
		private void UpdateAnimationKeyframes() {
            foreach (AnimationClip CurrentAnimationClip in AvatarAnimationClips) {
                foreach (EditorCurveBinding Binding in AnimationUtility.GetCurveBindings(CurrentAnimationClip)) {
                    if (Array.Exists(TargetCheekBoneNames, BoneName => Binding.path.Contains(BoneName))) {
                        Keyframe[] ExistKeyframes = AnimationUtility.GetEditorCurve(CurrentAnimationClip, Binding).keys;
                        if (CurrentAnimationClip.length > 0) {
							Keyframe[] newKeyframes = new Keyframe[ExistKeyframes.Length];
							AnimationOriginPosition = GetAnimationOriginTransform(Binding.path);
                            AvatarOriginPosition = GetAvatarOriginTransform(Binding.path);
                            switch (Binding.propertyName) {
                                case "m_LocalPosition.x":
                                    for (int Frame = 0; Frame < ExistKeyframes.Length; Frame++) {
                                        float newValue = AvatarOriginPosition.x + ((ExistKeyframes[Frame].value - AnimationOriginPosition.x) * AnimationStrength.x);
                                        newKeyframes[Frame] = new Keyframe(ExistKeyframes[Frame].time, newValue);
                                        Debug.Log("[AnimationOffsetUpdater] Changed " + Frame + " Keyframe X value from " + ExistKeyframes[Frame].value + " to " + newValue);
                                    }
                                    break;
                                case "m_LocalPosition.y":
                                    for (int Frame = 0; Frame < ExistKeyframes.Length; Frame++) {
                                        float newValue = AvatarOriginPosition.y + ((ExistKeyframes[Frame].value - AnimationOriginPosition.y) * AnimationStrength.y);
                                        newKeyframes[Frame] = new Keyframe(ExistKeyframes[Frame].time, newValue);
                                        Debug.Log("[AnimationOffsetUpdater] Changed " + Frame + " Keyframe Y value from " + ExistKeyframes[Frame].value + " to " + newValue);
                                    }
                                    break;
                                case "m_LocalPosition.z":
                                    for (int Frame = 0; Frame < ExistKeyframes.Length; Frame++) {
                                        float newValue = AvatarOriginPosition.z + ((ExistKeyframes[Frame].value - AnimationOriginPosition.z) * AnimationStrength.z);
                                        newKeyframes[Frame] = new Keyframe(ExistKeyframes[Frame].time, newValue);
                                        Debug.Log("[AnimationOffsetUpdater] Changed " + Frame + " Keyframe Z value from " + ExistKeyframes[Frame].value + " to " + newValue);
                                    }
                                    break;
                            }
							CurrentAnimationClip.SetCurve(Binding.path, typeof(Transform), Binding.propertyName, new AnimationCurve(newKeyframes));
						} else {
							Keyframe[] newKeyframes = new Keyframe[ExistKeyframes.Length];
							AvatarOriginPosition = GetAvatarOriginTransform(Binding.path);
                            switch (Binding.propertyName) {
                                case "m_LocalPosition.x":
                                    for (int AnimationOffset = 0; AnimationOffset < ExistKeyframes.Length; AnimationOffset++) {
                                        float newKeyframeValue = AvatarOriginPosition.x;
                                        newKeyframes[AnimationOffset] = new Keyframe(ExistKeyframes[AnimationOffset].time, newKeyframeValue);
                                        Debug.Log("[AnimationOffsetUpdater] Changed " + AnimationOffset + " Keyframe X value from " + ExistKeyframes[AnimationOffset].value + " to " + newKeyframeValue);
                                    }
                                    break;
                                case "m_LocalPosition.y":
                                    for (int AnimationOffset = 0; AnimationOffset < ExistKeyframes.Length; AnimationOffset++) {
                                        float newKeyframeValue = AvatarOriginPosition.y;
                                        newKeyframes[AnimationOffset] = new Keyframe(ExistKeyframes[AnimationOffset].time, newKeyframeValue);
                                        Debug.Log("[AnimationOffsetUpdater] Changed " + AnimationOffset + " Keyframe Y value from " + ExistKeyframes[AnimationOffset].value + " to " + newKeyframeValue);
                                    }
                                    break;
                                case "m_LocalPosition.z":
                                    for (int AnimationOffset = 0; AnimationOffset < ExistKeyframes.Length; AnimationOffset++) {
                                        float newKeyframeValue = AvatarOriginPosition.z;
                                        newKeyframes[AnimationOffset] = new Keyframe(ExistKeyframes[AnimationOffset].time, newKeyframeValue);
                                        Debug.Log("[AnimationOffsetUpdater] Changed " + AnimationOffset + " Keyframe Z value from " + ExistKeyframes[AnimationOffset].value + " to " + newKeyframeValue);
                                    }
                                    break;
                            }
							CurrentAnimationClip.SetCurve(Binding.path, typeof(Transform), Binding.propertyName, new AnimationCurve(newKeyframes));
						}
                    }
                }
            }
        }
    }
}
#endif