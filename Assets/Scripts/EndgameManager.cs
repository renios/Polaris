using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using LitJson;
using Dialogue;

public class EndgameManager : MonoBehaviour
{
	public SpriteRenderer[] sky;
	public GameObject balloon1, balloon2;
	public RectTransform observePanel;

	public GameObject scope;
	public GameObject scopeCompleteEffect;
	public CanvasGroup polarisSprite1, polarisSprite2;

	public GameObject dialogCanvas;

	public CanvasGroup frontDimmer, theEndText;
	public GameObject finalPopup;

	bool hasInput;

	IEnumerator Start() 
	{
		SaveData.Now.endingVisited = true;
		// 폴라리스의 친밀도를 1 올린다.
		int prog, req;
		GameManager.Instance.CheckFavority(1, out prog, out req);
		GameManager.Instance.IncreaseFavority(1, req - prog);
		Variables.Characters[1].StoryUnlocked++;
		GameManager.Instance.SaveGame();

		SoundManager.FadeMusicVolume(0, 0);
		yield return new WaitForSeconds(1);
		for(int i = 0; i < 5; i++)
        {
			if (i == 1)
				balloon1.SetActive(true);
			sky[i].DOFade(0, 1);
			yield return new WaitForSeconds(1);
        }
		yield return new WaitForSeconds(1);
		balloon2.SetActive(true);
		scope.SetActive(true);
		observePanel.DOAnchorPosY(0, 0.5f);
		yield return new WaitForSeconds(0.5f);

		yield return new WaitUntil(() => hasInput);
		hasInput = false;

		SoundManager.Play(SoundType.ClickImportant);
		scope.SetActive(false);
		balloon2.SetActive(false);
		observePanel.gameObject.SetActive(false);
		scopeCompleteEffect.SetActive(true);
		yield return new WaitForSeconds(2.25f);
		scopeCompleteEffect.SetActive(false);
		SoundManager.Play(SoundType.GachaResult);
		polarisSprite1.gameObject.SetActive(true);
		polarisSprite1.DOFade(1, 4f).SetEase(Ease.Linear);
		yield return new WaitForSeconds(4f);
		SoundManager.Play(SoundType.Stardust);
		polarisSprite2.gameObject.SetActive(true);
		polarisSprite2.DOFade(1, 2f);
		yield return new WaitForSeconds(3);

		// Start dialog
		SoundManager.FadeMusicVolume(1, 0);
		SoundManager.Play(SoundType.BgmDark);
        DialogueManager.PrepareCharacterDialog(1, 5);
        Variables.DialogAfterScene = "MainScene";

		dialogCanvas.SetActive(true);
		yield return PlayDialogue();

		frontDimmer.gameObject.SetActive(true);
		frontDimmer.DOFade(1, 2.5f);
		yield return new WaitForSeconds(3f);
		theEndText.gameObject.SetActive(true);
		theEndText.DOFade(1, 2.5f);
		yield return new WaitForSeconds(4f);
		theEndText.DOFade(0, 2.5f);
		yield return new WaitForSeconds(3f);

		finalPopup.SetActive(true);
    }

	IEnumerator PlayDialogue()
    {
		DialogueData dialog;

		try
		{
			var jsonAsset = Resources.Load<TextAsset>(DialogueManager.DialogFilePath);
			if (jsonAsset == null)
				jsonAsset = Resources.Load<TextAsset>("Dialogues/ErrorDialog");

			dialog = JsonMapper.ToObject<DialogueData>(jsonAsset.text);
			DialogueManager.Instance.fileType = DialogueFileType.JSON;
		}
		catch
		{
			dialog = DialogueParser.ParseFromCSV(DialogueManager.DialogFilePath);
			DialogueManager.Instance.fileType = DialogueFileType.TEXT;
		}

		DialogueManager.Instance.Displayer.Talker.text = DialogueManager.DefaultTalkerName;

		yield return DialogueManager.Instance.Play(dialog, (r) => { });
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void Click()
    {
		hasInput = true;
    }

	public void MoveScene()
    {
		SceneChanger.Instance.ChangeScene("TitleScene");
    }
}
