using System;

using UnityEditor;
using UnityEditor.Animations;

using VRC.SDK3.Avatars.Components;
using static VRC.SDK3.Avatars.Components.VRCAvatarDescriptor;

using nadena.dev.ndmf;
using nadena.dev.ndmf.vrchat;

using Avatar = VRSuya.Core.Avatar;
using Object = UnityEngine.Object;

/*
 * VRSuya Modular Component
 * Contact : vrsuya@gmail.com // Twitter : https://twitter.com/VRSuya
 */

[assembly: ExportsPlugin(typeof(VRSuya.Modular.Editor.RemoveAFKLayerPlugin))]

namespace VRSuya.Modular.Editor {

    public class RemoveAFKLayerPlugin : Plugin<RemoveAFKLayerPlugin> {

		public override string QualifiedName => "com.vrsuya.modular.removeafklayer";
		public override string DisplayName => "VRSuya Remove AFK Layer";

		protected override void Configure() {
			InPhase(BuildPhase.Transforming).Run(RemoveAFKLayerPass.Instance);
		}
	}

	public class RemoveAFKLayerPass : Pass<RemoveAFKLayerPass> {

		public override string DisplayName => "Remove AFK Layer";

		protected override void Execute(BuildContext TargetBuildContext) {
			RemoveAFKLayer TargetComponent = TargetBuildContext.AvatarRootObject.GetComponentInChildren<RemoveAFKLayer>();
			if (!TargetComponent) return;
			VRCAvatarDescriptor TargetAvatarDescriptor = TargetBuildContext.VRChatAvatarDescriptor();
			if (!TargetAvatarDescriptor) return;
			Avatar AvatarInstance = new Avatar();
			AnimatorController TargetFXLayer = AvatarInstance.GetAnimatorController(TargetBuildContext.AvatarRootObject, AnimLayerType.FX);
			if (!TargetFXLayer) return;
			for (int Index = TargetFXLayer.layers.Length - 1; Index >= 0; Index--) {
				string LayerName = TargetFXLayer.layers[Index].name;
				if (LayerName.Contains("AFK", StringComparison.OrdinalIgnoreCase) && LayerName != "TypeAFK") {
					TargetFXLayer.RemoveLayer(Index);
				}
			}
			Object.DestroyImmediate(TargetComponent);
		}
	}

	[CustomEditor(typeof(RemoveAFKLayer))]
	public class RemoveAFKLayerEditor : UnityEditor.Editor {

		public override void OnInspectorGUI() {
			EditorGUILayout.HelpBox("아바타 업로드할 때 FX 레이어에서 기존 AFK 레이어를 제거합니다", MessageType.Info);
			EditorGUILayout.HelpBox("アバターをアップロードする際、FXレイヤー内の既存のAFKレイヤーは削除されます。", MessageType.Info);
			EditorGUILayout.HelpBox("When uploading the avatar, the existing AFK layer in the FX layer will be removed.", MessageType.Info);
		}
	}
}
