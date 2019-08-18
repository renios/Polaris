using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Store
{
    public class StoreInfoPopup : MonoBehaviour
    {
        public Text Title, Description, PrevLevel, PrevValue, NextLevel, NextValue, RequireMoney;
        public Button SureButton;

        private bool selected, selectResult;

        public IEnumerator Show(string title, string desc, int storeItemIndex, System.Func<int, string> valueToString, System.Action<bool> result)
        {
            selected = false;
            Title.text = title;
            Description.text = desc;

            int curLevel = Variables.StoreUpgradeLevel[storeItemIndex];
            PrevLevel.text = "Lv. " + curLevel.ToString();
            PrevValue.text = valueToString(Variables.StoreUpgradeValue[storeItemIndex][curLevel]);
            NextLevel.text = "Lv. " + (curLevel + 1).ToString();
            NextValue.text = valueToString(Variables.StoreUpgradeValue[storeItemIndex][curLevel + 1]);
            RequireMoney.text = Variables.StoreUpgradeMoney[storeItemIndex][curLevel].ToString();
            gameObject.SetActive(true);

            if (Variables.Starlight < Variables.StoreUpgradeMoney[storeItemIndex][curLevel])
                SureButton.interactable = false;
            else
                SureButton.interactable = true;

            yield return new WaitUntil(() => selected);
            result(selectResult);
            gameObject.SetActive(false);
        }

        public void Select(bool value)
        {
            selected = true;
            selectResult = value;
        }
    }
}