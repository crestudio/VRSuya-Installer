#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;

using UnityEditor;
using UnityEngine;

using VRC.SDK3.Avatars.Components;

/*
 * VRSuya AvatarRebuilder
 * Contact : vrsuya@gmail.com // Twitter : https://twitter.com/VRSuya
 * Forked from emilianavt/ReassignBoneWeigthsToNewMesh.cs ( https://gist.github.com/emilianavt/721cd4dd2d4a62ba54b002b63f894dbf )
 * Thanks to Dalgona. & C_Carrot & Naru & Rekorn
 */

namespace VRSuya.Installer {

	[ExecuteInEditMode]
	[AddComponentMenu("")]
	public class RecoveryAvatar : AvatarRebuilder {

		static GameObject[] NewAvatarGameObjects;
		static GameObject[] OldAvatarGameObjects;

		static Transform[] NewArmatureTransforms;
		static Transform[] OldArmatureTransforms;

		static VRCAvatarDescriptor OldVRCAvatarDescriptor;
		static SkinnedMeshRenderer NewAvatarHeadVisemeSkinnedMeshRenderer;
		static SkinnedMeshRenderer NewAvatarHeadEyelidsSkinnedMeshRenderer;

		static BoneNameType TargetBoneType;
		static GameObject[] NewCheekBoneGameObjects;
		static GameObject[] OldCheekBoneGameObjects;

		static GameObject[] NewFeetBoneGameObjects;

		static SkinnedMeshRenderer[] OldAvatarSkinnedMeshRenderers;

		static List<HumanBodyBones> HumanBodyBoneList = VRSuya.Core.Avatar.GetHumanBoneList();
		static readonly string[] ArmatureNames = { "Armature", "armature", "Sonia", "Ash" };
		static readonly string[] ToeBoneName = { "Left Toe", "Right Toe", "Toe.L", "Toe.R", "Toe_L", "Toe_R" }; 
		static readonly Dictionary<BoneNameType, string[]> dictCheekBoneNames = new Dictionary<BoneNameType, string[]>() {
			{ BoneNameType.General, new string[] { "Cheek1_L", "Cheek1_R", "Cheek2_L", "Cheek2_R" } },
			{ BoneNameType.Komado, new string[] { "Cheek_Root_L", "Cheek_Root_R", "Cheek_L", "Cheek_R" } },
			{ BoneNameType.Yoll, new string[] { "Cheek1_L", "Cheek1_R", "ho_L", "ho_R" } }
		};
		static readonly string[,] dictToeNames = {
			{ "ThumbToe1_L", "ThumbToe2_L", "ThumbToe3_L" },
			{ "ThumbToe1_R", "ThumbToe2_R", "ThumbToe3_R" },
			{ "IndexToe1_L", "IndexToe2_L", "IndexToe3_L" },
			{ "IndexToe1_R", "IndexToe2_R", "IndexToe3_R" },
			{ "MiddleToe1_L", "MiddleToe2_L", "MiddleToe3_L" },
			{ "MiddleToe1_R", "MiddleToe2_R", "MiddleToe3_R" },
			{ "RingToe1_L", "RingToe2_L", "RingToe3_L" },
			{ "RingToe1_R", "RingToe2_R", "RingToe3_R" },
			{ "LittleToe1_L", "LittleToe2_L", "LittleToe3_L" },
			{ "LittleToe1_R", "LittleToe2_R", "LittleToe3_R" }
		};

		internal static SkinnedMeshRenderer[] GetSkinnedMeshRenderers() {
			SkinnedMeshRenderer[] AllNewAvatarSkinnedMeshRenderers = NewAvatarGameObject.GetComponentsInChildren<SkinnedMeshRenderer>(true);
			SkinnedMeshRenderer[] AllOldAvatarSkinnedMeshRenderers = OldAvatarGameObject.GetComponentsInChildren<SkinnedMeshRenderer>(true);

			NewAvatarGameObjects = new GameObject[AllNewAvatarSkinnedMeshRenderers.Length];
			OldAvatarGameObjects = new GameObject[AllOldAvatarSkinnedMeshRenderers.Length];

			NewAvatarSkinnedMeshRenderers = new SkinnedMeshRenderer[AllNewAvatarSkinnedMeshRenderers.Length];
			OldAvatarSkinnedMeshRenderers = new SkinnedMeshRenderer[AllOldAvatarSkinnedMeshRenderers.Length];

			int Index = 0;
			foreach (SkinnedMeshRenderer NewSkinnedMeshRenderer in AllNewAvatarSkinnedMeshRenderers) {
				foreach (SkinnedMeshRenderer OldSkinnedMeshRenderer in AllOldAvatarSkinnedMeshRenderers) {
					if (NewSkinnedMeshRenderer.name == OldSkinnedMeshRenderer.name) {
						OldAvatarSkinnedMeshRenderers[Index] = OldSkinnedMeshRenderer;
						NewAvatarSkinnedMeshRenderers[Index] = NewSkinnedMeshRenderer;
						OldAvatarGameObjects[Index] = OldSkinnedMeshRenderer.gameObject;
						NewAvatarGameObjects[Index] = NewSkinnedMeshRenderer.gameObject;
						Index++;
						break;
					}
				}
			}
			Array.Resize(ref NewAvatarSkinnedMeshRenderers, Index);
			Array.Resize(ref OldAvatarSkinnedMeshRenderers, Index);
			Array.Resize(ref NewAvatarGameObjects, Index);
			Array.Resize(ref OldAvatarGameObjects, Index);
			return NewAvatarSkinnedMeshRenderers;
		}

		internal static void Recovery() {
			TargetBoneType = GetBoneNameType();
			GetHeadSkinnedMeshRenderers();
			ResizeNewAvatarTransform();
			GetArmatureTransforms();
            if (TargetAvatar != Avatar.SELESTIA && TargetAvatar != Avatar.MANUKA) GetCheekTransforms();
			GetFeetTransforms();
			if (TargetBoneType == BoneNameType.Komado || TargetBoneType == BoneNameType.Yoll) GetOldCheekBoneGameObjects();
			RenameGameObjects();
			UnpackPrefab();
			if (ToggleReorderGameObject) ReorderGameObjects();
			if (ToggleRestoreArmatureTransform) {
				RetransformNewAvatarArmatureTransforms();
				GetArmatureTransforms();
			}
			CopyBlendshapeSettings();
			CopyGameObjectSettings();
			ReplaceSkinnedMeshRendererBoneSettings();
			CopyGameObjectActive();
			MoveGameObjects();
			if (TargetAvatar != Avatar.SELESTIA && TargetAvatar != Avatar.MANUKA) MoveCheekBoneGameObjects();
			MoveFeetBoneGameObjects();
			DeleteGameObjects();
			UpdateVRCAvatarDescriptor();
		}

		static BoneNameType GetBoneNameType() {
			BoneNameType TargetBoneType = BoneNameType.General;
			switch (TargetAvatar) {
				case Avatar.Karin:
				case Avatar.Milk:
				case Avatar.Mint:
				case Avatar.Rusk:
					TargetBoneType = BoneNameType.Komado;
					break;
				case Avatar.Yoll:
					TargetBoneType = BoneNameType.Yoll;
					break;
				default:
                    TargetBoneType = BoneNameType.General;
                    break;

            }
			return TargetBoneType;
		}

		static void GetArmatureTransforms() {
			Transform[] NewAvatarTransforms = NewAvatarGameObject.GetComponentsInChildren<Transform>(true);
			Transform[] OldAvatarTransforms = OldAvatarGameObject.GetComponentsInChildren<Transform>(true);
			NewArmatureTransforms = Array.Find(NewAvatarTransforms, NewTransform => Array.Exists(ArmatureNames, ArmatureName => NewTransform.gameObject.name == ArmatureName) == true).GetComponentsInChildren<Transform>(true);
			OldArmatureTransforms = Array.Find(OldAvatarTransforms, OldTransform => Array.Exists(ArmatureNames, ArmatureName => OldTransform.gameObject.name == ArmatureName) == true).GetComponentsInChildren<Transform>(true);
		}

		static void GetCheekTransforms() {
			string[] CheekBoneNames = dictCheekBoneNames[TargetBoneType].Take(2).ToArray();
			NewCheekBoneGameObjects = Array.FindAll(NewArmatureTransforms, ArmatureTransform => Array.Exists(CheekBoneNames, BoneName => ArmatureTransform.gameObject.name == BoneName) == true).Select(Transform => Transform.gameObject).ToArray();
		}

		static void GetFeetTransforms() {
			string[] FeetRootBoneNames = Enumerable.Range(0, dictToeNames.GetLength(0)).Select(x => dictToeNames[x, 0]).ToArray();
			NewFeetBoneGameObjects = Array.FindAll(NewArmatureTransforms, ArmatureTransform => Array.Exists(FeetRootBoneNames, BoneName => ArmatureTransform.gameObject.name == BoneName) == true).Select(Transform => Transform.gameObject).ToArray();
		}

		static SkinnedMeshRenderer GetHeadSkinnedMeshRenderers() {
			if (OldAvatarGameObject.GetComponent<VRCAvatarDescriptor>()) {
				OldVRCAvatarDescriptor = OldAvatarGameObject.GetComponent<VRCAvatarDescriptor>();
				SkinnedMeshRenderer OldAvatarHeadVisemeSkinnedMeshRenderer = null;
				SkinnedMeshRenderer OldAvatarHeadEyelidsSkinnedMeshRenderer = null;
				if (OldVRCAvatarDescriptor.VisemeSkinnedMesh) OldAvatarHeadVisemeSkinnedMeshRenderer = OldVRCAvatarDescriptor.VisemeSkinnedMesh;
				if (OldVRCAvatarDescriptor.customEyeLookSettings.eyelidsSkinnedMesh) OldAvatarHeadEyelidsSkinnedMeshRenderer = OldVRCAvatarDescriptor.customEyeLookSettings.eyelidsSkinnedMesh;
				if (OldAvatarHeadVisemeSkinnedMeshRenderer) NewAvatarHeadVisemeSkinnedMeshRenderer = Array.Find(NewAvatarSkinnedMeshRenderers, NewSkinnedMeshRenderer => OldAvatarHeadVisemeSkinnedMeshRenderer.gameObject.name == NewSkinnedMeshRenderer.gameObject.name);
				if (OldAvatarHeadEyelidsSkinnedMeshRenderer) NewAvatarHeadEyelidsSkinnedMeshRenderer = Array.Find(NewAvatarSkinnedMeshRenderers, NewSkinnedMeshRenderer => OldAvatarHeadEyelidsSkinnedMeshRenderer.gameObject.name == NewSkinnedMeshRenderer.gameObject.name);
			}
			return NewAvatarHeadVisemeSkinnedMeshRenderer;
		}

		static void RenameGameObjects() {
			foreach (Transform TargetTransform in NewArmatureTransforms) {
				switch (TargetTransform.name) {
					case "Eyo_hair 1":
                        Undo.RecordObject(TargetTransform, "Rename GameObject");
                        TargetTransform.name = "Eyo_hair";
                        EditorUtility.SetDirty(TargetTransform);
                        Undo.CollapseUndoOperations(UndoGroupIndex);
                        break;
					case "Imeris_hair 1":
                        Undo.RecordObject(TargetTransform, "Rename GameObject");
                        TargetTransform.name = "Imeris_hair";
                        EditorUtility.SetDirty(TargetTransform);
                        Undo.CollapseUndoOperations(UndoGroupIndex);
                        break;
				}
			}
		}

		static void UnpackPrefab() {
			while (PrefabUtility.IsPartOfAnyPrefab(NewAvatarGameObject)) {
				if (PrefabUtility.GetPrefabAssetType(NewAvatarGameObject) == PrefabAssetType.NotAPrefab) {
					break;
				} else {
                    Undo.RecordObject(NewAvatarGameObject, "Unpack Prefab");
                    PrefabUtility.UnpackPrefabInstance(NewAvatarGameObject, PrefabUnpackMode.OutermostRoot, InteractionMode.AutomatedAction);
                    EditorUtility.SetDirty(NewAvatarGameObject);
                    Undo.CollapseUndoOperations(UndoGroupIndex);
                }
			}
			while (PrefabUtility.IsPartOfAnyPrefab(OldAvatarGameObject)) {
				if (PrefabUtility.GetPrefabAssetType(OldAvatarGameObject) == PrefabAssetType.NotAPrefab) {
					break;
				} else {
                    Undo.RecordObject(OldAvatarGameObject, "Unpack Prefab");
                    if (PrefabUtility.GetPrefabAssetType(OldAvatarGameObject) != PrefabAssetType.NotAPrefab) PrefabUtility.UnpackPrefabInstance(OldAvatarGameObject, PrefabUnpackMode.OutermostRoot, InteractionMode.AutomatedAction);
                    EditorUtility.SetDirty(OldAvatarGameObject);
                    Undo.CollapseUndoOperations(UndoGroupIndex);
                }
			}
		}

		static void ResizeNewAvatarTransform() {
            Undo.RecordObject(NewAvatarGameObject, "Transform New Avatar");
            NewAvatarGameObject.transform.SetPositionAndRotation(OldAvatarGameObject.transform.position, OldAvatarGameObject.transform.rotation);
			NewAvatarGameObject.transform.localPosition = OldAvatarGameObject.transform.localPosition;
			NewAvatarGameObject.transform.localRotation = OldAvatarGameObject.transform.localRotation;
			NewAvatarGameObject.transform.localScale = OldAvatarGameObject.transform.localScale;
            EditorUtility.SetDirty(NewAvatarGameObject);
            Undo.CollapseUndoOperations(UndoGroupIndex);
		}

		static GameObject[] GetOldCheekBoneGameObjects() {
			string[] CheekBoneNames = dictCheekBoneNames[TargetBoneType].Take(2).ToArray();
			OldCheekBoneGameObjects = Array.FindAll(OldArmatureTransforms, OldTransform => Array.Exists(CheekBoneNames, BoneName => OldTransform.gameObject.name == BoneName) == true).Select(Item => Item.gameObject).ToArray();
			return OldCheekBoneGameObjects;
		}

		static void RetransformNewAvatarArmatureTransforms() {
			for (int NewIndex = 0; NewIndex < NewArmatureTransforms.Length; NewIndex++) {
				for (int OldIndex = 0; OldIndex < OldArmatureTransforms.Length; OldIndex++) {
					if (NewArmatureTransforms[NewIndex] && OldArmatureTransforms[OldIndex] && NewArmatureTransforms[NewIndex].name == OldArmatureTransforms[OldIndex].name) {
						if (TargetBoneType == BoneNameType.Komado || TargetBoneType == BoneNameType.Yoll) {
							if (Array.Exists(dictCheekBoneNames[TargetBoneType], BoneName => NewArmatureTransforms[NewIndex].name == BoneName)) continue;
						}
                        Undo.RecordObject(NewArmatureTransforms[NewIndex], "Tranform Armature GameObject");
                        NewArmatureTransforms[NewIndex].localPosition = OldArmatureTransforms[OldIndex].localPosition;
						NewArmatureTransforms[NewIndex].localRotation = OldArmatureTransforms[OldIndex].localRotation;
						NewArmatureTransforms[NewIndex].localScale = OldArmatureTransforms[OldIndex].localScale;
                        EditorUtility.SetDirty(NewArmatureTransforms[NewIndex]);
                        Undo.CollapseUndoOperations(UndoGroupIndex);
                        break;
					}
				}
			}
			for (int ArmatureIndex = 0; ArmatureIndex < NewArmatureTransforms.Length; ArmatureIndex++) {
				foreach (HumanBodyBones HumanBone in HumanBodyBoneList) {
					if (HumanBone == HumanBodyBones.LastBone) continue;
					if (OldAvatarAnimator.GetBoneTransform(HumanBone) == null) continue;
					if (NewArmatureTransforms[ArmatureIndex].name == OldAvatarAnimator.GetBoneTransform(HumanBone).name) {
                        Undo.RecordObject(NewArmatureTransforms[ArmatureIndex], "Tranform Armature GameObject");
                        NewArmatureTransforms[ArmatureIndex].localPosition = OldAvatarAnimator.GetBoneTransform(HumanBone).localPosition;
						NewArmatureTransforms[ArmatureIndex].localRotation = OldAvatarAnimator.GetBoneTransform(HumanBone).localRotation;
						NewArmatureTransforms[ArmatureIndex].localScale = OldAvatarAnimator.GetBoneTransform(HumanBone).localScale;
                        EditorUtility.SetDirty(NewArmatureTransforms[ArmatureIndex]);
                        Undo.CollapseUndoOperations(UndoGroupIndex);
                    }
				}
			}
		}

		static void CopyGameObjectSettings() {
			for (int Index = 0; Index < NewAvatarSkinnedMeshRenderers.Length; Index++) {
                Undo.RecordObject(NewAvatarSkinnedMeshRenderers[Index], "Copy SkinnedMeshRenderer Settings");

                NewAvatarSkinnedMeshRenderers[Index].gameObject.isStatic = OldAvatarSkinnedMeshRenderers[Index].gameObject.isStatic;
				NewAvatarSkinnedMeshRenderers[Index].gameObject.tag = OldAvatarSkinnedMeshRenderers[Index].gameObject.tag;
				NewAvatarSkinnedMeshRenderers[Index].gameObject.layer = OldAvatarSkinnedMeshRenderers[Index].gameObject.layer;

				NewAvatarSkinnedMeshRenderers[Index].gameObject.transform.localPosition = OldAvatarSkinnedMeshRenderers[Index].gameObject.transform.localPosition;
				NewAvatarSkinnedMeshRenderers[Index].gameObject.transform.localRotation = OldAvatarSkinnedMeshRenderers[Index].gameObject.transform.localRotation;
				NewAvatarSkinnedMeshRenderers[Index].gameObject.transform.localScale = OldAvatarSkinnedMeshRenderers[Index].gameObject.transform.localScale;

				NewAvatarSkinnedMeshRenderers[Index].localBounds = OldAvatarSkinnedMeshRenderers[Index].localBounds;
				NewAvatarSkinnedMeshRenderers[Index].quality = OldAvatarSkinnedMeshRenderers[Index].quality;
				NewAvatarSkinnedMeshRenderers[Index].updateWhenOffscreen = OldAvatarSkinnedMeshRenderers[Index].updateWhenOffscreen;

				Material[] NewSharedMaterials = new Material[OldAvatarSkinnedMeshRenderers[Index].sharedMaterials.Length];
				for (int MaterialIndex = 0; MaterialIndex < NewSharedMaterials.Length; MaterialIndex++) {
					NewSharedMaterials[MaterialIndex] = OldAvatarSkinnedMeshRenderers[Index].sharedMaterials[MaterialIndex];
				}
				NewAvatarSkinnedMeshRenderers[Index].sharedMaterials = NewSharedMaterials;
				for (int MaterialIndex = 0; MaterialIndex < OldAvatarSkinnedMeshRenderers[Index].sharedMaterials.Length; MaterialIndex++) {
					NewAvatarSkinnedMeshRenderers[Index].sharedMaterials[MaterialIndex] = OldAvatarSkinnedMeshRenderers[Index].sharedMaterials[MaterialIndex];
				}

				NewAvatarSkinnedMeshRenderers[Index].shadowCastingMode = OldAvatarSkinnedMeshRenderers[Index].shadowCastingMode;
				NewAvatarSkinnedMeshRenderers[Index].receiveShadows = OldAvatarSkinnedMeshRenderers[Index].receiveShadows;
				NewAvatarSkinnedMeshRenderers[Index].lightProbeUsage = OldAvatarSkinnedMeshRenderers[Index].lightProbeUsage;
				NewAvatarSkinnedMeshRenderers[Index].reflectionProbeUsage = OldAvatarSkinnedMeshRenderers[Index].reflectionProbeUsage;
				NewAvatarSkinnedMeshRenderers[Index].probeAnchor = OldAvatarSkinnedMeshRenderers[Index].probeAnchor;
				NewAvatarSkinnedMeshRenderers[Index].skinnedMotionVectors = OldAvatarSkinnedMeshRenderers[Index].skinnedMotionVectors;
				NewAvatarSkinnedMeshRenderers[Index].allowOcclusionWhenDynamic = OldAvatarSkinnedMeshRenderers[Index].allowOcclusionWhenDynamic;
                EditorUtility.SetDirty(NewAvatarSkinnedMeshRenderers[Index]);
                Undo.CollapseUndoOperations(UndoGroupIndex);
            }
		}

		static void CopyBlendshapeSettings() {
			for (int Index = 0; Index < NewAvatarSkinnedMeshRenderers.Length; Index++) {
                Undo.RecordObject(NewAvatarSkinnedMeshRenderers[Index], "Copy SkinnedMeshRenderer BlendShape Settings");
                Mesh NewAvatarMesh = NewAvatarSkinnedMeshRenderers[Index].sharedMesh;
				Mesh OldAvatarMesh = OldAvatarSkinnedMeshRenderers[Index].sharedMesh;
				string[] OldAvatarBlendshapeList = new string[OldAvatarMesh.blendShapeCount];
				string[] NewAvatarBlendshapeList = new string[NewAvatarMesh.blendShapeCount];
				if (OldAvatarMesh.blendShapeCount > 0) {
					for (int Offset = 0; Offset < OldAvatarMesh.blendShapeCount; Offset++) {
						OldAvatarBlendshapeList[Offset] = OldAvatarMesh.GetBlendShapeName(Offset);
					}
				}
				if (NewAvatarMesh.blendShapeCount > 0) {
					for (int Offset = 0; Offset < NewAvatarMesh.blendShapeCount; Offset++) {
						NewAvatarBlendshapeList[Offset] = NewAvatarMesh.GetBlendShapeName(Offset);
					}
				}
				for (int NewIndex = 0; NewIndex < NewAvatarBlendshapeList.Length; NewIndex++) {
					for (int OldIndex = 0; OldIndex < OldAvatarBlendshapeList.Length; OldIndex++) {
						if (NewAvatarBlendshapeList[NewIndex] == OldAvatarBlendshapeList[OldIndex]) {
							NewAvatarSkinnedMeshRenderers[Index].SetBlendShapeWeight(NewIndex, OldAvatarSkinnedMeshRenderers[Index].GetBlendShapeWeight(OldIndex));
							break;
						}
					}
				}
                EditorUtility.SetDirty(NewAvatarSkinnedMeshRenderers[Index]);
                Undo.CollapseUndoOperations(UndoGroupIndex);
            }
		}

		static void ReplaceSkinnedMeshRendererBoneSettings() {
			foreach (SkinnedMeshRenderer NewSkinnedMeshRenderer in NewAvatarSkinnedMeshRenderers) {
                Undo.RecordObject(NewSkinnedMeshRenderer, "Replace SkinnedMeshRenderer Bones Settings");
                Transform[] ChildBones = NewSkinnedMeshRenderer.bones;
				NewSkinnedMeshRenderer.rootBone = AvatarRootBone;
				for (int BoneIndex = 0; BoneIndex < ChildBones.Length; BoneIndex++) {
					for (int TransformIndex = 0; TransformIndex < OldArmatureTransforms.Length; TransformIndex++) {
						if (ChildBones[BoneIndex] && OldArmatureTransforms[TransformIndex] && ChildBones[BoneIndex].name == OldArmatureTransforms[TransformIndex].name) {
							if (TargetBoneType == BoneNameType.Komado || TargetBoneType == BoneNameType.Yoll) {
								if (Array.Exists(dictCheekBoneNames[TargetBoneType], BoneName => ChildBones[BoneIndex].name == BoneName)) continue;
							}
							if (ToggleResetRestPose == true) {
								OldArmatureTransforms[TransformIndex].transform.localPosition = ChildBones[BoneIndex].transform.localPosition;
								OldArmatureTransforms[TransformIndex].transform.localRotation = ChildBones[BoneIndex].transform.localRotation;
								OldArmatureTransforms[TransformIndex].transform.localScale = ChildBones[BoneIndex].transform.localScale;
							}
							ChildBones[BoneIndex] = OldArmatureTransforms[TransformIndex];
							break;
						}
					}
				}
				for (int BoneIndex = 0; BoneIndex < ChildBones.Length; BoneIndex++) {
					foreach (HumanBodyBones HumanBone in HumanBodyBoneList) {
						if (HumanBone == HumanBodyBones.LastBone) continue;
						if (OldAvatarAnimator.GetBoneTransform(HumanBone) == null) continue;
						if (ChildBones[BoneIndex].name == OldAvatarAnimator.GetBoneTransform(HumanBone).name) {
							ChildBones[BoneIndex] = OldAvatarAnimator.GetBoneTransform(HumanBone);
						}
					}
				}
				NewSkinnedMeshRenderer.bones = ChildBones;
                EditorUtility.SetDirty(NewSkinnedMeshRenderer);
                Undo.CollapseUndoOperations(UndoGroupIndex);
            }
		}

		static void CopyGameObjectActive() {
			for (int Index = 0; Index < NewAvatarGameObjects.Length; Index++) {
                Undo.RecordObject(NewAvatarGameObjects[Index], "Copy GameObject Active Status");
                NewAvatarGameObjects[Index].SetActive(OldAvatarGameObjects[Index].activeSelf);
                EditorUtility.SetDirty(NewAvatarGameObjects[Index]);
                Undo.CollapseUndoOperations(UndoGroupIndex);
            }
		}

		static void ReorderGameObjects() {
			string[] NewArmatureTransformNames = NewArmatureTransforms.Select(NewTransform => NewTransform.name).ToArray();
			for (int Index = OldArmatureTransforms.Length - 1; Index >= 0; Index--) {
				if (Array.Exists(NewArmatureTransformNames, Name => Name == OldArmatureTransforms[Index].name) == true) {
                    Undo.RecordObject(OldArmatureTransforms[Index], "Set GameObject order first");
                    OldArmatureTransforms[Index].transform.SetAsFirstSibling();
                    EditorUtility.SetDirty(OldArmatureTransforms[Index]);
                    Undo.CollapseUndoOperations(UndoGroupIndex);
                }
			}
			for (int Index = HumanBodyBoneList.Count - 1; Index >= 0; Index--) {
				if (HumanBodyBoneList[Index] == HumanBodyBones.LastBone) continue;
				if (OldAvatarAnimator.GetBoneTransform(HumanBodyBoneList[Index]) == null) continue;
                Undo.RecordObject(OldAvatarAnimator.GetBoneTransform(HumanBodyBoneList[Index]), "Set GameObject order first");
                OldAvatarAnimator.GetBoneTransform(HumanBodyBoneList[Index]).SetAsFirstSibling();
                EditorUtility.SetDirty(OldAvatarAnimator.GetBoneTransform(HumanBodyBoneList[Index]));
                Undo.CollapseUndoOperations(UndoGroupIndex);
            }
		}

		static void MoveGameObjects() {
			for (int Index = 0; Index < NewAvatarGameObjects.Length; Index++) {
                Undo.RecordObject(NewAvatarGameObjects[Index], "Move New GameObject");
                NewAvatarGameObjects[Index].transform.SetParent(OldAvatarGameObjects[Index].transform.parent, false);
				NewAvatarGameObjects[Index].transform.SetSiblingIndex(OldAvatarGameObjects[Index].transform.GetSiblingIndex() + 1);
                EditorUtility.SetDirty(NewAvatarGameObjects[Index]);
                Undo.CollapseUndoOperations(UndoGroupIndex);
            }
		}

		static void MoveCheekBoneGameObjects() {
			if (NewCheekBoneGameObjects.Length > 0) {
                string[] HeadChildTransformNames = GetChildTransforms(OldAvatarAnimator.GetBoneTransform(HumanBodyBones.Head)).Select(TransformItem => TransformItem.name).ToArray();
                foreach (GameObject CheekBoneGameObject in NewCheekBoneGameObjects) {
					if (!Array.Exists(HeadChildTransformNames, TransformName => CheekBoneGameObject.name == TransformName)) {
                        Undo.RecordObject(CheekBoneGameObject, "Move Cheek GameObject");
                        CheekBoneGameObject.transform.SetParent(OldAvatarAnimator.GetBoneTransform(HumanBodyBones.Head), false);
                        EditorUtility.SetDirty(CheekBoneGameObject);
                        Undo.CollapseUndoOperations(UndoGroupIndex);
                    } else if (TargetBoneType == BoneNameType.Komado || TargetBoneType == BoneNameType.Yoll) {
						Undo.RecordObject(CheekBoneGameObject, "Move Cheek GameObject");
						CheekBoneGameObject.transform.SetParent(OldAvatarAnimator.GetBoneTransform(HumanBodyBones.Head), false);
						EditorUtility.SetDirty(CheekBoneGameObject);
						Undo.CollapseUndoOperations(UndoGroupIndex);
					}
				}
			}
		}

		static void MoveFeetBoneGameObjects() {
			if (NewFeetBoneGameObjects.Length > 0) {
				Transform LeftFoot = OldAvatarAnimator.GetBoneTransform(HumanBodyBones.LeftFoot);
				Transform RightFoot = OldAvatarAnimator.GetBoneTransform(HumanBodyBones.RightFoot);
				Transform LeftToe = OldAvatarAnimator.GetBoneTransform(HumanBodyBones.LeftToes);
				Transform RightToe = OldAvatarAnimator.GetBoneTransform(HumanBodyBones.RightToes);
				Transform TargetLeft = null;
				Transform TargetRight = null;
				if (!LeftToe) {
					switch (LeftFoot.childCount) {
						case 1:
							TargetLeft = LeftFoot.GetChild(0);
							break;
						case 0:
							TargetLeft = LeftFoot;
							break;
						default:
							Transform[] SearchTransform = GetChildTransforms(LeftFoot);
                            Transform TargetGameObject = Array.Find(SearchTransform, TargetObject => Array.Exists(ToeBoneName, BoneName => TargetObject.name == BoneName));
							if (TargetGameObject) {
								TargetLeft = TargetGameObject;
							} else {
								TargetLeft = LeftFoot.GetChild(0);
							}
							break;
					}
				} else {
					TargetLeft = LeftToe;
				}
				if (!RightToe) {
					switch (RightFoot.childCount) {
						case 1:
							TargetRight = RightFoot.GetChild(0);
							break;
						case 0:
							TargetRight = RightFoot;
							break;
						default:
							Transform[] SearchTransform = GetChildTransforms(RightFoot);
							Transform TargetGameObject = Array.Find(SearchTransform, TargetObject => Array.Exists(ToeBoneName, BoneName => TargetObject.name == BoneName));
							if (TargetGameObject) {
								TargetRight = TargetGameObject;
							} else {
								TargetRight = RightFoot.GetChild(0);
							}
							break;
					}
				} else {
					TargetRight = RightToe;
				}
				string[] TargetLeftChildTransformNames = GetChildTransforms(TargetLeft).Select(TransformItem => TransformItem.name).ToArray();
                string[] TargetRightChildTransformNames = GetChildTransforms(TargetRight).Select(TransformItem => TransformItem.name).ToArray();
                foreach (GameObject ToeBoneGameObject in NewFeetBoneGameObjects) {
					switch (ToeBoneGameObject.name.Substring(ToeBoneGameObject.name.Length - 1, 1)) {
						case "L":
							if (!Array.Exists(TargetLeftChildTransformNames, TransformName => ToeBoneGameObject.name == TransformName)) {
                                Undo.RecordObject(ToeBoneGameObject, "Move Toe GameObject");
                                ToeBoneGameObject.transform.SetParent(TargetLeft, false);
                                EditorUtility.SetDirty(ToeBoneGameObject);
                                Undo.CollapseUndoOperations(UndoGroupIndex);
                            }
							break;
						case "R":
							if (!Array.Exists(TargetRightChildTransformNames, TransformName => ToeBoneGameObject.name == TransformName)) {
                                Undo.RecordObject(ToeBoneGameObject, "Move Toe GameObject");
                                ToeBoneGameObject.transform.SetParent(TargetRight, false);
                                EditorUtility.SetDirty(ToeBoneGameObject);
                                Undo.CollapseUndoOperations(UndoGroupIndex);
                            }
							break;
					}
				}
			}
		}

		static void DeleteGameObjects() {
			if (TargetBoneType == BoneNameType.Komado || TargetBoneType == BoneNameType.Yoll) {
				if (OldCheekBoneGameObjects.Length > 0) {
					for (int Index = 0; Index < OldCheekBoneGameObjects.Length; Index++) {
                        Undo.RecordObject(OldCheekBoneGameObjects[Index], "Delete Exist Bone GameObject");
                        DestroyImmediate(OldCheekBoneGameObjects[Index]);
                        Undo.CollapseUndoOperations(UndoGroupIndex);
                    }
				}
			}
			if (OldAvatarGameObjects.Length > 0) {
				for (int Index = 0; Index < OldAvatarGameObjects.Length; Index++) {
                    Undo.RecordObject(OldAvatarGameObjects[Index], "Delete Exist SkinnedMeshRenderer GameObject");
                    DestroyImmediate(OldAvatarGameObjects[Index]);
                    Undo.CollapseUndoOperations(UndoGroupIndex);
                }
			}
            Undo.RecordObject(NewAvatarGameObject, "Delete New Avatar GameObject");
            DestroyImmediate(NewAvatarGameObject);
            Undo.CollapseUndoOperations(UndoGroupIndex);
		}

		static void UpdateVRCAvatarDescriptor() {
			if (OldVRCAvatarDescriptor) {
                Undo.RecordObject(OldVRCAvatarDescriptor, "Update VRC Avatar Descriptor");
                if (NewAvatarHeadVisemeSkinnedMeshRenderer) {
					OldVRCAvatarDescriptor.VisemeSkinnedMesh = NewAvatarHeadVisemeSkinnedMeshRenderer;
				}
				if (NewAvatarHeadEyelidsSkinnedMeshRenderer) {
					OldVRCAvatarDescriptor.customEyeLookSettings.eyelidsSkinnedMesh = NewAvatarHeadEyelidsSkinnedMeshRenderer;
				}
                EditorUtility.SetDirty(OldVRCAvatarDescriptor);
                Undo.CollapseUndoOperations(UndoGroupIndex);
            }
		}

        static Transform[] GetChildTransforms(Transform TargetTransform) {
			Transform[] ReturnTransforms = new Transform[TargetTransform.childCount];
            for (int Index = 0; Index < TargetTransform.childCount; Index++) {
                ReturnTransforms[Index] = TargetTransform.GetChild(Index);
            }
			return ReturnTransforms;

        }
	}
}
#endif