using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MessageSet : MonoBehaviour 
{
    public static MessageSet Now { get; private set; }
    
    [Header("Money Spend Ask")]
    public GameObject moneySpendAskPanel;
    public Text targetLabel;
    public Image moneyTypeImage;
    public Text moneyLabel;
    public Sprite[] moneyTypeSprites;
    [Header("No Money Alert")]
    public GameObject noMoneyAlertPanel;
    public Text noMoneyLabel;

    int result; // Normally, 0 is set to True
    bool hasResult;

    void Awake()
    {
        Now = this;
    }

    public IEnumerator ShowMoneySpendAsk(string text, MoneyType type, int cost, Action<bool> afterResult)
    {
        hasResult = false;

        string moneyTypeString = "";
        switch (type)
        {
            case MoneyType.Starlight:
                moneyTypeString = "별빛을 사용하여";
                break;
            case MoneyType.MemorialPiece:
                moneyTypeString = "기억의 조각을 사용하여";
                break;
        }

        targetLabel.text = moneyTypeString + Environment.NewLine + text;
        moneyTypeImage.sprite = moneyTypeSprites[(int) type];
        moneyLabel.text = cost.ToString();
        moneySpendAskPanel.SetActive(true);
        yield return new WaitUntil(() => hasResult);
        moneySpendAskPanel.SetActive(false);

        var boolres = result == 0 ? true : false;
        afterResult(boolres);
    }

    public IEnumerator ShowNoMoneyAlert(MoneyType type)
    {
        hasResult = false;

        switch (type)
        {
            case MoneyType.Starlight:
                noMoneyLabel.text = "별빛이 부족합니다!";
                break;
            case MoneyType.MemorialPiece:
                noMoneyLabel.text = "기억의 조각이 부족합니다!";
                break;
        }
        noMoneyAlertPanel.SetActive(true);
        yield return new WaitUntil(() => hasResult);
        noMoneyAlertPanel.SetActive(false);
    }

    public void SetResult(int index)
    {
        hasResult = true;
        result = index;
    }
}
