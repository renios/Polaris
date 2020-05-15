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
		public GameObject answerTemplate;
		public RectTransform answerParent;
		public RectTransform popupBody;

		[HideInInspector] public bool showing;

		List<GameObject> answerObjs;

		public IEnumerator Show(string context, DialogueContent answers, int selectedPhase)
		{
			if (answerObjs == null)
				answerObjs = new List<GameObject>();
			else
			{
				foreach (var obj in answerObjs)
					Destroy(obj);
				answerObjs.Clear();
			}

			question.text = context;

			for(int i = 0; i < answers.JuncTexts.Length; i++)
			{
				var newObj = Instantiate(answerTemplate);
				newObj.transform.SetParent(answerParent);
				newObj.transform.localScale = Vector3.one;

				var phase = answers.Directions[i];
				newObj.GetComponent<ReadingQuizAnswerElement>().Set(answers.JuncTexts[i], phase == selectedPhase, phase == 1);
				newObj.SetActive(true);
				answerObjs.Add(newObj);
			}

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