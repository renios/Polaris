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
        public GameObject[] AlbumGroupInfo;
        public HorizontalScrollSnap AlbumSnap;
        public Text PageNumber;
        [HideInInspector]
        public List<int> GroupStartPage;
        public Dictionary<int, Dictionary<int, CharacterData>> GroupedChar;

        private readonly int maxPageElement = 9;

        private void Awake()
        {
            Instance = this;

            ConstructAlbum();
        }

        public void ConstructAlbum()
        {
            AlbumPage curPage = null;
            GroupStartPage = new List<int>();

            GroupedChar = new Dictionary<int, Dictionary<int, CharacterData>>();
            foreach(var chr in Variables.Characters)
            {
                if (!GroupedChar.ContainsKey(Variables.Constels[chr.Value.ConstelKey[0]].Group))
                    GroupedChar.Add(Variables.Constels[chr.Value.ConstelKey[0]].Group, new Dictionary<int, CharacterData>());
                GroupedChar[Variables.Constels[chr.Value.ConstelKey[0]].Group].Add(chr.Key, chr.Value);
                Debug.Log("Adding " + chr.Value.Name + " (" + chr.Key + ") to group " + Variables.Constels[chr.Value.ConstelKey[0]].Group);
            }

            for(int i = 0, cnt = 0, groupStart = 0; i < 5; i++, cnt = 0)
            {
                Debug.Log("Entered group " + i.ToString());
                GroupStartPage.Add(groupStart);
                foreach(var chr in GroupedChar[i])
                {
                    Debug.Log("- Looping: Target = " + chr.Value.Name + " (" + chr.Key + "), cnt: " + cnt);
                    for(int j = 0; j < chr.Value.Cards.Count; j++)
                    {
                        if (cnt % maxPageElement == 0)
                        {
                            var newObj = Instantiate(Resources.Load<GameObject>("Prefabs/AlbumGridPage"));
                            curPage = newObj.GetComponent<AlbumPage>();
                            newObj.transform.SetParent(PageParent.transform);
                            newObj.transform.localScale = Vector3.one;
                            newObj.transform.localPosition = Vector3.zero;
                            newObj.SetActive(true);
                            groupStart++;
                        }
                        curPage.CreateElement(chr.Key, j);
                        cnt++;
                    }
                }
            }
            GroupStartPage.Add(int.MaxValue);

            ChangePageIndex();
        }

        public void ChangePageIndex()
        {
            PageNumber.text = (AlbumSnap.CurrentPage + 1).ToString() + " / " + PageParent.transform.childCount.ToString();
            for(int i = 0; i < GroupStartPage.Count - 1; i++)
            {
                if (GroupStartPage[i] <= AlbumSnap.CurrentPage && AlbumSnap.CurrentPage < GroupStartPage[i + 1])
                    AlbumGroupInfo[i].SetActive(true);
                else
                    AlbumGroupInfo[i].SetActive(false);
            }
            SoundManager.Play(SoundType.AlbumPage);
        }

        public void JumpPageIndex(int index)
        {
            AlbumSnap.ChangePage(GroupStartPage[index]);
            ChangePageIndex();
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