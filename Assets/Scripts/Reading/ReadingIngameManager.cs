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
			var initDialog = DialogueParser.ParseFromCSV(DialogueManager.DialogRoot + "quiz_init");
			int levelRes = 0;
			yield return DialogueManager.Instance.Play(initDialog, r => { levelRes = r; });

			// Quiz 폴더로부터, 관측된 캐릭터에 관한 문제를 추가
			// 1: 쉬움, 2: 보통, 3: 어려움
			var quizList = new List<DialogueData>();
			foreach(var chr in Variables.Characters)
			{
				if(chr.Value.Observed)
				{
					for(int i = 0; ; i++)
					{
						var filePath = "Quizs/" + chr.Value.InternalName + "_" + levelRes.ToString() + "_" + i.ToString();
						// 파일이 존재하는가?
						var fileData = Resources.Load<TextAsset>(filePath);
						if (fileData == null)
							break;

						// 여기까지 왔으면 존재한다는 소리
						var quizData = DialogueParser.ParseFromCSV(filePath);
						quizList.Add(quizData);
					}
				}
			}

			// 5개 체리피킹
			var pickedQuizs = new List<DialogueData>();
			var pickCount = quizList.Count < 5 ? quizList.Count : 5;
			for(int i = 0; i < pickCount; i++)
			{
				int r = Random.Range(0, quizList.Count);
				pickedQuizs.Add(quizList[r]);
				quizList.RemoveAt(r);
			}

			int rightCount = 0, wrongCount = 0;
			var quizResult = new List<bool>();
			
			// 순차대로 재생
			foreach(var quiz in pickedQuizs)
			{
				int selectedAns = 0;
				bool isRightAns = false;
				yield return DialogueManager.Instance.Play(quiz, r =>
				{
					selectedAns = r;
					if (r == 1)
					{
						// 정답
						isRightAns = true;
						rightCount++;
					}
					else
					{
						// 오답
						isRightAns = false;
						wrongCount++;
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
				}

				// 문제 선택지 리스트 구축 및 표시
				string questionContext = "";
				var answerContent = new DialogueContent();
				for(int i = 0; i < quiz.Dialogues[0].Contents.Length; i++)
				{
					if (quiz.Dialogues[0].Contents[i].Type == 0) // 문제 내용
						questionContext += (i == 1 ? "" : "\n") + quiz.Dialogues[0].Contents[i].DialogText;
					else if(quiz.Dialogues[0].Contents[i].Type == 2) // 선택지
					{
						answerContent = quiz.Dialogues[0].Contents[i];
						break;
					}
				}
				yield return quizDisplayer.Show(questionContext, answerContent, selectedAns);
			}

			// 다 끝나면 결과 텍스트 출력 후 결과화면 보여주기
			//var finDialog = DialogueParser.ParseFromCSV(DialogueManager.DialogRoot + "quiz_fin");
			//var finText = finDialog.Dialogues[0].Contents[0].DialogText;
			//finDialog.Dialogues[0].Contents[0].DialogText = finText.Replace("#", (rightCount + wrongCount).ToString()).Replace("%", rightCount.ToString());
			//yield return DialogueManager.Instance.Play(finDialog, r => { });

			resultDisplayer.Show(quizResult, questionSolution);
		}
	}
}