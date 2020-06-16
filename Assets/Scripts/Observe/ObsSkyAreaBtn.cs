﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Observe
{
	public class ObsSkyAreaBtn : MonoBehaviour
	{
		public int index;
		public int price;
		[TextArea] public string openRule;
		public Text priceLabel;
		public Text openRuleLabel;
		public GameObject pricePanel;
		public GameObject openRulePanel;
		public ObserveManager manager;

		void Start()
		{
			priceLabel.text = price.ToString();
			openRuleLabel.text = openRule;

			pricePanel.GetComponent<Image>().alphaHitTestMinimumThreshold = 0.05f;
		}

		public void Clicked()
		{
			StartCoroutine(Clicked_Routine());
		}

		IEnumerator Clicked_Routine()
		{
			yield return MessageSet.Now.ShowMoneySpendAsk("해당 구역을 개방할까요?", MoneyType.Starlight, price, result =>
			{
				if (result)
				{
					if (Variables.Starlight < price)
						MessageSet.Now.ShowNoMoneyAlert(MoneyType.Starlight);
					else
						manager.UnlockSky(index);
				}
			});
		}
	}
}