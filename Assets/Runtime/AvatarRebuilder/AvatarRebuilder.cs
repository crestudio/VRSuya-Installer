#if UNITY_EDITOR
using System;

using UnityEditor;
using UnityEngine;

using static VRSuya.Core.Unity;

/*
 * VRSuya Installer
 * Contact : vrsuya@gmail.com // Twitter : https://twitter.com/VRSuya
 */

namespace VRSuya.Installer {

    [ExecuteInEditMode]
	public class AvatarRebuilder : ScriptableObject {

        public GameObject OldAvatarGameObject;
        public GameObject NewAvatarGameObject;

        Animator OldAvatarAnimator;
		Animator NewAvatarAnimator;

		Transform OldAvatarRootBone;
		Transform NewAvatarRootBone;

		const string UndoGroupName = "VRSuya AvatarRebuilder";
		int UndoGroupIndex;
		string StatusString;

		public string RequestRebuildAvatar() {
			UndoGroupIndex = InitializeUndoGroup(UndoGroupName);
			if (!CheckOldAvatar()) return StatusString;
			CopyModelImporter();
			if (!CheckNewAvatar()) return StatusString;
			if (IsVarientModelPrefab()) {
				ReplaceModelAsset();
			} else {

			}
			StatusString = "COMPLETED";
			return StatusString;
		}

		bool CheckOldAvatar() {
			if (!OldAvatarGameObject) {
				StatusString = "NO_OLD_AVATAR";
				return false;
			}
			OldAvatarAnimator = OldAvatarGameObject.GetComponent<Animator>();
			if (!OldAvatarAnimator) {
				StatusString = "NO_OLD_ANIMATOR";
				return false;
			}
			OldAvatarRootBone = OldAvatarAnimator.GetBoneTransform(HumanBodyBones.Hips);
			if (!OldAvatarRootBone) {
				StatusString = "NO_OLD_ROOTBONE";
				return false;
			}
			return true;
		}

		bool CheckNewAvatar() {
			if (!NewAvatarGameObject) {
				StatusString = "NO_NEW_AVATAR";
				return false;
			}
			if (NewAvatarGameObject == OldAvatarGameObject) {
				StatusString = "SAME_AVATAR";
				return false;
			}
			NewAvatarAnimator = NewAvatarGameObject.GetComponent<Animator>();
			if (!NewAvatarAnimator) {
				StatusString = "NO_NEW_ANIMATOR";
				return false;
			}
			NewAvatarRootBone = NewAvatarAnimator.GetBoneTransform(HumanBodyBones.Hips);
			if (!NewAvatarRootBone) {
				StatusString = "NO_NEW_ROOTBONE";
				return false;
			}
			return true;
		}

		void CopyModelImporter() {
			string OldModelPath = AssetDatabase.GetAssetPath(OldAvatarAnimator.avatar);
			string NewModelPath = AssetDatabase.GetAssetPath(NewAvatarGameObject);
			if (!string.IsNullOrEmpty(NewModelPath)) {
				CopyModelImporter CopyModelImporterInstance = new CopyModelImporter();
				CopyModelImporterInstance.RequestCopyModelImporter(OldModelPath, NewModelPath, UndoGroupIndex);
			}
		}

		bool IsVarientModelPrefab() {
			if (!PrefabUtility.IsPartOfVariantPrefab(OldAvatarGameObject)) return false;
			GameObject PrefabSource = PrefabUtility.GetCorrespondingObjectFromSource(OldAvatarGameObject);
			if (!PrefabSource) return false;
			string PrefabSourceAssetPath = AssetDatabase.GetAssetPath(PrefabSource);
			return PrefabSourceAssetPath.EndsWith(".fbx", StringComparison.OrdinalIgnoreCase);
		}

		void ReplaceModelAsset() {
			string OldModelPath = AssetDatabase.GetAssetPath(OldAvatarAnimator.avatar);
			string NewModelPath = AssetDatabase.GetAssetPath(NewAvatarAnimator.avatar);
			ReplaceModelAsset ReplaceModelAssetInstance = new ReplaceModelAsset();
			ReplaceModelAssetInstance.RequestReplaceModelAsset(OldModelPath, NewModelPath);
		}
	}
}
#endif