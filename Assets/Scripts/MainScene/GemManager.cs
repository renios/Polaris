using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GemManager : MonoBehaviour {

    public static GemManager Instance { get; set; }
    private static int Gem1;
    private static int Gem2;
    public Text GemText1;
    public Text GemText2;

    private void Awake()
    {
        Instance = this;
        //temp
        Gem1 = 1000; Gem2 = 1000;
        ShowState();
    }

    public void SaveState(bool gem, int num)
    {
        if (gem)
        {
            Gem1 += num;
            if (Gem1 > 99999) Gem1 = 99999;
            else if (Gem1 < 0) Gem1 = 0;
            ShowState();
        }
        else
        {
            Gem2 += num;
            if (Gem2 > 99999) Gem2 = 99999;
            else if (Gem2 < 0) Gem2 = 0;
            ShowState();
        }
    }

    private void ShowState()
    {
        GemText1.text = Gem1.ToString();
        GemText2.text = Gem2.ToString();
    }

}
