using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Tutorial
{
	public class TutorialAlbum : MonoBehaviour
	{
		public GameObject tutTextPanel1, tutTextPanel2;
		public GameObject helpPanel;
		public ScrollRect albumSnap;

		void Start()
		{
			ChangeState(Variables.TutorialStep);
		}

		public void ChangeState(int state)
		{
			switch(state)
			{
				case 5:
					tutTextPanel1.SetActive(true);
					albumSnap.horizontal = false;
					tutTextPanel1.GetComponent<Image>().alphaHitTestMinimumThreshold = 0.3f;
					break;
				case 6:
					tutTextPanel1.SetActive(false);
					helpPanel.SetActive(true);
					break;
				case 7:
					tutTextPanel2.SetActive(true);
					//tutTextPanel2.GetComponent<Image>().alphaHitTestMinimumThreshold = 0.3f;
					break;
				case 8:
					// Scene change to Dialogue Scene
					Dialogue.DialogueManager.DialogRoot = "Dialogues/TutorialReading/";
					Dialogue.DialogueManager.DialogFilePath = "Dialogues/TutorialReading/dialog1";
					Variables.DialogAfterScene = "GachaTut2"; // 임시. 나중에 독서 씬으로 바꿔야 함
					SceneChanger.Instance.ChangeScene("NewDialogScene");
					break;
			}
		}

		public void ProceedState()
		{
			if (gameObject.activeSelf)
				ChangeState(++Variables.TutorialStep);
		}
	}
}