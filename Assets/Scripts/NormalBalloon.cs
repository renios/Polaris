using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class NormalBalloon : Balloon
{
	public Text DialogueText;
	// int mItemDataIndex = -1;

	public override void Init()
	{
		//
	}

	public override void SetBalloonData(string str)
	{
		DialogueText.text = str;
	}

	// public void SetBalloonData(ItemData itemData, int itemIndex)
	// {
	// 	mItemDataIndex = itemIndex;
	// 	DialogueText.text = LoadText(itemIndex);
	// }

	// string LoadText(int itemIndex)
	// {
	// 	if (itemIndex < global::DialogueText.Text.Length)
	// 		return global::DialogueText.Text[itemIndex];
	// 	else 
	// 		return "";
	// }
}
