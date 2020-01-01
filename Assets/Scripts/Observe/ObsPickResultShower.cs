using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Observe
{
	public class ObsPickResultShower : MonoBehaviour
	{
		public RectTransform elementParent;
		public GameObject template;
		public RectTransform totalParent;

		public void Show(Dictionary<int, int> result)
		{
			foreach(var data in result)
			{
				var newObj = Instantiate(template);
				newObj.transform.SetParent(elementParent);
				newObj.transform.localScale = Vector3.one;
				var newElement = newObj.GetComponent<ObsPickResultElement>();
				newElement.SetAndApplyData(data.Key, data.Value);
				newObj.SetActive(true);
			}
			gameObject.SetActive(true);
			GameManager.Instance.SaveGame();

			LayoutRebuilder.MarkLayoutForRebuild(totalParent);
			LayoutRebuilder.ForceRebuildLayoutImmediate(totalParent);
		}
	}
}