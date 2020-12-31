using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Dialogue;

namespace Reading
{
	public class ReadingIngameManager : MonoBehaviour
	{
		public GameObject rightEffect, wrongEffect;
		public ReadingQuizDisplayer quizDisplayer;
		public ReadingResultDisplayer resultDisplayer;

		IEnumerator Start()
		{
			var questionSolution = new List<string>();

			// 퀴즈 파일은 모두 TEXT 파일이라고 가정
			DialogueManager.Instance.fileType = DialogueFileType.TEXT;
			DialogueManager.Instance.AssignCustomType(20, dialog =>
			{
				questionSolution.Add(dialog.DialogText);
			});

			// ROOT : 독서로비에서 고른 캐릭터 폴더
			DialogueManager.Instance.Displayer.ForeImage.sprite = Resources.Load<Sprite>(DialogueManager.DialogRoot + "image_dialogue");
			var initDialog = DialogueParser.ParseFromCSV(DialogueManager.DialogRoot + "quiz_init");
			yield return DialogueManager.Instance.Play(initDialog, r => { });

			// Quiz 폴더로부터, 관측된 캐릭터에 관한 문제를 추가
			var quizList = new List<DialogueData>();
			var quizCharIdxDic = new List<KeyValuePair<int, int>>();
			foreach(var chr in Variables.Characters)
			{
				if(chr.Value.Observed)
				{
					for(int i = 0; ; i++)
					{
						var filePath = "Quizs/" + chr.Value.InternalName + "_" + i.ToString();
						// 파일이 존재하는가?
						var fileData = Resources.Load<TextAsset>(filePath);
						if (fileData == null)
							break;

						// 여기까지 왔으면 존재한다는 소리
						var quizData = DialogueParser.ParseFromCSV(filePath);
						quizList.Add(quizData);
						quizCharIdxDic.Add(new KeyValuePair<int, int>(chr.Key, i));
					}
				}
			}
			Debug.Log(quizList.Count + " available quizs found.");

			// 5개 체리피킹
			var pickedQuizs = new List<DialogueData>();
			var pickedCharIdxDic = new List<KeyValuePair<int, int>>();
			var pickCount = quizList.Count < 5 ? quizList.Count : 5;
			for(int i = 0; i < pickCount; i++)
			{
				int r = Random.Range(0, quizList.Count);
				pickedQuizs.Add(quizList[r]);
				pickedCharIdxDic.Add(quizCharIdxDic[r]);
				quizList.RemoveAt(r);
				quizCharIdxDic.RemoveAt(r);
			}

			int rightCount = 0, wrongCount = 0;
			var quizResult = new List<bool>();
			
			// 순차대로 재생
			for(int i = 0; i < pickedQuizs.Count; i++)
			{
				int selectedAns = 0;
				bool isRightAns = false;

				var quiz = pickedQuizs[i];
				var charIdxDic = pickedCharIdxDic[i];
				yield return DialogueManager.Instance.Play(quiz, r =>
				{
					selectedAns = r;
					if (r == 1)
					{
						// 정답
						isRightAns = true;
						rightCount++;

						if (!Variables.Characters[charIdxDic.Key].QuizUnlockTable.ContainsKey(charIdxDic.Value))
						{
							Variables.Characters[charIdxDic.Key].QuizUnlockTable.Add(charIdxDic.Value, true);
							Variables.Characters[charIdxDic.Key].HasNewQuizAns = true;
							Variables.Characters[charIdxDic.Key].NewQuizAnsIndex.Add(charIdxDic.Value);
						}
						else if (Variables.Characters[charIdxDic.Key].QuizUnlockTable[charIdxDic.Value] == false)
						{
							Variables.Characters[charIdxDic.Key].QuizUnlockTable[charIdxDic.Value] = true;
							Variables.Characters[charIdxDic.Key].HasNewQuizAns = true;
							Variables.Characters[charIdxDic.Key].NewQuizAnsIndex.Add(charIdxDic.Value);
						}
					}
					else
					{
						// 오답
						isRightAns = false;
						wrongCount++;
						
						if(!Variables.Characters[charIdxDic.Key].QuizUnlockTable.ContainsKey(charIdxDic.Value))
							Variables.Characters[charIdxDic.Key].QuizUnlockTable.Add(charIdxDic.Value, false);
					}
					quizResult.Add(isRightAns);
				});

				if (isRightAns)
				{
					// 정답 효과 재생
					rightEffect.SetActive(true);
					yield return new WaitForSeconds(1.5f);
					rightEffect.SetActive(false);
				}
				else
				{
					// 오답 효과 재생
					wrongEffect.SetActive(true);
					yield return new WaitForSeconds(1.5f);
					wrongEffect.SetActive(false);
					
					// 문제와 그 정답 표시 (오답 노트의 개념으로)
					string questionContext = "", answerContext = "";
					for(int j = 0; j < quiz.Dialogues[0].Contents.Length; j++)
					{
						if (quiz.Dialogues[0].Contents[j].Type == 0) // 문제 내용
							questionContext += (j == 1 ? "" : "\n") + quiz.Dialogues[0].Contents[j].DialogText;
						else if(quiz.Dialogues[0].Contents[j].Type == 2) // 선택지
						{
							for (int k = 0; k < quiz.Dialogues[0].Contents[j].Directions.Length; k++)
							{
								if (quiz.Dialogues[0].Contents[j].Directions[k] == 1)
								{
									answerContext = quiz.Dialogues[0].Contents[j].JuncTexts[k];
									break;
								}
							}
						}
					}
					yield return quizDisplayer.Show(questionContext, answerContext);
				}
			}

			int rank;
			if (rightCount == 5)
				rank = 4;
			else if (rightCount == 3 || rightCount == 4)
				rank = 3;
			else if (rightCount == 1 || rightCount == 2)
				rank = 2;
			else
				rank = 1;
			
			SaveData.Now.lastReadingChar = Variables.QuizSelectedCharacter;
			SaveData.Now.lastReadingRank = rightCount;
			SaveData.Now.hasReadingResult = true;
			GameManager.Instance.SaveGame();
			
			// 결과 텍스트 출력. 대화 파일 해체분석 뒤 결과에 맞게 재창조.
			var finRawDialog = DialogueParser.ParseFromCSV(DialogueManager.DialogRoot + "quiz_fin");
			var finRawPhaseDic = new Dictionary<int, DialogueContent>();
			foreach (var phase in finRawDialog.Dialogues)
				finRawPhaseDic.Add(phase.Phase, phase.Contents[0]);

			var finDialog = new DialogueData();
			finDialog.Dialogues = new[] { new DialoguePhase()
			{
				Contents = new[] { finRawPhaseDic[0], finRawPhaseDic[rank] }
			} };
			var finText = finDialog.Dialogues[0].Contents[0].DialogText;
			finDialog.Dialogues[0].Contents[0].DialogText = finText.Replace("#", (rightCount + wrongCount).ToString()).Replace("$", rightCount.ToString());
			yield return DialogueManager.Instance.Play(finDialog, r => { });
			
			resultDisplayer.Show(quizResult, questionSolution);
		}

		public void Exit()
		{
			if (!Variables.TutorialFinished && Variables.TutorialStep == 9)
			{
				Variables.TutorialStep += 2;
				Variables.CutsceneAfterScene = "NewDialogScene";
				DialogueManager.DialogRoot = "Dialogues/TutorialReading/";
				DialogueManager.DialogFilePath = "Dialogues/TutorialReading/dialog2";
				Variables.DialogAfterScene = "ReadingLobby";

				SceneChanger.ChangeScene("Cutscene2", hideBar: true);
			}
			else
			{
				SceneChanger.Instance.ChangeScene("ReadingLobby");
			}
		}
	}
}