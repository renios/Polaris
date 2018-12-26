using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
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
			var splitedAnswer = answers[i - 1].Split(new[] {'@'}, StringSplitOptions.RemoveEmptyEntries).ToList();
			newButton.GetComponentInChildren<Text>().text = splitedAnswer[0];;
			newButton.name = i.ToString();
			newButton.onClick.AddListener(delegate
			{
				if (splitedAnswer.Count > 1)
				{
					SelectAnswer(newButton, splitedAnswer[1]);
				}
				else
				{
					SelectAnswer(newButton);
				}
			});
		}
	}

	public void SelectAnswer(Button button, string address = "") {
		var singleDialogueManager = FindObjectOfType<SingleDialogueManager>();
		singleDialogueManager.Selecting = false;
		
		var index = int.Parse(button.name);
		var answerText = button.GetComponentInChildren<Text>().text;
		singleDialogueManager.DialogueSubIndex = index;
		singleDialogueManager.NextDialogueAddress = address;
		singleDialogueManager.CallAddBalloon(answerText);
		this.gameObject.SetActive(false);
	}

	static List<string> ParseAnswers(string str) {
		var splitedString = str.Split(new[] {'#'}, StringSplitOptions.RemoveEmptyEntries).ToList();
		return splitedString;
	}
}