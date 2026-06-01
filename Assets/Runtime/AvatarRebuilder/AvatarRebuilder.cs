#if UNITY_EDITOR
using System;

using UnityEditor;
using UnityEngine;

using VRSuya.Core;

using static VRSuya.Core.Unity;

using Animator = UnityEngine.Animator;

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
				StatusString = "COMPLETED";
			} else {
				CreateBackup();
				if (!NewAvatarGameObject.scene.IsValid()) {
					PlaceGameObjectInScene();
					if (!CheckNewAvatar()) return StatusString;
				}
				AvatarRebuilderContext Context = new AvatarRebuilderContext {
					OldAvatarGameObject = OldAvatarGameObject,
					NewAvatarGameObject = NewAvatarGameObject,
					OldAvatarAnimator = OldAvatarAnimator,
					NewAvatarAnimator = NewAvatarAnimator,
					UndoGroupIndex = UndoGroupIndex
				};
				AvatarHandler AvatarHandlerInstance = new AvatarHandler();
				AvatarHandlerInstance.Context = Context;
				StatusString = AvatarHandlerInstance.UpdateAvatarArmature();
			}
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

		void CreateBackup() {
			DuplicateGameObject DuplicatorInstance = new DuplicateGameObject();
			GameObject DuplicatedAvatar = DuplicatorInstance.DuplicateGameObjectInstance(OldAvatarGameObject);
			Undo.RegisterCreatedObjectUndo(DuplicatedAvatar, UndoGroupName);
			DuplicatedAvatar.name = $"{OldAvatarGameObject.name} (Backup)";
			DuplicatedAvatar.transform.SetSiblingIndex(OldAvatarGameObject.transform.GetSiblingIndex() + 1);
			DuplicatedAvatar.SetActive(false);
			EditorUtility.SetDirty(DuplicatedAvatar);
			Undo.CollapseUndoOperations(UndoGroupIndex);
		}

		void PlaceGameObjectInScene() {
			GameObject NewAvatarGameObjectInstance = Instantiate(NewAvatarGameObject);
			NewAvatarGameObjectInstance.name = NewAvatarGameObject.name;
			NewAvatarGameObjectInstance.hideFlags = HideFlags.HideAndDontSave;
			Undo.RegisterCreatedObjectUndo(NewAvatarGameObjectInstance, UndoGroupName);
			Undo.CollapseUndoOperations(UndoGroupIndex);
			NewAvatarGameObject = NewAvatarGameObjectInstance;
		}
	}
}
#endif