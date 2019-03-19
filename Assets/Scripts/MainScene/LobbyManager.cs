using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class LobbyManager : MonoBehaviour
{
    private int currentCamera;
    private GameObject sdchara;
    public GameObject popup;

    void Awake()
    {
        sdchara = GameObject.Find("Characters").gameObject; 
        popup = GameObject.Find("Setting").transform.Find("Setting Panel").gameObject;
        //다락방 이동 시.
        if (Variables.CameraMove == true)
        {
            Camera.main.transform.position = new Vector3(0, 5.0119f, -10);
        }
        ShowCharacter();
    }

    void ShowCharacter()
    {
        float PositionZ = 0f;
        foreach (KeyValuePair<int, CharacterData> c in Variables.Characters)
        {
            var chrData = Variables.Characters[c.Key];
            for (int i = 0; i < c.Value.Cards.Count; i++)
            {
                var cardData = chrData.Cards[i];
                if (cardData.Observed && (c.Key != 1))
                {
                    string name = chrData.InternalName.Substring(0, 1).ToUpper() + chrData.InternalName.Substring(1);
                    var chr = Instantiate(Resources.Load<GameObject>("Prefabs/"+name));
                    chr.transform.SetParent(sdchara.transform);
                    chr.transform.localScale = new Vector3(0.25f, 0.25f, 1);
                    float PositionX = Random.Range(-1.0f, 1.0f);
                    float PositionY = Random.Range(-3.5f, 0.2f);
                    chr.transform.localPosition = new Vector3(PositionX, PositionY, PositionZ);
                    PositionZ += 0.1f;
                }
            }
        }
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
        SceneChanger.Instance.ChangeScene(sceneName);
    }

}