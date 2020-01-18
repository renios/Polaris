using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GemCheat : MonoBehaviour {

    //증감량
    public int num;

    public void CoinUp()
    {
        Variables.Starlight += num;
        GameManager.Instance.SaveGame();
    }

    public void CoinDown()
    {
        Variables.Starlight -= num;
        GameManager.Instance.SaveGame();
    }

}
