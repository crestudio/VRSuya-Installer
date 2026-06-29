#if UNITY_EDITOR
using System;
using System.Diagnostics;
using System.IO;

using UnityEditor;
using UnityEngine;

using static VRSuya.Core.Translator;

using Debug = UnityEngine.Debug;

/*
 * VRSuya Installer
 * Contact : vrsuya@gmail.com // Twitter : https://twitter.com/VRSuya
 */

namespace VRSuya.Installer {

	public class HDiffPatcher {

		public string RequestHDiffPatch(string TargetFilePath, string HDiffFilePath) {
			if (!IsValidInputFilePath(TargetFilePath, HDiffFilePath)) return null;
			string OutputFilePath = GetOutputFilePath(TargetFilePath);
			if (string.IsNullOrEmpty(OutputFilePath)) return null;
			if (!IsValidOutputFilePath(OutputFilePath)) return null;
			string HDiffPatchFilePath = GetHDiffPatchFilePath();
			if (string.IsNullOrEmpty(HDiffPatchFilePath)) return null;
			if (!GetPermission(HDiffPatchFilePath)) return null;
			bool PatchResult = false;
			try {
				EditorUtility.DisplayProgressBar(
					"VRSuya HDiffPatcher",
					$"Processing : {Path.GetFileName(TargetFilePath)}",
					0.5f
				);
				PatchResult = PatchHDiffPatch(
					HDiffPatchFilePath,
					TargetFilePath,
					HDiffFilePath,
					OutputFilePath
				);
			} finally {
				EditorUtility.ClearProgressBar();
			}
			if (!PatchResult) return null;
			string OutputAssetPath = GetUnityAssetPath(OutputFilePath);
			AssetDatabase.ImportAsset(OutputAssetPath, ImportAssetOptions.ForceUpdate);
			return OutputAssetPath;
		}

		bool IsValidInputFilePath(string TargetFilePath, string HDiffFilePath) {
			if (string.IsNullOrEmpty(TargetFilePath) || !File.Exists(TargetFilePath)) {
				Debug.LogError($"[VRSuya] HDiffPatcher : {GetTranslatedString("ERROR_FBX")}");
				return false;
			}
			if (string.IsNullOrEmpty(HDiffFilePath) || !File.Exists(HDiffFilePath)) {
				Debug.LogError($"[VRSuya] HDiffPatcher : {GetTranslatedString("ERROR_HDIFF")}");
				return false;
			}
			return true;
		}

		bool IsValidOutputFilePath(string OutputFilePath) {
			string OutputFullFilePath = Path.GetFullPath(OutputFilePath);
			string AssetDatabasePath = Path.GetFullPath(Application.dataPath);
			if (!OutputFullFilePath.StartsWith(AssetDatabasePath, StringComparison.OrdinalIgnoreCase)) {
				Debug.LogError($"[VRSuya] HDiffPatcher : {GetTranslatedString("ERROR_OUTPUTPATH")}");
				return false;
			}
			return true;
		}

		string GetOutputFilePath(string TargetFilePath) {
			string TargetFileDirectory = Path.GetDirectoryName(TargetFilePath);
			string TargetFileName = Path.GetFileNameWithoutExtension(TargetFilePath);
			string Date = DateTime.Now.ToString("yyMMdd");
			string RandomSuffix = Guid.NewGuid().ToString("N").Substring(0, 2).ToUpper();
			string NewAssetName = $"{TargetFileName}_{Date}_{RandomSuffix}.fbx";
			return Path.Combine(TargetFileDirectory, NewAssetName);
		}

		string GetUnityAssetPath(string TargetFilePath) {
			string AssetPath = Path.GetFullPath(TargetFilePath).Replace('\\', '/');
			string UnityProjectAssetsPath = Path.GetFullPath(Application.dataPath + "/..").Replace('\\', '/');
			return AssetPath.Substring(UnityProjectAssetsPath.Length + 1);
		}

		string GetHDiffPatchFilePath() {
			string Platform = GetPlatform();
			if (Platform == null) {
				Debug.LogError($"[VRSuya] HDiffPatcher : {GetTranslatedString("ERROR_PLATFORM")}");
				return null;
			}
			string HDiffFileName = Application.platform == RuntimePlatform.WindowsEditor ? "hpatchz.exe" : "hpatchz";
			string[] AssetGUIDs = AssetDatabase.FindAssets("hpatchz", new[] { $"Packages/com.vrsuya.installer/Library/HDiffPatch/{Platform}" });
			foreach (string AssetGUID in AssetGUIDs) {
				string AssetPath = AssetDatabase.GUIDToAssetPath(AssetGUID);
				if (Path.GetFileName(AssetPath) == HDiffFileName) {
					return Path.GetFullPath(AssetPath);
				}
			}
			Debug.LogError($"[VRSuya] HDiffPatcher : {GetTranslatedString("ERROR_NOHDIFFPATCH")}");
			return null;
		}

		string GetPlatform() {
			switch (Application.platform) {
				case RuntimePlatform.WindowsEditor: return "Windows";
				case RuntimePlatform.OSXEditor: return "Mac";
				case RuntimePlatform.LinuxEditor: return "Linux";
				default: return null;
			}
		}

		bool GetPermission(string HDiffPatchFilePath) {
			if (Application.platform == RuntimePlatform.WindowsEditor) return true;
			try {
				Process chmodProcess = new Process();
				chmodProcess.StartInfo = new ProcessStartInfo {
					FileName = "chmod",
					Arguments = $"+x \"{HDiffPatchFilePath}\"",
					UseShellExecute = false,
					CreateNoWindow = true
				};
				chmodProcess.Start();
				chmodProcess.WaitForExit(5000);
				return true;
			} catch (Exception chmodException) {
				Debug.LogError($"[VRSuya] HDiffPatcher : {GetTranslatedString("ERROR_NOPERMISSION")}\n{chmodException.Message}");
				return false;
			}
		}

		bool PatchHDiffPatch(string HDiffPatchFilePath, string TargetFilePath, string HDiffFilePath, string OutputFilePath) {
			string ProcessArguments = $"\"{TargetFilePath}\" \"{HDiffFilePath}\" \"{OutputFilePath}\"";
			bool ProcessResult = RunHDiffPatchProcess(
				HDiffPatchFilePath,
				ProcessArguments,
				out string ProcessStandardError
			);
			if (!ProcessResult) {
				Debug.LogError($"[VRSuya] HDiffPatcher : {GetTranslatedString("ERROR_FAILEDRUN")}\n{ProcessStandardError}");
				if (ProcessStandardError.Contains("oldDataSize")) {
					Debug.LogError($"[VRSuya] HDiffPatcher : {GetTranslatedString("NOT_MATCH")}");
				}
				return false;
			}
			return true;
		}

		bool RunHDiffPatchProcess(string HDiffPatchFilePath, string ProcessArguments, out string StandardErrorOutput) {
			StandardErrorOutput = string.Empty;
			try {
				using (Process HDiffPatchProcess = new Process()) {
					HDiffPatchProcess.StartInfo = new ProcessStartInfo {
						FileName = HDiffPatchFilePath,
						Arguments = ProcessArguments,
						UseShellExecute = false,
						RedirectStandardOutput = true,
						RedirectStandardError = true,
						CreateNoWindow = true
					};
					HDiffPatchProcess.Start();
					bool ProcessCompleted = HDiffPatchProcess.WaitForExit(300000);
					StandardErrorOutput = HDiffPatchProcess.StandardError.ReadToEnd();
					if (!ProcessCompleted) {
						HDiffPatchProcess.Kill();
						Debug.LogError($"[VRSuya] HDiffPatcher : {GetTranslatedString("ERROR_TIMEDOUT")}");
						return false;
					}
					return HDiffPatchProcess.ExitCode == 0;
				}
			} catch (Exception HDiffPatchException) {
				Debug.LogError($"[VRSuya] HDiffPatcher : {HDiffPatchException.Message}");
				return false;
			}
		}
	}
}
#endif