using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Reading
{
	public class ReadingCharPicker : MonoBehaviour
	{
		public GameObject charTemplate;
		public RectTransform charParent;

		[HideInInspector] public int resultIndex;
		[HideInInspector] public bool hasResult;

		public void LoadCharacter()
		{
			// Once per scene load
			foreach(var character in Variables.Characters)
			{
				var newObj = Instantiate(charTemplate);
				newObj.transform.SetParent(charParent);
				newObj.transform.localScale = Vector3.one;
				newObj.GetComponent<ReadingCharListToggle>().Set(character);
				newObj.SetActive(true);
			}
		}

		public IEnumerator Show(System.Action<int> afterResult)
		{
			hasResult = false;
			gameObject.SetActive(true);
			yield return new WaitUntil(() => hasResult);
			gameObject.SetActive(false);
			afterResult(resultIndex);
		}

		public void EndSelect()
		{
			hasResult = true;
		}
	}
}