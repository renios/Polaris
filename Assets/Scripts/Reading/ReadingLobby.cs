using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Reading
{
	public class ReadingLobby : MonoBehaviour
	{
		public Image selectedCharImg;
		public Text selectedCharName;
		public ReadingCharPicker charPicker;
        public GameObject tutorialObj;

        public Button changeCharBtn, diaryBtn;

		int selectedChar;
		Sprite diaryState;

		// Use this for initialization
		void Start()
		{
			charPicker.LoadCharacter();

			LoadSelectedCharacter(1);

            if (!Variables.TutorialFinished)
                tutorialObj.SetActive(true);
		}

		void Update()
		{
			var curDiarySprite = diaryBtn.image.overrideSprite;
			
			if(curDiarySprite != diaryState && curDiarySprite == diaryBtn.spriteState.pressedSprite)
				changeCharBtn.transform.SetAsFirstSibling();
			else if(curDiarySprite != diaryState)
				diaryBtn.transform.SetAsFirstSibling();

			diaryState = curDiarySprite;
		}

		public void SelectCharacter()
		{
			StartCoroutine(SelectCharacterInternal());
		}

		public IEnumerator SelectCharacterInternal()
		{
			yield return charPicker.Show(LoadSelectedCharacter);
		}

		public void LoadSelectedCharacter(int index)
		{
			selectedChar = index;
			selectedCharImg.sprite = Resources.Load<Sprite>("Characters/" + Variables.Characters[index].InternalName + "/image_album");
			//selectedCharName.text = Variables.Characters[index].Name;
		}

		public void StartReading()
		{
			Dialogue.DialogueManager.DialogRoot = "Characters/" + Variables.Characters[selectedChar].InternalName + "/";
			SceneChanger.Instance.ChangeScene("ReadingIngame");
		}
	}
}