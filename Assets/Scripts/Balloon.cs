using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace SuperScrollView
{
	public class Balloon : MonoBehaviour
	{
		public Text mDialogueText;
		int mItemDataIndex = -1;

		public void Init()
		{
			transform.DOScale(1, 0.5f).SetEase(Ease.OutExpo);
		}

		public void SetItemData(ItemData itemData, int itemIndex)
		{
			mItemDataIndex = itemIndex;
			mDialogueText.text = LoadText(itemIndex);
		}

		string LoadText(int itemIndex)
		{
			if (itemIndex < DialogueText.Text.Length)
				return DialogueText.Text[itemIndex];
			else 
				return "";
		}
	}
}
