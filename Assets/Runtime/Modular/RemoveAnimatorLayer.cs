using UnityEngine;

using VRC.SDKBase;

/*
 * VRSuya Modular Component
 * Contact : vrsuya@gmail.com // Twitter : https://twitter.com/VRSuya
 */

namespace VRSuya.Modular {

	[AddComponentMenu("VRSuya/Modular/VRSuya RemoveAnimatorLayer")]
	[HelpURL("https://vrsuya.booth.pm/")]
	public class RemoveAnimatorLayer : MonoBehaviour, IEditorOnly {
		public string[] TargetLayerName;
	}
}