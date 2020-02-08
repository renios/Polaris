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
					break;
				case 6:
					tutTextPanel1.SetActive(false);
					helpPanel.SetActive(true);
					break;
				case 7:
					tutTextPanel2.SetActive(true);
					break;
				case 8:
					// Scene change to Dialogue Scene
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