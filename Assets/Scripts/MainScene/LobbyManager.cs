using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class LobbyManager : MonoBehaviour
{
    private int currentCamera;
    public GameObject popup;

    void Awake()
    {
        popup = GameObject.Find("Setting").transform.Find("Setting Panel").gameObject;
    }

    void Start()
    {
        SoundManager.Play(SoundType.BgmMain);
        currentCamera = -1;
    }

    // Update is called once per frame
    void Update()
    {
        if (SwipeManager.Instance.IsSwiping(SwipeManager.SwipeDirection.Down))
        {
            if(!popup.activeSelf)
                Camera.main.transform.DOMove(new Vector3(0, 5.0119f, -10), 0.75f);
        }
        else if (SwipeManager.Instance.IsSwiping(SwipeManager.SwipeDirection.Up))
        {
            if(!popup.activeSelf)
                Camera.main.transform.DOMove(new Vector3(0, -5.0119f, -10), 0.75f);
        }
    }

    public void MoveCamera()
    {
        Camera.main.transform.DOMove(new Vector3(0, currentCamera * -5.0119f, -10), 0.75f);
        currentCamera *= -1;
    }

    public void ChangeScene(string sceneName)
    {
        SoundManager.Play(SoundType.ClickImportant);
        SceneChanger.Instance.ChangeScene(sceneName, 2);
    }

}