#if UNITY_EDITOR
using System;

using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

/*
 * VRSuya Avatar Setting Updater
 * Contact : vrsuya@gmail.com // Twitter : https://twitter.com/VRSuya
 */

namespace com.vrsuya.installer {

	[ExecuteInEditMode]
	[AddComponentMenu("")]
	public class UnitySetup : AvatarSettingUpdater{

		static readonly string[] dictHeadGameObjectName = { "Body", "Head", "Face" };

		/// <summary>외부에서 요청한 아바타 정보를 업데이트를 처리하는 메소드 입니다.</summary>
		internal static void UpdateAvatarStatus() {
			GetAvatarSkinnedMeshRenderers();
			GetAvatarMeshRenderers();
			GetAvatarAnchorOverride();
			return;
		}

		/// <summary>외부의 세팅 요청을 처리하는 메인 메소드 입니다.</summary>
		internal static void UpdateAvatarData() {
			if (ChangeTwosidedShadow) UpdateTwosidedShadow();
			if (ChangeAnchorOverride) UpdateAnchorOverride();
			if (ChangeBounds) UpdateBounds();
			return;
		}

		/* Unity 관련 아바타 변수 업데이트 */

		/// <summary>아바타의 SkinnedMeshRenderer 목록을 작성합니다.</summary>
		/// <returns>아바타의 모든 SkinnedMeshRenderer 배열</returns>
		internal static SkinnedMeshRenderer[] GetAvatarSkinnedMeshRenderers() {
            AvatarSkinnedMeshRenderers = AvatarGameObject.GetComponentsInChildren<SkinnedMeshRenderer>(true);
            return AvatarSkinnedMeshRenderers;
        }

		/// <summary>아바타의 MeshRenderer 목록을 작성합니다.</summary>
		/// <returns>아바타의 모든 MeshRenderer 배열</returns>
		internal static MeshRenderer[] GetAvatarMeshRenderers() {
            AvatarMeshRenderers = AvatarGameObject.GetComponentsInChildren<MeshRenderer>(true);
            return AvatarMeshRenderers;
        }

		/// <summary>아바타의 대표 AnchorOverride 포인트를 획득하는 메소드 입니다.</summary>
		/// <returns>기준이 되는 AnchorOverride 트랜스폼</returns>
		static Transform GetAvatarAnchorOverride() {
            foreach (SkinnedMeshRenderer TargetSkinnedMeshRenderer in AvatarSkinnedMeshRenderers) {
                if (Array.Exists(dictHeadGameObjectName, HeadName => TargetSkinnedMeshRenderer.gameObject.name == HeadName) == true) {
                    if (TargetSkinnedMeshRenderer.probeAnchor) {
                        AvatarAnchorOverride = TargetSkinnedMeshRenderer.probeAnchor.transform;
                        break;
                    }
                }
            }
			if (!AvatarAnchorOverride) {
				if (AvatarAnimator.GetBoneTransform(HumanBodyBones.Head)) {
					AvatarAnchorOverride = AvatarAnimator.GetBoneTransform(HumanBodyBones.Head);
				} else if (AvatarAnimator.GetBoneTransform(HumanBodyBones.Hips)) {
					AvatarAnchorOverride = AvatarAnimator.GetBoneTransform(HumanBodyBones.Hips);
				}
			}
            return AvatarAnchorOverride;
        }

		/* 실제 아바타 데이터 업데이트 */

		/// <summary>아바타의 렌더러 세팅을 Two-sided 그림자로 설정 합니다.</summary>
		static void UpdateTwosidedShadow() {
			foreach (SkinnedMeshRenderer TargetSkinnedMeshRenderer in AvatarSkinnedMeshRenderers) {
				if (TargetSkinnedMeshRenderer.shadowCastingMode != ShadowCastingMode.TwoSided) {
					Undo.RecordObject(TargetSkinnedMeshRenderer, "Changed Two-Sided Shadow Option");
					TargetSkinnedMeshRenderer.shadowCastingMode = ShadowCastingMode.TwoSided;
					Undo.CollapseUndoOperations(UndoGroupIndex);
				}
			}
			foreach (MeshRenderer TargetMeshRenderer in AvatarMeshRenderers) {
				if (TargetMeshRenderer.shadowCastingMode != ShadowCastingMode.TwoSided) {
					Undo.RecordObject(TargetMeshRenderer, "Changed Two-Sided Shadow Option");
					TargetMeshRenderer.shadowCastingMode = ShadowCastingMode.TwoSided;
					Undo.CollapseUndoOperations(UndoGroupIndex);
				}
			}
			return;
		}

		/// <summary>아바타의 AnchorOverride 세팅을 설정 합니다.</summary>
		static void UpdateAnchorOverride() {
			foreach (SkinnedMeshRenderer TargetSkinnedMeshRenderer in AvatarSkinnedMeshRenderers) {
				if (TargetSkinnedMeshRenderer.probeAnchor != AvatarAnchorOverride) {
					Undo.RecordObject(TargetSkinnedMeshRenderer, "Changed Anchor Override");
					TargetSkinnedMeshRenderer.probeAnchor = AvatarAnchorOverride;
					Undo.CollapseUndoOperations(UndoGroupIndex);
				}
			}
			foreach (MeshRenderer TargetMeshRenderer in AvatarMeshRenderers) {
				if (TargetMeshRenderer.probeAnchor != AvatarAnchorOverride) {
					Undo.RecordObject(TargetMeshRenderer, "Changed Anchor Override");
					TargetMeshRenderer.probeAnchor = AvatarAnchorOverride;
					Undo.CollapseUndoOperations(UndoGroupIndex);
				}
			}
			return;
		}

		/// <summary>아바타의 Bounds 세팅을 설정 합니다.</summary>
		static void UpdateBounds() {
			Bounds newBounds = new Bounds {
				center = new Vector3(0.0f, 0.0f, 0.0f),
				extents = new Vector3(1.0f, 1.0f, 1.0f),
			};
			foreach (SkinnedMeshRenderer TargetSkinnedMeshRenderer in AvatarSkinnedMeshRenderers) {
				if (TargetSkinnedMeshRenderer.localBounds != newBounds) {
					Undo.RecordObject(TargetSkinnedMeshRenderer, "Changed Bounds");
					TargetSkinnedMeshRenderer.updateWhenOffscreen = true;
					TargetSkinnedMeshRenderer.localBounds = newBounds;
					TargetSkinnedMeshRenderer.updateWhenOffscreen = false;
					EditorUtility.SetDirty(TargetSkinnedMeshRenderer);
					Undo.CollapseUndoOperations(UndoGroupIndex);
				}
			}
		}
	}
}
#endif