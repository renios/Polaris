using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Sirenix.OdinInspector.Demos;
using UnityEngine.UI;

public class SingleDialogueManager : MonoBehaviour {

	public Text CharacterName;
	public List<Balloon> Balloons;
	int _balloonIndex = 0;
	List<DialoguePiece> _dialogues = new List<DialoguePiece>();
	public bool Selecting = false;
	public int DialogueIndex = 0;
	public int DialogueSubIndex = 0;
	List<string> _texts = new List<string>();
	
	// Use this for initialization
	void Start () {
		if (FindObjectOfType<DialogueSelector>() != null)
		{
			var dialogueSelector = FindObjectOfType<DialogueSelector>();
			CharacterName.text = dialogueSelector.GetCharacterName();
			_dialogues = dialogueSelector.GetTestDialogues();
		}

		_texts = _dialogues.Find(x => x.index == "0").text;

		StartCoroutine(AddBalloon());
	}

	public void CallAddBalloon(string answer = null) {
		StartCoroutine(AddBalloon(answer));
	}

	IEnumerator AddBalloon(string answer = null) {
		GetComponent<RectTransform>().anchoredPosition = new Vector2(GetComponent<RectTransform>().anchoredPosition.x, 0);

		if (_balloonIndex >= _texts.Count) yield break;
		if (Selecting) yield break;

		Balloon newBalloon;
		// 일반 대화
		if (_texts[_balloonIndex][0] != '#') {
			newBalloon = Instantiate(Balloons[0], transform);
			newBalloon.Init();
			newBalloon.SetBalloonData(_texts[_balloonIndex]);
			_balloonIndex++;
			if (_balloonIndex == _texts.Count)
				LoadNextTexts();
		}
		// 유저의 대답을 출력
		else if (answer != null) {
			newBalloon = Instantiate(Balloons[1], transform);
			newBalloon.Init();
			newBalloon.SetBalloonData(answer);
			_balloonIndex++; // 선택지가 뜰 때는 카운트가 오르지 않고, 선택을 했을때 카운트가 오른다
			LoadNextTexts(); // 선택을 할 경우 무조건 다음 대화 묶음으로 넘어간다
			
			yield return new WaitForSeconds(newBalloon.GetComponent<DoTweenHelper>().Duration);
			
			StartCoroutine(AddBalloon()); // 다음 대화 묶음의 첫번째 텍스트를 자동으로 로드한다
		}
		// 유저가 대답 선택
		else {
			newBalloon = Instantiate(Balloons[2], transform);
			newBalloon.Init();
			newBalloon.SetBalloonData(_texts[_balloonIndex]);
			Selecting = true;
		}
		yield return new WaitForSeconds(newBalloon.GetComponent<DoTweenHelper>().Duration);
	}

	void LoadNextTexts() {
		DialogueIndex++;

		var newIndex = DialogueIndex.ToString();
		if (DialogueSubIndex != 0)
			newIndex = newIndex + "-" + DialogueSubIndex.ToString();

		var newTexts = _dialogues.Find(x => x.index == newIndex);
		if (newTexts != null) {
			_texts = _dialogues.Find(x => x.index == newIndex).text;
			_balloonIndex = 0; 
		}
		else
			Debug.LogWarning("next dialogue is null");

		DialogueSubIndex = 0;
	}

	void Update() {
		if (Input.GetKeyDown(KeyCode.Space))
		{
			StartCoroutine(AddBalloon());
		}
	}
}