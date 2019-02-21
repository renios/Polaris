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
using UnityEditor;
using System.Collections;
using System;
using System.Collections.Generic;

using AnyPortrait;

namespace AnyPortrait
{

	public class apDialog_Bake : EditorWindow
	{
		// Members
		//------------------------------------------------------------------
		private static apDialog_Bake s_window = null;

		private apEditor _editor = null;
		private apPortrait _targetPortrait = null;
		//private object _loadKey = null;

		private string[] _colorSpaceNames = new string[] { "Gamma", "Linear" };
#if UNITY_2018_2_OR_NEWER
		private string[] _renderPipelineNames = new string[] { "Default", "Lightweight Render Pipeline" };
#endif

		private string[] _sortingLayerNames = null;
		private int[] _sortingLayerIDs = null;

		private string[] _billboardTypeNames = new string[] { "None", "Billboard", "Billboard with fixed Up Vector" };

		//추가 : 탭으로 분류하자
		//private Vector2 _scroll_Bake = Vector2.zero;
		private Vector2 _scroll_Options = Vector2.zero;

		private enum TAB
		{
			Bake,
			Options
		}
		private TAB _tab = TAB.Bake;

		// Show Window
		//------------------------------------------------------------------
		public static object ShowDialog(apEditor editor, apPortrait portrait)
		{
			CloseDialog();

			if (editor == null || editor._portrait == null || editor._portrait._controller == null)
			{
				return null;
			}

			EditorWindow curWindow = EditorWindow.GetWindow(typeof(apDialog_Bake), true, "Bake", true);
			apDialog_Bake curTool = curWindow as apDialog_Bake;

			object loadKey = new object();
			if (curTool != null && curTool != s_window)
			{
				int width = 350;
				int height = 380;
				s_window = curTool;
				s_window.position = new Rect((editor.position.xMin + editor.position.xMax) / 2 - (width / 2),
												(editor.position.yMin + editor.position.yMax) / 2 - (height / 2),
												width, height);

				s_window.Init(editor, portrait, loadKey);

				return loadKey;
			}
			else
			{
				return null;
			}
		}

		public static void CloseDialog()
		{
			if (s_window != null)
			{
				try
				{
					s_window.Close();
				}
				catch (Exception ex)
				{
					Debug.LogError("Close Exception : " + ex);
				}
				s_window = null;
			}
		}

		// Init
		//------------------------------------------------------------------
		public void Init(apEditor editor, apPortrait portrait, object loadKey)
		{
			_editor = editor;
			//_loadKey = loadKey;
			_targetPortrait = portrait;


		}

		// GUI
		//------------------------------------------------------------------
		void OnGUI()
		{
			int width = (int)position.width;
			int height = (int)position.height;
			if (_editor == null || _targetPortrait == null)
			{
				CloseDialog();
				return;
			}

			//만약 Portriat가 바뀌었거나 Editor가 리셋되면 닫자
			if (_editor != apEditor.CurrentEditor || _targetPortrait != apEditor.CurrentEditor._portrait)
			{
				CloseDialog();
				return;
			}




			//Sorting Layer를 추가하자
			if (_sortingLayerNames == null || _sortingLayerIDs == null)
			{
				_sortingLayerNames = new string[SortingLayer.layers.Length];
				_sortingLayerIDs = new int[SortingLayer.layers.Length];
			}
			else if (_sortingLayerNames.Length != SortingLayer.layers.Length
				|| _sortingLayerIDs.Length != SortingLayer.layers.Length)
			{
				_sortingLayerNames = new string[SortingLayer.layers.Length];
				_sortingLayerIDs = new int[SortingLayer.layers.Length];
			}

			for (int i = 0; i < SortingLayer.layers.Length; i++)
			{
				_sortingLayerNames[i] = SortingLayer.layers[i].name;
				_sortingLayerIDs[i] = SortingLayer.layers[i].id;
			}


			int width_2Btn = (width - 14) / 2;
			EditorGUILayout.BeginHorizontal(GUILayout.Width(width), GUILayout.Height(25));
			GUILayout.Space(5);
			if(apEditorUtil.ToggledButton(_editor.GetText(TEXT.DLG_Bake), _tab == TAB.Bake, width_2Btn, 25))
			{
				_tab = TAB.Bake;
			}
			if(apEditorUtil.ToggledButton(_editor.GetText(TEXT.DLG_Setting), _tab == TAB.Options, width_2Btn, 25))
			{
				_tab = TAB.Options;
			}
			EditorGUILayout.EndHorizontal();

			
			if (_tab == TAB.Bake)
			{
				GUILayout.Space(5);

				// 1. Bake에 대한 UI
				//Bake 설정
				//EditorGUILayout.LabelField(_editor.GetText(TEXT.DLG_BakeSetting));//"Bake Setting"
				//GUILayout.Space(5);

				EditorGUILayout.ObjectField(_editor.GetText(TEXT.DLG_Portrait), _targetPortrait, typeof(apPortrait), true);//"Portait"

				GUILayout.Space(5);

				//"Bake Scale"
				float prevBakeScale = _targetPortrait._bakeScale;
				_targetPortrait._bakeScale = EditorGUILayout.FloatField(_editor.GetText(TEXT.DLG_BakeScale), _targetPortrait._bakeScale);

				//"Z Per Depth"
				float prevBakeZSize = _targetPortrait._bakeZSize;
				_targetPortrait._bakeZSize = EditorGUILayout.FloatField(_editor.GetText(TEXT.DLG_ZPerDepth), _targetPortrait._bakeZSize);

				if (_targetPortrait._bakeZSize < 0.5f)
				{
					_targetPortrait._bakeZSize = 0.5f;
				}

				if (prevBakeScale != _targetPortrait._bakeScale ||
					prevBakeZSize != _targetPortrait._bakeZSize)
				{
					apEditorUtil.SetEditorDirty();
				}


				//Bake 버튼
				GUILayout.Space(10);
				if (GUILayout.Button(_editor.GetText(TEXT.DLG_Bake), GUILayout.Height(45)))//"Bake"
				{
					GUI.FocusControl(null);

					//CheckChangedProperties(nextRootScale, nextZScale);
					apEditorUtil.SetEditorDirty();

					//-------------------------------------
					// Bake 함수를 실행한다. << 중요오오오오
					//-------------------------------------
					apBakeResult bakeResult = _editor.Controller.Bake();


					_editor.Notification("[" + _targetPortrait.name + "] is Baked", false, false);

					if (bakeResult.NumUnlinkedExternalObject > 0)
					{
						EditorUtility.DisplayDialog(_editor.GetText(TEXT.BakeWarning_Title),
							_editor.GetTextFormat(TEXT.BakeWarning_Body, bakeResult.NumUnlinkedExternalObject),
							_editor.GetText(TEXT.Okay));
					}
				}

				GUILayout.Space(10);
				apEditorUtil.GUI_DelimeterBoxH(width - 10);
				GUILayout.Space(10);


				//최적화 Bake
				EditorGUILayout.LabelField(_editor.GetText(TEXT.DLG_OptimizedBaking));//"Optimized Baking"

				//"Target"
				apPortrait nextOptPortrait = (apPortrait)EditorGUILayout.ObjectField(_editor.GetText(TEXT.DLG_Target), _targetPortrait._bakeTargetOptPortrait, typeof(apPortrait), true);

				if (nextOptPortrait != _targetPortrait._bakeTargetOptPortrait)
				{
					//타겟을 바꾸었다.
					bool isChanged = false;
					if (nextOptPortrait != null)
					{
						//1. 다른 Portrait를 선택했다.
						if (!nextOptPortrait._isOptimizedPortrait)
						{
							//1-1. 최적화된 객체가 아니다.
							EditorUtility.DisplayDialog(_editor.GetText(TEXT.OptBakeError_Title),
														_editor.GetText(TEXT.OptBakeError_NotOptTarget_Body),
														_editor.GetText(TEXT.Close));
						}
						else if (nextOptPortrait._bakeSrcEditablePortrait != _targetPortrait)
						{
							//1-2. 다른 대상으로부터 Bake된 Portrait같다. (물어보고 계속)
							bool isResult = EditorUtility.DisplayDialog(_editor.GetText(TEXT.OptBakeError_Title),
														_editor.GetText(TEXT.OptBakeError_SrcMatchError_Body),
														_editor.GetText(TEXT.Okay),
														_editor.GetText(TEXT.Cancel));

							if (isResult)
							{
								//뭐 선택하겠다는데요 뭐..
								isChanged = true;

							}
						}
						else
						{
							//1-3. 오케이. 변경 가능
							isChanged = true;
						}
					}
					else
					{
						//2. 선택을 해제했다.
						isChanged = true;
					}

					if (isChanged)
					{
						//Target을 변경한다.
						apEditorUtil.SetRecord_Portrait(apUndoGroupData.ACTION.Portrait_SettingChanged, _editor, _targetPortrait, null, false);
						_targetPortrait._bakeTargetOptPortrait = nextOptPortrait;
					}

				}

				string optBtnText = "";
				if (_targetPortrait._bakeTargetOptPortrait != null)
				{
					//optBtnText = "Optimized Bake to\n[" + _targetPortrait._bakeTargetOptPortrait.gameObject.name + "]";
					optBtnText = string.Format("{0}\n[{1}]", _editor.GetText(TEXT.DLG_OptimizedBakeTo), _targetPortrait._bakeTargetOptPortrait.gameObject.name);
				}
				else
				{
					//optBtnText = "Optimized Bake\n(Make New GameObject)";
					optBtnText = _editor.GetText(TEXT.DLG_OptimizedBakeMakeNew);
				}
				GUILayout.Space(10);

				if (GUILayout.Button(optBtnText, GUILayout.Height(45)))
				{
					GUI.FocusControl(null);

					//CheckChangedProperties(nextRootScale, nextZScale);

					//Optimized Bake를 하자
					apBakeResult bakeResult = _editor.Controller.OptimizedBake(_targetPortrait, _targetPortrait._bakeTargetOptPortrait);

					if (bakeResult.NumUnlinkedExternalObject > 0)
					{
						EditorUtility.DisplayDialog(_editor.GetText(TEXT.BakeWarning_Title),
							_editor.GetTextFormat(TEXT.BakeWarning_Body, bakeResult.NumUnlinkedExternalObject),
							_editor.GetText(TEXT.Okay));
					}

					_editor.Notification("[" + _targetPortrait.name + "] is Baked (Optimized)", false, false);
				}

				
				
			}
			else
			{
				//Vector2 curScroll = (_tab == TAB.Bake) ? _scroll_Bake : _scroll_Options;

				_scroll_Options = EditorGUILayout.BeginScrollView(_scroll_Options, false, true, GUILayout.Width(width), GUILayout.Height(height - 30));
			
				EditorGUILayout.BeginVertical(GUILayout.Width(width - 24));
				GUILayout.Space(5);

				width -= 24;

				// 2. Option에 대한 UI
				//1. Gamma Space Space			
				bool prevBakeGamma = _editor._isBakeColorSpaceToGamma;
				int iPrevColorSpace = prevBakeGamma ? 0 : 1;
				int iNextColorSpace = EditorGUILayout.Popup(_editor.GetUIWord(UIWORD.ColorSpace), iPrevColorSpace, _colorSpaceNames);
				if (iNextColorSpace != iPrevColorSpace)
				{
					apEditorUtil.SetRecord_Portrait(apUndoGroupData.ACTION.Portrait_SettingChanged, _editor, _targetPortrait, null, false);
					if (iNextColorSpace == 0)
					{
						//Gamma
						_editor._isBakeColorSpaceToGamma = true;
					}
					else
					{
						//Linear
						_editor._isBakeColorSpaceToGamma = false;
					}
				}

				GUILayout.Space(10);

				//2. Sorting Layer
				int prevSortingLayerID = _editor._portrait._sortingLayerID;
				int prevSortingOrder = _editor._portrait._sortingOrder;

				int layerIndex = -1;
				for (int i = 0; i < SortingLayer.layers.Length; i++)
				{
					if (SortingLayer.layers[i].id == _editor._portrait._sortingLayerID)
					{
						//찾았다.
						layerIndex = i;
						break;
					}
				}
				if (layerIndex < 0)
				{
					apEditorUtil.SetRecord_Portrait(apUndoGroupData.ACTION.Portrait_SettingChanged, _editor, _targetPortrait, null, false);

					//어라 레이어가 없는데용..
					//초기화해야겠다.
					_editor._portrait._sortingLayerID = -1;
					if (SortingLayer.layers.Length > 0)
					{
						_editor._portrait._sortingLayerID = SortingLayer.layers[0].id;
						layerIndex = 0;
					}
				}
				int nextIndex = EditorGUILayout.Popup(_editor.GetText(TEXT.SortingLayer), layerIndex, _sortingLayerNames);//"Sorting Layer"
				if (nextIndex != layerIndex)
				{
					apEditorUtil.SetRecord_Portrait(apUndoGroupData.ACTION.Portrait_SettingChanged, _editor, _targetPortrait, null, false);
					//레이어가 변경되었다.
					if (nextIndex >= 0 && nextIndex < SortingLayer.layers.Length)
					{
						//LayerID 변경
						_editor._portrait._sortingLayerID = SortingLayer.layers[nextIndex].id;
					}
				}
				_editor._portrait._sortingOrder = EditorGUILayout.IntField(_editor.GetText(TEXT.SortingOrder), _editor._portrait._sortingOrder);//"Sorting Order"

				GUILayout.Space(10);

				//3. 메카님 사용 여부

				//EditorGUILayout.LabelField("Animation Settings");
				bool prevIsUsingMecanim = _targetPortrait._isUsingMecanim;
				string prevMecanimPath = _targetPortrait._mecanimAnimClipResourcePath;
				_targetPortrait._isUsingMecanim = EditorGUILayout.Toggle(_editor.GetText(TEXT.IsMecanimAnimation), _targetPortrait._isUsingMecanim);//"Is Mecanim Animation"
				EditorGUILayout.LabelField(_editor.GetText(TEXT.AnimationClipExportPath));//"Animation Clip Export Path"

				GUIStyle guiStyle_ChangeBtn = new GUIStyle(GUI.skin.button);
				guiStyle_ChangeBtn.margin = GUI.skin.textField.margin;
				guiStyle_ChangeBtn.border = GUI.skin.textField.border;

				EditorGUILayout.BeginHorizontal(GUILayout.Width(width), GUILayout.Height(20));
				GUILayout.Space(5);
				EditorGUILayout.TextField(_targetPortrait._mecanimAnimClipResourcePath, GUILayout.Width(width - (70 + 15)));
				if (GUILayout.Button(_editor.GetText(TEXT.DLG_Change), guiStyle_ChangeBtn, GUILayout.Width(70), GUILayout.Height(18)))
				{
					string defaultPath = _targetPortrait._mecanimAnimClipResourcePath;
					if (string.IsNullOrEmpty(defaultPath))
					{
						defaultPath = Application.dataPath;
					}
					string nextPath = EditorUtility.SaveFolderPanel("Select to export animation clips", defaultPath, "");
					if (!string.IsNullOrEmpty(nextPath))
					{
						if (apEditorUtil.IsInAssetsFolder(nextPath))
						{
							//유효한 폴더인 경우
							_targetPortrait._mecanimAnimClipResourcePath = nextPath;
						}
						else
						{
							//유효한 폴더가 아닌 경우
							EditorUtility.DisplayDialog("Invalid Folder Path", "Invalid Clip Path", "Close");
						}
					}

					GUI.FocusControl(null);

				}
				EditorGUILayout.EndHorizontal();


				

				GUILayout.Space(10);

				//4. Important
				//"Is Important"
				bool nextImportant = EditorGUILayout.Toggle(new GUIContent(_editor.GetText(TEXT.DLG_Setting_IsImportant), "When this setting is on, it always updates and the physics effect works."), _targetPortrait._isImportant);
				if(nextImportant != _targetPortrait._isImportant)
				{
					apEditorUtil.SetRecord_Portrait(apUndoGroupData.ACTION.Portrait_SettingChanged, _editor, _targetPortrait, null, false);
					_targetPortrait._isImportant = nextImportant;
				}

				//"FPS (Important Off)"
				int nextFPS = EditorGUILayout.DelayedIntField(new GUIContent(_editor.GetText(TEXT.DLG_Setting_FPS), "This setting is used when <Important> is off"), _targetPortrait._FPS);
				if (_targetPortrait._FPS != nextFPS)
				{
					apEditorUtil.SetRecord_Portrait(apUndoGroupData.ACTION.Portrait_SettingChanged, _editor, _targetPortrait, null, false);
					if (nextFPS < 10)
					{
						nextFPS = 10;
					}
					_targetPortrait._FPS = nextFPS;
				}

				GUILayout.Space(10);

				//5. Billboard + Perspective
				
				apPortrait.BILLBOARD_TYPE nextBillboardType = (apPortrait.BILLBOARD_TYPE)EditorGUILayout.Popup(_editor.GetText(TEXT.DLG_Billboard), (int)_targetPortrait._billboardType, _billboardTypeNames);
				if(nextBillboardType != _targetPortrait._billboardType)
				{
					apEditorUtil.SetRecord_Portrait(apUndoGroupData.ACTION.Portrait_SettingChanged, _editor, _targetPortrait, null, false);
					_targetPortrait._billboardType = nextBillboardType;
				}

				GUILayout.Space(10);

				//6. Shadow

				apPortrait.SHADOW_CASTING_MODE nextChastShadows = (apPortrait.SHADOW_CASTING_MODE)EditorGUILayout.EnumPopup(_editor.GetUIWord(UIWORD.CastShadows), _targetPortrait._meshShadowCastingMode);
				bool nextReceiveShaodw = EditorGUILayout.Toggle(_editor.GetUIWord(UIWORD.ReceiveShadows), _targetPortrait._meshReceiveShadow);
				if(nextChastShadows != _targetPortrait._meshShadowCastingMode
					|| nextReceiveShaodw != _targetPortrait._meshReceiveShadow)
				{
					apEditorUtil.SetRecord_Portrait(apUndoGroupData.ACTION.Portrait_SettingChanged, _editor, _targetPortrait, null, false);
					_targetPortrait._meshShadowCastingMode = nextChastShadows;
					_targetPortrait._meshReceiveShadow = nextReceiveShaodw;
				}

				GUILayout.Space(10);

#if UNITY_2018_2_OR_NEWER
				//7. LWRP
				//LWRP 쉐이더를 쓸지 여부와 다시 강제로 생성하기 버튼을 만들자.
				bool prevUseLWRP = _editor._isUseLWRPShader;
				int iPrevUseLWRP = prevUseLWRP ? 1 : 0;
				int iNextUseLWRP = EditorGUILayout.Popup("Render Pipeline", iPrevUseLWRP, _renderPipelineNames);
				if (iNextUseLWRP != iPrevUseLWRP)
				{
					apEditorUtil.SetRecord_Portrait(apUndoGroupData.ACTION.Portrait_SettingChanged, _editor, _targetPortrait, null, false);
					if (iNextUseLWRP == 0)
					{
						//사용 안함
						_editor._isUseLWRPShader = false;
					}
					else
					{
						//LWRP 사용함
						_editor._isUseLWRPShader = true;
					}
				}
				if(GUILayout.Button("Generate Lightweight Shaders"))
				{
					apShaderGenerator shaderGenerator = new apShaderGenerator();
					shaderGenerator.GenerateLWRPShaders();
				}

				GUILayout.Space(10);
#endif

				//11.7 추가 : Ambient Light를 검은색으로 만든다.
				GUILayout.Space(10);
				if (GUILayout.Button(_editor.GetText(TEXT.DLG_AmbientToBlack), GUILayout.Height(20)))
				{
					MakeAmbientLightToBlack();
				}

				//CheckChangedProperties(nextRootScale, nextZScale);
				if (prevSortingLayerID != _editor._portrait._sortingLayerID ||
					prevSortingOrder != _editor._portrait._sortingOrder ||
					prevIsUsingMecanim != _targetPortrait._isUsingMecanim ||
					!string.Equals(prevMecanimPath, _targetPortrait._mecanimAnimClipResourcePath) ||
					prevBakeGamma != _editor._isBakeColorSpaceToGamma
#if UNITY_2018_2_OR_NEWER
					 || prevUseLWRP != _editor._isUseLWRPShader
#endif
					 )
				{
					apEditorUtil.SetEditorDirty();
					_editor.SaveEditorPref();
				}

				GUILayout.Space(height + 500);
				EditorGUILayout.EndVertical();

				EditorGUILayout.EndScrollView();
			}

			GUILayout.Space(5);
		}

		//private void CheckChangedProperties(float nextRootScale, float nextZScale)
		//{
		//	bool isChanged = false;
		//	if (nextRootScale != _targetPortrait._bakeScale
		//		|| nextZScale != _targetPortrait._bakeZSize)
		//	{
		//		isChanged = true;
		//	}

		//	if (isChanged)
		//	{
		//		apEditorUtil.SetRecord_Portrait(apUndoGroupData.ACTION.Portrait_BakeOptionChanged, _editor, _targetPortrait, null, false);

		//		if (nextRootScale < 0.0001f)
		//		{
		//			nextRootScale = 0.0001f;
		//		}
		//		if(nextZScale < 0.5f)
		//		{
		//			nextZScale = 0.5f;
		//		}

		//		_targetPortrait._bakeScale = nextRootScale;
		//		_targetPortrait._bakeZSize = nextZScale;

				

		//		//if(nextPhysicsScale < 0.0f)
		//		//{
		//		//	nextPhysicsScale = 0.0f;
		//		//}
		//		//_targetPortrait._physicBakeScale = nextPhysicsScale;

		//		GUI.FocusControl(null);
		//	}
		//}

		private void MakeAmbientLightToBlack()
		{	
			RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Flat;
			RenderSettings.ambientLight = Color.black;
			apEditorUtil.SetEditorDirty();
		}
	}

}