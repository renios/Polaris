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
//using UnityEngine.Profiling;
using UnityEngine.Rendering;
using System.Collections;
using System.Collections.Generic;
using System;

using AnyPortrait;

namespace AnyPortrait
{
	/// <summary>
	/// Rendered Class that has a Mesh.
	/// </summary>
	public class apOptMesh : MonoBehaviour
	{
		// Members
		//------------------------------------------------
		/// <summary>[Please do not use it] Parent Portrait</summary>
		public apPortrait _portrait = null;

		/// <summary>[Please do not use it] Unique ID</summary>
		public int _uniqueID = -1;//meshID가 아니라 meshTransform의 ID를 사용한다.

		/// <summary>
		/// Paranet Opt Transform
		/// </summary>
		public apOptTransform _parentTransform;

		// Components
		//------------------------------------------------
		[HideInInspector]
		public MeshFilter _meshFilter = null;

		//재질 관련 변수들
		//변경 12.11 : DrawCall 관리를 위한 코드 변경
		public enum MATERIAL_TYPE
		{
			/// <summary>공유된 재질. apOptSharedMaterial에서 받아온다. (apOptBatchedMaterial을 경유)</summary>
			Shared,
			/// <summary>동일한 재질을 사용하는 메시들의 텍스쳐나 색상 등을 한꺼번에 바꾸고자 할 때. apOptBatchedMaterial에서 받아온다.</summary>
			Batched,
			/// <summary>Shader의 파라미터를 변경한 후 인스턴스 타입을 사용한다.</summary>
			Instanced
		}

		[NonSerialized, HideInInspector]
		private MATERIAL_TYPE _materialType = MATERIAL_TYPE.Shared;

		[NonSerialized, HideInInspector]
		private Material _material_Cur = null;
		
		[NonSerialized, HideInInspector]
		private Material _material_Shared = null;

		[NonSerialized, HideInInspector]
		private apOptBatchedMaterial.MaterialUnit _materialUnit_Batched = null;

		[NonSerialized, HideInInspector]
		private Material _material_Batched = null;

		[NonSerialized, HideInInspector]
		private Material _material_Instanced = null;

		[NonSerialized, HideInInspector]
		private bool _isForceBatch2Shared = false;//만약 Batch가 작동하더라도, 강제로 Shared로 전환될 필요가 있다.




		[HideInInspector]
		public MeshRenderer _meshRenderer = null;

		///// <summary>
		///// 현재 사용중인 Material이다. 
		///// Instanced인지 Shared인지는 자동으로 결정된다. (기본값은 Shared)
		///// 저장은 안되는 값이다.
		///// </summary>
		//[NonSerialized, HideInInspector]
		//private Material _material = null;
		
		///// <summary>[Please do not use it] Baked Shared Material to Batch Rendering</summary>
		//[HideInInspector]
		//public Material _sharedMaterial = null;

		//[NonSerialized]
		//private Material _instanceMaterial = null;

		//private bool _isUseSharedMaterial = true;

		//기본 설정이 Batch Material을 사용하는 것이라면 별도의 값을 저장한다.
		//이때는 SharedMaterial이 null로 Bake된다
		/// <summary>[Please do not use it] Is Batch Target</summary>
		public bool _isBatchedMaterial = false;

		/// <summary>[Please do not use it] Batch Material ID</summary>
		public int _batchedMatID = -1;

		public bool _isDefaultColorGray = false;

		/// <summary>[Please do not use it] Current Rendered Texture (Read Only)</summary>
		[HideInInspector]
		public Texture2D _texture = null;

		/// <summary>[Please do not use it] Unique ID of Linked Texture Data</summary>
		[HideInInspector]
		public int _textureID = -1;


		/// <summary>[Please do not use it]</summary>
		[NonSerialized, HideInInspector]
		public Mesh _mesh = null;//<변경 : 저장 안됩니더

		// Vertex 값들
		//apRenderVertex에 해당하는 apOptRenderVertex의 배열 (리스트 아닙니더)로 저장한다.

		//<기본값>
		[SerializeField]
		private apOptRenderVertex[] _renderVerts = null;



		//RenderVert의 
		[SerializeField]
		private Vector3[] _vertPositions = null;

		[SerializeField]
		private Vector2[] _vertUVs = null;

		//[SerializeField]
		//private int[] _vertUniqueIDs = null;

		[SerializeField]
		private int[] _vertTris = null;

		[SerializeField]
		private int[] _vertTris_Flipped = null;//<<추가 : Flipped된 경우 Reverse된 값을 사용한다.

		//삭제 19.7.3 : RenderVert와 vertPositon의 개수가 다를 수 있다. (양면의 경우)
		//[SerializeField]
		//private int _nVert = 0;

		//변경 19.7.3 : 버텍스 개수를 두개로 분리. NonSerialized로 바꾸었다. (초기화에서 값 설정)
		[NonSerialized]
		private int _nRenderVerts = 0;

		[NonSerialized]
		private int _nVertPos = 0;
		


		/// <summary>Rendered Vertices</summary>
		public apOptRenderVertex[] RenderVertices { get { return _renderVerts; } }

		public Vector3[] LocalVertPositions { get { return _vertPositions; } }


		//<업데이트>
		[SerializeField, NonSerialized]
		//[NonSerialized]
		private Vector3[] _vertPositions_Updated = null;

		[SerializeField, NonSerialized]
		//[NonSerialized]
		private Vector3[] _vertPositions_Local = null;

		[SerializeField, NonSerialized]
		//[NonSerialized]
		private Vector2[] _vertPositions_World = null;

		//[SerializeField]
		//private Texture2D _texture_Updated = null;

		[SerializeField, HideInInspector]
		public Transform _transform = null;

		[NonSerialized]
		private bool _isInitMesh = false;

		[NonSerialized]
		private bool _isInitMaterial = false;

		[SerializeField]
		private Vector2 _pivotPos = Vector2.zero;

		[SerializeField]
		private bool _isVisibleDefault = true;

		[NonSerialized]
		private bool _isVisible = false;

		[NonSerialized]
		private bool _isUseRiggingCache = false;

		/// <summary>기본값이 아닌 외부에서 숨기려고 할 때 설정된다. RootUnit이 Show 될때 해제된다.</summary>
		[NonSerialized]
		private bool _isHide_External = false;


		//Mask인 경우
		//Child는 업데이트는 하지만 렌더링은 하지 않는다.
		//렌더링을 하지 않으므로 Mesh 갱신을 하지 않음
		//Parent는 업데이트 후 렌더링은 잠시 보류한다.
		//"통합" Vertex으로 정의된 SubMeshData에서 통합 작업을 거친 후에 Vertex 업데이트를 한다.
		//MaskMesh 업데이트는 Portrait에서 Calculate 후 일괄적으로 한다. (List로 관리한다.)
		/// <summary>[Please do not use it] Is Parent Mesh of Clipping Masking</summary>
		public bool _isMaskParent = false;

		/// <summary>[Please do not use it] Is Child Mesh of Clipping Masking</summary>
		public bool _isMaskChild = false;

		//Child인 경우
		/// <summary>[Please do not use it] Masking Parent Mesh ID if clipped</summary>
		public int _clipParentID = -1;

		/// <summary>[Please do not use it] Is Masking Parent Mesh if clipped</summary>
		public apOptMesh _parentOptMesh = null;

		//Parent인 경우
		/// <summary>[Please do not use it] Children if clipping mask </summary>
		public int[] _clipChildIDs = null;
		//public apOptMesh[] _childOptMesh = null;

		[NonSerialized]
		private Color _multiplyColor = new Color(0.5f, 0.5f, 0.5f, 1.0f);

		[NonSerialized]
		private bool _isAnyMeshColorRequest = false;

		[NonSerialized]
		private bool _isAnyTextureRequest = false;

		[NonSerialized]
		private bool _isAnyCustomPropertyRequest = false;

		
		

		/// <summary>[Please do not use it] Updated Matrix</summary>
		public apMatrix3x3 _matrix_Vert2Mesh = apMatrix3x3.identity;

		/// <summary>[Please do not use it] Updated Matrix</summary>
		public apMatrix3x3 _matrix_Vert2Mesh_Inverse = apMatrix3x3.identity;

		/// <summary>[Please do not use it] Rendering Shader Type</summary>
		[SerializeField]
		public apPortrait.SHADER_TYPE _shaderType = apPortrait.SHADER_TYPE.AlphaBlend;

		/// <summary>[Please do not use it] Shader (not Clipped)</summary>
		[SerializeField]
		public Shader _shaderNormal = null;

		/// <summary>[Please do not use it] Shader (Clipped)</summary>
		[SerializeField]
		public Shader _shaderClipping = null;

		//추가
		/// <summary>[Please do not use it] Shader (Mask Parent)</summary>
		[SerializeField]
		public Shader _shader_AlphaMask = null;//<< Mask Shader를 저장하고 나중에 생성한다.

		[NonSerialized]
		private Material _materialAlphaMask = null;

		/// <summary>[Please do not use it] Mask Texture Size</summary>
		[SerializeField]
		public int _clippingRenderTextureSize = 256;

		#region [미사용 코드]
		//private static Color[] ShaderTypeColor = new Color[] {  new Color(1.0f, 0.0f, 0.0f, 0.0f),
		//															new Color(0.0f, 1.0f, 0.0f, 0.0f),
		//															new Color(0.0f, 0.0f, 1.0f, 0.0f),
		//															new Color(0.0f, 0.0f, 0.0f, 1.0f)}; 
		#endregion

		//클리핑 Parent인 경우
		//한개의 카메라에 대해서만 검색한다.
		[NonSerialized]
		private bool _isRenderTextureCreated = false;

		[NonSerialized]
		private RenderTexture _maskRenderTexture = null;

		[NonSerialized]
		private RenderTargetIdentifier _maskRenderTargetID = -1;

		[NonSerialized]
		private Camera _targetCamera = null;

		[NonSerialized]
		private Transform cameraTransform = null;

		[NonSerialized]
		private CommandBuffer _commandBuffer = null;

		public RenderTexture MaskRenderTexture
		{
			get
			{
				if(!_isRenderTextureCreated || !_isVisible)
				{
					return null;
				}
				return _maskRenderTexture;
			}
		}

		/// <summary>[Please do not use it]</summary>
		[NonSerialized]
		public Vector4 _maskScreenSpaceOffset = Vector4.zero;


		private RenderTexture _prevParentRenderTexture = null;
		private RenderTexture _curParentRenderTexture = null;

		//효율적인 Mask를 렌더링하기 위한 변수.
		//딱 렌더링 부분만 렌더링하자
		private Vector3 _vertPosCenter = Vector3.zero;
		//private float _vertRangeMax = 0.0f;

		private float _vertRange_XMin = 0.0f;
		private float _vertRange_YMax = 0.0f;
		private float _vertRange_XMax = 0.0f;
		private float _vertRange_YMin = 0.0f;

		
		private int _shaderID_MainTex = -1;
		private int _shaderID_Color = -1;
		private int _shaderID_MaskTexture = -1;
		private int _shaderID_MaskScreenSpaceOffset = -1;

		//계산용 변수들
		private Vector3 _cal_localPos_LT = Vector3.zero;
		private Vector3 _cal_localPos_RB = Vector3.zero;
		private Vector3 _cal_vertWorldPos_Center = Vector3.zero;
		private Vector3 _cal_vertWorldPos_LT = Vector3.zero;
		private Vector3 _cal_vertWorldPos_RB = Vector3.zero;
		private Vector3 _cal_screenPos_Center = Vector3.zero;
		private Vector3 _cal_screenPos_LT = Vector3.zero;
		private Vector3 _cal_screenPos_RB = Vector3.zero;
		private float _cal_prevSizeWidth = 0.0f;
		private float _cal_prevSizeHeight = 0.0f;
		private float _cal_zoomScale = 0.0f;
		private float _cal_aspectRatio = 0.0f;
		private float _cal_newOrthoSize = 0.0f;
		private Vector2 _cal_centerMoveOffset = Vector2.zero;
		private float _cal_distCenterToCamera = 0.0f;
		private Vector3 _cal_nextCameraPos = Vector3.zero;
		//private Vector3 _cal_camOffset = Vector3.zero;
		private Matrix4x4 _cal_customWorldToCamera = Matrix4x4.identity;
		private Matrix4x4 _cal_customCullingMatrix = Matrix4x4.identity;
		private Matrix4x4 _cal_newLocalToProjMatrix = Matrix4x4.identity;
		private Matrix4x4 _cal_newWorldMatrix = Matrix4x4.identity;
		private Vector3 _cal_screenPosOffset = Vector3.zero;

		private Color _cal_MeshColor = Color.gray;

		private bool _cal_isVisibleRequest = false;
		private bool _cal_isVisibleRequest_Masked = false;

		//Transform이 Flipped된 경우 -> Vertex 배열을 역으로 계산해야한다.
		//추가 : 2.25

		//Flipped에 관해서 처리를 정밀하게 하자


		private bool _cal_isRootFlipped_X = false;
		private bool _cal_isRootFlipped_Y = false;
		private bool _cal_isRootFlipped = false;
		//private bool _cal_isRootFlipped_Prev = false;

		private bool _cal_isUpdateFlipped_X = false;
		private bool _cal_isUpdateFlipped_Y = false;
		private bool _cal_isUpdateFlipped = false;

		private bool _cal_isFlippedBuffer = false;
		private bool _cal_isFlippedBuffer_Prev = false;

		//추가 : 2.25 계산용 변수 하나 더
		private apMatrix3x3 _cal_Matrix_TFResult_World = apMatrix3x3.identity;

		[SerializeField]
		public bool _isAlways2Side = false;

		//추가 19.6.15 : Material Info가 추가되었다.
		//Bake가 안된 경우 (v1.1.6 또는 이전 버전)에 처리하기 위해서 배열형태로 만든다.
		[SerializeField, NonBackupField]
		private apOptMaterialInfo[] _materialInfo = null;

		public apOptMaterialInfo MaterialInfo
		{
			get
			{
				if(_materialInfo == null || _materialInfo.Length == 0) { return null; }
				return _materialInfo[0];
			}
		}

		public bool IsUseMaterialInfo
		{
			get { return MaterialInfo != null; }
		}

		

		// Init
		//------------------------------------------------
		void Awake()
		{	
			_transform = transform;

			_cal_isRootFlipped_X = false;
			_cal_isRootFlipped_Y = false;
			_cal_isRootFlipped = false;
			//_cal_isRootFlipped_Prev = false;

			_cal_isUpdateFlipped_X = false;
			_cal_isUpdateFlipped_Y = false;
			_cal_isUpdateFlipped = false;

			_cal_isFlippedBuffer = false;
			_cal_isFlippedBuffer_Prev = false;

			_shaderID_MainTex = Shader.PropertyToID("_MainTex");
			_shaderID_Color = Shader.PropertyToID("_Color");
			_shaderID_MaskTexture = Shader.PropertyToID("_MaskTex");
			_shaderID_MaskScreenSpaceOffset = Shader.PropertyToID("_MaskScreenSpaceOffset");
		}

		void Start()
		{
			//InitMesh(false);
			//InstantiateMesh();

			//this.enabled = true;
			this.enabled = false;

			//추가 9.26 : 생성이 안된 경우
			if(_isMaskParent && _isInitMesh && _isInitMaterial)
			{
				RegistCommandBuffer();
			}
		}


		void OnEnable()
		{
			if (_isInitMesh && _isInitMaterial)
			{
				CleanUpMaskParent();
			}

			
//#if UNITY_EDITOR
//			if(Application.isEditor && !Application.isPlaying)
//			{
//				//추가 : 
//				//만약 에디터에서 Material이 없는데 렌더링을 하고자 하는 경우
//				//임시로 Instanced Material을 만들어서 적용할 수 있다.

//				if (_meshFilter != null && _meshRenderer != null)
//				{
//					Debug.Log("OnEnabled : " + this.name);
//					if(_mesh == null || _meshFilter.sharedMesh == null)
//					{
//						Debug.Log("안보이는 Mesh의 구조를 자동으로 다시 설정 : " + this.name);

//						InitMesh(true);
//					}

//					if (_material_Instanced == null ||
//						_material_Cur == null)
//					{
//						if (_meshRenderer != null)
//						{
//							Debug.Log("안보이는 Mesh의 재질을 자동으로 다시 설정 : " + this.name);

//							MakeInstancedMaterial();

//							_meshRenderer.sharedMaterial = _material_Instanced;
//						}
//					}
//				}
				
//			}
//#endif
		}
#if UNITY_EDITOR
		// 이기능은 작성되었지만 유니티 2017, 2018 버그에 의해서 봉인
		// 유니티 2017, 2018에서 실행시 정상적으로 작동하지만 SendMessage 에러가 났다고 메시지를 무한정 띄운다.

		////추가 12.13 : 씬에 캐릭터가 로드 될 때, 메시나 재질이 초기화되지 않아서 안보이는 경우 (에디터에서만)
		////자동으로 갱신되는 기능
		////OnValidate는 이 스크립트가 로드되거나 인스펙터에서 수정될 때 호출된다.
		////게임이 실행될 때는 2번 호출된다. (왜..)
		////리셋되는 순서
		////1) 여러개의 메시들에게서 동시에 이 함수가 호출된다
		////2) 첫번째 호출이 일어날 때, Mesh나 Material이 없다면 Portrait의 OnMeshResetInEditor를 호출한다.
		////3) Portrait에서 모든 메시가 일괄적으로 리셋되고 Update를 1회 한다.
		////4) 다시 두번째 메시에서 OnValidate 함수가 호출되지만, 이미 3)에서 메시가 생성되었기에 더이상 메시를 리셋할 필요가 없다.
		//private void OnValidate()
		//{
		//	if(UnityEditor.BuildPipeline.isBuildingPlayer)
		//	{
		//		//Debug.Log("Ignore in BuildingPlayer Process : " + this.name);
		//		return;
		//	}
		//	if (Application.isEditor && !Application.isPlaying)
		//	{
		//		//Debug.Log("OnValidate : " + this.name + " / Mesh Filter : " + (_meshFilter != null) + " / Mesh Renderer : " + (_meshRenderer != null));

		//		bool isNeedToReset = false;
		//		if (_meshFilter != null && _meshRenderer != null)
		//		{
		//			if (_meshFilter.sharedMesh == null)
		//			{
		//				isNeedToReset = true;
		//				//Debug.LogError("OnValidate : 메시 다시 생성 : " + this.name);
		//				//InitMesh(true);
		//			}

		//			if (_meshRenderer.sharedMaterial == null)
		//			{
		//				isNeedToReset = true;
		//				//Debug.LogWarning("OnValidate : 재질 다시 생성 : " + this.name);
		//				//MakeInstancedMaterial();
		//				//_meshRenderer.sharedMaterial = _material_Instanced;
		//			}
		//		}

		//		if(isNeedToReset && _portrait != null)
		//		{
		//			_portrait.OnMeshResetInEditor();
		//		}
		//	}

		//}

		public bool IsMeshOrMaterialMissingInEditor()
		{
			if (_meshFilter != null && _meshRenderer != null)
			{
				if (_meshFilter.sharedMesh == null
					|| _meshRenderer.sharedMaterial == null)
				{
					return true;
				}
			}
			return false;
		}

		public void ResetMeshAndMaterialIfMissing()
		{
			if (!IsMeshOrMaterialMissingInEditor())
			{
				return;
			}
			if (_meshFilter != null && _meshRenderer != null)
			{
				if (_meshFilter.sharedMesh == null)
				{
					InitMesh(true);
				}

				if (_meshRenderer.sharedMaterial == null)
				{
					MakeInstancedMaterial();
					_meshRenderer.sharedMaterial = _material_Instanced;
				}
			}
		}
#endif

		//변경 : OnDisable이 아닌 Destroy 이벤트에서 Clipping Mask를 초기화하자
		//void OnDisable()
		void OnDestroy()
		{
			if (_isInitMesh && _isInitMaterial)
			{
				CleanUpMaskParent();


				//추가 12.12 : 재질 삭제
				try
				{
					if(_material_Instanced != null)
					{
						UnityEngine.Object.Destroy(_material_Instanced);
						_material_Instanced = null;
					}
				}
				catch (Exception)
				{

				}
			}
		}

		void OnWillRenderObject()
		{
#if UNITY_EDITOR
			if (!Application.isPlaying)
			{
				return;
			}
#endif
			if(_isMaskParent && _isInitMesh && _isInitMaterial)
			{
				RegistCommandBuffer();
			}
		}

		// Bake
		//------------------------------------------------
#if UNITY_EDITOR
		/// <summary>[Please do not use it] Bake Functions</summary>
		public void BakeMesh(Vector3[] vertPositions,
								Vector2[] vertUVs,
								int[] vertUniqueIDs,
								int[] vertTris,
								float[] depths,
								Vector2 pivotPos,
								apOptTransform parentTransform,
								Texture2D texture, int textureID,
								apPortrait.SHADER_TYPE shaderType,
								//Shader shaderNormal, Shader shaderClipping,//v1.1.6 또는 이전
								apOptMaterialInfo materialInfo,//v1.1.7 또는 이후
								Shader alphaMask,//<<AlphaMask는 따로
								int maskRenderTextureSize,
								bool isVisibleDefault,
								bool isMaskParent, bool isMaskChild,
								int batchedMatID, //Material batchedMaterial,//<<이건 필요없쩡..
								bool isAlways2Side,
								apPortrait.SHADOW_CASTING_MODE shadowCastMode,
								bool isReceiveShadow
								)
		{
			ClearMaterialForBake();

			_parentTransform = parentTransform;

			_vertPositions = vertPositions;
			_vertUVs = vertUVs;
			//_vertUniqueIDs = vertUniqueIDs;//<<삭제. 의미가 없다.
			_vertTris = vertTris;

			_isAlways2Side = isAlways2Side;

			//추가 : Flipped Tris를 만들자
			_vertTris_Flipped = new int[_vertTris.Length];
			for (int i = 0; i < _vertTris.Length - 2; i+=3)
			{
				_vertTris_Flipped[i + 0] = _vertTris[i + 2];
				_vertTris_Flipped[i + 1] = _vertTris[i + 1];
				_vertTris_Flipped[i + 2] = _vertTris[i + 0];
			}

			if(isAlways2Side)
			{
				//양면을 모두 만들자
				int nTri = _vertTris.Length;
				int nTri2Side = nTri * 2;

				int nVert = vertPositions.Length;
				int nVert2Side = nVert * 2;

				int[] vertTris2Side = new int[nTri2Side];

				//기존 : 같은 버텍스를 두번 공유한다.
				//Array.Copy(_vertTris, 0, vertTris2Side, 0, nTri);
				//Array.Copy(_vertTris_Flipped, 0, vertTris2Side, nTri, nTri);
				
				//_vertTris = vertTris2Side;//<<전환


				//변경 19.7.3 : 그냥 버텍스 리스트를 2개를 만들자
				//정방향
				Array.Copy(_vertTris, 0, vertTris2Side, 0, nTri);

				//역방향 (인덱스 값도 + nTri)
				for (int i = 0; i < _vertTris_Flipped.Length; i++)
				{
					vertTris2Side[i + nTri] = _vertTris_Flipped[i] + nVert;
				}

				_vertTris = vertTris2Side;//적용

				//다른 리스트도 두배로 증가
				_vertPositions = new Vector3[nVert2Side];
				_vertUVs = new Vector2[nVert2Side];
				
				for (int i = 0; i < nVert; i++)
				{
					_vertPositions[i] = vertPositions[i];
					_vertPositions[i + nVert] = vertPositions[i];//<<nVert만큼 뒤에 더 추가

					_vertUVs[i] = vertUVs[i];
					_vertUVs[i + nVert] = vertUVs[i];
				}
				
				//변경 19.7.3 : 양면 렌더링일때 (버텍스 수가 다름)
				_nRenderVerts = nVert;
				_nVertPos = nVert2Side;
			}
			else
			{
				//변경 19.7.3 : 단면 렌더링일때 (버텍스 수가 같음)
				_nRenderVerts = _vertPositions.Length;
				_nVertPos = _vertPositions.Length;
			}
			_texture = texture;
			_textureID = textureID;

			_pivotPos = pivotPos;

			//_nVert = _vertPositions.Length;///이전
			
			

			_isVisibleDefault = isVisibleDefault;

			transform.localPosition += new Vector3(-_pivotPos.x, -_pivotPos.y, 0.0f);

			_matrix_Vert2Mesh = apMatrix3x3.TRS(new Vector2(-_pivotPos.x, -_pivotPos.y), 0, Vector2.one);
			_matrix_Vert2Mesh_Inverse = _matrix_Vert2Mesh.inverse;

			_shaderType = shaderType;

			//이전 코드
			//_shaderNormal = shaderNormal;
			//_shaderClipping = shaderClipping;

			//변경된 코드 19.6.15 : MaterialInfo 이용
			_materialInfo = new apOptMaterialInfo[1];
			_materialInfo[0] = materialInfo;



			_shader_AlphaMask = alphaMask;//MaskShader를 넣는다.

			//_materialAlphaMask = new Material(alphaMask);이건 나중에 처리
			_clippingRenderTextureSize = maskRenderTextureSize;

			_isMaskParent = isMaskParent;
			_isMaskChild = isMaskChild;

			//Batch가 가능한 경우
			//1. Mask Child가 아닐 경우
			//2. Parent Tranform의 Default Color가 Gray인 경우
			_isDefaultColorGray =	Mathf.Abs(_parentTransform._meshColor2X_Default.r - 0.5f) < 0.004f &&
									Mathf.Abs(_parentTransform._meshColor2X_Default.g - 0.5f) < 0.004f &&
									Mathf.Abs(_parentTransform._meshColor2X_Default.b - 0.5f) < 0.004f &&
									Mathf.Abs(_parentTransform._meshColor2X_Default.a - 1.0f) < 0.004f;
			
			_isBatchedMaterial = !isMaskChild && _isDefaultColorGray;

			_batchedMatID = batchedMatID;

			//삭제 19.6.16 : 
			//if (_shaderNormal == null)
			//{
			//	Debug.LogError("Shader Normal is Null");
			//}
			//if (_shaderClipping == null)
			//{
			//	Debug.LogError("Shader Clipping is Null");
			//}

			//RenderVert를 만들어주자
			_renderVerts = new apOptRenderVertex[_nRenderVerts];
			for (int i = 0; i < _nRenderVerts; i++)
			{
				_renderVerts[i] = new apOptRenderVertex(
											_parentTransform, this,
											vertUniqueIDs[i], i,
											new Vector2(vertPositions[i].x, vertPositions[i].y),
											_vertUVs[i],
											depths[i]);

				_renderVerts[i].SetMatrix_1_Static_Vert2Mesh(_matrix_Vert2Mesh);
				_renderVerts[i].SetMatrix_3_Transform_Mesh(parentTransform._matrix_TFResult_WorldWithoutMod.MtrxToSpace);
				_renderVerts[i].Calculate();
			}

			if (_meshFilter == null || _mesh == null)
			{
				//일단 만들어두기는 한다.
				_meshFilter = GetComponent<MeshFilter>();
				_mesh = new Mesh();
				_mesh.name = this.name + "_Mesh";
				_mesh.Clear();

				_mesh.vertices = _vertPositions;
				_mesh.uv = _vertUVs;
				_mesh.triangles = _vertTris;

				_mesh.RecalculateNormals();
				_mesh.RecalculateBounds();

				

				_meshFilter.sharedMesh = _mesh;
			}

			_mesh.Clear();
			_cal_isFlippedBuffer = false;
			_cal_isFlippedBuffer_Prev = false;

			//재질 설정을 해주자
			//변경 12.12 : Bake시에는 Instanced만 사용하기로 한다.
			//일단 기존의 Material은 삭제
			if (_material_Instanced != null)
			{
				try
				{
					UnityEngine.Object.DestroyImmediate(_material_Instanced);
				}
				catch (Exception) { }
				_material_Instanced = null;
			}
			_materialType = MATERIAL_TYPE.Instanced;

			
			MakeInstancedMaterial();

			_material_Cur = _material_Instanced;
			_material_Batched = null;
			_material_Shared = null;
			_materialUnit_Batched = null;
			_isForceBatch2Shared = false;

			//일단 연결은 삭제한다.
			

			if (_meshRenderer == null)
			{
				_meshRenderer = GetComponent<MeshRenderer>();
			}

			//이전
			//_meshRenderer.sharedMaterial = _material;//<<현재 Bake된 Material 값을 넣어준다.
			
			//변경 12.12 : Bake에서는 Instanced Material을 넣는다.
			_meshRenderer.sharedMaterial = _material_Cur;
			


			//그림자 설정은 제외 > 변경 : 옵션에 따라서 설정한다.
			//_meshRenderer.receiveShadows = false;
			//_meshRenderer.shadowCastingMode = ShadowCastingMode.Off;

			//변경된 그림자 설정
			_meshRenderer.receiveShadows = isReceiveShadow;
			ShadowCastingMode castMode = ShadowCastingMode.Off;
			switch (shadowCastMode)
			{
				case apPortrait.SHADOW_CASTING_MODE.Off:
					castMode = ShadowCastingMode.Off;
					break;

				case apPortrait.SHADOW_CASTING_MODE.On:
					castMode = ShadowCastingMode.On;
					break;

				case apPortrait.SHADOW_CASTING_MODE.ShadowsOnly:
					castMode = ShadowCastingMode.ShadowsOnly;
					break;

				case apPortrait.SHADOW_CASTING_MODE.TwoSided:
					castMode = ShadowCastingMode.TwoSided;
					break;

			}
			_meshRenderer.shadowCastingMode = castMode;


			_meshRenderer.enabled = _isVisibleDefault;
			_meshRenderer.lightProbeUsage = LightProbeUsage.Off;

			
			//Mask 연결 정보는 일단 리셋
			_clipParentID = -1;
			_clipChildIDs = null;

			_vertPositions_Updated = new Vector3[_vertPositions.Length];
			_vertPositions_Local = new Vector3[_vertPositions.Length];
			_vertPositions_World = new Vector2[_vertPositions.Length];

			if (!_isAlways2Side)
			{
				//일반 업데이트

				for (int i = 0; i < _vertPositions.Length; i++)
				{
					//Calculate 전에는 직접 Pivot Pos를 적용해주자 (Calculate에서는 자동 적용)
					_vertPositions_Updated[i] = _renderVerts[i]._vertPos3_LocalUpdated;
				}
			}
			else
			{
				//양면 업데이트
				for (int i = 0; i < _nRenderVerts; i++)
				{
					//Calculate 전에는 직접 Pivot Pos를 적용해주자 (Calculate에서는 자동 적용)
					_vertPositions_Updated[i] = _renderVerts[i]._vertPos3_LocalUpdated;
					_vertPositions_Updated[i + _nRenderVerts] = _renderVerts[i]._vertPos3_LocalUpdated;
				}
			}
			
			

			_transform = transform;

			_shaderID_MainTex = Shader.PropertyToID("_MainTex");
			_shaderID_Color = Shader.PropertyToID("_Color");
			_shaderID_MaskTexture = Shader.PropertyToID("_MaskTex");
			_shaderID_MaskScreenSpaceOffset = Shader.PropertyToID("_MaskScreenSpaceOffset");


			InitMesh(true);

			RefreshMesh();

			if(_isVisibleDefault)
			{
				_meshRenderer.enabled = true;
				_isVisible = true;
				
			}
			else
			{
				_meshRenderer.enabled = false;
				_isVisible = false;
			}
		}
#endif


		//-----------------------------------------------------------------------
#if UNITY_EDITOR
		//Bake를 위해서 이전에 완성된 Material을 삭제하자.
		public void ClearMaterialForBake()
		{
			if(Application.isPlaying)
			{
				return;
			}

			_material_Instanced = null;
			_material_Cur = null;
			_material_Batched = null;
			_material_Shared = null;

			_isInitMaterial = false;
		}
#endif

		//Bake되지 않는 Mesh의 초기화를 호출한다.
		/// <summary>
		/// [Please do not use it]
		/// It is called by "Portrait"
		/// </summary>
		public void InitMesh(bool isForce)
		{
			if (!isForce && _isInitMesh)
			{
				return;
			}

			_transform = transform;


			//체크 19.7.3 : 만약 양면 렌더링인데, RenderVert와 _vertPositions의 개수가 같다면? (마이그레이션 코드)
			if (_isAlways2Side)
			{
				//Debug.Log("Check 2-Sided Mesh [" + this.name + "]");
				//Debug.Log("2-Sided Mesh Check / Vert Pos :" + _vertPositions.Length + ", Render Verts : " + _renderVerts.Length);
				if (_vertPositions.Length == _renderVerts.Length)
				{
					
					//이거의 개수를 두배로 늘려야 한다.
					//다른 리스트도 두배로 증가
					int nVert = _renderVerts.Length;
					int nVert2Side = _renderVerts.Length * 2;

					//Debug.LogError("Expand Verts : " + nVert + " > " + nVert2Side);

					Vector3[] prevVertPos = _vertPositions;
					Vector2[] prevVertUVs = _vertUVs;


					_vertPositions = new Vector3[nVert2Side];
					_vertUVs = new Vector2[nVert2Side];

					for (int i = 0; i < nVert; i++)
					{
						_vertPositions[i] = prevVertPos[i];
						_vertPositions[i + nVert] = prevVertPos[i];//<<nVert만큼 뒤에 더 추가

						_vertUVs[i] = prevVertUVs[i];
						_vertUVs[i + nVert] = prevVertUVs[i];
					}

					////Update, Local 배열도 다시 갱신
					//_vertPositions_Updated = new Vector3[_vertPositions.Length];
					//_vertPositions_Local = new Vector3[_vertPositions.Length];
					//_vertPositions_World = new Vector2[_vertPositions.Length];
					//for (int i = 0; i < _vertPositions.Length; i++)
					//{
					//	_vertPositions_Updated[i] = _vertPositions[i];
					//}
				}
			}
			


			if(_mesh == null)
			{
				_mesh = new Mesh();
				_mesh.name = this.name + "_Mesh (Instance)";
				_mesh.Clear();

				_mesh.vertices = _vertPositions;
				_mesh.triangles = _vertTris;
				_mesh.uv = _vertUVs;

				_mesh.RecalculateNormals();
				_mesh.RecalculateBounds();
			}

			
			
			if (_meshFilter == null)
			{
				_meshFilter = GetComponent<MeshFilter>();
			}

			_meshFilter.sharedMesh = _mesh;
			
			//_material_Instanced = null;
			//_material_Cur = null;
			//_isInitMaterial = false;

			if (_meshRenderer == null)
			{
				_meshRenderer = GetComponent<MeshRenderer>();
			}

			//_meshRenderer.material = _material;
			_meshRenderer.sharedMaterial = _material_Cur;

			


			_meshRenderer.enabled = _isVisibleDefault;
			_isVisible = _isVisibleDefault;
			



			_vertPositions_Updated = new Vector3[_vertPositions.Length];
			_vertPositions_Local = new Vector3[_vertPositions.Length];
			_vertPositions_World = new Vector2[_vertPositions.Length];
			for (int i = 0; i < _vertPositions.Length; i++)
			{
				_vertPositions_Updated[i] = _vertPositions[i];
			}

			//_texture_Updated = _texture;

			_isInitMesh = true;

			_cal_isFlippedBuffer = false;
			_cal_isFlippedBuffer_Prev = false;

			_isUseRiggingCache = false;

			//추가 19.7.3 : 버텍스 개수는 InitMesh에서 설정
			_nRenderVerts = (_renderVerts != null) ? _renderVerts.Length : 0;
			_nVertPos = (_vertPositions != null) ? _vertPositions.Length : 0;
			
		}

		/// <summary>
		/// [Please do not use it]
		/// Initialize Mesh
		/// </summary>
		public void InstantiateMesh()
		{	
			if(_mesh == null || _meshFilter == null || _meshFilter.mesh == null)
			{
				return;
			}

			_mesh = Instantiate<Mesh>(_meshFilter.sharedMesh);
			_meshFilter.mesh = _mesh;
		}


		//추가 : 먼저 바로 사용할 InstancedMaterial을 만든다.
		//이전과 달리 Batched / Shared는 런타임에서 만들어져서 연결한다.
		//기존 : Batched를 만들고 Instanced로 연결
		//변경 : Instanced를 먼저 만든 뒤, 공유 가능한 재질이 있는지 런타임에서 확인
		//AlphaMask와 Clipping도 만들자
		private void MakeInstancedMaterial()
		{
			//1. Material Instanced를 만들자.
			if (_material_Instanced == null)
			{
				//변경 19.6.16 : MaterialInfo를 이용하여 재질 만들기
				if (IsUseMaterialInfo)
				{
					apOptMaterialInfo matInfo = MaterialInfo;
					_material_Instanced = new Material(matInfo._shader);

					_material_Instanced.SetColor("_Color", _parentTransform._meshColor2X_Default);
					_material_Instanced.SetTexture("_MainTex", matInfo._mainTex);

					//추가 속성도 적용하자.
					matInfo.SetMaterialProperties(_material_Instanced);
				}
				else
				{
					//이전 방식
					if (_isMaskChild)
					{
						_material_Instanced = new Material(_shaderClipping);
					}
					else
					{
						_material_Instanced = new Material(_shaderNormal);
					}
					_material_Instanced.SetColor("_Color", _parentTransform._meshColor2X_Default);
					_material_Instanced.SetTexture("_MainTex", _texture);
				}
			}
			

			//2. Alpha Mask Material을 만들자.
			if(_isMaskParent && _materialAlphaMask == null)
			{
				_materialAlphaMask = new Material(_shader_AlphaMask);
			}

			_materialType = MATERIAL_TYPE.Instanced;
			_material_Cur = _material_Instanced;

			if(_meshRenderer != null && _meshRenderer.sharedMaterial == null)
			{
				_meshRenderer.sharedMaterial = _material_Cur;
			}
		}


		/// <summary>
		/// [Please do not use it]
		/// Initialize Materials
		/// </summary>
		public void InstantiateMaterial(apOptBatchedMaterial batchedMaterial)
		{
			if(_isInitMaterial)
			{
				return;
			}

			//1. Instanced Material(일반/Clipping)과 Alpha Mask Material을 만들자.
			MakeInstancedMaterial();

			//이제 Batched Material과 Shared Material을 각각 받아오자
			if(_isMaskChild)
			{
				//Mask Child라면 Batched/Shared를 사용하지 못한다.
				_material_Batched = null;
				_material_Shared = null;
				_materialUnit_Batched = null;
			}
			else
			{
				_materialUnit_Batched = batchedMaterial.GetMaterialUnit(_batchedMatID, this);
				if(_materialUnit_Batched != null)
				{
					_material_Batched = _materialUnit_Batched._material;
				}

				//변경 19.6.16
				if(IsUseMaterialInfo)
				{
					//Material Info를 사용한다면
					_material_Shared = batchedMaterial.GetSharedMaterial_MatInfo(MaterialInfo);
				}
				else
				{
					//이전 버전이라면
					_material_Shared = batchedMaterial.GetSharedMaterial_Prev(_texture, _shaderNormal);
				}
				
			}

			_materialType = MATERIAL_TYPE.Instanced;
			_meshRenderer.sharedMaterial = _material_Instanced;//<<일단 Instanced Material 넣기

			
			_isForceBatch2Shared = false;

			//자동으로 선택해보자
			AutoSelectMaterial();
			

			_isInitMaterial = true;
		}

		//---------------------------------------------------------------------------
		// Mask 관련 초기화
		//---------------------------------------------------------------------------
		/// <summary>
		/// [Please do not use it]
		/// Initialize if it is Mask Parent
		/// </summary>
		public void SetMaskBasicSetting_Parent(List<int> clipChildIDs)
		{
			if (clipChildIDs == null || clipChildIDs.Count == 0)
			{
				return;
			}
			_isMaskParent = true;
			_clipParentID = -1;
			_isMaskChild = false;


			if (_clipChildIDs == null || _clipChildIDs.Length != clipChildIDs.Count)
			{
				_clipChildIDs = new int[clipChildIDs.Count];
			}

			for (int i = 0; i < clipChildIDs.Count; i++)
			{
				_clipChildIDs[i] = clipChildIDs[i];
			}
			
		}

		/// <summary>
		/// [Please do not use it]
		/// Initialize if it is Mask Child
		/// </summary>
		public void SetMaskBasicSetting_Child(int parentID)
		{
			_isMaskParent = false;
			_clipParentID = parentID;
			_isMaskChild = true;

			_clipChildIDs = null;
		}


		#region [미사용 코드] Parent 중심의 Clipping
		//public void LinkAsMaskParent(apOptMesh[] childMeshes)
		//{
		//	if(_childOptMesh == null || _childOptMesh.Length != 3)
		//	{
		//		_childOptMesh = new apOptMesh[3];
		//	}

		//	_childOptMesh[0] = childMeshes[0];
		//	_childOptMesh[1] = childMeshes[1];
		//	_childOptMesh[2] = childMeshes[2];


		//	_meshRenderer.enabled = true;

		//	//이제 SubMesh 데이터를 만들어주자
		//	_subMeshes = new SubMeshData[4];
		//	//1. 자기 자신을 넣는다.
		//	int vertexIndexOffset = 0;
		//	_subMeshes[0] = new SubMeshData(SUBMESH_BASE, this, 0);
		//	_subMeshes[0].SetVisible(true);

		//	vertexIndexOffset += _subMeshes[0]._nVert;

		//	//2. 자식 Mesh를 넣는다.
		//	int iChildMesh = 1;
		//	for (int i = 0; i < 3; i++)
		//	{
		//		if(_childOptMesh[i] == null)
		//		{
		//			_subMeshes[iChildMesh] = null;
		//			iChildMesh++;
		//			continue;
		//		}

		//		_subMeshes[iChildMesh] = new SubMeshData(iChildMesh, _childOptMesh[i], vertexIndexOffset);
		//		vertexIndexOffset += _subMeshes[iChildMesh]._nVert;

		//		iChildMesh++;
		//	}

		//	int nTotalVerts = vertexIndexOffset;//<<전체 Vertex의 개수
		//										//이제 전체 Mesh를 만들자

		//	_vertPosList_ForMask = new Vector3[nTotalVerts];
		//	_vertColorList_ForMask = new Color[nTotalVerts];
		//	List<int> vertIndexList_ForMask = new List<int>();
		//	List<Vector2> vertUVs_ForMask = new List<Vector2>();

		//	for (int iSM = 0; iSM < 4; iSM++)
		//	{
		//		SubMeshData subMesh = _subMeshes[iSM];
		//		if(subMesh == null)
		//		{
		//			continue;
		//		}
		//		//Vertex 먼저
		//		Color vertColor = Color.clear;
		//		switch (iSM)
		//		{
		//			case SUBMESH_BASE: vertColor = VertexColor_Base; break;
		//			case SUBMESH_CLIP1: vertColor = VertexColor_Clip1; break;
		//			case SUBMESH_CLIP2: vertColor = VertexColor_Clip2; break;
		//			case SUBMESH_CLIP3: vertColor = VertexColor_Clip3; break;

		//		}
		//		for (int iVert = 0; iVert < subMesh._nVert; iVert++)
		//		{
		//			_vertPosList_ForMask[iVert + subMesh._vertIndexOffset] = subMesh._optMesh._renderVerts[iVert]._pos3_Local;
		//			_vertColorList_ForMask[iVert + subMesh._vertIndexOffset] = vertColor;
		//			vertUVs_ForMask.Add(subMesh._optMesh._vertUVs[iVert]);
		//		}

		//		for (int iTri = 0; iTri < subMesh._nTri; iTri++)
		//		{
		//			vertIndexList_ForMask.Add(subMesh._optMesh._vertTris[iTri] + subMesh._vertIndexOffset);
		//		}
		//	}

		//	_mesh.Clear();
		//	_mesh.vertices = _vertPosList_ForMask;
		//	_mesh.triangles = vertIndexList_ForMask.ToArray();
		//	_mesh.uv = vertUVs_ForMask.ToArray();
		//	_mesh.colors = _vertColorList_ForMask;



		//	//재질도 다시 세팅하자
		//	Color color_Base = new Color(0.5f, 0.5f, 0.5f, 1.0f);
		//	Color color_Clip1 = new Color(0.0f, 0.0f, 0.0f, 0.0f);
		//	Color color_Clip2 = new Color(0.0f, 0.0f, 0.0f, 0.0f);
		//	Color color_Clip3 = new Color(0.0f, 0.0f, 0.0f, 0.0f);

		//	Texture texture_Base = null;
		//	Texture texture_Clip1 = null;
		//	Texture texture_Clip2 = null;
		//	Texture texture_Clip3 = null;



		//	apPortrait.SHADER_TYPE shaderType_Clip1 = apPortrait.SHADER_TYPE.AlphaBlend;
		//	apPortrait.SHADER_TYPE shaderType_Clip2 = apPortrait.SHADER_TYPE.AlphaBlend;
		//	apPortrait.SHADER_TYPE shaderType_Clip3 = apPortrait.SHADER_TYPE.AlphaBlend;

		//	if(_subMeshes[SUBMESH_BASE] != null)
		//	{
		//		if (_subMeshes[SUBMESH_BASE]._isVisible)
		//		{
		//			color_Base = _subMeshes[SUBMESH_BASE].MeshColor;
		//		}
		//		else
		//		{
		//			color_Base = Color.clear;
		//		}
		//		texture_Base = _subMeshes[SUBMESH_BASE]._texture;
		//	}

		//	if(_subMeshes[SUBMESH_CLIP1] != null)
		//	{
		//		if(_subMeshes[SUBMESH_CLIP1]._isVisible)
		//		{	
		//			color_Clip1 = _subMeshes[SUBMESH_CLIP1].MeshColor;
		//		}
		//		else
		//		{
		//			color_Clip1 = Color.clear;
		//		}

		//		texture_Clip1 = _subMeshes[SUBMESH_CLIP1]._texture;
		//		shaderType_Clip1 = _subMeshes[SUBMESH_CLIP1]._optMesh._shaderType;
		//	}

		//	if(_subMeshes[SUBMESH_CLIP2] != null)
		//	{
		//		if(_subMeshes[SUBMESH_CLIP2]._isVisible)
		//		{
		//			color_Clip2 = _subMeshes[SUBMESH_CLIP2].MeshColor;
		//		}
		//		else
		//		{
		//			color_Clip2 = Color.clear;
		//		}

		//		texture_Clip2 = _subMeshes[SUBMESH_CLIP2]._texture;
		//		shaderType_Clip2 = _subMeshes[SUBMESH_CLIP2]._optMesh._shaderType;
		//	}

		//	if(_subMeshes[SUBMESH_CLIP3] != null)
		//	{
		//		if (_subMeshes[SUBMESH_CLIP3]._isVisible)
		//		{
		//			color_Clip3 = _subMeshes[SUBMESH_CLIP3].MeshColor;
		//		}
		//		else
		//		{
		//			color_Clip3 = Color.clear;
		//		}
		//		texture_Clip3 = _subMeshes[SUBMESH_CLIP3]._texture;
		//		shaderType_Clip3 = _subMeshes[SUBMESH_CLIP3]._optMesh._shaderType;
		//	}

		//	//_material = new Material(Shader.Find("AnyPortrait/Transparent/Masked Colored Texture (2X)"));
		//	//Debug.Log("Link Mask Clip - " + _shaderName_Clipping);
		//	//_material = new Material(Shader.Find(_shaderName_Clipping));
		//	_material = new Material(_shaderClipping);

		//	_material.SetColor("_Color", color_Base);
		//	_material.SetColor("_Color1", color_Clip1);
		//	_material.SetColor("_Color2", color_Clip2);
		//	_material.SetColor("_Color3", color_Clip3);

		//	_material.SetTexture("_MainTex", texture_Base);
		//	_material.SetTexture("_ClipTexture1", texture_Clip1);
		//	_material.SetTexture("_ClipTexture2", texture_Clip2);
		//	_material.SetTexture("_ClipTexture3", texture_Clip3);


		//	Debug.Log("Link Clip : " + shaderType_Clip1 + " / " + shaderType_Clip2 + " / " + shaderType_Clip3);

		//	_material.SetColor("_BlendOpt1", ShaderTypeColor[(int)shaderType_Clip1]);
		//	_material.SetColor("_BlendOpt2", ShaderTypeColor[(int)shaderType_Clip2]);
		//	_material.SetColor("_BlendOpt3", ShaderTypeColor[(int)shaderType_Clip3]);

		//	_meshRenderer.sharedMaterial = _material;

		//	RefreshMaskedMesh();

		//	_mesh.RecalculateBounds();
		//	_mesh.RecalculateNormals();


		//} 
		#endregion


		/// <summary>
		/// [Please do not use it]
		/// Initialize reference
		/// </summary>
		public void LinkAsMaskChild(apOptMesh parentMesh)
		{
			_parentOptMesh = parentMesh;

			
			if(_meshRenderer.sharedMaterial == null ||
				_material_Instanced == null)
			{
				MakeInstancedMaterial();
				_meshRenderer.sharedMaterial = _material_Instanced;
			}
		}

		//Mask Parent의 세팅을 리셋한다.
		//카메라 설정이나 씬이 변경되었을 때 호출해야한다.
		/// <summary>
		/// If it is Mask Parent, reset Command Buffers to Camera
		/// </summary>
		public void ResetMaskParentSetting()
		{
			CleanUpMaskParent();

#if UNITY_EDITOR
			if (!Application.isPlaying)
			{
				return;
			}
#endif

			RegistCommandBuffer();
		}

		
		//---------------------------------------------------------------------------


		// Update
		//------------------------------------------------
		void Update()
		{
			
		}

		void LateUpdate()
		{

		}
		





		// 외부 업데이트
		//------------------------------------------------
		public void ReadyToUpdate()
		{
			//?
		}

		/// <summary>
		/// [Please do not use it]
		/// Update Shape of Mesh
		/// </summary>
		/// <param name="isRigging"></param>
		/// <param name="isVertexLocal"></param>
		/// <param name="isVertexWorld"></param>
		/// <param name="isVisible"></param>
		public void UpdateCalculate(bool isRigging, bool isVertexLocal, bool isVertexWorld, bool isVisible, bool isOrthoCorrection)
		{
			_cal_isVisibleRequest = _isVisible;
			
			if (!_isHide_External)
			{
				_cal_isVisibleRequest = isVisible;
			}
			else
			{
				//강제로 Hide하는 요청이 있었다면?
				_cal_isVisibleRequest = false;
			}

			_cal_isVisibleRequest_Masked = _cal_isVisibleRequest;

			//추가
			//Mask 메시가 있다면
			if(_isMaskChild)
			{
				if(_parentOptMesh != null)
				{
					_cal_isVisibleRequest_Masked = _parentOptMesh._isVisible;
				}
			}

			//둘다 True일때 Show, 하나만 False여도 Hide
			//상태가 바뀔때에만 Show/Hide 토글
			if(_cal_isVisibleRequest && _cal_isVisibleRequest_Masked)
			{
				if(!_isVisible)
				{
					Show();
				}
			}
			else
			{
				if(_isVisible)
				{
					Hide();
				}
			}


			//안보이는건 업데이트하지 말자
			if (_isVisible)
			{
				//추가 2.25 : Flipped 관련 코드가 업데이트 초기에 등장한다.
				if (_cal_isRootFlipped)
				{
					//이전 계산에서 Flipped가 되었다면 일단 초기화
					_transform.localScale = Vector3.one;
					_transform.localPosition = new Vector3(-_pivotPos.x, -_pivotPos.y, 0);
				}

				////추가 : World 좌표계의 Flip을 별도로 계산한다.
				_cal_isRootFlipped_X = _parentTransform._rootUnit.IsFlippedX;
				_cal_isRootFlipped_Y = _parentTransform._rootUnit.IsFlippedY;
				//_cal_isRootFlipped_X = _transform.lossyScale.x < 0.0f;
				//_cal_isRootFlipped_Y = _transform.lossyScale.y < 0.0f;
				_cal_isRootFlipped = (_cal_isRootFlipped_X && !_cal_isRootFlipped_Y)
									|| (!_cal_isRootFlipped_X && _cal_isRootFlipped_Y);

				float flipWeight_X = 1;
				float flipWeight_Y = 1;

				if(_cal_isRootFlipped)
				{
					flipWeight_X = _cal_isRootFlipped_X ? -1 : 1;
					flipWeight_Y = _cal_isRootFlipped_Y ? -1 : 1;
				}
				

				_cal_Matrix_TFResult_World = _parentTransform._matrix_TFResult_World.MtrxToSpace;

				apOptRenderVertex rVert = null;

#if UNITY_EDITOR
				UnityEngine.Profiling.Profiler.BeginSample("Opt Mesh - Update calculate Render Vertices");
#endif
				

				apOptCalculatedResultStack calculateStack = _parentTransform.CalculatedStack;

				for (int i = 0; i < _nRenderVerts; i++)
				{

					rVert = _renderVerts[i];

					//리깅 추가
					if (isRigging)
					{
						//지연 코드 버전
						//이전 버전
						//rVert._matrix_Rigging.SetMatrixWithWeight(calculateStack.GetDeferredRiggingMatrix(i), calculateStack._result_RiggingWeight);

						//개선된 버전 + 캐시
						if (!_isUseRiggingCache)
						{
							rVert._matrix_Rigging.SetMatrixWithWeight(
								calculateStack.GetDeferredRiggingMatrix(i),
								calculateStack._result_RiggingWeight * calculateStack.GetDeferredRiggingWeight(i)//<이 Weight는 런타임에서는 바뀌지 않는다.
								);
						}
						else
						{
							//캐시를 이용해서 Weight를 가져온다.
							rVert._matrix_Rigging.SetMatrixWithWeight(
								calculateStack.GetDeferredRiggingMatrix(i),
								calculateStack._result_RiggingWeight * calculateStack.GetDeferredRiggingWeightCache(i)
								);
						}

						//테스트
						//if (calculateStack.GetDeferredRiggingWeight(i) < 0.7f)
						//{
						//	Debug.Log("[" + name + "] Rigging Weight가 0인 Vert <" + i + "> (" + calculateStack.GetDeferredRiggingWeight(i) + ")");
						//	Debug.Log(rVert._matrix_Rigging.ToString());
						//}
					}

					if (isVertexLocal)
					{
						rVert._matrix_Cal_VertLocal.SetTRS(calculateStack.GetDeferredLocalPos(i));
					}

					//rVert.SetMatrix_3_Transform_Mesh(_parentTransform._matrix_TFResult_World.MtrxToSpace);//<<기존
					rVert.SetMatrix_3_Transform_Mesh(_cal_Matrix_TFResult_World);//<<한번 더 계산된 값으로 변경
					
					if (isVertexWorld)
					{
						rVert._matrix_Cal_VertWorld.SetTRS(calculateStack._result_VertWorld[i]);
					}

					//추가
					if(isOrthoCorrection)
					{
						rVert.SetMatrix_5_OrthoCorrection(_parentTransform._convert2TargetMatrix3x3);
					}

					//추가
					rVert.SetMatrix_6_FlipWeight(flipWeight_X, flipWeight_Y);

					rVert.Calculate();

					//업데이트 데이터를 넣어준다.
					_vertPositions_Updated[i] =  rVert._vertPos3_LocalUpdated;
					_vertPositions_World[i] = rVert._vertPos_World;
					
				}

				//추가 19.7.3 : 양면인 경우에는, 뒤쪽면도 업데이트를 해야한다.
				if(_isAlways2Side)
				{
					for (int i = 0; i < _nRenderVerts; i++)
					{
						rVert = _renderVerts[i];
						_vertPositions_Updated[i + _nRenderVerts] = rVert._vertPos3_LocalUpdated;
						_vertPositions_World[i + _nRenderVerts] = rVert._vertPos_World;
					}
				}

				//리깅 캐시를 사용하는 것으로 변경
				if(!_isUseRiggingCache)
				{
					_isUseRiggingCache = true;
				}
#if UNITY_EDITOR
				UnityEngine.Profiling.Profiler.EndSample();
#endif
			}


			//_material.SetColor("_Color", _multiplyColor * _parentTransform._meshColor2X);


			//색상을 제어할 때
			//만약 Color 기본값인 경우 Batch를 위해 Shared로 교체해야한다.
			//색상 지정은 Instance일때만 가능하다

			//if ((_isAnyMeshColorRequest || _parentTransform._isAnyColorCalculated || !_isDefaultColorGray) && _instanceMaterial != null)//이전
			if (_isAnyMeshColorRequest || _parentTransform._isAnyColorCalculated || !_isDefaultColorGray)
			{
				//이전 코드
				//if(_isUseSharedMaterial && !_isMaskChild)
				//{
				//	//Shared를 쓰는 중이라면 교체해야함
				//	AutoSelectMaterial();
				//}

				//일단 색상을 먼저 계산한다. (값이 바뀌었다고 하니까..)
				_cal_MeshColor.r = _multiplyColor.r * _parentTransform._meshColor2X.r * 2;
				_cal_MeshColor.g = _multiplyColor.g * _parentTransform._meshColor2X.g * 2;
				_cal_MeshColor.b = _multiplyColor.b * _parentTransform._meshColor2X.b * 2;
				_cal_MeshColor.a = _multiplyColor.a * _parentTransform._meshColor2X.a;//Alpha는 2X가 아니다.
				
				//_instanceMaterial.SetColor(_shaderID_Color, _cal_MeshColor);
				_material_Instanced.SetColor(_shaderID_Color, _cal_MeshColor);

				//바로 호출하기 전에
				//만약 결과 값이 Gray(0.5, 0.5, 0.5, 1.0)이라면
				//Shared를 써야한다.

				AutoSelectMaterial(true);//일단 호출해보는 것으로..
			}
			else
			{
				//이전
				//if(!_isUseSharedMaterial && !_isMaskChild)
				//{
				//	//반대로 색상 선택이 없는데 Instance Material을 사용중이라면 Batch를 해야하는 건 아닌지 확인해보자
				//	AutoSelectMaterial();
				//}

				//변경
				if(_materialType == MATERIAL_TYPE.Instanced && !_isMaskChild)
				{
					//반대로 색상 선택이 없는데 Instance Material을 사용중이라면 Shared나 Batch를 써야할 것이다.
					AutoSelectMaterial();
				}
			}

			
			if(_isMaskChild)
			{
				//Parent의 Mask를 받아서 넣자
				if (_parentOptMesh != null)
				{
					_curParentRenderTexture = _parentOptMesh.MaskRenderTexture;
				}
				else
				{
					_curParentRenderTexture = null;
					Debug.LogError("Null Parent");
				}

				if(_curParentRenderTexture != _prevParentRenderTexture)
				{
					//_material.SetTexture(_shaderID_MaskTexture, _curParentRenderTexture);//이전
					_material_Instanced.SetTexture(_shaderID_MaskTexture, _curParentRenderTexture);

					_prevParentRenderTexture = _curParentRenderTexture;
				}
				//_material.SetVector(_shaderID_MaskScreenSpaceOffset, _parentOptMesh._maskScreenSpaceOffset);//이전
				_material_Instanced.SetVector(_shaderID_MaskScreenSpaceOffset, _parentOptMesh._maskScreenSpaceOffset);
			}

//#if UNITY_EDITOR
//			Profiler.BeginSample("Opt Mesh - Refresh Mesh");
//#endif

			#region [미사용 코드 : 스텐실 병합 방식에서는 MaskChild는 Refresh를 생략했다.]
			//if (!_isMaskChild)
			//{
			//	//Mask와 관련이 없는 경우만 갱신해준다.
			//	//메시 갱신
			//	RefreshMesh();
			//} 
			#endregion

			RefreshMesh();

			if(_isMaskParent)
			{
				//MaskParent면 CommandBuffer를 갱신한다.
				UpdateCommandBuffer();
			}

			if(_mesh != null)
			{
				_mesh.RecalculateNormals();
			}

//#if UNITY_EDITOR
//			Profiler.EndSample();
//#endif
		}


		public void RefreshMaskMesh_WithoutUpdateCalculate()
		{
			//Calculate는 하지 않고
			if(_isMaskParent)
			{
				//MaskParent면 CommandBuffer를 갱신한다.
				UpdateCommandBuffer();
			}

			if(_isMaskChild)
			{
				//Parent의 Mask를 받아서 넣자
				if (_parentOptMesh != null)
				{
					_curParentRenderTexture = _parentOptMesh.MaskRenderTexture;
				}
				else
				{
					_curParentRenderTexture = null;
					Debug.LogError("Null Parent");
				}

				if(_curParentRenderTexture != _prevParentRenderTexture)
				{
					//Debug.Log("Set Parent RenderTexture : " + (_curParentRenderTexture != null));
					
					//_material.SetTexture(_shaderID_MaskTexture, _curParentRenderTexture);//이전
					_material_Cur.SetTexture(_shaderID_MaskTexture, _curParentRenderTexture);

					_prevParentRenderTexture = _curParentRenderTexture;
				}
				
				//_material.SetVector(_shaderID_MaskScreenSpaceOffset, _parentOptMesh._maskScreenSpaceOffset);//이전
				_material_Cur.SetVector(_shaderID_MaskScreenSpaceOffset, _parentOptMesh._maskScreenSpaceOffset);
			}
		}


		// Vertex Refresh
		//------------------------------------------------
		/// <summary>
		/// [Please do not use it]
		/// </summary>
		public void RefreshMesh()
		{
			//if(_isMaskChild || _isMaskParent)
			//{
			//	return;
			//}

			//변경 -> MaskParent는 그대로 업데이트 가능하고, Child만 따로 업데이트하자
			#region [미사용 코드 : 스텐실 병합 방식에서는 MaskChild는 Refresh를 생략했다.]
			//if (_isMaskChild)
			//{
			//	return;
			//} 
			#endregion



			//Flipped 계산


			//Root의 Scale의 방향이 바뀌었으면 Flipped을 해야한다.
			if (_cal_isRootFlipped)
			{
				//_cal_isRootFlipped_Prev = _cal_isRootFlipped;

				_transform.localScale = new Vector3((_cal_isRootFlipped_X ? -1.0f : 1.0f),
														(_cal_isRootFlipped_Y ? -1.0f : 1.0f),
														1.0f);

				_transform.localPosition = new Vector3((_cal_isRootFlipped_X ? _pivotPos.x : -_pivotPos.x),
														(_cal_isRootFlipped_Y ? _pivotPos.y : -_pivotPos.y),
														0);

				//Debug.Log("Mesh [" + this.name + "] Changed" + (_cal_isRootFlipped_X ? " : Flipped X" : "") + (_cal_isRootFlipped_Y ? " : Flipped Y" : "") );
			}

			_cal_isUpdateFlipped_X = _parentTransform._matrix_TFResult_World._scale.x < 0.0f;
			_cal_isUpdateFlipped_Y = _parentTransform._matrix_TFResult_World._scale.y < 0.0f;
			_cal_isUpdateFlipped = (_cal_isUpdateFlipped_X && !_cal_isUpdateFlipped_Y)
								|| (!_cal_isUpdateFlipped_X && _cal_isUpdateFlipped_Y);

			_cal_isFlippedBuffer = (_cal_isUpdateFlipped && !_cal_isRootFlipped)
								|| (!_cal_isUpdateFlipped && _cal_isRootFlipped);

			//_cal_isFlippedBuffer = _cal_isUpdateFlipped;
			//_cal_isFlippedBuffer = ( *
			//					;

			//Transform 제어 -> Vert 제어
			if (_isMaskParent)
			{
				//마스크 처리를 위해서 Vertex의 위치나 분포를 저장해야한다.
				_vertPosCenter = Vector3.zero;
				//_vertRangeMax = -1.0f;

				//Left < Right
				//Bottom < Top

				_vertRange_XMin = float.MaxValue;//Max -> Min
				_vertRange_XMax = float.MinValue;//Min -> Max
				_vertRange_YMin = float.MaxValue;//Max -> Min
				_vertRange_YMax = float.MinValue;//Min -> Max

				for (int i = 0; i < _nVertPos; i++)
				{
					//_vertPositions_Local[i] = _transform.InverseTransformPoint(_vertPositions_Updated[i]);
					_vertPositions_Local[i] = _vertPositions_Updated[i];

					_vertRange_XMin = Mathf.Min(_vertRange_XMin, _vertPositions_Local[i].x);
					_vertRange_XMax = Mathf.Max(_vertRange_XMax, _vertPositions_Local[i].x);
					_vertRange_YMin = Mathf.Min(_vertRange_YMin, _vertPositions_Local[i].y);
					_vertRange_YMax = Mathf.Max(_vertRange_YMax, _vertPositions_Local[i].y);
				}

				//마스크를 만들 영역을 잡아준다.
				//추가 6.6 : 약간의 공백을 더 넣어준다.
				_vertRange_XMin -= 2.0f;
				_vertRange_XMax += 2.0f;
				_vertRange_YMin -= 2.0f;
				_vertRange_YMax += 2.0f;

				_vertPosCenter.x = (_vertRange_XMin + _vertRange_XMax) * 0.5f;
				_vertPosCenter.y = (_vertRange_YMin + _vertRange_YMax) * 0.5f;
				//_vertRangeMax = Mathf.Max(_vertRange_XMax - _vertRange_XMin, _vertRange_YMax - _vertRange_YMin);
			}
			else
			{
				for (int i = 0; i < _nVertPos; i++)
				{
					//_vertPositions_Local[i] = _transform.InverseTransformPoint(_vertPositions_Updated[i]);
					_vertPositions_Local[i] = _vertPositions_Updated[i];
				}
			}
			


			_mesh.vertices = _vertPositions_Local;
			_mesh.uv = _vertUVs;
			//추가3.22 : Flip 여부에 따라서 다른 Vertex 배열을 사용한다.
			if (_isAlways2Side)
			{
				_mesh.triangles = _vertTris;
				//_mesh.RecalculateNormals();
			}
			else
			{
				if (_cal_isFlippedBuffer)
				{
					_mesh.triangles = _vertTris_Flipped;
				}
				else
				{
					_mesh.triangles = _vertTris;
				}

				if (_cal_isFlippedBuffer_Prev != _cal_isFlippedBuffer)
				{
					//Flip 여부가 바뀔 대
					//Normal을 다시 계산한다.
					_mesh.RecalculateNormals();
					_cal_isFlippedBuffer_Prev = _cal_isFlippedBuffer;
				}
			}
			
			
			

			_mesh.RecalculateBounds();

		}



		#region [미사용 코드 : 스텐실 병합 방식]
		//public void RefreshClippedMesh()
		//{
		//	if (!_isMaskChild)
		//	{
		//		return;
		//	}
		//	apOptMesh targetMesh = null;
		//	int iVertOffset = 0;
		//	for (int i = 0; i < 2; i++)
		//	{

		//		if (i == 0)
		//		{
		//			targetMesh = _parentOptMesh;
		//			iVertOffset = 0;
		//		}
		//		else
		//		{
		//			targetMesh = this;
		//			iVertOffset = _nVertParent;
		//		}
		//		int nVert = targetMesh._nVert;
		//		for (int iVert = 0; iVert < nVert; iVert++)
		//		{
		//			_vertPosList_ClippedMerge[iVert + iVertOffset] =
		//				_transform.InverseTransformPoint(
		//					targetMesh._transform.TransformPoint(
		//						targetMesh._vertPositions_Updated[iVert]));
		//		}
		//	}


		//	_material.SetColor("_Color", MeshColor);
		//	_material.SetColor("_MaskColor", _parentOptMesh.MeshColor);
		//	_mesh.vertices = _vertPosList_ClippedMerge;



		//	_mesh.RecalculateBounds();
		//	_mesh.RecalculateNormals();

		//} 
		#endregion

		//--------------------------------------------------------------------------------
		// 클리핑 Mask Parent
		//--------------------------------------------------------------------------------
		//MaskParent일때, 커맨드 버퍼를 초기화한다.
		/// <summary>
		/// Clean up Command Buffers if it is Mask Parent
		/// </summary>
		public void CleanUpMaskParent()
		{
			if (!_isMaskParent)
			{
				return;
			}

			_isRenderTextureCreated = false;
			if (_targetCamera != null && _commandBuffer != null)
			{
				_targetCamera.RemoveCommandBuffer(CameraEvent.BeforeForwardOpaque, _commandBuffer);
			}
			_targetCamera = null;
			cameraTransform = null;
			_commandBuffer = null;

			_maskRenderTargetID = -1;
			if (_maskRenderTexture != null)
			{
				RenderTexture.ReleaseTemporary(_maskRenderTexture);
				_maskRenderTexture = null;
			}
		}

		private void RegistCommandBuffer()
		{
			if(!_isMaskParent)
			{
				CleanUpMaskParent();
				return;
			}

			//Debug.Log("RegistCommandBuffer [" + this.name + " - " + _isRenderTextureCreated + "]");
			if(_isRenderTextureCreated)
			{
				//이미 생성되었으면 포기
				//만약 다시 세팅하고 싶다면 RenderTexture 초기화 함수를 호출하자
				return;
			}

			

			if(//_material == null 
				_material_Cur == null
				|| _materialAlphaMask == null 
				|| _mesh == null)
			{
				//재질 생성이 안되었다면 포기
				//Debug.LogError("No Init [" + this.name + "]");
				return;
			}
			
			Camera[] cameras = Camera.allCameras;
			if(cameras == null || cameras.Length == 0)
			{
				//Debug.LogError("NoCamera");
				return;
			}
			
			Camera targetCam = null;
			Camera cam = null;
			int layer = gameObject.layer;
			//이걸 바라보는 카메라가 하나 있으면 이걸로 설정.
			for (int i = 0; i < cameras.Length; i++)
			{
				cam = cameras[i];

				if(cam.cullingMask == (cam.cullingMask | (1 << gameObject.layer)) && cam.enabled)
				{
					//이 카메라는 이 객체를 바라본다.
					targetCam = cam;
					break;
				}
			}

			if(targetCam == null)
			{
				//잉?
				//유효한 카메라가 없네요.
				//Clean하고 초기화
				//Debug.LogError("NoCamera To Render");
				CleanUpMaskParent();
				return;
			}

			_targetCamera = targetCam;
			cameraTransform = _targetCamera.transform;
			
			
			if(_maskRenderTexture == null)
			{
				_maskRenderTexture = RenderTexture.GetTemporary(_clippingRenderTextureSize, _clippingRenderTextureSize, 24, RenderTextureFormat.Default);
				_maskRenderTargetID = new RenderTargetIdentifier(_maskRenderTexture);
			}

			//_materialAlphaMask.SetTexture(_shaderID_MainTex, _material.mainTexture);
			//_materialAlphaMask.SetColor(_shaderID_Color, _material.color);

			_materialAlphaMask.SetTexture(_shaderID_MainTex, _material_Cur.mainTexture);
			_materialAlphaMask.SetColor(_shaderID_Color, _material_Cur.color);

			_commandBuffer = new CommandBuffer();
			_commandBuffer.name = "AP Clipping Mask [" + name + "]";
			_commandBuffer.SetRenderTarget(_maskRenderTargetID, 0);
			_commandBuffer.ClearRenderTarget(true, true, Color.clear);

			//일단은 기본값
			_vertPosCenter = Vector2.zero;
			//_vertRangeMax = -1.0f;

			_maskScreenSpaceOffset.x = 0;
			_maskScreenSpaceOffset.y = 0;
			_maskScreenSpaceOffset.z = 1;
			_maskScreenSpaceOffset.w = 1;



			_commandBuffer.DrawMesh(_mesh, transform.localToWorldMatrix, _materialAlphaMask);

			_targetCamera.AddCommandBuffer(CameraEvent.BeforeForwardOpaque, _commandBuffer);

			//Debug.Log("[" + name + "] OptMesh Parent Make Render Texture");

			_isRenderTextureCreated = true;
		}

		private void UpdateCommandBuffer()
		{
			if (!_isVisible
				|| !_isMaskParent
				|| !_isRenderTextureCreated
				|| _commandBuffer == null
				|| _targetCamera == null)
			{
				return;
			}

			//현재 Mesh의 화면상의 위치를 체크하여 적절히 "예쁘게 찍히도록" 만든다.
			//크기 비율
			//여백을 조금 추가한다.
			if (_targetCamera.orthographic)
			{
				_vertPosCenter.z = 0;
			}

			_cal_localPos_LT = new Vector3(_vertRange_XMin, _vertRange_YMax, 0);
			_cal_localPos_RB = new Vector3(_vertRange_XMax, _vertRange_YMin, 0);


			_cal_vertWorldPos_Center = transform.TransformPoint(_vertPosCenter);

			_cal_vertWorldPos_LT = transform.TransformPoint(_cal_localPos_LT);
			_cal_vertWorldPos_RB = transform.TransformPoint(_cal_localPos_RB);

			_cal_screenPos_Center = _targetCamera.WorldToScreenPoint(_cal_vertWorldPos_Center);
			_cal_screenPos_LT = _targetCamera.WorldToScreenPoint(_cal_vertWorldPos_LT);
			_cal_screenPos_RB = _targetCamera.WorldToScreenPoint(_cal_vertWorldPos_RB);

			if (!_targetCamera.orthographic)
			{
				Vector3 centerSceenPos = _cal_screenPos_LT * 0.5f + _cal_screenPos_RB * 0.5f;
				float distLT2RB_Half = 0.5f * Mathf.Sqrt(
					(_cal_screenPos_LT.x - _cal_screenPos_RB.x) * (_cal_screenPos_LT.x - _cal_screenPos_RB.x) 
					+ (_cal_screenPos_LT.y - _cal_screenPos_RB.y) * (_cal_screenPos_LT.y - _cal_screenPos_RB.y));
				distLT2RB_Half *= 1.6f;

				_cal_screenPos_LT.x = centerSceenPos.x - distLT2RB_Half;
				_cal_screenPos_LT.y = centerSceenPos.y - distLT2RB_Half;

				_cal_screenPos_RB.x = centerSceenPos.x + distLT2RB_Half;
				_cal_screenPos_RB.y = centerSceenPos.y + distLT2RB_Half;

				//_cal_screenPos_LT = (_cal_screenPos_LT - centerSceenPos) * 1.6f + centerSceenPos;
				//_cal_screenPos_RB = (_cal_screenPos_RB - centerSceenPos) * 1.6f + centerSceenPos;
			}
			

			//모든 버텍스가 화면안에 들어온다면 Sceen 좌표계 Scale이 0~1의 값을 가진다.
			_cal_prevSizeWidth = Mathf.Abs(_cal_screenPos_LT.x - _cal_screenPos_RB.x) / (float)Screen.width;
			_cal_prevSizeHeight = Mathf.Abs(_cal_screenPos_LT.y - _cal_screenPos_RB.y) / (float)Screen.height;

			if (_cal_prevSizeWidth < 0.001f) { _cal_prevSizeWidth = 0.001f; }
			if (_cal_prevSizeHeight < 0.001f) { _cal_prevSizeHeight = 0.001f; }


			//화면에 가득 찰 수 있도록 확대하는 비율은 W, H 중에서 "덜 확대하는 비율"로 진행한다.
			_cal_zoomScale = Mathf.Min(1.0f / _cal_prevSizeWidth, 1.0f / _cal_prevSizeHeight);

			//메시 자체를 평행이동하여 화면 중앙에 위치시켜야 한다.

			//<<이거 속도 빠르게 하자
			//_materialAlphaMask.SetTexture(_shaderID_MainTex, _material.mainTexture);
			//_materialAlphaMask.SetColor(_shaderID_Color, _material.color);

			_materialAlphaMask.SetTexture(_shaderID_MainTex, _material_Cur.mainTexture);
			_materialAlphaMask.SetColor(_shaderID_Color, _material_Cur.color);
			

			_cal_aspectRatio = (float)Screen.width / (float)Screen.height;
			if (_targetCamera.orthographic)
			{
				_cal_newOrthoSize = _targetCamera.orthographicSize / _cal_zoomScale;
				//Debug.Log("Ortho Scaled Size : " + _cal_newOrthoSize + "( Center : "+ _cal_screenPos_Center + " )");
			}
			else
			{
				//_cal_newOrthoSize = _targetCamera.nearClipPlane * Mathf.Tan(_targetCamera.fieldOfView * 0.5f * Mathf.Deg2Rad) / _cal_zoomScale;
				float zDepth = Mathf.Abs(_targetCamera.worldToCameraMatrix.MultiplyPoint3x4(_cal_vertWorldPos_Center).z);
				
				_cal_newOrthoSize = zDepth * Mathf.Tan(_targetCamera.fieldOfView * 0.5f * Mathf.Deg2Rad) / _cal_zoomScale;
				//Debug.Log("Ortho Scaled Size (Pers) : " + _cal_newOrthoSize + "( Center : " + _cal_screenPos_Center + " ) " + gameObject.name);
				
			}


			_cal_centerMoveOffset = new Vector2(_cal_screenPos_Center.x - (Screen.width / 2), _cal_screenPos_Center.y - (Screen.height / 2));
			_cal_centerMoveOffset.x /= (float)Screen.width;
			_cal_centerMoveOffset.y /= (float)Screen.height;

			_cal_centerMoveOffset.x *= _cal_aspectRatio * _cal_newOrthoSize;
			_cal_centerMoveOffset.y *= _cal_newOrthoSize;

			//다음 카메라 위치는
			//카메라가 바라보는 Ray를 역으로 쐈을때 Center -> Ray*Dist 만큼의 위치
			_cal_distCenterToCamera = Vector3.Distance(_cal_vertWorldPos_Center, _targetCamera.transform.position);
			_cal_nextCameraPos = _cal_vertWorldPos_Center + _targetCamera.transform.forward * (-_cal_distCenterToCamera);
			//_cal_camOffset = _cal_vertWorldPos_Center - _targetCamera.transform.position;

			_cal_customWorldToCamera = Matrix4x4.TRS(_cal_nextCameraPos, cameraTransform.rotation, Vector3.one).inverse;
			_cal_customWorldToCamera.m20 *= -1f;
			_cal_customWorldToCamera.m21 *= -1f;
			_cal_customWorldToCamera.m22 *= -1f;
			_cal_customWorldToCamera.m23 *= -1f;

			// CullingMatrix = Projection * WorldToCamera
			_cal_customCullingMatrix = Matrix4x4.Ortho(	-_cal_aspectRatio * _cal_newOrthoSize,    //Left
													_cal_aspectRatio * _cal_newOrthoSize,     //Right
													-_cal_newOrthoSize,                  //Bottom
													_cal_newOrthoSize,                   //Top
													//_targetCamera.nearClipPlane, _targetCamera.farClipPlane
													_cal_distCenterToCamera - 10,        //Near
													_cal_distCenterToCamera + 50         //Far
													)
								* _cal_customWorldToCamera;

			
			_cal_newLocalToProjMatrix = _cal_customCullingMatrix * transform.localToWorldMatrix;
			_cal_newWorldMatrix = _targetCamera.cullingMatrix.inverse * _cal_newLocalToProjMatrix;
			
			_commandBuffer.Clear();
			_commandBuffer.SetRenderTarget(_maskRenderTargetID, 0);
			_commandBuffer.ClearRenderTarget(true, true, Color.clear);

			_commandBuffer.DrawMesh(_mesh, _cal_newWorldMatrix, _materialAlphaMask);
			
			//ScreenSpace가 얼마나 바뀌었는가
			_cal_screenPosOffset = new Vector3(Screen.width / 2, Screen.height / 2, 0) - _cal_screenPos_Center;
			
			_maskScreenSpaceOffset.x = (_cal_screenPosOffset.x / (float)Screen.width);
			_maskScreenSpaceOffset.y = (_cal_screenPosOffset.y / (float)Screen.height);
			_maskScreenSpaceOffset.z = _cal_zoomScale;
			_maskScreenSpaceOffset.w = _cal_zoomScale;

		}

		// Functions
		//------------------------------------------------
		/// <summary>
		/// Show Mesh
		/// </summary>
		/// <param name="isResetHideFlag"></param>
		public void Show(bool isResetHideFlag = false)
		{	
			if(isResetHideFlag)
			{
				_isHide_External = false;
			}
			_meshRenderer.enabled = true;
			_isVisible = true;

			if (_isMaskParent)
			{
				CleanUpMaskParent();

#if UNITY_EDITOR
			if (!Application.isPlaying)
			{
				return;
			}
#endif

				RegistCommandBuffer();
			}

			_isUseRiggingCache = false;
		}

		/// <summary>
		/// Hide Mesh
		/// </summary>
		public void Hide()
		{
			_meshRenderer.enabled = false;
			_isVisible = false;

			if (_isMaskParent)
			{
				CleanUpMaskParent();
			}

			_isUseRiggingCache = false;
		}

		/// <summary>
		/// Show or Hide by default
		/// </summary>
		public void SetVisibleByDefault()
		{
			if(_isVisibleDefault)
			{
				Show(true);
			}
			else
			{
				Hide();
			}
		}

		/// <summary>
		/// Hide Mesh ignoring the result
		/// </summary>
		/// <param name="isHide"></param>
		public void SetHideForce(bool isHide)
		{
			_isHide_External = isHide;

			//실제 Visible 갱신은 다음 프레임의 업데이트때 수행된다.
		}





		//---------------------------------------------------------
		// Shader 제어 함수들
		//---------------------------------------------------------
		//추가 12.14
		// 각 함수에 isOverlapBatchedProperty 파라미터가 추가
		// 값이 true라면 -> Instanced 재질 값을 계산할 때 Batch의 색상 속성을 무시한다.
		//
		/// <summary>
		/// Set Main Color (2X)
		/// </summary>
		/// <param name="color2X"></param>
		/// <param name="isOverlapBatchedProperty"></param>
		public void SetMeshColor(Color color2X)
		{
			
			_multiplyColor = color2X;
			
			if(Mathf.Abs(_multiplyColor.r - 0.5f) < 0.004f &&
				Mathf.Abs(_multiplyColor.g - 0.5f) < 0.004f &&
				Mathf.Abs(_multiplyColor.b - 0.5f) < 0.004f &&
				Mathf.Abs(_multiplyColor.a - 1.0f) < 0.004f)
			{
				//기본 값이라면
				_isAnyMeshColorRequest = false;
				_multiplyColor = new Color(0.5f, 0.5f, 0.5f, 1.0f);
			}
			else
			{
				_isAnyMeshColorRequest = true;
			}

			_cal_MeshColor.r = _multiplyColor.r * _parentTransform._meshColor2X.r * 2;
			_cal_MeshColor.g = _multiplyColor.g * _parentTransform._meshColor2X.g * 2;
			_cal_MeshColor.b = _multiplyColor.b * _parentTransform._meshColor2X.b * 2;
			_cal_MeshColor.a = _multiplyColor.a * _parentTransform._meshColor2X.a;//Alpha는 2X가 아니다.

			_material_Instanced.SetColor(_shaderID_MainTex, _cal_MeshColor);

			AutoSelectMaterial(true);//색상 변경 요청시에는 Gray 체크를 한번 더 해야한다.
		}

		public void SetMeshAlpha(float alpha)
		{
			_multiplyColor.a = alpha;
			
			if(Mathf.Abs(_multiplyColor.r - 0.5f) < 0.004f &&
				Mathf.Abs(_multiplyColor.g - 0.5f) < 0.004f &&
				Mathf.Abs(_multiplyColor.b - 0.5f) < 0.004f &&
				Mathf.Abs(_multiplyColor.a - 1.0f) < 0.004f)
			{
				//기본 값이라면
				_isAnyMeshColorRequest = false;
				_multiplyColor = new Color(0.5f, 0.5f, 0.5f, 1.0f);
			}
			else
			{
				_isAnyMeshColorRequest = true;
			}

			_cal_MeshColor.r = _multiplyColor.r * _parentTransform._meshColor2X.r * 2;
			_cal_MeshColor.g = _multiplyColor.g * _parentTransform._meshColor2X.g * 2;
			_cal_MeshColor.b = _multiplyColor.b * _parentTransform._meshColor2X.b * 2;
			_cal_MeshColor.a = _multiplyColor.a * _parentTransform._meshColor2X.a;//Alpha는 2X가 아니다.

			_material_Instanced.SetColor(_shaderID_MainTex, _cal_MeshColor);

			AutoSelectMaterial(true);//색상 변경 요청시에는 Gray 체크를 한번 더 해야한다.
		}

		/// <summary>
		/// Set Main Texture
		/// </summary>
		/// <param name="texture"></param>
		public void SetMeshTexture(Texture2D texture)
		{
			if(_isMaskChild)
			{
				//Mask Child라면 그냥 Instanced Material에 넣는다.
				_material_Instanced.SetTexture(_shaderID_MainTex, texture);
			}
			else
			{
				//그 외에는 Shared Material과 비교한다.
				if(_material_Shared.mainTexture == texture)
				{
					_isAnyTextureRequest = false;
				}
				else
				{
					_isAnyTextureRequest = true;
				}
				_material_Instanced.SetTexture(_shaderID_MainTex, texture);
			}

			//이전 코드
			//if(_sharedMaterial.mainTexture == texture)
			//{
			//	_isAnyTextureRequest = false;

			//	if(!_isUseSharedMaterial)
			//	{
			//		//일단 이미지를 넣어준다.
			//		_instanceMaterial.SetTexture(_shaderID_MainTex, texture);
			//	}
			//}
			//else
			//{
			//	//새로운 텍스쳐를 요청했다.
			//	_isAnyTextureRequest = true;

			//	//Instance Material에 넣어준다.
			//	_instanceMaterial.SetTexture(_shaderID_MainTex, texture);
			//}

			AutoSelectMaterial();
		}

		/// <summary>
		/// Set Color as shader property (not Main Color)
		/// </summary>
		/// <param name="color"></param>
		/// <param name="propertyName"></param>
		public void SetCustomColor(Color color, string propertyName)
		{
			//값에 상관없이 이 함수가 호출되면 True
			_isAnyCustomPropertyRequest = true;
			//_instanceMaterial.SetColor(propertyName, color);//이전
			_material_Instanced.SetColor(propertyName, color);

			AutoSelectMaterial();
		}

		/// <summary>
		/// Set Alpha as shader property (not Main Color)
		/// </summary>
		/// <param name="color"></param>
		/// <param name="propertyName"></param>
		public void SetCustomAlpha(float alpha, string propertyName)
		{
			//값에 상관없이 이 함수가 호출되면 True
			_isAnyCustomPropertyRequest = true;
			//Color color = _instanceMaterial.GetColor(propertyName);
			//color.a = alpha;
			//_instanceMaterial.SetColor(propertyName, color);

			//변경 : 현재 Color -> Alpha 변경 -> Instanced에 전달
			Color color = _material_Cur.GetColor(propertyName);
			color.a = alpha;
			_material_Instanced.SetColor(propertyName, color);

			AutoSelectMaterial();
		}



		/// <summary>
		/// Set Texture as shader property (not Main Texture)
		/// </summary>
		/// <param name="texture"></param>
		/// <param name="propertyName"></param>
		public void SetCustomTexture(Texture2D texture, string propertyName)
		{
			//값에 상관없이 이 함수가 호출되면 True
			_isAnyCustomPropertyRequest = true;
			//_instanceMaterial.SetTexture(propertyName, texture);//이전
			_material_Instanced.SetTexture(propertyName, texture);

			AutoSelectMaterial();
		}

		/// <summary>
		/// Set Float Value as shader property
		/// </summary>
		/// <param name="floatValue"></param>
		/// <param name="propertyName"></param>
		public void SetCustomFloat(float floatValue, string propertyName)
		{
			//값에 상관없이 이 함수가 호출되면 True
			_isAnyCustomPropertyRequest = true;
			//_instanceMaterial.SetFloat(propertyName, floatValue);//이전
			_material_Instanced.SetFloat(propertyName, floatValue);

			AutoSelectMaterial();
		}


		



		/// <summary>
		/// Set Int Value as shader property
		/// </summary>
		/// <param name="intValue"></param>
		/// <param name="propertyName"></param>
		public void SetCustomInt(int intValue, string propertyName)
		{
			//값에 상관없이 이 함수가 호출되면 True
			_isAnyCustomPropertyRequest = true;
			//_instanceMaterial.SetInt(propertyName, intValue);//이전
			_material_Instanced.SetInt(propertyName, intValue);

			AutoSelectMaterial();
		}

		/// <summary>
		/// Set Vector4 Value as shader property
		/// </summary>
		/// <param name="vector4Value"></param>
		/// <param name="propertyName"></param>
		public void SetCustomVector4(Vector4 vector4Value, string propertyName)
		{
			//값에 상관없이 이 함수가 호출되면 True
			_isAnyCustomPropertyRequest = true;
			//_instanceMaterial.SetVector(propertyName, vector4Value);//이전
			_material_Instanced.SetVector(propertyName, vector4Value);

			AutoSelectMaterial();
		}




		// 추가 12.02 : UV Offset과 Size 조절
		/// <summary>
		/// Set UV Offset Value as shader property
		/// </summary>
		/// <param name="propertyName"></param>
		public void SetCustomTextureOffset(Vector2 uvOffset, string propertyName)
		{
			//값에 상관없이 이 함수가 호출되면 True
			_isAnyCustomPropertyRequest = true;
			//_instanceMaterial.SetTextureOffset(propertyName, uvOffset);//이전
			_material_Instanced.SetTextureOffset(propertyName, uvOffset);

			AutoSelectMaterial();
		}

		/// <summary>
		/// Set UV Scale Value as shader property
		/// </summary>
		/// <param name="propertyName"></param>
		public void SetCustomTextureScale(Vector2 uvScale, string propertyName)
		{
			//값에 상관없이 이 함수가 호출되면 True
			_isAnyCustomPropertyRequest = true;
			//_instanceMaterial.SetTextureScale(propertyName, uvScale);//이전
			_material_Instanced.SetTextureScale(propertyName, uvScale);

			AutoSelectMaterial();
		}





		private void AutoSelectMaterial(bool isCheckGrayColor = false)
		{
#if UNITY_EDITOR
			if(!Application.isPlaying)
			{
				return;
			}
#endif
			if(_isMaskChild)
			{
				//Mask Child는 무조건 Instanced를 이용한다.
				if(_materialType != MATERIAL_TYPE.Instanced)
				{
					_materialType = MATERIAL_TYPE.Instanced;
					_material_Cur = _material_Instanced;
					_meshRenderer.sharedMaterial = _material_Cur;
				}
				
				return;
			}

			//Batch된 재질이 작동하고 있는가
			bool isBatched = false;
			if(_materialUnit_Batched != null && _materialUnit_Batched.IsAnyChanged)
			{
				isBatched = true;
			}

			if(_isForceBatch2Shared)
			{
				//강제로 Shared로 전환해야하는 옵션이 켜질 수 있다.
				isBatched = false;
			}

			bool isColorChanged = _isAnyMeshColorRequest || _parentTransform._isAnyColorCalculated || !_isDefaultColorGray;
			if(isCheckGrayColor 
				&& isColorChanged
				&& !_isAnyTextureRequest
				&& !_isAnyCustomPropertyRequest
				)
			{
				//만약, 색상 변경 이벤트가 있었는데, (게다가 다른 이벤트는 없었다면)
				//Gray 체크 요청이 같이 왔다면 Instanced가 아니라 Shared로 바꿀 수 있을 것이다.
				if (isBatched)
				{
					//만약, Batch 재질이 작동하고 있고, 계산된 MeshColor가 Batch의 색상과 유사하다면, 이건 Batch 쪽으로 전환되어야 한다.
					//(색상에 한해서)

					Color batchedColor = _material_Batched.color;

					bool isBatchedColor = Mathf.Abs(_cal_MeshColor.r - batchedColor.r) < 0.004f &&
											Mathf.Abs(_cal_MeshColor.g - batchedColor.g) < 0.004f &&
											Mathf.Abs(_cal_MeshColor.b - batchedColor.b) < 0.004f &&
											Mathf.Abs(_cal_MeshColor.a - batchedColor.a) < 0.004f;

					if (isBatchedColor)
					{
						//Batch 재질과 같은 색상이다.
						isColorChanged = false;
					}
				}
				else
				{
					//일반적인 경우엔 Gray Color와 같다면 Instanced가 아닌 Shared로 전환한다.
					bool isGrayColor = Mathf.Abs(_cal_MeshColor.r - 0.5f) < 0.004f &&
								Mathf.Abs(_cal_MeshColor.g - 0.5f) < 0.004f &&
								Mathf.Abs(_cal_MeshColor.b - 0.5f) < 0.004f &&
								Mathf.Abs(_cal_MeshColor.a - 1.0f) < 0.004f;

					if (isGrayColor)
					{
						//Gray 색상이라면 색상 이벤트를 무시해도 된다.
						isColorChanged = false;
					}
				}
			}
			

			if (_isAnyTextureRequest
				|| _isAnyCustomPropertyRequest
				|| isColorChanged)
			{

				//이전 코드
				//if(_isUseSharedMaterial)
				//{
				//	//Shared -> Instance
				//	_isUseSharedMaterial = false;
				//	_material = _instanceMaterial;

				//	_meshRenderer.sharedMaterial = _material;
				//}
				//Instance Material을 선택해야한다.
				if (_materialType != MATERIAL_TYPE.Instanced)
				{
					_materialType = MATERIAL_TYPE.Instanced;
					_material_Cur = _material_Instanced;
					_meshRenderer.sharedMaterial = _material_Cur;

					_isForceBatch2Shared = false;
				}
			}
			else
			{
				//이전 코드
				//if(!_isUseSharedMaterial)
				//{
				//	//Instance -> Shared
				//	_isUseSharedMaterial = true;
				//	_material = _sharedMaterial;

				//	_meshRenderer.sharedMaterial = _material;
				//}

				//Shared Material을 선택해야한다.
				//기본적으론 Shared를 선택해야한다.
				//Batched Material의 "일괄 적용 요청"이 있었다면, Shared와 Batch 중에서 결정해야한다.
				

				if (isBatched )
				{
					if (_materialType != MATERIAL_TYPE.Batched)
					{
						//-> Batched
						_materialType = MATERIAL_TYPE.Batched;
						_material_Cur = _material_Batched;
						_meshRenderer.sharedMaterial = _material_Cur;
					}
				}
				else
				{
					if (_materialType != MATERIAL_TYPE.Shared)
					{
						//-> Shared
						_materialType = MATERIAL_TYPE.Shared;
						_material_Cur = _material_Shared;
						_meshRenderer.sharedMaterial = _material_Cur;
					}
				}
			}
		}


		//Material Property 값들을 초기화한다.
		//이 함수를 호출하면 MaskChild를 제외하면 Batch를 위해 SharedMaterial로 변경된다.
		/// <summary>
		/// Return the material value to its initial state. Batch rendering is enabled.
		/// </summary>
		public void ResetMaterialToBatch()
		{
			Debug.LogError("ResetMaterialToBatch");
			if(_isMaskChild)
			{
				return;
			}

			//Debug.Log("ResetMaterialToBatch : " + this.name);
			if(_material_Shared != null)
			{
				//Shared로 변경
				_material_Instanced.CopyPropertiesFromMaterial(_material_Shared);
				_materialType = MATERIAL_TYPE.Shared;
				_material_Cur = _material_Shared;
				_meshRenderer.sharedMaterial = _material_Cur;

				//Debug.Log(">> Shared");
			}

			//이전 코드
			//if(_isUseSharedMaterial)
			//{
			//	return;
			//}
			//_isUseSharedMaterial = true;
			//_material = _sharedMaterial;

			////일단 InstanceMat도 복사를 해서 리셋을 해준다.
			//_instanceMaterial.CopyPropertiesFromMaterial(_sharedMaterial);

			//_meshRenderer.sharedMaterial = _material;

			_isAnyMeshColorRequest = false;
			_isAnyTextureRequest = false;
			_isAnyCustomPropertyRequest = false;

			//중요 : Batched에서 강제로 Shared로 전환하게 만들어야 한다.
			_isForceBatch2Shared = true;

			//색상 값도 초기화
			_multiplyColor = new Color(0.5f, 0.5f, 0.5f, 1.0f);

			AutoSelectMaterial(true);

			
		}

		// Batch 관련 이벤트
		//-------------------------------------------------------------------------
		public void SyncMaterialPropertyByBatch_Texture(Texture2D texture)
		{
			_material_Instanced.SetTexture(_shaderID_MainTex, texture);

			//동기화 되었으므로, Instanced 자체의 옵션은 해제 - 텍스쳐
			_isAnyTextureRequest = false;

			_isForceBatch2Shared = false;//Batch를 막는 플래그를 끄자

			AutoSelectMaterial();
		}

		public void SyncMaterialPropertyByBatch_Color(Color color2X)
		{
			//일단 Batch의 색상 값을 가져다 쓴다.
			//이 상태에서 다시 Parent Opt Transform의 색상과 계산을 해서 _cal_MeshColor를 계산해야한다.
			//Batch와 동기화한 것이므로 isAnyMeshColorRequest 여부는 확인하지 않는다.
			_multiplyColor = color2X;

			_cal_MeshColor.r = _multiplyColor.r * _parentTransform._meshColor2X.r * 2;
			_cal_MeshColor.g = _multiplyColor.g * _parentTransform._meshColor2X.g * 2;
			_cal_MeshColor.b = _multiplyColor.b * _parentTransform._meshColor2X.b * 2;
			_cal_MeshColor.a = _multiplyColor.a * _parentTransform._meshColor2X.a;//Alpha는 2X가 아니다.

			_material_Instanced.SetColor(_shaderID_Color, _multiplyColor);

			//동기화 되었으므로, Instanced 자체의 옵션은 해제 - 색상
			_isAnyMeshColorRequest = false;

			_isForceBatch2Shared = false;//Batch를 막는 플래그를 끄자

			AutoSelectMaterial(true);//<<색상 계산을 해야한다.
		}

		public void SyncMaterialPropertyByBatch_CustomTexture(Texture2D texture, string propertyName)
		{
			_material_Instanced.SetTexture(propertyName, texture);
			_isForceBatch2Shared = false;//Batch를 막는 플래그를 끄자

			AutoSelectMaterial();
		}

		public void SyncMaterialPropertyByBatch_CustomColor(Color color, string propertyName)
		{
			_material_Instanced.SetColor(propertyName, color);
			_isForceBatch2Shared = false;//Batch를 막는 플래그를 끄자

			AutoSelectMaterial();
		}

		public void SyncMaterialPropertyByBatch_CustomFloat(float floatValue, string propertyName)
		{
			_material_Instanced.SetFloat(propertyName, floatValue);
			_isForceBatch2Shared = false;//Batch를 막는 플래그를 끄자

			AutoSelectMaterial();
		}

		public void SyncMaterialPropertyByBatch_CustomInt(int intValue, string propertyName)
		{
			_material_Instanced.SetInt(propertyName, intValue);
			_isForceBatch2Shared = false;//Batch를 막는 플래그를 끄자

			AutoSelectMaterial();
		}

		public void SyncMaterialPropertyByBatch_CustomVector4(Vector4 vecValue, string propertyName)
		{
			_material_Instanced.SetVector(propertyName, vecValue);
			_isForceBatch2Shared = false;//Batch를 막는 플래그를 끄자

			AutoSelectMaterial();
		}
		
		public void SyncMaterialPropertyByBatch_Reset(Material syncMaterial)
		{
			_material_Instanced.CopyPropertiesFromMaterial(syncMaterial);


			//색상은 별도로 초기화
			_multiplyColor = new Color(0.5f, 0.5f, 0.5f, 1.0f);
			_cal_MeshColor.r = _multiplyColor.r * _parentTransform._meshColor2X.r * 2;
			_cal_MeshColor.g = _multiplyColor.g * _parentTransform._meshColor2X.g * 2;
			_cal_MeshColor.b = _multiplyColor.b * _parentTransform._meshColor2X.b * 2;
			_cal_MeshColor.a = _multiplyColor.a * _parentTransform._meshColor2X.a;//Alpha는 2X가 아니다.


			//동기화 되었으므로, Instanced 자체의 옵션은 해제 - 전체
			_isAnyMeshColorRequest = false;
			_isAnyTextureRequest = false;
			_isAnyCustomPropertyRequest = false;

			_isForceBatch2Shared = false;//Batch를 막는 플래그를 끄자

			AutoSelectMaterial(true);
		}




		//-------------------------------------------------------------------------
		// 추가 12.8 : Extra Option에 의해서 Texture가 바뀌었을 경우
		public Texture2D GetCurrentProcessedTexture()
		{
			//이전
			//if(_isUseSharedMaterial)
			//{
			//	return _sharedMaterial.mainTexture as Texture2D;
			//}
			//else
			//{
			//	return _instanceMaterial.mainTexture as Texture2D;
			//}

			//변경
			return _material_Cur.mainTexture as Texture2D;
		}

		public void SetExtraChangedTexture(Texture2D texture)
		{
			if(texture == null)
			{
				if(_isMaskChild)
				{
					//Mask Child에서는 Extra Changed가 발생할 때 Null Texture를 적용할 수 없다.
					return;
				}
				//texture = _sharedMaterial.mainTexture as Texture2D;
				texture = _material_Shared.mainTexture as Texture2D;
			}

			if (_isMaskChild)
			{
				//Mask Child라면 무조건 Instanced 타입이다.
				_materialType = MATERIAL_TYPE.Instanced;
				_material_Cur = _material_Instanced;
				_material_Cur.SetTexture(_shaderID_MainTex, texture);
				_isAnyTextureRequest = true;
			}
			else
			{
				//Mask Child가 아니라면, Shared Material과 비교하여 다시 Shared로 바꿀지 결정한다.
				if(_material_Shared.mainTexture == texture
					|| _material_Batched.mainTexture == texture)
				{
					//Shared나 Batched로 돌아갈 수 있다. (경우에 따라서..)
					_isAnyTextureRequest = false;
				}
				else
				{
					//Shared -> Instanced (무조건)
					_isAnyTextureRequest = true;
				}

				_material_Instanced.SetTexture(_shaderID_MainTex, texture);

				//if (_sharedMaterial.mainTexture == texture)
				//{
				//	_isAnyTextureRequest = false;

				//	if (!_isUseSharedMaterial)
				//	{
				//		//일단 이미지를 넣어준다.
				//		_instanceMaterial.SetTexture(_shaderID_MainTex, texture);
				//	}
				//}
				//else
				//{
				//	//새로운 텍스쳐를 요청했다.
				//	_isAnyTextureRequest = true;

				//	//Instance Material에 넣어준다.
				//	_instanceMaterial.SetTexture(_shaderID_MainTex, texture);
				//}
			}
			

			AutoSelectMaterial();
		}


		// Get / Set
		//------------------------------------------------
		/// <summary>
		/// Calculated Mesh Color (2X)
		/// </summary>
		public Color MeshColor
		{
			get
			{
				//return _multiplyColor * 2.0f * _parentTransform._meshColor2X;
				return _cal_MeshColor;
			}
		}


		public MATERIAL_TYPE GetMaterialTypeForDebug()
		{
			return _materialType;
		}

		//---------------------------------------------------------
		// Mesh Renderer 의 Sorting Order 제어
		//---------------------------------------------------------
		public void SetSortingLayer(string sortingLayerName, int sortingLayerID)
		{
			_meshRenderer.sortingLayerName = sortingLayerName;
			_meshRenderer.sortingLayerID = sortingLayerID;
		}

		public string GetSortingLayerName()
		{
			return _meshRenderer.sortingLayerName;
		}

		public int GetSortingLayerID()
		{
			return _meshRenderer.sortingLayerID;
		}

		public void SetSortingOrder(int sortingOrder)
		{
			_meshRenderer.sortingOrder = sortingOrder;
		}

		public int GetSortingOrder()
		{
			return _meshRenderer.sortingOrder;
		}
	}
}