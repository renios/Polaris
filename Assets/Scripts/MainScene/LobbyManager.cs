using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.SceneManagement;

public class LobbyManager : MonoBehaviour
{
    private GameObject sdchara;
    private float PositionX;
    private float PositionY;
    public GameObject tutorialObj;
    public CharacterPicker charPicker;

    GameObject pickedCharacter;
    Vector3 pickedPosition;
    float pickedTime;
    bool pickedBalloon;
    List<GameObject> charObjList;

    void Awake()
    {
        charPicker.LoadCharacter();
        
        sdchara = GameObject.Find("Characters").gameObject; 
        
        charObjList = new List<GameObject>();
        PlaceSDCharacters();

        if (!Variables.TutorialFinished)
            tutorialObj.SetActive(true);
    }

    public void SelectCharacter()
    {
        StartCoroutine(SelectCharacter_Routine());
    }

    IEnumerator SelectCharacter_Routine()
    {
        yield return charPicker.Show(Variables.GetStoreValue(2), false, Variables.LobbyCharList, pickResult =>
        {
            Variables.LobbyCharList.Clear();
            foreach (var charIndex in pickResult)
                Variables.LobbyCharList.Add(charIndex);
            GameManager.Instance.SaveGame();
            
            PlaceSDCharacters();
        });
    }

    public void PlaceSDCharacters()
    {
        foreach (var obj in charObjList)
            Destroy(obj);
        charObjList.Clear();

        foreach (var idx in Variables.LobbyCharList)
        {
            var c = Variables.Characters[idx];

            var prefabName = c.InternalName.Substring(0, 1).ToUpper() + c.InternalName.Substring(1);
            var prefab = Resources.Load<GameObject>("Prefabs/" + prefabName);
            if (prefab != null)
            {
                var chr = Instantiate(prefab);
                chr.AddComponent<CharacterStarlight>();
                chr.GetComponent<CharacterStarlight>().CharacterData = idx;
                chr.transform.SetParent(sdchara.transform);
                chr.transform.localScale = new Vector3(0.9f, 0.9f, 1);
                float PositionX = Random.Range(-0.9f, 0.9f);
                float PositionY;
                int floor = Random.Range(0, 2);
                if (floor == 0) 
                    PositionY = -1f;
                else
                    PositionY = 6.5f;
                chr.transform.localPosition = new Vector3(PositionX, PositionY, -0.1f);

                charObjList.Add(chr);
            }
        }
    }

    void Start()
    {
        SoundManager.Play(SoundType.BgmMain);
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log(Variables.Starlight);
        // 잡고있으면 움직인다
        // 잡고있을때 떼면 떨어진다
        if (pickedCharacter != null) {
            CharacterStarlight starlight = pickedCharacter.GetComponent<CharacterStarlight>();
            var mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            if (Vector2.Distance(mousePosition, pickedPosition) >= 0.5f) {
                if (starlight.GetOverpoint())
                {
                    starlight.OnBalloonClicked();
                    pickedCharacter.GetComponent<Move>().Touch();
                    pickedCharacter = null;
                }
                else
                {
                    if (!pickedCharacter.GetComponent<Move>().IsPicked())
                    {
                        pickedCharacter.GetComponent<Move>().Pick();
                    }
                    if (mousePosition.x < -4.8f) PositionX = -4.8f;
                    else if (mousePosition.x > 4.8f) PositionX = 4.8f;
                    else PositionX = mousePosition.x;
                    if (mousePosition.y > 8.5f) PositionY = 8f;
                    else if (mousePosition.y < -8.5f) PositionY = -9.0f;
                    else PositionY = mousePosition.y - 0.5f;
                    pickedCharacter.transform.position = new Vector3(PositionX, PositionY, -9.5f);
                }
            }
            else {
                if (Time.time > pickedTime + 0.4f) {
                    if (starlight.GetOverpoint())
                        starlight.OnBalloonClicked();
                    pickedCharacter.GetComponent<Move>().Touch();
                    pickedCharacter = null;
                }
            }

            if (Input.GetMouseButtonUp(0) && pickedCharacter != null) {
                if (pickedCharacter.GetComponent<Move>().IsPicked()) {
                    pickedCharacter.GetComponent<Move>().Drop();
                }
                else {
                    if (starlight.GetOverpoint())
                        starlight.OnBalloonClicked();
                    pickedCharacter.GetComponent<Move>().Touch();
                }
                pickedCharacter = null;
            }
        }

        // 안 잡고있을때 누르면 잡는다
        if (Input.GetMouseButtonDown(0)) {
            TryPickCharacter();
        }
        
        //TODO : 씬 바꾸는 임시 코드 개선
        if (Input.GetKeyDown(KeyCode.Escape) && SceneManager.GetActiveScene().name != "MainTut")
        {
            SceneChanger.Instance.ChangeScene("TitleScene");
        }
    }

    private void TryPickCharacter()
    {
        var mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        RaycastHit2D hit = Physics2D.Raycast((Vector2)mousePosition, Vector2.zero, 0f);
        if (hit.collider != null && hit.collider.name.Contains("star_balloon"))
        {
            hit.collider.GetComponentInParent<CharacterStarlight>().OnBalloonClicked();
            pickedBalloon = true;
        }

        float characterHeight = 1.2f;
        int layermaskValue = (1 << 10) + (1 << 11); //10번 레이어와 11번 레이어를 체크

        var tryPick = Physics2D.OverlapCircle((Vector2)mousePosition - Vector2.up * characterHeight, characterHeight, layermaskValue);
        if (tryPick != null && tryPick.gameObject.GetComponent<Move>().allowTouch)
        {
            pickedCharacter = tryPick.gameObject;
            pickedPosition = mousePosition;
            pickedTime = Time.time;
        }
    }

    public void ChangeScene(string sceneName)
    {
        TryPickCharacter();
        if (pickedCharacter == null && !pickedBalloon )
        {
            SoundManager.Play(SoundType.ClickImportant);
            SceneChanger.Instance.ChangeScene(sceneName);
        }
        pickedBalloon = false; 
    }

    public void CloseGame()
    {
        GameManager.Instance.SaveGame();
        Application.Quit();
    }
}