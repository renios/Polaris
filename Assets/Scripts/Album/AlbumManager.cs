using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI.Extensions;
using LitJson;

public class AlbumManager : MonoBehaviour
{
    public static AlbumManager Instance { get; private set; }

    public GameObject PageParent;
    public AlbumCharInfo CharPopup;

    private readonly int maxPageElement = 6;

    private void Awake()
    {
        Instance = this;

        // Debug and test only
        LoadCharacter();

        ConstructAlbum();
    }

    // 앨범 씬 테스트를 위한 함수
    public void LoadCharacter()
    {
        var raw = Resources.Load<TextAsset>("Data/Characters");
        var charGroup = JsonMapper.ToObject<CharacterDataGroup>(raw.text);

        Variables.Characters = new Dictionary<int, CharacterData>();
        foreach(CharacterDataCore data in charGroup.Characters)
        {
            Variables.Characters.Add(data.CharNumber, data);
        }
    }

    public void ConstructAlbum()
    {
        int iCnt = 0;
        AlbumPage curPage = null;
        foreach(KeyValuePair<int, CharacterData> chr in Variables.Characters)
        {
            for(int i = 0; i < chr.Value.Cards.Count; i++)
            {
                if(iCnt % maxPageElement == 0)
                {
                    var newObj = Instantiate(Resources.Load<GameObject>("Prefabs/Clean Album Page"));
                    curPage = newObj.GetComponent<AlbumPage>();
                    newObj.transform.SetParent(PageParent.transform);
                    newObj.transform.localScale = Vector3.one;
                    newObj.SetActive(true);
                }
                curPage.CreateElement(chr.Key, i);
                iCnt++;
            }
        }
    }

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}