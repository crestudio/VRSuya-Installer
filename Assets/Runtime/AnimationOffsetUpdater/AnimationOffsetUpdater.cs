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
		AvatarAuthor TargetAvatarAuthorType = AvatarAuthor.General;
		public AvatarAuthor[] AvatarAuthors = (AvatarAuthor[])Enum.GetValues(typeof(AvatarAuthor));
		string[] TargetCheekBoneNames;

        Transform[] AvatarCheekBoneTransforms = new Transform[0];
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
			return;
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
		string[] GetTargetCheekBoneNames() {
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
		bool VerifyVariable() {
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
		void ClearVariable() {
			AnimationOriginPosition = new Vector3(0.0f, 0.0f, 0.0f);
			AvatarOriginPosition = new Vector3(0.0f, 0.0f, 0.0f);
			StatusCode = null;
            return;
        }


		/// <summary>FX 레이어에서 모구모구 애니메이션을 찾아서 리스트로 반환 합니다.</summary>
		/// <returns>모구모구 애니메이션 클립 어레이</returns>
		AnimationClip[] GetVRSuyaMogumoguAnimations(AnimatorController TargetAnimatorController) {
			AnimationClip[] NewMogumoguAnimationClips = new AnimationClip[4];
			if (TargetAnimatorController) {
				NewMogumoguAnimationClips = TargetAnimatorController.animationClips.Where(Item => Item.name.Contains("Mogumogu")).ToArray();
				Array.Sort(NewMogumoguAnimationClips, (Item1, Item2) => Item1.name.CompareTo(Item2.name));
			}
			return NewMogumoguAnimationClips;
		}

		/// <summary>아바타의 볼 트랜스폼의 위치를 가져옵니다.</summary>
		/// <returns>아바타의 볼 원점 Position</returns>
		Transform[] GetCheekBoneTransforms() {
			Transform[] CheekTransforms = new Transform[0];
            Transform[] ChildTransforms = AvatarGameObject.GetComponent<Animator>().GetBoneTransform(HumanBodyBones.Head).GetComponentsInChildren<Transform>(true);
			CheekTransforms = ChildTransforms.Where((HeadChildTransform) => Array.Exists(TargetCheekBoneNames, TargetBoneName => HeadChildTransform.name == TargetBoneName)).ToArray();
            return CheekTransforms;
        }

		/// <summary>아바타와 애니메이션에서 볼의 원점 기준을 가져옵니다</summary>
		void GetOriginPositions() {
			AnimationClip PoseAnimationClip = Array.Find(AvatarAnimationClips, Item => Item.length == 0);
            if (PoseAnimationClip) {
				foreach (EditorCurveBinding TargetBinding in AnimationUtility.GetCurveBindings(PoseAnimationClip)) {
					if (Array.Exists(TargetCheekBoneNames, BoneName => TargetBinding.path.Contains(BoneName))) {
						AnimationOriginPosition = GetAnimationOriginTransform(TargetBinding.path);
						break;
					}
				}
			}
			AvatarOriginPosition = AvatarCheekBoneTransforms[0].localPosition;
			return;
        }

		/// <summary>포즈 애니메이션에서 볼 원점의 위치를 가져옵니다.</summary>
		/// <returns>포즈 애니메이션 볼 원점 Position</returns>
		Vector3 GetAnimationOriginTransform(string TargetAnimationPath) {
            AnimationClip PoseAnimationClip = Array.Find(AvatarAnimationClips, Item => Item.length == 0);
			Vector3 NewOriginPosition = new Vector3(0.0f, 0.0f, 0.0f);
            if (PoseAnimationClip) {
				foreach (EditorCurveBinding TargetBinding in AnimationUtility.GetCurveBindings(PoseAnimationClip)) {
					if (TargetBinding.path.Contains(TargetAnimationPath)) {
						Keyframe[] ExistKeyframes = AnimationUtility.GetEditorCurve(PoseAnimationClip, TargetBinding).keys;
						switch (TargetBinding.propertyName) {
							case "m_LocalPosition.x":
								NewOriginPosition = new Vector3(ExistKeyframes[0].value, NewOriginPosition.y, NewOriginPosition.z);
								break;
							case "m_LocalPosition.y":
								NewOriginPosition = new Vector3(NewOriginPosition.x, ExistKeyframes[0].value, NewOriginPosition.z);
								break;
							case "m_LocalPosition.z":
								NewOriginPosition = new Vector3(NewOriginPosition.x, NewOriginPosition.y, ExistKeyframes[0].value);
								break;
						}
					}
				}
			}
            return NewOriginPosition;
        }

		/// <summary>아바타에서 볼 원점의 위치를 가져옵니다.</summary>
		/// <returns>아바타 볼 원점 Position</returns>
		Vector3 GetAvatarOriginTransform(string TargetAnimationPath) {
            Transform TargetCheekTransform = Array.Find(AvatarCheekBoneTransforms, CheekTransform => TargetAnimationPath.Contains(CheekTransform.name));
			Vector3 NewOriginPosition = new Vector3(0.0f, 0.0f, 0.0f);
            if (TargetCheekTransform) NewOriginPosition = TargetCheekTransform.localPosition;
            return NewOriginPosition;
        }

		/// <summary>포즈 애니메이션 클립들을 Array 맨 뒤로 이동합니다</summary>
		AnimationClip[] ReorderAnimationClips() {
            AnimationClip[] NewAnimationClips = new AnimationClip[AvatarAnimationClips.Length];
            int StartIndex = 0;
            int EndIndex = AvatarAnimationClips.Length - 1;
			foreach (AnimationClip TargetAnimationClip in AvatarAnimationClips) {
                if (TargetAnimationClip.length != 0) {
                    NewAnimationClips[StartIndex] = TargetAnimationClip;
                    StartIndex++;
                } else {
                    NewAnimationClips[EndIndex] = TargetAnimationClip;
                    EndIndex--;

				}
            }
            return NewAnimationClips;
        }

		/// <summary>애니메이션 클립들을 업데이트 합니다</summary>
		void UpdateAnimationKeyframes() {
            foreach (AnimationClip TargetAnimationClip in AvatarAnimationClips) {
                foreach (EditorCurveBinding TargetBinding in AnimationUtility.GetCurveBindings(TargetAnimationClip)) {
                    if (Array.Exists(TargetCheekBoneNames, BoneName => TargetBinding.path.Contains(BoneName))) {
                        Keyframe[] ExistKeyframes = AnimationUtility.GetEditorCurve(TargetAnimationClip, TargetBinding).keys;
                        if (TargetAnimationClip.length > 0) {
							Keyframe[] NewKeyframes = new Keyframe[ExistKeyframes.Length];
							AnimationOriginPosition = GetAnimationOriginTransform(TargetBinding.path);
                            AvatarOriginPosition = GetAvatarOriginTransform(TargetBinding.path);
                            switch (TargetBinding.propertyName) {
                                case "m_LocalPosition.x":
                                    for (int Frame = 0; Frame < ExistKeyframes.Length; Frame++) {
                                        float NewValue = AvatarOriginPosition.x + ((ExistKeyframes[Frame].value - AnimationOriginPosition.x) * AnimationStrength.x);
                                        NewKeyframes[Frame] = new Keyframe(ExistKeyframes[Frame].time, NewValue);
										Debug.Log($"[VRSuya] {Frame} Position X : {ExistKeyframes[Frame].value} → {NewValue}");
									}
                                    break;
                                case "m_LocalPosition.y":
                                    for (int Frame = 0; Frame < ExistKeyframes.Length; Frame++) {
                                        float NewValue = AvatarOriginPosition.y + ((ExistKeyframes[Frame].value - AnimationOriginPosition.y) * AnimationStrength.y);
                                        NewKeyframes[Frame] = new Keyframe(ExistKeyframes[Frame].time, NewValue);
										Debug.Log($"[VRSuya] {Frame} Position Y : {ExistKeyframes[Frame].value} → {NewValue}");
									}
                                    break;
                                case "m_LocalPosition.z":
                                    for (int Frame = 0; Frame < ExistKeyframes.Length; Frame++) {
                                        float NewValue = AvatarOriginPosition.z + ((ExistKeyframes[Frame].value - AnimationOriginPosition.z) * AnimationStrength.z);
                                        NewKeyframes[Frame] = new Keyframe(ExistKeyframes[Frame].time, NewValue);
                                        Debug.Log($"[VRSuya] {Frame} Position Z : {ExistKeyframes[Frame].value} → {NewValue}");
                                    }
                                    break;
                            }
							TargetAnimationClip.SetCurve(TargetBinding.path, typeof(Transform), TargetBinding.propertyName, new AnimationCurve(NewKeyframes));
						} else {
							Keyframe[] NewKeyframes = new Keyframe[ExistKeyframes.Length];
							AvatarOriginPosition = GetAvatarOriginTransform(TargetBinding.path);
                            switch (TargetBinding.propertyName) {
                                case "m_LocalPosition.x":
                                    for (int Frame = 0; Frame < ExistKeyframes.Length; Frame++) {
                                        float NewValue = AvatarOriginPosition.x;
                                        NewKeyframes[Frame] = new Keyframe(ExistKeyframes[Frame].time, NewValue);
										Debug.Log($"[VRSuya] {Frame} Position X : {ExistKeyframes[Frame].value} → {NewValue}");
									}
                                    break;
                                case "m_LocalPosition.y":
                                    for (int Frame = 0; Frame < ExistKeyframes.Length; Frame++) {
                                        float NewValue = AvatarOriginPosition.y;
                                        NewKeyframes[Frame] = new Keyframe(ExistKeyframes[Frame].time, NewValue);
										Debug.Log($"[VRSuya] {Frame} Position Y : {ExistKeyframes[Frame].value} → {NewValue}");
									}
                                    break;
                                case "m_LocalPosition.z":
                                    for (int Frame = 0; Frame < ExistKeyframes.Length; Frame++) {
                                        float NewValue = AvatarOriginPosition.z;
                                        NewKeyframes[Frame] = new Keyframe(ExistKeyframes[Frame].time, NewValue);
										Debug.Log($"[VRSuya] {Frame} Position Z : {ExistKeyframes[Frame].value} → {NewValue}");
									}
                                    break;
                            }
							TargetAnimationClip.SetCurve(TargetBinding.path, typeof(Transform), TargetBinding.propertyName, new AnimationCurve(NewKeyframes));
						}
                    }
                }
            }
			return;
        }
    }
}
#endif