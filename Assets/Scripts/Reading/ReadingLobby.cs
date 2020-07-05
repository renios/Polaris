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
		public CharacterPicker picker;
		public ReadingDiary diary;
		public ReadingRewardPanel rewardPanel;
        public GameObject tutorialObj;

        public Button changeCharBtn, diaryBtn;

		int selectedChar;
		Sprite diaryState;

		// Use this for initialization
		void Start()
		{
			picker.LoadCharacter(true);
			diary.ConstructDiary();

			LoadSelectedCharacter(new List<int>{SaveData.Now.lastObservedChar});

            if (!Variables.TutorialFinished)
                tutorialObj.SetActive(true);

            if (SaveData.Now.hasReadingResult)
	            rewardPanel.Show();
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
			yield return picker.Show(1, true, new List<int> {selectedChar}, LoadSelectedCharacter);
		}

		public void LoadSelectedCharacter(List<int> index)
		{
			selectedChar = index[0];
			selectedCharImg.sprite = Resources.Load<Sprite>("Characters/" + Variables.Characters[index[0]].InternalName + "/image_album");
			Variables.QuizSelectedCharacter = selectedChar;
			//selectedCharName.text = Variables.Characters[index].Name;
		}

		public void StartReading()
		{
			Dialogue.DialogueManager.DialogRoot = "Characters/" + Variables.Characters[selectedChar].InternalName + "/";
			SceneChanger.Instance.ChangeScene("ReadingIngame");
		}
	}
}