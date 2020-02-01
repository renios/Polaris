using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using DG.Tweening;

namespace Album
{
    public class AlbumStoryElement : MonoBehaviour
    {
        public Text StoryHeader, StoryDescription;
        public GameObject AddiInfoTemplate;
        public RectTransform AddiInfoParent;
        public int charIndex, storyIndex;
        public GameObject InfoPanel, Mask;

        private List<GameObject> addis = new List<GameObject>();

        public void Show(int chI, int stI)
        {
            while(addis.Count > 0)
            {
                Destroy(addis[0]);
                addis.RemoveAt(0);
            }

            InfoPanel.SetActive(true);
            charIndex = chI;
            storyIndex = stI;
            StoryHeader.text = Variables.Characters[chI].ChapterInfo[stI].Header;
            StoryDescription.text = Variables.Characters[chI].ChapterInfo[stI].Description;
            for(int i = 0; i < Variables.Characters[chI].ChapterInfo[stI].Additional.Length; i++)
            {
                var newObj = Instantiate(AddiInfoTemplate);
                newObj.GetComponentInChildren<Text>().text = Variables.Characters[chI].ChapterInfo[stI].Additional[i];
                newObj.transform.SetParent(AddiInfoParent);
                newObj.transform.localScale = Vector3.one;
                newObj.transform.localPosition = Vector3.zero;
                newObj.SetActive(true);
                addis.Add(newObj);
            }
            Mask.SetActive(false);
        }

        public void StartStory()
        {
            Variables.DialogAfterScene = SceneChanger.GetCurrentScene();
            Variables.DialogCharIndex = charIndex;
            Variables.DialogChapterIndex = storyIndex;
            SceneChanger.Instance.ChangeScene("NewDialogScene");
        }
    }
}