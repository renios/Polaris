using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Dialogue;

namespace Tutorial
{
	public class TutorialDialogue : MonoBehaviour
	{
		
		// Use this for initialization
		IEnumerator Start()
		{
			GetComponent<Image>().alphaHitTestMinimumThreshold = 0.4f;
			yield return new WaitUntil(() => DialogueManager.Instance.Interactor.HasResult);
			gameObject.SetActive(false);
		}
	}
}