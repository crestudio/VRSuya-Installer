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
	public class ProductSetup_Nyoronyoro : ProductSetup {

		static VRSuyaProduct Nyoronyoro;

		/// <summary>제품 정보를 AssetManager에게 요청하여 업데이트 한 후, 설치된 에셋 목록에 추가합니다.</summary>
		internal static void RegisterProduct() {
            InstalledProductNyoronyoro = false;
			Nyoronyoro = new VRSuyaProduct();
			Nyoronyoro = AssetManager.UpdateProductInformation(ProductName.Nyoronyoro);
			InstalledVRSuyaProducts = InstalledVRSuyaProducts.Concat(new VRSuyaProduct[] { Nyoronyoro }).ToArray();
			if (Nyoronyoro.SupportAvatarList.Length > 0) InstalledProductNyoronyoro = true;
		}
	}
}
#endif