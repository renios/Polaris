using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using LitJson;

namespace Dialogue
{
    public class DialogueManager : MonoBehaviour
    {
        public static DialogueManager Instance { get; private set; }

        public GameObject[] ObjectTemplate;
        public RectTransform ObjectParent;
        public Image StandingImage;
        public Text CharName;

        public DialogueData CurrentDialogue;
        public int CurrentPhase, CurrentIndex;

        private bool isInSelectMode = false;

        private void Awake()
        {
            Instance = this;

            var character = Variables.Characters[Variables.DialogCharIndex].InternalName;
            var card = Variables.Characters[Variables.DialogCharIndex].Cards[Variables.DialogCardIndex].InternalSubname;
            var dialogPath = "Characters/" + character + "/" + card + "/dialog_" + Variables.DialogChapterIndex;
            var dummyDialogPath = "Characters/acher/" + card + "/dialog_" + Variables.DialogChapterIndex; // Dummy 용도로 Acher 사용
            Debug.Log(character + " " + Variables.DialogChapterIndex);

            var jsonAsset = Resources.Load<TextAsset>(dialogPath);

            if (jsonAsset == null) // Dummy 용도로 Acher 사용
                jsonAsset = Resources.Load<TextAsset>(dummyDialogPath);

            CurrentDialogue = JsonMapper.ToObject<DialogueData>(jsonAsset.text);
            CharName.text = Variables.Characters[Variables.DialogCharIndex].Name;

            var imagePath = "Characters/" + character + "/" + card + "/image_dialogue";
            var dummyImagePath = "Characters/Acher/" + card + "/image_dialogue"; // Dummy 용도로 Acher 사용
            var imageSprite = Resources.Load<Sprite>(imagePath);
            if (imageSprite == null)
                imageSprite = Resources.Load<Sprite>(dummyImagePath);

            StandingImage.sprite = Resources.Load<Sprite>("Characters/" + character + "/" + card + "/image_dialogue");
            // TODO: 배경 이미지 로드...? 아니면 배경 이미지 바꾸는 커맨드 JSON에 넣을 것인지...?
        }

        private void Start()
        {
            SoundManager.Play(SoundType.BgmMain);
            if (Variables.Characters[Variables.DialogCharIndex].Cards[Variables.DialogCardIndex].StoryProgress <= Variables.DialogChapterIndex)
            {
                Variables.Characters[Variables.DialogCharIndex].Cards[Variables.DialogCardIndex].StoryProgress = Variables.DialogChapterIndex + 1;
                GameManager.Instance.SaveGame();
            }
            ShowDialogue();
        }

        public void ShowDialogue()
        {
            if(!isInSelectMode)
            {
                if(CurrentIndex >= CurrentDialogue.Dialogues[CurrentPhase].Contents.Length)
                {
                    SceneChanger.Instance.ChangeScene(Variables.DialogAfterScene);
                    return;
                }

                var curDialogData = CurrentDialogue.Dialogues[CurrentPhase].Contents[CurrentIndex];

                if(curDialogData.Type <= -1)
                {
                    if (curDialogData.NextPhase > 0)
                    {
                        CurrentPhase = curDialogData.NextPhase;
                        CurrentIndex = 0;
                        ShowDialogue();
                    }
                    else
                        SceneChanger.Instance.ChangeScene(Variables.DialogAfterScene);
                }
                else
                {
                    var newObject = Instantiate(ObjectTemplate[curDialogData.Type]);
                    var newDialogObj = newObject.GetComponentInChildren<DialogueObject>();

                    newObject.SetActive(true);
                    newDialogObj.Set(curDialogData);
                    newObject.transform.SetParent(ObjectParent);
                    newObject.transform.localScale = Vector3.one;
                    CurrentIndex++;
                    SoundManager.Play(SoundType.ClickDialogue);
                    LayoutRebuilder.MarkLayoutForRebuild(newObject.transform as RectTransform);
                    LayoutRebuilder.ForceRebuildLayoutImmediate(newObject.transform as RectTransform);

                    if (curDialogData.Type == 2)
                        isInSelectMode = true;
                }
            }
        }

        public void ShowDialogue(DialogueContent content)
        {
            var newObject = Instantiate(ObjectTemplate[content.Type]);
            var newDialogObj = newObject.GetComponentInChildren<DialogueObject>();

            newObject.transform.SetParent(ObjectParent);
            newObject.SetActive(true);
            newDialogObj.Set(content);
            newObject.transform.localScale = Vector3.one;
            isInSelectMode = false;
            LayoutRebuilder.MarkLayoutForRebuild(newObject.transform as RectTransform);
            LayoutRebuilder.ForceRebuildLayoutImmediate(newObject.transform as RectTransform);
        }

        public IEnumerator GetButtonInput(DialogueObject dialog, string text, int nextPhase)
        {
            SoundManager.Play(SoundType.ClickNormal);
            yield return dialog.Delete();
            ShowDialogue(new DialogueContent() { Type = 1, DialogText = text });
            if (nextPhase > -1)
            {
                CurrentPhase = nextPhase;
                CurrentIndex = 0;
            }
        }
    }
}