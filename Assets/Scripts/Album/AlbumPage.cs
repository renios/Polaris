using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Album
{
    public class AlbumPage : MonoBehaviour
    {
        public RectTransform ElementParent;
        public GameObject ElementTemplate;

        public void CreateElement(int charIndex, int cardIndex)
        {
            var newObj = Instantiate(ElementTemplate);
            var newElement = newObj.GetComponent<AlbumPageElement>();
            newElement.CharIndex = charIndex;
            newElement.CardIndex = cardIndex;

            var cardData = Variables.Characters[charIndex].Cards[cardIndex];
            if (cardData.Observed)
                newElement.Set(Variables.Characters[charIndex]);
            else
            {
                newObj.GetComponent<UnityEngine.UI.Button>().interactable = false;
                newElement.MaskObject.SetActive(true);
            }
            newObj.transform.SetParent(ElementParent);
            newObj.transform.localScale = Vector3.one;
            newObj.SetActive(true);
        }
    }
}