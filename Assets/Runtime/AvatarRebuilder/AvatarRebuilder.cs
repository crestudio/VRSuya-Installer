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

namespace VRSuya.Installer {

    [ExecuteInEditMode]
	[AddComponentMenu("VRSuya/VRSuya AvatarRebuilder")]
	public class AvatarRebuilder : MonoBehaviour {

		public GameObject NewAvatarGameObjectEditor = null;
		public GameObject OldAvatarGameObjectEditor = null;
		public int AvatarTypeIndexEditor;
		public SkinnedMeshRenderer[] NewAvatarSkinnedMeshRenderersEditor = new SkinnedMeshRenderer[0];
		public Transform AvatarRootBoneEditor = null;
		public bool ToggleRestoreArmatureTransformEditor = true;
		public bool ToggleResetRestPoseEditor = false;
		public bool ToggleReorderGameObjectEditor = true;
		public string StatusStringEditor = string.Empty;

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
			Eku, Emmelie, EYO,
			Firina, Flare, Fuzzy,
			Glaze, Grus,
			Hakka,
			IMERIS,
			Karin, Kikyo, Kipfel, Kokoa, Koyuki, KUMALY, Kuronatu,
			Lapwing, Lazuli, Leefa, Leeme, Lime, LUMINA, Lunalitt,
			Mafuyu, Maki, Mamehinata, MANUKA, Mariel, Marron, Maya, MAYO, Merino, Milfy, Milk, Milltina, Minahoshi, Minase, Mint, Mir, Mishe, Moe,
			Nayu, Nehail, Nochica,
			Platinum, Plum, Pochimaru,
			Quiche,
			Rainy, Ramune, Ramune_Old, RINDO, Rokona, Rue, Rurune, Rusk,
			SELESTIA, Sephira, Shinano, Shinra, SHIRAHA, Shiratsume, Sio, Sue, Sugar, Suzuhana,
			Tien, TubeRose,
			Ukon, Usasaki, Uzuki,
			VIVH,
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

		void SetEditorVariable() {
			NewAvatarGameObjectEditor = NewAvatarGameObject;
			OldAvatarGameObjectEditor = OldAvatarGameObject;
			NewAvatarSkinnedMeshRenderersEditor = NewAvatarSkinnedMeshRenderers;
			AvatarRootBoneEditor = AvatarRootBone;
			StatusStringEditor = StatusString;
		}

		public void UpdateSkinnedMeshRendererList() {
            SetStaticVariable();
			ClearVariable();
			if (VerifyVariable()) {
				RecoveryAvatar.GetSkinnedMeshRenderers();
				StatusString = "UPDATED_RENDERER";
			}
            SetEditorVariable();
        }

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

		static void ClearVariable() {
			NewAvatarSkinnedMeshRenderers = new SkinnedMeshRenderer[0];
			NewAvatarPatched = false;
			StatusString = string.Empty;
        }

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