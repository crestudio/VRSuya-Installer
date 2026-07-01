#if UNITY_EDITOR
using UnityEngine;

using VRC.SDKBase;

/*
 * VRSuya Modular Component
 * Contact : vrsuya@gmail.com // Twitter : https://twitter.com/VRSuya
 */

namespace VRSuya.Modular {

	[AddComponentMenu("VRSuya/Modular/VRSuya FixFacialAnimation")]
	[HelpURL("https://vrsuya.booth.pm/")]
	public class FixFacialAnimation : MonoBehaviour, IEditorOnly {
		public AnimationClip[] TargetAnimationClips;
		public bool AddBlinkBlendshape = true;
		public string[] TargetBlendshapes;
		public bool AddLayerControl = true;
		public int[] TargetLayerIndexs = new int[] { 1, 2 };
	}
}
#endif