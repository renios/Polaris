using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Album
{
    public class AlbumGroupChecker : MonoBehaviour
    {
        public int GroupIndex;
        public Text PercentText;
        public Slider GageSlider;
        public Text GageText;

        private void Start()
        {
            int allCnt = AlbumManager.Instance.GroupedChar[GroupIndex].Count;
            int activeCnt = 0;
            foreach (var chr in AlbumManager.Instance.GroupedChar[GroupIndex])
                foreach (var card in chr.Value.Cards)
                    if (card.Observed)
                        activeCnt++;
            GageSlider.maxValue = allCnt;
            GageSlider.value = activeCnt;
            GageText.text = activeCnt.ToString() + "/" + allCnt.ToString();
            PercentText.text = (activeCnt * 100f / allCnt).ToString("N0") + "%";
        }
    }
}