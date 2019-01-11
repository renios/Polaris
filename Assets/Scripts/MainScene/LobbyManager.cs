using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class LobbyManager : MonoBehaviour
{
    private int currentCamera;

    // Use this for initialization
    void Start()
    {
        currentCamera = -1;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void MoveCamera()
    {
        Camera.main.transform.DOMove(new Vector3(0, currentCamera * -5.0119f, -10), 0.75f);
        currentCamera *= -1;
    }

    public void ChangeScene(string sceneName)
    {
        //SceneChanger.ChangeScene(sceneName);
    }
}