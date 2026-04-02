#if MODULAR_AVATAR
using System.Collections.Generic;

using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;

using VRC.SDK3.Avatars.Components;
using static VRC.SDK3.Avatars.Components.VRCAvatarDescriptor;

using nadena.dev.ndmf;
using nadena.dev.ndmf.vrchat;

using static VRSuya.Core.Translator;

using Avatar = VRSuya.Core.Avatar;
using Object = UnityEngine.Object;

/*
 * VRSuya Modular Component
 * Contact : vrsuya@gmail.com // Twitter : https://twitter.com/VRSuya
 */

[assembly: ExportsPlugin(typeof(VRSuya.Modular.Editor.RemoveAnimatorLayerPlugin))]

namespace VRSuya.Modular.Editor {

    public class RemoveAnimatorLayerPlugin : Plugin<RemoveAnimatorLayerPlugin> {

		public override string QualifiedName => "com.vrsuya.modular.removeanimatorlayer";
		public override string DisplayName => "VRSuya Remove Animator Layer";

		protected override void Configure() {
			InPhase(BuildPhase.Optimizing).Run(RemoveAnimatorLayerPass.Instance);
		}
	}

	public class RemoveAnimatorLayerPass : Pass<RemoveAnimatorLayerPass> {

		public override string DisplayName => "Remove Animator Layer";

		protected override void Execute(BuildContext TargetBuildContext) {
			RemoveAnimatorLayer[] RemoveAnimatorLayerComponents = TargetBuildContext.AvatarRootObject.GetComponentsInChildren<RemoveAnimatorLayer>(true);
			if (RemoveAnimatorLayerComponents.Length > 0) {
				VRCAvatarDescriptor TargetAvatarDescriptor = TargetBuildContext.VRChatAvatarDescriptor();
				if (TargetAvatarDescriptor) {
					Avatar AvatarInstance = new Avatar();
					AnimatorController TargetFXLayer = AvatarInstance.GetAnimatorController(TargetBuildContext.AvatarRootObject, AnimLayerType.FX);
					if (TargetFXLayer) {
						List<string> RemoveLayerNames = new List<string>();
						foreach (RemoveAnimatorLayer TargetComponent in RemoveAnimatorLayerComponents) {
							if (TargetComponent) {
								foreach (string LayerName in TargetComponent.TargetLayerName) {
									if (!string.IsNullOrEmpty(LayerName) && !RemoveLayerNames.Contains(LayerName)) {
										RemoveLayerNames.Add(LayerName);
									}
								}
							}
						}
						for (int Index = TargetFXLayer.layers.Length - 1; Index >= 0; Index--) {
							string LayerName = TargetFXLayer.layers[Index].name;
							if (RemoveLayerNames.Contains(LayerName) && LayerName != "TypeAFK") {
								TargetFXLayer.RemoveLayer(Index);
							}
						}
					}
				}
				foreach (RemoveAnimatorLayer TargetComponent in RemoveAnimatorLayerComponents) {
					if (TargetComponent) Object.DestroyImmediate(TargetComponent);
				}
			}
		}
	}

	[CustomEditor(typeof(RemoveAnimatorLayer))]
	public class RemoveAnimatorLayerEditor : UnityEditor.Editor {

		SerializedProperty SerializedTargetLayerName;

		void OnEnable() {
			SerializedTargetLayerName = serializedObject.FindProperty("TargetLayerName");
		}

		public override void OnInspectorGUI() {
			serializedObject.Update();
			LanguageIndex = EditorGUILayout.Popup(GetTranslatedString("String_Language"), LanguageIndex, LanguageOption);
			EditorGUILayout.Space();
			EditorGUILayout.PropertyField(SerializedTargetLayerName, new GUIContent(GetTranslatedString("String_LayerName")));
			serializedObject.ApplyModifiedProperties();
		}
	}
}
#endif