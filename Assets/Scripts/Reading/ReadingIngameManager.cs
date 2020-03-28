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

			var quizList = new List<DialogueData>();

			for(int i = 0; ; i++)
			{
				var filePath = DialogueManager.DialogRoot + "Quizs/quiz_" + i.ToString();
				// 파일이 존재하는가?
				var fileData = Resources.Load<TextAsset>(filePath);
				if (fileData == null)
					break;

				// 여기까지 왔으면 존재한다는 소리
				var quizData = DialogueParser.ParseFromCSV(filePath);
				quizList.Add(quizData);
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

			// 다 끝나면 결과화면 보여주기
			resultDisplayer.Show(quizResult, questionSolution);
		}
	}
}