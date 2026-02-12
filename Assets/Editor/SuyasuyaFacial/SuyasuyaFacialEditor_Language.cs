using System.Collections.Generic;

/*
 * VRSuya Suyasuya Facial Editor
 * Contact : vrsuya@gmail.com // Twitter : https://twitter.com/VRSuya
 */

namespace com.vrsuya.suyasuyafacial {

	public class LanguageHelper : SuyasuyaFacialEditor {

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

		// 영어 사전 데이터
		static Dictionary<string, string> String_English = new Dictionary<string, string>() {
			{ "String_Language", "Language" },
			{ "String_Debug", "Debug" },
			{ "String_Avatar", "Avatar Type" },
			{ "String_TargetAvatar", "Target Avatar" },
			{ "String_TargetMesh", "Face Mesh" },
			{ "String_TargetFXLayer", "FX Layer" },
			{ "String_TargetAnimations", "Target Animation Clips" },
			{ "String_TargetBlendShape", "Facial Blendshapes with Value" },
			{ "String_TargetAnimationBlendShape", "Animated Face Blendshapes" },
			{ "String_Reload", "Reload" },
			{ "String_UpdateAnimations", "Update Animation Clips" },

			// 에러 코드
			{ "NO_SHAPEKEY", "No shapekeys with values set" },
			{ "NO_ANIMSHAPEKEY", "There are no face-related shape keys in the FX layer animation" },
			{ "NO_AVATAR", "No Avatar is selected" },
			{ "NO_FACEMESH", "Face mesh not found" },
			{ "COMPLETED", "{0} blendshapes have been added to the animation clip" }
		};

		// 한국어 사전 데이터
		static Dictionary<string, string> String_Korean = new Dictionary<string, string>() {
			{ "String_Language", "언어" },
			{ "String_Debug", "디버그" },
			{ "String_TargetAvatar", "대상 아바타" },
			{ "String_TargetMesh", "얼굴 메쉬" },
			{ "String_TargetFXLayer", "FX 레이어" },
			{ "String_TargetAnimations", "대상 애니메이션 클립" },
			{ "String_TargetBlendShape", "적용된 얼굴 쉐이프키" },
			{ "String_TargetAnimationBlendShape", "애니메이션 얼굴 쉐이프키" },
			{ "String_Reload", "다시 불러오기" },
			{ "String_UpdateAnimations", "애니메이션 업데이트" },

			// 에러 코드
			{ "NO_SHAPEKEY", "값이 설정된 쉐이프키가 없습니다" },
			{ "NO_ANIMSHAPEKEY", "FX 레이어의 애니메이션에서 얼굴 관련 쉐이프키가 없습니다" },
			{ "NO_AVATAR", "아바타가 지정되지 않았습니다" },
			{ "NO_FACEMESH", "얼굴 메쉬를 찾을 수 없습니다" },
			{ "COMPLETED", "{0}개의 블렌드쉐이프 애니메이션 키가 추가되었습니다" }
		};

		// 일본어 사전 데이터
		static Dictionary<string, string> String_Japanese = new Dictionary<string, string>() {
			{ "String_Language", "言語" },
			{ "String_Debug", "デバッグ" },
			{ "String_TargetAvatar", "対象アバター" },
			{ "String_TargetMesh", "顔メッシュ" },
			{ "String_TargetFXLayer", "FXレイヤー" },
			{ "String_TargetAnimations", "対象アニメーションクリップ" },
			{ "String_TargetBlendShape", "適用された顔のシェイプキー" },
			{ "String_TargetAnimationBlendShape", "アニメーション顔シェイプキー" },
			{ "String_Reload", "リロード" },
			{ "String_UpdateAnimations", "アニメーション·アップデート" },

			// 에러 코드
			{ "NO_SHAPEKEY", "値が設定されたシェイプキーがありません" },
			{ "NO_ANIMSHAPEKEY", "FXレイヤーのアニメーションで顔関連のシェイプキーがありません" },
			{ "NO_AVATAR", "アバターが指定されていません" },
			{ "NO_FACEMESH", "顔のメッシュが見つかりません" },
			{ "COMPLETED", "{0}個のブレンドシェイプアニメーションキーが追加されました" }
		};
	}
}
