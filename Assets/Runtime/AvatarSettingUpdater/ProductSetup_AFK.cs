#if UNITY_EDITOR
using System;
using System.Linq;

using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.Animations;

using VRC.SDK3.Avatars.Components;

/*
 * VRSuya Avatar Setting Updater
 * Contact : vrsuya@gmail.com // Twitter : https://twitter.com/VRSuya
 */

namespace com.vrsuya.installer {

	[ExecuteInEditMode]
	[AddComponentMenu("")]
	public class ProductSetup_AFK : ProductSetup {

		private static VRSuyaProduct AFK;
		private static GameObject VRSuyaAFKGameObject;

		/// <summary>제품 정보를 AssetManager에게 요청하여 업데이트 한 후, 설치된 에셋 목록에 추가합니다.</summary>
		internal static void RegisterProduct() {
			InstalledProductAFK = false;
			AFK = new VRSuyaProduct();
			AFK = AssetManager.UpdateProductInformation(ProductName.AFK);
			InstalledVRSuyaProducts = InstalledVRSuyaProducts.Concat(new VRSuyaProduct[] { AFK }).ToArray();
			if (AFK.SupportAvatarList.Length > 0) InstalledProductAFK = true;
			return;
		}

		/// <summary>외부의 세팅 요청을 처리하는 메인 메소드 입니다.</summary>
		internal static void RequestSetting() {
			if (InstallProductAFK) {
				VRSuyaAFKGameObject = Array.Find(VRSuyaGameObjects, gameObject => gameObject.name.Contains("VRSuya_AFK_Prefab"));
				if (!VRSuyaAFKGameObject) SetupPrefab();
				if (VRSuyaAFKGameObject) {
					UpdateParentConstraints();
					UpdatePrefabName();
					if (AvatarType == Avatar.Aldina ||
						AvatarType == Avatar.Kipfel ||
						AvatarType == Avatar.Leefa ||
						AvatarType == Avatar.Lunalitt ||
						AvatarType == Avatar.Milfy ||
						AvatarType == Avatar.Minase ||
						AvatarType == Avatar.Shinano ||
						AvatarType == Avatar.Sio ||
						AvatarType == Avatar.Sugar) DisableExistAFKAnimatorLayer();
				}
			}
			return;
		}

		/// <summary>아바타에 Prefab이 있는지 검사하고 없으면 설치하는 메소드 입니다.</summary>
		private static void SetupPrefab() {
			string[] ChildAvatarGameObjectNames = new string[0];
			foreach (Transform ChildTransform in AvatarGameObject.transform) {
				ChildAvatarGameObjectNames = ChildAvatarGameObjectNames.Concat(new string[] { ChildTransform.name }).ToArray();
			}
			if (!Array.Exists(ChildAvatarGameObjectNames, GameObjectName => GameObjectName.Contains("VRSuya_AFK_Prefab"))) {
				string[] PrefabFilePaths = new string[0];
				PrefabFilePaths = AFK.PrefabGUID.Select(AssetGUID => AssetDatabase.GUIDToAssetPath(AssetGUID)).ToArray();
				string TargetPrefabPath = Array.Find(PrefabFilePaths, FilePath => FilePath.Split('/')[FilePath.Split('/').Length - 1].Contains("VRSuya_AFK_Prefab_" + AvatarType.ToString()));
				if (string.IsNullOrEmpty(TargetPrefabPath)) TargetPrefabPath = Array.Find(PrefabFilePaths, FilePath => FilePath.Split('/')[FilePath.Split('/').Length - 1].Contains("VRSuya_AFK_Prefab"));
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
			VRSuyaAFKGameObject = Array.Find(VRSuyaGameObjects, gameObject => gameObject.name.Contains("VRSuya_AFK_Prefab"));
			return;
		}

		/// <summary>Parent Constraint 컴포넌트와 아바타의 손을 연결합니다.</summary>
		private static void UpdateParentConstraints() {
			GameObject VRSuyaAFKAnchorGameObject = Array.Find(VRSuyaAFKGameObject.GetComponentsInChildren<Transform>(true), transform => transform.gameObject.name == "Anchor").gameObject;
			if (VRSuyaAFKAnchorGameObject) {
				ParentConstraint AnchorParentConstraint = VRSuyaAFKAnchorGameObject.GetComponent<ParentConstraint>();
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
		private static void UpdatePrefabName() {
			if (VRSuyaAFKGameObject.name != "VRSuya_AFK_Prefab") {
				Undo.RecordObject(VRSuyaAFKGameObject, "Changed GameObject Name");
				VRSuyaAFKGameObject.name = "VRSuya_AFK_Prefab";
				EditorUtility.SetDirty(VRSuyaAFKGameObject);
				Undo.CollapseUndoOperations(UndoGroupIndex);
			}
		}

		/// <summary>AFK 이름의 애니메이터 레이어를 비활성화 합니다.</summary>
		private static void DisableExistAFKAnimatorLayer() {
			AnimatorController VRCFXLayer = (AnimatorController)Array.Find(AvatarVRCAvatarLayers, VRCAnimator => VRCAnimator.type == VRCAvatarDescriptor.AnimLayerType.FX).animatorController;
			if (VRCFXLayer) {
				float[] AFKLayerWeights = VRCFXLayer.layers
					.Where(Item => Item.name.Contains("AFK"))
					.Where(Item => Item.name != "TypeAFK")
					.Select(Item => Item.defaultWeight)
					.ToArray();
				if (AFKLayerWeights.Any(Item => Item > 0.0f)) {
					AnimatorControllerLayer[] NewAnimationLayers = new AnimatorControllerLayer[VRCFXLayer.layers.Length];
					for (int Index = 0; Index < NewAnimationLayers.Length; Index++) {
						AnimatorControllerLayer NewAnimationLayer = new AnimatorControllerLayer() {
							avatarMask = VRCFXLayer.layers[Index].avatarMask,
							blendingMode = VRCFXLayer.layers[Index].blendingMode,
							defaultWeight = (VRCFXLayer.layers[Index].name.Contains("AFK") && VRCFXLayer.layers[Index].name != "TypeAFK") ? 0.0f : VRCFXLayer.layers[Index].defaultWeight,
							iKPass = VRCFXLayer.layers[Index].iKPass,
							name = VRCFXLayer.layers[Index].name,
							stateMachine = VRCFXLayer.layers[Index].stateMachine,
							syncedLayerAffectsTiming = VRCFXLayer.layers[Index].syncedLayerAffectsTiming,
							syncedLayerIndex = VRCFXLayer.layers[Index].syncedLayerIndex
						};
						NewAnimationLayers[Index] = NewAnimationLayer;
					}
					Undo.RecordObject(VRCFXLayer, "Disabled Animator Controller AFK Layer");
					VRCFXLayer.layers = NewAnimationLayers;
					EditorUtility.SetDirty(VRCFXLayer);
					Undo.CollapseUndoOperations(UndoGroupIndex);
				}
			}
			return;
		}
	}
}
#endif