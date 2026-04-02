using UnityEngine;
using UnityEditor;

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

		public static int LanguageIndex = 0;
		public readonly string[] LanguageType = new[] { "English", "한국어", "日本語" };
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
			AvatarAuthorNames = LanguageHelper.ReturnAvatarAuthorName(SerializedAvatarAuthors);
			LanguageIndex = EditorGUILayout.Popup(LanguageHelper.GetContextString("String_Language"), LanguageIndex, LanguageType);
			EditorGUILayout.Space(EditorGUIUtility.singleLineHeight);
			EditorGUILayout.PropertyField(SerializedAvatarGameObject, new GUIContent(LanguageHelper.GetContextString("String_TargetAvatar")));
			AvatarAuthorType = EditorGUILayout.Popup(LanguageHelper.GetContextString("String_AvatarAuthor"), AvatarAuthorType, AvatarAuthorNames);
			SerializedProperty SelectedAvatarAuthorProperty = SerializedAvatarAuthors.GetArrayElementAtIndex(AvatarAuthorType);
			SelectedAvatarAuthor = SelectedAvatarAuthorProperty.enumNames[SelectedAvatarAuthorProperty.enumValueIndex];
			(target as AnimationOffsetUpdater).TargetAvatarAuthorName = SelectedAvatarAuthor;
            EditorGUILayout.PropertyField(SerializedAvatarAnimationClips, new GUIContent(LanguageHelper.GetContextString("String_AnimationClip")));
			GUI.enabled = false;
            EditorGUILayout.PropertyField(SerializedAnimationOriginPosition, new GUIContent(LanguageHelper.GetContextString("String_AnimationOrigin")));
            EditorGUILayout.PropertyField(SerializedAvatarOriginPosition, new GUIContent(LanguageHelper.GetContextString("String_AvatarOrigin")));
            GUI.enabled = true;
            EditorGUILayout.PropertyField(SerializedAnimationStrength, new GUIContent(LanguageHelper.GetContextString("String_AnimationStrength")));
            if (GUILayout.Button(LanguageHelper.GetContextString("String_GetPosition"))) {
                (target as AnimationOffsetUpdater).UpdateOriginPositions();
            }
            if (!string.IsNullOrEmpty(SerializedStatusCode.stringValue)) {
                EditorGUILayout.HelpBox(LanguageHelper.GetContextString(SerializedStatusCode.stringValue), MessageType.Warning);
            }
            serializedObject.ApplyModifiedProperties();
			EditorGUILayout.LabelField(string.Empty, GUI.skin.horizontalSlider);
			if (GUILayout.Button(LanguageHelper.GetContextString("String_UpdateAnimation"))) {
                (target as AnimationOffsetUpdater).UpdateAnimationOffset();
            }
        }
    }
}