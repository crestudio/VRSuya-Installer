using System;

using UnityEngine;
using UnityEditor;

/*
 * VRSuya Avatar Setting Updater Editor
 * Contact : vrsuya@gmail.com // Twitter : https://twitter.com/VRSuya
 */

namespace com.vrsuya.installer {

    [CustomEditor(typeof(AvatarSettingUpdater))]
    public class AvatarSettingUpdaterEditor : Editor {

        SerializedProperty SerializedAvatarGameObject;
        SerializedProperty SerializedChangeTwosidedShadow;
        SerializedProperty SerializedChangeAnchorOverride;
        SerializedProperty SerializedAvatarAnchorOverride;
		SerializedProperty SerializedChangeBounds;
		SerializedProperty SerializedKeepAnimatorController;
		SerializedProperty SerializedKeepLinkAnimatorLayer;
		SerializedProperty SerializedStatusCode;
		SerializedProperty SerializedStatusNeedMoreSpaceMenu;
		SerializedProperty SerializedStatusNeedMoreSpaceParameter;
		SerializedProperty SerializedInstalledVRSuyaProductAvatarsEditor;

		// 제품 추가시 추가해야 될 변수
		SerializedProperty SerializedInstalledProductAFK;
		SerializedProperty SerializedInstalledProductMogumogu;
		SerializedProperty SerializedInstalledProductWotagei;
		SerializedProperty SerializedInstalledProductFeet;
		SerializedProperty SerializedInstalledProductNyoronyoro;
		SerializedProperty SerializedInstalledProductModelWalking;
		SerializedProperty SerializedInstalledProductHandmotion;
		SerializedProperty SerializedInstalledProductSuyasuya;
		SerializedProperty SerializedInstalledProductSoundPad;
		// 검색용 VRSuya 아이템 업데이트 위치

		SerializedProperty SerializedInstallProductAFK;
		SerializedProperty SerializedInstallProductMogumogu;
		SerializedProperty SerializedInstallProductWotagei;
		SerializedProperty SerializedInstallProductFeet;
		SerializedProperty SerializedInstallProductNyoronyoro;
		SerializedProperty SerializedInstallProductModelWalking;
		SerializedProperty SerializedInstallProductHandmotion;
		SerializedProperty SerializedInstallProductSuyasuya;
		SerializedProperty SerializedInstallProductSoundPad;
		// 검색용 VRSuya 아이템 업데이트 위치

		public static int LanguageIndex = 0;
        public readonly string[] LanguageType = new[] { "English", "한국어", "日本語" };
		public static int AvatarType = 0;
		public static string[] AvatarNames = new string[0];
		public static string SelectedAvatarName = "";
		public static bool FoldAdvanced = false;
		public static int StatusNeedMoreSpaceMenu;
		public static int StatusNeedMoreSpaceParameter;
		static readonly string[] StringFormatCode = new string[] { "NO_MORE_MENU", "NO_MORE_PARAMETER" };

		void OnEnable() {
            SerializedAvatarGameObject = serializedObject.FindProperty("AvatarGameObjectEditor");
            SerializedChangeTwosidedShadow = serializedObject.FindProperty("ChangeTwosidedShadowEditor");
            SerializedChangeAnchorOverride = serializedObject.FindProperty("ChangeAnchorOverrideEditor");
			SerializedChangeBounds = serializedObject.FindProperty("ChangeBoundsEditor");
			SerializedKeepAnimatorController = serializedObject.FindProperty("KeepAnimatorControllerEditor");
			SerializedKeepLinkAnimatorLayer = serializedObject.FindProperty("KeepLinkAnimatorLayerEditor");
			SerializedAvatarAnchorOverride = serializedObject.FindProperty("AvatarAnchorOverrideEditor");
			SerializedStatusCode = serializedObject.FindProperty("StatusCodeEditor");
			SerializedStatusNeedMoreSpaceMenu = serializedObject.FindProperty("StatusNeedMoreSpaceMenuEditor");
			SerializedStatusNeedMoreSpaceParameter = serializedObject.FindProperty("StatusNeedMoreSpaceParameterEditor");
			SerializedInstalledVRSuyaProductAvatarsEditor = serializedObject.FindProperty("InstalledVRSuyaProductAvatarsEditor");

			// 제품 추가시 추가해야 될 변수
			SerializedInstalledProductAFK = serializedObject.FindProperty("InstalledProductAFKEditor");
			SerializedInstalledProductMogumogu = serializedObject.FindProperty("InstalledProductMogumoguEditor");
			SerializedInstalledProductWotagei = serializedObject.FindProperty("InstalledProductWotageiEditor");
			SerializedInstalledProductFeet = serializedObject.FindProperty("InstalledProductFeetEditor");
			SerializedInstalledProductNyoronyoro = serializedObject.FindProperty("InstalledProductNyoronyoroEditor");
			SerializedInstalledProductModelWalking = serializedObject.FindProperty("InstalledProductModelWalkingEditor");
			SerializedInstalledProductHandmotion = serializedObject.FindProperty("InstalledProductHandmotionEditor");
			SerializedInstalledProductSuyasuya = serializedObject.FindProperty("InstalledProductSuyasuyaEditor");
			SerializedInstalledProductSoundPad = serializedObject.FindProperty("InstalledProductSoundPadEditor");
			// 검색용 VRSuya 아이템 업데이트 위치

			SerializedInstallProductAFK = serializedObject.FindProperty("InstallProductAFKEditor");
			SerializedInstallProductMogumogu = serializedObject.FindProperty("InstallProductMogumoguEditor");
			SerializedInstallProductWotagei = serializedObject.FindProperty("InstallProductWotageiEditor");
			SerializedInstallProductFeet = serializedObject.FindProperty("InstallProductFeetEditor");
			SerializedInstallProductNyoronyoro = serializedObject.FindProperty("InstallProductNyoronyoroEditor");
			SerializedInstallProductModelWalking = serializedObject.FindProperty("InstallProductModelWalkingEditor");
			SerializedInstallProductHandmotion = serializedObject.FindProperty("InstallProductHandmotionEditor");
			SerializedInstallProductSuyasuya = serializedObject.FindProperty("InstallProductSuyasuyaEditor");
			SerializedInstallProductSoundPad = serializedObject.FindProperty("InstallProductSoundPadEditor");
			// 검색용 VRSuya 아이템 업데이트 위치
		}

        public override void OnInspectorGUI() {
            serializedObject.Update();

			AvatarNames = LanguageHelper.ReturnAvatarName(SerializedInstalledVRSuyaProductAvatarsEditor);
			LanguageIndex = EditorGUILayout.Popup(LanguageHelper.GetContextString("String_Language"), LanguageIndex, LanguageType);
			EditorGUILayout.Space(EditorGUIUtility.singleLineHeight);
            EditorGUILayout.PropertyField(SerializedAvatarGameObject, new GUIContent(LanguageHelper.GetContextString("String_TargetAvatar")));

			AvatarType = EditorGUILayout.Popup(LanguageHelper.GetContextString("String_Avatar"), AvatarType, AvatarNames);
			SerializedProperty SelectedAvatar = SerializedInstalledVRSuyaProductAvatarsEditor.GetArrayElementAtIndex(AvatarType);
			SelectedAvatarName = SelectedAvatar.enumNames[SelectedAvatar.enumValueIndex];
			(target as AvatarSettingUpdater).AvatarTypeNameEditor = SelectedAvatarName;

			FoldAdvanced = EditorGUILayout.Foldout(FoldAdvanced, LanguageHelper.GetContextString("String_Advanced"));

			if (FoldAdvanced) {
				EditorGUI.indentLevel++;
				EditorGUILayout.PropertyField(SerializedChangeTwosidedShadow, new GUIContent(LanguageHelper.GetContextString("String_TwoSidedShadow")));
				EditorGUILayout.PropertyField(SerializedChangeAnchorOverride, new GUIContent(LanguageHelper.GetContextString("String_ChangeAnchorOverride")));
				EditorGUILayout.PropertyField(SerializedChangeBounds, new GUIContent(LanguageHelper.GetContextString("String_ChangeBounds")));
				EditorGUILayout.PropertyField(SerializedKeepAnimatorController, new GUIContent(LanguageHelper.GetContextString("String_KeepAnimatorController")));
				if (SerializedKeepAnimatorController.boolValue == true) {
					EditorGUILayout.HelpBox(LanguageHelper.GetContextString("String_KeepAnimatorController_Info"), MessageType.Info);
				}
				// EditorGUILayout.PropertyField(SerializedKeepLinkAnimatorLayer, new GUIContent(LanguageHelper.GetContextString("String_KeepLinkAnimatorLayer")));
				EditorGUILayout.PropertyField(SerializedAvatarAnchorOverride, new GUIContent(LanguageHelper.GetContextString("String_ObjectAnchorOverride")));
				if (GUILayout.Button(LanguageHelper.GetContextString("String_GetAvatarData"))) {
					(target as AvatarSettingUpdater).UpdateUnityEditorStatus();
				}
				EditorGUI.indentLevel--;
				EditorGUILayout.Space(EditorGUIUtility.singleLineHeight);
			}

			EditorGUILayout.LabelField(LanguageHelper.GetContextString("String_SetupProduct"), EditorStyles.boldLabel);
			EditorGUI.indentLevel++;

			if (SelectedAvatarName == "None") {
				EditorGUILayout.HelpBox(ReturnStatusString("NO_VRSUYA_FILE"), MessageType.Warning);
			}
			// 제품 추가시 추가해야 될 변수
			GUI.enabled = ReturnInstalled(AvatarSettingUpdater.ProductName.AFK, SerializedInstalledProductAFK);
			EditorGUILayout.PropertyField(SerializedInstallProductAFK, new GUIContent(LanguageHelper.GetContextString("String_ProductAFK")));
			GUI.enabled = ReturnInstalled(AvatarSettingUpdater.ProductName.Mogumogu, SerializedInstalledProductMogumogu);
			EditorGUILayout.PropertyField(SerializedInstallProductMogumogu, new GUIContent(LanguageHelper.GetContextString("String_ProductMogumogu")));
			GUI.enabled = ReturnInstalled(AvatarSettingUpdater.ProductName.Wotagei, SerializedInstalledProductWotagei);
			EditorGUILayout.PropertyField(SerializedInstallProductWotagei, new GUIContent(LanguageHelper.GetContextString("String_ProductWotagei")));
			GUI.enabled = ReturnInstalled(AvatarSettingUpdater.ProductName.Feet, SerializedInstalledProductFeet);
			EditorGUILayout.PropertyField(SerializedInstallProductFeet, new GUIContent(LanguageHelper.GetContextString("String_ProductFeet")));
            GUI.enabled = ReturnInstalled(AvatarSettingUpdater.ProductName.Nyoronyoro, SerializedInstalledProductNyoronyoro);
            EditorGUILayout.PropertyField(SerializedInstallProductNyoronyoro, new GUIContent(LanguageHelper.GetContextString("String_ProductNyoronyoro")));
			// GUI.enabled = ReturnInstalled(AvatarSettingUpdater.ProductName.ModelWalking, SerializedInstalledProductModelWalking);
			// EditorGUILayout.PropertyField(SerializedInstallProductModelWalking, new GUIContent(LanguageHelper.GetContextString("String_ProductModelWalking")));
			GUI.enabled = ReturnInstalled(AvatarSettingUpdater.ProductName.Handmotion, SerializedInstalledProductHandmotion);
			EditorGUILayout.PropertyField(SerializedInstallProductHandmotion, new GUIContent(LanguageHelper.GetContextString("String_ProductHandmotion")));
			GUI.enabled = ReturnInstalled(AvatarSettingUpdater.ProductName.Suyasuya, SerializedInstalledProductSuyasuya);
			EditorGUILayout.PropertyField(SerializedInstallProductSuyasuya, new GUIContent(LanguageHelper.GetContextString("String_ProductSuyasuya")));
			GUI.enabled = ReturnInstalled(AvatarSettingUpdater.ProductName.SoundPad, SerializedInstalledProductSoundPad);
			EditorGUILayout.PropertyField(SerializedInstallProductSoundPad, new GUIContent(LanguageHelper.GetContextString("String_ProductSoundPad")));
			// 검색용 VRSuya 아이템 업데이트 위치

			EditorGUI.indentLevel--;
			GUI.enabled = true;
			if (!string.IsNullOrEmpty(SerializedStatusCode.stringValue)) {
				EditorGUILayout.HelpBox(ReturnStatusString(SerializedStatusCode.stringValue), MessageType.Warning);
            }
			serializedObject.ApplyModifiedProperties();
			EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
			if (GUILayout.Button(LanguageHelper.GetContextString("String_UpdateAvatarData") + " (" + SelectedAvatarName + ")")) {
                (target as AvatarSettingUpdater).UpdateAvatarSetting(LanguageIndex);
				Repaint();
			}
			/* if (GUILayout.Button(LanguageHelper.GetContextString("String_Debug"))) {
				(target as AvatarSettingUpdater).UpdateAvatarSetting();
				Repaint();
			} */
			if (GUILayout.Button(LanguageHelper.GetContextString("String_Undo"))) {
				Undo.PerformUndo();
				Repaint();
			}
		}

		/// <summary>요청한 VRSuya 제품의 아바타 파일이 설치 되어있는지 검사합니다.</summary>
		/// <returns>에셋 설치 여부</returns>
		static bool ReturnInstalled(AvatarSettingUpdater.ProductName RequestProductName, SerializedProperty ProductProperty) {
			if (ProductProperty.boolValue && AvatarSettingUpdater.ReturnAvatarInstalled(RequestProductName, SelectedAvatarName)) return true;
			return false;
		}

		/// <summary>요청한 StatusCode를 요청한 언어로 번역하여 현재 데이터 결과를 반영한 String으로 반환합니다.</summary>
		/// <returns>완전한 StatusCode의 String</returns>
		string ReturnStatusString(string StatusCode) {
			string ReturnString = LanguageHelper.GetContextString(StatusCode);
			if (Array.Exists(StringFormatCode, Code => StatusCode == Code)) {
				switch (StatusCode) {
					case "NO_MORE_MENU":
						StatusNeedMoreSpaceMenu = SerializedStatusNeedMoreSpaceMenu.intValue;
						ReturnString = string.Format(ReturnString, StatusNeedMoreSpaceMenu);
						break;
					case "NO_MORE_PARAMETER":
						StatusNeedMoreSpaceParameter = SerializedStatusNeedMoreSpaceParameter.intValue;
						ReturnString = string.Format(ReturnString, StatusNeedMoreSpaceParameter);
						break;
				}
			}
			return ReturnString;
		}
	}
}
