using UnityEngine;
using UnityEditor;

using static VRSuya.Core.Translator;

/*
 * VRSuya Animation Offset Updater Editor for Mogumogu Project
 * Contact : vrsuya@gmail.com // Twitter : https://twitter.com/VRSuya
 */

namespace VRSuya.Installer {

    [CustomEditor(typeof(AnimationOffsetUpdater))]
    public class AnimationOffsetUpdaterEditor : Editor {

        SerializedProperty SerializedAvatarGameObject;
        SerializedProperty SerializedAvatarAnimationClips;
        SerializedProperty SerializedAnimationStrength;
		SerializedProperty SerializedAvatarAuthors;
		SerializedProperty SerializedAnimationOriginPosition;
        SerializedProperty SerializedAvatarOriginPosition;
        SerializedProperty SerializedStatusCode;

		public static int AvatarAuthorType = 0;
		public static string[] AvatarAuthorNames = new string[0];
		public static string SelectedAvatarAuthor = string.Empty;

        void OnEnable() {
            SerializedAvatarGameObject = serializedObject.FindProperty("AvatarGameObject");
            SerializedAvatarAnimationClips = serializedObject.FindProperty("AvatarAnimationClips");
            SerializedAnimationStrength = serializedObject.FindProperty("AnimationStrength");
			SerializedAvatarAuthors = serializedObject.FindProperty("AvatarAuthors");
			SerializedAnimationOriginPosition = serializedObject.FindProperty("AnimationOriginPosition");
            SerializedAvatarOriginPosition = serializedObject.FindProperty("AvatarOriginPosition");
			SerializedStatusCode = serializedObject.FindProperty("StatusCode");
        }

        public override void OnInspectorGUI() {
            serializedObject.Update();
			AvatarAuthorNames = GetAvatarAuthorName(SerializedAvatarAuthors);
			LanguageIndex = EditorGUILayout.Popup(GetTranslatedString("String_Language"), LanguageIndex, LanguageOption);
			EditorGUILayout.Space(EditorGUIUtility.singleLineHeight);
			EditorGUILayout.PropertyField(SerializedAvatarGameObject, new GUIContent(GetTranslatedString("String_TargetAvatar")));
			AvatarAuthorType = EditorGUILayout.Popup(GetTranslatedString("String_AvatarAuthor"), AvatarAuthorType, AvatarAuthorNames);
			SerializedProperty SelectedAvatarAuthorProperty = SerializedAvatarAuthors.GetArrayElementAtIndex(AvatarAuthorType);
			SelectedAvatarAuthor = SelectedAvatarAuthorProperty.enumNames[SelectedAvatarAuthorProperty.enumValueIndex];
			(target as AnimationOffsetUpdater).TargetAvatarAuthorName = SelectedAvatarAuthor;
            EditorGUILayout.PropertyField(SerializedAvatarAnimationClips, new GUIContent(GetTranslatedString("String_AnimationClip")));
			GUI.enabled = false;
            EditorGUILayout.PropertyField(SerializedAnimationOriginPosition, new GUIContent(GetTranslatedString("String_AnimationOrigin")));
            EditorGUILayout.PropertyField(SerializedAvatarOriginPosition, new GUIContent(GetTranslatedString("String_AvatarOrigin")));
            GUI.enabled = true;
            EditorGUILayout.PropertyField(SerializedAnimationStrength, new GUIContent(GetTranslatedString("String_AnimationStrength")));
            if (GUILayout.Button(GetTranslatedString("String_GetPosition"))) {
                (target as AnimationOffsetUpdater).UpdateOriginPositions();
            }
            if (!string.IsNullOrEmpty(SerializedStatusCode.stringValue)) {
                EditorGUILayout.HelpBox(GetTranslatedString(SerializedStatusCode.stringValue), MessageType.Warning);
            }
            serializedObject.ApplyModifiedProperties();
			EditorGUILayout.LabelField(string.Empty, GUI.skin.horizontalSlider);
			if (GUILayout.Button(GetTranslatedString("String_UpdateAnimation"))) {
                (target as AnimationOffsetUpdater).UpdateAnimationOffset();
            }
        }
    }
}