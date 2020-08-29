using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class MoneyDisplayer : MonoBehaviour
{
    public MoneyType type;
    
    Text text;

    void Awake()
    {
        text = GetComponent<Text>();
    }

    void Update()
    {
        text.text = GameManager.Instance.GetCurrentMoney(type).ToString();
    }
}