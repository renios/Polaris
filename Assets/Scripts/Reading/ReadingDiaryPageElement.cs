using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Reading
{
    public class ReadingDiaryPageElement : MonoBehaviour
    {
        public GameObject observedPanel, notObservedPanel;
        public Image thumbImage;
        public Text nameText;
        public ReadingDiary diary;

        int index;
        
        public void Set(int charIndex)
        {
            var character = Variables.Characters[charIndex];
            if (!character.Observed)
                notObservedPanel.SetActive(true);
            else
            {
                observedPanel.SetActive(true);
                thumbImage.sprite = Resources.Load<Sprite>("Characters/" + character.InternalName + "/image_albumbutton");
                nameText.text = character.Name;
            }

            index = charIndex;
        }

        public void Pressed()
        {
            diary.ShowDetail(index);
        }
    }
}