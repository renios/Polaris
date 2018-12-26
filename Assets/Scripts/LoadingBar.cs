using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadingBar : MonoBehaviour
{
    public RectTransform FillBody;

    public void SetValue(float value)
    {
        FillBody.localScale = new Vector3(value / 9f * 10f, 1, 1);
    }
}