using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AlbumCharInfo : MonoBehaviour
{
    [Header("Character/Card Status")]
    public Image ShortImage;
    public Image ConstelImage;
    public Text Name, Subname, ConstelName;
    public Slider RarityBar;

    [Header("Character Informations")]
    public Text Lux;
    public Text Distance, CharDescription;

    [Header("Story Informations")]
    public Text FavorityLevel;
    public Text RequiredFavority;
    public GameObject[] StoryElement;

    public void Show(int charIndex, int cardIndex)
    {
        var character = Variables.Characters[charIndex];
        // TODO: Action for ShortImage & ConstelImage & ConstelName
        Name.text = character.Name;
        Subname.text = character.Cards[cardIndex].Subname;
        RarityBar.value = character.Cards[cardIndex].Rarity;
        Lux.text = character.Lux;
        Distance.text = character.LYDistance;
        CharDescription.text = character.Description;
        FavorityLevel.text = character.Cards[cardIndex].StoryProgress.ToString();
        if (character.Cards[cardIndex].StoryProgress >= character.Cards[cardIndex].Rarity)
            RequiredFavority.text = "-";
        else
            RequiredFavority.text = (Variables.FavorityThreshold[character.Cards[cardIndex].StoryProgress] - character.Cards[cardIndex].Favority).ToString();

        int maxAvailable = 1;
        for (; maxAvailable <= 5; maxAvailable++)
        {
            if (Variables.Characters[charIndex].Cards[cardIndex].Favority < Variables.FavorityThreshold[maxAvailable - 1])
                break;
        }
        for (int i = 0; i < 5; i++)
        {
            if (i < maxAvailable)
            {
                StoryElement[i].GetComponent<AlbumStoryElement>().Show(charIndex, cardIndex, i);
                StoryElement[i].SetActive(true);
            }
            else if (i < character.Cards[cardIndex].Rarity)
            {
                StoryElement[i].GetComponent<AlbumStoryElement>().Mask.SetActive(true);
                StoryElement[i].SetActive(true);
            }
            else
                StoryElement[i].SetActive(false);
        }
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}