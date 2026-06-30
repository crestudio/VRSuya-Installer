#if UNITY_EDITOR
using System.IO;

using UnityEditor;
using UnityEngine;

using VRSuya.Core;

/*
 * VRSuya Installer
 * Contact : vrsuya@gmail.com // Twitter : https://twitter.com/VRSuya
 */

namespace VRSuya.Installer {

	internal class RevertAvatar {

		AvatarRebuilderContext Context;

		internal RevertAvatar(ref AvatarRebuilderContext Context) {
			this.Context = Context;
		}

		internal void RequestRevertModelAsset() {
			File.Copy(Context.BackupModelFilePath, Context.OverwriteModelFilePath, overwrite: true);
			CopyModelImporter CopyModelImporterInstance = new CopyModelImporter();
			string OverwriteModelAssetPath = AssetUtility.GetUnityAssetPath(Context.OverwriteModelFilePath);
			string BackupModelAssetPath = AssetUtility.GetUnityAssetPath(Context.BackupModelFilePath);
			CopyModelImporterInstance.RequestCopyModelImporter(BackupModelAssetPath, OverwriteModelAssetPath, true);
			File.Delete(Context.BackupModelFilePath);
			File.Delete($"{Context.BackupModelFilePath}.meta");
			Context.BackupModelFilePath = null;
			Context.OverwriteModelFilePath = null;
			AssetDatabase.Refresh();
		}

		internal void RequestRevertPrefabAsset() {
			foreach (var PrefabKeyPair in Context.BackupPrefabFilePath) {
				File.Copy(PrefabKeyPair.Key, PrefabKeyPair.Value, overwrite: true);
				File.Delete(PrefabKeyPair.Key);
				File.Delete($"{PrefabKeyPair.Key}.meta");
			}
			Context.BackupPrefabFilePath = null;
			AssetDatabase.Refresh();
		}

		internal GameObject RequestRevertPrefabGameObject() {
			Object.DestroyImmediate(Context.OldAvatarGameObject);
			Context.BackupAvatarGameObject.name = Context.BackupAvatarGameObject.name.Replace(" (Backup)", "");
			Context.BackupAvatarGameObject.SetActive(true);
			EditorUtility.SetDirty(Context.BackupAvatarGameObject);
			Context.OldAvatarGameObject = Context.BackupAvatarGameObject;
			Context.BackupAvatarGameObject = null;
			Selection.activeGameObject = Context.BackupAvatarGameObject;
			return Context.OldAvatarGameObject;
		}
	}
}
#endif
