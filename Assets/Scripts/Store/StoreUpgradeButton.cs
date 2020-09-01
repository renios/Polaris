using System.Collections;
using System.Collections.Generic;
using NUnit.Framework.Internal;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace Store
{
    [RequireComponent(typeof(Button))]
    public class StoreUpgradeButton : MonoBehaviour
    {
        public int typeIndex;
        public bool showLevelNotValue;
        public string payContext;
        public string prefix, suffix;
        public Text prevLevelText, nextLevelText;
        public Text priceText;
        public GameObject moneyPanel, fulledPanel;

        int level, price;
        
        // Use this for initialization
        void Start ()
        {
            DisplayState();
        }

        void DisplayState()
        {
            var curValue = Variables.GetStoreValue(typeIndex, out level);
            prevLevelText.text = prefix + (showLevelNotValue ? level.ToString() : curValue.ToString()) + suffix;

            if (level == Variables.GetStoreMaxLevel(typeIndex))
            {
                nextLevelText.text = "MAX";
                moneyPanel.SetActive(false);
                fulledPanel.SetActive(true);
                GetComponent<Button>().interactable = false;
            }
            else
            {
                moneyPanel.SetActive(true);
                fulledPanel.SetActive(false);
                GetComponent<Button>().interactable = true;

                var nextValue = Variables.GetStoreValue(typeIndex, level + 1);
                nextLevelText.text = prefix + (showLevelNotValue ? (level + 1).ToString() : nextValue.ToString()) + suffix;

                price = Variables.GetStoreReqMoney(typeIndex, level);
                priceText.text = price.ToString();
            }
        }

        public void Clicked()
        {
            StartCoroutine(Clicked_Routine());
        }

        IEnumerator Clicked_Routine()
        {
            if (!GameManager.Instance.MoneyPayable(MoneyType.Starlight, price))
                yield return MessageSet.Now.ShowNoMoneyAlert(MoneyType.Starlight);
            else
            {
                var accepted = false;
                yield return MessageSet.Now.ShowMoneySpendAsk(payContext, MoneyType.Starlight, price, r => { accepted = r; });
                if (accepted)
                {
                    GameManager.Instance.PayMoney(MoneyType.Starlight, price);
                    Variables.StoreUpgradeLevel[typeIndex]++;
                    GameManager.Instance.SaveGame();
                    
                    DisplayState();
                }
            }
        }
    }
}