using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using DG.Tweening;

namespace Album
{
    public class AlbumStoryElement : MonoBehaviour, IPointerClickHandler
    {
        public Text StoryHeader, StoryDescription;
        public int charIndex, cardIndex, storyIndex;
        public GameObject Mask;
        public RectTransform FaceBody, StretchedBody;

        private bool isStretched = false;

        public void OnPointerClick(PointerEventData eventData)
        {
            if (!Mask.activeSelf)
            {
                if (isStretched)
                    GetComponent<RectTransform>().DOSizeDelta(FaceBody.sizeDelta, 0.4f);
                else
                    GetComponent<RectTransform>().DOSizeDelta(StretchedBody.sizeDelta, 0.4f);
                isStretched = !isStretched;
            }
        }

        public void Show(int chI, int caI, int stI)
        {
            charIndex = chI;
            cardIndex = caI;
            storyIndex = stI;
            StoryHeader.text = Variables.Characters[chI].Cards[caI].ChapterHeader[stI];
            StoryDescription.text = Variables.Characters[chI].Cards[caI].ChapterDesc[stI];
            Mask.SetActive(false);
            LayoutRebuilder.MarkLayoutForRebuild(StretchedBody);
            LayoutRebuilder.ForceRebuildLayoutImmediate(StretchedBody);
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