using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingPopup : MonoBehaviour {

    public ClickableObject[] clickables;

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
    }

    public void Awake()
    {
        GameObject obj2 = GameObject.Find("Room");
        if (obj2 != null) clickables = obj2.GetComponentsInChildren<ClickableObject>();
    }

}
