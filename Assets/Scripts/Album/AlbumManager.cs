using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LitJson;

public class AlbumManager : MonoBehaviour
{
    public GameObject PageTemplate;

    private readonly int maxPageElement = 6;

    private void Awake()
    {
        // Debug and test only
        LoadCharacter();

    }

    // 앨범 씬 테스트를 위한 함수
    public void LoadCharacter()
    {
        var raw = Resources.Load<TextAsset>("Data/Characters");
        var charGroup = JsonMapper.ToObject<CharacterDataGroup>(raw.text);

        Variables.Characters = new Dictionary<int, CharacterData>();
        foreach(CharacterData data in charGroup.Characters)
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
                    var newObj = Instantiate(PageTemplate);
                    curPage = newObj.GetComponent<AlbumPage>();
                    newObj.transform.SetParent(PageTemplate.transform.parent);
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