using System.Collections.Generic;
using System.Linq;

using UnityEditor;

using static VRSuya.Core.Translator;

/*
 * VRSuya AvatarRebuilder
 * Contact : vrsuya@gmail.com // Twitter : https://twitter.com/VRSuya
 * Thanks to Dalgona. & C_Carrot & Naru & Rekorn
 */

namespace VRSuya.Installer {

	public class LanguageHelper : AvatarRebuilderEditor {

		/// <summary>요청한 아바타 이름을 설정된 언어에 맞춰 리스트를 재작성합니다.</summary>
		/// <returns>아바타 이름의 현재 설정된 언어 버전</returns>
		internal static string[] ReturnAvatarName() {
			return typeof(AvatarRebuilder.Avatar)
				.GetFields()
				.Where(Item => Item.FieldType == typeof(AvatarRebuilder.Avatar))
				.Select(Item => Item.GetValue(null))
				.Cast<AvatarRebuilder.Avatar>()
				.Where(Item => dictAvatarNames.ContainsKey(Item))
				.Select(Item => dictAvatarNames[Item][LanguageIndex])
				.ToArray();
		}

		/// <summary>요청한 아바타 이름들을 설정된 언어에 맞춰 변환합니다.</summary>
		/// <returns>요청한 아바타 이름들의 현재 설정된 언어 버전</returns>
		static readonly Dictionary<AvatarRebuilder.Avatar, string[]> dictAvatarNames = new Dictionary<AvatarRebuilder.Avatar, string[]>() {
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
			{ AvatarRebuilder.Avatar.Eku, new string[] { "Eku", "에쿠", "エク" } },
			{ AvatarRebuilder.Avatar.Emmelie, new string[] { "Emmelie", "에밀리", "Emmelie" } },
			{ AvatarRebuilder.Avatar.EYO, new string[] { "EYO", "이요", "イヨ" } },
			{ AvatarRebuilder.Avatar.Firina, new string[] { "Firina", "휘리나", "フィリナ" } },
			{ AvatarRebuilder.Avatar.Flare, new string[] { "Flare", "플레어", "フレア" } },
			{ AvatarRebuilder.Avatar.Fuzzy, new string[] { "Fuzzy", "퍼지", "ファジー" } },
			{ AvatarRebuilder.Avatar.Glaze, new string[] { "Glaze", "글레이즈", "ぐれーず" } },
			{ AvatarRebuilder.Avatar.Grus, new string[] { "Grus", "그루스", "Grus" } },
			{ AvatarRebuilder.Avatar.Hakka, new string[] { "Hakka", "하카", "薄荷" } },
			{ AvatarRebuilder.Avatar.IMERIS, new string[] { "IMERIS", "이메리스", "イメリス" } },
			{ AvatarRebuilder.Avatar.Karin, new string[] { "Karin", "카린", "カリン" } },
			{ AvatarRebuilder.Avatar.Kikyo, new string[] { "Kikyo", "키쿄", "桔梗" } },
			{ AvatarRebuilder.Avatar.Kipfel, new string[] { "Kipfel", "키펠", "キプフェル" } },
			{ AvatarRebuilder.Avatar.Kokoa, new string[] { "Kokoa", "코코아", "ここあ" } },
			{ AvatarRebuilder.Avatar.Koyuki, new string[] { "Koyuki", "코유키", "狐雪" } },
			{ AvatarRebuilder.Avatar.KUMALY, new string[] { "KUMALY", "쿠마리", "クマリ" } },
			{ AvatarRebuilder.Avatar.Kuronatu, new string[] { "Kuronatu", "쿠로나츠", "くろなつ" } },
			{ AvatarRebuilder.Avatar.Lapwing, new string[] { "Lapwing", "랩윙", "Lapwing" } },
			{ AvatarRebuilder.Avatar.Lazuli, new string[] { "Lazuli", "라줄리", "ラズリ" } },
			{ AvatarRebuilder.Avatar.Leefa, new string[] { "Leefa", "리파", "リーファ" } },
			{ AvatarRebuilder.Avatar.Leeme, new string[] { "Leeme", "리메", "リーメ" } },
			{ AvatarRebuilder.Avatar.Lime, new string[] { "Lime", "라임", "ライム" } },
			{ AvatarRebuilder.Avatar.LUMINA, new string[] { "LUMINA", "루미나", "ルミナ" } },
			{ AvatarRebuilder.Avatar.Lunalitt, new string[] { "Lunalitt", "루나릿트", "ルーナリット" } },
			{ AvatarRebuilder.Avatar.Mafuyu, new string[] { "Mafuyu", "마후유", "真冬" } },
			{ AvatarRebuilder.Avatar.Maki, new string[] { "Maki", "마키", "碼希" } },
			{ AvatarRebuilder.Avatar.Mamehinata, new string[] { "Mamehinata", "마메히나타", "まめひなた" } },
			{ AvatarRebuilder.Avatar.MANUKA, new string[] { "MANUKA", "마누카", "マヌカ" } },
			{ AvatarRebuilder.Avatar.Mariel, new string[] { "Mariel", "마리엘", "まりえる" } },
			{ AvatarRebuilder.Avatar.Marron, new string[] { "Marron", "마론", "マロン" } },
			{ AvatarRebuilder.Avatar.Maya, new string[] { "Maya", "마야", "舞夜" } },
			{ AvatarRebuilder.Avatar.MAYO, new string[] { "MAYO", "마요", "まよ" } },
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
			{ AvatarRebuilder.Avatar.Nehail, new string[] { "Nehail", "네하일", "ネハイル" } },
			{ AvatarRebuilder.Avatar.Nochica, new string[] { "Nochica", "노치카", "ノーチカ" } },
			{ AvatarRebuilder.Avatar.Platinum, new string[] { "Platinum", "플레티늄", "プラチナ" } },
			{ AvatarRebuilder.Avatar.Plum, new string[] { "Plum", "플럼", "プラム" } },
			{ AvatarRebuilder.Avatar.Pochimaru, new string[] { "Pochimaru", "포치마루", "ぽちまる" } },
			{ AvatarRebuilder.Avatar.Quiche, new string[] { "Quiche", "킷슈", "キッシュ" } },
			{ AvatarRebuilder.Avatar.Rainy, new string[] { "Rainy", "레이니", "レイニィ" } },
			{ AvatarRebuilder.Avatar.Ramune, new string[] { "Ramune", "라무네", "ラムネ" } },
			{ AvatarRebuilder.Avatar.Ramune_Old, new string[] { "Ramune(Old)", "라무네(구)", "ラムネ（古）" } },
			{ AvatarRebuilder.Avatar.RINDO, new string[] { "RINDO", "린도", "竜胆" } },
			{ AvatarRebuilder.Avatar.Rokona, new string[] { "Rokona", "로코나", "ロコナ" } },
			{ AvatarRebuilder.Avatar.Rue, new string[] { "Rue", "루에", "ルウ" } },
			{ AvatarRebuilder.Avatar.Rurune, new string[] { "Rurune", "루루네", "ルルネ" } },
			{ AvatarRebuilder.Avatar.Rusk, new string[] { "Rusk", "러스크", "ラスク" } },
			{ AvatarRebuilder.Avatar.SELESTIA, new string[] { "SELESTIA", "셀레스티아", "セレスティア" } },
			{ AvatarRebuilder.Avatar.Sephira, new string[] { "Sephira", "세피라", "セフィラ" } },
			{ AvatarRebuilder.Avatar.Shinano, new string[] { "Shinano", "시나노", "しなの" } },
			{ AvatarRebuilder.Avatar.Shinra, new string[] { "Shinra", "신라", "森羅" } },
			{ AvatarRebuilder.Avatar.SHIRAHA, new string[] { "SHIRAHA", "시라하", "シラハ" } },
			{ AvatarRebuilder.Avatar.Shiratsume, new string[] { "Shiratsume", "시라츠메", "しらつめ" } },
			{ AvatarRebuilder.Avatar.Sio, new string[] { "Sio", "시오", "しお" } },
			{ AvatarRebuilder.Avatar.Sue, new string[] { "Sue", "스우", "透羽" } },
			{ AvatarRebuilder.Avatar.Sugar, new string[] { "Sugar", "슈가", "シュガ" } },
			{ AvatarRebuilder.Avatar.Suzuhana, new string[] { "Suzuhana", "스즈하나", "すずはな" } },
			{ AvatarRebuilder.Avatar.Tien, new string[] { "Tien", "티엔", "ティエン" } },
			{ AvatarRebuilder.Avatar.TubeRose, new string[] { "TubeRose", "튜베로즈", "TubeRose" } },
			{ AvatarRebuilder.Avatar.Ukon, new string[] { "Ukon", "우콘", "右近" } },
			{ AvatarRebuilder.Avatar.Usasaki, new string[] { "Usasaki", "우사사키", "うささき" } },
			{ AvatarRebuilder.Avatar.Uzuki, new string[] { "Uzuki", "우즈키", "卯月" } },
			{ AvatarRebuilder.Avatar.VIVH, new string[] { "VIVH", "비브", "ビィブ" } },
			{ AvatarRebuilder.Avatar.Wolferia, new string[] { "Wolferia", "울페리아", "ウルフェリア" } },
			{ AvatarRebuilder.Avatar.Yoll, new string[] { "Yoll", "요루", "ヨル" } },
			{ AvatarRebuilder.Avatar.YUGI_MIYO, new string[] { "YUGI MIYO", "유기 미요", "ユギ ミヨ" } },
			{ AvatarRebuilder.Avatar.Yuuko, new string[] { "Yuuko", "유우코", "幽狐" } }
			// 검색용 신규 아바타 추가 위치
		};
	}
}
