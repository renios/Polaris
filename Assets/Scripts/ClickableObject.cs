using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ClickableObject : MonoBehaviour
{
    public UnityEvent Pressed;
    public UnityEvent Clicked;

    public void OnMouseDown()
    {
        Clicked.Invoke();
    }
}