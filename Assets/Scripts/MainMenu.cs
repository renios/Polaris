using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void PushButton1()
    {
        Variables.CameraMove = false;
        SceneChanger.Instance.ChangeScene("MainScene");
    }
    public void PushButton2()
    {
        Variables.CameraMove = true;
        SceneChanger.Instance.ChangeScene("MainScene");
    }
    public void PushButton3()
    {
        SceneChanger.Instance.ChangeScene("GachaScene");
    }
    public void PushButton4()
    {
        SceneChanger.Instance.ChangeScene("AlbumScene");
    }
    public void PushButton5()
    {

    }
}
