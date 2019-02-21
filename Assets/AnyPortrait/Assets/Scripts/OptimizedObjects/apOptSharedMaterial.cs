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
	
	public class apOptSharedMaterial
	{

		//싱글톤이다.
		private static apOptSharedMaterial _instance = new apOptSharedMaterial();
		public static apOptSharedMaterial I { get { return _instance; } }

		// Members
		//-----------------------------------------------------------------------------
		/// <summary>
		/// Key - Value로 작동하는 재질 세트
		/// Key : Texture + Shader
		/// Value : Material
		/// 추가로, 사용량도 확인할 수 있다.
		/// </summary>
		public class OptMaterialSet
		{
			public Texture _texture = null;
			public Shader _shader = null;

			[NonSerialized]
			public Material _material = null;

			public List<apPortrait> _linkedPortraits = new List<apPortrait>();

			public OptMaterialSet(Texture texture, Shader shader)
			{
				_texture = texture;
				_shader = shader;
				_material = new Material(_shader);
				_material.SetColor("_Color", new Color(0.5f, 0.5f, 0.5f, 1.0f));
				_material.SetTexture("_MainTex", _texture);

				if(_linkedPortraits == null)
				{
					_linkedPortraits = new List<apPortrait>();
				}
				_linkedPortraits.Clear();
			}

			public void LinkPortrait(apPortrait portrait)
			{
				if(_linkedPortraits.Contains(portrait))
				{
					return;
				}
				_linkedPortraits.Add(portrait);
			}

			

			public void RemoveMaterial()
			{
				if(_material == null)
				{
					UnityEngine.Object.Destroy(_material);
					_material = null;
				}
			}

			/// <summary>
			/// Portrait의 등록을 해제한다.
			/// 등록된 모든 Portrait가 삭제되었다면, True를 리턴한다. (삭제하도록)
			/// </summary>
			/// <param name="portrait"></param>
			/// <returns></returns>
			public bool RemovePortrait(apPortrait portrait)
			{
				if(_linkedPortraits.Contains(portrait))
				{
					_linkedPortraits.Remove(portrait);
				}

				return (_linkedPortraits.Count == 0);
			}

		}

		private Dictionary<Texture, Dictionary<Shader, OptMaterialSet>> _materialSets = new Dictionary<Texture, Dictionary<Shader, OptMaterialSet>>();
		private Dictionary<apPortrait, List<OptMaterialSet>> _portrait2MaterialSets = new Dictionary<apPortrait, List<OptMaterialSet>>();
		
		

		// Init
		//-----------------------------------------------------------------------------
		private apOptSharedMaterial()
		{
			Clear();
		}

		public void Clear()
		{
			
			//리스트를 클리어 하기 전에 Material을 삭제해야한다.
			if (_materialSets == null)
			{
				_materialSets = new Dictionary<Texture, Dictionary<Shader, OptMaterialSet>>();
			}
			else
			{
				int nRemoved = 0;
				foreach (KeyValuePair<Texture, Dictionary<Shader, OptMaterialSet>> shaderMatSet in _materialSets)
				{
					foreach (KeyValuePair<Shader, OptMaterialSet> matSet in shaderMatSet.Value)
					{
						//재질 삭제
						matSet.Value.RemoveMaterial();
						nRemoved++;
					}
				}

				_materialSets.Clear();

				//Debug.LogWarning("Clear Shared Materials [" + nRemoved + "]");
			}
			_portrait2MaterialSets.Clear();
		}


		// Functions
		//-----------------------------------------------------------------------------
		public Material GetSharedMaterial(Texture texture, Shader shader, apPortrait portrait)
		{
#if UNITY_EDITOR
			if(UnityEditor.BuildPipeline.isBuildingPlayer)
			{
				return null;
			}
#endif
			OptMaterialSet matSet = null;

			//Debug.LogWarning("Shared Material - Get Shared Material [ " + texture.name + " / " + shader.name + " / " + portrait.name + " ]");
			if(_materialSets.ContainsKey(texture))
			{
				if(_materialSets[texture].ContainsKey(shader))
				{
					matSet = _materialSets[texture][shader];

					//Debug.Log(">> 기존에 만들어진 Material 리턴");
				}
				else
				{
					//새로운 Material Set 생성
					matSet = new OptMaterialSet(texture, shader);

					//Shader 키와 함께 등록
					_materialSets[texture].Add(shader, matSet);

					//Debug.Log(">> (!) 새로운 Material 리턴");
				}

			}
			else
			{
				//새로운 Material Set 생성
				matSet = new OptMaterialSet(texture, shader);

				//Texture 키와 리스트 생성
				_materialSets.Add(texture, new Dictionary<Shader, OptMaterialSet>());

				//Shader 키와 함께 등록
				_materialSets[texture].Add(shader, matSet);

				//Debug.Log(">> (!) 새로운 Material 리턴");
			}

			//Portrait 등록
			matSet.LinkPortrait(portrait);
			List<OptMaterialSet> matSetList = null;
			if(!_portrait2MaterialSets.ContainsKey(portrait))
			{
				_portrait2MaterialSets.Add(portrait, new List<OptMaterialSet>());
			}
			matSetList = _portrait2MaterialSets[portrait];
			if(!matSetList.Contains(matSet))
			{
				matSetList.Add(matSet);
			}
			

			//Shader Material 반환
			return matSet._material;
		}

		


		// Event
		//-----------------------------------------------------------------------------
		/// <summary>
		/// apPortrait가 없어질때 호출해야한다.
		/// 가져다쓴 Material이 있다면 최적화를 위해 등록을 해제하고 삭제 여부를 결정한다.
		/// </summary>
		/// <param name="portrait"></param>
		public void OnPortraitDestroyed(apPortrait portrait)
		{
			//Debug.LogError("Shared Material - OnPortraitDestroyed : " + portrait.name);

			if(!_portrait2MaterialSets.ContainsKey(portrait))
			{
				return;
			}
			


			List<OptMaterialSet> optMatSetList = _portrait2MaterialSets[portrait];
			OptMaterialSet curMatSet = null;

			List<Texture> removedTextureKey = new List<Texture>();
			List<Shader> removedShaderKey = new List<Shader>();
			int nRemoved = 0;

			for (int i = 0; i < optMatSetList.Count; i++)
			{
				curMatSet = optMatSetList[i];

				//Mat Set에서 Portrait를 삭제한다.
				if(curMatSet.RemovePortrait(portrait))
				{
					//모든 Portrait가 삭제되었다.
					//여기서 바로 재질 삭제
					curMatSet.RemoveMaterial();

					//리스트에서 삭제할 준비
					removedTextureKey.Add(curMatSet._texture);
					removedShaderKey.Add(curMatSet._shader);
					nRemoved++;
				}
			}

			//Debug.LogError(">> " + nRemoved + "개의 쓸모없는 Shared material이 삭제된다.");

			if(nRemoved > 0)
			{
				Texture curTex = null;
				Shader curShader = null;
				for (int i = 0; i < nRemoved; i++)
				{
					curTex = removedTextureKey[i];
					curShader = removedShaderKey[i];

					if(!_materialSets.ContainsKey(curTex))
					{
						continue;
					}

					//Texture + Shader에 해당하는 Material Set를 삭제한다.
					_materialSets[curTex].Remove(curShader);

					//만약 이 Texture 키에 대한 Shader-Set 리스트에 아무런 데이터가 없다면
					//이 텍스쳐에 대한 Set도 삭제
					if(_materialSets[curTex].Count == 0)
					{
						_materialSets.Remove(curTex);
					}
				}
			}

			//마지막으로 연결 리스트도 삭제
			_portrait2MaterialSets.Remove(portrait);
		}

		// Get / Set
		//-----------------------------------------------------------------------------
		public void DebugAllMaterials()
		{
			Debug.LogError("Shared Materials");
			int index = 0;
			foreach (KeyValuePair<Texture, Dictionary<Shader, OptMaterialSet>> tex2ShaderMat in _materialSets)
			{
				Debug.LogWarning("Texture : " + tex2ShaderMat.Key.name);
				foreach (KeyValuePair<Shader, OptMaterialSet> shader2Mat in tex2ShaderMat.Value)
				{
					Debug.Log(" [" + index + "] : " + shader2Mat.Key.name + " (" + shader2Mat.Value._linkedPortraits.Count + ")");
					
					index++;
				}
			}
		}
	}
}