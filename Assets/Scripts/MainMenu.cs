using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void PushButton1()
    {
        SceneChanger.Instance.ChangeScene("MainScene", 0);
    }
    public void PushButton2()
    {
        SceneChanger.Instance.ChangeScene("MainScene", 1);
    }
    public void PushButton3()
    {
        SceneChanger.Instance.ChangeScene("GachaScene", 2);
    }
    public void PushButton4()
    {
        SceneChanger.Instance.ChangeScene("AlbumScene", 2);
    }
    public void PushButton5()
    {

    }
}
