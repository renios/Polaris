using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Album
{
    public class AlbumCharInfo : MonoBehaviour
    {
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
        }

        public void Show(int charIndex, int cardIndex)
        {
            gameObject.SetActive(true);

            var character = Variables.Characters[charIndex];
            ShortImage.sprite = Resources.Load<Sprite>("Characters/" + character.InternalName + "/" + character.Cards[cardIndex].InternalSubname + "/image_album");
            FullImage.sprite = Resources.Load<Sprite>("Characters/" + character.InternalName + "/" + character.Cards[cardIndex].InternalSubname + "/image_full");
            Name.text = character.Name;
            Subname.text = character.Cards[cardIndex].Subname;
            ConstelName.text = Variables.Constels[character.ConstelKey[0]].Name;
            ConstelImage.sprite = Resources.Load<Sprite>("Constellations/Album/" + character.ConstelKey[0]);
            RarityBar.value = character.Cards[cardIndex].Rarity;
            Lux.text = character.Lux;
            Distance.text = character.LYDistance;
            CharDescription.text = character.Description;

            int curProgress, nextRequired;
            var favorLevel = GameManager.Instance.CheckFavority(character.CharNumber, cardIndex, out curProgress, out nextRequired);
            FavorityLevel.text = favorLevel.ToString();
            if (favorLevel > Variables.FavorityThreshold.Length)
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

            int maxAvailable = 0;
            for (; maxAvailable < 5; maxAvailable++)
            {
                if (Variables.Characters[charIndex].Cards[cardIndex].Favority < Variables.FavorityThreshold[maxAvailable])
                    break;
            }
            for (int i = 0; i < 6; i++)
            {
                if (i <= maxAvailable)
                {
                    StoryElement[i].SetActive(true);
                    StoryElement[i].GetComponent<AlbumStoryElement>().Show(charIndex, cardIndex, i);
                }
                else if (i <= character.Cards[cardIndex].Rarity)
                {
                    StoryElement[i].GetComponent<AlbumStoryElement>().Mask.SetActive(true);
                    StoryElement[i].SetActive(true);
                }
                else
                    StoryElement[i].SetActive(false);
            }
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
            Variables.DialogAfterScene = SceneChanger.GetCurrentScene();
            Variables.DialogCharIndex = StoryElement[curStoryIndex].GetComponent<AlbumStoryElement>().charIndex;
            Variables.DialogCardIndex = StoryElement[curStoryIndex].GetComponent<AlbumStoryElement>().cardIndex;
            Variables.DialogChapterIndex = StoryElement[curStoryIndex].GetComponent<AlbumStoryElement>().storyIndex;
            SceneChanger.Instance.ChangeScene("NewDialogScene");
        }

        
    }
}