#if UNITY_EDITOR
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using UnityEditor;
using UnityEngine;

/*
 * VRSuya Installer
 * Contact : vrsuya@gmail.com // Twitter : https://twitter.com/VRSuya
 */

namespace VRSuya.Installer {

	public class CopyModelImporter {

		const string UndoGroupName = "VRSuya CopyModelImporter";

		ModelImporter OldModelImporter;
		ModelImporter NewModelImporter;

		public void RequestCopyModelImporter(string OldModelPath, string NewModelPath, int UndoGroupIndex = -1) {
			OldModelImporter = AssetImporter.GetAtPath(OldModelPath) as ModelImporter;
			NewModelImporter = AssetImporter.GetAtPath(NewModelPath) as ModelImporter;
			Undo.RecordObject(NewModelImporter, UndoGroupName);
			NewModelImporter.addCollider = OldModelImporter.addCollider;
			NewModelImporter.animationCompression = OldModelImporter.animationCompression;
			NewModelImporter.animationPositionError = OldModelImporter.animationPositionError;
			NewModelImporter.animationRotationError = OldModelImporter.animationRotationError;
			NewModelImporter.animationScaleError = OldModelImporter.animationScaleError;
			NewModelImporter.animationType = OldModelImporter.animationType;
			NewModelImporter.animationWrapMode = OldModelImporter.animationWrapMode;
			NewModelImporter.autoGenerateAvatarMappingIfUnspecified = OldModelImporter.autoGenerateAvatarMappingIfUnspecified;
			NewModelImporter.avatarSetup = OldModelImporter.avatarSetup;
			NewModelImporter.bakeIK = OldModelImporter.bakeIK;
			NewModelImporter.clipAnimations = OldModelImporter.clipAnimations;
			NewModelImporter.extraExposedTransformPaths = OldModelImporter.extraExposedTransformPaths;
			NewModelImporter.extraUserProperties = OldModelImporter.extraUserProperties;
			NewModelImporter.generateAnimations = OldModelImporter.generateAnimations;
			NewModelImporter.generateSecondaryUV = OldModelImporter.generateSecondaryUV;
			NewModelImporter.globalScale = OldModelImporter.globalScale;
			NewModelImporter.humanDescription = OldModelImporter.humanDescription;
			NewModelImporter.humanoidOversampling = OldModelImporter.humanoidOversampling;
			NewModelImporter.importAnimatedCustomProperties = OldModelImporter.importAnimatedCustomProperties;
			NewModelImporter.importAnimation = OldModelImporter.importAnimation;
			NewModelImporter.importBlendShapeDeformPercent = OldModelImporter.importBlendShapeDeformPercent;
			NewModelImporter.importBlendShapeNormals = OldModelImporter.importBlendShapeNormals;
			NewModelImporter.importBlendShapes = OldModelImporter.importBlendShapes;
			NewModelImporter.importCameras = OldModelImporter.importCameras;
			NewModelImporter.importConstraints = OldModelImporter.importConstraints;
			NewModelImporter.importLights = OldModelImporter.importLights;
			NewModelImporter.importNormals = OldModelImporter.importNormals;
			NewModelImporter.importTangents = OldModelImporter.importTangents;
			NewModelImporter.importVisibility = OldModelImporter.importVisibility;
			NewModelImporter.indexFormat = OldModelImporter.indexFormat;
			NewModelImporter.isReadable = OldModelImporter.isReadable;
			NewModelImporter.keepQuads = OldModelImporter.keepQuads;
			NewModelImporter.materialImportMode = OldModelImporter.materialImportMode;
			NewModelImporter.materialLocation = OldModelImporter.materialLocation;
			NewModelImporter.materialName = OldModelImporter.materialName;
			NewModelImporter.materialSearch = OldModelImporter.materialSearch;
			NewModelImporter.maxBonesPerVertex = OldModelImporter.maxBonesPerVertex;
			NewModelImporter.meshCompression = OldModelImporter.meshCompression;
			NewModelImporter.meshOptimizationFlags = OldModelImporter.meshOptimizationFlags;
			NewModelImporter.minBoneWeight = OldModelImporter.minBoneWeight;
			NewModelImporter.motionNodeName = OldModelImporter.motionNodeName;
			NewModelImporter.normalCalculationMode = OldModelImporter.normalCalculationMode;
			NewModelImporter.normalSmoothingAngle = OldModelImporter.normalSmoothingAngle;
			NewModelImporter.normalSmoothingSource = OldModelImporter.normalSmoothingSource;
			NewModelImporter.optimizeBones = OldModelImporter.optimizeBones;
			NewModelImporter.optimizeGameObjects = OldModelImporter.optimizeGameObjects;
			NewModelImporter.optimizeMeshPolygons = OldModelImporter.optimizeMeshPolygons;
			NewModelImporter.optimizeMeshVertices = OldModelImporter.optimizeMeshVertices;
			NewModelImporter.preserveHierarchy = OldModelImporter.preserveHierarchy;
			NewModelImporter.removeConstantScaleCurves = OldModelImporter.removeConstantScaleCurves;
			NewModelImporter.resampleCurves = OldModelImporter.resampleCurves;
			NewModelImporter.secondaryUVAngleDistortion = OldModelImporter.secondaryUVAngleDistortion;
			NewModelImporter.secondaryUVAreaDistortion = OldModelImporter.secondaryUVAreaDistortion;
			NewModelImporter.secondaryUVHardAngle = OldModelImporter.secondaryUVHardAngle;
			NewModelImporter.secondaryUVMarginMethod = OldModelImporter.secondaryUVMarginMethod;
			NewModelImporter.secondaryUVMinLightmapResolution = OldModelImporter.secondaryUVMinLightmapResolution;
			NewModelImporter.secondaryUVMinObjectScale = OldModelImporter.secondaryUVMinObjectScale;
			NewModelImporter.secondaryUVPackMargin = OldModelImporter.secondaryUVPackMargin;
			NewModelImporter.skinWeights = OldModelImporter.skinWeights;
			NewModelImporter.sortHierarchyByName = OldModelImporter.sortHierarchyByName;
			NewModelImporter.sourceAvatar = OldModelImporter.sourceAvatar;
			NewModelImporter.strictVertexDataChecks = OldModelImporter.strictVertexDataChecks;
			NewModelImporter.swapUVChannels = OldModelImporter.swapUVChannels;
			NewModelImporter.useFileScale = OldModelImporter.useFileScale;
			NewModelImporter.useFileUnits = OldModelImporter.useFileUnits;
			NewModelImporter.useSRGBMaterialColor = OldModelImporter.useSRGBMaterialColor;
			NewModelImporter.weldVertices = OldModelImporter.weldVertices;
			CopyLegacyBlendShapeNormals();
			CopyMaterials();
			EditorUtility.SetDirty(NewModelImporter);
			AssetDatabase.WriteImportSettingsIfDirty(NewModelPath);
			NewModelImporter.SaveAndReimport();
			if (UndoGroupIndex != -1) Undo.CollapseUndoOperations(UndoGroupIndex);
		}

		void CopyLegacyBlendShapeNormals() {
			string PropertyName = "legacyComputeAllNormalsFromSmoothingGroupsWhenMeshHasBlendShapes";
			PropertyInfo OldProperty = OldModelImporter.GetType().GetProperty(PropertyName, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
			PropertyInfo NewProperty = NewModelImporter.GetType().GetProperty(PropertyName, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
			OldModelImporter.isReadable = true;
			NewModelImporter.isReadable = true;
			NewProperty.SetValue(NewModelImporter, (bool)OldProperty.GetValue(OldModelImporter));
		}

		void CopyMaterials() {
			Dictionary<string, Material> OldModelMaterialDictionary = OldModelImporter.GetExternalObjectMap()
				.Where(KeyValuePair => KeyValuePair.Value is Material)
				.ToDictionary(
					KeyValuePair => KeyValuePair.Key.name,
					KeyValuePair => KeyValuePair.Value as Material
				);
			foreach (KeyValuePair<AssetImporter.SourceAssetIdentifier, Object> NewModelExternalObject in NewModelImporter.GetExternalObjectMap()) {
				if (!(NewModelExternalObject.Value is Material)) continue;
				if (!OldModelMaterialDictionary.TryGetValue(NewModelExternalObject.Key.name, out Material MatchedMaterial)) continue;
				NewModelImporter.AddRemap(NewModelExternalObject.Key, MatchedMaterial);
			}
		}
	}
}
#endif