#if UNITY_EDITOR
using System;
using System.Linq;

using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;

using VRC.SDK3.Avatars.Components;
using VRC.SDK3.Dynamics.PhysBone.Components;

/*
 * VRSuya Avatar Setting Updater
 * Contact : vrsuya@gmail.com // Twitter : https://twitter.com/VRSuya
 */

namespace com.vrsuya.installer {

	[ExecuteInEditMode]
	[AddComponentMenu("")]
	public class ProductSetup_Mogumogu : ProductSetup {

		private static VRSuyaProduct Mogumogu;
		private static GameObject VRSuyaMogumoguGameObject;
		private static Transform[] AvatarCheekBoneTransforms;

		private static readonly Avatar[] dictAlreadyHaveCheekBoneAvatar = new Avatar[] { Avatar.Airi, Avatar.Milfy, Avatar.Sio };
		private static readonly string[] dictSELESTIAMogumoguLayerName = new string[] { "Cheek_L_Stretch", "Cheek_R_Stretch" };
		private static readonly string[] dictChocolatHairPhysBoneName = new string[] { "Hair_side_L", "Hair_side_R" };

		/// <summary>제품 정보를 AssetManager에게 요청하여 업데이트 한 후, 설치된 에셋 목록에 추가합니다.</summary>
		internal static void RegisterProduct() {
			InstalledProductMogumogu = false;
			Mogumogu = new VRSuyaProduct();
			Mogumogu = AssetManager.UpdateProductInformation(ProductName.Mogumogu);
			InstalledVRSuyaProducts = InstalledVRSuyaProducts.Concat(new VRSuyaProduct[] { Mogumogu }).ToArray();
			if (Mogumogu.SupportAvatarList.Length > 0) InstalledProductMogumogu = true;
			return;
		}

		/// <summary>외부의 세팅 요청을 처리하는 메인 메소드 입니다.</summary>
		internal static void RequestSetting() {
			if (InstallProductMogumogu) {
				string SearchWord = (AvatarType != Avatar.Sio) ? "Cheek" : "Hoppe";
				VRSuyaMogumoguGameObject = Array.Find(VRSuyaGameObjects, gameObject => gameObject.name.Contains("VRSuya_Mogumogu_PhysBone"));
				AvatarCheekBoneTransforms = Array.FindAll(AvatarAnimator.GetBoneTransform(HumanBodyBones.Head).GetComponentsInChildren<Transform>(true), transform => transform.name.Contains(SearchWord));
				if (!Array.Exists(dictAlreadyHaveCheekBoneAvatar, AlreadyHaveCheekAvatar => AvatarType == AlreadyHaveCheekAvatar)) {
					if (!VRSuyaMogumoguGameObject) {
						SetupPhysbonePrefab();
						SetupParticlePrefab();
					}
					if (VRSuyaMogumoguGameObject) {
						UpdatePhysBoneSetting();
						DisableExistPhysBone();
						if (AvatarType == Avatar.SELESTIA) DisableExistMoumoguAnimatorLayer();
						if (AvatarType == Avatar.Chocolat) EnableHairPhysBoneAnimated();
					}
				} else {
					SetupParticlePrefab();
					EnablePhysBoneAnimated();
				}
			}
			return;
		}

		/// <summary>아바타에 Physbone Prefab이 있는지 검사하고 없으면 설치하는 메소드 입니다.</summary>
		private static void SetupPhysbonePrefab() {
			string[] ChildAvatarGameObjectNames = new string[0];
			foreach (Transform ChildTransform in AvatarGameObject.transform) {
				ChildAvatarGameObjectNames = ChildAvatarGameObjectNames.Concat(new string[] { ChildTransform.name }).ToArray();
			}
			if (!Array.Exists(ChildAvatarGameObjectNames, GameObjectName => GameObjectName.Contains("VRSuya_Mogumogu_PhysBone"))) {
				string[] PrefabFilePaths = new string[0];
				PrefabFilePaths = Mogumogu.PrefabGUID.Select(AssetGUID => AssetDatabase.GUIDToAssetPath(AssetGUID)).ToArray();
				string TargetPrefabPath = Array.Find(PrefabFilePaths, FilePath => FilePath.Split('/')[FilePath.Split('/').Length - 1].Contains("VRSuya_Mogumogu_PhysBone_" + AvatarType.ToString()));
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
			VRSuyaMogumoguGameObject = Array.Find(VRSuyaGameObjects, gameObject => gameObject.name.Contains("VRSuya_Mogumogu_PhysBone"));
			return;
		}

		/// <summary>아바타에 Particle Prefab이 있는지 검사하고 없으면 설치하는 메소드 입니다.</summary>
		private static void SetupParticlePrefab() {
			string[] HeadChildAvatarGameObjectNames = new string[0];
			foreach (Transform ChildTransform in AvatarAnimator.GetBoneTransform(HumanBodyBones.Head)) {
				HeadChildAvatarGameObjectNames = HeadChildAvatarGameObjectNames.Concat(new string[] { ChildTransform.name }).ToArray();
			}
			if (!Array.Exists(HeadChildAvatarGameObjectNames, GameObjectName => GameObjectName.Contains("VRSuya_Mogumogu_Particle"))) {
				string[] PrefabFilePaths = new string[0];
				PrefabFilePaths = Mogumogu.PrefabGUID.Select(AssetGUID => AssetDatabase.GUIDToAssetPath(AssetGUID)).ToArray();
				string TargetPrefabPath = Array.Find(PrefabFilePaths, FilePath => FilePath.Split('/')[FilePath.Split('/').Length - 1].Contains("VRSuya_Mogumogu_Particle_" + AvatarType.ToString()));
				if (TargetPrefabPath != null) {
					GameObject TargetPrefab = (GameObject)AssetDatabase.LoadAssetAtPath(TargetPrefabPath, typeof(GameObject));
					GameObject TargetInstance = (GameObject)PrefabUtility.InstantiatePrefab(TargetPrefab);
					Undo.RegisterCreatedObjectUndo(TargetInstance, "Added New GameObject");
					TargetInstance.transform.parent = AvatarAnimator.GetBoneTransform(HumanBodyBones.Head);
					TransformPrefab(TargetInstance, AvatarGameObject, true);
					Undo.CollapseUndoOperations(UndoGroupIndex);
				}
			}
			GetVRSuyaGameObjects();
			return;
		}

		/// <summary>볼 PhysBone 컴포넌트와 아바타의 볼 본과 연결합니다.</summary>
		private static void UpdatePhysBoneSetting() {
			VRCPhysBone[] VRSuyaMogumoguPhysBones = VRSuyaMogumoguGameObject.GetComponentsInChildren<VRCPhysBone>(true);
			if (VRSuyaMogumoguPhysBones != null) {
				Transform Cheek_L = null;
				Transform Cheek_R = null;
				VRCPhysBone PhysBone_Cheek_L = Array.Find(VRSuyaMogumoguPhysBones, PhysBone => PhysBone.gameObject.name == "Cheek_L");
				VRCPhysBone PhysBone_Cheek_R = Array.Find(VRSuyaMogumoguPhysBones, PhysBone => PhysBone.gameObject.name == "Cheek_R");
				if (AvatarCheekBoneTransforms != null) {
					foreach (Transform TargetTransform in AvatarCheekBoneTransforms) {
						switch (TargetTransform.gameObject.name) {
							case "Cheek.L":
							case "Cheek1_L":
							case "Cheek_Root_L":
							case "Cheek_root_L":
								Cheek_L = TargetTransform;
								break;
							case "Cheek.R":
							case "Cheek1_R":
							case "Cheek_Root_R":
							case "Cheek_root_R":
								Cheek_R = TargetTransform;
								break;
						}
					}
				}
				if (Cheek_L && PhysBone_Cheek_L) {
					Undo.RecordObject(PhysBone_Cheek_L, "Changed PhysBone Root Transform");
					PhysBone_Cheek_L.rootTransform = Cheek_L;
					EditorUtility.SetDirty(PhysBone_Cheek_L);
					Undo.CollapseUndoOperations(UndoGroupIndex);
				}
				if (Cheek_R && PhysBone_Cheek_R) {
					Undo.RecordObject(PhysBone_Cheek_R, "Changed PhysBone Root Transform");
					PhysBone_Cheek_R.rootTransform = Cheek_R;
					EditorUtility.SetDirty(PhysBone_Cheek_R);
					Undo.CollapseUndoOperations(UndoGroupIndex);
				}
			}
			return;
		}

		/// <summary>기존 아바타에 존재하는 PhysBone 컴포넌트를 비활성화 합니다.</summary>
		private static void DisableExistPhysBone() {
			if (AvatarCheekBoneTransforms.Length > 0) {
				foreach (Transform TargetTransform in AvatarCheekBoneTransforms) {
					if (TargetTransform.GetComponent<VRCPhysBone>()) {
						Undo.RecordObject(TargetTransform.GetComponent<VRCPhysBone>(), "Disabled PhysBone");
						TargetTransform.GetComponent<VRCPhysBone>().enabled = false;
						EditorUtility.SetDirty(TargetTransform.GetComponent<VRCPhysBone>());
						Undo.CollapseUndoOperations(UndoGroupIndex);
					}
				}
			}
			return;
		}

		/// <summary>기존 아바타에 존재하는 PhysBone 컴포넌트에서 Animated 속성을 활성화 합니다.</summary>
		private static void EnablePhysBoneAnimated() {
			if (AvatarCheekBoneTransforms.Length > 0) {
				foreach (Transform TargetTransform in AvatarCheekBoneTransforms) {
					if (TargetTransform.GetComponent<VRCPhysBone>()) {
						if (!TargetTransform.GetComponent<VRCPhysBone>().isAnimated) {
							Undo.RecordObject(TargetTransform.GetComponent<VRCPhysBone>(), "Enabled Animated PhysBone");
							TargetTransform.GetComponent<VRCPhysBone>().isAnimated = true;
							EditorUtility.SetDirty(TargetTransform.GetComponent<VRCPhysBone>());
							Undo.CollapseUndoOperations(UndoGroupIndex);
						}
					}
				}
			}
			return;
		}

		/// <summary>기존 아바타에 존재하는 헤어 PhysBone 컴포넌트에서 Animated 속성을 활성화 합니다.</summary>
		private static void EnableHairPhysBoneAnimated() {
			VRCPhysBone[] AvatarPhysBoneComponents = AvatarGameObject.GetComponentsInChildren<VRCPhysBone>();
			VRCPhysBone[] SideHairPhysBoneComponents = Array
				.FindAll(AvatarPhysBoneComponents, PhysBone => Array
				.Exists(dictChocolatHairPhysBoneName, HairPhysBoneName => PhysBone.name == HairPhysBoneName));
			if (SideHairPhysBoneComponents.Length > 0) {
				foreach (VRCPhysBone TargetPhysBone in SideHairPhysBoneComponents) {
					if (!TargetPhysBone.isAnimated) {
						Undo.RecordObject(TargetPhysBone, "Enabled Animated PhysBone");
						TargetPhysBone.isAnimated = true;
						EditorUtility.SetDirty(TargetPhysBone);
						Undo.CollapseUndoOperations(UndoGroupIndex);
					}
				}
			}
			return;
		}

		/// <summary>셀레스티아의 피직스본 애니메이터 레이어를 비활성화 합니다.</summary>
		private static void DisableExistMoumoguAnimatorLayer() {
			AnimatorController VRCFXLayer = (AnimatorController)Array.Find(AvatarVRCAvatarLayers, VRCAnimator => VRCAnimator.type == VRCAvatarDescriptor.AnimLayerType.FX).animatorController;
			if (VRCFXLayer) {
				if (Array.Exists(VRCFXLayer.layers, ExistLayer => Array.Exists(dictSELESTIAMogumoguLayerName, TargetName => ExistLayer.name == TargetName))) {
					if (VRCFXLayer.layers.Where(ExistLayer => ExistLayer.defaultWeight != 0.0f).ToArray().Length > 0) {
						AnimatorControllerLayer[] newAnimationLayers = new AnimatorControllerLayer[VRCFXLayer.layers.Length];
						for (int Index = 0; Index < newAnimationLayers.Length; Index++) {
							AnimatorControllerLayer newAnimationLayer = new AnimatorControllerLayer();
							newAnimationLayer.avatarMask = VRCFXLayer.layers[Index].avatarMask;
							newAnimationLayer.blendingMode = VRCFXLayer.layers[Index].blendingMode;
							newAnimationLayer.iKPass = VRCFXLayer.layers[Index].iKPass;
							newAnimationLayer.name = VRCFXLayer.layers[Index].name;
							newAnimationLayer.stateMachine = VRCFXLayer.layers[Index].stateMachine;
							newAnimationLayer.syncedLayerAffectsTiming = VRCFXLayer.layers[Index].syncedLayerAffectsTiming;
							newAnimationLayer.syncedLayerIndex = VRCFXLayer.layers[Index].syncedLayerIndex;
							if (Array.Exists(dictSELESTIAMogumoguLayerName, TargetName => VRCFXLayer.layers[Index].name == TargetName)) {
								newAnimationLayer.defaultWeight = 0.0f;
							} else {
								newAnimationLayer.defaultWeight = VRCFXLayer.layers[Index].defaultWeight;
							}
							newAnimationLayers[Index] = newAnimationLayer;
						}
						Undo.RecordObject(VRCFXLayer, "Disabled Animator Controller Cheek Bone Layer");
						VRCFXLayer.layers = newAnimationLayers;
						EditorUtility.SetDirty(VRCFXLayer);
						Undo.CollapseUndoOperations(UndoGroupIndex);
					}
				}
			}
			return;
		}
	}
}
#endif