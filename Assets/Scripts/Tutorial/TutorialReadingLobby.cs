using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Tutorial
{
	public class TutorialReadingLobby : MonoBehaviour
	{
		public GameObject popupPanel, dimmerFull, dimmer1, dimmer2, dimmer3;
		public GameObject tutMsgPanel, tutIndicateObj1, tutIndicateObj2, tutIndicateObj3;
		public Text tutText;

		bool allowMoveState = false;

		// Use this for initialization
		void Start()
		{
			ChangeState(Variables.TutorialStep);
		}

		public void ChangeState(int state)
		{
			switch(state)
			{
				case 8: // INIT OF READING LOBBY
					tutMsgPanel.SetActive(true);
					tutText.text = "여기에서는 원하는 별과 같이 책을 읽을 수 있는 것 같아.";
					dimmerFull.SetActive(true);
					break;
				case 9:
					tutText.text = "지금은 폴라리스와 같이 책을 읽어 보자.";
					tutIndicateObj1.SetActive(true);
					dimmerFull.SetActive(false);
					dimmer1.SetActive(true);
					break;
				case 11:
					popupPanel.SetActive(true);
					break;
			}
		}

		// Update is called once per frame
		void Update()
		{
			switch(Variables.TutorialStep)
			{
				case 8:
					if (Input.GetMouseButtonDown(0))
						ChangeState(++Variables.TutorialStep);
					break;
				case 9:
					if (allowMoveState)
						SceneChanger.Instance.ChangeScene("ReadingIngame");
					break;
				case 11:
					if (allowMoveState)
						SceneChanger.Instance.ChangeScene("GachaTut2");
					break;
			}
		}

		public void MoveState()
		{
			if (gameObject.activeSelf)
				allowMoveState = true;
		}
	}
}