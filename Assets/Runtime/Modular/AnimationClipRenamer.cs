using System.Collections.Generic;

using UnityEngine;

using VRC.SDKBase;

using static VRSuya.Core.RenameStruct;

/*
 * VRSuya Modular Component
 * Contact : vrsuya@gmail.com // Twitter : https://twitter.com/VRSuya
 */

namespace VRSuya.Modular {

	[AddComponentMenu("VRSuya/Modular/VRSuya AnimationClipRenamer")]
	[HelpURL("https://vrsuya.booth.pm/")]
	public class AnimationClipRenamer : MonoBehaviour, IEditorOnly {
		public AnimationClip[] TargetAnimationClips;
		public List<RenameExpression> TargetPathRenameList;
		public List<RenameExpression> TargetBlendshapeRenameList;
	}
}