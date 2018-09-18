using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Linq;
using System;

public class SelectBalloon : Balloon
{
	public Button AnswerButton;
	public Transform ImageTransform;

	public override void Init()
	{
		//
	}

	public override void SetBalloonData(string str)
	{
		var answers = ParseAnswers(str);
		for (var i = 1; i <= answers.Count; i++) {
			var newButton = Instantiate(AnswerButton, ImageTransform);
			newButton.GetComponentInChildren<Text>().text = answers[i-1];
			newButton.name = i.ToString();
			newButton.onClick.AddListener(delegate { SelectAnswer(newButton); });
		}
	}

	public void SelectAnswer(Button button) {
		var singleDialogueManager = FindObjectOfType<SingleDialogueManager>();
		singleDialogueManager.Selecting = false;
		
		var index = int.Parse(button.name);
		var answerText = button.GetComponentInChildren<Text>().text;
		singleDialogueManager.DialogueSubIndex = index;
		singleDialogueManager.CallAddBalloon(answerText);
		this.gameObject.SetActive(false);
	}

	static List<string> ParseAnswers(string str) {
		var splitedString = str.Split('#').ToList();
		splitedString.RemoveAt(0);
		return splitedString;
	}
}