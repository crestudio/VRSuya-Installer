#if UNITY_EDITOR
using System;
using System.Linq;

using UnityEditor;
using UnityEngine;

/*
 * VRSuya Avatar Setting Updater
 * Contact : vrsuya@gmail.com // Twitter : https://twitter.com/VRSuya
 */

namespace com.vrsuya.installer {

	[ExecuteInEditMode]
	[AddComponentMenu("")]
	public class ProductSetup_SoundPad : ProductSetup {

		static VRSuyaProduct SoundPad;
		static GameObject VRSuyaSoundPadGameObject;

		/// <summary>제품 정보를 AssetManager에게 요청하여 업데이트 한 후, 설치된 에셋 목록에 추가합니다.</summary>
		internal static void RegisterProduct() {
			InstalledProductSoundPad = false;
			SoundPad = new VRSuyaProduct();
			SoundPad = AssetManager.UpdateProductInformation(ProductName.SoundPad);
			InstalledVRSuyaProducts = InstalledVRSuyaProducts.Concat(new VRSuyaProduct[] { SoundPad }).ToArray();
			if (SoundPad.SupportAvatarList.Length > 0) InstalledProductSoundPad = true;
		}

		/// <summary>외부의 세팅 요청을 처리하는 메인 메소드 입니다.</summary>
		internal static void RequestSetting() {
			if (InstallProductSoundPad) {
				VRSuyaSoundPadGameObject = Array.Find(VRSuyaGameObjects, gameObject => gameObject.name.Contains("VRSuya_SoundPad"));
				if (!VRSuyaSoundPadGameObject) SetupPrefab();
				if (VRSuyaSoundPadGameObject) RepositionPrefab(VRSuyaSoundPadGameObject);
			}
		}

		/// <summary>아바타에 Prefab이 있는지 검사하고 없으면 설치하는 메소드 입니다.</summary>
		static void SetupPrefab() {
			string[] ChildAvatarGameObjectNames = new string[0];
			foreach (Transform ChildTransform in AvatarGameObject.transform) {
				ChildAvatarGameObjectNames = ChildAvatarGameObjectNames.Concat(new string[] { ChildTransform.name }).ToArray();
			}
			if (!Array.Exists(ChildAvatarGameObjectNames, GameObjectName => GameObjectName.Contains("VRSuya_SoundPad"))) {
				string[] PrefabFilePaths = new string[0];
				PrefabFilePaths = SoundPad.PrefabGUID.Select(AssetGUID => AssetDatabase.GUIDToAssetPath(AssetGUID)).ToArray();
				string TargetPrefabPath = Array.Find(PrefabFilePaths, FilePath => FilePath.Split('/')[FilePath.Split('/').Length - 1].Contains("VRSuya_SoundPad_" + AvatarType.ToString()));
				if (string.IsNullOrEmpty(TargetPrefabPath)) TargetPrefabPath = Array.Find(PrefabFilePaths, FilePath => FilePath.Split('/')[FilePath.Split('/').Length - 1].Contains("VRSuya_SoundPad"));
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
			VRSuyaSoundPadGameObject = Array.Find(VRSuyaGameObjects, gameObject => gameObject.name.Contains("VRSuya_SoundPad"));
		}

		/// <summary>Prefab의 Transform을 변경합니다.</summary>
		static void RepositionPrefab(GameObject TargetGameObject) {
			Transform AvatarLeftHand = AvatarAnimator.GetBoneTransform(HumanBodyBones.LeftHand);
			Vector3 newPrefabPosition = AvatarLeftHand.position + new Vector3(-0.1f, -0.05f, 0.025f);
			TargetGameObject.transform.position = newPrefabPosition;
			TargetGameObject.transform.rotation = Quaternion.Euler(-180f, 0f, 0f);
		}
	}
}
#endif