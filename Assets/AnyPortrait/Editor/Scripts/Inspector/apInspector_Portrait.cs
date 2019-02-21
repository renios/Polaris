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
using UnityEditor;
//using UnityEngine.Profiling;

using AnyPortrait;

namespace AnyPortrait
{

	[CustomEditor(typeof(apPortrait))]
	public class apInspector_Portrait : Editor
	{
		private apPortrait _targetPortrait = null;
		private apControlParam.CATEGORY _curControlCategory = apControlParam.CATEGORY.Etc;
		private bool _showBaseInspector = false;
		private List<apControlParam> _controlParams = null;


		private bool _isFold_RootPortraits = false;
		private bool _isFold_AnimationClips = false;
		private bool _isFold_ConrolParameters = false;
		



		void OnEnable()
		{
			_targetPortrait = null;

			_isFold_RootPortraits = true;
			_isFold_AnimationClips = true;
			_isFold_ConrolParameters = true;
		}

		public override void OnInspectorGUI()
		{
			//return;

			//base.OnInspectorGUI();
			apPortrait targetPortrait = target as apPortrait;

			if (targetPortrait != _targetPortrait)
			{
				_targetPortrait = targetPortrait;
				Init();
			}
			if (_targetPortrait == null)
			{
				//Profiler.EndSample();
				return;
			}

			//Profiler.BeginSample("anyPortrait Inspector GUI");


			//return;
			if (apEditor.IsOpen())
			{
				//에디터가 작동중에는 안보이도록 하자
				EditorGUILayout.LabelField("Editor is opened");

				//Profiler.EndSample();

				return;
			}

			try
			{
				bool request_OpenEditor = false;
				bool request_QuickBake = false;
				bool request_RefreshMeshes = false;
				bool prevImportant = _targetPortrait._isImportant;
				MonoBehaviour prevAnimEventListener = _targetPortrait._optAnimEventListener;
				int prevSortingLayerID = _targetPortrait._sortingLayerID;
				int prevSortingOrder = _targetPortrait._sortingOrder;

				if (!EditorApplication.isPlaying)
				{
					
					if (!_targetPortrait._isOptimizedPortrait)
					{
						GUILayout.Space(10);
						if (GUILayout.Button("Open Editor and Select", GUILayout.Height(30)))
						{
							request_OpenEditor = true;
							
						}
						if (GUILayout.Button("Quick Bake", GUILayout.Height(25)))
						{
							request_QuickBake = true;
						}
					}
					else
					{
						GUILayout.Space(10);
						if (GUILayout.Button("Open Editor (Not Selectable)", GUILayout.Height(30)))
						{
							//열기만 하고 선택은 못함
							request_OpenEditor = true;
						}
					}
					//추가 12.18 : Mesh를 리프레시 하자
					if (GUILayout.Button("Refresh Meshes", GUILayout.Height(25)))
					{
						request_RefreshMeshes = true;
					}
				}

				GUILayout.Space(10);

				_targetPortrait._isImportant = EditorGUILayout.Toggle("Is Important", _targetPortrait._isImportant);
				_targetPortrait._optAnimEventListener = (MonoBehaviour)EditorGUILayout.ObjectField("Event Listener", _targetPortrait._optAnimEventListener, typeof(MonoBehaviour), true);


				GUILayout.Space(5);
				//추가3.22
				//Sorting Layer
				string[] sortingLayerName = new string[SortingLayer.layers.Length];
				int layerIndex = -1;
				for (int i = 0; i < SortingLayer.layers.Length; i++)
				{
					sortingLayerName[i] = SortingLayer.layers[i].name;
					if (SortingLayer.layers[i].id == _targetPortrait._sortingLayerID)
					{
						layerIndex = i;
					}
				}
				int nextLayerIndex = EditorGUILayout.Popup("Sorting Layer", layerIndex, sortingLayerName);
				int nextLayerOrder = EditorGUILayout.IntField("Sorting Order", _targetPortrait._sortingOrder);

				if(nextLayerIndex != layerIndex)
				{
					//Sorting Layer를 바꾸자
					if(nextLayerIndex >= 0 && nextLayerIndex < SortingLayer.layers.Length)
					{
						string nextLayerName = SortingLayer.layers[nextLayerIndex].name;
						_targetPortrait.SetSortingLayer(nextLayerName);
					}
				}
				if(nextLayerOrder != _targetPortrait._sortingOrder)
				{
					_targetPortrait.SetSortingOrder(nextLayerOrder);
				}


				if(prevImportant != _targetPortrait._isImportant ||
					prevAnimEventListener != _targetPortrait._optAnimEventListener ||
					prevSortingLayerID != _targetPortrait._sortingLayerID ||
					prevSortingOrder != _targetPortrait._sortingOrder)
				{
					apEditorUtil.SetEditorDirty();
				}


				GUILayout.Space(5);

				//빌보드
				apPortrait.BILLBOARD_TYPE nextBillboard = (apPortrait.BILLBOARD_TYPE)EditorGUILayout.EnumPopup("Billboard Type", _targetPortrait._billboardType);
				if(nextBillboard != _targetPortrait._billboardType)
				{
					_targetPortrait._billboardType = nextBillboard;
					apEditorUtil.SetEditorDirty();
				}

				GUILayout.Space(5);
				
				_isFold_RootPortraits = EditorGUILayout.Foldout(_isFold_RootPortraits, "Root Portraits");
				if(_isFold_RootPortraits)
				{
					string strRootPortrait = "";
					if(_targetPortrait._optRootUnitList.Count == 0)
					{
						strRootPortrait = "No Baked Portrait";
					}
					else if(_targetPortrait._optRootUnitList.Count == 1)
					{
						strRootPortrait = "1 Baked Portrait";
					}
					else
					{
						strRootPortrait = _targetPortrait._optRootUnitList.Count + " Baked Portraits";
					}
					EditorGUILayout.LabelField(strRootPortrait);
					GUILayout.Space(5);
					for (int i = 0; i < _targetPortrait._optRootUnitList.Count; i++)
					{
						apOptRootUnit rootUnit = _targetPortrait._optRootUnitList[i];
						EditorGUILayout.ObjectField("[" + i + "]", rootUnit, typeof(apOptRootUnit), true);
					}

					GUILayout.Space(20);
				}


				
				

				_isFold_AnimationClips = EditorGUILayout.Foldout(_isFold_AnimationClips, "Animation Settings");
				if(_isFold_AnimationClips)
				{
					EditorGUILayout.LabelField("Animation Clips");
					string strAnimClips = "";
					if(_targetPortrait._animClips.Count == 0)
					{
						strAnimClips = "No Animation Clip";
					}
					else if(_targetPortrait._animClips.Count == 1)
					{
						strAnimClips = "1 Animation Clip";
					}
					else
					{
						strAnimClips = _targetPortrait._animClips.Count + " Animation Clips";
					}
					EditorGUILayout.LabelField(strAnimClips);
					GUILayout.Space(5);
					
					
					for (int i = 0; i < _targetPortrait._animClips.Count; i++)
					{
						EditorGUILayout.BeginHorizontal();
						GUILayout.Space(5);
						apAnimClip animClip = _targetPortrait._animClips[i];
						if(animClip._uniqueID == _targetPortrait._autoPlayAnimClipID)
						{
							EditorGUILayout.LabelField("[" + i + "] (Auto)", GUILayout.Width(80));
						}
						else
						{
							EditorGUILayout.LabelField("[" + i + "]", GUILayout.Width(80));
						}
						EditorGUILayout.TextField(animClip._name);
						try
						{
							AnimationClip nextAnimationClip = EditorGUILayout.ObjectField(animClip._animationClipForMecanim, typeof(AnimationClip), false) as AnimationClip;
							if(nextAnimationClip != animClip._animationClipForMecanim)
							{
								UnityEditor.SceneManagement.EditorSceneManager.MarkAllScenesDirty();
								Undo.IncrementCurrentGroup();
								Undo.RegisterCompleteObjectUndo(_targetPortrait, "Animation Changed");
								
								animClip._animationClipForMecanim = nextAnimationClip;
							}
						}
						catch (Exception)
						{ }
						
						EditorGUILayout.EndHorizontal();
					}
					GUILayout.Space(10);
					AnimationClip nextEmptyAnimClip = EditorGUILayout.ObjectField("Empty Animation Clip", _targetPortrait._emptyAnimClipForMecanim, typeof(AnimationClip), false) as AnimationClip;
					if (nextEmptyAnimClip != _targetPortrait._emptyAnimClipForMecanim)
					{
						UnityEditor.SceneManagement.EditorSceneManager.MarkAllScenesDirty();
						Undo.IncrementCurrentGroup();
						Undo.RegisterCompleteObjectUndo(_targetPortrait, "Animation Changed");

						_targetPortrait._emptyAnimClipForMecanim = nextEmptyAnimClip;
					}

					GUILayout.Space(10);
					EditorGUILayout.LabelField("Mecanim Settings");
					bool isNextUsingMecanim = EditorGUILayout.Toggle("Use Mecanim", _targetPortrait._isUsingMecanim);
					if (_targetPortrait._isUsingMecanim != isNextUsingMecanim)
					{
						UnityEditor.SceneManagement.EditorSceneManager.MarkAllScenesDirty();
						Undo.IncrementCurrentGroup();
						Undo.RegisterCompleteObjectUndo(_targetPortrait, "Mecanim Setting Changed");

						_targetPortrait._isUsingMecanim = isNextUsingMecanim;
					}


					if(_targetPortrait._isUsingMecanim)
					{
						GUILayout.Space(10);
						try
						{
							Animator nextAnimator = EditorGUILayout.ObjectField("Animator", _targetPortrait._animator, typeof(Animator), true) as Animator;
							if (nextAnimator != _targetPortrait._animator)
							{
								//하위에 있는 Component일 때에만 변동 가능
								if (nextAnimator == null)
								{
									_targetPortrait._animator = null;
								}
								else
								{
									if (nextAnimator == _targetPortrait.GetComponent<Animator>())
									{
										_targetPortrait._animator = nextAnimator;
									}
									else
									{
										EditorUtility.DisplayDialog("Invalid Animator", "Invalid Animator. Only the Animator, which is its own component, is valid.", "Okay");

									}
								}

							}
						}
						catch(Exception)
						{

						}
						if (_targetPortrait._animator == null)
						{
							//1. Animator가 없다면
							// > 생성하기
							// > 생성되어 있다면 다시 링크
							GUIStyle guiStyle_WarningText = new GUIStyle(GUI.skin.label);
							guiStyle_WarningText.normal.textColor = Color.red;
							EditorGUILayout.LabelField("Warning : No Animator!", guiStyle_WarningText);
							GUILayout.Space(5);

							if(GUILayout.Button("Add / Check Animator", GUILayout.Height(25)))
							{
								UnityEditor.SceneManagement.EditorSceneManager.MarkAllScenesDirty();
								Undo.IncrementCurrentGroup();
								Undo.RegisterCompleteObjectUndo(_targetPortrait, "Mecanim Setting Changed");

								Animator animator = _targetPortrait.gameObject.GetComponent<Animator>();
								if(animator == null)
								{
									animator = _targetPortrait.gameObject.AddComponent<Animator>();
								}
								_targetPortrait._animator = animator;
							}
						}
						else
						{
							//2. Animator가 있다면
							if (GUILayout.Button("Refresh Layers", GUILayout.Height(25)))
							{
								UnityEditor.SceneManagement.EditorSceneManager.MarkAllScenesDirty();
								Undo.IncrementCurrentGroup();
								Undo.RegisterCompleteObjectUndo(_targetPortrait, "Mecanim Setting Changed");

								//Animator의 Controller가 있는지 체크해야한다.
								
								if(_targetPortrait._animator.runtimeAnimatorController == null)
								{
									//AnimatorController가 없다면 Layer는 초기화
									_targetPortrait._animatorLayerBakedData.Clear();
								}
								else
								{
									//AnimatorController가 있다면 레이어에 맞게 설정
									_targetPortrait._animatorLayerBakedData.Clear();
									UnityEditor.Animations.AnimatorController animatorController = _targetPortrait._animator.runtimeAnimatorController as UnityEditor.Animations.AnimatorController;

									if(animatorController != null && animatorController.layers.Length > 0)
									{
										for (int iLayer = 0; iLayer < animatorController.layers.Length; iLayer++)
										{
											apAnimMecanimData_Layer newLayerData = new apAnimMecanimData_Layer();
											newLayerData._layerIndex = iLayer;
											newLayerData._layerName = animatorController.layers[iLayer].name;
											newLayerData._blendType = apAnimMecanimData_Layer.MecanimLayerBlendType.Unknown;
											switch (animatorController.layers[iLayer].blendingMode)
											{
												case UnityEditor.Animations.AnimatorLayerBlendingMode.Override:
													newLayerData._blendType = apAnimMecanimData_Layer.MecanimLayerBlendType.Override;
													break;

												case UnityEditor.Animations.AnimatorLayerBlendingMode.Additive:
													newLayerData._blendType = apAnimMecanimData_Layer.MecanimLayerBlendType.Additive;
													break;
											}

											_targetPortrait._animatorLayerBakedData.Add(newLayerData);
										}
									}
								}
							}
							GUILayout.Space(5);
							EditorGUILayout.LabelField("Animator Controller Layers");
							for (int i = 0; i < _targetPortrait._animatorLayerBakedData.Count; i++)
							{
								apAnimMecanimData_Layer layer = _targetPortrait._animatorLayerBakedData[i];
								EditorGUILayout.BeginHorizontal();
								GUILayout.Space(5);
								EditorGUILayout.LabelField("[" + layer._layerIndex + "]", GUILayout.Width(50));
								EditorGUILayout.TextField(layer._layerName);
								apAnimMecanimData_Layer.MecanimLayerBlendType nextBlendType = (apAnimMecanimData_Layer.MecanimLayerBlendType)EditorGUILayout.EnumPopup(layer._blendType);
								EditorGUILayout.EndHorizontal();

								if (nextBlendType != layer._blendType)
								{
									UnityEditor.SceneManagement.EditorSceneManager.MarkAllScenesDirty();
									Undo.IncrementCurrentGroup();
									Undo.RegisterCompleteObjectUndo(_targetPortrait, "Mecanim Setting Changed");

									_targetPortrait._animatorLayerBakedData[i]._blendType = nextBlendType;
								}
							}
						}
						
					}


					GUILayout.Space(20);

				}

				
				bool isChanged = false;

				_isFold_ConrolParameters = EditorGUILayout.Foldout(_isFold_ConrolParameters, "Control Parameters");
				if (_isFold_ConrolParameters)
				{
#if UNITY_2017_3_OR_NEWER
					_curControlCategory = (apControlParam.CATEGORY)EditorGUILayout.EnumFlagsField(new GUIContent("Category"), _curControlCategory);
#else				
					_curControlCategory = (apControlParam.CATEGORY)EditorGUILayout.EnumMaskPopup(new GUIContent("Category"), _curControlCategory);
#endif

					EditorGUILayout.Space();
					//1. 컨르롤러를 제어할 수 있도록 하자
					
					if (_controlParams != null)
					{
						for (int i = 0; i < _controlParams.Count; i++)
						{
							if ((int)(_controlParams[i]._category & _curControlCategory) != 0)
							{
								if (GUI_ControlParam(_controlParams[i]))
								{
									isChanged = true;
								}
							}
						}
					}

					GUILayout.Space(20);
				}
				

				GUILayout.Space(10);

				//2. 토글 버튼을 두어서 기본 Inspector 출력 여부를 결정하자.
				string strBaseButton = "Show All Properties";
				if (_showBaseInspector)
				{
					strBaseButton = "Hide Properties";
				}

				if (GUILayout.Button(strBaseButton, GUILayout.Height(20)))
				{
					_showBaseInspector = !_showBaseInspector;
				}

				if (_showBaseInspector)
				{
					base.OnInspectorGUI();
				}


				if (!Application.isPlaying && isChanged)
				{
					//플레이 중이라면 자동으로 업데이트 될 것이다.
					_targetPortrait.UpdateForce();
				}

				if (_targetPortrait != null)
				{	
					if (request_OpenEditor)
					{
						if(_targetPortrait._isOptimizedPortrait)
						{
							RequestDelayedOpenEditor(_targetPortrait, REQUEST_TYPE.Open);
						}
						else
						{
							RequestDelayedOpenEditor(_targetPortrait, REQUEST_TYPE.OpenAndSet);
						}
						//apEditor anyPortraitEditor = apEditor.ShowWindow();
						//if (anyPortraitEditor != null && !_targetPortrait._isOptimizedPortrait)
						//{
						//	anyPortraitEditor.SetPortraitByInspector(_targetPortrait, false);
						//}
					}
					else if (request_QuickBake)
					{
						RequestDelayedOpenEditor(_targetPortrait, REQUEST_TYPE.QuickBake);
						//apEditor anyPortraitEditor = apEditor.ShowWindow();
						//if (anyPortraitEditor != null)
						//{
						//	anyPortraitEditor.SetPortraitByInspector(_targetPortrait, true);

						//	Selection.activeObject = _targetPortrait.gameObject;
						//}
					}
					else if(request_RefreshMeshes)
					{
						_targetPortrait.OnMeshResetInEditor();
					}
				}
			}
			catch (Exception ex)
			{
				Debug.LogError("apInspector_Portrait Exception : " + ex);
			}

			//Profiler.EndSample();
		}



		private void Init()
		{
			_curControlCategory = apControlParam.CATEGORY.Head |
									apControlParam.CATEGORY.Body |
									apControlParam.CATEGORY.Face |
									apControlParam.CATEGORY.Hair |
									apControlParam.CATEGORY.Equipment |
									apControlParam.CATEGORY.Force |
									apControlParam.CATEGORY.Etc;

			_showBaseInspector = false;

			//_isFold_BasicSettings = true;
			_isFold_RootPortraits = true;
			//_isFold_AnimationSettings = true;
			_isFold_AnimationClips = true;
			_isFold_ConrolParameters = true;

			_controlParams = null;
			if (_targetPortrait._controller != null)
			{
				_controlParams = _targetPortrait._controller._controlParams;
			}


			_requestPortrait = null;
			_requestType = REQUEST_TYPE.None;
			_coroutine = null;

			EditorApplication.update -= ExecuteCoroutine;
		}

		private bool GUI_ControlParam(apControlParam controlParam)
		{
			if (controlParam == null)
			{ return false; }

			bool isChanged = false;

			EditorGUILayout.LabelField(controlParam._keyName);

			switch (controlParam._valueType)
			{
				//case apControlParam.TYPE.Bool:
				//	{
				//		bool bPrev = controlParam._bool_Cur;
				//		controlParam._bool_Cur = EditorGUILayout.Toggle(controlParam._bool_Cur);
				//		if(bPrev != controlParam._bool_Cur)
				//		{
				//			isChanged = true;
				//		}
				//	}
				//	break;

				case apControlParam.TYPE.Int:
					{
						int iPrev = controlParam._int_Cur;
						controlParam._int_Cur = EditorGUILayout.IntSlider(controlParam._int_Cur, controlParam._int_Min, controlParam._int_Max);

						if (iPrev != controlParam._int_Cur)
						{
							isChanged = true;
						}
					}
					break;

				case apControlParam.TYPE.Float:
					{
						float fPrev = controlParam._float_Cur;
						controlParam._float_Cur = EditorGUILayout.Slider(controlParam._float_Cur, controlParam._float_Min, controlParam._float_Max);

						if (Mathf.Abs(fPrev - controlParam._float_Cur) > 0.0001f)
						{
							isChanged = true;
						}
					}
					break;

				case apControlParam.TYPE.Vector2:
					{
						Vector2 v2Prev = controlParam._vec2_Cur;
						controlParam._vec2_Cur.x = EditorGUILayout.Slider(controlParam._vec2_Cur.x, controlParam._vec2_Min.x, controlParam._vec2_Max.x);
						controlParam._vec2_Cur.y = EditorGUILayout.Slider(controlParam._vec2_Cur.y, controlParam._vec2_Min.y, controlParam._vec2_Max.y);

						if (Mathf.Abs(v2Prev.x - controlParam._vec2_Cur.x) > 0.0001f ||
							Mathf.Abs(v2Prev.y - controlParam._vec2_Cur.y) > 0.0001f)
						{
							isChanged = true;
						}
					}
					break;

			}

			GUILayout.Space(5);

			return isChanged;
		}


		private apPortrait _requestPortrait = null;
		private enum REQUEST_TYPE
		{
			None,
			Open,
			OpenAndSet,
			QuickBake
		}
		private REQUEST_TYPE _requestType = REQUEST_TYPE.None;
		private IEnumerator _coroutine = null;


		private void RequestDelayedOpenEditor(apPortrait portrait, REQUEST_TYPE requestType)
		{
			if(_coroutine != null)
			{
				return;
			}

			_requestPortrait = portrait;
			_requestType = requestType;
			_coroutine = Crt_RequestEditor();

			EditorApplication.update -= ExecuteCoroutine;
			EditorApplication.update += ExecuteCoroutine;
		}

		private void ExecuteCoroutine()
		{
			if(_coroutine == null)
			{
				_requestType = REQUEST_TYPE.None;
				_requestPortrait = null;

				//Debug.Log("ExecuteCoroutine => End");
				EditorApplication.update -= ExecuteCoroutine;
				return;
			}

			//Debug.Log("Update Coroutine");
			bool isResult = _coroutine.MoveNext();
			
			if(!isResult)
			{
				_coroutine = null;
				_requestType = REQUEST_TYPE.None;
				_requestPortrait = null;
				//Debug.Log("ExecuteCoroutine => End");
				EditorApplication.update -= ExecuteCoroutine;
				return;
			}
		}
		private IEnumerator Crt_RequestEditor()
		{
			yield return new WaitForEndOfFrame();
			Selection.activeObject = null;

			yield return new WaitForEndOfFrame();
			if (_requestPortrait != null)
			{
				try
				{
					apEditor anyPortraitEditor = apEditor.ShowWindow();
					if (_requestType == REQUEST_TYPE.OpenAndSet)
					{
						anyPortraitEditor.SetPortraitByInspector(_requestPortrait, false);
					}
					else if (_requestType == REQUEST_TYPE.QuickBake)
					{
						anyPortraitEditor.SetPortraitByInspector(_requestPortrait, true);
						Selection.activeObject = _requestPortrait.gameObject;
					}
				}
				catch (Exception ex)
				{
					Debug.LogError("Open Editor Error : " + ex);
				}
			}
			_requestType = REQUEST_TYPE.None;
			_requestPortrait = null;
		}
	}

}