﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Album
{
    public class AlbumCharInfo : MonoBehaviour
    {
        public RectTransform MasterPanel;

        [Header("Character/Card Status")]
        public Image ShortImage;
        public Image ConstelImage;
        public Image FullImage;
        public GameObject FullIllust;
        public Text Name, Subname, ConstelName;
        public Slider RarityBar;

        [Header("Character Informations")]
        public Text Lux;
        public Text Distance, CharDescription;

        [Header("Story Informations")]
        public Text FavorityLevel;
        public Text RequiredFavority;
        public Slider FavorityGage;
        public GameObject[] StoryElement;
        public GameObject StoryMsgBox;

        [Header("Frame Panel")]
        public Scrollbar VerticalBar;
        public GameObject UpArrow, DownArrow;

        public Button[] BasicUI;

        private void Update()
        {
            if (UpArrow.activeSelf && VerticalBar.value >= 1)
                UpArrow.SetActive(false);
            if (!UpArrow.activeSelf && VerticalBar.value < 1)
                UpArrow.SetActive(true);
            if (DownArrow.activeSelf && VerticalBar.value <= 0)
                DownArrow.SetActive(false);
            if (!DownArrow.activeSelf && VerticalBar.value > 0)
                DownArrow.SetActive(true);

            LayoutRebuilder.MarkLayoutForRebuild(MasterPanel);
            LayoutRebuilder.ForceRebuildLayoutImmediate(MasterPanel);
        }

        public void Show(int charIndex)
        {
            AlbumManager.Instance.TutorialObj.GetComponent<Tutorial.TutorialAlbum>().ProceedState();
            gameObject.SetActive(true);

            var character = Variables.Characters[charIndex];
            ShortImage.sprite = Resources.Load<Sprite>("Characters/" + character.InternalName + "/image_album");
            FullImage.sprite = Resources.Load<Sprite>("Characters/" + character.InternalName + "/image_full");
            Name.text = character.Name;
            ConstelName.text = Variables.Constels[character.MainConstel].Name;
            ConstelImage.sprite = Resources.Load<Sprite>("Constellations/Album/" + character.ConstelKey[0]);
            Lux.text = character.LuxText;
            Distance.text = character.LYDistance;
            CharDescription.text = character.Description;

            int curProgress, nextRequired;
            var favorLevel = GameManager.Instance.CheckFavority(character.CharNumber, out curProgress, out nextRequired);
            FavorityLevel.text = favorLevel.ToString();
            if (favorLevel > Variables.values.MaxFavorityLevel)
            {
                RequiredFavority.text = "FULL";
                FavorityGage.maxValue = 1;
                FavorityGage.value = 1;
            }
            else
            {
                RequiredFavority.text = curProgress.ToString() + "/" + nextRequired.ToString();
                FavorityGage.maxValue = nextRequired;
                FavorityGage.value = curProgress;
            }

            UpdateStoryList(charIndex);
            VerticalBar.value = 1;
            LayoutRebuilder.MarkLayoutForRebuild(StoryElement[0].transform.parent as RectTransform);
            LayoutRebuilder.ForceRebuildLayoutImmediate(StoryElement[0].transform.parent as RectTransform);
        }

        public void Hide()
        {
            SoundManager.Play(SoundType.ClickDialogue);
            gameObject.SetActive(false);
        }

        public void ShowFullImage()
        {
            SoundManager.Play(SoundType.ClickNormal);
            FullIllust.gameObject.SetActive(true);
        }

        public void HideFullImage()
        {
            SoundManager.Play(SoundType.ClickDialogue);
            FullIllust.gameObject.SetActive(false);
        }

        public void UpdateStoryList(int charIndex)
        {
            int curProgress, nextRequired;
            var favorLevel = GameManager.Instance.CheckFavority(charIndex, out curProgress, out nextRequired);
            var storyLevel = Variables.Characters[charIndex].StoryUnlocked;
            for (int i = 0; i < 6; i++)
            {
                // 해금된 스토리 레벨일 경우 내용을 그대로 보여줍니다.
                if (i <= storyLevel)
                {
                    StoryElement[i].SetActive(true);
                    StoryElement[i].GetComponent<AlbumStoryElement>().Show(charIndex, i);
                }
                // 스토리 레벨보다 캐릭터 레벨이 높을 때, 최초 한 단락만 구매 창을 띄웁니다.
                else if (charIndex != 1 && i == storyLevel + 1 && storyLevel + 1 < favorLevel) // 폴라리스의 스토리는 기억의 조각으로도 살 수 없어요.
                {
                    StoryElement[i].SetActive(true);
                    StoryElement[i].GetComponent<AlbumStoryElement>().SetBuyable(charIndex, i);
                }
                // 그렇지 않으면 잠급니다.
                else if (i <= Variables.values.MaxFavorityLevel)
                {
                    StoryElement[i].GetComponent<AlbumStoryElement>().Mask.SetActive(true);
                    StoryElement[i].GetComponent<AlbumStoryElement>().InfoPanel.SetActive(false);
                    StoryElement[i].GetComponent<AlbumStoryElement>().BuyButton.SetActive(false);
                    StoryElement[i].SetActive(true);
                }
                else
                    StoryElement[i].SetActive(false);
            }
            LayoutRebuilder.MarkLayoutForRebuild(StoryElement[0].transform.parent as RectTransform);
            LayoutRebuilder.ForceRebuildLayoutImmediate(StoryElement[0].transform.parent as RectTransform);
        }

        int curStoryIndex = -1;

        public void AskToRunStory(int index)
        {
            SoundManager.Play(SoundType.ClickNormal);
            curStoryIndex = index;
            StoryMsgBox.SetActive(true);
            StoryMsgBox.GetComponentInChildren<Text>().text = "'" + StoryElement[index].GetComponent<AlbumStoryElement>().StoryHeader.text + "'\n대화를 다시 보시겠어요?";
        }

        public void RunStory()
        {
            Dialogue.DialogueManager.PrepareCharacterDialog
                (StoryElement[curStoryIndex].GetComponent<AlbumStoryElement>().charIndex,
                StoryElement[curStoryIndex].GetComponent<AlbumStoryElement>().storyIndex);
            Variables.DialogAfterScene = SceneChanger.GetCurrentScene();

            SceneChanger.Instance.ChangeScene("NewDialogScene");
        }
    }
}