using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

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
	public List<DialoguePiece> join;
	public List<DialoguePiece> first;
	public List<DialoguePiece> second;
	public List<DialoguePiece> third;
	public List<DialoguePiece> fourth;
	public List<DialoguePiece> fifth;
	public List<DialoguePiece> final;
}

[Serializable]
public class DialoguePiece
{
	public string index;
	public List<string> text;

	public DialoguePiece() {
		index = "0";
		text = new List<string>() { "* 해당하는 대화가 없어요" };
	}
}

public class DialogueSelector : MonoBehaviour
{
#if UNITY_EDITOR
	[PropertyOrder(-1)]
	[OnInspectorGUI]
	public void DrawInfo()
	{
		Sirenix.Utilities.Editor.SirenixEditorGUI.InfoMessageBox(
			"테스트하려는 1:1 대화를 선택하세요" + '\n' +
			"0: 첫 만남 대사" + '\n' +
			"1~5: 호감도 레벨 1~5단계 달성시 잠금해제" + '\n' +
			"6: 최대 호감도 달성시 개별 엔딩");
	}
#endif

	[HideLabel]
	[HorizontalGroup("Split")]
	public CharacterName CharacterName;

	[HideLabel]
	[VerticalGroup("Split/Right")]
	public int Level;

	Data _selectedData;

	void Awake()
	{
		var nameString = EnumToString(SelectedDialogueData.SelectedCharacterName);
		var jsonText = Resources.Load<TextAsset>("SingleDialogue" + "_" + nameString);
		var characterData = JsonUtility.FromJson<Data>(jsonText.text);
		_selectedData = characterData;
//		var characterData = JsonUtility.FromJson<DataList>(jsonText.text);
//		_selectedData = characterData.dataList.Find(x => x.name == SelectedDialogueData.SelectedCharacterName.ToString());
	}

	public List<DialoguePiece> GetTestDialogues()
	{
		return GetDialogueByLevel(_selectedData);
	}

	public string GetCharacterName()
	{
		return _selectedData.name;
	}

	public List<DialoguePiece> GetDialogueByLevel(Data data)
	{
		List<DialoguePiece> dialogue;
		switch (SelectedDialogueData.SelectedDialogueLevel)
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
				dialogue = data.dialogues.join;
				break;
		}

		if (dialogue.Count < 1)
			dialogue = data.dialogues.join;

		return dialogue;
	}

	string EnumToString(CharacterName name)
	{
		switch (name)
		{
			case CharacterName.멜리크: return "ml";
			case CharacterName.베가: return "bg";
			case CharacterName.아케르: return "ac";
			case CharacterName.안카: return "ak";
			case CharacterName.안타레스: return "at";
			case CharacterName.에니프: return "en";
			case CharacterName.폴라리스: return "pl";
			default: return "pl";
		}
	}
}