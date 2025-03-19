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
	[AddComponentMenu("VRSuya/VRSuya Avatar Setting Updater")]
	public class AvatarSettingUpdater : MonoBehaviour {

        // 아바타 에디터용 변수
        public GameObject AvatarGameObjectEditor;
		public string AvatarTypeNameEditor;
		public Transform AvatarAnchorOverrideEditor;
		public bool ChangeTwosidedShadowEditor = false;
		public bool ChangeAnchorOverrideEditor = true;
		public bool ChangeBoundsEditor = false;
		public bool KeepAnimatorControllerEditor = false;
		public bool KeepLinkAnimatorLayerEditor = true;

		public Avatar[] InstalledVRSuyaProductAvatarsEditor;

		public bool InstalledProductAFKEditor = false;
		public bool InstalledProductMogumoguEditor = false;
		public bool InstalledProductWotageiEditor = false;
		public bool InstalledProductFeetEditor = false;
		public bool InstalledProductNyoronyoroEditor = false;
		public bool InstalledProductModelWalkingEditor = false;
		public bool InstalledProductHandmotionEditor = false;
		public bool InstalledProductSuyasuyaEditor = false;
		public bool InstalledProductSoundPadEditor = false;
		// 검색용 업데이트 위치

		public bool InstallProductAFKEditor = false;
		public bool InstallProductMogumoguEditor = false;
		public bool InstallProductWotageiEditor = false;
		public bool InstallProductFeetEditor = false;
		public bool InstallProductNyoronyoroEditor = false;
		public bool InstallProductModelWalkingEditor = false;
		public bool InstallProductHandmotionEditor = false;
		public bool InstallProductSuyasuyaEditor = false;
		public bool InstallProductSoundPadEditor = false;
		// 검색용 업데이트 위치

		public int StatusNeedMoreSpaceMenuEditor = 0;
		public int StatusNeedMoreSpaceParameterEditor = 0;
		public string StatusCodeEditor = "";

		// 아바타 관련 변수
		protected static GameObject AvatarGameObject;
		protected static Avatar AvatarType = Avatar.General;
		protected static Animator AvatarAnimator;
		protected static SkinnedMeshRenderer[] AvatarSkinnedMeshRenderers;
		protected static MeshRenderer[] AvatarMeshRenderers;
		protected static Transform AvatarAnchorOverride;

		protected static VRCAvatarDescriptor AvatarVRCAvatarDescriptor;
        protected static VRCAvatarDescriptor.CustomAnimLayer[] AvatarVRCAvatarLayers;
		protected static VRCExpressionsMenu AvatarVRCMenu;
		protected static VRCExpressionParameters AvatarVRCParameter;

		protected static GameObject[] VRSuyaGameObjects;

		// 아바타 세팅 선택 옵션
		protected static bool ChangeTwosidedShadow = false;
		protected static bool ChangeAnchorOverride = true;
		protected static bool ChangeBounds = false;
		protected static bool KeepAnimatorController = false;
		protected static bool KeepLinkAnimatorLayer = true;

		// 언어 세팅
		protected static string TargetLanguage = "_EN";

		// 제품 구조체
		public struct VRSuyaProduct {
			public ProductName ProductName;
			public string LocomotionAnimatorGUID;
			public string ActionAnimatorGUID;
			public Dictionary<VRCAvatarDescriptor.AnimLayerType, AnimatorControllerLayer[]> RequiredAnimatorLayers;
            public Dictionary<VRCAvatarDescriptor.AnimLayerType, AnimatorControllerParameter[]> RequiredAnimatorParameters;
            public List<VRCExpressionsMenu.Control> RequiredVRCMenus;
			public int RequiredVRCMemoryCount;
            public VRCExpressionParameters.Parameter[] RequiredVRCParameters;
            public Avatar[] SupportAvatarList;
            public string[] AnimationControllerGUID;
			public string[] MenuGUID;
			public string[] ParameterGUID;
			public string[] PrefabGUID;
		}

		// 제품 추가시 추가해야 될 변수
		public enum ProductName {
            AFK,
            Mogumogu,
            Wotagei,
            Feet,
            Nyoronyoro,
			ModelWalking,
			Handmotion,
			Suyasuya,
			SoundPad
			// 검색용 업데이트 위치
		}

		public enum Avatar {
			General, None, 
			Airi, Aldina, Angura, Anon, Anri, Ash, 
			Chiffon, Chocolat, Cygnet, 
			Emmelie, EYO, 
			Firina, Fuzzy, 
			Glaze, Grus, 
			Hakka, 
			IMERIS,
			Karin, Kikyo, Kokoa, Koyuki, Kuronatu, 
			Lapwing, Leefa, Leeme, Lime, Lunalitt, 
			Maki, Mamehinata, MANUKA, Mariel, Marron, Maya, Merino, Milk, Milltina, Minahoshi, Minase, Mint, Mir, Mishe, Moe, 
			Nayu, 
			Platinum, 
			Quiche, 
			Rainy, Ramune_Old, RINDO, Rue, Rusk, 
			SELESTIA, Sephira, Shinano, Shinra, Sio, Sue, Sugar, Suzuhana, 
			Tien, TubeRose, 
			Ukon, Usasaki, Uzuki, 
			Wolferia,
			Yoll, YUGI_MIYO, Yuuko
		}

		public enum VRCAssetType {
			NULL, VRSuya, Template, Export, Locomotion, Gesture, Action, FX, Menu, Parameter
		}

		// 제품 설치 상태
		protected static bool InstalledProductAFK = false;
		protected static bool InstalledProductMogumogu = false;
		protected static bool InstalledProductWotagei = false;
		protected static bool InstalledProductFeet = false;
		protected static bool InstalledProductNyoronyoro = false;
		protected static bool InstalledProductModelWalking = false;
		protected static bool InstalledProductHandmotion = false;
		protected static bool InstalledProductSuyasuya = false;
		protected static bool InstalledProductSoundPad = false;
		// 검색용 업데이트 위치

		protected static bool InstallProductAFK = false;
		protected static bool InstallProductMogumogu = false;
		protected static bool InstallProductWotagei = false;
		protected static bool InstallProductFeet = false;
		protected static bool InstallProductNyoronyoro = false;
		protected static bool InstallProductModelWalking = false;
		protected static bool InstallProductHandmotion = false;
		protected static bool InstallProductSuyasuya = false;
		protected static bool InstallProductSoundPad = false;
		// 검색용 업데이트 위치

		// 추가될 VRSuya 데이터
		protected static VRSuyaProduct[] InstalledVRSuyaProducts;
		protected static Avatar[] InstalledVRSuyaProductAvatars;
		protected static VRSuyaProduct[] RequestSetupVRSuyaProductList;

		// 상태 반환
		protected static string StatusCode = "";
		protected static int UndoGroupIndex;

        // 컴포넌트 최초 로드시 동작
        void OnEnable() {
			if (!AvatarGameObjectEditor) AvatarGameObjectEditor = this.gameObject;
			UpdateUnityEditorStatus();
		}

		/// <summary>에디터 변수 -> 정적 변수 동기화합니다.</summary>
		private void SetStaticVariable() {
            AvatarGameObject = AvatarGameObjectEditor;
			if (AvatarTypeNameEditor != null) AvatarType = (Avatar)Enum.Parse(typeof(Avatar), AvatarTypeNameEditor);
			AvatarAnchorOverride = AvatarAnchorOverrideEditor;
			ChangeTwosidedShadow = ChangeTwosidedShadowEditor;
			ChangeAnchorOverride = ChangeAnchorOverrideEditor;
			ChangeBounds = ChangeBoundsEditor;
			KeepAnimatorController = KeepAnimatorControllerEditor;
			KeepLinkAnimatorLayer = KeepLinkAnimatorLayerEditor;
			InstallProductAFK = InstallProductAFKEditor;
			InstallProductMogumogu = InstallProductMogumoguEditor;
			InstallProductWotagei = InstallProductWotageiEditor;
			InstallProductFeet = InstallProductFeetEditor;
			InstallProductNyoronyoro = InstallProductNyoronyoroEditor;
			InstallProductModelWalking = InstallProductModelWalkingEditor;
			InstallProductHandmotion = InstallProductHandmotionEditor;
			InstallProductSuyasuya = InstallProductSuyasuyaEditor;
			InstallProductSoundPad = InstallProductSoundPadEditor;
			// 검색용 업데이트 위치
			return;
        }

		/// <summary>정적 변수 -> 에디터 변수 동기화합니다.</summary>
		private void SetEditorVariable() {
			AvatarGameObjectEditor = AvatarGameObject;
			AvatarAnchorOverrideEditor = AvatarAnchorOverride;
			InstalledProductAFKEditor = InstalledProductAFK;
			InstalledProductMogumoguEditor = InstalledProductMogumogu;
			InstalledProductWotageiEditor = InstalledProductWotagei;
			InstalledProductFeetEditor = InstalledProductFeet;
			InstalledProductNyoronyoroEditor = InstalledProductNyoronyoro;
			InstalledProductModelWalkingEditor = InstalledProductModelWalking;
			InstalledProductHandmotionEditor = InstalledProductHandmotion;
			InstalledProductSuyasuyaEditor = InstalledProductSuyasuya;
			InstalledProductSoundPadEditor = InstalledProductSoundPad;
			// 검색용 업데이트 위치
			InstalledVRSuyaProductAvatarsEditor = InstalledVRSuyaProductAvatars;
			StatusCodeEditor = StatusCode;
			return;
		}

		/// <summary>언어 설정에 따른 String를 선택합니다</summary>
		private void SetLanguageString(int LanguageIndex) {
			switch (LanguageIndex) {
				case 0:
					TargetLanguage = "_EN";
					break;
				case 1:
					TargetLanguage = "_KO";
					break;
				case 2:
					TargetLanguage = "_JA";
					break;
			}
			return;
		}

		/// <summary>현재 Unity 프로젝트와 아바타 변수를 초기화 한 후 다시 검사합니다.</summary>
		public void UpdateUnityEditorStatus() {
            SetStaticVariable();
			ClearVariable();
            if (VerifyVariable()) {
				AssetDatabase.Refresh();
				ProductSetup.RequestProductRegister();
				UnitySetup.UpdateAvatarStatus();
				ProductSetup.GetVRSuyaGameObjects();
            }
			ProductSetup.UpdateInstalledAvatarList();
			SetEditorVariable();
			return;
        }

		/// <summary>
		/// 본 프로그램의 메인 세팅 로직입니다.
		/// </summary>
		public void UpdateAvatarSetting(int LanguageIndex) {
			Undo.IncrementCurrentGroup();
			Undo.SetCurrentGroupName("VRSuya Avatar Setting Updater");
			UndoGroupIndex = Undo.GetCurrentGroup();
			SetStaticVariable();
			SetLanguageString(LanguageIndex);
			if (VerifyVariable()) {
				AssetDatabase.Refresh();
				ProductSetup.RequestProductRegister();
				ProductSetup.UpdateInstalledAvatarList();
				AddRequestSetupVRSuyaProduct();
				AssetManager.CheckDestinationFolder();
				if (AssetManager.CheckVRCAssets()) {
					if (VerifyVRCSDK()) {
						ProductSetup.GetVRSuyaGameObjects();
						ProductSetup.RequestSetup();
						UnitySetup.GetAvatarSkinnedMeshRenderers();
						UnitySetup.GetAvatarMeshRenderers();
						UnitySetup.UpdateAvatarData();

						Debug.Log("[VRSuya] Update Completed");
						DestroyImmediate(this);
					}
				}
            }
			SetEditorVariable();
			return;
        }

		/// <summary>디버그용 메소드를 실행합니다.</summary>
		public void DebugMethod() {
			SetStaticVariable();
			if (VerifyVariable()) {
				AssetDatabase.Refresh();
				ProductSetup.RequestProductRegister();
				ProductSetup.UpdateInstalledAvatarList();
			}
			SetEditorVariable();
			return;
		}

		/// <summary>정적 변수를 초기화 합니다.</summary>
		private void ClearVariable() {
			AvatarType = Avatar.General;
			AvatarAnimator = null;
            AvatarSkinnedMeshRenderers = new SkinnedMeshRenderer[0];
            AvatarMeshRenderers = new MeshRenderer[0];
			AvatarAnchorOverride = null;
			AvatarVRCAvatarDescriptor = null;
			AvatarVRCAvatarLayers = null;
			AvatarVRCMenu = null;
			AvatarVRCParameter = null;
			VRSuyaGameObjects = new GameObject[0];
			InstalledProductAFK = false;
			InstalledProductMogumogu = false;
			InstalledProductWotagei = false;
			InstalledProductFeet = false;
			InstalledProductNyoronyoro = false;
			InstalledProductModelWalking = false;
			InstalledProductHandmotion = false;
			InstalledProductSuyasuya = false;
			InstalledProductSoundPad = false;
			// 검색용 업데이트 위치
			InstalledVRSuyaProducts = new VRSuyaProduct[0];
			InstalledVRSuyaProductAvatars = new Avatar[0];
			RequestSetupVRSuyaProductList = new VRSuyaProduct[0];
			StatusCode = "";
			StatusNeedMoreSpaceMenuEditor = 0;
			StatusNeedMoreSpaceParameterEditor = 0;
            return;
        }

		/// <summary>아바타의 현재 상태를 검사하여 설치가 가능한지 확인합니다.</summary>
		/// <returns>설치 가능 여부</returns>
		private bool VerifyVariable() {
            if (!AvatarGameObject) {
				StatusCode = "NO_AVATAR";
				return false;
			}
            AvatarGameObject.TryGetComponent(typeof(Animator), out Component Animator);
            if (!Animator) {
                StatusCode = "NO_ANIMATOR";
				return false;
			} else {
				AvatarAnimator = AvatarGameObject.GetComponent<Animator>();
			}
			AvatarGameObject.TryGetComponent(typeof(VRCAvatarDescriptor), out Component AvatarDescriptor);
            if (!AvatarDescriptor) {
				StatusCode = "NO_VRCAVATARDESCRIPTOR";
				return false;
			} else {
				AvatarVRCAvatarDescriptor = AvatarGameObject.GetComponent<VRCAvatarDescriptor>();
				AvatarVRCAvatarLayers = AvatarVRCAvatarDescriptor.baseAnimationLayers;
				AvatarVRCMenu = AvatarVRCAvatarDescriptor.expressionsMenu;
				AvatarVRCParameter = AvatarVRCAvatarDescriptor.expressionParameters;
			}
			return true;
        }

		/// <summary>설치된 제품과 세팅 요청한 제품을 검사하여, 세팅 요청 목록에 넣습니다.</summary>
		private void AddRequestSetupVRSuyaProduct() {
			RequestSetupVRSuyaProductList = new VRSuyaProduct[0];
			if (InstalledProductAFK && InstallProductAFK) {
				VRSuyaProduct RequestProduct = Array.Find(InstalledVRSuyaProducts, Product => Product.ProductName == ProductName.AFK);
				RequestSetupVRSuyaProductList = RequestSetupVRSuyaProductList.Concat(new VRSuyaProduct[] { RequestProduct }).ToArray();
			}
			if (InstalledProductMogumogu && InstallProductMogumogu) {
				VRSuyaProduct RequestProduct = Array.Find(InstalledVRSuyaProducts, Product => Product.ProductName == ProductName.Mogumogu);
				RequestSetupVRSuyaProductList = RequestSetupVRSuyaProductList.Concat(new VRSuyaProduct[] { RequestProduct }).ToArray();
			}
			if (InstalledProductWotagei && InstallProductWotagei) {
				VRSuyaProduct RequestProduct = Array.Find(InstalledVRSuyaProducts, Product => Product.ProductName == ProductName.Wotagei);
				RequestSetupVRSuyaProductList = RequestSetupVRSuyaProductList.Concat(new VRSuyaProduct[] { RequestProduct }).ToArray();
			}
			if (InstalledProductFeet && InstallProductFeet) {
				VRSuyaProduct RequestProduct = Array.Find(InstalledVRSuyaProducts, Product => Product.ProductName == ProductName.Feet);
				RequestSetupVRSuyaProductList = RequestSetupVRSuyaProductList.Concat(new VRSuyaProduct[] { RequestProduct }).ToArray();
			}
            if (InstalledProductNyoronyoro && InstallProductNyoronyoro) {
                VRSuyaProduct RequestProduct = Array.Find(InstalledVRSuyaProducts, Product => Product.ProductName == ProductName.Nyoronyoro);
                RequestSetupVRSuyaProductList = RequestSetupVRSuyaProductList.Concat(new VRSuyaProduct[] { RequestProduct }).ToArray();
            }
			if (InstalledProductModelWalking && InstallProductModelWalking) {
				VRSuyaProduct RequestProduct = Array.Find(InstalledVRSuyaProducts, Product => Product.ProductName == ProductName.ModelWalking);
				RequestSetupVRSuyaProductList = RequestSetupVRSuyaProductList.Concat(new VRSuyaProduct[] { RequestProduct }).ToArray();
			}
			if (InstalledProductHandmotion && InstallProductHandmotion) {
				VRSuyaProduct RequestProduct = Array.Find(InstalledVRSuyaProducts, Product => Product.ProductName == ProductName.Handmotion);
				RequestSetupVRSuyaProductList = RequestSetupVRSuyaProductList.Concat(new VRSuyaProduct[] { RequestProduct }).ToArray();
			}
			if (InstalledProductSuyasuya && InstallProductSuyasuya) {
				VRSuyaProduct RequestProduct = Array.Find(InstalledVRSuyaProducts, Product => Product.ProductName == ProductName.Suyasuya);
				RequestSetupVRSuyaProductList = RequestSetupVRSuyaProductList.Concat(new VRSuyaProduct[] { RequestProduct }).ToArray();
			}
			if (InstalledProductSoundPad && InstallProductSoundPad) {
				VRSuyaProduct RequestProduct = Array.Find(InstalledVRSuyaProducts, Product => Product.ProductName == ProductName.SoundPad);
				RequestSetupVRSuyaProductList = RequestSetupVRSuyaProductList.Concat(new VRSuyaProduct[] { RequestProduct }).ToArray();
			}
			// 검색용 업데이트 위치
			return;
		}

		/// <summary>세팅해야 할 메뉴와 파라메터를 검사하여 세팅 가능한지 확인합니다.</summary>
		/// <returns>설치 가능 여부</returns>
		private bool VerifyVRCSDK() {
			if (!AvatarVRCMenu) {
				StatusCode = "NO_VRCSDK_MENU";
				return false;
			}
			if (!AvatarVRCParameter) {
				StatusCode = "NO_VRCSDK_PARAMETER";
				return false;
			}
			int CurrentAvatarVRCMenuCount = AvatarVRCAvatarDescriptor.expressionsMenu.controls.Count;
			int CurrentAvatarVRCParameterCosts = AvatarVRCParameter.CalcTotalCost();
			int RequiredMenuCount = 0;
			int RequiredParametersCost = 0;
			if (RequestSetupVRSuyaProductList.Length > 0) {
				foreach (VRSuyaProduct RequestProduct in RequestSetupVRSuyaProductList) {
					foreach (VRCExpressionsMenu.Control RequestMenu in RequestProduct.RequiredVRCMenus) {
						if (!AvatarVRCAvatarDescriptor.expressionsMenu.controls.Exists(ExistMenu => ExistMenu.subMenu == RequestMenu.subMenu)) RequiredMenuCount++;
					}
					foreach (VRCExpressionParameters.Parameter RequestParameter in RequestProduct.RequiredVRCParameters) {
						if (!Array.Exists(AvatarVRCParameter.parameters, ExistParameter => ExistParameter.name == RequestParameter.name)) {
							switch (RequestParameter.valueType) {
								case VRCExpressionParameters.ValueType.Bool:
									RequiredParametersCost = RequiredParametersCost + 1;
									break;
								case VRCExpressionParameters.ValueType.Int:
								case VRCExpressionParameters.ValueType.Float:
									RequiredParametersCost = RequiredParametersCost + 8;
									break;
							}
						}
					}
				}
			}
			if (CurrentAvatarVRCMenuCount + RequiredMenuCount > VRCExpressionsMenu.MAX_CONTROLS) {
				StatusCode = "NO_MORE_MENU";
				StatusNeedMoreSpaceMenuEditor = (CurrentAvatarVRCMenuCount + RequiredMenuCount) - VRCExpressionsMenu.MAX_CONTROLS;
				return false;
			}
			if (CurrentAvatarVRCParameterCosts + RequiredParametersCost > VRCExpressionParameters.MAX_PARAMETER_COST) {
				StatusCode = "NO_MORE_PARAMETER";
				StatusNeedMoreSpaceParameterEditor = (CurrentAvatarVRCParameterCosts + RequiredParametersCost) - VRCExpressionParameters.MAX_PARAMETER_COST;
				return false;
            }
			return true;
		}

		/// <summary>요청한 제품에서 아바타를 검색하여 세팅이 가능한지 확인합니다.</summary>
		/// <returns>아바타 파일 존재 여부</returns>
		public static bool ReturnAvatarInstalled(ProductName Product, string AvatarName) {
			bool ReturnResult = false;
			if (!string.IsNullOrEmpty(AvatarName)) {
				Avatar RequestAvatarType;
				if (Enum.TryParse<Avatar>(AvatarName, out RequestAvatarType)) {
					if (Array.Exists(InstalledVRSuyaProducts, InstalledProduct => InstalledProduct.ProductName == Product)) {
						VRSuyaProduct TargetProduct = Array.Find(InstalledVRSuyaProducts, InstalledProduct => InstalledProduct.ProductName == Product);
						ReturnResult = Array.Exists(TargetProduct.SupportAvatarList, SupportAvatar => SupportAvatar == RequestAvatarType);
					}
				}
			}
			return ReturnResult;
		}

		/// <summary>요청한 제품에서 읽어들인 내용을 디버그 합니다.</summary>
		public static void PrintProductInformation(VRSuyaProduct RequestProduct) {
			string RequestProductName = RequestProduct.ProductName.ToString();
			string[] AnimatorLayers = RequestProduct.RequiredAnimatorLayers.Select(Item => Item.Key.ToString() + " (" + string.Join(", ", Item.Value.Select(Layer => Layer.name).ToArray()) + ")").ToArray();
			string[] AnimatorParameters = RequestProduct.RequiredAnimatorParameters.Select(Item => Item.Key.ToString() + " (" + string.Join(", ", Item.Value.Select(Layer => Layer.name).ToArray()) + ")").ToArray();
			string[] VRCMenus = RequestProduct.RequiredVRCMenus.Select(Item => Item.name).ToArray();
			string[] RequiredParameters = RequestProduct.RequiredVRCParameters.Select(Item => Item.name).ToArray();
			string[] SupportAvatars = RequestProduct.SupportAvatarList.Select(Item => Item.ToString()).ToArray();
			string[] AnimationControllers = RequestProduct.AnimationControllerGUID.Select(Item => GUIDToAssetName(Item, true)).ToArray();
			string[] Menus = RequestProduct.MenuGUID.Select(Item => GUIDToAssetName(Item, true)).ToArray();
			string[] Parameters = RequestProduct.ParameterGUID.Select(Item => GUIDToAssetName(Item, true)).ToArray();
			string[] Prefabs = RequestProduct.PrefabGUID.Select(Item => GUIDToAssetName(Item, true)).ToArray();
			Debug.Log(RequestProductName + " Product Name : " + RequestProduct.ProductName.ToString());
			Debug.Log(RequestProductName + " Locomotion Animator : " + GUIDToAssetName(RequestProduct.LocomotionAnimatorGUID, true));
			Debug.Log(RequestProductName + " Action Animator : " + GUIDToAssetName(RequestProduct.ActionAnimatorGUID, true));
			Debug.Log(RequestProductName + " Required Animator Layers : " + string.Join(", ", AnimatorLayers));
			Debug.Log(RequestProductName + " Required Animator Parameters : " + string.Join(", ", AnimatorParameters));
			Debug.Log(RequestProductName + " Required VRC Menus : " + string.Join(", ", VRCMenus));
			Debug.Log(RequestProductName + " Required VRC MemoryCount : " + RequestProduct.RequiredVRCMemoryCount);
			Debug.Log(RequestProductName + " Required VRC Parameters : " + string.Join(", ", RequiredParameters));
			Debug.Log(RequestProductName + " Support Avatar : " + string.Join(", ", SupportAvatars));
			Debug.Log(RequestProductName + " AnimationController Files : " + string.Join(", ", AnimationControllers));
			Debug.Log(RequestProductName + " Menu Files : " + string.Join(", ", Menus));
			Debug.Log(RequestProductName + " Parameter Files : " + string.Join(", ", Parameters));
			Debug.Log(RequestProductName + " Prefab Files : " + string.Join(", ", Prefabs));
			return;
		}

		/// <summary>요청한 GUID를 파일 이름으로 반환합니다. 2번째 인자는 확장명 포함 여부를 결정합니다.</summary>
		/// <returns>파일 이름</returns>
		internal static string GUIDToAssetName(string GUID, bool OnlyFileName) {
			string FileName = "";
			FileName = AssetDatabase.GUIDToAssetPath(GUID).Split('/')[AssetDatabase.GUIDToAssetPath(GUID).Split('/').Length - 1];
			if (OnlyFileName) FileName = FileName.Split('.')[0];
			return FileName;
		}
	}
}
#endif