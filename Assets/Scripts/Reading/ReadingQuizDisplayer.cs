using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Dialogue;

namespace Reading
{
	public class ReadingQuizDisplayer : MonoBehaviour
	{
		public Text question;
		public Text answer;
		public RectTransform popupBody;

		[HideInInspector] public bool showing;

		public IEnumerator Show(string q, string a)
		{
			question.text = q;
			answer.text = a;
			
			showing = true;
			gameObject.SetActive(true);
			LayoutRebuilder.MarkLayoutForRebuild(popupBody);
			LayoutRebuilder.ForceRebuildLayoutImmediate(popupBody);

			yield return new WaitWhile(() => showing);
			gameObject.SetActive(false);
		}

		public void Hide()
		{
			showing = false;
		}
	}
}