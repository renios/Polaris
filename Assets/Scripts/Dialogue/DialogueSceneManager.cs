using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LitJson;

namespace Dialogue
{
	public class DialogueSceneManager : MonoBehaviour
	{

		// Use this for initialization
		IEnumerator Start()
		{
            var defaultImage = DialogueManager.DialogRoot + "image_dialogue";
            DialogueData dialog;

            try
            {
                var jsonAsset = Resources.Load<TextAsset>(DialogueManager.DialogFilePath);
                if (jsonAsset == null)
                    jsonAsset = Resources.Load<TextAsset>("Dialogues/ErrorDialog");

                dialog = JsonMapper.ToObject<DialogueData>(jsonAsset.text);
                DialogueManager.Instance.fileType = DialogueFileType.JSON;
            }
            catch 
            { 
                dialog = DialogueParser.ParseFromCSV(DialogueManager.DialogFilePath); 
                DialogueManager.Instance.fileType = DialogueFileType.TEXT; 
            }

            DialogueManager.Instance.Displayer.Talker.text = DialogueManager.DefaultTalkerName;
            DialogueManager.Instance.Displayer.ForeImage.sprite = Resources.Load<Sprite>(defaultImage);
            DialogueManager.Instance.Displayer.ForeImage.preserveAspect = true;

            yield return DialogueManager.Instance.Play(dialog, (r) => { });

            if (Variables.IsDialogAppended)
                SceneChanger.Instance.UnloadAppendedScene("AppendDialogScene", () => { Variables.IsDialogAppended = false; });
            else
                SceneChanger.Instance.ChangeScene(Variables.DialogAfterScene);
        }
	}
}