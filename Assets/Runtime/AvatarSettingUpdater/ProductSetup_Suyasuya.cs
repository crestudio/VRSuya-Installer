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
	public class ProductSetup_Suyasuya : ProductSetup {

		static VRSuyaProduct Suyasuya;

		/// <summary>제품 정보를 AssetManager에게 요청하여 업데이트 한 후, 설치된 에셋 목록에 추가합니다.</summary>
		internal static void RegisterProduct() {
			InstalledProductSuyasuya = false;
			Suyasuya = new VRSuyaProduct();
			Suyasuya = AssetManager.UpdateProductInformation(ProductName.Suyasuya);
			InstalledVRSuyaProducts = InstalledVRSuyaProducts.Concat(new VRSuyaProduct[] { Suyasuya }).ToArray();
			if (Suyasuya.SupportAvatarList.Length > 0) InstalledProductSuyasuya = true;
			return;
		}
	}
}
#endif