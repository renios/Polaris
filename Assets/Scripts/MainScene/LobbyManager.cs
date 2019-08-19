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
    Vector3 pickedPosition;
    float pickedTime;

    void Awake()
    {
        sdchara = GameObject.Find("Characters").gameObject; 
        popup = GameObject.Find("Setting").transform.Find("Setting Panel").gameObject;
        /*
        //다락방 이동 시.
        if (Variables.CameraMove == true)
        {
            Camera.main.transform.position = new Vector3(0, 5.0119f, -10);
        }
        */
        ShowCharacter();
    }

    void ShowCharacter()
    {
        float PositionZ = 0f;
        if (Variables.Characters == null) { Debug.Log("세이브 파일이 비정상적임"); return; }
        foreach (KeyValuePair<int, CharacterData> c in Variables.Characters)
        {
            //var chrData = Variables.Characters[c.Key];
            for (int i = 0; i < c.Value.Cards.Count; i++)
            {
                var cardData = c.Value.Cards[i];
                if (cardData.Observed && cardData.Observable)
                {
                    string name = c.Value.InternalName.Substring(0, 1).ToUpper() + c.Value.InternalName.Substring(1);
                    GameObject sd = Resources.Load<GameObject>("Prefabs/" + name);
                    if (sd != null)
                    {
                        var chr = Instantiate(sd);
                        chr.AddComponent<CharacterStarlight>();
                        chr.GetComponent<CharacterStarlight>().CharacterData = new int[2] {c.Key, i};
                        chr.transform.SetParent(sdchara.transform);
                        chr.transform.localScale = new Vector3(0.25f, 0.25f, 1);
                        float PositionX = Random.Range(-0.9f, 0.9f);
                        float PositionY;
                        int floor = Random.Range(0, 3);
                        if (floor == 0) PositionY = -2.0f;
                        else if (floor == 1) PositionY = -0.2f;
                        else PositionY = PositionY = 1.35f;
                        chr.transform.localPosition = new Vector3(PositionX, PositionY, PositionZ);
                        PositionZ -= 0.1f;
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
        Debug.Log(Variables.Starlight);
        // 잡고있으면 움직인다
        // 잡고있을때 떼면 떨어진다
        if (pickedCharacter != null) {
            var mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            if (Vector2.Distance(mousePosition, pickedPosition) >= 0.25f) {
                if (!pickedCharacter.GetComponent<Move>().IsPicked()) {
                    pickedCharacter.GetComponent<Move>().Pick();
                }
                if (mousePosition.x < -2.5f) PositionX = -2.5f;
                else if (mousePosition.x > 2.5f) PositionX = 2.5f;
                else PositionX = mousePosition.x;
                if (mousePosition.y > -0.5f) PositionY = -0.5f;
                else if (mousePosition.y < -9.0f) PositionY = -9.0f;
                else PositionY = mousePosition.y;
                pickedCharacter.transform.position = new Vector3(PositionX, PositionY, pickedCharacter.transform.position.z) ;
            }
            else {
                if (Time.time > pickedTime + 0.4f) {
                    pickedCharacter.GetComponent<Move>().Touch();
                    pickedCharacter = null;
                }
            }


            if (Input.GetMouseButtonUp(0) && pickedCharacter != null) {
                if (pickedCharacter.GetComponent<Move>().IsPicked()) {
                    pickedCharacter.GetComponent<Move>().Drop();
                }
                else {
                    pickedCharacter.GetComponent<Move>().Touch();
                }
                pickedCharacter = null;
            }
        }

        // 안 잡고있을때 누르면 잡는다
        if (Input.GetMouseButtonDown(0)) {
            var mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            RaycastHit2D hit = Physics2D.Raycast((Vector2)mousePosition, Vector2.zero, 0f);
            if(hit.collider != null && hit.collider.name.Contains("star_balloon"))
                hit.collider.GetComponentInParent<CharacterStarlight>().OnBalloonClicked();

            float characterHeight = 0.6f;
            int layermaskValue = (1 << 10) + (1 << 11); //10번 레이어와 11번 레이어를 체크

            var tryPick = Physics2D.OverlapCircle((Vector2)mousePosition - Vector2.up * characterHeight, characterHeight, layermaskValue);
            if (tryPick != null)
            {
                pickedCharacter = tryPick.gameObject;
                pickedPosition = mousePosition;
                pickedTime = Time.time;
            }
        }
        
        /*

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

        */

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