using UnityEngine;
using UnityEditor;

using static VRSuya.Core.Translator;

/*
 * VRSuya Suyasuya Facial Editor
 * Contact : vrsuya@gmail.com // Twitter : https://twitter.com/VRSuya
 */

namespace VRSuya.Installer {

    [CustomEditor(typeof(SuyasuyaFacial))]
    public class SuyasuyaFacialEditor : Editor {

        SerializedProperty SerializedAvatarGameObject;
        SerializedProperty SerializedAvatarHeadSkinnedMeshRenderer;
		SerializedProperty SerializedAvatarFXAnimatorController;
		SerializedProperty SerializedTargetAnimationClips;
        SerializedProperty SerializedTargetBlendShapes;
		SerializedProperty SerializedTargetAnimationBlendShapes;
		SerializedProperty SerializedStatusCode;
		SerializedProperty SerializedCountUpdatedCurve;

		public static bool FoldAvatar = false;
		public static bool FoldAnimation = false;

		void OnEnable() {
            SerializedAvatarGameObject = serializedObject.FindProperty("AvatarGameObject");
			SerializedAvatarHeadSkinnedMeshRenderer = serializedObject.FindProperty("AvatarHeadSkinnedMeshRenderer");
			SerializedAvatarFXAnimatorController = serializedObject.FindProperty("AvatarFXAnimatorController");
			SerializedTargetAnimationClips = serializedObject.FindProperty("TargetAnimationClips");
			SerializedTargetBlendShapes = serializedObject.FindProperty("TargetBlendShapes");
			SerializedTargetAnimationBlendShapes = serializedObject.FindProperty("TargetAnimationBlendShapes");
			SerializedStatusCode = serializedObject.FindProperty("StatusCode");
			SerializedCountUpdatedCurve = serializedObject.FindProperty("CountUpdatedCurve");
		}

        public override void OnInspectorGUI() {
            serializedObject.Update();
			LanguageIndex = EditorGUILayout.Popup(GetTranslatedString("String_Language"), LanguageIndex, LanguageOption);
			EditorGUILayout.Space(EditorGUIUtility.singleLineHeight);
            EditorGUILayout.PropertyField(SerializedAvatarGameObject, new GUIContent(GetTranslatedString("String_TargetAvatar")));
			EditorGUILayout.PropertyField(SerializedAvatarHeadSkinnedMeshRenderer, new GUIContent(GetTranslatedString("String_TargetMesh")));
			EditorGUILayout.PropertyField(SerializedAvatarFXAnimatorController, new GUIContent(GetTranslatedString("String_TargetFXLayer")));
			EditorGUILayout.PropertyField(SerializedTargetAnimationClips, new GUIContent(GetTranslatedString("String_TargetAnimations")));
			if (GUILayout.Button(GetTranslatedString("String_Reload"))) {
				(target as SuyasuyaFacial).ReloadVariable();
				Repaint();
			}
			EditorGUILayout.LabelField(string.Empty, GUI.skin.horizontalSlider);
			EditorGUI.indentLevel++;
			if (SerializedTargetBlendShapes.arraySize > 0) {
				FoldAvatar = EditorGUILayout.Foldout(FoldAvatar, GetTranslatedString("String_TargetBlendShape"));
				if (FoldAvatar) {
					for (int Index = 0; Index < SerializedTargetBlendShapes.arraySize; Index++) {
						SerializedProperty BlendShapeProperty = SerializedTargetBlendShapes.GetArrayElementAtIndex(Index);
						SerializedProperty ActiveValueProperty = BlendShapeProperty.FindPropertyRelative("ActiveValue");
						SerializedProperty BlendShapeNameProperty = BlendShapeProperty.FindPropertyRelative("BlendShapeName");
						EditorGUILayout.BeginHorizontal();
						ActiveValueProperty.boolValue = EditorGUILayout.ToggleLeft(BlendShapeNameProperty.stringValue, ActiveValueProperty.boolValue);
						EditorGUILayout.EndHorizontal();
					}
				}
			} else {
				EditorGUILayout.LabelField(GetTranslatedString("String_TargetBlendShape"));
				EditorGUILayout.HelpBox(GetTranslatedString("NO_SHAPEKEY"), MessageType.Info);
			}
			EditorGUILayout.Space(EditorGUIUtility.singleLineHeight);
			if (SerializedTargetAnimationBlendShapes.arraySize > 0) {
				FoldAnimation = EditorGUILayout.Foldout(FoldAnimation, GetTranslatedString("String_TargetAnimationBlendShape"));
				if (FoldAnimation) {
					for (int Index = 0; Index < SerializedTargetAnimationBlendShapes.arraySize; Index++) {
						SerializedProperty BlendShapeProperty = SerializedTargetAnimationBlendShapes.GetArrayElementAtIndex(Index);
						SerializedProperty ActiveValueProperty = BlendShapeProperty.FindPropertyRelative("ActiveValue");
						SerializedProperty BlendShapeNameProperty = BlendShapeProperty.FindPropertyRelative("BlendShapeName");
						EditorGUILayout.BeginHorizontal();
						ActiveValueProperty.boolValue = EditorGUILayout.ToggleLeft(BlendShapeNameProperty.stringValue, ActiveValueProperty.boolValue);
						EditorGUILayout.EndHorizontal();
					}
				}
			} else {
				EditorGUILayout.LabelField(GetTranslatedString("String_TargetAnimationBlendShape"));
				EditorGUILayout.HelpBox(GetTranslatedString("NO_ANIMSHAPEKEY"), MessageType.Info);
			}
			EditorGUI.indentLevel--;
			EditorGUILayout.LabelField(string.Empty, GUI.skin.horizontalSlider);
			if (!string.IsNullOrEmpty(SerializedStatusCode.stringValue)) {
				EditorGUILayout.HelpBox(ReturnStatusString(SerializedStatusCode.stringValue), MessageType.Warning);
            }
			serializedObject.ApplyModifiedProperties();
            if (GUILayout.Button(GetTranslatedString("String_UpdateAnimations"))) {
                (target as SuyasuyaFacial).UpdateAnimationClips();
				Repaint();
			}
		}

		string ReturnStatusString(string StatusCode) {
			string ReturnString = GetTranslatedString(StatusCode);
			if (SerializedCountUpdatedCurve.intValue > 0) {
				ReturnString = string.Format(ReturnString, SerializedCountUpdatedCurve.intValue);
			}
			return ReturnString;
		}
	}
}
