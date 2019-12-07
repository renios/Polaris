using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraAspectFitter : MonoBehaviour
{
    public float size;
    int width, height;

    // Use this for initialization
    void Start()
    {
        Fit();
    }

    // Update is called once per frame
    void Update()
    {
        Fit();
    }

    void Fit()
    {
        width = Screen.width;
        height = Screen.height;

        // Base aspect : 1920 x 1080

        if (width / (float)height > 9f / 16f) // 아이패드의 경우
            Camera.main.orthographicSize = size;
        else
            Camera.main.orthographicSize = (size * (9f / 16f)) * ((float)height / width);
    }
}