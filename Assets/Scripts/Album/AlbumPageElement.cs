using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AlbumPageElement : MonoBehaviour
{
    public Text Name, Subname;
    public Image Thumbnail;
    public Slider RarityBar;
    public GameObject MaskObject, NewFlag;

    public int CharIndex, CardIndex;

    public void CheckNewStory()
    {
        int maxAvailable = 1;
        for(; maxAvailable <= 5; maxAvailable++)
        {
            if (Variables.Characters[CharIndex].Cards[CardIndex].Favority < Variables.FavorityThreshold[maxAvailable - 1])
                break;
        }
        if (maxAvailable >= Variables.Characters[CharIndex].Cards[CardIndex].StoryProgress)
            NewFlag.SetActive(true);
    }

    public void Clicked()
    {
        AlbumManager.Instance.CharPopup.Show(CharIndex, CardIndex);
    }
}