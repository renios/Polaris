using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Reading
{
    public class ReadingDiaryDetailAnswer : MonoBehaviour
    {
        public Text answerText;
        public GameObject newAnswerTag;

        public void Set(int charIdx, int quizIdx)
        {
            if (Variables.Characters[charIdx].QuizUnlockTable.ContainsKey(quizIdx) &&
                Variables.Characters[charIdx].QuizUnlockTable[quizIdx] == true)
            {
                var dialog = Dialogue.DialogueParser.ParseFromCSV("Quizs/" + Variables.Characters[charIdx].InternalName + "_" + quizIdx);
                foreach (var content in dialog.Dialogues[0].Contents)
                {
                    if (content.Type == 20) // 해설
                    {
                        answerText.text = content.DialogText;
                        answerText.color = Color.black;
                        break;
                    }
                }
            }
            else
            {
                answerText.text = "???";
                answerText.color = Color.gray;
            }

            if (Variables.Characters[charIdx].NewQuizAnsIndex.Contains(quizIdx))
                newAnswerTag.SetActive(true);
        }
    }
}