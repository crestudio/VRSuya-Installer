#if UNITY_EDITOR
using System.Collections.Generic;

using UnityEngine;

/*
 * VRSuya Installer
 * Contact : vrsuya@gmail.com // Twitter : https://twitter.com/VRSuya
 */

namespace VRSuya.Installer {

	public class AvatarRebuilderContext {

		public GameObject OldAvatarGameObject;
		public GameObject NewAvatarGameObject;

		public Animator OldAvatarAnimator;
		public Animator NewAvatarAnimator;

		public GameObject BackupAvatarGameObject;

		public string OverwriteModelFilePath;
		public string BackupModelFilePath;

		public Dictionary<string, string> BackupPrefabFilePath = new Dictionary<string, string>();

		public int UndoGroupIndex;
	}
}
#endif
