using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Store
{
    public class StoreManager : MonoBehaviour
    {
        public StoreInfoPopup InfoPopup;
        public Button[] UpgradeBtns;

        private void Start()
        {
            DisplayAtButton();
        }

        public void DisplayAtButton()
        {
            for(int i = 0; i < UpgradeBtns.Length; i++)
            {
                if(Variables.StoreUpgradeLevel[i] > Variables.StoreUpgradeValue[i].Length)
                {
                    UpgradeBtns[i].interactable = false;
                    UpgradeBtns[i].GetComponentInChildren<Text>().text = "현재 Lv. " + Variables.StoreUpgradeLevel[i].ToString() + "\n최대 레벨입니다.";
                }
                else
                {
                    UpgradeBtns[i].GetComponentInChildren<Text>().text = 
                        "현재 Lv. " + Variables.StoreUpgradeLevel[i].ToString() + 
                        "\n다음 Lv. " + (Variables.StoreUpgradeLevel[i] + 1).ToString();
                }
            }
        }

        public void BuyObserveTime()
        {
            StartCoroutine(BuyInternal(
                0, "망원경 업그레이드 1",
                "망원경을 <color=red>더 오래 작동</color>시킬 수 있어요.\n만나는 천체들과의 <color=red>친밀도를 더 많이 쌓을 기회</color>가 될 수도?",
                (x) => ((x < 60 ? "" : (x / 60).ToString() + "시간") + (x % 60 > 0 ? " " + (x % 60).ToString() + "분" : ""))));
        }

        public void BuyObserveMore()
        {
            StartCoroutine(BuyInternal(
                1, "망원경 업그레이드 2",
                "<color=red>관측 범위 내의 여러 천체</color>들을 한꺼번에 만날 수 있어요.\n다양한 천체들과의 <color=red>친밀도를 빠르게 쌓을 기회</color>가 될 수도?",
                (x) => ("최대 " + x.ToString() + "명")));
        }

        public void BuyLobbyStars()
        {
            StartCoroutine(BuyInternal(
                2, "로비 업그레이드",
                "집안을 넓게 치우면 더 많은 친구들이 머물 수 있어요.",
                (x) => ("최대 " + x.ToString() + "명")));
        }

        IEnumerator BuyInternal(int index, string title, string desc, System.Func<int, string> valueToString)
        {
            bool res = false;
            yield return InfoPopup.Show(title, desc, index, valueToString, (r) => { res = r; });
            if(res == true)
            {
                Variables.Starlight -= Variables.StoreUpgradeMoney[index][Variables.StoreUpgradeLevel[index]];
                Variables.StoreUpgradeLevel[index]++;
            }

            DisplayAtButton();
        }
    }
}