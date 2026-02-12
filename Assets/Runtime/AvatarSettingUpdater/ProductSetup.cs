#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;

using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;

using VRC.SDK3.Avatars.Components;
using VRC.SDK3.Avatars.ScriptableObjects;

using Animator = VRSuya.Core.Animator;

/*
 * VRSuya Avatar Setting Updater
 * Contact : vrsuya@gmail.com // Twitter : https://twitter.com/VRSuya
 */

namespace com.vrsuya.installer {

	[ExecuteInEditMode]
	[AddComponentMenu("")]
	public class ProductSetup : AvatarSettingUpdater {

		// 추가될 Unity 데이터
		protected static AnimatorControllerLayer[] VRSuyaLocomotionLayers;
		protected static AnimatorControllerLayer[] VRSuyaGestureLayers;
		protected static AnimatorControllerLayer[] VRSuyaActionLayers;
		protected static AnimatorControllerLayer[] VRSuyaFXLayers;

		protected static AnimatorControllerParameter[] VRSuyaLocomotionParameters;
		protected static AnimatorControllerParameter[] VRSuyaGestureParameters;
		protected static AnimatorControllerParameter[] VRSuyaActionParameters;
		protected static AnimatorControllerParameter[] VRSuyaFXParameters;

		// 추가될 VRCSDK 데이터
		protected static List<VRCExpressionsMenu.Control> VRSuyaMenus;
		protected static VRCExpressionParameters.Parameter[] VRSuyaParameters;

		/// <summary>상속 클래스가 존재하는지 확인 한 후 해당 제품의 최종 세팅 요청을 합니다.</summary>
		internal static void RequestSetup() {
			ClearVariable();
			if (typeof(ProductSetup).IsAssignableFrom(typeof(ProductSetup_AFK))) {
				if (Array.Exists(RequestSetupVRSuyaProductList, Product => Product.ProductName == ProductName.AFK)) ProductSetup_AFK.RequestSetting();
			}
			if (typeof(ProductSetup).IsAssignableFrom(typeof(ProductSetup_Mogumogu))) {
				if (Array.Exists(RequestSetupVRSuyaProductList, Product => Product.ProductName == ProductName.Mogumogu)) ProductSetup_Mogumogu.RequestSetting();
			}
			if (typeof(ProductSetup).IsAssignableFrom(typeof(ProductSetup_Wotagei))) {
				if (Array.Exists(RequestSetupVRSuyaProductList, Product => Product.ProductName == ProductName.Wotagei)) ProductSetup_Wotagei.RequestSetting();
			}
			if (typeof(ProductSetup).IsAssignableFrom(typeof(ProductSetup_Feet))) {
				if (Array.Exists(RequestSetupVRSuyaProductList, Product => Product.ProductName == ProductName.Feet)) ProductSetup_Feet.RequestSetting();
			}
			if (typeof(ProductSetup).IsAssignableFrom(typeof(ProductSetup_ModelWalking))) {
				if (Array.Exists(RequestSetupVRSuyaProductList, Product => Product.ProductName == ProductName.ModelWalking)) ProductSetup_ModelWalking.RequestSetting();
			}
			if (typeof(ProductSetup).IsAssignableFrom(typeof(ProductSetup_SoundPad))) {
				if (Array.Exists(RequestSetupVRSuyaProductList, Product => Product.ProductName == ProductName.SoundPad)) ProductSetup_SoundPad.RequestSetting();
			}
			// 검색용 VRSuya 아이템 업데이트 위치
			if (RequestSetupVRSuyaProductList.Length > 0) {
				foreach (var RequestedAllLayers in RequestSetupVRSuyaProductList.Select(Product => Product.RequiredAnimatorLayers)) {
					foreach (KeyValuePair<VRCAvatarDescriptor.AnimLayerType, AnimatorControllerLayer[]> RequestedLayers in RequestedAllLayers) {
						switch (RequestedLayers.Key) {
							case VRCAvatarDescriptor.AnimLayerType.Base:
								VRSuyaLocomotionLayers = VRSuyaLocomotionLayers.Concat(RequestedLayers.Value).ToArray();
								break;
							case VRCAvatarDescriptor.AnimLayerType.Gesture:
								VRSuyaGestureLayers = VRSuyaGestureLayers.Concat(RequestedLayers.Value).ToArray();
								break;
							case VRCAvatarDescriptor.AnimLayerType.Action:
								VRSuyaActionLayers = VRSuyaActionLayers.Concat(RequestedLayers.Value).ToArray();
								break;
							case VRCAvatarDescriptor.AnimLayerType.FX:
								VRSuyaFXLayers = VRSuyaFXLayers.Concat(RequestedLayers.Value).ToArray();
								break;
						}
					}
				}
				foreach (var RequestedAllParameters in RequestSetupVRSuyaProductList.Select(Product => Product.RequiredAnimatorParameters)) {
					foreach (KeyValuePair<VRCAvatarDescriptor.AnimLayerType, AnimatorControllerParameter[]> RequestedParamter in RequestedAllParameters) {
						switch (RequestedParamter.Key) {
							case VRCAvatarDescriptor.AnimLayerType.Base:
								VRSuyaLocomotionParameters = VRSuyaLocomotionParameters.Concat(RequestedParamter.Value).ToArray();
								break;
							case VRCAvatarDescriptor.AnimLayerType.Gesture:
								VRSuyaGestureParameters = VRSuyaGestureParameters.Concat(RequestedParamter.Value).ToArray();
								break;
							case VRCAvatarDescriptor.AnimLayerType.Action:
								VRSuyaActionParameters = VRSuyaActionParameters.Concat(RequestedParamter.Value).ToArray();
								break;
							case VRCAvatarDescriptor.AnimLayerType.FX:
								VRSuyaFXParameters = VRSuyaFXParameters.Concat(RequestedParamter.Value).ToArray();
								break;
						}
					}
				}
				foreach (var RequestedMenus in RequestSetupVRSuyaProductList.Select(Product => Product.RequiredVRCMenus)) {
					VRSuyaMenus.AddRange(RequestedMenus);
				}
				foreach (var RequestedParameters in RequestSetupVRSuyaProductList.Select(Product => Product.RequiredVRCParameters)) {
					VRSuyaParameters = VRSuyaParameters.Concat(RequestedParameters).ToArray();
				}
			}
			bool[] RequestUnityAnimator = new bool[] { VRSuyaLocomotionLayers.Any(), VRSuyaGestureLayers.Any(), VRSuyaActionLayers.Any(), VRSuyaFXLayers.Any(),
				VRSuyaLocomotionParameters.Any(), VRSuyaGestureParameters.Any(), VRSuyaActionParameters.Any(), VRSuyaFXParameters.Any() };
			if (RequestUnityAnimator.Any(Result => Result == true)) UpdateUnityAnimator();
			if (VRSuyaParameters.Length > 0) UpdateAvatarParameters();
			if (VRSuyaMenus.Count > 0) UpdateAvatarMenus();
		}

		/// <summary>상속 클래스가 존재하는지 확인 한 후 해당 제품의 업데이트 및 등록 요청을 합니다.</summary>
		internal static void RequestProductRegister() {
			InstalledVRSuyaProducts = new VRSuyaProduct[0];
			if (typeof(ProductSetup).IsAssignableFrom(typeof(ProductSetup_AFK))) ProductSetup_AFK.RegisterProduct();
			if (typeof(ProductSetup).IsAssignableFrom(typeof(ProductSetup_Mogumogu))) ProductSetup_Mogumogu.RegisterProduct();
			if (typeof(ProductSetup).IsAssignableFrom(typeof(ProductSetup_Wotagei))) ProductSetup_Wotagei.RegisterProduct();
			if (typeof(ProductSetup).IsAssignableFrom(typeof(ProductSetup_Feet))) ProductSetup_Feet.RegisterProduct();
            if (typeof(ProductSetup).IsAssignableFrom(typeof(ProductSetup_Nyoronyoro))) ProductSetup_Nyoronyoro.RegisterProduct();
			if (typeof(ProductSetup).IsAssignableFrom(typeof(ProductSetup_ModelWalking))) ProductSetup_ModelWalking.RegisterProduct();
			if (typeof(ProductSetup).IsAssignableFrom(typeof(ProductSetup_Handmotion))) ProductSetup_Handmotion.RegisterProduct();
			if (typeof(ProductSetup).IsAssignableFrom(typeof(ProductSetup_Suyasuya))) ProductSetup_Suyasuya.RegisterProduct();
			if (typeof(ProductSetup).IsAssignableFrom(typeof(ProductSetup_SoundPad))) ProductSetup_SoundPad.RegisterProduct();
			// 검색용 VRSuya 아이템 업데이트 위치
		}

		/// <summary>아바타 하위의 GameObject들을 검사하여 VRSuya GameObject 목록을 작성합니다.</summary>
		/// <returns>VRSuya 이름이 들어간 GameObject 배열</returns>
		internal static GameObject[] GetVRSuyaGameObjects() {
			VRSuyaGameObjects = Array.FindAll(AvatarGameObject.GetComponentsInChildren<Transform>(true), transform => transform.gameObject.name.Contains("VRSuya")).Select(transform => transform.gameObject).ToArray();
			VRSuyaGameObjects = VRSuyaGameObjects.Where(gameObject => gameObject != AvatarGameObject).ToArray();
			return VRSuyaGameObjects;
		}

		/// <summary>정적 변수를 초기화 합니다.</summary>
		static void ClearVariable() {
			VRSuyaLocomotionLayers = new AnimatorControllerLayer[0];
			VRSuyaGestureLayers = new AnimatorControllerLayer[0];
			VRSuyaActionLayers = new AnimatorControllerLayer[0];
			VRSuyaFXLayers = new AnimatorControllerLayer[0];

			VRSuyaLocomotionParameters = new AnimatorControllerParameter[0];
			VRSuyaGestureParameters = new AnimatorControllerParameter[0];
			VRSuyaActionParameters = new AnimatorControllerParameter[0];
			VRSuyaFXParameters = new AnimatorControllerParameter[0];

			VRSuyaMenus = new List<VRCExpressionsMenu.Control>();
			VRSuyaParameters = new VRCExpressionParameters.Parameter[0];
		}

		/// <summary>설치된 VRSuya 제품에서 지원하는 아바타 목록을 추려 사용 가능한 아바타 목록을 만듭니다.</summary>
		internal static void UpdateInstalledAvatarList() {
			Avatar[] AllInstalledAvatars = InstalledVRSuyaProducts.SelectMany(Product => Product.SupportAvatarList).ToArray();
			if (AllInstalledAvatars.Length > 0) {
				InstalledVRSuyaProductAvatars = AllInstalledAvatars.Distinct().ToArray();
			} else {
				InstalledVRSuyaProductAvatars = new Avatar[] { Avatar.None };
			}
		}

		/// <summary>아바타의 각 VRC 애니메이터에 세팅해야 하는 값을 보냅니다.</summary>
		static void UpdateUnityAnimator() {
			AnimatorController VRCLocomotionLayer = null;
			AnimatorController VRCGestureLayer = null;
			AnimatorController VRCActionLayer = null;
			AnimatorController VRCFXLayer = null;
			if (Array.Find(AvatarVRCAvatarLayers, VRCAnimator => VRCAnimator.type == VRCAvatarDescriptor.AnimLayerType.Base).animatorController != null) {
				VRCLocomotionLayer = (AnimatorController)Array.Find(AvatarVRCAvatarLayers, VRCAnimator => VRCAnimator.type == VRCAvatarDescriptor.AnimLayerType.Base).animatorController;
			}
			if (Array.Find(AvatarVRCAvatarLayers, VRCAnimator => VRCAnimator.type == VRCAvatarDescriptor.AnimLayerType.Gesture).animatorController != null) {
				VRCGestureLayer = (AnimatorController)Array.Find(AvatarVRCAvatarLayers, VRCAnimator => VRCAnimator.type == VRCAvatarDescriptor.AnimLayerType.Gesture).animatorController;
			}
			if (Array.Find(AvatarVRCAvatarLayers, VRCAnimator => VRCAnimator.type == VRCAvatarDescriptor.AnimLayerType.Action).animatorController != null) {
				VRCActionLayer = (AnimatorController)Array.Find(AvatarVRCAvatarLayers, VRCAnimator => VRCAnimator.type == VRCAvatarDescriptor.AnimLayerType.Action).animatorController;
			}
			if (Array.Find(AvatarVRCAvatarLayers, VRCAnimator => VRCAnimator.type == VRCAvatarDescriptor.AnimLayerType.FX).animatorController != null) {
				VRCFXLayer = (AnimatorController)Array.Find(AvatarVRCAvatarLayers, VRCAnimator => VRCAnimator.type == VRCAvatarDescriptor.AnimLayerType.FX).animatorController;
			}
			if (VRCLocomotionLayer) UpdateTargetAnimatorController(VRCLocomotionLayer, VRSuyaLocomotionLayers, VRSuyaLocomotionParameters);
			if (VRCGestureLayer) UpdateTargetAnimatorController(VRCGestureLayer, VRSuyaGestureLayers, VRSuyaGestureParameters);
			if (VRCActionLayer) UpdateTargetAnimatorController(VRCActionLayer, VRSuyaActionLayers, VRSuyaActionParameters);
			if (VRCFXLayer) UpdateTargetAnimatorController(VRCFXLayer, VRSuyaFXLayers, VRSuyaFXParameters);
		}

		/// <summary>세팅해야 하는 애니메이터 컨트롤러에 파라메터와 레이어가 존재하는지 확인 후 추가합니다.</summary>
		static void UpdateTargetAnimatorController(AnimatorController TargetController, AnimatorControllerLayer[] TargetLayers, AnimatorControllerParameter[] TargetParameters) {
			foreach (AnimatorControllerParameter NewParameter in TargetParameters) {
				if (!Array.Exists(TargetController.parameters, ExistParameter => NewParameter.name == ExistParameter.name)) {
					Undo.RecordObject(TargetController, "Added Unity Animator Controller Parameter");
					if (KeepLinkAnimatorLayer) {
						TargetController.parameters = TargetController.parameters.Concat(new AnimatorControllerParameter[] { NewParameter }).ToArray();
					} else {
						Animator AnimatorInstance = new Animator();
						AnimatorInstance.AddParameter(TargetController, NewParameter);
					}
					EditorUtility.SetDirty(TargetController);
					Undo.CollapseUndoOperations(UndoGroupIndex);
				}
			}
			if (KeepLinkAnimatorLayer) {
				foreach (AnimatorControllerLayer NewLayer in TargetLayers) {
					if (!Array.Exists(TargetController.layers, ExistLayer => NewLayer.name == ExistLayer.name)) {
						Undo.RecordObject(TargetController, "Added Unity Animator Controller Layer");
						TargetController.layers = TargetController.layers.Concat(new AnimatorControllerLayer[] { NewLayer }).ToArray();
						EditorUtility.SetDirty(TargetController);
						Undo.CollapseUndoOperations(UndoGroupIndex);
					}
				}
			} else {
				AnimatorControllerLayer[] RequiredLayers = TargetLayers.Where(TargetLayer => !Array.Exists(TargetController.layers, ExistLayer => TargetLayer.name == ExistLayer.name)).ToArray();
				if (RequiredLayers.Length > 0) {
					Animator AnimatorInstance = new Animator();
					Undo.RecordObject(TargetController, "Added Unity Animator Controller Layer");
					AnimatorControllerLayer[] newAnimatorLayers = AnimatorInstance.DuplicateAnimatorLayers(TargetController, RequiredLayers);
					TargetController.layers = newAnimatorLayers;
					EditorUtility.SetDirty(TargetController);
					Undo.CollapseUndoOperations(UndoGroupIndex);
				}
			}
		}

		/// <summary>세팅해야 하는 파라메터 큐를 아바타 파라메터에 존재하는지 확인 후 추가합니다.</summary>
		static void UpdateAvatarParameters() {
            foreach (VRCExpressionParameters.Parameter NewParameter in VRSuyaParameters) {
                if (!Array.Exists(AvatarVRCParameter.parameters, ExistParameter => ExistParameter.name == NewParameter.name)) {
					Undo.RecordObject(AvatarVRCParameter, "Added VRC Parameter");
					AvatarVRCParameter.parameters = AvatarVRCParameter.parameters.Concat(new VRCExpressionParameters.Parameter[] { NewParameter }).ToArray();
					Undo.CollapseUndoOperations(UndoGroupIndex);
				}
            }
			EditorUtility.SetDirty(AvatarVRCParameter);
        }

		/// <summary>세팅해야 하는 메뉴 큐를 아바타 메뉴에 존재하는지 확인 후 추가합니다.</summary>
		static void UpdateAvatarMenus() {
            foreach (VRCExpressionsMenu.Control NewMenu in VRSuyaMenus) {
                if (!AvatarVRCMenu.controls.Exists(ExistMenu => ExistMenu.subMenu == NewMenu.subMenu)) {
					Undo.RecordObject(AvatarVRCMenu, "Added VRC Menu");
					AvatarVRCMenu.controls.Add(NewMenu);
					Undo.CollapseUndoOperations(UndoGroupIndex);
				}
			}
			EditorUtility.SetDirty(AvatarVRCMenu);
		}

		/// <summary>요청한 GameObject의 Transform을 Origin에 맞춰주는 메소드</summary>
		internal static void TransformPrefab(GameObject TargetGameObject, GameObject BaseGameObject, bool KeepScale) {
			TargetGameObject.transform.localPosition = new Vector3(0, 0, 0);
			TargetGameObject.transform.localRotation = new Quaternion(0, 0, 0, 0);
			if (!KeepScale) {
				TargetGameObject.transform.localScale = BaseGameObject.transform.localScale;
			} else {
				TargetGameObject.transform.localScale = new Vector3(1, 1, 1);
			}
		}
	}
}
#endif