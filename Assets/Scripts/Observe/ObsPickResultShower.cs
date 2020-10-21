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
				newObj.transform.localPosition = Vector3.zero;
				var newElement = newObj.GetComponent<ObsPickResultElement>();
				newElement.SetAndApplyData(data.Key, data.Value);
				newObj.SetActive(true);

				SaveData.Now.lastObservedChar = data.Key;
			}
			gameObject.SetActive(true);
			GameManager.Instance.SaveGame();

			LayoutRebuilder.MarkLayoutForRebuild(totalParent);
			LayoutRebuilder.ForceRebuildLayoutImmediate(totalParent);
		}

		public void Close()
        {
			bool allObserved = true;
			if(!SaveData.Now.endingVisited)
            {
				foreach (var chr in Variables.Characters.Values)
				{
					allObserved = chr.Observed;
					if (!allObserved)
						break;
				}
			}

			if (allObserved && !SaveData.Now.endingVisited)
				SceneChanger.ChangeScene("EndgameScene", "GachaFadeIn");
			else
				gameObject.SetActive(false);
        }
	}
}