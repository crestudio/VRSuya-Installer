#if UNITY_EDITOR
using UnityEngine;

using VRC.SDKBase;

/*
 * VRSuya Modular Component
 * Contact : vrsuya@gmail.com // Twitter : https://twitter.com/VRSuya
 */

namespace VRSuya.Modular {

	[AddComponentMenu("VRSuya/Modular/VRSuya MenuSelector")]
	[HelpURL("https://vrsuya.booth.pm/")]
	public class MenuSelector : MonoBehaviour, IEditorOnly {
		public int TargetMenuType = 0;
	}
}
#endif