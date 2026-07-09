#if UNITY_EDITOR
using System.IO;
using System.Linq;

using UnityEditor;
using UnityEngine;

using VRSuya.Core;

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

		public AvatarRebuilder(GameObject NewOldAvatarGameObject, GameObject NewNewAvatarGameObject) {
			OldAvatarGameObject = NewOldAvatarGameObject;
			NewAvatarGameObject = NewNewAvatarGameObject;
		}

		public AvatarRebuilderContext Context;

		const string UndoGroupName = "VRSuya AvatarRebuilder";
		string StatusString;
		public bool CanRevert = false;

		public string RequestRebuildAvatar() {
			if (!CheckPrefabMode()) return StatusString;
			Context = new AvatarRebuilderContext {
				OldAvatarGameObject = OldAvatarGameObject,
				NewAvatarGameObject = NewAvatarGameObject,
				UndoGroupIndex = UnityUtility.InitializeUndoGroup(UndoGroupName)
			};
			if (!CheckOldAvatar()) return StatusString;
			if (!CheckNewAvatar()) return StatusString;
			if (UnityUtility.IsVariantModelPrefab(Context.OldAvatarGameObject)) {
				if (!ReplaceModelAsset()) return StatusString;
				PrefabCleaner PrefabCleanerInstance = new PrefabCleaner(ref Context);
				PrefabCleanerInstance.RequestCleanupPrefab();
				StatusString = "COMPLETED";
			} else {
				CopyModelImporter();
				if (!CheckNewAvatarAnimator()) return StatusString;
				CreateBackup();
				string PatchedAvatarAssetPath = AssetDatabase.GetAssetPath(Context.NewAvatarGameObject);
				Context.PatchedAvatarFilePath = Path.GetFullPath(PatchedAvatarAssetPath);
				if (!Context.NewAvatarGameObject.scene.IsValid()) {
					PlaceGameObjectInScene();
					if (!CheckNewAvatar()) return StatusString;
				}
				RenameGameObjects();
				AvatarHandler AvatarHandlerInstance = new AvatarHandler(Context);
				StatusString = AvatarHandlerInstance.UpdateAvatarArmature();
			}
			if (StatusString == "COMPLETED") {
				NewAvatarGameObject = null;
				CanRevert = true;
			}
			return StatusString;
		}

		public void RequestRevertAvatar() {
			if (CanRevert) {
				RevertAvatar RevertAvatarInstance = new RevertAvatar(ref Context);
				if (UnityUtility.IsVariantModelPrefab(Context.OldAvatarGameObject)) {
					if (!string.IsNullOrEmpty(Context.BackupModelFilePath) && !string.IsNullOrEmpty(Context.OverwriteModelFilePath)) {
						RevertAvatarInstance.RequestRevertModelAsset();
					}
					if (Context.BackupPrefabFilePath.Count != 0) {
						RevertAvatarInstance.RequestRevertPrefabAsset();
					}
				} else {
					OldAvatarGameObject = RevertAvatarInstance.RequestRevertPrefabGameObject();
					RevertAvatarInstance.RequestRemovePatchedModelAsset();
				}
				CanRevert = false;
			}
		}

		bool CheckPrefabMode() {
			if (UnityUtility.IsPrefabEditingMode()) {
				StatusString = "NO_PREFAB_MODE";
				return false;
			}
			return true;
		}

		bool CheckOldAvatar() {
			if (!Context.OldAvatarGameObject) {
				StatusString = "NO_OLD_AVATAR";
				return false;
			}
			if (!Context.OldAvatarGameObject.scene.IsValid()) {
				StatusString = "NO_OLD_AVATAR_SCENE";
				return false;
			}
			Context.OldAvatarAnimator = Context.OldAvatarGameObject.GetComponent<Animator>();
			if (!Context.OldAvatarAnimator) {
				StatusString = "NO_OLD_ANIMATOR";
				return false;
			}
			return true;
		}

		bool CheckNewAvatar() {
			if (!Context.NewAvatarGameObject) {
				StatusString = "NO_NEW_AVATAR";
				return false;
			}
			if (Context.NewAvatarGameObject == Context.OldAvatarGameObject) {
				StatusString = "SAME_AVATAR";
				return false;
			}
			return true;
		}

		bool CheckNewAvatarAnimator() {
			Context.NewAvatarAnimator = Context.NewAvatarGameObject.GetComponent<Animator>();
			if (!Context.NewAvatarAnimator) {
				StatusString = "NO_NEW_ANIMATOR";
				return false;
			}
			if (Context.OldAvatarAnimator.avatar == Context.NewAvatarAnimator.avatar) {
				StatusString = "SAME_AVATAR";
				return false;
			}
			return true;
		}

		void CopyModelImporter() {
			string OldModelPath = AssetDatabase.GetAssetPath(Context.OldAvatarAnimator.avatar);
			string NewModelPath = AssetDatabase.GetAssetPath(Context.NewAvatarGameObject);
			if (!string.IsNullOrEmpty(NewModelPath)) {
				CopyModelImporter CopyModelImporterInstance = new CopyModelImporter();
				CopyModelImporterInstance.RequestCopyModelImporter(OldModelPath, NewModelPath, false, Context.UndoGroupIndex);
			}
		}

		bool ReplaceModelAsset() {
			string OldModelPath = AssetDatabase.GetAssetPath(Context.OldAvatarAnimator.avatar);
			string NewModelPath = AssetDatabase.GetAssetPath(Context.NewAvatarGameObject);
			if (OldModelPath == NewModelPath) {
				StatusString = "SAME_AVATAR";
				return false;
			}
			if (!string.IsNullOrEmpty(NewModelPath)) {
				ReplaceModelAsset ReplaceModelAssetInstance = new ReplaceModelAsset(ref Context);
				ReplaceModelAssetInstance.RequestReplaceModelAsset(OldModelPath, NewModelPath, Context.UndoGroupIndex);
				return true;
			}
			return false;
		}

		void CreateBackup() {
			GameObject DuplicatedAvatar = DuplicateUtility.DuplicateGameObject(Context.OldAvatarGameObject);
			Undo.RegisterCreatedObjectUndo(DuplicatedAvatar, UndoGroupName);
			DuplicatedAvatar.name = $"{Context.OldAvatarGameObject.name} (Backup)";
			DuplicatedAvatar.transform.SetSiblingIndex(Context.OldAvatarGameObject.transform.GetSiblingIndex() + 1);
			DuplicatedAvatar.SetActive(false);
			EditorUtility.SetDirty(DuplicatedAvatar);
			Undo.CollapseUndoOperations(Context.UndoGroupIndex);
			Context.BackupAvatarGameObject = DuplicatedAvatar;
			Selection.activeGameObject = Context.OldAvatarGameObject;
		}

		void PlaceGameObjectInScene() {
			GameObject NewAvatarGameObjectInstance = Instantiate(Context.NewAvatarGameObject);
			NewAvatarGameObjectInstance.name = NewAvatarGameObject.name;
			NewAvatarGameObjectInstance.transform.position = OldAvatarGameObject.transform.position;
			NewAvatarGameObjectInstance.transform.rotation = OldAvatarGameObject.transform.rotation;
			NewAvatarGameObjectInstance.transform.localScale = OldAvatarGameObject.transform.localScale;
			NewAvatarGameObjectInstance.hideFlags = HideFlags.HideAndDontSave;
			Undo.RegisterCreatedObjectUndo(NewAvatarGameObjectInstance, UndoGroupName);
			Undo.CollapseUndoOperations(Context.UndoGroupIndex);
			Context.NewAvatarGameObject = NewAvatarGameObjectInstance;
		}

		void RenameGameObjects() {
			Transform[] NewAvatarTransforms = Context.NewAvatarGameObject.GetComponentsInChildren<Transform>(true);
			Transform EYOHairTransform = NewAvatarTransforms.FirstOrDefault(Item => Item.gameObject.name == "Eyo_hair 1");
			if (EYOHairTransform) {
				Undo.RecordObject(EYOHairTransform, UndoGroupName);
				EYOHairTransform.name = "Eyo_hair";
				EditorUtility.SetDirty(EYOHairTransform);
				Undo.CollapseUndoOperations(Context.UndoGroupIndex);
			}
			Transform IMERISHairTransform = NewAvatarTransforms.FirstOrDefault(Item => Item.gameObject.name == "Imeris_hair 1");
			if (IMERISHairTransform) {
				Undo.RecordObject(IMERISHairTransform, UndoGroupName);
				IMERISHairTransform.name = "Imeris_hair";
				EditorUtility.SetDirty(IMERISHairTransform);
				Undo.CollapseUndoOperations(Context.UndoGroupIndex);
			}
		}
	}
}
#endif