using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MoneyDisplayer : MonoBehaviour
{
    public Text StarlightText;

    void Update()
    {
        StarlightText.text = Variables.Starlight.ToString();
    }
}