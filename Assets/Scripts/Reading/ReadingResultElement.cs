using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Reading
{
	public class ReadingResultElement : MonoBehaviour
	{
		public Text numberText, solutionText;
		public GameObject OObject, XObject;

		public void Set(int index, bool result, string solution)
		{
			numberText.text = index.ToString() + ".";
			OObject.SetActive(result);
			XObject.SetActive(!result);
			solutionText.text = solution;
		}
	}
}