using UnityEngine;

using VRC.SDK3.Dynamics.Constraint.Components;
using VRC.SDKBase;

/*
 * VRSuya Modular Component
 * Contact : vrsuya@gmail.com // Twitter : https://twitter.com/VRSuya
 */

namespace VRSuya.Modular {

	[AddComponentMenu("VRSuya/Modular/VRSuya ConstraintConnector")]
	[HelpURL("https://vrsuya.booth.pm/")]
	public class ConstraintConnector : MonoBehaviour, IEditorOnly {
		public VRCParentConstraint LeftHandConstraint;
		public VRCParentConstraint RightHandConstraint;
	}
}