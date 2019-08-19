using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GemCheat : MonoBehaviour {

    //증감량
    public int num;

    public void CoinUp()
    {
        GemManager.Instance.SaveState(num);
    }

    public void CoinDown()
    {
        GemManager.Instance.SaveState(-num);
    }

}
