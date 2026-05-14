using UnityEngine;

using VRC.SDKBase;

/*
 * VRSuya Modular Component
 * Contact : vrsuya@gmail.com // Twitter : https://twitter.com/VRSuya
 */

namespace VRSuya.Modular {

	[AddComponentMenu("VRSuya/Modular/VRSuya FixLocomotion")]
	[HelpURL("https://vrsuya.booth.pm/")]
	public class FixLocomotion : MonoBehaviour, IEditorOnly {
		public AnimationClip TargetAnimationClip;
	}
}