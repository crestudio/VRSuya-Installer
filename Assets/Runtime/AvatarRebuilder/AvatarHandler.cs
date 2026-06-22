#if UNITY_EDITOR
using System.Collections.Generic;
using System.Linq;

using UnityEditor;
using UnityEngine;

/*
 * VRSuya Installer
 * Contact : vrsuya@gmail.com // Twitter : https://twitter.com/VRSuya
 */

namespace VRSuya.Installer {

	internal class AvatarHandler {

		readonly AvatarRebuilderContext Context;

		internal AvatarHandler(AvatarRebuilderContext Context) {
			this.Context = Context;
		}

		Dictionary<string, Transform> AvatarBoneDictionary = new Dictionary<string, Transform>();
		Transform[] MissingBoneTransforms = new Transform[0];

		static readonly string[] RestoreBoneNames = new string[] {
			"Cheek_Root_L", "Cheek_Root_R", "Cheek_L", "Cheek_R", "ho_L", "ho_R"
		};
		const float Threshold = 0.01f;

		const string UndoGroupName = "VRSuya AvatarRebuilder";

		internal string UpdateAvatarArmature() {
			try {
				AvatarBoneDictionary = GetAvatarBoneDictionary();
				if (AvatarBoneDictionary.Count == 0) return "NO_MATCHED_SKINNEDMESHRENDERERS";
				MissingBoneTransforms = GetMissingBoneTransforms();
				AddMissingBones();
				RestoreCheekTransform();
				UpdateSkinnedMeshRenderer();
				UpdateAnimatorAvatar();
			} finally {
				Object.DestroyImmediate(Context.NewAvatarGameObject);
			}
			return "COMPLETED";
		}

		Dictionary<string, Transform> GetAvatarBoneDictionary() {
			HashSet<string> NewSkinnedMeshRendererNames = new HashSet<string>(
				Context.NewAvatarGameObject
				.GetComponentsInChildren<SkinnedMeshRenderer>(true)
				.Select(Item => Item.name)
			);
			SkinnedMeshRenderer[] ModelSkinnedMeshRenderers = Context.OldAvatarGameObject
				.GetComponentsInChildren<SkinnedMeshRenderer>(true)
				.Where(Item => NewSkinnedMeshRendererNames.Contains(Item.name))
				.ToArray();
			HashSet<Transform> AvatarBoneTransforms = new HashSet<Transform>(
				ModelSkinnedMeshRenderers
					.SelectMany(Item => Item.bones)
					.Where(Item => Item != null)
			);
			Dictionary<string, Transform> NewAvatarBoneDictionary = new Dictionary<string, Transform>();
			foreach (Transform TargetTransform in AvatarBoneTransforms) {
				if (!NewAvatarBoneDictionary.ContainsKey(TargetTransform.name)) {
					NewAvatarBoneDictionary[TargetTransform.name] = TargetTransform;
				} else {
					Debug.LogWarning($"[VRSuya] Duplicate bone name detected in the old avatar model : {TargetTransform.name}");
				}
			}
			return NewAvatarBoneDictionary;
		}

		Transform[] GetMissingBoneTransforms() {
			HashSet<Transform> NewBoneTransforms = new HashSet<Transform>(
				Context.NewAvatarGameObject
					.GetComponentsInChildren<SkinnedMeshRenderer>(true)
					.SelectMany(Item => Item.bones)
					.Where(Item => Item != null)
			);
			return NewBoneTransforms
				.Where(Item => !AvatarBoneDictionary.ContainsKey(Item.name))
				.ToArray();
		}

		void AddMissingBones() {
			IEnumerable<Transform> SortedMissingBones = MissingBoneTransforms.OrderBy(Item => GetTransformDepth(Item));
			foreach (Transform TargetTransform in SortedMissingBones) {
				Transform ParentTransform = GetParentTransform(TargetTransform);
				if (!ParentTransform) {
					Debug.LogWarning($"[VRSuya] Parent bone of {TargetTransform.name} could not be found in the old avatar");
					continue;
				}
				GameObject NewBoneGameObject = new GameObject(TargetTransform.name);
				Undo.RegisterCreatedObjectUndo(NewBoneGameObject, UndoGroupName);
				NewBoneGameObject.transform.SetParent(ParentTransform, worldPositionStays: false);
				NewBoneGameObject.transform.localPosition = TargetTransform.localPosition;
				NewBoneGameObject.transform.localRotation = TargetTransform.localRotation;
				NewBoneGameObject.transform.localScale = TargetTransform.localScale;
				AvatarBoneDictionary[NewBoneGameObject.name] = NewBoneGameObject.transform;
				EditorUtility.SetDirty(ParentTransform.gameObject);
				Undo.CollapseUndoOperations(Context.UndoGroupIndex);
			}
		}

		void UpdateSkinnedMeshRenderer() {
			Dictionary<string, SkinnedMeshRenderer> ExistingSkinnedMeshRendererDictionary = Context.OldAvatarGameObject
				.GetComponentsInChildren<SkinnedMeshRenderer>(true)
				.GroupBy(Item => Item.name)
				.ToDictionary(
					Item => Item.Key,
					Item => Item.First()
				);
			SkinnedMeshRenderer[] NewSkinnedMeshRenderers = Context.NewAvatarGameObject.GetComponentsInChildren<SkinnedMeshRenderer>(true);
			foreach (SkinnedMeshRenderer TargetSkinnedMeshRenderer in NewSkinnedMeshRenderers) {
				if (!ExistingSkinnedMeshRendererDictionary.TryGetValue(TargetSkinnedMeshRenderer.name, out SkinnedMeshRenderer OldSkinnedMeshRenderer)) continue;
				Transform[] NewBones = GetNewBones(TargetSkinnedMeshRenderer.bones);
				Undo.RecordObject(OldSkinnedMeshRenderer, UndoGroupName);
				OldSkinnedMeshRenderer.sharedMesh = TargetSkinnedMeshRenderer.sharedMesh;
				OldSkinnedMeshRenderer.bones = NewBones;
				EditorUtility.SetDirty(OldSkinnedMeshRenderer);
				Undo.CollapseUndoOperations(Context.UndoGroupIndex);
			}
		}

		Transform[] GetNewBones(Transform[] NewSkinnedMeshRendererBones) {
			Transform[] NewBones = new Transform[NewSkinnedMeshRendererBones.Length];
			for (int Index = 0; Index < NewSkinnedMeshRendererBones.Length; Index++) {
				Transform NewBone = NewSkinnedMeshRendererBones[Index];
				if (!NewBone) {
					NewBones[Index] = null;
					continue;
				}
				if (AvatarBoneDictionary.TryGetValue(NewBone.name, out Transform BoneTransform)) {
					NewBones[Index] = BoneTransform;
				} else {
					Debug.LogWarning($"[VRSuya] Failed to find the {NewBone.name} bone");
					NewBones[Index] = null;
				}
			}
			return NewBones;
		}

		void UpdateAnimatorAvatar() {
			Undo.RecordObject(Context.OldAvatarAnimator, UndoGroupName);
			Context.OldAvatarAnimator.avatar = Context.NewAvatarAnimator.avatar;
			EditorUtility.SetDirty(Context.OldAvatarAnimator);
			Undo.CollapseUndoOperations(Context.UndoGroupIndex);
		}

		void RestoreCheekTransform() {
			List<Transform> RestoreBoneList = AvatarBoneDictionary
				.Where(Item => RestoreBoneNames.Contains(Item.Key))
				.Select(Item => Item.Value)
				.ToList();
			if (RestoreBoneList.Count > 0) {
				HashSet<Transform> NewAvatarBoneTransform = new HashSet<Transform>(
					Context.NewAvatarGameObject
						.GetComponentsInChildren<SkinnedMeshRenderer>(true)
						.SelectMany(Item => Item.bones)
						.Where(Item => RestoreBoneNames.Contains(Item.name))
				);
				foreach (Transform TargetTransform in RestoreBoneList) {
					Transform NewTransform = NewAvatarBoneTransform.FirstOrDefault(Item => Item.name == TargetTransform.name);
					if (NewTransform) {
						float PositionDifference = Vector3.Distance(TargetTransform.localPosition, NewTransform.localPosition);
						if (PositionDifference <= Threshold) continue;
						Undo.RecordObject(TargetTransform, UndoGroupName);
						TargetTransform.transform.localPosition = NewTransform.localPosition;
						TargetTransform.transform.localRotation = NewTransform.localRotation;
						TargetTransform.transform.localScale = NewTransform.localScale;
						EditorUtility.SetDirty(TargetTransform);
						Undo.CollapseUndoOperations(Context.UndoGroupIndex);
					}
				}
			}
		}

		int GetTransformDepth(Transform TargetTransform) {
			int Depth = 0;
			Transform CurrentTransform = TargetTransform;
			while (CurrentTransform.parent != null) {
				Depth++;
				CurrentTransform = CurrentTransform.parent;
			}
			return Depth;
		}

		Transform GetParentTransform(Transform TargetTransform) {
			Transform ParentTransform = TargetTransform.parent;
			while (ParentTransform != null) {
				if (AvatarBoneDictionary.TryGetValue(ParentTransform.name, out Transform NewTransform)) {
					return NewTransform;
				}
				ParentTransform = ParentTransform.parent;
			}
			return null;
		}
	}
}
#endif
