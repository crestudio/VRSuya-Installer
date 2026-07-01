#if UNITY_EDITOR
using System;
using System.Collections.Generic;

using UnityEngine;

using VRC.SDKBase;

/*
 * VRSuya Modular Component
 * Contact : vrsuya@gmail.com // Twitter : https://twitter.com/VRSuya
 */

namespace VRSuya.Modular {

	[AddComponentMenu("VRSuya/Modular/VRSuya AnimationClipRenamer")]
	[HelpURL("https://vrsuya.booth.pm/")]
	public class AnimationClipRenamer : MonoBehaviour, IEditorOnly {

		[Serializable]
		public struct RenameExpression {
			public string Before;
			public string After;

			public RenameExpression(string BeforeWord, string AfterWord) {
				Before = BeforeWord;
				After = AfterWord;
			}
		};

		public AnimationClip[] TargetAnimationClips;
		public List<RenameExpression> TargetPathRenameList;
		public List<RenameExpression> TargetBlendshapeRenameList;
	}
}
#endif