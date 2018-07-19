using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace SuperScrollView
{
	public class SingleDialogueManager : MonoBehaviour {

		public List<Balloon> items;
		int itemIndex = 0;

		// Use this for initialization
		void Start () {
			StartCoroutine(AddItem());
		}

		public IEnumerator AddItem() {
			GetComponent<RectTransform>().anchoredPosition = new Vector2(GetComponent<RectTransform>().anchoredPosition.x, 0);

			Balloon item = Instantiate(items[0], transform);
			item.Init();
			item.SetItemData(null, itemIndex);
			itemIndex++;
			yield return new WaitForSeconds(item.GetComponent<DoTweenHelper>().duration);
		}

		void Update() {
			if (Input.GetKeyDown(KeyCode.Space))
			{
				StartCoroutine(AddItem());
			}
		}
	}
}