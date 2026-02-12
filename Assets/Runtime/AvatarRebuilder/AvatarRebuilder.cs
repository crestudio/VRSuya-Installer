#if UNITY_EDITOR
using System;

using UnityEditor;
using UnityEngine;

/*
 * VRSuya AvatarRebuilder
 * Contact : vrsuya@gmail.com // Twitter : https://twitter.com/VRSuya
 * Forked from emilianavt/ReassignBoneWeigthsToNewMesh.cs ( https://gist.github.com/emilianavt/721cd4dd2d4a62ba54b002b63f894dbf )
 * Thanks to Dalgona. & C_Carrot & Naru & Rekorn
 */

namespace com.vrsuya.avatarrebuilder {

    [ExecuteInEditMode]
	[AddComponentMenu("VRSuya/VRSuya Avatar Rebuilder")]
	public class AvatarRebuilder : MonoBehaviour {

		// 에디터용 변수
		public GameObject NewAvatarGameObjectEditor = null;
		public GameObject OldAvatarGameObjectEditor = null;
		public int AvatarTypeIndexEditor;
		public SkinnedMeshRenderer[] NewAvatarSkinnedMeshRenderersEditor = new SkinnedMeshRenderer[0];
		public Transform AvatarRootBoneEditor = null;
		public bool ToggleRestoreArmatureTransformEditor = true;
		public bool ToggleResetRestPoseEditor = false;
		public bool ToggleReorderGameObjectEditor = true;
		public string StatusStringEditor = "";

		// 정적 변수
		protected static GameObject NewAvatarGameObject;
		protected static Animator NewAvatarAnimator;
		protected static GameObject OldAvatarGameObject;
		protected static Animator OldAvatarAnimator;
		protected static SkinnedMeshRenderer[] NewAvatarSkinnedMeshRenderers;

		protected static Avatar TargetAvatar;

		public enum Avatar {
			General,
			Airi, Aldina, Angura, Anon, Anri, Ash,
			Chiffon, Chise, Chocolat, Cygnet,
			Emmelie, EYO,
			Firina, Fuzzy,
			Glaze, Grus,
			Hakka,
			IMERIS,
			Karin, Kikyo, Kipfel, Kokoa, Koyuki, Kuronatu,
			Lapwing, Leefa, Leeme, Lime, Lunalitt,
			Maki, Mamehinata, MANUKA, Mariel, Marron, Maya, Merino, Milfy, Milk, Milltina, Minahoshi, Minase, Mint, Mir, Mishe, Moe,
			Nayu,
			Platinum,
			Quiche,
			Rainy, Ramune_Old, RINDO, Rue, Rusk,
			SELESTIA, Sephira, Shinano, Shinra, Sio, Sue, Sugar, Suzuhana,
			Tien, TubeRose,
			Ukon, Usasaki, Uzuki,
			Wolferia,
			Yoll, YUGI_MIYO, Yuuko
			// 검색용 신규 아바타 추가 위치
		}

		protected enum BoneNameType {
			General, Komado, Yoll
		}

		protected static Transform AvatarRootBone;
		protected static bool ToggleRestoreArmatureTransform;
		protected static bool ToggleResetRestPose;
		protected static bool ToggleReorderGameObject;

		protected static string StatusString;
		protected static bool ActiveAvatarRebuilder;
		protected static bool NewAvatarPatched;
		protected static int UndoGroupIndex;

		// 컴포넌트 최초 로드시 동작
		void OnEnable() {
			if (!ActiveAvatarRebuilder) {
				OldAvatarGameObjectEditor = this.gameObject;
				if (OldAvatarGameObjectEditor.GetComponent<Animator>()) {
					OldAvatarAnimator = OldAvatarGameObjectEditor.GetComponent<Animator>();
				}
				if (OldAvatarAnimator) {
					if (OldAvatarAnimator.GetBoneTransform(HumanBodyBones.Hips)) {
						AvatarRootBoneEditor = OldAvatarAnimator.GetBoneTransform(HumanBodyBones.Hips);
					}
				}
				SetStaticVariable();
			}
		}

		/// <summary>에디터 변수 -> 정적 변수 동기화합니다.</summary>
		void SetStaticVariable() {
			NewAvatarGameObject = NewAvatarGameObjectEditor;
			OldAvatarGameObject = OldAvatarGameObjectEditor;
			if (Enum.IsDefined(typeof(Avatar), AvatarTypeIndexEditor)) TargetAvatar = (Avatar)AvatarTypeIndexEditor;
			NewAvatarSkinnedMeshRenderers = NewAvatarSkinnedMeshRenderersEditor;
			AvatarRootBone = AvatarRootBoneEditor;
            ToggleRestoreArmatureTransform = ToggleRestoreArmatureTransformEditor;
			ToggleResetRestPose = ToggleResetRestPoseEditor;
			ToggleReorderGameObject = ToggleReorderGameObjectEditor;
		}

		/// <summary>정적 변수 -> 에디터 변수 동기화합니다.</summary>
		void SetEditorVariable() {
			NewAvatarGameObjectEditor = NewAvatarGameObject;
			OldAvatarGameObjectEditor = OldAvatarGameObject;
			NewAvatarSkinnedMeshRenderersEditor = NewAvatarSkinnedMeshRenderers;
			AvatarRootBoneEditor = AvatarRootBone;
			StatusStringEditor = StatusString;
		}

		/// <summary>패치 대상 SkinnedMeshRenderer 목록을 가져옵니다</summary>
		public void UpdateSkinnedMeshRendererList() {
            SetStaticVariable();
			ClearVariable();
			if (VerifyVariable()) {
				RecoveryAvatar.GetSkinnedMeshRenderers();
				StatusString = "UPDATED_RENDERER";
			}
            SetEditorVariable();
        }

		/// <summary>
		/// 본 프로그램의 메인 세팅 로직입니다.
		/// </summary>
		public void ReplaceSkinnedMeshRendererGameObjects() {
			Undo.IncrementCurrentGroup();
			Undo.SetCurrentGroupName("VRSuya Avatar Rebuilder");
            UndoGroupIndex = Undo.GetCurrentGroup();
			ActiveAvatarRebuilder = true;
			SetStaticVariable();
			ClearVariable();
			AvatarHandler.CheckExistNewAvatarInScene();
			if (VerifyVariable()) {
				AvatarHandler.CreateDuplicateAvatar();
				AvatarHandler.RequestCheckNewAvatar();
				RecoveryAvatar.GetSkinnedMeshRenderers();
				RecoveryAvatar.Recovery();
				Debug.Log($"[VRSuya] Update Completed");
				ActiveAvatarRebuilder = false;
				DestroyImmediate(this);
			}
			SetEditorVariable();
			ActiveAvatarRebuilder = false;
        }

		/// <summary>정적 변수를 초기화 합니다.</summary>
		static void ClearVariable() {
			NewAvatarSkinnedMeshRenderers = new SkinnedMeshRenderer[0];
			NewAvatarPatched = false;
			StatusString = "";
        }

		/// <summary>아바타의 현재 상태를 검사하여 설치가 가능한지 확인합니다.</summary>
		/// <returns>설치 가능 여부</returns>
		static bool VerifyVariable() {
			if (!NewAvatarGameObject) {
				StatusString = "NO_AVATAR";
				return false;
			}
			if (NewAvatarGameObject == OldAvatarGameObject) {
				StatusString = "SAME_OBJECT";
				return false;
			}
			NewAvatarGameObject.TryGetComponent(typeof(Animator), out Component NewAnimator);
			if (!NewAnimator) {
				StatusString = "NO_NEW_ANIMATOR";
				return false;
			} else {
				NewAvatarAnimator = NewAvatarGameObject.GetComponent<Animator>();
			}
			if (!AvatarRootBone) {
				OldAvatarGameObject.TryGetComponent(typeof(Animator), out Component OldAnimator);
				if (!OldAnimator) {
					StatusString = "NO_OLD_ANIMATOR";
					return false;
				} else {
					OldAvatarAnimator = OldAvatarGameObject.GetComponent<Animator>();
					if (OldAvatarAnimator.GetBoneTransform(HumanBodyBones.Hips)) {
						AvatarRootBone = OldAvatarAnimator.GetBoneTransform(HumanBodyBones.Hips);
					} else {
						StatusString = "NO_ROOTBONE";
						return false;
					}
				}
			}
			return true;
        }
    }
}
#endif