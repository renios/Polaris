using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Observe
{
    public class ObserveTimeButton : MonoBehaviour
    {
        public int Index;
        public GameObject ButtonObj, PanelObj;
        public Text costLabel;

        [HideInInspector] public int SpendMoney;
        
        void Start()
        {
            if(Variables.StoreUpgradeLevel[0] >= Index)
            {
                ButtonObj.SetActive(true);
                PanelObj.SetActive(false);
            }
            else
            {
                ButtonObj.SetActive(false);
                PanelObj.SetActive(true);
            }

            SpendMoney = Variables.values.observeCost[Index];
            costLabel.text = SpendMoney.ToString();
        }
    }
}