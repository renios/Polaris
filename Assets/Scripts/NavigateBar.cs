using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NavigateBar : MonoBehaviour 
{
    public void ChangeScene(string sceneName)
    {
        SceneChanger.Instance.ChangeScene(sceneName);
    }
}