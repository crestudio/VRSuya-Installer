using UnityEngine;
using UnityEditor;

/*
 * VRSuya Suyasuya Facial Editor
 * Contact : vrsuya@gmail.com // Twitter : https://twitter.com/VRSuya
 */

namespace com.vrsuya.suyasuyafacial {

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

		public static int LanguageIndex = 0;
        public readonly string[] LanguageType = new[] { "English", "한국어", "日本語" };
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
			LanguageIndex = EditorGUILayout.Popup(LanguageHelper.GetContextString("String_Language"), LanguageIndex, LanguageType);
			EditorGUILayout.Space(EditorGUIUtility.singleLineHeight);
            EditorGUILayout.PropertyField(SerializedAvatarGameObject, new GUIContent(LanguageHelper.GetContextString("String_TargetAvatar")));
			EditorGUILayout.PropertyField(SerializedAvatarHeadSkinnedMeshRenderer, new GUIContent(LanguageHelper.GetContextString("String_TargetMesh")));
			EditorGUILayout.PropertyField(SerializedAvatarFXAnimatorController, new GUIContent(LanguageHelper.GetContextString("String_TargetFXLayer")));
			EditorGUILayout.PropertyField(SerializedTargetAnimationClips, new GUIContent(LanguageHelper.GetContextString("String_TargetAnimations")));
			if (GUILayout.Button(LanguageHelper.GetContextString("String_Reload"))) {
				(target as SuyasuyaFacial).ReloadVariable();
				Repaint();
			}
			EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
			EditorGUI.indentLevel++;
			if (SerializedTargetBlendShapes.arraySize > 0) {
				FoldAvatar = EditorGUILayout.Foldout(FoldAvatar, LanguageHelper.GetContextString("String_TargetBlendShape"));
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
				EditorGUILayout.LabelField(LanguageHelper.GetContextString("String_TargetBlendShape"));
				EditorGUILayout.HelpBox(LanguageHelper.GetContextString("NO_SHAPEKEY"), MessageType.Info);
			}
			EditorGUILayout.Space(EditorGUIUtility.singleLineHeight);
			if (SerializedTargetAnimationBlendShapes.arraySize > 0) {
				FoldAnimation = EditorGUILayout.Foldout(FoldAnimation, LanguageHelper.GetContextString("String_TargetAnimationBlendShape"));
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
				EditorGUILayout.LabelField(LanguageHelper.GetContextString("String_TargetAnimationBlendShape"));
				EditorGUILayout.HelpBox(LanguageHelper.GetContextString("NO_ANIMSHAPEKEY"), MessageType.Info);
			}
			EditorGUI.indentLevel--;
			EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
			if (!string.IsNullOrEmpty(SerializedStatusCode.stringValue)) {
				EditorGUILayout.HelpBox(ReturnStatusString(SerializedStatusCode.stringValue), MessageType.Warning);
            }
			serializedObject.ApplyModifiedProperties();
            if (GUILayout.Button(LanguageHelper.GetContextString("String_UpdateAnimations"))) {
                (target as SuyasuyaFacial).UpdateAnimationClips();
				Repaint();
			}
		}

		/// <summary>요청한 StatusCode를 요청한 언어로 번역하여 현재 데이터 결과를 반영한 String으로 반환합니다.</summary>
		/// <returns>완전한 StatusCode의 String</returns>
		string ReturnStatusString(string StatusCode) {
			string ReturnString = LanguageHelper.GetContextString(StatusCode);
			if (SerializedCountUpdatedCurve.intValue > 0) {
				ReturnString = string.Format(ReturnString, SerializedCountUpdatedCurve.intValue);
			}
			return ReturnString;
		}
	}
}
