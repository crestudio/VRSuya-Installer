#if UNITY_EDITOR
using UnityEngine;

/*
 * VRSuya Installer
 * Contact : vrsuya@gmail.com // Twitter : https://twitter.com/VRSuya
 */

namespace VRSuya.Installer {

	internal class AvatarRebuilderContext {

		internal GameObject OldAvatarGameObject;
		internal GameObject NewAvatarGameObject;

		internal Animator OldAvatarAnimator;
		internal Animator NewAvatarAnimator;

		internal int UndoGroupIndex;
	}
}
#endif
