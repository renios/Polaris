using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Reading
{
    public class ReadingDiaryDetailPopup : MonoBehaviour
    {
        public GameObject body;
        public Image thumbnail;
        public Text constelName, characterName;
        public Slider progressSlider;
        public Text progressText;
        public GameObject answerTemplate;
        public RectTransform answerBody, scrollBody;

        List<GameObject> answers;

        public void Show(int charIdx)
        {
            if(answers == null)
                answers = new List<GameObject>();
            foreach(var obj in answers)
                Destroy(obj);
            answers.Clear();
            
            var character = Variables.Characters[charIdx];

            thumbnail.sprite = Resources.Load<Sprite>("Characters/" + character.InternalName + "/image_albumbutton");
            constelName.text = character.ConstelWeight[0] >= 1 ? Variables.Constels[character.ConstelKey[0]].Name : "알 수 없음";
            characterName.text = character.Name;
            
            // 전체 퀴즈 개수 체크
            int quizCount = 0;
            for (;; quizCount++)
            {
                var dummy = Resources.Load<TextAsset>("Quizs/" + character.InternalName + "_" + quizCount);
                if (dummy == null)
                    break;
            }
            
            // 진행도 체크
            int unlockCount = 0;
            foreach (var quizState in character.QuizUnlockTable)
                if (quizState.Value == true)
                    unlockCount++;

            progressSlider.maxValue = quizCount;
            progressSlider.value = unlockCount;
            progressText.text = unlockCount + "/" + quizCount;
            
            // 정답들 생성
            for (int i = 0; i < quizCount; i++)
            {
                var newObj = Instantiate(answerTemplate);
                var newAnswer = newObj.GetComponent<ReadingDiaryDetailAnswer>();
                newAnswer.Set(charIdx, i);
                newObj.transform.SetParent(answerBody);
                newObj.transform.localScale = Vector3.one;
                newObj.transform.localPosition = Vector3.zero;
                newObj.SetActive(true);
                answers.Add(newObj);
            }
            
            body.SetActive(true);
            StartCoroutine(RebuildScrollBody());

            if (character.HasNewQuizAns)
            {
                character.NewQuizAnsIndex.Clear();
                character.HasNewQuizAns = false;
                GameManager.Instance.SaveGame();
            }
        }

        IEnumerator RebuildScrollBody()
        {
            for (int i = 0; i < 3; i++)
            {
                LayoutRebuilder.MarkLayoutForRebuild(scrollBody);
                LayoutRebuilder.ForceRebuildLayoutImmediate(scrollBody);
                yield return null;
            }
        }

        public void Hide()
        {
            body.SetActive(false);
        }
    }
}