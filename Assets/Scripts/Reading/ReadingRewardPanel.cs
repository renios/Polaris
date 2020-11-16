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
            var rightCount = SaveData.Now.lastReadingRank;

            charImage.sprite = Resources.Load<Sprite>("Characters/" + character.InternalName + "/image_albumbutton");
            constelName.text = character.ConstelWeight[0] >= 1 ? Variables.Constels[character.ConstelKey[0]].Name : "알 수 없음";
            charName.text = character.Name;

            int favIncrement = 1;
            if(SaveData.Now.lastReadingChar != 1)  // 폴라리스의 호감도가 40 50씩 올라가는 일은 없어야 합니다...
                favIncrement = GameManager.Instance.IncreaseFavority(SaveData.Now.lastReadingChar, rightCount * 10);

            int favLevel, favCur, favNext;
            favLevel = GameManager.Instance.CheckFavority(SaveData.Now.lastReadingChar, out favCur, out favNext);
            favBar.maxValue = favNext;
            favBar.value = favCur;
            favLevelText.text = favLevel.ToString();
            favProgressText.text = favCur + "/" + favNext;
            favIncreaseText.text = "+" + favIncrement;

            if (favLevel < 5 && character.StoryUnlocked <= 3)
                character.StoryUnlocked = favLevel - 1;

            var pieceDelta = 0;
            if (rightCount >= 1)
                pieceDelta++;
            if (rightCount >= 4)
                pieceDelta++;
            GameManager.Instance.IncreaseMoney(MoneyType.MemorialPiece, pieceDelta);
            pieceIncreaseText.text = "+" + pieceDelta;

            SaveData.Now.hasReadingResult = false;
            GameManager.Instance.SaveGame();
            gameObject.SetActive(true);
        }
    }

}