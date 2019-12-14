using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Observe
{
    public class ObserveTimeButton : MonoBehaviour
    {
        public int Index, SpendMoney;
        public GameObject ButtonObj, PanelObj;

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
        }
    }
}