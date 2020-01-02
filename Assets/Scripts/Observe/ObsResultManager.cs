using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace Observe
{
	public class ObsResultManager : MonoBehaviour
	{
		public Image charImage;
		public Image favPanelCharThumb;
		public GameObject obsEffect;

		[Header("Character Popup Animation")]
		public Graphic fadePanel;
		public CanvasGroup nameTag;
		public Text nameText;
		public Text oneWord;
		public CanvasGroup favPanel;
		public Text favText;
		public GameObject touchToGoText;

		[Header("Favority Increase Animation")]
		public Text favCharText;
		public Text favConstelText;
		public GameObject favIncreasePanel;
		public Slider favSlider;
		public Text favSliderText;
		public Text favLevelText;
		public GameObject favIncreasedLabel;

		[Header("View Dialogue Popup")]
		public GameObject dialogPanel;
		public Button payButton;

		[Header("Total Result Display")]
		public ObsPickResultShower resultDisplay;

		bool touchExists;
		bool responceExists, dialogResponce;
		ObserveStatus status;
		List<bool> isFirst = new List<bool>();

		IEnumerator Start()
		{
			status = ObserveStatus.Load();

			// 가장 관측 횟수가 높은 캐릭터부터 차례대로...
			var orderedRes = status.pickResult.OrderByDescending(p => p.Value);
			for (int i = 0; i < orderedRes.Count(); i++)
			{
				// 캐릭터 정보를 가져옵니다.
				var charKey = orderedRes.ElementAt(i).Key;
				if (Variables.Characters[charKey].Cards[0].Observed)
					isFirst.Add(false);
				else
					isFirst.Add(true);

				// 각종 텍스트와 이미지를 적용시킵니다.
				charImage.sprite = Resources.Load<Sprite>("Characters/" + Variables.Characters[charKey].InternalName + "/default/image_full");
				nameText.text = Variables.Characters[charKey].Name;
				favText.text = "+" + status.charFavData[charKey].ToString();
				favPanelCharThumb.sprite = Resources.Load<Sprite>("Characters/" + Variables.Characters[charKey].InternalName + "/default/image_obspopup");
				favCharText.text = Variables.Characters[charKey].Name;
				favConstelText.text = Variables.Constels[Variables.Characters[charKey].ConstelKey[0]].Name;

				// 캐릭터 팝업 연출을 실행합니다.
				obsEffect.SetActive(true);
				yield return CharPopupAnim(isFirst[i]);
				obsEffect.SetActive(false);
				yield return new WaitUntil(() => touchExists);
				touchExists = false;

				// 만약 첫 획득이라면, 첫 번째 스토리를 보고 다음 루프로 넘어갑니다.
				// 그 중에서도 만약 튜토리얼 관측이었다면, 씬을 변경하고(Append가 아님) Start 코루틴을 완전히 빠져나옵니다.
				if (Variables.Characters[charKey].Cards[0].Observed == false || Variables.Characters[charKey].Cards[0].Favority == 0)
				{	
					if (charKey == 1)
					{
						status.userChecked = true;
						status.Save();
						Variables.Characters[charKey].Cards[0].Observed = true;
						Variables.Characters[charKey].Cards[0].Favority++;
						GameManager.Instance.SaveGame();

						Variables.DialogCharIndex = charKey;
						Variables.DialogCardIndex = 0;
						Variables.DialogChapterIndex = 0;
						Variables.DialogAfterScene = "MainTut";
					    ChangeScene("NewDialogScene");
						yield break;
					}
					else
					{
						yield return StartStory(charKey, 0);
						if (i < orderedRes.Count() - 1)
							SoundManager.FadeMusicVolume(0, 0.01f);
						continue;
					}
				}
				
				// 친밀도 증가 연출을 실행합니다.
				favIncreasePanel.SetActive(true);
				yield return FavIncreaseAnim(charKey, status.charFavData[charKey]);
				favIncreasePanel.SetActive(false);
				yield return new WaitUntil(() => touchExists);
				touchExists = false;

				// 이전과 이후 친밀도 레벨을 확인한 뒤, 만약 친밀도 레벨이 올라갔다면 스토리 실행 여부를 묻는 팝업을 띄워 줍니다.
				// 그렇지 않았다면, 페이드아웃 연출 실행 후 다음 루프로 넘어갑니다.
				float p1;
				int p2, p3;
				var prevFavLev = GameManager.Instance.CheckFavority(charKey, 0, out p2, out p3);
				var nextFavLev = GameManager.Instance.CheckAfterFavority(charKey, 0, status.charFavData[charKey], out p1, out p2);
				if (nextFavLev > prevFavLev)
				{
					favIncreasePanel.SetActive(false);
					yield return ViewDialogAlert(charKey, prevFavLev);
				}
				else
					yield return FadeoutWithoutDialog();

				if(i < orderedRes.Count() - 1)
					SoundManager.FadeMusicVolume(0, 0.01f);
			}

			// 모든 루프를 다 돈 뒤(모든 캐릭터의 연출을 본 뒤), 종합 정보를 띄워주고 페이드 인 합니다. 유저가 이를 체크했음을 확인합니다.
			resultDisplay.Show(status.charFavData);
			status.userChecked = true;
			status.Save();
			float t = 0;
			while (t < 0.2f) // 0.2초 동안
			{
				fadePanel.color = new Color(fadePanel.color.r, fadePanel.color.g, fadePanel.color.b, 1 - t * 5);
				yield return null;
				t += Time.deltaTime;
			}
			fadePanel.color = new Color(fadePanel.color.r, fadePanel.color.g, fadePanel.color.b, 0);
			
			// 튜토리얼 데이터를 처리합니다.
			if (Variables.tutState == 9)
			{
				Variables.isTutorialFinished = true;
				Variables.tutState = 10;
			}
		}

		void Update()
		{
			if (Input.GetMouseButtonDown(0))
				touchExists = true;
		}

		IEnumerator CharPopupAnim(bool first)
		{
			touchExists = false;
			fadePanel.color = new Color(fadePanel.color.r, fadePanel.color.g, fadePanel.color.b, 1);
			nameTag.alpha = 0;
			//oneWord.color = new Color(oneWord.color.r, oneWord.color.g, oneWord.color.b, 0);
			favPanel.alpha = 0;
			touchToGoText.SetActive(false);

			SoundManager.Play(SoundType.GachaResult);

			float t = 0;
			while (t < 5) // 5초 동안
			{
				if (!first && touchExists)
					break;

				fadePanel.color = new Color(fadePanel.color.r, fadePanel.color.g, fadePanel.color.b, 1 - t / 5);
				yield return null;
				t += Time.deltaTime;
			}
			fadePanel.color = new Color(fadePanel.color.r, fadePanel.color.g, fadePanel.color.b, 0);
			SoundManager.FadeMusicVolume(1, 1.5f);

			t = 0;
			while(t < 1) // 1초 동안
			{
				if (!first && touchExists)
					break;

				nameTag.alpha = t;
				if (!first)
					favPanel.alpha = t;
				//else
				//	oneWord.color = new Color(oneWord.color.r, oneWord.color.g, oneWord.color.b, t);
					
				yield return null;
				t += Time.deltaTime;
			}
			nameTag.alpha = 1;
			if (!first)
				favPanel.alpha = 1;
			//else
			//	oneWord.color = new Color(oneWord.color.r, oneWord.color.g, oneWord.color.b, 1);

			touchExists = false;
			touchToGoText.SetActive(true);
		}

		IEnumerator FavIncreaseAnim(int charIndex, int deltaFav)
		{
			touchExists = false;

			int curFavLevel, curRequiredFav;
			float curFavValue;
			curFavLevel = GameManager.Instance.CheckAfterFavority(charIndex, 0, 0, out curFavValue, out curRequiredFav);
			
			float t = 0;
			while(t <= deltaFav) // '단계당 1초'
			{
				if (touchExists)
					break;

				var localFavLevel = GameManager.Instance.CheckAfterFavority(charIndex, 0, t, out curFavValue, out curRequiredFav);
				if (localFavLevel > curFavLevel)
					favIncreasedLabel.SetActive(true);
				favLevelText.text = localFavLevel.ToString();
				favSliderText.text = curRequiredFav < 0 ? "FULL" : curFavValue.ToString("N0") + "/" + curRequiredFav.ToString();
				favSlider.maxValue = curRequiredFav < 0 ? 1 : curRequiredFav;
				favSlider.value = curRequiredFav < 0 ? 1 : curFavValue;
				yield return null;
				t += curRequiredFav * Time.deltaTime;
			}
			var finalFavLevel = GameManager.Instance.CheckAfterFavority(charIndex, 0, deltaFav, out curFavValue, out curRequiredFav);
			if (finalFavLevel > curFavLevel)
				favIncreasedLabel.SetActive(true);
			favLevelText.text = finalFavLevel.ToString();
			favSliderText.text = curRequiredFav < 0 ? "FULL" : curFavValue.ToString("N0") + "/" + curRequiredFav.ToString();
			favSlider.maxValue = curRequiredFav < 0 ? 1 : curRequiredFav;
			favSlider.value = curRequiredFav < 0 ? 1 : curFavValue;

			touchExists = false;
		}

		IEnumerator ViewDialogAlert(int charIndex, int chapterIndex)
		{
			dialogPanel.SetActive(true);
			yield return new WaitUntil(() => responceExists);
			responceExists = false;

			if (dialogResponce == true)
				yield return StartStory(charIndex, chapterIndex);
			else
				yield return FadeoutWithoutDialog();
		}

		IEnumerator StartStory(int charIndex, int chapterIndex)
		{
			Variables.DialogCharIndex = charIndex;
			Variables.DialogCardIndex = 0;
			Variables.DialogChapterIndex = chapterIndex;
			yield return AppendDialogScene();
			yield return new WaitWhile(() => Variables.IsDialogAppended);
		}

		IEnumerator FadeoutWithoutDialog()
		{
			float t = 0;
			while (t <= 0.25f) // 0.25초 동안
			{
				fadePanel.color = new Color(fadePanel.color.r, fadePanel.color.g, fadePanel.color.b, t * 4);
				yield return null;
				t += Time.deltaTime;
			}
			fadePanel.color = new Color(fadePanel.color.r, fadePanel.color.g, fadePanel.color.b, 1);
		}

		IEnumerator LoadNextScene(string sceneName)
		{
			yield return FadeoutWithoutDialog();

			AsyncOperation loading = SceneManager.LoadSceneAsync(sceneName);
			loading.allowSceneActivation = false;
			while (!loading.isDone)
			{
				if (loading.progress >= 0.9f)
					loading.allowSceneActivation = true;
				yield return null;
			}
		}

		IEnumerator AppendDialogScene()
		{
			Variables.IsDialogAppended = true;

			yield return FadeoutWithoutDialog();

			AsyncOperation loading = SceneManager.LoadSceneAsync("AppendDialogScene", LoadSceneMode.Additive);
			loading.allowSceneActivation = false;
			while (!loading.isDone)
			{
				if (loading.progress >= 0.9f)
					loading.allowSceneActivation = true;
				yield return null;
			}
		}

		public void DialogButtonClick(bool value)
		{
			responceExists = true;
			dialogResponce = value;
		}

		public void ChangeScene(string sceneName)
		{
			StartCoroutine(LoadNextScene(sceneName));
		}
	}
}