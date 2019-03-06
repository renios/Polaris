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
        public int charIndex, cardIndex, storyIndex;
        public GameObject InfoPanel, Mask;

        private bool isStretched = false;

        public void Show(int chI, int caI, int stI)
        {
            InfoPanel.SetActive(true);
            charIndex = chI;
            cardIndex = caI;
            storyIndex = stI;
            StoryHeader.text = Variables.Characters[chI].Cards[caI].ChapterHeader[stI];
            StoryDescription.text = Variables.Characters[chI].Cards[caI].ChapterDesc[stI];
            Mask.SetActive(false);
        }

        public void StartStory()
        {
            Variables.DialogAfterScene = SceneChanger.GetCurrentScene();
            Variables.DialogCharIndex = charIndex;
            Variables.DialogCardIndex = cardIndex;
            Variables.DialogChapterIndex = storyIndex;
            SceneChanger.Instance.ChangeScene("NewDialogScene", 2);
        }
    }
}