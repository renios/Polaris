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
	public List<string> fourth;
	public List<string> fifth;
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

	[HideLabel]
	[HorizontalGroup("Split")]
	public CharacterName characterName;

	[HideLabel]
	[VerticalGroup("Split/Right")]
	public int level;

	Data selectedData;

	void Awake()
	{
		TextAsset jsonText = Resources.Load<TextAsset>("SingleDialogue");
		DataList characterData = JsonUtility.FromJson<DataList>(jsonText.text);
		selectedData = characterData.dataList.Find(x => x.name == characterName.ToString());
	}

	public List<string> GetTestDialogues()
	{
		return GetDialogueByLevel(selectedData);
	}

	public string GetCharacterName()
	{
		return selectedData.name;
	}

	public List<string> GetDialogueByLevel(Data data)
	{
		List<string> dialogue;
		switch (level)
		{
			case 0:
				dialogue = data.dialogues.join;
				break;
			case 1:
				dialogue = data.dialogues.first;
				break;
			case 2:
				dialogue = data.dialogues.second;
				break;
			case 3:
				dialogue = data.dialogues.third;
				break;
			case 4:
				dialogue = data.dialogues.fourth;
				break;
			case 5:
				dialogue = data.dialogues.fifth;
				break;
			case 6:
				dialogue = data.dialogues.final;
				break;
			default:
				dialogue = new List<string>{ "*해당하는 대화가 없어요" };
				break;
		}

		if (dialogue.Count < 1)
			dialogue = new List<string>{ "*해당하는 대화가 없어요" };

		return dialogue;
	}
}
}