using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Observe
{
	public class ObsPickResultElement : MonoBehaviour
	{
		public Text constelName, charName;
		public Image charThumb;
		public Slider favBar;
		public Text favText, favLevel;
		public GameObject favIncreased;
		public Text favIncrement;

		public void SetAndApplyData(int charKey, int deltaFav)
		{
			var charData = Variables.Characters[charKey];

			charData.Cards[0].Favority += deltaFav;
			if (!charData.Cards[0].Observed)
				charData.Cards[0].Observed = true;

			charName.text = charData.Name;
			for(int i = 0; i < charData.ConstelKey.Length; i++)
			{
				if (i == 0)
					constelName.text = Variables.Constels[charData.ConstelKey[i]].Name;
				else
					constelName.text += ", " + Variables.Constels[charData.ConstelKey[i]].Name;
			}
			charThumb.sprite = Resources.Load<Sprite>("Characters/" + charData.InternalName + "/default/image_obspopup");
			
			int progress, required;
			int level = GameManager.Instance.CheckFavority(charKey, 0, out progress, out required);
			favLevel.text = level.ToString();
			favText.text = required < 0 ? "FULL" : progress.ToString() + "/" + required.ToString();
			favBar.maxValue = required < 0 ? 1 : required;
			favBar.value = required < 0 ? 1 : progress;
			favIncrement.text = "(+" +  deltaFav.ToString() + ")";
			if (progress - deltaFav < 0)
				favIncreased.SetActive(true);
		}
	}
}