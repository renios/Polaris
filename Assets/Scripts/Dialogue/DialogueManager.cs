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
        public static string ImageRootPath { get; private set; }

        public DialogueDisplayer Displayer;
        public DialogueInteractor Interactor;
        public DialogueInput Receiver;

        public DialogueData CurrentDialogue;

        public string talkerNPC;

        private void Awake()
        {
            Instance = this;

            var character = Variables.Characters[Variables.DialogCharIndex].InternalName;
            var card = Variables.Characters[Variables.DialogCharIndex].Cards[Variables.DialogCardIndex].InternalSubname;
            var dialogPath = "Characters/" + character + "/" + card + "/dialog_" + Variables.DialogChapterIndex;
            var imagePath = "Characters/" + character + "/" + card + "/image_dialogue";
            ImageRootPath = "Characters/" + character + "/" + card + "/DialogueImage/";
            var dummyDialogPath = "Characters/acher/" + card + "/dialog_" + Variables.DialogChapterIndex; // Dummy 용도로 Acher 사용

            Debug.Log(character + " " + Variables.DialogChapterIndex);
            try
            {
                var jsonAsset = Resources.Load<TextAsset>(dialogPath);
                if (jsonAsset == null)
                    jsonAsset = Resources.Load<TextAsset>(dummyDialogPath);

                CurrentDialogue = JsonMapper.ToObject<DialogueData>(jsonAsset.text);
            }
            catch { CurrentDialogue = DialogueParser.ParseFromCSV(dialogPath); }

            Displayer.Talker.text = Variables.Characters[Variables.DialogCharIndex].Name;
            Displayer.ForeImage.sprite = Resources.Load<Sprite>(imagePath);
            Displayer.ForeImage.preserveAspect = true;

            talkerNPC = Displayer.Talker.text; Debug.Log(talkerNPC);
        }

        private IEnumerator Start()
        {
            if (Variables.Characters[Variables.DialogCharIndex].Cards[Variables.DialogCardIndex].StoryProgress <= Variables.DialogChapterIndex)
            {
                Variables.Characters[Variables.DialogCharIndex].Cards[Variables.DialogCardIndex].StoryProgress = Variables.DialogChapterIndex + 1;
                GameManager.Instance.SaveGame();
            }

            int finalPhase = 0;
            yield return PlayDialogue(CurrentDialogue, r => finalPhase = r);
            Debug.Log("Ended. Final phase: " + finalPhase);
            SceneChanger.Instance.ChangeScene(Variables.DialogAfterScene);
        }

        public void Play(DialogueData data)
        {
            int finalPhase;
            StartCoroutine(PlayDialogue(data, r => finalPhase = r));
        }

        IEnumerator PlayDialogue(DialogueData data, System.Action<int> result)
        {
            Displayer.ClearAll();

            var dicPhase = new Dictionary<int, DialoguePhase>();
            foreach(var phase in data.Dialogues)
                dicPhase.Add(phase.Phase, phase);

            int curPhase = 0;
            for(int i = 0; i < dicPhase[curPhase].Contents.Length; i++)
            {
                var dialog = dicPhase[curPhase].Contents[i];
                switch(dialog.Type)
                {
                    case 0:
                        yield return ShowText(talkerNPC/*dialog.Talker*/, dialog.DialogText);
                        break;
                    case 1:
                        yield return ShowText("주인공"/*dialog.Talker*/, dialog.DialogText);
                        break;
                    case 2:
                        yield return ShowInteraction(dialog.JuncTexts, dialog.Directions);
                        if (dialog.Directions[Interactor.Result] > -1)
                        {
                            curPhase = dialog.Directions[Interactor.Result];
                            i = -1;
                        }
                        break;
                    case 10:
                        Displayer.DisplayForeImage(dialog.ImageKey);
                        break;
                    case 11:
                        SoundManager.Play(dialog.BgmKey);
                        break;
                    case -1:
                        if (dialog.NextPhase == -1)
                            result(curPhase);
                        else
                        {
                            curPhase = dialog.NextPhase;
                            i = -1;
                        }
                        break;
                    default:
                        Debug.LogError("Oops, unknown type.");
                        break;
                }
            }
        }

        IEnumerator ShowText(string talker, string text)
        {
            yield return Displayer.DisplayText(talker, text);
        }

        IEnumerator ShowInteraction(string[] texts, int[] nexts)
        {
            Interactor.gameObject.SetActive(true);
            yield return Interactor.Show(texts, nexts);
            Interactor.gameObject.SetActive(false);
        }
    }
}