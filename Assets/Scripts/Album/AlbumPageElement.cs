using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Album
{
    public class AlbumPageElement : MonoBehaviour
    {
        public Image Thumbnail;
        public Slider RarityBar, FavorityGage;
        public Text FavorLevel, FavorityLeft;
        public GameObject MaskObject, NewFlag;

        public int CharIndex, CardIndex;

        public void CheckNewStory()
        {
            int maxAvailable = 1;
            for (; maxAvailable <= 5; maxAvailable++)
            {
                if (Variables.Characters[CharIndex].Cards[CardIndex].Favority < Variables.FavorityThreshold[maxAvailable - 1])
                    break;
            }
            if (maxAvailable > Variables.Characters[CharIndex].Cards[CardIndex].StoryProgress)
                NewFlag.SetActive(true);
            else
                NewFlag.SetActive(false);
        }

        public void Set(CharacterData data)
        {
            Thumbnail.sprite = Resources.Load<Sprite>("Characters/" + data.InternalName + "/" + data.Cards[CardIndex].InternalSubname + "/image_albumbutton");
            RarityBar.value = data.Cards[CardIndex].Rarity;

            int curProgress, nextRequired;
            var favorLevel = GameManager.Instance.CheckFavority(data.CharNumber, CardIndex, out curProgress, out nextRequired);
            FavorLevel.text = favorLevel.ToString();
            if (favorLevel > Variables.Characters[data.CharNumber].Cards[CardIndex].Rarity)
            {
                FavorityLeft.text = "FULL";
                FavorityGage.maxValue = 1;
                FavorityGage.value = 1;
            }
            else
            {
                FavorityLeft.text = curProgress.ToString() + "/" + nextRequired.ToString();
                FavorityGage.maxValue = nextRequired;
                FavorityGage.value = curProgress;
            }

            CheckNewStory();
        }

        public void Clicked()
        {
            SoundManager.Play(SoundType.ClickNormal);
            AlbumManager.Instance.CharPopup.Show(CharIndex, CardIndex);
        }
    }
}