#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using UnityEditor;
using UnityEditor.Animations;

using VRC.SDK3.Avatars.Components;

/*
 * VRSuya Suyasuya Facial
 * Contact : vrsuya@gmail.com // Twitter : https://twitter.com/VRSuya
 */

namespace com.vrsuya.suyasuyafacial {

	[Serializable]
	public struct FaceBlendShape {
		public bool ActiveValue;
		public int IndexValue;
		public string BlendShapeName;
		public float BlendShapeValue;

		public FaceBlendShape(bool BoolValue, int IntValue, string StringName, float FloatValue) {
			ActiveValue = BoolValue;
			IndexValue = IntValue;
			BlendShapeName = StringName;
			BlendShapeValue = FloatValue;
		}
	};

	[Serializable]
	public struct AnimationBlendShape {
		public bool ActiveValue;
		public string BlendShapeName;

		public AnimationBlendShape(bool BoolValue, string StringName) {
			ActiveValue = BoolValue;
			BlendShapeName = StringName;
		}
	};

	[ExecuteInEditMode]
	[AddComponentMenu("VRSuya/VRSuya Suyasuya Facial")]
	public class SuyasuyaFacial : MonoBehaviour {

        // 아바타 에디터용 변수
        public GameObject AvatarGameObject;
		public SkinnedMeshRenderer AvatarHeadSkinnedMeshRenderer;
		public AnimatorController AvatarFXAnimatorController;
		public AnimationClip[] TargetAnimationClips = new AnimationClip[0];
		public FaceBlendShape[] TargetBlendShapes = new FaceBlendShape[0];
		public AnimationBlendShape[] TargetAnimationBlendShapes = new AnimationBlendShape[0];
		
		// 상태 반환
		public string StatusCode = "";
		public int CountUpdatedCurve = 0;
		public int UndoGroupIndex;

		// 컴포넌트 최초 로드시 동작
		void OnEnable() {
			if (!AvatarGameObject) AvatarGameObject = this.gameObject;
			AvatarHeadSkinnedMeshRenderer = GetAvatarHeadSkinnedMeshRenderer(AvatarGameObject);
			if (AvatarHeadSkinnedMeshRenderer) {
				TargetBlendShapes = GetAvatarBlendShapeStatus(AvatarHeadSkinnedMeshRenderer);
			}
			AvatarFXAnimatorController = GetAvatarFXAnimatorController(AvatarGameObject);
			if (AvatarFXAnimatorController) {
				TargetAnimationBlendShapes = GetAnimationBlendShapeStatus(AvatarFXAnimatorController);
			}
			if (TargetAnimationClips.Length == 0) TargetAnimationClips = GetVRSuyaSuyasuyaAnimations();
			CleanupBlendShapeList();
		}

		/// <summary>
		/// 본 프로그램의 메인 세팅 로직입니다.
		/// </summary>
		public void UpdateAnimationClips() {
			Undo.IncrementCurrentGroup();
			Undo.SetCurrentGroupName("VRSuya Suyasuya Facial");
			UndoGroupIndex = Undo.GetCurrentGroup();
			if (VerifyVariable()) {
				UpdateSuyasuyaAnimationClips();
				StatusCode = "COMPLETED";
				Debug.Log("[VRSuya] Update Animation Clips Completed");
            }
        }

		/// <summary>변수를 초기화하고 다시 불러옵니다.</summary>
		public void ReloadVariable() {
			TargetBlendShapes = new FaceBlendShape[0];
			TargetAnimationBlendShapes = new AnimationBlendShape[0];
			CountUpdatedCurve = 0;
			if (!AvatarGameObject) AvatarGameObject = this.gameObject;
			if (!AvatarHeadSkinnedMeshRenderer) {
				AvatarHeadSkinnedMeshRenderer = GetAvatarHeadSkinnedMeshRenderer(AvatarGameObject);
			}
			if (AvatarHeadSkinnedMeshRenderer) {
				TargetBlendShapes = GetAvatarBlendShapeStatus(AvatarHeadSkinnedMeshRenderer);
			}
			if (!AvatarFXAnimatorController) AvatarFXAnimatorController = GetAvatarFXAnimatorController(AvatarGameObject);
			if (AvatarFXAnimatorController) {
				TargetAnimationBlendShapes = GetAnimationBlendShapeStatus(AvatarFXAnimatorController);
			}
			if (TargetAnimationClips.Length == 0) TargetAnimationClips = GetVRSuyaSuyasuyaAnimations();
			CleanupBlendShapeList();
		}

		/// <summary>아바타의 현재 상태를 검사하여 패치가 가능한지 확인합니다.</summary>
		/// <returns>설치 가능 여부</returns>
		bool VerifyVariable() {
			if (AvatarHeadSkinnedMeshRenderer) {
				return true;
			} else {
				if (AvatarGameObject) {
					AvatarHeadSkinnedMeshRenderer = GetAvatarHeadSkinnedMeshRenderer(AvatarGameObject);
					if (AvatarHeadSkinnedMeshRenderer) {
						return true;
					} else {
						StatusCode = "NO_FACEMESH";
						return false;
					}
				} else {
					StatusCode = "NO_AVATAR";
					return false;
				}
			}
        }

		/// <summary>아바타의 Head 스킨드 메쉬 렌더러를 찾습니다.</summary>
		/// <returns>아바타의 Head 스킨드 메쉬 렌더러</returns>
		SkinnedMeshRenderer GetAvatarHeadSkinnedMeshRenderer(GameObject TargetGameObject) {
			AvatarGameObject.TryGetComponent(typeof(VRCAvatarDescriptor), out Component AvatarDescriptor);
			if (AvatarDescriptor) {
				return AvatarDescriptor.GetComponent<VRCAvatarDescriptor>().VisemeSkinnedMesh;
			}
			return null;
		}

		/// <summary>아바타의 FX 레이어 애니메이터 컨트롤러를 찾습니다.</summary>
		/// <returns>아바타의 FX 레이어 애니메이터 컨트롤러</returns>
		AnimatorController GetAvatarFXAnimatorController(GameObject TargetGameObject) {
			AvatarGameObject.TryGetComponent(typeof(VRCAvatarDescriptor), out Component AvatarDescriptor);
			if (AvatarDescriptor) {
				VRCAvatarDescriptor.CustomAnimLayer TargetAnimatorController = Array.Find(AvatarDescriptor.GetComponent<VRCAvatarDescriptor>().baseAnimationLayers, AnimationLayer => AnimationLayer.type == VRCAvatarDescriptor.AnimLayerType.FX);
				return (AnimatorController)TargetAnimatorController.animatorController;
			}
			return null;
		}

		/// <summary>아바타에서 적용된 Blendshape 리스트를 반환합니다.</summary>
		/// <returns>활성화 아바타 BlendShape 어레이</returns>
		FaceBlendShape[] GetAvatarBlendShapeStatus(SkinnedMeshRenderer TargetSkinnedMeshRenderer) {
			FaceBlendShape[] newFaceBlendShapeList = new FaceBlendShape[0];
			if (TargetSkinnedMeshRenderer.sharedMesh.blendShapeCount > 0) {
				for (int Index = 0; Index < TargetSkinnedMeshRenderer.sharedMesh.blendShapeCount; Index++) {
					if (TargetSkinnedMeshRenderer.GetBlendShapeWeight(Index) > 0) {
						FaceBlendShape TargetBlendShape = new FaceBlendShape(
							false,
							Index,
							TargetSkinnedMeshRenderer.sharedMesh.GetBlendShapeName(Index),
							TargetSkinnedMeshRenderer.GetBlendShapeWeight(Index)
						);
						newFaceBlendShapeList = newFaceBlendShapeList.Concat(new FaceBlendShape[] { TargetBlendShape }).ToArray();
					}
				}
			}
			return newFaceBlendShapeList;
		}

		/// <summary>애니메이션에서 적용된 Blendshape 리스트를 반환합니다.</summary>
		/// <returns>활성화 FX 레이어 BlendShape 어레이</returns>
		AnimationBlendShape[] GetAnimationBlendShapeStatus(AnimatorController TargetAnimatorController) {
			AnimationBlendShape[] newAnimationBlendShapeList = new AnimationBlendShape[0];
			string TargetPath = "Body";
			if (AvatarHeadSkinnedMeshRenderer) TargetPath = AvatarHeadSkinnedMeshRenderer.name;
			string[] AvatarBlendShapes = new string[0];
            if (TargetBlendShapes.Length > 0) {
				AvatarBlendShapes = TargetBlendShapes.Select(AvatarBlendShape => AvatarBlendShape.BlendShapeName).ToArray();
			}
            if (TargetAnimatorController.animationClips.Length > 0) {
				foreach (AnimationClip TargetAnimationClip in TargetAnimatorController.animationClips) {
					string[] TargetAnimationBlendshapeList = GetExistAnimationBlendshapes(TargetAnimationClip, true)
						.Where((AnimationBlendShape) => AnimationBlendShape.Path == TargetPath)
						.Select((AnimationBlendShape) => AnimationBlendShape.BlendShapeName)
						.ToArray();
					if (TargetAnimationBlendshapeList.Length > 0) {
						for (int Index = 0; Index < TargetAnimationBlendshapeList.Length; Index++) {
							if (!Array.Exists(AvatarBlendShapes, AvatarBlendShape => AvatarBlendShape == TargetAnimationBlendshapeList[Index])) {
								AnimationBlendShape TargetBlendShape = new AnimationBlendShape(
									false,
									TargetAnimationBlendshapeList[Index]
									);
								newAnimationBlendShapeList = newAnimationBlendShapeList.Concat(new AnimationBlendShape[] { TargetBlendShape }).ToArray();
							}
						}
					}
				}
				newAnimationBlendShapeList = newAnimationBlendShapeList.Distinct().ToArray();
				Array.Sort(newAnimationBlendShapeList, (a, b) => string.Compare(a.BlendShapeName, b.BlendShapeName, StringComparison.Ordinal));
			}
			return newAnimationBlendShapeList;
		}

		/// <summary>블렌드쉐이프 리스트에서 중복된 값을 제외합니다.</summary>
		void CleanupBlendShapeList() {
			if (TargetAnimationClips.Length > 0) {
				string TargetPath = "Body";
				if (AvatarHeadSkinnedMeshRenderer) TargetPath = AvatarHeadSkinnedMeshRenderer.name;
				string[] ExistAnimationBlendShapes = new string[0];
				foreach (AnimationClip TargetAnimationClip in TargetAnimationClips) {
					ExistAnimationBlendShapes = ExistAnimationBlendShapes.Concat(GetExistAnimationBlendshapes(TargetAnimationClip, false)
						.Where((AnimationBlendShape) => AnimationBlendShape.Path == TargetPath)
						.Select((AnimationBlendShape) => AnimationBlendShape.BlendShapeName)
						.ToArray()).Distinct().ToArray();
				}
				if (ExistAnimationBlendShapes.Length > 0) {
					if (TargetBlendShapes.Length > 0) TargetBlendShapes = Array.FindAll(TargetBlendShapes, TargetBlendShape => !ExistAnimationBlendShapes.Contains(TargetBlendShape.BlendShapeName));
					if (TargetAnimationBlendShapes.Length > 0) TargetAnimationBlendShapes = Array.FindAll(TargetAnimationBlendShapes, TargetAnimationBlendShape => !ExistAnimationBlendShapes.Contains(TargetAnimationBlendShape.BlendShapeName));
				}
			}
		}

		/// <summary>에셋에서 Suyasuya 애니메이션을 찾아서 리스트로 반환 합니다.</summary>
		/// <returns>Suyasuya 애니메이션 클립 어레이</returns>
		AnimationClip[] GetVRSuyaSuyasuyaAnimations() {
			Dictionary<string,string> dictSuyasuyaAnimationClips = new Dictionary<string, string> {
				{ "3e14bdc27b543fc4d9c16398be0a9d9c", "VRSuya_Suyasuya_Animation_50_Sleeping_Animation" },
				{ "b6c63881d1b4cc740a95793be2b0ff7f", "VRSuya_Suyasuya_Animation_50_Sleeping_NoAnimation" },
				{ "fd098f6d505fb1749971fc83d7d4b6c1", "VRSuya_Suyasuya_Animation_50_WakeUp" },
				{ "70b313f6f6d5c2746a2a78c8ab7254d5", "VRSuya_Suyasuya_Animation_100_Sleeping_Animation" },
				{ "3809f2986699be140a19aa19b1bf40b8", "VRSuya_Suyasuya_Animation_100_Sleeping_NoAnimation" },
				{ "22426a8e76770e84da9c2c1d2497dadd", "VRSuya_Suyasuya_Animation_100_WakeUp"}
			};
			AnimationClip[] newAnimationClips = new AnimationClip[0];
			foreach (KeyValuePair<string, string> dictTargetAnimationClip in dictSuyasuyaAnimationClips) {
				if (!String.IsNullOrEmpty(GUIDToAssetName(dictTargetAnimationClip.Key, true))) {
					AnimationClip TargetAnimationClip = AssetDatabase.LoadAssetAtPath<AnimationClip>(AssetDatabase.GUIDToAssetPath(dictTargetAnimationClip.Key));
					if (TargetAnimationClip && TargetAnimationClip.GetType() == typeof(AnimationClip)) {
						newAnimationClips = newAnimationClips.Concat(new AnimationClip[] { TargetAnimationClip }).ToArray();
					}
				}
			}
			return newAnimationClips;
		}

		/// <summary>애니메이션 클립에서 0 값의 Blendshape 애니메이션 키를 추가합니다.</summary>
		void UpdateSuyasuyaAnimationClips() {
			CountUpdatedCurve = 0;
			foreach (AnimationClip CurrentAnimationClip in TargetAnimationClips) {
				string[] AddBlendshapeList = GetAddBlendshapeList(CurrentAnimationClip);
				if (AddBlendshapeList.Length > 0) {
					Undo.RecordObject(CurrentAnimationClip, "Added new blendshape animation");
					foreach (string TargetBlendshape in AddBlendshapeList) {
						EditorCurveBinding newEditorCurveBinding = new EditorCurveBinding {
							type = typeof(SkinnedMeshRenderer),
							path = AvatarHeadSkinnedMeshRenderer.name,
							propertyName = "blendShape." + TargetBlendshape
						};
						Keyframe[] newKeyframe = new Keyframe[1];
						newKeyframe[0] = new Keyframe(0f, 0f);
						AnimationCurve newAnimationCurve = new AnimationCurve(newKeyframe);
						AnimationUtility.SetEditorCurve(CurrentAnimationClip, newEditorCurveBinding, newAnimationCurve);
						CountUpdatedCurve++;
					}
					EditorUtility.SetDirty(CurrentAnimationClip);
					Undo.CollapseUndoOperations(UndoGroupIndex);
				}
			}
			Debug.Log($"[VRSuya] {CountUpdatedCurve} blendshapes have been added to the animation clip");
		}

		/// <summary>애니메이션 클립에서 존재하는 Blendshape 리스트를 반환 합니다.</summary>
		/// <returns>애니메이션 클립에 존재하는 Blendshape 리스트</returns>
		List<(string Path, string BlendShapeName)> GetExistAnimationBlendshapes(AnimationClip TargetAnimationClip, bool HasValue) {
			List<(string Path, string BlendShapeName)> newExistBlendshapes = new List<(string, string)> { };
			foreach (EditorCurveBinding Binding in AnimationUtility.GetCurveBindings(TargetAnimationClip)) {
				if (Binding.type == typeof(SkinnedMeshRenderer)) {
					if (Binding.propertyName.Contains("blendShape")) {
						if (HasValue) {
							AnimationCurve TargetAnimationCurve = AnimationUtility.GetEditorCurve(TargetAnimationClip, Binding);
							if (Array.Exists(TargetAnimationCurve.keys, AnimKey => AnimKey.value > 0.0f)) {
								string BlendShapeName = Binding.propertyName.Remove(0, 11);
								newExistBlendshapes.Add((Binding.path, BlendShapeName));
							}
						} else {
							string BlendShapeName = Binding.propertyName.Remove(0, 11);
							newExistBlendshapes.Add((Binding.path, BlendShapeName));
						}
					}
				}
			}
			return newExistBlendshapes;
		}

		/// <summary>애니메이션 클립과 비교하여 실제로 삽입해야 되는 Blendshape 어레이를 반환합니다.</summary>
		/// <returns>중복 되지 않는 Blendshape 어레이</returns>
		string[] GetAddBlendshapeList(AnimationClip TargetAnimationClip) {
			string[] TargetAnimationBlendshapeList = GetExistAnimationBlendshapes(TargetAnimationClip, false)
				.Where((AnimationBlendShape) => AnimationBlendShape.Path == AvatarHeadSkinnedMeshRenderer.name)
				.Select((AnimationBlendShape) => AnimationBlendShape.BlendShapeName)
				.ToArray();
			string[] AvatarBlendShapes = TargetBlendShapes
				.Where((AvatarBlendShape) => AvatarBlendShape.ActiveValue == true)
				.Select((AvatarBlendShape) => AvatarBlendShape.BlendShapeName)
				.ToArray();
			string[] AnimationBlendShapes = TargetAnimationBlendShapes
				.Where((AnimationBlendShape) => AnimationBlendShape.ActiveValue == true)
				.Select((AnimationBlendShape) => AnimationBlendShape.BlendShapeName)
				.ToArray();
			string[] FinalBlendShapes = AvatarBlendShapes.Concat(AnimationBlendShapes).ToArray();
			string[] AddBlendshapeList = FinalBlendShapes
				.Where((TargetBlendShape) => Array.Exists(TargetAnimationBlendshapeList, AnimationBlendShape => TargetBlendShape != AnimationBlendShape))
				.ToArray();
			return AddBlendshapeList;
		}

		/// <summary>요청한 GUID를 파일 이름으로 반환합니다. 2번째 인자는 확장명 포함 여부를 결정합니다.</summary>
		/// <returns>파일 이름</returns>
		string GUIDToAssetName(string GUID, bool OnlyFileName) {
			string FileName = "";
			FileName = AssetDatabase.GUIDToAssetPath(GUID).Split('/')[AssetDatabase.GUIDToAssetPath(GUID).Split('/').Length - 1];
			if (OnlyFileName) FileName = FileName.Split('.')[0];
			return FileName;
		}
	}
}
#endif