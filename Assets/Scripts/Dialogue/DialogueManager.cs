using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using LitJson;

namespace Dialogue
{
    public enum DialogueFileType { JSON, TEXT }

    public class DialogueManager : MonoBehaviour
    {
        public static DialogueManager Instance { get; private set; }

        public static string DialogRoot { get; set; }
        public static string DialogFilePath { get; set; }
        public static string DefaultTalkerName { get; set; }
        public static string ImageRootPath { get { return DialogRoot + "DialogueImage/"; } }

        public DialogueDisplayer Displayer;
        public DialogueInteractor Interactor;
        public DialogueInput Receiver;

        public DialogueData CurrentDialogue;

        public Image nameTag;
        public GameObject tutorialObj;

        private DialogueFileType fileType;

        public static void PrepareCharacterDialog(int charIndex, int chapter)
        {
            DialogRoot = Variables.GetCharacterRootFolder(charIndex);
            DialogFilePath = Variables.GetCharacterRootFolder(charIndex) + "dialog_" + chapter;
            DefaultTalkerName = Variables.Characters[charIndex].Name;

            if (Variables.Characters[charIndex].StoryProgress <= chapter)
            {
                Variables.Characters[charIndex].StoryProgress = chapter + 1;
                GameManager.Instance.SaveGame();
            }
        }

        private void Awake()
        {
            Instance = this;

            var defaultImage = DialogRoot + "image_dialogue";

            try
            {
                var jsonAsset = Resources.Load<TextAsset>(DialogFilePath);
                if (jsonAsset == null)
                    jsonAsset = Resources.Load<TextAsset>("Dialogues/ErrorDialog");

                CurrentDialogue = JsonMapper.ToObject<DialogueData>(jsonAsset.text);
                fileType = DialogueFileType.JSON;
            }
            catch { CurrentDialogue = DialogueParser.ParseFromCSV(DialogFilePath); fileType = DialogueFileType.TEXT; }

            Displayer.Talker.text = DefaultTalkerName;
            Displayer.ForeImage.sprite = Resources.Load<Sprite>(defaultImage);
            Displayer.ForeImage.preserveAspect = true;
        }

        private IEnumerator Start()
        {
            int finalPhase = 0;
            yield return PlayDialogue(CurrentDialogue, r => finalPhase = r);
            Debug.Log("Ended. Final phase: " + finalPhase);

            if (Variables.IsDialogAppended)
                SceneChanger.Instance.UnloadAppendedScene("AppendDialogScene", () => { Variables.IsDialogAppended = false; });
            else
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
                        if (fileType == DialogueFileType.TEXT && dialog.Talker == null)
                            nameTag.enabled = false;
                        else
                        {
                            nameTag.enabled = true;
                            nameTag.sprite = Resources.Load<Sprite>("Images/dialogue_nametag");
                        }
                        yield return ShowText(fileType == DialogueFileType.JSON ? DefaultTalkerName : dialog.Talker, dialog.DialogText);
                        break;
                    case 1:
                        nameTag.enabled = true;
                        nameTag.sprite = Resources.Load<Sprite>("Images/dialogue_nametag2");
                        yield return ShowText("나", dialog.DialogText);
                        break;
                    case 2:
                        if (!Variables.TutorialFinished && Variables.TutorialStep == 5)
                            tutorialObj.SetActive(true);
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