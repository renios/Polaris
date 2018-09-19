using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class NormalBalloon : Balloon
{
	public Text DialogueText;

	public override void Init()
	{
		//
	}

	public override void SetBalloonData(string str)
	{
		DialogueText.text = str;
	}
}
