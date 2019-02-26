using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ClickableObject : MonoBehaviour
{
    public UnityEvent Pressed;
    public UnityEvent Clicked;
    private bool enable;

    private void Start()
    {
        SetEnable(true);
    }

    public void SetEnable(bool enable)
    {
        this.enable = enable;
    }

    public void OnMouseDown()
    {
        if(enable)
            Clicked.Invoke();
    }
}