/*
*	Copyright (c) 2017-2019. RainyRizzle. All rights reserved
*	Contact to : https://www.rainyrizzle.com/ , contactrainyrizzle@gmail.com
*
*	This file is part of [AnyPortrait].
*
*	AnyPortrait can not be copied and/or distributed without
*	the express perission of [Seungjik Lee].
*
*	Unless this file is downloaded from the Unity Asset Store or RainyRizzle homepage, 
*	this file and its users are illegal.
*	In that case, the act may be subject to legal penalties.
*/

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

using AnyPortrait;

namespace AnyPortrait
{
	//Material Batch를 위해서 Material정보를 ID와 함께 저장해놓는다.
	//Bake 후에 다시 Link를 하면 이 정보를 참조하여 Material를 가져간다.
	//CustomShader를 포함하는 반면 Clipped Mesh는 예외로 둔다. (알아서 Shader로 만들 것)
	/// <summary>
	/// Material manager class for batch rendering
	/// This is done automatically, so it is recommended that you do not control it with scripts.
	/// </summary>
	[Serializable]
	public class apOptBatchedMaterial
	{
		// Members
		//----------------------------------------------------
		[Serializable]
		public class MaterialUnit
		{
			[SerializeField]
			public int _uniqueID = -1;

			//수정 : 직렬화 안되도록
			[NonSerialized]
			public Material _material = null;

			//추가 : 원래의 재질 속성을 저장하기 위한 원본 재질. 실제로 적용되진 않고, Reset 용도로 사용한다.
			[NonSerialized]
			public Material _material_Original = null;

			//일종의 키값이 되는 데이터
			[SerializeField]
			private Texture2D _texture = null;
			

			[SerializeField]
			public int _textureID = -1;

			[SerializeField]
			private Shader _shader = null;

			[NonSerialized, NonBackupField]
			public List<apOptMesh> _linkedMeshes = new List<apOptMesh>();

			//추가 12.13 : 일괄 변형 요청 : 텍스쳐, 색상 등등
			//Custom이 속성 변경 요청이 아니라면, 기본값과 비교해서 자동 리셋할 수도 있다.
			[NonSerialized]
			public bool _isRequested_Texture = false;

			[NonSerialized]
			public bool _isRequested_Color = false;

			[NonSerialized]
			public bool _isRequested_Custom = false;
			
			[NonSerialized]
			private int _shaderID_MainTex = -1;
			[NonSerialized]
			private int _shaderID_Color = -1;
			

			public MaterialUnit()
			{
				_linkedMeshes.Clear();
			}

			public MaterialUnit(int uniqueID, Texture2D texture, int textureID, Shader shader)
			{
				_uniqueID = uniqueID;

				_texture = texture;
				_textureID = textureID;
				_shader = shader;

				//수정 : 생성할 때에는 Material을 생성하지 않는다.
				//_material = new Material(_shader);
				//_material.SetTexture("_MainTex", _texture);
				//_material.SetColor("_Color", new Color(0.5f, 0.5f, 0.5f, 1.0f));
			}

			public bool IsEqualMaterial(Texture2D texture, int textureID, Shader shader)
			{
				return _texture == texture
					&& _textureID == textureID
					&& _shader == shader;
			}

			public void MakeMaterial()
			{
				_material = new Material(_shader);

				_shaderID_MainTex = Shader.PropertyToID("_MainTex");
				_shaderID_Color = Shader.PropertyToID("_Color");

				_material.SetTexture(_shaderID_MainTex, _texture);
				_material.SetColor(_shaderID_Color, new Color(0.5f, 0.5f, 0.5f, 1.0f));

				_material_Original = new Material(_material);

				ResetRequestProperties();
			}

			public void LinkMesh(apOptMesh optMesh)
			{
				if(!_linkedMeshes.Contains(optMesh))
				{
					_linkedMeshes.Add(optMesh);
				}
			}

			//초기화시에는 연결된 메시 리스트를 날린다.
			public void ClearLinkedMeshes()
			{
				_linkedMeshes.Clear();
			}




			//일괄 변경 요청 함수들
			//public void RefreshLinkedMeshes()
			//{
			//	apOptMesh curMesh = null;
			//	for (int i = 0; i < _linkedMeshes.Count; i++)
			//	{
			//		curMesh = _linkedMeshes[i];
			//		if(curMesh == null)
			//		{
			//			continue;
			//		}

			//		//Batched 재질이 갱신되었다는 이벤트 호출
			//		curMesh.OnBatchedMaterialPropertiesChanged();
			//	}
			//}


			/// <summary>
			/// 변경 요청을 리셋한다.
			/// </summary>
			public void ResetRequestProperties()
			{
				_isRequested_Texture = false;
				_isRequested_Color = false;
				_isRequested_Custom = false;

				//원래 프로퍼티에서 복사를 하자.
				_material.CopyPropertiesFromMaterial(_material_Original);

				apOptMesh curMesh = null;
				for (int i = 0; i < _linkedMeshes.Count; i++)
				{
					curMesh = _linkedMeshes[i];
					if (curMesh == null)
					{
						continue;
					}

					//각 메시의 Instanced Material도 Batch Material과 동일한 값을 가지도록 한다.
					curMesh.SyncMaterialPropertyByBatch_Reset(_material_Original);
				}
			}

			/// <summary>
			/// 기본 텍스쳐(_MainTex) 변경 요청. 기본값인 경우 요청을 초기화한다.
			/// </summary>
			/// <param name="texture"></param>
			public void RequestImage(Texture2D texture)
			{

				if (texture == _texture)
				{
					//원래대로 돌아왔다.
					_isRequested_Texture = false;
				}
				else
				{
					_isRequested_Texture = true;
				}
				_material.SetTexture(_shaderID_MainTex, texture);

				apOptMesh curMesh = null;
				for (int i = 0; i < _linkedMeshes.Count; i++)
				{
					curMesh = _linkedMeshes[i];
					if (curMesh == null)
					{
						continue;
					}

					//각 메시의 Instanced Material도 Batch Material과 동일한 값을 가지도록 한다.
					curMesh.SyncMaterialPropertyByBatch_Texture(texture);
				}

			}

			/// <summary>
			/// 기본 색상(_Color) 변경 요청. 기본값인 경우 요청을 초기화한다.
			/// </summary>
			/// <param name="color"></param>
			public void RequestColor(Color color)
			{
				bool isGray = Mathf.Abs(color.r - 0.5f) < 0.004f && 
								Mathf.Abs(color.g - 0.5f) < 0.004f && 
								Mathf.Abs(color.b - 0.5f) < 0.004f && 
								Mathf.Abs(color.a - 1.0f) < 0.004f;

				if(isGray)
				{
					_isRequested_Color = false;
					color = new Color(0.5f, 0.5f, 0.5f, 1.0f);
				}
				else
				{
					_isRequested_Color = true;
				}

				_material.SetColor(_shaderID_Color, color);

				apOptMesh curMesh = null;
				for (int i = 0; i < _linkedMeshes.Count; i++)
				{
					curMesh = _linkedMeshes[i];
					if (curMesh == null)
					{
						continue;
					}

					//각 메시의 Instanced Material도 Batch Material과 동일한 값을 가지도록 한다.
					curMesh.SyncMaterialPropertyByBatch_Color(color);
				}
			}

			/// <summary>
			/// 기본 색상(_Color)의 알파 채널 변경 요청. 기본값인 경우 요청을 초기화한다.
			/// </summary>
			/// <param name="color"></param>
			public void RequestAlpha(float alpha)
			{
				Color color = _material.GetColor(_shaderID_Color);
				color.a = alpha;

				bool isGray = Mathf.Abs(color.r - 0.5f) < 0.004f && 
								Mathf.Abs(color.g - 0.5f) < 0.004f && 
								Mathf.Abs(color.b - 0.5f) < 0.004f && 
								Mathf.Abs(color.a - 1.0f) < 0.004f;

				if(isGray)
				{
					_isRequested_Color = false;
					color = new Color(0.5f, 0.5f, 0.5f, 1.0f);
				}
				else
				{
					_isRequested_Color = true;
				}

				_material.SetColor(_shaderID_Color, color);

				apOptMesh curMesh = null;
				for (int i = 0; i < _linkedMeshes.Count; i++)
				{
					curMesh = _linkedMeshes[i];
					if (curMesh == null)
					{
						continue;
					}

					//각 메시의 Instanced Material도 Batch Material과 동일한 값을 가지도록 한다.
					curMesh.SyncMaterialPropertyByBatch_Color(color);
				}
			}

			/// <summary>
			/// 임의의 프로퍼티의 텍스쳐 속성을 변경한다. 호출 즉시 항상 Batched Material을 사용하게 된다.
			/// </summary>
			public void RequestCustomImage(Texture2D texture, string propertyName)
			{
				_isRequested_Custom = true;//<<항상 Request 사용 상태
				_material.SetTexture(propertyName, texture);

				apOptMesh curMesh = null;
				for (int i = 0; i < _linkedMeshes.Count; i++)
				{
					curMesh = _linkedMeshes[i];
					if (curMesh == null)
					{
						continue;
					}

					//각 메시의 Instanced Material도 Batch Material과 동일한 값을 가지도록 한다.
					curMesh.SyncMaterialPropertyByBatch_CustomTexture(texture, propertyName);
				}
			}

			/// <summary>
			/// 임의의 프로퍼티의 색상 속성을 변경한다. 호출 즉시 항상 Batched Material을 사용하게 된다.
			/// </summary>
			public void RequestCustomColor(Color color, string propertyName)
			{
				_isRequested_Custom = true;//<<항상 Request 사용 상태
				_material.SetColor(propertyName, color);

				apOptMesh curMesh = null;
				for (int i = 0; i < _linkedMeshes.Count; i++)
				{
					curMesh = _linkedMeshes[i];
					if (curMesh == null)
					{
						continue;
					}

					//각 메시의 Instanced Material도 Batch Material과 동일한 값을 가지도록 한다.
					curMesh.SyncMaterialPropertyByBatch_CustomColor(color, propertyName);
				}
			}

			/// <summary>
			/// 임의의 프로퍼티의 투명도 속성을 변경한다. 호출 즉시 항상 Batched Material을 사용하게 된다.
			/// </summary>
			public void RequestCustomAlpha(float alpha, string propertyName)
			{
				_isRequested_Custom = true;//<<항상 Request 사용 상태
				Color curColor = _material.GetColor(propertyName);
				curColor.a = alpha;
				_material.SetColor(propertyName, curColor);

				apOptMesh curMesh = null;
				for (int i = 0; i < _linkedMeshes.Count; i++)
				{
					curMesh = _linkedMeshes[i];
					if (curMesh == null)
					{
						continue;
					}

					//각 메시의 Instanced Material도 Batch Material과 동일한 값을 가지도록 한다.
					curMesh.SyncMaterialPropertyByBatch_CustomColor(curColor, propertyName);
				}
			}

			/// <summary>
			/// 임의의 프로퍼티의 Float 속성을 변경한다. 호출 즉시 항상 Batched Material을 사용하게 된다.
			/// </summary>
			public void RequestCustomFloat(float floatValue, string propertyName)
			{
				_isRequested_Custom = true;//<<항상 Request 사용 상태
				_material.SetFloat(propertyName, floatValue);

				apOptMesh curMesh = null;
				for (int i = 0; i < _linkedMeshes.Count; i++)
				{
					curMesh = _linkedMeshes[i];
					if (curMesh == null)
					{
						continue;
					}

					//각 메시의 Instanced Material도 Batch Material과 동일한 값을 가지도록 한다.
					curMesh.SyncMaterialPropertyByBatch_CustomFloat(floatValue, propertyName);
				}
			}

			/// <summary>
			/// 임의의 프로퍼티의 Int 속성을 변경한다. 호출 즉시 항상 Batched Material을 사용하게 된다.
			/// </summary>
			public void RequestCustomInt(int intValue, string propertyName)
			{
				_isRequested_Custom = true;//<<항상 Request 사용 상태
				_material.SetInt(propertyName, intValue);

				apOptMesh curMesh = null;
				for (int i = 0; i < _linkedMeshes.Count; i++)
				{
					curMesh = _linkedMeshes[i];
					if (curMesh == null)
					{
						continue;
					}

					//각 메시의 Instanced Material도 Batch Material과 동일한 값을 가지도록 한다.
					curMesh.SyncMaterialPropertyByBatch_CustomInt(intValue, propertyName);
				}
			}

			/// <summary>
			/// 임의의 프로퍼티의 Vector 속성을 변경한다. 호출 즉시 항상 Batched Material을 사용하게 된다.
			/// </summary>
			public void RequestCustomVector4(Vector4 vec4Value, string propertyName)
			{
				_isRequested_Custom = true;//<<항상 Request 사용 상태
				_material.SetVector(propertyName, vec4Value);

				apOptMesh curMesh = null;
				for (int i = 0; i < _linkedMeshes.Count; i++)
				{
					curMesh = _linkedMeshes[i];
					if (curMesh == null)
					{
						continue;
					}

					//각 메시의 Instanced Material도 Batch Material과 동일한 값을 가지도록 한다.
					curMesh.SyncMaterialPropertyByBatch_CustomVector4(vec4Value, propertyName);
				}
			}

			public bool IsAnyChanged
			{
				get
				{
					return _isRequested_Texture
						|| _isRequested_Color
						|| _isRequested_Custom;
				}
			}
		}

		[SerializeField]
		public List<MaterialUnit> _matUnits = new List<MaterialUnit>();
		
		
		[NonSerialized]
		private apPortrait _parentPortrait = null;

		// Init
		//----------------------------------------------------
		public apOptBatchedMaterial()
		{

		}

		public void Link(apPortrait portrait)
		{
			_parentPortrait = portrait;
			for (int i = 0; i < _matUnits.Count; i++)
			{
				_matUnits[i]._linkedMeshes.Clear();
			}
		}

		public void Clear(bool isDestroyMaterial)
		{
			if (isDestroyMaterial)
			{
				MaterialUnit curUnit = null;
				for (int i = 0; i < _matUnits.Count; i++)
				{
					curUnit = _matUnits[i];
					try
					{
						if (curUnit._material != null)
						{
							UnityEngine.Object.DestroyImmediate(curUnit._material);
						}

						if (curUnit._material_Original != null)
						{
							UnityEngine.Object.DestroyImmediate(curUnit._material_Original);
						}
					}
					catch (Exception)
					{

					}
				}
			}
			_matUnits.Clear();
		}

		

		// Functions
		//----------------------------------------------------
		public MaterialUnit MakeBatchedMaterial(Texture2D texture, int textureID, Shader shader)
		{
			MaterialUnit result = _matUnits.Find(delegate (MaterialUnit a)
			{
				return a.IsEqualMaterial(texture, textureID, shader);
			});
			if(result != null)
			{
				//수정 : 이 함수는 Bake에서 호출되므로 Material은 만들지 않는다.
				//Material resultMat = result._material;
				//if(resultMat == null)
				//{
				//	result.MakeMaterial();
				//	resultMat = result._material;
				//}
				return result;
			}

			//새로 만들자
			int newID = _matUnits.Count + 1;

			result = new MaterialUnit(newID, texture, textureID, shader);
			_matUnits.Add(result);

			return result;
		}


		public MaterialUnit GetMaterialUnit(int materialID, apOptMesh optMesh)
		{
			MaterialUnit result = _matUnits.Find(delegate (MaterialUnit a)
			{
				return a._uniqueID == materialID;
			});
			if(result != null)
			{
				if(result._material == null)
				{
					result.MakeMaterial();
				}

				result.LinkMesh(optMesh);

				return result;
			}
			return null;
		}


		


		//추가 : Shared Materal도 여기서 만들어야 한다.
		public Material GetSharedMaterial(Texture2D mainTex, Shader shader)
		{
			if(_parentPortrait == null)
			{
				//아직 연결이 안되었다.
				return null;
			}
			return apOptSharedMaterial.I.GetSharedMaterial(mainTex, shader, _parentPortrait);
		}


		//public void DebugAllMaterials()
		//{
		//	MaterialUnit curUnit = null;
		//	Debug.LogError("Batched Materials");
		//	for (int i = 0; i < _matUnits.Count; i++)
		//	{
		//		string strMaterialName = "<None Material>";
		//		string strTextureName = "<None Texture>";
		//		int nLinked = 0;

		//		curUnit = _matUnits[i];
		//		if(curUnit._material != null)
		//		{
		//			strMaterialName = curUnit._material.name;
		//			if(curUnit._material.mainTexture != null)
		//			{
		//				strTextureName = curUnit._material.mainTexture.name;
		//			}
		//		}
		//		nLinked = curUnit._linkedMeshes.Count;
		//		Debug.Log("[" + i + "] : " + strMaterialName + " / " + strTextureName + "  (" + nLinked + ")");
		//	}
		//}

		// 추가 12.13 : 일괄 수정 요청
		//---------------------------------------------------------------
		
		/// <summary>
		/// Batched Material 의 속성을 초기화한다. 이 함수 호출 후에는 Shared Material로 유도한다.
		/// </summary>
		public void ResetAllProperties()
		{
			MaterialUnit curUnit = null;
			//apOptMesh curMesh = null;
				
			for (int i = 0; i < _matUnits.Count; i++)
			{
				curUnit = _matUnits[i];
				curUnit.ResetRequestProperties();
				//curUnit.RefreshLinkedMeshes();
			}
		}

		public void ResetProperties(int textureID)
		{
			MaterialUnit curUnit = null;
			for (int i = 0; i < _matUnits.Count; i++)
			{
				curUnit = _matUnits[i];
				curUnit.ResetRequestProperties();
				//curUnit.RefreshLinkedMeshes();
			}
		}

		public void SetMeshImageAll(int textureID, Texture2D newTexture)
		{
			MaterialUnit curUnit = null;
			for (int i = 0; i < _matUnits.Count; i++)
			{
				curUnit = _matUnits[i];
				if(curUnit._textureID != textureID)
				{
					continue;
				}

				curUnit.RequestImage(newTexture);
				//curUnit.RefreshLinkedMeshes();
			}
		}

		public void SetMeshColorAll(int textureID, Color color)
		{
			MaterialUnit curUnit = null;
			for (int i = 0; i < _matUnits.Count; i++)
			{
				curUnit = _matUnits[i];
				if(curUnit._textureID != textureID)
				{
					continue;
				}

				curUnit.RequestColor(color);
				//curUnit.RefreshLinkedMeshes();
			}
		}

		public void SetMeshAlphaAll(int textureID, float alpha)
		{
			MaterialUnit curUnit = null;
			for (int i = 0; i < _matUnits.Count; i++)
			{
				curUnit = _matUnits[i];
				if(curUnit._textureID != textureID)
				{
					continue;
				}

				curUnit.RequestAlpha(alpha);
				//curUnit.RefreshLinkedMeshes();
			}
		}

		public void SetMeshCustomImageAll(int textureID, Texture2D newTexture, string propertyName)
		{
			MaterialUnit curUnit = null;
			for (int i = 0; i < _matUnits.Count; i++)
			{
				curUnit = _matUnits[i];
				if(curUnit._textureID != textureID)
				{
					continue;
				}

				curUnit.RequestCustomImage(newTexture, propertyName);
				//curUnit.RefreshLinkedMeshes();
			}
		}

		public void SetMeshCustomColorAll(int textureID, Color color, string propertyName)
		{
			MaterialUnit curUnit = null;
			for (int i = 0; i < _matUnits.Count; i++)
			{
				curUnit = _matUnits[i];
				if(curUnit._textureID != textureID)
				{
					continue;
				}

				curUnit.RequestCustomColor(color, propertyName);
				//curUnit.RefreshLinkedMeshes();
			}
		}

		public void SetMeshCustomAlphaAll(int textureID, float alpha, string propertyName)
		{
			MaterialUnit curUnit = null;
			for (int i = 0; i < _matUnits.Count; i++)
			{
				curUnit = _matUnits[i];
				if(curUnit._textureID != textureID)
				{
					continue;
				}

				curUnit.RequestCustomAlpha(alpha, propertyName);
				//curUnit.RefreshLinkedMeshes();
			}
		}

		public void SetMeshCustomFloatAll(int textureID, float floatValue, string propertyName)
		{
			MaterialUnit curUnit = null;
			for (int i = 0; i < _matUnits.Count; i++)
			{
				curUnit = _matUnits[i];
				if(curUnit._textureID != textureID)
				{
					continue;
				}

				curUnit.RequestCustomFloat(floatValue, propertyName);
				//curUnit.RefreshLinkedMeshes();
			}
		}


		public void SetMeshCustomIntAll(int textureID, int intValue, string propertyName)
		{
			MaterialUnit curUnit = null;
			for (int i = 0; i < _matUnits.Count; i++)
			{
				curUnit = _matUnits[i];
				if(curUnit._textureID != textureID)
				{
					continue;
				}

				curUnit.RequestCustomInt(intValue, propertyName);
				//curUnit.RefreshLinkedMeshes();
			}
		}

		public void SetMeshCustomVector4All(int textureID, Vector4 vecValue, string propertyName)
		{
			MaterialUnit curUnit = null;
			for (int i = 0; i < _matUnits.Count; i++)
			{
				curUnit = _matUnits[i];
				if(curUnit._textureID != textureID)
				{
					continue;
				}

				curUnit.RequestCustomVector4(vecValue, propertyName);
				//curUnit.RefreshLinkedMeshes();
			}
		}

		// Get / Set
		//----------------------------------------------------
		

	}
}