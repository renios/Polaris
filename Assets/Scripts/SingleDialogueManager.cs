using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Sirenix.OdinInspector.Demos;
using UnityEngine.UI;

public class SingleDialogueManager : MonoBehaviour {

	public Text characterName;
	public List<Balloon> balloons;
	int balloonIndex = 0;
	List<string> dialogues = new List<string>();
	public bool Selecting = false;
	
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

	public void CallAddBalloon(string answer = null) {
		StartCoroutine(AddBalloon(answer));
	}

	IEnumerator AddBalloon(string answer = null) {
		GetComponent<RectTransform>().anchoredPosition = new Vector2(GetComponent<RectTransform>().anchoredPosition.x, 0);

		if (balloonIndex >= dialogues.Count) yield break;
		if (Selecting) yield break;

		Balloon newBalloon;
		// 일반 대화
		if (dialogues[balloonIndex][0] != '#') {
			newBalloon = Instantiate(balloons[0], transform);
			newBalloon.Init();
			newBalloon.SetBalloonData(dialogues[balloonIndex]);
			balloonIndex++;
		}
		// 유저의 대답을 출력
		else if (answer != null) {
			newBalloon = Instantiate(balloons[1], transform);
			newBalloon.Init();
			newBalloon.SetBalloonData(answer);
			balloonIndex++; // 선택지가 뜰 때는 카운트가 오르지 않고, 선택을 했을때 카운트가 오른다
		}
		// 유저가 대답 선택
		else {
			newBalloon = Instantiate(balloons[2], transform);
			newBalloon.Init();
			newBalloon.SetBalloonData(dialogues[balloonIndex]);
			Selecting = true;
		}
		yield return new WaitForSeconds(newBalloon.GetComponent<DoTweenHelper>().duration);
	}

	void Update() {
		if (Input.GetKeyDown(KeyCode.Space))
		{
			StartCoroutine(AddBalloon());
		}
	}
}