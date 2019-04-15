using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class LobbyManager : MonoBehaviour
{
    private int currentCamera;
    private GameObject sdchara;
    public GameObject popup;
    private float PositionX;
    private float PositionY;


    GameObject pickedCharacter;

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
                if (cardData.Observed && cardData.Observable)
                {
                    string name = chrData.InternalName.Substring(0, 1).ToUpper() + chrData.InternalName.Substring(1);
                    GameObject sd = Resources.Load<GameObject>("Prefabs/" + name);
                    if (sd != null)
                    {
                        var chr = Instantiate(sd);
                        chr.transform.SetParent(sdchara.transform);
                        chr.transform.localScale = new Vector3(0.25f, 0.25f, 1);
                        float PositionX = Random.Range(-0.9f, 0.9f);
                        float PositionY;
                        int floor = Random.Range(0, 3);
                        if (floor == 0) PositionY = -3.25f;
                        else if (floor == 1) PositionY = -1.5f;
                        else PositionY = PositionY = -0.15f;
                        chr.transform.localPosition = new Vector3(PositionX, PositionY, PositionZ);
                        PositionZ += 0.1f;
                    }
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
        // 잡고있으면 움직인다
        // 잡고있을때 떼면 떨어진다
        if (pickedCharacter != null) {
            var mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            if (mousePosition.x < -2.5f) PositionX = -2.5f;
            else if (mousePosition.x > 2.5f) PositionX = 2.5f;
            else PositionX = mousePosition.x;
            if (mousePosition.y > -0.5f) PositionY = -0.5f;
            else if (mousePosition.y < -9.0f) PositionY = -9.0f;
            else PositionY = mousePosition.y;
            pickedCharacter.transform.position = new Vector3(PositionX, PositionY, pickedCharacter.transform.position.z) ;

            if (Input.GetMouseButtonUp(0)) {
                pickedCharacter.GetComponent<Move>().Drop();
                pickedCharacter = null;
            }
        }

        // 안 잡고있을때 누르면 잡는다
        if (Input.GetMouseButtonDown(0)) {
            var mousePosition = (Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition);
            float characterHeight = 0.6f;
            int layermaskValue = (1 << 10) + (1 << 11); //10번 레이어와 11번 레이어를 체크

            var tryPick = Physics2D.OverlapCircle(mousePosition - Vector2.up*characterHeight, characterHeight, layermaskValue);
            if (tryPick != null) {
                pickedCharacter = tryPick.gameObject;
                pickedCharacter.GetComponent<Move>().Pick();
            }
        }

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
        //TODO : 씬 바꾸는 임시 코드 개선
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SceneChanger.Instance.ChangeScene("TitleScene");
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