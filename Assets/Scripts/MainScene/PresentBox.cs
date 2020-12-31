using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;

public class PresentBox : MonoBehaviour, IPointerDownHandler
{
	public GameObject balloon;
	//public GameObject starGetEffect;

    public void OnPointerDown(PointerEventData eventData)
    {
        
    }

    private void OnMouseDown()
    {
		if (balloon.activeSelf)
		{
			SaveData.Now.allowPresent = false;
			GameManager.Instance.IncreaseMoney(MoneyType.Starlight, Random.Range(500, 1001));
			GameManager.Instance.SaveGame();

			balloon.SetActive(false);
			//starGetEffect.SetActive(true);
		}
	}

    // Use this for initialization
    void Start () 
	{
		if(SaveData.Now.allowPresent && Variables.Starlight < 500)
        {
			balloon.SetActive(true);
			StartCoroutine(ActiveAnimation());
        }
	}

	IEnumerator ActiveAnimation()
    {
		while(balloon.activeSelf)
        {
			transform.DOScale(new Vector3(1.05f, 1.05f, 1), 0.25f);
			yield return new WaitForSeconds(0.25f);
			transform.DORotate(new Vector3(0, 0, -10), 0.075f);
			yield return new WaitForSeconds(0.075f);
			transform.DORotate(new Vector3(0, 0, 10), 0.15f);
			yield return new WaitForSeconds(0.15f);
			transform.DORotate(new Vector3(0, 0, -10), 0.15f);
			yield return new WaitForSeconds(0.15f);
			transform.DORotate(new Vector3(0, 0, 0), 0.075f);
			yield return new WaitForSeconds(0.075f);
			transform.DOScale(Vector3.one, 0.25f);
			yield return new WaitForSeconds(2.3f);
		}
    }
}
