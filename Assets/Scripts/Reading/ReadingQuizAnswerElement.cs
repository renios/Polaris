using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Reading
{
	public class ReadingQuizAnswerElement : MonoBehaviour
	{
		public Text answer;
		public GameObject userSelected;
		public GameObject isRightAnswer, answerNullField;

		public void Set(string context, bool selected, bool right)
		{
			answer.text = context;
			answerNullField.SetActive(!right);
			isRightAnswer.SetActive(right);
			userSelected.SetActive(selected);
		}
	}
}