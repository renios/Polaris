using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class CutscenePlayer : MonoBehaviour
{
	public Image targetImage;
	public float fadeTime;
	
	[Header("Sprites and Timestamps")]
	public Sprite[] sprites;
	public float[] times;

	bool skipped = false;
	
	IEnumerator Start()
	{
		yield return new WaitForSeconds(times[0]);
		
		for (int i = 0; i < sprites.Length; i++)
		{
			targetImage.sprite = sprites[i];
			targetImage.GetComponent<CanvasGroup>().DOFade(1, fadeTime);
			yield return new WaitForSeconds(fadeTime);

			yield return new WaitForSeconds(times[i + 1] - times[i] - 2 * fadeTime);

			targetImage.GetComponent<CanvasGroup>().DOFade(0, fadeTime);
			yield return new WaitForSeconds(fadeTime);
		}
		
		if(!skipped)
			SceneChanger.ChangeScene(Variables.CutsceneAfterScene, hideBar: true);
	}

	public void Skip()
	{
		skipped = true;
		SceneChanger.ChangeScene(Variables.CutsceneAfterScene, hideBar: true);
	}
}