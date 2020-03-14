using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Reading
{
	public class ReadingCharListToggle : MonoBehaviour
	{
		public GameObject truePanel, falsePanel;
		public Image image;
		public Text text;
		public ReadingCharPicker picker;

		int index;

		public void Set(KeyValuePair<int, CharacterData> dataSet)
		{
			index = dataSet.Key;
			if(dataSet.Value.Observed)
			{
				truePanel.SetActive(true);
				falsePanel.SetActive(false);
				image.sprite = Resources.Load<Sprite>("Characters/" + dataSet.Value.InternalName + "/image_albumbutton");
				text.text = dataSet.Value.Name;
			}
			else
			{
				truePanel.SetActive(false);
				falsePanel.SetActive(true);
				text.text = "???";
			}
		}

		public void OnSelected()
		{
			picker.resultIndex = index;
		}
	}
}