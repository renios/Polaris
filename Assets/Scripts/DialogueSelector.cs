using UnityEngine;
using Enums;

namespace Sirenix.OdinInspector.Demos
{
public class DialogueSelector : MonoBehaviour
{
#if UNITY_EDITOR
	[PropertyOrder(-1)]
	[OnInspectorGUI]
	public void DrawInfo()
	{
		Sirenix.Utilities.Editor.SirenixEditorGUI.InfoMessageBox(
			"테스트하려는 1:1 대화를 선택하세요");
	}
#endif
	
	[HorizontalGroup("Split")]
	public CharactorName Name;

	[HideLabel]
	[VerticalGroup("Split/Right")]
	public int level;
}
}