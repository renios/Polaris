using UnityEngine;
using Enums;
using System.Collections.Generic;
using System.Linq;
using System;

namespace Sirenix.OdinInspector.Demos
{
[Serializable]
public class DataList {
	public List<Data> dataList;
}

[Serializable]
public class Data
{
	public string name;
	public Dialogues dialogues;
}

[Serializable]
public class Dialogues
{
	public List<string> join;
	public List<string> first;
	public List<string> second;
	public List<string> third;
	public List<string> final;
}

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
	public CharacterName characterName;

	[HideLabel]
	[VerticalGroup("Split/Right")]
	public int level;

	public List<string> GetTestDialogues()
	{
		TextAsset jsonText = Resources.Load<TextAsset>("Dialogue");
		DataList characterData = JsonUtility.FromJson<DataList>(jsonText.text);
		
		Data selectedData = characterData.dataList.Find(x => x.name == characterName.ToString());
		List<string> selectedDialogue = GetDialogueByLevel(selectedData);
		
		return selectedDialogue;
	}

	public List<string> GetDialogueByLevel(Data data)
	{
		switch (level)
		{
			case 0:
				return data.dialogues.join;
			case 1:
				return data.dialogues.first;
			case 2:
				return data.dialogues.second;
			case 3:
				return data.dialogues.third;
			default:
				return data.dialogues.join;
		}
	}
}
}