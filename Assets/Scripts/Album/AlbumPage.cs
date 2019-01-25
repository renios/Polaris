using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlbumPage : MonoBehaviour
{
    public RectTransform ElementParent;
    public GameObject ElementTemplate;

    public void CreateElement(int charIndex)
    {
        var newObj = Instantiate(ElementTemplate);
        var newElement = newObj.GetComponent<AlbumPageElement>();


    }

    public void CreateElement(int charIndex, int cardIndex)
    {
        var newObj = Instantiate(ElementTemplate);
        var newElement = newObj.GetComponent<AlbumPageElement>();
        newElement.CharIndex = charIndex;
        newElement.CardIndex = cardIndex;

        var cardData = Variables.Characters[charIndex].Cards[cardIndex];
        if (cardData.Observed)
        {
            newElement.Name.text = Variables.Characters[charIndex].Name;
            newElement.Subname.text = cardData.Subname;
            newElement.RarityBar.value = cardData.Rarity;
        }
        else
            newElement.MaskObject.SetActive(true);
        newObj.transform.SetParent(ElementParent);
        newObj.SetActive(true);
    }
}