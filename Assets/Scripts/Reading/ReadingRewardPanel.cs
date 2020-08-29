using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Reading
{
    public class ReadingRewardPanel : MonoBehaviour
    {
        public Image charImage;
        public Text constelName, charName;
        public Slider favBar;
        public Text favLevelText, favProgressText, favIncreaseText;
        public Text pieceIncreaseText;

        public void Show()
        {
            var character = Variables.Characters[SaveData.Now.lastReadingChar];
            var rank = SaveData.Now.lastReadingRank;

            charImage.sprite = Resources.Load<Sprite>("Characters/" + character.InternalName + "/image_albumbutton");
            constelName.text = character.ConstelWeight[0] >= 1 ? Variables.Constels[character.ConstelKey[0]].Name : "알 수 없음";
            charName.text = character.Name;

            var favDelta = rank;
            character.Favority += favDelta;

            int favLevel, favCur, favNext;
            favLevel = GameManager.Instance.CheckFavority(SaveData.Now.lastReadingChar, out favCur, out favNext);
            favBar.maxValue = favNext;
            favBar.value = favCur;
            favLevelText.text = favLevel.ToString();
            favProgressText.text = favCur + "/" + favNext;
            favIncreaseText.text = "+" + favDelta;

            var pieceDelta = rank;
            GameManager.Instance.IncreaseMoney(MoneyType.MemorialPiece, pieceDelta);
            pieceIncreaseText.text = "+" + pieceDelta;

            SaveData.Now.hasReadingResult = false;
            GameManager.Instance.SaveGame();
            gameObject.SetActive(true);
        }
    }

}