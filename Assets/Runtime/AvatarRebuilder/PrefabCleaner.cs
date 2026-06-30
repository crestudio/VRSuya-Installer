#if UNITY_EDITOR
using System;
using System.IO;
using System.Linq;

using UnityEditor;
using UnityEngine;

using VRSuya.Core;

/*
 * VRSuya Installer
 * Contact : vrsuya@gmail.com // Twitter : https://twitter.com/VRSuya
 */

namespace VRSuya.Installer {

	internal class PrefabCleaner {

		AvatarRebuilderContext Context;

		internal PrefabCleaner(ref AvatarRebuilderContext Context) {
			this.Context = Context;
		}

		const string TargetPropertyPath = "m_Bones";

		const string UndoGroupName = "VRSuya AvatarRebuilder";

		internal bool RequestCleanupPrefab() {
			if (UnityUtility.IsVariantModelPrefab(Context.OldAvatarGameObject)) {
				return ClearPrefabObjectRecursively(Context.OldAvatarGameObject);
			}
			return false;
		}

		void CreatePrefabBackup(string TargetPrefabAssetPath) {
			string TargetPrefabFilePath = Path.GetFullPath(TargetPrefabAssetPath);
			string BackupAssetPath = GetPrefabBackupAssetPath(TargetPrefabAssetPath);
			string BackupFilePath = Path.GetFullPath(BackupAssetPath);
			File.Copy(TargetPrefabFilePath, BackupFilePath);
			Context.BackupPrefabFilePath.Add(BackupFilePath, TargetPrefabFilePath);
			AssetDatabase.Refresh();
		}

		string GetPrefabBackupAssetPath(string OldAssetPath) {
			string DirectoryPath = Path.GetDirectoryName(OldAssetPath);
			string FileName = Path.GetFileNameWithoutExtension(OldAssetPath);
			string Date = DateTime.Now.ToString("yyMMdd");
			string RandomSuffix = Guid.NewGuid().ToString("N").Substring(0, 2).ToUpper();
			return $"{DirectoryPath}/{FileName}_Backup_{Date}_{RandomSuffix}.prefab";
		}

		bool ClearPrefabObjectRecursively(GameObject TargetGameObject) {
			bool IsModified = false;
			if (!PrefabUtility.IsPartOfVariantPrefab(TargetGameObject)) return IsModified;
			GameObject PrefabSourceGameObject = TargetGameObject;
			string PrefabSourceAssetPath = AssetDatabase.GetAssetPath(TargetGameObject);
			while (PrefabSourceGameObject) {
				string CurrentAssetPath = AssetDatabase.GetAssetPath(PrefabSourceGameObject);
				if (!string.IsNullOrEmpty(CurrentAssetPath)) PrefabSourceAssetPath = CurrentAssetPath;
				if (HasBoneProperty(PrefabSourceGameObject)) {
					if (!PrefabSourceAssetPath.EndsWith(".fbx", StringComparison.OrdinalIgnoreCase)) {
						CreatePrefabBackup(AssetDatabase.GetAssetPath(PrefabSourceGameObject));
					}
					if (ClearPrefabObject(PrefabSourceGameObject)) {
						IsModified = true;
					}
				}
				GameObject ParentPrefabSourceGameObject = PrefabUtility.GetCorrespondingObjectFromSource(PrefabSourceGameObject);
				if (!ParentPrefabSourceGameObject) break;
				PrefabSourceGameObject = ParentPrefabSourceGameObject;
			}
			return IsModified;
		}

		bool HasBoneProperty(GameObject TargetGameObject) {
			if (!PrefabUtility.IsPartOfPrefabInstance(TargetGameObject)) return false;
			PropertyModification[] TargetPropertyModifications = PrefabUtility.GetPropertyModifications(TargetGameObject);
			if (TargetPropertyModifications == null || TargetPropertyModifications.Length == 0) return false;
			return TargetPropertyModifications.Any(Item => Item.propertyPath.StartsWith(TargetPropertyPath));
		}

		bool ClearPrefabObject(GameObject TargetGameObject) {
			bool IsChanged = false;
			if (!PrefabUtility.IsPartOfPrefabInstance(TargetGameObject)) return IsChanged;
			PropertyModification[] OldPropertyModifications = PrefabUtility.GetPropertyModifications(TargetGameObject);
			if (OldPropertyModifications == null || OldPropertyModifications.Length == 0) return IsChanged;
			PropertyModification[] NewPropertyModifications = OldPropertyModifications
				.Where(Item => !Item.propertyPath.StartsWith(TargetPropertyPath))
				.ToArray();
			if (!NewPropertyModifications.SequenceEqual(OldPropertyModifications)) {
				Undo.RecordObject(TargetGameObject, UndoGroupName);
				PrefabUtility.SetPropertyModifications(TargetGameObject, NewPropertyModifications);
				EditorUtility.SetDirty(TargetGameObject);
				Undo.CollapseUndoOperations(Context.UndoGroupIndex);
				AssetDatabase.SaveAssetIfDirty(TargetGameObject);
			}
			return IsChanged;
		}
	}
}
#endif
