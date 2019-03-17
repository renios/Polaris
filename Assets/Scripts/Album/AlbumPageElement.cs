using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Album
{
    public class AlbumPageElement : MonoBehaviour
    {
        public Text Name, Subname, ConstelName;
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
            Name.text = data.Name;
            Subname.text = data.Cards[CardIndex].Subname;
            Thumbnail.sprite = Resources.Load<Sprite>("Characters/" + data.InternalName + "/" + data.Cards[CardIndex].InternalSubname + "/image_albumbutton");
            ConstelName.text = "";
            for (int i = 0; i < data.ConstelKey.Length; i++)
            {
                if (i > 0)
                    ConstelName.text += ", ";
                ConstelName.text += Variables.Constels[data.ConstelKey[i]].Name;
            }
            RarityBar.value = data.Cards[CardIndex].Rarity;

            // Check favority
            var favority = data.Cards[CardIndex].Favority;
            int cnt = 0, clrdFavority = 0;
            for(; cnt < Variables.FavorityThreshold.Length; cnt++)
            {
                if (favority < Variables.FavorityThreshold[cnt])
                    break;
                clrdFavority += Variables.FavorityThreshold[cnt];
            }
            FavorLevel.text = cnt.ToString();
            if (cnt >= Variables.FavorityThreshold.Length)
            {
                FavorityLeft.text = "FULL";
                FavorityGage.maxValue = 1;
                FavorityGage.value = 1;
            }
            else
            {
                FavorityLeft.text = (favority - clrdFavority).ToString() + "/" + Variables.FavorityThreshold[cnt].ToString();
                FavorityGage.maxValue = Variables.FavorityThreshold[cnt];
                FavorityGage.value = favority - clrdFavority;
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