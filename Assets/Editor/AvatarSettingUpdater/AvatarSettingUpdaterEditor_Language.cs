using System.Collections.Generic;
using System.Linq;

using UnityEditor;

/*
 * VRSuya Avatar Setting Updater Editor
 * Contact : vrsuya@gmail.com // Twitter : https://twitter.com/VRSuya
 */

namespace com.vrsuya.installer {

	public class LanguageHelper : AvatarSettingUpdaterEditor {

		/// <summary>요청한 값을 설정된 언어에 맞춰 값을 반환합니다.</summary>
		/// <returns>요청한 String의 현재 설정된 언어 버전</returns>
		internal static string GetContextString(string RequestContext) {
			string ReturnContext = RequestContext;
			switch (LanguageIndex) {
				case 0:
					if (String_English.ContainsKey(RequestContext)) {
						ReturnContext = String_English[RequestContext];
					}
					break;
				case 1:
					if (String_Korean.ContainsKey(RequestContext)) {
						ReturnContext = String_Korean[RequestContext];
					}
					break;
				case 2:
					if (String_Japanese.ContainsKey(RequestContext)) {
						ReturnContext = String_Japanese[RequestContext];
					}
					break;
			}
			return ReturnContext;
		}

		/// <summary>요청한 아바타 이름을 설정된 언어에 맞춰 리스트를 재작성합니다.</summary>
		/// <returns>아바타 이름의 현재 설정된 언어 버전</returns>
		internal static string[] ReturnAvatarName(SerializedProperty AvatarNameListProperty) {
			int AvatarNameCount = AvatarNameListProperty.arraySize;
			AvatarSettingUpdater.Avatar[] InstalledVRSuyaProductAvatars = new AvatarSettingUpdater.Avatar[AvatarNameCount];

			for (int Index = 0; Index < AvatarNameCount; Index++)
			{
				SerializedProperty ArrayItem = AvatarNameListProperty.GetArrayElementAtIndex(Index);
				string AvatarEnumName = ArrayItem.enumNames[ArrayItem.enumValueIndex];
				InstalledVRSuyaProductAvatars[Index] = (AvatarSettingUpdater.Avatar)System.Enum.Parse(typeof(AvatarSettingUpdater.Avatar), AvatarEnumName);
			}

			return InstalledVRSuyaProductAvatars
				.Where((AvatarName) => dictAvatarNames.ContainsKey(AvatarName))
				.Select((AvatarName) => dictAvatarNames[AvatarName][LanguageIndex])
				.ToArray();
		}

		// 영어 사전 데이터
		private static Dictionary<string, string> String_English = new Dictionary<string, string>() {
			{ "String_Language", "Language" },
			{ "String_Debug", "Debug" },
			{ "String_Avatar", "Avatar Type" },
			{ "String_TargetAvatar", "Target Avatar" },
			{ "String_Advanced", "Advanced" },
			{ "String_TwoSidedShadow", "Set to Two-sided Shadow" },
			{ "String_ChangeAnchorOverride", "Change AnchorOverride" },
			{ "String_ChangeBounds", "Change Bounds" },
			{ "String_KeepAnimatorController", "Keep Animator Controller" },
			{ "String_KeepAnimatorController_Info", "Don't replace Locomotion and Action Layer, use it if you want edit it yourself" },
			{ "String_KeepLinkAnimatorLayer", "Keep link Animator Layer with Original" },
			{ "String_ObjectAnchorOverride", "AnchorOverride GameObject" },
			{ "String_SetupProduct", "VRSuya products to setup" },
			{ "String_GetAvatarData", "Get Avatar Data" },
			{ "String_UpdateAvatarData", "Update Avatar" },
			{ "String_Undo", "Undo" },

			// 제품명
			{ "String_ProductAFK", "AFK Package" },
			{ "String_ProductMogumogu", "Mogumogu Project" },
			{ "String_ProductWotagei", "Wotagei" },
			{ "String_ProductFeet", "HopeskyD Asiasi Project" },
			{ "String_ProductNyoronyoro", "Nyoronyoro Locomotion" },
			{ "String_ProductModelWalking", "Model Walking" },
			{ "String_ProductHandmotion", "Handmotion" },
			{ "String_ProductSuyasuya", "Suyasuya" },
			{ "String_ProductSoundPad", "SoundPad" },
			// 검색용 업데이트 위치

			// 에러 코드
			{ "NO_AVATAR", "No Avatar is selected" },
			{ "NO_ANIMATOR", "Not found Animator Component in the Avatar" },
			{ "NO_VRCAVATARDESCRIPTOR", "Not found VRC Avatar Descriptor Component in the Avatar" },
			{ "NO_VRCSDK_MENU", "Not found VRC Avatar Menu" },
			{ "NO_VRCSDK_PARAMETER", "Not found VRC Avatar Parameter" },
			{ "NO_MORE_MENU", "Need {0} more space to add VRC Menu" },
			{ "NO_MORE_PARAMETER", "Need {0} more space to add VRC Parameter" },
			{ "NO_SOURCE_FILE", "Not found VRC Assets(likes Animator Controller, Menu, Parameter) in the Avatar" },
			{ "NO_VRSUYA_FILE", "VRSuya package is not installed on Unity project" }
		};

		// 한국어 사전 데이터
		private static Dictionary<string, string> String_Korean = new Dictionary<string, string>() {
			{ "String_Language", "언어" },
			{ "String_Debug", "디버그" },
			{ "String_Avatar", "아바타 종류" },
			{ "String_TargetAvatar", "대상 아바타" },
			{ "String_Advanced", "고급" },
			{ "String_TwoSidedShadow", "Two-sided 그림자 설정" },
			{ "String_ChangeAnchorOverride", "AnchorOverride 설정" },
			{ "String_ChangeBounds", "Bounds 설정" },
			{ "String_KeepAnimatorController", "애니메이터 유지" },
			{ "String_KeepAnimatorController_Info", "로코모션과 액션 레이어를 교체하지 않습니다, 직접 편집하는 경우 사용하세요" },
			{ "String_KeepLinkAnimatorLayer", "애니메이터 레이어를 원본과 연결" },
			{ "String_ObjectAnchorOverride", "AnchorOverride 오브젝트" },
			{ "String_SetupProduct", "세팅하려는 VRSuya 제품" },
			{ "String_GetAvatarData", "아바타 데이터 가져오기" },
			{ "String_UpdateAvatarData", "아바타 업데이트" },
			{ "String_Undo", "실행 취소" },

			// 제품명
			{ "String_ProductAFK", "AFK 3종 세트" },
			{ "String_ProductMogumogu", "모구모구 프로젝트" },
			{ "String_ProductWotagei", "오타게" },
			{ "String_ProductFeet", "HopeskyD 아시아시 프로젝트" },
			{ "String_ProductNyoronyoro", "뇨로뇨로 로코모션" },
			{ "String_ProductModelWalking", "모델 워킹" },
			{ "String_ProductHandmotion", "핸드모션" },
			{ "String_ProductSuyasuya", "스야스야" },
			{ "String_ProductSoundPad", "사운드패드" },
			// 검색용 업데이트 위치

			// 에러 코드
			{ "NO_AVATAR", "아바타가 지정되지 않았습니다" },
			{ "NO_ANIMATOR", "아바타에서 애니메이터를 찾을 수 없습니다" },
			{ "NO_VRCAVATARDESCRIPTOR", "아바타에서 VRC 아바타 디스크립터를 찾을 수 없습니다" },
			{ "NO_VRCSDK_MENU", "VRC 메뉴가 존재하지 않습니다" },
			{ "NO_VRCSDK_PARAMETER", "VRC 파라메터가 존재하지 않습니다" },
			{ "NO_MORE_MENU", "VRC 메뉴를 추가할 공간이 {0}개 부족합니다" },
			{ "NO_MORE_PARAMETER", "VRC 파라메터를 추가할 공간이 {0}개 부족합니다" },
			{ "NO_SOURCE_FILE", "아바타에서 VRC용 에셋(애니메이터, 메뉴, 파라메터)을 찾을 수 없습니다" },
			{ "NO_VRSUYA_FILE", "Unity 프로젝트에 VRSuya 패키지가 설치되어 있지 않습니다" }
		};

		// 일본어 사전 데이터
		private static Dictionary<string, string> String_Japanese = new Dictionary<string, string>() {
			{ "String_Language", "言語" },
			{ "String_Debug", "デバッグ" },
			{ "String_Avatar", "アバタータイプ" },
			{ "String_TargetAvatar", "対象アバター" },
			{ "String_Advanced", "詳細" },
			{ "String_TwoSidedShadow", "Two-sided影設定" },
			{ "String_ChangeAnchorOverride", "AnchorOverride設定" },
			{ "String_ChangeBounds", "Bounds設定" },
			{ "String_KeepAnimatorController", "Animator 維持" },
			{ "String_KeepAnimatorController_Info", "LocomotionとActionレイヤーを交換しません、自分で編集する場合は使用してください" },
			{ "String_KeepLinkAnimatorLayer", "アニメーターレイヤーを原本と接続" },
			{ "String_ObjectAnchorOverride", "AnchorOverrideオブジェクト" },
			{ "String_SetupProduct", "セットしようとするVRSuya製品" },
			{ "String_GetAvatarData", "アバターデータの取得" },
			{ "String_UpdateAvatarData", "アバターアップデート" },
			{ "String_Undo", "元に戻す" },

			// 제품명
			{ "String_ProductAFK", "AFK 3種セット" },
			{ "String_ProductMogumogu", "もぐもぐ プロジェクト" },
			{ "String_ProductWotagei", "ヲタ芸" },
			{ "String_ProductFeet", "HopeskyD 足足プロジェクト" },
			{ "String_ProductNyoronyoro", "にょろにょろ ロコモーション" },
			{ "String_ProductModelWalking", "ランウェイ・モデルウォーク" },
			{ "String_ProductHandmotion", "ハンドモーション" },
			{ "String_ProductSuyasuya", "すやすや" },
			{ "String_ProductSoundPad", "サウンドパッド" },
			// 검색용 업데이트 위치

			// 에러 코드
			{ "NO_AVATAR", "アバターが指定されていません" },
			{ "NO_ANIMATOR", "アバターにアニメーターが見つかりません" },
			{ "NO_VRCAVATARDESCRIPTOR", "アバターにVRCアバターディスクリプターが見つかりません" },
			{ "NO_VRCSDK_MENU", "VRCメニューが存在しません" },
			{ "NO_VRCSDK_PARAMETER", "VRCパラメータが存在しません" },
			{ "NO_MORE_MENU", "VRCメニューを追加するスペース{0}スロットが不足しています" },
			{ "NO_MORE_PARAMETER", "VRCパラメータを追加するスペース{0}スロットが不足しています" },
			{ "NO_SOURCE_FILE", "アバターにVRC用アセット(アニメーター、メニュー、パラメータ)が見つかりません" },
			{ "NO_VRSUYA_FILE", "UnityプロジェクトにVRSuyaパッケージがインストールされていません" }
		};

		/// <summary>요청한 아바타 이름들을 설정된 언어에 맞춰 변환합니다.</summary>
		/// <returns>요청한 아바타 이름들의 현재 설정된 언어 버전</returns>
		private static readonly Dictionary<AvatarSettingUpdater.Avatar, string[]> dictAvatarNames = new Dictionary<AvatarSettingUpdater.Avatar, string[]>() {
			{ AvatarSettingUpdater.Avatar.General, new string[] { "General", "일반", "一般" } },
			{ AvatarSettingUpdater.Avatar.None, new string[] { "None", "없음", "無い" } },
			{ AvatarSettingUpdater.Avatar.Airi, new string[] { "Airi", "아이리", "愛莉" } },
			{ AvatarSettingUpdater.Avatar.Aldina, new string[] { "Aldina", "알디나", "アルディナ" } },
			{ AvatarSettingUpdater.Avatar.Angura, new string[] { "Angura", "앙그라", "アングラ" } },
			{ AvatarSettingUpdater.Avatar.Anon, new string[] { "Anon", "아논", "あのん" } },
			{ AvatarSettingUpdater.Avatar.Anri, new string[] { "Anri", "안리", "杏里" } },
			{ AvatarSettingUpdater.Avatar.Ash, new string[] { "Ash", "애쉬", "アッシュ" } },
			{ AvatarSettingUpdater.Avatar.Chiffon, new string[] { "Chiffon", "쉬폰", "シフォン" } },
			{ AvatarSettingUpdater.Avatar.Chocolat, new string[] { "Chocolat", "쇼콜라", "ショコラ" } },
			{ AvatarSettingUpdater.Avatar.Cygnet, new string[] { "Cygnet", "시그넷", "シグネット" } },
			{ AvatarSettingUpdater.Avatar.Emmelie, new string[] { "Emmelie", "에밀리", "Emmelie" } },
			{ AvatarSettingUpdater.Avatar.EYO, new string[] { "EYO", "이요", "イヨ" } },
			{ AvatarSettingUpdater.Avatar.Firina, new string[] { "Firina", "휘리나", "フィリナ" } },
			{ AvatarSettingUpdater.Avatar.Fuzzy, new string[] { "Fuzzy", "퍼지", "ファジー" } },
			{ AvatarSettingUpdater.Avatar.Glaze, new string[] { "Glaze", "글레이즈", "ぐれーず" } },
			{ AvatarSettingUpdater.Avatar.Grus, new string[] { "Grus", "그루스", "Grus" } },
			{ AvatarSettingUpdater.Avatar.Hakka, new string[] { "Hakka", "하카", "薄荷" } },
			{ AvatarSettingUpdater.Avatar.IMERIS, new string[] { "IMERIS", "이메리스", "イメリス" } },
			{ AvatarSettingUpdater.Avatar.Karin, new string[] { "Karin", "카린", "カリン" } },
			{ AvatarSettingUpdater.Avatar.Kikyo, new string[] { "Kikyo", "키쿄", "桔梗" } },
			{ AvatarSettingUpdater.Avatar.Kokoa, new string[] { "Kokoa", "코코아", "ここあ" } },
			{ AvatarSettingUpdater.Avatar.Koyuki, new string[] { "Koyuki", "코유키", "狐雪" } },
			{ AvatarSettingUpdater.Avatar.Kuronatu, new string[] { "Kuronatu", "쿠로나츠", "くろなつ" } },
            { AvatarSettingUpdater.Avatar.Lapwing, new string[] { "Lapwing", "랩윙", "Lapwing" } },
            { AvatarSettingUpdater.Avatar.Leefa, new string[] { "Leefa", "리파", "リーファ" } },
			{ AvatarSettingUpdater.Avatar.Leeme, new string[] { "Leeme", "리메", "リーメ" } },
            { AvatarSettingUpdater.Avatar.Lime, new string[] { "Lime", "라임", "ライム" } },
            { AvatarSettingUpdater.Avatar.Lunalitt, new string[] { "Lunalitt", "루나릿트", "ルーナリット" } },
			{ AvatarSettingUpdater.Avatar.Maki, new string[] { "Maki", "마키", "碼希" } },
			{ AvatarSettingUpdater.Avatar.Mamehinata, new string[] { "Mamehinata", "마메히나타", "まめひなた" } },
			{ AvatarSettingUpdater.Avatar.MANUKA, new string[] { "MANUKA", "마누카", "マヌカ" } },
			{ AvatarSettingUpdater.Avatar.Mariel, new string[] { "Mariel", "마리엘", "まりえる" } },
			{ AvatarSettingUpdater.Avatar.Marron, new string[] { "Marron", "마론", "マロン" } },
			{ AvatarSettingUpdater.Avatar.Maya, new string[] { "Maya", "마야", "舞夜" } },
			{ AvatarSettingUpdater.Avatar.Merino, new string[] { "Merino", "메리노", "メリノ" } },
			{ AvatarSettingUpdater.Avatar.Milfy, new string[] { "Milfy", "미르피", "ミルフィ" } },
			{ AvatarSettingUpdater.Avatar.Milk, new string[] { "Milk(New)", "밀크(신)", "ミルク（新）" } },
			{ AvatarSettingUpdater.Avatar.Milltina, new string[] { "Milltina", "밀티나", "ミルティナ" } },
			{ AvatarSettingUpdater.Avatar.Minahoshi, new string[] { "Minahoshi", "미나호시", "みなほし" } },
			{ AvatarSettingUpdater.Avatar.Minase, new string[] { "Minase", "미나세", "水瀬" } },
			{ AvatarSettingUpdater.Avatar.Mint, new string[] { "Mint", "민트", "ミント" } },
			{ AvatarSettingUpdater.Avatar.Mir, new string[] { "Mir", "미르", "ミール" } },
			{ AvatarSettingUpdater.Avatar.Mishe, new string[] { "Mishe", "미셰", "ミーシェ" } },
			{ AvatarSettingUpdater.Avatar.Moe, new string[] { "Moe", "모에", "萌" } },
			{ AvatarSettingUpdater.Avatar.Nayu, new string[] { "Nayu", "나유", "ナユ" } },
			{ AvatarSettingUpdater.Avatar.Platinum, new string[] { "Platinum", "플레티늄", "プラチナ" } },
			{ AvatarSettingUpdater.Avatar.Quiche, new string[] { "Quiche", "킷슈", "キッシュ" } },
			{ AvatarSettingUpdater.Avatar.Rainy, new string[] { "Rainy", "레이니", "レイニィ" } },
			{ AvatarSettingUpdater.Avatar.Ramune_Old, new string[] { "Ramune(Old)", "라무네(구)", "ラムネ（古）" } },
			{ AvatarSettingUpdater.Avatar.RINDO, new string[] { "RINDO", "린도", "竜胆" } },
			{ AvatarSettingUpdater.Avatar.Rue, new string[] { "Rue", "루에", "ルウ" } },
			{ AvatarSettingUpdater.Avatar.Rusk, new string[] { "Rusk", "러스크", "ラスク" } },
			{ AvatarSettingUpdater.Avatar.SELESTIA, new string[] { "SELESTIA", "셀레스티아", "セレスティア" } },
			{ AvatarSettingUpdater.Avatar.Sephira, new string[] { "Sephira", "세피라", "セフィラ" } },
			{ AvatarSettingUpdater.Avatar.Shinano, new string[] { "Shinano", "시나노", "しなの" } },
			{ AvatarSettingUpdater.Avatar.Shinra, new string[] { "Shinra", "신라", "森羅" } },
			{ AvatarSettingUpdater.Avatar.Sio, new string[] { "Sio", "시오", "しお" } },
			{ AvatarSettingUpdater.Avatar.Sue, new string[] { "Sue", "스우", "透羽" } },
			{ AvatarSettingUpdater.Avatar.Sugar, new string[] { "Sugar", "슈가", "シュガ" } },
			{ AvatarSettingUpdater.Avatar.Suzuhana, new string[] { "Suzuhana", "스즈하나", "すずはな" } },
			{ AvatarSettingUpdater.Avatar.Tien, new string[] { "Tien", "티엔", "ティエン" } },
            { AvatarSettingUpdater.Avatar.TubeRose, new string[] { "TubeRose", "튜베로즈", "TubeRose" } },
            { AvatarSettingUpdater.Avatar.Ukon, new string[] { "Ukon", "우콘", "右近" } },
			{ AvatarSettingUpdater.Avatar.Usasaki, new string[] { "Usasaki", "우사사키", "うささき" } },
            { AvatarSettingUpdater.Avatar.Uzuki, new string[] { "Uzuki", "우즈키", "卯月" } },
            { AvatarSettingUpdater.Avatar.Wolferia, new string[] { "Wolferia", "울페리아", "ウルフェリア" } },
			{ AvatarSettingUpdater.Avatar.Yoll, new string[] { "Yoll", "요루", "ヨル" } },
            { AvatarSettingUpdater.Avatar.YUGI_MIYO, new string[] { "YUGI MIYO", "유기 미요", "ユギ ミヨ" } },
            { AvatarSettingUpdater.Avatar.Yuuko, new string[] { "Yuuko", "유우코", "幽狐" } }
		};
	}
}
