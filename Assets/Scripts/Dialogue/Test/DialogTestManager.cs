using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Dialogue.Test
{
	public class DialogTestManager : MonoBehaviour
	{
		public InputField rootFolderInput;
		public InputField fileNameInput;
		public InputField defaultTalkerInput;

		public void RunDialogue()
		{
			DialogueManager.DialogRoot = rootFolderInput.text;
			DialogueManager.DialogFilePath = rootFolderInput.text + fileNameInput.text;
			DialogueManager.DefaultTalkerName = defaultTalkerInput.text;

			Variables.DialogAfterScene = SceneChanger.GetCurrentScene();
			SceneChanger.Instance.ChangeScene("NewDialogScene");
		}
	}
}