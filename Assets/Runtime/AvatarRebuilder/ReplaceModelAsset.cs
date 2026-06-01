#if UNITY_EDITOR
using System;
using System.IO;

using UnityEditor;
using UnityEngine;

/*
 * VRSuya Installer
 * Contact : vrsuya@gmail.com // Twitter : https://twitter.com/VRSuya
 */

namespace VRSuya.Installer {

	public class ReplaceModelAsset {

		public void RequestReplaceModelAsset(string OldModelPath, string NewModelPath, int UndoGroupIndex) {
			string OldAvatarFullPath = Path.GetFullPath(OldModelPath);
			string NewAvatarFullPath = Path.GetFullPath(NewModelPath);
			string NewAvatarMetaFullPath = $"{NewAvatarFullPath}.meta";
			string BackupAssetPath = GetBackupModelPath(OldModelPath);
			string BackupFullPath = Path.GetFullPath(BackupAssetPath);
			File.Copy(OldAvatarFullPath, BackupFullPath);
			AssetDatabase.ImportAsset(BackupAssetPath);
			CopyModelImporter CopyModelImporterInstance = new CopyModelImporter();
			CopyModelImporterInstance.RequestCopyModelImporter(OldModelPath, BackupAssetPath, UndoGroupIndex);
			File.Copy(NewAvatarFullPath, OldAvatarFullPath, overwrite: true);
			File.Delete(NewAvatarFullPath);
			File.Delete(NewAvatarMetaFullPath);
			AssetDatabase.Refresh();
		}

		string GetBackupModelPath(string OldAssetPath) {
			string DirectoryPath = Path.GetDirectoryName(OldAssetPath);
			string FileName = Path.GetFileNameWithoutExtension(OldAssetPath);
			string Date = DateTime.Now.ToString("yyMMdd");
			string RandomSuffix = Guid.NewGuid().ToString("N").Substring(0, 2).ToUpper();
			return $"{DirectoryPath}/{FileName}_Backup_{Date}_{RandomSuffix}.fbx";
		}
	}
}
#endif