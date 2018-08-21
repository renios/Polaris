using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class SelectBalloon : Balloon
{
	public Text FirstAnswerText;
	public Text SecondAnswerText;
	
	public override void Init()
	{
		//
	}

	public override void SetBalloonData(string str)
	{
		var splitedString = str.Split('#');
		FirstAnswerText.text = splitedString[1];
		SecondAnswerText.text = splitedString[2];
	}

	public void SelectAnswer(int index) {
		var singleDialogueManager = FindObjectOfType<SingleDialogueManager>();
		if (index == 1) {
			StartCoroutine(singleDialogueManager.AddBalloon(FirstAnswerText.text));
		}
		else {
			StartCoroutine(singleDialogueManager.AddBalloon(SecondAnswerText.text));
		}
		Destroy(this.gameObject);
	}
}