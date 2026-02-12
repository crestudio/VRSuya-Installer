#if UNITY_EDITOR
using System.Linq;
using System.Reflection;

using UnityEditor;
using UnityEngine;

using VRSuya.Core;

/*
 * VRSuya AvatarRebuilder
 * Contact : vrsuya@gmail.com // Twitter : https://twitter.com/VRSuya
 * Forked from emilianavt/ReassignBoneWeigthsToNewMesh.cs ( https://gist.github.com/emilianavt/721cd4dd2d4a62ba54b002b63f894dbf )
 * Thanks to Dalgona. & C_Carrot & Naru & Rekorn
 */

namespace com.vrsuya.avatarrebuilder {

	[ExecuteInEditMode]
	[AddComponentMenu("")]
	public class AvatarHandler : AvatarRebuilder {

		/// <summary>
		/// 본 프로그램의 메인 세팅 로직입니다.
		/// </summary>
		internal static void RequestCheckNewAvatar() {
			if (!IsSameFBX() && !NewAvatarPatched) {
				ApplyNewAvatarFBXModel();
			} else if (!IsSameFBX() && NewAvatarPatched) {
				ReplaceOldAvatarAnimatorAvatar();
			}
		}

        /// <summary>새 아바타 GameObject가 Scene에 존재하는지 여부를 알려줍니다.</summary>
        /// <returns>새 아바타 GameObject가 Scene에 존재하는지 여부</returns>
        internal static void CheckExistNewAvatarInScene() {
            if (!NewAvatarGameObject.scene.IsValid()) {
                ApplyNewAvatarFBXModel();
                PlaceGameObejctInScene();
            }
        }

        /// <summary>기존 아바타를 복제하여 백업본을 생성합니다.</summary>
        internal static void CreateDuplicateAvatar() {
			DuplicateGameObject DuplicatorInstance = new DuplicateGameObject();
			string TargetName = OldAvatarGameObject.name;
			GameObject DuplicatedAvatar = DuplicatorInstance.DuplicateGameObjectInstance(OldAvatarGameObject);
			Undo.RegisterCreatedObjectUndo(DuplicatedAvatar, "Duplicated Old Avatar");
			DuplicatedAvatar.name = TargetName + " (Backup)";
			DuplicatedAvatar.transform.SetSiblingIndex(OldAvatarGameObject.transform.GetSiblingIndex() + 1);
			DestroyImmediate(DuplicatedAvatar.GetComponent<AvatarRebuilder>());
			DuplicatedAvatar.SetActive(false);
            EditorUtility.SetDirty(DuplicatedAvatar);
			Selection.activeGameObject = OldAvatarGameObject;
			Undo.CollapseUndoOperations(UndoGroupIndex);
		}

        /// <summary>기존 아바타와 신규 아바타가 같은 FBX 파일을 사용하는지 알려줍니다.</summary>
        /// <returns>같은 FBX 파일을 사용 여부</returns>
        static bool IsSameFBX() {
			UnityEngine.Avatar OldAnimatorAvatar = OldAvatarAnimator.avatar;
			UnityEngine.Avatar NewAnimatorAvatar = NewAvatarAnimator.avatar;
			string OldAvatarAssetPath = AssetDatabase.GetAssetPath(OldAnimatorAvatar);
			string NewAvatarAssetPath = AssetDatabase.GetAssetPath(NewAnimatorAvatar);
			if (OldAvatarAssetPath == NewAvatarAssetPath) return (true);
			return false;
		}

		/// <summary>기존 아바타의 FBX 메타 데이터를 복제하여, 새 아바타의 FBX 메타 데이터에 적용 합니다.</summary>
		static void ApplyNewAvatarFBXModel() {
			UnityEngine.Avatar OldAnimatorAvatar = OldAvatarAnimator.avatar;
			string OldAvatarAssetPath = AssetDatabase.GetAssetPath(OldAnimatorAvatar);
			string NewAvatarAssetPath = AssetDatabase.GetAssetPath(NewAvatarGameObject);
			if (string.IsNullOrEmpty(NewAvatarAssetPath)) NewAvatarAssetPath = AssetDatabase.GetAssetPath(NewAvatarAnimator.avatar);
			ModelImporter OldAvatarModelImporter = AssetImporter.GetAtPath(OldAvatarAssetPath) as ModelImporter;
			ModelImporter NewAvatarModelImporter = AssetImporter.GetAtPath(NewAvatarAssetPath) as ModelImporter;
			Undo.RecordObject(NewAvatarModelImporter, "Pasted from Old Avatar FBX Import Settings");
			NewAvatarModelImporter.addCollider = OldAvatarModelImporter.addCollider;
			NewAvatarModelImporter.animationCompression = OldAvatarModelImporter.animationCompression;
			NewAvatarModelImporter.animationPositionError = OldAvatarModelImporter.animationPositionError;
			NewAvatarModelImporter.animationRotationError = OldAvatarModelImporter.animationRotationError;
			NewAvatarModelImporter.animationScaleError = OldAvatarModelImporter.animationScaleError;
			NewAvatarModelImporter.animationType = OldAvatarModelImporter.animationType;
			NewAvatarModelImporter.animationWrapMode = OldAvatarModelImporter.animationWrapMode;
			NewAvatarModelImporter.autoGenerateAvatarMappingIfUnspecified = OldAvatarModelImporter.autoGenerateAvatarMappingIfUnspecified;
			NewAvatarModelImporter.avatarSetup = OldAvatarModelImporter.avatarSetup;
			NewAvatarModelImporter.bakeIK = OldAvatarModelImporter.bakeIK;
			NewAvatarModelImporter.clipAnimations = OldAvatarModelImporter.clipAnimations;
			NewAvatarModelImporter.extraExposedTransformPaths = OldAvatarModelImporter.extraExposedTransformPaths;
			NewAvatarModelImporter.extraUserProperties = OldAvatarModelImporter.extraUserProperties;
			NewAvatarModelImporter.generateAnimations = OldAvatarModelImporter.generateAnimations;
			NewAvatarModelImporter.generateSecondaryUV = OldAvatarModelImporter.generateSecondaryUV;
			NewAvatarModelImporter.globalScale = OldAvatarModelImporter.globalScale;
			NewAvatarModelImporter.humanDescription = OldAvatarModelImporter.humanDescription;
			NewAvatarModelImporter.humanoidOversampling = OldAvatarModelImporter.humanoidOversampling;
			NewAvatarModelImporter.importAnimatedCustomProperties = OldAvatarModelImporter.importAnimatedCustomProperties;
			NewAvatarModelImporter.importAnimation = OldAvatarModelImporter.importAnimation;
			NewAvatarModelImporter.importBlendShapeNormals = OldAvatarModelImporter.importBlendShapeNormals;
			NewAvatarModelImporter.importBlendShapes = OldAvatarModelImporter.importBlendShapes;
			NewAvatarModelImporter.importCameras = OldAvatarModelImporter.importCameras;
			NewAvatarModelImporter.importConstraints = OldAvatarModelImporter.importConstraints;
			NewAvatarModelImporter.importLights = OldAvatarModelImporter.importLights;
			NewAvatarModelImporter.importNormals = OldAvatarModelImporter.importNormals;
			NewAvatarModelImporter.importTangents = OldAvatarModelImporter.importTangents;
			NewAvatarModelImporter.importVisibility = OldAvatarModelImporter.importVisibility;
			NewAvatarModelImporter.indexFormat = OldAvatarModelImporter.indexFormat;
			NewAvatarModelImporter.isReadable = OldAvatarModelImporter.isReadable;
			NewAvatarModelImporter.keepQuads = OldAvatarModelImporter.keepQuads;
			NewAvatarModelImporter.materialImportMode = OldAvatarModelImporter.materialImportMode;
			NewAvatarModelImporter.materialLocation = OldAvatarModelImporter.materialLocation;
			NewAvatarModelImporter.materialName = OldAvatarModelImporter.materialName;
			NewAvatarModelImporter.materialSearch = OldAvatarModelImporter.materialSearch;
			NewAvatarModelImporter.maxBonesPerVertex = OldAvatarModelImporter.maxBonesPerVertex;
			NewAvatarModelImporter.meshCompression = OldAvatarModelImporter.meshCompression;
			NewAvatarModelImporter.meshOptimizationFlags = OldAvatarModelImporter.meshOptimizationFlags;
			NewAvatarModelImporter.minBoneWeight = OldAvatarModelImporter.minBoneWeight;
			NewAvatarModelImporter.motionNodeName = OldAvatarModelImporter.motionNodeName;
			NewAvatarModelImporter.normalCalculationMode = OldAvatarModelImporter.normalCalculationMode;
			NewAvatarModelImporter.normalSmoothingAngle = OldAvatarModelImporter.normalSmoothingAngle;
			NewAvatarModelImporter.normalSmoothingSource = OldAvatarModelImporter.normalSmoothingSource;
			NewAvatarModelImporter.optimizeGameObjects = OldAvatarModelImporter.optimizeGameObjects;
			NewAvatarModelImporter.optimizeMeshPolygons = OldAvatarModelImporter.optimizeMeshPolygons;
			NewAvatarModelImporter.optimizeMeshVertices = OldAvatarModelImporter.optimizeMeshVertices;
			NewAvatarModelImporter.preserveHierarchy = OldAvatarModelImporter.preserveHierarchy;
			NewAvatarModelImporter.resampleCurves = OldAvatarModelImporter.resampleCurves;
			NewAvatarModelImporter.secondaryUVAngleDistortion = OldAvatarModelImporter.secondaryUVAngleDistortion;
			NewAvatarModelImporter.secondaryUVAreaDistortion = OldAvatarModelImporter.secondaryUVAreaDistortion;
			NewAvatarModelImporter.secondaryUVHardAngle = OldAvatarModelImporter.secondaryUVHardAngle;
			NewAvatarModelImporter.secondaryUVPackMargin = OldAvatarModelImporter.secondaryUVPackMargin;
			NewAvatarModelImporter.skinWeights = OldAvatarModelImporter.skinWeights;
			NewAvatarModelImporter.sortHierarchyByName = OldAvatarModelImporter.sortHierarchyByName;
			NewAvatarModelImporter.sourceAvatar = OldAvatarModelImporter.sourceAvatar;
			NewAvatarModelImporter.swapUVChannels = OldAvatarModelImporter.swapUVChannels;
			NewAvatarModelImporter.useFileScale = OldAvatarModelImporter.useFileScale;
			NewAvatarModelImporter.useFileUnits = OldAvatarModelImporter.useFileUnits;
			NewAvatarModelImporter.useSRGBMaterialColor = OldAvatarModelImporter.useSRGBMaterialColor;
			NewAvatarModelImporter.weldVertices = OldAvatarModelImporter.weldVertices;
			CheckLegacyBlendShapeNormals(OldAvatarModelImporter, NewAvatarModelImporter);
			CheckAvatarMaterials(OldAvatarModelImporter, NewAvatarModelImporter);
			EditorUtility.SetDirty(NewAvatarModelImporter);
			AssetDatabase.WriteImportSettingsIfDirty(NewAvatarAssetPath);
			NewAvatarModelImporter.SaveAndReimport();
			Undo.CollapseUndoOperations(UndoGroupIndex);
			NewAvatarPatched = true;
		}

		/// <summary>Legacy Blend Shape Normals 속성을 강제 복제합니다.</summary>
		static void CheckLegacyBlendShapeNormals(ModelImporter OldModelImporter, ModelImporter NewModelImporter) {
			string PropertyName = "legacyComputeAllNormalsFromSmoothingGroupsWhenMeshHasBlendShapes";
			PropertyInfo OldLegacyProperty = OldModelImporter.GetType().GetProperty(PropertyName, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
			PropertyInfo NewLegacyProperty = NewModelImporter.GetType().GetProperty(PropertyName, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
			OldModelImporter.isReadable = true;
			NewModelImporter.isReadable = true;
			NewLegacyProperty.SetValue(NewModelImporter, (bool)OldLegacyProperty.GetValue(OldModelImporter));
		}

		/// <summary>기존 아바타에서 지정된 Material이 있을 경우에 신규 아바타로 Material을 복제합니다.</summary>
		static void CheckAvatarMaterials(ModelImporter OldModelImporter, ModelImporter NewModelImporter) {
			Object[] OldAvatarObjects = OldModelImporter.GetExternalObjectMap().Values.ToArray();
			Material[] OldAvatarMaterials = new Material[OldAvatarObjects.Length];
			for (int Index = 0; Index < OldAvatarObjects.Length; Index++) {
				Material TargetMaterial = (Material)OldAvatarObjects[Index];
				if (TargetMaterial) {
					OldAvatarMaterials[Index] = TargetMaterial;
				} else {
					OldAvatarMaterials[Index] = null;
				}
			}
			foreach (Material TargetMaterial in OldAvatarMaterials) {
				if (!TargetMaterial) continue;
				NewModelImporter.AddRemap(new AssetImporter.SourceAssetIdentifier(TargetMaterial), TargetMaterial);
			}
		}

		/// <summary>새 아바타 GameObject를 Scene에 배치를 합니다.</summary>
		static void PlaceGameObejctInScene() {
			GameObject NewInstance = Instantiate(NewAvatarGameObject);
			NewInstance.name = NewAvatarGameObject.name;
			Undo.RegisterCreatedObjectUndo(NewInstance, "Added New GameObject");
			Undo.CollapseUndoOperations(UndoGroupIndex);
			NewAvatarGameObject = NewInstance;
		}

        /// <summary>새로 복제된 기존 아바타의 Animator의 Avatar를 신규 아바타의 Avatar로 변경합니다.</summary>
        static void ReplaceOldAvatarAnimatorAvatar() {
			OldAvatarAnimator.avatar = NewAvatarAnimator.avatar;
            EditorUtility.SetDirty(OldAvatarAnimator);
		}

    }
}
#endif