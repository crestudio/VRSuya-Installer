#if UNITY_EDITOR
using System;
using System.Linq;

using UnityEditor;
using UnityEngine;
using UnityEngine.Animations;

/*
 * VRSuya Avatar Setting Updater
 * Contact : vrsuya@gmail.com // Twitter : https://twitter.com/VRSuya
 */

namespace com.vrsuya.installer {

	[ExecuteInEditMode]
	[AddComponentMenu("")]
	public class ProductSetup_Wotagei : ProductSetup {

		static VRSuyaProduct Wotagei;
		static GameObject VRSuyaWotageiGameObject;

		/// <summary>제품 정보를 AssetManager에게 요청하여 업데이트 한 후, 설치된 에셋 목록에 추가합니다.</summary>
		internal static void RegisterProduct() {
			InstalledProductWotagei = false;
			Wotagei = new VRSuyaProduct();
			Wotagei = AssetManager.UpdateProductInformation(ProductName.Wotagei);
			InstalledVRSuyaProducts = InstalledVRSuyaProducts.Concat(new VRSuyaProduct[] { Wotagei }).ToArray();
			if (Wotagei.SupportAvatarList.Length > 0) InstalledProductWotagei = true;
			return;
		}

		/// <summary>외부의 세팅 요청을 처리하는 메인 메소드 입니다.</summary>
		internal static void RequestSetting() {
			if (InstallProductWotagei) {
				VRSuyaWotageiGameObject = Array.Find(VRSuyaGameObjects, gameObject => gameObject.name.Contains("VRSuya_Wotagei"));
				if (!VRSuyaWotageiGameObject) SetupPrefab();
				if (VRSuyaWotageiGameObject) {
					UpdateParentConstraints();
					UpdatePrefabName();
				}
			}
			return;
		}

		/// <summary>아바타에 Prefab이 있는지 검사하고 없으면 설치하는 메소드 입니다.</summary>
		static void SetupPrefab() {
			string[] ChildAvatarGameObjectNames = new string[0];
			foreach (Transform ChildTransform in AvatarGameObject.transform) {
				ChildAvatarGameObjectNames = ChildAvatarGameObjectNames.Concat(new string[] { ChildTransform.name }).ToArray();
			}
			if (!Array.Exists(ChildAvatarGameObjectNames, GameObjectName => GameObjectName.Contains("VRSuya_Wotagei"))) {
				string[] PrefabFilePaths = new string[0];
				PrefabFilePaths = Wotagei.PrefabGUID.Select(AssetGUID => AssetDatabase.GUIDToAssetPath(AssetGUID)).ToArray();
				string TargetPrefabPath = Array.Find(PrefabFilePaths, FilePath => FilePath.Split('/')[FilePath.Split('/').Length - 1].Contains("VRSuya_Wotagei_" + AvatarType.ToString()));
				if (string.IsNullOrEmpty(TargetPrefabPath)) TargetPrefabPath = Array.Find(PrefabFilePaths, FilePath => FilePath.Split('/')[FilePath.Split('/').Length - 1].Contains("VRSuya_Wotagei"));
				if (TargetPrefabPath != null) {
					GameObject TargetPrefab = (GameObject)AssetDatabase.LoadAssetAtPath(TargetPrefabPath, typeof(GameObject));
					GameObject TargetInstance = (GameObject)PrefabUtility.InstantiatePrefab(TargetPrefab);
					Undo.RegisterCreatedObjectUndo(TargetInstance, "Added New GameObject");
					TargetInstance.transform.parent = AvatarGameObject.transform;
					TransformPrefab(TargetInstance, AvatarGameObject, true);
					Undo.CollapseUndoOperations(UndoGroupIndex);
				}
			}
			GetVRSuyaGameObjects();
			VRSuyaWotageiGameObject = Array.Find(VRSuyaGameObjects, gameObject => gameObject.name.Contains("VRSuya_Wotagei"));
			return;
		}

		/// <summary>Parent Constraint 컴포넌트와 아바타의 손을 연결합니다.</summary>
		static void UpdateParentConstraints() {
			GameObject LeftHandGameObject = Array.Find(VRSuyaWotageiGameObject.GetComponentsInChildren<Transform>(true), gameObject => gameObject.name == "LeftHand").gameObject;
			GameObject RightHandGameObject = Array.Find(VRSuyaWotageiGameObject.GetComponentsInChildren<Transform>(true), gameObject => gameObject.name == "RightHand").gameObject;
			if (LeftHandGameObject) {
				ParentConstraint AnchorParentConstraint = LeftHandGameObject.GetComponent<ParentConstraint>();
				if (AnchorParentConstraint) {
					Undo.RecordObject(AnchorParentConstraint, "Changed Parent Constraint");
					AnchorParentConstraint.SetSource(0, new ConstraintSource() { sourceTransform = AvatarAnimator.GetBoneTransform(HumanBodyBones.LeftHand), weight = 1 });
					AnchorParentConstraint.constraintActive = true;
					EditorUtility.SetDirty(AnchorParentConstraint);
					Undo.CollapseUndoOperations(UndoGroupIndex);
				}
			}
			if (RightHandGameObject) {
				ParentConstraint AnchorParentConstraint = RightHandGameObject.GetComponent<ParentConstraint>();
				if (AnchorParentConstraint) {
					Undo.RecordObject(AnchorParentConstraint, "Changed Parent Constraint");
					AnchorParentConstraint.SetSource(0, new ConstraintSource() { sourceTransform = AvatarAnimator.GetBoneTransform(HumanBodyBones.RightHand), weight = 1 });
					AnchorParentConstraint.constraintActive = true;
					EditorUtility.SetDirty(AnchorParentConstraint);
					Undo.CollapseUndoOperations(UndoGroupIndex);
				}
			}
			return;
		}

		/// <summary>Prefab의 이름을 애니메이션 Path 규격에 맞춰 변경합니다.</summary>
		static void UpdatePrefabName() {
			if (VRSuyaWotageiGameObject.name != "VRSuya_Wotagei") {
				Undo.RecordObject(VRSuyaWotageiGameObject, "Changed GameObject Name");
				VRSuyaWotageiGameObject.name = "VRSuya_Wotagei";
				EditorUtility.SetDirty(VRSuyaWotageiGameObject);
				Undo.CollapseUndoOperations(UndoGroupIndex);
			}
		}
	}
}
#endif