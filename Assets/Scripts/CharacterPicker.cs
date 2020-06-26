using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterPicker : MonoBehaviour 
{
	public GameObject charTemplate;
	public RectTransform charParent;
	public Text countText;
	
	[HideInInspector] public bool waiting, hasResult;

	int maxCharCount;
	bool queuedSelect;
	List<int> pickedCharList;
	List<CharacterPickerElement> objList;
	Dictionary<int, int> charObjLinker;

	public void LoadCharacter(bool includePolaris = false)
	{
		objList = new List<CharacterPickerElement>();
		charObjLinker = new Dictionary<int, int>();
		int i = 0;
		// Once per scene load
		foreach(var character in Variables.Characters)
		{
			if (character.Key == 1 && !includePolaris)
				continue;
			
			var newObj = Instantiate(charTemplate);
			newObj.transform.SetParent(charParent);
			newObj.transform.localScale = Vector3.one;
			newObj.transform.localPosition = Vector3.zero;
			newObj.GetComponent<CharacterPickerElement>().Set(character);
			newObj.SetActive(true);
			objList.Add(newObj.GetComponent<CharacterPickerElement>());
			charObjLinker.Add(character.Key, i++);
		}
	}

	public IEnumerator Show(int pickCount, bool allowSelectQueue, List<int> prePickedChars, System.Action<List<int>> afterResult)
	{
		maxCharCount = pickCount;
		queuedSelect = allowSelectQueue;
		pickedCharList = new List<int>();

		foreach (var obj in objList)
		{
			//obj.toggledByCode = true;
			obj.picked = false;
			obj.toggle.isOn = false;
		}
		foreach (var charIndex in prePickedChars)
		{
			pickedCharList.Add(charIndex);
			//objList[charObjLinker[charIndex]].toggledByCode = true;
			objList[charObjLinker[charIndex]].picked = true;
			objList[charObjLinker[charIndex]].toggle.isOn = true;
		}

		countText.text = "최대 " + maxCharCount.ToString() + "명의 캐릭터를 선택할 수 있습니다.";
		hasResult = false;
		gameObject.SetActive(true);
		waiting = true;
		yield return new WaitUntil(() => hasResult);
		waiting = false;
		gameObject.SetActive(false);
		afterResult(pickedCharList);
	}

	public bool TryAddCharacter(int index)
	{
		//Debug.Log("List Count: " + pickedCharList.Count + ", Adding " + index);
		if (pickedCharList.Count == maxCharCount)
		{
			if (!queuedSelect)
				return false;

			var prevIdx = pickedCharList[0];
			//Debug.Log("- Removing " + prevIdx);
			objList[charObjLinker[prevIdx]].toggledByCode = true;
			objList[charObjLinker[prevIdx]].picked = false;
			objList[charObjLinker[prevIdx]].toggle.isOn = false;
			pickedCharList.RemoveAt(0);
		} 
		
		pickedCharList.Add(index);
		return true;
	}

	public bool RemoveCharacter(int index)
	{
		if (maxCharCount == 1)
			return false;
		
		pickedCharList.Remove(index);
		return true;
	}

	public void EndSelect()
	{
		hasResult = true;
	}
}
