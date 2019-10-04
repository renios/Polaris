using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CanvasScaler))]
public class ScalerFitter : MonoBehaviour
{
    int width, height;

    void Update()
    {
        width = Screen.width;
        height = Screen.height;

        if (width / (float)height > 9f / 16f) // 아이패드의 경우
            GetComponent<CanvasScaler>().matchWidthOrHeight = 1;
        else
            GetComponent<CanvasScaler>().matchWidthOrHeight = 0;
    }
}