using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;
using LitJson;

namespace Album
{
    public class AlbumManager : MonoBehaviour
    {
        public static AlbumManager Instance { get; private set; }

        public GameObject PageParent;
        public AlbumCharInfo CharPopup;
        public HorizontalScrollSnap AlbumSnap;
        public Text PageNumber;

        private readonly int maxPageElement = 9;

        private void Awake()
        {
            Instance = this;

            // Debug and test only
            //LoadCharacter();

            ConstructAlbum();
        }

        // 앨범 씬 테스트를 위한 함수
        public void LoadCharacter()
        {
            var raw = Resources.Load<TextAsset>("Data/Characters");
            var charGroup = JsonMapper.ToObject<CharacterDataGroup>(raw.text);
            var constelRaw = Resources.Load<TextAsset>("Data/Constels");
            var constelGroup = JsonMapper.ToObject(constelRaw.text);

            Variables.Characters = new Dictionary<int, CharacterData>();
            foreach (CharacterDataCore data in charGroup.Characters)
            {
                Variables.Characters.Add(data.CharNumber, data);
            }

            Variables.Constels = new Dictionary<string, ConstelData>();
            foreach (JsonData data in constelGroup["constels"])
            {
                var newConstel = new ConstelData((string)data["key"], (string)data["name"]);
                Variables.Constels.Add(newConstel.InternalName, newConstel);
            }
        }

        public void ConstructAlbum()
        {
            int iCnt = 0;
            AlbumPage curPage = null;
            foreach (KeyValuePair<int, CharacterData> chr in Variables.Characters)
            {
                for (int i = 0; i < chr.Value.Cards.Count; i++)
                {
                    if (iCnt % maxPageElement == 0)
                    {
                        var newObj = Instantiate(Resources.Load<GameObject>("Prefabs/AlbumGridPage"));
                        curPage = newObj.GetComponent<AlbumPage>();
                        newObj.transform.SetParent(PageParent.transform);
                        newObj.transform.localScale = Vector3.one;
                        newObj.SetActive(true);
                    }
                    curPage.CreateElement(chr.Key, i);
                    iCnt++;
                }
            }
            ChangePageIndex();
        }

        public void ChangePageIndex()
        {
            PageNumber.text = (AlbumSnap.CurrentPage + 1).ToString() + " / " + PageParent.transform.childCount.ToString();
            SoundManager.Play(SoundType.AlbumPage);
        }

        // Use this for initialization
        void Start()
        {
            SoundManager.Play(SoundType.BgmMain);
        }

        // Update is called once per frame
        void Update()
        {
            //TODO : 씬 바꾸는 임시 코드 개선
            if (Input.GetKeyDown(KeyCode.Escape) && Variables.isTutorialFinished)
            {
                SceneChanger.Instance.ChangeScene("MainScene");
            }
        }
    }
}