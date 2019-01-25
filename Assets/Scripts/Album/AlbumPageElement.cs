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
        int maxAvailable = 0;
        for(int fullThr = 0; maxAvailable < 5; maxAvailable++)
        {
            fullThr += Variables.FavorityThreshold[maxAvailable];
            if (Variables.Characters[CharIndex].Cards[CardIndex].Favority < fullThr)
                break;
        }
        if (maxAvailable >= Variables.Characters[CharIndex].Cards[CardIndex].StoryProgress)
            NewFlag.SetActive(true);
    }
}