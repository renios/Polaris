using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Sirenix.OdinInspector.Demos;
using UnityEngine.UI;

namespace SuperScrollView
{
	public class SingleDialogueManager : MonoBehaviour {

		public Text characterName;
		public List<Balloon> balloons;
		int balloonIndex = 0;
		List<string> dialogues = new List<string>();

		// Use this for initialization
		void Start () {
			if (FindObjectOfType<DialogueSelector>() != null)
			{
				DialogueSelector dialogueSelector = FindObjectOfType<DialogueSelector>();
				characterName.text = dialogueSelector.GetCharacterName();
				dialogues = dialogueSelector.GetTestDialogues();
			}

			StartCoroutine(AddBalloon());
		}

		public IEnumerator AddBalloon() {
			GetComponent<RectTransform>().anchoredPosition = new Vector2(GetComponent<RectTransform>().anchoredPosition.x, 0);

			if (balloonIndex >= dialogues.Count) yield break;

			Balloon newBalloon = Instantiate(balloons[0], transform);
			newBalloon.Init();
			// TODO: balloonData를 제대로 끼워넣어야 함
			newBalloon.SetBalloonData(dialogues[balloonIndex]);
			// newBalloon.SetBalloonData(null, balloonIndex);
			balloonIndex++;
			yield return new WaitForSeconds(newBalloon.GetComponent<DoTweenHelper>().duration);
		}

		void Update() {
			if (Input.GetKeyDown(KeyCode.Space))
			{
				StartCoroutine(AddBalloon());
			}
		}
	}
}