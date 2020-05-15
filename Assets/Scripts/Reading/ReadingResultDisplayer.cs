using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Reading
{
	public class ReadingResultDisplayer : MonoBehaviour
	{
		public GameObject solutionTemplate;
		public RectTransform solutionParent;

		public void Show(List<bool> result, List<string> solution)
		{
			for(int i = 0; i < result.Count; i++)
			{
				var newObj = Instantiate(solutionTemplate);
				newObj.transform.SetParent(solutionParent);
				newObj.transform.localScale = Vector3.one;
				newObj.GetComponent<ReadingResultElement>().Set(i + 1, result[i], solution[i]);
				newObj.SetActive(true);
			}
			gameObject.SetActive(true);
		}
	}
}