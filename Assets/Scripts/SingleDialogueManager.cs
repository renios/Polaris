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
			AddItem();
		}

		public void AddItem() {
			Balloon item = Instantiate(items[0], transform);
			item.Init();
			item.SetItemData(null, itemIndex);
			itemIndex++;
		}

		void Update() {
			if (Input.GetKeyDown(KeyCode.Space))
			{
				AddItem();
			}
		}
	}
}