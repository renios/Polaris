using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GemManager : MonoBehaviour {

    public static GemManager Instance { get; set; }
    private static int Gem1 = 0;
    //private static int Gem2 = 0;
    private Text GemText1;
    //private Text GemText2;

    private void Awake()
    {
        Instance = this;
        GemText1 = GameObject.Find("Gem1_Text").GetComponent<Text>();
        //GemText2 = GameObject.Find("Gem2_Text").GetComponent<Text>();
        //temp
        ShowState();
    }

    private void Update()
    {
        if(Gem1 != Variables.Starlight)
            ShowState();
    }


    public void SaveState(int num)
    {
        int temp = Gem1 += num;
        if (temp > 99999) Gem1 = 99999;
         //일단 0미만으로 내려가야는 경우엔 안바뀌도록 
        else if (temp < 0) Gem1 = 0;
        ShowState();
        
    }

    private void ShowState()
    {
        //@Gem과 Starlight를 동기화. 이후 수정 필요할 수도
        Gem1 = Variables.Starlight;
        GemText1.text = Gem1.ToString();
       // GemText2.text = Gem2.ToString();
    }

}
