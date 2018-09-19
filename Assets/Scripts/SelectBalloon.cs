using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Linq;
using System;
using System.Diagnostics;

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
			var splitedAnswer = answers[i - 1].Split(new char[] {'@'}, StringSplitOptions.RemoveEmptyEntries).ToList();
			newButton.GetComponentInChildren<Text>().text = splitedAnswer[0];;
			newButton.name = i.ToString();
			newButton.onClick.AddListener(delegate
			{
				splitedAnswer.ForEach(s => UnityEngine.Debug.Log("<<"+s+">>"));
				UnityEngine.Debug.Log(splitedAnswer.Count);
				if (splitedAnswer.Count > 1)
				{
					SelectAnswer(newButton, splitedAnswer[1]);
				}
				else
				{
					UnityEngine.Debug.Log("else case");
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
		singleDialogueManager.CallAddBalloon(answerText);
		singleDialogueManager.NextDialogueAddress = address;
		this.gameObject.SetActive(false);
	}

	static List<string> ParseAnswers(string str) {
		var splitedString = str.Split(new char[] {'#'}, StringSplitOptions.RemoveEmptyEntries).ToList();
		return splitedString;
	}
}