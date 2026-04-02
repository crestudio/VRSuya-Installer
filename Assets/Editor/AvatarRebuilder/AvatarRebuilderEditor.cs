using UnityEngine;
using UnityEditor;

using static VRSuya.Core.Translator;

/*
 * VRSuya AvatarRebuilder
 * Contact : vrsuya@gmail.com // Twitter : https://twitter.com/VRSuya
 * Thanks to Dalgona. & C_Carrot & Naru & Rekorn
 */

namespace VRSuya.Installer {

    [CustomEditor(typeof(AvatarRebuilder))]
    public class AvatarRebuilderEditor : Editor {

        SerializedProperty SerializedNewAvatarGameObject;
        SerializedProperty SerializedOldAvatarGameObject;
        SerializedProperty SerializedNewAvatarSkinnedMeshRenderers;

        SerializedProperty SerializedAvatarRootBone;
        SerializedProperty SerializedToggleRestoreArmatureTransform;
        SerializedProperty SerializedToggleResetRestPose;
        SerializedProperty SerializedToggleReorderGameObject;

		SerializedProperty SerializedStatusString;

		public static int AvatarType = 0;
		public static string[] AvatarNames = new string[0];
        public static bool FoldAdvanced = false;

        void OnEnable() {
            SerializedNewAvatarGameObject = serializedObject.FindProperty("NewAvatarGameObjectEditor");
            SerializedOldAvatarGameObject = serializedObject.FindProperty("OldAvatarGameObjectEditor");
            SerializedNewAvatarSkinnedMeshRenderers = serializedObject.FindProperty("NewAvatarSkinnedMeshRenderersEditor");

            SerializedAvatarRootBone = serializedObject.FindProperty("AvatarRootBoneEditor");
            SerializedToggleRestoreArmatureTransform = serializedObject.FindProperty("ToggleRestoreArmatureTransformEditor");
            SerializedToggleResetRestPose = serializedObject.FindProperty("ToggleResetRestPoseEditor");
            SerializedToggleReorderGameObject = serializedObject.FindProperty("ToggleReorderGameObjectEditor");

			SerializedStatusString = serializedObject.FindProperty("StatusStringEditor");
        }

        public override void OnInspectorGUI() {
            serializedObject.Update();
            AvatarNames = LanguageHelper.ReturnAvatarName();
            LanguageIndex = EditorGUILayout.Popup(GetTranslatedString("String_Language"), LanguageIndex, LanguageOption);
            EditorGUILayout.Space(EditorGUIUtility.singleLineHeight);
            GUI.enabled = false;
            EditorGUILayout.PropertyField(SerializedOldAvatarGameObject, new GUIContent(GetTranslatedString("String_OriginalAvatar")));
            GUI.enabled = true;
            EditorGUILayout.PropertyField(SerializedNewAvatarGameObject, new GUIContent(GetTranslatedString("String_NewAvatar")));
			AvatarType = EditorGUILayout.Popup(GetTranslatedString("String_AvatarType"), AvatarType, AvatarNames);
            (target as AvatarRebuilder).AvatarTypeIndexEditor = AvatarType;
			EditorGUILayout.HelpBox(GetTranslatedString("String_General"), MessageType.Info);
			FoldAdvanced = EditorGUILayout.Foldout(FoldAdvanced, GetTranslatedString("String_Advanced"));
			if (FoldAdvanced) {
				EditorGUI.indentLevel++;
				GUI.enabled = false;
				EditorGUILayout.PropertyField(SerializedAvatarRootBone, new GUIContent(GetTranslatedString("String_RootBone")));
				GUI.enabled = true;
				EditorGUILayout.PropertyField(SerializedToggleRestoreArmatureTransform, new GUIContent(GetTranslatedString("String_RestoreTransform")));
				EditorGUILayout.PropertyField(SerializedToggleResetRestPose, new GUIContent(GetTranslatedString("String_RestPose")));
				EditorGUILayout.PropertyField(SerializedToggleReorderGameObject, new GUIContent(GetTranslatedString("String_ReorderGameObject")));
				GUI.enabled = false;
				EditorGUILayout.PropertyField(SerializedNewAvatarSkinnedMeshRenderers, new GUIContent(GetTranslatedString("String_SkinnedMeshRendererList")));
				GUI.enabled = true;
				if (GUILayout.Button(GetTranslatedString("String_ImportSkinnedMeshRenderer"))) {
					(target as AvatarRebuilder).UpdateSkinnedMeshRendererList();
				}
				EditorGUI.indentLevel--;
			}
			if (!string.IsNullOrEmpty(SerializedStatusString.stringValue)) {
				EditorGUILayout.HelpBox(GetTranslatedString(SerializedStatusString.stringValue), MessageType.Warning);
			}
			serializedObject.ApplyModifiedProperties();
            EditorGUILayout.HelpBox(GetTranslatedString("String_Warning"), MessageType.Warning);
            if (GUILayout.Button(GetTranslatedString("String_ReplaceAvatar"))) {
                (target as AvatarRebuilder).ReplaceSkinnedMeshRendererGameObjects();
            }
		}
    }
}