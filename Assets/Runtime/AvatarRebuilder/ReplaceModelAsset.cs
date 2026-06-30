#if UNITY_EDITOR
using System;
using System.IO;

using UnityEditor;

/*
 * VRSuya Installer
 * Contact : vrsuya@gmail.com // Twitter : https://twitter.com/VRSuya
 */

namespace VRSuya.Installer {

	public class ReplaceModelAsset {

		AvatarRebuilderContext Context;

		internal ReplaceModelAsset(ref AvatarRebuilderContext Context) {
			this.Context = Context;
		}

		public void RequestReplaceModelAsset(string OldModelPath, string NewModelPath, int UndoGroupIndex) {
			string OldAvatarFilePath = Path.GetFullPath(OldModelPath);
			string NewAvatarFilePath = Path.GetFullPath(NewModelPath);
			string NewAvatarMetaFilePath = $"{NewAvatarFilePath}.meta";
			string BackupAssetPath = GetBackupModelPath(OldModelPath);
			string BackupFilePath = Path.GetFullPath(BackupAssetPath);
			File.Copy(OldAvatarFilePath, BackupFilePath);
			Context.OverwriteModelFilePath = OldAvatarFilePath;
			Context.BackupModelFilePath = BackupFilePath;
			AssetDatabase.ImportAsset(BackupAssetPath);
			CopyModelImporter CopyModelImporterInstance = new CopyModelImporter();
			CopyModelImporterInstance.RequestCopyModelImporter(OldModelPath, BackupAssetPath, true, UndoGroupIndex);
			File.Copy(NewAvatarFilePath, OldAvatarFilePath, overwrite: true);
			File.Delete(NewAvatarFilePath);
			File.Delete(NewAvatarMetaFilePath);
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