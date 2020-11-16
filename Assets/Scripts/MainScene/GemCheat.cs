using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GemCheat : MonoBehaviour 
{

    //증감량
    public int starlight;
    public int piece;

    public void CheatStaright()
    {
        GameManager.Instance.IncreaseMoney(MoneyType.Starlight, starlight);
        GameManager.Instance.SaveGame();
    }

    public void CheatPiece()
    {
        GameManager.Instance.IncreaseMoney(MoneyType.MemorialPiece, piece);
        GameManager.Instance.SaveGame();
    }

}
