using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingPopup : MonoBehaviour {

    public ClickableObject[] clickables;
    public GameObject obj;

    public void show()
    {
        obj = GameObject.Find("Room");
        if(obj!=null) clickables = obj.GetComponentsInChildren<ClickableObject>();
        gameObject.SetActive(true);
        if (clickables == null) return;
        foreach (ClickableObject obj in clickables)
        {
            obj.SetEnable(false);
        }
    }

    public void hide()
    {
        gameObject.SetActive(false);
        if (clickables == null) return;
        foreach (ClickableObject obj in clickables)
        {
            obj.SetEnable(true);
        }
        clickables = null;
    }
}
