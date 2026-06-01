#if UNITY_EDITOR
using UnityEngine;

/*
 * VRSuya Installer
 * Contact : vrsuya@gmail.com // Twitter : https://twitter.com/VRSuya
 */

namespace VRSuya.Installer {

	[ExecuteInEditMode]
	public class AvatarRebuilderContext {

		public GameObject OldAvatarGameObject;
		public GameObject NewAvatarGameObject;

		public Animator OldAvatarAnimator;
		public Animator NewAvatarAnimator;

		public int UndoGroupIndex;
	}
}
#endif
