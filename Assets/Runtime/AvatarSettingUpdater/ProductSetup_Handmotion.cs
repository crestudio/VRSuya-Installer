#if UNITY_EDITOR
using System.Linq;

using UnityEngine;

/*
 * VRSuya Avatar Setting Updater
 * Contact : vrsuya@gmail.com // Twitter : https://twitter.com/VRSuya
 */

namespace com.vrsuya.installer {

	[ExecuteInEditMode]
	[AddComponentMenu("")]
	public class ProductSetup_Handmotion : ProductSetup {

		static VRSuyaProduct Handmotion;

		/// <summary>제품 정보를 AssetManager에게 요청하여 업데이트 한 후, 설치된 에셋 목록에 추가합니다.</summary>
		internal static void RegisterProduct() {
			InstalledProductHandmotion = false;
			Handmotion = new VRSuyaProduct();
			Handmotion = AssetManager.UpdateProductInformation(ProductName.Handmotion);
			InstalledVRSuyaProducts = InstalledVRSuyaProducts.Concat(new VRSuyaProduct[] { Handmotion }).ToArray();
			if (Handmotion.SupportAvatarList.Length > 0) InstalledProductHandmotion = true;
			return;
		}
	}
}
#endif