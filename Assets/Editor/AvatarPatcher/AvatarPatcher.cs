using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using UnityEditor;
using UnityEngine;

using Newtonsoft.Json.Linq;

using VRSuya.Core;
using static VRSuya.Core.Translator;

using Avatar = VRSuya.Core.Avatar;
using Animator = UnityEngine.Animator;

/*
 * VRSuya AvatarPatcher
 * Contact : vrsuya@gmail.com // Twitter : https://twitter.com/VRSuya
 */

namespace VRSuya.Installer {

	public class AvatarPatcher : EditorWindow {

		public GameObject AvatarGameObject;
		public TextAsset JSON_Asset;

		Animator AvatarAnimator;

		const float Threshold = 0.001f;
		const float BorderX = 30f;

		[MenuItem("Tools/VRSuya/Installer/AvatarPatcher", priority = 1000)]
		static void CreateWindow() {
			AvatarPatcher AppWindow = GetWindowWithRect<AvatarPatcher>(new Rect(0, 0, 400, 180), true, "VRSuya AvatarPatcher");
			AppWindow.Initialize();
		}

		void OnGUI() {
			EditorGUILayout.Space(EditorGUIUtility.singleLineHeight);
			EditorGUILayout.BeginHorizontal();
			GUILayout.Space(BorderX);
			EditorGUIUtility.labelWidth = 100f;
			LanguageIndex = EditorGUILayout.Popup(GetTranslatedString("String_Language"), LanguageIndex, LanguageOption);
			GUILayout.Space(BorderX);
			EditorGUILayout.EndHorizontal();
			EditorGUILayout.Space(EditorGUIUtility.singleLineHeight);
			EditorGUILayout.BeginHorizontal();
			GUILayout.Space(BorderX);
			AvatarGameObject = (GameObject)EditorGUILayout.ObjectField(GetTranslatedString("String_TargetAvatar"), AvatarGameObject, typeof(GameObject), true);
			GUILayout.Space(BorderX);
			EditorGUILayout.EndHorizontal();
			EditorGUILayout.BeginHorizontal();
			GUILayout.Space(BorderX);
			JSON_Asset = (TextAsset)EditorGUILayout.ObjectField("JSON", JSON_Asset, typeof(TextAsset), false);
			GUILayout.Space(BorderX);
			EditorGUILayout.EndHorizontal();
			EditorGUILayout.Space(EditorGUIUtility.singleLineHeight);
			EditorGUILayout.BeginHorizontal();
			GUILayout.Space(BorderX);
			GUI.backgroundColor = Color.cyan;
			if (GUILayout.Button(GetTranslatedString("String_ReplaceAvatar"), GUILayout.Height(40))) {
				RequestPatchAvatar();
				Close();
			}
			GUI.backgroundColor = Color.white;
			GUILayout.Space(BorderX);
			EditorGUILayout.EndHorizontal();
		}

		void Initialize() {
			Avatar AvatarInstance = new Avatar();
			AvatarGameObject = AvatarInstance.GetAvatarGameObject();
		}

		public GameObject RequestPatchAvatar() {
			if (VerifyVariable(out GameObject NewAvatarGameObject)) {
				JObject JSON_Object = JObject.Parse(JSON_Asset.text);
				string AvatarName = AvatarGameObject.name;
				AvatarGameObject.name = $"{AvatarName} (Backup)";
				AvatarGameObject.SetActive(false);
				NewAvatarGameObject.name = AvatarName;
				NewAvatarGameObject.transform.SetSiblingIndex(AvatarGameObject.transform.GetSiblingIndex() + 1);
				Dictionary<string, Transform> NewBoneTransforms = GetBoneTransfom(NewAvatarGameObject, JSON_Object);
				if (PatchAvatar(AvatarGameObject, NewAvatarGameObject, JSON_Object, NewBoneTransforms)) {
					EditorUtility.DisplayDialog("VRSuya AvatarPatcher",
						string.Format(GetTranslatedString("COMPLETED_PATCH"), AvatarName),
						GetTranslatedString("String_Okay")
					);
					return NewAvatarGameObject;
				}
			}
			return null;
		}

		bool VerifyVariable(out GameObject NewAvatarGameObject) {
			NewAvatarGameObject = null;
			if (!AvatarGameObject) {
				return false;
			}
			if (!JSON_Asset) {
				return false;
			}
			DuplicateGameObject DuplicateInstance = new DuplicateGameObject();
			NewAvatarGameObject = DuplicateInstance.DuplicateGameObjectInstance(AvatarGameObject);
			if (!NewAvatarGameObject) {
				return false;
			}
			NewAvatarGameObject.TryGetComponent(out Animator AvatarAnimatorComponent);
			if (AvatarAnimatorComponent) {
				AvatarAnimator = AvatarAnimatorComponent;
			} else {
				DestroyImmediate(NewAvatarGameObject);
				return false;
			}
			JObject JSON_Object = JObject.Parse(JSON_Asset.text);
			JToken JSONVersion_Data = JSON_Object["JsonVersion"];
			int JSON_Version = (JSONVersion_Data != null) ? JSONVersion_Data.Value<int>() : 0;
			if (JSON_Version >= 2) {
				return true;
			} else {
				return false;
			}
		}

		Dictionary<string, Transform> GetBoneTransfom(GameObject TargetGameObject, JObject JSON_Object) {
			Dictionary<string, Transform> JSON_BoneTransforms = new Dictionary<string, Transform>();
			JArray Bone_Data = (JArray)JSON_Object["Bones"];
			if (Bone_Data == null) return JSON_BoneTransforms;
			Transform[] AvatarHumanoidTransforms = Enum.GetValues(typeof(HumanBodyBones))
				.Cast<HumanBodyBones>()
				.Where(Item => Item != HumanBodyBones.LastBone)
				.Select(Item => AvatarAnimator.GetBoneTransform(Item))
				.Where(Item => Item != null)
				.ToArray();
			Transform[] AllAvatarTransforms = TargetGameObject.GetComponentsInChildren<Transform>(true);
			List<Transform> NewBoneTransforms = new List<Transform>();
			foreach (JToken TargetBone in Bone_Data) {
				string BoneName = TargetBone["Name"].ToString();
				GameObject NewGameObject = new GameObject(BoneName);
				Transform NewTransform = NewGameObject.transform;
				NewTransform.SetParent(AvatarGameObject.transform);
				NewBoneTransforms.Add(NewTransform);
			}
			foreach (JToken TargetBone in Bone_Data) {
				string BoneName = TargetBone["Name"].ToString();
				string ParentName = TargetBone["Parent"].ToString();
				Transform NewTransform = NewBoneTransforms.First(Item => Item.gameObject.name == BoneName);
				JArray HeadPosition_Data = (JArray)TargetBone["Head"];
				JArray TailPosition_Data = (JArray)TargetBone["Tail"];
				Vector3 HeadPosition = GetBoneTransform(HeadPosition_Data);
				Vector3 TailPosition = GetBoneTransform(TailPosition_Data);
				Transform ParentTransform = AllAvatarTransforms.FirstOrDefault(Item => Item.name == ParentName);
				if (NewBoneTransforms.Any(Item => Item.name == ParentName)) {
					ParentTransform = NewBoneTransforms.First(Item => Item.name == ParentName);
				} else if (AvatarHumanoidTransforms.Any(Item => Item.name == ParentName)) {
					ParentTransform = AvatarHumanoidTransforms.First(Item => Item.name == ParentName);
				} else {
					ParentTransform = AllAvatarTransforms.FirstOrDefault(Item => Item.name == ParentName);
				}
				if (ParentTransform) {
					NewTransform.SetParent(ParentTransform);
					NewTransform.position = TargetGameObject.transform.TransformPoint(HeadPosition);
					Vector3 BoneVector = (TargetGameObject.transform.TransformPoint(TailPosition) - NewTransform.position).normalized;
					if (BoneVector != Vector3.zero) {
						NewTransform.rotation = TargetGameObject.transform.rotation * Quaternion.FromToRotation(Vector3.up, BoneVector);
					}
					JSON_BoneTransforms.Add(BoneName, NewTransform);
				}
			}
			return JSON_BoneTransforms;
		}

		Vector3 GetBoneTransform(JArray JSON_Array) {
			float X_Blender = (float)JSON_Array[0];
			float Y_Blender = (float)JSON_Array[1];
			float Z_Blender = (float)JSON_Array[2];
			float X_Unity = -X_Blender;
			float Y_Unity = Z_Blender;
			float Z_Unity = -Y_Blender;
			return new Vector3(X_Unity, Y_Unity, Z_Unity);
		}

		bool PatchAvatar(GameObject OldAvatarGameObject, GameObject NewAvatarGameObject, JObject JSON_Object, Dictionary<string, Transform> NewBoneTransforms) {
			JToken JSON_Token = JSON_Object["TargetObjects"];
			if (JSON_Token == null) return false;
			string AvatarAssetPath = AssetDatabase.GetAssetPath(AvatarAnimator.avatar);
			SkinnedMeshRenderer[] AvatarSkinnedMeshRenderers = NewAvatarGameObject
					.GetComponentsInChildren<SkinnedMeshRenderer>(true)
					.Where(Item => AssetDatabase.GetAssetPath(Item.sharedMesh) == AvatarAssetPath)
					.ToArray();
			foreach (JToken TargetToken in JSON_Token) {
				string TargetName = TargetToken["Name"].ToString();
				SkinnedMeshRenderer TargetSkinnedMeshRenderer = AvatarSkinnedMeshRenderers.FirstOrDefault(Item => Item.gameObject.name == TargetName);
				if (!TargetSkinnedMeshRenderer) TargetSkinnedMeshRenderer = NewAvatarGameObject.GetComponentsInChildren<SkinnedMeshRenderer>(true).FirstOrDefault(Item => Item.name == TargetName);
				if (TargetSkinnedMeshRenderer) {
					Mesh OldMesh = TargetSkinnedMeshRenderer.sharedMesh;
					Mesh NewMesh = Instantiate(OldMesh);
					NewMesh.name = $"VRSuya_{OldMesh.name}";
					Vector2[] NewMeshUV = NewMesh.uv;
					List<Transform> BoneTransforms = TargetSkinnedMeshRenderer.bones.ToList();
					JToken WeightsToken = TargetToken["Weights"];
					foreach (JProperty TargetProperty in WeightsToken.Cast<JProperty>()) {
						string TargetWeightName = TargetProperty.Name;
						if (NewBoneTransforms.TryGetValue(TargetWeightName, out Transform NewBoneTransform)) {
							if (!BoneTransforms.Contains(NewBoneTransform)) {
								BoneTransforms.Add(NewBoneTransform);
							}
						}
					}
					TargetSkinnedMeshRenderer.bones = BoneTransforms.ToArray();
					Matrix4x4[] NewBindPose = new Matrix4x4[BoneTransforms.Count];
					for (int Index = 0; Index < BoneTransforms.Count; Index++) {
						NewBindPose[Index] = BoneTransforms[Index].worldToLocalMatrix * TargetSkinnedMeshRenderer.transform.localToWorldMatrix;
					}
					NewMesh.bindposes = NewBindPose;
					BoneWeight[] NewBoneWeights = NewMesh.boneWeights;
					Dictionary<int, List<VertexWeight>> VertexWeightList = new Dictionary<int, List<VertexWeight>>();
					for (int Index = 0; Index < NewBoneWeights.Length; Index++) {
						BoneWeight TargetBoneWeight = NewBoneWeights[Index];
						List<VertexWeight> NewVertexWeights = new List<VertexWeight>();
						if (TargetBoneWeight.weight0 > 0) NewVertexWeights.Add(new VertexWeight(TargetBoneWeight.boneIndex0, TargetBoneWeight.weight0));
						if (TargetBoneWeight.weight1 > 0) NewVertexWeights.Add(new VertexWeight(TargetBoneWeight.boneIndex1, TargetBoneWeight.weight1));
						if (TargetBoneWeight.weight2 > 0) NewVertexWeights.Add(new VertexWeight(TargetBoneWeight.boneIndex2, TargetBoneWeight.weight2));
						if (TargetBoneWeight.weight3 > 0) NewVertexWeights.Add(new VertexWeight(TargetBoneWeight.boneIndex3, TargetBoneWeight.weight3));
						VertexWeightList[Index] = NewVertexWeights;
					}
					foreach (JProperty TargetProperty in WeightsToken.Cast<JProperty>()) {
						string TargetBoneName = TargetProperty.Name;
						if (NewBoneTransforms.TryGetValue(TargetBoneName, out Transform NewBoneTransform)) {
							int TargetBoneIndex = BoneTransforms.IndexOf(NewBoneTransform);
							JArray WeightDatas = (JArray)TargetProperty.Value;
							List<UVWeight> UVWeightList = WeightDatas
								.Select(Item => new UVWeight(new Vector2((float)Item["Position"][0], (float)Item["Position"][1]), (float)Item["Weight"]))
								.ToList();
							for (int Index = 0; Index < NewMeshUV.Length; Index++) {
								Vector2 TargetUVPosition = NewMeshUV[Index];
								var TargetUVWeight = UVWeightList
									.Select(Item => new {
										Data = Item,
										Distance = Vector2.Distance(TargetUVPosition, Item.UVPosition)
									})
									.Where(Item => Item.Distance < Threshold)
									.OrderBy(Item => Item.Distance)
									.FirstOrDefault();
								if (TargetUVWeight != null && TargetUVWeight.Data.WeightValue > 0) {
									if (!VertexWeightList.ContainsKey(Index)) {
										VertexWeightList[Index] = new List<VertexWeight>();
									}
									VertexWeight TargetVertexWeight = VertexWeightList[Index].FirstOrDefault(Item => Item.VertexIndex == TargetBoneIndex);
									if (TargetVertexWeight != null) {
										TargetVertexWeight.WeightValue += TargetUVWeight.Data.WeightValue;
									} else {
										VertexWeightList[Index].Add(new VertexWeight(TargetBoneIndex, TargetUVWeight.Data.WeightValue));
									}
								}
							}
						}
					}
					for (int Index = 0; Index < NewBoneWeights.Length; Index++) {
						if (VertexWeightList.TryGetValue(Index, out List<VertexWeight> TargetVertexWeights)) {
							TargetVertexWeights = TargetVertexWeights.OrderByDescending(Item => Item.WeightValue).Take(4).ToList();
							BoneWeight NewBoneWeight = new BoneWeight();
							if (TargetVertexWeights.Count > 0) { NewBoneWeight.boneIndex0 = TargetVertexWeights[0].VertexIndex; NewBoneWeight.weight0 = TargetVertexWeights[0].WeightValue; }
							if (TargetVertexWeights.Count > 1) { NewBoneWeight.boneIndex1 = TargetVertexWeights[1].VertexIndex; NewBoneWeight.weight1 = TargetVertexWeights[1].WeightValue; }
							if (TargetVertexWeights.Count > 2) { NewBoneWeight.boneIndex2 = TargetVertexWeights[2].VertexIndex; NewBoneWeight.weight2 = TargetVertexWeights[2].WeightValue; }
							if (TargetVertexWeights.Count > 3) { NewBoneWeight.boneIndex3 = TargetVertexWeights[3].VertexIndex; NewBoneWeight.weight3 = TargetVertexWeights[3].WeightValue; }
							NewBoneWeights[Index] = NewBoneWeight;
						}
					}
					NewMesh.boneWeights = NewBoneWeights;
					string NewAssetPath = SaveMeshAsset(OldMesh, NewMesh, NewAvatarGameObject.name, TargetName);
					if (!string.IsNullOrEmpty(NewAssetPath)) {
						Mesh LoadedSavedMeshAsset = AssetDatabase.LoadAssetAtPath<Mesh>(NewAssetPath);
						TargetSkinnedMeshRenderer.sharedMesh = LoadedSavedMeshAsset;
					}
				}
			}
			return true;
		}

		string SaveMeshAsset(Mesh OldMesh, Mesh NewMesh, string TargetName, string TargetMeshName) {
			string MeshAssetPath = AssetDatabase.GetAssetPath(OldMesh);
			if (string.IsNullOrEmpty(MeshAssetPath)) {
				MeshAssetPath = "Assets/VRSuya/Export";
			} else {
				MeshAssetPath = Path.GetDirectoryName(MeshAssetPath);
			}
			string NewDirectoryPath = Path.Combine(MeshAssetPath, "VRSuya");
			if (!AssetDatabase.IsValidFolder(NewDirectoryPath)) {
				AssetDatabase.CreateFolder(MeshAssetPath, "VRSuya");
			}
			string Date = DateTime.Now.ToString("yyMMdd");
			string NewAssetName = $"VRSuya_{TargetName}_{TargetMeshName}_{Date}.asset";
			string NewAssetPath = Path.Combine(NewDirectoryPath, NewAssetName).Replace("\\", "/");
			AssetDatabase.CreateAsset(NewMesh, NewAssetPath);
			AssetDatabase.SaveAssets();
			AssetDatabase.Refresh();
			return NewAssetPath;
		}

		class VertexWeight {

			public int VertexIndex;
			public float WeightValue;

			public VertexWeight(int TargetVertexIndex, float TargetWeightValue) {
				VertexIndex = TargetVertexIndex;
				WeightValue = TargetWeightValue;
			}
		}

		class UVWeight {

			public Vector2 UVPosition;
			public float WeightValue;

			public UVWeight(Vector2 TargetUVPosition, float TargetWeightValue) {
				UVPosition = TargetUVPosition;
				WeightValue = TargetWeightValue;
			}
		}
	}
}