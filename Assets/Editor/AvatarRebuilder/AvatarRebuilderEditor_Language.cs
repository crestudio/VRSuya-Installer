using System.Collections.Generic;
using System.Linq;

using UnityEditor;

/*
 * VRSuya AvatarRebuilder
 * Contact : vrsuya@gmail.com // Twitter : https://twitter.com/VRSuya
 * Thanks to Dalgona. & C_Carrot & Naru & Rekorn
 */

namespace com.vrsuya.avatarrebuilder {

	public class LanguageHelper : AvatarRebuilderEditor {

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
		internal static string[] ReturnAvatarName() {
			return typeof(AvatarRebuilder.Avatar)
				.GetFields()
				.Where(field => field.FieldType == typeof(AvatarRebuilder.Avatar))
				.Select(field => field.GetValue(null))
				.Cast<AvatarRebuilder.Avatar>()
				.Where(avatarEnum => dictAvatarNames.ContainsKey(avatarEnum))
				.Select(avatarEnum => dictAvatarNames[avatarEnum][LanguageIndex])
				.ToArray();
		}

		// 영어 사전 데이터
		private static readonly Dictionary<string, string> String_English = new Dictionary<string, string>() {
			// UI 데이터
			{ "String_Language", "Language" },
			{ "String_Debug", "Debug" },
			{ "String_OriginalAvatar", "Original Avatar" },
			{ "String_NewAvatar", "New Avatar" },
			{ "String_AvatarType", "Avatar Type" },
			{ "String_Advanced", "Advanced" },
			{ "String_RootBone", "Avatar Root Bone" },
			{ "String_RestoreTransform", "Restore Transforms" },
			{ "String_RestPose", "Reset to Rest Pose" },
			{ "String_ReorderGameObject", "Reorder Armature" },
			{ "String_SkinnedMeshRendererList", "Replacement SkinnedMeshRenderer List" },
			{ "String_ImportSkinnedMeshRenderer", "Import SkinnedMeshRenderer List" },
			{ "String_ReplaceAvatar", "Replace Avatar" },

			// 상태 메시지
			{ "String_General", "Please select [General] for avatars that do not exist in the list" },
			{ "String_Warning", "Other components than SkinnedMeshRenderer will not be copied" },

			// 에러 코드
			{ "UPDATED_RENDERER", "Completed import SkinnedMeshRenderer List" },
			{ "NO_AVATAR", "No Avatar is selected" },
			{ "SAME_OBJECT", "Same as the original avatar! Select a new GameObject of the same avatar" },
			{ "NO_NEW_ANIMATOR", "Not found Animator Component in the New Avatar" },
			{ "NO_ROOTBONE", "Not found Hips bone in the New Avatar" },
			{ "NO_OLD_ANIMATOR", "Not found Animator Component in the Original Avatar" }
		};

		// 한국어 사전 데이터
		private static readonly Dictionary<string, string> String_Korean = new Dictionary<string, string>() {
			// UI 데이터
			{ "String_Language", "언어" },
			{ "String_Debug", "디버그" },
			{ "String_OriginalAvatar", "원본 아바타" },
			{ "String_NewAvatar", "신규 아바타" },
			{ "String_AvatarType", "아바타 타입" },
			{ "String_Advanced", "고급" },
			{ "String_RootBone", "아바타 루트 본" },
			{ "String_RestoreTransform", "아바타 트랜스폼 복원" },
			{ "String_RestPose", "기본 포즈로 복원" },
			{ "String_ReorderGameObject", "아마추어 순서 복원" },
			{ "String_SkinnedMeshRendererList", "복원될 스킨드 메쉬 렌더러 목록" },
			{ "String_ImportSkinnedMeshRenderer", "스킨드 메쉬 렌더러 목록 가져오기" },
			{ "String_ReplaceAvatar", "아바타 교체" },

			// 상태 메시지
			{ "String_General", "목록에 존재하지 않는 아바타는 [일반]을 선택해 주세요" },
			{ "String_Warning", "스킨드 메쉬 렌더러 외의 속성은 가져오지 않습니다!" },

			// 에러 코드
			{ "UPDATED_RENDERER", "복원될 스킨드 메쉬 렌더러 목록을 가져왔습니다" },
			{ "NO_AVATAR", "아바타가 지정되지 않았습니다" },
			{ "SAME_OBJECT", "원본과 같은 아바타입니다, 복구하려는 아바타와 같은 종류의 아바타를 만들어 넣어주세요" },
			{ "NO_NEW_ANIMATOR", "새 아바타에서 애니메이터를 찾을 수 없습니다" },
			{ "NO_ROOTBONE", "아바타에서 루트 본을 찾을 수 없습니다" },
			{ "NO_OLD_ANIMATOR", "원본 아바타에서 애니메이터를 찾을 수 없습니다" }
		};

		// 일본어 사전 데이터
		private static readonly Dictionary<string, string> String_Japanese = new Dictionary<string, string>() {
			// UI 데이터
			{ "String_Language", "言語" },
			{ "String_Debug", "デバッグ" },
			{ "String_OriginalAvatar", "原本アバター" },
			{ "String_NewAvatar", "新規アバター" },
			{ "String_AvatarType", "アバタータイプ" },
			{ "String_Advanced", "詳細" },
			{ "String_RootBone", "アバタールートボーン" },
			{ "String_RestoreTransform", "アバターTransform復元" },
			{ "String_RestPose", "基本ポーズに復元" },
			{ "String_ReorderGameObject", "アーマチュア順序復元" },
			{ "String_SkinnedMeshRendererList", "復元されるSkinnedMeshRenderer一覧" },
			{ "String_ImportSkinnedMeshRenderer", "SkinnedMeshRendererリストを取得" },
			{ "String_ReplaceAvatar", "アバター交換" },

			// 상태 메시지
			{ "String_General", "リストに存在しないアバターは「一般」を選択してください" },
			{ "String_Warning", "SkinnedMeshRenderer以外のプロパティは取得しません！" },

			// 에러 코드
			{ "UPDATED_RENDERER", "復元されるSkinnedMeshRendererのリストを取得しました。" },
			{ "NO_AVATAR", "アバターが指定されていません" },
			{ "SAME_OBJECT", "原本と同じアバターです、復旧したいアバターと同じ種類のアバターを作って入れてください" },
			{ "NO_NEW_ANIMATOR", "新しいアバターにアニメーターが見つかりません" },
			{ "NO_ROOTBONE", "アバターにルートボーンが見つかりません" },
			{ "NO_OLD_ANIMATOR", "元のアバターにアニメーターが見つかりません" }
		};

		/// <summary>요청한 아바타 이름들을 설정된 언어에 맞춰 변환합니다.</summary>
		/// <returns>요청한 아바타 이름들의 현재 설정된 언어 버전</returns>
		private static readonly Dictionary<AvatarRebuilder.Avatar, string[]> dictAvatarNames = new Dictionary<AvatarRebuilder.Avatar, string[]>() {
			{ AvatarRebuilder.Avatar.General, new string[] { "General", "일반", "一般" } },
			{ AvatarRebuilder.Avatar.Airi, new string[] { "Airi", "아이리", "愛莉" } },
			{ AvatarRebuilder.Avatar.Aldina, new string[] { "Aldina", "알디나", "アルディナ" } },
			{ AvatarRebuilder.Avatar.Angura, new string[] { "Angura", "앙그라", "アングラ" } },
			{ AvatarRebuilder.Avatar.Anon, new string[] { "Anon", "아논", "あのん" } },
			{ AvatarRebuilder.Avatar.Anri, new string[] { "Anri", "안리", "杏里" } },
			{ AvatarRebuilder.Avatar.Ash, new string[] { "Ash", "애쉬", "アッシュ" } },
			{ AvatarRebuilder.Avatar.Chiffon, new string[] { "Chiffon", "쉬폰", "シフォン" } },
			{ AvatarRebuilder.Avatar.Chise, new string[] { "Chise", "치세", "チセ" } },
			{ AvatarRebuilder.Avatar.Chocolat, new string[] { "Chocolat", "쇼콜라", "ショコラ" } },
			{ AvatarRebuilder.Avatar.Cygnet, new string[] { "Cygnet", "시그넷", "シグネット" } },
			{ AvatarRebuilder.Avatar.Emmelie, new string[] { "Emmelie", "에밀리", "Emmelie" } },
			{ AvatarRebuilder.Avatar.EYO, new string[] { "EYO", "이요", "イヨ" } },
			{ AvatarRebuilder.Avatar.Firina, new string[] { "Firina", "휘리나", "フィリナ" } },
			{ AvatarRebuilder.Avatar.Fuzzy, new string[] { "Fuzzy", "퍼지", "ファジー" } },
			{ AvatarRebuilder.Avatar.Glaze, new string[] { "Glaze", "글레이즈", "ぐれーず" } },
			{ AvatarRebuilder.Avatar.Grus, new string[] { "Grus", "그루스", "Grus" } },
			{ AvatarRebuilder.Avatar.Hakka, new string[] { "Hakka", "하카", "薄荷" } },
			{ AvatarRebuilder.Avatar.IMERIS, new string[] { "IMERIS", "이메리스", "イメリス" } },
			{ AvatarRebuilder.Avatar.Karin, new string[] { "Karin", "카린", "カリン" } },
			{ AvatarRebuilder.Avatar.Kikyo, new string[] { "Kikyo", "키쿄", "桔梗" } },
			{ AvatarRebuilder.Avatar.Kokoa, new string[] { "Kokoa", "코코아", "ここあ" } },
			{ AvatarRebuilder.Avatar.Koyuki, new string[] { "Koyuki", "코유키", "狐雪" } },
			{ AvatarRebuilder.Avatar.Kuronatu, new string[] { "Kuronatu", "쿠로나츠", "くろなつ" } },
			{ AvatarRebuilder.Avatar.Lapwing, new string[] { "Lapwing", "랩윙", "Lapwing" } },
			{ AvatarRebuilder.Avatar.Leefa, new string[] { "Leefa", "리파", "リーファ" } },
			{ AvatarRebuilder.Avatar.Leeme, new string[] { "Leeme", "리메", "リーメ" } },
			{ AvatarRebuilder.Avatar.Lime, new string[] { "Lime", "라임", "ライム" } },
			{ AvatarRebuilder.Avatar.Lunalitt, new string[] { "Lunalitt", "루나릿트", "ルーナリット" } },
			{ AvatarRebuilder.Avatar.Maki, new string[] { "Maki", "마키", "碼希" } },
			{ AvatarRebuilder.Avatar.Mamehinata, new string[] { "Mamehinata", "마메히나타", "まめひなた" } },
			{ AvatarRebuilder.Avatar.MANUKA, new string[] { "MANUKA", "마누카", "マヌカ" } },
			{ AvatarRebuilder.Avatar.Mariel, new string[] { "Mariel", "마리엘", "まりえる" } },
			{ AvatarRebuilder.Avatar.Marron, new string[] { "Marron", "마론", "マロン" } },
			{ AvatarRebuilder.Avatar.Maya, new string[] { "Maya", "마야", "舞夜" } },
			{ AvatarRebuilder.Avatar.Merino, new string[] { "Merino", "메리노", "メリノ" } },
			{ AvatarRebuilder.Avatar.Milfy, new string[] { "Milfy", "미르피", "ミルフィ" } },
			{ AvatarRebuilder.Avatar.Milk, new string[] { "Milk(New)", "밀크(신)", "ミルク（新）" } },
			{ AvatarRebuilder.Avatar.Milltina, new string[] { "Milltina", "밀티나", "ミルティナ" } },
			{ AvatarRebuilder.Avatar.Minahoshi, new string[] { "Minahoshi", "미나호시", "みなほし" } },
			{ AvatarRebuilder.Avatar.Minase, new string[] { "Minase", "미나세", "水瀬" } },
			{ AvatarRebuilder.Avatar.Mint, new string[] { "Mint", "민트", "ミント" } },
			{ AvatarRebuilder.Avatar.Mir, new string[] { "Mir", "미르", "ミール" } },
			{ AvatarRebuilder.Avatar.Mishe, new string[] { "Mishe", "미셰", "ミーシェ" } },
			{ AvatarRebuilder.Avatar.Moe, new string[] { "Moe", "모에", "萌" } },
			{ AvatarRebuilder.Avatar.Nayu, new string[] { "Nayu", "나유", "ナユ" } },
			{ AvatarRebuilder.Avatar.Platinum, new string[] { "Platinum", "플레티늄", "プラチナ" } },
			{ AvatarRebuilder.Avatar.Quiche, new string[] { "Quiche", "킷슈", "キッシュ" } },
			{ AvatarRebuilder.Avatar.Rainy, new string[] { "Rainy", "레이니", "レイニィ" } },
			{ AvatarRebuilder.Avatar.Ramune_Old, new string[] { "Ramune(Old)", "라무네(구)", "ラムネ（古）" } },
			{ AvatarRebuilder.Avatar.RINDO, new string[] { "RINDO", "린도", "竜胆" } },
			{ AvatarRebuilder.Avatar.Rue, new string[] { "Rue", "루에", "ルウ" } },
			{ AvatarRebuilder.Avatar.Rusk, new string[] { "Rusk", "러스크", "ラスク" } },
			{ AvatarRebuilder.Avatar.SELESTIA, new string[] { "SELESTIA", "셀레스티아", "セレスティア" } },
			{ AvatarRebuilder.Avatar.Sephira, new string[] { "Sephira", "세피라", "セフィラ" } },
			{ AvatarRebuilder.Avatar.Shinano, new string[] { "Shinano", "시나노", "しなの" } },
			{ AvatarRebuilder.Avatar.Shinra, new string[] { "Shinra", "신라", "森羅" } },
			{ AvatarRebuilder.Avatar.Sio, new string[] { "Sio", "시오", "しお" } },
			{ AvatarRebuilder.Avatar.Sue, new string[] { "Sue", "스우", "透羽" } },
			{ AvatarRebuilder.Avatar.Sugar, new string[] { "Sugar", "슈가", "シュガ" } },
			{ AvatarRebuilder.Avatar.Suzuhana, new string[] { "Suzuhana", "스즈하나", "すずはな" } },
			{ AvatarRebuilder.Avatar.Tien, new string[] { "Tien", "티엔", "ティエン" } },
			{ AvatarRebuilder.Avatar.TubeRose, new string[] { "TubeRose", "튜베로즈", "TubeRose" } },
			{ AvatarRebuilder.Avatar.Ukon, new string[] { "Ukon", "우콘", "右近" } },
			{ AvatarRebuilder.Avatar.Usasaki, new string[] { "Usasaki", "우사사키", "うささき" } },
			{ AvatarRebuilder.Avatar.Uzuki, new string[] { "Uzuki", "우즈키", "卯月" } },
			{ AvatarRebuilder.Avatar.Wolferia, new string[] { "Wolferia", "울페리아", "ウルフェリア" } },
			{ AvatarRebuilder.Avatar.Yoll, new string[] { "Yoll", "요루", "ヨル" } },
			{ AvatarRebuilder.Avatar.YUGI_MIYO, new string[] { "YUGI MIYO", "유기 미요", "ユギ ミヨ" } },
			{ AvatarRebuilder.Avatar.Yuuko, new string[] { "Yuuko", "유우코", "幽狐" } }
		};
	}
}
