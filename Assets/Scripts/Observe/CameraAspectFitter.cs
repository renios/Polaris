using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Observe
{
    public class CameraAspectFitter : MonoBehaviour
    {
        int width, height;

        void Update()
        {
            width = Screen.width;
            height = Screen.height;

            // Base aspect : 1920 x 1080

            if (width / (float)height > 9f / 16f) // 아이패드의 경우
                Camera.main.orthographicSize = 960;
            else
                Camera.main.orthographicSize = 540 * ((float)height / width);
        }
    }
}
