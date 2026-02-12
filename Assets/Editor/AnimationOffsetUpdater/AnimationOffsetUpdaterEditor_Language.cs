using System.Collections.Generic;
using System.Linq;

using UnityEditor;

/*
 * VRSuya Animation Offset Updater Editor for Mogumogu Project
 * Contact : vrsuya@gmail.com // Twitter : https://twitter.com/VRSuya
 */

namespace com.vrsuya.animationoffsetupdater {

	public class LanguageHelper : AnimationOffsetUpdaterEditor {

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
		internal static string[] ReturnAvatarAuthorName(SerializedProperty AvatarAuthorListProperty) {
			int AvatarAuthorCount = AvatarAuthorListProperty.arraySize;
			AnimationOffsetUpdater.AvatarAuthor[] AvatarAuthorNames = new AnimationOffsetUpdater.AvatarAuthor[AvatarAuthorCount];
			for (int Index = 0; Index < AvatarAuthorCount; Index++) {
				SerializedProperty ArrayItem = AvatarAuthorListProperty.GetArrayElementAtIndex(Index);
				string AvatarAuthorEnumName = ArrayItem.enumNames[ArrayItem.enumValueIndex];
				AvatarAuthorNames[Index] = (AnimationOffsetUpdater.AvatarAuthor)System.Enum.Parse(typeof(AnimationOffsetUpdater.AvatarAuthor), AvatarAuthorEnumName);
			}
			return AvatarAuthorNames
				.Where((AvatarAuthorName) => dictAvatarAuthorNames.ContainsKey(AvatarAuthorName))
				.Select((AvatarAuthorName) => dictAvatarAuthorNames[AvatarAuthorName][LanguageIndex])
				.ToArray();
		}

		// 영어 사전 데이터
		static Dictionary<string, string> String_English = new Dictionary<string, string>() {
			{ "String_Language", "Language" },
			{ "String_Debug", "Debug" },
			{ "String_TargetAvatar", "Target Avatar" },
			{ "String_AvatarAuthor", "Avatar Author" },
			{ "String_AnimationClip", "Animation Clip" },
			{ "String_AnimationOrigin", "Animation Origin" },
			{ "String_AvatarOrigin", "Avatar Origin" },
			{ "String_AnimationStrength", "Animation Strength" },
			{ "String_GetPosition", "Get cheek bone position" },
			{ "String_UpdateAnimation", "Update Animation" },

			// 성공 코드
			{ "COMPLETED_GETPOSITION", "Imported cheek bone origin position" },
			{ "COMPLETED_UPDATE", "Updated the offset of the animation clip" },

			// 에러 코드
			{ "NO_ANIMATOR", "Not found Animator Component in the Avatar!" },
			{ "NO_CLIPS", "There is no animation clip to update!" },
			{ "NO_CHEEKBONE", "Not found any cheek bone in the Avatar!" }
		};

		// 한국어 사전 데이터
		static Dictionary<string, string> String_Korean = new Dictionary<string, string>() {
			{ "String_Language", "언어" },
			{ "String_Debug", "디버그" },
			{ "String_TargetAvatar", "대상 아바타" },
			{ "String_AvatarAuthor", "아바타 제작자" },
			{ "String_AnimationClip", "애니메이션 클립" },
			{ "String_AnimationOrigin", "애니메이션 본 원점" },
			{ "String_AvatarOrigin", "아바타 볼 원점" },
			{ "String_AnimationStrength", "애니메이션 강도" },
			{ "String_GetPosition", "볼 데이터 가져오기" },
			{ "String_UpdateAnimation", "애니메이션 업데이트" },

			// 성공 코드
			{ "COMPLETED_GETPOSITION", "볼 위치 데이터를 가져왔습니다." },
			{ "COMPLETED_UPDATE", "애니메이션 클립의 오프셋을 업데이트 하였습니다" },

			// 에러 코드
			{ "NO_ANIMATOR", "아바타에서 애니메이터를 찾을 수 없습니다!" },
			{ "NO_CLIPS", "작업할 애니메이션 클립이 없습니다!" },
			{ "NO_CHEEKBONE", "아바타에서 볼 본을 찾을 수 없습니다!" }
		};

		// 일본어 사전 데이터
		static Dictionary<string, string> String_Japanese = new Dictionary<string, string>() {
			{ "String_Language", "言語" },
			{ "String_Debug", "デバッグ" },
			{ "String_TargetAvatar", "対象アバター" },
			{ "String_AvatarAuthor", "アバター製作者" },
			{ "String_AnimationClip", "アニメーション·クリップ" },
			{ "String_AnimationOrigin", "アニメーションほっぺの原点" },
			{ "String_AvatarOrigin", "アバターほっぺの原点" },
			{ "String_AnimationStrength", "アニメーション強盗" },
			{ "String_GetPosition", "ほっぺデータのインポート" },
			{ "String_UpdateAnimation", "アニメーション·アップデート" },

			// 성공 코드
			{ "COMPLETED_GETPOSITION", "ほっぺ位置データを取得しました" },
			{ "COMPLETED_UPDATE", "アニメーション·クリップのオフセットを更新しました" },

			// 에러 코드
			{ "NO_ANIMATOR", "アバターにアニメーターが見つかりません" },
			{ "NO_CLIPS", "作業するアニメーション·クリップがありません！" },
			{ "NO_CHEEKBONE", "アバターにほっぺの骨が見つかりません！" },
		};

		/// <summary>요청한 아바타 제작자 이름들을 설정된 언어에 맞춰 변환합니다.</summary>
		/// <returns>요청한 아바타 제작자 이름들의 현재 설정된 언어 버전</returns>
		static readonly Dictionary<AnimationOffsetUpdater.AvatarAuthor, string[]> dictAvatarAuthorNames = new Dictionary<AnimationOffsetUpdater.AvatarAuthor, string[]>() {
			{ AnimationOffsetUpdater.AvatarAuthor.General, new string[] { "General", "일반", "一般" } },
			{ AnimationOffsetUpdater.AvatarAuthor.ChocolateRice, new string[] { "Chocolate rice", "초콜렛 라이스", "チョコレートライス" } },
			{ AnimationOffsetUpdater.AvatarAuthor.JINGO, new string[] { "JINGO", "진권", "ジンゴ" } },
			{ AnimationOffsetUpdater.AvatarAuthor.Komado, new string[] { "Komado", "코마도", "こまど" } },
			{ AnimationOffsetUpdater.AvatarAuthor.Plusone, new string[] { "Plusone", "플러스원", "ぷらすわん" } },
		};
	}
}
