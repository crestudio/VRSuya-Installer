#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using UnityEditor;
using UnityEditor.Animations;

using VRC.SDK3.Avatars.Components;
using VRC.SDK3.Avatars.ScriptableObjects;

/*
 * VRSuya Avatar Setting Updater
 * Contact : vrsuya@gmail.com // Twitter : https://twitter.com/VRSuya
 */

namespace com.vrsuya.installer {

	[ExecuteInEditMode]
	[AddComponentMenu("")]
	public class AssetManager : ProductSetup {

		private static readonly Dictionary<ProductName, string> dictProductGUID = new Dictionary<ProductName, string>() {
			{ ProductName.AFK, "cddaae54ebed24fd4ba3b6ed60c355ea" },
			{ ProductName.Mogumogu, "29f78992daf6d7548bec811c4ff3cfd6" },
			{ ProductName.Wotagei, "201253d8e16814cbfa1ddbdea4d2f030" },
			{ ProductName.Feet, "b3267c74ea5c8274d88f728b4c84d10d" },
			{ ProductName.Nyoronyoro, "699ae58fc7b34ba4388d4ec8cf6e9d62" },
			{ ProductName.ModelWalking, "d797d8857806b2f4cb4ff83df63c4c2b" },
			{ ProductName.Handmotion, "9f09dddbe7e80e54aa1e8565fb510b9d" },
			{ ProductName.Suyasuya, "93e06b6d83f9f48f1b06cff09857ca25" },
			{ ProductName.SoundPad, "3a1ae7adbb14c05499d750ca56efa6ba" }
			// 검색용 업데이트 위치
		};

		private static readonly Dictionary<ProductName, string> dictProductPath = new Dictionary<ProductName, string>() {
			{ ProductName.AFK, "Assets/VRSuya/AFK" },
			{ ProductName.Mogumogu, "Assets/VRSuya/Mogumogu" },
			{ ProductName.Wotagei, "Assets/VRSuya/Wotagei" },
			{ ProductName.Feet, "Assets/VRSuya/HopeskyD/Feet" },
			{ ProductName.Nyoronyoro, "Assets/VRSuya/Nyoronyoro" },
			{ ProductName.ModelWalking, "Assets/VRSuya/ModelWalking" },
			{ ProductName.Handmotion, "Assets/VRSuya/Handmotion" },
			{ ProductName.Suyasuya, "Assets/VRSuya/Suyasuya" },
			{ ProductName.SoundPad, "Assets/VRSuya/SoundPad" }
			// 검색용 업데이트 위치
		};

		private static readonly Dictionary<ProductName, string> dictPresentMenuFileName = new Dictionary<ProductName, string>() {
			{ ProductName.AFK, "VRSuya_AFK_Menu{0}.asset" },
			{ ProductName.Mogumogu, "VRSuya_Mogumogu_Menu{0}.asset" },
			{ ProductName.Wotagei, "VRSuya_Wotagei_Menu{0}.asset" },
			{ ProductName.Feet, "VRSuya_HopeskyD_Feet_Menu{0}.asset" },
			{ ProductName.ModelWalking, "VRSuya_ModelWalking_Menu{0}.asset" },
			{ ProductName.Handmotion, "VRSuya_Handmotion_Menu{0}.asset" },
			{ ProductName.Suyasuya, "VRSuya_Suyasuya_Menu{0}.asset" },
			{ ProductName.SoundPad, "VRSuya_SoundPad_Menu{0}.asset" }
			// 검색용 업데이트 위치
		};

		private static Dictionary<VRCAssetType, string> dictVRCSDKAssetGUID = new Dictionary<VRCAssetType, string>() {
			{ VRCAssetType.VRSuya, "00679ffab5ad14d42afccca44034c525" },
			{ VRCAssetType.Template, "00679ffab5ad14d42afccca44034c525" },
			{ VRCAssetType.Export, "13684ec2ba89160419ef0d32a11968cd" },
			{ VRCAssetType.Locomotion, "4239c7ee49e2a664bbbf793ea643905b" },
			{ VRCAssetType.Gesture, "198a3bfd1d4bc7e4f8c4f6a30c8eaf84" },
			{ VRCAssetType.Action, "3139f3f76b6fbd840a86c6c5550c826d" },
			{ VRCAssetType.FX, "62d9e950f357d0948a865df86e1e5206" },
			{ VRCAssetType.Menu, "6e83d70ca00491a45828d1f8b52e871d" },
			{ VRCAssetType.Parameter, "dd63989bd535d7842bf14e5edda08e6f" }
		};

		private static Dictionary<VRCAssetType, string> dictVRCSDKAssetFilePath = new Dictionary<VRCAssetType, string>() {
			{ VRCAssetType.VRSuya, "Assets/VRSuya" },
			{ VRCAssetType.Template, "Packages/com.vrsuya.avatarsettingupdater/Controller" },
			{ VRCAssetType.Export, "Assets/VRSuya/Export" },
			{ VRCAssetType.Locomotion, "VRSuya_Default_LocomotionLayer.controller" },
			{ VRCAssetType.Gesture, "VRSuya_Default_GestureLayer.controller" },
			{ VRCAssetType.Action, "VRSuya_Default_ActionLayer.controller" },
			{ VRCAssetType.FX, "VRSuya_Default_FXLayer.controller" },
			{ VRCAssetType.Menu, "VRSuya_Default_Menu.asset" },
			{ VRCAssetType.Parameter, "VRSuya_Default_Parameter.asset" }
		};

		private static readonly Dictionary<string, string> dictMasterActionLayer = new Dictionary<string, string>() {
			{ "GUID", "0cea068b6a31b5c4f98bfeba0f8bc11d" },
			{ "AssetFileName", "VRSuya_Wotagei_ActionLayer_AFK.controller" }
		};

		private static readonly string[] dictAnimatorControllerName = new string[] { "LocomotionLayer", "GestureLayer", "ActionLayer", "FXLayer" };

		private static readonly string[] dictIgnoreLayerName = new string[] { "Base Layer", "AllParts", "Left Hand", "Right Hand", "Locomotion" };

		private static readonly string[] dictIgnorePrefabName = new string[] { "Cyalume", "LightStick", "Guide", "Sphere", "Transparent", "Particle", "Modular" };

		private static readonly string dictDefaultFilename = "Default";

		/// <summary>요청한 타입의 VRSuya 제품의 상세 내용을 업데이트하여 반환합니다.</summary>
		/// <returns>내용이 업데이트 된 VRSuyaProduct 오브젝트</returns>
		internal static VRSuyaProduct UpdateProductInformation(ProductName TypeProduct) {
			VRSuyaProduct RequestedVRSuyaProduct = new VRSuyaProduct();

			// 검색 경로 지정
			string SearchPath = AssetDatabase.GUIDToAssetPath(dictProductGUID[TypeProduct]);
			if (string.IsNullOrEmpty(SearchPath)) SearchPath = dictProductPath[TypeProduct];

			// 제품 데이터 등록
			RequestedVRSuyaProduct.ProductName = TypeProduct;
			RequestedVRSuyaProduct.AnimationControllerGUID = AssetDatabase.FindAssets("t:AnimatorController", new[] { SearchPath });
			RequestedVRSuyaProduct.MenuGUID = AssetDatabase.FindAssets("Menu", new[] { SearchPath });
			RequestedVRSuyaProduct.ParameterGUID = AssetDatabase.FindAssets("Parameter", new[] { SearchPath });
			RequestedVRSuyaProduct.PrefabGUID = AssetDatabase.FindAssets("t:Prefab", new[] { SearchPath });

			Dictionary<VRCAvatarDescriptor.AnimLayerType, string> AvatarAnimatorControllerGUID = GetRequestAvatarTypeAnimatorControllerGUID(SearchPath, AvatarType);

			if (AvatarAnimatorControllerGUID.ContainsKey(VRCAvatarDescriptor.AnimLayerType.Base)) {
				RequestedVRSuyaProduct.LocomotionAnimatorGUID = AvatarAnimatorControllerGUID[VRCAvatarDescriptor.AnimLayerType.Base];
			}
			if (AvatarAnimatorControllerGUID.ContainsKey(VRCAvatarDescriptor.AnimLayerType.Action)) {
				RequestedVRSuyaProduct.ActionAnimatorGUID = AvatarAnimatorControllerGUID[VRCAvatarDescriptor.AnimLayerType.Action];
			}

			RequestedVRSuyaProduct.RequiredAnimatorLayers = ResolveAnimationControllerLayer(AvatarAnimatorControllerGUID);
			RequestedVRSuyaProduct.RequiredAnimatorParameters = ResolveAnimationControllerParameter(AvatarAnimatorControllerGUID);
			RequestedVRSuyaProduct.RequiredVRCMenus = ResolveMenu(RequestedVRSuyaProduct.MenuGUID, TypeProduct);
			RequestedVRSuyaProduct.RequiredVRCParameters = ResolveParameter(RequestedVRSuyaProduct.ParameterGUID);
			RequestedVRSuyaProduct.RequiredVRCMemoryCount = ResolveParameterCost(RequestedVRSuyaProduct.ParameterGUID);
			RequestedVRSuyaProduct.SupportAvatarList = FindAllAvatarNames(TypeProduct, RequestedVRSuyaProduct.PrefabGUID);

			return RequestedVRSuyaProduct;
		}

		/// <summary>파일 목록에서 요청한 아바타 타입의 애니메이터 레이어 GUID 목록을 반환합니다.</summary>
		/// <returns>해당 아바타의 애니메이터 컨트롤러 배열</returns>
		private static Dictionary<VRCAvatarDescriptor.AnimLayerType, string> GetRequestAvatarTypeAnimatorControllerGUID(string SearchPath, Avatar AvatarType) {
			Dictionary<VRCAvatarDescriptor.AnimLayerType, string> AnimatorGUID = new Dictionary<VRCAvatarDescriptor.AnimLayerType, string>();
			foreach (string TargetVRCAnimatorType in dictAnimatorControllerName) {
				string TargetAvatarType = "";
				if (AvatarType != Avatar.General) TargetAvatarType = AvatarType.ToString();
				string[] TargetVRCAnimatorGUID = AssetDatabase.FindAssets(TargetVRCAnimatorType + " t:AnimatorController " + TargetAvatarType, new[] { SearchPath });
				if (TargetVRCAnimatorGUID.Length == 0) TargetVRCAnimatorGUID = AssetDatabase.FindAssets(TargetVRCAnimatorType + " t:AnimatorController", new[] { SearchPath });
				if (TargetVRCAnimatorGUID.Length > 0) {
					VRCAvatarDescriptor.AnimLayerType TargetControllerType = VRCAvatarDescriptor.AnimLayerType.FX;
					switch (TargetVRCAnimatorType) {
						case "LocomotionLayer":
							TargetControllerType = VRCAvatarDescriptor.AnimLayerType.Base;
							break;
						case "GestureLayer":
							TargetControllerType = VRCAvatarDescriptor.AnimLayerType.Gesture;
							break;
						case "ActionLayer":
							TargetControllerType = VRCAvatarDescriptor.AnimLayerType.Action;
							break;
						case "FXLayer":
							TargetControllerType = VRCAvatarDescriptor.AnimLayerType.FX;
							break;
					}
					AnimatorGUID.Add(TargetControllerType, TargetVRCAnimatorGUID[0]);
				}
			}
			return AnimatorGUID;
		}

		/// <summary>파일 목록에서 요청한 아바타 타입의 애니메이터 레이어 목록을 반환합니다.</summary>
		/// <returns>Unity 애니메이터 레이어 배열</returns>
		private static Dictionary<VRCAvatarDescriptor.AnimLayerType, AnimatorControllerLayer[]> ResolveAnimationControllerLayer(Dictionary<VRCAvatarDescriptor.AnimLayerType, string> AnimatorGUID) {
			Dictionary<VRCAvatarDescriptor.AnimLayerType, AnimatorControllerLayer[]> AnimatorLayerInAssets = new Dictionary<VRCAvatarDescriptor.AnimLayerType, AnimatorControllerLayer[]>();
			foreach (KeyValuePair<VRCAvatarDescriptor.AnimLayerType, string> AnimatorLayer in AnimatorGUID) {
				AnimatorController TargetController = AssetDatabase.LoadAssetAtPath<AnimatorController>(AssetDatabase.GUIDToAssetPath(AnimatorLayer.Value));
				if (TargetController) {
					AnimatorControllerLayer[] TargetControllerLayers = new AnimatorControllerLayer[0];
					foreach (AnimatorControllerLayer Layer in TargetController.layers) {
						if (!Array.Exists(dictIgnoreLayerName, LayerName => LayerName == Layer.name)) {
							TargetControllerLayers = TargetControllerLayers.Concat(new AnimatorControllerLayer[] { Layer }).ToArray();
						}
					}
					AnimatorLayerInAssets.Add(AnimatorLayer.Key, TargetControllerLayers);
				}
			}
			return AnimatorLayerInAssets;
		}

		/// <summary>파일 목록에서 요청한 아바타 타입의 애니메이터 파라메터 목록을 반환합니다.</summary>
		/// <returns>Unity 애니메이터 파라메터 배열</returns>
		private static Dictionary<VRCAvatarDescriptor.AnimLayerType, AnimatorControllerParameter[]> ResolveAnimationControllerParameter(Dictionary<VRCAvatarDescriptor.AnimLayerType, string> AnimatorGUID) {
			Dictionary<VRCAvatarDescriptor.AnimLayerType, AnimatorControllerParameter[]> AnimatorParameterInAssets = new Dictionary<VRCAvatarDescriptor.AnimLayerType, AnimatorControllerParameter[]>();
			foreach (KeyValuePair<VRCAvatarDescriptor.AnimLayerType, string> AnimatorLayer in AnimatorGUID) {
				AnimatorController TargetController = AssetDatabase.LoadAssetAtPath<AnimatorController>(AssetDatabase.GUIDToAssetPath(AnimatorLayer.Value));
				if (TargetController) {
					AnimatorControllerParameter[] TargetControllerParameters = new AnimatorControllerParameter[0];
					foreach (AnimatorControllerParameter Parameter in TargetController.parameters) {
						TargetControllerParameters = TargetControllerParameters.Concat(new AnimatorControllerParameter[] { Parameter }).ToArray();
					}
					AnimatorParameterInAssets.Add(AnimatorLayer.Key, TargetControllerParameters);
				}
			}
			return AnimatorParameterInAssets;
		}

		/// <summary>요청한 파일 목록에서 메뉴 목록을 반환합니다.</summary>
		/// <returns>VRC 메뉴 리스트</returns>
		private static List<VRCExpressionsMenu.Control> ResolveMenu(string[] AssetsGUID, ProductName TypeProduct) {
			List<VRCExpressionsMenu.Control> MenusInAssets = new List<VRCExpressionsMenu.Control>();
			foreach (string AssetGUID in AssetsGUID) {
				if (GUIDToAssetName(AssetGUID, false).Contains(string.Format(dictPresentMenuFileName[TypeProduct], TargetLanguage))) {
					VRCExpressionsMenu MenuFile = AssetDatabase.LoadAssetAtPath<VRCExpressionsMenu>(AssetDatabase.GUIDToAssetPath(AssetGUID));
					if (MenuFile) MenusInAssets.AddRange(MenuFile.controls);
				} else if (GUIDToAssetName(AssetGUID, false).Contains(string.Format(dictPresentMenuFileName[TypeProduct], string.Empty))) {
					VRCExpressionsMenu MenuFile = AssetDatabase.LoadAssetAtPath<VRCExpressionsMenu>(AssetDatabase.GUIDToAssetPath(AssetGUID));
					if (MenuFile) MenusInAssets.AddRange(MenuFile.controls);
				}
			}
			return MenusInAssets;
		}

		/// <summary>요청한 파일 목록에서 파라메터 목록을 반환합니다.</summary>
		/// <returns>VRC 파라메터 배열</returns>
		private static VRCExpressionParameters.Parameter[] ResolveParameter(string[] AssetsGUID) {
			VRCExpressionParameters.Parameter[] ParametersInAssets = new VRCExpressionParameters.Parameter[0];
			foreach (string AssetGUID in AssetsGUID) {
				VRCExpressionParameters ParameterFile = AssetDatabase.LoadAssetAtPath<VRCExpressionParameters>(AssetDatabase.GUIDToAssetPath(AssetGUID));
				if (ParameterFile) ParametersInAssets = ParametersInAssets.Concat(ParameterFile.parameters).ToArray();
			}
			return ParametersInAssets;
		}

		/// <summary>요청한 파일 목록에서 등록하는데 필요한 파라메터 총 메모리를 반환합니다.</summary>
		/// <returns>필요한 총 파라메터 메모리 정수형</returns>
		private static int ResolveParameterCost(string[] AssetsGUID) {
			int RequiredParameterCost = 0;
			foreach (string AssetGUID in AssetsGUID) {
				VRCExpressionParameters ParameterFile = AssetDatabase.LoadAssetAtPath<VRCExpressionParameters>(AssetDatabase.GUIDToAssetPath(AssetGUID));
				if (ParameterFile) RequiredParameterCost = RequiredParameterCost + ParameterFile.CalcTotalCost();
			}
			return RequiredParameterCost;
		}

		/// <summary>요청한 파일 목록에서 지정된 이름 뒤에 위치한 아바타 이름을 분석하여 Avatar 형태로 반환합니다.</summary>
		/// <returns>Avatar Enum 배열</returns>
		private static Avatar[] FindAllAvatarNames(ProductName TypeProduct, string[] AssetsGUID) {
			Avatar[] AvatarNames = new Avatar[0];
			string SearchWord = TypeProduct.ToString() + "_";
			switch (TypeProduct) {
				case ProductName.AFK:
					SearchWord = "Prefab_";
					break;
				case ProductName.Mogumogu:
					SearchWord = "PhysBone_";
					break;
			}
			foreach (string AssetGUID in AssetsGUID) {
				string AssetName = GUIDToAssetName(AssetGUID, true);
				if (AssetName.Contains(SearchWord)) {
					string AvatarName = AssetName.Substring(AssetName.IndexOf(SearchWord) + SearchWord.Length);
					if (Array.Exists(dictIgnorePrefabName, Name => AvatarName == Name)) continue;
					Avatar AvatarType;
					if (Enum.TryParse<Avatar>(AvatarName, out AvatarType)) AvatarNames = AvatarNames.Concat(new Avatar[] { AvatarType }).ToArray();
					// if (!Enum.TryParse<Avatar>(AvatarName, out AvatarType)) Debug.LogError($"[VRSuya] Prefab 아바타 이름 오류 : {AssetDatabase.GUIDToAssetPath(AssetGUID)}");
				}
			}

			// 제품 추가시 추가해야 될 변수
			if (TypeProduct == ProductName.AFK || 
				TypeProduct == ProductName.Wotagei) {
				if (AssetsGUID.Length > 0) AvatarNames = AvatarNames.Concat(new Avatar[] { Avatar.General }).ToArray();
			} else if (TypeProduct == ProductName.Nyoronyoro || 
				TypeProduct == ProductName.ModelWalking || 
				TypeProduct == ProductName.Handmotion || 
				TypeProduct == ProductName.Suyasuya ||
				TypeProduct == ProductName.SoundPad) {
				// 검색용 업데이트 위치
				if (AssetsGUID.Length > 0) AvatarNames = (Avatar[])Enum.GetValues(typeof(Avatar));
            }
			AvatarNames = AvatarNames.Distinct().ToArray();

			// General 아바타를 Array 맨 처음으로 이동
			int TargetIndex = Array.IndexOf(AvatarNames, Avatar.General);
			if (TargetIndex != -1) {
				Avatar ArrayFirst = AvatarNames[TargetIndex];
				for (int Index = TargetIndex; Index > 0; Index--) {
					AvatarNames[Index] = AvatarNames[Index - 1];
				}
				AvatarNames[0] = ArrayFirst;
			}
			return AvatarNames;
		}

		/// <summary>VRC용 에셋 목록을 점검하여 설치 제품에서 요구하는 경우 설치 시도 및 아바타에 설정합니다.</summary>
		/// <returns>VRC용 에셋 설치 성공 여부</returns>
		internal static bool CheckVRCAssets() {
			bool Result = true;
			VRCAvatarDescriptor.AnimLayerType[] RequestAnimatorType = RequestSetupVRSuyaProductList.SelectMany(RequestProduct => RequestProduct.RequiredAnimatorLayers.Select(RequiredAnimator => RequiredAnimator.Key)).ToArray();
			int RequestMenuLength = RequestSetupVRSuyaProductList.Select(RequestProduct => RequestProduct.RequiredVRCMenus).Count();
			int RequestParameterLength = RequestSetupVRSuyaProductList.Select(RequestProduct => RequestProduct.RequiredVRCParameters).Count();
			if (RequestAnimatorType != null) {
				RequestAnimatorType = RequestAnimatorType.Distinct().ToArray();
				foreach (VRCAvatarDescriptor.AnimLayerType AnimatorType in RequestAnimatorType) {
					Result = CheckTargetAnimatorControllerAsset(AnimatorType);
				}
			}
			if (RequestMenuLength > 0) {
				if (AvatarVRCMenu == null) {
					string AssetGUID = CreateVRCAsset(VRCAssetType.Menu);
					if (!string.IsNullOrEmpty(AssetGUID)) {
						Undo.RecordObject(AvatarVRCAvatarDescriptor, "Added new VRC Menu");
						if (!AvatarVRCAvatarDescriptor.customExpressions) AvatarVRCAvatarDescriptor.customExpressions = true;
						AvatarVRCAvatarDescriptor.expressionsMenu = AssetDatabase.LoadAssetAtPath<VRCExpressionsMenu>(AssetDatabase.GUIDToAssetPath(AssetGUID));
						AvatarVRCMenu = AvatarVRCAvatarDescriptor.expressionsMenu;
						EditorUtility.SetDirty(AvatarVRCAvatarDescriptor);
						Undo.CollapseUndoOperations(UndoGroupIndex);
					} else {
						Result = false;
					}
				}
			}
			if (RequestParameterLength > 0) {
				if (AvatarVRCParameter == null) {
					string AssetGUID = CreateVRCAsset(VRCAssetType.Parameter);
					if (!string.IsNullOrEmpty(AssetGUID)) {
						Undo.RecordObject(AvatarVRCAvatarDescriptor, "Added new VRC Parameter");
						if (!AvatarVRCAvatarDescriptor.customExpressions) AvatarVRCAvatarDescriptor.customExpressions = true;
						AvatarVRCAvatarDescriptor.expressionParameters = AssetDatabase.LoadAssetAtPath<VRCExpressionParameters>(AssetDatabase.GUIDToAssetPath(AssetGUID));
						AvatarVRCParameter = AvatarVRCAvatarDescriptor.expressionParameters;
						EditorUtility.SetDirty(AvatarVRCAvatarDescriptor);
						Undo.CollapseUndoOperations(UndoGroupIndex);
					} else {
						Result = false;
					}
				}
			}
			return Result;
		}

		/// <summary>요청한 애니메이터 컨트롤러 파트를 점검하고 에셋을 생성 시도 및 아바타에 설정합니다.</summary>
		/// <returns>요청한 애니메이터 컨트롤러 에셋 생성 성공 여부</returns>
		private static bool CheckTargetAnimatorControllerAsset(VRCAvatarDescriptor.AnimLayerType TargetType) {
			VRCAssetType TargetAssetType;
			bool Result = true;
			switch (TargetType) {
				case VRCAvatarDescriptor.AnimLayerType.Base:
					TargetAssetType = VRCAssetType.Locomotion;
					if (!KeepAnimatorController) PreallocateAnimatorController(TargetType);
					break;
				case VRCAvatarDescriptor.AnimLayerType.Gesture:
					TargetAssetType = VRCAssetType.Gesture;
					break;
				case VRCAvatarDescriptor.AnimLayerType.Action:
					TargetAssetType = VRCAssetType.Action;
					if (!KeepAnimatorController) PreallocateAnimatorController(TargetType);
					break;
				case VRCAvatarDescriptor.AnimLayerType.FX:
					TargetAssetType = VRCAssetType.FX;
					break;
				default:
					TargetAssetType = VRCAssetType.NULL;
					break;
			}
			if (Array.Find(AvatarVRCAvatarLayers, VRCLayer => TargetType == VRCLayer.type).animatorController == null) {
				string AssetGUID = CreateVRCAsset(TargetAssetType);
				if (string.IsNullOrEmpty(AssetGUID)) {
					Result = false;
				} else {
					Undo.RecordObject(AvatarVRCAvatarDescriptor, "Changed VRC Layer");
					int Index = Array.FindIndex(AvatarVRCAvatarLayers, Layer => TargetType == Layer.type);
					AnimatorController TargetAnimationController = AssetDatabase.LoadAssetAtPath<AnimatorController>(AssetDatabase.GUIDToAssetPath(AssetGUID));
					if (!AvatarVRCAvatarDescriptor.customizeAnimationLayers) AvatarVRCAvatarDescriptor.customizeAnimationLayers = true;
					if (AvatarVRCAvatarLayers[Index].isDefault == true) {
						VRCAvatarDescriptor.CustomAnimLayer newCustomAnimLayer;
						newCustomAnimLayer.isEnabled = false;
						newCustomAnimLayer.type = TargetType;
						newCustomAnimLayer.animatorController = TargetAnimationController;
						newCustomAnimLayer.mask = TargetAnimationController.layers[0].avatarMask;
						newCustomAnimLayer.isDefault = false;
						AvatarVRCAvatarLayers[Index] = newCustomAnimLayer;
						EditorUtility.SetDirty(AvatarVRCAvatarDescriptor);
					} else {
						if (AvatarVRCAvatarLayers[Index].animatorController != null) {
							if (AvatarVRCAvatarLayers[Index].animatorController.name != TargetAnimationController.name) {
								AvatarVRCAvatarLayers[Index].animatorController = TargetAnimationController;
								EditorUtility.SetDirty(AvatarVRCAvatarDescriptor);
							}
						} else {
							AvatarVRCAvatarLayers[Index].animatorController = TargetAnimationController;
							EditorUtility.SetDirty(AvatarVRCAvatarDescriptor);
						}
					}
					Undo.CollapseUndoOperations(UndoGroupIndex);
				}
			}
			return Result;
		}

		/// <summary>요청한 타입의 애니메이터 컨트롤러를 아바타에 사전 할당을 합니다.</summary>
		private static void PreallocateAnimatorController(VRCAvatarDescriptor.AnimLayerType TargetType) {
			string AssetGUID = "";
			switch (TargetType) {
				case VRCAvatarDescriptor.AnimLayerType.Base:
					if (InstalledProductNyoronyoro && InstallProductNyoronyoro) {
						AssetGUID = Array.Find(RequestSetupVRSuyaProductList, Product => Product.ProductName == ProductName.Nyoronyoro).LocomotionAnimatorGUID;
					} else if (InstalledProductWotagei && InstallProductWotagei) {
                        AssetGUID = Array.Find(RequestSetupVRSuyaProductList, Product => Product.ProductName == ProductName.Wotagei).LocomotionAnimatorGUID;
                    }
					break;
				case VRCAvatarDescriptor.AnimLayerType.Action:
					if (InstalledProductModelWalking && InstallProductModelWalking) {
						AssetGUID = Array.Find(RequestSetupVRSuyaProductList, Product => Product.ProductName == ProductName.ModelWalking).ActionAnimatorGUID;
						break;
					}
					if (InstalledProductAFK && InstallProductAFK && InstalledProductWotagei && InstallProductWotagei) {
						AssetGUID = GetMasterActionAnimatorController();
						break;
					} else if (InstalledProductAFK && InstallProductAFK) {
						AssetGUID = Array.Find(RequestSetupVRSuyaProductList, Product => Product.ProductName == ProductName.AFK).ActionAnimatorGUID;
					} else if (InstalledProductWotagei && InstallProductWotagei) {
						AssetGUID = Array.Find(RequestSetupVRSuyaProductList, Product => Product.ProductName == ProductName.Wotagei).ActionAnimatorGUID;
					}
					break;
			}
			if (!string.IsNullOrEmpty(AssetGUID)) {
				Undo.RecordObject(AvatarVRCAvatarDescriptor, "Changed VRC Layer");
				int Index = Array.FindIndex(AvatarVRCAvatarLayers, Layer => TargetType == Layer.type);
				AnimatorController TargetAnimationController = AssetDatabase.LoadAssetAtPath<AnimatorController>(AssetDatabase.GUIDToAssetPath(AssetGUID));
				if (AvatarVRCAvatarLayers[Index].isDefault == true) {
					VRCAvatarDescriptor.CustomAnimLayer newCustomAnimLayer;
					newCustomAnimLayer.isEnabled = false;
					newCustomAnimLayer.type = TargetType;
					newCustomAnimLayer.animatorController = TargetAnimationController;
					newCustomAnimLayer.mask = TargetAnimationController.layers[0].avatarMask;
					newCustomAnimLayer.isDefault = false;
					AvatarVRCAvatarLayers[Index] = newCustomAnimLayer;
					EditorUtility.SetDirty(AvatarVRCAvatarDescriptor);
				} else {
					if (AvatarVRCAvatarLayers[Index].animatorController != null) {
						if (AvatarVRCAvatarLayers[Index].animatorController.name != TargetAnimationController.name) {
							AvatarVRCAvatarLayers[Index].animatorController = TargetAnimationController;
							EditorUtility.SetDirty(AvatarVRCAvatarDescriptor);
						}
					} else {
						AvatarVRCAvatarLayers[Index].animatorController = TargetAnimationController;
						EditorUtility.SetDirty(AvatarVRCAvatarDescriptor);
					}
				}
				Undo.CollapseUndoOperations(UndoGroupIndex);
			}
			return;
		}

		/// <summary>요청한 에셋을 Export 폴더에 아바타 이름으로 복사합니다.</summary>
		/// <returns>복사된 에셋의 GUID</returns>
		private static string CreateVRCAsset(VRCAssetType TargetType) {
			string SourceAssetPath = AssetDatabase.GUIDToAssetPath(dictVRCSDKAssetGUID[TargetType]);
			if (string.IsNullOrEmpty(SourceAssetPath)) SourceAssetPath = dictVRCSDKAssetFilePath[TargetType];
			string DestinationAssetPath = AssetDatabase.GUIDToAssetPath(dictVRCSDKAssetGUID[VRCAssetType.Export]);
			if (string.IsNullOrEmpty(DestinationAssetPath)) DestinationAssetPath = dictVRCSDKAssetFilePath[VRCAssetType.Export];
			string CopiedAssetGUID = "";

			if (!string.IsNullOrEmpty(AssetDatabase.AssetPathToGUID(SourceAssetPath))) {
				string OriginalFileName = SourceAssetPath.Split('/')[SourceAssetPath.Split('/').Length - 1];
				string ExportFileName = OriginalFileName.Replace(dictDefaultFilename, AvatarGameObject.name);
				string FinalDestinationAssetPath = DestinationAssetPath + "/" + ExportFileName;
				object ReadFile = null;
				switch (TargetType) {
					case VRCAssetType.Locomotion:
					case VRCAssetType.Gesture:
					case VRCAssetType.Action:
					case VRCAssetType.FX:
						ReadFile = AssetDatabase.LoadAssetAtPath<AnimatorController>(FinalDestinationAssetPath);
						break;
					case VRCAssetType.Menu:
						ReadFile = AssetDatabase.LoadAssetAtPath<VRCExpressionsMenu>(FinalDestinationAssetPath);
						break;
					case VRCAssetType.Parameter:
						ReadFile = AssetDatabase.LoadAssetAtPath<VRCExpressionParameters>(FinalDestinationAssetPath);
						break;
				}
				if (ReadFile == null) {
					FileUtil.CopyFileOrDirectory(SourceAssetPath, FinalDestinationAssetPath);
					AssetDatabase.Refresh();
				}
				CopiedAssetGUID = AssetDatabase.AssetPathToGUID(FinalDestinationAssetPath);
			} else {
				StatusCode = "NO_SOURCE_FILE";
			}
			return CopiedAssetGUID;
		}

		/// <summary>AFK + Wotagei ActionLayer 파일이 존재하는지 확인 후 올바른 GUID를 반환합니다.</summary>
		/// <returns>AFK + Wotagei ActionLayer 파일의 GUID</returns>
		private static string GetMasterActionAnimatorController() {
			string AssetGUID = dictMasterActionLayer["GUID"];
			string AssetPath = AssetDatabase.GUIDToAssetPath(AssetGUID);
			if (string.IsNullOrEmpty(AssetPath)) {
				string SearchPath = AssetDatabase.GUIDToAssetPath(dictProductGUID[ProductName.Wotagei]);
				if (string.IsNullOrEmpty(SearchPath)) SearchPath = dictProductPath[ProductName.Wotagei];
				string[] ResultGUID = AssetDatabase.FindAssets(dictMasterActionLayer["AssetFileName"] + " t:AnimatorController", new[] { SearchPath });
				if (ResultGUID != null) {
					AssetGUID = ResultGUID[0];
				} else {
					AssetGUID = "";
				}
			}
			return AssetGUID;
		}

		/// <summary>Export 폴더를 검사하여 존재하는지 확인 후, 없는 경우 생성 및 사전 데이터 업데이트</summary>
		internal static void CheckDestinationFolder() {
			string DestinationAssetPath = AssetDatabase.GUIDToAssetPath(dictVRCSDKAssetGUID[VRCAssetType.Export]);
			if (string.IsNullOrEmpty(DestinationAssetPath)) DestinationAssetPath = dictVRCSDKAssetFilePath[VRCAssetType.Export];
			if (!AssetDatabase.IsValidFolder(DestinationAssetPath)) {
				string SourceAssetPath = AssetDatabase.GUIDToAssetPath(dictVRCSDKAssetGUID[VRCAssetType.VRSuya]);
				if (string.IsNullOrEmpty(SourceAssetPath)) SourceAssetPath = dictVRCSDKAssetFilePath[VRCAssetType.VRSuya];
				string DestinationGUID = AssetDatabase.CreateFolder(SourceAssetPath, "Export");
				AssetDatabase.Refresh();
				dictVRCSDKAssetGUID[VRCAssetType.Export] = DestinationGUID;
				dictVRCSDKAssetFilePath[VRCAssetType.Export] = AssetDatabase.GUIDToAssetPath(DestinationGUID);
			}
		}
	}
}
#endif
