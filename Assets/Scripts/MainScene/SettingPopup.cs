using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingPopup : MonoBehaviour {

    public ClickableObject[] clickables;
    public Button[] buttons;

    public void Show()
    {
        gameObject.SetActive(true);
        if (clickables != null)
        {
            foreach (ClickableObject obj in clickables)
            {
                obj.SetEnable(false);
            }
        }
        if(buttons != null)
        {
            foreach (Button obj in buttons)
            {
                obj.interactable = false;
            }
        }
    }

    public void Hide()
    {
        gameObject.SetActive(false);
        if (clickables != null)
        {
            foreach (ClickableObject obj in clickables)
            {
                obj.SetEnable(true);
            }
        }
        if (buttons != null)
        {
            foreach (Button obj in buttons)
            {
                obj.interactable = true;
            }
        }
    }

    public void Awake()
    {
        GameObject obj = GameObject.Find("Main UI Canvas").transform.Find("Navigate Panel").gameObject;
        if (obj != null) buttons = obj.GetComponentsInChildren<Button>();
        GameObject obj2 = GameObject.Find("Room");
        if (obj2 != null) clickables = obj2.GetComponentsInChildren<ClickableObject>();
    }

}
